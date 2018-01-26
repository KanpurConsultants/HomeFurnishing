using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using AutoMapper;
using System.Configuration;
using Presentation;
using Components.ExceptionHandlers;
using Services.BasicSetup;
using Models.Customize.ViewModels;
using Models.Customize.Models;
using Services.Customize;
using TimePlanValidator;
using TimePlanValidator.ViewModels;
using TimePlanValidator.Common;
using ProjLib.Constants;
using Models.BasicSetup.ViewModels;
using CookieNotifier;
using Presentation.Helper;
using Models.Company.Models;
using Components.Logging;
using System.Xml.Linq;
using Models.BasicSetup.Models;
using Models.Customize;

namespace Customize.Controllers
{

    [Authorize]
    public class DiscountTypeController : System.Web.Mvc.Controller
    {
        List<string> UserRoles = new List<string>();

        bool TimePlanValidation = true;
        string ExceptionMsg = "";
        bool Continue = true;

        private readonly ILogger _logger;
        IDiscountTypeService _DiscountTypeService;
        IDocumentTypeService _DocumentTypeService;
        IExceptionHandler _exception;
        IDocumentValidation _documentValidation;
        private readonly IModificationCheck _modificationCheck;

        private ActiivtyLogViewModel logVm = new ActiivtyLogViewModel();

        public DiscountTypeController(IDiscountTypeService DiscountTypeService, IDocumentTypeService DocumentTypeService, IExceptionHandler exec,
            ILogger log, IModificationCheck modificationCheck, IDocumentValidation DocValidation)
        {
            _DiscountTypeService = DiscountTypeService;
            _DocumentTypeService = DocumentTypeService;
            _exception = exec;
            _documentValidation = DocValidation;
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
            IQueryable<DiscountType> DiscountType = _DiscountTypeService.GetDiscountTypeListForIndex();
            return View(DiscountType);
        }



        private void PrepareViewBag(int id)
        {

        }



        // GET: /JobReceiveHeader/Create

        public ActionResult Create()//DocumentTypeId
        {
            DiscountType vm = new DiscountType();
            vm.DocTypeId = Constants.DocumentTypeIdConstants.DiscountType;
            vm.IsActive = true;
            ViewBag.Mode = "Add";
            return View("Create", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Post(DiscountType vm)
        {
            DiscountType pt = vm;
            
            if (vm.Rate > 100)
                ModelState.AddModelError("Rate", "Rate % value can not be greater then 100.");


            if (ModelState.IsValid)
            {
                if (vm.DiscountTypeId <= 0)
                {
                    pt.Rate = vm.Rate;
                    pt.IsActive = true;
                    pt.CreatedDate = DateTime.Now;
                    pt.ModifiedDate = DateTime.Now;
                    pt.CreatedBy = User.Identity.Name;
                    pt.ModifiedBy = User.Identity.Name;
                    pt.ObjectState = Model.ObjectState.Added;
                    

                    try
                    {
                        _DiscountTypeService.Create(pt);
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
                        DocTypeId = Constants.DocumentTypeIdConstants.DiscountType,
                        DocId = pt.DiscountTypeId,
                        ActivityType = (int)ActivityTypeContants.Added,
                    }));


                    return RedirectToAction("Create").Success("Data saved successfully");
                }

                else
                {
                    List<LogTypeViewModel> LogList = new List<LogTypeViewModel>();

                    DiscountType temp = _DiscountTypeService.Find(pt.DiscountTypeId);
                    
                    DiscountType ExRec = Mapper.Map<DiscountType>(temp);

                    temp.Rate = vm.Rate;
                    temp.DiscountTypeName = pt.DiscountTypeName;
                    temp.IsActive = pt.IsActive;
                    temp.ModifiedDate = DateTime.Now;
                    temp.ModifiedBy = User.Identity.Name;
                    temp.ObjectState = Model.ObjectState.Modified;
                    

                    LogList.Add(new LogTypeViewModel
                    {
                        ExObj = ExRec,
                        Obj = temp,
                    });

                    XElement Modifications = _modificationCheck.CheckChanges(LogList);

                    try
                    {
                        _DiscountTypeService.Update(temp);
                    }

                    catch (Exception ex)
                    {
                        ViewBag.Mode = "Edit";
                        string message = _exception.HandleException(ex);
                        ModelState.AddModelError("", message);
                        return View("Create", pt);
                    }

                    _logger.LogActivityDetail(logVm.Map(new ActiivtyLogViewModel
                    {
                        DocTypeId = Constants.DocumentTypeIdConstants.DiscountType,
                        DocId = temp.DiscountTypeId,
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
            DiscountType pt = _DiscountTypeService.Find(id);
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
            DiscountType header = _DiscountTypeService.Find(id);
            return Remove(id);
        }





        // GET: /JobReceiveHeader/Edit/5
        private ActionResult Edit(int id, string IndexType)
        {
            DiscountType pt = _DiscountTypeService.Find(id);
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
            DiscountType DiscountType = _DiscountTypeService.Find(id);

            if (DiscountType == null)
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

                    DiscountType temp = _DiscountTypeService.Find(vm.id);
                    int DocTypeId = (int)temp.DocTypeId;

                    LogList.Add(new LogTypeViewModel
                    {
                        ExObj = Mapper.Map<DiscountType>(temp),
                    });

                    XElement Modifications = _modificationCheck.CheckChanges(LogList);

                    _DiscountTypeService.Delete(vm.id);

                    _logger.LogActivityDetail(logVm.Map(new ActiivtyLogViewModel
                    {
                        DocTypeId = DocTypeId,
                        DocId = temp.DiscountTypeId,
                        ActivityType = (int)ActivityTypeContants.Deleted,
                        UserRemark = vm.Reason,
                        DocNo = temp.DiscountTypeName,
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
            var nextId = _DiscountTypeService.NextId(DocId);
            return Edit(nextId, "");
        }
        [HttpGet]
        public ActionResult PrevPage(int DocId)//CurrentHeaderId
        {
            var PrevId = _DiscountTypeService.PrevId(DocId);
            return Edit(PrevId, "");
        }

        protected override void Dispose(bool disposing)
        {
            if (!string.IsNullOrEmpty((string)TempData["CSEXC"]))
            {
                GenerateCookie.CreateNotificationCookie(NotificationTypeConstants.Danger, (string)TempData["CSEXC"]);
                TempData.Remove("CSEXC");
            }
            if (disposing)
            {
                _DiscountTypeService.Dispose();
            }
            base.Dispose(disposing);
        }



        public JsonResult DuplicateCheckForCreate(string docno, int doctypeId)
        {
            var Exists = (_DiscountTypeService.CheckForNameExists(docno));
            return Json(new { returnvalue = Exists });
        }

        public JsonResult DuplicateCheckForEdit(string docno, int doctypeId, int headerid)
        {
            var Exists = (_DiscountTypeService.CheckForNameExists(docno, headerid));
            return Json(new { returnvalue = Exists });
        }

        [HttpGet]
        public ActionResult Report()
        {
            DocumentType Dt = _DocumentTypeService.Find(Constants.DocumentTypeIdConstants.DiscountType);
            return Redirect((string)System.Configuration.ConfigurationManager.AppSettings["CustomizeDomain"] + "/Report_ReportPrint/ReportPrint/?MenuId=" + Dt.ReportMenuId);
        }
    }
}



