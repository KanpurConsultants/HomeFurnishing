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
using Service;
using Model.ViewModel;
using Model.Models;
using Jobs.Helpers;
using Model.ViewModels;
using Core.Common;
using Presentation.ViewModels;
using Jobs.Constants.DocumentType;

namespace Jobs.Areas.PropertyTax.Controllers
{

    [Authorize]
    public class Dimension1Controller : System.Web.Mvc.Controller
    {
        List<string> UserRoles = new List<string>();

        bool TimePlanValidation = true;
        string ExceptionMsg = "";
        bool Continue = true;

        private readonly ILogger _logger;
        IDimension1Service _Dimension1Service;
        IDimension1ExtendedService _Dimension1ExtendedService;
        IDocumentTypeService _DocumentTypeService;
        IExceptionHandler _exception;
        private readonly IModificationCheck _modificationCheck;

        private ActiivtyLogViewModel logVm = new ActiivtyLogViewModel();

        public Dimension1Controller(IDimension1Service Dimension1Service, IDimension1ExtendedService Dimension1ExtendedService, IDocumentTypeService DocumentTypeService, IExceptionHandler exec,
            ILogger log, IModificationCheck modificationCheck)
        {
            _Dimension1Service = Dimension1Service;
            _Dimension1ExtendedService = Dimension1ExtendedService;
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



        public ActionResult Index(int id, string IndexType)
        {
            IQueryable<Dimension1> Dimension1 = _Dimension1Service.GetDimension1List(id);
            ViewBag.id = id;
            return View(Dimension1);
        }



        private void PrepareViewBag(int id)
        {

        }



        // GET: /JobReceiveHeader/Create

        public ActionResult Create(int id)//DocumentTypeId
        {
            Dimension1ViewModel vm = new Dimension1ViewModel();
            vm.DocTypeId = DocumentTypeIdConstants.Dimension1;
            vm.ProductTypeId = id;
            vm.IsActive = true;
            ViewBag.Mode = "Add";
            return View("Create", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Post(Dimension1ViewModel vm)
        {
            
            if (ModelState.IsValid)
            {
                if (vm.Dimension1Id <= 0)
                {
                    Dimension1 pt = new Dimension1();
                    pt.Dimension1Name = vm.Dimension1Name;
                    pt.ProductTypeId = vm.ProductTypeId;
                    pt.IsActive = true;
                    pt.CreatedDate = DateTime.Now;
                    pt.ModifiedDate = DateTime.Now;
                    pt.CreatedBy = User.Identity.Name;
                    pt.ModifiedBy = User.Identity.Name;
                    pt.ObjectState = Model.ObjectState.Added;

                    

                    try
                    {
                        _Dimension1Service.Create(pt);
                    }



                    catch (Exception ex)
                    {
                        ViewBag.Mode = "Add";
                        string message = _exception.HandleException(ex);
                        ModelState.AddModelError("", message);
                        return View("Create", vm);
                    }

                    Dimension1Extended Dimension1Extended = new Dimension1Extended();
                    Dimension1Extended.Dimension1Id = pt.Dimension1Id;
                    Dimension1Extended.CostCenterId = vm.CostCenterId ?? 0;
                    Dimension1Extended.Multiplier = vm.Multiplier ?? 0;
                    Dimension1Extended.ObjectState = Model.ObjectState.Added;
                    _Dimension1ExtendedService.Create(Dimension1Extended);



                    _logger.LogActivityDetail(logVm.Map(new ActiivtyLogViewModel
                    {
                        DocTypeId = DocumentTypeIdConstants.Dimension1,
                        DocId = pt.Dimension1Id,
                        ActivityType = (int)ActivityTypeContants.Added,
                    }));


                    return RedirectToAction("Create", new { id = vm.ProductTypeId }).Success("Data saved successfully");
                }

                else
                {
                    List<LogTypeViewModel> LogList = new List<LogTypeViewModel>();

                    Dimension1 temp = _Dimension1Service.Find(vm.Dimension1Id);
                    Dimension1Extended tempExtended = _Dimension1ExtendedService.Find(vm.Dimension1Id);
                    
                    Dimension1 ExRec = Mapper.Map<Dimension1>(temp);

                    temp.Dimension1Name = vm.Dimension1Name;
                    temp.IsActive = vm.IsActive;
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
                        _Dimension1Service.Update(temp);
                    }



                    catch (Exception ex)
                    {
                        ViewBag.Mode = "Edit";
                        string message = _exception.HandleException(ex);
                        ModelState.AddModelError("", message);
                        return View("Create", vm);
                    }

                    tempExtended.Multiplier = vm.Multiplier ?? 0;
                    tempExtended.CostCenterId = vm.CostCenterId ?? 0;
                    _Dimension1ExtendedService.Update(tempExtended);

                    _logger.LogActivityDetail(logVm.Map(new ActiivtyLogViewModel
                    {
                        DocTypeId = DocumentTypeIdConstants.Dimension1,
                        DocId = temp.Dimension1Id,
                        ActivityType = (int)ActivityTypeContants.Modified,
                        xEModifications = Modifications,
                    }));

                    return RedirectToAction("Index", new { id = vm.ProductTypeId }).Success("Data saved successfully");

                }

            }
            return View("Create", vm);
        }


        [HttpGet]
        public ActionResult Modify(int id, string IndexType)
        {
            Dimension1 pt = _Dimension1Service.Find(id);
            Dimension1Extended ptExtended = _Dimension1ExtendedService.Find(id);

            Dimension1ViewModel vm = new Dimension1ViewModel();
            vm.Dimension1Id = pt.Dimension1Id;
            vm.Dimension1Name = pt.Dimension1Name;
            vm.ProductTypeId = pt.ProductTypeId;
            vm.IsActive = pt.IsActive;
            vm.CreatedBy = pt.CreatedBy;
            vm.CreatedDate = pt.CreatedDate;
            vm.Multiplier = ptExtended.Multiplier;
            vm.CostCenterId = ptExtended.CostCenterId;


            if (pt == null)
            {
                return HttpNotFound();
            }
            ViewBag.Mode = "Edit";
            return View("Create", vm);
        }

       

        [HttpGet]
        public ActionResult Delete(int id)
        {
            Dimension1 header = _Dimension1Service.Find(id);
            return Remove(id);
        }





        // GET: /JobReceiveHeader/Edit/5
        private ActionResult Edit(int id, string IndexType)
        {
            Dimension1 pt = _Dimension1Service.Find(id);
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
            Dimension1 Dimension1 = _Dimension1Service.Find(id);

            if (Dimension1 == null)
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

                    Dimension1 temp = _Dimension1Service.Find(vm.id);

                    LogList.Add(new LogTypeViewModel
                    {
                        ExObj = Mapper.Map<Dimension1>(temp),
                    });

                    XElement Modifications = _modificationCheck.CheckChanges(LogList);

                    _Dimension1ExtendedService.Delete(vm.id);
                    _Dimension1Service.Delete(vm.id);

                    _logger.LogActivityDetail(logVm.Map(new ActiivtyLogViewModel
                    {
                        DocTypeId = DocumentTypeIdConstants.Dimension1,
                        DocId = temp.Dimension1Id,
                        ActivityType = (int)ActivityTypeContants.Deleted,
                        UserRemark = vm.Reason,
                        DocNo = temp.Dimension1Name ,
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
        public ActionResult NextPage(int DocId, int ProductTypeId)//CurrentHeaderId
        {
            var nextId = _Dimension1Service.NextId(DocId, ProductTypeId);
            return Edit(nextId, "");
        }
        [HttpGet]
        public ActionResult PrevPage(int DocId, int ProductTypeId)//CurrentHeaderId
        {
            var PrevId = _Dimension1Service.PrevId(DocId, ProductTypeId);
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
                _Dimension1Service.Dispose();
            }
            base.Dispose(disposing);
        }



        public JsonResult DuplicateCheckForCreate(string docno, int doctypeId)
        {
            var Exists = (_Dimension1Service.CheckForNameExists(docno));
            return Json(new { returnvalue = Exists });
        }

        public JsonResult DuplicateCheckForEdit(string docno, int doctypeId, int headerid)
        {
            var Exists = (_Dimension1Service.CheckForNameExists(docno, headerid));
            return Json(new { returnvalue = Exists });
        }

        [HttpGet]
        public ActionResult Report()
        {
            DocumentType Dt = _DocumentTypeService.Find(DocumentTypeIdConstants.Dimension1);
            return Redirect((string)System.Configuration.ConfigurationManager.AppSettings["CustomizeDomain"] + "/Report_ReportPrint/ReportPrint/?MenuId=" + Dt.ReportMenuId);
        }
    }
}
