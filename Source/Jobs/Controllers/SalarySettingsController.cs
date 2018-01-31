using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Model.Models;
using Data.Models;
using Service;
using Data.Infrastructure;
using Presentation;
using Model.ViewModel;
using Core.Common;
using AutoMapper;
using System.Xml.Linq;
using Jobs.Helpers;

namespace Jobs.Controllers
{
    [Authorize]
    public class SalarySettingsController : System.Web.Mvc.Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        List<string> UserRoles = new List<string>();
        ActiivtyLogViewModel LogVm = new ActiivtyLogViewModel();

        ISalarySettingsService _SalarySettingsService;
        IUnitOfWork _unitOfWork;
        IExceptionHandlingService _exception;
        public SalarySettingsController(ISalarySettingsService SalarySettingsService, IUnitOfWork unitOfWork, IExceptionHandlingService exec)
        {
            _SalarySettingsService = SalarySettingsService;
            _unitOfWork = unitOfWork;
            _exception = exec;

            UserRoles = (List<string>)System.Web.HttpContext.Current.Session["Roles"];

            //Log Initialization
            LogVm.SessionId = 0;
            LogVm.ControllerName = System.Web.HttpContext.Current.Request.RequestContext.RouteData.GetRequiredString("controller");
            LogVm.ActionName = System.Web.HttpContext.Current.Request.RequestContext.RouteData.GetRequiredString("action");
            LogVm.User = System.Web.HttpContext.Current.Request.RequestContext.HttpContext.User.Identity.Name;
        }

        public void PrepareViewBag(int id)
        {
            ViewBag.Name = new DocumentTypeService(_unitOfWork).Find(id).DocumentTypeName;
            ViewBag.id = id;
            ViewBag.UnitConversionForList = (from p in db.UnitConversonFor
                                             select p).ToList();
            ViewBag.DeliveryUnitList = new UnitService(_unitOfWork).GetUnitList().ToList();
        }

        // GET: /SalarySettingsMaster/Create

        public ActionResult Create(int id)//DocTypeId
        {
            if (!UserRoles.Contains("SysAdmin"))
            {
                return View("~/Views/Shared/InValidSettings.cshtml");
            }

            var DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            var SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            PrepareViewBag(id);
            var settings = new SalarySettingsService(_unitOfWork).GetSalarySettingsForDocument(id, DivisionId, SiteId);

            if (settings == null)
            {
                SalarySettingsViewModel vm = new SalarySettingsViewModel();
                vm.DocTypeName = new DocumentTypeService(_unitOfWork).Find(id).DocumentTypeName;
                vm.SiteId = SiteId;
                vm.DivisionId = DivisionId;
                vm.DocTypeId = id;
                return View("Create", vm);
            }
            else
            {
                SalarySettingsViewModel temp = AutoMapper.Mapper.Map<SalarySettings, SalarySettingsViewModel>(settings);
                temp.DocTypeName = new DocumentTypeService(_unitOfWork).Find(id).DocumentTypeName;
                return View("Create", temp);
            }

        }

        // POST: /ProductMaster/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Post(SalarySettingsViewModel vm)
        {
            SalarySettings pt = AutoMapper.Mapper.Map<SalarySettingsViewModel, SalarySettings>(vm);




            if (ModelState.IsValid)
            {

                if (vm.SalarySettingsId <= 0)
                {
                    pt.CreatedDate = DateTime.Now;
                    pt.ModifiedDate = DateTime.Now;
                    pt.CreatedBy = User.Identity.Name;
                    pt.ModifiedBy = User.Identity.Name;
                    pt.ObjectState = Model.ObjectState.Added;
                    _SalarySettingsService.Create(pt);

                    try
                    {
                        _unitOfWork.Save();
                    }
                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        ModelState.AddModelError("", message);
                        PrepareViewBag(vm.DocTypeId);
                        return View("Create", vm);
                    }

                    LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                    {
                        DocTypeId = pt.DocTypeId,
                        DocId = pt.SalarySettingsId,
                        ActivityType = (int)ActivityTypeContants.SettingsAdded,
                    }));

                    return RedirectToAction("Index", "SalaryHeader", new { id = vm.DocTypeId }).Success("Data saved successfully");
                }
                else
                {
                    List<LogTypeViewModel> LogList = new List<LogTypeViewModel>();

                    SalarySettings temp = db.SalarySettings.Find(pt.SalarySettingsId);

                    SalarySettings ExRec = Mapper.Map<SalarySettings>(temp);

                    temp.WizardMenuId = pt.WizardMenuId;
                    temp.ImportMenuId = pt.ImportMenuId;
                    temp.CalculationId = pt.CalculationId;
                    temp.SqlProcDocumentPrint = pt.SqlProcDocumentPrint;
                    

                    temp.ModifiedDate = DateTime.Now;
                    temp.ModifiedBy = User.Identity.Name;
                    temp.ObjectState = Model.ObjectState.Modified;

                    GetChanges(db, temp);


                    _SalarySettingsService.Update(temp);

                    LogList.Add(new LogTypeViewModel
                    {
                        ExObj = ExRec,
                        Obj = temp,
                    });

                    XElement Modifications = new ModificationsCheckService().CheckChanges(LogList);

                    try
                    {
                        _unitOfWork.Save();
                    }

                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        ModelState.AddModelError("", message);
                        PrepareViewBag(vm.DocTypeId);
                        return View("Create", pt);
                    }

                    LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                    {
                        DocTypeId = temp.DocTypeId,
                        DocId = temp.SalarySettingsId,
                        ActivityType = (int)ActivityTypeContants.SettingsModified,
                        xEModifications = Modifications,
                    }));

                    return RedirectToAction("Index", "SalaryHeader", new { id = vm.DocTypeId }).Success("Data saved successfully");

                }

            }
            PrepareViewBag(vm.DocTypeId);
            return View("Create", vm);
        }











        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        private void GetChanges(ApplicationDbContext con, SalarySettings settings)
        {

            var Entry = con.Entry<SalarySettings>(settings);

            var PropNames = Entry.CurrentValues.PropertyNames;

            var ModifiedRecords = (from name in PropNames
                                   where Entry.Property(name).IsModified
                                   select name).ToList();

        }
    }
}
