using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Model.Models;
using Data.Models;
using Service;
using Data.Infrastructure;
using Core.Common;
using Model.ViewModels;
using AutoMapper;
using Jobs.Helpers;
using Model.ViewModel;
using System.Xml.Linq;
using DocumentEvents;
using CustomEventArgs;
using SalaryDocumentEvents;
using Reports.Controllers;

namespace Jobs.Controllers
{

    [Authorize]
    public class SalaryLineController : System.Web.Mvc.Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private bool EventException = false;

        List<string> UserRoles = new List<string>();
        ActiivtyLogViewModel LogVm = new ActiivtyLogViewModel();

        ISalaryLineService _SalaryLineService;
        IUnitOfWork _unitOfWork;
        IExceptionHandlingService _exception;

        bool TimePlanValidation = true;
        string ExceptionMsg = "";
        bool Continue = true;

        public SalaryLineController(ISalaryLineService Salary, IUnitOfWork unitOfWork, IExceptionHandlingService exec)
        {
            _SalaryLineService = Salary;
            _unitOfWork = unitOfWork;
            _exception = exec;

            UserRoles = (List<string>)System.Web.HttpContext.Current.Session["Roles"];

            //Log Initialization
            LogVm.SessionId = 0;
            LogVm.ControllerName = System.Web.HttpContext.Current.Request.RequestContext.RouteData.GetRequiredString("controller");
            LogVm.ActionName = System.Web.HttpContext.Current.Request.RequestContext.RouteData.GetRequiredString("action");
            LogVm.User = System.Web.HttpContext.Current.Request.RequestContext.HttpContext.User.Identity.Name;
        }


        [HttpGet]
        public JsonResult Index(int id)
        {
            var p = _SalaryLineService.GetSalaryLineListForIndex(id).ToList();

            SalaryHeader Header = new SalaryHeaderService(_unitOfWork).Find(id);

            return Json(p, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult _Index(int id, int Status)
        {
            ViewBag.Status = Status;
            ViewBag.SalaryHeaderId = id;
            var p = _SalaryLineService.GetSalaryLineListForIndex(id).ToList();
            return PartialView(p);
        }


        [HttpGet]
        public ActionResult _SalaryLineReferenceIndex(int id)
        {
            IQueryable<SalaryLineReferenceIndexViewModel> p = new SalaryLineService(_unitOfWork).GetSalaryLineReferenceList(id);
            SalaryLineReferenceSummaryViewModel vm = new SalaryLineReferenceSummaryViewModel();
            vm.SalaryLineReferenceSummaryVM = p.ToList();
            ViewBag.DocNo = p.ToList().FirstOrDefault().SalaryHeaderDocNo +"-"+ p.ToList().FirstOrDefault().PersonName;
            return PartialView("_SalaryLineReference", vm);
        }


        private void PrepareViewBag(SalaryLineViewModel vm)
        {
            ViewBag.DeliveryUnitList = new UnitService(_unitOfWork).GetUnitList().ToList();
            if (vm != null)
            {
                SalaryHeaderViewModel H = new SalaryHeaderService(_unitOfWork).GetSalaryHeader(vm.SalaryHeaderId);
                ViewBag.DocNo = H.DocTypeName + "-" + H.DocNo;
            }
        }

        [HttpGet]
        public ActionResult CreateLine(int id, bool? IsRefBased)
        {
            return _Create(id, null, IsRefBased);
        }

        [HttpGet]
        public ActionResult CreateLineAfter_Submit(int id, bool? IsRefBased)
        {
            return _Create(id, null, IsRefBased);
        }

        [HttpGet]
        public ActionResult CreateLineAfter_Approve(int id, bool? IsRefBased)
        {
            return _Create(id, null, IsRefBased);
        }

        public ActionResult _Create(int Id, DateTime? date, bool? IsRefBased) //Id ==>Sale Order Header Id
        {
            SalaryHeader H = new SalaryHeaderService(_unitOfWork).Find(Id);
            SalaryLineViewModel s = new SalaryLineViewModel();

            //Getting Settings
            var settings = new SalarySettingsService(_unitOfWork).GetSalarySettingsForDocument(H.DocTypeId, H.DivisionId, H.SiteId);
            s.SalarySettings = Mapper.Map<SalarySettings, SalarySettingsViewModel>(settings);

            s.DocumentTypeSettings = new DocumentTypeSettingsService(_unitOfWork).GetDocumentTypeSettingsForDocument(H.DocTypeId);


            s.SalaryHeaderId = H.SalaryHeaderId;
            ViewBag.Status = H.Status;
            s.DocTypeId = H.DocTypeId;
            s.SiteId = H.SiteId;
            s.DivisionId = H.DivisionId;
            //if (date != null) s.DueDate = date??DateTime.Today;
            PrepareViewBag(s);
            ViewBag.LineMode = "Create";
            //if (!string.IsNullOrEmpty((string)TempData["CSEXCL"]))
            //{
            //    ViewBag.CSEXCL = TempData["CSEXCL"];
            //    TempData["CSEXCL"] = null;
            //}
            if (IsRefBased == true)
            {
                return PartialView("_CreateForSaleEnquiry", s);

            }
            else
            {
                return PartialView("_Create", s);
            }

        }






        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _CreatePost(SalaryLineViewModel svm)
        {
            List<LogTypeViewModel> LogList = new List<LogTypeViewModel>();
            bool BeforeSave = true;
            SalaryHeader temp = new SalaryHeaderService(_unitOfWork).Find(svm.SalaryHeaderId);

            var settings = new SalarySettingsService(_unitOfWork).GetSalarySettingsForDocument(temp.DocTypeId, temp.DivisionId, temp.SiteId);


            #region BeforeSave
            try
            {

                if (svm.SalaryLineId <= 0)
                    BeforeSave = SalaryDocEvents.beforeLineSaveEvent(this, new JobEventArgs(svm.SalaryHeaderId, EventModeConstants.Add), ref db);
                else
                    BeforeSave = SalaryDocEvents.beforeLineSaveEvent(this, new JobEventArgs(svm.SalaryHeaderId, EventModeConstants.Edit), ref db);

            }
            catch (Exception ex)
            {
                string message = _exception.HandleException(ex);
                TempData["CSEXCL"] += message;
                EventException = true;
            }

            if (!BeforeSave)
                ModelState.AddModelError("", "Validation failed before save.");
            #endregion



            SalaryLine s = Mapper.Map<SalaryLineViewModel, SalaryLine>(svm);

            ViewBag.Status = temp.Status;




            if (svm.SalaryLineId <= 0)
            {
                ViewBag.LineMode = "Create";
            }
            else
            {
                ViewBag.LineMode = "Edit";
            }

            if (ModelState.IsValid && BeforeSave && !EventException)
            {

                if (svm.SalaryLineId <= 0)
                {
                    s.CreatedDate = DateTime.Now;
                    s.ModifiedDate = DateTime.Now;
                    s.CreatedBy = User.Identity.Name;
                    s.Sr = _SalaryLineService.GetMaxSr(s.SalaryHeaderId);
                    s.ModifiedBy = User.Identity.Name;
                    s.ObjectState = Model.ObjectState.Added;
                    db.SalaryLine.Add(s);



                    if (svm.linecharges != null)
                        foreach (var item in svm.linecharges)
                        {
                            item.LineTableId = s.SalaryLineId;
                            item.HeaderTableId = temp.SalaryHeaderId;
                            item.ObjectState = Model.ObjectState.Added;
                            db.SalaryLineCharge.Add(item);
                        }

                    if (svm.footercharges != null)
                        foreach (var item in svm.footercharges)
                        {

                            if (item.Id > 0)
                            {

                                var footercharge = new SalaryHeaderChargeService(_unitOfWork).Find(item.Id);
                                footercharge.Rate = item.Rate;
                                footercharge.Amount = item.Amount;
                                footercharge.ObjectState = Model.ObjectState.Modified;
                                db.SalaryHeaderCharge.Add(footercharge);

                            }

                            else
                            {
                                item.HeaderTableId = s.SalaryHeaderId;
                                item.ObjectState = Model.ObjectState.Added;
                                db.SalaryHeaderCharge.Add(item);
                            }
                        }


                    //SalaryHeader header = new SalaryHeaderService(_unitOfWork).Find(s.SalaryHeaderId);
                    if (temp.Status != (int)StatusConstants.Drafted && temp.Status != (int)StatusConstants.Import)
                    {
                        temp.Status = (int)StatusConstants.Modified;
                        temp.ModifiedDate = DateTime.Now;
                        temp.ModifiedBy = User.Identity.Name;

                    }

                    temp.ObjectState = Model.ObjectState.Modified;
                    db.SalaryHeader.Add(temp);


                    

                    try
                    {
                        SalaryDocEvents.onLineSaveEvent(this, new JobEventArgs(s.SalaryHeaderId, s.SalaryLineId, EventModeConstants.Add), ref db);
                    }
                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        TempData["CSEXCL"] += message;
                        EventException = true;
                    }



                    try
                    {
                        if (EventException)
                        { throw new Exception(); }

                        db.SaveChanges();
                    }

                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        TempData["CSEXCL"] += message;
                        PrepareViewBag(svm);

                        return PartialView("_Create", svm);
                    }


                    try
                    {
                        SalaryDocEvents.afterLineSaveEvent(this, new JobEventArgs(s.SalaryHeaderId, s.SalaryLineId, EventModeConstants.Add), ref db);
                    }
                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        TempData["CSEXCL"] += message;
                    }

                    LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                    {
                        DocTypeId = temp.DocTypeId,
                        DocId = temp.SalaryHeaderId,
                        DocLineId = s.SalaryLineId,
                        ActivityType = (int)ActivityTypeContants.Added,                       
                        DocNo = temp.DocNo,
                        DocDate = temp.DocDate,
                        DocStatus=temp.Status,
                    }));

                    return RedirectToAction("_Create", new { id = svm.SalaryHeaderId });

                }


                else
                {
                    SalaryLine templine = (from p in db.SalaryLine
                                             where p.SalaryLineId == s.SalaryLineId
                                             select p).FirstOrDefault();

                    SalaryLine ExTempLine = new SalaryLine();
                    ExTempLine = Mapper.Map<SalaryLine>(templine);

                    templine.Remark = s.Remark;
                    templine.ModifiedDate = DateTime.Now;
                    templine.ModifiedBy = User.Identity.Name;
                    templine.ObjectState = Model.ObjectState.Modified;
                    db.SalaryLine.Add(templine);

                    int Status = 0;
                    if (temp.Status != (int)StatusConstants.Drafted && temp.Status != (int)StatusConstants.Import)
                    {
                        Status = temp.Status;
                        temp.Status = (int)StatusConstants.Modified;
                        temp.ModifiedBy = User.Identity.Name;
                        temp.ModifiedDate = DateTime.Now;
                    }


                    temp.ObjectState = Model.ObjectState.Modified;
                    db.SalaryHeader.Add(temp);






                    LogList.Add(new LogTypeViewModel
                    {
                        ExObj = ExTempLine,
                        Obj = templine
                    });

                    if (svm.linecharges != null)
                    {
                        var ProductChargeList = (from p in db.SalaryLineCharge
                                                 where p.LineTableId == templine.SalaryLineId
                                                 select p).ToList();

                        foreach (var item in svm.linecharges)
                        {
                            var productcharge = (ProductChargeList.Where(m => m.Id == item.Id)).FirstOrDefault();

                            var ExProdcharge = Mapper.Map<SalaryLineCharge>(productcharge);
                            productcharge.Rate = item.Rate;
                            productcharge.Amount = item.Amount;
                            productcharge.DealQty = 0;
                            LogList.Add(new LogTypeViewModel
                            {
                                ExObj = ExProdcharge,
                                Obj = productcharge
                            });
                            productcharge.ObjectState = Model.ObjectState.Modified;
                            db.SalaryLineCharge.Add(productcharge);
                        }
                    }

                    if (svm.footercharges != null)
                    {
                        var footerChargerecords = (from p in db.SalaryHeaderCharge
                                                   where p.HeaderTableId == temp.SalaryHeaderId
                                                   select p).ToList();

                        foreach (var item in svm.footercharges)
                        {
                            var footercharge = footerChargerecords.Where(m => m.Id == item.Id).FirstOrDefault();
                            var Exfootercharge = Mapper.Map<SalaryHeaderCharge>(footercharge);
                            footercharge.Rate = item.Rate;
                            footercharge.Amount = item.Amount;
                            LogList.Add(new LogTypeViewModel
                            {
                                ExObj = Exfootercharge,
                                Obj = footercharge,
                            });
                            footercharge.ObjectState = Model.ObjectState.Modified;
                            db.SalaryHeaderCharge.Add(footercharge);
                        }
                    }


                    XElement Modifications = new ModificationsCheckService().CheckChanges(LogList);

                    try
                    {
                        SalaryDocEvents.onLineSaveEvent(this, new JobEventArgs(s.SalaryHeaderId, templine.SalaryLineId, EventModeConstants.Edit), ref db);
                    }
                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        TempData["CSEXCL"] += message;
                        EventException = true;
                    }

                    try
                    {
                        if (EventException)
                        { throw new Exception(); }
                        db.SaveChanges();
                    }

                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        TempData["CSEXCL"] += message;
                        PrepareViewBag(svm);
                        return PartialView("_Create", svm);
                    }

                    try
                    {
                        SalaryDocEvents.afterLineSaveEvent(this, new JobEventArgs(s.SalaryHeaderId, templine.SalaryLineId, EventModeConstants.Edit), ref db);
                    }
                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        TempData["CSEXC"] += message;
                    }


                    //Saving the Activity Log

                    LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                    {
                        DocTypeId = temp.DocTypeId,
                        DocId = templine.SalaryHeaderId,
                        DocLineId = templine.SalaryLineId,
                        ActivityType = (int)ActivityTypeContants.Modified,                       
                        DocNo = temp.DocNo,
                        xEModifications = Modifications,
                        DocDate = temp.DocDate,
                        DocStatus=temp.Status,
                    }));

                    //End of Saving the Activity Log

                    return Json(new { success = true });
                }

            }
            PrepareViewBag(svm);
            return PartialView("_Create", svm);
        }





        [HttpGet]
        public ActionResult _ModifyLine(int id)
        {
            return _Modify(id);
        }

        [HttpGet]
        public ActionResult _ModifyLineAfterSubmit(int id)
        {
            return _Modify(id);
        }

        [HttpGet]
        public ActionResult _ModifyLineAfterApprove(int id)
        {
            return _Modify(id);
        }

        private ActionResult _Modify(int id)
        {
            SalaryLineViewModel temp = _SalaryLineService.GetSalaryLine(id);

            SalaryHeader H = new SalaryHeaderService(_unitOfWork).Find(temp.SalaryHeaderId);

            //Getting Settings
            var settings = new SalarySettingsService(_unitOfWork).GetSalarySettingsForDocument(H.DocTypeId, H.DivisionId, H.SiteId);


            temp.SalarySettings = Mapper.Map<SalarySettings, SalarySettingsViewModel>(settings);

            temp.DocumentTypeSettings = new DocumentTypeSettingsService(_unitOfWork).GetDocumentTypeSettingsForDocument(H.DocTypeId);

            //ViewBag.DocNo = H.DocNo;

            if (temp == null)
            {
                return HttpNotFound();
            }
            PrepareViewBag(temp);

            #region DocTypeTimeLineValidation
            try
            {

                TimePlanValidation = DocumentValidation.ValidateDocumentLine(new DocumentUniqueId { LockReason = temp.LockReason }, User.Identity.Name, out ExceptionMsg, out Continue);

            }
            catch (Exception ex)
            {
                string message = _exception.HandleException(ex);
                TempData["CSEXCL"] += message;
                TimePlanValidation = false;
            }

            if (!TimePlanValidation)
                TempData["CSEXCL"] += ExceptionMsg;
            #endregion

            if ((TimePlanValidation || Continue))
                ViewBag.LineMode = "Edit";

            //if (string.IsNullOrEmpty(temp.LockReason))
            //    ViewBag.LineMode = "Edit";
            //else
            //    TempData["CSEXCL"] += temp.LockReason;

            return PartialView("_Create", temp);
        }


        [HttpGet]
        public ActionResult _DeleteLine(int id)
        {
            return _Delete(id);
        }
        [HttpGet]
        public ActionResult _DeleteLine_AfterSubmit(int id)
        {
            return _Delete(id);
        }

        [HttpGet]
        public ActionResult _DeleteLine_AfterApprove(int id)
        {
            return _Delete(id);
        }

        private ActionResult _Delete(int id)
        {
            SalaryLineViewModel temp = _SalaryLineService.GetSalaryLine(id);

            SalaryHeader H = new SalaryHeaderService(_unitOfWork).Find(temp.SalaryHeaderId);

            //Getting Settings
            var settings = new SalarySettingsService(_unitOfWork).GetSalarySettingsForDocument(H.DocTypeId, H.DivisionId, H.SiteId);

            temp.SalarySettings = Mapper.Map<SalarySettings, SalarySettingsViewModel>(settings);
            temp.DocumentTypeSettings = new DocumentTypeSettingsService(_unitOfWork).GetDocumentTypeSettingsForDocument(H.DocTypeId);

            if (temp == null)
            {
                return HttpNotFound();
            }
            PrepareViewBag(temp);
            //ViewBag.LineMode = "Delete";

            #region DocTypeTimeLineValidation
            try
            {

                TimePlanValidation = DocumentValidation.ValidateDocumentLine(new DocumentUniqueId { LockReason = temp.LockReason }, User.Identity.Name, out ExceptionMsg, out Continue);

            }
            catch (Exception ex)
            {
                string message = _exception.HandleException(ex);
                TempData["CSEXCL"] += message;
                TimePlanValidation = false;
            }

            if (!TimePlanValidation)
                TempData["CSEXCL"] += ExceptionMsg;
            #endregion

            if ((TimePlanValidation || Continue))
                ViewBag.LineMode = "Delete";

            return PartialView("_Create", temp);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePost(SalaryLineViewModel vm)
        {
            bool BeforeSave = true;
            try
            {
                BeforeSave = SalaryDocEvents.beforeLineDeleteEvent(this, new JobEventArgs(vm.SalaryHeaderId, vm.SalaryLineId), ref db);
            }
            catch (Exception ex)
            {
                string message = _exception.HandleException(ex);
                TempData["CSEXC"] += message;
                EventException = true;
            }

            if (!BeforeSave)
                TempData["CSEXC"] += "Validation failed before delete.";

            if (BeforeSave && !EventException)
            {

                List<LogTypeViewModel> LogList = new List<LogTypeViewModel>();

                SalaryLine SalaryLine = (from p in db.SalaryLine
                                             where p.SalaryLineId == vm.SalaryLineId
                                             select p).FirstOrDefault();
                SalaryHeader header = (from p in db.SalaryHeader
                                         where p.SalaryHeaderId == SalaryLine.SalaryHeaderId
                                         select p).FirstOrDefault();




                LogList.Add(new LogTypeViewModel
                {
                    Obj = Mapper.Map<SalaryLine>(SalaryLine),
                });


                LedgerLine LedgerLine = (from L in db.LedgerLine
                                   join H in db.LedgerHeader on L.LedgerHeaderId equals H.LedgerHeaderId into LedgerHeaderTable
                                   from LedgerHeaderTab in LedgerHeaderTable.DefaultIfEmpty()
                                   where L.ReferenceDocLineId == SalaryLine.SalaryLineId && L.ReferenceDocTypeId == header.DocTypeId
                                   select L).FirstOrDefault();

                if (LedgerLine != null)
                {
                    LedgerLine.ObjectState = Model.ObjectState.Deleted;
                    db.LedgerLine.Remove(LedgerLine);

                    IEnumerable<Ledger> LedgerList = (from L in db.Ledger where L.LedgerLineId == LedgerLine.LedgerLineId select L).ToList();
                    foreach (Ledger Ledger in LedgerList)
                    {
                        IEnumerable<LedgerAdj> LedgerAdjList = (from L in db.LedgerAdj where L.CrLedgerId == Ledger.LedgerId select L).ToList();

                        foreach (LedgerAdj LedgerAdj in LedgerAdjList)
                        {
                            LedgerAdj.ObjectState = Model.ObjectState.Deleted;
                            db.LedgerAdj.Remove(LedgerAdj);
                        }

                        Ledger.ObjectState = Model.ObjectState.Deleted;
                        db.Ledger.Remove(Ledger);
                    }

                }


                //_SalaryLineService.Delete(SalaryLine);
                SalaryLine.ObjectState = Model.ObjectState.Deleted;
                db.SalaryLine.Remove(SalaryLine);




                


                if (header.Status != (int)StatusConstants.Drafted && header.Status != (int)StatusConstants.Import)
                {
                    header.Status = (int)StatusConstants.Modified;
                    header.ModifiedDate = DateTime.Now;
                    header.ModifiedBy = User.Identity.Name;
                    db.SalaryHeader.Add(header);
                }

                var chargeslist = (from p in db.SalaryLineCharge
                                   where p.LineTableId == vm.SalaryLineId
                                   select p).ToList();

                if (chargeslist != null)
                    foreach (var item in chargeslist)
                    {
                        item.ObjectState = Model.ObjectState.Deleted;
                        db.SalaryLineCharge.Remove(item);
                    }

                if (vm.footercharges != null)
                    foreach (var item in vm.footercharges)
                    {
                        var footer = (from p in db.SalaryHeaderCharge
                                      where p.Id == item.Id
                                      select p).FirstOrDefault();

                        footer.Rate = item.Rate;
                        footer.Amount = item.Amount;
                        footer.ObjectState = Model.ObjectState.Modified;
                        db.SalaryHeaderCharge.Add(footer);
                    }







                XElement Modifications = new ModificationsCheckService().CheckChanges(LogList);

                try
                {
                    SalaryDocEvents.onLineDeleteEvent(this, new JobEventArgs(SalaryLine.SalaryHeaderId, SalaryLine.SalaryLineId), ref db);
                }
                catch (Exception ex)
                {
                    string message = _exception.HandleException(ex);
                    TempData["CSEXCL"] += message;
                    EventException = true;
                }

                try
                {
                    if (EventException)
                        throw new Exception();

                    db.SaveChanges();
                }

                catch (Exception ex)
                {
                    string message = _exception.HandleException(ex);
                    TempData["CSEXCL"] += message;
                    PrepareViewBag(vm);
                    ViewBag.LineMode = "Delete";
                    return PartialView("_Create", vm);

                }

                try
                {
                    SalaryDocEvents.afterLineDeleteEvent(this, new JobEventArgs(SalaryLine.SalaryHeaderId, SalaryLine.SalaryLineId), ref db);
                }
                catch (Exception ex)
                {
                    string message = _exception.HandleException(ex);
                    TempData["CSEXC"] += message;
                }

                //Saving the Activity Log

                LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                {
                    DocTypeId = header.DocTypeId,
                    DocId = header.SalaryHeaderId,
                    DocLineId = SalaryLine.SalaryLineId,
                    ActivityType = (int)ActivityTypeContants.Deleted,                  
                    DocNo = header.DocNo,
                    xEModifications = Modifications,
                    DocDate = header.DocDate,
                    DocStatus=header.Status,
                }));
            }

            return Json(new { success = true });

        }



        public JsonResult GetProductDetailJson(int ProductId, int SalaryId)
        {
            ProductViewModel product = new ProductService(_unitOfWork).GetProduct(ProductId);
            //List<Product> ProductJson = new List<Product>();

            //ProductJson.Add(new Product()
            //{
            //    ProductId = product.ProductId,
            //    StandardCost = product.StandardCost,
            //    UnitId = product.UnitId
            //});            

            var DealUnitId = _SalaryLineService.GetSalaryLineListForIndex(SalaryId).OrderByDescending(m => m.SalaryLineId).FirstOrDefault();

            //Decimal Rate = _SalaryLineService.GetJobRate(SalaryId, ProductId);

            Decimal Rate = 0;
            Decimal Discount = 0;
            Decimal Incentive = 0;
            Decimal Loss = 0;





            var Record = new SalaryHeaderService(_unitOfWork).Find(SalaryId);

            var Settings = new SalarySettingsService(_unitOfWork).GetSalarySettingsForDocument(Record.DocTypeId, Record.DivisionId, Record.SiteId);

            
            return Json(new { ProductId = product.ProductId, 
                StandardCost = Rate, 
                Discount = Discount, 
                Incentive = Incentive, 
                Loss = Loss,
                UnitId = product.UnitId,
                Specification = product.ProductSpecification, 
                SalesTaxGroupProductId = product.SalesTaxGroupProductId,
                SalesTaxGroupProductName = product.SalesTaxGroupProductName});
        }

        protected override void Dispose(bool disposing)
        {
            if (!string.IsNullOrEmpty((string)TempData["CSEXC"]))
            {
                CookieGenerator.CreateNotificationCookie(NotificationTypeConstants.Danger, (string)TempData["CSEXC"]);
                TempData.Remove("CSEXC");
            }
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}
