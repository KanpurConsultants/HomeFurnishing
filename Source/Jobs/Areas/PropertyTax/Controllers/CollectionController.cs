using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using AutoMapper;
using System.Configuration;
using Presentation;
using Components.ExceptionHandlers;
using Services.PropertyTax;
using Service;
using Model.ViewModels;
using Jobs.Controllers;
using Presentation.ViewModels;
using Model.Models;
using Core.Common;
using Reports.Controllers;
using Model.ViewModel;
using Jobs.Helpers;
using Reports.Reports;

namespace Jobs.Areas.PropertyTax.Controllers
{

    [Authorize]
    public class CollectionController : System.Web.Mvc.Controller
    {
        List<string> UserRoles = new List<string>();

        bool TimePlanValidation = true;
        string ExceptionMsg = "";
        bool Continue = true;

        ICollectionService _CollectionService;
        IPropertyHeaderService _PropertyHeaderService;
        IDocumentTypeService _documentTypeService;
        ICollectionSettingsService _CollectionSettingsService;
        IExceptionHandler _exception;
        IProcessService _ProcessService;

        public CollectionController(ICollectionService CollectionService, IPropertyHeaderService PropertyHeaderService, IExceptionHandler exec, IDocumentTypeService DocumentTypeServ,
            ICollectionSettingsService CollectionSettingsServ,
            IProcessService ProcessService)
        {
            _CollectionService = CollectionService;
            _PropertyHeaderService = PropertyHeaderService;
            _exception = exec;
            _documentTypeService = DocumentTypeServ;
            _CollectionSettingsService = CollectionSettingsServ;
            _ProcessService = ProcessService;

            UserRoles = (List<string>)System.Web.HttpContext.Current.Session["Roles"];
        }

        // GET: /LedgerHeader/


        public ActionResult DocumentTypeIndex(int id)//DocumentCategoryId
        {
            var p = _documentTypeService.FindByDocumentCategory(id).ToList();

            if (p != null)
            {
                if (p.Count == 1)
                    return RedirectToAction("Index", new { id = p.FirstOrDefault().DocumentTypeId });
            }

            return View("DocumentTypeList", p);
        }


        public ActionResult PropertyIndex(int id)//DocumentTypeId 
        {
            int PersonId = DocumentTypeIdConstants.Property;
            IQueryable<PropertyHeaderViewModel> p = _PropertyHeaderService.GetPropertyHeaderList(PersonId, User.Identity.Name);
            ViewBag.Name = _documentTypeService.Find(id).DocumentTypeName;
            ViewBag.IndexStatus = "All";
            ViewBag.id = id;
            return View(p);
        }

        public ActionResult Index(int id, int PersonId, string IndexType)//DocumentTypeId 
        {
            if (IndexType == "PTS")
            {
                return RedirectToAction("Index_PendingToSubmit", new { id });
            }
            else if (IndexType == "PTR")
            {
                return RedirectToAction("Index_PendingToReview", new { id });
            }
            IQueryable<CollectionViewModel> p = _CollectionService.GetCollectionList(id, User.Identity.Name).Where(m => m.PersonId == PersonId);
            ViewBag.Name = _documentTypeService.Find(id).DocumentTypeName;
            PrepareViewBag(id);
            ViewBag.PendingToSubmit = PendingToSubmitCount(id);
            ViewBag.PendingToReview = PendingToReviewCount(id);
            ViewBag.IndexStatus = "All";
            ViewBag.id = id;
            ViewBag.PersonId = PersonId;
            return View(p);
        }

        public ActionResult Index_PendingToSubmit(int id, int PersonId)
        {
            IQueryable<CollectionViewModel> p = _CollectionService.GetCollectionListPendingToSubmit(id, User.Identity.Name).Where(m => m.PersonId == PersonId);

            PrepareViewBag(id);
            ViewBag.PendingToSubmit = PendingToSubmitCount(id);
            ViewBag.PendingToReview = PendingToReviewCount(id);
            ViewBag.IndexStatus = "PTS";
            ViewBag.PersonId = PersonId;
            return View("Index", p);
        }

        public ActionResult Index_PendingToReview(int id, int PersonId)
        {
            IQueryable<CollectionViewModel> p = _CollectionService.GetCollectionListPendingToReview(id, User.Identity.Name).Where(m => m.PersonId == PersonId);
            PrepareViewBag(id);
            ViewBag.PendingToSubmit = PendingToSubmitCount(id);
            ViewBag.PendingToReview = PendingToReviewCount(id);
            ViewBag.IndexStatus = "PTR";
            ViewBag.PersonId = PersonId;
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




        // GET: /LedgerHeader/Create

        public ActionResult Create(int id, int PersonId)//DocumentTypeId
        {
            CollectionViewModel p = new CollectionViewModel();

            PropertyHeaderViewModel propertyheader = _PropertyHeaderService.GetPropertyHeader(PersonId);
            p = Mapper.Map<PropertyHeaderViewModel, CollectionViewModel>(propertyheader);

            p.DocDate = DateTime.Now;
            p.CreatedDate = DateTime.Now;
            p.DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            p.SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            p.DocTypeId = id;
            p.AgentDocTypeId = DocumentTypeIdConstants.Agent;
            PrepareViewBag(id);

            var settings = _CollectionSettingsService.GetCollectionSettingsForDocument(id);
            p.CollectionSettings = Mapper.Map<CollectionSettings, CollectionSettingsViewModel>(settings);

            p.DocNo = _documentTypeService.FGetNewDocNo("DocNo", ConfigurationManager.AppSettings["DataBaseSchema"] + ".LedgerHeaders", p.DocTypeId, p.DocDate, p.DivisionId, p.SiteId);
            ViewBag.Mode = "Add";
            ViewBag.Name = _documentTypeService.Find(id).DocumentTypeName;
            ViewBag.id = id;

            return View(p);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Post(CollectionViewModel svm)
        {
            bool BeforeSave = true;


            var settings = _CollectionSettingsService.GetCollectionSettingsForDocument(svm.DocTypeId);

            if (settings != null)
            {
                if (settings.IsVisibleReason == true && (svm.ReferenceLedgerAccountId == null || svm.ReferenceLedgerAccountId == 0))
                {
                    ModelState.AddModelError("ReferenceLedgerAccountId", "Reason field is required");
                }
            }

            if (svm.PaymentModeId == null || svm.PaymentModeId == 0)
            {
                ModelState.AddModelError("PaymentModeId", "Payment Mode field is required");
            }


            #region DocTypeTimeLineValidation

            try
            {

                if (svm.LedgerHeaderId <= 0)
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
                if (svm.LedgerHeaderId <= 0)
                {
                    try
                    {
                        _CollectionService.Create(svm, User.Identity.Name);
                    }

                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        TempData["CSEXC"] += message;
                        PrepareViewBag(svm.DocTypeId);
                        ViewBag.Mode = "Add";
                        return View("Create", svm);
                    }

                    return RedirectToAction("Modify", "Collection", new { Id = svm.LedgerHeaderId }).Success("Data saved successfully");

                }
                #endregion


                //EditLogic
                #region EditRecord

                else
                {
                    try
                    {
                        _CollectionService.Update(svm, User.Identity.Name);
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

                    return RedirectToAction("Index", new { id = svm.DocTypeId, PersonId = svm.PersonId }).Success("Data saved successfully");
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
            LedgerHeader header = _CollectionService.Find(id);
            if (header.Status == (int)StatusConstants.Drafted || header.Status == (int)StatusConstants.Import)
                return Edit(id, IndexType);
            else
                return HttpNotFound();
        }

        [HttpGet]
        public ActionResult ModifyAfter_Submit(int id, string IndexType)
        {
            LedgerHeader header = _CollectionService.Find(id);
            if (header.Status == (int)StatusConstants.Submitted || header.Status == (int)StatusConstants.Modified)
                return Edit(id, IndexType);
            else
                return HttpNotFound();
        }

        [HttpGet]
        public ActionResult ModifyAfter_Approve(int id, string IndexType)
        {
            LedgerHeader header = _CollectionService.Find(id);
            if (header.Status == (int)StatusConstants.Approved)
                return Edit(id, IndexType);
            else
                return HttpNotFound();
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            LedgerHeader header = _CollectionService.Find(id);
            if (header.Status == (int)StatusConstants.Drafted || header.Status == (int)StatusConstants.Import)
                return Remove(id);
            else
                return HttpNotFound();
        }

        [HttpGet]
        public ActionResult DeleteAfter_Submit(int id)
        {
            LedgerHeader header = _CollectionService.Find(id);
            if (header.Status == (int)StatusConstants.Submitted || header.Status == (int)StatusConstants.Modified)
                return Remove(id);
            else
                return HttpNotFound();
        }

        [HttpGet]
        public ActionResult DeleteAfter_Approve(int id)
        {
            LedgerHeader header = _CollectionService.Find(id);
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



        // GET: /LedgerHeader/Edit/5
        private ActionResult Edit(int id, string IndexType)
        {
            ViewBag.IndexStatus = IndexType;
            CollectionViewModel s = _CollectionService.GetCollection(id);

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

            var settings = _CollectionSettingsService.GetCollectionSettingsForDocument(s.DocTypeId);
            s.CollectionSettings = Mapper.Map<CollectionSettings, CollectionSettingsViewModel>(settings);
            
            PrepareViewBag(s.DocTypeId);
            if (s == null)
            {
                return HttpNotFound();
            }

            ViewBag.Mode = "Edit";
            ViewBag.transactionType = "";

            ViewBag.Name = _documentTypeService.Find(s.DocTypeId).DocumentTypeName;
            ViewBag.id = s.DocTypeId;

            if (!(System.Web.HttpContext.Current.Request.UrlReferrer.PathAndQuery).Contains("Create"))
                _CollectionService.LogDetailInfo(s);

            return View("Create", s);
        }

        private ActionResult Remove(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LedgerHeader LedgerHeader = _CollectionService.Find(id);

            if (LedgerHeader == null)
            {
                return HttpNotFound();
            }

            #region DocTypeTimeLineValidation

            try
            {
                TimePlanValidation = DocumentValidation.ValidateDocument(Mapper.Map<DocumentUniqueId>(LedgerHeader), DocumentTimePlanTypeConstants.Delete, User.Identity.Name, out ExceptionMsg, out Continue);
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

            CollectionViewModel s = _CollectionService.GetCollection(id);

            var settings = _CollectionSettingsService.GetCollectionSettingsForDocument(s.DocTypeId);
            s.CollectionSettings = Mapper.Map<CollectionSettings, CollectionSettingsViewModel>(settings);



            PrepareViewBag(s.DocTypeId);
            if (s == null)
            {
                return HttpNotFound();
            }

            if (String.IsNullOrEmpty(transactionType) || transactionType == "detail")
                _CollectionService.LogDetailInfo(s);

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
                    _CollectionService.Delete(vm, User.Identity.Name);
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
            LedgerHeader s = _CollectionService.Find(id);

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

            LedgerHeader pd = _CollectionService.Find(Id);

            CollectionViewModel s = _CollectionService.GetCollection(Id);

            if (ModelState.IsValid)
            {

                if (User.Identity.Name == pd.ModifiedBy || UserRoles.Contains("Admin"))
                {
                    try
                    {
                        _CollectionService.Submit(Id, User.Identity.Name, GenGatePass, UserRemark);
                    }

                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        TempData["CSEXC"] += message;
                        return RedirectToAction("Index", new { id = pd.DocTypeId, PersonId = s.PersonId });
                    }
                }
                else
                    return RedirectToAction("Index", new { id = pd.DocTypeId, PersonId = s.PersonId, IndexType = IndexType }).Warning("Record can be submitted by user " + pd.ModifiedBy + " only.");
            }

            return RedirectToAction("Index", new { id = pd.DocTypeId, PersonId = s.PersonId, IndexType = IndexType });
        }





        public ActionResult Review(int id, string IndexType, string TransactionType)
        {
            return RedirectToAction("Detail", new { id = id, IndexType = IndexType, transactionType = string.IsNullOrEmpty(TransactionType) ? "review" : TransactionType });
        }


        [HttpPost, ActionName("Detail")]
        [MultipleButton(Name = "Command", Argument = "Review")]
        public ActionResult Reviewed(int Id, string IndexType, string UserRemark, string IsContinue)
        {
            LedgerHeader pd = _CollectionService.Find(Id);

            if (ModelState.IsValid)
            {

                _CollectionService.Review(Id, User.Identity.Name, UserRemark);

                string ReturnUrl = System.Configuration.ConfigurationManager.AppSettings["CurrentDomain"] + "/" + "Collection" + "/" + "Index" + "/" + pd.DocTypeId + "?IndexType=" + IndexType;

                return Redirect(ReturnUrl);
            }

            return RedirectToAction("Index", new { id = pd.DocTypeId, IndexType = IndexType }).Warning("Error in Reviewing.");
        }

        [HttpGet]
        public ActionResult Report(int id)
        {

            var Dt = _documentTypeService.Find(id);

            Dictionary<int, string> DefaultValue = new Dictionary<int, string>();

            if (!Dt.ReportMenuId.HasValue)
                throw new Exception("Report Menu not configured in document types");

            var menu = _CollectionService.GetMenu(Dt.ReportMenuId ?? 0);

            if (menu != null)
            {
                var header = _CollectionService.GetReportHeader(menu.MenuName);

                var Line = _CollectionService.GetReportLine("DocumentType", header.ReportHeaderId);
                if (Line != null)
                    DefaultValue.Add(Line.ReportLineId, id.ToString());
                var Site = _CollectionService.GetReportLine("Site", header.ReportHeaderId);
                if (Site != null)
                    DefaultValue.Add(Site.ReportLineId, ((int)System.Web.HttpContext.Current.Session["SiteId"]).ToString());
                var Division = _CollectionService.GetReportLine("Division", header.ReportHeaderId);
                if (Division != null)
                    DefaultValue.Add(Division.ReportLineId, ((int)System.Web.HttpContext.Current.Session["DivisionId"]).ToString());
            }

            TempData["ReportLayoutDefaultValues"] = DefaultValue;

            return Redirect((string)System.Configuration.ConfigurationManager.AppSettings["CustomizeDomain"] + "/Report_ReportPrint/ReportPrint/?MenuId=" + Dt.ReportMenuId);

        }


        public int PendingToSubmitCount(int id)
        {
            return (_CollectionService.GetCollectionListPendingToSubmit(id, User.Identity.Name)).Count();
        }

        public int PendingToReviewCount(int id)
        {
            return (_CollectionService.GetCollectionListPendingToReview(id, User.Identity.Name)).Count();
        }

        [HttpGet]
        public ActionResult NextPage(int DocId, int DocTypeId)//CurrentHeaderId
        {
            var nextId = _CollectionService.NextPrevId(DocId, DocTypeId, User.Identity.Name, PrevNextConstants.Next);
            return Edit(nextId, "");
        }
        [HttpGet]
        public ActionResult PrevPage(int DocId, int DocTypeId)//CurrentHeaderId
        {
            var PrevId = _CollectionService.NextPrevId(DocId, DocTypeId, User.Identity.Name, PrevNextConstants.Prev);
            return Edit(PrevId, "");
        }

        protected override void Dispose(bool disposing)
        {
            if (!string.IsNullOrEmpty((string)TempData["CSEXC"]))
            {
                //GenerateCookie.CreateNotificationCookie(NotificationTypeConstants.Danger, (string)TempData["CSEXC"]);
                //TempData.Remove("CSEXC");
            }
            if (disposing)
            {
                _CollectionService.Dispose();
            }
            base.Dispose(disposing);
        }



        public ActionResult GeneratePrints(string Ids, int DocTypeId)
        {

            if (!string.IsNullOrEmpty(Ids))
            {
                try
                {
                    var MergedPdf = GetReport(Ids, DocTypeId, User.Identity.Name);
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
            var Exists = (_CollectionService.CheckForDocNoExists(docno,doctypeId));
            return Json(new { returnvalue = Exists });
        }

        public JsonResult DuplicateCheckForEdit(string docno, int doctypeId, int headerid)
        {
            var Exists = (_CollectionService.CheckForDocNoExists(docno, headerid,doctypeId));
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

        public JsonResult GetIntrestBalanceJson(int PersonId)
        {
            var temp = _PropertyHeaderService.GetIntrestBalance(PersonId);
            return Json(temp);
        }

        public JsonResult GetArearBalanceJson(int PersonId)
        {
            var temp = _PropertyHeaderService.GetArearBalance(PersonId);
            return Json(temp);
        }

        public JsonResult GetExcessBalanceJson(int PersonId)
        {
            var temp = _PropertyHeaderService.GetExcessBalance(PersonId);
            return Json(temp);
        }


        public JsonResult GetCurrentYearBalanceJson(int PersonId)
        {
            var temp = _PropertyHeaderService.GetCurrentYearBalance(PersonId);
            return Json(temp);
        }

        public JsonResult GetNetOutstandingJson(int PersonId)
        {
            var temp = _PropertyHeaderService.GetNetOutstanding(PersonId);
            return Json(temp);
        }

        public ActionResult GetReasonAccount(string searchTerm, int pageSize, int pageNum)
        {
            return new JsonpResult
            {
                Data = _CollectionService.GetReasonAccount(searchTerm, pageSize, pageNum),
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        //[HttpGet]
        //public ActionResult Report()
        //{
        //    DocumentType Dt = _documentTypeService.Find(Constants.DocumentTypeIdConstants.Collection);
        //    return Redirect((string)System.Configuration.ConfigurationManager.AppSettings["CustomizeDomain"] + "/Report_ReportPrint/ReportPrint/?MenuId=" + Dt.ReportMenuId);
        //}

        public byte[] GetReport(string Ids, int DocTypeId, string UserName)
        {

            int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];

            var Settings = _CollectionSettingsService.GetCollectionSettingsForDocument(DocTypeId);

            string ReportSql = "";

            if (!string.IsNullOrEmpty(Settings.DocumentPrint))
                ReportSql = _CollectionService.GetReportHeader(Settings.DocumentPrint).ReportSQL;


            List<byte[]> PdfStream = new List<byte[]>();
            foreach (var item in Ids.Split(',').Select(Int32.Parse))
            {

                DirectReportPrint drp = new DirectReportPrint();

                var pd = _CollectionService.Find(item);

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
                        Pdf = drp.DirectDocumentPrint(Settings.SqlProcDocumentPrint, UserName, item);

                        PdfStream.Add(Pdf);
                    }
                    else if (pd.Status == (int)StatusConstants.Submitted || pd.Status == (int)StatusConstants.ModificationSubmitted)
                    {
                        Pdf = drp.DirectDocumentPrint(Settings.SqlProcDocumentPrint_AfterSubmit, UserName, item);

                        PdfStream.Add(Pdf);
                    }
                    else
                    {
                        Pdf = drp.DirectDocumentPrint(Settings.SqlProcDocumentPrint_AfterApprove, UserName, item);
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
