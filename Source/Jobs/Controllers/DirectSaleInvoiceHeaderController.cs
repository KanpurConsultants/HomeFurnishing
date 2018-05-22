using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Model.Models;
using Data.Models;
using Service;
using Data.Infrastructure;
using Presentation.ViewModels;
using Presentation;
using Core.Common;
using Model.ViewModels;
using System.Configuration;
using AutoMapper;
using Jobs.Helpers;
using Model.ViewModel;
using Reports.Controllers;
using System.Xml.Linq;
using System.Data.SqlClient;
using Reports.Reports;
using System.Data;
using SaleInvoiceDocumentEvents;
using CustomEventArgs;


namespace Jobs.Controllers
{
    [Authorize]
    public class DirectSaleInvoiceHeaderController : System.Web.Mvc.Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();
        private bool EventException = false;

        List<string> UserRoles = new List<string>();
        ActiivtyLogViewModel LogVm = new ActiivtyLogViewModel();

        bool TimePlanValidation = true;
        string ExceptionMsg = "";
        bool Continue = true;

        ISaleInvoiceHeaderService _SaleInvoiceHeaderService;
        ISaleDispatchHeaderService _SaleDispatchHeaderService;
        IPackingHeaderService _PackingHeaderService;
        IActivityLogService _ActivityLogService;
        IUnitOfWork _unitOfWork;
        IExceptionHandlingService _exception;

        public DirectSaleInvoiceHeaderController(ISaleInvoiceHeaderService SaleInvoiceHeaderService, ISaleDispatchHeaderService SaleDispatchHeaderService, IActivityLogService ActivityLogService, IUnitOfWork unitOfWork, IExceptionHandlingService exec, IPackingHeaderService PackingHeaderService)
        {
            _SaleInvoiceHeaderService = SaleInvoiceHeaderService;
            _SaleDispatchHeaderService = SaleDispatchHeaderService;
            _ActivityLogService = ActivityLogService;
            _unitOfWork = unitOfWork;
            _exception = exec;
            _PackingHeaderService = PackingHeaderService;
            if (!SaleInvoiceEvents.Initialized)
            {
                SaleInvoiceEvents Obj = new SaleInvoiceEvents();
            }

            UserRoles = (List<string>)System.Web.HttpContext.Current.Session["Roles"];

            //Log Initialization
            LogVm.SessionId = 0;
            LogVm.ControllerName = System.Web.HttpContext.Current.Request.RequestContext.RouteData.GetRequiredString("controller");
            LogVm.ActionName = System.Web.HttpContext.Current.Request.RequestContext.RouteData.GetRequiredString("action");
            LogVm.User = System.Web.HttpContext.Current.Request.RequestContext.HttpContext.User.Identity.Name;
        }

        private void PrepareViewBag(int id)
        {
            ViewBag.Name = new DocumentTypeService(_unitOfWork).Find(id).DocumentTypeName;
            ViewBag.id = id;
            ViewBag.DealUnitList = new UnitService(_unitOfWork).GetUnitList().ToList();
            ViewBag.ShipMethodList = new ShipMethodService(_unitOfWork).GetShipMethodList().ToList();
            ViewBag.CurrencyList = new CurrencyService(_unitOfWork).GetCurrencyList().ToList();
            ViewBag.SalesTaxGroupList = new ChargeGroupPersonService(_unitOfWork).GetChargeGroupPersonList((int)(TaxTypeConstants.SalesTax)).ToList();
            ViewBag.DeliveryTermsList = new DeliveryTermsService(_unitOfWork).GetDeliveryTermsList().ToList();

            ViewBag.AdminSetting = UserRoles.Contains("Admin").ToString();
            var DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            var SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            var settings = new SaleInvoiceSettingService(_unitOfWork).GetSaleInvoiceSettingForDocument(id, DivisionId, SiteId);
            if (settings != null)
            {
                ViewBag.ImportMenuId = settings.ImportMenuId;
                ViewBag.SqlProcDocumentPrint = settings.SqlProcDocumentPrint;
                ViewBag.ExportMenuId = settings.ExportMenuId;
                ViewBag.SqlProcGatePass = settings.SqlProcGatePass;
            }


        }

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
            if (IndexType == "PTS")
            {
                return RedirectToAction("Index_PendingToSubmit", new { id });
            }
            else if (IndexType == "PTR")
            {
                return RedirectToAction("Index_PendingToReview", new { id });
            }
            IQueryable<SaleInvoiceHeaderIndexViewModel> p = _SaleInvoiceHeaderService.GetSaleInvoiceHeaderList(id, User.Identity.Name);
            PrepareViewBag(id);
            ViewBag.PendingToSubmit = PendingToSubmitCount(id);
            ViewBag.PendingToReview = PendingToReviewCount(id);
            ViewBag.IndexStatus = "All";


            #region "Setting Section"
            var DocumentTypeSettings = new DocumentTypeSettingsService(_unitOfWork).GetDocumentTypeSettingsForDocument(id);
            ViewBag.ProductUidCaption = DocumentTypeSettings.ProductUidCaption ?? "Product Uid";
            ViewBag.ProductCaption = DocumentTypeSettings.ProductCaption ?? "Product";
            ViewBag.ProductGroupCaption = DocumentTypeSettings.ProductGroupCaption ?? "Product Group";

            int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            var settings = new SaleInvoiceSettingService(_unitOfWork).GetSaleInvoiceSettingForDocument(id, DivisionId, SiteId);
            if (settings != null)
            {
                ViewBag.isVisibleProductUid_Index = settings.isVisibleProductUid_Index ?? false;
                ViewBag.isVisibleProduct_Index = settings.isVisibleProduct_Index ?? false;
                ViewBag.isVisibleProductGroup_Index = settings.isVisibleProductGroup_Index ?? false;
            }
            else
            {
                ViewBag.isVisibleProductUid_Index = false;
                ViewBag.isVisibleProduct_Index = false;
                ViewBag.isVisibleProductGroup_Index = false;
            }
            #endregion

            return View(p);
        }

        public ActionResult Index_PendingToSubmit(int id)
        {
            var PendingToSubmit = _SaleInvoiceHeaderService.GetSaleInvoiceHeaderListPendingToSubmit(id, User.Identity.Name);

            ViewBag.Name = new DocumentTypeService(_unitOfWork).Find(id).DocumentTypeName;
            ViewBag.id = id;
            ViewBag.PendingToSubmit = PendingToSubmitCount(id);
            ViewBag.PendingToReview = PendingToReviewCount(id);
            ViewBag.IndexStatus = "PTS";

            #region "Setting Section"
            var DocumentTypeSettings = new DocumentTypeSettingsService(_unitOfWork).GetDocumentTypeSettingsForDocument(id);
            ViewBag.ProductUidCaption = DocumentTypeSettings.ProductUidCaption ?? "Product Uid";
            ViewBag.ProductCaption = DocumentTypeSettings.ProductCaption ?? "Product";
            ViewBag.ProductGroupCaption = DocumentTypeSettings.ProductGroupCaption ?? "Product Group";

            int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            var settings = new SaleInvoiceSettingService(_unitOfWork).GetSaleInvoiceSettingForDocument(id, DivisionId, SiteId);
            if (settings != null)
            {
                ViewBag.isVisibleProductUid_Index = settings.isVisibleProductUid_Index ?? false;
                ViewBag.isVisibleProduct_Index = settings.isVisibleProduct_Index ?? false;
                ViewBag.isVisibleProductGroup_Index = settings.isVisibleProductGroup_Index ?? false;
            }
            else
            {
                ViewBag.isVisibleProductUid_Index = false;
                ViewBag.isVisibleProduct_Index = false;
                ViewBag.isVisibleProductGroup_Index = false;
            }
            #endregion

            return View("Index", PendingToSubmit);
        }

        public ActionResult Index_PendingToReview(int id)
        {
            var PendingtoReview = _SaleInvoiceHeaderService.GetSaleInvoiceHeaderListPendingToReview(id, User.Identity.Name);
            ViewBag.Name = new DocumentTypeService(_unitOfWork).Find(id).DocumentTypeName;
            ViewBag.id = id;
            ViewBag.PendingToSubmit = PendingToSubmitCount(id);
            ViewBag.PendingToReview = PendingToReviewCount(id);
            ViewBag.IndexStatus = "PTR";


            #region "Setting Section"
            var DocumentTypeSettings = new DocumentTypeSettingsService(_unitOfWork).GetDocumentTypeSettingsForDocument(id);
            ViewBag.ProductUidCaption = DocumentTypeSettings.ProductUidCaption ?? "Product Uid";
            ViewBag.ProductCaption = DocumentTypeSettings.ProductCaption ?? "Product";
            ViewBag.ProductGroupCaption = DocumentTypeSettings.ProductGroupCaption ?? "Product Group";

            int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            var settings = new SaleInvoiceSettingService(_unitOfWork).GetSaleInvoiceSettingForDocument(id, DivisionId, SiteId);
            if (settings != null)
            {
                ViewBag.isVisibleProductUid_Index = settings.isVisibleProductUid_Index ?? false;
                ViewBag.isVisibleProduct_Index = settings.isVisibleProduct_Index ?? false;
                ViewBag.isVisibleProductGroup_Index = settings.isVisibleProductGroup_Index ?? false;
            }
            else
            {
                ViewBag.isVisibleProductUid_Index = false;
                ViewBag.isVisibleProduct_Index = false;
                ViewBag.isVisibleProductGroup_Index = false;
            }
            #endregion

            return View("Index", PendingtoReview);
        }



        public ActionResult Create(int id)//DocTypeId
        {
            DirectSaleInvoiceHeaderViewModel vm = new DirectSaleInvoiceHeaderViewModel();

            vm.DocDate = DateTime.Now.Date;
            vm.DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            vm.SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            vm.DocTypeId = id;
            vm.CreatedDate = DateTime.Now;
            List<DocumentTypeHeaderAttributeViewModel> tem = new DocumentTypeService(_unitOfWork).GetDocumentTypeHeaderAttribute(id).ToList();
            vm.DocumentTypeHeaderAttributes = tem;

            //Getting Settings
            var settings = new SaleInvoiceSettingService(_unitOfWork).GetSaleInvoiceSettingForDocument(id, vm.DivisionId, vm.SiteId);

            if (settings == null && UserRoles.Contains("SysAdmin"))
            {
                return RedirectToAction("Create", "SaleInvoiceSetting", new { id = id }).Warning("Please create Sale Invoice settings");
            }
            else if (settings == null && !UserRoles.Contains("SysAdmin"))
            {
                return View("~/Views/Shared/InValidSettings.cshtml");
            }

            if (new RolePermissionService(_unitOfWork).IsActionAllowed(UserRoles, id, settings.ProcessId, this.ControllerContext.RouteData.Values["controller"].ToString(), "Create") == false)
            {
                return View("~/Views/Shared/PermissionDenied.cshtml").Warning("You don't have permission to do this task.");
            }


            if (settings != null)
            {
                vm.ShipMethodId = settings.ShipMethodId;
                vm.DeliveryTermsId = settings.DeliveryTermsId;
                vm.CurrencyId = settings.CurrencyId;
                vm.SalesTaxGroupPersonId = settings.SalesTaxGroupPersonId;
                vm.GodownId = settings.GodownId;
                vm.ProcessId = settings.ProcessId;
            }

            if (settings != null)
            {
                if (settings.CalculationId != null)
                {
                    var CalculationHeaderLedgerAccount = (from H in db.CalculationHeaderLedgerAccount where H.CalculationId == settings.CalculationId && H.DocTypeId == id && H.SiteId == vm.SiteId && H.DivisionId == vm.DivisionId select H).FirstOrDefault();
                    var CalculationLineLedgerAccount = (from H in db.CalculationLineLedgerAccount where H.CalculationId == settings.CalculationId && H.DocTypeId == id && H.SiteId == vm.SiteId && H.DivisionId == vm.DivisionId select H).FirstOrDefault();

                    if (CalculationHeaderLedgerAccount == null && CalculationLineLedgerAccount == null && UserRoles.Contains("SysAdmin"))
                    {
                        return RedirectToAction("Create", "CalculationHeaderLedgerAccount", null).Warning("Ledger posting settings is not defined for current site and division.");
                    }
                    else if (CalculationHeaderLedgerAccount == null && CalculationLineLedgerAccount == null && !UserRoles.Contains("SysAdmin"))
                    {
                        return View("~/Views/Shared/InValidSettings.cshtml").Warning("Ledger posting settings is not defined for current site and division.");
                    }
                }
            }

            vm.DocumentTypeSettings = new DocumentTypeSettingsService(_unitOfWork).GetDocumentTypeSettingsForDocument(vm.DocTypeId);


            int CustomerDoctypeId = 0;
            int? FinancierDocTypeId = null;

            if (new DocumentTypeService(_unitOfWork).Find(MasterDocTypeConstants.Customer) != null)
            {
                CustomerDoctypeId = new DocumentTypeService(_unitOfWork).Find(MasterDocTypeConstants.Customer).DocumentTypeId;
            }
            else if (new DocumentTypeService(_unitOfWork).Find(MasterDocTypeConstants.Customer) != null)
            {
                CustomerDoctypeId = new DocumentTypeService(_unitOfWork).Find(MasterDocTypeConstants.Buyer).DocumentTypeId;
            }

            if (new DocumentTypeService(_unitOfWork).Find(MasterDocTypeConstants.Financier) != null)
            {
                FinancierDocTypeId = new DocumentTypeService(_unitOfWork).Find(MasterDocTypeConstants.Financier).DocumentTypeId;
            }

            vm.BuyerDocTypeId = CustomerDoctypeId;
            vm.FinancierDocTypeId = FinancierDocTypeId;

            vm.SaleInvoiceSettings = Mapper.Map<SaleInvoiceSetting, SaleInvoiceSettingsViewModel>(settings);
            ViewBag.Mode = "Add";
            PrepareViewBag(id);

            vm.DocNo = new DocumentTypeService(_unitOfWork).FGetNewDocNo("DocNo", ConfigurationManager.AppSettings["DataBaseSchema"] + ".SaleInvoiceHeaders", vm.DocTypeId, vm.DocDate, vm.DivisionId, vm.SiteId);
            return View("Create", vm);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePost(DirectSaleInvoiceHeaderViewModel vm)
        {
            SiteDivisionSettings SiteDivisionSettings = new SiteDivisionSettingsService(_unitOfWork).GetSiteDivisionSettings(vm.SiteId, vm.DivisionId, vm.DocDate);
            if (SiteDivisionSettings != null)
            {
                if (SiteDivisionSettings.IsApplicableGST == true)
                {
                    if (vm.SalesTaxGroupPersonId == 0 || vm.SalesTaxGroupPersonId == null)
                    {
                        ModelState.AddModelError("", "Sales Tax Group Person is not defined for party, it is required.");
                    }
                }
            }


            if (vm.SaleInvoiceSettings != null)
            {
                if (vm.SaleInvoiceHeaderId <= 0)
                {
                    var temp = (from H in db.SaleInvoiceHeader
                                where H.DocTypeId == vm.DocTypeId && H.DocNo == vm.DocNo && H.SiteId == vm.SiteId && H.DivisionId == vm.DivisionId
                                select H).FirstOrDefault();

                    if (temp != null)
                    {
                        if (vm.SaleInvoiceSettings.IsAutoDocNo == true)
                        {
                            vm.DocNo = new DocumentTypeService(_unitOfWork).FGetNewDocNo("DocNo", ConfigurationManager.AppSettings["DataBaseSchema"] + ".SaleInvoiceHeaders", vm.DocTypeId, vm.DocDate, vm.DivisionId, vm.SiteId);
                        }
                    }
                }
            }


            if (vm.DocumentTypeHeaderAttributes != null)
            {
                foreach (var pta in vm.DocumentTypeHeaderAttributes)
                {
                    if (pta.DataType == "Number")
                        if (pta.Value != null)
                            if (pta.Value.Replace(".","").All(char.IsDigit) == false)
                                ModelState.AddModelError("", pta.Name + " should be a numeric value.");

                }
            }

            #region DocTypeTimeLineValidation

            try
            {

                if (vm.SaleInvoiceHeaderId <= 0)
                    TimePlanValidation = DocumentValidation.ValidateDocument(Mapper.Map<DocumentUniqueId>(vm), DocumentTimePlanTypeConstants.Create, User.Identity.Name, out ExceptionMsg, out Continue);
                else
                    TimePlanValidation = DocumentValidation.ValidateDocument(Mapper.Map<DocumentUniqueId>(vm), DocumentTimePlanTypeConstants.Modify, User.Identity.Name, out ExceptionMsg, out Continue);

            }
            catch (Exception ex)
            {
                string message = _exception.HandleException(ex);
                TempData["CSEXC"] += message;
                TimePlanValidation = false;
            }

            if (!TimePlanValidation)
                TempData["CSEXC"] += ExceptionMsg;

            #endregion

            if (ModelState.IsValid && (TimePlanValidation || Continue))
            {
                #region CreateRecord
                if (vm.SaleInvoiceHeaderId == 0)
                {
                    PackingHeader packingHeder = Mapper.Map<DirectSaleInvoiceHeaderViewModel, PackingHeader>(vm);
                    SaleDispatchHeader saledispatchheader = Mapper.Map<DirectSaleInvoiceHeaderViewModel, SaleDispatchHeader>(vm);
                    SaleInvoiceHeader saleinvoiceheaderdetail = Mapper.Map<DirectSaleInvoiceHeaderViewModel, SaleInvoiceHeader>(vm);

                    packingHeder.BuyerId = vm.SaleToBuyerId;
                    packingHeder.CreatedBy = User.Identity.Name;
                    packingHeder.ModifiedBy = User.Identity.Name;
                    packingHeder.CreatedDate = DateTime.Now;
                    packingHeder.ModifiedDate = DateTime.Now;
                    packingHeder.ObjectState = Model.ObjectState.Added;
                    _PackingHeaderService.Create(packingHeder);


                    saledispatchheader.PackingHeaderId = packingHeder.PackingHeaderId;
                    saledispatchheader.CreatedDate = DateTime.Now;
                    saledispatchheader.ModifiedDate = DateTime.Now;
                    saledispatchheader.CreatedBy = User.Identity.Name;
                    saledispatchheader.ModifiedBy = User.Identity.Name;
                    saledispatchheader.Status = (int)StatusConstants.Drafted;
                    _SaleDispatchHeaderService.Create(saledispatchheader);


                    saleinvoiceheaderdetail.SaleDispatchHeaderId = saledispatchheader.SaleDispatchHeaderId;
                    saleinvoiceheaderdetail.CreatedDate = DateTime.Now;
                    saleinvoiceheaderdetail.ModifiedDate = DateTime.Now;
                    saleinvoiceheaderdetail.CreatedBy = User.Identity.Name;
                    saleinvoiceheaderdetail.ModifiedBy = User.Identity.Name;
                    saleinvoiceheaderdetail.Status = (int)StatusConstants.Drafted;
                    _SaleInvoiceHeaderService.Create(saleinvoiceheaderdetail);


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
                       DocTypeId = saleinvoiceheaderdetail.DocTypeId,
                       DocId = saleinvoiceheaderdetail.SaleInvoiceHeaderId,
                       ActivityType = (int)ActivityTypeContants.Added,
                       DocNo = saleinvoiceheaderdetail.DocNo,
                       DocDate = saleinvoiceheaderdetail.DocDate,
                       DocStatus = saleinvoiceheaderdetail.Status,
                   }));


                    return RedirectToAction("Modify", new { id = saleinvoiceheaderdetail.SaleInvoiceHeaderId }).Success("Data saved Successfully");
                }
                #endregion

                #region EditRecord
                else
                {
                    List<LogTypeViewModel> LogList = new List<LogTypeViewModel>();

                    SaleInvoiceHeader saleinvoiceheaderdetail = _SaleInvoiceHeaderService.FindDirectSaleInvoice(vm.SaleInvoiceHeaderId);

                    SaleInvoiceHeader ExRec = Mapper.Map<SaleInvoiceHeader>(saleinvoiceheaderdetail);

                    SaleDispatchHeader saledispatchheader = _SaleDispatchHeaderService.Find(saleinvoiceheaderdetail.SaleDispatchHeaderId.Value);

                    StockHeader StockHeader = new StockHeaderService(_unitOfWork).Find(saledispatchheader.StockHeaderId ?? 0);

                    PackingHeader packingHeader = _PackingHeaderService.Find(saledispatchheader.PackingHeaderId.Value);

                    int status = saleinvoiceheaderdetail.Status;

                    bool IsBillToPartyChanged = false;
                    if (vm.BillToBuyerId != saleinvoiceheaderdetail.BillToBuyerId)
                        IsBillToPartyChanged = true;

                    if (saleinvoiceheaderdetail.Status != (int)StatusConstants.Drafted)
                    {
                        saleinvoiceheaderdetail.Status = (int)StatusConstants.Modified;
                        saledispatchheader.Status = (int)StatusConstants.Modified;
                        packingHeader.Status = (int)StatusConstants.Modified;
                    }


                    saleinvoiceheaderdetail.BillToBuyerId = vm.BillToBuyerId;
                    saleinvoiceheaderdetail.SaleToBuyerId = vm.SaleToBuyerId;
                    saleinvoiceheaderdetail.CurrencyId = vm.CurrencyId;
                    saleinvoiceheaderdetail.DocDate = vm.DocDate;
                    saleinvoiceheaderdetail.DocNo = vm.DocNo;
                    saleinvoiceheaderdetail.FinancierId = vm.FinancierId;
                    saleinvoiceheaderdetail.SalesExecutiveId = vm.SalesExecutiveId;
                    saleinvoiceheaderdetail.SalesTaxGroupPersonId = vm.SalesTaxGroupPersonId;
                    saleinvoiceheaderdetail.Remark = vm.Remark;
                    saleinvoiceheaderdetail.TermsAndConditions = vm.TermsAndConditions;
                    saleinvoiceheaderdetail.ModifiedDate = DateTime.Now;
                    saleinvoiceheaderdetail.ModifiedBy = User.Identity.Name;
                    _SaleInvoiceHeaderService.Update(saleinvoiceheaderdetail);


                    saledispatchheader.DocNo = vm.DocNo;
                    saledispatchheader.DocDate = vm.DocDate;
                    saledispatchheader.SaleToBuyerId = vm.SaleToBuyerId;
                    saledispatchheader.ShipToPartyAddress = vm.ShipToPartyAddress;
                    saledispatchheader.Remark = vm.Remark;
                    saledispatchheader.ModifiedDate = DateTime.Now;
                    saledispatchheader.ModifiedBy = User.Identity.Name;
                    _SaleDispatchHeaderService.Update(saledispatchheader);


                    if (StockHeader != null)
                    {
                        StockHeader.DocNo = vm.DocNo;
                        StockHeader.DocDate = vm.DocDate;
                        StockHeader.PersonId = vm.SaleToBuyerId;
                        StockHeader.GodownId = vm.GodownId;
                        StockHeader.Remark = vm.Remark;
                        StockHeader.ModifiedDate = DateTime.Now;
                        StockHeader.ModifiedBy = User.Identity.Name;
                        new StockHeaderService(_unitOfWork).Update(StockHeader);
                    }

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




                    packingHeader.BuyerId = vm.SaleToBuyerId;
                    packingHeader.DocDate = vm.DocDate;
                    packingHeader.DocNo = vm.DocNo;
                    packingHeader.GodownId = vm.GodownId;
                    packingHeader.Remark = vm.Remark;
                    packingHeader.ObjectState = Model.ObjectState.Modified;
                    packingHeader.ModifiedDate = DateTime.Now;
                    packingHeader.ModifiedBy = User.Identity.Name;
                    _unitOfWork.Repository<PackingHeader>().Update(packingHeader);



                    if (IsBillToPartyChanged == true)
                    {
                        IEnumerable<SaleInvoiceHeaderCharge> HeaderChargeList = (from Hc in db.SaleInvoiceHeaderCharge where Hc.HeaderTableId == saleinvoiceheaderdetail.SaleInvoiceHeaderId select Hc).ToList();
                        foreach (SaleInvoiceHeaderCharge HeaderCharge in HeaderChargeList)
                        {
                            HeaderCharge.PersonID = vm.BillToBuyerId;
                            _unitOfWork.Repository<SaleInvoiceHeaderCharge>().Update(HeaderCharge);
                        }

                        IEnumerable<SaleInvoiceLineCharge> LineChargeList = (from Hc in db.SaleInvoiceLineCharge where Hc.HeaderTableId == saleinvoiceheaderdetail.SaleInvoiceHeaderId select Hc).ToList();
                        foreach (SaleInvoiceLineCharge LineCharge in LineChargeList)
                        {
                            LineCharge.PersonID = vm.BillToBuyerId;
                            _unitOfWork.Repository<SaleInvoiceLineCharge>().Update(LineCharge);
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
                }
                #endregion

            }
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
            if (header.Status == (int)StatusConstants.Drafted)
                return Edit(id, IndexType);
            else
                return HttpNotFound();
        }

        [HttpGet]
        public ActionResult ModifyAfter_Submit(int id, string IndexType)
        {
            SaleInvoiceHeader header = _SaleInvoiceHeaderService.FindDirectSaleInvoice(id);
            if (header.Status == (int)StatusConstants.Submitted || header.Status == (int)StatusConstants.Modified)
                return Edit(id, IndexType);
            else
                return HttpNotFound();
        }



        private ActionResult Edit(int id, string IndexType)
        {

            ViewBag.IndexStatus = IndexType;
            SaleInvoiceHeader s = _SaleInvoiceHeaderService.FindDirectSaleInvoice(id);
            SaleDispatchHeader DispactchHeader = _SaleDispatchHeaderService.Find(s.SaleDispatchHeaderId.Value);
            PackingHeader packingHeader = _PackingHeaderService.Find(DispactchHeader.PackingHeaderId.Value);

            if (s.Status != (int)StatusConstants.Drafted)
                if (new RolePermissionService(_unitOfWork).IsActionAllowed(UserRoles, s.DocTypeId, null, this.ControllerContext.RouteData.Values["controller"].ToString(), "Edit") == false)
                    return RedirectToAction("DetailInformation", new { id = id, IndexType = IndexType }).Warning("You don't have permission to do this task.");


            DirectSaleInvoiceHeaderViewModel vm = new DirectSaleInvoiceHeaderViewModel();

            string SiteName = db.Site.Find(s.SiteId).SiteName;
            int LoginSiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            if (s.SiteId != LoginSiteId)
                s.LockReason = "Can't modify " + SiteName + " record.You have to login with " + SiteName;
                


            #region DocTypeTimeLineValidation
            try
            {
                TimePlanValidation = DocumentValidation.ValidateDocument(Mapper.Map<DocumentUniqueId>(s), DocumentTimePlanTypeConstants.Modify, User.Identity.Name, out ExceptionMsg, out Continue);

            }
            catch (Exception ex)
            {
                string message = _exception.HandleException(ex);
                TempData["CSEXC"] += message;
                TimePlanValidation = false;
            }

            if (!TimePlanValidation)
                TempData["CSEXC"] += ExceptionMsg;
            #endregion

            if ((!TimePlanValidation && !Continue))
            {
                return RedirectToAction("DetailInformation", new { id = id, IndexType = IndexType });
            }

            

            vm = Mapper.Map<SaleInvoiceHeader, DirectSaleInvoiceHeaderViewModel>(s);
            vm.SaleToBuyerId = DispactchHeader.SaleToBuyerId;
            vm.DeliveryTermsId = DispactchHeader.DeliveryTermsId;
            vm.ShipToPartyAddress = DispactchHeader.ShipToPartyAddress;
            vm.GodownId = packingHeader.GodownId;

            var SaleInvoiceReturn = db.SaleInvoiceReturnLine.Where(i => i.SaleInvoiceLine.SaleInvoiceHeaderId == id).FirstOrDefault();
            if (SaleInvoiceReturn != null)
            {
                vm.ReturnNature = (from H in db.SaleInvoiceReturnHeader where H.SaleInvoiceReturnHeaderId == SaleInvoiceReturn.SaleInvoiceReturnHeaderId select new { DocTypeNature = H.DocType.Nature }).FirstOrDefault().DocTypeNature;
                if (vm.ReturnNature == TransactionNatureConstants.Credit)
                    vm.AdditionalInfo = "Credit Note is generated for this invoice.";
                else if (vm.ReturnNature == TransactionNatureConstants.Debit)
                    vm.AdditionalInfo = "Debit Note is generated for this invoice.";
                else
                    vm.AdditionalInfo = "Invoice is cancelled.";
            }

            var Line = (from L in db.SaleInvoiceLine where L.SaleInvoiceHeaderId == id select L).FirstOrDefault();
            if (Line != null)
            {
                if (Line.SaleOrderLineId != null)
                    vm.IsPartyLocked = true;
                else
                {
                    if (s.SaleDispatchHeaderId == null && Line.SaleDispatchLineId != null)
                        vm.IsPartyLocked = true;
                }
            }

            List<DocumentTypeHeaderAttributeViewModel> tem = _SaleInvoiceHeaderService.GetDocumentHeaderAttribute(id).ToList();
            vm.DocumentTypeHeaderAttributes = tem;

            //Getting Settings
            var settings = new SaleInvoiceSettingService(_unitOfWork).GetSaleInvoiceSettingForDocument(s.DocTypeId, vm.DivisionId, vm.SiteId);

            if (settings == null && UserRoles.Contains("SysAdmin"))
            {
                return RedirectToAction("Create", "SaleInvoiceSetting", new { id = s.DocTypeId }).Warning("Please create Sale Invoice settings");
            }
            else if (settings == null && !UserRoles.Contains("SysAdmin"))
            {
                return View("~/Views/Shared/InValidSettings.cshtml");
            }

            if (settings != null)
            {
                vm.ProcessId = settings.ProcessId;
            }

            vm.DocumentTypeSettings = new DocumentTypeSettingsService(_unitOfWork).GetDocumentTypeSettingsForDocument(vm.DocTypeId);

            int CustomerDoctypeId = 0;
            int? FinancierDocTypeId = null;

            if (new DocumentTypeService(_unitOfWork).Find(MasterDocTypeConstants.Customer) != null)
            {
                CustomerDoctypeId = new DocumentTypeService(_unitOfWork).Find(MasterDocTypeConstants.Customer).DocumentTypeId;
            }
            else if (new DocumentTypeService(_unitOfWork).Find(MasterDocTypeConstants.Customer) != null)
            {
                CustomerDoctypeId = new DocumentTypeService(_unitOfWork).Find(MasterDocTypeConstants.Buyer).DocumentTypeId;
            }

            if (new DocumentTypeService(_unitOfWork).Find(MasterDocTypeConstants.Financier) != null)
            {
                FinancierDocTypeId = new DocumentTypeService(_unitOfWork).Find(MasterDocTypeConstants.Financier).DocumentTypeId;
            }

            vm.BuyerDocTypeId = CustomerDoctypeId;
            vm.FinancierDocTypeId = FinancierDocTypeId;


            vm.SaleInvoiceSettings = Mapper.Map<SaleInvoiceSetting, SaleInvoiceSettingsViewModel>(settings);
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

        [HttpGet]
        public ActionResult DetailInformation(int id, string IndexType)
        {
            return RedirectToAction("Detail", new { id = id, transactionType = "detail", IndexType = IndexType });
        }

        [Authorize]
        public ActionResult Detail(int id, string transactionType, string IndexType)
        {
            ViewBag.transactionType = transactionType;
            ViewBag.IndexStatus = IndexType;

            SaleInvoiceHeader s = _SaleInvoiceHeaderService.FindDirectSaleInvoice(id);
            SaleDispatchHeader DispactchHeader = _SaleDispatchHeaderService.Find(s.SaleDispatchHeaderId.Value);
            PackingHeader packingHeader = _PackingHeaderService.Find(DispactchHeader.PackingHeaderId.Value);

            DirectSaleInvoiceHeaderViewModel vm = new DirectSaleInvoiceHeaderViewModel();
            vm = Mapper.Map<SaleInvoiceHeader, DirectSaleInvoiceHeaderViewModel>(s);
            vm.SaleToBuyerId = DispactchHeader.SaleToBuyerId;
            vm.DeliveryTermsId = DispactchHeader.DeliveryTermsId;
            vm.GodownId = packingHeader.GodownId;

            

            var SaleInvoiceReturn = db.SaleInvoiceReturnLine.Where(i => i.SaleInvoiceLine.SaleInvoiceHeaderId == id).FirstOrDefault();
            if (SaleInvoiceReturn != null)
            {
                vm.ReturnNature = (from H in db.SaleInvoiceReturnHeader where H.SaleInvoiceReturnHeaderId == SaleInvoiceReturn.SaleInvoiceReturnHeaderId select new { DocTypeNature = H.DocType.Nature }).FirstOrDefault().DocTypeNature;
                if (vm.ReturnNature == TransactionNatureConstants.Credit)
                    vm.LockReason = "Credit Note is generated for this invoice.";
                else
                    vm.LockReason = "Invoice is cancelled.";
            }



            //Getting Settings
            var settings = new SaleInvoiceSettingService(_unitOfWork).GetSaleInvoiceSettingForDocument(s.DocTypeId, s.DivisionId, s.SiteId);

            if (settings == null && UserRoles.Contains("SysAdmin"))
            {
                return RedirectToAction("Create", "SaleInvoiceSetting", new { id = id }).Warning("Please create Sale Invoice settings");
            }
            else if (settings == null && !UserRoles.Contains("SysAdmin"))
            {
                return View("~/Views/Shared/InValidSettings.cshtml");
            }
            vm.SaleInvoiceSettings = Mapper.Map<SaleInvoiceSetting, SaleInvoiceSettingsViewModel>(settings);

            vm.SiteName = db.Site.Find(vm.SiteId).SiteName;

            

            vm.DocumentTypeSettings = new DocumentTypeSettingsService(_unitOfWork).GetDocumentTypeSettingsForDocument(vm.DocTypeId);
            PrepareViewBag(s.DocTypeId);

            if (String.IsNullOrEmpty(transactionType) || transactionType == "detail")
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


        [HttpGet]
        public ActionResult Delete(int id)
        {
            SaleInvoiceHeader header = _SaleInvoiceHeaderService.FindDirectSaleInvoice(id);
            if (header.Status == (int)StatusConstants.Drafted)
                return Remove(id);
            else
                return HttpNotFound();
        }

        [HttpGet]
        public ActionResult DeleteAfter_Submit(int id)
        {
            SaleInvoiceHeader header = _SaleInvoiceHeaderService.FindDirectSaleInvoice(id);
            if (header.Status == (int)StatusConstants.Submitted || header.Status == (int)StatusConstants.Modified || header.Status == (int)StatusConstants.ModificationSubmitted)
                return Remove(id);
            else
                return HttpNotFound();
        }

        [HttpGet]
        public ActionResult DeleteAfter_Approve(int id)
        {
            SaleInvoiceHeader header = _SaleInvoiceHeaderService.FindDirectSaleInvoice(id);
            if (header.Status == (int)StatusConstants.Approved)
                return Remove(id);
            else
                return HttpNotFound();
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

            #region DocTypeTimeLineValidation
            try
            {
                TimePlanValidation = DocumentValidation.ValidateDocument(Mapper.Map<DocumentUniqueId>(SaleinvoiceHeader), DocumentTimePlanTypeConstants.Delete, User.Identity.Name, out ExceptionMsg, out Continue);
                TempData["CSEXC"] += ExceptionMsg;
            }
            catch (Exception ex)
            {
                string message = _exception.HandleException(ex);
                TempData["CSEXC"] += message;
                TimePlanValidation = false;
            }

            if (!TimePlanValidation && !Continue)
            {
                return PartialView("AjaxError");
            }
            #endregion

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
                SaleDispatchHeader Sd = (from H in db.SaleDispatchHeader where H.SaleDispatchHeaderId == Si.SaleDispatchHeaderId select H).FirstOrDefault();
                PackingHeader Ph = (from H in db.PackingHeader where H.PackingHeaderId == Sd.PackingHeaderId select H).FirstOrDefault();
                LedgerHeader LH = (from H in db.LedgerHeader where H.LedgerHeaderId == Si.LedgerHeaderId select H).FirstOrDefault();
                StockHeader SH = (from H in db.StockHeader where H.StockHeaderId == Sd.StockHeaderId select H).FirstOrDefault();


                //IEnumerable<Stock> StockList = (from L in db.Stock where L.StockHeaderId == Sd.StockHeaderId select L).ToList();
                //foreach(Stock Stock in StockList)
                //{
                //    Stock.ObjectState = Model.ObjectState.Deleted;
                //    db.Stock.Remove(Stock);
                //}

                IEnumerable<Ledger> LedgerList = (from L in db.Ledger where L.LedgerHeaderId == Si.LedgerHeaderId select L).ToList();
                foreach (Ledger Ledger in LedgerList)
                {
                    var LedgerAdjs = (from p in db.LedgerAdj
                                      where p.CrLedgerId == Ledger.LedgerId || p.DrLedgerId == Ledger.LedgerId
                                      select p).ToList();

                    foreach (var item2 in LedgerAdjs)
                    {
                        item2.ObjectState = Model.ObjectState.Deleted;
                        db.LedgerAdj.Remove(item2);
                    }

                    Ledger.ObjectState = Model.ObjectState.Deleted;
                    db.Ledger.Remove(Ledger);
                }



                //new StockService(_unitOfWork).DeleteStockForDocHeader(Sd.SaleDispatchHeaderId, Sd.DocTypeId, Sd.SiteId, Sd.DivisionId, db);
                //new LedgerService(_unitOfWork).DeleteLedgerForDocHeader(Si.SaleInvoiceHeaderId, Si.DocTypeId, Si.SiteId, Si.DivisionId);

                var attributes = (from A in db.SaleInvoiceHeaderAttributes where A.HeaderTableId == vm.id select A).ToList();

                foreach (var ite2 in attributes)
                {
                    ite2.ObjectState = Model.ObjectState.Deleted;
                    db.SaleInvoiceHeaderAttributes.Remove(ite2);
                }


                var GatePassHEader = (from p in db.GatePassHeader
                                      where p.GatePassHeaderId == Sd.GatePassHeaderId
                                      select p).FirstOrDefault();

                if (Sd.GatePassHeaderId.HasValue)
                {
                    var GatePassLines = (from p in db.GatePassLine
                                         where p.GatePassHeaderId == GatePassHEader.GatePassHeaderId
                                         select p).ToList();


                    foreach (var item in GatePassLines)
                    {
                        item.ObjectState = Model.ObjectState.Deleted;
                        db.GatePassLine.Remove(item);
                    }

                    GatePassHEader.ObjectState = Model.ObjectState.Deleted;
                    db.GatePassHeader.Remove(GatePassHEader);
                }


                var SaleInvoiceLine = (from L in db.SaleInvoiceLine where L.SaleInvoiceHeaderId == vm.id select L).ToList();


                int cnt = 0;
                foreach (var item in SaleInvoiceLine)
                {

                    cnt = cnt + 1;

                    var linecharges = (from L in db.SaleInvoiceLineCharge where L.LineTableId == item.SaleInvoiceLineId select L).ToList();

                    foreach (var citem in linecharges)
                    {
                        citem.ObjectState = Model.ObjectState.Deleted;
                        //db.SaleInvoiceLineCharge.Attach(citem);
                        db.SaleInvoiceLineCharge.Remove(citem);
                    }

                    SaleInvoiceLineDetail LineDetail = (from L in db.SaleInvoiceLineDetail where L.SaleInvoiceLineId == item.SaleInvoiceLineId select L).FirstOrDefault();
                    if (LineDetail != null)
                    {
                        LineDetail.ObjectState = Model.ObjectState.Deleted;
                        //db.SaleInvoiceLineDetail.Attach(LineDetail);
                        db.SaleInvoiceLineDetail.Remove(LineDetail);
                    }


                    item.ObjectState = Model.ObjectState.Deleted;
                    //db.SaleInvoiceLine.Attach(item);
                    db.SaleInvoiceLine.Remove(item);

                }


                List<int> StockIdList = new List<int>();


                var SaleDispatchLine = (from L in db.SaleDispatchLine where L.SaleDispatchHeaderId == Sd.SaleDispatchHeaderId select L).ToList();

                foreach (var item in SaleDispatchLine)
                {
                    if (item.StockId != null)
                    {
                        StockIdList.Add((int)item.StockId);
                    }

                    item.ObjectState = Model.ObjectState.Deleted;
                    //db.SaleDispatchLine.Attach(item);
                    db.SaleDispatchLine.Remove(item);
                }

                var PackingLine = (from L in db.PackingLine where L.PackingHeaderId == Ph.PackingHeaderId select L).ToList();

                foreach (var item in PackingLine)
                {
                    item.ObjectState = Model.ObjectState.Deleted;
                    //db.PackingLine.Attach(item);
                    db.PackingLine.Remove(item);
                }


                foreach (var item in StockIdList)
                {
                    Stock Stock = db.Stock.Find(item);
                    Stock.ObjectState = Model.ObjectState.Deleted;
                    //db.Stock.Attach(Stock);
                    db.Stock.Remove(Stock);
                }


                //var ledges = (from L in db.Ledger where L.LedgerHeaderId == Si.LedgerHeaderId select L).ToList();
                //foreach (var item in ledges)
                //{
                //    item.ObjectState = Model.ObjectState.Deleted;
                //    db.Ledger.Attach(item);
                //    db.Ledger.Remove(item);
                //}





                var headercharges = (from L in db.SaleInvoiceHeaderCharge where L.HeaderTableId == Si.SaleInvoiceHeaderId select L).ToList();

                foreach (var citem in headercharges)
                {
                    citem.ObjectState = Model.ObjectState.Deleted;
                    //db.SaleInvoiceHeaderCharge.Attach(citem);
                    db.SaleInvoiceHeaderCharge.Remove(citem);
                }



                Si.ObjectState = Model.ObjectState.Deleted;
                //db.SaleInvoiceHeader.Attach(Si);
                db.SaleInvoiceHeader.Remove(Si);

                Sd.ObjectState = Model.ObjectState.Deleted;
                //db.SaleDispatchHeader.Attach(Sd);
                db.SaleDispatchHeader.Remove(Sd);

                Ph.ObjectState = Model.ObjectState.Deleted;
                //db.PackingHeader.Attach(Ph);
                db.PackingHeader.Remove(Ph);

                if (LH != null)
                {
                    LH.ObjectState = Model.ObjectState.Deleted;
                    //db.LedgerHeader.Attach(LH);
                    db.LedgerHeader.Remove(LH);
                }

                if (SH != null)
                {
                    SH.ObjectState = Model.ObjectState.Deleted;
                    //db.StockHeader.Attach(SH);
                    db.StockHeader.Remove(SH);
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





        public ActionResult Submit(int id, string IndexType, string TransactionType)
        {
            SaleInvoiceHeader s = db.SaleInvoiceHeader.Find(id);
            if (new RolePermissionService(_unitOfWork).IsActionAllowed(UserRoles, s.DocTypeId, null, this.ControllerContext.RouteData.Values["controller"].ToString(), "Submit") == false)
            {
                return View("~/Views/Shared/PermissionDenied.cshtml").Warning("You don't have permission to do this task.");
            }

            #region DocTypeTimeLineValidation

            
            try
            {
                TimePlanValidation = Submitvalidation(id, out ExceptionMsg);
                TempData["CSEXC"] += ExceptionMsg;
            }
            catch (Exception ex)
            {
                string message = _exception.HandleException(ex);
                TempData["CSEXC"] += message;
                TimePlanValidation = false;
            }
            try
            {
                TimePlanValidation = DocumentValidation.ValidateDocument(Mapper.Map<DocumentUniqueId>(s), DocumentTimePlanTypeConstants.Submit, User.Identity.Name, out ExceptionMsg, out Continue);
                TempData["CSEXC"] += ExceptionMsg;
            }
            catch (Exception ex)
            {
                string message = _exception.HandleException(ex);
                TempData["CSEXC"] += message;
                TimePlanValidation = false;
            }

            if (!TimePlanValidation && !Continue)
            {
                return RedirectToAction("Index", new { id = s.DocTypeId, IndexType = IndexType });
            }
            #endregion

            return RedirectToAction("Detail", new { id = id, IndexType = IndexType, transactionType = string.IsNullOrEmpty(TransactionType) ? "submit" : TransactionType });

        }


        [HttpPost, ActionName("Detail")]
        [MultipleButton(Name = "Command", Argument = "Submit")]
        public ActionResult Submitted(int Id, string IndexType, string UserRemark, string IsContinue, string GenGatePass)
        {
            //int SaleAc = 6650;
            int ActivityType;

            bool BeforeSave = true;
            try
            {
                BeforeSave = SaleInvoiceDocEvents.beforeHeaderSubmitEvent(this, new SaleEventArgs(Id), ref db);
            }
            catch (Exception ex)
            {
                string message = _exception.HandleException(ex);
                TempData["CSEXC"] += message;
                EventException = true;
            }

            if (!BeforeSave)
                TempData["CSEXC"] += "Falied validation before submit.";


            SaleInvoiceHeader pd = new SaleInvoiceHeaderService(_unitOfWork).FindDirectSaleInvoice(Id);
            SaleDispatchHeader Dh = _SaleDispatchHeaderService.Find(pd.SaleDispatchHeaderId.Value);
            PackingHeader Ph = _PackingHeaderService.Find(Dh.PackingHeaderId.Value);

            if (ModelState.IsValid && BeforeSave && !EventException)
            {
                if (User.Identity.Name == pd.ModifiedBy || UserRoles.Contains("Admin"))
                {

                    pd.Status = (int)StatusConstants.Submitted;
                    Dh.Status = (int)StatusConstants.Submitted;
                    Ph.Status = (int)StatusConstants.Submitted;
                    ActivityType = (int)ActivityTypeContants.Submitted;

                    pd.ReviewBy = null;
                    Dh.ReviewBy = null;
                    Ph.ReviewBy = null;





                    SaleInvoiceSetting Settings = new SaleInvoiceSettingService(_unitOfWork).GetSaleInvoiceSettingForDocument(pd.DocTypeId, pd.DivisionId, pd.SiteId);

                    if (!string.IsNullOrEmpty(GenGatePass) && GenGatePass == "true")
                    {

                        if (!String.IsNullOrEmpty(Settings.SqlProcGatePass))
                        {
                            SqlParameter SqlParameterId = new SqlParameter("@Id", Dh.SaleDispatchHeaderId);
                            IEnumerable<GatePassGeneratedViewModel> GatePasses = db.Database.SqlQuery<GatePassGeneratedViewModel>(Settings.SqlProcGatePass + " @Id", SqlParameterId).ToList();

                            if (Dh.GatePassHeaderId == null)
                            {
                                SqlParameter DocDate = new SqlParameter("@DocDate", DateTime.Now.Date);
                                DocDate.SqlDbType = SqlDbType.DateTime;
                                SqlParameter Godown = new SqlParameter("@GodownId", Ph.GodownId);
                                SqlParameter DocType = new SqlParameter("@DocTypeId", new DocumentTypeService(_unitOfWork).Find(TransactionDoctypeConstants.GatePass).DocumentTypeId);
                                GatePassHeader GPHeader = new GatePassHeader();
                                GPHeader.CreatedBy = User.Identity.Name;
                                GPHeader.CreatedDate = DateTime.Now;
                                GPHeader.DivisionId = pd.DivisionId;
                                GPHeader.DocDate = DateTime.Now.Date;
                                GPHeader.DocNo = db.Database.SqlQuery<string>("Web.GetNewDocNoGatePass @DocTypeId, @DocDate, @GodownId ", DocType, DocDate, Godown).FirstOrDefault();
                                GPHeader.DocTypeId = new DocumentTypeService(_unitOfWork).FindByName(MasterDocTypeConstants.GatePass).DocumentTypeId;
                                GPHeader.ModifiedBy = User.Identity.Name;
                                GPHeader.ModifiedDate = DateTime.Now;
                                GPHeader.Remark = pd.Remark;
                                GPHeader.PersonId = pd.SaleToBuyerId;
                                GPHeader.SiteId = pd.SiteId;
                                GPHeader.GodownId = Ph.GodownId;
                                GPHeader.ReferenceDocTypeId = pd.DocTypeId;
                                GPHeader.ReferenceDocId = pd.SaleInvoiceHeaderId;
                                GPHeader.ReferenceDocNo = pd.DocNo;
                                GPHeader.ObjectState = Model.ObjectState.Added;
                                //db.GatePassHeader.Add(GPHeader);
                                new GatePassHeaderService(_unitOfWork).Create(GPHeader);

                                //new GatePassHeaderService(_unitOfWork).Create(GPHeader);


                                foreach (GatePassGeneratedViewModel item in GatePasses)
                                {
                                    GatePassLine Gline = new GatePassLine();
                                    Gline.CreatedBy = User.Identity.Name;
                                    Gline.CreatedDate = DateTime.Now;
                                    Gline.GatePassHeaderId = GPHeader.GatePassHeaderId;
                                    Gline.ModifiedBy = User.Identity.Name;
                                    Gline.ModifiedDate = DateTime.Now;
                                    Gline.Product = item.ProductName;
                                    Gline.Qty = item.Qty;
                                    Gline.Specification = item.Specification;
                                    Gline.UnitId = item.UnitId;
                                    // new GatePassLineService(_unitOfWork).Create(Gline);
                                    Gline.ObjectState = Model.ObjectState.Added;
                                    //db.GatePassLine.Add(Gline);
                                    new GatePassLineService(_unitOfWork).Create(Gline);
                                }

                                Dh.GatePassHeaderId = GPHeader.GatePassHeaderId;

                            }
                            else
                            {
                                //List<GatePassLine> LineList = new GatePassLineService(_unitOfWork).GetGatePassLineList(pd.GatePassHeaderId ?? 0).ToList();

                                List<GatePassLine> LineList = (from p in db.GatePassLine
                                                               where p.GatePassHeaderId == Dh.GatePassHeaderId
                                                               select p).ToList();

                                foreach (var ittem in LineList)
                                {

                                    ittem.ObjectState = Model.ObjectState.Deleted;
                                    //db.GatePassLine.Remove(ittem);

                                    new GatePassLineService(_unitOfWork).Delete(ittem);
                                }

                                GatePassHeader GPHeader = new GatePassHeaderService(_unitOfWork).Find(Dh.GatePassHeaderId ?? 0);

                                GPHeader.PersonId = pd.SaleToBuyerId;
                                GPHeader.GodownId = Ph.GodownId;

                                foreach (GatePassGeneratedViewModel item in GatePasses)
                                {
                                    GatePassLine Gline = new GatePassLine();
                                    Gline.CreatedBy = User.Identity.Name;
                                    Gline.CreatedDate = DateTime.Now;
                                    Gline.GatePassHeaderId = GPHeader.GatePassHeaderId;
                                    Gline.ModifiedBy = User.Identity.Name;
                                    Gline.ModifiedDate = DateTime.Now;
                                    Gline.Product = item.ProductName;
                                    Gline.Qty = item.Qty;
                                    Gline.Specification = item.Specification;
                                    Gline.UnitId = item.UnitId;

                                    //new GatePassLineService(_unitOfWork).Create(Gline);
                                    Gline.ObjectState = Model.ObjectState.Added;
                                    //db.GatePassLine.Add(Gline);
                                    new GatePassLineService(_unitOfWork).Create(Gline);
                                }

                                new GatePassHeaderService(_unitOfWork).Update(GPHeader);
                                Dh.GatePassHeaderId = GPHeader.GatePassHeaderId;

                            }
                        }

                    }



                    _SaleDispatchHeaderService.Update(Dh);
                    _PackingHeaderService.Update(Ph);
                    _SaleInvoiceHeaderService.Update(pd);






                    #region "Ledger Posting"
                    try
                    {
                        LedgerPost(pd);
                    }
                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        ModelState.AddModelError("", message);
                        return RedirectToAction("Detail", new { id = Id, transactionType = "submit" });
                    }
                    #endregion

                    try
                    {
                        SaleInvoiceDocEvents.onHeaderSubmitEvent(this, new SaleEventArgs(Id), ref db);
                    }
                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        TempData["CSEXC"] += message;
                        EventException = true;
                    }


                    try
                    {
                        _unitOfWork.Save();
                    }

                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        TempData["CSEXC"] += message;
                        return RedirectToAction("Index", new { id = pd.DocTypeId });
                    }


                    try
                    {
                        SaleInvoiceDocEvents.afterHeaderSubmitEvent(this, new SaleEventArgs(Id), ref db);
                    }
                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        TempData["CSEXC"] += message;
                    }


                    LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                    {
                        DocTypeId = pd.DocTypeId,
                        DocId = pd.SaleInvoiceHeaderId,
                        ActivityType = ActivityType,
                        UserRemark = UserRemark,
                        DocNo = pd.DocNo,
                        DocDate = pd.DocDate,
                        DocStatus = pd.Status,
                    }));

                    return RedirectToAction("Index", new { id = pd.DocTypeId, IndexType = IndexType }).Success("Record submitted successfully.");
                }
                else
                    return RedirectToAction("Index", new { id = pd.DocTypeId, IndexType = IndexType }).Warning("Record can be submitted by user " + pd.ModifiedBy + " only.");
            }

            return View();
        }


        public ActionResult GenerateGatePass(string Ids, int DocTypeId)
        {

            if (!string.IsNullOrEmpty(Ids))
            {
                int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
                int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
                int PK = 0;
                var Settings = new SaleInvoiceSettingService(_unitOfWork).GetSaleInvoiceSettingForDocument(DocTypeId, DivisionId, SiteId);
                var GatePassDocTypeID = new DocumentTypeService(_unitOfWork).FindByName(TransactionDocCategoryConstants.GatePass).DocumentTypeId;
                string SaleinvoiceIds = "";
                try
                {
                    foreach (var item in Ids.Split(',').Select(Int32.Parse))
                    {
                        TimePlanValidation = true;

                        SaleInvoiceHeader pd = new SaleInvoiceHeaderService(_unitOfWork).FindDirectSaleInvoice(item);
                        SaleDispatchHeader Dh = _SaleDispatchHeaderService.Find(pd.SaleDispatchHeaderId.Value);
                        PackingHeader Ph = _PackingHeaderService.Find(Dh.PackingHeaderId.Value);


                        if (!Dh.GatePassHeaderId.HasValue)
                        {
                            #region DocTypeTimeLineValidation
                            try
                            {

                                TimePlanValidation = DocumentValidation.ValidateDocument(Mapper.Map<DocumentUniqueId>(Dh), DocumentTimePlanTypeConstants.GatePassCreate, User.Identity.Name, out ExceptionMsg, out Continue);

                            }
                            catch (Exception ex)
                            {
                                string message = _exception.HandleException(ex);
                                TempData["CSEXC"] += message;
                                TimePlanValidation = false;
                            }
                            #endregion

                            if ((TimePlanValidation || Continue))
                            {
                                if (!String.IsNullOrEmpty(Settings.SqlProcGatePass) && Dh.Status == (int)StatusConstants.Submitted && !Dh.GatePassHeaderId.HasValue)
                                {

                                    SqlParameter SqlParameterUserId = new SqlParameter("@Id", pd.SaleDispatchHeaderId.Value);
                                    IEnumerable<GatePassGeneratedViewModel> GatePasses = db.Database.SqlQuery<GatePassGeneratedViewModel>(Settings.SqlProcGatePass + " @Id", SqlParameterUserId).ToList();

                                    if (Dh.GatePassHeaderId == null)
                                    {
                                        SqlParameter DocDate = new SqlParameter("@DocDate", DateTime.Now.Date);
                                        DocDate.SqlDbType = SqlDbType.DateTime;
                                        SqlParameter Godown = new SqlParameter("@GodownId", Ph.GodownId);
                                        SqlParameter DocType = new SqlParameter("@DocTypeId", GatePassDocTypeID);
                                        GatePassHeader GPHeader = new GatePassHeader();
                                        GPHeader.CreatedBy = User.Identity.Name;
                                        GPHeader.CreatedDate = DateTime.Now;
                                        GPHeader.DivisionId = pd.DivisionId;
                                        GPHeader.DocDate = DateTime.Now.Date;
                                        GPHeader.DocNo = db.Database.SqlQuery<string>("Web.GetNewDocNoGatePass @DocTypeId, @DocDate, @GodownId ", DocType, DocDate, Godown).FirstOrDefault();
                                        GPHeader.DocTypeId = GatePassDocTypeID;
                                        GPHeader.ModifiedBy = User.Identity.Name;
                                        GPHeader.ModifiedDate = DateTime.Now;
                                        GPHeader.Remark = pd.Remark;
                                        GPHeader.PersonId = pd.SaleToBuyerId;
                                        GPHeader.SiteId = pd.SiteId;
                                        GPHeader.GodownId = Ph.GodownId;
                                        GPHeader.GatePassHeaderId = PK++;
                                        GPHeader.ReferenceDocTypeId = pd.DocTypeId;
                                        GPHeader.ReferenceDocId = pd.SaleInvoiceHeaderId;
                                        GPHeader.ReferenceDocNo = pd.DocNo;
                                        GPHeader.ObjectState = Model.ObjectState.Added;
                                        db.GatePassHeader.Add(GPHeader);
                                        //new GatePassHeaderService(_unitOfWork).Create(GPHeader);                                   



                                        foreach (GatePassGeneratedViewModel GatepassLine in GatePasses)
                                        {
                                            GatePassLine Gline = new GatePassLine();
                                            Gline.CreatedBy = User.Identity.Name;
                                            Gline.CreatedDate = DateTime.Now;
                                            Gline.GatePassHeaderId = GPHeader.GatePassHeaderId;
                                            Gline.ModifiedBy = User.Identity.Name;
                                            Gline.ModifiedDate = DateTime.Now;
                                            Gline.Product = GatepassLine.ProductName;
                                            Gline.Qty = GatepassLine.Qty;
                                            Gline.Specification = GatepassLine.Specification;
                                            Gline.UnitId = GatepassLine.UnitId;
                                            // new GatePassLineService(_unitOfWork).Create(Gline);
                                            Gline.ObjectState = Model.ObjectState.Added;
                                            db.GatePassLine.Add(Gline);
                                        }

                                        Dh.GatePassHeaderId = GPHeader.GatePassHeaderId;
                                        Dh.ObjectState = Model.ObjectState.Modified;
                                        db.SaleDispatchHeader.Add(Dh);
                                        SaleinvoiceIds += pd.SaleInvoiceHeaderId + ", ";
                                    }

                                    db.SaveChanges();
                                }
                            }
                            else
                                TempData["CSEXC"] += ExceptionMsg;
                        }
                    }
                    //_unitOfWork.Save();
                }

                catch (Exception ex)
                {
                    string message = _exception.HandleException(ex);
                    return Json(new { success = "Error", data = message }, JsonRequestBehavior.AllowGet);
                }

                LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                {
                    DocTypeId = GatePassDocTypeID,
                    ActivityType = (int)ActivityTypeContants.Added,
                    Narration = "GatePass created for Cloth Sate Invoice " + SaleinvoiceIds,
                }));

                if (string.IsNullOrEmpty((string)TempData["CSEXC"]))
                    return Json(new { success = "Success" }, JsonRequestBehavior.AllowGet).Success("Gate passes generated successfully");
                else
                    return Json(new { success = "Success" }, JsonRequestBehavior.AllowGet);

            }
            return Json(new { success = "Error", data = "No Records Selected." }, JsonRequestBehavior.AllowGet);

        }


        public ActionResult Review(int id, string IndexType, string TransactionType)
        {
            return RedirectToAction("Detail", new { id = id, IndexType = IndexType, transactionType = string.IsNullOrEmpty(TransactionType) ? "review" : TransactionType });
        }


        [HttpPost, ActionName("Detail")]
        [MultipleButton(Name = "Command", Argument = "Review")]
        public ActionResult Reviewed(int Id, string IndexType, string UserRemark, string IsContinue)
        {
            SaleInvoiceHeader pd = new SaleInvoiceHeaderService(_unitOfWork).FindDirectSaleInvoice(Id);
            SaleDispatchHeader Dh = _SaleDispatchHeaderService.Find(pd.SaleDispatchHeaderId.Value);
            PackingHeader Ph = _PackingHeaderService.Find(Dh.PackingHeaderId.Value);

            if (ModelState.IsValid)
            {
                pd.ReviewCount = (pd.ReviewCount ?? 0) + 1;
                pd.ReviewBy += User.Identity.Name + ", ";

                Dh.ReviewCount = (Dh.ReviewCount ?? 0) + 1;
                Dh.ReviewBy += User.Identity.Name + ", ";

                Ph.ReviewCount = (Ph.ReviewCount ?? 0) + 1;
                Ph.ReviewBy += User.Identity.Name + ", ";

                _SaleInvoiceHeaderService.Update(pd);
                _SaleDispatchHeaderService.Update(Dh);
                _PackingHeaderService.Update(Ph);

                _unitOfWork.Save();

                LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                {
                    DocTypeId = pd.DocTypeId,
                    DocId = pd.SaleInvoiceHeaderId,
                    ActivityType = (int)ActivityTypeContants.Reviewed,
                    UserRemark = UserRemark,
                    DocNo = pd.DocNo,
                    DocDate = pd.DocDate,
                    DocStatus = pd.Status,
                }));

                return RedirectToAction("Index", new { id = pd.DocTypeId, IndexType = IndexType }).Success("Record reviewed successfully.");
            }

            return RedirectToAction("Index", new { id = pd.DocTypeId, IndexType = IndexType }).Warning("Error in reviewing.");
        }

        public void LedgerPost(SaleInvoiceHeader pd)
        {
            LedgerHeaderViewModel LedgerHeaderViewModel = new LedgerHeaderViewModel();

            SaleInvoiceSetting Settings = new SaleInvoiceSettingService(_unitOfWork).GetSaleInvoiceSettingForDocument(pd.DocTypeId, pd.DivisionId, pd.SiteId);


            LedgerHeaderViewModel.LedgerHeaderId = pd.LedgerHeaderId ?? 0;
            LedgerHeaderViewModel.DocHeaderId = pd.SaleInvoiceHeaderId;
            LedgerHeaderViewModel.DocTypeId = pd.DocTypeId;
            LedgerHeaderViewModel.ProcessId = Settings.ProcessId;
            LedgerHeaderViewModel.DocDate = pd.DocDate;
            LedgerHeaderViewModel.DocNo = pd.DocNo;
            LedgerHeaderViewModel.DivisionId = pd.DivisionId;
            LedgerHeaderViewModel.SiteId = pd.SiteId;
            LedgerHeaderViewModel.Narration = _SaleInvoiceHeaderService.GetNarration(pd.SaleInvoiceHeaderId);
            LedgerHeaderViewModel.Remark = pd.Remark;
            LedgerHeaderViewModel.ExchangeRate = pd.ExchangeRate;
            LedgerHeaderViewModel.CreatedBy = pd.CreatedBy;
            LedgerHeaderViewModel.CreatedDate = DateTime.Now.Date;
            LedgerHeaderViewModel.ModifiedBy = pd.ModifiedBy;
            LedgerHeaderViewModel.ModifiedDate = DateTime.Now.Date;

            IEnumerable<SaleInvoiceHeaderCharge> SaleInvoiceHeaderCharges = from H in db.SaleInvoiceHeaderCharge where H.HeaderTableId == pd.SaleInvoiceHeaderId select H;
            IEnumerable<SaleInvoiceLineCharge> SaleInvoiceLineCharges = from L in db.SaleInvoiceLineCharge where L.HeaderTableId == pd.SaleInvoiceHeaderId select L;

            new CalculationService(_unitOfWork).LedgerPosting(ref LedgerHeaderViewModel, SaleInvoiceHeaderCharges, SaleInvoiceLineCharges);

            if (pd.LedgerHeaderId == null)
            {
                pd.LedgerHeaderId = LedgerHeaderViewModel.LedgerHeaderId;
                _SaleInvoiceHeaderService.Update(pd);
            }
        }


        public int PendingToSubmitCount(int id)
        {
            return (_SaleInvoiceHeaderService.GetSaleInvoiceHeaderListPendingToSubmit(id, User.Identity.Name)).Count();
        }

        public int PendingToReviewCount(int id)
        {
            return (_SaleInvoiceHeaderService.GetSaleInvoiceHeaderListPendingToReview(id, User.Identity.Name)).Count();
        }



        public JsonResult GetPersonLedgerBalance(int PersonId)
        {
            Decimal Balance = 0;
            SqlParameter SqlParameterPersonId = new SqlParameter("@PersonId", PersonId);

            PersonLedgerBalance PersonLedgerBalance = db.Database.SqlQuery<PersonLedgerBalance>("" + System.Configuration.ConfigurationManager.AppSettings["DataBaseSchema"] + ".GetPersonLedgerBalance @PersonId", SqlParameterPersonId).FirstOrDefault();
            if (PersonLedgerBalance != null)
            {
                Balance = PersonLedgerBalance.Balance;
            }

            return Json(Balance);
        }

        public JsonResult GetPersonDetail(int PersonId)
        {
            var PersonDetail = (from B in db.BusinessEntity
                                where B.PersonID == PersonId
                                select new
                                {
                                    CreditDays = B.CreaditDays ?? 0,
                                    CreditLimit = B.CreaditLimit ?? 0,
                                    SalesTaxGroupPartyId = B.SalesTaxGroupPartyId,
                                    SalesTaxGroupPartyName = B.SalesTaxGroupParty.ChargeGroupPersonName
                                }).FirstOrDefault();

            return Json(PersonDetail);
        }

        public ActionResult NextPage(int DocId, int DocTypeId)//CurrentHeaderId
        {
            var nextId = new NextPrevIdService(_unitOfWork).GetNextPrevId(DocId, DocTypeId, User.Identity.Name, "", "Web.SaleInvoiceHeaders", "SaleInvoiceHeaderId", PrevNextConstants.Next);
            return Edit(nextId, "");
        }
        [HttpGet]
        public ActionResult PrevPage(int DocId, int DocTypeId)//CurrentHeaderId
        {
            var PrevId = new NextPrevIdService(_unitOfWork).GetNextPrevId(DocId, DocTypeId, User.Identity.Name, "", "Web.SaleInvoiceHeaders", "SaleInvoiceHeaderId", PrevNextConstants.Prev);
            return Edit(PrevId, "");
        }

        [HttpGet]
        public ActionResult History()
        {
            //To Be Implemented
            return View("~/Views/Shared/UnderImplementation.cshtml");
        }

        [HttpGet]
        public ActionResult Email()
        {
            //To Be Implemented
            return View("~/Views/Shared/UnderImplementation.cshtml");
        }


        public ActionResult GeneratePrints(string Ids, int DocTypeId)
        {

            if (!string.IsNullOrEmpty(Ids))
            {
                int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
                int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];

                var Settings = new SaleInvoiceSettingService(_unitOfWork).GetSaleInvoiceSettingForDocument(DocTypeId, DivisionId, SiteId);

                if (new RolePermissionService(_unitOfWork).IsActionAllowed(UserRoles, DocTypeId, Settings.ProcessId, this.ControllerContext.RouteData.Values["controller"].ToString(), "GeneratePrints") == false)
                {
                    return View("~/Views/Shared/PermissionDenied.cshtml").Warning("You don't have permission to do this task.");
                }

                try
                {

                    List<byte[]> PdfStream = new List<byte[]>();
                    foreach (var item in Ids.Split(',').Select(Int32.Parse))
                    {

                        DirectReportPrint drp = new DirectReportPrint();

                        var pd = db.SaleInvoiceHeader.Find(item);

                        LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                        {
                            DocTypeId = pd.DocTypeId,
                            DocId = pd.SaleInvoiceHeaderId,
                            ActivityType = (int)ActivityTypeContants.Print,
                            DocNo = pd.DocNo,
                            DocDate = pd.DocDate,
                            DocStatus = pd.Status,
                        }));

                        byte[] Pdf;

                        if (pd.Status == (int)StatusConstants.Drafted || pd.Status == (int)StatusConstants.Modified || pd.Status == (int)StatusConstants.Import)
                        {
                            if (Settings.SqlProcDocumentPrint == null || Settings.SqlProcDocumentPrint == "")
                            {
                                SaleInvoiceHeaderRDL cr = new SaleInvoiceHeaderRDL();
                                //drp.CreateRDLFile("DocPrint_SaleInvoice", cr.DocPrint_SaleInvoice());
                                List<ListofQuery> QueryList = new List<ListofQuery>();
                                QueryList = DocumentPrintData(item);
                                Pdf = drp.DocumentPrint_New(QueryList, User.Identity.Name);
                            }
                            else
                                Pdf = drp.DirectDocumentPrint(Settings.SqlProcDocumentPrint, User.Identity.Name, item);

                            PdfStream.Add(Pdf);
                        }
                        else if (pd.Status == (int)StatusConstants.Submitted || pd.Status == (int)StatusConstants.ModificationSubmitted)
                        {
                            if (Settings.SqlProcDocumentPrint_AfterSubmit == null || Settings.SqlProcDocumentPrint_AfterSubmit == "")
                            {
                                SaleInvoiceHeaderRDL cr = new SaleInvoiceHeaderRDL();
                                drp.CreateRDLFile("DocPrint_SaleInvoice", cr.DocPrint_SaleInvoice());
                                List<ListofQuery> QueryList = new List<ListofQuery>();
                                QueryList = DocumentPrintData(item);
                                Pdf = drp.DocumentPrint_New(QueryList, User.Identity.Name);
                            }
                            else
                                Pdf = drp.DirectDocumentPrint(Settings.SqlProcDocumentPrint_AfterSubmit, User.Identity.Name, item);

                            PdfStream.Add(Pdf);
                        }
                        else if (pd.Status == (int)StatusConstants.Approved)
                        {
                            if (Settings.SqlProcDocumentPrint_AfterApprove == null || Settings.SqlProcDocumentPrint_AfterApprove == "")
                            {
                                SaleInvoiceHeaderRDL cr = new SaleInvoiceHeaderRDL();
                                drp.CreateRDLFile("DocPrint_SaleInvoice", cr.DocPrint_SaleInvoice());
                                List<ListofQuery> QueryList = new List<ListofQuery>();
                                QueryList = DocumentPrintData(item);
                                Pdf = drp.DocumentPrint_New(QueryList, User.Identity.Name);
                            }
                            else
                                Pdf = drp.DirectDocumentPrint(Settings.SqlProcDocumentPrint_AfterApprove, User.Identity.Name, item);
                            PdfStream.Add(Pdf);
                        }

                    }

                    PdfMerger pm = new PdfMerger();

                    byte[] Merge = pm.MergeFiles(PdfStream);

                    if (Merge != null)
                        return File(Merge, "application/pdf");

                }

                catch (Exception ex)
                {
                    string message = _exception.HandleException(ex);
                    return Json(new { success = "Error", data = message }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = "Success" }, JsonRequestBehavior.AllowGet);

            }
            return Json(new { success = "Error", data = "No Records Selected." }, JsonRequestBehavior.AllowGet);

        }


        private List<ListofQuery> DocumentPrintData(int item)
        {
            SaleInvoiceHeader SIH = new SaleInvoiceHeaderService(_unitOfWork).FindDirectSaleInvoice(item);
            DocumentType DT = new DocumentTypeService(_unitOfWork).Find(SIH.DocTypeId);

            List<ListofQuery> DocumentPrintData = new List<ListofQuery>();
            String QueryMain;

            if (DT.DocumentTypeName == "Sale Invoice Local")
            {
                QueryMain = @"IF OBJECT_ID ('dbo.TempRug_SaleInvoicePrint_Local') IS NOT NULL
    DROP TABLE dbo.TempRug_SaleInvoicePrint_Local

DECLARE @TotalAmount DECIMAL
SET @TotalAmount = (SELECT Max(Amount) FROM Web.SaleInvoiceHeaderCharges WHERE HeaderTableId = " + item + @" AND ChargeId = 34 ) 
 
DECLARE @ReturnAmount DECIMAL
DECLARE @DebitAmount DECIMAL
DECLARE @CreaditAmount DECIMAL

SELECT
@ReturnAmount = sum(CASE WHEN CT.ChargeTypeName IN('Amount', 'CGST', 'SGST', 'IGST') AND JIRH.Nature = 'Return' THEN isnull(H.Amount, 0) ELSE 0 END),
@DebitAmount = sum(CASE WHEN JIRH.Nature = 'Debit Note' AND C.ChargeName = 'Net Amount' THEN isnull(H.Amount, 0) ELSE 0 END),
@CreaditAmount = sum(CASE WHEN JIRH.Nature = 'Credit Note' AND C.ChargeName = 'Net Amount' THEN isnull(H.Amount, 0) ELSE 0 END)
FROM Web.SaleInvoiceReturnLineCharges H
LEFT JOIN Web.ChargeTypes CT WITH(Nolock) ON CT.ChargeTypeId = H.ChargeTypeId
LEFT JOIN web.Charges C WITH(Nolock) ON C.ChargeId = H.ChargeId
LEFT JOIN Web.SaleInvoiceReturnLines JIRL WITH(Nolock) ON JIRL.SaleInvoiceReturnLineId = H.LineTableId
LEFT JOIN Web.SaleInvoiceLines JIL WITH(Nolock) ON JIL.SaleInvoiceLineId = JIRL.SaleInvoiceLineId
LEFT JOIN Web.SaleInvoiceReturnHeaders JIRH WITH(Nolock) ON JIRH.SaleInvoiceReturnHeaderId = JIRL.SaleInvoiceReturnHeaderId
WHERE JIL.SaleInvoiceHeaderId = " + item + @"

SET @TotalAmount = isnull(@TotalAmount, 0) - isnull(@ReturnAmount, 0) - isnull(@DebitAmount, 0) + isnull(@CreaditAmount, 0)


DECLARE @UnitDealCnt INT
SELECT
@UnitDealCnt = sum(CASE WHEN JOL.DealunitId != JOL.DealunitId THEN 1 ELSE 0 END)
FROM Web.SaleInvoiceLines L WITH(Nolock)
LEFT JOIN Web.SaleOrderLines JOL WITH(Nolock) ON JOL.SaleOrderLineId = L.SaleOrderLineId
WHERE L.SaleInvoiceHeaderId = " + item + @"


DECLARE @DocDate DATETIME
DECLARE @Site INT
DECLARE @Division INT
SELECT  @DocDate = DocDate,@Site = SiteId,@Division = DivisionId FROM Web.SaleInvoiceHeaders WHERE SaleInvoiceHeaderId = " + item + @"



SELECT H.SaleInvoiceHeaderId,H.DocTypeId,H.DocNo,DocIdCaption + ' No' AS DocIdCaption, H.SiteId,H.DivisionId,H.DocDate,DTS.DocIdCaption + ' Date' AS DocIdCaptionDate,
DTS.PartyCaption + ' Doc No' AS PartyDocCaption, DTS.PartyCaption + ' Doc Date' AS PartyDocDateCaption, H.CreditDays,H.Remark,DT.DocumentTypeShortName,	
H.ModifiedBy + ' ' + Replace(replace(convert(NVARCHAR, H.ModifiedDate, 106), ' ', '/'), '/20', '/') + substring(convert(NVARCHAR, H.ModifiedDate), 13, 7) AS ModifiedBy,
H.ModifiedDate,(CASE WHEN Isnull(H.Status, 0)= 0 OR Isnull(H.Status, 0)= 8 THEN 0 ELSE 1 END)  AS Status, CUR.Name AS CurrencyName,(CASE WHEN SPR.[Party GST NO]
        IS NULL THEN 'Yes' ELSE 'No' END ) AS ReverseCharge,
VDC.CompanyName,VDC.CompanyBankName,  Replace(VDC.CompanyBankBranch,'SWIFT.CODE : UBININBBBHD ','') as CompanyBankBranch, 'IFSC' CompanyBankIFSC, P.Name AS PartyName, DTS.PartyCaption AS  PartyCaption, P.Suffix AS PartySuffix,	
isnull(PA.Address,'')+' '+isnull(C.CityName,'')+','+isnull(PA.ZipCode,'')+(CASE WHEN isnull(CS.StateName,'') <> isnull(S.StateName,'') AND SPR.[Party GST NO] IS NOT NULL THEN ',State : '+isnull(S.StateName,'')+(CASE WHEN S.StateCode IS NULL THEN '' ELSE ', Code : '+S.StateCode END)    ELSE '' END ) AS PartyAddress,
isnull(S.StateName, '') AS PartyStateName, isnull(S.StateCode, '') AS PartyStateCode, P.Mobile AS PartyMobileNo, SPR.*,
--Plan Detail
POH.DocNo AS PlanNo,PB.BuyerSpecification3,
--Caption Fields
DTS.ContraDocTypeCaption,DTS.SignatoryMiddleCaption,DTS.SignatoryRightCaption,
--Line Table
PD.ProductName,DTS.ProductCaption,U.UnitName,U.DecimalPlaces,DU.UnitName AS DealUnitName,DTS.DealQtyCaption,DU.DecimalPlaces AS DealDecimalPlaces,
isnull(L.Qty,0) AS Qty, isnull(L.Rate, 0) AS Rate, isnull(L.Amount, 0) AS Amount, isnull(L.DealQty, 0) AS DealQty,
D1.Dimension1Name,DTS.Dimension1Caption,D2.Dimension2Name,DTS.Dimension2Caption,D3.Dimension3Name,DTS.Dimension3Caption,D4.Dimension4Name,DTS.Dimension4Caption,
DTS.SpecificationCaption,DTS.SignatoryleftCaption,L.Remark AS LineRemark,PQ.ProductQualityName,L.RateRemark,
NULL AS DiscountPer,NULL AS DiscountAmt,
STC.Code AS SalesTaxProductCodes,
(CASE WHEN DTS.PrintProductGroup >0 THEN isnull(PG.ProductGroupName,'') ELSE '' END)+(CASE WHEN DTS.PrintProductdescription >0 THEN isnull(','+PD.Productdescription,'') ELSE '' END) AS ProductGroupName,
DTS.ProductGroupCaption,isnull(CGPD.PrintingDescription, CGPD.ChargeGroupProductName) AS ChargeGroupProductName,
--Receive Lines
DTS.ProductUidCaption,PU.ProductUidName,0  AS LossQty, PL.Qty AS RecQty, JRL.LotNo AS LotNo, (CASE WHEN DTS.PrintSpecification >0 THEN PL.Specification ELSE '' END)  AS Specification,
 --Formula Fields
isnull(@TotalAmount,0) AS NetAmount,
isnull(@ReturnAmount,0) AS ReturnAmount,
isnull(@DebitAmount, 0) AS DebitAmount,
isnull(@CreaditAmount, 0) AS CreaditAmount,
--SalesTaxGroupPersonId
CGP.ChargeGroupPersonName,
--Other Fields
@UnitDealCnt  AS DealUnitCnt, isnull(SB.Name, SIH.ShipToParty) AS ShipToParty, SDH.ShipToPartyAddress, SC.CityName AS ShipPartyCity, isnull(SS.StateName, SIH.ShipToPartyState) AS ShipPartyState,
(CASE WHEN Isnull(H.Status, 0) = 0 OR Isnull(H.Status, 0) = 8 THEN 'Provisional ' + isnull(DT.PrintTitle, DT.DocumentTypeName) ELSE isnull(DT.PrintTitle, DT.DocumentTypeName) END) AS ReportTitle,
'DocPrint_SaleInvoice_Bhadohi.rdl' AS ReportName, SalesTaxGroupProductCaption, SDS.SalesTaxProductCodeCaption,
SIH.ShipToPartyMobile,SIH.ShipToPartyGST,SIH.ShipToPartyStateCode, SIH.TransportMode,SIH.VehicleNo,SIH.GrossWeight,SIH.NetWeight
INTO TempRug_SaleInvoicePrint_Local
FROM Web.SaleInvoiceHeaders H WITH(Nolock)
LEFT JOIN web.DocumentTypes DT WITH(Nolock) ON DT.DocumentTypeId=H.DocTypeId
LEFT JOIN Web._DocumentTypeSettings DTS WITH (Nolock) ON DTS.DocumentTypeId=DT.DocumentTypeId
LEFT JOIN Web.SaleInvoiceSettings JIS WITH (Nolock) ON JIS.DocTypeId=DT.DocumentTypeId AND JIS.SiteId = H.siteid AND H.DivisionId= JIS.DivisionId
LEFT JOIN web.ViewDivisionCompany VDC WITH (Nolock) ON VDC.DivisionId=H.DivisionId
LEFT JOIN Web.Sites SI WITH (Nolock) ON SI.SiteId=H.SiteId
LEFT JOIN Web.Divisions DIV WITH (Nolock) ON DIV.DivisionId=H.DivisionId
LEFT JOIN Web.Companies Com ON Com.CompanyId = DIV.CompanyId
LEFT JOIN Web.Cities CC WITH (Nolock) ON CC.CityId=Com.CityId
LEFT JOIN Web.States CS WITH (Nolock) ON CS.StateId=CC.StateId
--LEFT JOIN Web.Processes PS WITH (Nolock) ON PS.ProcessId=H.ProcessId
--LEFT JOIN Web.SalesTaxProductCodes PSSTC WITH (Nolock) ON PSSTC.SalesTaxProductCodeId=PS.SalesTaxProductCodeId
LEFT JOIN Web.People P WITH (Nolock) ON P.PersonID=H.SaleToBuyerId
LEFT JOIN (SELECT TOP 1 * FROM web.SiteDivisionSettings WHERE @DocDate BETWEEN StartDate AND IsNull(EndDate, getdate()) AND SiteId = @Site AND DivisionId = @Division ORDER BY StartDate) SDS ON H.DivisionId = SDS.DivisionId AND H.SiteId = SDS.SiteId
LEFT JOIN(SELECT* FROM Web.PersonAddresses WITH (nolock) WHERE AddressType IS NULL) PA ON PA.PersonId = P.PersonID
LEFT JOIN Web.Cities C WITH (nolock) ON C.CityId = PA.CityId
LEFT JOIN Web.States S WITH (Nolock) ON S.StateId=C.StateId
LEFT JOIN web.SaleDispatchHeaders SDH ON SDH.SaleDispatchHeaderId = H.SaleDispatchHeaderId
LEFT JOIN web.People SB ON SB.PersonID = SDH.SaleToBuyerId
LEFT JOIN(SELECT* FROM Web.PersonAddresses WITH (nolock) WHERE AddressType IS NULL) SPA ON SPA.PersonId = SB.PersonID
LEFT JOIN Web.Cities SC WITH (nolock) ON SC.CityId = SPA.CityId
LEFT JOIN Web.States SS WITH (Nolock) ON SS.StateId= SC.StateId
LEFT JOIN web.ChargeGroupPersons CGP WITH (Nolock) ON CGP.ChargeGroupPersonId=H.SalesTaxGroupPersonId
LEFT JOIN Web.Currencies CUR WITH (Nolock) ON CUR.Id=H.CurrencyId
LEFT JOIN Web.SaleInvoiceLines L WITH (Nolock) ON L.SaleInvoiceHeaderId=H.SaleInvoiceHeaderId
LEFT JOIN Web.SaleDispatchLines JRL WITH (Nolock) ON JRL.SaleDispatchLineId =L.SaleDispatchLineId
LEFT JOIN web.PackingLines PL ON PL.PackingLineId = JRL.PackingLineId
LEFT JOIN web.ProductUids PU WITH (Nolock) ON PU.ProductUidId=PL.ProductUidId
--	LEFT JOIN Web.JobReceiveHeaders JRH WITH (Nolock) ON JRH.JobReceiveHeaderId=JRL.JobReceiveHeaderId
LEFT JOIN Web.SaleOrderLines POl WITH (Nolock) ON POl.SaleOrderLineId=PL.SaleOrderLineId
LEFT JOIN Web.SaleOrderHeaders POH WITH (Nolock) ON POH.SaleOrderHeaderId=POL.SaleOrderHeaderId
LEFT JOIN web.Products PD WITH (Nolock) ON PD.ProductId=isnull(PL.ProductId, POL.ProductId)
LEFT JOIN Web.FinishedProduct Fp ON PD.ProductId = Fp.ProductId
LEFT JOIN Web.Colours PCol ON Fp.ColourId = PCol.ColourId
LEFT JOIN Web.ProductCategories Pc ON Fp.ProductCategoryId = Pc.ProductCategoryId
LEFT JOIN Web.ProductQualities PQ ON Fp.ProductQualityId = PQ.ProductQualityId
LEFT JOIN web.ProductBuyers PB WITH (Nolock) ON PB.ProductId =PD.ProductId AND PB.BuyerId = SDH.SaleToBuyerId
LEFT JOIN web.ProductGroups PG WITH(Nolock) ON PG.ProductGroupId=PD.ProductGroupid
LEFT JOIN Web.SalesTaxProductCodes STC WITH (Nolock) ON STC.SalesTaxProductCodeId= IsNull(PD.SalesTaxProductCodeId, Pg.DefaultSalesTaxProductCodeId)
LEFT JOIN Web.Dimension1 D1 WITH(Nolock) ON D1.Dimension1Id=PL.Dimension1Id
LEFT JOIN web.Dimension2 D2 WITH (Nolock) ON D2.Dimension2Id=PL.Dimension2Id
LEFT JOIN web.Dimension3 D3 WITH (Nolock) ON D3.Dimension3Id=PL.Dimension3Id
LEFT JOIN Web.Dimension4 D4 WITH (nolock) ON D4.Dimension4Id=PL.Dimension4Id
LEFT JOIN web.Units U WITH (Nolock) ON U.UnitId=PD.UnitId
LEFT JOIN web.Units DU WITH (Nolock) ON DU.UnitId=PL.DealUnitId
LEFT JOIN Web.Std_PersonRegistrations SPR WITH (Nolock) ON SPR.CustomerId=H.BillToBuyerId
LEFT JOIN web.ChargeGroupProducts CGPD WITH (Nolock) ON L.SalesTaxGroupProductId = CGPD.ChargeGroupProductId
LEFT JOIN
(
SELECT SIH.HeaderTableId,
Max(CASE WHEN DTA.Name = 'Ship To Party' THEN SIH.Value ELSE NULL END) AS ShipToParty,
Max(CASE WHEN DTA.Name = 'Ship To Party Mobile No' THEN SIH.Value  ELSE NULL END) AS ShipToPartyMobile,
Max(CASE WHEN DTA.Name = 'Ship To Party GSTIN' THEN SIH.Value  ELSE NULL END) AS ShipToPartyGST,
Max(CASE WHEN DTA.Name = 'Ship To Party State Name' THEN SIH.Value  ELSE NULL END) AS ShipToPartyState,
Max(CASE WHEN DTA.Name = 'Ship To Party State Code' THEN SIH.Value  ELSE NULL END) AS ShipToPartyStateCode,
Max(CASE WHEN DTA.Name = 'Transport Mode' THEN SIH.Value  ELSE NULL END) AS TransportMode,
Max(CASE WHEN DTA.Name = 'Vehicle No' THEN SIH.Value  ELSE NULL END) AS VehicleNo,
Max(CASE WHEN DTA.Name = 'Gross Weight' THEN SIH.Value  ELSE NULL END) AS GrossWeight,
Max(CASE WHEN DTA.Name = 'Net Weight' THEN SIH.Value  ELSE NULL END) AS NetWeight
FROM web.SaleInvoiceHeaderAttributes SIH WITH (Nolock)
LEFT JOIN web.DocumentTypeHeaderAttributes DTA ON DTA.DocumentTypeHeaderAttributeId = SIH.DocumentTypeHeaderAttributeId
WHERE SIH.HeaderTableId = " + item + @"
GROUP BY SIH.HeaderTableId
) SIH ON SIH.HeaderTableId =H.SaleInvoiceHeaderId
WHERE H.SaleInvoiceHeaderId= " + item + @"


SELECT H.BuyerSpecification3 AS ProductQualityName, H.RateRemark, H.Rate, Max(H.DocTypeId) as DocTypeId,
Max(H.SaleInvoiceHeaderId) AS SaleInvoiceHeaderId, Max(H.DocNo)AS DocNo, Max(H.DocIdCaption) AS DocIdCaption,
Max(H.SiteId) AS SiteId, Max(H.DivisionId) AS DivisionId, Max(H.DocDate) AS DocDate, Max(H.DocIdCaptionDate) AS DocIdCaptionDate,
Max(H.PartyDocCaption) AS PartyDocCaption, Max(H.PartyDocDateCaption) AS PartyDocDateCaption, Max(H.CreditDays) AS CreditDays,
Max(H.Remark) AS Remark, Max(H.DocumentTypeShortName) AS DocumentTypeShortName, Max(H.ModifiedBy) AS ModifiedBy, Max(H.ModifiedDate) AS ModifiedDate,
Max(H.Status) AS Status, Max(H.CurrencyName) AS CurrencyName, Max(H.ReverseCharge) ReverseCharge, Max(H.CompanyName) CompanyName, Max(H.CompanyBankName) CompanyBankName, Max(H.CompanyBankBranch) CompanyBankBranch, Max(H.CompanyBankIFSC) CompanyBankIFSC,
Max(H.PartyName) PartyName, Max(H.PartyCaption) PartyCaption, Max(H.PartySuffix) PartySuffix, Max(H.PartyAddress) PartyAddress, Max(H.PartyStateName) PartyStateName, Max(H.PartyStateCode) PartyStateCode, Max(H.PartyMobileNo) PartyMobileNo,
Max(H.CustomerId) CustomerId, NULL  AS[Party TIN NO], NULL  AS[Party PAN NO], Max(H.[Party AADHAR NO])[Party AADHAR NO], Max(H.[Party GST NO])[Party GST NO], Max(H.[Party CST NO])[Party CST NO],
Max(H.PlanNo) PlanNo, Max(H.ContraDocTypeCaption) ContraDocTypeCaption, Max(H.SignatoryMiddleCaption) SignatoryMiddleCaption, Max(H.SignatoryRightCaption) SignatoryRightCaption, Max(H.ProductName) ProductName, Max(H.ProductCaption) ProductCaption,
Max(H.UnitName) UnitName, Max(H.DecimalPlaces) DecimalPlaces, Max(H.DealUnitName) DealUnitName, Max(H.DealQtyCaption) DealQtyCaption, Max(H.DealDecimalPlaces) DealDecimalPlaces,
sum(H.Qty) Qty, sum(H.Amount) Amount, sum(H.DealQty) DealQty, Max(H.Dimension1Name) Dimension1Name, Max(H.Dimension1Caption) Dimension1Caption, Max(H.Dimension2Name) Dimension2Name, Max(H.Dimension2Caption) Dimension2Caption,
Max(H.Dimension3Name) Dimension3Name, Max(H.Dimension3Caption) Dimension3Caption, Max(H.Dimension4Name) Dimension4Name, Max(H.Dimension4Caption) Dimension4Caption, Max(H.SpecificationCaption) SpecificationCaption, Max(H.SignatoryleftCaption) SignatoryleftCaption, Max(H.LineRemark) LineRemark,
Max(H.DiscountPer) DiscountPer, sum(H.DiscountAmt) DiscountAmt, Max(H.SalesTaxProductCodes) SalesTaxProductCodes, Max(H.ProductGroupName) ProductGroupName, Max(H.ProductGroupCaption) ProductGroupCaption, Max(H.ChargeGroupProductName) ChargeGroupProductName,
Max(H.ProductUidCaption) ProductUidCaption, Max(H.ProductUidName) ProductUidName, sum(H.LossQty) LossQty, Sum(H.RecQty) RecQty, Max(H.LotNo) LotNo, Max(H.Specification) Specification, Max(H.NetAmount) NetAmount, Max(H.ReturnAmount) ReturnAmount, Max(H.DebitAmount) DebitAmount, Max(H.CreaditAmount) CreaditAmount, Max(H.ChargeGroupPersonName) ChargeGroupPersonName,
Max(ShipToPartyMobile) AS ShipToPartyMobile, Max(ShipToPartyGST) AS ShipToPartyGST, Max(ShipToPartyStateCode) AS ShipToPartyStateCode,
Max(TransportMode) AS TransportMode, Max(VehicleNo) AS VehicleNo, Max(GrossWeight) AS GrossWeight, Max(NetWeight) AS NetWeight,
Max(H.DealUnitCnt) DealUnitCnt, Max(H.ShipToParty) ShipToParty, Max(H.ShipToPartyAddress) ShipToPartyAddress, Max(H.ShipPartyCity) ShipPartyCity, Max(H.ShipPartyState) ShipPartyState, Max(H.ReportTitle) ReportTitle, Max(H.ReportName) ReportName, Max(H.SalesTaxGroupProductCaption) SalesTaxGroupProductCaption, Max(H.SalesTaxProductCodeCaption) SalesTaxProductCodeCaption
FROM TempRug_SaleInvoicePrint_Local H
GROUP BY H.BuyerSpecification3, H.RateRemark, H.Rate";
            }
            else
            {
                QueryMain = @"DECLARE @TotalAmount DECIMAL 
SET @TotalAmount = (SELECT Max(Amount) FROM Web.SaleInvoiceHeaderCharges WHERE HeaderTableId = " + item + @" AND ChargeId = 34 ) 
 
DECLARE @ReturnAmount DECIMAL
DECLARE @DebitAmount DECIMAL
DECLARE @CreaditAmount DECIMAL

SELECT
@ReturnAmount = sum(CASE WHEN CT.ChargeTypeName IN('Amount', 'CGST', 'SGST', 'IGST') AND JIRH.Nature = 'Return' THEN isnull(H.Amount, 0) ELSE 0 END),
@DebitAmount = sum(CASE WHEN JIRH.Nature = 'Debit Note' AND C.ChargeName = 'Net Amount' THEN isnull(H.Amount, 0) ELSE 0 END),
@CreaditAmount = sum(CASE WHEN JIRH.Nature = 'Credit Note' AND C.ChargeName = 'Net Amount' THEN isnull(H.Amount, 0) ELSE 0 END)
FROM Web.SaleInvoiceReturnLineCharges H
LEFT JOIN Web.ChargeTypes CT WITH(Nolock) ON CT.ChargeTypeId = H.ChargeTypeId
LEFT JOIN web.Charges C WITH(Nolock) ON C.ChargeId = H.ChargeId
LEFT JOIN Web.SaleInvoiceReturnLines JIRL WITH(Nolock) ON JIRL.SaleInvoiceReturnLineId = H.LineTableId
LEFT JOIN Web.SaleInvoiceLines JIL WITH(Nolock) ON JIL.SaleInvoiceLineId = JIRL.SaleInvoiceLineId
LEFT JOIN Web.SaleInvoiceReturnHeaders JIRH WITH(Nolock) ON JIRH.SaleInvoiceReturnHeaderId = JIRL.SaleInvoiceReturnHeaderId
WHERE JIL.SaleInvoiceHeaderId = " + item + @"

SET @TotalAmount = isnull(@TotalAmount, 0) - isnull(@ReturnAmount, 0) - isnull(@DebitAmount, 0) + isnull(@CreaditAmount, 0)


DECLARE @UnitDealCnt INT
SELECT
@UnitDealCnt = sum(CASE WHEN JOL.DealunitId != JOL.DealunitId THEN 1 ELSE 0 END)
FROM Web.SaleInvoiceLines L WITH(Nolock)
LEFT JOIN Web.SaleOrderLines JOL WITH(Nolock) ON JOL.SaleOrderLineId = L.SaleOrderLineId
WHERE L.SaleInvoiceHeaderId = " + item + @"


DECLARE @DocDate DATETIME
DECLARE @Site INT
DECLARE @Division INT
SELECT  @DocDate = DocDate,@Site = SiteId,@Division = DivisionId FROM Web.SaleInvoiceHeaders WHERE SaleInvoiceHeaderId = " + item + @"



SELECT H.SaleInvoiceHeaderId,H.DocTypeId,H.DocNo,DocIdCaption + ' No' AS DocIdCaption, H.SiteId,H.DivisionId,H.DocDate,DTS.DocIdCaption + ' Date' AS DocIdCaptionDate,
DTS.PartyCaption + ' Doc No' AS PartyDocCaption, DTS.PartyCaption + ' Doc Date' AS PartyDocDateCaption, H.CreditDays,H.Remark,DT.DocumentTypeShortName,	
H.ModifiedBy + ' ' + Replace(replace(convert(NVARCHAR, H.ModifiedDate, 106), ' ', '/'), '/20', '/') + substring(convert(NVARCHAR, H.ModifiedDate), 13, 7) AS ModifiedBy,
H.ModifiedDate,(CASE WHEN Isnull(H.Status, 0)= 0 OR Isnull(H.Status, 0)= 8 THEN 0 ELSE 1 END)  AS Status, CUR.Name AS CurrencyName,(CASE WHEN SPR.[Party GST NO]
        IS NULL THEN 'Yes' ELSE 'No' END ) AS ReverseCharge,
VDC.CompanyName,VDC.CompanyBankName, VDC.CompanyBankBranch, 'IFSC' CompanyBankIFSC,P.Name AS PartyName, DTS.PartyCaption AS  PartyCaption, P.Suffix AS PartySuffix,	
isnull(PA.Address,'')+' '+isnull(C.CityName,'')+','+isnull(PA.ZipCode,'')+(CASE WHEN isnull(CS.StateName,'') <> isnull(S.StateName,'') AND SPR.[Party GST NO] IS NOT NULL THEN ',State : '+isnull(S.StateName,'')+(CASE WHEN S.StateCode IS NULL THEN '' ELSE ', Code : '+S.StateCode END)    ELSE '' END ) AS PartyAddress,
isnull(S.StateName, '') AS PartyStateName, isnull(S.StateCode, '') AS PartyStateCode, P.Mobile AS PartyMobileNo, SPR.*,
--Plan Detail
POH.DocNo AS PlanNo,
--Caption Fields
DTS.ContraDocTypeCaption,DTS.SignatoryMiddleCaption,DTS.SignatoryRightCaption,
--Line Table
PD.ProductName,DTS.ProductCaption,U.UnitName,U.DecimalPlaces,DU.UnitName AS DealUnitName,DTS.DealQtyCaption,DU.DecimalPlaces AS DealDecimalPlaces,
isnull(L.Qty,0) AS Qty, isnull(L.Rate, 0) AS Rate, isnull(L.Amount, 0) AS Amount, isnull(L.DealQty, 0) AS DealQty,
D1.Dimension1Name,DTS.Dimension1Caption,D2.Dimension2Name,DTS.Dimension2Caption,D3.Dimension3Name,DTS.Dimension3Caption,D4.Dimension4Name,DTS.Dimension4Caption,
DTS.SpecificationCaption,DTS.SignatoryleftCaption,L.Remark AS LineRemark,
null AS DiscountPer,null AS DiscountAmt,
STC.Code AS SalesTaxProductCodes,
(CASE WHEN DTS.PrintProductGroup >0 THEN isnull(PG.ProductGroupName,'') ELSE '' END)+(CASE WHEN DTS.PrintProductdescription >0 THEN isnull(','+PD.Productdescription,'') ELSE '' END) AS ProductGroupName,
DTS.ProductGroupCaption,isnull(CGPD.PrintingDescription, CGPD.ChargeGroupProductName) AS ChargeGroupProductName,
--Receive Lines
DTS.ProductUidCaption,PU.ProductUidName,0  AS LossQty, PL.Qty AS RecQty, JRL.LotNo AS LotNo, (CASE WHEN DTS.PrintSpecification >0 THEN PL.Specification ELSE '' END)  AS Specification,
 --Formula Fields
isnull(@TotalAmount,0) AS NetAmount,
isnull(@ReturnAmount,0) AS ReturnAmount,
isnull(@DebitAmount, 0) AS DebitAmount,
isnull(@CreaditAmount, 0) AS CreaditAmount,
--SalesTaxGroupPersonId
CGP.ChargeGroupPersonName,
--Other Fields
@UnitDealCnt  AS DealUnitCnt, SB.Name AS ShipToParty, SDH.ShipToPartyAddress, SC.CityName AS ShipPartyCity, SS.StateName AS ShipPartyState,
(CASE WHEN Isnull(H.Status, 0) = 0 OR Isnull(H.Status, 0) = 8 THEN 'Provisional ' + isnull(DT.PrintTitle, DT.DocumentTypeName) ELSE isnull(DT.PrintTitle, DT.DocumentTypeName) END) AS ReportTitle,
'DocPrint_SaleInvoice.rdl' AS ReportName, SalesTaxGroupProductCaption, SDS.SalesTaxProductCodeCaption
FROM Web.SaleInvoiceHeaders H WITH(Nolock)
LEFT JOIN web.DocumentTypes DT WITH(Nolock) ON DT.DocumentTypeId=H.DocTypeId
LEFT JOIN Web._DocumentTypeSettings DTS WITH (Nolock) ON DTS.DocumentTypeId=DT.DocumentTypeId
LEFT JOIN Web.SaleInvoiceSettings JIS WITH (Nolock) ON JIS.DocTypeId=DT.DocumentTypeId AND JIS.SiteId = H.siteid AND H.DivisionId= JIS.DivisionId
LEFT JOIN web.ViewDivisionCompany VDC WITH (Nolock) ON VDC.DivisionId=H.DivisionId
LEFT JOIN Web.Sites SI WITH (Nolock) ON SI.SiteId=H.SiteId
LEFT JOIN Web.Divisions DIV WITH (Nolock) ON DIV.DivisionId=H.DivisionId
LEFT JOIN Web.Companies Com ON Com.CompanyId = DIV.CompanyId
LEFT JOIN Web.Cities CC WITH (Nolock) ON CC.CityId=Com.CityId
LEFT JOIN Web.States CS WITH (Nolock) ON CS.StateId=CC.StateId
--LEFT JOIN Web.Processes PS WITH (Nolock) ON PS.ProcessId=H.ProcessId
--LEFT JOIN Web.SalesTaxProductCodes PSSTC WITH (Nolock) ON PSSTC.SalesTaxProductCodeId=PS.SalesTaxProductCodeId
LEFT JOIN Web.People P WITH (Nolock) ON P.PersonID=H.SaleToBuyerId
LEFT JOIN (SELECT TOP 1 * FROM web.SiteDivisionSettings WHERE @DocDate BETWEEN StartDate AND IsNull(EndDate, getdate()) AND SiteId = @Site AND DivisionId = @Division ORDER BY StartDate) SDS ON H.DivisionId = SDS.DivisionId AND H.SiteId = SDS.SiteId
LEFT JOIN(SELECT* FROM Web.PersonAddresses WITH (nolock) WHERE AddressType IS NULL) PA ON PA.PersonId = P.PersonID
LEFT JOIN Web.Cities C WITH (nolock) ON C.CityId = PA.CityId
LEFT JOIN Web.States S WITH (Nolock) ON S.StateId=C.StateId
LEFT JOIN web.SaleDispatchHeaders SDH ON SDH.SaleDispatchHeaderId = H.SaleDispatchHeaderId
LEFT JOIN web.People SB ON SB.PersonID = SDH.SaleToBuyerId
LEFT JOIN(SELECT* FROM Web.PersonAddresses WITH (nolock) WHERE AddressType IS NULL) SPA ON SPA.PersonId = SB.PersonID
LEFT JOIN Web.Cities SC WITH (nolock) ON SC.CityId = SPA.CityId
LEFT JOIN Web.States SS WITH (Nolock) ON SS.StateId= SC.StateId
LEFT JOIN web.ChargeGroupPersons CGP WITH (Nolock) ON CGP.ChargeGroupPersonId=H.SalesTaxGroupPersonId
LEFT JOIN Web.Currencies CUR WITH (Nolock) ON CUR.Id=H.CurrencyId
LEFT JOIN Web.SaleInvoiceLines L WITH (Nolock) ON L.SaleInvoiceHeaderId=H.SaleInvoiceHeaderId
LEFT JOIN Web.SaleDispatchLines JRL WITH (Nolock) ON JRL.SaleDispatchLineId =L.SaleDispatchLineId
LEFT JOIN web.PackingLines PL ON PL.PackingLineId = JRL.PackingLineId
LEFT JOIN web.ProductUids PU WITH (Nolock) ON PU.ProductUidId=PL.ProductUidId
--	LEFT JOIN Web.JobReceiveHeaders JRH WITH (Nolock) ON JRH.JobReceiveHeaderId=JRL.JobReceiveHeaderId
LEFT JOIN Web.SaleOrderLines POl WITH (Nolock) ON POl.SaleOrderLineId=PL.SaleOrderLineId
LEFT JOIN Web.SaleOrderHeaders POH WITH (Nolock) ON POH.SaleOrderHeaderId=POL.SaleOrderHeaderId
LEFT JOIN web.Products PD WITH (Nolock) ON PD.ProductId=isnull(PL.ProductId, POL.ProductId)
LEFT JOIN web.ProductGroups PG WITH(Nolock) ON PG.ProductGroupId=PD.ProductGroupid
LEFT JOIN Web.SalesTaxProductCodes STC WITH (Nolock) ON STC.SalesTaxProductCodeId= IsNull(PD.SalesTaxProductCodeId, Pg.DefaultSalesTaxProductCodeId)
LEFT JOIN Web.Dimension1 D1 WITH(Nolock) ON D1.Dimension1Id=PL.Dimension1Id
LEFT JOIN web.Dimension2 D2 WITH (Nolock) ON D2.Dimension2Id=PL.Dimension2Id
LEFT JOIN web.Dimension3 D3 WITH (Nolock) ON D3.Dimension3Id=PL.Dimension3Id
LEFT JOIN Web.Dimension4 D4 WITH (nolock) ON D4.Dimension4Id=PL.Dimension4Id
LEFT JOIN web.Units U WITH (Nolock) ON U.UnitId=PD.UnitId
LEFT JOIN web.Units DU WITH (Nolock) ON DU.UnitId=PL.DealUnitId
LEFT JOIN Web.Std_PersonRegistrations SPR WITH (Nolock) ON SPR.CustomerId=H.BillToBuyerId
LEFT JOIN web.ChargeGroupProducts CGPD WITH (Nolock) ON L.SalesTaxGroupProductId = CGPD.ChargeGroupProductId
WHERE H.SaleInvoiceHeaderId= " + item + @"
ORDER BY L.Sr";

            }

           ListofQuery QryMain = new ListofQuery();
            QryMain.Query = QueryMain;
            QryMain.QueryName = nameof(QueryMain);
            DocumentPrintData.Add(QryMain);


            String QueryCalculation;
            QueryCalculation = @"DECLARE @StrGrossAmount AS NVARCHAR(50)  
DECLARE @StrBasicExciseDuty AS NVARCHAR(50)  
DECLARE @StrExciseECess AS NVARCHAR(50)  
DECLARE @StrExciseHECess AS NVARCHAR(50)  

DECLARE @StrSalesTaxTaxableAmt AS NVARCHAR(50)  
DECLARE @StrVAT AS NVARCHAR(50)  
DECLARE @StrSAT AS NVARCHAR(50) 
DECLARE @StrCST AS NVARCHAR(50) 

SET @StrGrossAmount = 'Gross Amount'
SET @StrBasicExciseDuty = 'Basic Excise Duty'
SET @StrExciseECess ='Excise ECess'
SET @StrExciseHECess = 'Excise HECess'

SET @StrSalesTaxTaxableAmt = 'Sales Tax Taxable Amt'
SET @StrVAT = 'VAT'
SET @StrSAT = 'SAT'
SET @StrCST = 'CST'

DECLARE @Qry NVARCHAR(Max);
SET @Qry = '
		DECLARE @GrossAmount AS DECIMAL 
		DECLARE @BasicExciseDutyAmount AS DECIMAL 
		DECLARE @SalesTaxTaxableAmt AS DECIMAL 
		
		SELECT @GrossAmount = sum ( CASE WHEN C.ChargeName = ''' + @StrGrossAmount + ''' THEN  H.Amount  ELSE 0 END ) ,
		@BasicExciseDutyAmount = sum( CASE WHEN C.ChargeName = ''' + @StrBasicExciseDuty + ''' THEN  H.Amount  ELSE 0 END ) ,
		@SalesTaxTaxableAmt = sum( CASE WHEN C.ChargeName = ''' + @StrSalesTaxTaxableAmt + ''' THEN  H.Amount  ELSE 0 END )
		FROM web.SaleInvoiceheadercharges H
		LEFT JOIN web.ChargeTypes CT ON CT.ChargeTypeId = H.ChargeTypeId 
		LEFT JOIN web.Charges C ON C.ChargeId = H.ChargeId 
		WHERE H.Amount <> 0 AND H.HeaderTableId	= ' + Convert(Varchar," + item + @" ) + '
		GROUP BY H.HeaderTableId
		
		
		SELECT H.Id, H.HeaderTableId, H.Sr, C.ChargeName, H.Amount, H.ChargeTypeId,  CT.ChargeTypeName, 
		--CASE WHEN C.ChargeName = ''Vat'' THEN ( H.Amount*100/ @GrossAmount ) ELSE H.Rate End  AS Rate,
		CASE 
		WHEN @SalesTaxTaxableAmt>0 And C.ChargeName IN ( ''' + @StrVAT + ''',''' + @StrSAT + ''',''' + @StrCST+ ''')  THEN ( H.Amount*100/ @SalesTaxTaxableAmt   ) 
		WHEN @GrossAmount>0 AND C.ChargeName IN ( ''' + @StrBasicExciseDuty + ''')  THEN ( H.Amount*100/ @GrossAmount   ) 
		WHEN  @BasicExciseDutyAmount>0 AND  C.ChargeName IN ( ''' + @StrExciseECess + ''', ''' +@StrExciseHECess+ ''')  THEN ( H.Amount*100/ @BasicExciseDutyAmount   ) 
		ELSE 0 End  AS Rate,
		''StdDocPrintSub_CalculationHeader.rdl'' AS ReportName,
		''Transaction Charges'' AS ReportTitle     
		FROM  web.SaleInvoiceheadercharges  H
		LEFT JOIN web.ChargeTypes CT ON CT.ChargeTypeId = H.ChargeTypeId 
		LEFT JOIN web.Charges C ON C.ChargeId = H.ChargeId 
		WHERE  ( isnull(H.ChargeTypeId,0) <> ''4'' OR C.ChargeName = ''Net Amount'') AND H.Amount <> 0
--WHERE  1=1
		AND H.HeaderTableId	= ' + Convert(Varchar," + item + @"  ) + ''
		
	--PRINT @Qry; 	
	
	DECLARE @TmpData TABLE
	(
	id BIGINT,
	HeaderTableId INT,
	Sr INT,
	ChargeName NVARCHAR(50),
	Amount DECIMAL(18,4),
	ChargeTypeId INT,
	ChargeTypeName NVARCHAR(50),
	Rate DECIMAL(38,20),
	ReportName nVarchar(255),
	ReportTitle nVarchar(255)
	);
	
	
	Insert Into @TmpData EXEC(@Qry)
	SELECT id,HeaderTableId,Sr,ChargeName,Amount,ChargeTypeId,ChargeTypeName,Rate,ReportName 
	FROM @TmpData
	ORDER BY Sr	";


            ListofQuery QryCalculation = new ListofQuery();
            QryCalculation.Query = QueryCalculation;
            QryCalculation.QueryName = nameof(QueryCalculation);
            DocumentPrintData.Add(QryCalculation);


            String QueryGSTSummary;
            QueryGSTSummary = @"DECLARE @Qry NVARCHAR(Max);

SET @Qry = '
SELECT  
--CASE WHEN PS.ProcessName IN (''Purchase'',''Sale'') THEN isnull(STGP.PrintingDescription,STGP.ChargeGroupProductName) ELSE PS.GSTPrintDesc END as ChargeGroupProductName, 
isnull(STGP.PrintingDescription,STGP.ChargeGroupProductName) as ChargeGroupProductName, 
Sum(CASE WHEN ct.ChargeTypeName =''Sales Taxable Amount'' THEN lc.Amount ELSE 0 End) AS TaxableAmount,
Sum(CASE WHEN ct.ChargeTypeName =''IGST'' THEN lc.Amount ELSE 0 End) AS IGST,
Sum(CASE WHEN ct.ChargeTypeName =''CGST'' THEN lc.Amount ELSE 0 End) AS CGST,
Sum(CASE WHEN ct.ChargeTypeName =''SGST'' THEN lc.Amount ELSE 0 End) AS SGST,
Sum(CASE WHEN ct.ChargeTypeName =''GST Cess'' THEN lc.Amount ELSE 0 End) AS GSTCess,
''StdDocPrintSub_GSTSummary.rdl'' AS ReportName
FROM Web.SaleInvoiceLines L
LEFT JOIN Web.SaleInvoiceLineCharges LC ON L.SaleInvoiceLineId = LC.LineTableId 
LEFT JOIN web.SaleInvoiceheaders H ON H.SaleInvoiceHeaderId = L.SaleInvoiceHeaderId
LEFT JOIN Web.Charges C ON C.ChargeId=LC.ChargeId
LEFT JOIN web.ChargeTypes CT ON LC.ChargeTypeId = CT.ChargeTypeId 
LEFT JOIN web.ChargeGroupProducts STGP ON L.SalesTaxGroupProductId = STGP.ChargeGroupProductId
WHERE L.SaleInvoiceHeaderId =" + item + @" 
GROUP BY isnull(STGP.PrintingDescription,STGP.ChargeGroupProductName)
--GROUP BY CASE WHEN PS.ProcessName IN (''Purchase'',''Sale'') THEN isnull(STGP.PrintingDescription,STGP.ChargeGroupProductName) ELSE PS.GSTPrintDesc END '

--PRINT @Qry;
EXEC(@Qry);	";


            ListofQuery QryGSTSummary = new ListofQuery();
            QryGSTSummary.Query = QueryGSTSummary;
            QryGSTSummary.QueryName = nameof(QueryGSTSummary);
            DocumentPrintData.Add(QryGSTSummary);

            return DocumentPrintData;

        }

        [HttpGet]
        private ActionResult PrintOut(int id, string SqlProcForPrint)
        {
            String query = SqlProcForPrint;
            return Redirect((string)System.Configuration.ConfigurationManager.AppSettings["JobsDomain"] + "/Report_DocumentPrint/DocumentPrint/?DocumentId=" + id + "&queryString=" + query);
        }

        [HttpGet]
        public ActionResult Print(int id)
        {
            SaleInvoiceHeader header = _SaleInvoiceHeaderService.FindDirectSaleInvoice(id);
            if (header.Status == (int)StatusConstants.Drafted || header.Status == (int)StatusConstants.Import)
            {
                var SEttings = new SaleInvoiceSettingService(_unitOfWork).GetSaleInvoiceSettingForDocument(header.DocTypeId, header.DivisionId, header.SiteId);
                if (string.IsNullOrEmpty(SEttings.SqlProcDocumentPrint))
                    throw new Exception("Document Print Not Configured");
                else
                    return PrintOut(id, SEttings.SqlProcDocumentPrint);
            }
            else
                return HttpNotFound();

        }

        [HttpGet]
        public ActionResult PrintAfter_Submit(int id)
        {
            SaleInvoiceHeader header = _SaleInvoiceHeaderService.Find(id);
            if (header.Status == (int)StatusConstants.Submitted || header.Status == (int)StatusConstants.Modified)
            {
                var SEttings = new SaleInvoiceSettingService(_unitOfWork).GetSaleInvoiceSettingForDocument(header.DocTypeId, header.DivisionId, header.SiteId);
                if (string.IsNullOrEmpty(SEttings.SqlProcDocumentPrint_AfterSubmit))
                    throw new Exception("Document Print Not Configured");
                else
                    return PrintOut(id, SEttings.SqlProcDocumentPrint_AfterSubmit);
            }
            else
                return HttpNotFound();
        }

        [HttpGet]
        public ActionResult PrintAfter_Approve(int id)
        {
            SaleInvoiceHeader header = _SaleInvoiceHeaderService.Find(id);
            if (header.Status == (int)StatusConstants.Approved)
            {
                var SEttings = new SaleInvoiceSettingService(_unitOfWork).GetSaleInvoiceSettingForDocument(header.DocTypeId, header.DivisionId, header.SiteId);
                if (string.IsNullOrEmpty(SEttings.SqlProcDocumentPrint_AfterApprove))
                    throw new Exception("Document Print Not Configured");
                else
                    return PrintOut(id, SEttings.SqlProcDocumentPrint_AfterApprove);
            }
            else
                return HttpNotFound();
        }


        [HttpGet]
        public ActionResult Report(int id)
        {
            DocumentType Dt = new DocumentType();
            Dt = new DocumentTypeService(_unitOfWork).Find(id);

            SaleInvoiceSetting SEttings = new SaleInvoiceSettingService(_unitOfWork).GetSaleInvoiceSettingForDocument(Dt.DocumentTypeId, (int)System.Web.HttpContext.Current.Session["DivisionId"], (int)System.Web.HttpContext.Current.Session["SiteId"]);

            Dictionary<int, string> DefaultValue = new Dictionary<int, string>();

            //if (!Dt.ReportMenuId.HasValue)
            //    throw new Exception("Report Menu not configured in document types");

            if (!Dt.ReportMenuId.HasValue)
                return Redirect((string)System.Configuration.ConfigurationManager.AppSettings["JobsDomain"] + "/GridReport/GridReportLayout/?MenuName=Sale Invoice Report&DocTypeId=" + id.ToString());

            Model.Models.Menu menu = new MenuService(_unitOfWork).Find(Dt.ReportMenuId ?? 0);

            if (menu != null)
            {
                ReportHeader header = new ReportHeaderService(_unitOfWork).GetReportHeaderByName(menu.MenuName);

                ReportLine Line = new ReportLineService(_unitOfWork).GetReportLineByName("DocumentType", header.ReportHeaderId);
                if (Line != null)
                    DefaultValue.Add(Line.ReportLineId, id.ToString());
                ReportLine Site = new ReportLineService(_unitOfWork).GetReportLineByName("Site", header.ReportHeaderId);
                if (Site != null)
                    DefaultValue.Add(Site.ReportLineId, ((int)System.Web.HttpContext.Current.Session["SiteId"]).ToString());
                ReportLine Division = new ReportLineService(_unitOfWork).GetReportLineByName("Division", header.ReportHeaderId);
                if (Division != null)
                    DefaultValue.Add(Division.ReportLineId, ((int)System.Web.HttpContext.Current.Session["DivisionId"]).ToString());
                //ReportLine Process = new ReportLineService(_unitOfWork).GetReportLineByName("Process", header.ReportHeaderId);
                //if (Process != null)
                //    DefaultValue.Add(Process.ReportLineId, ((int)SEttings.ProcessId).ToString());
            }

            TempData["ReportLayoutDefaultValues"] = DefaultValue;

            return Redirect((string)System.Configuration.ConfigurationManager.AppSettings["JobsDomain"] + "/Report_ReportPrint/ReportPrint/?MenuId=" + Dt.ReportMenuId);

        }

        public ActionResult Import(int id)//Document Type Id
        {
            //ControllerAction ca = new ControllerActionService(_unitOfWork).Find(id);

            int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];

            var settings = new SaleInvoiceSettingService(_unitOfWork).GetSaleInvoiceSettingForDocument(id, DivisionId, SiteId);

            if (settings != null)
            {
                if (settings.ImportMenuId != null)
                {
                    MenuViewModel menuviewmodel = new MenuService(_unitOfWork).GetMenu((int)settings.ImportMenuId);

                    if (menuviewmodel == null)
                    {
                        return View("~/Views/Shared/UnderImplementation.cshtml");
                    }
                    else if (!string.IsNullOrEmpty(menuviewmodel.URL))
                    {
                        return Redirect(System.Configuration.ConfigurationManager.AppSettings[menuviewmodel.URL] + "/" + menuviewmodel.ControllerName + "/" + menuviewmodel.ActionName + "/" + id + "?MenuId=" + menuviewmodel.MenuId);
                    }
                    else
                    {
                        return RedirectToAction(menuviewmodel.ActionName, menuviewmodel.ControllerName, new { MenuId = menuviewmodel.MenuId, id = id });
                    }
                }
            }
            return RedirectToAction("Index", new { id = id });
        }

        public ActionResult GetCustomPerson(string searchTerm, int pageSize, int pageNum, int filter)//DocTypeId
        {
            var Query = _SaleInvoiceHeaderService.GetCustomPerson(filter, searchTerm);
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

        #region submitValidation
        public bool Submitvalidation(int id, out string Msg)
        {
            Msg = "";
            int SaleinvoiceLine = (new SaleInvoiceLineService(_unitOfWork).GetDirectSaleInvoiceLineListForIndex(id)).Count();
            if (SaleinvoiceLine == 0)
            {
                Msg = "Add Line Record. <br />";
            }
            else
            {
                Msg = "";
            }
            return (string.IsNullOrEmpty(Msg));
        }

        #endregion submitValidation

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

        [HttpGet]
        public ActionResult _CreateInvoiceReturn(int id)
        {
            InvoiceReturn InvoiceReturn = new InvoiceReturn();
            InvoiceReturn.SaleInvoiceHeaderId = id;
            ViewBag.ReasonList = new ReasonService(_unitOfWork).GetReasonList(TransactionDocCategoryConstants.SaleInvoiceReturn).ToList();
            InvoiceReturn.DocDate = DateTime.Now;
            return PartialView("_InvoiceReturn", InvoiceReturn);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult _CreateInvoiceReturnPost(InvoiceReturn svm)
        {
            int Cnt = 0;
            int Serial = 0;
            int pk = 0;
            int Gpk = 0;
            int PersonCount = 0;
            bool HeaderChargeEdit = false;

            SaleInvoiceHeader SaleInvoiceHeader = new SaleInvoiceHeaderService(_unitOfWork).FindDirectSaleInvoice(svm.SaleInvoiceHeaderId);
            SaleDispatchHeader SaleDispatchHeader = new SaleDispatchHeaderService(_unitOfWork).Find(SaleInvoiceHeader.SaleDispatchHeaderId ?? 0);
            var DispatchLine = new SaleDispatchLineService(_unitOfWork).GetSaleDispatchLineList(SaleDispatchHeader.SaleDispatchHeaderId);


            List<LineChargeRates> LineChargeRates = new List<LineChargeRates>();

            var SaleInvoiceSettings = new SaleInvoiceSettingService(_unitOfWork).GetSaleInvoiceSettingForDocument(SaleInvoiceHeader.DocTypeId, SaleInvoiceHeader.DivisionId, SaleInvoiceHeader.SiteId);
            int InvoiceRetHeaderDocTypeId = 0;

            if (SaleInvoiceSettings.SaleInvoiceReturnDocTypeId == null)
            {
                string message = "Invoice Return Document Type is not difined in settings.";
                ModelState.AddModelError("", message);
                return PartialView("_InvoiceReturn", svm);
            }
            else
            {
                InvoiceRetHeaderDocTypeId = (int)SaleInvoiceSettings.SaleInvoiceReturnDocTypeId;
            }


            if (ModelState.IsValid)
            {
                var SaleInvoiceLineList = (from p in db.ViewSaleInvoiceBalance
                                           join l in db.SaleInvoiceLine on p.SaleInvoiceLineId equals l.SaleInvoiceLineId into linetable
                                           from linetab in linetable.DefaultIfEmpty()
                                           join t in db.SaleInvoiceHeader on p.SaleInvoiceHeaderId equals t.SaleInvoiceHeaderId into table
                                           from tab in table.DefaultIfEmpty()
                                           join t1 in db.SaleDispatchLine on p.SaleDispatchLineId equals t1.SaleDispatchLineId into table1
                                           from tab1 in table1.DefaultIfEmpty()
                                           join packtab in db.PackingLine on tab1.PackingLineId equals packtab.PackingLineId
                                           join product in db.Product on p.ProductId equals product.ProductId into table2
                                           from tab2 in table2.DefaultIfEmpty()
                                           join B in db.BusinessEntity on tab.SaleToBuyerId equals B.PersonID into BusinessEntityTable
                                           from BusinessEntityTab in BusinessEntityTable.DefaultIfEmpty()
                                           where p.SaleInvoiceHeaderId == SaleInvoiceHeader.SaleInvoiceHeaderId
                                           && p.BalanceQty > 0
                                           select new SaleInvoiceReturnLineViewModel
                                           {
                                               Dimension1Name = packtab.Dimension1.Dimension1Name,
                                               Dimension2Name = packtab.Dimension2.Dimension2Name,
                                               Specification = packtab.Specification,
                                               InvoiceBalQty = p.BalanceQty,
                                               Qty = p.BalanceQty,
                                               SaleInvoiceHeaderDocNo = tab.DocNo,
                                               ProductName = tab2.ProductName,
                                               ProductId = p.ProductId,
                                               GodownId = tab1.GodownId,
                                               SaleInvoiceLineId = p.SaleInvoiceLineId,
                                               UnitId = tab2.UnitId,
                                               UnitConversionMultiplier = linetab.UnitConversionMultiplier ?? 0,
                                               DealUnitId = linetab.DealUnitId,
                                               Rate = linetab.Rate,
                                               RateAfterDiscount = packtab.SaleOrderLine == null ? 0 : (packtab.SaleOrderLine.Amount / packtab.SaleOrderLine.DealQty),
                                               Amount = linetab.Amount,
                                               unitDecimalPlaces = tab2.Unit.DecimalPlaces,
                                               DealunitDecimalPlaces = linetab.DealUnit.DecimalPlaces,
                                               DiscountPer = linetab.DiscountPer,
                                               DiscountAmount = linetab.DiscountAmount,
                                               ProductUidName = packtab.ProductUid.ProductUidName,
                                               SalesTaxGroupPersonId = BusinessEntityTab.SalesTaxGroupPartyId,
                                               SalesTaxGroupProductId = linetab.SalesTaxGroupProductId
                                           }).ToList();
                
                if (SaleInvoiceLineList.Sum(i => i.InvoiceBalQty) > 0)
                {
                    SaleInvoiceSetting Settings = new SaleInvoiceSettingService(_unitOfWork).GetSaleInvoiceSettingForDocument(InvoiceRetHeaderDocTypeId, SaleInvoiceHeader.DivisionId, SaleInvoiceHeader.SiteId);

                    SaleDispatchReturnHeader GoodsRetHeader = new SaleDispatchReturnHeader();
                    GoodsRetHeader.DocTypeId = (int)Settings.DocTypeDispatchReturnId;
                    GoodsRetHeader.DocDate = svm.DocDate;
                    GoodsRetHeader.DocNo = new DocumentTypeService(_unitOfWork).FGetNewDocNo("DocNo", ConfigurationManager.AppSettings["DataBaseSchema"] + ".SaleDispatchReturnHeaders", GoodsRetHeader.DocTypeId, svm.DocDate, SaleInvoiceHeader.DivisionId, SaleInvoiceHeader.SiteId);
                    GoodsRetHeader.SiteId = SaleInvoiceHeader.SiteId;
                    GoodsRetHeader.DivisionId = SaleInvoiceHeader.DivisionId;
                    GoodsRetHeader.BuyerId = SaleInvoiceHeader.SaleToBuyerId;
                    GoodsRetHeader.ReasonId = svm.ReasonId;
                    GoodsRetHeader.GodownId = DispatchLine.FirstOrDefault().GodownId;
                    GoodsRetHeader.Remark = svm.Remark;
                    GoodsRetHeader.CreatedDate = DateTime.Now;
                    GoodsRetHeader.ModifiedDate = DateTime.Now;
                    GoodsRetHeader.CreatedBy = User.Identity.Name;
                    GoodsRetHeader.ModifiedBy = User.Identity.Name;
                    GoodsRetHeader.ObjectState = Model.ObjectState.Added;
                    new SaleDispatchReturnHeaderService(_unitOfWork).Create(GoodsRetHeader);

                    SaleInvoiceReturnHeader InvoiceRetHeader = new SaleInvoiceReturnHeader();
                    InvoiceRetHeader.DocTypeId = InvoiceRetHeaderDocTypeId;
                    InvoiceRetHeader.DocDate = svm.DocDate;
                    InvoiceRetHeader.DocNo = new DocumentTypeService(_unitOfWork).FGetNewDocNo("DocNo", ConfigurationManager.AppSettings["DataBaseSchema"] + ".SaleInvoiceReturnHeaders", InvoiceRetHeader.DocTypeId, svm.DocDate, SaleInvoiceHeader.DivisionId, SaleInvoiceHeader.SiteId);
                    InvoiceRetHeader.BuyerId = SaleInvoiceHeader.SaleToBuyerId;
                    InvoiceRetHeader.SiteId = SaleInvoiceHeader.SiteId;
                    InvoiceRetHeader.DivisionId = SaleInvoiceHeader.DivisionId;
                    InvoiceRetHeader.BuyerId = SaleInvoiceHeader.SaleToBuyerId;
                    InvoiceRetHeader.SalesTaxGroupPersonId = SaleInvoiceHeader.SalesTaxGroupPersonId;


                    //InvoiceRetHeader.CurrencyId = SaleInvoiceHeader.CurrencyId;
                    InvoiceRetHeader.ReasonId = svm.ReasonId;
                    InvoiceRetHeader.Remark = svm.Remark;
                    InvoiceRetHeader.Nature = TransactionNatureConstants.Return;
                    InvoiceRetHeader.CreatedDate = DateTime.Now;
                    InvoiceRetHeader.ModifiedDate = DateTime.Now;
                    InvoiceRetHeader.CreatedBy = User.Identity.Name;
                    InvoiceRetHeader.ModifiedBy = User.Identity.Name;
                    InvoiceRetHeader.SaleDispatchReturnHeaderId = GoodsRetHeader.SaleDispatchReturnHeaderId;
                    InvoiceRetHeader.ObjectState = Model.ObjectState.Added;
                    new SaleInvoiceReturnHeaderService(_unitOfWork).Create(InvoiceRetHeader);

                    int CalculationId = Settings.CalculationId;

                    //IEnumerable<SaleInvoiceLine> SaleInvoiceLineList = new SaleInvoiceLineService(_unitOfWork).GetSaleInvoiceLineList(Sh.SaleInvoiceHeaderId);



                    List<LineDetailListViewModel> LineList = new List<LineDetailListViewModel>();
                    List<HeaderChargeViewModel> HeaderCharges = new List<HeaderChargeViewModel>();
                    List<LineChargeViewModel> LineCharges = new List<LineChargeViewModel>();


                    foreach (var item in SaleInvoiceLineList)
                    {
                        decimal balqty = (from p in db.ViewSaleInvoiceBalance
                                          where p.SaleInvoiceLineId == item.SaleInvoiceLineId
                                          select p.BalanceQty).FirstOrDefault();


                        if (item.Qty > 0 && item.Qty <= balqty)
                        {
                            SaleInvoiceReturnLine line = new SaleInvoiceReturnLine();
                            //var receipt = new SaleDispatchLineService(_unitOfWork).Find(item.SaleDispatchLineId );


                            line.SaleInvoiceReturnHeaderId = InvoiceRetHeader.SaleInvoiceReturnHeaderId;
                            line.SaleInvoiceLineId = item.SaleInvoiceLineId;
                            line.Qty = item.Qty;
                            line.Sr = Serial++;
                            line.DiscountPer = item.DiscountPer;
                            line.DiscountAmount = item.DiscountAmount;
                            line.Rate = item.Rate;
                            line.DealQty = item.UnitConversionMultiplier * item.Qty;
                            line.DealUnitId = item.DealUnitId;
                            line.UnitConversionMultiplier = item.UnitConversionMultiplier;
                            line.Amount = item.Amount;

                            line.SalesTaxGroupProductId = item.SalesTaxGroupProductId;
                            line.Remark = item.Remark;
                            line.CreatedDate = DateTime.Now;
                            line.ModifiedDate = DateTime.Now;
                            line.CreatedBy = User.Identity.Name;
                            line.ModifiedBy = User.Identity.Name;
                            line.SaleInvoiceReturnLineId = pk;


                            SaleDispatchReturnLine GLine = Mapper.Map<SaleInvoiceReturnLine, SaleDispatchReturnLine>(line);
                            GLine.SaleDispatchLineId = new SaleInvoiceLineService(_unitOfWork).Find(line.SaleInvoiceLineId).SaleDispatchLineId;
                            GLine.SaleDispatchReturnHeaderId = GoodsRetHeader.SaleDispatchReturnHeaderId;
                            GLine.SaleDispatchReturnLineId = Gpk;
                            GLine.Qty = line.Qty;
                            GLine.GodownId = (int)item.GodownId;
                            GLine.ObjectState = Model.ObjectState.Added;


                            SaleDispatchLine SaleDispatchLine = new SaleDispatchLineService(_unitOfWork).Find(GLine.SaleDispatchLineId);
                            PackingLine PackingLin = new PackingLineService(_unitOfWork).Find(SaleDispatchLine.PackingLineId);

                            StockViewModel StockViewModel = new StockViewModel();


                            if (Cnt == 0)
                            {
                                StockViewModel.StockHeaderId = GoodsRetHeader.StockHeaderId ?? 0;
                            }
                            else
                            {
                                if (GoodsRetHeader.StockHeaderId != null && GoodsRetHeader.StockHeaderId != 0)
                                {
                                    StockViewModel.StockHeaderId = (int)GoodsRetHeader.StockHeaderId;
                                }
                                else
                                {
                                    StockViewModel.StockHeaderId = -1;
                                }

                            }

                            StockViewModel.StockId = -Cnt;

                            StockViewModel.DocHeaderId = GoodsRetHeader.SaleDispatchReturnHeaderId;
                            StockViewModel.DocLineId = SaleDispatchLine.SaleDispatchLineId;
                            StockViewModel.DocTypeId = GoodsRetHeader.DocTypeId;
                            StockViewModel.StockHeaderDocDate = GoodsRetHeader.DocDate;
                            StockViewModel.StockDocDate = GoodsRetHeader.DocDate;
                            StockViewModel.DocNo = GoodsRetHeader.DocNo;
                            StockViewModel.DivisionId = GoodsRetHeader.DivisionId;
                            StockViewModel.SiteId = GoodsRetHeader.SiteId;
                            StockViewModel.CurrencyId = null;
                            StockViewModel.PersonId = GoodsRetHeader.BuyerId;
                            StockViewModel.ProductId = PackingLin.ProductId;
                            StockViewModel.ProductUidId = PackingLin.ProductUidId;
                            StockViewModel.HeaderFromGodownId = null;
                            StockViewModel.HeaderGodownId = GLine.GodownId;
                            StockViewModel.HeaderProcessId = Settings.ProcessId;
                            StockViewModel.GodownId = (int)GLine.GodownId;
                            StockViewModel.Remark = svm.Remark;
                            StockViewModel.Status = 0;
                            StockViewModel.ProcessId = null;
                            StockViewModel.LotNo = null;
                            StockViewModel.CostCenterId = null;
                            StockViewModel.Qty_Iss = 0;
                            StockViewModel.Qty_Rec = GLine.Qty;
                            StockViewModel.Rate = null;
                            StockViewModel.ExpiryDate = null;
                            StockViewModel.Specification = PackingLin.Specification;
                            StockViewModel.Dimension1Id = PackingLin.Dimension1Id;
                            StockViewModel.Dimension2Id = PackingLin.Dimension2Id;
                            StockViewModel.CreatedBy = User.Identity.Name;
                            StockViewModel.CreatedDate = DateTime.Now;
                            StockViewModel.ModifiedBy = User.Identity.Name;
                            StockViewModel.ModifiedDate = DateTime.Now;

                            string StockPostingError = "";
                            StockPostingError = new StockService(_unitOfWork).StockPost(ref StockViewModel);

                            if (StockPostingError != "")
                            {
                                string message = StockPostingError;
                                ModelState.AddModelError("", message);
                                return PartialView("_InvoiceReturn", svm);
                            }


                            if (Cnt == 0)
                            {
                                GoodsRetHeader.StockHeaderId = StockViewModel.StockHeaderId;
                            }


                            GLine.StockId = StockViewModel.StockId;


                            new SaleDispatchReturnLineService(_unitOfWork).Create(GLine);

                            line.SaleDispatchReturnLineId = GLine.SaleDispatchReturnLineId;
                            line.ObjectState = Model.ObjectState.Added;
                            new SaleInvoiceReturnLineService(_unitOfWork).Create(line);

                            LineList.Add(new LineDetailListViewModel { Amount = line.Amount, Rate = line.Rate, LineTableId = line.SaleInvoiceReturnLineId, HeaderTableId = item.SaleInvoiceReturnHeaderId, PersonID = InvoiceRetHeader.BuyerId, DealQty = line.DealQty });

                            List<CalculationProductViewModel> ChargeRates = new CalculationProductService(_unitOfWork).GetChargeRates(CalculationId, InvoiceRetHeader.DocTypeId, InvoiceRetHeader.SiteId, InvoiceRetHeader.DivisionId,
                                Settings.ProcessId ?? 0, item.SalesTaxGroupPersonId, item.SalesTaxGroupProductId).ToList();
                            if (ChargeRates != null)
                            {
                                LineChargeRates.Add(new LineChargeRates { LineId = line.SaleInvoiceReturnLineId, ChargeRates = ChargeRates });
                            }



                            Gpk++;
                            pk++;

                            Cnt = Cnt + 1;
                        }
                    }


                    var LineListWithReferences = (from p in LineList
                                                  join t3 in LineChargeRates on p.LineTableId equals t3.LineId into LineChargeRatesTable
                                                  from LineChargeRatesTab in LineChargeRatesTable.DefaultIfEmpty()
                                                  orderby p.LineTableId
                                                  select new LineDetailListViewModel
                                                  {
                                                      Amount = p.Amount,
                                                      DealQty = p.DealQty,
                                                      HeaderTableId = p.HeaderTableId,
                                                      LineTableId = p.LineTableId,
                                                      PersonID = p.PersonID,
                                                      Rate = p.Rate,
                                                      CostCenterId = p.CostCenterId,
                                                      ChargeRates = LineChargeRatesTab.ChargeRates,
                                                  }).ToList();

                    if (CalculationId != null)
                    {
                        new ChargesCalculationService(_unitOfWork).CalculateCharges(LineListWithReferences, InvoiceRetHeader.SaleInvoiceReturnHeaderId, (int)CalculationId, null, out LineCharges, out HeaderChargeEdit, out HeaderCharges, "Web.SaleInvoiceReturnHeaderCharges", "Web.SaleInvoiceReturnLineCharges", out PersonCount, InvoiceRetHeader.DocTypeId, InvoiceRetHeader.SiteId, InvoiceRetHeader.DivisionId);
                    }

                    // Saving Charges
                    foreach (var item in LineCharges)
                    {
                        SaleInvoiceReturnLineCharge PoLineCharge = Mapper.Map<LineChargeViewModel, SaleInvoiceReturnLineCharge>(item);
                        PoLineCharge.ObjectState = Model.ObjectState.Added;
                        new SaleInvoiceReturnLineChargeService(_unitOfWork).Create(PoLineCharge);
                    }


                    //Saving Header charges
                    for (int i = 0; i < HeaderCharges.Count(); i++)
                    {
                        SaleInvoiceReturnHeaderCharge POHeaderCharge = Mapper.Map<HeaderChargeViewModel, SaleInvoiceReturnHeaderCharge>(HeaderCharges[i]);
                        POHeaderCharge.HeaderTableId = InvoiceRetHeader.SaleInvoiceReturnHeaderId;
                        POHeaderCharge.PersonID = InvoiceRetHeader.BuyerId;
                        POHeaderCharge.ObjectState = Model.ObjectState.Added;
                        new SaleInvoiceReturnHeaderChargeService(_unitOfWork).Create(POHeaderCharge);
                    }

                    try
                    {
                        _unitOfWork.Save();
                    }
                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        ModelState.AddModelError("", message);
                        return PartialView("_InvoiceReturn", svm);
                    }

                    LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                    {
                        DocTypeId = SaleInvoiceHeader.DocTypeId,
                        DocId = InvoiceRetHeader.SaleInvoiceReturnHeaderId,
                        ActivityType = (int)ActivityTypeContants.MultipleCreate,
                        DocNo = SaleInvoiceHeader.DocNo,
                        DocDate = SaleInvoiceHeader.DocDate,
                        DocStatus = SaleInvoiceHeader.Status,
                    }));


                    try
                    {
                        StockHeader StockHeader = new StockHeaderService(_unitOfWork).Find((int)GoodsRetHeader.StockHeaderId);
                        StockHeader.DocHeaderId = GoodsRetHeader.SaleDispatchReturnHeaderId;
                        new StockHeaderService(_unitOfWork).Update(StockHeader);
                        _unitOfWork.Save();
                    }
                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                    }




                    //return Json(new { success = true });
                    //return Redirect(System.Configuration.ConfigurationManager.AppSettings["SaleDomain"] + "/SaleInvoiceReturnHeader/Submit/" + InvoiceRetHeader.SaleInvoiceReturnHeaderId);
                    //return Redirect(System.Configuration.ConfigurationManager.AppSettings["SaleDomain"] + "/DirectSaleInvoiceHeader/_InvoiceReturnSubmit/" + InvoiceRetHeader.SaleInvoiceReturnHeaderId);
                    //return RedirectToAction("Index", new { id = SaleInvoiceHeader.DocTypeId, IndexType = "All" }).Success("Record submitted successfully.");
                    return Json(new { success = true, Url = "/SaleInvoiceReturnHeader/Submit/" + InvoiceRetHeader.SaleInvoiceReturnHeaderId });


                }
            }
            else
            {
                string message = "Balance is 0 for this invoice.";
                ModelState.AddModelError("", message);
                return PartialView("_InvoiceReturn", svm);
            }
            return PartialView("_InvoiceReturn", svm);
        }

        public ActionResult _InvoiceReturnSubmit(int id)
        {
            return Redirect(System.Configuration.ConfigurationManager.AppSettings["SaleDomain"] + "/SaleInvoiceReturnHeader/Submit/" + id);
        }

        public ActionResult GetPackingHeader(string searchTerm, int pageSize, int pageNum, int filter)
        {
            var Query = new SaleInvoiceLineService(_unitOfWork).GetPendingPackingHeaderForSaleInvoice(filter, searchTerm);
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

        



        //public JsonResult GetPartialLockReason(int Id)
        //{
        //    string PartialLockReason = "";

        //    var SaleInvoiceReturn = db.SaleInvoiceReturnLine.Where(i => i.SaleInvoiceLine.SaleInvoiceHeaderId == Id).FirstOrDefault();
        //    if (SaleInvoiceReturn != null)
        //    {
        //        var ReturnNature = (from H in db.SaleInvoiceReturnHeader where H.SaleInvoiceReturnHeaderId == SaleInvoiceReturn.SaleInvoiceReturnHeaderId select new { DocTypeNature = H.DocType.Nature }).FirstOrDefault().DocTypeNature;
        //        PartialLockReason = ReturnNature;
        //    }
        //    return Json(PartialLockReason);
        //}
    }


    public class PersonLedgerBalance
    {
        public Decimal Balance { get; set; }
    }

    public class InvoiceReturn
    {
        public int SaleInvoiceHeaderId { get; set; }
        public DateTime DocDate { get; set; }
        public int ReasonId { get; set; }
        public string ReasonName { get; set; }
        public string Remark { get; set; }
    }
}
