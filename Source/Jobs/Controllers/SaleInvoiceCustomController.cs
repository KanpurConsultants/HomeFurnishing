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
using System.Data.SqlClient;

namespace Jobs.Controllers
{
    [Authorize]
    public class SaleInvoiceCustomController : System.Web.Mvc.Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        ActiivtyLogViewModel LogVm = new ActiivtyLogViewModel();
        List<string> UserRoles = new List<string>();
        ISaleInvoiceHeaderService _SaleInvoiceHeaderService;
        IUnitOfWork _unitOfWork;
        IExceptionHandlingService _exception;
        public SaleInvoiceCustomController(ISaleInvoiceHeaderService SaleInvoiceHeaderService, IUnitOfWork unitOfWork, IExceptionHandlingService exec)
        {
            _SaleInvoiceHeaderService = SaleInvoiceHeaderService;
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
            IQueryable<SaleInvoiceHeaderIndexViewModel> SaleInvoiceHeader = _SaleInvoiceHeaderService.GetSaleInvoiceListForCustomIndex(id, User.Identity.Name);
            ViewBag.IndexStatus = "All";
            ViewBag.Name = "Sale Invoice Custom";
            ViewBag.id = id;
            return View(SaleInvoiceHeader);
        }



        public ActionResult Create(int id)//DocTypeId
        {
            SaleInvoiceCustomViewModel p = new SaleInvoiceCustomViewModel();
            List<DocumentTypeHeaderAttributeViewModel> tem = new DocumentTypeService(_unitOfWork).GetDocumentTypeHeaderAttribute(id).ToList();
            p.DocumentTypeHeaderAttributes = tem;
            p.DocTypeId = id;
            PrepareViewBag(id);
            ViewBag.Mode = "Add";
            ViewBag.Name = "Sale Invoice Custom";
            return View(p);
        }

        // POST: /ProductMaster/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePost(SaleInvoiceCustomViewModel vm)
        {

                        
                    List<LogTypeViewModel> LogList = new List<LogTypeViewModel>();

                    SaleInvoiceHeader saleinvoiceheaderdetail = _SaleInvoiceHeaderService.FindDirectSaleInvoice(vm.SaleInvoiceHeaderId);

                    SaleInvoiceHeader ExRec = Mapper.Map<SaleInvoiceHeader>(saleinvoiceheaderdetail);







                    if (vm.DocumentTypeHeaderAttributes != null)
                    {
                        foreach (var pta in vm.DocumentTypeHeaderAttributes)
                        {

                            SaleInvoiceHeaderAttributes SaleInvoiceHeaderAttribute = (from A in db.SaleInvoiceHeaderAttributes
                                                                                      where A.HeaderTableId == saleinvoiceheaderdetail.SaleInvoiceHeaderId && A.DocumentTypeHeaderAttributeId == pta.DocumentTypeHeaderAttributeId
                                                                                      select A).FirstOrDefault();

                            if (SaleInvoiceHeaderAttribute != null)
                            {
                                SaleInvoiceHeaderAttribute.Value = pta.Value;
                                SaleInvoiceHeaderAttribute.ObjectState = Model.ObjectState.Modified;
                                _unitOfWork.Repository<SaleInvoiceHeaderAttributes>().Add(SaleInvoiceHeaderAttribute);
                            }
                            else
                            {
                                SaleInvoiceHeaderAttributes pa = new SaleInvoiceHeaderAttributes()
                                {
                                    Value = pta.Value,
                                    HeaderTableId = saleinvoiceheaderdetail.SaleInvoiceHeaderId,
                                    DocumentTypeHeaderAttributeId = pta.DocumentTypeHeaderAttributeId,
                                };
                                pa.ObjectState = Model.ObjectState.Added;
                                _unitOfWork.Repository<SaleInvoiceHeaderAttributes>().Add(pa);
                            }
                        }
                    }

                    
                    LogList.Add(new LogTypeViewModel
                    {
                        ExObj = ExRec,
                        Obj = saleinvoiceheaderdetail,
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
                        DocTypeId = saleinvoiceheaderdetail.DocTypeId,
                        DocId = saleinvoiceheaderdetail.SaleInvoiceHeaderId,
                        ActivityType = (int)ActivityTypeContants.Modified,
                        DocNo = saleinvoiceheaderdetail.DocNo,
                        xEModifications = Modifications,
                        DocDate = saleinvoiceheaderdetail.DocDate,
                        DocStatus = saleinvoiceheaderdetail.Status,
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


            PrepareViewBag(vm.DocTypeId);
            ViewBag.Mode = "Add";
            return View("Create", vm);
        }



        [HttpGet]
        public ActionResult Modify(int id, string IndexType)
        {
            SaleInvoiceHeader header = _SaleInvoiceHeaderService.FindDirectSaleInvoice(id);
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
            SaleInvoiceHeader s = _SaleInvoiceHeaderService.FindDirectSaleInvoice(id);
            SaleInvoiceCustomViewModel vm = new SaleInvoiceCustomViewModel();
            List<DocumentTypeHeaderAttributeViewModel> tem = _SaleInvoiceHeaderService.GetDocumentHeaderAttribute(id).ToList();
            vm.DocumentTypeHeaderAttributes = tem;
            vm.DocTypeId = s.DocTypeId;
            vm.SaleInvoiceHeaderId = id;
            PrepareViewBag(s.DocTypeId);
            ViewBag.Mode = "Edit";

            if (!(System.Web.HttpContext.Current.Request.UrlReferrer.PathAndQuery).Contains("Create"))
                LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                {
                    DocTypeId = s.DocTypeId,
                    DocId = s.SaleInvoiceHeaderId,
                    ActivityType = (int)ActivityTypeContants.Detail,
                    DocNo = s.DocNo,
                    DocDate = s.DocDate,
                    DocStatus = s.Status,
                }));

            return View("Create", vm);
        }

        // GET: /ProductMaster/Delete/5


        public ActionResult GetPendingSaleInvoiceToCustomAttribute(string searchTerm, int pageSize, int pageNum)
        {
            var Query = new SaleInvoiceHeaderService(_unitOfWork).GetPendingSaleInvoiceToCustomAttribute(searchTerm);
            var temp = Query.Skip(pageSize * (pageNum - 1))
                .Take(pageSize)
                .ToList();

            var count = Query.Count();

            ComboBoxPagedResult Data = new ComboBoxPagedResult();
            Data.Results = temp;
            Data.Total = count;

            return new JsonpResult
            {
                Data = Data,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }



        [HttpGet]
        public ActionResult NextPage(int id)
        {
            var nextId = _SaleInvoiceHeaderService.NextId(id);
            return RedirectToAction("Edit", new { id = nextId });
        }
        [HttpGet]
        public ActionResult PrevPage(int id)
        {
            var nextId = _SaleInvoiceHeaderService.PrevId(id);
            return RedirectToAction("Edit", new { id = nextId });
        }


        private ActionResult Remove(int id)
        {
            ReasonViewModel rvm = new ReasonViewModel()
            {
                id = id,
            };

            SaleInvoiceHeader SaleinvoiceHeader = db.SaleInvoiceHeader.Find(id);

            if (new RolePermissionService(_unitOfWork).IsActionAllowed(UserRoles, SaleinvoiceHeader.DocTypeId, null, this.ControllerContext.RouteData.Values["controller"].ToString(), "Remove") == false)
            {
                return PartialView("~/Views/Shared/PermissionDenied_Modal.cshtml").Warning("You don't have permission to do this task.");
            }



            return PartialView("_Reason", rvm);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(ReasonViewModel vm)
        {
            if (ModelState.IsValid)
            {

                db.Configuration.AutoDetectChangesEnabled = false;

                SaleInvoiceHeader Si = (from H in db.SaleInvoiceHeader where H.SaleInvoiceHeaderId == vm.id select H).FirstOrDefault();
                
                var attributes = (from A in db.SaleInvoiceHeaderAttributes where A.HeaderTableId == vm.id select A).ToList();

                foreach (var ite2 in attributes)
                {
                    ite2.ObjectState = Model.ObjectState.Deleted;
                    db.SaleInvoiceHeaderAttributes.Remove(ite2);
                }

                
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
                    DocId = Si.SaleInvoiceHeaderId,
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