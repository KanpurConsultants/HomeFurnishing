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
        IEnumerable<DashBoardSingleValue> GetVehicleSale();
        IEnumerable<DashBoardSingleValue> GetVehicleProfit();
        IEnumerable<DashBoardSingleValue> GetVehicleStock();



        IEnumerable<DashBoardSingleValue> GetExpense();
        IEnumerable<DashBoardSingleValue> GetDebtors();
        IEnumerable<DashBoardSingleValue> GetCreditors();
        IEnumerable<DashBoardSingleValue> GetBankBalance();
        IEnumerable<DashBoardSingleValue> GetCashBalance();

        


        IEnumerable<DashBoardPieChartData> GetVehicleSalePieChartData();
        IEnumerable<DashBoardSaleBarChartData> GetVehicleSaleBarChartData();
        IEnumerable<DashBoardPieChartData> GetSpareSalePieChartData();
        IEnumerable<DashBoardSaleBarChartData> GetSpareSaleBarChartData();



        IEnumerable<DashBoardTabularData> GetVehicleSaleDetailFinancierWise();
        IEnumerable<DashBoardTabularData> GetVehicleSaleDetailSalesManWise();
        IEnumerable<DashBoardTabularData> GetVehicleSaleDetailProductTypeWise();


        IEnumerable<DashBoardTabularData> GetVehicleProfitDetail();
        IEnumerable<DashBoardTabularData> GetDebtorsDetail();
        IEnumerable<DashBoardTabularData> GetBankBalanceDetail();
        IEnumerable<DashBoardTabularData> GetVehicleStockDetail();
        IEnumerable<DashBoardTabularData> GetExpenseDetail();
        IEnumerable<DashBoardTabularData> GetCreditorsDetail();
        IEnumerable<DashBoardTabularData> GetCashBalanceDetail();
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

        public IEnumerable<DashBoardSingleValue> GetVehicleSale()
        {
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", MonthStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", MonthEndDate);

            mQry = @" SELECT Convert(NVARCHAR,Convert(DECIMAL(18,2),Round(IsNull(Sum(Hc.Amount),0)/10000000,2))) + ' Crore' AS Value
                    FROM Web.SaleInvoiceHeaders H 
                    LEFT JOIN Web.SaleInvoiceHeaderCharges Hc ON H.SaleInvoiceHeaderId = Hc.HeaderTableId
                    LEFT JOIN Web.DocumentTypes D ON H.DocTypeId = D.DocumentTypeId
                    LEFT JOIN Web.Charges C ON Hc.ChargeId = C.ChargeId
                    WHERE C.ChargeName = 'Net Amount'
                    AND  H.DocDate BETWEEN @FromDate AND @ToDate
                    AND D.DocumentCategoryId = 464 ";

            IEnumerable<DashBoardSingleValue> VehicleSale = db.Database.SqlQuery<DashBoardSingleValue>(mQry, SqlParameterFromDate, SqlParameterToDate).ToList();
            return VehicleSale;
        }
        public IEnumerable<DashBoardSingleValue> GetVehicleProfit()
        {
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", MonthStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", MonthEndDate);

            mQry = @" SELECT Convert(NVARCHAR,Convert(DECIMAL(18,2),Round(Sum(L.Amount - Purch.PurchaseAmount - IsNull(CreditNote.CreditNoteAmount,0) + IsNull(purch.RetensionTarget,0))/10000000,2))) + ' Crore' AS Value
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
            mQry = @"SELECT Convert(NVARCHAR,Convert(DECIMAL(18,0),IsNull(Sum(VStock.StockQty),0))) AS StockQty, 
                        Convert(NVARCHAR,Convert(DECIMAL(18,2),Round(IsNull(Sum(VStock.StockAmount),0)/10000000,2))) + ' Crore'   AS Value
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
                                        @" SELECT Convert(NVARCHAR,Convert(DECIMAL(18,2),Round((IsNull(Sum(VMain.Balance),0))/10000000,2))) + ' Crore' As Value
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
                                        @" SELECT Convert(NVARCHAR,Convert(DECIMAL(18,2),Round((IsNull(Sum(VMain.Balance),0))/10000000,2))) + ' Crore' As Value
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
                                        @" SELECT Convert(NVARCHAR,Convert(DECIMAL(18,2),Round((IsNull(-Sum(VMain.Balance),0))/10000000,2))) + ' Crore' As Value
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
                                        @" SELECT Convert(NVARCHAR,Convert(DECIMAL(18,2),Round((IsNull(Sum(VMain.Balance),0))/10000000,2))) + ' Crore' As Value
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
                                        @" SELECT Convert(NVARCHAR,Convert(DECIMAL(18,2),Round((IsNull(Sum(VMain.Balance),0))/100000,2))) + ' Lakh' As Value
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
        //public IEnumerable<DashBoardPieChartData> GetSpareSalePieChartData()
        //{
        //    mQry = @"DECLARE @Month INT 
        //            DECLARE @Year INT
        //            SELECT @Month =  Datepart(MONTH,getdate())
        //            SELECT @Year =  Datepart(YEAR,getdate())
        //            DECLARE @FromDate DATETIME
        //            DECLARE @ToDate DATETIME
        //            SELECT @FromDate = DATEADD(month,@Month-1,DATEADD(year,@Year-1900,0)), @ToDate = DATEADD(day,-1,DATEADD(month,@Month,DATEADD(year,@Year-1900,0))) 

        //            SELECT S.SiteName As label, Sum(Hc.Amount) AS value,
        //            CASE WHEN row_number() OVER (ORDER BY S.SiteName) = 1 THEN '#f56954'
        //              WHEN row_number() OVER (ORDER BY S.SiteName) = 2 THEN '#00a65a'
        //              WHEN row_number() OVER (ORDER BY S.SiteName) = 3 THEN '#f39c12'
        //              WHEN row_number() OVER (ORDER BY S.SiteName) = 4 THEN '#00c0ef'
        //              WHEN row_number() OVER (ORDER BY S.SiteName) = 5 THEN '#3c8dbc'
        //              WHEN row_number() OVER (ORDER BY S.SiteName) = 6 THEN '#d2d6de'
        //              ELSE '#f56954'
        //            END AS color 
        //            FROM Web.SaleInvoiceHeaders H 
        //            LEFT JOIN Web.SaleInvoiceHeaderCharges Hc ON H.SaleInvoiceHeaderId = Hc.HeaderTableId
        //            LEFT JOIN Web.DocumentTypes D ON H.DocTypeId = D.DocumentTypeId
        //            LEFT JOIN Web.Charges C ON Hc.ChargeId = C.ChargeId
        //            LEFT JOIN Web.Sites S ON h.SiteId = S.SiteId
        //            WHERE C.ChargeName = 'Net Amount'
        //            AND  H.DocDate BETWEEN @FromDate AND @ToDate
        //            AND D.DocumentCategoryId = 4012
        //            GROUP BY S.SiteName ";

        //    IEnumerable<DashBoardPieChartData> SaleSalePieChartData = db.Database.SqlQuery<DashBoardPieChartData>(mQry).ToList();
        //    return SaleSalePieChartData;
        //}
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



        public IEnumerable<DashBoardTabularData> GetVehicleSaleDetailFinancierWise()
        {
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", MonthStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", MonthEndDate);

            mQry = @"SELECT P.Name AS Head, Convert(NVARCHAR,Convert(DECIMAL(18,2),Round(IsNull(Sum(VCharge.Amount),0)/10000000,2))) + ' Crore' AS Value
                    FROM Web.SaleInvoiceHeaders H 
                    LEFT JOIN Web.DocumentTypes D ON H.DocTypeId = D.DocumentTypeId
                    LEFT JOIN Web.People P ON H.FinancierId = P.PersonID
                    LEFT JOIN 
	                    (SELECT Hc.HeaderTableId AS SaleInvoiceHeaderId, Hc.Amount 
	                    FROM Web.SaleInvoiceHeaderCharges Hc
	                    LEFT JOIN Web.Charges C ON Hc.ChargeId = C.ChargeId
	                    WHERE C.ChargeName = 'Net Amount') AS VCharge ON H.SaleInvoiceHeaderId = VCharge.SaleInvoiceHeaderId
                    WHERE H.FinancierId IS NOT NULL
                    AND  H.DocDate BETWEEN @FromDate AND @ToDate
                    AND D.DocumentCategoryId = 464
                    GROUP BY P.Name
                    ORDER BY P.Name ";

            IEnumerable<DashBoardTabularData> DashBoardTabularData = db.Database.SqlQuery<DashBoardTabularData>(mQry, SqlParameterFromDate, SqlParameterToDate).ToList();
            return DashBoardTabularData;
        }

        public IEnumerable<DashBoardTabularData> GetVehicleSaleDetailSalesManWise()
        {
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", MonthStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", MonthEndDate);

            mQry = @"SELECT P.Name AS Head, Convert(NVARCHAR,Convert(DECIMAL(18,2),Round(IsNull(Sum(VCharge.Amount),0)/10000000,2))) + ' Crore' AS Value
                    FROM Web.SaleInvoiceHeaders H 
                    LEFT JOIN Web.DocumentTypes D ON H.DocTypeId = D.DocumentTypeId
                    LEFT JOIN Web.People P ON H.SalesExecutiveId = P.PersonID
                    LEFT JOIN 
	                    (SELECT Hc.HeaderTableId AS SaleInvoiceHeaderId, Hc.Amount 
	                    FROM Web.SaleInvoiceHeaderCharges Hc
	                    LEFT JOIN Web.Charges C ON Hc.ChargeId = C.ChargeId
	                    WHERE C.ChargeName = 'Net Amount') AS VCharge ON H.SaleInvoiceHeaderId = VCharge.SaleInvoiceHeaderId
                    WHERE H.FinancierId IS NOT NULL
                    AND  H.DocDate BETWEEN @FromDate AND @ToDate
                    AND D.DocumentCategoryId = 464
                    GROUP BY P.Name
                    ORDER BY P.Name ";

            IEnumerable<DashBoardTabularData> DashBoardTabularData = db.Database.SqlQuery<DashBoardTabularData>(mQry, SqlParameterFromDate, SqlParameterToDate).ToList();
            return DashBoardTabularData;
        }

        public IEnumerable<DashBoardTabularData> GetVehicleSaleDetailProductTypeWise()
        {
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", MonthStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", MonthEndDate);

            mQry = @"SELECT VProductType.ProductTypeName AS Head, Convert(NVARCHAR,Convert(DECIMAL(18,2),Round(IsNull(Sum(VCharge.Amount),0)/10000000,2))) + ' Crore' AS Value
                        FROM Web.SaleInvoiceHeaders H 
                        LEFT JOIN Web.DocumentTypes D ON H.DocTypeId = D.DocumentTypeId
                        LEFT JOIN 
                            (SELECT Hc.HeaderTableId AS SaleInvoiceHeaderId, Hc.Amount 
                            FROM Web.SaleInvoiceHeaderCharges Hc
                            LEFT JOIN Web.Charges C ON Hc.ChargeId = C.ChargeId
                            WHERE C.ChargeName = 'Net Amount') AS VCharge ON H.SaleInvoiceHeaderId = VCharge.SaleInvoiceHeaderId
                        LEFT JOIN (
	                        SELECT H.SaleInvoiceHeaderId, Max(Pt.ProductTypeName) AS ProductTypeName
	                        FROM Web.SaleInvoiceHeaders H 
	                        LEFT JOIN Web.SaleInvoiceLines L ON H.SaleInvoiceHeaderId = L.SaleInvoiceHeaderId
	                        LEFT JOIN Web.Products P ON L.ProductId = P.ProductId
	                        LEFT JOIN Web.ProductGroups Pg ON P.ProductGroupId = Pg.ProductGroupId
	                        LEFT JOIN Web.ProductTypes Pt ON Pg.ProductTypeId = Pt.ProductTypeId
	                        LEFT JOIN Web.ProductNatures Pn ON Pt.ProductNatureId = Pn.ProductNatureId
	                        WHERE Pn.ProductNatureName = 'LOB'
	                        GROUP BY H.SaleInvoiceHeaderId) AS VProductType ON H.SaleInvoiceHeaderId = VProductType.SaleInvoiceHeaderId
                        WHERE H.DocDate BETWEEN '01/Feb/2018' AND '28/Feb/2018'
                        AND D.DocumentCategoryId = 464
                        AND VProductType.ProductTypeName IS NOT NULL
                        GROUP BY VProductType.ProductTypeName
                        ORDER BY VProductType.ProductTypeName ";

            IEnumerable<DashBoardTabularData> DashBoardTabularData = db.Database.SqlQuery<DashBoardTabularData>(mQry, SqlParameterFromDate, SqlParameterToDate).ToList();
            return DashBoardTabularData;
        }
        public IEnumerable<DashBoardTabularData> GetVehicleProfitDetail()
        {
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", MonthStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", MonthEndDate);

            mQry = @" SELECT Pt.ProductTypeName As Head, Convert(NVARCHAR,Convert(DECIMAL(18,2),Round(Sum(L.Amount - Purch.PurchaseAmount - IsNull(CreditNote.CreditNoteAmount,0) + IsNull(purch.RetensionTarget,0))/10000000,2))) + ' Crore' AS Value
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
                        AND PL.ProductUidId IS NOT NULL 
                        Group By Pt.ProductTypeName ";

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
                                        Convert(NVARCHAR,Convert(DECIMAL(18,2),Round(Sum(isnull(H.Balance,0))/10000000,2))) + ' Crore' AS Value
                                        FROM cteAcGroup H 
                                        GROUP BY H.BaseLedgerAccountGroupId 
                                        Having Sum(isnull(H.Balance,0)) <> 0 

                                        UNION ALL 

                                        SELECT Ag.LedgerAccountGroupId AS LedgerAccountGroupId, Max(Ag.LedgerAccountGroupName) AS Head, 
                                        Convert(NVARCHAR,Convert(DECIMAL(18,2),Round(Sum(isnull(H.Balance,0))/10000000,2))) + ' Crore'  AS Value
                                        FROM cteLedgerBalance H 
                                        LEFT JOIN Web.LedgerAccounts A ON H.LedgerAccountId = A.LedgerAccountId
                                        LEFT JOIN Web.LedgerAccountGroups Ag On A.LedgerAccountGroupId = Ag.LedgerAccountGroupId
                                        Where isnull(H.Balance,0) <> 0 
                                        Group By Ag.LedgerAccountGroupId ";

            IEnumerable<DashBoardTabularData> DashBoardTabularData = db.Database.SqlQuery<DashBoardTabularData>(mQry, SqlParameterSiteId, SqlParameterDivisionId, SqlParameterFromDate, SqlParameterToDate, SqlParameterCostCenter, SqlParameterLedgerAccountGroup).ToList();
            return DashBoardTabularData;
        }
        public IEnumerable<DashBoardTabularData> GetBankBalanceDetail()
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
                                        @"SELECT H.LedgerAccountId AS LedgerAccountGroupId, LedgerAccountName AS Head, 
                                        Convert(NVARCHAR,Convert(DECIMAL(18,2),Round(isnull(H.Balance,0)/10000000,2))) + ' Crore'  AS Value
                                        FROM cteLedgerBalance H 
                                        Where isnull(H.Balance,0) <> 0 ";

            IEnumerable<DashBoardTabularData> DashBoardTabularData = db.Database.SqlQuery<DashBoardTabularData>(mQry, SqlParameterSiteId, SqlParameterDivisionId, SqlParameterFromDate, SqlParameterToDate, SqlParameterCostCenter, SqlParameterLedgerAccountGroup).ToList();
            return DashBoardTabularData;
        }
        public IEnumerable<DashBoardTabularData> GetVehicleStockDetail()
        {
            mQry = @"SELECT VStock.ProductTypeName AS Head, Convert(NVARCHAR,Convert(DECIMAL(18,0),IsNull(Sum(VStock.StockQty),0))) AS StockQty, 
                        Convert(NVARCHAR,Convert(DECIMAL(18,2),Round(IsNull(Sum(VStock.StockAmount),0)/10000000,2))) + ' Crore'   AS Value
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

            IEnumerable<DashBoardTabularData> DashBoardTabularData = db.Database.SqlQuery<DashBoardTabularData>(mQry).ToList();
            return DashBoardTabularData;
        }
        public IEnumerable<DashBoardTabularData> GetExpenseDetail()
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
                                        Convert(NVARCHAR,Convert(DECIMAL(18,2),Round(Sum(isnull(H.Balance,0))/10000000,2))) + ' Crore' AS Value
                                        FROM cteAcGroup H 
                                        GROUP BY H.BaseLedgerAccountGroupId 
                                        Having Sum(isnull(H.Balance,0)) <> 0 

                                        UNION ALL 

                                        SELECT Ag.LedgerAccountGroupId AS LedgerAccountGroupId, Max(Ag.LedgerAccountGroupName) AS Head, 
                                        Convert(NVARCHAR,Convert(DECIMAL(18,2),Round(Sum(isnull(H.Balance,0))/10000000,2))) + ' Crore'  AS Value
                                        FROM cteLedgerBalance H 
                                        LEFT JOIN Web.LedgerAccounts A ON H.LedgerAccountId = A.LedgerAccountId
                                        LEFT JOIN Web.LedgerAccountGroups Ag On A.LedgerAccountGroupId = Ag.LedgerAccountGroupId
                                        Where isnull(H.Balance,0) <> 0 
                                        Group By Ag.LedgerAccountGroupId ";

            IEnumerable<DashBoardTabularData> DashBoardTabularData = db.Database.SqlQuery<DashBoardTabularData>(mQry, SqlParameterSiteId, SqlParameterDivisionId, SqlParameterFromDate, SqlParameterToDate, SqlParameterCostCenter, SqlParameterLedgerAccountGroup).ToList();
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
                                        Convert(NVARCHAR,Convert(DECIMAL(18,2),Round(-Sum(isnull(H.Balance,0))/10000000,2))) + ' Crore' AS Value
                                        FROM cteAcGroup H 
                                        GROUP BY H.BaseLedgerAccountGroupId 
                                        Having Sum(isnull(H.Balance,0)) <> 0 

                                        UNION ALL 

                                        SELECT Ag.LedgerAccountGroupId AS LedgerAccountGroupId, Max(Ag.LedgerAccountGroupName) AS Head, 
                                        Convert(NVARCHAR,Convert(DECIMAL(18,2),Round(-Sum(isnull(H.Balance,0))/10000000,2))) + ' Crore'  AS Value
                                        FROM cteLedgerBalance H 
                                        LEFT JOIN Web.LedgerAccounts A ON H.LedgerAccountId = A.LedgerAccountId
                                        LEFT JOIN Web.LedgerAccountGroups Ag On A.LedgerAccountGroupId = Ag.LedgerAccountGroupId
                                        Where isnull(H.Balance,0) <> 0 
                                        Group By Ag.LedgerAccountGroupId ";

            IEnumerable<DashBoardTabularData> DashBoardTabularData = db.Database.SqlQuery<DashBoardTabularData>(mQry, SqlParameterSiteId, SqlParameterDivisionId, SqlParameterFromDate, SqlParameterToDate, SqlParameterCostCenter, SqlParameterLedgerAccountGroup).ToList();
            return DashBoardTabularData;
        }
        public IEnumerable<DashBoardTabularData> GetCashBalanceDetail()
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
                                        Convert(NVARCHAR,Convert(DECIMAL(18,2),Round(isnull(H.Balance,0)/100000,2))) + ' Lakh'  AS Value
                                        FROM cteLedgerBalance H 
                                        Where isnull(H.Balance,0) <> 0 ";

            IEnumerable<DashBoardTabularData> DashBoardTabularData = db.Database.SqlQuery<DashBoardTabularData>(mQry, SqlParameterSiteId, SqlParameterDivisionId, SqlParameterFromDate, SqlParameterToDate, SqlParameterCostCenter, SqlParameterLedgerAccountGroup).ToList();
            return DashBoardTabularData;
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
