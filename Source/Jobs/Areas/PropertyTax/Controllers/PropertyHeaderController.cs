using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Model.Models;
using Data.Models;
using Service;
using Data.Infrastructure;
using Presentation;
using Presentation.ViewModels;
using Model.ViewModels;
using Core.Common;
using System.Configuration;
using System.IO;
using ImageResizer;
using System.Data.SqlClient;
using Model.ViewModel;
using AutoMapper;
using Jobs.Helpers;
using Services.PropertyTax;
using Components.ExceptionHandlers;
using Jobs.Controllers;
using System.Net;
using Reports.Controllers;
using System.Data;
using Reports.Reports;

namespace Jobs.Areas.PropertyTax.Controllers
{

    [Authorize]
    public class PropertyHeaderController : System.Web.Mvc.Controller
    {
        List<string> UserRoles = new List<string>();
        private ApplicationDbContext db = new ApplicationDbContext();

        bool TimePlanValidation = true;
        string ExceptionMsg = "";
        bool Continue = true;

        IPropertyHeaderService _PropertyHeaderService;
        IDocumentTypeService _documentTypeService;
        IExceptionHandler _exception;
        IProcessService _ProcessService;
        IGodownService _GodownService;

        public PropertyHeaderController(IPropertyHeaderService PropertyHeaderService, IExceptionHandler exec, IDocumentTypeService DocumentTypeServ,
            IProcessService ProcessService,
            IGodownService GodownService)
        {
            _PropertyHeaderService = PropertyHeaderService;
            _exception = exec;
            _documentTypeService = DocumentTypeServ;
            _ProcessService = ProcessService;
            _GodownService = GodownService;

            UserRoles = (List<string>)System.Web.HttpContext.Current.Session["Roles"];
        }

        // GET: /Person/


        //public ActionResult DocumentTypeIndex(int id)//DocumentCategoryId
        //{
        //    var p = _documentTypeService.FindByDocumentCategory(id).ToList();

        //    if (p != null)
        //    {
        //        if (p.Count == 1)
        //            return RedirectToAction("Index", new { id = p.FirstOrDefault().DocumentTypeId });
        //    }

        //    return View("DocumentTypeList", p);
        //}

        public ActionResult DocumentTypeIndex(int id)//DocumentCategoryId
        {
            var p = _documentTypeService.FindByDocumentCategory(id).ToList();

            if (p != null)
            {
                if (p.Count == 1)
                    return RedirectToAction("WardIndex", new { id = p.FirstOrDefault().DocumentTypeId });
            }

            return View("DocumentTypeList", p);
        }

        public ActionResult WardIndex(int id)//DocumentTypeId 
        {
            var SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];

            IQueryable<WardIndexViewModel> Godown = _PropertyHeaderService.GetGodownListForIndex(SiteId);
            ViewBag.id = id;
            return View(Godown);
        }



        public ActionResult Index(int id, int GodownId, string IndexType)//DocumentTypeId 
        {
            if (IndexType == "PTS")
            {
                return RedirectToAction("Index_PendingToSubmit", new { id = id, GodownId = GodownId });
            }
            else if (IndexType == "PTR")
            {
                return RedirectToAction("Index_PendingToReview", new { id = id, GodownId = GodownId });
            }
            //IQueryable<PropertyHeaderViewModel> p = _PropertyHeaderService.GetPropertyHeaderList(id, GodownId, User.Identity.Name);
            IEnumerable<PropertyHeaderViewModel> p = _PropertyHeaderService.GetPropertyHeaderList(id, GodownId, User.Identity.Name);
            ViewBag.Name = _documentTypeService.Find(id).DocumentTypeName;
            PrepareViewBag(id);
            ViewBag.PendingToSubmit = PendingToSubmitCount(id, GodownId);
            ViewBag.PendingToReview = PendingToReviewCount(id, GodownId);
            ViewBag.IndexStatus = "All";
            ViewBag.id = id;
            ViewBag.GodownId = GodownId;
            return View(p);
        }

        public ActionResult Index_PendingToSubmit(int id, int GodownId)
        {
            IQueryable<PropertyHeaderViewModel> p = _PropertyHeaderService.GetPropertyHeaderListPendingToSubmit(id, GodownId, User.Identity.Name);

            PrepareViewBag(id);
            ViewBag.PendingToSubmit = PendingToSubmitCount(id, GodownId);
            ViewBag.PendingToReview = PendingToReviewCount(id, GodownId);
            ViewBag.IndexStatus = "PTS";
            ViewBag.GodownId = GodownId;
            return View("Index", p);
        }

        public ActionResult Index_PendingToReview(int id, int GodownId)
        {
            IQueryable<PropertyHeaderViewModel> p = _PropertyHeaderService.GetPropertyHeaderListPendingToReview(id, GodownId, User.Identity.Name);
            PrepareViewBag(id);
            ViewBag.PendingToSubmit = PendingToSubmitCount(id, GodownId);
            ViewBag.PendingToReview = PendingToReviewCount(id, GodownId);
            ViewBag.IndexStatus = "PTR";
            ViewBag.GodownId = GodownId;
            return View("Index", p);
        }

        private void PrepareViewBag(int id)
        {
            var DocType = _documentTypeService.Find(id);
            int Cid = DocType.DocumentCategoryId;
            ViewBag.DocTypeList = _documentTypeService.FindByDocumentCategory(Cid).ToList();
            ViewBag.Name = DocType.DocumentTypeName;
            ViewBag.id = id;

            var DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            var SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];

        }




        // GET: /Person/Create

        public ActionResult Create(int id, int GodownId)//DocumentTypeId
        {
            PropertyHeaderViewModel p = new PropertyHeaderViewModel();

            p.CreatedDate = DateTime.Now;
            p.DivisionIds = System.Web.HttpContext.Current.Session["DivisionId"].ToString();
            p.SiteIds = System.Web.HttpContext.Current.Session["SiteId"].ToString();
            p.DocTypeId = id;
            PrepareViewBag(id);
            List<DocumentTypeAttributeViewModel> tem = _PropertyHeaderService.GetAttributeForDocumentType(id).ToList();
            p.DocumentTypeAttributes = tem;

            //p.Code = _PropertyHeaderService.FGetNewPersonCode(p.DocTypeId);
            ViewBag.Mode = "Add";
            ViewBag.Name = _documentTypeService.Find(id).DocumentTypeName;
            ViewBag.id = id;
            ViewBag.GodownId = GodownId;
            p.GodownId = GodownId;
            return View(p);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Post(PropertyHeaderViewModel svm)
        {
            bool BeforeSave = true;

            int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            svm.Suffix = svm.Code;

            #region DocTypeTimeLineValidation

            try
            {

                if (svm.PersonID <= 0)
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



            if (ModelState.IsValid && BeforeSave && (TimePlanValidation || Continue))
            {
                //CreateLogic
                #region CreateRecord
                if (svm.PersonID <= 0)
                {
                    try
                    {
                        svm.Code = _PropertyHeaderService.FGetNewPersonCode(SiteId, svm.GodownId, svm.BinLocationId);
                        
                        _PropertyHeaderService.Create(svm, User.Identity.Name);
                    }

                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        TempData["CSEXC"] += message;
                        PrepareViewBag(svm.DocTypeId);
                        ViewBag.Mode = "Add";
                        return View("Create", svm);
                    }

                    return RedirectToAction("Modify", "PropertyHeader", new { Id = svm.PersonID }).Success("Data saved successfully");

                }
                #endregion


                //EditLogic
                #region EditRecord

                else
                {
                    try
                    {
                        _PropertyHeaderService.Update(svm, User.Identity.Name);
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

                    return RedirectToAction("Index", new { id = svm.DocTypeId, GodownId = svm.GodownId }).Success("Data saved successfully");

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
            Person header = _PropertyHeaderService.Find(id);
            if (header.Status == (int)StatusConstants.Drafted || header.Status == (int)StatusConstants.Import)
                return Edit(id, IndexType);
            else
                return HttpNotFound();
        }

        [HttpGet]
        public ActionResult ModifyAfter_Submit(int id, string IndexType)
        {
            Person header = _PropertyHeaderService.Find(id);
            if (header.Status == (int)StatusConstants.Submitted || header.Status == (int)StatusConstants.Modified)
                return Edit(id, IndexType);
            else
                return HttpNotFound();
        }

        [HttpGet]
        public ActionResult ModifyAfter_Approve(int id, string IndexType)
        {
            Person header = _PropertyHeaderService.Find(id);
            if (header.Status == (int)StatusConstants.Approved)
                return Edit(id, IndexType);
            else
                return HttpNotFound();
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            Person header = _PropertyHeaderService.Find(id);
            if (header.Status == (int)StatusConstants.Drafted || header.Status == (int)StatusConstants.Import)
                return Remove(id);
            else
                return HttpNotFound();
        }

        [HttpGet]
        public ActionResult DeleteAfter_Submit(int id)
        {
            Person header = _PropertyHeaderService.Find(id);
            if (header.Status == (int)StatusConstants.Submitted || header.Status == (int)StatusConstants.Modified)
                return Remove(id);
            else
                return HttpNotFound();
        }

        [HttpGet]
        public ActionResult DeleteAfter_Approve(int id)
        {
            Person header = _PropertyHeaderService.Find(id);
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



        // GET: /Person/Edit/5
        private ActionResult Edit(int id, string IndexType)
        {
            ViewBag.IndexStatus = IndexType;
            PropertyHeaderViewModel s = _PropertyHeaderService.GetPropertyHeader(id);

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


            List<DocumentTypeAttributeViewModel> tem = _PropertyHeaderService.GetAttributeForPerson(id).ToList();
            s.DocumentTypeAttributes = tem;
            
            PrepareViewBag(s.DocTypeId);
            if (s == null)
            {
                return HttpNotFound();
            }

            ViewBag.Mode = "Edit";
            ViewBag.transactionType = "";

            ViewBag.Name = _documentTypeService.Find(s.DocTypeId).DocumentTypeName;
            ViewBag.id = s.DocTypeId;


            ViewBag.CollectionDocTypeId = DocumentTypeIdConstants.Collection;

            //var AttachmentList = (from p in db.DocumentAttachment.AsNoTracking()
            //                      where p.DocId == id && p.DocTypeId == s.DocTypeId
            //                   select p).ToList();
            //ViewBag.AttachmentList = Json(AttachmentList);

            string[] proinfo = new string[] { "a", "b" };
            ViewBag.ProInfo = proinfo;



            if (!(System.Web.HttpContext.Current.Request.UrlReferrer.PathAndQuery).Contains("Create"))
                _PropertyHeaderService.LogDetailInfo(s);

            return View("Create", s);
        }

        private ActionResult Remove(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Person Person = _PropertyHeaderService.Find(id);

            if (Person == null)
            {
                return HttpNotFound();
            }

            #region DocTypeTimeLineValidation

            try
            {
                TimePlanValidation = DocumentValidation.ValidateDocument(Mapper.Map<DocumentUniqueId>(Person), DocumentTimePlanTypeConstants.Delete, User.Identity.Name, out ExceptionMsg, out Continue);
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

            PropertyHeaderViewModel s = _PropertyHeaderService.GetPropertyHeader(id);


            PrepareViewBag(s.DocTypeId);
            if (s == null)
            {
                return HttpNotFound();
            }

            if (String.IsNullOrEmpty(transactionType) || transactionType == "detail")
                _PropertyHeaderService.LogDetailInfo(s);

            return View("Create", s);
        }

        // POST: /PurchaseOrderHeader/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(ReasonViewModel vm)
        {
            if (ModelState.IsValid)
            {
                //Commit the DB
                try
                {
                    _PropertyHeaderService.Delete(vm, User.Identity.Name);
                }

                catch (Exception ex)
                {
                    string message = _exception.HandleException(ex);
                    TempData["CSEXC"] += message;
                    return PartialView("_Reason", vm);
                }

                return Json(new { success = true });
            }
            return PartialView("_Reason", vm);
        }


        public ActionResult Submit(int id, string IndexType, string TransactionType)
        {

            #region DocTypeTimeLineValidation
            Person s = _PropertyHeaderService.Find(id);

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

            Person pd = _PropertyHeaderService.Find(Id);
            PropertyHeaderViewModel PropertyHeader = _PropertyHeaderService.GetPropertyHeader(Id);


            if (ModelState.IsValid)
            {

                if (User.Identity.Name == pd.ModifiedBy || UserRoles.Contains("Admin"))
                {
                    try
                    {
                        _PropertyHeaderService.Submit(Id, User.Identity.Name, GenGatePass, UserRemark);
                    }

                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        TempData["CSEXC"] += message;
                        return RedirectToAction("Index", new { id = pd.DocTypeId, GodownId = PropertyHeader.GodownId });
                    }
                }
                else
                    return RedirectToAction("Index", new { id = pd.DocTypeId, GodownId = PropertyHeader.GodownId, IndexType = IndexType }).Warning("Record can be submitted by user " + pd.ModifiedBy + " only.");
            }

            return RedirectToAction("Index", new { id = pd.DocTypeId, GodownId = PropertyHeader.GodownId, IndexType = IndexType });
        }





        public ActionResult Review(int id, string IndexType, string TransactionType)
        {
            return RedirectToAction("Detail", new { id = id, IndexType = IndexType, transactionType = string.IsNullOrEmpty(TransactionType) ? "review" : TransactionType });
        }


        [HttpPost, ActionName("Detail")]
        [MultipleButton(Name = "Command", Argument = "Review")]
        public ActionResult Reviewed(int Id, string IndexType, string UserRemark, string IsContinue)
        {
            Person pd = _PropertyHeaderService.Find(Id);
            PropertyHeaderViewModel PropertyHeader = _PropertyHeaderService.GetPropertyHeader(Id);

            if (ModelState.IsValid)
            {

                _PropertyHeaderService.Review(Id, User.Identity.Name, UserRemark);

                string ReturnUrl = System.Configuration.ConfigurationManager.AppSettings["CurrentDomain"] + "/" + "PropertyHeader" + "/" + "Index" + "/" + pd.DocTypeId + "?GodownId=" + PropertyHeader.GodownId + "?IndexType=" + IndexType;

                return Redirect(ReturnUrl);
            }

            return RedirectToAction("Index", new { id = pd.DocTypeId, GodownId = PropertyHeader.GodownId, IndexType = IndexType }).Warning("Error in Reviewing.");
        }

        [HttpGet]
        public ActionResult Report(int id)
        {

            var Dt = _documentTypeService.Find(id);

            Dictionary<int, string> DefaultValue = new Dictionary<int, string>();

            if (!Dt.ReportMenuId.HasValue)
                throw new Exception("Report Menu not configured in document types");

            var menu = _PropertyHeaderService.GetMenu(Dt.ReportMenuId ?? 0);

            if (menu != null)
            {
                var header = _PropertyHeaderService.GetReportHeader(menu.MenuName);

                var Line = _PropertyHeaderService.GetReportLine("DocumentType", header.ReportHeaderId);
                if (Line != null)
                    DefaultValue.Add(Line.ReportLineId, id.ToString());
                var Site = _PropertyHeaderService.GetReportLine("Site", header.ReportHeaderId);
                if (Site != null)
                    DefaultValue.Add(Site.ReportLineId, ((int)System.Web.HttpContext.Current.Session["SiteId"]).ToString());
                var Division = _PropertyHeaderService.GetReportLine("Division", header.ReportHeaderId);
                if (Division != null)
                    DefaultValue.Add(Division.ReportLineId, ((int)System.Web.HttpContext.Current.Session["DivisionId"]).ToString());
            }

            TempData["ReportLayoutDefaultValues"] = DefaultValue;

            return Redirect((string)System.Configuration.ConfigurationManager.AppSettings["CustomizeDomain"] + "/Report_ReportPrint/ReportPrint/?MenuId=" + Dt.ReportMenuId);

        }


        public int PendingToSubmitCount(int id, int GodownId)
        {
            return (_PropertyHeaderService.GetPropertyHeaderListPendingToSubmit(id, GodownId, User.Identity.Name)).Count();
        }

        public int PendingToReviewCount(int id, int GodownId)
        {
            return (_PropertyHeaderService.GetPropertyHeaderListPendingToReview(id, GodownId, User.Identity.Name)).Count();
        }

        [HttpGet]
        public ActionResult NextPage(int DocId, int DocTypeId)//CurrentHeaderId
        {
            var nextId = _PropertyHeaderService.NextPrevId(DocId, DocTypeId, User.Identity.Name, PrevNextConstants.Next);
            return Edit(nextId, "");
        }
        [HttpGet]
        public ActionResult PrevPage(int DocId, int DocTypeId)//CurrentHeaderId
        {
            var PrevId = _PropertyHeaderService.NextPrevId(DocId, DocTypeId, User.Identity.Name, PrevNextConstants.Prev);
            return Edit(PrevId, "");
        }

        protected override void Dispose(bool disposing)
        {
            if (!string.IsNullOrEmpty((string)TempData["CSEXC"]))
            {
                //GenerateCookie.CreateNotificationCookie(NotificationTypeConstants.Danger, (string)TempData["CSEXC"]);
                TempData.Remove("CSEXC");
            }
            if (disposing)
            {
                _PropertyHeaderService.Dispose();
            }
            base.Dispose(disposing);
        }



        public ActionResult GeneratePrints(string Ids, int DocTypeId)
        {

            if (!string.IsNullOrEmpty(Ids))
            {
                try
                {
                    var MergedPdf = GetReport(Ids, DocTypeId, User.Identity.Name, "Web.spRep_PropertyPrint");
                    return File(MergedPdf, "application/pdf");
                }
                catch (Exception ex)
                {
                    string message = _exception.HandleException(ex);
                    return Json(new { success = "Error", data = message }, JsonRequestBehavior.AllowGet);
                }

            }
            return Json(new { success = "Error", data = "No Records Selected." }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult GenerateInvoicePrints(string Ids, int DocTypeId)
        {

            if (!string.IsNullOrEmpty(Ids))
            {
                try
                {
                    var MergedPdf = GetReport(Ids, DocTypeId, User.Identity.Name, "Web.spRep_InvoicePrint");
                    return File(MergedPdf, "application/pdf");
                }
                catch (Exception ex)
                {
                    string message = _exception.HandleException(ex);
                    return Json(new { success = "Error", data = message }, JsonRequestBehavior.AllowGet);
                }

            }
            return Json(new { success = "Error", data = "No Records Selected." }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult DuplicateCheckForCreate(string docno, int doctypeId)
        {
            var Exists = (_PropertyHeaderService.CheckForDocNoExists(docno,doctypeId));
            return Json(new { returnvalue = Exists });
        }

        public JsonResult DuplicateCheckForEdit(string docno, int doctypeId, int headerid)
        {
            var Exists = (_PropertyHeaderService.CheckForDocNoExists(docno, headerid,doctypeId));
            return Json(new { returnvalue = Exists });
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

        public JsonResult GetHouseNoJson(int GodownId, int BinLocationId)
        {
            int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];

            var temp = _PropertyHeaderService.FGetNewHouseNo(SiteId,GodownId, BinLocationId);

            if (temp != null)
            {
                return Json(temp);
            }
            else
            {
                return null;
            }
        }

        public JsonResult GetAttachmentJson(int DocId, int DocTypeId)
        {
            var AttachmentList = (from p in db.DocumentAttachment.AsNoTracking()
                                  where p.DocId == DocId && p.DocTypeId == DocTypeId
                                  select p).ToList();
            if (AttachmentList != null)
            {
                return Json(AttachmentList);
            }
            else
            {
                return null;
            }
        }

        public ActionResult GetPersonWithDocType(string searchTerm, int pageSize, int pageNum, int filter)
        {
            return new JsonpResult
            {
                Data = _PropertyHeaderService.GetPersonWithDocType(searchTerm, pageSize, pageNum, filter),
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }




        public ActionResult GetProperty(string searchTerm, int pageSize, int pageNum)
        {
            return new JsonpResult
            {
                Data = _PropertyHeaderService.GetProperty(searchTerm, pageSize, pageNum),
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }


        //[HttpGet]
        //public ActionResult Report()
        //{
        //    DocumentType Dt = _documentTypeService.Find(Constants.DocumentTypeIdConstants.Godown);
        //    return Redirect((string)System.Configuration.ConfigurationManager.AppSettings["CustomizeDomain"] + "/Report_ReportPrint/ReportPrint/?MenuId=" + Dt.ReportMenuId);
        //}

        public ActionResult GIS()
        {
            return Redirect(System.Configuration.ConfigurationManager.AppSettings["PropertyTaxGISURL"]);
        }

        [HttpPost]
        public JsonResult AttachDocument(int DocId, int DocTypeId)
        {
            if (Request.Files[0] != null && Request.Files[0].ContentLength > 0)
            {
                UploadDocument ud = new UploadDocument();
                string FileNames = "";
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    string FileName = "";
                    string FolderName = "";

                    ud.UploadFile(i, new Dictionary<string, string>(), DocTypeId + "_" + DocId, FileTypeConstants.Other, out FolderName, out FileName);

                    DocumentAttachment da = new DocumentAttachment();

                    da.DocId = DocId;
                    da.DocTypeId = DocTypeId;
                    da.FileFolderName = FolderName;
                    da.FileName = FileName;
                    da.CreatedBy = User.Identity.Name;
                    da.CreatedDate = DateTime.Now;
                    da.ModifiedBy = User.Identity.Name;
                    da.ModifiedDate = DateTime.Now;
                    da.ObjectState = Model.ObjectState.Added;
                    db.DocumentAttachment.Add(da);

                    FileNames += da.FileName + ", ";

                }

                db.SaveChanges();



            }

            return Json("");
            //return Edit(DocId, "");
            //return RedirectToAction("Modify", "PropertyHeader", new { Id = DocId }).Success("Attachments saved successfully");
            //return Redirect(Request.Url.AbsoluteUri);

            //DocumentAttachmentViewModel vm = new DocumentAttachmentViewModel();
            //vm.DocId = DocId;
            //vm.DocTypeId = DocTypeId;
            //IExceptionHandlingService exec = null ;
            //new DocumentAttachmentController(exec).AttachDocument(vm);

            //return RedirectToAction("AttachDocument", new { DocId = vm.DocId, DocTypeId = vm.DocTypeId });
        }


        public byte[] GetReport(string Ids, int DocTypeId, string UserName, string SqlProc)
        {

            int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];

            //var Settings = new PersonSettingService(_unitOfWork).GetJobOrderSettingsForDocument(DocTypeId, DivisionId, SiteId);

            string DocumentPrint = "";
            //string SqlProcDocumentPrint = "Web.spRep_PropertyPrint";
            //string SqlProcDocumentPrint_AfterSubmit = "Web.spRep_PropertyPrint";
            //string SqlProcDocumentPrint_AfterApprove = "Web.spRep_PropertyPrint";

            //string SqlProcDocumentPrint = "Web.spRep_InvoicePrint";
            //string SqlProcDocumentPrint_AfterSubmit = "Web.spRep_InvoicePrint";
            //string SqlProcDocumentPrint_AfterApprove = "Web.spRep_InvoicePrint";

            string SqlProcDocumentPrint = SqlProc;
            string SqlProcDocumentPrint_AfterSubmit = SqlProc;
            string SqlProcDocumentPrint_AfterApprove = SqlProc;

            string ReportSql = "";

            if (!string.IsNullOrEmpty(DocumentPrint))
                ReportSql = _PropertyHeaderService.GetReportHeader(DocumentPrint).ReportSQL;


            List<byte[]> PdfStream = new List<byte[]>();
            foreach (var item in Ids.Split(',').Select(Int32.Parse))
            {

                DirectReportPrint drp = new DirectReportPrint();

                var pd = _PropertyHeaderService.Find(item);

                byte[] Pdf;

                if (!string.IsNullOrEmpty(ReportSql))
                {
                    Pdf = drp.rsDirectDocumentPrint(ReportSql, UserName, item);
                    PdfStream.Add(Pdf);
                }
                else
                {
                    if (pd.Status == (int)StatusConstants.Drafted || pd.Status == (int)StatusConstants.Import || pd.Status == (int)StatusConstants.Modified)
                    {
                        Pdf = drp.DirectDocumentPrint(SqlProcDocumentPrint, UserName, item);

                        PdfStream.Add(Pdf);
                    }
                    else if (pd.Status == (int)StatusConstants.Submitted || pd.Status == (int)StatusConstants.ModificationSubmitted)
                    {
                        Pdf = drp.DirectDocumentPrint(SqlProcDocumentPrint_AfterSubmit, UserName, item);

                        PdfStream.Add(Pdf);
                    }
                    else
                    {
                        Pdf = drp.DirectDocumentPrint(SqlProcDocumentPrint_AfterApprove, UserName, item);
                        PdfStream.Add(Pdf);
                    }
                }
            }

            PdfMerger pm = new PdfMerger();

            byte[] Merge = pm.MergeFiles(PdfStream);

            return Merge;
        }

    }


}
