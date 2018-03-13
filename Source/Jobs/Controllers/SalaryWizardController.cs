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
        public ActionResult SalaryWizard(int id, int? SalaryHeaderId)
        {
            SalaryWizardViewModel vm = new SalaryWizardViewModel();
            vm.DocDate = DateTime.Now;
            vm.DocTypeId = id;

            List<SelectListItem> WagesPayTypeList = new List<SelectListItem>();
            WagesPayTypeList.Add(new SelectListItem { Text = "Daily", Value = "Daily" });
            WagesPayTypeList.Add(new SelectListItem { Text = "Monthly", Value = "Monthly" });
            ViewBag.WagesPayTypeList = WagesPayTypeList;

            vm.SalaryHeaderId = SalaryHeaderId ?? 0;
            if (vm.SalaryHeaderId != 0)
            {
                SalaryHeader Header = db.SalaryHeader.Find(vm.SalaryHeaderId);
                vm.DocDate = Header.DocDate;
                vm.Remark = Header.Remark;
            }




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
                if (SalaryDataList.FirstOrDefault().SalaryHeaderId != null && SalaryDataList.FirstOrDefault().SalaryHeaderId > 0)
                {
                    var HeaderRecord = db.SalaryHeader.Find(SalaryDataList.FirstOrDefault().SalaryHeaderId);

                    var LineRecords = (from L in db.SalaryLine where L.SalaryHeaderId == HeaderRecord.SalaryHeaderId select L).ToList();
                    var LineChargeRecords = (from Lc in db.SalaryLineCharge where Lc.HeaderTableId == HeaderRecord.SalaryHeaderId select Lc).ToList();
                    var HeaderChargeRecords = (from Hc in db.SalaryHeaderCharge where Hc.HeaderTableId == HeaderRecord.SalaryHeaderId select Hc).ToList();

                    foreach (var item in LineChargeRecords)
                    {
                        item.ObjectState = Model.ObjectState.Deleted;
                        db.SalaryLineCharge.Remove(item);
                    }

                    foreach (var item in LineRecords)
                    {
                        item.ObjectState = Model.ObjectState.Deleted;
                        db.SalaryLine.Remove(item);
                    }

                    foreach (var item in HeaderChargeRecords)
                    {
                        item.ObjectState = Model.ObjectState.Deleted;
                        db.SalaryHeaderCharge.Remove(item);
                    }

                    HeaderRecord.ObjectState = Model.ObjectState.Deleted;
                    db.SalaryHeader.Remove(HeaderRecord);
                }

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
                            Line.NetPayable = (LineCharge.Amount ?? 0) + (Line.OtherAddition ?? 0) - (Line.OtherDeduction ?? 0) - (Line.LoanEMI ?? 0) - (Line.Advance ?? 0);
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


                //return Json(new { success = true, Url = "/LedgerHeader/Submit/" + LedgerHeader.LedgerHeaderId });
                //return Json(new { success = true, Url = "/SalaryHeader/Submit/" + Header.SalaryHeaderId });
                return (string)System.Configuration.ConfigurationManager.AppSettings["JobsDomain"] + "/SalaryHeader/Submit/" + Header.SalaryHeaderId;
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
