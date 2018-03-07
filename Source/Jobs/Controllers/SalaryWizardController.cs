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

            List<SelectListItem> WagesPayTypeList = new List<SelectListItem>();
            WagesPayTypeList.Add(new SelectListItem { Text = "Daily", Value = "Daily" });
            WagesPayTypeList.Add(new SelectListItem { Text = "Monthly", Value = "Monthly" });
            ViewBag.WagesPayTypeList = WagesPayTypeList;

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
                string Remark = SalaryDataList.FirstOrDefault().HeaderRemark;

                SalaryHeader Header = new SalaryHeader();
                Header.DocDate = DocDate;
                Header.DocTypeId = DocTypeId;
                Header.DocNo = DocNo;
                Header.DivisionId = DivisionId;
                Header.SiteId = SiteId;
                Header.Remark = Remark;
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
                    Line.Advance = SalaryData.Advance;
                    Line.NetSalary = 0;
                    Line.CreatedDate = DateTime.Now;
                    Line.CreatedBy = User.Identity.Name;
                    Line.ModifiedDate = DateTime.Now;
                    Line.ModifiedBy = User.Identity.Name;
                    Line.ObjectState = Model.ObjectState.Added;
                    

                    var EmployeeChargesList = (from H in db.EmployeeCharge where H.HeaderTableId == Line.EmployeeId select H).OrderBy(m=>m.Sr).ToList();
                    string WagesPayType = (from E in db.Employee where E.EmployeeId == Line.EmployeeId select E).FirstOrDefault().WagesPayType;

                    Decimal TotalAmount = 0;
                    int j = 0, Sr = 0;
                    foreach(var EmployeeCharge in EmployeeChargesList)
                    {
                        SalaryLineCharge LineCharge = new SalaryLineCharge();
                        LineCharge.Id = j--;
                        LineCharge.HeaderTableId = Header.SalaryHeaderId;
                        LineCharge.LineTableId = Line.SalaryLineId;
                        LineCharge.Sr = Sr ++;
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

                        string ChargeName = db.Charge.Find(EmployeeCharge.ChargeId).ChargeName;

                        if (ChargeName == "Basic Salary")
                        {
                            if (WagesPayType == "Daily")
                                LineCharge.Amount = EmployeeCharge.Amount * Line.Days;
                            else
                                LineCharge.Amount = (EmployeeCharge.Amount * Line.Days / SalaryData.MonthDays);
                        }
                        else if (ChargeName == "Net Salary")
                        {
                            LineCharge.Amount = TotalAmount;
                        }
                        else
                        {
                            LineCharge.Amount = EmployeeCharge.Amount;
                        }
                        if (LineCharge.AddDeduct==0)
                            TotalAmount = TotalAmount - LineCharge.Amount ?? 0;
                        else if (LineCharge.AddDeduct == 1)
                            TotalAmount = TotalAmount + LineCharge.Amount ?? 0;


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
                        if (NetSalaryCharge != null && LineCharge.ChargeId == NetSalaryCharge.ChargeId)
                            Line.NetSalary = (LineCharge.Amount ?? 0) + (Line.OtherAddition ?? 0) - (Line.OtherDeduction ?? 0) - (Line.LoanEMI ?? 0) - (Line.Advance ?? 0);



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
                                           where L.SalaryHeaderId == Header.SalaryHeaderId && L.NetSalary > 0
                                           select new
                                           {
                                               SalaryLineId = L.SalaryLineId,
                                               DocTypeId = L.SalaryHeader.DocTypeId,
                                               LedgerAccountId = LedgerAccountTab.LedgerAccountId,
                                               LoanEMI = L.LoanEMI,
                                               Advance = L.Advance,
                                               NetSalary = L.NetSalary,
                                           }).ToList();

                int LedgerId_Running = 0;
                foreach (var LedgerLine_Temp in LedgerLineList_Temp)
                {
                    #region LedgerSave
                    Ledger Ledger = new Ledger();
                    Ledger.LedgerId = LedgerId_Running--;
                    Ledger.AmtDr = 0;
                    Ledger.AmtCr = LedgerLine_Temp.NetSalary;
                    Ledger.ChqNo = null;
                    Ledger.ChqDate = null;
                    Ledger.ContraLedgerAccountId = LedgerHeader.LedgerAccountId;
                    Ledger.CostCenterId = null;
                    Ledger.DueDate = null;
                    Ledger.LedgerAccountId = LedgerLine_Temp.LedgerAccountId;
                    Ledger.LedgerHeaderId = LedgerHeader.LedgerHeaderId;
                    Ledger.LedgerLineId = null;
                    Ledger.ProductUidId = null;
                    Ledger.Narration = LedgerHeader.Narration;
                    Ledger.ObjectState = Model.ObjectState.Added;
                    db.Ledger.Add(Ledger);
                    #endregion


                    if ((LedgerLine_Temp.LoanEMI ?? 0) > 0)
                    {
                        int LoanAdjustementAccountId = 0;
                        LedgerAccount LoanAdjustementAccount = new LedgerAccountService(_unitOfWork).Find("Loan Adjustement A/C");
                        if (LoanAdjustementAccount != null)
                            LoanAdjustementAccountId = LoanAdjustementAccount.LedgerAccountId;

                        Decimal LoanEMI_RunningBalance = LedgerLine_Temp.LoanEMI ?? 0;
                        IEnumerable<LoanLedgerIdList> LoanList = _SalaryWizardService.GetLoanList(LedgerLine_Temp.LedgerAccountId);
                        foreach (var LoanLedger in LoanList)
                        {
                            if (LoanEMI_RunningBalance > 0)
                            {
                                LedgerAdj LedgerAdj = new LedgerAdj();
                                LedgerAdj.CrLedgerId = Ledger.LedgerId;
                                LedgerAdj.DrLedgerId = LoanLedger.LedgerId;

                                if (LoanLedger.Amount >= LoanEMI_RunningBalance)
                                    LedgerAdj.Amount = LoanEMI_RunningBalance;
                                else
                                    LedgerAdj.Amount = LoanLedger.Amount;

                                LedgerAdj.SiteId = LedgerHeader.SiteId;
                                LedgerAdj.CreatedDate = DateTime.Now;
                                LedgerAdj.ModifiedDate = DateTime.Now;
                                LedgerAdj.CreatedBy = User.Identity.Name;
                                LedgerAdj.ModifiedBy = User.Identity.Name;
                                LedgerAdj.ObjectState = Model.ObjectState.Added;
                                db.LedgerAdj.Add(LedgerAdj);

                                LoanEMI_RunningBalance = LoanEMI_RunningBalance - LedgerAdj.Amount;
                            }
                        }

                        #region LedgerSave
                        Ledger Ledger1 = new Ledger();
                        Ledger1.LedgerId = LedgerId_Running--;
                        Ledger1.AmtDr = 0;
                        Ledger1.AmtCr = (decimal)LedgerLine_Temp.LoanEMI;
                        Ledger1.ChqNo = null;
                        Ledger1.ChqDate = null;
                        Ledger1.ContraLedgerAccountId = LoanAdjustementAccountId;
                        Ledger1.CostCenterId = null;
                        Ledger1.DueDate = null;
                        Ledger1.LedgerAccountId = LedgerLine_Temp.LedgerAccountId;
                        Ledger1.LedgerHeaderId = LedgerHeader.LedgerHeaderId;
                        Ledger1.LedgerLineId = null;
                        Ledger1.ProductUidId = null;
                        Ledger1.Narration = LedgerHeader.Narration;
                        Ledger1.ObjectState = Model.ObjectState.Added;
                        db.Ledger.Add(Ledger1);
                        #endregion

                        #region ContraLedgerSave
                        Ledger ContraLedger1 = new Ledger();
                        ContraLedger1.LedgerId = LedgerId_Running--;
                        ContraLedger1.AmtDr = (decimal)LedgerLine_Temp.LoanEMI;
                        ContraLedger1.AmtCr = 0;
                        ContraLedger1.LedgerHeaderId = LedgerHeader.LedgerHeaderId;
                        ContraLedger1.CostCenterId = LedgerHeader.CostCenterId;
                        ContraLedger1.LedgerLineId = null;
                        ContraLedger1.LedgerAccountId = LoanAdjustementAccountId;
                        ContraLedger1.ContraLedgerAccountId = LedgerLine_Temp.LedgerAccountId;
                        ContraLedger1.ChqNo = null;
                        ContraLedger1.ChqDate = null;
                        ContraLedger1.ObjectState = Model.ObjectState.Added;
                        db.Ledger.Add(ContraLedger1);
                        #endregion
                    }


                    if ((LedgerLine_Temp.Advance ?? 0) > 0)
                    {
                        int AdvanceAdjustementAccountId = 0;
                        LedgerAccount AdvanceAdjustementAccount = new LedgerAccountService(_unitOfWork).Find("Advance Adjustement A/C");
                        if (AdvanceAdjustementAccount != null)
                            AdvanceAdjustementAccountId = AdvanceAdjustementAccount.LedgerAccountId;

                        Decimal Advance_RunningBalance = LedgerLine_Temp.Advance ?? 0;
                        IEnumerable<AdvanceLedgerIdList> AdvanceList = _SalaryWizardService.GetAdvanceList(LedgerLine_Temp.LedgerAccountId);
                        foreach (var AdvanceLedger in AdvanceList)
                        {
                            if (Advance_RunningBalance > 0)
                            {
                                LedgerAdj LedgerAdj = new LedgerAdj();
                                LedgerAdj.CrLedgerId = Ledger.LedgerId;
                                LedgerAdj.DrLedgerId = AdvanceLedger.LedgerId;

                                if (AdvanceLedger.Amount >= Advance_RunningBalance)
                                    LedgerAdj.Amount = Advance_RunningBalance;
                                else
                                    LedgerAdj.Amount = AdvanceLedger.Amount;

                                LedgerAdj.SiteId = LedgerHeader.SiteId;
                                LedgerAdj.CreatedDate = DateTime.Now;
                                LedgerAdj.ModifiedDate = DateTime.Now;
                                LedgerAdj.CreatedBy = User.Identity.Name;
                                LedgerAdj.ModifiedBy = User.Identity.Name;
                                LedgerAdj.ObjectState = Model.ObjectState.Added;
                                db.LedgerAdj.Add(LedgerAdj);

                                Advance_RunningBalance = Advance_RunningBalance - LedgerAdj.Amount;
                            }
                        }

                        #region LedgerSave
                        Ledger Ledger1 = new Ledger();
                        Ledger1.LedgerId = LedgerId_Running--;
                        Ledger1.AmtDr = 0;
                        Ledger1.AmtCr = (decimal)LedgerLine_Temp.Advance;
                        Ledger1.ChqNo = null;
                        Ledger1.ChqDate = null;
                        Ledger1.ContraLedgerAccountId = AdvanceAdjustementAccountId;
                        Ledger1.CostCenterId = null;
                        Ledger1.DueDate = null;
                        Ledger1.LedgerAccountId = LedgerLine_Temp.LedgerAccountId;
                        Ledger1.LedgerHeaderId = LedgerHeader.LedgerHeaderId;
                        Ledger1.LedgerLineId = null;
                        Ledger1.ProductUidId = null;
                        Ledger1.Narration = LedgerHeader.Narration;
                        Ledger1.ObjectState = Model.ObjectState.Added;
                        db.Ledger.Add(Ledger1);
                        #endregion

                        #region ContraLedgerSave
                        Ledger ContraLedger1 = new Ledger();
                        ContraLedger1.LedgerId = LedgerId_Running--;
                        ContraLedger1.AmtDr = (decimal)LedgerLine_Temp.Advance;
                        ContraLedger1.AmtCr = 0;
                        ContraLedger1.LedgerHeaderId = LedgerHeader.LedgerHeaderId;
                        ContraLedger1.CostCenterId = LedgerHeader.CostCenterId;
                        ContraLedger1.LedgerLineId = null;
                        ContraLedger1.LedgerAccountId = AdvanceAdjustementAccountId;
                        ContraLedger1.ContraLedgerAccountId = LedgerLine_Temp.LedgerAccountId;
                        ContraLedger1.ChqNo = null;
                        ContraLedger1.ChqDate = null;
                        ContraLedger1.ObjectState = Model.ObjectState.Added;
                        db.Ledger.Add(ContraLedger1);
                        #endregion
                    }

                    #region ContraLedgerSave
                    Ledger ContraLedger = new Ledger();
                    ContraLedger.LedgerId = LedgerId_Running--;
                    ContraLedger.AmtDr = LedgerLine_Temp.NetSalary;
                    ContraLedger.AmtCr = 0;
                    ContraLedger.LedgerHeaderId = LedgerHeader.LedgerHeaderId;
                    ContraLedger.CostCenterId = LedgerHeader.CostCenterId;
                    ContraLedger.LedgerLineId = null;
                    ContraLedger.LedgerAccountId = LedgerHeader.LedgerAccountId.Value;
                    ContraLedger.ContraLedgerAccountId = LedgerLine_Temp.LedgerAccountId;
                    ContraLedger.ChqNo = null;
                    ContraLedger.ChqDate = null;
                    ContraLedger.ObjectState = Model.ObjectState.Added;
                    db.Ledger.Add(ContraLedger);
                    #endregion
                }


                #region "Calculation Ledger Posting"
                int PartyAccountId = 0;
                LedgerAccount PartyAccount = new LedgerAccountService(_unitOfWork).Find("|PARTY|");
                if (PartyAccount != null)
                    PartyAccountId = PartyAccount.LedgerAccountId;

                var LineChargeList = (from Lc in db.SalaryLineCharge
                                      where Lc.HeaderTableId == Header.SalaryHeaderId
                                        && Lc.Amount > 0 && Lc.LedgerAccountCrId != null && Lc.LedgerAccountDrId != null
                                      select Lc).ToList();

                foreach(var LineCharge in LineChargeList)
                {
                    int EmployeeLedgerAccountId = (from A in db.LedgerAccount where A.PersonId == LineCharge.PersonID select A).FirstOrDefault().LedgerAccountId;

                    Ledger Ledger = new Ledger();
                    Ledger.LedgerId = LedgerId_Running--;
                    Ledger.AmtDr = 0;
                    Ledger.AmtCr = LineCharge.Amount ?? 0;
                    Ledger.ChqNo = null;
                    Ledger.ChqDate = null;
                    Ledger.LedgerAccountId = LineCharge.LedgerAccountCrId == PartyAccountId ?  EmployeeLedgerAccountId : (int)LineCharge.LedgerAccountCrId;
                    Ledger.ContraLedgerAccountId = LineCharge.LedgerAccountDrId == PartyAccountId ? EmployeeLedgerAccountId : (int)LineCharge.LedgerAccountDrId;
                    Ledger.CostCenterId = null;
                    Ledger.DueDate = null;
                    Ledger.LedgerHeaderId = LedgerHeader.LedgerHeaderId;
                    Ledger.LedgerLineId = null;
                    Ledger.ProductUidId = null;
                    Ledger.Narration = LedgerHeader.Narration;
                    Ledger.ObjectState = Model.ObjectState.Added;
                    db.Ledger.Add(Ledger);

                    Ledger ContraLedger = new Ledger();
                    ContraLedger.LedgerId = LedgerId_Running--;
                    ContraLedger.AmtDr = LineCharge.Amount ?? 0;
                    ContraLedger.AmtCr = 0;
                    ContraLedger.ChqNo = null;
                    ContraLedger.ChqDate = null;
                    ContraLedger.LedgerAccountId = LineCharge.LedgerAccountDrId == PartyAccountId ? EmployeeLedgerAccountId : (int)LineCharge.LedgerAccountDrId; 
                    ContraLedger.ContraLedgerAccountId = LineCharge.LedgerAccountCrId == PartyAccountId ? EmployeeLedgerAccountId : (int)LineCharge.LedgerAccountCrId;
                    ContraLedger.CostCenterId = null;
                    ContraLedger.DueDate = null;
                    ContraLedger.LedgerHeaderId = LedgerHeader.LedgerHeaderId;
                    ContraLedger.LedgerLineId = null;
                    ContraLedger.ProductUidId = null;
                    ContraLedger.Narration = LedgerHeader.Narration;
                    ContraLedger.ObjectState = Model.ObjectState.Added;
                    db.Ledger.Add(Ledger);
                }
                #endregion


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
                return (string)System.Configuration.ConfigurationManager.AppSettings["JobsDomain"] + "/SalaryHeader/Modify/" + Header.SalaryHeaderId;
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
