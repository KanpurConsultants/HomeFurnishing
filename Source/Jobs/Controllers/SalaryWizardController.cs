using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Core.Common;
using Model.Models;
using Data.Models;
using Service;
using Data.Infrastructure;
using System.Configuration;
using Model.ViewModel;
using System.Data.SqlClient;
using System.Data;
using System.Xml.Linq;
using Model.DatabaseViews;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Dynamic;
using Jobs.Helpers;
using System.Text;
using Model.ViewModels;
using System.Web.Script.Serialization;

namespace Jobs.Controllers
{
    [Authorize]
    public class SalaryWizardController : System.Web.Mvc.Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();
        protected string connectionString = (string)System.Web.HttpContext.Current.Session["DefaultConnectionString"];
        ISalaryWizardService _SalaryWizardService;
        IUnitOfWork _unitOfWork;
        IExceptionHandlingService _exception;
        List<string> UserRoles = new List<string>();


        public SalaryWizardController(ISalaryWizardService SalaryWizardService, IUnitOfWork unitOfWork, IExceptionHandlingService exec)
        {
            _SalaryWizardService = SalaryWizardService;
            UserRoles = (List<string>)System.Web.HttpContext.Current.Session["Roles"];

            _exception = exec;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public ActionResult SalaryWizard(int id)
        {
            SalaryWizardViewModel vm = new SalaryWizardViewModel();
            vm.DocDate = DateTime.Now;
            vm.DocTypeId = id;

            return View(vm);
        }

        public JsonResult SalaryWizardFill(SalaryWizardViewModel vm)
        {
            IEnumerable<SalaryWizardResultViewModel> SalaryWizardResultViewModel = _SalaryWizardService.GetSalaryDetail(vm);

            JsonResult json = Json(new { Success = true, Data = SalaryWizardResultViewModel }, JsonRequestBehavior.AllowGet);
            json.MaxJsonLength = int.MaxValue;
            return json;
        }

        public string Post(List<SalaryWizardResultViewModel> SalaryDataList)
        {
            int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];

            if (SalaryDataList.Count > 0)
            {
                DateTime DocDate  = Convert.ToDateTime(SalaryDataList.FirstOrDefault().DocDate);
                int DocTypeId = SalaryDataList.FirstOrDefault().DocTypeId;
                string DocNo = new DocumentTypeService(_unitOfWork).FGetNewDocNo("DocNo", ConfigurationManager.AppSettings["DataBaseSchema"] + ".SalaryHeaders", DocTypeId, DocDate, DivisionId, SiteId);

                SalaryHeader Header = new SalaryHeader();
                Header.DocDate = DocDate;
                Header.DocTypeId = DocTypeId;
                Header.DocNo = DocNo;
                Header.DivisionId = DivisionId;
                Header.SiteId = SiteId;
                Header.Remark = null;
                Header.CreatedDate = DateTime.Now;
                Header.CreatedBy = User.Identity.Name;
                Header.ModifiedDate = DateTime.Now;
                Header.ModifiedBy = User.Identity.Name;
                Header.ObjectState = Model.ObjectState.Added;
                db.SalaryHeader.Add(Header);

                int i = 0;
                foreach(var SalaryData in SalaryDataList)
                {
                    SalaryLine Line = new SalaryLine();
                    Line.SalaryLineId = i--;
                    Line.SalaryHeaderId = Header.SalaryHeaderId;
                    Line.EmployeeId = SalaryData.EmployeeId;
                    Line.Days = SalaryData.Days;
                    Line.OtherAddition = SalaryData.Additions;
                    Line.OtherDeduction = SalaryData.Deductions;
                    Line.LoanEMI = SalaryData.LoanEMI;
                    Line.NetSalary = 0;
                    Line.CreatedDate = DateTime.Now;
                    Line.CreatedBy = User.Identity.Name;
                    Line.ModifiedDate = DateTime.Now;
                    Line.ModifiedBy = User.Identity.Name;
                    Line.ObjectState = Model.ObjectState.Added;
                    

                    var EmployeeChargesList = (from H in db.EmployeeCharge where H.HeaderTableId == Line.EmployeeId select H).ToList();
                    int j = 0, Sr = 0;
                    foreach(var EmployeeCharge in EmployeeChargesList)
                    {
                        SalaryLineCharge LineCharge = new SalaryLineCharge();
                        LineCharge.Id = j--;
                        LineCharge.HeaderTableId = Header.SalaryHeaderId;
                        LineCharge.LineTableId = Line.SalaryLineId;
                        LineCharge.Sr = Sr + 1;
                        LineCharge.ChargeId = EmployeeCharge.ChargeId;
                        LineCharge.AddDeduct = EmployeeCharge.AddDeduct;
                        LineCharge.AffectCost = EmployeeCharge.AffectCost;
                        LineCharge.ChargeTypeId = EmployeeCharge.ChargeTypeId;
                        LineCharge.CalculateOnId = EmployeeCharge.CalculateOnId;
                        LineCharge.PersonID = EmployeeCharge.PersonID;
                        LineCharge.LedgerAccountDrId = EmployeeCharge.LedgerAccountDrId;
                        LineCharge.LedgerAccountCrId = EmployeeCharge.LedgerAccountCrId;
                        LineCharge.ContraLedgerAccountId = EmployeeCharge.ContraLedgerAccountId;
                        LineCharge.CostCenterId = EmployeeCharge.CostCenterId;
                        LineCharge.RateType = EmployeeCharge.RateType;
                        LineCharge.IncludedInBase = EmployeeCharge.IncludedInBase;
                        LineCharge.ParentChargeId = EmployeeCharge.ParentChargeId;
                        LineCharge.Rate = EmployeeCharge.Rate;
                        LineCharge.Amount = (EmployeeCharge.Amount * Line.Days/30);
                        LineCharge.DealQty = 0;
                        LineCharge.IsVisible = EmployeeCharge.IsVisible;
                        LineCharge.IncludedCharges = EmployeeCharge.IncludedCharges;
                        LineCharge.IncludedChargesCalculation = EmployeeCharge.IncludedChargesCalculation;
                        LineCharge.IsVisibleLedgerAccountDr = false;
                        LineCharge.filterLedgerAccountGroupsDrId = null;
                        LineCharge.filterLedgerAccountGroupsCrId = null;
                        LineCharge.OMSId = EmployeeCharge.OMSId;
                        LineCharge.ObjectState = Model.ObjectState.Added;
                        db.SalaryLineCharge.Add(LineCharge);


                        Charge NetSalaryCharge = (from C in db.Charge where C.ChargeName == "Net Salary" select C).FirstOrDefault();
                        if (NetSalaryCharge != null)
                            Line.NetSalary = (LineCharge.Amount ?? 0) + (Line.OtherAddition ?? 0) - (Line.OtherDeduction ?? 0) - (Line.LoanEMI ?? 0);
                    }


                     
                    db.SalaryLine.Add(Line);
                }


                try
                {
                    db.SaveChanges();
                }

                catch (Exception ex)
                {
                    string message = _exception.HandleException(ex);
                    TempData["CSEXC"] += message;
                }



                int SalaryAccountId = 0;
                LedgerAccount SalaryAccount = new LedgerAccountService(_unitOfWork).Find("Salary Account");
                if (SalaryAccount != null)
                    SalaryAccountId = SalaryAccount.LedgerAccountId;

                LedgerHeader LedgerHeader = new LedgerHeader();
                LedgerHeader.DocNo = Header.DocNo;
                LedgerHeader.DocTypeId = Header.DocTypeId;
                LedgerHeader.DocDate = Header.DocDate;
                LedgerHeader.DivisionId = Header.DivisionId;
                LedgerHeader.SiteId = Header.SiteId;
                LedgerHeader.PaymentFor = null;
                LedgerHeader.AdjustmentType = null;
                LedgerHeader.ProcessId = null;
                LedgerHeader.GodownId = null;
                LedgerHeader.LedgerAccountId = SalaryAccountId;
                LedgerHeader.DrCr = null;
                LedgerHeader.PartyDocNo = null;
                LedgerHeader.PartyDocDate = null;
                LedgerHeader.Narration = Header.Remark;
                LedgerHeader.ReferenceDocId = Header.SalaryHeaderId;
                LedgerHeader.ReferenceDocTypeId = Header.DocTypeId;
                LedgerHeader.CreatedDate = DateTime.Now;
                LedgerHeader.CreatedBy = User.Identity.Name;
                LedgerHeader.ModifiedDate = DateTime.Now;
                LedgerHeader.ModifiedBy = User.Identity.Name;
                LedgerHeader.ObjectState = Model.ObjectState.Added;
                db.LedgerHeader.Add(LedgerHeader);


                var LedgerLineList_Temp = (from L in db.SalaryLine
                                           join E in db.Employee on L.EmployeeId equals E.EmployeeId into EmployeeTable
                                           from EmployeeTab in EmployeeTable.DefaultIfEmpty()
                                           join A in db.LedgerAccount on EmployeeTab.PersonID equals A.PersonId into LedgerAccountTable
                                           from LedgerAccountTab in LedgerAccountTable.DefaultIfEmpty()
                                           where L.SalaryHeaderId == Header.SalaryHeaderId
                                           select new
                                           {
                                               SalaryLineId = L.SalaryLineId,
                                               DocTypeId = L.SalaryHeader.DocTypeId,
                                               LedgerAccountId = LedgerAccountTab.LedgerAccountId,
                                               NetSalary = L.NetSalary
                                           }).ToList();

                int LedgerLineId_Running = 0, LedgerId_Running = 0;
                foreach (var LedgerLine_Temp in LedgerLineList_Temp)
                {
                    LedgerLine LedgerLine = new LedgerLine();
                    LedgerLine.LedgerLineId = LedgerLineId_Running -- ;
                    LedgerLine.LedgerHeaderId = LedgerHeader.LedgerHeaderId;
                    LedgerLine.LedgerAccountId = LedgerLine_Temp.LedgerAccountId;
                    LedgerLine.Amount = LedgerLine_Temp.NetSalary;
                    LedgerLine.ChqNo = null;
                    LedgerLine.ChqDate = null;
                    LedgerLine.CostCenterId = null;
                    LedgerLine.BaseRate = 0;
                    LedgerLine.BaseValue = 0;
                    LedgerLine.ReferenceId = null;
                    LedgerLine.ProductUidId = null;
                    LedgerLine.ReferenceDocTypeId = LedgerLine_Temp.DocTypeId;
                    LedgerLine.ReferenceDocId = null;
                    LedgerLine.ReferenceDocLineId = LedgerLine_Temp.SalaryLineId;
                    LedgerLine.DrCr = NatureConstants.Credit;
                    LedgerLine.CreatedDate = DateTime.Now;
                    LedgerLine.ModifiedDate = DateTime.Now;
                    LedgerLine.CreatedBy = User.Identity.Name;
                    LedgerLine.Remark = null;
                    LedgerLine.ModifiedBy = User.Identity.Name;
                    LedgerLine.ObjectState = Model.ObjectState.Added;
                    db.LedgerLine.Add(LedgerLine);



                    #region LedgerSave
                    Ledger Ledger = new Ledger();
                    Ledger.LedgerId = LedgerId_Running--;
                    Ledger.AmtDr = 0;
                    Ledger.AmtCr = LedgerLine.Amount;
                    Ledger.ChqNo = LedgerLine.ChqNo;
                    Ledger.ChqDate = LedgerLine.ChqDate;
                    Ledger.ContraLedgerAccountId = LedgerHeader.LedgerAccountId;
                    Ledger.CostCenterId = LedgerLine.CostCenterId;
                    Ledger.DueDate = LedgerLine.ChqDate;
                    Ledger.LedgerAccountId = LedgerLine.LedgerAccountId;
                    Ledger.LedgerHeaderId = LedgerLine.LedgerHeaderId;
                    Ledger.LedgerLineId = LedgerLine.LedgerLineId;
                    Ledger.ProductUidId = LedgerLine.ProductUidId;
                    Ledger.Narration = LedgerHeader.Narration + LedgerLine.Remark;
                    Ledger.ObjectState = Model.ObjectState.Added;
                    db.Ledger.Add(Ledger);
                    #endregion


                    #region ContraLedgerSave
                    Ledger ContraLedger = new Ledger();
                    ContraLedger.LedgerId = LedgerId_Running--;
                    ContraLedger.AmtDr = LedgerLine.Amount;
                    ContraLedger.AmtCr = 0;
                    ContraLedger.LedgerHeaderId = LedgerHeader.LedgerHeaderId;
                    ContraLedger.CostCenterId = LedgerHeader.CostCenterId;
                    ContraLedger.LedgerLineId = LedgerLine.LedgerLineId;
                    ContraLedger.LedgerAccountId = LedgerHeader.LedgerAccountId.Value;
                    ContraLedger.ContraLedgerAccountId = LedgerLine.LedgerAccountId;
                    ContraLedger.ChqNo = LedgerLine.ChqNo;
                    ContraLedger.ChqDate = LedgerLine.ChqDate;
                    ContraLedger.ObjectState = Model.ObjectState.Added;
                    db.Ledger.Add(ContraLedger);
                    #endregion
                }


                try
                {
                    db.SaveChanges();
                }

                catch (Exception ex)
                {
                    string message = _exception.HandleException(ex);
                    TempData["CSEXC"] += message;
                }

                //return Json(new { success = true, Url = "/LedgerHeader/Submit/" + LedgerHeader.LedgerHeaderId });
                return (string)System.Configuration.ConfigurationManager.AppSettings["JobsDomain"] + "/LedgerHeader/Detail/" + LedgerHeader.LedgerHeaderId;
            }
            return null;
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
