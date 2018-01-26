using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using AutoMapper;
using System.Configuration;
using Presentation;
using Components.ExceptionHandlers;
using Components.Logging;
using System.Xml.Linq;
using Services.PropertyTax;
using Service;
using Model.ViewModel;
using Core.Common;
using Model.ViewModels;
using Presentation.ViewModels;
using Model.Models;
using Jobs.Helpers;

namespace Jobs.Areas.PropertyTax.Controllers
{

    [Authorize]
    public class PaymentModeController : System.Web.Mvc.Controller
    {
        List<string> UserRoles = new List<string>();

        bool TimePlanValidation = true;
        string ExceptionMsg = "";
        bool Continue = true;

        private readonly ILogger _logger;
        IPaymentModeService _PaymentModeService;
        IPaymentModeLedgerAccountService _PaymentModeLedgerAccountService;
        IDocumentTypeService _DocumentTypeService;
        IExceptionHandler _exception;
        private readonly IModificationCheck _modificationCheck;

        private ActiivtyLogViewModel logVm = new ActiivtyLogViewModel();

        public PaymentModeController(IPaymentModeService PaymentModeService, IDocumentTypeService DocumentTypeService, IPaymentModeLedgerAccountService PaymentModeLedgerAccountService, IExceptionHandler exec,
            ILogger log, IModificationCheck modificationCheck)
        {
            _PaymentModeService = PaymentModeService;
            _PaymentModeLedgerAccountService = PaymentModeLedgerAccountService;
            _DocumentTypeService = DocumentTypeService;
            _exception = exec;
            _logger = log;
            _modificationCheck = modificationCheck;

            UserRoles = (List<string>)System.Web.HttpContext.Current.Session["Roles"];
            logVm.SessionId = 0;
            logVm.ControllerName = System.Web.HttpContext.Current.Request.RequestContext.RouteData.GetRequiredString("controller");
            logVm.ActionName = System.Web.HttpContext.Current.Request.RequestContext.RouteData.GetRequiredString("action");
            logVm.User = System.Web.HttpContext.Current.Request.RequestContext.HttpContext.User.Identity.Name;
        }

        // GET: /JobReceiveHeader/


       
        public ActionResult Index(string IndexType)
        {
            IQueryable<PaymentModeViewModel> PaymentMode = _PaymentModeService.GetPaymentModeListForIndex();
            return View(PaymentMode);
        }



        private void PrepareViewBag(int id)
        {

        }



        // GET: /JobReceiveHeader/Create

        public ActionResult Create()//DocumentTypeId
        {
            PaymentModeViewModel vm = new PaymentModeViewModel();
            vm.DocTypeId = DocumentTypeIdConstants.PaymentMode;
            ViewBag.Mode = "Add";
            return View("Create", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Post(PaymentModeViewModel vm)
        {
            int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];

            if (ModelState.IsValid)
            {
                if (vm.PaymentModeId <= 0)
                {
                    PaymentMode paymentmode = new PaymentMode();
                    paymentmode.PaymentModeName = vm.PaymentModeName;
                    paymentmode.DocTypeId = vm.DocTypeId;
                    paymentmode.IsActive = true;
                    paymentmode.CreatedDate = DateTime.Now;
                    paymentmode.ModifiedDate = DateTime.Now;
                    paymentmode.CreatedBy = User.Identity.Name;
                    paymentmode.ModifiedBy = User.Identity.Name;
                    paymentmode.ObjectState = Model.ObjectState.Added;



                    PaymentModeLedgerAccount paymentmodeledgeraccounts = new PaymentModeLedgerAccount();
                    paymentmodeledgeraccounts.PaymentModeId = paymentmode.PaymentModeId;
                    paymentmodeledgeraccounts.LedgerAccountId  = (int) vm.LedgerAccountId;
                    paymentmodeledgeraccounts.SiteId = SiteId;
                    paymentmodeledgeraccounts.DivisionId = DivisionId;
                    paymentmodeledgeraccounts.CreatedDate = DateTime.Now;
                    paymentmodeledgeraccounts.ModifiedDate = DateTime.Now;
                    paymentmodeledgeraccounts.CreatedBy = User.Identity.Name;
                    paymentmodeledgeraccounts.ModifiedBy = User.Identity.Name;
                    paymentmodeledgeraccounts.ObjectState = Model.ObjectState.Added;

                    try
                    {
                        _PaymentModeService.Create(paymentmode);
                        _PaymentModeLedgerAccountService.Create(paymentmodeledgeraccounts);
                    }



                    catch (Exception ex)
                    {
                        ViewBag.Mode = "Add";
                        string message = _exception.HandleException(ex);
                        ModelState.AddModelError("", message);
                        return View("Create", vm);
                    }



                    _logger.LogActivityDetail(logVm.Map(new ActiivtyLogViewModel
                    {
                        DocTypeId = DocumentTypeIdConstants.PaymentMode,
                        DocId = paymentmode.PaymentModeId,
                        ActivityType = (int)ActivityTypeContants.Added,
                    }));


                    return RedirectToAction("Create").Success("Data saved successfully");
                }

                else
                {
                    List<LogTypeViewModel> LogList = new List<LogTypeViewModel>();

                    PaymentMode paymentmode = _PaymentModeService.Find(vm.PaymentModeId);
                    
                    PaymentMode ExRec = Mapper.Map<PaymentMode>(paymentmode);

                    paymentmode.PaymentModeName = vm.PaymentModeName;
                    paymentmode.ModifiedDate = DateTime.Now;
                    paymentmode.ModifiedBy = User.Identity.Name;
                    paymentmode.ObjectState = Model.ObjectState.Modified;


                    

                    LogList.Add(new LogTypeViewModel
                    {
                        ExObj = ExRec,
                        Obj = paymentmode,
                    });

                    XElement Modifications = _modificationCheck.CheckChanges(LogList);


                    if (vm.PaymentModeLedgerAccountId == null || vm.PaymentModeLedgerAccountId == 0)
                    {
                        PaymentModeLedgerAccount paymentmodeledgeraccounts = new PaymentModeLedgerAccount();
                        paymentmodeledgeraccounts.PaymentModeId = paymentmode.PaymentModeId;
                        paymentmodeledgeraccounts.LedgerAccountId = (int)vm.LedgerAccountId;
                        paymentmodeledgeraccounts.SiteId = SiteId;
                        paymentmodeledgeraccounts.DivisionId = DivisionId;
                        paymentmodeledgeraccounts.CreatedDate = DateTime.Now;
                        paymentmodeledgeraccounts.ModifiedDate = DateTime.Now;
                        paymentmodeledgeraccounts.CreatedBy = User.Identity.Name;
                        paymentmodeledgeraccounts.ModifiedBy = User.Identity.Name;
                        paymentmodeledgeraccounts.ObjectState = Model.ObjectState.Added;
                        _PaymentModeLedgerAccountService.Create(paymentmodeledgeraccounts);
                    }
                    else
                    {
                        PaymentModeLedgerAccount paymentmodeledgeraccounts = _PaymentModeLedgerAccountService.Find((int)vm.PaymentModeLedgerAccountId);
                        paymentmodeledgeraccounts.LedgerAccountId = (int)vm.LedgerAccountId;
                        paymentmodeledgeraccounts.ModifiedDate = DateTime.Now;
                        paymentmodeledgeraccounts.ModifiedBy = User.Identity.Name;
                        paymentmodeledgeraccounts.ObjectState = Model.ObjectState.Modified;
                        _PaymentModeLedgerAccountService.Update(paymentmodeledgeraccounts);
                    }

                    try
                    {
                        _PaymentModeService.Update(paymentmode);
                    }

                    catch (Exception ex)
                    {
                        ViewBag.Mode = "Edit";
                        string message = _exception.HandleException(ex);
                        ModelState.AddModelError("", message);
                        return View("Create", vm);
                    }

                    _logger.LogActivityDetail(logVm.Map(new ActiivtyLogViewModel
                    {
                        DocTypeId = DocumentTypeIdConstants.PaymentMode,
                        DocId = paymentmode.PaymentModeId,
                        ActivityType = (int)ActivityTypeContants.Modified,
                        xEModifications = Modifications,
                    }));

                    return RedirectToAction("Index").Success("Data saved successfully");

                }

            }
            return View("Create", vm);
        }


        [HttpGet]
        public ActionResult Modify(int id, string IndexType)
        {
            PaymentModeViewModel pt = _PaymentModeService.GetPaymentModeListForEdit(id);
            if (pt == null)
            {
                return HttpNotFound();
            }
            ViewBag.Mode = "Edit";
            return View("Create", pt);
        }

       

        [HttpGet]
        public ActionResult Delete(int id)
        {
            PaymentMode header = _PaymentModeService.Find(id);
            return Remove(id);
        }





        // GET: /JobReceiveHeader/Edit/5
        private ActionResult Edit(int id, string IndexType)
        {
            PaymentModeViewModel pt = _PaymentModeService.GetPaymentModeListForEdit(id);
            if (pt == null)
            {
                return HttpNotFound();
            }
            ViewBag.Mode = "Edit";
            return View("Create", pt);
        }

        private ActionResult Remove(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PaymentMode PaymentMode = _PaymentModeService.Find(id);

            if (PaymentMode == null)
            {
                return HttpNotFound();
            }

            

            ReasonViewModel rvm = new ReasonViewModel()
            {
                id = id,
            };
            return PartialView("_Reason", rvm);
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
                    List<LogTypeViewModel> LogList = new List<LogTypeViewModel>();

                    PaymentMode temp = _PaymentModeService.Find(vm.id);
                    int DocTypeId = (int)temp.DocTypeId;

                    LogList.Add(new LogTypeViewModel
                    {
                        ExObj = Mapper.Map<PaymentMode>(temp),
                    });

                    XElement Modifications = _modificationCheck.CheckChanges(LogList);

                    _PaymentModeService.Delete(vm.id);

                    _logger.LogActivityDetail(logVm.Map(new ActiivtyLogViewModel
                    {
                        DocTypeId = DocTypeId,
                        DocId = temp.PaymentModeId,
                        ActivityType = (int)ActivityTypeContants.Deleted,
                        UserRemark = vm.Reason,
                        DocNo = temp.PaymentModeName,
                        xEModifications = Modifications,
                        DocDate = temp.CreatedDate,
                    }));
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



        [HttpGet]
        public ActionResult NextPage(int DocId)//CurrentHeaderId
        {
            var nextId = _PaymentModeService.NextId(DocId);
            return Edit(nextId, "");
        }
        [HttpGet]
        public ActionResult PrevPage(int DocId)//CurrentHeaderId
        {
            var PrevId = _PaymentModeService.PrevId(DocId);
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
                _PaymentModeService.Dispose();
            }
            base.Dispose(disposing);
        }



        public JsonResult DuplicateCheckForCreate(string docno, int doctypeId)
        {
            var Exists = (_PaymentModeService.CheckForNameExists(docno));
            return Json(new { returnvalue = Exists });
        }

        public JsonResult DuplicateCheckForEdit(string docno, int doctypeId, int headerid)
        {
            var Exists = (_PaymentModeService.CheckForNameExists(docno, headerid));
            return Json(new { returnvalue = Exists });
        }

        [HttpGet]
        public ActionResult Report()
        {
            DocumentType Dt = _DocumentTypeService.Find(DocumentTypeIdConstants.PaymentMode);
            return Redirect((string)System.Configuration.ConfigurationManager.AppSettings["CustomizeDomain"] + "/Report_ReportPrint/ReportPrint/?MenuId=" + Dt.ReportMenuId);
        }
    }
}
