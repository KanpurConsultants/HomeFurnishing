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
            IEnumerable<DashBoardDoubleValue> VehicleSale = _DashBoardAutoService.GetVehicleSale();

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


        public JsonResult GetVehiclePurchase()
        {
            IEnumerable<DashBoardSingleValue> VehiclePurchase = _DashBoardAutoService.GetVehiclePurchase();

            JsonResult json = Json(new { Success = true, Data = VehiclePurchase }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetVehiclePurchaseOrder()
        {
            IEnumerable<DashBoardSingleValue> VehiclePurchaseOrder = _DashBoardAutoService.GetVehiclePurchaseOrder();

            JsonResult json = Json(new { Success = true, Data = VehiclePurchaseOrder }, JsonRequestBehavior.AllowGet);
            return json;
        }



        public JsonResult GetWorkshopSale()
        {
            IEnumerable<DashBoardDoubleValue> WorkshopSale = _DashBoardAutoService.GetWorkshopSale();

            JsonResult json = Json(new { Success = true, Data = WorkshopSale }, JsonRequestBehavior.AllowGet);
            return json;
        }
        public JsonResult GetSpareSale()
        {
            IEnumerable<DashBoardDoubleValue> SpareSale = _DashBoardAutoService.GetSpareSale();

            JsonResult json = Json(new { Success = true, Data = SpareSale }, JsonRequestBehavior.AllowGet);
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




        public JsonResult GetVehicleSaleDetailSalesManWise()
        {
            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardSaleDetailSalesManWise = _DashBoardAutoService.GetVehicleSaleDetailSalesManWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardSaleDetailSalesManWise }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetVehicleSaleDetailProductTypeWise()
        {
            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardSaleDetailProductTypeWise = _DashBoardAutoService.GetVehicleSaleDetailProductTypeWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardSaleDetailProductTypeWise }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetVehicleSaleDetailProductGroupWise()
        {
            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardSaleDetailProductGroupWise = _DashBoardAutoService.GetVehicleSaleDetailProductGroupWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardSaleDetailProductGroupWise }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetVehicleProfitDetailProductGroupWise()
        {
            IEnumerable<DashBoardTabularData> DashBoardTabularData = _DashBoardAutoService.GetVehicleProfitDetailProductGroupWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardTabularData }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetVehicleProfitDetailSalesManWise()
        {
            IEnumerable<DashBoardTabularData> DashBoardTabularData = _DashBoardAutoService.GetVehicleProfitDetailSalesManWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardTabularData }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetVehicleProfitDetailBranchWise()
        {
            IEnumerable<DashBoardTabularData> DashBoardTabularData = _DashBoardAutoService.GetVehicleProfitDetailBranchWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardTabularData }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetDebtorsDetail()
        {
            IEnumerable<DashBoardTabularData> DashBoardTabularData = _DashBoardAutoService.GetDebtorsDetail();

            JsonResult json = Json(new { Success = true, Data = DashBoardTabularData }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetBankBalanceDetailBankAc()
        {
            IEnumerable<DashBoardTabularData> DashBoardTabularData = _DashBoardAutoService.GetBankBalanceDetailBankAc();

            JsonResult json = Json(new { Success = true, Data = DashBoardTabularData }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetBankBalanceDetailBankODAc()
        {
            IEnumerable<DashBoardTabularData> DashBoardTabularData = _DashBoardAutoService.GetBankBalanceDetailBankODAc();

            JsonResult json = Json(new { Success = true, Data = DashBoardTabularData }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetBankBalanceDetailChannelFinanceAc()
        {
            IEnumerable<DashBoardTabularData> DashBoardTabularData = _DashBoardAutoService.GetBankBalanceDetailChannelFinanceAc();

            JsonResult json = Json(new { Success = true, Data = DashBoardTabularData }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetVehicleStockDetailProductTypeWise()
        {
            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardTabularData = _DashBoardAutoService.GetVehicleStockDetailProductTypeWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardTabularData }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetVehicleStockDetailProductGroupWise()
        {
            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardTabularData = _DashBoardAutoService.GetVehicleStockDetailProductGroupWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardTabularData }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetExpenseDetailLedgerAccountWise()
        {
            IEnumerable<DashBoardTabularData> DashBoardTabularData = _DashBoardAutoService.GetExpenseDetailLedgerAccountWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardTabularData }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetExpenseDetailBranchWise()
        {
            IEnumerable<DashBoardTabularData> DashBoardTabularData = _DashBoardAutoService.GetExpenseDetailBranchWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardTabularData }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetExpenseDetailCostCenterWise()
        {
            IEnumerable<DashBoardTabularData> DashBoardTabularData = _DashBoardAutoService.GetExpenseDetailCostCenterWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardTabularData }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetCreditorsDetail()
        {
            IEnumerable<DashBoardTabularData> DashBoardTabularData = _DashBoardAutoService.GetCreditorsDetail();

            JsonResult json = Json(new { Success = true, Data = DashBoardTabularData }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetCashBalanceDetailLedgerAccountWise()
        {
            IEnumerable<DashBoardTabularData> DashBoardTabularData = _DashBoardAutoService.GetCashBalanceDetailLedgerAccountWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardTabularData }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetCashBalanceDetailBranchWise()
        {
            IEnumerable<DashBoardTabularData> DashBoardTabularData = _DashBoardAutoService.GetCashBalanceDetailBranchWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardTabularData }, JsonRequestBehavior.AllowGet);
            return json;
        }



        public JsonResult GetVehiclePurchaseDetailProductTypeWise()
        {
            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardPurchaseDetailProductTypeWise = _DashBoardAutoService.GetVehiclePurchaseDetailProductTypeWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardPurchaseDetailProductTypeWise }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetVehiclePurchaseDetailProductGroupWise()
        {
            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardPurchaseDetailProductGroupWise = _DashBoardAutoService.GetVehiclePurchaseDetailProductGroupWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardPurchaseDetailProductGroupWise }, JsonRequestBehavior.AllowGet);
            return json;
        }


        public JsonResult GetVehiclePurchaseOrderDetailProductTypeWise()
        {
            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardPurchaseOrderDetailProductTypeWise = _DashBoardAutoService.GetVehiclePurchaseOrderDetailProductTypeWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardPurchaseOrderDetailProductTypeWise }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetVehiclePurchaseOrderDetailProductGroupWise()
        {
            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardPurchaseOrderDetailProductGroupWise = _DashBoardAutoService.GetVehiclePurchaseOrderDetailProductGroupWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardPurchaseOrderDetailProductGroupWise }, JsonRequestBehavior.AllowGet);
            return json;
        }



        public JsonResult GetWorkshopSaleDetailProductTypeWise()
        {
            IEnumerable<DashBoardTabularData> DashBoardPurchaseDetailProductTypeWise = _DashBoardAutoService.GetWorkshopSaleDetailProductTypeWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardPurchaseDetailProductTypeWise }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetWorkshopSaleDetailProductGroupWise()
        {
            IEnumerable<DashBoardTabularData> DashBoardPurchaseDetailProductGroupWise = _DashBoardAutoService.GetWorkshopSaleDetailProductGroupWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardPurchaseDetailProductGroupWise }, JsonRequestBehavior.AllowGet);
            return json;
        }


        public JsonResult GetSpareSaleDetailProductTypeWise()
        {
            IEnumerable<DashBoardTabularData> DashBoardPurchaseDetailProductTypeWise = _DashBoardAutoService.GetSpareSaleDetailProductTypeWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardPurchaseDetailProductTypeWise }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetSpareSaleDetailProductGroupWise()
        {
            IEnumerable<DashBoardTabularData> DashBoardPurchaseDetailProductGroupWise = _DashBoardAutoService.GetSpareSaleDetailProductGroupWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardPurchaseDetailProductGroupWise }, JsonRequestBehavior.AllowGet);
            return json;
        }

    }   
}