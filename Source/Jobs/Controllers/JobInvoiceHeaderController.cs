using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Model.Models;
using Data.Models;
using Data.Infrastructure;
using Presentation.ViewModels;
using Presentation;
using Core.Common;
using Model.ViewModel;
using Jobs.Helpers;
using AutoMapper;
using System.Configuration;
using Service;
using System.Xml.Linq;
using CustomEventArgs;
using DocumentEvents;
using JobInvoiceDocumentEvents;
using Reports.Reports;
using Reports.Controllers;
using Model.ViewModels;

namespace Jobs.Controllers
{
    [Authorize]
    public class JobInvoiceHeaderController : System.Web.Mvc.Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        List<string> UserRoles = new List<string>();
        ActiivtyLogViewModel LogVm = new ActiivtyLogViewModel();

        bool TimePlanValidation = true;
        private bool EventException = false;
        string ExceptionMsg = "";
        bool Continue = true;

        IJobInvoiceHeaderService _JobInvoiceHeaderService;
        IUnitOfWork _unitOfWork;
        IExceptionHandlingService _exception;
        public JobInvoiceHeaderController(IJobInvoiceHeaderService JobInvoiceHeaderService, IUnitOfWork unitOfWork, IExceptionHandlingService exec)
        {
            _JobInvoiceHeaderService = JobInvoiceHeaderService;
            _unitOfWork = unitOfWork;
            _exception = exec;
            if (!JobInvoiceEvents.Initialized)
            {
                JobInvoiceEvents Obj = new JobInvoiceEvents();
            }

            UserRoles = (List<string>)System.Web.HttpContext.Current.Session["Roles"];

            //Log Initialization
            LogVm.SessionId = 0;
            LogVm.ControllerName = System.Web.HttpContext.Current.Request.RequestContext.RouteData.GetRequiredString("controller");
            LogVm.ActionName = System.Web.HttpContext.Current.Request.RequestContext.RouteData.GetRequiredString("action");
            LogVm.User = System.Web.HttpContext.Current.Request.RequestContext.HttpContext.User.Identity.Name;
        }

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

        void PrepareViewBag(int id)
        {
            ViewBag.Name = new DocumentTypeService(_unitOfWork).Find(id).DocumentTypeName;
            ViewBag.id = id;
            var DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            var SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            var settings = new JobInvoiceSettingsService(_unitOfWork).GetJobInvoiceSettingsForDocument(id, DivisionId, SiteId);
            ViewBag.AdminSetting = UserRoles.Contains("Admin").ToString();
            if (settings != null)
            {
                ViewBag.WizardId = settings.WizardMenuId;
                ViewBag.ImportMenuId = settings.ImportMenuId;
                ViewBag.SqlProcDocumentPrint = settings.SqlProcDocumentPrint;
                ViewBag.ExportMenuId = settings.ExportMenuId;
            }

        }

        // GET: /JobInvoiceHeaderMaster/

        public ActionResult Index(int id, string IndexType)//DocumentTypeId
        {
            if (IndexType == "PTS")
            {
                return RedirectToAction("Index_PendingToSubmit", new { id });
            }
            else if (IndexType == "PTR")
            {
                return RedirectToAction("Index_PendingToReview", new { id });
            }
            int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];

            var settings = new JobInvoiceSettingsService(_unitOfWork).GetJobInvoiceSettingsForDocument(id, DivisionId, SiteId);
            if (settings != null)
                ViewBag.HeaderJobWorker = settings.isVisibleHeaderJobWorker;

            IQueryable<JobInvoiceHeaderViewModel> JobInvoiceHeader = _JobInvoiceHeaderService.GetJobInvoiceHeaderList(id, User.Identity.Name);
            PrepareViewBag(id);
            ViewBag.PendingToSubmit = PendingToSubmitCount(id);
            ViewBag.PendingToReview = PendingToReviewCount(id);
            ViewBag.IndexStatus = "All";
            return View(JobInvoiceHeader);
        }


        public ActionResult Index_PendingToSubmit(int id)
        {

            int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];

            var settings = new JobInvoiceSettingsService(_unitOfWork).GetJobInvoiceSettingsForDocument(id, DivisionId, SiteId);
            if (settings != null)
                ViewBag.HeaderJobWorker = settings.isVisibleHeaderJobWorker;


            IQueryable<JobInvoiceHeaderViewModel> p = _JobInvoiceHeaderService.GetJobInvoiceHeaderListPendingToSubmit(id, User.Identity.Name);

            PrepareViewBag(id);
            ViewBag.PendingToSubmit = PendingToSubmitCount(id);
            ViewBag.PendingToReview = PendingToReviewCount(id);
            ViewBag.IndexStatus = "PTS";
            return View("Index", p);
        }

        public ActionResult Index_PendingToReview(int id)
        {

            int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];

            var settings = new JobInvoiceSettingsService(_unitOfWork).GetJobInvoiceSettingsForDocument(id, DivisionId, SiteId);
            if (settings != null)
                ViewBag.HeaderJobWorker = settings.isVisibleHeaderJobWorker;


            IQueryable<JobInvoiceHeaderViewModel> p = _JobInvoiceHeaderService.GetJobInvoiceHeaderListPendingToReview(id, User.Identity.Name);
            PrepareViewBag(id);
            ViewBag.PendingToSubmit = PendingToSubmitCount(id);
            ViewBag.PendingToReview = PendingToReviewCount(id);
            ViewBag.IndexStatus = "PTR";
            return View("Index", p);
        }


        // GET: /JobInvoiceHeaderMaster/Create

        public ActionResult Create(int id)//DocumentTypeId
        {
            PrepareViewBag(id);
            JobInvoiceHeaderViewModel vm = new JobInvoiceHeaderViewModel();
            vm.DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            vm.SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            vm.CreatedDate = DateTime.Now;

            List<DocumentTypeHeaderAttributeViewModel> tem = new DocumentTypeService(_unitOfWork).GetDocumentTypeHeaderAttribute(id).ToList();
            vm.DocumentTypeHeaderAttributes = tem;

            //Getting Settings
            var settings = new JobInvoiceSettingsService(_unitOfWork).GetJobInvoiceSettingsForDocument(id, vm.DivisionId, vm.SiteId);

            if (settings == null && UserRoles.Contains("SysAdmin"))
            {
                return RedirectToAction("Create", "JobInvoiceSettings", new { id = id }).Warning("Please create job order settings");
            }
            else if (settings == null && !UserRoles.Contains("SysAdmin"))
            {
                return View("~/Views/Shared/InValidSettings.cshtml");
            }

            vm.JobInvoiceSettings = Mapper.Map<JobInvoiceSettings, JobInvoiceSettingsViewModel>(settings);

            if (settings != null)
            {
                if (settings.CalculationId != null)
                {
                    var CalculationHeaderLedgerAccount = (from H in db.CalculationHeaderLedgerAccount where H.CalculationId == settings.CalculationId && H.DocTypeId == id && H.SiteId == vm.SiteId && H.DivisionId == vm.DivisionId select H).FirstOrDefault();
                    var CalculationLineLedgerAccount = (from H in db.CalculationLineLedgerAccount where H.CalculationId == settings.CalculationId && H.DocTypeId == id && H.SiteId == vm.SiteId && H.DivisionId == vm.DivisionId select H).FirstOrDefault();

                    if (CalculationHeaderLedgerAccount == null && CalculationLineLedgerAccount == null && UserRoles.Contains("SysAdmin"))
                    {
                        return RedirectToAction("Create", "JobInvoiceSettings", new { id = id }).Warning("Ledger posting settings is not defined for current site and division.");
                    }
                    else if (CalculationHeaderLedgerAccount == null && CalculationLineLedgerAccount == null && !UserRoles.Contains("SysAdmin"))
                    {
                        return View("~/Views/Shared/InValidSettings.cshtml").Warning("Ledger posting settings is not defined for current site and division.");
                    }
                }
            }


            if (settings != null)
            {
                vm.SalesTaxGroupPersonId = settings.SalesTaxGroupPersonId;
            }

            vm.ProcessId = settings.ProcessId;
            vm.DocDate = DateTime.Now;
            vm.DocTypeId = id;
            vm.DocNo = new DocumentTypeService(_unitOfWork).FGetNewDocNo("DocNo", ConfigurationManager.AppSettings["DataBaseSchema"] + ".JobInvoiceHeaders", vm.DocTypeId, vm.DocDate, vm.DivisionId, vm.SiteId);
            vm.DocumentTypeSettings = new DocumentTypeSettingsService(_unitOfWork).GetDocumentTypeSettingsForDocument(vm.DocTypeId);
            ViewBag.Mode = "Add";
            return View("Create", vm);
        }

        // POST: /ProductMaster/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Post(JobInvoiceHeaderViewModel vm)
        {
            JobInvoiceHeader pt = AutoMapper.Mapper.Map<JobInvoiceHeaderViewModel, JobInvoiceHeader>(vm);

            if (vm.JobInvoiceSettings.isVisibleHeaderJobWorker && !vm.JobWorkerId.HasValue)
                ModelState.AddModelError("", "The JobWorker field is required.");

            SiteDivisionSettings SiteDivisionSettings = new SiteDivisionSettingsService(_unitOfWork).GetSiteDivisionSettings(vm.SiteId, vm.DivisionId, vm.DocDate);
            if (SiteDivisionSettings != null)
            {
                if (SiteDivisionSettings.IsApplicableGST == true)
                {
                    if (vm.SalesTaxGroupPersonId == 0 || vm.SalesTaxGroupPersonId == null)
                    {
                        ModelState.AddModelError("", "Sales Tax Group Person is not defined for party, it is required.");
                    }
                }
            }

            #region BeforeSave
            bool BeforeSave = true;

            try
            {

                if (vm.JobInvoiceHeaderId <= 0)
                    BeforeSave = JobInvoiceDocEvents.beforeHeaderSaveEvent(this, new JobEventArgs(vm.JobInvoiceHeaderId, EventModeConstants.Add), ref db);
                else
                    BeforeSave = JobInvoiceDocEvents.beforeHeaderSaveEvent(this, new JobEventArgs(vm.JobInvoiceHeaderId, EventModeConstants.Edit), ref db);


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

                if (vm.JobInvoiceHeaderId <= 0)
                    TimePlanValidation = DocumentValidation.ValidateDocument(Mapper.Map<DocumentUniqueId>(vm), DocumentTimePlanTypeConstants.Create, User.Identity.Name, out ExceptionMsg, out Continue);
                else
                    TimePlanValidation = DocumentValidation.ValidateDocument(Mapper.Map<DocumentUniqueId>(vm), DocumentTimePlanTypeConstants.Modify, User.Identity.Name, out ExceptionMsg, out Continue);

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
                #region CreateRecord
                if (vm.JobInvoiceHeaderId <= 0)
                {
                    pt.DivisionId = vm.DivisionId;
                    pt.SiteId = vm.SiteId;
                    pt.CreatedDate = DateTime.Now;
                    pt.ModifiedDate = DateTime.Now;
                    pt.CreatedBy = User.Identity.Name;
                    pt.ModifiedBy = User.Identity.Name;
                    pt.ObjectState = Model.ObjectState.Added;
                    //_JobInvoiceHeaderService.Create(pt);
                    db.JobInvoiceHeader.Add(pt);


                    if (vm.DocumentTypeHeaderAttributes != null)
                    {
                        foreach (var Attributes in vm.DocumentTypeHeaderAttributes)
                        {
                            JobInvoiceHeaderAttributes JobInvoiceHeaderAttribute = (from A in db.JobInvoiceHeaderAttributes
                                                                                    where A.HeaderTableId == pt.JobInvoiceHeaderId
                                                                                && A.DocumentTypeHeaderAttributeId == Attributes.DocumentTypeHeaderAttributeId
                                                                                select A).FirstOrDefault();

                            if (JobInvoiceHeaderAttribute != null)
                            {
                                JobInvoiceHeaderAttribute.Value = Attributes.Value;
                                JobInvoiceHeaderAttribute.ObjectState = Model.ObjectState.Modified;
                                db.JobInvoiceHeaderAttributes.Add(JobInvoiceHeaderAttribute);
                            }
                            else
                            {
                                JobInvoiceHeaderAttributes HeaderAttribute = new JobInvoiceHeaderAttributes()
                                {
                                    HeaderTableId = pt.JobInvoiceHeaderId,
                                    Value = Attributes.Value,
                                    DocumentTypeHeaderAttributeId = Attributes.DocumentTypeHeaderAttributeId,
                                };
                                HeaderAttribute.ObjectState = Model.ObjectState.Added;
                                db.JobInvoiceHeaderAttributes.Add(HeaderAttribute);
                            }
                        }
                    }

                    try
                    {
                        JobInvoiceDocEvents.onHeaderSaveEvent(this, new JobEventArgs(pt.JobInvoiceHeaderId, EventModeConstants.Add), ref db);
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
                        TempData["CSEXC"] += message;
                        PrepareViewBag(vm.DocTypeId);
                        ViewBag.Mode = "Add";
                        return View("Create", vm);
                    }

                    try
                    {
                        JobInvoiceDocEvents.afterHeaderSaveEvent(this, new JobEventArgs(pt.JobInvoiceHeaderId, EventModeConstants.Add), ref db);
                    }
                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        TempData["CSEXC"] += message;
                    }

                    LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                    {
                        DocTypeId = pt.DocTypeId,
                        DocId = pt.JobInvoiceHeaderId,
                        ActivityType = (int)ActivityTypeContants.Added,
                        DocNo = pt.DocNo,
                        DocDate = pt.DocDate,
                        DocStatus = pt.Status,
                    }));

                    return RedirectToAction("Modify", new { id = pt.JobInvoiceHeaderId }).Success("Data saved successfully");
                }
                #endregion

                #region EditRecord
                else
                {
                    List<LogTypeViewModel> LogList = new List<LogTypeViewModel>();

                    JobInvoiceHeader temp = _JobInvoiceHeaderService.Find(pt.JobInvoiceHeaderId);


                    JobInvoiceHeader ExRec = new JobInvoiceHeader();
                    ExRec = Mapper.Map<JobInvoiceHeader>(temp);

                    int status = temp.Status;

                    if (temp.Status != (int)StatusConstants.Drafted && temp.Status != (int)StatusConstants.Import)
                        temp.Status = (int)StatusConstants.Modified;

                    temp.Remark = pt.Remark;
                    temp.DocNo = pt.DocNo;
                    temp.DocDate = pt.DocDate;
                    temp.SalesTaxGroupPersonId = pt.SalesTaxGroupPersonId;
                    temp.JobWorkerId = pt.JobWorkerId;
                    temp.JobWorkerDocNo = pt.JobWorkerDocNo;
                    temp.JobWorkerDocDate = pt.JobWorkerDocDate;
                    temp.ProcessId = pt.ProcessId;
                    temp.ModifiedDate = DateTime.Now;
                    temp.ModifiedBy = User.Identity.Name;
                    temp.ObjectState = Model.ObjectState.Modified;
                    //_JobInvoiceHeaderService.Update(temp);
                    db.JobInvoiceHeader.Add(temp);

                    LogList.Add(new LogTypeViewModel
                    {
                        ExObj = ExRec,
                        Obj = temp,
                    });

                    if (vm.DocumentTypeHeaderAttributes != null)
                    {
                        foreach (var Attributes in vm.DocumentTypeHeaderAttributes)
                        {

                            JobInvoiceHeaderAttributes JobInvoiceHeaderAttribute = (from A in db.JobInvoiceHeaderAttributes
                                                                                    where A.HeaderTableId == temp.JobInvoiceHeaderId
                                                                                && A.DocumentTypeHeaderAttributeId == Attributes.DocumentTypeHeaderAttributeId
                                                                                select A).FirstOrDefault();

                            if (JobInvoiceHeaderAttribute != null)
                            {
                                JobInvoiceHeaderAttribute.Value = Attributes.Value;
                                JobInvoiceHeaderAttribute.ObjectState = Model.ObjectState.Modified;
                                db.JobInvoiceHeaderAttributes.Add(JobInvoiceHeaderAttribute);
                            }
                            else
                            {
                                JobInvoiceHeaderAttributes HeaderAttribute = new JobInvoiceHeaderAttributes()
                                {
                                    Value = Attributes.Value,
                                    HeaderTableId = temp.JobInvoiceHeaderId,
                                    DocumentTypeHeaderAttributeId = Attributes.DocumentTypeHeaderAttributeId,
                                };
                                HeaderAttribute.ObjectState = Model.ObjectState.Added;
                                db.JobInvoiceHeaderAttributes.Add(HeaderAttribute);
                            }
                        }
                    }


                    XElement Modifications = new ModificationsCheckService().CheckChanges(LogList);

                    try
                    {
                        JobInvoiceDocEvents.onHeaderSaveEvent(this, new JobEventArgs(temp.JobInvoiceHeaderId, EventModeConstants.Edit), ref db);
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
                        TempData["CSEXC"] += message;
                        PrepareViewBag(vm.DocTypeId);
                        ViewBag.Mode = "Edit";
                        return View("Create", pt);
                    }

                    try
                    {
                        JobInvoiceDocEvents.afterHeaderSaveEvent(this, new JobEventArgs(temp.JobInvoiceHeaderId, EventModeConstants.Edit), ref db);
                    }
                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        TempData["CSEXC"] += message;
                    }

                    LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                    {
                        DocTypeId = temp.DocTypeId,
                        DocId = temp.JobInvoiceHeaderId,
                        ActivityType = (int)ActivityTypeContants.Modified,
                        DocNo = pt.DocNo,
                        xEModifications = Modifications,
                        DocDate = pt.DocDate,
                        DocStatus = pt.Status,
                    }));

                    return RedirectToAction("Index", new { id = vm.DocTypeId }).Success("Data saved successfully");
                }
                #endregion
            }
            PrepareViewBag(vm.DocTypeId);
            ViewBag.Mode = "Add";
            return View("Create", vm);
        }


        [HttpGet]
        public ActionResult Modify(int id, string IndexType)
        {
            JobInvoiceHeader header = _JobInvoiceHeaderService.Find(id);
            if (header.Status == (int)StatusConstants.Drafted || header.Status == (int)StatusConstants.Import)
                return Edit(id, IndexType);
            else
                return HttpNotFound();
        }

        [HttpGet]
        public ActionResult ModifyAfter_Submit(int id, string IndexType)
        {
            JobInvoiceHeader header = _JobInvoiceHeaderService.Find(id);
            if (header.Status == (int)StatusConstants.Submitted || header.Status == (int)StatusConstants.Modified)
                return Edit(id, IndexType);
            else
                return HttpNotFound();
        }

        [HttpGet]
        public ActionResult ModifyAfter_Approve(int id, string IndexType)
        {
            JobInvoiceHeader header = _JobInvoiceHeaderService.Find(id);
            if (header.Status == (int)StatusConstants.Approved)
                return Edit(id, IndexType);
            else
                return HttpNotFound();
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            JobInvoiceHeader header = _JobInvoiceHeaderService.Find(id);
            if (header.Status == (int)StatusConstants.Drafted || header.Status == (int)StatusConstants.Import)
                return Remove(id);
            else
                return HttpNotFound();
        }

        [HttpGet]
        public ActionResult DeleteAfter_Submit(int id)
        {
            JobInvoiceHeader header = _JobInvoiceHeaderService.Find(id);
            if (header.Status == (int)StatusConstants.Submitted || header.Status == (int)StatusConstants.Modified)
                return Remove(id);
            else
                return HttpNotFound();
        }

        [HttpGet]
        public ActionResult DeleteAfter_Approve(int id)
        {
            JobInvoiceHeader header = _JobInvoiceHeaderService.Find(id);
            if (header.Status == (int)StatusConstants.Approved)
                return Remove(id);
            else
                return HttpNotFound();
        }

        [HttpGet]
        public ActionResult DetailInformation(int id, int? DocLineId)
        {
            return RedirectToAction("Detail", new { id = id, transactionType = "detail", DocLineId = DocLineId });
        }


        // GET: /ProductMaster/Edit/5

        private ActionResult Edit(int id, string IndexType)
        {
            ViewBag.IndexStatus = IndexType;

            JobInvoiceHeaderViewModel pt = _JobInvoiceHeaderService.GetJobInvoiceHeader(id);

            #region DocTypeTimeLineValidation
            try
            {

                TimePlanValidation = DocumentValidation.ValidateDocument(Mapper.Map<DocumentUniqueId>(pt), DocumentTimePlanTypeConstants.Modify, User.Identity.Name, out ExceptionMsg, out Continue);

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
            { return RedirectToAction("DetailInformation", new { id = id, IndexType = IndexType }); }
            //Job Order Settings
            var settings = new JobInvoiceSettingsService(_unitOfWork).GetJobInvoiceSettingsForDocument(pt.DocTypeId, pt.DivisionId, pt.SiteId);

            if (settings == null && UserRoles.Contains("SysAdmin"))
            {
                return RedirectToAction("Create", "JobInvoiceSettings", new { id = pt.DocTypeId }).Warning("Please create job Invoice settings");
            }
            else if (settings == null && !UserRoles.Contains("SysAdmin"))
            {
                return View("~/Views/Shared/InValidSettings.cshtml");
            }

            pt.JobInvoiceSettings = Mapper.Map<JobInvoiceSettings, JobInvoiceSettingsViewModel>(settings);
            pt.DocumentTypeSettings = new DocumentTypeSettingsService(_unitOfWork).GetDocumentTypeSettingsForDocument(pt.DocTypeId);

            List<DocumentTypeHeaderAttributeViewModel> tem = _JobInvoiceHeaderService.GetDocumentHeaderAttribute(id).ToList();
            pt.DocumentTypeHeaderAttributes = tem;

            PrepareViewBag(pt.DocTypeId);
            if (pt == null)
            {
                return HttpNotFound();
            }
            ViewBag.Mode = "Edit";
            if ((System.Web.HttpContext.Current.Request.UrlReferrer.PathAndQuery).Contains("Create"))
                LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                {
                    DocTypeId = pt.DocTypeId,
                    DocId = pt.JobInvoiceHeaderId,
                    ActivityType = (int)ActivityTypeContants.Detail,
                    DocNo = pt.DocNo,
                    DocDate = pt.DocDate,
                    DocStatus = pt.Status,
                }));

            return View("Create", pt);
        }


        [Authorize]
        public ActionResult Detail(int id, string IndexType, string transactionType, int? DocLineId)
        {
            if (DocLineId.HasValue)
                ViewBag.DocLineId = DocLineId;

            ViewBag.transactionType = transactionType;
            ViewBag.IndexStatus = IndexType;

            JobInvoiceHeaderViewModel pt = _JobInvoiceHeaderService.GetJobInvoiceHeader(id);

            //Getting Settings
            var settings = new JobInvoiceSettingsService(_unitOfWork).GetJobInvoiceSettingsForDocument(pt.DocTypeId, pt.DivisionId, pt.SiteId);

            pt.JobInvoiceSettings = Mapper.Map<JobInvoiceSettings, JobInvoiceSettingsViewModel>(settings);
            pt.DocumentTypeSettings = new DocumentTypeSettingsService(_unitOfWork).GetDocumentTypeSettingsForDocument(pt.DocTypeId);

            PrepareViewBag(pt.DocTypeId);
            if (pt == null)
            {
                return HttpNotFound();
            }
            if (String.IsNullOrEmpty(transactionType) || transactionType == "detail")
                LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                {
                    DocTypeId = pt.DocTypeId,
                    DocId = pt.JobInvoiceHeaderId,
                    ActivityType = (int)ActivityTypeContants.Detail,
                    DocNo = pt.DocNo,
                    DocDate = pt.DocDate,
                    DocStatus = pt.Status,
                }));

            return View("Create", pt);
        }




        public ActionResult Submit(int id, string IndexType, string TransactionType)
        {
            #region DocTypeTimeLineValidation

            bool TimePlanValidation = true;
            string ExceptionMsg = "";
            bool Continue = true;

            JobInvoiceHeader s = db.JobInvoiceHeader.Find(id);
            try
            {
                TimePlanValidation = Submitvalidation(id, out ExceptionMsg);
                TempData["CSEXC"] += ExceptionMsg;
            }
            catch (Exception ex)
            {
                string message = _exception.HandleException(ex);
                TempData["CSEXC"] += message;
                TimePlanValidation = false;
            }

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
        public ActionResult Submitted(int Id, string IndexType, string UserRemark, string IsContinue)
        {

            bool BeforeSave = true;
            try
            {
                BeforeSave = JobInvoiceDocEvents.beforeHeaderSubmitEvent(this, new JobEventArgs(Id), ref db);
            }
            catch (Exception ex)
            {
                string message = _exception.HandleException(ex);
                TempData["CSEXC"] += message;
                EventException = true;
            }

            if (!BeforeSave)
                TempData["CSEXC"] += "Falied validation before submit.";

            if (ModelState.IsValid && BeforeSave && !EventException)
            {

                JobInvoiceHeader pd = new JobInvoiceHeaderService(_unitOfWork).Find(Id);
                int ActivityType;
                if (User.Identity.Name == pd.ModifiedBy || UserRoles.Contains("Admin"))
                {

                    pd.Status = (int)StatusConstants.Submitted;
                    ActivityType = (int)ActivityTypeContants.Submitted;

                    var settings = new JobInvoiceSettingsService(_unitOfWork).GetJobInvoiceSettingsForDocument(pd.DocTypeId, pd.DivisionId, pd.SiteId);


                    #region "Ledger Posting"

                    LedgerHeaderViewModel LedgerHeaderViewModel = new LedgerHeaderViewModel();

                    LedgerHeaderViewModel.LedgerHeaderId = pd.LedgerHeaderId ?? 0;
                    LedgerHeaderViewModel.DocHeaderId = pd.JobInvoiceHeaderId;
                    LedgerHeaderViewModel.DocTypeId = pd.DocTypeId;
                    LedgerHeaderViewModel.DocDate = pd.DocDate;
                    LedgerHeaderViewModel.DocNo = pd.DocNo;
                    LedgerHeaderViewModel.DivisionId = pd.DivisionId;
                    LedgerHeaderViewModel.ProcessId = pd.ProcessId;
                    LedgerHeaderViewModel.SiteId = pd.SiteId;
                    LedgerHeaderViewModel.ProcessId = pd.ProcessId;
                    LedgerHeaderViewModel.Narration = "";
                    LedgerHeaderViewModel.Remark = pd.Remark;
                    LedgerHeaderViewModel.CreatedBy = pd.CreatedBy;
                    LedgerHeaderViewModel.CreatedDate = DateTime.Now.Date;
                    LedgerHeaderViewModel.ModifiedBy = pd.ModifiedBy;
                    LedgerHeaderViewModel.ModifiedDate = DateTime.Now.Date;

                    IEnumerable<JobInvoiceHeaderCharge> JobInvoiceHeaderCharges = from H in db.JobInvoiceHeaderCharges where H.HeaderTableId == Id select H;
                    IEnumerable<JobInvoiceLineCharge> JobInvoiceLineCharges = from L in db.JobInvoiceLineCharge where L.HeaderTableId == Id select L;

                    try
                    {
                        if (settings.isLedgerPostingLineWise == true)
                        {
                            LedgerPostingDBLineWise(ref LedgerHeaderViewModel, JobInvoiceHeaderCharges, JobInvoiceLineCharges, ref db);
                        }
                        else
                        {
                            new CalculationService(_unitOfWork).LedgerPostingDB(ref LedgerHeaderViewModel, JobInvoiceHeaderCharges, JobInvoiceLineCharges, ref db);
                        }

                    }
                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        ModelState.AddModelError("", message);
                        return RedirectToAction("Detail", new { id = Id, IndexType = IndexType, transactionType = "submit" });
                    }


                    if (pd.LedgerHeaderId == null)
                    {
                        pd.LedgerHeaderId = LedgerHeaderViewModel.LedgerHeaderId;
                    }

                    #endregion

                    pd.ReviewBy = null;
                    pd.ObjectState = Model.ObjectState.Modified;
                    db.JobInvoiceHeader.Add(pd);

                    try
                    {
                        JobInvoiceDocEvents.onHeaderSubmitEvent(this, new JobEventArgs(Id), ref db);
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
                        TempData["CSEXC"] += message;
                        return RedirectToAction("Index", new { id = pd.DocTypeId });
                    }

                    try
                    {
                        JobInvoiceDocEvents.afterHeaderSubmitEvent(this, new JobEventArgs(Id), ref db);
                    }
                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        TempData["CSEXC"] += message;

                    }

                    LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                    {
                        DocTypeId = pd.DocTypeId,
                        DocId = pd.JobInvoiceHeaderId,
                        ActivityType = ActivityType,
                        UserRemark = UserRemark,
                        DocNo = pd.DocNo,
                        DocDate = pd.DocDate,
                        DocStatus = pd.Status,
                    }));

                    if (!string.IsNullOrEmpty(IsContinue) && IsContinue == "True")
                    {

                        int nextId = new NextPrevIdService(_unitOfWork).GetNextPrevId(Id, pd.DocTypeId, User.Identity.Name, ForActionConstants.PendingToSubmit, "Web.JobInvoiceHeaders", "JobInvoiceHeaderId", PrevNextConstants.Next);

                        if (nextId == 0)
                        {


                            var PendingtoSubmitCount = PendingToSubmitCount(pd.DocTypeId);
                            if (PendingtoSubmitCount > 0)
                                return RedirectToAction("Index_PendingToSubmit", new { id = pd.DocTypeId, IndexType = IndexType });
                            else
                                return RedirectToAction("Index", new { id = pd.DocTypeId, IndexType = IndexType });

                        }

                        return RedirectToAction("Detail", new { id = nextId, TransactionType = "submitContinue", IndexType = IndexType }).Success("Invoice " + pd.DocNo + " submitted successfully.");

                    }

                    else
                        return RedirectToAction("Index", new { id = pd.DocTypeId, IndexType = IndexType });
                }
                else
                    return RedirectToAction("Index", new { id = pd.DocTypeId, IndexType = IndexType }).Warning("Record can be submitted by user " + pd.ModifiedBy + " only.");
            }

            return View();
        }



        public ActionResult Review(int id, string IndexType, string TransactionType)
        {
            return RedirectToAction("Detail", new { id = id, IndexType = IndexType, transactionType = string.IsNullOrEmpty(TransactionType) ? "review" : TransactionType });
        }


        [HttpPost, ActionName("Detail")]
        [MultipleButton(Name = "Command", Argument = "Review")]
        public ActionResult Reviewed(int Id, string IndexType, string UserRemark, string IsContinue)
        {
            bool BeforeSave = true;
            try
            {
                BeforeSave = JobInvoiceDocEvents.beforeHeaderReviewEvent(this, new JobEventArgs(Id), ref db);
            }
            catch (Exception ex)
            {
                string message = _exception.HandleException(ex);
                TempData["CSEXC"] += message;
            }

            if (!BeforeSave)
                TempData["CSEXC"] += "Falied validation before review.";

            if (ModelState.IsValid && BeforeSave)
            {
                JobInvoiceHeader pd = new JobInvoiceHeaderService(_unitOfWork).Find(Id);

                pd.ReviewCount = (pd.ReviewCount ?? 0) + 1;
                pd.ReviewBy += User.Identity.Name + ", ";

                pd.ObjectState = Model.ObjectState.Modified;
                db.JobInvoiceHeader.Add(pd);

                try
                {
                    JobInvoiceDocEvents.onHeaderReviewEvent(this, new JobEventArgs(Id), ref db);
                }
                catch (Exception ex)
                {
                    string message = _exception.HandleException(ex);
                    TempData["CSEXC"] += message;
                }

                db.SaveChanges();

                try
                {
                    JobInvoiceDocEvents.afterHeaderReviewEvent(this, new JobEventArgs(Id), ref db);
                }
                catch (Exception ex)
                {
                    string message = _exception.HandleException(ex);
                    TempData["CSEXC"] += message;
                }

                LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                {
                    DocTypeId = pd.DocTypeId,
                    DocId = pd.JobInvoiceHeaderId,
                    ActivityType = (int)ActivityTypeContants.Reviewed,
                    UserRemark = UserRemark,
                    DocNo = pd.DocNo,
                    DocDate = pd.DocDate,
                    DocStatus = pd.Status,
                }));

                if (!string.IsNullOrEmpty(IsContinue) && IsContinue == "True")
                {
                    JobInvoiceHeader HEader = _JobInvoiceHeaderService.Find(Id);

                    int nextId = new NextPrevIdService(_unitOfWork).GetNextPrevId(Id, HEader.DocTypeId, User.Identity.Name, ForActionConstants.PendingToReview, "Web.JobInvoiceHeaders", "JobInvoiceHeaderId", PrevNextConstants.Next);
                    if (nextId == 0)
                    {
                        var PendingtoSubmitCount = PendingToReviewCount(HEader.DocTypeId);
                        if (PendingtoSubmitCount > 0)
                            return RedirectToAction("Index_PendingToReview", new { id = HEader.DocTypeId, IndexType = IndexType });
                        else
                            return RedirectToAction("Index", new { id = HEader.DocTypeId, IndexType = IndexType });

                    }

                    ViewBag.PendingToReview = PendingToReviewCount(Id);
                    return RedirectToAction("Detail", new { id = nextId, transactionType = "ReviewContinue", IndexType = IndexType });
                }


                else
                    return RedirectToAction("Index", new { id = pd.DocTypeId, IndexType = IndexType }).Success(pd.DocNo + " Reviewed Successfully.");
            }

            return View();
        }







        // GET: /ProductMaster/Delete/5

        private ActionResult Remove(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            JobInvoiceHeader JobInvoiceHeader = db.JobInvoiceHeader.Find(id);

            #region DocTypeTimeLineValidation

            bool TimePlanValidation = true;
            string ExceptionMsg = "";
            try
            {
                TimePlanValidation = DocumentValidation.ValidateDocument(Mapper.Map<DocumentUniqueId>(JobInvoiceHeader), DocumentTimePlanTypeConstants.Delete, User.Identity.Name, out ExceptionMsg, out Continue);
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

            if (JobInvoiceHeader == null)
            {
                return HttpNotFound();
            }
            ReasonViewModel vm = new ReasonViewModel()
            {
                id = id,
            };

            return PartialView("_Reason", vm);
        }

        // POST: /ProductMaster/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(ReasonViewModel vm)
        {

            #region BeforeSave
            bool BeforeSave = true;
            try
            {
                BeforeSave = JobInvoiceDocEvents.beforeHeaderDeleteEvent(this, new JobEventArgs(vm.id), ref db);
            }
            catch (Exception ex)
            {
                string message = _exception.HandleException(ex);
                TempData["CSEXC"] += message;
                EventException = true;
            }

            if (!BeforeSave)
                TempData["CSEXC"] += "Failed validation before delete";
            #endregion

            if (ModelState.IsValid && BeforeSave && !EventException)
            {
                List<LogTypeViewModel> LogList = new List<LogTypeViewModel>();

                //var temp = _JobInvoiceHeaderService.Find(vm.id);

                try
                {
                    JobInvoiceDocEvents.onHeaderDeleteEvent(this, new JobEventArgs(vm.id), ref db);
                }
                catch (Exception ex)
                {
                    string message = _exception.HandleException(ex);
                    TempData["CSEXC"] += message;
                    EventException = true;
                }

                var temp = db.JobInvoiceHeader.Find(vm.id);


                LogList.Add(new LogTypeViewModel
                {
                    ExObj = Mapper.Map<JobInvoiceHeader>(temp),
                });

                var line = (from p in db.JobInvoiceLine
                            where p.JobInvoiceHeaderId == vm.id
                            select p).ToList();

                new JobOrderLineStatusService(_unitOfWork).DeleteJobQtyOnInvoiceMultiple(temp.JobInvoiceHeaderId, ref db);
                new JobReceiveLineStatusService(_unitOfWork).DeleteJobReceiveQtyOnInvoiceMultiple(temp.JobInvoiceHeaderId, ref db);


                var attributes = (from A in db.JobInvoiceHeaderAttributes where A.HeaderTableId == vm.id select A).ToList();

                foreach (var ite2 in attributes)
                {
                    ite2.ObjectState = Model.ObjectState.Deleted;
                    db.JobInvoiceHeaderAttributes.Remove(ite2);
                }


                //var LedgerHeaders = new LedgerHeaderService(_unitOfWork).FindByDocNoHeader(temp.DocNo, temp.DocTypeId, temp.SiteId, temp.DivisionId );
                //var LedgerLines = new LedgerLineService(_unitOfWork).FindByLedgerHeader(LedgerHeaders.LedgerHeaderId);

                var LedgerHeaders = (from p in db.LedgerHeader
                                     where p.LedgerHeaderId == temp.LedgerHeaderId
                                     select p).FirstOrDefault();

                if (LedgerHeaders != null)
                {
                    IEnumerable<LedgerAdj> LedgerAdjList = (from p in db.Ledger
                                                            join LA in db.LedgerAdj on p.LedgerId equals LA.CrLedgerId into LATable
                                                            from LAtTab in LATable.DefaultIfEmpty()
                                                            where p.LedgerHeaderId == LedgerHeaders.LedgerHeaderId && LAtTab.LedgerAdjId != null
                                                            select LAtTab).ToList();
                    foreach (LedgerAdj item in LedgerAdjList)
                    {
                        item.ObjectState = Model.ObjectState.Deleted;
                        db.LedgerAdj.Remove(item);
                    }

                    var LedgerLines = (from p in db.Ledger
                                       where p.LedgerHeaderId == LedgerHeaders.LedgerHeaderId
                                       select p).ToList();

                    foreach (var item in LedgerLines)
                    {
                        item.ObjectState = Model.ObjectState.Deleted;
                        db.Ledger.Remove(item);
                        //new LedgerLineService(_unitOfWork).Delete(item.LedgerLineId );
                    }

                    LedgerHeaders.ObjectState = Model.ObjectState.Deleted;
                    db.LedgerHeader.Remove(LedgerHeaders);
                }

                // For Delete TDS Posted
                var LedgerHeaders1 = (from p in db.LedgerHeader
                                     where p.ReferenceDocId == (int) temp.JobInvoiceHeaderId &&  p.ReferenceDocTypeId ==(int?) temp.DocTypeId
                                     select p).FirstOrDefault();

                if (LedgerHeaders1 != null)
                {
                    IEnumerable<LedgerAdj> LedgerAdjList = (from p in db.Ledger
                                                            join LA in db.LedgerAdj on p.LedgerId equals LA.DrLedgerId into LATable
                                                            from LAtTab in LATable.DefaultIfEmpty()
                                                            where p.LedgerHeaderId == LedgerHeaders1.LedgerHeaderId && LAtTab.LedgerAdjId != null
                                                            select LAtTab).ToList();
                    foreach (LedgerAdj item in LedgerAdjList)
                    {
                        item.ObjectState = Model.ObjectState.Deleted;
                        db.LedgerAdj.Remove(item);
                    }

                    var Ledgers = (from p in db.Ledger
                                       where p.LedgerHeaderId == LedgerHeaders1.LedgerHeaderId
                                       select p).ToList();

                    foreach (var item in Ledgers)
                    {
                        item.ObjectState = Model.ObjectState.Deleted;
                        db.Ledger.Remove(item);
                        //new LedgerLineService(_unitOfWork).Delete(item.LedgerLineId );
                    }

                    var LedgerLines = (from p in db.LedgerLine
                                   where p.LedgerHeaderId == LedgerHeaders1.LedgerHeaderId
                                   select p).ToList();

                    foreach (var item in LedgerLines)
                    {
                        item.ObjectState = Model.ObjectState.Deleted;
                        db.LedgerLine.Remove(item);
                        //new LedgerLineService(_unitOfWork).Delete(item.LedgerLineId );
                    }

                    LedgerHeaders1.ObjectState = Model.ObjectState.Deleted;
                    db.LedgerHeader.Remove(LedgerHeaders1);
                }

                var LineIds = line.Select(m => m.JobInvoiceLineId).ToArray();

                var LineCharges = (from p in db.JobInvoiceLineCharge
                                   where LineIds.Contains(p.LineTableId)
                                   select p).ToList();

                var HeaderCharges = (from p in db.JobInvoiceHeaderCharges
                                     where p.HeaderTableId == vm.id
                                     select p).ToList();



                //new LedgerHeaderService(_unitOfWork).Delete(LedgerHeaders.LedgerHeaderId);




                foreach (var item in LineCharges)
                {
                    item.ObjectState = Model.ObjectState.Deleted;
                    db.JobInvoiceLineCharge.Remove(item);
                }

                foreach (var item in line)
                {

                    LogList.Add(new LogTypeViewModel
                    {
                        ExObj = Mapper.Map<JobInvoiceLine>(item),
                    });

                    //var linecharges = new JobInvoiceLineChargeService(_unitOfWork).GetCalculationProductList(item.JobInvoiceLineId);
                    //foreach (var citem in linecharges)
                    //    new JobInvoiceLineChargeService(_unitOfWork).Delete(citem.Id);

                    JobInvoiceLineStatus Status = new JobInvoiceLineStatus();
                    Status.JobInvoiceLineId = item.JobInvoiceLineId;
                    db.JobInvoiceLineStatus.Attach(Status);
                    Status.ObjectState = Model.ObjectState.Deleted;

                    //new JobInvoiceLineService(_unitOfWork).Delete(item.JobInvoiceLineId);
                    item.ObjectState = Model.ObjectState.Deleted;
                    db.JobInvoiceLine.Remove(item);
                }

                //var headercharges = new JobInvoiceHeaderChargeService(_unitOfWork).GetCalculationFooterList(vm.id);
                //foreach (var item in headercharges)
                //    new JobInvoiceHeaderChargeService(_unitOfWork).Delete(item.Id);


                foreach (var item in HeaderCharges)
                {
                    item.ObjectState = Model.ObjectState.Deleted;
                    db.JobInvoiceHeaderCharges.Remove(item);
                }

                //_JobInvoiceHeaderService.Delete(vm.id);
                temp.ObjectState = Model.ObjectState.Deleted;
                db.JobInvoiceHeader.Remove(temp);

                XElement Modifications = new ModificationsCheckService().CheckChanges(LogList);



                try
                {
                    if (EventException)
                        throw new Exception();

                    db.SaveChanges();
                    //_unitOfWork.Save();
                }

                catch (Exception ex)
                {
                    string message = _exception.HandleException(ex);
                    TempData["CSEXC"] += message;
                    return PartialView("_Reason", vm);
                }

                try
                {
                    JobInvoiceDocEvents.afterHeaderDeleteEvent(this, new JobEventArgs(vm.id), ref db);
                }
                catch (Exception ex)
                {
                    string message = _exception.HandleException(ex);
                    TempData["CSEXC"] += message;
                }

                LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                {
                    DocTypeId = temp.DocTypeId,
                    DocId = vm.id,
                    ActivityType = (int)ActivityTypeContants.Deleted,
                    UserRemark = vm.Reason,
                    DocNo = temp.DocNo,
                    xEModifications = Modifications,
                    DocDate = temp.DocDate,
                    DocStatus = temp.Status,
                }));


                return Json(new { success = true });

            }
            return PartialView("_Reason", vm);
        }

        [HttpGet]
        public ActionResult NextPage(int DocId, int DocTypeId)//CurrentHeaderId
        {
            var nextId = new NextPrevIdService(_unitOfWork).GetNextPrevId(DocId, DocTypeId, User.Identity.Name, "", "Web.JobInvoiceHeaders", "JobInvoiceHeaderId", PrevNextConstants.Next);
            return Edit(nextId, "");
        }
        [HttpGet]
        public ActionResult PrevPage(int DocId, int DocTypeId)//CurrentHeaderId
        {
            var PrevId = new NextPrevIdService(_unitOfWork).GetNextPrevId(DocId, DocTypeId, User.Identity.Name, "", "Web.JobInvoiceHeaders", "JobInvoiceHeaderId", PrevNextConstants.Prev);
            return Edit(PrevId, "");
        }

        [HttpGet]
        public ActionResult History()
        {
            //To Be Implemented
            return View("~/Views/Shared/UnderImplementation.cshtml");
        }

        [HttpGet]
        public ActionResult Email()
        {
            //To Be Implemented
            return View("~/Views/Shared/UnderImplementation.cshtml");
        }

        [HttpGet]
        private ActionResult PrintOut(int id, string SqlProcForPrint)
        {
            String query = SqlProcForPrint;
            return Redirect((string)System.Configuration.ConfigurationManager.AppSettings["JobsDomain"] + "/Report_DocumentPrint/DocumentPrint/?DocumentId=" + id + "&queryString=" + query);
        }

        [HttpGet]
        public ActionResult Print(int id)
        {
            JobInvoiceHeader header = _JobInvoiceHeaderService.Find(id);
            if (header.Status == (int)StatusConstants.Drafted || header.Status == (int)StatusConstants.Import)
            {
                var SEttings = new JobInvoiceSettingsService(_unitOfWork).GetJobInvoiceSettingsForDocument(header.DocTypeId, header.DivisionId, header.SiteId);
                if (string.IsNullOrEmpty(SEttings.SqlProcDocumentPrint))
                    throw new Exception("Document Print Not Configured");
                else
                    return PrintOut(id, SEttings.SqlProcDocumentPrint);
            }
            else
                return HttpNotFound();

        }

        [HttpGet]
        public ActionResult PrintAfter_Submit(int id)
        {
            JobInvoiceHeader header = _JobInvoiceHeaderService.Find(id);
            if (header.Status == (int)StatusConstants.Submitted || header.Status == (int)StatusConstants.Modified)
            {
                var SEttings = new JobInvoiceSettingsService(_unitOfWork).GetJobInvoiceSettingsForDocument(header.DocTypeId, header.DivisionId, header.SiteId);
                if (string.IsNullOrEmpty(SEttings.SqlProcDocumentPrint_AfterSubmit))
                    throw new Exception("Document Print Not Configured");
                else
                    return PrintOut(id, SEttings.SqlProcDocumentPrint_AfterSubmit);
            }
            else
                return HttpNotFound();
        }

        [HttpGet]
        public ActionResult PrintAfter_Approve(int id)
        {
            JobInvoiceHeader header = _JobInvoiceHeaderService.Find(id);
            if (header.Status == (int)StatusConstants.Approved)
            {
                var SEttings = new JobInvoiceSettingsService(_unitOfWork).GetJobInvoiceSettingsForDocument(header.DocTypeId, header.DivisionId, header.SiteId);
                if (string.IsNullOrEmpty(SEttings.SqlProcDocumentPrint_AfterApprove))
                    throw new Exception("Document Print Not Configured");
                else
                    return PrintOut(id, SEttings.SqlProcDocumentPrint_AfterApprove);
            }
            else
                return HttpNotFound();
        }


        [HttpGet]
        public ActionResult Report(int id)
        {
            DocumentType Dt = new DocumentType();
            Dt = new DocumentTypeService(_unitOfWork).Find(id);

            JobInvoiceSettings SEttings = new JobInvoiceSettingsService(_unitOfWork).GetJobInvoiceSettingsForDocument(Dt.DocumentTypeId, (int)System.Web.HttpContext.Current.Session["DivisionId"], (int)System.Web.HttpContext.Current.Session["SiteId"]);

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


        public int PendingToSubmitCount(int id)
        {
            return (_JobInvoiceHeaderService.GetJobInvoiceHeaderListPendingToSubmit(id, User.Identity.Name)).Count();
        }
        public int PendingToReviewCount(int id)
        {
            return (_JobInvoiceHeaderService.GetJobInvoiceHeaderListPendingToReview(id, User.Identity.Name)).Count();
        }

        public ActionResult Import(int id)//Document Type Id
        {
            //ControllerAction ca = new ControllerActionService(_unitOfWork).Find(id);
            JobInvoiceHeaderViewModel vm = new JobInvoiceHeaderViewModel();

            vm.DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            vm.SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];

            var settings = new JobInvoiceSettingsService(_unitOfWork).GetJobInvoiceSettingsForDocument(id, vm.DivisionId, vm.SiteId);

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
                        if (menuviewmodel.AreaName != null && menuviewmodel.AreaName != "")
                        {
                            return Redirect(System.Configuration.ConfigurationManager.AppSettings[menuviewmodel.URL] + "/" + menuviewmodel.AreaName + "/" + menuviewmodel.ControllerName + "/" + menuviewmodel.ActionName + "/" + id + "?MenuId=" + menuviewmodel.MenuId);
                        }
                        else
                        {
                            return Redirect(System.Configuration.ConfigurationManager.AppSettings[menuviewmodel.URL] + "/" + menuviewmodel.ControllerName + "/" + menuviewmodel.ActionName + "/" + id + "?MenuId=" + menuviewmodel.MenuId);
                        }
                    }
                    else
                    {
                        return RedirectToAction(menuviewmodel.ActionName, menuviewmodel.ControllerName, new { MenuId = menuviewmodel.MenuId, id = id });
                    }
                }
            }
            return RedirectToAction("Index", new { id = id });
        }

        public ActionResult GeneratePrints(string Ids, int DocTypeId)
        {

            if (!string.IsNullOrEmpty(Ids))
            {
                int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
                int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];

                var Settings = new JobInvoiceSettingsService(_unitOfWork).GetJobInvoiceSettingsForDocument(DocTypeId, DivisionId, SiteId);

                string ReportSql = "";

                if (!string.IsNullOrEmpty(Settings.DocumentPrint))
                    ReportSql = db.ReportHeader.Where((m) => m.ReportName == Settings.DocumentPrint).FirstOrDefault().ReportSQL;

                try
                {

                    List<byte[]> PdfStream = new List<byte[]>();
                    foreach (var item in Ids.Split(',').Select(Int32.Parse))
                    {

                        DirectReportPrint drp = new DirectReportPrint();

                        var pd = db.JobInvoiceHeader.Find(item);

                        if (Settings.isAllowedDuplicatePrint ==false && pd.IsDocumentPrinted ==true)
                            throw new Exception("Duplicate Print Not Allowed");

                        LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                        {
                            DocTypeId = pd.DocTypeId,
                            DocId = pd.JobInvoiceHeaderId,
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
                                if (Settings.SqlProcDocumentPrint==null || Settings.SqlProcDocumentPrint =="")
                                {
                                    List < string> QueryList = new List<string>();
                                    QueryList = DocumentPrintData(item);
                                    Pdf = drp.DocumentPrint(QueryList, User.Identity.Name);
                                }
                                else 
                                Pdf = drp.DirectDocumentPrint(Settings.SqlProcDocumentPrint, User.Identity.Name, item);

                                PdfStream.Add(Pdf);
                            }
                            else if (pd.Status == (int)StatusConstants.Submitted || pd.Status == (int)StatusConstants.ModificationSubmitted)
                            {
                                if (Settings.SqlProcDocumentPrint_AfterSubmit == null || Settings.SqlProcDocumentPrint_AfterSubmit == "")
                                {
                                    List<string> QueryList = new List<string>();
                                    QueryList = DocumentPrintData(item);
                                    Pdf = drp.DocumentPrint(QueryList, User.Identity.Name);
                                }
                                else
                                    Pdf = drp.DirectDocumentPrint(Settings.SqlProcDocumentPrint_AfterSubmit, User.Identity.Name, item);

                                PdfStream.Add(Pdf);
                            }
                            else if (pd.Status == (int)StatusConstants.Approved)
                            {
                                if (Settings.SqlProcDocumentPrint_AfterApprove == null || Settings.SqlProcDocumentPrint_AfterApprove == "")
                                {
                                    List<string> QueryList = new List<string>();
                                    QueryList = DocumentPrintData(item);
                                    Pdf = drp.DocumentPrint(QueryList, User.Identity.Name);
                                }
                                else
                                    Pdf = drp.DirectDocumentPrint(Settings.SqlProcDocumentPrint_AfterApprove, User.Identity.Name, item);
                                PdfStream.Add(Pdf);
                            }
                        }

                        //JobInvoiceHeader temp = _JobInvoiceHeaderService.Find(item);
                        pd.IsDocumentPrinted = true;
                        pd.ObjectState = Model.ObjectState.Modified;
                        //_JobInvoiceHeaderService.Update(temp);
                        db.JobInvoiceHeader.Add(pd);
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

        private List<string> DocumentPrintData (int item)
        {
            List<string> DocumentPrintData = new List<string>();
            String mQry;
            mQry = @"DECLARE @ReportName NVARCHAR (Max)
                     DECLARE @TotalAmount DECIMAL
                     SET @TotalAmount = (SELECT Max(Amount) FROM Web.JobInvoiceHeaderCharges WHERE HeaderTableId = " + item + @" AND ChargeId = 34 ) 
		
                     DECLARE @DebitNoteAmount DECIMAL
                     SET @DebitNoteAmount =
                                        (
                                            SELECT sum(DL.Amount) AS DebitAmount
                                            FROM web.JobInvoiceHeaders H WITH(Nolock)
                                            LEFT JOIN web.Ledgers L WITH(Nolock)  ON L.LedgerHeaderId = H.LedgerHeaderId
                                            LEFT JOIN web.LedgerLines DL WITH(Nolock) ON DL.ReferenceId = L.LedgerId
                                            LEFT JOIN web.LedgerHeaders LH ON LH.LedgerHeaderId = DL.LedgerHeaderId
                                            WHERE H.JobInvoiceHeaderId = " + item + @"
                                            AND LH.DocTypeId = 559
	                                    ) 
     	
                     DECLARE @CreditNoteAmount DECIMAL
                                        SET @CreditNoteAmount =
                                        (
                                            SELECT sum(DL.Amount) AS DebitAmount
                                            FROM web.JobInvoiceHeaders H WITH(Nolock)
                                            LEFT JOIN web.Ledgers L WITH(Nolock)  ON L.LedgerHeaderId = H.LedgerHeaderId
                                            LEFT JOIN web.LedgerLines DL WITH(Nolock) ON DL.ReferenceId = L.LedgerId
                                            LEFT JOIN web.LedgerHeaders LH ON LH.LedgerHeaderId = DL.LedgerHeaderId
                                            WHERE H.JobInvoiceHeaderId = " + item + @"
                                            AND LH.DocTypeId = 560
	                                    ) 	
     	
     	            IF @DebitNoteAmount > 0
                                 SET @ReportName = 'JobInvoicePrint_WithDebitNote'
                                  ELSE
                                 SET @ReportName = 'JobInvoicePrint'


                                            SELECT H.jobInvoiceHeaderId,max(P.Name) AS Name, max(PA.Address)AS Address, max(City.CityName) AS CityName, max(P.Mobile) AS MobileNo
                                            , (SELECT TOP 1  PR.RegistrationNo  FROM web.PersonRegistrations PR WHERE PersonId = max(L.JobWorkerId) AND RegistrationType = 'Tin No' ) AS TinNo
                                               ,(SELECT TOP 1  PR.RegistrationNo  FROM web.PersonRegistrations PR WHERE PersonId = max(L.JobWorkerId) AND RegistrationType = 'Pan No' ) AS PanNo
                                                   , max(DT.DocumentTypeShortName + '-' + H.DocNo) AS InvoiceNo, replace(convert(VARCHAR, max(H.DocDate), 106), ' ', '/')  AS InvoicedateDate,
                                                  max(H.JobWorkerDocNo) AS DyersDocNo, replace(convert(VARCHAR, max(H.JobWorkerDocDate), 106), ' ', '/') AS DyersDocDate,
                                                  isnull(max(H.CreditDays), 0) AS CreditDayes,
                                                  max(DTJR.DocumentTypeShortName + '-' + JRH.DocNo) AS RectNo, replace(convert(VARCHAR, max(JRH.DocDate), 106), ' ', '/') AS RectDate, max(DTJO.DocumentTypeShortName + '-' + JOH.DocNo) AS OrderNo,
                                                  max(PD.ProductName) AS ProductName, max(D2.Dimension2Name) AS Dimension2Name, D1.Dimension1Name,
                                                 null  AS SubReportProcList,
                                                 sum(convert(DECIMAL(18, 4), isnull(JRL.Qty, 0)) + convert(DECIMAL(18, 4), isnull(JRL.LossQty, 0))) AS QtyDyed, Sum(Isnull(JRL.Qty, 0)) AS Qty, Sum(Isnull(JRL.LossQty, 0)) AS LossQty, sum(isnull(JRL.PassQty, 0)) AS PassQty, L.Rate, sum(isnull(L.Amount, 0)) AS Amount,
                                                 @TotalAmount as NetAmount, @DebitNoteAmount AS DebitNoteAmount, isnull(@TotalAmount, 0) - isnull(@DebitNoteAmount, 0) + isnull(@CreditNoteAmount, 0) NetPayableAmount,
                                                 max(U.UnitName) AS Unit, isnull(max(U.DecimalPlaces), 0) AS DecimalPlaces, max(H.Siteid) AS Siteid, max(H.DivisionId) AS DivisionId, CASE WHEN isnull(Max(convert(INT, H.IsDocumentPrinted)), 0) = 1 THEN max(Dt.DocumentTypeName) + ' [Duplicate]' ELSE  max(Dt.DocumentTypeName) END AS ReportTitle, @ReportName+'.rdl'  AS ReportName,
                                                  'Web.JobInvoiceHeaderCharges' AS ChargesTableName, max(WC.CompanyName) AS CompanyName, max(H.Remark) AS HeaderRemark,
                                                  max(DN.DocDate) AS DebitNoteDate, max(DN.DocNo) AS DebitNotNo, max(DN.Narration) AS DebitNoteReason,
                                                  isnull(@CreditNoteAmount, 0) AS CreditNoteAmount
                                                  FROM
                                                 (SELECT * FROM web._JobInvoiceHeaders WITH(nolock) WHERE jobInvoiceHeaderId =  " + item + @") H
                                                 LEFT JOIN web._JobInvoiceLines L WITH(nolock) ON L.JobInvoiceHeaderId = H.jobInvoiceHeaderId
                                                 LEFT JOIN Web.DocumentTypes DT WITH(nolock) ON DT.DocumentTypeId = H.DocTypeId
                                                 LEFT JOIN Web._People P WITH(nolock) ON P.PersonID = L.JobWorkerId
                                                 LEFT JOIN Web.PersonAddresses PA WITH(nolock) ON PA.PersonId = P.PersonID
                                                 LEFT JOIN Web.Cities City WITH(nolock) ON City.CityId = PA.CityId
                                                 LEFT JOIN Web._JobReceiveLines JRL ON JRL.JobReceiveLineId = L.JobReceiveLineId
                                                 LEFT JOIN Web._JobReceiveHeaders JRH ON JRL.JobReceiveHeaderId = JRH.JobReceiveHeaderId
                                                 LEFT JOIN Web.DocumentTypes DTJR WITH(nolock) ON DTJR.DocumentTypeId = JRH.DocTypeId
                                                 LEFT JOIN Web._JobOrderLines JOL WITH(nolock) ON  JOL.JoborderLineId = JRL.JobOrderLineId
                                                 LEFT JOIN Web._JobOrderHeaders JOH WITH(nolock) ON JOL.JobOrderHeaderId = JOH.JobOrderHeaderId
                                                 LEFT JOIN Web.Products PD WITH(nolock) ON PD.ProductId = JOL.ProductId
                                                 LEFT JOIN web.Dimension1 D1  WITH(nolock) ON D1.Dimension1Id = JOL.Dimension1Id
                                                 LEFT JOIN web.Dimension2 D2 WITH(nolock) ON D2.Dimension2Id = JOL.Dimension2Id
                                                 LEFT JOIN Web.Units U WITH(nolock) ON U.UnitId = PD.UnitId
                                                 LEFT JOIN Web.DocumentTypes DTJO WITH(nolock) ON DTJO.DocumentTypeId = JOH.DocTypeId
                                                 LEFT JOIN Web.Divisions WD WITH(nolock) ON WD.DivisionId = H.DivisionId
                                                 LEFT JOIN web.Companies WC WITH(nolock) ON WC.CompanyId = WD.CompanyId
                                                 LEFT JOIN
                                                 (
                                                     SELECT L.LedgerHeaderId, Max(LH.DocDate) AS DocDate, Max(LH.DocNo) AS DocNo, Max(LH.Narration) AS Narration,
                                                     Max(LH.Remark) AS Remark, Max(DL.Remark) AS LineRemark, sum(DL.Amount) AS DebitAmount
                                                     FROM(SELECT * FROM web.JobInvoiceHeaders H WITH(Nolock) WHERE H.JobInvoiceHeaderId = " + item + @" )   H
                                                     LEFT JOIN web.Ledgers L WITH(Nolock)  ON L.LedgerHeaderId = H.LedgerHeaderId
                                                     LEFT JOIN web.LedgerLines DL WITH(Nolock) ON DL.ReferenceId = L.LedgerId
                                                     LEFT JOIN web.LedgerHeaders LH ON LH.LedgerHeaderId = DL.LedgerHeaderId
                                                     WHERE  LH.DocTypeId = 559
                                                     GROUP BY L.LedgerHeaderId
                                                 )  DN ON DN.LedgerHeaderId = H.LedgerHeaderId
                                                 WHERE H.jobInvoiceHeaderId = " + item + @"
                                                 GROUP BY  H.jobInvoiceHeaderId, DTJR.DocumentTypeShortName + '-' + JRH.DocNo, D1.Dimension1Name, L.Rate, JOL.ProductId--, D2.Dimension2Name
                                                 ORDER BY max(L.JobInvoiceLineId)";


            String mQry1;
            mQry1 = @"DECLARE @Id INT = " + item + @"
                    DECLARE @TableName NVARCHAR(255) = 'web.JobInvoiceHeaderCharges'

                    DECLARE @StrGrossAmount AS NVARCHAR(50)
                    DECLARE @StrBasicExciseDuty AS NVARCHAR(50)
                    DECLARE @StrExciseECess AS NVARCHAR(50)
                    DECLARE @StrExciseHECess AS NVARCHAR(50)

                    DECLARE @StrSalesTaxTaxableAmt AS NVARCHAR(50)
                    DECLARE @StrVAT AS NVARCHAR(50)
                    DECLARE @StrSAT AS NVARCHAR(50)
                    DECLARE @StrCST AS NVARCHAR(50)

                    SET @StrGrossAmount = 'Gross Amount'
                    SET @StrBasicExciseDuty = 'Basic Excise Duty'
                    SET @StrExciseECess = 'Excise ECess'
                    SET @StrExciseHECess = 'Excise HECess'

                    SET @StrSalesTaxTaxableAmt = 'Sales Tax Taxable Amt'
                    SET @StrVAT = 'VAT'
                    SET @StrSAT = 'SAT'
                    SET @StrCST = 'CST'

                    DECLARE @Qry NVARCHAR(Max);
                                                        SET @Qry = '
                            DECLARE @GrossAmount AS DECIMAL
                            DECLARE @BasicExciseDutyAmount AS DECIMAL
                            DECLARE @SalesTaxTaxableAmt AS DECIMAL

                            SELECT @GrossAmount = sum(CASE WHEN C.ChargeName = ''' + @StrGrossAmount + ''' THEN  H.Amount  ELSE 0 END),
                            @BasicExciseDutyAmount = sum(CASE WHEN C.ChargeName = ''' + @StrBasicExciseDuty + ''' THEN  H.Amount  ELSE 0 END),
                            @SalesTaxTaxableAmt = sum(CASE WHEN C.ChargeName = ''' + @StrSalesTaxTaxableAmt + ''' THEN  H.Amount  ELSE 0 END)
                            FROM ' + @TableName + ' H
                            LEFT JOIN web.ChargeTypes CT ON CT.ChargeTypeId = H.ChargeTypeId
                            LEFT JOIN web.Charges C ON C.ChargeId = H.ChargeId
                            WHERE H.Amount <> 0 AND H.HeaderTableId = ' + Convert(Varchar,@Id ) + '
                            GROUP BY H.HeaderTableId


                            SELECT H.Id, H.HeaderTableId, H.Sr, C.ChargeName, H.Amount, H.ChargeTypeId,  CT.ChargeTypeName, 
		                    --CASE WHEN C.ChargeName = ''Vat'' THEN(H.Amount * 100 / @GrossAmount) ELSE H.Rate End  AS Rate,
                            CASE
                            WHEN @SalesTaxTaxableAmt> 0 And C.ChargeName IN (''' + @StrVAT + ''', ''' + @StrSAT + ''', ''' + @StrCST+ ''')  THEN(H.Amount * 100 / @SalesTaxTaxableAmt)
                            WHEN @GrossAmount> 0 AND C.ChargeName IN (''' + @StrBasicExciseDuty + ''')  THEN(H.Amount * 100 / @GrossAmount)
                            WHEN @BasicExciseDutyAmount> 0 AND C.ChargeName IN (''' + @StrExciseECess + ''', ''' +@StrExciseHECess+ ''')  THEN(H.Amount * 100 / @BasicExciseDutyAmount)
                            ELSE 0 End AS Rate,
		                    ''TransactionChargesPrint.rdl'' AS ReportName,
                            ''Transaction Charges'' AS ReportTitle
                            FROM  ' + @TableName + '  H
                            LEFT JOIN web.ChargeTypes CT ON CT.ChargeTypeId = H.ChargeTypeId
                            LEFT JOIN web.Charges C ON C.ChargeId = H.ChargeId
                            WHERE(isnull(H.ChargeTypeId, 0) <> ''4'' OR C.ChargeName = ''Net Amount'') AND H.Amount <> 0
                            AND H.HeaderTableId = ' + Convert(Varchar,@Id ) + ''


                        DECLARE @TmpData TABLE
                        (
                        id BIGINT,
                        HeaderTableId INT,
                        Sr INT,
                        ChargeName NVARCHAR(50),
                        Amount DECIMAL(18, 4),
                        ChargeTypeId INT,
                        ChargeTypeName NVARCHAR(50),
                        Rate DECIMAL(38, 20),
                        ReportName nVarchar(255),
                        ReportTitle nVarchar(255)
                        );


                                    Insert Into @TmpData EXEC(@Qry)
                                    SELECT id, HeaderTableId, Sr, ChargeName, Amount, ChargeTypeId, ChargeTypeName, Rate, ReportName FROM @TmpData
                                    ORDER BY Sr";

            DocumentPrintData.Add(mQry);
            DocumentPrintData.Add(mQry1);
            return DocumentPrintData;

        }
        public JsonResult JobInvoicePrint(string Id)
        {
           
            IEnumerable<PrintViewModel> JobInvoicePrint = _JobInvoiceHeaderService.JobInvoicePrint(Id, User.Identity.Name);
            if (JobInvoicePrint != null)
            {
                JsonResult json = Json(new { Success = true, Data = JobInvoicePrint.ToList() }, JsonRequestBehavior.AllowGet);
                json.MaxJsonLength = int.MaxValue;
                return json;
            }
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCustomPerson(string searchTerm, int pageSize, int pageNum, int filter)//DocTypeId
        {
            var Query = _JobInvoiceHeaderService.GetCustomPerson(filter, searchTerm);
            var temp = Query.Skip(pageSize * (pageNum - 1))
                .Take(pageSize)
                .ToList();

            var count = Query.Count();

            ComboBoxPagedResult Data = new ComboBoxPagedResult();
            Data.Results = temp;
            Data.Total = count;

            return new JsonpResult
            {
                Data = Data,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #region submitValidation
        public bool Submitvalidation(int id, out string Msg)
        {   
            Msg = "";
            int Stockline = (new JobInvoiceLineService(_unitOfWork).GetLineListForIndex(id)).Count();
            if (Stockline == 0)
            {
                Msg = "Add Line Record. <br />";
            }
            else
            {
                Msg = "";
            }
            return (string.IsNullOrEmpty(Msg));
        }

        #endregion submitValidation

        public ActionResult GetSummary(int id)
        {
            return Redirect(System.Configuration.ConfigurationManager.AppSettings["JobsDomain"] + "/Rug/WeavingInvoice/GetSummary/" + id);
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

        public void LedgerPostingDBLineWise(ref LedgerHeaderViewModel LedgerHeaderViewModel, IEnumerable<JobInvoiceHeaderCharge> HeaderTable, IEnumerable<JobInvoiceLineCharge> LineTable, ref ApplicationDbContext Context)
        {
            int PersonAccountId = 6612;
            int LedgerHeaderId = 0;

            if (LedgerHeaderViewModel.LedgerHeaderId == 0)
            {
                LedgerHeader LedgerHeader = new LedgerHeader();

                LedgerHeader.DocTypeId = LedgerHeaderViewModel.DocTypeId;
                LedgerHeader.DocDate = LedgerHeaderViewModel.DocDate;
                LedgerHeader.DocNo = LedgerHeaderViewModel.DocNo;
                LedgerHeader.DivisionId = LedgerHeaderViewModel.DivisionId;
                LedgerHeader.SiteId = LedgerHeaderViewModel.SiteId;
                LedgerHeader.Narration = LedgerHeaderViewModel.Narration;
                LedgerHeader.Remark = LedgerHeaderViewModel.Remark;
                LedgerHeader.CreatedBy = LedgerHeaderViewModel.CreatedBy;
                LedgerHeader.ProcessId = LedgerHeaderViewModel.ProcessId;
                LedgerHeader.CreatedDate = DateTime.Now.Date;
                LedgerHeader.ModifiedBy = LedgerHeaderViewModel.ModifiedBy;
                LedgerHeader.ModifiedDate = DateTime.Now.Date;
                LedgerHeader.ObjectState = Model.ObjectState.Added;
                Context.LedgerHeader.Add(LedgerHeader);
                //new LedgerHeaderService(_unitOfWork).Create(LedgerHeader);
            }
            else
            {

                int LedHeadId = LedgerHeaderViewModel.LedgerHeaderId;
                LedgerHeader LedgerHeader = (from p in Context.LedgerHeader
                                             where p.LedgerHeaderId == LedHeadId
                                             select p).FirstOrDefault();

                LedgerHeader.DocTypeId = LedgerHeaderViewModel.DocTypeId;
                LedgerHeader.DocDate = LedgerHeaderViewModel.DocDate;
                LedgerHeader.DocNo = LedgerHeaderViewModel.DocNo;
                LedgerHeader.DivisionId = LedgerHeaderViewModel.DivisionId;
                LedgerHeader.ProcessId = LedgerHeaderViewModel.ProcessId;
                LedgerHeader.SiteId = LedgerHeaderViewModel.SiteId;
                LedgerHeader.Narration = LedgerHeaderViewModel.Narration;
                LedgerHeader.Remark = LedgerHeaderViewModel.Remark;
                LedgerHeader.ModifiedBy = LedgerHeaderViewModel.ModifiedBy;
                LedgerHeader.ModifiedDate = DateTime.Now.Date;
                LedgerHeader.ObjectState = Model.ObjectState.Modified;
                Context.LedgerHeader.Add(LedgerHeader);
                //new LedgerHeaderService(_unitOfWork).Update(LedgerHeader);

                IEnumerable<LedgerAdj> LedgerAdjList = (from p in Context.Ledger
                                                        join LA in Context.LedgerAdj on p.LedgerId equals LA.CrLedgerId into LATable
                                                        from LAtTab in LATable.DefaultIfEmpty()
                                                        where p.LedgerHeaderId == LedHeadId && LAtTab.LedgerAdjId != null
                                                        select LAtTab).ToList();
                foreach (LedgerAdj item in LedgerAdjList)
                {
                    item.ObjectState = Model.ObjectState.Deleted;
                    Context.LedgerAdj.Remove(item);
                    //new LedgerService(_unitOfWork).Delete(item);
                }

                IEnumerable<Ledger> LedgerList = (from p in Context.Ledger
                                                  where p.LedgerHeaderId == LedHeadId
                                                  select p).ToList();
                foreach (Ledger item in LedgerList)
                {
                    item.ObjectState = Model.ObjectState.Deleted;
                    Context.Ledger.Remove(item);
                    //new LedgerService(_unitOfWork).Delete(item);
                }

                LedgerHeaderId = LedgerHeader.LedgerHeaderId;
            }


            IEnumerable<LedgerPostingViewModel> LedgerHeaderAmtDr = (from H in HeaderTable
                                                                     join A in Context.LedgerAccount on H.PersonID equals A.PersonId into LedgerAccountTable
                                                                     from LedgerAccountTab in LedgerAccountTable.DefaultIfEmpty()
                                                                     where H.LedgerAccountDrId != null && H.Amount != 0 && H.Amount != null
                                                                     select new LedgerPostingViewModel
                                                                     {
                                                                         LedgerAccountId = (int)(H.LedgerAccountDrId == PersonAccountId ? LedgerAccountTab.LedgerAccountId : H.LedgerAccountDrId),
                                                                         ContraLedgerAccountId = (H.LedgerAccountCrId == PersonAccountId ? LedgerAccountTab.LedgerAccountId : H.LedgerAccountCrId),
                                                                         CostCenterId = H.CostCenterId,
                                                                         AmtDr = Math.Abs((H.Amount > 0 ? H.Amount : 0) ?? 0),
                                                                         AmtCr = Math.Abs((H.Amount < 0 ? H.Amount : 0) ?? 0)
                                                                     }).ToList();

            IEnumerable<LedgerPostingViewModel> LedgerHeaderAmtCr = (from H in HeaderTable
                                                                     join A in Context.LedgerAccount on H.PersonID equals A.PersonId into LedgerAccountTable
                                                                     from LedgerAccountTab in LedgerAccountTable.DefaultIfEmpty()
                                                                     where H.LedgerAccountCrId != null && H.Amount != 0 && H.Amount != null
                                                                     select new LedgerPostingViewModel
                                                                     {
                                                                         LedgerAccountId = (int)(H.LedgerAccountCrId == PersonAccountId ? LedgerAccountTab.LedgerAccountId : H.LedgerAccountCrId),
                                                                         ContraLedgerAccountId = (H.LedgerAccountDrId == PersonAccountId ? LedgerAccountTab.LedgerAccountId : H.LedgerAccountDrId),
                                                                         CostCenterId = H.CostCenterId,
                                                                         AmtCr = Math.Abs((H.Amount > 0 ? H.Amount : 0) ?? 0),
                                                                         AmtDr = Math.Abs((H.Amount < 0 ? H.Amount : 0) ?? 0)
                                                                     }).ToList();

            IEnumerable<LedgerPostingViewModel> LedgerLineAmtDr = (from L in LineTable
                                                                   join A in Context.LedgerAccount on L.PersonID equals A.PersonId into LedgerAccountTable
                                                                   from LedgerAccountTab in LedgerAccountTable.DefaultIfEmpty()
                                                                   where L.LedgerAccountDrId != null && L.Amount != 0 && L.Amount != null
                                                                   select new LedgerPostingViewModel
                                                                   {
                                                                       LedgerAccountId = (int)(L.LedgerAccountDrId == PersonAccountId ? LedgerAccountTab.LedgerAccountId : L.LedgerAccountDrId),
                                                                       ContraLedgerAccountId = (L.LedgerAccountCrId == PersonAccountId ? LedgerAccountTab.LedgerAccountId : L.LedgerAccountCrId),
                                                                       CostCenterId = L.CostCenterId,
                                                                       AmtDr = Math.Abs((L.Amount > 0 ? L.Amount : 0) ?? 0),
                                                                       AmtCr = Math.Abs((L.Amount < 0 ? L.Amount : 0) ?? 0),
                                                                       ReferenceDocLineId = L.LineTableId
                                                                   }).ToList();

            IEnumerable<LedgerPostingViewModel> LedgerLineAmtCr = (from L in LineTable
                                                                   join A in Context.LedgerAccount on L.PersonID equals A.PersonId into LedgerAccountTable
                                                                   from LedgerAccountTab in LedgerAccountTable.DefaultIfEmpty()
                                                                   where L.LedgerAccountCrId != null && L.Amount != 0 && L.Amount != null
                                                                   select new LedgerPostingViewModel
                                                                   {
                                                                       LedgerAccountId = (int)(L.LedgerAccountCrId == PersonAccountId ? LedgerAccountTab.LedgerAccountId : L.LedgerAccountCrId),
                                                                       ContraLedgerAccountId = (L.LedgerAccountDrId == PersonAccountId ? LedgerAccountTab.LedgerAccountId : L.LedgerAccountDrId),
                                                                       CostCenterId = L.CostCenterId,
                                                                       AmtCr = Math.Abs((L.Amount > 0 ? L.Amount : 0) ?? 0),
                                                                       AmtDr = Math.Abs((L.Amount < 0 ? L.Amount : 0) ?? 0),
                                                                       ReferenceDocLineId = L.LineTableId
                                                                   }).ToList();


            IEnumerable<LedgerPostingViewModel> LedgerCombind = LedgerHeaderAmtDr.Union(LedgerHeaderAmtCr).Union(LedgerLineAmtDr).Union(LedgerLineAmtCr).ToList();

            //IEnumerable<LedgerPostingViewModel> LedgerPost = (from L in LedgerCombind
            //                                                  select new LedgerPostingViewModel
            //                                                  {
            //                                                      LedgerAccountId = L.LedgerAccountId,
            //                                                      ContraLedgerAccountId = L.ContraLedgerAccountId,
            //                                                      CostCenterId = L.CostCenterId,
            //                                                      AmtDr = L.AmtDr,
            //                                                      AmtCr = L.AmtCr,
            //                                                      ReferenceDocLineId = L.ReferenceDocLineId
            //                                                  }).ToList();


            IEnumerable<LedgerPostingViewModel> TEmpLedger = (from L in LedgerCombind
                                                              group new { L } by new { L.LedgerAccountId, L.CostCenterId, L.ReferenceDocLineId } into Result
                                                              select new LedgerPostingViewModel
                                                              {
                                                                  LedgerAccountId = Result.Key.LedgerAccountId,
                                                                  ContraLedgerAccountId = Result.Max(i => i.L.ContraLedgerAccountId),
                                                                  CostCenterId = Result.Key.CostCenterId,
                                                                  AmtDr = Result.Sum(i => i.L.AmtDr),
                                                                  AmtCr = Result.Sum(i => i.L.AmtCr),
                                                                  ReferenceDocLineId = Result.Key.ReferenceDocLineId,
                                                              }).ToList();


            IEnumerable<LedgerPostingViewModel> LedgerPost = (from L in TEmpLedger
                                                              select new LedgerPostingViewModel
                                                              {
                                                                  LedgerAccountId = L.LedgerAccountId,
                                                                  ContraLedgerAccountId = L.ContraLedgerAccountId,
                                                                  CostCenterId = L.CostCenterId,
                                                                  AmtDr = (L.AmtDr - L.AmtCr) > 0 ? (L.AmtDr - L.AmtCr) : 0,
                                                                  AmtCr = (L.AmtCr - L.AmtDr) > 0 ? (L.AmtCr - L.AmtDr) : 0,
                                                                  ReferenceDocLineId = L.ReferenceDocLineId,
                                                              }).ToList();


            var temp = (from L in LedgerPost
                        group L by 1 into Result
                        select new
                        {
                            AmtDr = Result.Sum(i => i.AmtDr),
                            AmtCr = Result.Sum(i => i.AmtCr)
                        }).FirstOrDefault();


            if (temp != null)
            {
                if (temp.AmtDr != temp.AmtCr)
                {
                    throw new Exception("Debit amount and credit amount is not equal.Check account posting.");
                }
            }



            foreach (LedgerPostingViewModel item in LedgerPost)
            {
                Ledger Ledger = new Ledger();

                if (LedgerHeaderId != 0)
                {
                    Ledger.LedgerHeaderId = LedgerHeaderId;
                }
                Ledger.LedgerAccountId = item.LedgerAccountId;
                Ledger.ContraLedgerAccountId = item.ContraLedgerAccountId;


                var TempCostCenter = (from C in Context.CostCenter
                                      where C.CostCenterId == item.CostCenterId && C.LedgerAccountId == item.LedgerAccountId
                                      select new { CostCenterId = C.CostCenterId }).FirstOrDefault();

                if (TempCostCenter != null)
                {
                    Ledger.CostCenterId = item.CostCenterId;

                }



                Ledger.ReferenceDocLineId = item.ReferenceDocLineId;
                Ledger.ReferenceDocTypeId = LedgerHeaderViewModel.DocTypeId;



                Ledger.AmtDr = item.AmtDr * (LedgerHeaderViewModel.ExchangeRate ?? 1);
                Ledger.AmtCr = item.AmtCr * (LedgerHeaderViewModel.ExchangeRate ?? 1);
                Ledger.Narration = "";

                //new LedgerService(_unitOfWork).Create(Ledger);
                Ledger.ObjectState = Model.ObjectState.Added;
                Context.Ledger.Add(Ledger);
            }
        }

        public JsonResult GetJobWorkerDetailJson(int JobWorkerId)
        {
            return Json(new JobInvoiceHeaderService(_unitOfWork).GetJobWorkerDetail(JobWorkerId));
        }

    }



}
