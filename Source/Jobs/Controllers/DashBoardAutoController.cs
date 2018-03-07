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
            IEnumerable<DashBoardSale> VehicleSale = _DashBoardAutoService.GetVehicleSale();

            JsonResult json = Json(new { Success = true, Data = VehicleSale }, JsonRequestBehavior.AllowGet);
            return json;
        }
        public JsonResult GetVehicleProfit()
        {
            IEnumerable<DashBoardProfit> VehicleProfit = _DashBoardAutoService.GetVehicleProfit();

            JsonResult json = Json(new { Success = true, Data = VehicleProfit }, JsonRequestBehavior.AllowGet);
            return json;
        }
        public JsonResult GetVehicleStock()
        {
            IEnumerable<DashBoardStock> VehicleStock = _DashBoardAutoService.GetVehicleStock();

            JsonResult json = Json(new { Success = true, Data = VehicleStock }, JsonRequestBehavior.AllowGet);
            return json;
        }



        public JsonResult GetExpense()
        {
            IEnumerable<DashBoardExpense> Expense = _DashBoardAutoService.GetExpense();

            JsonResult json = Json(new { Success = true, Data = Expense }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetDebtors()
        {
            IEnumerable<DashBoardDebtors> Debtors = _DashBoardAutoService.GetDebtors();

            JsonResult json = Json(new { Success = true, Data = Debtors }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetCreditors()
        {
            IEnumerable<DashBoardCreditors> Creditors = _DashBoardAutoService.GetCreditors();

            JsonResult json = Json(new { Success = true, Data = Creditors }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetBankBalance()
        {
            IEnumerable<DashBoardBankBalance> BankBalance = _DashBoardAutoService.GetBankBalance();

            JsonResult json = Json(new { Success = true, Data = BankBalance }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetCashBalance()
        {
            IEnumerable<DashBoardCashBalance> CashBalance = _DashBoardAutoService.GetCashBalance();

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
            IEnumerable<DashBoardSaleDetailFinancierWise> DashBoardSaleDetailFinancierWise = _DashBoardAutoService.GetVehicleSaleDetailFinancierWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardSaleDetailFinancierWise }, JsonRequestBehavior.AllowGet);
            return json;
        }
    }   
}