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
using Core.Common;
using Presentation.ViewModels;
using Services.PropertyTax;
using Jobs.Helpers;

namespace Jobs.Areas.PropertyTax.Controllers
{

    [Authorize]
    public class CasteController : System.Web.Mvc.Controller
    {
        List<string> UserRoles = new List<string>();

        bool TimePlanValidation = true;
        string ExceptionMsg = "";
        bool Continue = true;

        private readonly ILogger _logger;
        ICasteService _CasteService;
        IDocumentTypeService _DocumentTypeService;
        IExceptionHandler _exception;
        private readonly IModificationCheck _modificationCheck;

        private ActiivtyLogViewModel logVm = new ActiivtyLogViewModel();

        public CasteController(ICasteService CasteService, IDocumentTypeService DocumentTypeService, IExceptionHandler exec,
            ILogger log, IModificationCheck modificationCheck)
        {
            _CasteService = CasteService;
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
            IQueryable<Caste> Caste = _CasteService.GetCasteListForIndex();
            return View(Caste);
        }



        private void PrepareViewBag(int id)
        {

        }



        // GET: /JobReceiveHeader/Create

        public ActionResult Create()//DocumentTypeId
        {
            Caste vm = new Caste();
            vm.DocTypeId = DocumentTypeIdConstants.Caste;
            vm.IsActive = true;
            ViewBag.Mode = "Add";
            return View("Create", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Post(Caste vm)
        {
            Caste pt = vm;
            if (ModelState.IsValid)
            {
                if (vm.CasteId <= 0)
                {

                    pt.IsActive = true;
                    pt.CreatedDate = DateTime.Now;
                    pt.ModifiedDate = DateTime.Now;
                    pt.CreatedBy = User.Identity.Name;
                    pt.ModifiedBy = User.Identity.Name;
                    pt.ObjectState = Model.ObjectState.Added;
                    

                    try
                    {
                        _CasteService.Create(pt);
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
                        DocTypeId = DocumentTypeIdConstants.Caste,
                        DocId = pt.CasteId,
                        ActivityType = (int)ActivityTypeContants.Added,
                    }));


                    return RedirectToAction("Create").Success("Data saved successfully");
                }

                else
                {
                    List<LogTypeViewModel> LogList = new List<LogTypeViewModel>();

                    Caste temp = _CasteService.Find(pt.CasteId);
                    
                    Caste ExRec = Mapper.Map<Caste>(temp);

                    temp.CasteName = pt.CasteName;
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
                        _CasteService.Update(temp);
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
                        DocTypeId = DocumentTypeIdConstants.Caste,
                        DocId = temp.CasteId,
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
            Caste pt = _CasteService.Find(id);
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
            Caste header = _CasteService.Find(id);
            return Remove(id);
        }





        // GET: /JobReceiveHeader/Edit/5
        private ActionResult Edit(int id, string IndexType)
        {
            Caste pt = _CasteService.Find(id);
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
            Caste Caste = _CasteService.Find(id);

            if (Caste == null)
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

                    Caste temp = _CasteService.Find(vm.id);
                    int DocTypeId = (int)temp.DocTypeId;

                    LogList.Add(new LogTypeViewModel
                    {
                        ExObj = Mapper.Map<Caste>(temp),
                    });

                    XElement Modifications = _modificationCheck.CheckChanges(LogList);

                    _CasteService.Delete(vm.id);

                    _logger.LogActivityDetail(logVm.Map(new ActiivtyLogViewModel
                    {
                        DocTypeId = DocTypeId,
                        DocId = temp.CasteId,
                        ActivityType = (int)ActivityTypeContants.Deleted,
                        UserRemark = vm.Reason,
                        DocNo = temp.CasteName,
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
            var nextId = _CasteService.NextId(DocId);
            return Edit(nextId, "");
        }
        [HttpGet]
        public ActionResult PrevPage(int DocId)//CurrentHeaderId
        {
            var PrevId = _CasteService.PrevId(DocId);
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
                _CasteService.Dispose();
            }
            base.Dispose(disposing);
        }



        public JsonResult DuplicateCheckForCreate(string docno, int doctypeId)
        {
            var Exists = (_CasteService.CheckForNameExists(docno));
            return Json(new { returnvalue = Exists });
        }

        public JsonResult DuplicateCheckForEdit(string docno, int doctypeId, int headerid)
        {
            var Exists = (_CasteService.CheckForNameExists(docno, headerid));
            return Json(new { returnvalue = Exists });
        }

        [HttpGet]
        public ActionResult Report()
        {
            DocumentType Dt = _DocumentTypeService.Find(DocumentTypeIdConstants.Caste);
            return Redirect((string)System.Configuration.ConfigurationManager.AppSettings["CustomizeDomain"] + "/Report_ReportPrint/ReportPrint/?MenuId=" + Dt.ReportMenuId);
        }
    }
}
