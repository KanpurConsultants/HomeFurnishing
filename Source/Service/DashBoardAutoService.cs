using System.Collections.Generic;
using System.Linq;
using Data;
using Data.Infrastructure;
using Model.Models;
using Model.ViewModel;
using Core.Common;
using System;
using Model;
using System.Threading.Tasks;
using Data.Models;
using System.Data.SqlClient;
using System.Configuration;

namespace Service
{
    public interface IDashBoardAutoService : IDisposable
    {
        IEnumerable<DashBoardDoubleValue> GetVehicleSale();
        IEnumerable<DashBoardSingleValue> GetVehicleProfit();
        IEnumerable<DashBoardSingleValue> GetVehicleStock();



        IEnumerable<DashBoardSingleValue> GetExpense();
        IEnumerable<DashBoardSingleValue> GetDebtors();
        IEnumerable<DashBoardSingleValue> GetCreditors();
        IEnumerable<DashBoardSingleValue> GetBankBalance();
        IEnumerable<DashBoardSingleValue> GetCashBalance();


        IEnumerable<DashBoardSingleValue> GetVehiclePurchaseOrder();
        IEnumerable<DashBoardSingleValue> GetVehiclePurchase();
        IEnumerable<DashBoardDoubleValue> GetWorkshopSale();
        IEnumerable<DashBoardDoubleValue> GetSpareSale();



        IEnumerable<DashBoardTabularData_ThreeColumns> GetVehicleSaleDetailSalesManWise();
        IEnumerable<DashBoardTabularData_ThreeColumns> GetVehicleSaleDetailProductTypeWise();
        IEnumerable<DashBoardTabularData_ThreeColumns> GetVehicleSaleDetailProductGroupWise();


        IEnumerable<DashBoardTabularData> GetVehicleProfitDetailProductGroupWise();
        IEnumerable<DashBoardTabularData> GetVehicleProfitDetailSalesManWise();
        IEnumerable<DashBoardTabularData> GetVehicleProfitDetailBranchWise();


        IEnumerable<DashBoardTabularData> GetDebtorsDetail();

        IEnumerable<DashBoardTabularData> GetBankBalanceDetailBankAc();
        IEnumerable<DashBoardTabularData> GetBankBalanceDetailBankODAc();
        IEnumerable<DashBoardTabularData> GetBankBalanceDetailChannelFinanceAc();

        IEnumerable<DashBoardTabularData_ThreeColumns> GetVehicleStockDetailProductTypeWise();
        IEnumerable<DashBoardTabularData_ThreeColumns> GetVehicleStockDetailProductGroupWise();

        IEnumerable<DashBoardTabularData> GetExpenseDetailLedgerAccountWise();
        IEnumerable<DashBoardTabularData> GetExpenseDetailBranchWise();
        IEnumerable<DashBoardTabularData> GetExpenseDetailCostCenterWise();

        IEnumerable<DashBoardTabularData> GetCreditorsDetail();

        IEnumerable<DashBoardTabularData> GetCashBalanceDetailLedgerAccountWise();
        IEnumerable<DashBoardTabularData> GetCashBalanceDetailBranchWise();

        IEnumerable<DashBoardTabularData_ThreeColumns> GetVehiclePurchaseDetailProductTypeWise();
        IEnumerable<DashBoardTabularData_ThreeColumns> GetVehiclePurchaseDetailProductGroupWise();

        IEnumerable<DashBoardTabularData_ThreeColumns> GetVehiclePurchaseOrderDetailProductTypeWise();
        IEnumerable<DashBoardTabularData_ThreeColumns> GetVehiclePurchaseOrderDetailProductGroupWise();

        IEnumerable<DashBoardTabularData> GetWorkshopSaleDetailProductTypeWise();
        IEnumerable<DashBoardTabularData> GetWorkshopSaleDetailProductGroupWise();

        IEnumerable<DashBoardTabularData> GetSpareSaleDetailProductTypeWise();
        IEnumerable<DashBoardTabularData> GetSpareSaleDetailProductGroupWise();


        IEnumerable<DashBoardPieChartData> GetVehicleSalePieChartData();
        IEnumerable<DashBoardSaleBarChartData> GetVehicleSaleBarChartData();
        IEnumerable<DashBoardPieChartData> GetSpareSalePieChartData();
        IEnumerable<DashBoardSaleBarChartData> GetSpareSaleBarChartData();
    }

    public class DashBoardAutoService : IDashBoardAutoService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        string mQry = "";
        private readonly IUnitOfWorkForService _unitOfWork;

        DateTime? MonthStartDate = null;
        DateTime? MonthEndDate = null;
        DateTime? YearStartDate = null;
        DateTime? YearEndDate = null;
        DateTime? SoftwareStartDate = null;
        DateTime? TodayDate = null;


        public DashBoardAutoService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
            Init();
        }
        public DashBoardAutoService()
        {
            Init();
        }

        private void Init()
        {
            mQry = @"DECLARE @Month INT 
                    DECLARE @Year INT
                    SELECT @Month =  Datepart(MONTH,getdate())
                    SELECT @Year =  Datepart(YEAR,getdate())
                    DECLARE @FromDate DATETIME
                    DECLARE @ToDate DATETIME
                    SELECT DATEADD(month,@Month-1,DATEADD(year,@Year-1900,0)) As MonthStartDate, 
                    DATEADD(day,-1,DATEADD(month,@Month,DATEADD(year,@Year-1900,0))) As MonthEndDate,
                    CASE WHEN DATEPART(MM,GETDATE()) < 4 THEN DATEADD(MONTH,-9,DATEADD(DD,-DATEPART(DY,GETDATE())+1,GETDATE()))
                    ELSE DATEADD(MONTH,3,DATEADD(DD,-DATEPART(DY,GETDATE())+1,GETDATE())) END AS YearStartDate,
                    CASE WHEN DATEPART(MM,GETDATE()) < 4 THEN DATEADD(MONTH,-9,DATEADD(DD,-1,DATEADD(YY,DATEDIFF(YY,0,GETDATE())+1,0)))
                    ELSE DATEADD(MONTH,3,DATEADD(DD,-1,DATEADD(YY,DATEDIFF(YY,0,GETDATE())+1,0))) END AS YearEndDate,
                    Convert(DATETIME,'01/Apr/2001') AS SoftwareStartDate,
                    GETDATE() As TodayDate ";
            SessnionValues SessnionValues = db.Database.SqlQuery<SessnionValues>(mQry).FirstOrDefault();

            MonthStartDate = SessnionValues.MonthStartDate;
            MonthEndDate = SessnionValues.MonthEndDate;
            YearStartDate = SessnionValues.YearStartDate;
            YearEndDate = SessnionValues.YearEndDate;
            SoftwareStartDate = SessnionValues.SoftwareStartDate;
            TodayDate = SessnionValues.TodayDate;
        }

        public IEnumerable<DashBoardDoubleValue> GetVehicleSale()
        {
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", MonthStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", MonthEndDate);
            SqlParameter SqlParameterTodayDate = new SqlParameter("@TodayDate", TodayDate);

            mQry = @"Select Convert(NVARCHAR,IsNull(Sum(VMain.MonthSale),0)) AS Value1,
                    Convert(NVARCHAR,IsNull(Sum(VMain.TodaySale),0)) AS Value2
                    From (" + GetSaleSummarySubQry(464) + ") As VMain ";

            IEnumerable<DashBoardDoubleValue> VehicleSale = db.Database.SqlQuery<DashBoardDoubleValue>(mQry, SqlParameterFromDate, SqlParameterToDate, SqlParameterTodayDate).ToList();
            return VehicleSale;
        }
        public IEnumerable<DashBoardSingleValue> GetVehicleProfit()
        {
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", MonthStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", MonthEndDate);

            mQry = @" SELECT Convert(NVARCHAR,Sum(L.Amount - Purch.PurchaseAmount - IsNull(CreditNote.CreditNoteAmount,0) + IsNull(purch.RetensionTarget,0))) AS Value
                        FROM Web.SaleInvoiceHeaders H
                        LEFT JOIN web.SaleInvoiceLines L ON H.SaleInvoiceHeaderId = L.SaleInvoiceHeaderId 
                        LEFT JOIN web.SaleDispatchLines DL ON L.SaleDispatchLineId = DL.SaleDispatchLineId 
                        LEFT JOIN web.PackingLines PL ON DL.PackingLineId = PL.PackingLineId 
                        LEFT JOIN web.SaleDispatchReturnLines DRL ON DL.SaleDispatchLineId = DRL.SaleDispatchLineId 
                        LEFT JOIN web.People P ON H.BillToBuyerId = P.PersonID 
                        LEFT JOIN web.Products Pr ON PL.ProductId = Pr.ProductId 
                        LEFT JOIN web.ProductGroups Pg ON Pr.ProductGroupId = Pg.ProductGroupId 
                        LEFT JOIN Web.ProductTypes Pt ON Pg.ProductTypeId = Pt.ProductTypeId
                        LEFT JOIN web.ProductUids UID ON PL.ProductUidId = UID.ProductUIDId 
                        LEFT JOIN (
			                        SELECT L.SaleInvoiceLineId, Sum(L.Amount) AS CreditNoteAmount
			                        FROM Web.SaleInvoiceReturnHeaders H
			                        LEFT JOIN web.SaleInvoiceReturnLines L ON L.SaleInvoiceReturnHeaderId = H.SaleInvoiceReturnHeaderId 
			                        WHERE H.Nature ='Credit Note'
			                        GROUP BY L.SaleInvoiceLineId 
		                          ) AS CreditNote ON CreditNote.SaleInvoiceLineId = L.SaleInvoiceLineId 
                        LEFT JOIN (
			                        SELECT L.JobInvoiceHeaderId, Max(H.DocNo) AS TmlInvoiceNo, Max(H.DocDate) AS TmlInvoiceDate, Max(RL.ProductUidId) AS ProductUidId, Sum(L.Amount) AS PurchaseAmount, Max(P.StandardCost) - Sum(L.Amount) AS PriceDiff,
			                        (SELECT Sum(promo.FlatDiscount) FROM Web.PromoCodes promo WHERE max(h.DocDate) BETWEEN promo.FromDate AND promo.ToDate AND (promo.ProductId = max(p.ProductId) OR promo.ProductGroupId = max(p.ProductGroupId) OR IsNull(Max(p.ProductCategoryId),-1)=promo.ProductGroupId OR Max(pg.ProductTypeId) = promo.ProductTypeId OR IsNull(promo.ProductId,0)+IsNull(promo.ProductGroupId,0)+IsNull(promo.ProductCategoryId,0)+IsNull(promo.ProductTypeId,0) = 0)) AS RetensionTarget
			                        FROM Web.JobInvoiceHeaders H
			                        LEFT JOIN web.JobInvoiceLines L ON H.JobInvoiceHeaderId = L.JobInvoiceHeaderId 
			                        LEFT JOIN web.JobReceiveLines RL ON L.JobReceiveLineId = RL.JobReceiveLineId 
			                        LEFT JOIN web.Products P ON RL.ProductId = p.ProductId 
			                        LEFT JOIN web.ProductGroups pg ON pg.ProductGroupId = p.ProductGroupId 
			                        WHERE H.DocTypeId = 631
			                        GROUP BY L.JobInvoiceHeaderId 
		                          ) AS Purch ON PL.productUidId = Purch.ProductUidId
                        WHERE DRL.SaleDispatchReturnLineId IS NULL AND H.DocDate BETWEEN @FromDate AND @ToDate
                        AND PL.ProductUidId IS NOT NULL  ";

            IEnumerable<DashBoardSingleValue> VehicleProfit = db.Database.SqlQuery<DashBoardSingleValue>(mQry, SqlParameterFromDate, SqlParameterToDate).ToList();
            return VehicleProfit;
        }
        public IEnumerable<DashBoardSingleValue> GetVehicleStock()
        {
            mQry = @"SELECT Convert(NVARCHAR,IsNull(Sum(VStock.StockQty),0)) AS StockQty, 
                        Convert(NVARCHAR,IsNull(Sum(VStock.StockAmount),0)) AS Value
                        FROM (
	                        SELECT L.Qty AS StockQty,
	                        (SELECT hC.Amount FROM Web.JobInvoiceHeaderCharges hc WITH (Nolock) 
			                        LEFT JOIN Web.Charges cc ON cc.ChargeId = hc.ChargeId 
			                        WHERE cc.ChargeName ='Net Amount' 
			                        AND HC.HeaderTableId = H.JobInvoiceHeaderId ) StockAmount
	                        FROM Web.JobInvoiceHeaders  H WITH (Nolock)
	                        LEFT JOIN web.Sites S  WITH (NoLock) ON S.SiteId = H.SiteId 
	                        LEFT JOIN web.Divisions D  WITH (NoLock) ON D.DivisionId = H.DivisionId 
	                        LEFT JOIN web.Processes PR WITH (NoLock) ON PR.ProcessId = H.ProcessId 
	                        LEFT JOIN web.JobInvoiceLines L WITH (NoLock) ON L.JobInvoiceHeaderId = H.JobInvoiceHeaderId 
	                        LEFT JOIN web.JobReceiveLines R WITH (NoLock) ON R.JobReceiveLineId = L.JobReceiveLineId 
	                        LEFT JOIN web.JobOrderLines JOL WITH (NoLock) ON JOL.JobOrderLineId = R.JobOrderLineId 
	                        LEFT JOIN web.Products P WITH (NoLock) ON P.ProductId = JOL.ProductId 
	                        LEFT JOIN web.ProductGroups PG WITH (NoLock) ON PG.ProductGroupId = P.ProductGroupId 
	                        LEFT JOIN web.ProductTypes PT WITH (NoLock) ON PT.ProductTypeId  = PG.ProductTypeId 
	                        LEFT JOIN web.ProductCategories PC WITH (NoLock) ON PC.ProductCategoryId = P.ProductCategoryId 
	                        LEFT JOIN web.ProductNatures PN WITH (NoLock) ON PN.ProductNatureId = PT.ProductNatureId 
	                        LEFT JOIN 
	                        (
		                        SELECT PL.ProductUidId, Max(PH.DocDate) AS PackingDate, Max(PL.PackingLineId) AS  PackingLineId
		                        FROM web.PackingLines PL WITH (NoLock)
		                        LEFT JOIN web.PackingHeaders PH WITH (NoLock) ON Pl.PackingHeaderId = Ph.PackingHeaderId 
		                        LEFT JOIN web.SaleDispatchLines SDL  WITH (NoLock) ON SDL.PackingLineId = PL.PackingLineId 
		                        LEFT JOIN web.SaleDispatchReturnLines SDRL  WITH (NoLock) ON SDRL.SaleDispatchLineId= SDL.SaleDispatchLineId 
		                        WHERE SDRL.SaleDispatchReturnLineId IS NULL 
		                        GROUP BY PL.ProductUidId 
		                        ) AS PL ON PL.ProductUidId = R.ProductUidId
	                        WHERE PR.ProcessName ='Purchase' AND PL.PackingDate IS NULL 
	                        AND JOL.ProductId IS NOT NULL AND PN.ProductNatureName ='LOB' 
                        ) AS VStock ";


            IEnumerable<DashBoardSingleValue> VehicleStock = db.Database.SqlQuery<DashBoardSingleValue>(mQry).ToList();
            return VehicleStock;
        }
        public IEnumerable<DashBoardSingleValue> GetExpense()
        {
            mQry = "SELECT Convert(nvarchar,LedgerAccountGroupId) As Value FROM Web.LedgerAccountGroups WHERE LedgerAccountGroupName = '" + Jobs.Constants.LedgerAccountGroup.LedgerAccountGroupConstants.DirectExpenses.LedgerAccountGroupName + "'";
            DashBoardSingleValue SundryDebtorsLedgerAccountGroup = db.Database.SqlQuery<DashBoardSingleValue>(mQry).FirstOrDefault();

            SqlParameter SqlParameterSiteId = new SqlParameter("@Site", (object)DBNull.Value);
            SqlParameter SqlParameterDivisionId = new SqlParameter("@Division", (object)DBNull.Value);
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", SoftwareStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", TodayDate);
            SqlParameter SqlParameterCostCenter = new SqlParameter("@CostCenter", (object)DBNull.Value);
            SqlParameter SqlParameterLedgerAccountGroup = new SqlParameter("@LedgerAccountGroup", SundryDebtorsLedgerAccountGroup.Value);

            mQry = new FinancialDisplayService(_unitOfWork).GetQryForTrialBalance(null, null, SoftwareStartDate.ToString(), TodayDate.ToString(), null, "False", "False", SundryDebtorsLedgerAccountGroup.Value) +
                                        @" SELECT Convert(NVARCHAR,IsNull(Sum(VMain.Balance),0)) As Value
                                        FROM
                                        (
                                            SELECT Sum(isnull(H.Balance,0)) AS Balance
                                            FROM cteAcGroup H 
                                            GROUP BY H.BaseLedgerAccountGroupId 

                                            UNION ALL 
                
                                            SELECT isnull(H.Balance,0) AS Balance
                                            FROM cteLedgerBalance H 
                                         ) As VMain ";


            IEnumerable<DashBoardSingleValue> DashBoardSingleValue = db.Database.SqlQuery<DashBoardSingleValue>(mQry, SqlParameterSiteId, SqlParameterDivisionId, SqlParameterFromDate, SqlParameterToDate, SqlParameterCostCenter, SqlParameterLedgerAccountGroup).ToList();
            return DashBoardSingleValue;
        }
        public IEnumerable<DashBoardSingleValue> GetDebtors()
        {
            mQry = "SELECT Convert(nvarchar,LedgerAccountGroupId) As Value FROM Web.LedgerAccountGroups WHERE LedgerAccountGroupName = '"+ Jobs.Constants.LedgerAccountGroup.LedgerAccountGroupConstants.SundryDebtors.LedgerAccountGroupName + "'";
            DashBoardSingleValue SundryDebtorsLedgerAccountGroup = db.Database.SqlQuery<DashBoardSingleValue>(mQry).FirstOrDefault();

            SqlParameter SqlParameterSiteId = new SqlParameter("@Site", (object)DBNull.Value);
            SqlParameter SqlParameterDivisionId = new SqlParameter("@Division", (object)DBNull.Value);
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", SoftwareStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", TodayDate);
            SqlParameter SqlParameterCostCenter = new SqlParameter("@CostCenter", (object)DBNull.Value);
            SqlParameter SqlParameterLedgerAccountGroup = new SqlParameter("@LedgerAccountGroup", SundryDebtorsLedgerAccountGroup.Value);

            mQry = new FinancialDisplayService(_unitOfWork).GetQryForTrialBalance(null, null, SoftwareStartDate.ToString(), TodayDate.ToString(), null, "False", "False", SundryDebtorsLedgerAccountGroup.Value) +
                                        @" SELECT Convert(NVARCHAR,IsNull(Sum(VMain.Balance),0)) As Value
                                        FROM
                                        (
                                            SELECT Sum(isnull(H.Balance,0)) AS Balance
                                            FROM cteAcGroup H 
                                            GROUP BY H.BaseLedgerAccountGroupId 

                                            UNION ALL 
                
                                            SELECT isnull(H.Balance,0) AS Balance
                                            FROM cteLedgerBalance H 
                                         ) As VMain ";


            IEnumerable<DashBoardSingleValue> DashBoardSingleValue = db.Database.SqlQuery<DashBoardSingleValue>(mQry, SqlParameterSiteId, SqlParameterDivisionId, SqlParameterFromDate, SqlParameterToDate, SqlParameterCostCenter, SqlParameterLedgerAccountGroup).ToList();
            return DashBoardSingleValue;
        }
        public IEnumerable<DashBoardSingleValue> GetCreditors()
        {
            mQry = "SELECT Convert(nvarchar,LedgerAccountGroupId) As Value FROM Web.LedgerAccountGroups WHERE LedgerAccountGroupName = '" + Jobs.Constants.LedgerAccountGroup.LedgerAccountGroupConstants.SundryCreditors.LedgerAccountGroupName + "'";
            DashBoardSingleValue SundryDebtorsLedgerAccountGroup = db.Database.SqlQuery<DashBoardSingleValue>(mQry).FirstOrDefault();

            SqlParameter SqlParameterSiteId = new SqlParameter("@Site", (object)DBNull.Value);
            SqlParameter SqlParameterDivisionId = new SqlParameter("@Division", (object)DBNull.Value);
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", SoftwareStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", TodayDate);
            SqlParameter SqlParameterCostCenter = new SqlParameter("@CostCenter", (object)DBNull.Value);
            SqlParameter SqlParameterLedgerAccountGroup = new SqlParameter("@LedgerAccountGroup", SundryDebtorsLedgerAccountGroup.Value);

            mQry = new FinancialDisplayService(_unitOfWork).GetQryForTrialBalance(null, null, SoftwareStartDate.ToString(), TodayDate.ToString(), null, "False", "False", SundryDebtorsLedgerAccountGroup.Value) +
                                        @" SELECT Convert(NVARCHAR,IsNull(-Sum(VMain.Balance),0)) As Value
                                        FROM
                                        (
                                            SELECT Sum(isnull(H.Balance,0)) AS Balance
                                            FROM cteAcGroup H 
                                            GROUP BY H.BaseLedgerAccountGroupId 

                                            UNION ALL 
                
                                            SELECT isnull(H.Balance,0) AS Balance
                                            FROM cteLedgerBalance H 
                                         ) As VMain ";


            IEnumerable<DashBoardSingleValue> DashBoardSingleValue = db.Database.SqlQuery<DashBoardSingleValue>(mQry, SqlParameterSiteId, SqlParameterDivisionId, SqlParameterFromDate, SqlParameterToDate, SqlParameterCostCenter, SqlParameterLedgerAccountGroup).ToList();

            return DashBoardSingleValue;
        }
        public IEnumerable<DashBoardSingleValue> GetBankBalance()
        {
            mQry = "SELECT Convert(nvarchar,LedgerAccountGroupId) As Value FROM Web.LedgerAccountGroups WHERE LedgerAccountGroupName = '" + Jobs.Constants.LedgerAccountGroup.LedgerAccountGroupConstants.BankAccounts.LedgerAccountGroupName + "'";
            DashBoardSingleValue SundryDebtorsLedgerAccountGroup = db.Database.SqlQuery<DashBoardSingleValue>(mQry).FirstOrDefault();

            SqlParameter SqlParameterSiteId = new SqlParameter("@Site", (object)DBNull.Value);
            SqlParameter SqlParameterDivisionId = new SqlParameter("@Division", (object)DBNull.Value);
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", SoftwareStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", TodayDate);
            SqlParameter SqlParameterCostCenter = new SqlParameter("@CostCenter", (object)DBNull.Value);
            SqlParameter SqlParameterLedgerAccountGroup = new SqlParameter("@LedgerAccountGroup", SundryDebtorsLedgerAccountGroup.Value);

            mQry = new FinancialDisplayService(_unitOfWork).GetQryForTrialBalance(null, null, SoftwareStartDate.ToString(), TodayDate.ToString(), null, "False", "False", SundryDebtorsLedgerAccountGroup.Value) +
                                        @" SELECT Convert(NVARCHAR,IsNull(Sum(VMain.Balance),0)) As Value
                                        FROM
                                        (
                                            SELECT Sum(isnull(H.Balance,0)) AS Balance
                                            FROM cteAcGroup H 
                                            GROUP BY H.BaseLedgerAccountGroupId 

                                            UNION ALL 
                
                                            SELECT isnull(H.Balance,0) AS Balance
                                            FROM cteLedgerBalance H 
                                         ) As VMain ";


            IEnumerable<DashBoardSingleValue> DashBoardSingleValue = db.Database.SqlQuery<DashBoardSingleValue>(mQry, SqlParameterSiteId, SqlParameterDivisionId, SqlParameterFromDate, SqlParameterToDate, SqlParameterCostCenter, SqlParameterLedgerAccountGroup).ToList();
            return DashBoardSingleValue;
        }
        public IEnumerable<DashBoardSingleValue> GetCashBalance()
        {
            mQry = "SELECT Convert(nvarchar,LedgerAccountGroupId) As Value FROM Web.LedgerAccountGroups WHERE LedgerAccountGroupName = '" + Jobs.Constants.LedgerAccountGroup.LedgerAccountGroupConstants.CashinHand.LedgerAccountGroupName + "'";
            DashBoardSingleValue SundryDebtorsLedgerAccountGroup = db.Database.SqlQuery<DashBoardSingleValue>(mQry).FirstOrDefault();

            SqlParameter SqlParameterSiteId = new SqlParameter("@Site", (object)DBNull.Value);
            SqlParameter SqlParameterDivisionId = new SqlParameter("@Division", (object)DBNull.Value);
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", SoftwareStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", TodayDate);
            SqlParameter SqlParameterCostCenter = new SqlParameter("@CostCenter", (object)DBNull.Value);
            SqlParameter SqlParameterLedgerAccountGroup = new SqlParameter("@LedgerAccountGroup", SundryDebtorsLedgerAccountGroup.Value);

            mQry = new FinancialDisplayService(_unitOfWork).GetQryForTrialBalance(null, null, SoftwareStartDate.ToString(), TodayDate.ToString(), null, "False", "False", SundryDebtorsLedgerAccountGroup.Value) +
                                        @" SELECT Convert(NVARCHAR,IsNull(Sum(VMain.Balance),0)) As Value
                                        FROM
                                        (
                                            SELECT Sum(isnull(H.Balance,0)) AS Balance
                                            FROM cteAcGroup H 
                                            GROUP BY H.BaseLedgerAccountGroupId 

                                            UNION ALL 
                
                                            SELECT isnull(H.Balance,0) AS Balance
                                            FROM cteLedgerBalance H 
                                         ) As VMain ";


            IEnumerable<DashBoardSingleValue> DashBoardSingleValue = db.Database.SqlQuery<DashBoardSingleValue>(mQry, SqlParameterSiteId, SqlParameterDivisionId, SqlParameterFromDate, SqlParameterToDate, SqlParameterCostCenter, SqlParameterLedgerAccountGroup).ToList();
            return DashBoardSingleValue;
        }
        public IEnumerable<DashBoardSingleValue> GetVehiclePurchaseOrder()
        {
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", MonthStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", MonthEndDate);

            mQry = @" SELECT Convert(NVARCHAR,IsNull(Sum(Hc.Amount),0)) AS Value
                    FROM Web.JobOrderHeaders H 
                    LEFT JOIN Web.JobOrderHeaderCharges Hc ON H.JobOrderHeaderId = Hc.HeaderTableId
                    LEFT JOIN Web.DocumentTypes D ON H.DocTypeId = D.DocumentTypeId
                    LEFT JOIN Web.Charges C ON Hc.ChargeId = C.ChargeId
                    WHERE C.ChargeName = 'Net Amount'
                    AND  H.DocDate BETWEEN @FromDate AND @ToDate
                    AND D.DocumentCategoryId = 218 ";

            IEnumerable<DashBoardSingleValue> VehiclePurchaseOrder = db.Database.SqlQuery<DashBoardSingleValue>(mQry, SqlParameterFromDate, SqlParameterToDate).ToList();
            return VehiclePurchaseOrder;
        }
        public IEnumerable<DashBoardSingleValue> GetVehiclePurchase()
        {
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", MonthStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", MonthEndDate);

            mQry = @" SELECT Convert(NVARCHAR,IsNull(Sum(Hc.Amount),0)) AS Value
                    FROM Web.JobInvoiceHeaders H 
                    LEFT JOIN Web.JobInvoiceHeaderCharges Hc ON H.JobInvoiceHeaderId = Hc.HeaderTableId
                    LEFT JOIN Web.DocumentTypes D ON H.DocTypeId = D.DocumentTypeId
                    LEFT JOIN Web.Charges C ON Hc.ChargeId = C.ChargeId
                    WHERE C.ChargeName = 'Net Amount'
                    AND  H.DocDate BETWEEN @FromDate AND @ToDate
                    AND D.DocumentCategoryId = 461 ";

            IEnumerable<DashBoardSingleValue> VehiclePurchase = db.Database.SqlQuery<DashBoardSingleValue>(mQry, SqlParameterFromDate, SqlParameterToDate).ToList();
            return VehiclePurchase;
        }
        public IEnumerable<DashBoardDoubleValue> GetWorkshopSale()
        {
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", MonthStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", MonthEndDate);
            SqlParameter SqlParameterTodayDate = new SqlParameter("@TodayDate", TodayDate);

            mQry = @"Select Convert(NVARCHAR,IsNull(Sum(VMain.MonthSale),0)) AS Value1,
                    Convert(NVARCHAR,IsNull(Sum(VMain.TodaySale),0)) AS Value2
                    From (" + GetSaleSummarySubQry(244) + ") As VMain ";

            IEnumerable<DashBoardDoubleValue> WorkshopSale = db.Database.SqlQuery<DashBoardDoubleValue>(mQry, SqlParameterFromDate, SqlParameterToDate, SqlParameterTodayDate).ToList();
            return WorkshopSale;
        }
        public IEnumerable<DashBoardDoubleValue> GetSpareSale()
        {
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", MonthStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", MonthEndDate);
            SqlParameter SqlParameterTodayDate = new SqlParameter("@TodayDate", TodayDate);

            mQry = @"Select Convert(NVARCHAR,IsNull(Sum(VMain.MonthSale),0)) AS Value1,
                    Convert(NVARCHAR,IsNull(Sum(VMain.TodaySale),0)) AS Value2
                    From (" + GetSaleSummarySubQry(4012) + ") As VMain ";

            IEnumerable<DashBoardDoubleValue> SpareSale = db.Database.SqlQuery<DashBoardDoubleValue>(mQry, SqlParameterFromDate, SqlParameterToDate, SqlParameterTodayDate).ToList();
            return SpareSale;
        }


        public IEnumerable<DashBoardTabularData_ThreeColumns> GetVehicleSaleDetailSalesManWise()
        {
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", MonthStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", MonthEndDate);

            mQry = @"SELECT VMain.SalesMan AS Head, Convert(NVARCHAR,Convert(Int,Sum(VMain.Qty))) AS Value1,
                    Convert(NVARCHAR,IsNull(Sum(VMain.Amount),0)) AS Value2
                    FROM ( " + GetSaleDetailSubQry(464) + @") As VMain
                    GROUP BY VMain.SalesMan
                    ORDER BY VMain.SalesMan ";

            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardTabularData_ThreeColumns = db.Database.SqlQuery<DashBoardTabularData_ThreeColumns>(mQry, SqlParameterFromDate, SqlParameterToDate).ToList();
            return DashBoardTabularData_ThreeColumns;
        }
        public IEnumerable<DashBoardTabularData_ThreeColumns> GetVehicleSaleDetailProductTypeWise()
        {
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", MonthStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", MonthEndDate);

            mQry = @"SELECT VMain.ProductType AS Head, Convert(NVARCHAR,Convert(Int,Sum(VMain.Qty))) AS Value1,
                    Convert(NVARCHAR,IsNull(Sum(VMain.Amount),0)) AS Value2
                    FROM ( " + GetSaleDetailSubQry(464) + @") As VMain
                    GROUP BY VMain.ProductType
                    ORDER BY VMain.ProductType ";

            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardTabularData_ThreeColumns = db.Database.SqlQuery<DashBoardTabularData_ThreeColumns>(mQry, SqlParameterFromDate, SqlParameterToDate).ToList();
            return DashBoardTabularData_ThreeColumns;
        }
        public IEnumerable<DashBoardTabularData_ThreeColumns> GetVehicleSaleDetailProductGroupWise()
        {
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", MonthStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", MonthEndDate);

            mQry = @"SELECT VMain.ProductGroup AS Head, Convert(NVARCHAR,Convert(Int,Sum(VMain.Qty))) AS Value1,
                    Convert(NVARCHAR,IsNull(Sum(VMain.Amount),0)) AS Value2
                    FROM ( " + GetSaleDetailSubQry(464) + @") As VMain
                    GROUP BY VMain.ProductGroup
                    ORDER BY VMain.ProductGroup ";

            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardTabularData_ThreeColumns = db.Database.SqlQuery<DashBoardTabularData_ThreeColumns>(mQry, SqlParameterFromDate, SqlParameterToDate).ToList();
            return DashBoardTabularData_ThreeColumns;
        }


        public IEnumerable<DashBoardTabularData> GetVehicleProfitDetailProductGroupWise()
        {
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", MonthStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", MonthEndDate);

            mQry = @" SELECT VMain.ProductGroupName As Head, Convert(NVARCHAR,Sum(VMain.Amount)) AS Value
                        FROM (" + GetVehicleProfitDetailSubQry() + @") As VMain
                        Group By VMain.ProductGroupName ";

            IEnumerable<DashBoardTabularData> DashBoardTabularData = db.Database.SqlQuery<DashBoardTabularData>(mQry, SqlParameterFromDate, SqlParameterToDate).ToList();
            return DashBoardTabularData;
        }
        public IEnumerable<DashBoardTabularData> GetVehicleProfitDetailSalesManWise()
        {
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", MonthStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", MonthEndDate);

            mQry = @" SELECT VMain.SalesManName As Head, Convert(NVARCHAR,Sum(VMain.Amount)) AS Value
                        FROM (" + GetVehicleProfitDetailSubQry() + @") As VMain
                        Group By VMain.SalesManName ";

            IEnumerable<DashBoardTabularData> DashBoardTabularData = db.Database.SqlQuery<DashBoardTabularData>(mQry, SqlParameterFromDate, SqlParameterToDate).ToList();
            return DashBoardTabularData;
        }
        public IEnumerable<DashBoardTabularData> GetVehicleProfitDetailBranchWise()
        {
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", MonthStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", MonthEndDate);

            mQry = @" SELECT VMain.BranchName As Head, Convert(NVARCHAR,Sum(VMain.Amount)) AS Value
                        FROM (" + GetVehicleProfitDetailSubQry() + @") As VMain
                        Group By VMain.BranchName ";

            IEnumerable<DashBoardTabularData> DashBoardTabularData = db.Database.SqlQuery<DashBoardTabularData>(mQry, SqlParameterFromDate, SqlParameterToDate).ToList();
            return DashBoardTabularData;
        }


        public IEnumerable<DashBoardTabularData> GetDebtorsDetail()
        {
            mQry = "SELECT Convert(nvarchar,LedgerAccountGroupId) As Value FROM Web.LedgerAccountGroups WHERE LedgerAccountGroupName = '"+ Jobs.Constants.LedgerAccountGroup.LedgerAccountGroupConstants.SundryDebtors.LedgerAccountGroupName + "'";
            DashBoardSingleValue SundryDebtorsLedgerAccountGroup = db.Database.SqlQuery<DashBoardSingleValue>(mQry).FirstOrDefault();

            SqlParameter SqlParameterSiteId = new SqlParameter("@Site", (object)DBNull.Value);
            SqlParameter SqlParameterDivisionId = new SqlParameter("@Division", (object)DBNull.Value);
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", SoftwareStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", TodayDate);
            SqlParameter SqlParameterCostCenter = new SqlParameter("@CostCenter", (object)DBNull.Value);
            SqlParameter SqlParameterLedgerAccountGroup = new SqlParameter("@LedgerAccountGroup", SundryDebtorsLedgerAccountGroup.Value);

            mQry = new FinancialDisplayService(_unitOfWork).GetQryForTrialBalance(null, null, SoftwareStartDate.ToString(), TodayDate.ToString(), null, "False", "False", SundryDebtorsLedgerAccountGroup.Value) +
                                        @"SELECT H.BaseLedgerAccountGroupId AS LedgerAccountGroupId, Max(BaseLedgerAccountGroupName) AS Head, 
                                        Convert(NVARCHAR,Sum(isnull(H.Balance,0))) AS Value
                                        FROM cteAcGroup H 
                                        GROUP BY H.BaseLedgerAccountGroupId 
                                        Having Sum(isnull(H.Balance,0)) <> 0 

                                        UNION ALL 

                                        SELECT Ag.LedgerAccountGroupId AS LedgerAccountGroupId, Max(Ag.LedgerAccountGroupName) AS Head, 
                                        Convert(NVARCHAR,Sum(isnull(H.Balance,0)))  AS Value
                                        FROM cteLedgerBalance H 
                                        LEFT JOIN Web.LedgerAccounts A ON H.LedgerAccountId = A.LedgerAccountId
                                        LEFT JOIN Web.LedgerAccountGroups Ag On A.LedgerAccountGroupId = Ag.LedgerAccountGroupId
                                        Where isnull(H.Balance,0) <> 0 
                                        Group By Ag.LedgerAccountGroupId ";

            IEnumerable<DashBoardTabularData> DashBoardTabularData = db.Database.SqlQuery<DashBoardTabularData>(mQry, SqlParameterSiteId, SqlParameterDivisionId, SqlParameterFromDate, SqlParameterToDate, SqlParameterCostCenter, SqlParameterLedgerAccountGroup).ToList();
            return DashBoardTabularData;
        }

        public IEnumerable<DashBoardTabularData> GetBankBalanceDetailBankAc()
        {
            mQry = "SELECT Convert(nvarchar,LedgerAccountGroupId) As Value FROM Web.LedgerAccountGroups WHERE LedgerAccountGroupName = '" + Jobs.Constants.LedgerAccountGroup.LedgerAccountGroupConstants.BankAccounts.LedgerAccountGroupName + "'";
            DashBoardSingleValue SundryDebtorsLedgerAccountGroup = db.Database.SqlQuery<DashBoardSingleValue>(mQry).FirstOrDefault();

            SqlParameter SqlParameterSiteId = new SqlParameter("@Site", (object)DBNull.Value);
            SqlParameter SqlParameterDivisionId = new SqlParameter("@Division", (object)DBNull.Value);
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", SoftwareStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", TodayDate);
            SqlParameter SqlParameterCostCenter = new SqlParameter("@CostCenter", (object)DBNull.Value);
            SqlParameter SqlParameterLedgerAccountGroup = new SqlParameter("@LedgerAccountGroup", SundryDebtorsLedgerAccountGroup.Value);

            mQry = new FinancialDisplayService(_unitOfWork).GetQryForTrialBalance(null, null, SoftwareStartDate.ToString(), TodayDate.ToString(), null, "False", "False", SundryDebtorsLedgerAccountGroup.Value) +
                                        @"SELECT H.BaseLedgerAccountGroupId AS LedgerAccountGroupId, Max(BaseLedgerAccountGroupName) AS Head, 
                                        Convert(NVARCHAR,Sum(isnull(H.Balance,0))) AS Value
                                        FROM cteAcGroup H 
                                        GROUP BY H.BaseLedgerAccountGroupId 
                                        Having Sum(isnull(H.Balance,0)) <> 0 

                                        UNION ALL 

                                        SELECT H.LedgerAccountId AS LedgerAccountGroupId, LedgerAccountName AS Head, 
                                        Convert(NVARCHAR,isnull(H.Balance,0))  AS Value
                                        FROM cteLedgerBalance H 
                                        Where isnull(H.Balance,0) <> 0 ";

            IEnumerable<DashBoardTabularData> DashBoardTabularData = db.Database.SqlQuery<DashBoardTabularData>(mQry, SqlParameterSiteId, SqlParameterDivisionId, SqlParameterFromDate, SqlParameterToDate, SqlParameterCostCenter, SqlParameterLedgerAccountGroup).ToList();
            return DashBoardTabularData;
        }

        public IEnumerable<DashBoardTabularData> GetBankBalanceDetailBankODAc()
        {
            mQry = "SELECT Convert(nvarchar,LedgerAccountGroupId) As Value FROM Web.LedgerAccountGroups WHERE LedgerAccountGroupName = '" + Jobs.Constants.LedgerAccountGroup.LedgerAccountGroupConstants.BankODAc.LedgerAccountGroupName + "'";
            DashBoardSingleValue SundryDebtorsLedgerAccountGroup = db.Database.SqlQuery<DashBoardSingleValue>(mQry).FirstOrDefault();

            SqlParameter SqlParameterSiteId = new SqlParameter("@Site", (object)DBNull.Value);
            SqlParameter SqlParameterDivisionId = new SqlParameter("@Division", (object)DBNull.Value);
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", SoftwareStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", TodayDate);
            SqlParameter SqlParameterCostCenter = new SqlParameter("@CostCenter", (object)DBNull.Value);
            SqlParameter SqlParameterLedgerAccountGroup = new SqlParameter("@LedgerAccountGroup", SundryDebtorsLedgerAccountGroup.Value);

            mQry = new FinancialDisplayService(_unitOfWork).GetQryForTrialBalance(null, null, SoftwareStartDate.ToString(), TodayDate.ToString(), null, "False", "False", SundryDebtorsLedgerAccountGroup.Value) +
                                        @"SELECT H.BaseLedgerAccountGroupId AS LedgerAccountGroupId, Max(BaseLedgerAccountGroupName) AS Head, 
                                        Convert(NVARCHAR,Sum(isnull(H.Balance,0))) AS Value
                                        FROM cteAcGroup H 
                                        GROUP BY H.BaseLedgerAccountGroupId 
                                        Having Sum(isnull(H.Balance,0)) <> 0 

                                        UNION ALL 

                                        SELECT H.LedgerAccountId AS LedgerAccountGroupId, LedgerAccountName AS Head, 
                                        Convert(NVARCHAR,isnull(H.Balance,0))  AS Value
                                        FROM cteLedgerBalance H 
                                        Where isnull(H.Balance,0) <> 0 ";

            IEnumerable<DashBoardTabularData> DashBoardTabularData = db.Database.SqlQuery<DashBoardTabularData>(mQry, SqlParameterSiteId, SqlParameterDivisionId, SqlParameterFromDate, SqlParameterToDate, SqlParameterCostCenter, SqlParameterLedgerAccountGroup).ToList();
            return DashBoardTabularData;
        }

        public IEnumerable<DashBoardTabularData> GetBankBalanceDetailChannelFinanceAc()
        {
            mQry = "SELECT Convert(nvarchar,LedgerAccountGroupId) As Value FROM Web.LedgerAccountGroups WHERE LedgerAccountGroupName = 'Channel Finance Vehicle'";
            DashBoardSingleValue SundryDebtorsLedgerAccountGroup = db.Database.SqlQuery<DashBoardSingleValue>(mQry).FirstOrDefault();

            SqlParameter SqlParameterSiteId = new SqlParameter("@Site", (object)DBNull.Value);
            SqlParameter SqlParameterDivisionId = new SqlParameter("@Division", (object)DBNull.Value);
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", SoftwareStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", TodayDate);
            SqlParameter SqlParameterCostCenter = new SqlParameter("@CostCenter", (object)DBNull.Value);
            SqlParameter SqlParameterLedgerAccountGroup = new SqlParameter("@LedgerAccountGroup", SundryDebtorsLedgerAccountGroup.Value);

            mQry = new FinancialDisplayService(_unitOfWork).GetQryForTrialBalance(null, null, SoftwareStartDate.ToString(), TodayDate.ToString(), null, "False", "False", SundryDebtorsLedgerAccountGroup.Value) +
                                        @"SELECT H.BaseLedgerAccountGroupId AS LedgerAccountGroupId, Max(BaseLedgerAccountGroupName) AS Head, 
                                        Convert(NVARCHAR,Sum(isnull(H.Balance,0))) AS Value
                                        FROM cteAcGroup H 
                                        GROUP BY H.BaseLedgerAccountGroupId 
                                        Having Sum(isnull(H.Balance,0)) <> 0 

                                        UNION ALL 

                                        SELECT H.LedgerAccountId AS LedgerAccountGroupId, LedgerAccountName AS Head, 
                                        Convert(NVARCHAR,isnull(H.Balance,0))  AS Value
                                        FROM cteLedgerBalance H 
                                        Where isnull(H.Balance,0) <> 0 ";

            IEnumerable<DashBoardTabularData> DashBoardTabularData = db.Database.SqlQuery<DashBoardTabularData>(mQry, SqlParameterSiteId, SqlParameterDivisionId, SqlParameterFromDate, SqlParameterToDate, SqlParameterCostCenter, SqlParameterLedgerAccountGroup).ToList();
            return DashBoardTabularData;
        }

        public IEnumerable<DashBoardTabularData_ThreeColumns> GetVehicleStockDetailProductTypeWise()
        {
            mQry = @"SELECT VStock.ProductTypeName AS Head, Convert(NVARCHAR,Convert(Int,IsNull(Sum(VStock.StockQty),0))) AS Value1, 
                        Convert(NVARCHAR,IsNull(Sum(VStock.StockAmount),0))   AS Value2
                        FROM (
	                        SELECT Pt.ProductTypeName, L.Qty AS StockQty,
	                        (SELECT hC.Amount FROM Web.JobInvoiceHeaderCharges hc WITH (Nolock) 
			                        LEFT JOIN Web.Charges cc ON cc.ChargeId = hc.ChargeId 
			                        WHERE cc.ChargeName ='Net Amount' 
			                        AND HC.HeaderTableId = H.JobInvoiceHeaderId ) StockAmount
	                        FROM Web.JobInvoiceHeaders  H WITH (Nolock)
	                        LEFT JOIN web.Sites S  WITH (NoLock) ON S.SiteId = H.SiteId 
	                        LEFT JOIN web.Divisions D  WITH (NoLock) ON D.DivisionId = H.DivisionId 
	                        LEFT JOIN web.Processes PR WITH (NoLock) ON PR.ProcessId = H.ProcessId 
	                        LEFT JOIN web.JobInvoiceLines L WITH (NoLock) ON L.JobInvoiceHeaderId = H.JobInvoiceHeaderId 
	                        LEFT JOIN web.JobReceiveLines R WITH (NoLock) ON R.JobReceiveLineId = L.JobReceiveLineId 
	                        LEFT JOIN web.JobOrderLines JOL WITH (NoLock) ON JOL.JobOrderLineId = R.JobOrderLineId 
	                        LEFT JOIN web.Products P WITH (NoLock) ON P.ProductId = JOL.ProductId 
	                        LEFT JOIN web.ProductGroups PG WITH (NoLock) ON PG.ProductGroupId = P.ProductGroupId 
	                        LEFT JOIN web.ProductTypes PT WITH (NoLock) ON PT.ProductTypeId  = PG.ProductTypeId 
	                        LEFT JOIN web.ProductCategories PC WITH (NoLock) ON PC.ProductCategoryId = P.ProductCategoryId 
	                        LEFT JOIN web.ProductNatures PN WITH (NoLock) ON PN.ProductNatureId = PT.ProductNatureId 
	                        LEFT JOIN 
	                        (
		                        SELECT PL.ProductUidId, Max(PH.DocDate) AS PackingDate, Max(PL.PackingLineId) AS  PackingLineId
		                        FROM web.PackingLines PL WITH (NoLock)
		                        LEFT JOIN web.PackingHeaders PH WITH (NoLock) ON Pl.PackingHeaderId = Ph.PackingHeaderId 
		                        LEFT JOIN web.SaleDispatchLines SDL  WITH (NoLock) ON SDL.PackingLineId = PL.PackingLineId 
		                        LEFT JOIN web.SaleDispatchReturnLines SDRL  WITH (NoLock) ON SDRL.SaleDispatchLineId= SDL.SaleDispatchLineId 
		                        WHERE SDRL.SaleDispatchReturnLineId IS NULL 
		                        GROUP BY PL.ProductUidId 
		                        ) AS PL ON PL.ProductUidId = R.ProductUidId
	                        WHERE PR.ProcessName ='Purchase' AND PL.PackingDate IS NULL 
	                        AND JOL.ProductId IS NOT NULL AND PN.ProductNatureName ='LOB' 
                        ) AS VStock 
                        GROUP BY VStock.ProductTypeName ";

            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardTabularData = db.Database.SqlQuery<DashBoardTabularData_ThreeColumns>(mQry).ToList();
            return DashBoardTabularData;
        }
        public IEnumerable<DashBoardTabularData_ThreeColumns> GetVehicleStockDetailProductGroupWise()
        {
            mQry = @"SELECT VStock.ProductGroupName AS Head, Convert(NVARCHAR,Convert(Int,IsNull(Sum(VStock.StockQty),0))) AS Value1, 
                        Convert(NVARCHAR,IsNull(Sum(VStock.StockAmount),0)) AS Value2
                        FROM (
	                        SELECT Pt.ProductTypeName, PG.ProductGroupName, L.Qty AS StockQty,
	                        (SELECT hC.Amount FROM Web.JobInvoiceHeaderCharges hc WITH (Nolock) 
			                        LEFT JOIN Web.Charges cc ON cc.ChargeId = hc.ChargeId 
			                        WHERE cc.ChargeName ='Net Amount' 
			                        AND HC.HeaderTableId = H.JobInvoiceHeaderId ) StockAmount
	                        FROM Web.JobInvoiceHeaders  H WITH (Nolock)
	                        LEFT JOIN web.Sites S  WITH (NoLock) ON S.SiteId = H.SiteId 
	                        LEFT JOIN web.Divisions D  WITH (NoLock) ON D.DivisionId = H.DivisionId 
	                        LEFT JOIN web.Processes PR WITH (NoLock) ON PR.ProcessId = H.ProcessId 
	                        LEFT JOIN web.JobInvoiceLines L WITH (NoLock) ON L.JobInvoiceHeaderId = H.JobInvoiceHeaderId 
	                        LEFT JOIN web.JobReceiveLines R WITH (NoLock) ON R.JobReceiveLineId = L.JobReceiveLineId 
	                        LEFT JOIN web.JobOrderLines JOL WITH (NoLock) ON JOL.JobOrderLineId = R.JobOrderLineId 
	                        LEFT JOIN web.Products P WITH (NoLock) ON P.ProductId = JOL.ProductId 
	                        LEFT JOIN web.ProductGroups PG WITH (NoLock) ON PG.ProductGroupId = P.ProductGroupId 
	                        LEFT JOIN web.ProductTypes PT WITH (NoLock) ON PT.ProductTypeId  = PG.ProductTypeId 
	                        LEFT JOIN web.ProductCategories PC WITH (NoLock) ON PC.ProductCategoryId = P.ProductCategoryId 
	                        LEFT JOIN web.ProductNatures PN WITH (NoLock) ON PN.ProductNatureId = PT.ProductNatureId 
	                        LEFT JOIN 
	                        (
		                        SELECT PL.ProductUidId, Max(PH.DocDate) AS PackingDate, Max(PL.PackingLineId) AS  PackingLineId
		                        FROM web.PackingLines PL WITH (NoLock)
		                        LEFT JOIN web.PackingHeaders PH WITH (NoLock) ON Pl.PackingHeaderId = Ph.PackingHeaderId 
		                        LEFT JOIN web.SaleDispatchLines SDL  WITH (NoLock) ON SDL.PackingLineId = PL.PackingLineId 
		                        LEFT JOIN web.SaleDispatchReturnLines SDRL  WITH (NoLock) ON SDRL.SaleDispatchLineId= SDL.SaleDispatchLineId 
		                        WHERE SDRL.SaleDispatchReturnLineId IS NULL 
		                        GROUP BY PL.ProductUidId 
		                        ) AS PL ON PL.ProductUidId = R.ProductUidId
	                        WHERE PR.ProcessName ='Purchase' AND PL.PackingDate IS NULL 
	                        AND JOL.ProductId IS NOT NULL AND PN.ProductNatureName ='LOB' 
                        ) AS VStock 
                        GROUP BY VStock.ProductGroupName ";

            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardTabularData = db.Database.SqlQuery<DashBoardTabularData_ThreeColumns>(mQry).ToList();
            return DashBoardTabularData;
        }

        public IEnumerable<DashBoardTabularData> GetExpenseDetailLedgerAccountWise()
        {
            mQry = "SELECT Convert(nvarchar,LedgerAccountGroupId) As Value FROM Web.LedgerAccountGroups WHERE LedgerAccountGroupName = '" + Jobs.Constants.LedgerAccountGroup.LedgerAccountGroupConstants.DirectExpenses.LedgerAccountGroupName + "'";
            DashBoardSingleValue SundryDebtorsLedgerAccountGroup = db.Database.SqlQuery<DashBoardSingleValue>(mQry).FirstOrDefault();

            SqlParameter SqlParameterSiteId = new SqlParameter("@Site", (object)DBNull.Value);
            SqlParameter SqlParameterDivisionId = new SqlParameter("@Division", (object)DBNull.Value);
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", SoftwareStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", TodayDate);
            SqlParameter SqlParameterCostCenter = new SqlParameter("@CostCenter", (object)DBNull.Value);
            SqlParameter SqlParameterLedgerAccountGroup = new SqlParameter("@LedgerAccountGroup", SundryDebtorsLedgerAccountGroup.Value);

            mQry = new FinancialDisplayService(_unitOfWork).GetQryForTrialBalance(null, null, SoftwareStartDate.ToString(), TodayDate.ToString(), null, "False", "False", SundryDebtorsLedgerAccountGroup.Value) +
                                        @"SELECT H.BaseLedgerAccountGroupId AS LedgerAccountGroupId, Max(BaseLedgerAccountGroupName) AS Head, 
                                        Convert(NVARCHAR,Sum(isnull(H.Balance,0))) AS Value
                                        FROM cteAcGroup H 
                                        GROUP BY H.BaseLedgerAccountGroupId 
                                        Having Sum(isnull(H.Balance,0)) <> 0 

                                        UNION ALL 

                                        SELECT Ag.LedgerAccountGroupId AS LedgerAccountGroupId, Max(Ag.LedgerAccountGroupName) AS Head, 
                                        Convert(NVARCHAR,Sum(isnull(H.Balance,0)))  AS Value
                                        FROM cteLedgerBalance H 
                                        LEFT JOIN Web.LedgerAccounts A ON H.LedgerAccountId = A.LedgerAccountId
                                        LEFT JOIN Web.LedgerAccountGroups Ag On A.LedgerAccountGroupId = Ag.LedgerAccountGroupId
                                        Where isnull(H.Balance,0) <> 0 
                                        Group By Ag.LedgerAccountGroupId ";

            IEnumerable<DashBoardTabularData> DashBoardTabularData = db.Database.SqlQuery<DashBoardTabularData>(mQry, SqlParameterSiteId, SqlParameterDivisionId, SqlParameterFromDate, SqlParameterToDate, SqlParameterCostCenter, SqlParameterLedgerAccountGroup).ToList();
            return DashBoardTabularData;
        }
        public IEnumerable<DashBoardTabularData> GetExpenseDetailBranchWise()
        {
            mQry = "SELECT Convert(nvarchar,LedgerAccountGroupId) As Value FROM Web.LedgerAccountGroups WHERE LedgerAccountGroupName = '" + Jobs.Constants.LedgerAccountGroup.LedgerAccountGroupConstants.DirectExpenses.LedgerAccountGroupName + "'";
            DashBoardSingleValue SundryDebtorsLedgerAccountGroup = db.Database.SqlQuery<DashBoardSingleValue>(mQry).FirstOrDefault();

            SqlParameter SqlParameterAsOnDate = new SqlParameter("@AsOnDate", TodayDate);
            SqlParameter SqlParameterLedgerAccountGroup = new SqlParameter("@LedgerAccountGroupId", SundryDebtorsLedgerAccountGroup.Value);

            mQry = GetLedgerAccountHierarchySubQry() +
                    @"SELECT S.SiteName AS Head, 
                        Convert(NVARCHAR,IsNull(Sum(L.AmtDr),0) - IsNull(Sum(L.AmtCr),0)) AS Value
                        FROM CTE C
                        LEFT JOIN Web.LedgerAccounts A ON C.LedgerAccountGroupId = A.LedgerAccountGroupId
                        LEFT JOIN Web.Ledgers L ON A.LedgerAccountId = L.LedgerAccountId
                        LEFT JOIN Web.LedgerHeaders H ON L.LedgerHeaderId = H.LedgerHeaderId
                        LEFT JOIN Web.Sites S ON H.SiteId = S.SiteId
                        WHERE H.DocDate <= getdate()
                        GROUP BY S.SiteName
                        HAVING IsNull(Sum(L.AmtDr),0) - IsNull(Sum(L.AmtCr),0) <> 0 ";

            IEnumerable<DashBoardTabularData> DashBoardTabularData = db.Database.SqlQuery<DashBoardTabularData>(mQry, SqlParameterAsOnDate, SqlParameterLedgerAccountGroup).ToList();
            return DashBoardTabularData;
        }
        public IEnumerable<DashBoardTabularData> GetExpenseDetailCostCenterWise()
        {
            mQry = "SELECT Convert(nvarchar,LedgerAccountGroupId) As Value FROM Web.LedgerAccountGroups WHERE LedgerAccountGroupName = '" + Jobs.Constants.LedgerAccountGroup.LedgerAccountGroupConstants.DirectExpenses.LedgerAccountGroupName + "'";
            DashBoardSingleValue SundryDebtorsLedgerAccountGroup = db.Database.SqlQuery<DashBoardSingleValue>(mQry).FirstOrDefault();

            SqlParameter SqlParameterAsOnDate = new SqlParameter("@AsOnDate", TodayDate);
            SqlParameter SqlParameterLedgerAccountGroup = new SqlParameter("@LedgerAccountGroupId", SundryDebtorsLedgerAccountGroup.Value);

            mQry = GetLedgerAccountHierarchySubQry() +
                    @"SELECT CT.CostCenterName AS Head, 
                        Convert(NVARCHAR,IsNull(Sum(L.AmtDr),0) - IsNull(Sum(L.AmtCr),0)) AS Value
                        FROM CTE C
                        LEFT JOIN Web.LedgerAccounts A ON C.LedgerAccountGroupId = A.LedgerAccountGroupId
                        LEFT JOIN Web.Ledgers L ON A.LedgerAccountId = L.LedgerAccountId
                        LEFT JOIN Web.LedgerHeaders H ON L.LedgerHeaderId = H.LedgerHeaderId
                        LEFT JOIN Web.CostCenters CT ON L.CostCenterId = CT.CostCenterId
                        WHERE H.DocDate <= getdate()
                        GROUP BY CT.CostCenterName
                        HAVING IsNull(Sum(L.AmtDr),0) - IsNull(Sum(L.AmtCr),0) <> 0 ";

            IEnumerable<DashBoardTabularData> DashBoardTabularData = db.Database.SqlQuery<DashBoardTabularData>(mQry, SqlParameterAsOnDate, SqlParameterLedgerAccountGroup).ToList();
            return DashBoardTabularData;
        }

        public IEnumerable<DashBoardTabularData> GetCreditorsDetail()
        {
            mQry = "SELECT Convert(nvarchar,LedgerAccountGroupId) As Value FROM Web.LedgerAccountGroups WHERE LedgerAccountGroupName = '" + Jobs.Constants.LedgerAccountGroup.LedgerAccountGroupConstants.SundryCreditors.LedgerAccountGroupName + "'";
            DashBoardSingleValue SundryDebtorsLedgerAccountGroup = db.Database.SqlQuery<DashBoardSingleValue>(mQry).FirstOrDefault();

            SqlParameter SqlParameterSiteId = new SqlParameter("@Site", (object)DBNull.Value);
            SqlParameter SqlParameterDivisionId = new SqlParameter("@Division", (object)DBNull.Value);
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", SoftwareStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", TodayDate);
            SqlParameter SqlParameterCostCenter = new SqlParameter("@CostCenter", (object)DBNull.Value);
            SqlParameter SqlParameterLedgerAccountGroup = new SqlParameter("@LedgerAccountGroup", SundryDebtorsLedgerAccountGroup.Value);

            mQry = new FinancialDisplayService(_unitOfWork).GetQryForTrialBalance(null, null, SoftwareStartDate.ToString(), TodayDate.ToString(), null, "False", "False", SundryDebtorsLedgerAccountGroup.Value) +
                                        @"SELECT H.BaseLedgerAccountGroupId AS LedgerAccountGroupId, Max(BaseLedgerAccountGroupName) AS Head, 
                                        Convert(NVARCHAR,-Sum(isnull(H.Balance,0))) AS Value
                                        FROM cteAcGroup H 
                                        GROUP BY H.BaseLedgerAccountGroupId 
                                        Having Sum(isnull(H.Balance,0)) <> 0 

                                        UNION ALL 

                                        SELECT Ag.LedgerAccountGroupId AS LedgerAccountGroupId, Max(Ag.LedgerAccountGroupName) AS Head, 
                                        Convert(NVARCHAR,-Sum(isnull(H.Balance,0)))  AS Value
                                        FROM cteLedgerBalance H 
                                        LEFT JOIN Web.LedgerAccounts A ON H.LedgerAccountId = A.LedgerAccountId
                                        LEFT JOIN Web.LedgerAccountGroups Ag On A.LedgerAccountGroupId = Ag.LedgerAccountGroupId
                                        Where isnull(H.Balance,0) <> 0 
                                        Group By Ag.LedgerAccountGroupId ";

            IEnumerable<DashBoardTabularData> DashBoardTabularData = db.Database.SqlQuery<DashBoardTabularData>(mQry, SqlParameterSiteId, SqlParameterDivisionId, SqlParameterFromDate, SqlParameterToDate, SqlParameterCostCenter, SqlParameterLedgerAccountGroup).ToList();
            return DashBoardTabularData;
        }

        public IEnumerable<DashBoardTabularData> GetCashBalanceDetailLedgerAccountWise()
        {
            mQry = "SELECT Convert(nvarchar,LedgerAccountGroupId) As Value FROM Web.LedgerAccountGroups WHERE LedgerAccountGroupName = '" + Jobs.Constants.LedgerAccountGroup.LedgerAccountGroupConstants.CashinHand.LedgerAccountGroupName + "'";
            DashBoardSingleValue SundryDebtorsLedgerAccountGroup = db.Database.SqlQuery<DashBoardSingleValue>(mQry).FirstOrDefault();

            SqlParameter SqlParameterSiteId = new SqlParameter("@Site", (object)DBNull.Value);
            SqlParameter SqlParameterDivisionId = new SqlParameter("@Division", (object)DBNull.Value);
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", SoftwareStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", TodayDate);
            SqlParameter SqlParameterCostCenter = new SqlParameter("@CostCenter", (object)DBNull.Value);
            SqlParameter SqlParameterLedgerAccountGroup = new SqlParameter("@LedgerAccountGroup", SundryDebtorsLedgerAccountGroup.Value);

            mQry = new FinancialDisplayService(_unitOfWork).GetQryForTrialBalance(null, null, SoftwareStartDate.ToString(), TodayDate.ToString(), null, "False", "False", SundryDebtorsLedgerAccountGroup.Value) +
                                        @"SELECT H.LedgerAccountId AS LedgerAccountGroupId, LedgerAccountName AS Head, 
                                        Convert(NVARCHAR,isnull(H.Balance,0))  AS Value
                                        FROM cteLedgerBalance H 
                                        Where isnull(H.Balance,0) <> 0 ";

            IEnumerable<DashBoardTabularData> DashBoardTabularData = db.Database.SqlQuery<DashBoardTabularData>(mQry, SqlParameterSiteId, SqlParameterDivisionId, SqlParameterFromDate, SqlParameterToDate, SqlParameterCostCenter, SqlParameterLedgerAccountGroup).ToList();
            return DashBoardTabularData;
        }
        public IEnumerable<DashBoardTabularData> GetCashBalanceDetailBranchWise()
        {
            mQry = "SELECT Convert(nvarchar,LedgerAccountGroupId) As Value FROM Web.LedgerAccountGroups WHERE LedgerAccountGroupName = '" + Jobs.Constants.LedgerAccountGroup.LedgerAccountGroupConstants.CashinHand.LedgerAccountGroupName + "'";
            DashBoardSingleValue SundryDebtorsLedgerAccountGroup = db.Database.SqlQuery<DashBoardSingleValue>(mQry).FirstOrDefault();

            SqlParameter SqlParameterAsOnDate = new SqlParameter("@AsOnDate", TodayDate);
            SqlParameter SqlParameterLedgerAccountGroup = new SqlParameter("@LedgerAccountGroupId", SundryDebtorsLedgerAccountGroup.Value);

            mQry = GetLedgerAccountHierarchySubQry() +
                    @"SELECT S.SiteName AS Head, 
                        Convert(NVARCHAR,IsNull(Sum(L.AmtDr),0) - IsNull(Sum(L.AmtCr),0))  AS Value
                        FROM CTE C
                        LEFT JOIN Web.LedgerAccounts A ON C.LedgerAccountGroupId = A.LedgerAccountGroupId
                        LEFT JOIN Web.Ledgers L ON A.LedgerAccountId = L.LedgerAccountId
                        LEFT JOIN Web.LedgerHeaders H ON L.LedgerHeaderId = H.LedgerHeaderId
                        LEFT JOIN Web.Sites S ON H.SiteId = S.SiteId
                        WHERE H.DocDate <= getdate()
                        GROUP BY S.SiteName
                        HAVING IsNull(Sum(L.AmtDr),0) - IsNull(Sum(L.AmtCr),0) <> 0 ";

            IEnumerable<DashBoardTabularData> DashBoardTabularData = db.Database.SqlQuery<DashBoardTabularData>(mQry, SqlParameterAsOnDate, SqlParameterLedgerAccountGroup).ToList();
            return DashBoardTabularData;
        }

        public IEnumerable<DashBoardTabularData_ThreeColumns> GetVehiclePurchaseDetailProductTypeWise()
        {
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", MonthStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", MonthEndDate);

            mQry = @"SELECT VMain.ProductType AS Head, Convert(NVARCHAR,Convert(Int,Sum(VMain.Qty))) AS Value1,
                    Convert(NVARCHAR,IsNull(Sum(VMain.Amount),0)) AS Value2
                    FROM ( " + GetPurchaseDetailSubQry() + @") As VMain
                    GROUP BY VMain.ProductType
                    ORDER BY VMain.ProductType ";

            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardTabularData_ThreeColumns = db.Database.SqlQuery<DashBoardTabularData_ThreeColumns>(mQry, SqlParameterFromDate, SqlParameterToDate).ToList();
            return DashBoardTabularData_ThreeColumns;
        }
        public IEnumerable<DashBoardTabularData_ThreeColumns> GetVehiclePurchaseDetailProductGroupWise()
        {
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", MonthStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", MonthEndDate);

            mQry = @"SELECT VMain.ProductGroup AS Head, Convert(NVARCHAR,Convert(Int,Sum(VMain.Qty))) AS Value1,
                    Convert(NVARCHAR,IsNull(Sum(VMain.Amount),0)) AS Value2
                    FROM ( " + GetPurchaseDetailSubQry() + @") As VMain
                    GROUP BY VMain.ProductGroup
                    ORDER BY VMain.ProductGroup ";

            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardTabularData_ThreeColumns = db.Database.SqlQuery<DashBoardTabularData_ThreeColumns>(mQry, SqlParameterFromDate, SqlParameterToDate).ToList();
            return DashBoardTabularData_ThreeColumns;
        }

        public IEnumerable<DashBoardTabularData_ThreeColumns> GetVehiclePurchaseOrderDetailProductTypeWise()
        {
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", MonthStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", MonthEndDate);

            mQry = @"SELECT VMain.ProductType AS Head, Convert(NVARCHAR,Convert(Int,Sum(VMain.Qty))) AS Value1,
                    Convert(NVARCHAR,IsNull(Sum(VMain.Amount),0)) AS Value2
                    FROM ( " + GetPurchaseOrderDetailSubQry() + @") As VMain
                    GROUP BY VMain.ProductType
                    ORDER BY VMain.ProductType ";

            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardTabularData_ThreeColumns = db.Database.SqlQuery<DashBoardTabularData_ThreeColumns>(mQry, SqlParameterFromDate, SqlParameterToDate).ToList();
            return DashBoardTabularData_ThreeColumns;
        }
        public IEnumerable<DashBoardTabularData_ThreeColumns> GetVehiclePurchaseOrderDetailProductGroupWise()
        {
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", MonthStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", MonthEndDate);

            mQry = @"SELECT VMain.ProductGroup AS Head, Convert(NVARCHAR,Convert(Int,Sum(VMain.Qty))) AS Value1,
                    Convert(NVARCHAR,IsNull(Sum(VMain.Amount),0)) AS Value2
                    FROM ( " + GetPurchaseOrderDetailSubQry() + @") As VMain
                    GROUP BY VMain.ProductGroup
                    ORDER BY VMain.ProductGroup ";

            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardTabularData_ThreeColumns = db.Database.SqlQuery<DashBoardTabularData_ThreeColumns>(mQry, SqlParameterFromDate, SqlParameterToDate).ToList();
            return DashBoardTabularData_ThreeColumns;
        }

        public IEnumerable<DashBoardTabularData> GetWorkshopSaleDetailProductTypeWise()
        {
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", MonthStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", MonthEndDate);

            mQry = @"SELECT VMain.ProductType AS Head, Convert(NVARCHAR,IsNull(Sum(VMain.Amount),0)) AS Value
                    FROM ( " + GetSaleDetailSubQry(244) + @") As VMain
                    GROUP BY VMain.ProductType
                    ORDER BY VMain.ProductType ";

            IEnumerable<DashBoardTabularData> DashBoardTabularData_ThreeColumns = db.Database.SqlQuery<DashBoardTabularData>(mQry, SqlParameterFromDate, SqlParameterToDate).ToList();
            return DashBoardTabularData_ThreeColumns;
        }
        public IEnumerable<DashBoardTabularData> GetWorkshopSaleDetailProductGroupWise()
        {
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", MonthStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", MonthEndDate);

            mQry = @"SELECT VMain.ProductGroup AS Head, Convert(NVARCHAR,IsNull(Sum(VMain.Amount),0)) AS Value
                    FROM ( " + GetSaleDetailSubQry(244) + @") As VMain
                    GROUP BY VMain.ProductGroup
                    ORDER BY VMain.ProductGroup ";

            IEnumerable<DashBoardTabularData> DashBoardTabularData_ThreeColumns = db.Database.SqlQuery<DashBoardTabularData>(mQry, SqlParameterFromDate, SqlParameterToDate).ToList();
            return DashBoardTabularData_ThreeColumns;
        }

        public IEnumerable<DashBoardTabularData> GetSpareSaleDetailProductTypeWise()
        {
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", MonthStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", MonthEndDate);

            mQry = @"SELECT VMain.ProductType AS Head, Convert(NVARCHAR,IsNull(Sum(VMain.Amount),0)) AS Value
                    FROM ( " + GetSaleDetailSubQry(4012) + @") As VMain
                    GROUP BY VMain.ProductType
                    ORDER BY VMain.ProductType ";

            IEnumerable<DashBoardTabularData> DashBoardTabularData_ThreeColumns = db.Database.SqlQuery<DashBoardTabularData>(mQry, SqlParameterFromDate, SqlParameterToDate).ToList();
            return DashBoardTabularData_ThreeColumns;
        }
        public IEnumerable<DashBoardTabularData> GetSpareSaleDetailProductGroupWise()
        {
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", MonthStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", MonthEndDate);

            mQry = @"SELECT VMain.ProductGroup AS Head, Convert(NVARCHAR,IsNull(Sum(VMain.Amount),0)) AS Value
                    FROM ( " + GetSaleDetailSubQry(4012) + @") As VMain
                    GROUP BY VMain.ProductGroup
                    ORDER BY VMain.ProductGroup ";

            IEnumerable<DashBoardTabularData> DashBoardTabularData_ThreeColumns = db.Database.SqlQuery<DashBoardTabularData>(mQry, SqlParameterFromDate, SqlParameterToDate).ToList();
            return DashBoardTabularData_ThreeColumns;
        }




        public IEnumerable<DashBoardPieChartData> GetVehicleSalePieChartData()
        {
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", MonthStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", MonthEndDate);

            mQry = @"SELECT S.SiteName As label, Round(Sum(Hc.Amount)/10000000,2) AS value,
                    CASE WHEN row_number() OVER (ORDER BY S.SiteName) = 1 THEN '#f56954'
	                     WHEN row_number() OVER (ORDER BY S.SiteName) = 2 THEN '#00a65a'
	                     WHEN row_number() OVER (ORDER BY S.SiteName) = 3 THEN '#f39c12'
	                     WHEN row_number() OVER (ORDER BY S.SiteName) = 4 THEN '#00c0ef'
	                     WHEN row_number() OVER (ORDER BY S.SiteName) = 5 THEN '#3c8dbc'
	                     WHEN row_number() OVER (ORDER BY S.SiteName) = 6 THEN '#d2d6de'
                         WHEN row_number() OVER (ORDER BY S.SiteName) = 7 THEN '#c685c3'
                         WHEN row_number() OVER (ORDER BY S.SiteName) = 8 THEN '#b2d6ce'
                         WHEN row_number() OVER (ORDER BY S.SiteName) = 9 THEN '#a2d6ce'
	                     ELSE '#f56954'
                    END AS color 
                    FROM Web.SaleInvoiceHeaders H 
                    LEFT JOIN Web.SaleInvoiceHeaderCharges Hc ON H.SaleInvoiceHeaderId = Hc.HeaderTableId
                    LEFT JOIN Web.DocumentTypes D ON H.DocTypeId = D.DocumentTypeId
                    LEFT JOIN Web.Charges C ON Hc.ChargeId = C.ChargeId
                    LEFT JOIN Web.Sites S ON h.SiteId = S.SiteId
                    WHERE C.ChargeName = 'Net Amount'
                    AND  H.DocDate BETWEEN @FromDate AND @ToDate
                    AND D.DocumentCategoryId = 464
                    GROUP BY S.SiteName ";

            IEnumerable<DashBoardPieChartData> VehicleSalePieChartData = db.Database.SqlQuery<DashBoardPieChartData>(mQry, SqlParameterFromDate, SqlParameterToDate).ToList();
            return VehicleSalePieChartData;
        }
        public IEnumerable<DashBoardSaleBarChartData> GetVehicleSaleBarChartData()
        {
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", YearStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", YearEndDate);


            mQry = @"SELECT LEFT(DATENAME(month, H.DocDate),3) AS Month, 
                    Round(Sum(Hc.Amount)/10000000,2) AS Amount
                    FROM Web.SaleInvoiceHeaders H 
                    LEFT JOIN Web.SaleInvoiceHeaderCharges Hc ON H.SaleInvoiceHeaderId = Hc.HeaderTableId
                    LEFT JOIN Web.DocumentTypes D ON H.DocTypeId = D.DocumentTypeId
                    LEFT JOIN Web.Charges C ON Hc.ChargeId = C.ChargeId
                    WHERE C.ChargeName = 'Net Amount'
                    AND D.DocumentCategoryId = 464
                    AND  H.DocDate BETWEEN @FromDate AND @ToDate
                    GROUP BY DATENAME(month, H.DocDate)
                    ORDER BY DatePart(Year,Max(H.DocDate)) + Convert(DECIMAL(18,2),DatePart(month,Max(H.DocDate))) / 100 ";

            IEnumerable<DashBoardSaleBarChartData> ChartData = db.Database.SqlQuery<DashBoardSaleBarChartData>(mQry, SqlParameterFromDate, SqlParameterToDate).ToList();
            return ChartData;
        }

        public IEnumerable<DashBoardPieChartData> GetSpareSalePieChartData()
        {
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", MonthStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", MonthEndDate);

            mQry = @"SELECT S.SiteName As label, Round(Sum(Hc.Amount)/100000,2) AS value,
                    CASE WHEN row_number() OVER (ORDER BY S.SiteName) = 1 THEN '#f56954'
	                     WHEN row_number() OVER (ORDER BY S.SiteName) = 2 THEN '#00a65a'
	                     WHEN row_number() OVER (ORDER BY S.SiteName) = 3 THEN '#f39c12'
	                     WHEN row_number() OVER (ORDER BY S.SiteName) = 4 THEN '#00c0ef'
	                     WHEN row_number() OVER (ORDER BY S.SiteName) = 5 THEN '#3c8dbc'
	                     WHEN row_number() OVER (ORDER BY S.SiteName) = 6 THEN '#d2d6de'
	                     ELSE '#f56954'
                    END AS color 
                    FROM Web.SaleInvoiceHeaders H 
                    LEFT JOIN Web.SaleInvoiceHeaderCharges Hc ON H.SaleInvoiceHeaderId = Hc.HeaderTableId
                    LEFT JOIN Web.DocumentTypes D ON H.DocTypeId = D.DocumentTypeId
                    LEFT JOIN Web.Charges C ON Hc.ChargeId = C.ChargeId
                    LEFT JOIN Web.Sites S ON h.SiteId = S.SiteId
                    WHERE C.ChargeName = 'Net Amount'
                    AND  H.DocDate BETWEEN @FromDate AND @ToDate
                    AND D.DocumentCategoryId = 4012
                    GROUP BY S.SiteName ";

            IEnumerable<DashBoardPieChartData> SaleSalePieChartData = db.Database.SqlQuery<DashBoardPieChartData>(mQry, SqlParameterFromDate, SqlParameterToDate).ToList();
            return SaleSalePieChartData;
        }
        public IEnumerable<DashBoardSaleBarChartData> GetSpareSaleBarChartData()
        {
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", YearStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", YearEndDate);


            mQry = @"SELECT LEFT(DATENAME(month, H.DocDate),3) AS Month, 
                    Round(Sum(Hc.Amount)/100000,2) AS Amount
                    FROM Web.SaleInvoiceHeaders H 
                    LEFT JOIN Web.SaleInvoiceHeaderCharges Hc ON H.SaleInvoiceHeaderId = Hc.HeaderTableId
                    LEFT JOIN Web.DocumentTypes D ON H.DocTypeId = D.DocumentTypeId
                    LEFT JOIN Web.Charges C ON Hc.ChargeId = C.ChargeId
                    WHERE C.ChargeName = 'Net Amount'
                    AND D.DocumentCategoryId = 4012
                    AND  H.DocDate BETWEEN @FromDate AND @ToDate
                    GROUP BY DATENAME(month, H.DocDate)
                    ORDER BY DatePart(Year,Max(H.DocDate)) + Convert(DECIMAL(18,2),DatePart(month,Max(H.DocDate))) / 100 ";

            IEnumerable<DashBoardSaleBarChartData> ChartData = db.Database.SqlQuery<DashBoardSaleBarChartData>(mQry, SqlParameterFromDate, SqlParameterToDate).ToList();
            return ChartData;
        }


        public string GetVehicleProfitDetailSubQry()
        {
            mQry = @" SELECT Pt.ProductTypeName As ProductTypeName, Pg.ProductGroupName, Se.Name As SalesManName, S.SiteName As BranchName,
                        L.Amount - IsNull(Purch.PurchaseAmount,0) - IsNull(CreditNote.CreditNoteAmount,0) + IsNull(purch.RetensionTarget,0) AS Amount
                        FROM Web.SaleInvoiceHeaders H
                        LEFT JOIN web.SaleInvoiceLines L ON H.SaleInvoiceHeaderId = L.SaleInvoiceHeaderId 
                        LEFT JOIN web.SaleDispatchLines DL ON L.SaleDispatchLineId = DL.SaleDispatchLineId 
                        LEFT JOIN web.PackingLines PL ON DL.PackingLineId = PL.PackingLineId 
                        LEFT JOIN web.SaleDispatchReturnLines DRL ON DL.SaleDispatchLineId = DRL.SaleDispatchLineId 
                        LEFT JOIN web.People P ON H.BillToBuyerId = P.PersonID 
                        LEFT JOIN web.Products Pr ON PL.ProductId = Pr.ProductId 
                        LEFT JOIN web.ProductGroups Pg ON Pr.ProductGroupId = Pg.ProductGroupId 
                        LEFT JOIN Web.ProductTypes Pt ON Pg.ProductTypeId = Pt.ProductTypeId
                        LEFT JOIN web.ProductUids UID ON PL.ProductUidId = UID.ProductUIDId 
                        LEFT JOIN Web.People Se On H.SalesExecutiveId = Se.PersonId
                        LEFT JOIN Web.Sites S On H.SiteId = S.SiteId
                        LEFT JOIN (
			                        SELECT L.SaleInvoiceLineId, Sum(L.Amount) AS CreditNoteAmount
			                        FROM Web.SaleInvoiceReturnHeaders H
			                        LEFT JOIN web.SaleInvoiceReturnLines L ON L.SaleInvoiceReturnHeaderId = H.SaleInvoiceReturnHeaderId 
			                        WHERE H.Nature ='Credit Note'
			                        GROUP BY L.SaleInvoiceLineId 
		                          ) AS CreditNote ON CreditNote.SaleInvoiceLineId = L.SaleInvoiceLineId 
                        LEFT JOIN (
			                        SELECT L.JobInvoiceHeaderId, Max(H.DocNo) AS TmlInvoiceNo, Max(H.DocDate) AS TmlInvoiceDate, Max(RL.ProductUidId) AS ProductUidId, Sum(L.Amount) AS PurchaseAmount, Max(P.StandardCost) - Sum(L.Amount) AS PriceDiff,
			                        (SELECT Sum(promo.FlatDiscount) FROM Web.PromoCodes promo WHERE max(h.DocDate) BETWEEN promo.FromDate AND promo.ToDate AND (promo.ProductId = max(p.ProductId) OR promo.ProductGroupId = max(p.ProductGroupId) OR IsNull(Max(p.ProductCategoryId),-1)=promo.ProductGroupId OR Max(pg.ProductTypeId) = promo.ProductTypeId OR IsNull(promo.ProductId,0)+IsNull(promo.ProductGroupId,0)+IsNull(promo.ProductCategoryId,0)+IsNull(promo.ProductTypeId,0) = 0)) AS RetensionTarget
			                        FROM Web.JobInvoiceHeaders H
			                        LEFT JOIN web.JobInvoiceLines L ON H.JobInvoiceHeaderId = L.JobInvoiceHeaderId 
			                        LEFT JOIN web.JobReceiveLines RL ON L.JobReceiveLineId = RL.JobReceiveLineId 
			                        LEFT JOIN web.Products P ON RL.ProductId = p.ProductId 
			                        LEFT JOIN web.ProductGroups pg ON pg.ProductGroupId = p.ProductGroupId 
			                        WHERE H.DocTypeId = 631
			                        GROUP BY L.JobInvoiceHeaderId 
		                          ) AS Purch ON PL.productUidId = Purch.ProductUidId
                        WHERE DRL.SaleDispatchReturnLineId IS NULL AND H.DocDate BETWEEN @FromDate AND @ToDate
                        AND PL.ProductUidId IS NOT NULL ";

            return mQry;
        }
        public string GetSaleDetailSubQry(int DocumentCategoryId)
        {
            mQry = @"SELECT P.Name AS SalesMan, VLine.ProductTypeName As ProductType, VLine.ProductGroupName As ProductGroup, 
                    VLine.Qty AS Qty, VCharge.Amount AS Amount
                    FROM Web.SaleInvoiceHeaders H 
                    LEFT JOIN Web.DocumentTypes D ON H.DocTypeId = D.DocumentTypeId
                    LEFT JOIN Web.People P ON H.SalesExecutiveId = P.PersonID
                    LEFT JOIN 
	                    (SELECT Hc.HeaderTableId AS SaleInvoiceHeaderId, Hc.Amount 
	                    FROM Web.SaleInvoiceHeaderCharges Hc
	                    LEFT JOIN Web.Charges C ON Hc.ChargeId = C.ChargeId
	                    WHERE C.ChargeName = 'Net Amount') AS VCharge ON H.SaleInvoiceHeaderId = VCharge.SaleInvoiceHeaderId
                    LEFT JOIN (
	                    SELECT H.SaleInvoiceHeaderId, Max(Pt.ProductTypeName) AS ProductTypeName, Max(Pg.ProductGroupName) AS ProductGroupName, 
                        Sum(L.Qty) As Qty
	                    FROM Web.SaleInvoiceHeaders H 
	                    LEFT JOIN Web.SaleInvoiceLines L ON H.SaleInvoiceHeaderId = L.SaleInvoiceHeaderId
	                    LEFT JOIN Web.Products P ON L.ProductId = P.ProductId
	                    LEFT JOIN Web.ProductGroups Pg ON P.ProductGroupId = Pg.ProductGroupId
	                    LEFT JOIN Web.ProductTypes Pt ON Pg.ProductTypeId = Pt.ProductTypeId
	                    LEFT JOIN Web.ProductNatures Pn ON Pt.ProductNatureId = Pn.ProductNatureId
	                    WHERE Pn.ProductNatureName Not In ('Addition/Deduction')
	                    GROUP BY H.SaleInvoiceHeaderId) AS VLine ON H.SaleInvoiceHeaderId = VLine.SaleInvoiceHeaderId
                    WHERE H.DocDate BETWEEN @FromDate AND @ToDate
                    AND D.DocumentCategoryId = " + DocumentCategoryId.ToString();

            return mQry;
        }
        public string GetPurchaseDetailSubQry()
        {
            mQry = @"SELECT VLine.ProductTypeName As ProductType, VLine.ProductGroupName As ProductGroup, 
                    VLine.Qty AS Qty, VCharge.Amount AS Amount
                    FROM Web.JobInvoiceHeaders H 
                    LEFT JOIN Web.DocumentTypes D ON H.DocTypeId = D.DocumentTypeId
                    LEFT JOIN 
	                    (SELECT Hc.HeaderTableId AS JobInvoiceHeaderId, Hc.Amount 
	                    FROM Web.JobInvoiceHeaderCharges Hc
	                    LEFT JOIN Web.Charges C ON Hc.ChargeId = C.ChargeId
	                    WHERE C.ChargeName = 'Net Amount') AS VCharge ON H.JobInvoiceHeaderId = VCharge.JobInvoiceHeaderId
                    LEFT JOIN (
	                    SELECT H.JobInvoiceHeaderId, Max(Pt.ProductTypeName) AS ProductTypeName, Max(Pg.ProductGroupName) AS ProductGroupName, 
                        Sum(L.Qty) As Qty
	                    FROM Web.JobInvoiceHeaders H 
	                    LEFT JOIN Web.JobInvoiceLines L ON H.JobInvoiceHeaderId = L.JobInvoiceHeaderId
                        LEFT JOIN Web.JobReceiveLines Jrl On L.JobReceiveLineId = Jrl.JobReceiveLineId
	                    LEFT JOIN Web.Products P ON Jrl.ProductId = P.ProductId
	                    LEFT JOIN Web.ProductGroups Pg ON P.ProductGroupId = Pg.ProductGroupId
	                    LEFT JOIN Web.ProductTypes Pt ON Pg.ProductTypeId = Pt.ProductTypeId
	                    LEFT JOIN Web.ProductNatures Pn ON Pt.ProductNatureId = Pn.ProductNatureId
	                    WHERE Pn.ProductNatureName = 'LOB'
	                    GROUP BY H.JobInvoiceHeaderId) AS VLine ON H.JobInvoiceHeaderId = VLine.JobInvoiceHeaderId
                    WHERE H.DocDate BETWEEN @FromDate AND @ToDate
                    AND D.DocumentCategoryId = 461 ";

            return mQry;
        }
        public string GetPurchaseOrderDetailSubQry()
        {
            mQry = @"SELECT VLine.ProductTypeName As ProductType, VLine.ProductGroupName As ProductGroup, 
                    VLine.Qty AS Qty, VCharge.Amount AS Amount
                    FROM Web.JobOrderHeaders H 
                    LEFT JOIN Web.DocumentTypes D ON H.DocTypeId = D.DocumentTypeId
                    LEFT JOIN 
	                    (SELECT Hc.HeaderTableId AS JobOrderHeaderId, Hc.Amount 
	                    FROM Web.JobOrderHeaderCharges Hc
	                    LEFT JOIN Web.Charges C ON Hc.ChargeId = C.ChargeId
	                    WHERE C.ChargeName = 'Net Amount') AS VCharge ON H.JobOrderHeaderId = VCharge.JobOrderHeaderId
                    LEFT JOIN (
	                    SELECT H.JobOrderHeaderId, Max(Pt.ProductTypeName) AS ProductTypeName, Max(Pg.ProductGroupName) AS ProductGroupName, 
                        Sum(L.Qty) As Qty
	                    FROM Web.JobOrderHeaders H 
	                    LEFT JOIN Web.JobOrderLines L ON H.JobOrderHeaderId = L.JobOrderHeaderId
	                    LEFT JOIN Web.Products P ON L.ProductId = P.ProductId
	                    LEFT JOIN Web.ProductGroups Pg ON P.ProductGroupId = Pg.ProductGroupId
	                    LEFT JOIN Web.ProductTypes Pt ON Pg.ProductTypeId = Pt.ProductTypeId
	                    LEFT JOIN Web.ProductNatures Pn ON Pt.ProductNatureId = Pn.ProductNatureId
	                    WHERE Pn.ProductNatureName = 'LOB'
	                    GROUP BY H.JobOrderHeaderId) AS VLine ON H.JobOrderHeaderId = VLine.JobOrderHeaderId
                    WHERE H.DocDate BETWEEN @FromDate AND @ToDate
                    AND D.DocumentCategoryId = 218 ";

            return mQry;
        }
        public string GetLedgerAccountHierarchySubQry()
        {
            mQry = @"WITH CTE AS (
                      SELECT *, LedgerAccountGroupId as TopParent FROM Web.LedgerAccountGroups WHERE LedgerAccountGroupId = @LedgerAccountGroupId 
                      UNION ALL
                      SELECT Ag.*, C.TopParent 
                      FROM Web.LedgerAccountGroups Ag
                      JOIN CTE C on C.LedgerAccountGroupId = Ag.ParentLedgerAccountGroupId
                      WHERE Ag.LedgerAccountGroupId <> Ag.ParentLedgerAccountGroupId
                    ) ";

            return mQry;
        }
        public string GetSaleSummarySubQry(int DocumentCategoryId)
        {
            mQry = @" SELECT IsNull(Sum(Hc.Amount),0) AS MonthSale,
                    IsNull(Sum(Case When H.DocDate = @TodayDate Then Hc.Amount Else 0 End),0) AS TodaySale
                    FROM Web.SaleInvoiceHeaders H 
                    LEFT JOIN Web.SaleInvoiceHeaderCharges Hc ON H.SaleInvoiceHeaderId = Hc.HeaderTableId
                    LEFT JOIN Web.DocumentTypes D ON H.DocTypeId = D.DocumentTypeId
                    LEFT JOIN Web.Charges C ON Hc.ChargeId = C.ChargeId
                    WHERE C.ChargeName = 'Net Amount'
                    AND  H.DocDate BETWEEN @FromDate AND @ToDate
                    AND D.DocumentCategoryId = " + DocumentCategoryId.ToString();
            return mQry;
        }


        public string GetFormattedValue(string FieldName)
        {
            string Value = @" SELECT 
                            CASE WHEN IsNull(@Value, 0) <= 100000 THEN Convert(NVARCHAR, Convert(DECIMAL(18, 2), Round(IsNull(@Value, 0) / 1000, 2))) +' Thousand'
                                WHEN IsNull(@Value,0) <= 10000000 THEN Convert(NVARCHAR, Convert(DECIMAL(18, 2), Round(IsNull(@Value, 0) / 100000, 2))) +' Lakh'
     ELSE Convert(NVARCHAR, Convert(DECIMAL(18, 2), Round(IsNull(@Value, 0) / 10000000, 2))) END     ";

            return Value;
        }

        public void Dispose()
        {
        }
    }
    public class DashBoardPieChartData
    {
        public string label { get; set; }
        public Decimal value { get; set; }
        public string color { get; set; }
        public string highlight { get; set; }
    }
    public class DashBoardSaleBarChartData
    {
        public string Month { get; set; }
        public Decimal Amount { get; set; }
    }

    
    public class DashBoardTabularData
    {
        public string Head { get; set; }
        public string Value { get; set; }
    }
    public class DashBoardSingleValue
    {
        public string Value { get; set; }
    }

    public class DashBoardTabularData_ThreeColumns
    {
        public string Head { get; set; }
        public string Value1 { get; set; }
        public string Value2 { get; set; }
    }
    public class DashBoardDoubleValue
    {
        public string Value1 { get; set; }
        public string Value2 { get; set; }
    }

    public class SessnionValues
    {
        public DateTime MonthStartDate { get; set; }
        public DateTime MonthEndDate { get; set; }
        public DateTime YearStartDate { get; set; }
        public DateTime YearEndDate { get; set; }
        public DateTime SoftwareStartDate { get; set; }
        public DateTime TodayDate { get; set; }
    }

}
