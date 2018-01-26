using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using Components.ExceptionHandlers;
using Components.Logging;
using System.Xml.Linq;
using Service;
using Model.ViewModel;
using Model.Models;
using Core.Common;
using Services.PropertyTax;

namespace Jobs.Areas.PropertyTax.Controllers
{

    [Authorize]
    public class BinLocationController : System.Web.Mvc.Controller
    {
        List<string> UserRoles = new List<string>();

        IBinLocationService _BinLocationService;
        IGodownService _GodownService;
        IDocumentTypeService _DocumentTypeService;
        IJobOrderSettingsService _jobOrderSettingsService;
        IExceptionHandler _exception;

        private readonly ILogger _logger;
        bool TimePlanValidation = true;
        string ExceptionMsg = "";
        bool Continue = true;
        private readonly IModificationCheck _modificationCheck;

        ActiivtyLogViewModel LogVm = new ActiivtyLogViewModel();

        public BinLocationController(IBinLocationService BinLocationService, IDocumentTypeService DocumentTypeService, IExceptionHandler exec, IGodownService GodownService,
            ILogger log, 
            IModificationCheck modificationCheck,
            IJobOrderSettingsService jobOrderSettingsServ)
        {
            _BinLocationService = BinLocationService;
            _GodownService = GodownService;
            _DocumentTypeService = DocumentTypeService;
            _jobOrderSettingsService = jobOrderSettingsServ;
            _logger = log;
            _exception = exec;
            _modificationCheck = modificationCheck;

            UserRoles = (List<string>)System.Web.HttpContext.Current.Session["Roles"];
            LogVm.SessionId = 0;
            LogVm.ControllerName = System.Web.HttpContext.Current.Request.RequestContext.RouteData.GetRequiredString("controller");
            LogVm.ActionName = System.Web.HttpContext.Current.Request.RequestContext.RouteData.GetRequiredString("action");
            LogVm.User = System.Web.HttpContext.Current.Request.RequestContext.HttpContext.User.Identity.Name;
        }



        [HttpGet]
        public JsonResult Index(int id)
        {
            var p = _BinLocationService.GetBinLocationListForIndex(id).ToList();
            return Json(p, JsonRequestBehavior.AllowGet);

        }


        private void PrepareViewBag()
        {

        }


        public ActionResult _Create(int Id) //Id ==>GodownId
        {
            BinLocation s = new BinLocation();
            ViewBag.LineMode = "Create";

            Godown Godown = _GodownService.Find(Id);
            ViewBag.DocNo = Godown.GodownName;
            s.GodownId = Id;
            return PartialView("_Create", s);
        }






        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult _CreatePost(BinLocation svm)
        {
            bool BeforeSave = true;
            Godown temp = _GodownService.Find(svm.GodownId);


            if (_BinLocationService.IsDuplicateBinLocationName(svm.GodownId, svm.BinLocationCode, svm.BinLocationId))
                ModelState.AddModelError("BinLocationId", "Chak Code is already entered in Property.");

            if (_BinLocationService.IsDuplicateBinLocationName(svm.GodownId, svm.BinLocationName, svm.BinLocationId))
                ModelState.AddModelError("BinLocationId", "Chak Name is already entered in Property.");

            if (svm.BinLocationId <= 0)
            {
                ViewBag.LineMode = "Create";
            }
            else
            {
                ViewBag.LineMode = "Edit";
            }

            if (ModelState.IsValid && BeforeSave)
            {

                if (svm.BinLocationId <= 0)
                {
                    try
                    {
                        svm.IsActive = true;
                        svm.CreatedDate = DateTime.Now;
                        svm.ModifiedDate = DateTime.Now;
                        svm.CreatedBy = User.Identity.Name;
                        svm.ModifiedBy = User.Identity.Name;
                        svm.ObjectState = Model.ObjectState.Added;
                        _BinLocationService.Create(svm);

                        _logger.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                        {
                            DocTypeId = DocumentTypeIdConstants.BinLocation,
                            DocId = temp.GodownId,
                            DocLineId = svm.BinLocationId,
                            ActivityType = (int)ActivityTypeContants.Added,
                            DocNo = temp.GodownName,
                            DocDate = temp.CreatedDate,
                        }));

                    }

                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        TempData["CSEXCL"] += message;
                        return PartialView("_Create", svm);
                    }

                    return RedirectToAction("_Create", new { id = svm.GodownId });

                }


                else
                {

                    try
                    {
                        BinLocation binlocation = _BinLocationService.Find(svm.BinLocationId);

                        List<LogTypeViewModel> LogList = new List<LogTypeViewModel>();

                        BinLocation ExTempLine = new BinLocation();
                        ExTempLine = Mapper.Map<BinLocation>(binlocation);

                        binlocation.BinLocationCode = svm.BinLocationCode;
                        binlocation.BinLocationName = svm.BinLocationName;
                        binlocation.IsActive = svm.IsActive;
                        binlocation.ModifiedDate = DateTime.Now;
                        binlocation.ModifiedBy = User.Identity.Name;
                        binlocation.ObjectState = Model.ObjectState.Modified;
                        _BinLocationService.Update(binlocation);

                        LogList.Add(new LogTypeViewModel
                        {
                            ExObj = ExTempLine,
                            Obj = binlocation
                        });


                        XElement Modifications = _modificationCheck.CheckChanges(LogList);

                        _logger.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                        {
                            DocTypeId = DocumentTypeIdConstants.BinLocation,
                            DocId = temp.GodownId,
                            DocLineId = binlocation.BinLocationId,
                            ActivityType = (int)ActivityTypeContants.Modified,
                            DocNo = temp.GodownName,
                            xEModifications = Modifications,
                            DocDate = temp.CreatedDate,
                        }));

                    }

                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        TempData["CSEXCL"] += message;
                        return PartialView("_Create", svm);
                    }

                    return Json(new { success = true });
                }

            }
            return PartialView("_Create", svm);
        }



        public ActionResult _Modify(int id)
        {
            BinLocation temp = _BinLocationService.Find(id);


            Godown Godown = _GodownService.Find(temp.GodownId);
            ViewBag.DocNo = Godown.GodownName;
            //Getting Settings

            if (temp == null)
            {
                return HttpNotFound();
            }


            if ((TimePlanValidation || Continue))
                ViewBag.LineMode = "Edit";


            return PartialView("_Create", temp);
        }




        public ActionResult _Delete(int id)
        {
            BinLocation temp = _BinLocationService.Find(id);


            //Getting Settings

            if (temp == null)
            {
                return HttpNotFound();
            }

            

            if ((TimePlanValidation || Continue))
                ViewBag.LineMode = "Delete";

            return PartialView("_Create", temp);
        }


        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult DeletePost(BinLocation vm)
        {
            bool BeforeSave = true;

            if (BeforeSave)
            {
                try
                {
                    Godown temp = _GodownService.Find(vm.GodownId);

                    List<LogTypeViewModel> LogList = new List<LogTypeViewModel>();

                    BinLocation BinLocation = _BinLocationService.Find(vm.BinLocationId);
                    LogList.Add(new LogTypeViewModel
                    {
                        Obj = Mapper.Map<BinLocation>(BinLocation),
                    });

                    LogList.Add(new LogTypeViewModel
                    {
                        Obj = Mapper.Map<BinLocation>(BinLocation),
                    });

                    XElement Modifications = _modificationCheck.CheckChanges(LogList);


                    _BinLocationService.Delete(vm.BinLocationId);

                    _logger.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                    {
                        DocTypeId = DocumentTypeIdConstants.BinLocation,
                        DocId = temp.GodownId,
                        DocLineId = BinLocation.BinLocationId,
                        ActivityType = (int)ActivityTypeContants.Deleted,
                        DocNo = temp.GodownName,
                        xEModifications = Modifications,
                        DocDate = temp.CreatedDate,
                    }));

                }

                catch (Exception ex)
                {
                    string message = _exception.HandleException(ex);
                    TempData["CSEXCL"] += message;
                    ViewBag.LineMode = "Delete";
                    return PartialView("_Create", vm);

                }
            }

            return Json(new { success = true });

        }

        [HttpGet]
        public ActionResult Report()
        {
            DocumentType Dt = _DocumentTypeService.Find(DocumentTypeIdConstants.BinLocation);
            return Redirect((string)System.Configuration.ConfigurationManager.AppSettings["CustomizeDomain"] + "/Report_ReportPrint/ReportPrint/?MenuId=" + Dt.ReportMenuId);
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
                _BinLocationService.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
