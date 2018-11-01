using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Model.Models;
using Data.Models;
using Service;
using Data.Infrastructure;
using Presentation.ViewModels;
using Presentation;
using Core.Common;
using Model.ViewModel;
using AutoMapper;
using System.Xml.Linq;
using Model.ViewModels;
using Jobs.Helpers;
using Reports.Controllers;

namespace Jobs.Controllers
{
    [Authorize]
    public class CustomHeaderController : System.Web.Mvc.Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        bool TimePlanValidation = true;
        string ExceptionMsg = "";
        bool Continue = true;

        ActiivtyLogViewModel LogVm = new ActiivtyLogViewModel();
        List<string> UserRoles = new List<string>();
        ICustomHeaderService _CustomHeaderService;
        IUnitOfWork _unitOfWork;
        IExceptionHandlingService _exception;
        public CustomHeaderController(ICustomHeaderService CustomHeaderService, IUnitOfWork unitOfWork, IExceptionHandlingService exec)
        {
            _CustomHeaderService = CustomHeaderService;
            _unitOfWork = unitOfWork;
            _exception = exec;

            //Log Initialization
            LogVm.SessionId = 0;
            LogVm.ControllerName = System.Web.HttpContext.Current.Request.RequestContext.RouteData.GetRequiredString("controller");
            LogVm.ActionName = System.Web.HttpContext.Current.Request.RequestContext.RouteData.GetRequiredString("action");
            LogVm.User = System.Web.HttpContext.Current.Request.RequestContext.HttpContext.User.Identity.Name;
        }
        // GET: /SaleEnquiryProductMappingMaster/

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

        public ActionResult Index(int id, string IndexType)//DocTypeId 
        {
            IQueryable<CustomHeaderIndexViewModel> CustomHeader = _CustomHeaderService.GetCustomHeaderIndex(id, User.Identity.Name);
            ViewBag.IndexStatus = "All";
            DocumentType DT = new DocumentTypeService(_unitOfWork).Find(id);
            ViewBag.Name = DT.DocumentTypeName;
            ViewBag.id = id;
            return View(CustomHeader);
        }
        public ActionResult Create(int id)//DocTypeId
        {
            CustomHeaderViewModel p = new CustomHeaderViewModel();
            List<DocumentTypeHeaderAttributeViewModel> tem = new DocumentTypeService(_unitOfWork).GetDocumentTypeHeaderAttribute(id).ToList();
            p.DocumentTypeHeaderAttributes = tem;
            p.DocDate = DateTime.Now.Date;
            p.DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            p.SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            p.DocTypeId = id;
            PrepareViewBag(id);
            ViewBag.Mode = "Add";
            p.DocumentTypeSettings = new DocumentTypeSettingsService(_unitOfWork).GetDocumentTypeSettingsForDocument(p.DocTypeId);
            DocumentType DT = new DocumentTypeService(_unitOfWork).Find(id);
            ViewBag.Name = DT.DocumentTypeName;
            return View(p);
        }

        // POST: /ProductMaster/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePost(CustomHeaderViewModel vm)
        {


            List<LogTypeViewModel> LogList = new List<LogTypeViewModel>();

            //try
            //{

            //    if (vm.CustomHeaderId <= 0)
            //        TimePlanValidation = DocumentValidation.ValidateDocument(Mapper.Map<DocumentUniqueId>(vm), DocumentTimePlanTypeConstants.Create, User.Identity.Name, out ExceptionMsg, out Continue);
            //    else
            //        TimePlanValidation = DocumentValidation.ValidateDocument(Mapper.Map<DocumentUniqueId>(vm), DocumentTimePlanTypeConstants.Modify, User.Identity.Name, out ExceptionMsg, out Continue);

            //}
            //catch (Exception ex)
            //{
            //    string message = _exception.HandleException(ex);
            //    TempData["CSEXC"] += message;
            //    TimePlanValidation = false;
            //}

            //if (!TimePlanValidation)
            //    TempData["CSEXC"] += ExceptionMsg;

            if (vm.DocumentTypeHeaderAttributes != null)
            {
                foreach (var pta in vm.DocumentTypeHeaderAttributes)
                {
                    if (pta.DataType == "Number")
                        if (pta.Value != null)
                        {
                            var count = pta.Value.Count(x => x == '.');
                            if (count > 1)
                                ModelState.AddModelError("", pta.Name + " should be a numeric value.");
                            else
                            {
                                if (pta.Value.Replace(".", "").All(char.IsDigit) == false)
                                    ModelState.AddModelError("", pta.Name + " should be a numeric value.");
                            }
                        }

                }
            }

            if (ModelState.IsValid && (TimePlanValidation || Continue))
            {
                CustomHeader Customheaderdetail = Mapper.Map<CustomHeaderViewModel, CustomHeader>(vm);

                #region CreateRecord
                if (vm.CustomHeaderId == 0)
                {
                    
                    Customheaderdetail.CreatedDate = DateTime.Now;
                    Customheaderdetail.ModifiedDate = DateTime.Now;
                    Customheaderdetail.CreatedBy = User.Identity.Name;
                    Customheaderdetail.ModifiedBy = User.Identity.Name;
                    Customheaderdetail.Status = (int)StatusConstants.Drafted;
                    _CustomHeaderService.Create(Customheaderdetail);

                    try
                    {
                        _unitOfWork.Save();
                    }

                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        TempData["CSEXC"] += message;
                        PrepareViewBag(vm.DocTypeId);
                        ViewBag.Mode = "Add";
                        return View("Create", vm);
                    }

                    LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                    {
                        DocTypeId = Customheaderdetail.DocTypeId,
                        DocId = Customheaderdetail.CustomHeaderId,
                        ActivityType = (int)ActivityTypeContants.Added,
                        DocNo = Customheaderdetail.DocNo,
                        DocDate = Customheaderdetail.DocDate,
                        DocStatus = Customheaderdetail.Status,
                    }));


                    if (vm.DocumentTypeHeaderAttributes != null)
                    {
                        foreach (var pta in vm.DocumentTypeHeaderAttributes)
                        {

                            CustomHeaderAttributes CustomHeaderAttribute = (from A in db.CustomHeaderAttributes
                                                                            where A.HeaderTableId == vm.CustomHeaderId && A.DocumentTypeHeaderAttributeId == pta.DocumentTypeHeaderAttributeId
                                                                            select A).FirstOrDefault();

                            if (CustomHeaderAttribute != null)
                            {
                                CustomHeaderAttribute.Value = pta.Value;
                                CustomHeaderAttribute.ObjectState = Model.ObjectState.Modified;
                                _unitOfWork.Repository<CustomHeaderAttributes>().Add(CustomHeaderAttribute);
                            }
                            else
                            {
                                CustomHeaderAttributes pa = new CustomHeaderAttributes()
                                {
                                    Value = pta.Value,
                                    HeaderTableId = Customheaderdetail.CustomHeaderId,
                                    DocumentTypeHeaderAttributeId = pta.DocumentTypeHeaderAttributeId,
                                };
                                pa.ObjectState = Model.ObjectState.Added;
                                _unitOfWork.Repository<CustomHeaderAttributes>().Add(pa);
                            }
                        }


                        LogList.Add(new LogTypeViewModel
                        {
                            //ExObj = ExRec,
                            Obj = Customheaderdetail,
                        });

                        XElement Modifications = new ModificationsCheckService().CheckChanges(LogList);

                        try
                        {
                            _unitOfWork.Save();
                        }

                        catch (Exception ex)
                        {
                            string message = _exception.HandleException(ex);
                            TempData["CSEXC"] += message;
                            PrepareViewBag(vm.DocTypeId);
                            ViewBag.Mode = "Edit";
                            return View("Create", vm);
                        }

                        LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                        {
                            DocTypeId = Customheaderdetail.DocTypeId,
                            DocId = Customheaderdetail.CustomHeaderId,
                            ActivityType = (int)ActivityTypeContants.Modified,
                            DocNo = Customheaderdetail.DocNo,
                            xEModifications = Modifications,
                            DocDate = Customheaderdetail.DocDate,
                            DocStatus = Customheaderdetail.Status,
                        }));
                                              

                        var ModelStateErrorList = ModelState.Select(x => x.Value.Errors).Where(y => y.Count > 0).ToList();
                        string Messsages = "";
                        if (ModelStateErrorList.Count > 0)
                        {
                            foreach (var ModelStateError in ModelStateErrorList)
                            {
                                foreach (var Error in ModelStateError)
                                {
                                    if (!Messsages.Contains(Error.ErrorMessage))
                                        Messsages = Error.ErrorMessage + System.Environment.NewLine;
                                }
                            }
                            if (Messsages != "")
                                ModelState.AddModelError("", Messsages);
                        }
                    }

                    return RedirectToAction("Index", new { id = vm.DocTypeId }).Success("Data saved successfully");

                }
                #endregion

                //        #region EditRecord
                else
                {

                    CustomHeader Customheaderdetail1 = _CustomHeaderService.FindCustomHeader(vm.CustomHeaderId);

                    CustomHeader ExRec = Mapper.Map<CustomHeader>(Customheaderdetail1);

                    int status = Customheaderdetail1.Status;

                    if (Customheaderdetail1.Status != (int)StatusConstants.Drafted)
                    {
                        Customheaderdetail1.Status = (int)StatusConstants.Modified;
                    }

                    Customheaderdetail1.DocDate = vm.DocDate;
                    Customheaderdetail1.DocNo = vm.DocNo;
                    Customheaderdetail1.Remark = vm.Remark;
                    Customheaderdetail1.ModifiedDate = DateTime.Now;
                    Customheaderdetail1.ModifiedBy = User.Identity.Name;
                    _CustomHeaderService.Update(Customheaderdetail1);


                    if (vm.DocumentTypeHeaderAttributes != null)
                    {
                        foreach (var pta in vm.DocumentTypeHeaderAttributes)
                        {

                            CustomHeaderAttributes CustomHeaderAttribute = (from A in db.CustomHeaderAttributes
                                                                            where A.HeaderTableId == vm.CustomHeaderId && A.DocumentTypeHeaderAttributeId == pta.DocumentTypeHeaderAttributeId
                                                                            select A).FirstOrDefault();

                            if (CustomHeaderAttribute != null)
                            {
                                CustomHeaderAttribute.Value = pta.Value;
                                CustomHeaderAttribute.ObjectState = Model.ObjectState.Modified;
                                _unitOfWork.Repository<CustomHeaderAttributes>().Add(CustomHeaderAttribute);
                            }
                            else
                            {
                                CustomHeaderAttributes pa = new CustomHeaderAttributes()
                                {
                                    Value = pta.Value,
                                    HeaderTableId = Customheaderdetail1.CustomHeaderId,
                                    DocumentTypeHeaderAttributeId = pta.DocumentTypeHeaderAttributeId,
                                };
                                pa.ObjectState = Model.ObjectState.Added;
                                _unitOfWork.Repository<CustomHeaderAttributes>().Add(pa);
                            }
                        }


                        LogList.Add(new LogTypeViewModel
                        {
                            //ExObj = ExRec,
                            Obj = Customheaderdetail,
                        });

                        XElement Modifications = new ModificationsCheckService().CheckChanges(LogList);

                        try
                        {
                            _unitOfWork.Save();
                        }

                        catch (Exception ex)
                        {
                            string message = _exception.HandleException(ex);
                            TempData["CSEXC"] += message;
                            PrepareViewBag(vm.DocTypeId);
                            ViewBag.Mode = "Edit";
                            return View("Create", vm);
                        }

                        LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                        {
                            DocTypeId = Customheaderdetail1.DocTypeId,
                            DocId = Customheaderdetail1.CustomHeaderId,
                            ActivityType = (int)ActivityTypeContants.Modified,
                            DocNo = Customheaderdetail1.DocNo,
                            xEModifications = Modifications,
                            DocDate = Customheaderdetail1.DocDate,
                            DocStatus = Customheaderdetail1.Status,
                        }));

                        return RedirectToAction("Index", new { id = vm.DocTypeId }).Success("Data saved successfully");

                        var ModelStateErrorList = ModelState.Select(x => x.Value.Errors).Where(y => y.Count > 0).ToList();
                        string Messsages = "";
                        if (ModelStateErrorList.Count > 0)
                        {
                            foreach (var ModelStateError in ModelStateErrorList)
                            {
                                foreach (var Error in ModelStateError)
                                {
                                    if (!Messsages.Contains(Error.ErrorMessage))
                                        Messsages = Error.ErrorMessage + System.Environment.NewLine;
                                }
                            }
                            if (Messsages != "")
                                ModelState.AddModelError("", Messsages);
                        }
                    }
                }
            }
            vm.DocumentTypeSettings = new DocumentTypeSettingsService(_unitOfWork).GetDocumentTypeSettingsForDocument(vm.DocTypeId);
            PrepareViewBag(vm.DocTypeId);
            ViewBag.Mode = "Add";
            return View("Create", vm);
        }




        [HttpGet]
        public ActionResult Modify(int id, string IndexType)
        {
            CustomHeader header = _CustomHeaderService.FindCustomHeader(id);
            return Edit(id, IndexType);
        }

        private void PrepareViewBag(int id)
        {
            ViewBag.Name = "Sale Invoice Custom";
            ViewBag.id = id;

        }

        // GET: /ProductMaster/Edit/5

        private ActionResult Edit(int id, string IndexType)
        {

            ViewBag.IndexStatus = IndexType;
            CustomHeader s = _CustomHeaderService.FindCustomHeader(id);
            CustomHeaderViewModel vm = new CustomHeaderViewModel();
            List<DocumentTypeHeaderAttributeViewModel> tem = _CustomHeaderService.GetDocumentHeaderAttribute(id).ToList();
            vm.DocumentTypeHeaderAttributes = tem;
            vm.DocTypeId = s.DocTypeId;
            vm.CustomHeaderId = id;
            vm.DocDate = s.DocDate;
            vm.DocNo = s.DocNo;
            vm.Remark = s.Remark;
            PrepareViewBag(s.DocTypeId);
            ViewBag.Mode = "Edit";
            vm.DocumentTypeSettings = new DocumentTypeSettingsService(_unitOfWork).GetDocumentTypeSettingsForDocument(s.DocTypeId);
            if (!(System.Web.HttpContext.Current.Request.UrlReferrer.PathAndQuery).Contains("Create"))
                LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                {
                    DocTypeId = s.DocTypeId,
                    DocId = s.CustomHeaderId,
                    ActivityType = (int)ActivityTypeContants.Detail,
                    DocNo = s.DocNo,
                    DocDate = s.DocDate,
                    DocStatus = s.Status,
                }));

            return View("Create", vm);
        }


        [HttpGet]
        public ActionResult NextPage(int id)
        {
            var nextId = _CustomHeaderService.NextId(id);
            return RedirectToAction("Edit", new { id = nextId });
        }
        [HttpGet]
        public ActionResult PrevPage(int id)
        {
            var nextId = _CustomHeaderService.PrevId(id);
            return RedirectToAction("Edit", new { id = nextId });
        }


        private ActionResult Remove(int id)
        {
            ReasonViewModel rvm = new ReasonViewModel()
            {
                id = id,
            };

            CustomHeader CustomHeader = db.CustomHeader.Find(id);

            if (new RolePermissionService(_unitOfWork).IsActionAllowed(UserRoles, CustomHeader.DocTypeId, null, this.ControllerContext.RouteData.Values["controller"].ToString(), "Remove") == false)
            {
                return PartialView("~/Views/Shared/PermissionDenied_Modal.cshtml").Warning("You don't have permission to do this task.");
            }



            return PartialView("_Reason", rvm);

        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Delete(ReasonViewModel vm)
        {
            if (ModelState.IsValid)
            {

                db.Configuration.AutoDetectChangesEnabled = false;

                CustomHeader Si = (from H in db.CustomHeader where H.CustomHeaderId == vm.id select H).FirstOrDefault();

                var attributes = (from A in db.CustomHeaderAttributes where A.HeaderTableId == vm.id select A).ToList();

                foreach (var ite2 in attributes)
                {
                    ite2.ObjectState = Model.ObjectState.Deleted;
                    db.CustomHeaderAttributes.Remove(ite2);
                }

                Si.ObjectState = Model.ObjectState.Deleted;
                db.CustomHeader.Attach(Si);
                db.CustomHeader.Remove(Si);

                //Commit the DB
                try
                {
                    db.SaveChanges();
                    db.Configuration.AutoDetectChangesEnabled = true;
                }

                catch (Exception ex)
                {
                    string message = _exception.HandleException(ex);
                    db.Configuration.AutoDetectChangesEnabled = true;
                    TempData["CSEXC"] += message;
                    PrepareViewBag(Si.DocTypeId);
                    return PartialView("_Reason", vm);
                }

                LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                {
                    DocTypeId = Si.DocTypeId,
                    DocId = Si.CustomHeaderId,
                    ActivityType = (int)ActivityTypeContants.Deleted,
                    UserRemark = vm.Reason,
                    DocNo = Si.DocNo,
                    DocDate = Si.DocDate,
                    DocStatus = Si.Status,
                }));

                return Json(new { success = true });
            }
            return PartialView("_Reason", vm);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
