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
    public class DashBoardRugController : Controller
    {
        IDashBoardRugService _DashBoardRugService;
        public DashBoardRugController(IDashBoardRugService DashBoardRugService)
        {
            _DashBoardRugService = DashBoardRugService;
        }

        public ActionResult DashBoardRug()
        {
            return View();
        }
        public JsonResult GetSaleOrder()
        {
            IEnumerable<DashBoardTrippleValue> SaleOrder = _DashBoardRugService.GetSaleOrder();

            JsonResult json = Json(new { Success = true, Data = SaleOrder }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetSaleInvoice()
        {
            IEnumerable<DashBoardTrippleValue> SaleInvoice = _DashBoardRugService.GetSaleInvoice();

            JsonResult json = Json(new { Success = true, Data = SaleInvoice }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetSaleOrderStatus()
        {
            IEnumerable<DashBoardTrippleValue> SaleOrderStatus = _DashBoardRugService.GetSaleOrderStatus();

            JsonResult json = Json(new { Success = true, Data = SaleOrderStatus }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetVehicleProfit()
        {
            IEnumerable<DashBoardSingleValue> VehicleProfit = _DashBoardRugService.GetVehicleProfit();

            JsonResult json = Json(new { Success = true, Data = VehicleProfit }, JsonRequestBehavior.AllowGet);
            return json;
        }
        public JsonResult GetVehicleStock()
        {
            IEnumerable<DashBoardSingleValue> VehicleStock = _DashBoardRugService.GetVehicleStock();

            JsonResult json = Json(new { Success = true, Data = VehicleStock }, JsonRequestBehavior.AllowGet);
            return json;
        }



        public JsonResult GetExpense()
        {
            IEnumerable<DashBoardSingleValue> Expense = _DashBoardRugService.GetExpense();

            JsonResult json = Json(new { Success = true, Data = Expense }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetDebtors()
        {
            IEnumerable<DashBoardSingleValue> Debtors = _DashBoardRugService.GetDebtors();




            JsonResult json = Json(new { Success = true, Data = Debtors }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetCreditors()
        {
            IEnumerable<DashBoardSingleValue> Creditors = _DashBoardRugService.GetCreditors();

            JsonResult json = Json(new { Success = true, Data = Creditors }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetBankBalance()
        {
            IEnumerable<DashBoardSingleValue> BankBalance = _DashBoardRugService.GetBankBalance();

            JsonResult json = Json(new { Success = true, Data = BankBalance }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetCashBalance()
        {
            IEnumerable<DashBoardSingleValue> CashBalance = _DashBoardRugService.GetCashBalance();

            JsonResult json = Json(new { Success = true, Data = CashBalance }, JsonRequestBehavior.AllowGet);
            return json;
        }


        public JsonResult GetPurchaseInoice()
        {
            IEnumerable<DashBoardTrippleValue> Purchase = _DashBoardRugService.GetPurchaseInvoice();

            JsonResult json = Json(new { Success = true, Data = Purchase }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetPurchaseOrder()
        {
            IEnumerable<DashBoardTrippleValue> PurchaseOrder = _DashBoardRugService.GetPurchaseOrder();

            JsonResult json = Json(new { Success = true, Data = PurchaseOrder }, JsonRequestBehavior.AllowGet);
            return json;
        }
        
        public JsonResult GetWorkshopSale()
        {
            IEnumerable<DashBoardDoubleValue> WorkshopSale = _DashBoardRugService.GetWorkshopSale();

            JsonResult json = Json(new { Success = true, Data = WorkshopSale }, JsonRequestBehavior.AllowGet);
            return json;
        }
        public JsonResult GetSpareSale()
        {
            IEnumerable<DashBoardDoubleValue> SpareSale = _DashBoardRugService.GetSpareSale();

            JsonResult json = Json(new { Success = true, Data = SpareSale }, JsonRequestBehavior.AllowGet);
            return json;
        }


        public JsonResult GetSaleInvoicePieChartData()
        {
            IEnumerable<DashBoardPieChartData> VehiclePieChartData = _DashBoardRugService.GetSaleInvoicePieChartData();

            JsonResult json = Json(new { Success = true, Data = VehiclePieChartData }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetSaleInvoiceChartData()
        {
            IEnumerable<DashBoardSaleBarChartData> VehicleSaleChartData = _DashBoardRugService.GetSaleInvoiceBarChartData();

            JsonResult json = Json(new { Success = true, Data = VehicleSaleChartData }, JsonRequestBehavior.AllowGet);
            return json;
        }
        public JsonResult GetSpareSalePieChartData()
        {
            IEnumerable<DashBoardPieChartData> SparePieChartData = _DashBoardRugService.GetSpareSalePieChartData();

            JsonResult json = Json(new { Success = true, Data = SparePieChartData }, JsonRequestBehavior.AllowGet);
            return json;
        }
        public JsonResult GetSpareSaleChartData()
        {
            IEnumerable<DashBoardSaleBarChartData> SpareSaleChartData = _DashBoardRugService.GetSpareSaleBarChartData();

            JsonResult json = Json(new { Success = true, Data = SpareSaleChartData }, JsonRequestBehavior.AllowGet);
            return json;
        }



        public JsonResult GetSaleInvoiceDetailCategoryWise()
        {
            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardSaleInvoiceDetailCategoryWise = _DashBoardRugService.GetSaleInvoiceDetailCategoryWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardSaleInvoiceDetailCategoryWise }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetSaleInvoiceDetailTypeWise()
        {
            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardSaleInvoiceDetailTypeWise = _DashBoardRugService.GetSaleInvoiceDetailTypeWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardSaleInvoiceDetailTypeWise }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetSaleInvoiceDetailBuyerWise()
        {
            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardSaleInvoiceDetailBuyerWise = _DashBoardRugService.GetSaleInvoiceDetailBuyerWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardSaleInvoiceDetailBuyerWise }, JsonRequestBehavior.AllowGet);
            return json;
        }



        public JsonResult GetSaleOrderDetailCategoryWise()
        {
            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardSaleOrderDetailCategoryWise = _DashBoardRugService.GetSaleOrderDetailCategoryWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardSaleOrderDetailCategoryWise }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetSaleOrderDetailTypeWise()
        {
            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardSaleOrderDetailDetailTypeWise = _DashBoardRugService.GetSaleOrderDetailTypeWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardSaleOrderDetailDetailTypeWise }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetSaleOrderDetailBuyerWise()
        {
            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardSaleOrderDetailBuyerWise = _DashBoardRugService.GetSaleOrderDetailBuyerWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardSaleOrderDetailBuyerWise }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetVehicleProfitDetailProductGroupWise()
        {
            IEnumerable<DashBoardTabularData> DashBoardTabularData = _DashBoardRugService.GetVehicleProfitDetailProductGroupWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardTabularData }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetVehicleProfitDetailSalesManWise()
        {
            IEnumerable<DashBoardTabularData> DashBoardTabularData = _DashBoardRugService.GetVehicleProfitDetailSalesManWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardTabularData }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetVehicleProfitDetailBranchWise()
        {
            IEnumerable<DashBoardTabularData> DashBoardTabularData = _DashBoardRugService.GetVehicleProfitDetailBranchWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardTabularData }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetDebtorsDetail()
        {
            IEnumerable<DashBoardTabularData> DashBoardTabularData = _DashBoardRugService.GetDebtorsDetail();

            JsonResult json = Json(new { Success = true, Data = DashBoardTabularData }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetBankBalanceDetailBankAc()
        {
            IEnumerable<DashBoardTabularData> DashBoardTabularData = _DashBoardRugService.GetBankBalanceDetailBankAc();

            JsonResult json = Json(new { Success = true, Data = DashBoardTabularData }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetBankBalanceDetailBankODAc()
        {
            IEnumerable<DashBoardTabularData> DashBoardTabularData = _DashBoardRugService.GetBankBalanceDetailBankODAc();

            JsonResult json = Json(new { Success = true, Data = DashBoardTabularData }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetBankBalanceDetailChannelFinanceAc()
        {
            IEnumerable<DashBoardTabularData> DashBoardTabularData = _DashBoardRugService.GetBankBalanceDetailChannelFinanceAc();

            JsonResult json = Json(new { Success = true, Data = DashBoardTabularData }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetVehicleStockDetailProductTypeWise()
        {
            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardTabularData = _DashBoardRugService.GetVehicleStockDetailProductTypeWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardTabularData }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetVehicleStockDetailProductGroupWise()
        {
            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardTabularData = _DashBoardRugService.GetVehicleStockDetailProductGroupWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardTabularData }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetExpenseDetailLedgerAccountWise()
        {
            IEnumerable<DashBoardTabularData> DashBoardTabularData = _DashBoardRugService.GetExpenseDetailLedgerAccountWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardTabularData }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetExpenseDetailBranchWise()
        {
            IEnumerable<DashBoardTabularData> DashBoardTabularData = _DashBoardRugService.GetExpenseDetailBranchWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardTabularData }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetExpenseDetailCostCenterWise()
        {
            IEnumerable<DashBoardTabularData> DashBoardTabularData = _DashBoardRugService.GetExpenseDetailCostCenterWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardTabularData }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetCreditorsDetail()
        {
            IEnumerable<DashBoardTabularData> DashBoardTabularData = _DashBoardRugService.GetCreditorsDetail();

            JsonResult json = Json(new { Success = true, Data = DashBoardTabularData }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetCashBalanceDetailLedgerAccountWise()
        {
            IEnumerable<DashBoardTabularData> DashBoardTabularData = _DashBoardRugService.GetCashBalanceDetailLedgerAccountWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardTabularData }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetCashBalanceDetailBranchWise()
        {
            IEnumerable<DashBoardTabularData> DashBoardTabularData = _DashBoardRugService.GetCashBalanceDetailBranchWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardTabularData }, JsonRequestBehavior.AllowGet);
            return json;
        }



        public JsonResult GetPurchaseDetailProductTypeWise()
        {
            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardPurchaseDetailProductTypeWise = _DashBoardRugService.GetPurchaseDetailProductTypeWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardPurchaseDetailProductTypeWise }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetPurchaseDetailProductGroupWise()
        {
            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardPurchaseDetailProductGroupWise = _DashBoardRugService.GetPurchaseDetailProductGroupWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardPurchaseDetailProductGroupWise }, JsonRequestBehavior.AllowGet);
            return json;
        }


        public JsonResult GetPurchaseOrderDetailProductTypeWise()
        {
            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardPurchaseOrderDetailProductTypeWise = _DashBoardRugService.GetPurchaseOrderDetailProductTypeWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardPurchaseOrderDetailProductTypeWise }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetPurchaseOrderDetailProductGroupWise()
        {
            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardPurchaseOrderDetailProductGroupWise = _DashBoardRugService.GetPurchaseOrderDetailProductGroupWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardPurchaseOrderDetailProductGroupWise }, JsonRequestBehavior.AllowGet);
            return json;
        }



        public JsonResult GetWorkshopSaleDetailProductTypeWise()
        {
            IEnumerable<DashBoardTabularData> DashBoardPurchaseDetailProductTypeWise = _DashBoardRugService.GetWorkshopSaleDetailProductTypeWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardPurchaseDetailProductTypeWise }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetWorkshopSaleDetailProductGroupWise()
        {
            IEnumerable<DashBoardTabularData> DashBoardPurchaseDetailProductGroupWise = _DashBoardRugService.GetWorkshopSaleDetailProductGroupWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardPurchaseDetailProductGroupWise }, JsonRequestBehavior.AllowGet);
            return json;
        }


        public JsonResult GetSpareSaleDetailProductTypeWise()
        {
            IEnumerable<DashBoardTabularData> DashBoardPurchaseDetailProductTypeWise = _DashBoardRugService.GetSpareSaleDetailProductTypeWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardPurchaseDetailProductTypeWise }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetSpareSaleDetailProductGroupWise()
        {
            IEnumerable<DashBoardTabularData> DashBoardPurchaseDetailProductGroupWise = _DashBoardRugService.GetSpareSaleDetailProductGroupWise();

            JsonResult json = Json(new { Success = true, Data = DashBoardPurchaseDetailProductGroupWise }, JsonRequestBehavior.AllowGet);
            return json;
        }

    }   
}