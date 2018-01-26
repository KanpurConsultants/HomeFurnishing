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
using Services.PropertyTax;
using Model.ViewModel;
using Presentation.ViewModels;
using Model.Models;
using Core.Common;
using Jobs.Helpers;

namespace Jobs.Areas.PropertyTax.Controllers
{

    [Authorize]
    public class GodownController : System.Web.Mvc.Controller
    {
        List<string> UserRoles = new List<string>();

        bool TimePlanValidation = true;
        string ExceptionMsg = "";
        bool Continue = true;

        private readonly ILogger _logger;
        IGodownService _GodownService;
        IDocumentTypeService _DocumentTypeService;
        IBinLocationService _BinLocationService;
        IExceptionHandler _exception;
        private readonly IModificationCheck _modificationCheck;

        private ActiivtyLogViewModel logVm = new ActiivtyLogViewModel();

        public GodownController(IGodownService GodownService, IBinLocationService BinLocationService, IDocumentTypeService DocumentTypeService, IExceptionHandler exec,
            ILogger log, IModificationCheck modificationCheck)
        {
            _GodownService = GodownService;
            _BinLocationService = BinLocationService;
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
            var SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];

            IQueryable<Godown> Godown = _GodownService.GetGodownListForIndex(SiteId);
            return View(Godown);
        }



        private void PrepareViewBag(int id)
        {

        }



        // GET: /JobReceiveHeader/Create

        public ActionResult Create()//DocumentTypeId
        {
            Godown vm = new Godown();
            vm.SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            vm.IsActive = true;
            ViewBag.Mode = "Add";
            return View("Create", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Post(Godown vm)
        {
            Godown pt = vm;
            if (ModelState.IsValid)
            {
                if (vm.GodownId <= 0)
                {
                    //pt.GodownId = _GodownService.GetNewId();
                    pt.IsActive = true;
                    pt.CreatedDate = DateTime.Now;
                    pt.ModifiedDate = DateTime.Now;
                    pt.CreatedBy = User.Identity.Name;
                    pt.ModifiedBy = User.Identity.Name;
                    pt.ObjectState = Model.ObjectState.Added;
                    

                    try
                    {
                        _GodownService.Create(pt);
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
                        DocTypeId = DocumentTypeIdConstants.Godown,
                        DocId = pt.GodownId,
                        ActivityType = (int)ActivityTypeContants.Added,
                    }));


                    //return RedirectToAction("Create").Success("Data saved successfully");
                    return RedirectToAction("Modify", "Godown", new { Id = vm.GodownId }).Success("Data saved successfully");
                }

                else
                {
                    List<LogTypeViewModel> LogList = new List<LogTypeViewModel>();

                    Godown temp = _GodownService.Find(pt.GodownId);
                    
                    Godown ExRec = Mapper.Map<Godown>(temp);

                    temp.GodownCode = pt.GodownCode;
                    temp.GodownName = pt.GodownName;
                    temp.GateId = pt.GateId;
                    temp.PersonId = pt.PersonId;
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
                        _GodownService.Update(temp);
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
                        DocTypeId = DocumentTypeIdConstants.Godown,
                        DocId = temp.GodownId,
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
            Godown pt = _GodownService.Find(id);
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
            Godown header = _GodownService.Find(id);
            return Remove(id);
        }





        // GET: /JobReceiveHeader/Edit/5
        private ActionResult Edit(int id, string IndexType)
        {
            Godown pt = _GodownService.Find(id);
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
            Godown Godown = _GodownService.Find(id);

            if (Godown == null)
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

                    Godown temp = _GodownService.Find(vm.id);
                    
                    LogList.Add(new LogTypeViewModel
                    {
                        ExObj = Mapper.Map<Godown>(temp),
                    });

                    IEnumerable<BinLocation> BinLocationList = _BinLocationService.GetBinLocationListForGodown(vm.id);

                    foreach(BinLocation item in BinLocationList)
                    {
                        _BinLocationService.Delete(item);
                    }

                    XElement Modifications = _modificationCheck.CheckChanges(LogList);

                    _GodownService.Delete(vm.id);




                    _logger.LogActivityDetail(logVm.Map(new ActiivtyLogViewModel
                    {
                        DocTypeId = DocumentTypeIdConstants.Godown,
                        DocId = temp.GodownId,
                        ActivityType = (int)ActivityTypeContants.Deleted,
                        UserRemark = vm.Reason,
                        DocNo = temp.GodownName,
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
            var nextId = _GodownService.NextId(DocId);
            return Edit(nextId, "");
        }
        [HttpGet]
        public ActionResult PrevPage(int DocId)//CurrentHeaderId
        {
            var PrevId = _GodownService.PrevId(DocId);
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
                _GodownService.Dispose();
            }
            base.Dispose(disposing);
        }



        public JsonResult DuplicateCheckForCreate(string docno, int doctypeId)
        {
            var Exists = (_GodownService.CheckForNameExists(docno));
            return Json(new { returnvalue = Exists });
        }

        public JsonResult DuplicateCheckForEdit(string docno, int doctypeId, int headerid)
        {
            var Exists = (_GodownService.CheckForNameExists(docno, headerid));
            return Json(new { returnvalue = Exists });
        }

        [HttpGet]
        public ActionResult Report()
        {
            DocumentType Dt = _DocumentTypeService.Find(DocumentTypeIdConstants.Godown);
            return Redirect((string)System.Configuration.ConfigurationManager.AppSettings["CustomizeDomain"] + "/Report_ReportPrint/ReportPrint/?MenuId=" + Dt.ReportMenuId);
        }
    }
}
