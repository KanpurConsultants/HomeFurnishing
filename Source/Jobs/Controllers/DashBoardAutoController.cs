using System.Collections.Generic;
using System.Web.Mvc;
using Service;

//using ProjLib.ViewModels;
using System.Data.SqlClient;
using System.Data;
using System;

namespace Module
{
    [Authorize]
    public class DashBoardAutoController : Controller
    {
        IDashBoardAutoService _DashBoardAutoService;
        public DashBoardAutoController(IDashBoardAutoService DashBoardAutoService)
        {
            _DashBoardAutoService = DashBoardAutoService;
        }

        public ActionResult DashBoardAuto()
        {
            return View();
        }
        public JsonResult GetVehicleSale()
        {
            IEnumerable<DashBoardSingleValue> VehicleSale = _DashBoardAutoService.GetVehicleSale();

            JsonResult json = Json(new { Success = true, Data = VehicleSale }, JsonRequestBehavior.AllowGet);
            return json;
        }
        public JsonResult GetVehicleProfit()
        {
            IEnumerable<DashBoardSingleValue> VehicleProfit = _DashBoardAutoService.GetVehicleProfit();

            JsonResult json = Json(new { Success = true, Data = VehicleProfit }, JsonRequestBehavior.AllowGet);
            return json;
        }
        public JsonResult GetVehicleStock()
        {
            IEnumerable<DashBoardSingleValue> VehicleStock = _DashBoardAutoService.GetVehicleStock();

            JsonResult json = Json(new { Success = true, Data = VehicleStock }, JsonRequestBehavior.AllowGet);
            return json;
        }



        public JsonResult GetExpense()
        {
            IEnumerable<DashBoardSingleValue> Expense = _DashBoardAutoService.GetExpense();

            JsonResult json = Json(new { Success = true, Data = Expense }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetDebtors()
        {
            IEnumerable<DashBoardSingleValue> Debtors = _DashBoardAutoService.GetDebtors();

            JsonResult json = Json(new { Success = true, Data = Debtors }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetCreditors()
        {
            IEnumerable<DashBoardSingleValue> Creditors = _DashBoardAutoService.GetCreditors();

            JsonResult json = Json(new { Success = true, Data = Creditors }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetBankBalance()
        {
            IEnumerable<DashBoardSingleValue> BankBalance = _DashBoardAutoService.GetBankBalance();

            JsonResult json = Json(new { Success = true, Data = BankBalance }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetCashBalance()
        {
            IEnumerable<DashBoardSingleValue> CashBalance = _DashBoardAutoService.GetCashBalance();

            JsonResult json = Json(new { Success = true, Data = CashBalance }, JsonRequestBehavior.AllowGet);
            return json;
        }








        public JsonResult GetVehicleSalePieChartData()
        {
            IEnumerable<DashBoardPieChartData> VehiclePieChartData = _DashBoardAutoService.GetVehicleSalePieChartData();

            JsonResult json = Json(new { Success = true, Data = VehiclePieChartData }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetVehicleSaleChartData()
        {
            IEnumerable<DashBoardSaleBarChartData> VehicleSaleChartData = _DashBoardAutoService.GetVehicleSaleBarChartData();

            JsonResult json = Json(new { Success = true, Data = VehicleSaleChartData }, JsonRequestBehavior.AllowGet);
            return json;
        }
        public JsonResult GetSpareSalePieChartData()
        {
            IEnumerable<DashBoardPieChartData> SparePieChartData = _DashBoardAutoService.GetSpareSalePieChartData();

            JsonResult json = Json(new { Success = true, Data = SparePieChartData }, JsonRequestBehavior.AllowGet);
            return json;
        }
        public JsonResult GetSpareSaleChartData()
        {
            IEnumerable<DashBoardSaleBarChartData> SpareSaleChartData = _DashBoardAutoService.GetSpareSaleBarChartData();

            JsonResult json = Json(new { Success = true, Data = SpareSaleChartData }, JsonRequestBehavior.AllowGet);
            return json;
        }




        public JsonResult GetVehicleSaleDetailFinancierWise()
        {
            IEnumerable<DashBoardTabularData> DashBoardSaleDetailFinancierWise = _DashBoardAutoService.GetVehicleSaleDetailFinancierWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardSaleDetailFinancierWise }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetVehicleSaleDetailSalesManWise()
        {
            IEnumerable<DashBoardTabularData> DashBoardSaleDetailSalesManWise = _DashBoardAutoService.GetVehicleSaleDetailSalesManWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardSaleDetailSalesManWise }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetVehicleSaleDetailProductTypeWise()
        {
            IEnumerable<DashBoardTabularData> DashBoardSaleDetailProductTypeWise = _DashBoardAutoService.GetVehicleSaleDetailProductTypeWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardSaleDetailProductTypeWise }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetVehicleProfitDetail()
        {
            IEnumerable<DashBoardTabularData> DashBoardTabularData = _DashBoardAutoService.GetVehicleProfitDetail();

            JsonResult json = Json(new { Success = true, Data = DashBoardTabularData }, JsonRequestBehavior.AllowGet);
            return json;
        }
        public JsonResult GetDebtorsDetail()
        {
            IEnumerable<DashBoardTabularData> DashBoardTabularData = _DashBoardAutoService.GetDebtorsDetail();

            JsonResult json = Json(new { Success = true, Data = DashBoardTabularData }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetBankBalanceDetail()
        {
            IEnumerable<DashBoardTabularData> DashBoardTabularData = _DashBoardAutoService.GetBankBalanceDetail();

            JsonResult json = Json(new { Success = true, Data = DashBoardTabularData }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetVehicleStockDetail()
        {
            IEnumerable<DashBoardTabularData> DashBoardTabularData = _DashBoardAutoService.GetVehicleStockDetail();

            JsonResult json = Json(new { Success = true, Data = DashBoardTabularData }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetExpenseDetail()
        {
            IEnumerable<DashBoardTabularData> DashBoardTabularData = _DashBoardAutoService.GetExpenseDetail();

            JsonResult json = Json(new { Success = true, Data = DashBoardTabularData }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetCreditorsDetail()
        {
            IEnumerable<DashBoardTabularData> DashBoardTabularData = _DashBoardAutoService.GetCreditorsDetail();

            JsonResult json = Json(new { Success = true, Data = DashBoardTabularData }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetCashBalanceDetail()
        {
            IEnumerable<DashBoardTabularData> DashBoardTabularData = _DashBoardAutoService.GetCashBalanceDetail();

            JsonResult json = Json(new { Success = true, Data = DashBoardTabularData }, JsonRequestBehavior.AllowGet);
            return json;
        }
    }   
}