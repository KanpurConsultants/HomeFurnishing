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
        IEnumerable<DashBoardSale> GetVehicleSale();
        IEnumerable<DashBoardProfit> GetVehicleProfit();
        IEnumerable<DashBoardStock> GetVehicleStock();



        IEnumerable<DashBoardExpense> GetExpense();
        IEnumerable<DashBoardDebtors> GetDebtors();
        IEnumerable<DashBoardCreditors> GetCreditors();
        IEnumerable<DashBoardBankBalance> GetBankBalance();
        IEnumerable<DashBoardCashBalance> GetCashBalance();

        


        IEnumerable<DashBoardPieChartData> GetVehicleSalePieChartData();
        IEnumerable<DashBoardSaleBarChartData> GetVehicleSaleBarChartData();
        IEnumerable<DashBoardPieChartData> GetSpareSalePieChartData();
        IEnumerable<DashBoardSaleBarChartData> GetSpareSaleBarChartData();



        IEnumerable<DashBoardSaleDetailFinancierWise> GetVehicleSaleDetailFinancierWise();

    }

    public class DashBoardAutoService : IDashBoardAutoService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        string mQry = "";
        public DashBoardAutoService()
        {
        }

        public IEnumerable<DashBoardSale> GetVehicleSale()
        {
            mQry = @"DECLARE @Month INT 
                    DECLARE @Year INT
                    SELECT @Month =  Datepart(MONTH,getdate())
                    SELECT @Year =  Datepart(YEAR,getdate())
                    DECLARE @FromDate DATETIME
                    DECLARE @ToDate DATETIME
                    SELECT @FromDate = DATEADD(month,@Month-1,DATEADD(year,@Year-1900,0)), @ToDate = DATEADD(day,-1,DATEADD(month,@Month,DATEADD(year,@Year-1900,0))) 


                    SELECT Convert(NVARCHAR,Convert(DECIMAL(18,2),Round(IsNull(Sum(Hc.Amount),0)/10000000,2))) + ' Crore' AS SaleAmount
                    FROM Web.SaleInvoiceHeaders H 
                    LEFT JOIN Web.SaleInvoiceHeaderCharges Hc ON H.SaleInvoiceHeaderId = Hc.HeaderTableId
                    LEFT JOIN Web.DocumentTypes D ON H.DocTypeId = D.DocumentTypeId
                    LEFT JOIN Web.Charges C ON Hc.ChargeId = C.ChargeId
                    WHERE C.ChargeName = 'Net Amount'
                    AND  H.DocDate BETWEEN @FromDate AND @ToDate
                    AND D.DocumentCategoryId = 464 ";

            IEnumerable<DashBoardSale> VehicleSale = db.Database.SqlQuery<DashBoardSale>(mQry).ToList();
            return VehicleSale;
        }
        public IEnumerable<DashBoardProfit> GetVehicleProfit()
        {
            mQry = @"DECLARE @Month INT 
                    DECLARE @Year INT
                    SELECT @Month =  Datepart(MONTH,getdate())
                    SELECT @Year =  Datepart(YEAR,getdate())
                    DECLARE @FromDate DATETIME
                    DECLARE @ToDate DATETIME
                    SELECT @FromDate = DATEADD(month,@Month-1,DATEADD(year,@Year-1900,0)), @ToDate = DATEADD(day,-1,DATEADD(month,@Month,DATEADD(year,@Year-1900,0))) 

                    SELECT Convert(NVARCHAR,Convert(DECIMAL(18,2),Round((IsNull(Sum(VSale.SaleAmount),0) - IsNull(Sum(VPurchase.PurchaseAmount),0))/10000000,2))) + ' Crore' AS ProfitAmount
                    FROM (
	                    SELECT VProductUid.ProductUidId, Sum(Hc.Amount) AS SaleAmount
	                    FROM Web.SaleInvoiceHeaders H 
	                    LEFT JOIN Web.SaleInvoiceHeaderCharges Hc ON H.SaleInvoiceHeaderId = Hc.HeaderTableId
	                    LEFT JOIN (
		                    SELECT Max(Pl.ProductUidId) AS ProductUidId, L.SaleInvoiceHeaderId
		                    FROM Web.SaleInvoiceLines L 
		                    LEFT JOIN Web.SaleDispatchLines Sdl ON L.SaleDispatchLineId = Sdl.SaleDispatchLineId
		                    LEFT JOIN Web.PackingLines Pl ON Sdl.PackingLineId = Pl.PackingLineId
		                    WHERE Pl.ProductUidId IS NOT NULL
		                    GROUP BY L.SaleInvoiceHeaderId
	                    ) AS VProductUid ON H.SaleInvoiceHeaderId = VProductUid.SaleInvoiceHeaderId
	                    LEFT JOIN Web.DocumentTypes D ON H.DocTypeId = D.DocumentTypeId
	                    LEFT JOIN Web.Charges C ON Hc.ChargeId = C.ChargeId
	                    WHERE C.ChargeName = 'Net Amount'
	                    AND  H.DocDate BETWEEN @FromDate AND @ToDate
	                    AND D.DocumentCategoryId = 464
	                    GROUP BY VProductUid.ProductUidId
                    ) AS VSale
                    LEFT JOIN (
	                    SELECT VProductUid.ProductUidId, Sum(Hc.Amount) AS PurchaseAmount
	                    FROM Web.JobInvoiceHeaders H 
	                    LEFT JOIN Web.JobInvoiceHeaderCharges Hc ON H.JobInvoiceHeaderId = Hc.HeaderTableId
	                    LEFT JOIN (
		                    SELECT Max(Jrl.ProductUidId) AS ProductUidId, L.JobInvoiceHeaderId
		                    FROM Web.JobInvoiceLines L 
		                    LEFT JOIN Web.JobReceiveLines Jrl ON L.JobReceiveLineId = Jrl.JobReceiveLineId
		                    WHERE Jrl.ProductUidId IS NOT NULL
		                    GROUP BY L.JobInvoiceHeaderId
	                    ) AS VProductUid ON H.JobInvoiceHeaderId = VProductUid.JobInvoiceHeaderId
	                    LEFT JOIN Web.DocumentTypes D ON H.DocTypeId = D.DocumentTypeId
	                    LEFT JOIN Web.Charges C ON Hc.ChargeId = C.ChargeId
	                    WHERE C.ChargeName = 'Net Amount'
	                    AND D.DocumentCategoryId = 461
	                    GROUP BY VProductUid.ProductUidId
                    ) AS VPurchase ON VSale.ProductUidId = VPurchase.ProductUidId ";


            IEnumerable<DashBoardProfit> VehicleProfit = db.Database.SqlQuery<DashBoardProfit>(mQry).ToList();
            return VehicleProfit;
        }
        public IEnumerable<DashBoardStock> GetVehicleStock()
        {
            mQry = @"SELECT Convert(NVARCHAR,Convert(DECIMAL(18,0),IsNull(Sum(VStock.StockQty),0))) AS StockQty, 
                        Convert(NVARCHAR,Convert(DECIMAL(18,2),Round(IsNull(Sum(VStock.StockAmount),0)/10000000,2))) + ' Crore'   AS StockAmount
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


            IEnumerable<DashBoardStock> VehicleStock = db.Database.SqlQuery<DashBoardStock>(mQry).ToList();
            return VehicleStock;
        }



        public IEnumerable<DashBoardExpense> GetExpense()
        {
            mQry = @" SELECT '10 Crore' AS ExpenseAmount ";

            IEnumerable<DashBoardExpense> Expense = db.Database.SqlQuery<DashBoardExpense>(mQry).ToList();
            return Expense;
        }
        public IEnumerable<DashBoardDebtors> GetDebtors()
        {
            mQry = @" SELECT '11 Crore' AS DebtorsAmount ";

            IEnumerable<DashBoardDebtors> Debtors = db.Database.SqlQuery<DashBoardDebtors>(mQry).ToList();
            return Debtors;
        }
        public IEnumerable<DashBoardCreditors> GetCreditors()
        {
            mQry = @" SELECT '12 Crore' AS CreditorsAmount ";

            IEnumerable<DashBoardCreditors> Creditors = db.Database.SqlQuery<DashBoardCreditors>(mQry).ToList();
            return Creditors;
        }
        public IEnumerable<DashBoardBankBalance> GetBankBalance()
        {
            mQry = @" SELECT '13 Crore' AS BankBalanceAmount ";

            IEnumerable<DashBoardBankBalance> BankBalance = db.Database.SqlQuery<DashBoardBankBalance>(mQry).ToList();
            return BankBalance;
        }
        public IEnumerable<DashBoardCashBalance> GetCashBalance()
        {
            mQry = @" SELECT '14 Crore' AS CashBalanceAmount ";

            IEnumerable<DashBoardCashBalance> CashBalance = db.Database.SqlQuery<DashBoardCashBalance>(mQry).ToList();
            return CashBalance;
        }
        public IEnumerable<DashBoardPieChartData> GetVehicleSalePieChartData()
        {
            mQry = @"DECLARE @Month INT 
                    DECLARE @Year INT
                    SELECT @Month =  Datepart(MONTH,getdate())
                    SELECT @Year =  Datepart(YEAR,getdate())
                    DECLARE @FromDate DATETIME
                    DECLARE @ToDate DATETIME
                    SELECT @FromDate = DATEADD(month,@Month-1,DATEADD(year,@Year-1900,0)), @ToDate = DATEADD(day,-1,DATEADD(month,@Month,DATEADD(year,@Year-1900,0))) 

                    SELECT S.SiteName As label, Sum(Hc.Amount) AS value,
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

            IEnumerable<DashBoardPieChartData> VehicleSalePieChartData = db.Database.SqlQuery<DashBoardPieChartData>(mQry).ToList();
            return VehicleSalePieChartData;
        }
        public IEnumerable<DashBoardSaleBarChartData> GetVehicleSaleBarChartData()
        {
            mQry = @"SELECT DATENAME(month, H.DocDate) AS Month, 
                    Round(Sum(Hc.Amount)/10000000,2) AS Amount
                    FROM Web.SaleInvoiceHeaders H 
                    LEFT JOIN Web.SaleInvoiceHeaderCharges Hc ON H.SaleInvoiceHeaderId = Hc.HeaderTableId
                    LEFT JOIN Web.DocumentTypes D ON H.DocTypeId = D.DocumentTypeId
                    LEFT JOIN Web.Charges C ON Hc.ChargeId = C.ChargeId
                    WHERE C.ChargeName = 'Net Amount'
                    AND D.DocumentCategoryId = 464
                    GROUP BY DATENAME(month, H.DocDate)
                    ORDER BY DatePart(month,Max(H.DocDate)) ";

            IEnumerable<DashBoardSaleBarChartData> ChartData = db.Database.SqlQuery<DashBoardSaleBarChartData>(mQry).ToList();
            return ChartData;
        }
        public IEnumerable<DashBoardPieChartData> GetSpareSalePieChartData()
        {
            mQry = @"DECLARE @Month INT 
                    DECLARE @Year INT
                    SELECT @Month =  Datepart(MONTH,getdate())
                    SELECT @Year =  Datepart(YEAR,getdate())
                    DECLARE @FromDate DATETIME
                    DECLARE @ToDate DATETIME
                    SELECT @FromDate = DATEADD(month,@Month-1,DATEADD(year,@Year-1900,0)), @ToDate = DATEADD(day,-1,DATEADD(month,@Month,DATEADD(year,@Year-1900,0))) 

                    SELECT S.SiteName As label, Sum(Hc.Amount) AS value,
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

            IEnumerable<DashBoardPieChartData> SaleSalePieChartData = db.Database.SqlQuery<DashBoardPieChartData>(mQry).ToList();
            return SaleSalePieChartData;
        }
        public IEnumerable<DashBoardSaleBarChartData> GetSpareSaleBarChartData()
        {
            mQry = @"SELECT DATENAME(month, H.DocDate) AS Month, 
                    Round(Sum(Hc.Amount)/10000000,2) AS Amount
                    FROM Web.SaleInvoiceHeaders H 
                    LEFT JOIN Web.SaleInvoiceHeaderCharges Hc ON H.SaleInvoiceHeaderId = Hc.HeaderTableId
                    LEFT JOIN Web.DocumentTypes D ON H.DocTypeId = D.DocumentTypeId
                    LEFT JOIN Web.Charges C ON Hc.ChargeId = C.ChargeId
                    WHERE C.ChargeName = 'Net Amount'
                    AND D.DocumentCategoryId = 464
                    GROUP BY DATENAME(month, H.DocDate)
                    ORDER BY DatePart(month,Max(H.DocDate)) ";

            IEnumerable<DashBoardSaleBarChartData> ChartData = db.Database.SqlQuery<DashBoardSaleBarChartData>(mQry).ToList();
            return ChartData;
        }



        public IEnumerable<DashBoardSaleDetailFinancierWise> GetVehicleSaleDetailFinancierWise()
        {
            mQry = @"SELECT P.Name AS FinancierName, Convert(NVARCHAR,Convert(DECIMAL(18,2),Round(IsNull(Sum(VCharge.Amount),0)/10000000,2))) + ' Crore' AS Amount
                    FROM Web.SaleInvoiceHeaders H 
                    LEFT JOIN Web.People P ON H.FinancierId = P.PersonID
                    LEFT JOIN 
	                    (SELECT Hc.HeaderTableId AS SaleInvoiceHeaderId, Hc.Amount 
	                    FROM Web.SaleInvoiceHeaderCharges Hc
	                    LEFT JOIN Web.Charges C ON Hc.ChargeId = C.ChargeId
	                    WHERE C.ChargeName = 'Net Amount') AS VCharge ON H.SaleInvoiceHeaderId = VCharge.SaleInvoiceHeaderId
                    WHERE H.FinancierId IS NOT NULL
                    GROUP BY P.Name
                    ORDER BY P.Name ";

            IEnumerable<DashBoardSaleDetailFinancierWise> VehicleSaleDetailFinancierWise = db.Database.SqlQuery<DashBoardSaleDetailFinancierWise>(mQry).ToList();
            return VehicleSaleDetailFinancierWise;
        }

        public IEnumerable<DashBoardSaleDetailAgentWise> GetVehicleSaleDetailAgentWise()
        {
            mQry = @"SELECT P.Name AS FinancierName, Convert(NVARCHAR,Convert(DECIMAL(18,2),Round(IsNull(Sum(VCharge.Amount),0)/10000000,2))) + ' Crore' AS Amount
                    FROM Web.SaleInvoiceHeaders H 
                    LEFT JOIN Web.People P ON H.FinancierId = P.PersonID
                    LEFT JOIN 
	                    (SELECT Hc.HeaderTableId AS SaleInvoiceHeaderId, Hc.Amount 
	                    FROM Web.SaleInvoiceHeaderCharges Hc
	                    LEFT JOIN Web.Charges C ON Hc.ChargeId = C.ChargeId
	                    WHERE C.ChargeName = 'Net Amount') AS VCharge ON H.SaleInvoiceHeaderId = VCharge.SaleInvoiceHeaderId
                    WHERE H.FinancierId IS NOT NULL
                    GROUP BY P.Name
                    ORDER BY P.Name ";

            IEnumerable<DashBoardSaleDetailAgentWise> VehicleSaleDetailAgentWise = db.Database.SqlQuery<DashBoardSaleDetailAgentWise>(mQry).ToList();
            return VehicleSaleDetailAgentWise;
        }


        public void Dispose()
        {
        }
    }

    public class DashBoardSale
    {
        public string SaleAmount { get; set; }
    }
    public class DashBoardProfit
    {
        public string ProfitAmount { get; set; }
    }
    public class DashBoardStock
    {
        public string StockQty { get; set; }
        public string StockAmount { get; set; }
    }
    public class DashBoardExpense
    {
        public string ExpenseAmount { get; set; }
    }
    public class DashBoardDebtors
    {
        public string DebtorsAmount { get; set; }
    }
    public class DashBoardCreditors
    {
        public string CreditorsAmount { get; set; }
    }
    public class DashBoardBankBalance
    {
        public string BankBalanceAmount { get; set; }
    }
    public class DashBoardCashBalance
    {
        public string CashBalanceAmount { get; set; }
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



    public class DashBoardSaleDetailFinancierWise
    {
        public string FinancierName { get; set; }
        public string Amount { get; set; }
    }

    public class DashBoardSaleDetailAgentWise
    {
        public string AgentName { get; set; }
        public string Amount { get; set; }
    }

}
