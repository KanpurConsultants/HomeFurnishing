using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Core.Common;
using Model.Models;
using Data.Models;
using Service;
using Jobs.Helpers;
using Data.Infrastructure;
using Presentation.ViewModels;
using AutoMapper;
using Microsoft.AspNet.Identity;
using System.Configuration;
using Presentation;
using Model.ViewModel;
using System.Data.SqlClient;
using System.Xml.Linq;
using CustomEventArgs;
using DocumentEvents;
using SalaryDocumentEvents;
using Reports.Reports;
using Reports.Controllers;
using Model.ViewModels;




namespace Jobs.Controllers
{

    [Authorize]
    public class SalaryHeaderController : System.Web.Mvc.Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private bool EventException = false;

        List<string> UserRoles = new List<string>();
        ActiivtyLogViewModel LogVm = new ActiivtyLogViewModel();

        bool TimePlanValidation = true;
        string ExceptionMsg = "";
        bool Continue = true;

        ISalaryHeaderService _SalaryHeaderService;
        IUnitOfWork _unitOfWork;
        IExceptionHandlingService _exception;

        public SalaryHeaderController(ISalaryHeaderService PurchaseOrderHeaderService, IUnitOfWork unitOfWork, IExceptionHandlingService exec)
        {
            _SalaryHeaderService = PurchaseOrderHeaderService;
            _exception = exec;
            _unitOfWork = unitOfWork;
            if (!SalaryEvents.Initialized)
            {
                SalaryEvents Obj = new SalaryEvents();
            }

            UserRoles = (List<string>)System.Web.HttpContext.Current.Session["Roles"];

            //Log Initialization
            LogVm.SessionId = 0;
            LogVm.ControllerName = System.Web.HttpContext.Current.Request.RequestContext.RouteData.GetRequiredString("controller");
            LogVm.ActionName = System.Web.HttpContext.Current.Request.RequestContext.RouteData.GetRequiredString("action");
            LogVm.User = System.Web.HttpContext.Current.Request.RequestContext.HttpContext.User.Identity.Name;

        }

        // GET: /SalaryHeader/


        public ActionResult DocumentTypeIndex(int id)//DocumentCategoryId
        {
            var p = new DocumentTypeService(_unitOfWork).FindByDocumentCategory(id).ToList();

            if (p != null)
            {
                if (p.Count == 1)
                    return RedirectToAction("Index", new { id = p.FirstOrDefault().DocumentTypeId });
            }

            return View("DocumentTypeList", p);
        }

        public ActionResult Index(int id, string IndexType)//DocumentTypeId 
        {
            //var IpAddr = GetIPAddress();

            if (IndexType == "PTS")
            {
                return RedirectToAction("Index_PendingToSubmit", new { id });
            }
            else if (IndexType == "PTR")
            {
                return RedirectToAction("Index_PendingToReview", new { id });
            }
            IQueryable<SalaryHeaderViewModel> p = _SalaryHeaderService.GetSalaryHeaderList(id, User.Identity.Name);
            ViewBag.Name = new DocumentTypeService(_unitOfWork).Find(id).DocumentTypeName;
            PrepareViewBag(id);
            ViewBag.PendingToSubmit = PendingToSubmitCount(id);
            ViewBag.PendingToReview = PendingToReviewCount(id);
            ViewBag.IndexStatus = "All";

            ViewBag.id = id;
            return View(p);
        }

        public ActionResult Index_PendingToSubmit(int id)
        {
            IQueryable<SalaryHeaderViewModel> p = _SalaryHeaderService.GetSalaryHeaderListPendingToSubmit(id, User.Identity.Name);

            PrepareViewBag(id);
            ViewBag.PendingToSubmit = PendingToSubmitCount(id);
            ViewBag.PendingToReview = PendingToReviewCount(id);
            ViewBag.IndexStatus = "PTS";
            return View("Index", p);
        }

        public ActionResult Index_PendingToReview(int id)
        {
            IQueryable<SalaryHeaderViewModel> p = _SalaryHeaderService.GetSalaryHeaderListPendingToReview(id, User.Identity.Name);
            PrepareViewBag(id);
            ViewBag.PendingToSubmit = PendingToSubmitCount(id);
            ViewBag.PendingToReview = PendingToReviewCount(id);
            ViewBag.IndexStatus = "PTR";
            return View("Index", p);
        }

        private void PrepareViewBag(int id)
        {
            DocumentType DocType = new DocumentTypeService(_unitOfWork).Find(id);
            DocumentTypeSettingsViewModel DTS = new DocumentTypeSettingsService(_unitOfWork).GetDocumentTypeSettingsForDocument(id);
           
            int Cid = DocType.DocumentCategoryId;
            ViewBag.DocTypeList = new DocumentTypeService(_unitOfWork).FindByDocumentCategory(Cid).ToList();
            ViewBag.Name = DocType.DocumentTypeName;
            ViewBag.PartyCaption = DTS.PartyCaption;
            ViewBag.id = id;
            ViewBag.UnitConvForList = (from p in db.UnitConversonFor
                                       select p).ToList();
              ViewBag.AdminSetting =UserRoles.Contains("Admin").ToString();
            var DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            var SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            
            var settings = new SalarySettingsService(_unitOfWork).GetSalarySettingsForDocument(id, DivisionId, SiteId);
            if (settings != null)
            {
                ViewBag.WizardId = settings.WizardMenuId;
                ViewBag.ImportMenuId = settings.ImportMenuId;
                ViewBag.SqlProcDocumentPrint = settings.SqlProcDocumentPrint;
                ViewBag.ExportMenuId = settings.ExportMenuId;
            }



        }


        [HttpGet]
        public ActionResult BarcodePrint(int id)
        {

            string GenDocId = "";

            SalaryHeader header = _SalaryHeaderService.Find(id);
            GenDocId = header.DocTypeId.ToString() + '-' + header.DocNo;
            //return RedirectToAction("PrintBarCode", "Report_BarcodePrint", new { GenHeaderId = id });
            return Redirect((string)System.Configuration.ConfigurationManager.AppSettings["JobsDomain"] + "/Report_BarcodePrint/PrintBarCode/?GenHeaderId=" + GenDocId + "&queryString=" + GenDocId);
        }




        // GET: /SalaryHeader/Create

        public ActionResult Create(int id)//DocumentTypeId
        {
            SalaryHeaderViewModel p = new SalaryHeaderViewModel();
            p.DocDate = DateTime.Now;
            p.CreatedDate = DateTime.Now;
            p.DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            p.SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            List<DocumentTypeHeaderAttributeViewModel> tem = new DocumentTypeService(_unitOfWork).GetDocumentTypeHeaderAttribute(id).ToList();

            //Getting Settings
            var settings = new SalarySettingsService(_unitOfWork).GetSalarySettingsForDocument(id, p.DivisionId, p.SiteId);

            if (settings == null && UserRoles.Contains("SysAdmin"))
            {
                return RedirectToAction("Create", "SalarySettings", new { id = id }).Warning("Please create job order settings");
            }
            else if (settings == null && !UserRoles.Contains("SysAdmin"))
            {
                return View("~/Views/Shared/InValidSettings.cshtml");
            }
            p.SalarySettings = Mapper.Map<SalarySettings, SalarySettingsViewModel>(settings);

            if (new RolePermissionService(_unitOfWork).IsActionAllowed(UserRoles, id, null, this.ControllerContext.RouteData.Values["controller"].ToString(), "Create") == false)
            {
                return View("~/Views/Shared/PermissionDenied.cshtml").Warning("You don't have permission to do this task.");
            }


            p.DocTypeId = id;
            p.DocumentTypeSettings = new DocumentTypeSettingsService(_unitOfWork).GetDocumentTypeSettingsForDocument(p.DocTypeId);

            PrepareViewBag(id);

            p.DocNo = new DocumentTypeService(_unitOfWork).FGetNewDocNo("DocNo", ConfigurationManager.AppSettings["DataBaseSchema"] + ".SalaryHeaders", p.DocTypeId, p.DocDate, p.DivisionId, p.SiteId);
            ViewBag.Mode = "Add";
            ViewBag.Name = new DocumentTypeService(_unitOfWork).Find(id).DocumentTypeName;
            ViewBag.id = id;
            return View(p);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Post(SalaryHeaderViewModel svm)
        {
            bool BeforeSave = true;
            bool CostCenterGenerated = false;

            SalaryHeader s = Mapper.Map<SalaryHeaderViewModel, SalaryHeader>(svm);

            #region BeforeSaveEvents

            try
            {

                if (svm.SalaryHeaderId <= 0)
                    BeforeSave = SalaryDocEvents.beforeHeaderSaveEvent(this, new JobEventArgs(svm.SalaryHeaderId, EventModeConstants.Add), ref db);
                else
                    BeforeSave = SalaryDocEvents.beforeHeaderSaveEvent(this, new JobEventArgs(svm.SalaryHeaderId, EventModeConstants.Edit), ref db);

            }
            catch (Exception ex)
            {
                string message = _exception.HandleException(ex);
                TempData["CSEXC"] += message;
                EventException = true;
            }

            if (!BeforeSave)
                TempData["CSEXC"] += "Failed validation before save";
            #endregion

            #region DocTypeTimeLineValidation

            try
            {

                if (svm.SalaryHeaderId <= 0)
                    TimePlanValidation = DocumentValidation.ValidateDocument(Mapper.Map<DocumentUniqueId>(svm), DocumentTimePlanTypeConstants.Create, User.Identity.Name, out ExceptionMsg, out Continue);
                else
                    TimePlanValidation = DocumentValidation.ValidateDocument(Mapper.Map<DocumentUniqueId>(svm), DocumentTimePlanTypeConstants.Modify, User.Identity.Name, out ExceptionMsg, out Continue);

            }
            catch (Exception ex)
            {
                string message = _exception.HandleException(ex);
                TempData["CSEXC"] += message;
                TimePlanValidation = false;
            }

            if (!TimePlanValidation)
                TempData["CSEXC"] += ExceptionMsg;

            #endregion

            if (ModelState.IsValid && BeforeSave && !EventException && (TimePlanValidation || Continue))
            {
                //CreateLogic
                #region CreateRecord
                if (svm.SalaryHeaderId <= 0)
                {
                    s.CreatedDate = DateTime.Now;
                    s.ModifiedDate = DateTime.Now;
                    s.CreatedBy = User.Identity.Name;
                    s.ModifiedBy = User.Identity.Name;
                    s.Status = (int)StatusConstants.Drafted;
                    s.ObjectState = Model.ObjectState.Added;
                    db.SalaryHeader.Add(s);

                    try
                    {
                        SalaryDocEvents.onHeaderSaveEvent(this, new JobEventArgs(s.SalaryHeaderId, EventModeConstants.Add), ref db);
                    }
                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        TempData["CSEXC"] += message;
                        EventException = true;
                    }


                    try
                    {
                        if (EventException)
                        { throw new Exception(); }
                        //_unitOfWork.Save();
                        db.SaveChanges();
                    }

                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        TempData["CSEXC"] += message;
                        PrepareViewBag(svm.DocTypeId);
                        ViewBag.Mode = "Add";
                        return View("Create", svm);
                    }



                    try
                    {
                        SalaryDocEvents.afterHeaderSaveEvent(this, new JobEventArgs(s.SalaryHeaderId, EventModeConstants.Add), ref db);
                    }
                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        TempData["CSEXC"] += message;
                    }

                    LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                    {
                        DocTypeId = s.DocTypeId,
                        DocId = s.SalaryHeaderId,
                        ActivityType = (int)ActivityTypeContants.Added,
                        DocNo = s.DocNo,
                        DocDate = s.DocDate,
                        DocStatus = s.Status,
                    }));


                    return RedirectToAction("Modify", "SalaryHeader", new { Id = s.SalaryHeaderId }).Success("Data saved successfully");

                }
                #endregion


                //EditLogic
                #region EditRecord

                else
                {
                    List<LogTypeViewModel> LogList = new List<LogTypeViewModel>();

                    SalaryHeader temp = db.SalaryHeader.Find(s.SalaryHeaderId);

                    SalaryHeader ExRec = Mapper.Map<SalaryHeader>(temp);

                    int status = temp.Status;

                    if (temp.Status != (int)StatusConstants.Drafted && temp.Status != (int)StatusConstants.Import)
                        temp.Status = (int)StatusConstants.Modified;



                    temp.DocDate = s.DocDate;
                    temp.DocNo = s.DocNo;
                    temp.Remark = s.Remark;
                    temp.ModifiedDate = DateTime.Now;
                    temp.ModifiedBy = User.Identity.Name;
                    temp.ObjectState = Model.ObjectState.Modified;
                    db.SalaryHeader.Add(temp);
                    //_SalaryHeaderService.Update(temp);

                    LogList.Add(new LogTypeViewModel
                    {
                        ExObj = ExRec,
                        Obj = temp,
                    });




                    XElement Modifications = new ModificationsCheckService().CheckChanges(LogList);

                    try
                    {
                        SalaryDocEvents.onHeaderSaveEvent(this, new JobEventArgs(s.SalaryHeaderId, EventModeConstants.Edit), ref db);
                    }
                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        TempData["CSEXC"] += message;
                        EventException = true;
                    }

                    try
                    {
                        if (EventException)
                        { throw new Exception(); }

                        db.SaveChanges();
                        //_unitOfWork.Save();
                    }

                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);

                        PrepareViewBag(svm.DocTypeId);
                        TempData["CSEXC"] += message;
                        ViewBag.id = svm.DocTypeId;
                        ViewBag.Mode = "Edit";
                        return View("Create", svm);
                    }

                    try
                    {
                        SalaryDocEvents.afterHeaderSaveEvent(this, new JobEventArgs(s.SalaryHeaderId, EventModeConstants.Edit), ref db);
                    }
                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        TempData["CSEXC"] += message;
                    }

                    LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                    {
                        DocTypeId = temp.DocTypeId,
                        DocId = temp.SalaryHeaderId,
                        ActivityType = (int)ActivityTypeContants.Modified,
                        DocNo = temp.DocNo,
                        xEModifications = Modifications,
                        DocDate = temp.DocDate,
                        DocStatus = temp.Status,
                    }));

                    return RedirectToAction("Index", new { id = svm.DocTypeId }).Success("Data saved successfully");

                }
                #endregion

            }
            PrepareViewBag(svm.DocTypeId);
            ViewBag.Mode = "Add";
            return View("Create", svm);
        }


        [HttpGet]
        public ActionResult Modify(int id, string IndexType)
        {
            SalaryHeader header = _SalaryHeaderService.Find(id);
            if (header.Status == (int)StatusConstants.Drafted || header.Status == (int)StatusConstants.Import)
                return Edit(id, IndexType);
            else
                return HttpNotFound();
        }

        [HttpGet]
        public ActionResult ModifyAfter_Submit(int id, string IndexType)
        {
            SalaryHeader header = _SalaryHeaderService.Find(id);
            if (header.Status == (int)StatusConstants.Submitted || header.Status == (int)StatusConstants.Modified)
                return Edit(id, IndexType);
            else
                return HttpNotFound();
        }

        [HttpGet]
        public ActionResult ModifyAfter_Approve(int id, string IndexType)
        {
            SalaryHeader header = _SalaryHeaderService.Find(id);
            if (header.Status == (int)StatusConstants.Approved)
                return Edit(id, IndexType);
            else
                return HttpNotFound();
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            SalaryHeader header = _SalaryHeaderService.Find(id);
            if (header.Status == (int)StatusConstants.Drafted || header.Status == (int)StatusConstants.Import)
                return Remove(id);
            else
                return HttpNotFound();
        }

        [HttpGet]
        public ActionResult DeleteAfter_Submit(int id)
        {
            SalaryHeader header = _SalaryHeaderService.Find(id);
            if (header.Status == (int)StatusConstants.Submitted || header.Status == (int)StatusConstants.Modified)
                return Remove(id);
            else
                return HttpNotFound();
        }

        [HttpGet]
        public ActionResult DeleteAfter_Approve(int id)
        {
            SalaryHeader header = _SalaryHeaderService.Find(id);
            if (header.Status == (int)StatusConstants.Approved)
                return Remove(id);
            else
                return HttpNotFound();
        }

        [HttpGet]
        public ActionResult DetailInformation(int id, int? DocLineId, string IndexType)
        {
            return RedirectToAction("Detail", new { id = id, transactionType = "detail", DocLineId = DocLineId, IndexType = IndexType });
        }



        // GET: /SalaryHeader/Edit/5
        private ActionResult Edit(int id, string IndexType)
        {
            ViewBag.IndexStatus = IndexType;
            SalaryHeaderViewModel s = _SalaryHeaderService.GetSalaryHeader(id);

            if (new RolePermissionService(_unitOfWork).IsActionAllowed(UserRoles, s.DocTypeId, null, this.ControllerContext.RouteData.Values["controller"].ToString(), "Edit") == false)
            {
                return View("~/Views/Shared/PermissionDenied.cshtml").Warning("You don't have permission to do this task.");
            }

            #region DocTypeTimeLineValidation
            try
            {

                TimePlanValidation = DocumentValidation.ValidateDocument(Mapper.Map<DocumentUniqueId>(s), DocumentTimePlanTypeConstants.Modify, User.Identity.Name, out ExceptionMsg, out Continue);

            }
            catch (Exception ex)
            {
                string message = _exception.HandleException(ex);
                TempData["CSEXC"] += message;
                TimePlanValidation = false;
            }

            if (!TimePlanValidation)
                TempData["CSEXC"] += ExceptionMsg;
            #endregion

            if ((!TimePlanValidation && !Continue))
            {
                return RedirectToAction("DetailInformation", new { id = id, IndexType = IndexType });
            }

            //Job Order Settings
            var settings = new SalarySettingsService(_unitOfWork).GetSalarySettingsForDocument(s.DocTypeId, s.DivisionId, s.SiteId);

            if (settings == null && UserRoles.Contains("SysAdmin"))
            {
                return RedirectToAction("Create", "SalarySettings", new { id = s.DocTypeId }).Warning("Please create job order settings");
            }
            else if (settings == null && !UserRoles.Contains("SysAdmin"))
            {
                return View("~/Views/Shared/InValidSettings.cshtml");
            }

            s.SalarySettings = Mapper.Map<SalarySettings, SalarySettingsViewModel>(settings);
            s.DocumentTypeSettings = new DocumentTypeSettingsService(_unitOfWork).GetDocumentTypeSettingsForDocument(s.DocTypeId);

            ////Perks

            
            PrepareViewBag(s.DocTypeId);
            if (s == null)
            {
                return HttpNotFound();
            }



            s.CalculationFooterChargeCount = new SalaryHeaderChargeService(_unitOfWork).GetCalculationFooterList(id).Count();

            //ViewBag.transactionType = "detail";

            ViewBag.Mode = "Edit";
            ViewBag.transactionType = "";

            ViewBag.Name = new DocumentTypeService(_unitOfWork).Find(s.DocTypeId).DocumentTypeName;
            ViewBag.id = s.DocTypeId;

            if (!(System.Web.HttpContext.Current.Request.UrlReferrer.PathAndQuery).Contains("Create"))
                LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                {
                    DocTypeId = s.DocTypeId,
                    DocId = s.SalaryHeaderId,
                    ActivityType = (int)ActivityTypeContants.Detail,
                    DocNo = s.DocNo,
                    DocDate = s.DocDate,
                    DocStatus = s.Status,
                }));

            return View("Create", s);
        }

        private ActionResult Remove(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SalaryHeader SalaryHeader = _SalaryHeaderService.Find(id);

            if (SalaryHeader == null)
            {
                return HttpNotFound();
            }

            if (new RolePermissionService(_unitOfWork).IsActionAllowed(UserRoles, SalaryHeader.DocTypeId, null, this.ControllerContext.RouteData.Values["controller"].ToString(), "Remove") == false)
            {
                return PartialView("~/Views/Shared/PermissionDenied_Modal.cshtml").Warning("You don't have permission to do this task.");
            }

            #region DocTypeTimeLineValidation

            try
            {
                TimePlanValidation = DocumentValidation.ValidateDocument(Mapper.Map<DocumentUniqueId>(SalaryHeader), DocumentTimePlanTypeConstants.Delete, User.Identity.Name, out ExceptionMsg, out Continue);
                TempData["CSEXC"] += ExceptionMsg;
            }
            catch (Exception ex)
            {
                string message = _exception.HandleException(ex);
                TempData["CSEXC"] += message;
                TimePlanValidation = false;
            }

            if (!TimePlanValidation && !Continue)
            {
                return PartialView("AjaxError");
            }
            #endregion

            ReasonViewModel rvm = new ReasonViewModel()
            {
                id = id,
            };
            return PartialView("_Reason", rvm);
        }



        [Authorize]
        public ActionResult Detail(int id, string IndexType, string transactionType, int? DocLineId)
        {
            if (DocLineId.HasValue)
                ViewBag.DocLineId = DocLineId;
            //Saving ViewBag Data::

            ViewBag.transactionType = transactionType;
            ViewBag.IndexStatus = IndexType;

            SalaryHeaderViewModel s = _SalaryHeaderService.GetSalaryHeader(id);
            //Getting Settings
            var settings = new SalarySettingsService(_unitOfWork).GetSalarySettingsForDocument(s.DocTypeId, s.DivisionId, s.SiteId);

            s.SalarySettings = Mapper.Map<SalarySettings, SalarySettingsViewModel>(settings);

            s.DocumentTypeSettings = new DocumentTypeSettingsService(_unitOfWork).GetDocumentTypeSettingsForDocument(s.DocTypeId);



            PrepareViewBag(s.DocTypeId);
            if (s == null)
            {
                return HttpNotFound();
            }

            if (String.IsNullOrEmpty(transactionType) || transactionType == "detail")
                LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                {
                    DocTypeId = s.DocTypeId,
                    DocId = s.SalaryHeaderId,
                    ActivityType = (int)ActivityTypeContants.Detail,
                    DocNo = s.DocNo,
                    DocDate = s.DocDate,
                    DocStatus = s.Status,
                }));


            return View("Create", s);
        }





        // POST: /PurchaseOrderHeader/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(ReasonViewModel vm)
        {
            List<LogTypeViewModel> LogList = new List<LogTypeViewModel>();
            bool BeforeSave = true;

            try
            {
                BeforeSave = SalaryDocEvents.beforeHeaderDeleteEvent(this, new JobEventArgs(vm.id), ref db);
            }
            catch (Exception ex)
            {
                string message = _exception.HandleException(ex);
                TempData["CSEXC"] += message;
                EventException = true;
            }

            if (!BeforeSave)
                TempData["CSEXC"] += "Failed validation before delete";

            var SalaryHeader = (from p in db.SalaryHeader
                                  where p.SalaryHeaderId == vm.id
                                  select p).FirstOrDefault();

            if (ModelState.IsValid && BeforeSave && !EventException)
            {
                LogList.Add(new LogTypeViewModel
                {
                    ExObj = Mapper.Map<SalaryHeader>(SalaryHeader),
                });

                var LedgerHeader = (from H in db.LedgerHeader where H.LedgerHeaderId == SalaryHeader.LedgerHeaderId select H).FirstOrDefault();
                if (LedgerHeader != null)
                {
                    var LedgerList = (from L in db.Ledger where L.LedgerHeaderId == LedgerHeader.LedgerHeaderId select L).ToList();
                    foreach(var ledger in LedgerList)
                    {
                        var LedgerAdjList = (from L in db.LedgerAdj where L.CrLedgerId == ledger.LedgerId select L).ToList();
                        foreach(var ledgeradj in LedgerAdjList)
                        {
                            ledgeradj.ObjectState = Model.ObjectState.Deleted;
                            db.LedgerAdj.Remove(ledgeradj);
                        }
                        ledger.ObjectState = Model.ObjectState.Deleted;
                        db.Ledger.Remove(ledger);
                    }

                    var LedgerLineList = (from L in db.LedgerLine where L.LedgerHeaderId == LedgerHeader.LedgerHeaderId select L).ToList();
                    foreach (var ledgerline in LedgerLineList)
                    {
                        ledgerline.ObjectState = Model.ObjectState.Deleted;
                        db.LedgerLine.Remove(ledgerline);
                    }

                    LedgerHeader.ObjectState = Model.ObjectState.Deleted;
                    db.LedgerHeader.Remove(LedgerHeader);
                }


                
                //var SalaryLine = new SalaryLineService(_unitOfWork).GetSalaryLineforDelete(vm.id);
                var SalaryLine = (from p in db.SalaryLine
                                    where p.SalaryHeaderId == vm.id
                                    select p).ToList();

                var JOLineIds = SalaryLine.Select(m => m.SalaryLineId).ToArray();



                var LineChargeRecords = (from p in db.SalaryLineCharge
                                         where JOLineIds.Contains(p.LineTableId)
                                         select p).ToList();

                var HeaderChargeRecords = (from p in db.SalaryHeaderCharge
                                           where p.HeaderTableId == vm.id
                                           select p).ToList();


                try
                {
                    SalaryDocEvents.onHeaderDeleteEvent(this, new JobEventArgs(vm.id), ref db);
                }
                catch (Exception ex)
                {
                    string message = _exception.HandleException(ex);
                    TempData["CSEXC"] += message;
                    EventException = true;
                }





                foreach (var item in LineChargeRecords)
                {
                    item.ObjectState = Model.ObjectState.Deleted;
                    db.SalaryLineCharge.Remove(item);
                }





                //Mark ObjectState.Delete to all the Purchase Order Lines. 
                foreach (var item in SalaryLine)
                {

                    LogList.Add(new LogTypeViewModel
                    {
                        ExObj = Mapper.Map<SalaryLine>(item),
                    });



                    item.ObjectState = Model.ObjectState.Deleted;
                    db.SalaryLine.Remove(item);


                }





                foreach (var item in HeaderChargeRecords)
                {
                    item.ObjectState = Model.ObjectState.Deleted;
                    db.SalaryHeaderCharge.Remove(item);
                }



                // Now delete the Purhcase Order Header
                //_SalaryHeaderService.Delete(SalaryHeader);

                int ReferenceDocId = SalaryHeader.SalaryHeaderId;
                int ReferenceDocTypeId = SalaryHeader.DocTypeId;


                SalaryHeader.ObjectState = Model.ObjectState.Deleted;
                db.SalaryHeader.Remove(SalaryHeader);


                var settings = new SalarySettingsService(_unitOfWork).GetSalarySettingsForDocument(SalaryHeader.DocTypeId, SalaryHeader.DivisionId, SalaryHeader.SiteId);


                XElement Modifications = new ModificationsCheckService().CheckChanges(LogList);


                //Commit the DB
                try
                {
                    if (EventException)
                        throw new Exception();

                    db.SaveChanges();
                }

                catch (Exception ex)
                {
                    string message = _exception.HandleException(ex);
                    TempData["CSEXC"] += message;
                    return PartialView("_Reason", vm);
                }

                try
                {
                    SalaryDocEvents.afterHeaderDeleteEvent(this, new JobEventArgs(vm.id), ref db);
                }
                catch (Exception ex)
                {
                    string message = _exception.HandleException(ex);
                    TempData["CSEXC"] += message;
                }

                LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                {
                    DocTypeId = SalaryHeader.DocTypeId,
                    DocId = SalaryHeader.SalaryHeaderId,
                    ActivityType = (int)ActivityTypeContants.Deleted,
                    UserRemark = vm.Reason,
                    DocNo = SalaryHeader.DocNo,
                    xEModifications = Modifications,
                    DocDate = SalaryHeader.DocDate,
                    DocStatus = SalaryHeader.Status,
                }));

                return Json(new { success = true });
            }
            return PartialView("_Reason", vm);
        }


        public ActionResult Submit(int id, string IndexType, string TransactionType)
        {
            SalaryHeader s = db.SalaryHeader.Find(id);
            if (new RolePermissionService(_unitOfWork).IsActionAllowed(UserRoles, s.DocTypeId, null, this.ControllerContext.RouteData.Values["controller"].ToString(), "Submit") == false)
            {
                return View("~/Views/Shared/PermissionDenied.cshtml").Warning("You don't have permission to do this task.");
            }

            #region DocTypeTimeLineValidation
            


            try
            {
                TimePlanValidation = DocumentValidation.ValidateDocument(Mapper.Map<DocumentUniqueId>(s), DocumentTimePlanTypeConstants.Submit, User.Identity.Name, out ExceptionMsg, out Continue);
                TempData["CSEXC"] += ExceptionMsg;
            }
            catch (Exception ex)
            {
                string message = _exception.HandleException(ex);
                TempData["CSEXC"] += message;
                TimePlanValidation = false;
            }

            if (!TimePlanValidation && !Continue)
            {
                return RedirectToAction("Index", new { id = s.DocTypeId, IndexType = IndexType });
            }
            #endregion


            return RedirectToAction("Detail", new { id = id, IndexType = IndexType, transactionType = string.IsNullOrEmpty(TransactionType) ? "submit" : TransactionType });
        }


        [HttpPost, ActionName("Detail")]
        [MultipleButton(Name = "Command", Argument = "Submit")]
        public ActionResult Submitted(int Id, string IndexType, string UserRemark, string IsContinue, string GenGatePass)
        {

            bool BeforeSave = true;
            try
            {
                BeforeSave = SalaryDocEvents.beforeHeaderSubmitEvent(this, new JobEventArgs(Id), ref db);
            }
            catch (Exception ex)
            {
                string message = _exception.HandleException(ex);
                TempData["CSEXC"] += message;
                EventException = true;
            }

            if (!BeforeSave)
                TempData["CSEXC"] += "Falied validation before submit.";

            SalaryHeader pd = db.SalaryHeader.Find(Id);


            if (ModelState.IsValid && BeforeSave && !EventException)
            {
                int Cnt = 0;
                int CountUid = 0;
                //SalaryHeader pd = new SalaryHeaderService(_unitOfWork).Find(Id);              

                int ActivityType;
                if (User.Identity.Name == pd.ModifiedBy || UserRoles.Contains("Admin"))
                {

                    pd.Status = (int)StatusConstants.Submitted;
                    ActivityType = (int)ActivityTypeContants.Submitted;

                    SalarySettings Settings = new SalarySettingsService(_unitOfWork).GetSalarySettingsForDocument(pd.DocTypeId, pd.DivisionId, pd.SiteId);


                     pd.LedgerHeaderId = LedgerPosting(Id);


                    //_SalaryHeaderService.Update(pd);
                    pd.ReviewBy = null;
                    pd.ObjectState = Model.ObjectState.Modified;
                    db.SalaryHeader.Add(pd);



                    try
                    {
                        SalaryDocEvents.onHeaderSubmitEvent(this, new JobEventArgs(Id), ref db);
                    }
                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        TempData["CSEXC"] += message;
                        EventException = true;
                    }

                    try
                    {
                        if (EventException)
                        { throw new Exception(); }

                        db.SaveChanges();
                    }

                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        TempData["CSEXC"] += message;
                        return RedirectToAction("Index", new { id = pd.DocTypeId });
                    }




                    try
                    {
                        SalaryDocEvents.afterHeaderSubmitEvent(this, new JobEventArgs(Id), ref db);
                    }
                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        TempData["CSEXC"] += message;
                    }


                    LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                    {
                        DocTypeId = pd.DocTypeId,
                        DocId = pd.SalaryHeaderId,
                        ActivityType = ActivityType,
                        UserRemark = UserRemark,
                        DocNo = pd.DocNo,
                        DocDate = pd.DocDate,
                        DocStatus = pd.Status,
                    }));



                    return Redirect(System.Configuration.ConfigurationManager.AppSettings["CurrentDomain"] + "/" + "SalaryHeader" + "/" + "Index" + "/" + pd.DocTypeId + "?IndexType=" + IndexType);
                }
                else
                    return RedirectToAction("Index", new { id = pd.DocTypeId, IndexType = IndexType }).Warning("Record can be submitted by user " + pd.ModifiedBy + " only.");
            }

            return RedirectToAction("Index", new { id = pd.DocTypeId, IndexType = IndexType });
        }





        public ActionResult Review(int id, string IndexType, string TransactionType)
        {
            return RedirectToAction("Detail", new { id = id, IndexType = IndexType, transactionType = string.IsNullOrEmpty(TransactionType) ? "review" : TransactionType });
        }


        [HttpPost, ActionName("Detail")]
        [MultipleButton(Name = "Command", Argument = "Review")]
        public ActionResult Reviewed(int Id, string IndexType, string UserRemark, string IsContinue)
        {
            SalaryHeader pd = db.SalaryHeader.Find(Id);

            if (ModelState.IsValid)
            {

                pd.ReviewCount = (pd.ReviewCount ?? 0) + 1;
                pd.ReviewBy += User.Identity.Name + ", ";
                pd.ObjectState = Model.ObjectState.Modified;
                db.SalaryHeader.Add(pd);

                try
                {
                    SalaryDocEvents.onHeaderReviewEvent(this, new JobEventArgs(Id), ref db);
                }
                catch (Exception ex)
                {
                    string message = _exception.HandleException(ex);
                    TempData["CSEXC"] += message;
                }

                db.SaveChanges();

                try
                {
                    SalaryDocEvents.afterHeaderReviewEvent(this, new JobEventArgs(Id), ref db);
                }
                catch (Exception ex)
                {
                    string message = _exception.HandleException(ex);
                    TempData["CSEXC"] += message;
                }

                LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                {
                    DocTypeId = pd.DocTypeId,
                    DocId = pd.SalaryHeaderId,
                    ActivityType = (int)ActivityTypeContants.Reviewed,
                    UserRemark = UserRemark,
                    DocNo = pd.DocNo,
                    DocDate = pd.DocDate,
                    DocStatus = pd.Status,
                }));



                string ReturnUrl = System.Configuration.ConfigurationManager.AppSettings["CurrentDomain"] + "/" + "SalaryHeader" + "/" + "Index" + "/" + pd.DocTypeId + "?IndexType=" + IndexType;
                if (!string.IsNullOrEmpty(IsContinue) && IsContinue == "True")
                {

                    int nextId = new NextPrevIdService(_unitOfWork).GetNextPrevId(Id, pd.DocTypeId, User.Identity.Name, ForActionConstants.PendingToReview, "Web.SalaryHeaders", "SalaryHeaderId", PrevNextConstants.Next);

                    if (nextId == 0)
                    {
                        var PendingtoSubmitCount = _SalaryHeaderService.GetSalaryHeaderListPendingToSubmit(pd.DocTypeId, User.Identity.Name).Count();
                        if (PendingtoSubmitCount > 0)
                            ReturnUrl = System.Configuration.ConfigurationManager.AppSettings["CurrentDomain"] + "/" + "SalaryHeader" + "/" + "Index_PendingToReview" + "/" + pd.DocTypeId + "?IndexType=" + IndexType;
                        else
                            ReturnUrl = System.Configuration.ConfigurationManager.AppSettings["CurrentDomain"] + "/" + "SalaryHeader" + "/" + "Index" + "/" + pd.DocTypeId + "?IndexType=" + IndexType;
                    }
                    else
                        ReturnUrl = System.Configuration.ConfigurationManager.AppSettings["CurrentDomain"] + "/" + "SalaryHeader" + "/" + "Review" + "/" + nextId + "?TransactionType=ReviewContinue&IndexType=" + IndexType;
                }


                return Redirect(ReturnUrl);

            }

            return RedirectToAction("Index", new { id = pd.DocTypeId, IndexType = IndexType }).Warning("Error in Reviewing.");
        }

        [HttpGet]
        public ActionResult Report(int id)
        {
            DocumentType Dt = new DocumentType();
            Dt = new DocumentTypeService(_unitOfWork).Find(id);

            SalarySettings SEttings = new SalarySettingsService(_unitOfWork).GetSalarySettingsForDocument(Dt.DocumentTypeId, (int)System.Web.HttpContext.Current.Session["DivisionId"], (int)System.Web.HttpContext.Current.Session["SiteId"]);

            Dictionary<int, string> DefaultValue = new Dictionary<int, string>();

            if (!Dt.ReportMenuId.HasValue)
                throw new Exception("Report Menu not configured in document types");

            Model.Models.Menu menu = new MenuService(_unitOfWork).Find(Dt.ReportMenuId ?? 0);

            if (menu != null)
            {
                ReportHeader header = new ReportHeaderService(_unitOfWork).GetReportHeaderByName(menu.MenuName);

                ReportLine Line = new ReportLineService(_unitOfWork).GetReportLineByName("DocumentType", header.ReportHeaderId);
                if (Line != null)
                    DefaultValue.Add(Line.ReportLineId, id.ToString());
                ReportLine Site = new ReportLineService(_unitOfWork).GetReportLineByName("Site", header.ReportHeaderId);
                if (Site != null)
                    DefaultValue.Add(Site.ReportLineId, ((int)System.Web.HttpContext.Current.Session["SiteId"]).ToString());
                ReportLine Division = new ReportLineService(_unitOfWork).GetReportLineByName("Division", header.ReportHeaderId);
                if (Division != null)
                    DefaultValue.Add(Division.ReportLineId, ((int)System.Web.HttpContext.Current.Session["DivisionId"]).ToString());
                //ReportLine Process = new ReportLineService(_unitOfWork).GetReportLineByName("Process", header.ReportHeaderId);
                //if (Process != null)
                //    DefaultValue.Add(Process.ReportLineId, ((int)SEttings.ProcessId).ToString());
            }

            TempData["ReportLayoutDefaultValues"] = DefaultValue;

            return Redirect((string)System.Configuration.ConfigurationManager.AppSettings["JobsDomain"] + "/Report_ReportPrint/ReportPrint/?MenuId=" + Dt.ReportMenuId);

        }


        public ActionResult Wizard(int id)//Document Type Id
        {
            //ControllerAction ca = new ControllerActionService(_unitOfWork).Find(id);
            SalaryHeaderViewModel vm = new SalaryHeaderViewModel();

            vm.DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            vm.SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];

            var settings = new SalarySettingsService(_unitOfWork).GetSalarySettingsForDocument(id, vm.DivisionId, vm.SiteId);

            if (settings != null)
            {
                if (settings.WizardMenuId != null)
                {
                    MenuViewModel menuviewmodel = new MenuService(_unitOfWork).GetMenu((int)settings.WizardMenuId);

                    if (menuviewmodel == null)
                    {
                        return View("~/Views/Shared/UnderImplementation.cshtml");
                    }
                    else if (!string.IsNullOrEmpty(menuviewmodel.URL))
                    {
                        return Redirect(System.Configuration.ConfigurationManager.AppSettings[menuviewmodel.URL] + "/" + menuviewmodel.ControllerName + "/" + menuviewmodel.ActionName + "/" + id + "?MenuId=" + menuviewmodel.MenuId);
                    }
                    else
                    {
                        return RedirectToAction(menuviewmodel.ActionName, menuviewmodel.ControllerName, new { MenuId = menuviewmodel.MenuId, id = menuviewmodel.RouteId });
                    }
                }
            }
            return RedirectToAction("Index", new { id = id });
        }


        public ActionResult Import(int id)//Document Type Id
        {
            //ControllerAction ca = new ControllerActionService(_unitOfWork).Find(id);
            SalaryHeaderViewModel vm = new SalaryHeaderViewModel();

            vm.DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            vm.SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];

            var settings = new SalarySettingsService(_unitOfWork).GetSalarySettingsForDocument(id, vm.DivisionId, vm.SiteId);

            if (settings != null)
            {
                if (settings.ImportMenuId != null)
                {
                    MenuViewModel menuviewmodel = new MenuService(_unitOfWork).GetMenu((int)settings.ImportMenuId);

                    if (menuviewmodel == null)
                    {
                        return View("~/Views/Shared/UnderImplementation.cshtml");
                    }
                    else if (!string.IsNullOrEmpty(menuviewmodel.URL))
                    {
                        return Redirect(System.Configuration.ConfigurationManager.AppSettings[menuviewmodel.URL] + "/" + menuviewmodel.ControllerName + "/" + menuviewmodel.ActionName + "/" + id + "?MenuId=" + menuviewmodel.MenuId);
                    }
                    else
                    {
                        return RedirectToAction(menuviewmodel.ActionName, menuviewmodel.ControllerName, new { MenuId = menuviewmodel.MenuId, id = id });
                    }
                }
            }
            return RedirectToAction("Index", new { id = id });
        }

        public ActionResult Action_Menu(int Id, int MenuId, string ReturnUrl)
        {
            MenuViewModel menuviewmodel = new MenuService(_unitOfWork).GetMenu(MenuId);

            if (menuviewmodel != null)
            {
                if (!string.IsNullOrEmpty(menuviewmodel.URL))
                {
                    return Redirect(System.Configuration.ConfigurationManager.AppSettings[menuviewmodel.URL] + "/" + menuviewmodel.ControllerName + "/" + menuviewmodel.ActionName + "?Id=" + Id + "&ReturnUrl=" + ReturnUrl);
                }
                else
                {
                    return RedirectToAction(menuviewmodel.ActionName, menuviewmodel.ControllerName, new { id = Id, ReturnUrl = ReturnUrl });
                }
            }
            return null;
        }

        public int PendingToSubmitCount(int id)
        {
            return (_SalaryHeaderService.GetSalaryHeaderListPendingToSubmit(id, User.Identity.Name)).Count();
        }

        public int PendingToReviewCount(int id)
        {
            return (_SalaryHeaderService.GetSalaryHeaderListPendingToReview(id, User.Identity.Name)).Count();
        }

        [HttpGet]
        public ActionResult NextPage(int DocId, int DocTypeId)//CurrentHeaderId
        {
            var nextId = new NextPrevIdService(_unitOfWork).GetNextPrevId(DocId, DocTypeId, User.Identity.Name, "", "Web.SalaryHeaders", "SalaryHeaderId", PrevNextConstants.Next);
            return Edit(nextId, "");
        }
        [HttpGet]
        public ActionResult PrevPage(int DocId, int DocTypeId)//CurrentHeaderId
        {
            var PrevId = new NextPrevIdService(_unitOfWork).GetNextPrevId(DocId, DocTypeId, User.Identity.Name, "", "Web.SalaryHeaders", "SalaryHeaderId", PrevNextConstants.Prev);
            return Edit(PrevId, "");
        }

        protected override void Dispose(bool disposing)
        {
            if (!string.IsNullOrEmpty((string)TempData["CSEXC"]))
            {
                CookieGenerator.CreateNotificationCookie(NotificationTypeConstants.Danger, (string)TempData["CSEXC"]);
                TempData.Remove("CSEXC");
            }
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult GeneratePrints(string Ids, int DocTypeId)
        {
            if (!string.IsNullOrEmpty(Ids))
            {
                int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
                int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];

                var Settings = new SalarySettingsService(_unitOfWork).GetSalarySettingsForDocument(DocTypeId, DivisionId, SiteId);

                if (new RolePermissionService(_unitOfWork).IsActionAllowed(UserRoles, DocTypeId, null, this.ControllerContext.RouteData.Values["controller"].ToString(), "GeneratePrints") == false)
                {
                    return View("~/Views/Shared/PermissionDenied.cshtml").Warning("You don't have permission to do this task.");
                }

                string ReportSql = "";

                if (Settings.DocumentPrintReportHeaderId.HasValue)
                    ReportSql = db.ReportHeader.Where((m) => m.ReportHeaderId == Settings.DocumentPrintReportHeaderId).FirstOrDefault().ReportSQL;

                try
                {

                    List<byte[]> PdfStream = new List<byte[]>();
                    foreach (var item in Ids.Split(',').Select(Int32.Parse))
                    {

                        DirectReportPrint drp = new DirectReportPrint();
                        var pd = db.SalaryHeader.Find(item);

                        if (Settings.isAllowedDuplicatePrint == false && pd.IsDocumentPrinted == true)
                            throw new Exception("Duplicate Print Not Allowed");
                        LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                        {
                            DocTypeId = pd.DocTypeId,
                            DocId = pd.SalaryHeaderId,
                            ActivityType = (int)ActivityTypeContants.Print,
                            DocNo = pd.DocNo,
                            DocDate = pd.DocDate,
                            DocStatus = pd.Status,
                        }));

                        byte[] Pdf;

                        if (!string.IsNullOrEmpty(ReportSql))
                        {
                            Pdf = drp.rsDirectDocumentPrint(ReportSql, User.Identity.Name, item);
                            PdfStream.Add(Pdf);
                        }
                        else
                        {

                            if (pd.Status == (int)StatusConstants.Drafted || pd.Status == (int)StatusConstants.Modified || pd.Status == (int)StatusConstants.Import)
                            {
                                //LogAct(item.ToString());
                                Pdf = drp.DirectDocumentPrint(Settings.SqlProcDocumentPrint, User.Identity.Name, item);

                                PdfStream.Add(Pdf);
                            }
                            else if (pd.Status == (int)StatusConstants.Submitted || pd.Status == (int)StatusConstants.ModificationSubmitted)
                            {
                                Pdf = drp.DirectDocumentPrint(Settings.SqlProcDocumentPrint, User.Identity.Name, item);

                                PdfStream.Add(Pdf);
                            }
                            else if (pd.Status == (int)StatusConstants.Approved)
                            {
                                Pdf = drp.DirectDocumentPrint(Settings.SqlProcDocumentPrint, User.Identity.Name, item);
                                PdfStream.Add(Pdf);
                            }

                        }

                        if (pd.Status ==1)
                        pd.IsDocumentPrinted = true;
                        pd.ObjectState = Model.ObjectState.Modified;
                        db.SalaryHeader.Add(pd);
                        db.SaveChanges();
                    }

                    PdfMerger pm = new PdfMerger();

                    byte[] Merge = pm.MergeFiles(PdfStream);

                    if (Merge != null)
                        return File(Merge, "application/pdf");

                }

                catch (Exception ex)
                {
                    string message = _exception.HandleException(ex);
                    return Json(new { success = "Error", data = message }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = "Success" }, JsonRequestBehavior.AllowGet);

            }
            return Json(new { success = "Error", data = "No Records Selected." }, JsonRequestBehavior.AllowGet);

        }

        protected string GetIPAddress()
        {
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
                    return addresses[0];
                }
            }

            return context.Request.ServerVariables["REMOTE_ADDR"];
        }

        public int LedgerPosting(int SalaryHeaderId)
        {
            SalaryHeader Header = db.SalaryHeader.Find(SalaryHeaderId);

            LedgerHeaderViewModel LedgerHeaderViewModel = new LedgerHeaderViewModel();

            LedgerHeaderViewModel.LedgerHeaderId = Header.LedgerHeaderId ?? 0;
            LedgerHeaderViewModel.DocHeaderId = Header.SalaryHeaderId;
            LedgerHeaderViewModel.DocTypeId = Header.DocTypeId;
            LedgerHeaderViewModel.DocDate = Header.DocDate;
            LedgerHeaderViewModel.DocNo = Header.DocNo;
            LedgerHeaderViewModel.DivisionId = Header.DivisionId;
            LedgerHeaderViewModel.ProcessId = null;
            LedgerHeaderViewModel.SiteId = Header.SiteId;
            LedgerHeaderViewModel.Narration = "";
            LedgerHeaderViewModel.Remark = Header.Remark;
            LedgerHeaderViewModel.CreatedBy = Header.CreatedBy;
            LedgerHeaderViewModel.CreatedDate = DateTime.Now.Date;
            LedgerHeaderViewModel.ModifiedBy = Header.ModifiedBy;
            LedgerHeaderViewModel.ModifiedDate = DateTime.Now.Date;

            IEnumerable<SalaryHeaderCharge> SalaryHeaderHeaderCharges = from H in db.SalaryHeaderCharge where H.HeaderTableId == Header.SalaryHeaderId select H;
            IEnumerable<SalaryLineCharge> SalaryHeaderLineCharges = from L in db.SalaryLineCharge where L.HeaderTableId == Header.SalaryHeaderId select L;

            if (SalaryHeaderLineCharges.Count() > 0)
                new CalculationService(_unitOfWork).LedgerPostingDB(ref LedgerHeaderViewModel, SalaryHeaderHeaderCharges, SalaryHeaderLineCharges, ref db);
            else
            {
                if (LedgerHeaderViewModel.LedgerHeaderId == 0)
                {
                    LedgerHeader LedgerHeader = new LedgerHeader();

                    LedgerHeader.DocHeaderId = LedgerHeaderViewModel.DocHeaderId;
                    LedgerHeader.DocTypeId = LedgerHeaderViewModel.DocTypeId;
                    LedgerHeader.DocDate = LedgerHeaderViewModel.DocDate;
                    LedgerHeader.DocNo = LedgerHeaderViewModel.DocNo;
                    LedgerHeader.DivisionId = LedgerHeaderViewModel.DivisionId;
                    LedgerHeader.SiteId = LedgerHeaderViewModel.SiteId;
                    LedgerHeader.PartyDocNo = LedgerHeaderViewModel.PartyDocNo;
                    LedgerHeader.PartyDocDate = LedgerHeaderViewModel.PartyDocDate;
                    LedgerHeader.Narration = LedgerHeaderViewModel.Narration;
                    LedgerHeader.Remark = LedgerHeaderViewModel.Remark;
                    LedgerHeader.CreatedBy = LedgerHeaderViewModel.CreatedBy;
                    LedgerHeader.ProcessId = LedgerHeaderViewModel.ProcessId;
                    LedgerHeader.CreatedDate = DateTime.Now.Date;
                    LedgerHeader.ModifiedBy = LedgerHeaderViewModel.ModifiedBy;
                    LedgerHeader.ModifiedDate = DateTime.Now.Date;
                    LedgerHeader.ObjectState = Model.ObjectState.Added;
                    db.LedgerHeader.Add(LedgerHeader);
                    //new LedgerHeaderService(_unitOfWork).Create(LedgerHeader);
                }
                else
                {

                    int LedHeadId = LedgerHeaderViewModel.LedgerHeaderId;
                    LedgerHeader LedgerHeader = (from p in db.LedgerHeader
                                                 where p.LedgerHeaderId == LedHeadId
                                                 select p).FirstOrDefault();

                    LedgerHeader.DocHeaderId = LedgerHeaderViewModel.DocHeaderId;
                    LedgerHeader.DocTypeId = LedgerHeaderViewModel.DocTypeId;
                    LedgerHeader.DocDate = LedgerHeaderViewModel.DocDate;
                    LedgerHeader.DocNo = LedgerHeaderViewModel.DocNo;
                    LedgerHeader.DivisionId = LedgerHeaderViewModel.DivisionId;
                    LedgerHeader.ProcessId = LedgerHeaderViewModel.ProcessId;
                    LedgerHeader.SiteId = LedgerHeaderViewModel.SiteId;
                    LedgerHeader.PartyDocNo = LedgerHeaderViewModel.PartyDocNo;
                    LedgerHeader.PartyDocDate = LedgerHeaderViewModel.PartyDocDate;
                    LedgerHeader.Narration = LedgerHeaderViewModel.Narration;
                    LedgerHeader.Remark = LedgerHeaderViewModel.Remark;
                    LedgerHeader.ModifiedBy = LedgerHeaderViewModel.ModifiedBy;
                    LedgerHeader.ModifiedDate = DateTime.Now.Date;
                    LedgerHeader.ObjectState = Model.ObjectState.Modified;
                    db.LedgerHeader.Add(LedgerHeader);
                }

                    IEnumerable<SalaryLine> SalaryLineList = from L in db.SalaryLine where L.SalaryHeaderId == Header.SalaryHeaderId select L;

                foreach (var Line in SalaryLineList)
                {
                    #region LedgerSave
                    Ledger Ledger = new Ledger();

                    LedgerAccount LA = new LedgerAccountService(_unitOfWork).GetLedgerAccountByPersondId(Line.EmployeeId);
                    PersonProcess PP = new PersonProcessService(_unitOfWork).GetPersonProcessIdListByPersonId(Line.EmployeeId).FirstOrDefault();
                    Process P = new ProcessService(_unitOfWork).Find(PP.ProcessId);
                    LedgerAccount LAC = new LedgerAccountService(_unitOfWork).Find(P.AccountId);

                    Ledger.AmtCr = Line.NetPayable;
                    Ledger.ContraLedgerAccountId = LAC.LedgerAccountId;
                    Ledger.LedgerAccountId = LA.LedgerAccountId;
                    Ledger.LedgerHeaderId = LedgerHeaderViewModel.LedgerHeaderId;
                    Ledger.Narration = Header.Remark + Line.Remark;
                    Ledger.ObjectState = Model.ObjectState.Added;
                    Ledger.LedgerId = 1;
                    db.Ledger.Add(Ledger);

                    #endregion

                    #region ContraLedgerSave
                    Ledger ContraLedger = new Ledger();

                    ContraLedger.AmtDr = Line.NetPayable;
                    ContraLedger.LedgerHeaderId = LedgerHeaderViewModel.LedgerHeaderId;
                    ContraLedger.LedgerAccountId = LAC.LedgerAccountId;
                    ContraLedger.ContraLedgerAccountId = LA.LedgerAccountId;
                    ContraLedger.ObjectState = Model.ObjectState.Added;
                    db.Ledger.Add(ContraLedger);
                    #endregion
                }

            }

            Header.LedgerHeaderId = LedgerHeaderViewModel.LedgerHeaderId;


            var LedgerLineList_Temp = (from L in db.SalaryLine
                                       join A in db.LedgerAccount on L.EmployeeId equals A.PersonId into LedgerAccountTable
                                       from LedgerAccountTab in LedgerAccountTable.DefaultIfEmpty()

                                       where L.SalaryHeaderId == Header.SalaryHeaderId
                                       select new
                                       {
                                           SalaryLineId = L.SalaryLineId,
                                           DocTypeId = L.SalaryHeader.DocTypeId,
                                           LedgerAccountId = LedgerAccountTab.LedgerAccountId,
                                           OtherAddition = L.OtherAddition,
                                           OtherDeduction = L.OtherDeduction,
                                           LoanEMI = L.LoanEMI,
                                           Advance = L.Advance,
                                       }).ToList();

            int LedgerId_Running = -1, LedgerAdjId_Running = -1;
            foreach (var LedgerLine_Temp in LedgerLineList_Temp)
            {
                #region "Loan Ledger Posting"
                if ((LedgerLine_Temp.LoanEMI ?? 0) > 0)
                {
                    int LoanAdjustementAccountId = 0;
                    LedgerAccount LoanAdjustementAccount = new LedgerAccountService(_unitOfWork).Find("Loan Adjustement A/C");
                    if (LoanAdjustementAccount != null)
                        LoanAdjustementAccountId = LoanAdjustementAccount.LedgerAccountId;


                    Ledger Ledger = new Ledger();
                    Ledger.LedgerId = LedgerId_Running--;
                    Ledger.AmtDr = 0;
                    Ledger.AmtCr = (decimal)LedgerLine_Temp.LoanEMI;
                    Ledger.ChqNo = null;
                    Ledger.ChqDate = null;
                    Ledger.ContraLedgerAccountId = LoanAdjustementAccountId;
                    Ledger.CostCenterId = null;
                    Ledger.DueDate = null;
                    Ledger.LedgerAccountId = LedgerLine_Temp.LedgerAccountId;
                    Ledger.LedgerHeaderId = (int)Header.LedgerHeaderId;
                    Ledger.LedgerLineId = null;
                    Ledger.ProductUidId = null;
                    Ledger.Narration = Header.Remark;
                    Ledger.ObjectState = Model.ObjectState.Added;
                    db.Ledger.Add(Ledger);


                    Decimal LoanEMI_RunningBalance = LedgerLine_Temp.LoanEMI ?? 0;
                    IEnumerable<LoanLedgerIdList> LoanList = new SalaryWizardService().GetLoanList(LedgerLine_Temp.LedgerAccountId);
                    foreach (var LoanLedger in LoanList)
                    {
                        if (LoanEMI_RunningBalance > 0)
                        {
                            LedgerAdj LedgerAdj = new LedgerAdj();
                            LedgerAdj.LedgerAdjId = LedgerAdjId_Running--;
                            LedgerAdj.CrLedgerId = Ledger.LedgerId;
                            LedgerAdj.DrLedgerId = LoanLedger.LedgerId;

                            if (LoanLedger.Amount >= LoanEMI_RunningBalance)
                                LedgerAdj.Amount = LoanEMI_RunningBalance;
                            else
                                LedgerAdj.Amount = LoanLedger.Amount;

                            LedgerAdj.SiteId = Header.SiteId;
                            LedgerAdj.CreatedDate = DateTime.Now;
                            LedgerAdj.ModifiedDate = DateTime.Now;
                            LedgerAdj.CreatedBy = User.Identity.Name;
                            LedgerAdj.ModifiedBy = User.Identity.Name;
                            LedgerAdj.ObjectState = Model.ObjectState.Added;
                            db.LedgerAdj.Add(LedgerAdj);

                            LoanEMI_RunningBalance = LoanEMI_RunningBalance - LedgerAdj.Amount;
                        }
                    }

                    Ledger ContraLedger = new Ledger();
                    ContraLedger.LedgerId = LedgerId_Running--;
                    ContraLedger.AmtDr = (decimal)LedgerLine_Temp.LoanEMI;
                    ContraLedger.AmtCr = 0;
                    ContraLedger.LedgerHeaderId = (int)Header.LedgerHeaderId;
                    ContraLedger.CostCenterId = null;
                    ContraLedger.LedgerLineId = null;
                    ContraLedger.LedgerAccountId = LoanAdjustementAccountId;
                    ContraLedger.ContraLedgerAccountId = LedgerLine_Temp.LedgerAccountId;
                    ContraLedger.ChqNo = null;
                    ContraLedger.ChqDate = null;
                    ContraLedger.ObjectState = Model.ObjectState.Added;
                    db.Ledger.Add(ContraLedger);
                }

                #endregion

                #region "Advance Ledger Posting"
                if ((LedgerLine_Temp.Advance ?? 0) > 0)
                {
                    int AdvanceAdjustementAccountId = 0;
                    LedgerAccount AdvanceAdjustementAccount = new LedgerAccountService(_unitOfWork).Find("Advance Adjustement A/C");
                    if (AdvanceAdjustementAccount != null)
                        AdvanceAdjustementAccountId = AdvanceAdjustementAccount.LedgerAccountId;


                    Ledger Ledger = new Ledger();
                    Ledger.LedgerId = LedgerId_Running--;
                    Ledger.AmtDr = 0;
                    Ledger.AmtCr = (decimal)LedgerLine_Temp.Advance;
                    Ledger.ChqNo = null;
                    Ledger.ChqDate = null;
                    Ledger.ContraLedgerAccountId = AdvanceAdjustementAccountId;
                    Ledger.CostCenterId = null;
                    Ledger.DueDate = null;
                    Ledger.LedgerAccountId = LedgerLine_Temp.LedgerAccountId;
                    Ledger.LedgerHeaderId = (int)Header.LedgerHeaderId;
                    Ledger.LedgerLineId = null;
                    Ledger.ProductUidId = null;
                    Ledger.Narration = Header.Remark;
                    Ledger.ObjectState = Model.ObjectState.Added;
                    db.Ledger.Add(Ledger);

                    Decimal Advance_RunningBalance = LedgerLine_Temp.Advance ?? 0;
                    IEnumerable<AdvanceLedgerIdList> AdvanceList = new SalaryWizardService().GetAdvanceList(LedgerLine_Temp.LedgerAccountId);
                    foreach (var AdvanceLedger in AdvanceList)
                    {
                        if (Advance_RunningBalance > 0)
                        {
                            LedgerAdj LedgerAdj = new LedgerAdj();
                            LedgerAdj.LedgerAdjId = LedgerAdjId_Running--;
                            LedgerAdj.CrLedgerId = Ledger.LedgerId;
                            LedgerAdj.DrLedgerId = AdvanceLedger.LedgerId;

                            if (AdvanceLedger.Amount >= Advance_RunningBalance)
                                LedgerAdj.Amount = Advance_RunningBalance;
                            else
                                LedgerAdj.Amount = AdvanceLedger.Amount;

                            LedgerAdj.SiteId = Header.SiteId;
                            LedgerAdj.CreatedDate = DateTime.Now;
                            LedgerAdj.ModifiedDate = DateTime.Now;
                            LedgerAdj.CreatedBy = User.Identity.Name;
                            LedgerAdj.ModifiedBy = User.Identity.Name;
                            LedgerAdj.ObjectState = Model.ObjectState.Added;
                            db.LedgerAdj.Add(LedgerAdj);

                            Advance_RunningBalance = Advance_RunningBalance - LedgerAdj.Amount;
                        }
                    }

                    Ledger ContraLedger = new Ledger();
                    ContraLedger.LedgerId = LedgerId_Running--;
                    ContraLedger.AmtDr = (decimal)LedgerLine_Temp.Advance;
                    ContraLedger.AmtCr = 0;
                    ContraLedger.LedgerHeaderId = (int)Header.LedgerHeaderId;
                    ContraLedger.CostCenterId = null;
                    ContraLedger.LedgerLineId = null;
                    ContraLedger.LedgerAccountId = AdvanceAdjustementAccountId;
                    ContraLedger.ContraLedgerAccountId = LedgerLine_Temp.LedgerAccountId;
                    ContraLedger.ChqNo = null;
                    ContraLedger.ChqDate = null;
                    ContraLedger.ObjectState = Model.ObjectState.Added;
                    db.Ledger.Add(ContraLedger);
                }
                #endregion



                #region "Addition Ledger Posting"
                if ((LedgerLine_Temp.OtherAddition ?? 0) > 0)
                {
                    int AdditionAccountId = 0;
                    LedgerAccount AdditionAdjustementAccount = new LedgerAccountService(_unitOfWork).Find("Addition A/C");
                    if (AdditionAdjustementAccount != null)
                        AdditionAccountId = AdditionAdjustementAccount.LedgerAccountId;

                    Ledger Ledger = new Ledger();
                    Ledger.LedgerId = LedgerId_Running--;
                    Ledger.AmtDr = 0;
                    Ledger.AmtCr = (decimal)LedgerLine_Temp.OtherAddition;
                    Ledger.ChqNo = null;
                    Ledger.ChqDate = null;
                    Ledger.ContraLedgerAccountId = AdditionAccountId;
                    Ledger.CostCenterId = null;
                    Ledger.DueDate = null;
                    Ledger.LedgerAccountId = LedgerLine_Temp.LedgerAccountId;
                    Ledger.LedgerHeaderId = (int)Header.LedgerHeaderId;
                    Ledger.LedgerLineId = null;
                    Ledger.ProductUidId = null;
                    Ledger.Narration = Header.Remark;
                    Ledger.ObjectState = Model.ObjectState.Added;
                    db.Ledger.Add(Ledger);

                    Ledger ContraLedger = new Ledger();
                    ContraLedger.LedgerId = LedgerId_Running--;
                    ContraLedger.AmtDr = (decimal)LedgerLine_Temp.OtherAddition;
                    ContraLedger.AmtCr = 0;
                    ContraLedger.LedgerHeaderId = (int)Header.LedgerHeaderId;
                    ContraLedger.CostCenterId = null;
                    ContraLedger.LedgerLineId = null;
                    ContraLedger.LedgerAccountId = AdditionAccountId;
                    ContraLedger.ContraLedgerAccountId = LedgerLine_Temp.LedgerAccountId;
                    ContraLedger.ChqNo = null;
                    ContraLedger.ChqDate = null;
                    ContraLedger.ObjectState = Model.ObjectState.Added;
                    db.Ledger.Add(ContraLedger);
                }
                #endregion


                #region "Deduction Ledger Posting"
                if ((LedgerLine_Temp.OtherDeduction ?? 0) > 0)
                {
                    int DeductionAccountId = 0;
                    LedgerAccount DeductionAdjustementAccount = new LedgerAccountService(_unitOfWork).Find("Deduction A/C");
                    if (DeductionAdjustementAccount != null)
                        DeductionAccountId = DeductionAdjustementAccount.LedgerAccountId;

                    Ledger Ledger = new Ledger();
                    Ledger.LedgerId = LedgerId_Running--;
                    Ledger.AmtDr = 0;
                    Ledger.AmtCr = (decimal)LedgerLine_Temp.OtherDeduction;
                    Ledger.ChqNo = null;
                    Ledger.ChqDate = null;
                    Ledger.ContraLedgerAccountId = DeductionAccountId;
                    Ledger.CostCenterId = null;
                    Ledger.DueDate = null;
                    Ledger.LedgerAccountId = LedgerLine_Temp.LedgerAccountId;
                    Ledger.LedgerHeaderId = (int)Header.LedgerHeaderId;
                    Ledger.LedgerLineId = null;
                    Ledger.ProductUidId = null;
                    Ledger.Narration = Header.Remark;
                    Ledger.ObjectState = Model.ObjectState.Added;
                    db.Ledger.Add(Ledger);

                    Ledger ContraLedger = new Ledger();
                    ContraLedger.LedgerId = LedgerId_Running--;
                    ContraLedger.AmtDr = (decimal)LedgerLine_Temp.OtherDeduction;
                    ContraLedger.AmtCr = 0;
                    ContraLedger.LedgerHeaderId = (int)Header.LedgerHeaderId;
                    ContraLedger.CostCenterId = null;
                    ContraLedger.LedgerLineId = null;
                    ContraLedger.LedgerAccountId = DeductionAccountId;
                    ContraLedger.ContraLedgerAccountId = LedgerLine_Temp.LedgerAccountId;
                    ContraLedger.ChqNo = null;
                    ContraLedger.ChqDate = null;
                    ContraLedger.ObjectState = Model.ObjectState.Added;
                    db.Ledger.Add(ContraLedger);
                }
                #endregion

            }


            return LedgerHeaderViewModel.LedgerHeaderId;
        }
    }
}
