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
using System.Data;

namespace Service
{
    public interface IFinancialDisplayService : IDisposable
    {
        IEnumerable<TrialBalanceViewModel> GetTrialBalance(FinancialDisplaySettings Settings);
        IEnumerable<TrialBalanceSummaryViewModel> GetTrialBalanceSummary(FinancialDisplaySettings Settings);
        IEnumerable<TrialBalanceViewModel> GetTrialBalanceDetail(FinancialDisplaySettings Settings);
        IEnumerable<TrialBalanceViewModel> GetTrialBalanceDetailWithFullHierarchy(FinancialDisplaySettings Settings);
        IEnumerable<TrialBalanceSummaryViewModel> GetTrialBalanceDetailSummary(FinancialDisplaySettings Settings);
        IEnumerable<TrialBalanceSummaryViewModel> GetTrialBalanceDetailSummaryWithFullHierarchy(FinancialDisplaySettings Settings);
        IEnumerable<SubTrialBalanceViewModel> GetSubTrialBalance(FinancialDisplaySettings Settings);
        IEnumerable<SubTrialBalanceSummaryViewModel> GetSubTrialBalanceSummary(FinancialDisplaySettings Settings);
        IEnumerable<LedgerBalanceViewModel> GetLedgerBalance(FinancialDisplaySettings Settings);
        IEnumerable<BalanceSheetViewModel> GetBalanceSheet(FinancialDisplaySettings Settings);
        IEnumerable<BalanceSheetViewModel> GetBalanceSheetDetail(FinancialDisplaySettings Settings);
        IEnumerable<ProfitAndLossViewModel> GetProfitAndLoss(FinancialDisplaySettings Settings);
        IEnumerable<ProfitAndLossViewModel> GetProfitAndLossDetail(FinancialDisplaySettings Settings);
    }

    public class FinancialDisplayService : IFinancialDisplayService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;

        public static TrialBalanceDetailViewModel TempVar = new TrialBalanceDetailViewModel();



        public FinancialDisplayService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public IEnumerable<TrialBalanceViewModel> GetTrialBalance(FinancialDisplaySettings Settings)
        {
            var SiteSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "Site" select H).FirstOrDefault();
            var DivisionSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "Division" select H).FirstOrDefault();
            var FromDateSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "FromDate" select H).FirstOrDefault();
            var ToDateSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "ToDate" select H).FirstOrDefault();
            var CostCenterSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "CostCenter" select H).FirstOrDefault();
            var IsIncludeZeroBalanceSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "IsIncludeZeroBalance" select H).FirstOrDefault();
            var IsIncludeOpeningSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "IsIncludeOpening" select H).FirstOrDefault();

            var LedgerAccountGroupSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "LedgerAccountGroup" select H).FirstOrDefault();


            string SiteId = SiteSetting.Value;
            string DivisionId = DivisionSetting.Value;
            string FromDate = FromDateSetting.Value;
            string ToDate = ToDateSetting.Value;
            string CostCenterId = CostCenterSetting.Value;
            string IsIncludeZeroBalance = IsIncludeZeroBalanceSetting.Value;
            string IsIncludeOpening = IsIncludeOpeningSetting.Value;
            string LedgerAccountGroup = LedgerAccountGroupSetting.Value;


            SqlParameter SqlParameterSiteId = new SqlParameter("@Site", !string.IsNullOrEmpty(SiteId) ? SiteId : (object)DBNull.Value);
            SqlParameter SqlParameterDivisionId = new SqlParameter("@Division", !string.IsNullOrEmpty(DivisionId) ? DivisionId : (object)DBNull.Value);
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", FromDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", ToDate);
            SqlParameter SqlParameterCostCenter = new SqlParameter("@CostCenter", !string.IsNullOrEmpty(CostCenterId) ? CostCenterId : (object)DBNull.Value);
            SqlParameter SqlParameterLedgerAccountGroup = new SqlParameter("@LedgerAccountGroup", !string.IsNullOrEmpty(LedgerAccountGroup) ? LedgerAccountGroup : (object)DBNull.Value);


            string mQry = GetQryForTrialBalance(SiteId, DivisionId, FromDate, ToDate, CostCenterId, IsIncludeZeroBalance, IsIncludeOpening, LedgerAccountGroup) +
                                        @"SELECT H.BaseLedgerAccountGroupId AS LedgerAccountGroupId, '<Strong>' +  Max(BaseLedgerAccountGroupName) + '</Strong>' AS LedgerAccountGroupName, 
                                        CASE WHEN Sum(isnull(H.Balance,0)) > 0 THEN Sum(isnull(H.Balance,0)) ELSE NULL  END AS AmtDr,
                                        CASE WHEN Sum(isnull(H.Balance,0)) < 0 THEN abs(Sum(isnull(H.Balance,0))) ELSE NULL END AS AmtCr,
                                        NULL AS LedgerAccountId, 'Trial Balance' ReportType, 'Trial Balance' AS OpenReportType, Max(BaseLedgerAccountGroupName) As OrderByColumn
                                        FROM cteAcGroup H 
                                        GROUP BY H.BaseLedgerAccountGroupId 
                                        Having (Sum(isnull(H.Opening,0)) <> 0 Or Sum(isnull(H.AmtDr,0)) <> 0 Or Sum(isnull(H.AmtCr,0)) <> 0 Or Sum(isnull(H.Balance,0)) <> 0) " +
                                        (IsIncludeZeroBalance == "False" ? " And Sum(isnull(H.Balance,0)) <> 0 " : "") +

                                        @"UNION ALL 
                
                                        SELECT H.LedgerAccountId AS LedgerAccountGroupId, LedgerAccountName AS LedgerAccountGroupName, 
                                        CASE WHEN isnull(H.Balance,0) > 0 THEN isnull(H.Balance,0) ELSE NULL  END AS AmtDr,
                                        CASE WHEN isnull(H.Balance,0) < 0 THEN abs(isnull(H.Balance,0)) ELSE NULL END AS AmtCr,
                                        H.LedgerAccountId AS LedgerAccountId, 'Trial Balance' ReportType, 'Ledger' AS OpenReportType, LedgerAccountName  As OrderByColumn
                                        FROM cteLedgerBalance H 
                                        Where (isnull(H.Opening,0) <> 0 Or isnull(H.AmtDr,0) <> 0 Or isnull(H.AmtCr,0) <> 0 Or isnull(H.Balance,0) <> 0) " +
                                        (IsIncludeZeroBalance == "False" ? " And isnull(H.Balance,0) <> 0 " : "") +
                                        @" ORDER BY OrderByColumn ";


            List<TrialBalanceViewModel> TrialBalanceList = db.Database.SqlQuery<TrialBalanceViewModel>(mQry, SqlParameterSiteId, SqlParameterDivisionId, SqlParameterFromDate, SqlParameterToDate, SqlParameterCostCenter, SqlParameterLedgerAccountGroup).ToList();

            decimal TotalAmtDr = TrialBalanceList.Sum(i => i.AmtDr).Value;
            decimal TotalAmtCr = TrialBalanceList.Sum(i => i.AmtCr).Value;

            if (LedgerAccountGroup == null || LedgerAccountGroup == "" || LedgerAccountGroup == "0")
                if (TotalAmtDr != TotalAmtCr)
                {
                    TrialBalanceViewModel BlankItem = new TrialBalanceViewModel();
                    TrialBalanceList.Add(BlankItem);
                    TrialBalanceViewModel DiffVm = new TrialBalanceViewModel();
                    DiffVm.LedgerAccountGroupName = "<Strong>Difference in Trial Balance</Strong>";
                    if (TotalAmtDr > TotalAmtCr)
                        DiffVm.AmtCr = TotalAmtDr - TotalAmtCr;
                    else
                        DiffVm.AmtDr = TotalAmtCr - TotalAmtDr;
                    TrialBalanceList.Add(DiffVm);
                }


            return TrialBalanceList;

        }

        public IEnumerable<TrialBalanceSummaryViewModel> GetTrialBalanceSummary(FinancialDisplaySettings Settings)
        {
            var SiteSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "Site" select H).FirstOrDefault();
            var DivisionSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "Division" select H).FirstOrDefault();
            var FromDateSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "FromDate" select H).FirstOrDefault();
            var ToDateSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "ToDate" select H).FirstOrDefault();
            var CostCenterSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "CostCenter" select H).FirstOrDefault();
            var IsIncludeZeroBalanceSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "IsIncludeZeroBalance" select H).FirstOrDefault();
            var IsIncludeOpeningSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "IsIncludeOpening" select H).FirstOrDefault();

            var LedgerAccountGroupSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "LedgerAccountGroup" select H).FirstOrDefault();


            string SiteId = SiteSetting.Value;
            string DivisionId = DivisionSetting.Value;
            string FromDate = FromDateSetting.Value;
            string ToDate = ToDateSetting.Value;
            string CostCenterId = CostCenterSetting.Value;
            string IsIncludeZeroBalance = IsIncludeZeroBalanceSetting.Value;
            string IsIncludeOpening = IsIncludeOpeningSetting.Value;
            string LedgerAccountGroup = LedgerAccountGroupSetting.Value;


            SqlParameter SqlParameterSiteId = new SqlParameter("@Site", !string.IsNullOrEmpty(SiteId) ? SiteId : (object)DBNull.Value);
            SqlParameter SqlParameterDivisionId = new SqlParameter("@Division", !string.IsNullOrEmpty(DivisionId) ? DivisionId : (object)DBNull.Value);
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", FromDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", ToDate);
            SqlParameter SqlParameterCostCenter = new SqlParameter("@CostCenter", !string.IsNullOrEmpty(CostCenterId) ? CostCenterId : (object)DBNull.Value);
            SqlParameter SqlParameterLedgerAccountGroup = new SqlParameter("@LedgerAccountGroup", !string.IsNullOrEmpty(LedgerAccountGroup) ? LedgerAccountGroup : (object)DBNull.Value);


            string mQry = GetQryForTrialBalance(SiteId, DivisionId, FromDate, ToDate, CostCenterId, IsIncludeZeroBalance, IsIncludeOpening, LedgerAccountGroup) +
                            @"SELECT H.BaseLedgerAccountGroupId AS LedgerAccountGroupId, '<Strong>' +  Max(BaseLedgerAccountGroupName) + '</Strong>' AS LedgerAccountGroupName, 
                                CASE WHEN Sum(isnull(H.Opening,0)) <> 0 THEN Abs(Sum(isnull(H.Opening,0))) ELSE NULL END AS Opening,
                                CASE WHEN Sum(isnull(H.Opening,0)) > 0 THEN 'Dr' 
	                                 WHEN Sum(isnull(H.Opening,0)) < 0 THEN 'Cr' 
                                ELSE NULL END AS OpeningDrCr,
                                CASE WHEN Sum(isnull(H.AmtDr,0)) <> 0 Then Sum(isnull(H.AmtDr,0)) Else Null End AS AmtDr,
                                CASE WHEN Sum(isnull(H.AmtCr,0)) <> 0 Then Sum(isnull(H.AmtCr,0)) Else Null End AS AmtCr,
                                CASE WHEN Abs(Sum(isnull(H.Balance,0))) <> 0 Then Abs(Sum(isnull(H.Balance,0))) Else Null End As Balance,
                                CASE WHEN Sum(isnull(H.Balance,0)) > 0 THEN 'Dr' 
	                                 WHEN Sum(isnull(H.Balance,0)) < 0 THEN 'Cr' 
                                ELSE NULL END AS BalanceDrCr,
                                NULL AS LedgerAccountId, 'Trial Balance' ReportType, 'Trial Balance' AS OpenReportType, Max(BaseLedgerAccountGroupName) As OrderByColumn
                                FROM cteAcGroup H 
                                GROUP BY H.BaseLedgerAccountGroupId
                                Having (Sum(isnull(H.Opening,0)) <> 0 Or Sum(isnull(H.AmtDr,0)) <> 0 Or Sum(isnull(H.AmtCr,0)) <> 0 Or Sum(isnull(H.Balance,0)) <> 0) " +
                                (IsIncludeZeroBalance == "False" ? " And Sum(isnull(H.Balance,0)) <> 0 " : "") +

                                @" UNION ALL 

                                SELECT H.LedgerAccountId AS LedgerAccountGroupId, LedgerAccountName AS LedgerAccountGroupName, 
                                CASE WHEN isnull(H.Opening,0) <> 0 THEN Abs(isnull(H.Opening,0)) ELSE NULL END AS Opening,
                                CASE WHEN isnull(H.Opening,0) > 0 THEN 'Dr' 
	                                 WHEN isnull(H.Opening,0) < 0 THEN 'Cr' 
                                ELSE NULL END AS OpeningDrCr,
                                CASE WHEN isnull(H.AmtDr,0) <> 0 Then isnull(H.AmtDr,0) Else Null End AS AmtDr,
                                CASE WHEN isnull(H.AmtCr,0) <> 0 Then isnull(H.AmtCr,0) Else Null End AS AmtCr,
                                CASE WHEN Abs(isnull(H.Balance,0)) <> 0 Then Abs(isnull(H.Balance,0)) Else Null End As Balance,
                                CASE WHEN isnull(H.Balance,0) > 0 THEN 'Dr' 
	                                 WHEN isnull(H.Balance,0) < 0 THEN 'Cr' 
                                ELSE NULL END AS BalanceDrCr,
                                H.LedgerAccountId AS LedgerAccountId, 'Trial Balance' ReportType, 'Ledger' AS OpenReportType, LedgerAccountName  As OrderByColumn
                                FROM cteLedgerBalance H 
                                Where (isnull(H.Opening,0) <> 0 Or isnull(H.AmtDr,0) <> 0 Or isnull(H.AmtCr,0) <> 0 Or isnull(H.Balance,0) <> 0) " +
                                (IsIncludeZeroBalance == "False" ? " And isnull(H.Balance,0) <> 0 " : "") +
                                @" ORDER BY OrderByColumn ";

            IEnumerable<TrialBalanceSummaryViewModel> TrialBalanceSummaryList = db.Database.SqlQuery<TrialBalanceSummaryViewModel>(mQry, SqlParameterSiteId, SqlParameterDivisionId, SqlParameterFromDate, SqlParameterToDate, SqlParameterCostCenter, SqlParameterLedgerAccountGroup).ToList();

            Decimal TotalAmtDr = 0;
            Decimal TotalAmtCr = 0;
            Decimal TotalOpening = 0;
            string TotalOpeningDrCr = "";
            Decimal TotalBalance = 0;
            string TotalBalanceDrCr = "";

            TotalAmtDr = TrialBalanceSummaryList.Sum(m => m.AmtDr) ?? 0;
            TotalAmtCr = TrialBalanceSummaryList.Sum(m => m.AmtCr) ?? 0;

            foreach (var item in TrialBalanceSummaryList)
            {
                if (item.OpeningDrCr == "Dr")
                    TotalOpening = TotalOpening + (item.Opening ?? 0);
                else
                    TotalOpening = TotalOpening - (item.Opening ?? 0);

                if (item.BalanceDrCr == "Dr")
                    TotalBalance = TotalBalance + (item.Balance ?? 0);
                else
                    TotalBalance = TotalBalance - (item.Balance ?? 0);
            }

            if (TotalOpening > 0)
                TotalOpeningDrCr = "Dr";
            else if (TotalOpening < 0)
                TotalOpeningDrCr = "Cr";

            if (TotalBalance > 0)
                TotalBalanceDrCr = "Dr";
            else if (TotalBalance < 0)
                TotalBalanceDrCr = "Cr";


            foreach (var item in TrialBalanceSummaryList)
            {
                item.TotalAmtDr = TotalAmtDr;
                item.TotalAmtCr = TotalAmtCr;

                item.TotalOpening = Math.Abs(TotalOpening);
                item.TotalOpeningDrCr = TotalOpeningDrCr;
                item.TotalBalance = Math.Abs(TotalBalance);
                item.TotalBalanceDrCr = TotalBalanceDrCr;

            }

            return TrialBalanceSummaryList;

        }


        public IEnumerable<SubTrialBalanceViewModel> GetSubTrialBalance(FinancialDisplaySettings Settings)
        {
            var SiteSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "Site" select H).FirstOrDefault();
            var DivisionSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "Division" select H).FirstOrDefault();
            var FromDateSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "FromDate" select H).FirstOrDefault();
            var ToDateSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "ToDate" select H).FirstOrDefault();
            var CostCenterSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "CostCenter" select H).FirstOrDefault();
            var IsIncludeZeroBalanceSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "IsIncludeZeroBalance" select H).FirstOrDefault();
            var IsIncludeOpeningSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "IsIncludeOpening" select H).FirstOrDefault();

            var LedgerAccountGroupSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "LedgerAccountGroup" select H).FirstOrDefault();



            string SiteId = SiteSetting.Value;
            string DivisionId = DivisionSetting.Value;
            string FromDate = FromDateSetting.Value;
            string ToDate = ToDateSetting.Value;
            string CostCenterId = CostCenterSetting.Value;
            string IsIncludeZeroBalance = IsIncludeZeroBalanceSetting.Value;
            string IsIncludeOpening = IsIncludeOpeningSetting.Value;
            string LedgerAccountGroup = LedgerAccountGroupSetting.Value;

            SqlParameter SqlParameterSiteId = new SqlParameter("@Site", !string.IsNullOrEmpty(SiteId) ? SiteId : (object)DBNull.Value);
            SqlParameter SqlParameterDivisionId = new SqlParameter("@Division", !string.IsNullOrEmpty(DivisionId) ? DivisionId : (object)DBNull.Value);
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", FromDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", ToDate);
            SqlParameter SqlParameterCostCenter = new SqlParameter("@CostCenter", !string.IsNullOrEmpty(CostCenterId) ? CostCenterId : (object)DBNull.Value);
            SqlParameter SqlParameterLedgerAccountGroup = new SqlParameter("@LedgerAccountGroup", !string.IsNullOrEmpty(LedgerAccountGroup) ? LedgerAccountGroup : (object)DBNull.Value);


            string mQry = @"SELECT LA.LedgerAccountId, Max(LA.LedgerAccountName + 
		                    Case WHEN P.PersonId IS NOT NULL 
			                     THEN CASE WHEN P.Suffix = P.Code THEN ' [' + P.Code + ']'
			 								                      ELSE ', ' + P.Suffix + ' [' + P.Code + ']' END	
			                     ELSE  ', ' + LA.LedgerAccountSuffix END) AS LedgerAccountName, max(LAG.LedgerAccountGroupName) AS LedgerAccountGroupName, 
                            CASE WHEN sum(isnull(H.AmtDr,0)) - sum(isnull(H.AmtCr,0)) > 0 THEN sum(isnull(H.AmtDr,0)) - sum(isnull(H.AmtCr,0)) ELSE NULL  END AS AmtDr,
                            CASE WHEN sum(isnull(H.AmtDr,0)) - sum(isnull(H.AmtCr,0)) < 0 THEN abs(sum(isnull(H.AmtDr,0)) - sum(isnull(H.AmtCr,0))) ELSE NULL END AS AmtCr,
                            'Sub Trial Balance' AS ReportType, 'Ledger' AS OpenReportType    
                            FROM web.Ledgers H  WITH (Nolock) 
                            LEFT JOIN web.LedgerHeaders LH WITH (Nolock) ON LH.LedgerHeaderId = H.LedgerHeaderId 
                            LEFT JOIN web.LedgerAccounts LA  WITH (Nolock) ON LA.LedgerAccountId = H.LedgerAccountId 
                            LEFT JOIN web.LedgerAccountGroups LAG  WITH (Nolock) ON LAG.LedgerAccountGroupId = LA.LedgerAccountGroupId 
                            LEFT JOIN Web.People P On LA.PersonId = P.PersonId
                            WHERE 1 = 1 
                            AND LH.DocDate <= @ToDate
                            AND LH.DocDate >= (Case When LAG.LedgerAccountGroupNature in ('Expense','Income') Then @FromDate Else '01/01/1900' End) " +
                            (SiteId != null ? " AND LH.SiteId IN (SELECT Items FROM [dbo].[Split] (@Site, ','))" : "") +
                            (DivisionId != null ? " AND LH.DivisionId IN (SELECT Items FROM [dbo].[Split] (@Division, ','))" : "") +
                            (CostCenterId != null ? " AND H.CostCenterId IN (SELECT Items FROM [dbo].[Split] (@CostCenter, ','))" : "") +
                            (LedgerAccountGroup != null ? " AND LAG.LedgerAccountGroupId = @LedgerAccountGroup " : "") +
                            (IsIncludeOpening == "False" ? " AND LH.DocDate >= @FromDate" : "") +
                            @" GROUP BY LA.LedgerAccountId  " +
                            (IsIncludeZeroBalance == "False" ? " HAVING sum(isnull(H.AmtDr,0))  - sum(isnull(H.AmtCr,0)) <> 0 " : "") +
                            "Order By LedgerAccountName ";

            List<SubTrialBalanceViewModel> SubTrialBalanceList = db.Database.SqlQuery<SubTrialBalanceViewModel>(mQry, SqlParameterSiteId, SqlParameterDivisionId, SqlParameterFromDate, SqlParameterToDate, SqlParameterCostCenter, SqlParameterLedgerAccountGroup).ToList();

            decimal TotalAmtDr = SubTrialBalanceList.Sum(i => i.AmtDr).Value;
            decimal TotalAmtCr = SubTrialBalanceList.Sum(i => i.AmtCr).Value;

            if (TotalAmtDr != TotalAmtCr)
            {
                SubTrialBalanceViewModel BlankItem = new SubTrialBalanceViewModel();
                SubTrialBalanceList.Add(BlankItem);
                SubTrialBalanceViewModel DiffVm = new SubTrialBalanceViewModel();
                DiffVm.LedgerAccountName = "Difference in Trial Balance";
                if (TotalAmtDr > TotalAmtCr)
                    DiffVm.AmtCr = TotalAmtDr - TotalAmtCr;
                else
                    DiffVm.AmtDr = TotalAmtCr - TotalAmtDr;
                SubTrialBalanceList.Add(DiffVm);
            }


            return SubTrialBalanceList;

        }


        public IEnumerable<SubTrialBalanceSummaryViewModel> GetSubTrialBalanceSummary(FinancialDisplaySettings Settings)
        {
            var SiteSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "Site" select H).FirstOrDefault();
            var DivisionSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "Division" select H).FirstOrDefault();
            var FromDateSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "FromDate" select H).FirstOrDefault();
            var ToDateSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "ToDate" select H).FirstOrDefault();
            var CostCenterSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "CostCenter" select H).FirstOrDefault();
            var IsIncludeZeroBalanceSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "IsIncludeZeroBalance" select H).FirstOrDefault();
            var IsIncludeOpeningSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "IsIncludeOpening" select H).FirstOrDefault();

            var LedgerAccountGroupSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "LedgerAccountGroup" select H).FirstOrDefault();

            string SiteId = SiteSetting.Value;
            string DivisionId = DivisionSetting.Value;
            string FromDate = FromDateSetting.Value;
            string ToDate = ToDateSetting.Value;
            string CostCenterId = CostCenterSetting.Value;
            string IsIncludeZeroBalance = IsIncludeZeroBalanceSetting.Value;
            string IsIncludeOpening = IsIncludeOpeningSetting.Value;
            string LedgerAccountGroup = LedgerAccountGroupSetting.Value;

            SqlParameter SqlParameterSiteId = new SqlParameter("@Site", !string.IsNullOrEmpty(SiteId) ? SiteId : (object)DBNull.Value);
            SqlParameter SqlParameterDivisionId = new SqlParameter("@Division", !string.IsNullOrEmpty(DivisionId) ? DivisionId : (object)DBNull.Value);
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", FromDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", ToDate);
            SqlParameter SqlParameterCostCenter = new SqlParameter("@CostCenter", !string.IsNullOrEmpty(CostCenterId) ? CostCenterId : (object)DBNull.Value);
            SqlParameter SqlParameterLedgerAccountGroup = new SqlParameter("@LedgerAccountGroup", !string.IsNullOrEmpty(LedgerAccountGroup) ? LedgerAccountGroup : (object)DBNull.Value);


            //            string mCondStr = "";
            //            if (SiteId != null) mCondStr = mCondStr + " AND LH.SiteId IN (SELECT Items FROM [dbo].[Split] (@Site, ','))";
            //            if (DivisionId != null) mCondStr = mCondStr + " AND LH.SiteId IN (SELECT Items FROM [dbo].[Split] (@Division, ','))";
            //            if (CostCenterId != null) mCondStr = mCondStr + " AND H.CostCenter IN (SELECT Items FROM [dbo].[Split] (@CostCenter, ','))";
            //            if (LedgerAccountGroup != null && LedgerAccountGroup != "") mCondStr = mCondStr + " AND LAG.LedgerAccountGroupId = @LedgerAccountGroup";

            //            string mOpeningDateCondStr = "";
            //            if (FromDate != null) mOpeningDateCondStr = mOpeningDateCondStr + " AND LH.DocDate < @FromDate ";


            //            string mDateCondStr = "";
            //            if (FromDate != null) mDateCondStr = mDateCondStr + " AND LH.DocDate >= @FromDate ";
            //            if (ToDate != null) mDateCondStr = mDateCondStr + " AND LH.DocDate <= @ToDate ";


            //            string mQry = @"SELECT VMain.LedgerAccountId,  max(LA.LedgerAccountName + ',' + LA.LedgerAccountSuffix )   AS LedgerAccountName, max(LAG.LedgerAccountGroupName) AS LedgerAccountGroupName, 
            //                            CASE WHEN abs(Sum(Isnull(VMain.Opening,0))) = 0 THEN NULL ELSE abs(Sum(Isnull(VMain.Opening,0))) END AS Opening, 
            //                            CASE WHEN Sum(Isnull(VMain.Opening,0)) = 0 THEN NULL ELSE Sum(Isnull(VMain.Opening,0)) END AS OpeningValue, 
            //                            CASE WHEN Sum(Isnull(VMain.Opening,0)) >= 0 THEN 'Dr' ELSE 'Cr' END AS OpeningDrCr, 
            //                            CASE WHEN Sum(isnull(Vmain.AmtDr,0)) = 0 THEN NULL ELSE Sum(isnull(Vmain.AmtDr,0)) END AS AmtDr, CASE WHEN sum(isnull(VMain.AmtCr,0)) = 0 THEN NULL ELSE sum(isnull(VMain.AmtCr,0)) END AS AmtCr,
            //                            abs(Sum(Isnull(VMain.Opening,0)) + Sum(isnull(Vmain.AmtDr,0)) - sum(isnull(VMain.AmtCr,0))) AS Balance,
            //                            Sum(Isnull(VMain.Opening,0)) + Sum(isnull(Vmain.AmtDr,0)) - sum(isnull(VMain.AmtCr,0)) AS BalanceValue,
            //                            CASE WHEN ( Sum(Isnull(VMain.Opening,0)) + Sum(isnull(Vmain.AmtDr,0)) - sum(isnull(VMain.AmtCr,0))) >= 0 THEN 'Dr' ELSE 'Cr' END AS BalanceDrCr,
            //                            'Sub Trial Balance' AS ReportType, 'Ledger' AS OpenReportType
            //                            FROM
            //                            (
            //                            SELECT H.LedgerAccountId ,  sum(isnull(H.AmtDr,0)) - sum(isnull(H.AmtCr,0)) AS Opening,
            //                            0 AS AmtDr,0 AS  AmtCr    
            //                            FROM web.LedgerHeaders LH  WITH (Nolock)
            //                            LEFT JOIN web.Ledgers H WITH (Nolock) ON LH.LedgerHeaderId = H.LedgerHeaderId 
            //                            LEFT JOIN web.LedgerAccounts LA  WITH (Nolock) ON LA.LedgerAccountId = H.LedgerAccountId 
            //                            LEFT JOIN web.LedgerAccountGroups LAG  WITH (Nolock) ON LAG.LedgerAccountGroupId = LA.LedgerAccountGroupId 
            //                            WHERE 1 = 1 
            //                            AND LH.DocDate < @ToDate " +
            //                            (SiteId != null ? " AND LH.SiteId IN (SELECT Items FROM [dbo].[Split] (@Site, ','))" : "") +
            //                            (DivisionId != null ? " AND LH.DivisionId IN (SELECT Items FROM [dbo].[Split] (@Division, ','))" : "") +
            //                            (CostCenterId != null ? " AND H.CostCenterId IN (SELECT Items FROM [dbo].[Split] (@CostCenter, ','))" : "") +
            //                            (LedgerAccountGroup != null ? " AND LAG.LedgerAccountGroupId = @LedgerAccountGroup " : "") +
            //                            (IsIncludeOpening == "False" ? " AND LH.DocDate >= @FromDate" : "") +
            //                            @" GROUP BY H.LedgerAccountId " +

            //                            @" UNION ALL 
            //
            //                            SELECT H.LedgerAccountId, 0 AS Opening,
            //                            sum(isnull(H.AmtDr,0)) AS AmtDr,sum(isnull(H.AmtCr,0)) AS AmtCr
            //                            FROM web.LedgerHeaders LH  WITH (Nolock)
            //                            LEFT JOIN  web.Ledgers H  WITH (Nolock) ON LH.LedgerHeaderId = H.LedgerHeaderId 
            //                            LEFT JOIN web.LedgerAccounts LA  WITH (Nolock) ON LA.LedgerAccountId = H.LedgerAccountId 
            //                            LEFT JOIN web.LedgerAccountGroups LAG  WITH (Nolock) ON LAG.LedgerAccountGroupId = LA.LedgerAccountGroupId 
            //                            WHERE 1 = 1 
            //                            AND LH.DocDate <= @ToDate " +
            //                            (SiteId != null ? " AND LH.SiteId IN (SELECT Items FROM [dbo].[Split] (@Site, ','))" : "") +
            //                            (DivisionId != null ? " AND LH.DivisionId IN (SELECT Items FROM [dbo].[Split] (@Division, ','))" : "") +
            //                            (CostCenterId != null ? " AND H.CostCenterId IN (SELECT Items FROM [dbo].[Split] (@CostCenter, ','))" : "") +
            //                            (LedgerAccountGroup != null ? " AND LAG.LedgerAccountGroupId = @LedgerAccountGroup " : "") +
            //                            (IsIncludeOpening == "False" ? " AND LH.DocDate >= @FromDate" : "") +

            //                            @" GROUP BY H.LedgerAccountId 
            //                            ) AS VMain
            //                            LEFT JOIN web.LedgerAccounts LA  WITH (Nolock) ON LA.LedgerAccountId = VMain.LedgerAccountId 
            //                            LEFT JOIN web.LedgerAccountGroups LAG  WITH (Nolock) ON LAG.LedgerAccountGroupId = LA.LedgerAccountGroupId 
            //                            Where 1=1 
            //                            GROUP BY VMain.LedgerAccountId
            //                            Order By max(LA.LedgerAccountName + ',' + LA.LedgerAccountSuffix) ";

            string mPrimaryQry = @" WITH cteLedgerBalance AS
                                    ( 
                                                SELECT L.LedgerAccountId, Max(LA.LedgerAccountName + 
		                                        Case WHEN P.PersonId IS NOT NULL 
			                                         THEN CASE WHEN P.Suffix = P.Code THEN ' [' + P.Code + ']'
			 								                                          ELSE ', ' + P.Suffix + ' [' + P.Code + ']' END	
			                                         ELSE ', ' + LA.LedgerAccountSuffix END) AS LedgerAccountName,
                                                Sum(Case When LAG.LedgerAccountGroupNature in ('Expense','Income') Then 0 Else CASE WHEN H.DocDate < @FromDate THEN L.AmtDr - L.AmtCr ELSE 0 END END) AS Opening,
                                                Sum(CASE WHEN H.DocDate >= @FromDate AND H.DocDate <= @ToDate THEN L.AmtDr ELSE 0 END) AS AmtDr,
                                                Sum(CASE WHEN H.DocDate >= @FromDate AND H.DocDate <= @ToDate THEN L.AmtCr ELSE 0 END) AS AmtCr,
                                                Sum(Case When LAG.LedgerAccountGroupNature in ('Expense','Income') Then 
			                                        CASE WHEN H.DocDate >= @FromDate AND H.DocDate <= @ToDate THEN L.AmtDr-L.AmtCr ELSE 0 END 
		                                            ELSE CASE WHEN H.DocDate <= @ToDate THEN L.AmtDr-L.AmtCr ELSE 0 END END) AS Balance
                                                FROM Web.LedgerHeaders H 
                                                INNER JOIN web.Ledgers L ON L.LedgerHeaderId = H.LedgerHeaderId
                                                LEFT JOIN web.LedgerAccounts LA ON L.LedgerAccountId = LA.LedgerAccountId
                                                LEFT JOIN web.LedgerAccountGroups LAG ON LA.LedgerAccountGroupId = LAG.LedgerAccountGroupId  		
                                                LEFT JOIN Web.People P On LA.PersonId = P.PersonId
                                                WHERE 1 = 1 " +
                                                (SiteId != null ? " AND H.SiteId IN (SELECT Items FROM [dbo].[Split] (@Site, ','))" : "") +
                                                (DivisionId != null ? " AND H.DivisionId IN (SELECT Items FROM [dbo].[Split] (@Division, ','))" : "") +
                                                (CostCenterId != null ? " AND L.CostCenterId IN (SELECT Items FROM [dbo].[Split] (@CostCenter, ','))" : "") +
                                                (LedgerAccountGroup != null ? " AND LAG.LedgerAccountGroupId = @LedgerAccountGroup " : "") +
                                                (IsIncludeOpening == "False" ? " AND H.DocDate >= @FromDate" : "") +
                                                @" GROUP BY L.LedgerAccountId 
                                )";


            string mQry = mPrimaryQry + @" SELECT H.LedgerAccountId AS LedgerAccountId, LedgerAccountName AS LedgerAccountName, 
                                CASE WHEN isnull(H.Opening,0) <> 0 THEN Abs(isnull(H.Opening,0)) ELSE NULL END AS Opening,
                                CASE WHEN isnull(H.Opening,0) > 0 THEN 'Dr' 
	                                 WHEN isnull(H.Opening,0) < 0 THEN 'Cr' 
                                ELSE NULL END AS OpeningDrCr,
                                CASE WHEN isnull(H.AmtDr,0) <> 0 Then isnull(H.AmtDr,0) Else Null End AS AmtDr,
                                CASE WHEN isnull(H.AmtCr,0) <> 0 Then isnull(H.AmtCr,0) Else Null End AS AmtCr,
                                CASE WHEN Abs(isnull(H.Balance,0)) <> 0 Then Abs(isnull(H.Balance,0)) Else Null End As Balance,
                                CASE WHEN isnull(H.Balance,0) > 0 THEN 'Dr' 
	                                 WHEN isnull(H.Balance,0) < 0 THEN 'Cr' 
                                ELSE NULL END AS BalanceDrCr,
                                H.LedgerAccountId AS LedgerAccountId, 'Trial Balance' ReportType, 'Ledger' AS OpenReportType, LedgerAccountName  As OrderByColumn
                                FROM cteLedgerBalance H 
                                Where (isnull(H.Opening,0) <> 0 Or isnull(H.AmtDr,0) <> 0 Or isnull(H.AmtCr,0) <> 0 Or isnull(H.Balance,0) <> 0) " +
                                (IsIncludeZeroBalance == "False" ? " And isnull(H.Balance,0) <> 0 " : "") +
                                @" ORDER BY OrderByColumn ";

            IEnumerable<SubTrialBalanceSummaryViewModel> SubTrialBalanceSummaryList = db.Database.SqlQuery<SubTrialBalanceSummaryViewModel>(mQry, SqlParameterSiteId, SqlParameterDivisionId, SqlParameterFromDate, SqlParameterToDate, SqlParameterCostCenter, SqlParameterLedgerAccountGroup).ToList();

            Decimal TotalAmtDr = 0;
            Decimal TotalAmtCr = 0;
            Decimal TotalOpening = 0;
            string TotalOpeningDrCr = "";
            Decimal TotalBalance = 0;
            string TotalBalanceDrCr = "";

            TotalAmtDr = SubTrialBalanceSummaryList.Sum(m => m.AmtDr) ?? 0;
            TotalAmtCr = SubTrialBalanceSummaryList.Sum(m => m.AmtCr) ?? 0;

            foreach (var item in SubTrialBalanceSummaryList)
            {
                if (item.OpeningDrCr == "Dr")
                    TotalOpening = TotalOpening + (item.Opening ?? 0);
                else
                    TotalOpening = TotalOpening - (item.Opening ?? 0);

                if (item.BalanceDrCr == "Dr")
                    TotalBalance = TotalBalance + (item.Balance ?? 0);
                else
                    TotalBalance = TotalBalance - (item.Balance ?? 0);
            }

            if (TotalOpening > 0)
                TotalOpeningDrCr = "Dr";
            else if (TotalOpening < 0)
                TotalOpeningDrCr = "Cr";

            if (TotalBalance > 0)
                TotalBalanceDrCr = "Dr";
            else if (TotalBalance < 0)
                TotalBalanceDrCr = "Cr";


            foreach (var item in SubTrialBalanceSummaryList)
            {
                item.TotalAmtDr = TotalAmtDr;
                item.TotalAmtCr = TotalAmtCr;

                item.TotalOpening = Math.Abs(TotalOpening);
                item.TotalOpeningDrCr = TotalOpeningDrCr;
                item.TotalBalance = Math.Abs(TotalBalance);
                item.TotalBalanceDrCr = TotalBalanceDrCr;

            }


            return SubTrialBalanceSummaryList;

        }

        public IEnumerable<LedgerBalanceViewModel> GetLedgerBalance(FinancialDisplaySettings Settings)
        {
            var SiteSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "Site" select H).FirstOrDefault();
            var DivisionSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "Division" select H).FirstOrDefault();
            var FromDateSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "FromDate" select H).FirstOrDefault();
            var ToDateSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "ToDate" select H).FirstOrDefault();
            var CostCenterSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "CostCenter" select H).FirstOrDefault();
            var IsShowContraAccountSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "IsShowContraAccount" select H).FirstOrDefault();

            var LedgerAccountSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "LedgerAccount" select H).FirstOrDefault();


            string SiteId = SiteSetting.Value;
            string DivisionId = DivisionSetting.Value;
            string FromDate = FromDateSetting.Value;
            string ToDate = ToDateSetting.Value;
            string CostCenterId = CostCenterSetting.Value;
            string IsShowContraAccount = IsShowContraAccountSetting.Value;

            string LedgerAccount = LedgerAccountSetting.Value;



            SqlParameter SqlParameterSiteId = new SqlParameter("@Site", !string.IsNullOrEmpty(SiteId) ? SiteId : (object)DBNull.Value);
            SqlParameter SqlParameterDivisionId = new SqlParameter("@Division", !string.IsNullOrEmpty(DivisionId) ? DivisionId : (object)DBNull.Value);
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", FromDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", ToDate);
            SqlParameter SqlParameterCostCenter = new SqlParameter("@CostCenter", !string.IsNullOrEmpty(CostCenterId) ? CostCenterId : (object)DBNull.Value);
            SqlParameter SqlParameterLedgerAccount = new SqlParameter("@LedgerAccount", LedgerAccount);



            string mCondStr = "";
            if (SiteId != null) mCondStr = mCondStr + " AND LH.SiteId IN (SELECT Items FROM [dbo].[Split] (@Site, ','))";
            if (DivisionId != null) mCondStr = mCondStr + " AND LH.SiteId IN (SELECT Items FROM [dbo].[Split] (@Division, ','))";
            if (CostCenterId != null) mCondStr = mCondStr + " AND H.CostCenterId IN (SELECT Items FROM [dbo].[Split] (@CostCenter, ','))";
            if (LedgerAccount != null && LedgerAccount != "") mCondStr = mCondStr + " AND LA.LedgerAccountId = @LedgerAccount";

            //string mOpeningDateCondStr = "";
            //if (FromDate != null) mOpeningDateCondStr = mOpeningDateCondStr + " AND LH.DocDate < @FromDate ";

            string mOpeningDateCondStr = "";
            if (FromDate != null)
            {
                mOpeningDateCondStr = mOpeningDateCondStr + " AND LH.DocDate >= (Case When LAG.LedgerAccountGroupNature in ('Expense','Income') Then @FromDate Else '01/01/1900' End) ";
                mOpeningDateCondStr = mOpeningDateCondStr + " AND LH.DocDate < @FromDate ";
            }



            string mDateCondStr = "";
            if (FromDate != null) mDateCondStr = mDateCondStr + " AND LH.DocDate >= @FromDate ";
            if (ToDate != null) mDateCondStr = mDateCondStr + " AND LH.DocDate <= @ToDate ";



            string mQry = @"SELECT VMain.LedgerAccountId, VMain.LedgerHeaderId, VMain.DocHeaderId, VMain.DocTypeId,  VMain.LedgerAccountName,VMain.PersonId, VMain.ContraLedgerAccountName, 
                            D.DivisionShortCode + S.SiteShortCode + '-' + VMain.DocumentTypeShortName + '-' + VMain.DocNo As DocNo, VMain.DocumentTypeShortName, REPLACE(CONVERT(VARCHAR(11),VMain.DocDate,106), ' ','/')   AS DocDate, IsNull(VMain.Narration,'') As Narration, VMain.LedgerId, 
                            CASE WHEN VMain.AmtDr = 0 THEN NULL ELSE VMain.AmtDr END AS AmtDr, CASE WHEN VMain.AmtCr = 0 THEN NULL ELSE VMain.AmtCr END AS AmtCr,  
                            abs(sum(isnull(VMain.AmtDr,0)) OVER( PARTITION BY VMain.LedgerAccountId  ORDER BY VMain.DocDate, VMain.DocTypeId,  VMain.DocNo, VMain.LedgerId ) - sum(isnull(VMain.AmtCr,0)) OVER( PARTITION BY VMain.LedgerAccountId  ORDER BY VMain.DocDate, VMain.DocTypeId,  VMain.DocNo ,VMain.LedgerId ))  AS Balance,
                            CASE WHEN sum(isnull(VMain.AmtDr,0)) OVER( ORDER BY VMain.DocDate, VMain.DocTypeId,  VMain.DocNo ,VMain.LedgerId ) - sum(isnull(VMain.AmtCr,0)) OVER( PARTITION BY VMain.LedgerAccountId  ORDER BY VMain.DocDate, VMain.DocTypeId,  VMain.DocNo ,VMain.LedgerId )  >= 0 THEN 'Dr' ELSE 'Cr' END  AS BalanceDrCr,
                            VMain.LedgerAccountName AS LedgerAccountText,
                            S.SiteName AS SiteText,D.DivisionName AS DivisionText,VMain.CostCenterName  AS CostCenterName,
                            'Ledgers' AS ReportType, Null AS OpenReportType
                            FROM
                            (
	                            SELECT Max(LH.SiteId) AS SiteId, Max(LH.DivisionId) AS DivisionId, H.LedgerAccountId, 0 AS LedgerHeaderId, 0 AS DocHeaderId, 0 AS DocTypeId, Max(LA.LedgerAccountName) AS LedgerAccountName,max(LA.PersonId) AS PersonId,  'Opening' AS ContraLedgerAccountName, 'Opening' AS DocNo, 'Opening' AS DocumentTypeShortName, @FromDate AS DocDate, 'Opening' AS Narration,
	                            'Opening' AS Narration1, 'Opening' AS Narration2, 0 AS LedgerId, 
	                            CASE WHEN sum(isnull(H.AmtDr,0)) - sum(isnull(H.AmtCr,0)) > 0 THEN sum(isnull(H.AmtDr,0)) - sum(isnull(H.AmtCr,0)) ELSE 0 END AS AmtDr,
	                            CASE WHEN sum(isnull(H.AmtDr,0)) - sum(isnull(H.AmtCr,0)) < 0 THEN abs(sum(isnull(H.AmtDr,0)) - sum(isnull(H.AmtCr,0))) ELSE 0 END AS AmtCr,
	                            NULL AS DomainName, NULL AS ControllerActionId ,'Opening' AS CostCenterName
	                            FROM web.LedgerHeaders LH   WITH (Nolock) 
	                            LEFT JOIN  web.Ledgers H WITH (Nolock) ON LH.LedgerHeaderId = H.LedgerHeaderId 
	                            LEFT JOIN web.LedgerAccounts LA  WITH (Nolock) ON LA.LedgerAccountId = H.LedgerAccountId 
                                LEFT JOIN Web.LedgerAccountGroups LAG On LA.LedgerAccountGroupId = LAG.LedgerAccountGroupId
	                            WHERE H.LedgerAccountId IS NOT NULL " + mCondStr + mOpeningDateCondStr +
                                @" GROUP BY H.LedgerAccountId
	 
	                            UNION ALL 
	
	                            SELECT LH.SiteId, LH.DivisionId, H.LedgerAccountId,  H.LedgerHeaderId, IsNull(LH.DocHeaderId,H.LedgerHeaderId) AS DocHeaderId,  LH.DocTypeId,  LA.LedgerAccountName,LA.PersonId, CLA.LedgerAccountName AS ContraLedgerAccountName, LH.DocNo, DT.DocumentTypeShortName, LH.DocDate  AS DocDate, 
	                            CASE When '" + IsShowContraAccount + @"' = 'True' And CLA.LedgerAccountName Is Not Null 
			                            Then '<Strong>' + CLA.LedgerAccountName + 
		                                    Case WHEN P.PersonId IS NOT NULL 
			                                     THEN CASE WHEN P.Suffix = P.Code THEN ' [' + P.Code + ']'
			 								                                      ELSE ', ' + P.Suffix + ' [' + P.Code + ']' END	
			                                     ELSE ', ' + CLA.LedgerAccountSuffix END  + '</Strong>' + '</br>' + 
				                            CASE When Lh.PartyDocNo Is Not Null THen 'Party Doc No : ' + LH.PartyDocNo + ', ' Else '' End + 
				                            CASE When Lh.PartyDocDate Is Not Null THen 'Party Doc Date : ' +  REPLACE(CONVERT(VARCHAR(11),LH.PartyDocDate,106), ' ','/')  + '</br>' Else '' End + 
				                            CASE When H.ChqNo Is Not Null THen 'Chq No.: ' + H.ChqNo + ', ' Else '' End + 
				                            CASE When H.ChqDate Is Not Null THen 'Chq Date: ' + REPLACE(CONVERT(VARCHAR(11),H.ChqDate,106), ' ','/') + '</br>' Else '' End +  
				                            CASE When IsNull(LH.Narration,'') = '' Then '' Else ' (' + IsNull(LH.Narration,'') + ')' End  
		                            Else 
			                            CASE When Lh.PartyDocNo Is Not Null THen 'Party Doc No : ' + LH.PartyDocNo + ', ' Else '' End + 
				                            CASE When Lh.PartyDocDate Is Not Null THen 'Party Doc Date : ' +  REPLACE(CONVERT(VARCHAR(11),LH.PartyDocDate,106), ' ','/')  + '</br>' Else '' End + 
				                            CASE When H.ChqNo Is Not Null THen 'Chq No.: ' + H.ChqNo + ', ' Else '' End + 
				                            CASE When H.ChqDate Is Not Null THen 'Chq Date: ' + REPLACE(CONVERT(VARCHAR(11),H.ChqDate,106), ' ','/') + '</br>' Else '' End +  
				                            CASE When IsNull(LH.Narration,'') = '' Then '' Else ' (' + IsNull(LH.Narration,'') + ')' End  
		                            End As Narration,
		
		
	                            CLA.LedgerAccountName AS Narration1, H.Narration AS Narration2,
	                            H.LedgerId, H.AmtDr, H.AmtCr, DT.DomainName, DT.ControllerActionId ,C.CostCenterName      
	                            FROM web.Ledgers H  WITH (Nolock) 
	                            LEFT JOIN web.LedgerHeaders LH WITH (Nolock) ON LH.LedgerHeaderId = H.LedgerHeaderId 
	                            LEFT JOIN web.DocumentTypes DT WITH (Nolock) ON DT.DocumentTypeId = LH.DocTypeId 
	                            LEFT JOIN web.LedgerAccounts LA  WITH (Nolock) ON LA.LedgerAccountId = H.LedgerAccountId 
	                            LEFT JOIN web.LedgerAccounts CLA  WITH (Nolock) ON CLA.LedgerAccountId = H.ContraLedgerAccountId  
                                LEFT JOIN Web.People P On CLA.PersonId = P.PersonId
	                            LEFT JOIN Web.CostCenters C WITH (Nolock) ON C.CostCenterId=H.CostCenterId
	                            WHERE LA.LedgerAccountId IS NOT NULL " + mCondStr + mDateCondStr +
                            @" ) VMain 
                            LEFT JOIN Web.Sites S ON S.SiteId = VMain.SiteId
                            LEFT JOIN Web.Divisions D ON D.DivisionId = VMain.DivisionId ";


            IEnumerable<LedgerBalanceViewModel> LedgerBalanceList = db.Database.SqlQuery<LedgerBalanceViewModel>(mQry, SqlParameterSiteId, SqlParameterDivisionId, SqlParameterFromDate, SqlParameterToDate, SqlParameterCostCenter, SqlParameterLedgerAccount).ToList();

            Decimal TotalAmtDr = 0;
            Decimal TotalAmtCr = 0;

            TotalAmtDr = LedgerBalanceList.Sum(m => m.AmtDr) ?? 0;
            TotalAmtCr = LedgerBalanceList.Sum(m => m.AmtCr) ?? 0;

            foreach (var item in LedgerBalanceList)
            {
                item.TotalAmtDr = TotalAmtDr;
                item.TotalAmtCr = TotalAmtCr;
            }

            return LedgerBalanceList;

        }


        public IEnumerable<TrialBalanceViewModel> GetTrialBalanceDetail(FinancialDisplaySettings Settings)
        {
            var SiteSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "Site" select H).FirstOrDefault();
            var DivisionSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "Division" select H).FirstOrDefault();
            var FromDateSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "FromDate" select H).FirstOrDefault();
            var ToDateSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "ToDate" select H).FirstOrDefault();
            var CostCenterSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "CostCenter" select H).FirstOrDefault();
            var IsIncludeZeroBalanceSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "IsIncludeZeroBalance" select H).FirstOrDefault();
            var IsIncludeOpeningSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "IsIncludeOpening" select H).FirstOrDefault();

            var LedgerAccountGroupSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "LedgerAccountGroup" select H).FirstOrDefault();


            string SiteId = SiteSetting.Value;
            string DivisionId = DivisionSetting.Value;
            string FromDate = FromDateSetting.Value;
            string ToDate = ToDateSetting.Value;
            string CostCenterId = CostCenterSetting.Value;
            string IsIncludeZeroBalance = IsIncludeZeroBalanceSetting.Value;
            string IsIncludeOpening = IsIncludeOpeningSetting.Value;
            string LedgerAccountGroup = LedgerAccountGroupSetting.Value;


            SqlParameter SqlParameterSiteId = new SqlParameter("@Site", !string.IsNullOrEmpty(SiteId) ? SiteId : (object)DBNull.Value);
            SqlParameter SqlParameterDivisionId = new SqlParameter("@Division", !string.IsNullOrEmpty(DivisionId) ? DivisionId : (object)DBNull.Value);
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", FromDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", ToDate);
            SqlParameter SqlParameterCostCenter = new SqlParameter("@CostCenter", !string.IsNullOrEmpty(CostCenterId) ? CostCenterId : (object)DBNull.Value);
            SqlParameter SqlParameterLedgerAccountGroup = new SqlParameter("@LedgerAccountGroup", !string.IsNullOrEmpty(LedgerAccountGroup) ? LedgerAccountGroup : (object)DBNull.Value);

            Decimal TotalAmtDr = 0;
            Decimal TotalAmtCr = 0;


            string mQry = GetQryForTrialBalance(SiteId, DivisionId, FromDate, ToDate, CostCenterId, IsIncludeZeroBalance, IsIncludeOpening, LedgerAccountGroup) +
                            @"SELECT H.BaseLedgerAccountGroupId AS LedgerAccountGroupId, '<Strong>' +  Max(BaseLedgerAccountGroupName) + '</Strong>' AS LedgerAccountGroupName, 
                                        CASE WHEN Sum(isnull(H.Balance,0)) > 0 THEN Sum(isnull(H.Balance,0)) ELSE NULL  END AS AmtDr,
                                        CASE WHEN Sum(isnull(H.Balance,0)) < 0 THEN abs(Sum(isnull(H.Balance,0))) ELSE NULL END AS AmtCr,
                                        NULL AS LedgerAccountId, 'Trial Balance' ReportType, 'Trial Balance' AS OpenReportType, Max(BaseLedgerAccountGroupName) As OrderByColumn,
                                        Max(BaseLedgerAccountGroupName) As ParentLedgerAccountGroupName
                                        FROM cteAcGroup H 
                                        LEFT JOIN Web.LedgerAccountGroups Ag ON H.BaseLedgerAccountGroupId = Ag.LedgerAccountGroupId
                                        GROUP BY H.BaseLedgerAccountGroupId 
                                        Having (Sum(isnull(H.Opening,0)) <> 0 Or Sum(isnull(H.AmtDr,0)) <> 0 Or Sum(isnull(H.AmtCr,0)) <> 0 Or Sum(isnull(H.Balance,0)) <> 0) " +
                            (IsIncludeZeroBalance == "False" ? " And Sum(isnull(H.Balance,0)) <> 0 " : "") +
                            @" ORDER BY OrderByColumn ";

            List<TrialBalanceViewModel> TrialBalanceList = db.Database.SqlQuery<TrialBalanceViewModel>(mQry, SqlParameterSiteId, SqlParameterDivisionId, SqlParameterFromDate, SqlParameterToDate, SqlParameterCostCenter, SqlParameterLedgerAccountGroup).ToList();

            if (LedgerAccountGroup == null || LedgerAccountGroup == "" || LedgerAccountGroup == "0")
            { 
                TotalAmtDr = TrialBalanceList.Sum(m => m.AmtDr) ?? 0;
                TotalAmtCr = TrialBalanceList.Sum(m => m.AmtCr) ?? 0;

                if (TotalAmtDr != TotalAmtCr)
                {
                    TrialBalanceViewModel BlankItem = new TrialBalanceViewModel();
                    BlankItem.ParentLedgerAccountGroupName = "ZZZ";
                    TrialBalanceList.Add(BlankItem);
                    TrialBalanceViewModel DiffVm = new TrialBalanceViewModel();
                    DiffVm.LedgerAccountGroupName = "<Strong>Difference in Trial Balance</Strong>";
                    DiffVm.ParentLedgerAccountGroupName = "ZZZ";
                    if (TotalAmtDr > TotalAmtCr)
                        DiffVm.AmtCr = TotalAmtDr - TotalAmtCr;
                    else
                        DiffVm.AmtDr = TotalAmtCr - TotalAmtDr;
                    TrialBalanceList.Add(DiffVm);
                }
            }

            TotalAmtDr = TrialBalanceList.Sum(m => m.AmtDr) ?? 0;
            TotalAmtCr = TrialBalanceList.Sum(m => m.AmtCr) ?? 0;


            LedgerAccountGroup = string.Join<string>(",", TrialBalanceList.Select(m => m.LedgerAccountGroupId.ToString()));
            //LedgerAccountGroup = string.Join<string>(",", db.LedgerAccountGroup.Select(m => m.LedgerAccountGroupId.ToString()));
            //LedgerAccountGroup = "27,1012";
            SqlParameter SqlParameterSiteId_Child = new SqlParameter("@Site", !string.IsNullOrEmpty(SiteId) ? SiteId : (object)DBNull.Value);
            SqlParameter SqlParameterDivisionId_Child = new SqlParameter("@Division", !string.IsNullOrEmpty(DivisionId) ? DivisionId : (object)DBNull.Value);
            SqlParameter SqlParameterFromDate_Child = new SqlParameter("@FromDate", FromDate);
            SqlParameter SqlParameterToDate_Child = new SqlParameter("@ToDate", ToDate);
            SqlParameter SqlParameterCostCenter_Child = new SqlParameter("@CostCenter", !string.IsNullOrEmpty(CostCenterId) ? CostCenterId : (object)DBNull.Value);
            SqlParameter SqlParameterLedgerAccountGroup_Child = new SqlParameter("@LedgerAccountGroup", !string.IsNullOrEmpty(LedgerAccountGroup) ? LedgerAccountGroup : (object)DBNull.Value);

            mQry = GetQryForTrialBalance(SiteId, DivisionId, FromDate, ToDate, CostCenterId, IsIncludeZeroBalance, IsIncludeOpening, LedgerAccountGroup) +
                            @"SELECT H.BaseLedgerAccountGroupId AS LedgerAccountGroupId, '&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;' + '<Strong>' +  Max(BaseLedgerAccountGroupName) + '</Strong>' AS LedgerAccountGroupName, 
                                        CASE WHEN Sum(isnull(H.Balance,0)) > 0 THEN Sum(isnull(H.Balance,0)) ELSE NULL  END AS AmtDr,
                                        CASE WHEN Sum(isnull(H.Balance,0)) < 0 THEN abs(Sum(isnull(H.Balance,0))) ELSE NULL END AS AmtCr,
                                        NULL AS LedgerAccountId, 'Trial Balance' ReportType, 'Trial Balance' AS OpenReportType, Max(BaseLedgerAccountGroupName) As OrderByColumn,
                                        Max(PAg.LedgerAccountGroupName) AS ParentLedgerAccountGroupName
                                        FROM cteAcGroup H 
                                        LEFT JOIN Web.LedgerAccountGroups Ag ON H.BaseLedgerAccountGroupId = Ag.LedgerAccountGroupId
                                        LEFT JOIN Web.LedgerAccountGroups PAg ON Ag.ParentLedgerAccountGroupId = PAg.LedgerAccountGroupId
                                        GROUP BY H.BaseLedgerAccountGroupId 
                                        Having (Sum(isnull(H.Opening,0)) <> 0 Or Sum(isnull(H.AmtDr,0)) <> 0 Or Sum(isnull(H.AmtCr,0)) <> 0 Or Sum(isnull(H.Balance,0)) <> 0) " +
                            (IsIncludeZeroBalance == "False" ? " And Sum(isnull(H.Balance,0)) <> 0 " : "") +

                            @"UNION ALL 
                
                                        SELECT H.LedgerAccountId AS LedgerAccountGroupId, '&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;' + H.LedgerAccountName AS LedgerAccountGroupName, 
                                        CASE WHEN isnull(H.Balance,0) > 0 THEN isnull(H.Balance,0) ELSE NULL  END AS AmtDr,
                                        CASE WHEN isnull(H.Balance,0) < 0 THEN abs(isnull(H.Balance,0)) ELSE NULL END AS AmtCr,
                                        H.LedgerAccountId AS LedgerAccountId, 'Trial Balance' ReportType, 'Ledger' AS OpenReportType, H.LedgerAccountName  As OrderByColumn,
                                        Ag.LedgerAccountGroupName AS ParentLedgerAccountGroupName
                                        FROM cteLedgerBalance H 
                                        LEFT JOIN Web.LedgerAccounts A ON H.LedgerAccountId = A.LedgerAccountId 
                                        LEFT JOIN Web.LedgerAccountGroups Ag ON A.LedgerAccountGroupId = Ag.LedgerAccountGroupId 
                                        Where (isnull(H.Opening,0) <> 0 Or isnull(H.AmtDr,0) <> 0 Or isnull(H.AmtCr,0) <> 0 Or isnull(H.Balance,0) <> 0) " +
                            (IsIncludeZeroBalance == "False" ? " And isnull(H.Balance,0) <> 0 " : "") +
                            @" ORDER BY OrderByColumn ";

            IEnumerable<TrialBalanceViewModel> ChileTrialBalanceList = db.Database.SqlQuery<TrialBalanceViewModel>(mQry, SqlParameterSiteId_Child, SqlParameterDivisionId_Child, SqlParameterFromDate_Child, SqlParameterToDate_Child, SqlParameterCostCenter_Child, SqlParameterLedgerAccountGroup_Child).ToList();

            IEnumerable<TrialBalanceViewModel> TrialBalanceViewModelCombind = TrialBalanceList.Union(ChileTrialBalanceList).ToList().OrderBy(m => m.ParentLedgerAccountGroupName);

            foreach (var item in TrialBalanceViewModelCombind)
            {
                item.TotalAmtDr = TotalAmtDr;
                item.TotalAmtCr = TotalAmtCr;
            }

            return TrialBalanceViewModelCombind;

        }


        public IEnumerable<TrialBalanceSummaryViewModel> GetTrialBalanceDetailSummary(FinancialDisplaySettings Settings)
        {
            var SiteSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "Site" select H).FirstOrDefault();
            var DivisionSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "Division" select H).FirstOrDefault();
            var FromDateSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "FromDate" select H).FirstOrDefault();
            var ToDateSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "ToDate" select H).FirstOrDefault();
            var CostCenterSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "CostCenter" select H).FirstOrDefault();
            var IsIncludeZeroBalanceSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "IsIncludeZeroBalance" select H).FirstOrDefault();
            var IsIncludeOpeningSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "IsIncludeOpening" select H).FirstOrDefault();

            var LedgerAccountGroupSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "LedgerAccountGroup" select H).FirstOrDefault();


            string SiteId = SiteSetting.Value;
            string DivisionId = DivisionSetting.Value;
            string FromDate = FromDateSetting.Value;
            string ToDate = ToDateSetting.Value;
            string CostCenterId = CostCenterSetting.Value;
            string IsIncludeZeroBalance = IsIncludeZeroBalanceSetting.Value;
            string IsIncludeOpening = IsIncludeOpeningSetting.Value;
            string LedgerAccountGroup = LedgerAccountGroupSetting.Value;


            SqlParameter SqlParameterSiteId = new SqlParameter("@Site", !string.IsNullOrEmpty(SiteId) ? SiteId : (object)DBNull.Value);
            SqlParameter SqlParameterDivisionId = new SqlParameter("@Division", !string.IsNullOrEmpty(DivisionId) ? DivisionId : (object)DBNull.Value);
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", FromDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", ToDate);
            SqlParameter SqlParameterCostCenter = new SqlParameter("@CostCenter", !string.IsNullOrEmpty(CostCenterId) ? CostCenterId : (object)DBNull.Value);
            SqlParameter SqlParameterLedgerAccountGroup = new SqlParameter("@LedgerAccountGroup", !string.IsNullOrEmpty(LedgerAccountGroup) ? LedgerAccountGroup : (object)DBNull.Value);




            string mQry = GetQryForTrialBalance(SiteId, DivisionId, FromDate, ToDate, CostCenterId, IsIncludeZeroBalance, IsIncludeOpening, LedgerAccountGroup) +
                            @"SELECT H.BaseLedgerAccountGroupId AS LedgerAccountGroupId, '<Strong>' +  Max(H.BaseLedgerAccountGroupName) + '</Strong>' AS LedgerAccountGroupName, 
                                CASE WHEN Sum(isnull(H.Opening,0)) <> 0 THEN Abs(Sum(isnull(H.Opening,0))) ELSE NULL END AS Opening,
                                CASE WHEN Sum(isnull(H.Opening,0)) > 0 THEN 'Dr' 
	                                 WHEN Sum(isnull(H.Opening,0)) < 0 THEN 'Cr' 
                                ELSE NULL END AS OpeningDrCr,
                                CASE WHEN Sum(isnull(H.AmtDr,0)) <> 0 Then Sum(isnull(H.AmtDr,0)) Else Null End AS AmtDr,
                                CASE WHEN Sum(isnull(H.AmtCr,0)) <> 0 Then Sum(isnull(H.AmtCr,0)) Else Null End AS AmtCr,
                                CASE WHEN Abs(Sum(isnull(H.Balance,0))) <> 0 Then Abs(Sum(isnull(H.Balance,0))) Else Null End As Balance,
                                CASE WHEN Sum(isnull(H.Balance,0)) > 0 THEN 'Dr' 
	                                 WHEN Sum(isnull(H.Balance,0)) < 0 THEN 'Cr' 
                                ELSE NULL END AS BalanceDrCr,
                                NULL AS LedgerAccountId, 'Trial Balance' ReportType, 'Trial Balance' AS OpenReportType, Max(H.BaseLedgerAccountGroupName) As OrderByColumn,
                                Max(BaseLedgerAccountGroupName) As ParentLedgerAccountGroupName
                                FROM cteAcGroup H 
                                GROUP BY H.BaseLedgerAccountGroupId
                                Having (Sum(isnull(H.Opening,0)) <> 0 Or Sum(isnull(H.AmtDr,0)) <> 0 Or Sum(isnull(H.AmtCr,0)) <> 0 Or Sum(isnull(H.Balance,0)) <> 0) " +
                                (IsIncludeZeroBalance == "False" ? " And Sum(isnull(H.Balance,0)) <> 0 " : "");

            IEnumerable<TrialBalanceSummaryViewModel> TrialBalanceSummaryList = db.Database.SqlQuery<TrialBalanceSummaryViewModel>(mQry, SqlParameterSiteId, SqlParameterDivisionId, SqlParameterFromDate, SqlParameterToDate, SqlParameterCostCenter, SqlParameterLedgerAccountGroup).ToList();

            Decimal TotalAmtDr = 0;
            Decimal TotalAmtCr = 0;
            Decimal TotalOpening = 0;
            string TotalOpeningDrCr = "";
            Decimal TotalBalance = 0;
            string TotalBalanceDrCr = "";

            TotalAmtDr = TrialBalanceSummaryList.Sum(m => m.AmtDr) ?? 0;
            TotalAmtCr = TrialBalanceSummaryList.Sum(m => m.AmtCr) ?? 0;

            foreach (var item in TrialBalanceSummaryList)
            {
                if (item.OpeningDrCr == "Dr")
                    TotalOpening = TotalOpening + (item.Opening ?? 0);
                else
                    TotalOpening = TotalOpening - (item.Opening ?? 0);

                if (item.BalanceDrCr == "Dr")
                    TotalBalance = TotalBalance + (item.Balance ?? 0);
                else
                    TotalBalance = TotalBalance - (item.Balance ?? 0);
            }

            if (TotalOpening > 0)
                TotalOpeningDrCr = "Dr";
            else if (TotalOpening < 0)
                TotalOpeningDrCr = "Cr";

            if (TotalBalance > 0)
                TotalBalanceDrCr = "Dr";
            else if (TotalBalance < 0)
                TotalBalanceDrCr = "Cr";



            LedgerAccountGroup = string.Join<string>(",", TrialBalanceSummaryList.Select(m => m.LedgerAccountGroupId.ToString()));
            SqlParameter SqlParameterSiteId_Child = new SqlParameter("@Site", !string.IsNullOrEmpty(SiteId) ? SiteId : (object)DBNull.Value);
            SqlParameter SqlParameterDivisionId_Child = new SqlParameter("@Division", !string.IsNullOrEmpty(DivisionId) ? DivisionId : (object)DBNull.Value);
            SqlParameter SqlParameterFromDate_Child = new SqlParameter("@FromDate", FromDate);
            SqlParameter SqlParameterToDate_Child = new SqlParameter("@ToDate", ToDate);
            SqlParameter SqlParameterCostCenter_Child = new SqlParameter("@CostCenter", !string.IsNullOrEmpty(CostCenterId) ? CostCenterId : (object)DBNull.Value);
            SqlParameter SqlParameterLedgerAccountGroup_Child = new SqlParameter("@LedgerAccountGroup", !string.IsNullOrEmpty(LedgerAccountGroup) ? LedgerAccountGroup : (object)DBNull.Value);

            mQry = GetQryForTrialBalance(SiteId, DivisionId, FromDate, ToDate, CostCenterId, IsIncludeZeroBalance, IsIncludeOpening, LedgerAccountGroup) +
                            @"SELECT H.BaseLedgerAccountGroupId AS LedgerAccountGroupId, '&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;' + '<Strong>' +  Max(H.BaseLedgerAccountGroupName) + '</Strong>' AS LedgerAccountGroupName, 
                                CASE WHEN Sum(isnull(H.Opening,0)) <> 0 THEN Abs(Sum(isnull(H.Opening,0))) ELSE NULL END AS Opening,
                                CASE WHEN Sum(isnull(H.Opening,0)) > 0 THEN 'Dr' 
	                                 WHEN Sum(isnull(H.Opening,0)) < 0 THEN 'Cr' 
                                ELSE NULL END AS OpeningDrCr,
                                CASE WHEN Sum(isnull(H.AmtDr,0)) <> 0 Then Sum(isnull(H.AmtDr,0)) Else Null End AS AmtDr,
                                CASE WHEN Sum(isnull(H.AmtCr,0)) <> 0 Then Sum(isnull(H.AmtCr,0)) Else Null End AS AmtCr,
                                CASE WHEN Abs(Sum(isnull(H.Balance,0))) <> 0 Then Abs(Sum(isnull(H.Balance,0))) Else Null End As Balance,
                                CASE WHEN Sum(isnull(H.Balance,0)) > 0 THEN 'Dr' 
	                                 WHEN Sum(isnull(H.Balance,0)) < 0 THEN 'Cr' 
                                ELSE NULL END AS BalanceDrCr,
                                NULL AS LedgerAccountId, 'Trial Balance' ReportType, 'Trial Balance' AS OpenReportType, Max(H.BaseLedgerAccountGroupName) As OrderByColumn,
                                Max(PAg.LedgerAccountGroupName) AS ParentLedgerAccountGroupName
                                FROM cteAcGroup H 
                                LEFT JOIN Web.LedgerAccountGroups Ag ON H.BaseLedgerAccountGroupId = Ag.LedgerAccountGroupId
                                LEFT JOIN Web.LedgerAccountGroups PAg ON Ag.ParentLedgerAccountGroupId = PAg.LedgerAccountGroupId
                                GROUP BY H.BaseLedgerAccountGroupId
                                Having (Sum(isnull(H.Opening,0)) <> 0 Or Sum(isnull(H.AmtDr,0)) <> 0 Or Sum(isnull(H.AmtCr,0)) <> 0 Or Sum(isnull(H.Balance,0)) <> 0) " +
                                (IsIncludeZeroBalance == "False" ? " And Sum(isnull(H.Balance,0)) <> 0 " : "") +

                                @" UNION ALL 

                                SELECT H.LedgerAccountId AS LedgerAccountGroupId, '&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;' + H.LedgerAccountName AS LedgerAccountGroupName, 
                                CASE WHEN isnull(H.Opening,0) <> 0 THEN Abs(isnull(H.Opening,0)) ELSE NULL END AS Opening,
                                CASE WHEN isnull(H.Opening,0) > 0 THEN 'Dr' 
	                                 WHEN isnull(H.Opening,0) < 0 THEN 'Cr' 
                                ELSE NULL END AS OpeningDrCr,
                                CASE WHEN isnull(H.AmtDr,0) <> 0 Then isnull(H.AmtDr,0) Else Null End AS AmtDr,
                                CASE WHEN isnull(H.AmtCr,0) <> 0 Then isnull(H.AmtCr,0) Else Null End AS AmtCr,
                                CASE WHEN Abs(isnull(H.Balance,0)) <> 0 Then Abs(isnull(H.Balance,0)) Else Null End As Balance,
                                CASE WHEN isnull(H.Balance,0) > 0 THEN 'Dr' 
	                                 WHEN isnull(H.Balance,0) < 0 THEN 'Cr' 
                                ELSE NULL END AS BalanceDrCr,
                                H.LedgerAccountId AS LedgerAccountId, 'Trial Balance' ReportType, 'Ledger' AS OpenReportType, H.LedgerAccountName  As OrderByColumn,
                                Ag.LedgerAccountGroupName AS ParentLedgerAccountGroupName
                                FROM cteLedgerBalance H 
                                LEFT JOIN Web.LedgerAccounts A ON H.LedgerAccountId = A.LedgerAccountId 
                                LEFT JOIN Web.LedgerAccountGroups Ag ON A.LedgerAccountGroupId = Ag.LedgerAccountGroupId 
                                Where (isnull(H.Opening,0) <> 0 Or isnull(H.AmtDr,0) <> 0 Or isnull(H.AmtCr,0) <> 0 Or isnull(H.Balance,0) <> 0) " +
                                (IsIncludeZeroBalance == "False" ? " And isnull(H.Balance,0) <> 0 " : "") +
                                @" ORDER BY OrderByColumn ";

            IEnumerable<TrialBalanceSummaryViewModel> ChileTrialBalanceSummaryList = db.Database.SqlQuery<TrialBalanceSummaryViewModel>(mQry, SqlParameterSiteId_Child, SqlParameterDivisionId_Child, SqlParameterFromDate_Child, SqlParameterToDate_Child, SqlParameterCostCenter_Child, SqlParameterLedgerAccountGroup_Child).ToList();

            IEnumerable<TrialBalanceSummaryViewModel> TrialBalanceSummaryViewModelCombind = TrialBalanceSummaryList.Union(ChileTrialBalanceSummaryList).ToList().OrderBy(m => m.ParentLedgerAccountGroupName);

            foreach (var item in TrialBalanceSummaryViewModelCombind)
            {
                item.TotalAmtDr = TotalAmtDr;
                item.TotalAmtCr = TotalAmtCr;

                item.TotalOpening = Math.Abs(TotalOpening);
                item.TotalOpeningDrCr = TotalOpeningDrCr;
                item.TotalBalance = Math.Abs(TotalBalance);
                item.TotalBalanceDrCr = TotalBalanceDrCr;
            }

            return TrialBalanceSummaryViewModelCombind;

        }

        //        public IEnumerable<TrialBalanceDetailViewModel> GetTrialBalanceDetail(FinancialDisplaySettings Settings)
        //        {
        //            var SiteSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "Site" select H).FirstOrDefault();
        //            var DivisionSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "Division" select H).FirstOrDefault();
        //            var FromDateSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "FromDate" select H).FirstOrDefault();
        //            var ToDateSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "ToDate" select H).FirstOrDefault();
        //            var CostCenterSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "CostCenter" select H).FirstOrDefault();
        //            var LedgerAccountGroupSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "LedgerAccountGroup" select H).FirstOrDefault();


        //            string SiteId = SiteSetting.Value;
        //            string DivisionId = DivisionSetting.Value;
        //            string FromDate = FromDateSetting.Value;
        //            string ToDate = ToDateSetting.Value;
        //            string CostCenterId = CostCenterSetting.Value;
        //            string LedgerAccountGroup = LedgerAccountGroupSetting.Value;


        //            SqlParameter SqlParameterSiteId = new SqlParameter("@Site", !string.IsNullOrEmpty(SiteId) ? SiteId : (object)DBNull.Value);
        //            SqlParameter SqlParameterDivisionId = new SqlParameter("@Division", !string.IsNullOrEmpty(DivisionId) ? DivisionId : (object)DBNull.Value);
        //            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", FromDate);
        //            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", ToDate);
        //            SqlParameter SqlParameterCostCenter = new SqlParameter("@CostCenter", CostCenterId);
        //            SqlParameter SqlParameterLedgerAccountGroup = new SqlParameter("@LedgerAccountGroup", LedgerAccountGroup);


        //            string mCondStr = "";
        //            if (SiteId != null) mCondStr = mCondStr + " AND LH.SiteId IN (SELECT Items FROM [dbo].[Split] (@Site, ','))";
        //            if (DivisionId != null) mCondStr = mCondStr + " AND LH.SiteId IN (SELECT Items FROM [dbo].[Split] (@Division, ','))";
        //            if (CostCenterId != null) mCondStr = mCondStr + " AND H.CostCenter IN (SELECT Items FROM [dbo].[Split] (@CostCenter, ','))";
        //            if (FromDate != null) mCondStr = mCondStr + " AND LH.DocDate >= @FromDate";
        //            if (ToDate != null) mCondStr = mCondStr + " AND LH.DocDate <= @ToDate";
        //            if (LedgerAccountGroup != null && LedgerAccountGroup != "") mCondStr = mCondStr + " AND LAG.LedgerAccountGroupId = @LedgerAccountGroup";

        //            string mBalanceCondStr = "";
        //            //if (IncludeZeroBalance != null) mCondStr = "HAVING sum(isnull(H.AmtDr,0))  <> sum(isnull(H.AmtCr,0))";


        //            string mQry = @"WITH CTE_LedgerBalance AS 
        //                            (
        //	                            SELECT LAG.LedgerAccountGroupId, max(LAG.LedgerAccountGroupName) AS LedgerAccountGroupName, Max(Lag.ParentLedgerAccountGroupId) AS ParentLedgerAccountGroupId,
        //	                            Sum(isnull(H.AmtDr,0)) - sum(isnull(H.AmtCr,0)) AS Balance
        //	                            FROM web.LedgerHeaders LH  WITH (Nolock)
        //	                            LEFT JOIN web.Ledgers H WITH (Nolock) ON LH.LedgerHeaderId = H.LedgerHeaderId 
        //	                            LEFT JOIN web.LedgerAccounts LA  WITH (Nolock) ON LA.LedgerAccountId = H.LedgerAccountId 
        //	                            LEFT JOIN web.LedgerAccountGroups LAG  WITH (Nolock) ON LAG.LedgerAccountGroupId = LA.LedgerAccountGroupId 
        //	                            WHERE LAG.LedgerAccountGroupId IS NOT NULL   AND LH.DocDate >=  '01/Apr/2017' 
        //	                            AND LH.DocDate <= '30/Sep/2017'
        //	                            --AND LAG.LedgerAccountGroupName LIKE '%Sundry Debtors%'
        //	                            GROUP BY LAG.LedgerAccountGroupId 
        //                            ),--SELECT CASE WHEN balance>0 THEN balance END AS dr FROM cte_LedgerBalance, 
        //                            CTE_LedgerAccountGroup AS 
        //                            (
        //	                            SELECT L.LedgerAccountGroupId AS BaseLedgerAccountGroupId, L.LedgerAccountGroupId, L.LedgerAccountGroupName, L.ParentLedgerAccountGroupId AS ParentLedgerAccountGroupId, 0 AS [level]
        //	                            FROM CTE_LedgerBalance L
        //	                            UNION ALL
        //	                            SELECT H.BaseLedgerAccountGroupId, L.LedgerAccountGroupId, L.LedgerAccountGroupName, L.ParentLedgerAccountGroupId, H.level + 1
        //	                            FROM CTE_LedgerAccountGroup H 
        //	                            INNER JOIN CTE_LedgerBalance L ON H.ParentLedgerAccountGroupId = L.LedgerAccountGroupId
        //                            ),
        //                            CTE_LedgerBalanceTotals AS 
        //                            (
        //	                            SELECT IsNull(Sum(VTotals.AmtDr),0) AS TotalAmtDr, IsNull(Sum(VTotals.AmtCr),0) AS TotalAmtCr
        //	                            FROM (
        //		                            SELECT L.LedgerAccountGroupId, Max(L.LedgerAccountGroupName) AS LedgerAccountGroupName, Max(L.ParentLedgerAccountGroupId) AS ParentLedgerAccountGroupId, 
        //		                            CASE WHEN Sum(isnull(Lb.Balance,0)) > 0 THEN Sum(isnull(Lb.Balance,0)) ELSE NULL  END AS AmtDr,
        //		                            CASE WHEN Sum(isnull(Lb.Balance,0)) < 0 THEN abs(Sum(isnull(Lb.Balance,0))) ELSE NULL END AS AmtCr
        //		                            FROM CTE_LedgerAccountGroup L 
        //		                            LEFT JOIN CTE_LedgerBalance Lb ON L.BaseLedgerAccountGroupId = Lb.LedgerAccountGroupId 
        //		                            WHERE L.ParentLedgerAccountGroupId IS NULL
        //		                            GROUP BY L.LedgerAccountGroupId
        //	                            ) AS VTotals
        //                            )
        //
        //                            SELECT L.LedgerAccountGroupId, Max(L.LedgerAccountGroupName)  AS LedgerAccountGroupName, Max(L.ParentLedgerAccountGroupId) AS ParentLedgerAccountGroupId, 
        //                            CASE WHEN Sum(isnull(Lb.Balance,0)) > 0 THEN Sum(isnull(Lb.Balance,0)) ELSE NULL  END AS AmtDr,
        //                            CASE WHEN Sum(isnull(Lb.Balance,0)) < 0 THEN abs(Sum(isnull(Lb.Balance,0))) ELSE NULL END AS AmtCr,
        //                            Max(IsNull(Lbt.TotalAmtDr,0)) AS TotalAmtDr, Max(IsNull(Lbt.TotalAmtCr,0)) AS TotalAmtCr,
        //                            'Trial Balance' AS ReportType, 'Sub Trial Balance' AS OpenReportType
        //                            FROM CTE_LedgerAccountGroup L 
        //                            LEFT JOIN CTE_LedgerBalance Lb ON L.BaseLedgerAccountGroupId = Lb.LedgerAccountGroupId 
        //                            LEFT JOIN CTE_LedgerBalanceTotals Lbt ON 1=1
        //                            GROUP BY L.LedgerAccountGroupId
        //
        //                            UNION ALL 
        //
        //                            SELECT LA.LedgerAccountId AS LedgerAccountGroupId, max(LA.LedgerAccountName) AS LedgerAccountGroupName, 
        //                            Max(LA.LedgerAccountGroupId) AS ParentLedgerAccountGroupId,
        //                            CASE WHEN sum(isnull(H.AmtDr,0)) - sum(isnull(H.AmtCr,0)) > 0 THEN sum(isnull(H.AmtDr,0)) - sum(isnull(H.AmtCr,0)) ELSE NULL  END AS AmtDr,
        //                            CASE WHEN sum(isnull(H.AmtDr,0)) - sum(isnull(H.AmtCr,0)) < 0 THEN abs(sum(isnull(H.AmtDr,0)) - sum(isnull(H.AmtCr,0))) ELSE NULL END AS AmtCr,
        //                            Max(IsNull(Lbt.TotalAmtDr,0)) AS TotalAmtDr, Max(IsNull(Lbt.TotalAmtCr,0)) AS TotalAmtCr,
        //                            'Trial Balance' AS ReportType, 'Ledger' AS OpenReportType
        //                            FROM web.LedgerHeaders LH  WITH (Nolock)
        //                            LEFT JOIN web.Ledgers H WITH (Nolock) ON LH.LedgerHeaderId = H.LedgerHeaderId 
        //                            LEFT JOIN web.LedgerAccounts LA  WITH (Nolock) ON LA.LedgerAccountId = H.LedgerAccountId 
        //                            LEFT JOIN CTE_LedgerBalanceTotals Lbt ON 1=1
        //                            LEFT JOIN (
        //	                            SELECT Ag.ParentLedgerAccountGroupId
        //	                            FROM Web.LedgerAccountGroups Ag
        //	                            WHERE Ag.ParentLedgerAccountGroupId IS NOT NULL
        //	                            GROUP BY Ag.ParentLedgerAccountGroupId
        //                            ) AS V1 ON La.LedgerAccountGroupId = V1.ParentLedgerAccountGroupId
        //                            WHERE LH.DocDate >=  '01/Apr/2017' 
        //                            AND LH.DocDate <= '30/Sep/2017'
        //                            AND V1.ParentLedgerAccountGroupId IS NOT NULL
        //                            GROUP BY LA.LedgerAccountId ";


        //            IEnumerable<TrialBalanceDetailViewModel> TrialBalanceDetailList = db.Database.SqlQuery<TrialBalanceDetailViewModel>(mQry, SqlParameterSiteId, SqlParameterDivisionId, SqlParameterFromDate, SqlParameterToDate, SqlParameterLedgerAccountGroup).ToList();

        //            return TrialBalanceDetailList;

        //        }




        public string GetQryForTrialBalance(string SiteId, string DivisionId, string FromDate, string ToDate,
                            string CostCenterId, string IsIncludeZeroBalance, string IsIncludeOpening, string LedgerAccountGroup)
        {
            //And H.DocDate >= (Case When LAG.LedgerAccountGroupNature in ('Expenses','Income') Then @FromDate Else '1900/Jan/01' End) 

            string mQry = @" DECLARE @TempcteGroupBalance AS TABLE (
	                        LedgerAccountGroupId INT, 
	                        LedgerAccountGroupName NVARCHAR (50), 
	                        ParentLedgerAccountGroupId INT, 
	                        ParentLedgerAccountGroupName NVARCHAR (50), 
	                        Opening DECIMAL (38, 2), 
	                        AmtDr DECIMAL (38, 2), 
	                        AmtCr DECIMAL (38, 2), 
	                        Balance DECIMAL (38, 2)); ";


            mQry = mQry + @"With cteGroupBalance AS
                                    (
                                        SELECT LA.LedgerAccountGroupId, Max(LAG.LedgerAccountGroupName ) LedgerAccountGroupName, 
                                        Max(LAG.ParentLedgerAccountGroupId) ParentLedgerAccountGroupId, Max(PLAG.LedgerAccountGroupName) AS ParentLedgerAccountGroupName, 
                                        Sum(Case When LAG.LedgerAccountGroupNature in ('Expense','Income') Then 0 Else CASE WHEN H.DocDate < @FromDate THEN L.AmtDr-L.AmtCr ELSE 0 END END) AS Opening,
                                        Sum(CASE WHEN H.DocDate >= @FromDate AND H.DocDate <= @ToDate THEN L.AmtDr ELSE 0 END) AS AmtDr,
                                        Sum(CASE WHEN H.DocDate >= @FromDate AND H.DocDate <= @ToDate THEN L.AmtCr ELSE 0 END) AS AmtCr,
                                        Sum(Case When LAG.LedgerAccountGroupNature in ('Expense','Income') Then 
			                                        CASE WHEN H.DocDate >= @FromDate AND H.DocDate <= @ToDate THEN L.AmtDr-L.AmtCr ELSE 0 END 
		                                        ELSE CASE WHEN H.DocDate <= @ToDate THEN L.AmtDr-L.AmtCr ELSE 0 END END) AS Balance
                                        FROM Web.LedgerHeaders H 
                                        INNER JOIN web.Ledgers L ON L.LedgerHeaderId = H.LedgerHeaderId
                                        LEFT JOIN web.LedgerAccounts LA ON L.LedgerAccountId = LA.LedgerAccountId
                                        LEFT JOIN web.LedgerAccountGroups LAG ON LA.LedgerAccountGroupId = LAG.LedgerAccountGroupId  		
                                        LEFT JOIN web.LedgerAccountGroups PLAG ON PLAG.LedgerAccountGroupId = LAG.ParentLedgerAccountGroupId  		
                                        WHERE 1=1 " +
                            (SiteId != null ? " AND H.SiteId IN (SELECT Items FROM [dbo].[Split] (@Site, ','))" : "") +
                            (DivisionId != null ? " AND H.DivisionId IN (SELECT Items FROM [dbo].[Split] (@Division, ','))" : "") +
                            (CostCenterId != null ? " AND L.CostCenterId IN (SELECT Items FROM [dbo].[Split] (@CostCenter, ','))" : "") +
                            (IsIncludeOpening == "False" ? " AND H.DocDate >= @FromDate" : "") +
                            @" GROUP BY LA.LedgerAccountGroupId " +
                        //(IsIncludeZeroBalance == "False" ? " Having Sum(CASE WHEN H.DocDate <= @ToDate THEN L.AmtDr-L.AmtCr ELSE 0 END) <> 0 " : "") +
                        @") 

                    INSERT INTO @TempcteGroupBalance (LedgerAccountGroupId, LedgerAccountGroupName, ParentLedgerAccountGroupId, ParentLedgerAccountGroupName,
                    Opening, AmtDr, AmtCr, Balance)
                    SELECT IsNull(B.LedgerAccountGroupId,Ag.LedgerAccountGroupId) AS LedgerAccountGroupId, 
                    IsNull(B.LedgerAccountGroupName,Ag.LedgerAccountGroupName) AS LedgerAccountGroupName, 
                    IsNull(B.ParentLedgerAccountGroupId, Ag.ParentLedgerAccountGroupId) AS ParentLedgerAccountGroupId, 
                    IsNull(B.ParentLedgerAccountGroupName,Pag.LedgerAccountGroupName) AS ParentLedgerAccountGroupName,
                    IsNull(B.Opening,0) AS Opening, IsNull(B.AmtDr,0) AS AmtDr, 
                    IsNull(B.AmtCr,0) AS AmtCr, IsNull(B.Balance,0) AS Balance
                    FROM Web.LedgerAccountGroups Ag 
                    LEFT JOIN Web.LedgerAccountGroups Pag ON Ag.ParentLedgerAccountGroupId = Pag.LedgerAccountGroupId
                    LEFT JOIN cteGroupBalance B ON Ag.LedgerAccountGroupId = B.LedgerAccountGroupId;";


            mQry = mQry + @" WITH cteLedgerBalance AS
                                    (
                                        SELECT L.LedgerAccountId, Max(LA.LedgerAccountName + 
		                                Case WHEN P.PersonId IS NOT NULL 
			                                 THEN CASE WHEN P.Suffix = P.Code THEN ' [' + P.Code + ']'
			 								                                  ELSE ', ' + P.Suffix + ' [' + P.Code + ']' END	
			                                 ELSE ', ' + LA.LedgerAccountSuffix END) AS LedgerAccountName,
                                        Sum(Case When LAG.LedgerAccountGroupNature in ('Expense','Income') Then 0 Else CASE WHEN H.DocDate < @FromDate THEN L.AmtDr - L.AmtCr ELSE 0 END END) AS Opening,
                                        Sum(CASE WHEN H.DocDate >= @FromDate AND H.DocDate <= @ToDate THEN L.AmtDr ELSE 0 END) AS AmtDr,
                                        Sum(CASE WHEN H.DocDate >= @FromDate AND H.DocDate <= @ToDate THEN L.AmtCr ELSE 0 END) AS AmtCr,
                                        Sum(Case When LAG.LedgerAccountGroupNature in ('Expense','Income') Then 
			                                        CASE WHEN H.DocDate >= @FromDate AND H.DocDate <= @ToDate THEN L.AmtDr-L.AmtCr ELSE 0 END 
		                                        ELSE CASE WHEN H.DocDate <= @ToDate THEN L.AmtDr-L.AmtCr ELSE 0 END END) AS Balance
                                        FROM Web.LedgerHeaders H 
                                        INNER JOIN web.Ledgers L ON L.LedgerHeaderId = H.LedgerHeaderId
                                        LEFT JOIN web.LedgerAccounts LA ON L.LedgerAccountId = LA.LedgerAccountId
                                        LEFT JOIN web.LedgerAccountGroups LAG ON LA.LedgerAccountGroupId = LAG.LedgerAccountGroupId  		
                                        LEFT JOIN Web.People P On LA.PersonId = P.PersonId
                                        WHERE 1 = 1 " +
                            (SiteId != null ? " AND H.SiteId IN (SELECT Items FROM [dbo].[Split] (@Site, ','))" : "") +
                            (DivisionId != null ? " AND H.DivisionId IN (SELECT Items FROM [dbo].[Split] (@Division, ','))" : "") +
                            (CostCenterId != null ? " AND L.CostCenterId IN (SELECT Items FROM [dbo].[Split] (@CostCenter, ','))" : "") +
                            (IsIncludeOpening == "False" ? " AND H.DocDate >= @FromDate" : "") +
                            @" And " + (LedgerAccountGroup != null && LedgerAccountGroup != "" ? "LA.LedgerAccountGroupId IN (SELECT Items FROM [dbo].[Split] (@LedgerAccountGroup, ',')) " : "LA.LedgerAccountGroupId  Is Null ") +
                            @" GROUP BY L.LedgerAccountId " +
                        @"), 
                        cteAcGroup as
                                    (
                                        SELECT ag.LedgerAccountGroupId AS BaseLedgerAccountGroupId, ag.LedgerAccountGroupName AS BaseLedgerAccountGroupName, ag.LedgerAccountGroupId, ag.LedgerAccountGroupName, ag.ParentLedgerAccountGroupId, ag.ParentLedgerAccountGroupName, ag.Opening, ag.AmtDr, ag.AmtCr, ag.Balance, 0 AS Level   
                                        FROM @TempcteGroupBalance ag		
                                        WHERE  " + (LedgerAccountGroup != null && LedgerAccountGroup != "" ? " ag.ParentLedgerAccountGroupId IN (SELECT Items FROM [dbo].[Split] (@LedgerAccountGroup, ','))  " : "ag.ParentLedgerAccountGroupId Is Null ") +
                            @"UNION ALL
                                        SELECT cteAcGroup.BaseLedgerAccountGroupId, cteAcGroup.BaseLedgerAccountGroupName , ag.LedgerAccountGroupId, ag.LedgerAccountGroupName, ag.ParentLedgerAccountGroupId, ag.ParentLedgerAccountGroupName, ag.Opening, ag.AmtDr, ag.AmtCr, ag.Balance, LEVEL +1     
                                        FROM @TempcteGroupBalance ag
                                        INNER JOIN cteAcGroup  ON cteAcGroup.LedgerAccountGroupId = Ag.ParentLedgerAccountGroupId 
                                    ) ";

            return mQry;
        }


        public IEnumerable<TrialBalanceViewModel> GetTrialBalanceDetailWithFullHierarchy(FinancialDisplaySettings Settings)
        {
            string SpaceChar = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";

            var SiteSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "Site" select H).FirstOrDefault();
            var DivisionSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "Division" select H).FirstOrDefault();
            var FromDateSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "FromDate" select H).FirstOrDefault();
            var ToDateSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "ToDate" select H).FirstOrDefault();
            var CostCenterSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "CostCenter" select H).FirstOrDefault();
            var IsIncludeZeroBalanceSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "IsIncludeZeroBalance" select H).FirstOrDefault();
            var IsIncludeOpeningSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "IsIncludeOpening" select H).FirstOrDefault();

            var LedgerAccountGroupSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "LedgerAccountGroup" select H).FirstOrDefault();


            string SiteId = SiteSetting.Value;
            string DivisionId = DivisionSetting.Value;
            string FromDate = FromDateSetting.Value;
            string ToDate = ToDateSetting.Value;
            string CostCenterId = CostCenterSetting.Value;
            string IsIncludeZeroBalance = IsIncludeZeroBalanceSetting.Value;
            string IsIncludeOpening = IsIncludeOpeningSetting.Value;
            string LedgerAccountGroup = LedgerAccountGroupSetting.Value;


            SqlParameter SqlParameterSiteId = new SqlParameter("@Site", !string.IsNullOrEmpty(SiteId) ? SiteId : (object)DBNull.Value);
            SqlParameter SqlParameterDivisionId = new SqlParameter("@Division", !string.IsNullOrEmpty(DivisionId) ? DivisionId : (object)DBNull.Value);
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", FromDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", ToDate);
            SqlParameter SqlParameterCostCenter = new SqlParameter("@CostCenter", !string.IsNullOrEmpty(CostCenterId) ? CostCenterId : (object)DBNull.Value);
            SqlParameter SqlParameterLedgerAccountGroup = new SqlParameter("@LedgerAccountGroup", !string.IsNullOrEmpty(LedgerAccountGroup) ? LedgerAccountGroup : (object)DBNull.Value);

            Decimal TotalAmtDr = 0;
            Decimal TotalAmtCr = 0;


            string mQry = GetQryForTrialBalance(SiteId, DivisionId, FromDate, ToDate, CostCenterId, IsIncludeZeroBalance, IsIncludeOpening, LedgerAccountGroup) +
                            @"SELECT H.BaseLedgerAccountGroupId AS LedgerAccountGroupId, '<Strong>' +  Max(BaseLedgerAccountGroupName) + '</Strong>' AS LedgerAccountGroupName, 
                                        CASE WHEN Sum(isnull(H.Balance,0)) > 0 THEN Sum(isnull(H.Balance,0)) ELSE NULL  END AS AmtDr,
                                        CASE WHEN Sum(isnull(H.Balance,0)) < 0 THEN abs(Sum(isnull(H.Balance,0))) ELSE NULL END AS AmtCr,
                                        NULL AS LedgerAccountId, 'Trial Balance' ReportType, 'Trial Balance' AS OpenReportType, Max(BaseLedgerAccountGroupName) As OrderByColumn,
                                        Max(BaseLedgerAccountGroupName) As ParentLedgerAccountGroupName, Max(BaseLedgerAccountGroupName) As TopParentLedgerAccountGroupName, 0 As GroupLevel
                                        FROM cteAcGroup H 
                                        LEFT JOIN Web.LedgerAccountGroups Ag ON H.BaseLedgerAccountGroupId = Ag.LedgerAccountGroupId
                                        GROUP BY H.BaseLedgerAccountGroupId 
                                        Having (Sum(isnull(H.Opening,0)) <> 0 Or Sum(isnull(H.AmtDr,0)) <> 0 Or Sum(isnull(H.AmtCr,0)) <> 0 Or Sum(isnull(H.Balance,0)) <> 0) " +
                            (IsIncludeZeroBalance == "False" ? " And Sum(isnull(H.Balance,0)) <> 0 " : "") +
                            @" ORDER BY OrderByColumn ";

            List<TrialBalanceViewModel> TrialBalanceList = db.Database.SqlQuery<TrialBalanceViewModel>(mQry, SqlParameterSiteId, SqlParameterDivisionId, SqlParameterFromDate, SqlParameterToDate, SqlParameterCostCenter, SqlParameterLedgerAccountGroup).ToList();


            if (LedgerAccountGroup == null || LedgerAccountGroup == "" || LedgerAccountGroup == "0")
            {
                TotalAmtDr = TrialBalanceList.Sum(m => m.AmtDr) ?? 0;
                TotalAmtCr = TrialBalanceList.Sum(m => m.AmtCr) ?? 0;

                if (TotalAmtDr != TotalAmtCr)
                {
                    TrialBalanceViewModel BlankItem = new TrialBalanceViewModel();
                    BlankItem.TopParentLedgerAccountGroupName = "ZZZ";
                    TrialBalanceList.Add(BlankItem);
                    TrialBalanceViewModel DiffVm = new TrialBalanceViewModel();
                    DiffVm.LedgerAccountGroupName = "<Strong>Difference in Trial Balance</Strong>";
                    DiffVm.TopParentLedgerAccountGroupName = "ZZZ";
                    if (TotalAmtDr > TotalAmtCr)
                        DiffVm.AmtCr = TotalAmtDr - TotalAmtCr;
                    else
                        DiffVm.AmtDr = TotalAmtCr - TotalAmtDr;
                    TrialBalanceList.Add(DiffVm);
                }
            }

            TotalAmtDr = TrialBalanceList.Sum(m => m.AmtDr) ?? 0;
            TotalAmtCr = TrialBalanceList.Sum(m => m.AmtCr) ?? 0;


            //LedgerAccountGroup = string.Join<string>(",", TrialBalanceList.Select(m => m.LedgerAccountGroupId.ToString()));
            LedgerAccountGroup = string.Join<string>(",", db.LedgerAccountGroup.Select(m => m.LedgerAccountGroupId.ToString()));
            //LedgerAccountGroup = "27,1012";
            SqlParameter SqlParameterSiteId_Child = new SqlParameter("@Site", !string.IsNullOrEmpty(SiteId) ? SiteId : (object)DBNull.Value);
            SqlParameter SqlParameterDivisionId_Child = new SqlParameter("@Division", !string.IsNullOrEmpty(DivisionId) ? DivisionId : (object)DBNull.Value);
            SqlParameter SqlParameterFromDate_Child = new SqlParameter("@FromDate", FromDate);
            SqlParameter SqlParameterToDate_Child = new SqlParameter("@ToDate", ToDate);
            SqlParameter SqlParameterCostCenter_Child = new SqlParameter("@CostCenter", !string.IsNullOrEmpty(CostCenterId) ? CostCenterId : (object)DBNull.Value);
            SqlParameter SqlParameterLedgerAccountGroup_Child = new SqlParameter("@LedgerAccountGroup", !string.IsNullOrEmpty(LedgerAccountGroup) ? LedgerAccountGroup : (object)DBNull.Value);

            mQry = GetQryForTrialBalance(SiteId, DivisionId, FromDate, ToDate, CostCenterId, IsIncludeZeroBalance, IsIncludeOpening, LedgerAccountGroup) +
                            @"SELECT H.BaseLedgerAccountGroupId AS LedgerAccountGroupId, REPLICATE('" + SpaceChar + @"', Max(Tag.[GroupLevel])) + '<Strong>' +  Max(BaseLedgerAccountGroupName) + '</Strong>' AS LedgerAccountGroupName, 
                                        CASE WHEN Sum(isnull(H.Balance,0)) > 0 THEN Sum(isnull(H.Balance,0)) ELSE NULL  END AS AmtDr,
                                        CASE WHEN Sum(isnull(H.Balance,0)) < 0 THEN abs(Sum(isnull(H.Balance,0))) ELSE NULL END AS AmtCr,
                                        NULL AS LedgerAccountId, 'Trial Balance' ReportType, 'Trial Balance' AS OpenReportType, 
                                        Web.FGetAllParentsForChild (H.BaseLedgerAccountGroupId) + Convert(NVARCHAR,Max(Tag.[GroupLevel])) + Max(BaseLedgerAccountGroupName) As OrderByColumn,
                                        Max(PAg.LedgerAccountGroupName) AS ParentLedgerAccountGroupName,
                                        Max(Tag.TopParentLedgerAccountGroupName) AS TopParentLedgerAccountGroupName, Max(Tag.[GroupLevel]) AS GroupLevel
                                        FROM cteAcGroup H 
                                        LEFT JOIN Web.LedgerAccountGroups Ag ON H.BaseLedgerAccountGroupId = Ag.LedgerAccountGroupId
                                        LEFT JOIN Web.LedgerAccountGroups PAg ON Ag.ParentLedgerAccountGroupId = PAg.LedgerAccountGroupId
                                        LEFT JOIN Web.ViewLedgerAccountGroupsWithParent Tag ON Ag.LedgerAccountGroupId = Tag.LedgerAccountGroupId
                                        GROUP BY H.BaseLedgerAccountGroupId 
                                        Having (Sum(isnull(H.Opening,0)) <> 0 Or Sum(isnull(H.AmtDr,0)) <> 0 Or Sum(isnull(H.AmtCr,0)) <> 0 Or Sum(isnull(H.Balance,0)) <> 0) " +
                            (IsIncludeZeroBalance == "False" ? " And Sum(isnull(H.Balance,0)) <> 0 " : "") +

                            @"UNION ALL 
                
                                        SELECT H.LedgerAccountId AS LedgerAccountGroupId, REPLICATE('" + SpaceChar + @"', Tag.[GroupLevel] + 1) + H.LedgerAccountName AS LedgerAccountGroupName, 
                                        CASE WHEN isnull(H.Balance,0) > 0 THEN isnull(H.Balance,0) ELSE NULL  END AS AmtDr,
                                        CASE WHEN isnull(H.Balance,0) < 0 THEN abs(isnull(H.Balance,0)) ELSE NULL END AS AmtCr,
                                        H.LedgerAccountId AS LedgerAccountId, 'Trial Balance' ReportType, 'Ledger' AS OpenReportType, 
                                        Web.FGetAllParentsForChild (Ag.LedgerAccountGroupId) + Convert(NVARCHAR,Tag.[GroupLevel] + 1) + H.LedgerAccountName As OrderByColumn,
                                        Ag.LedgerAccountGroupName AS ParentLedgerAccountGroupName,
                                        IsNull(Tag.TopParentLedgerAccountGroupName,Tag.LedgerAccountGroupName) AS TopParentLedgerAccountGroupName, Tag.[GroupLevel] + 1 AS GroupLevel
                                        FROM cteLedgerBalance H 
                                        LEFT JOIN Web.LedgerAccounts A ON H.LedgerAccountId = A.LedgerAccountId 
                                        LEFT JOIN Web.LedgerAccountGroups Ag ON A.LedgerAccountGroupId = Ag.LedgerAccountGroupId 
                                        LEFT JOIN Web.ViewLedgerAccountGroupsWithParent Tag ON Ag.LedgerAccountGroupId = Tag.LedgerAccountGroupId
                                        Where (isnull(H.Opening,0) <> 0 Or isnull(H.AmtDr,0) <> 0 Or isnull(H.AmtCr,0) <> 0 Or isnull(H.Balance,0) <> 0) " +
                            (IsIncludeZeroBalance == "False" ? " And isnull(H.Balance,0) <> 0 " : "") +
                            @" ORDER BY OrderByColumn ";

            IEnumerable<TrialBalanceViewModel> ChileTrialBalanceList = db.Database.SqlQuery<TrialBalanceViewModel>(mQry, SqlParameterSiteId_Child, SqlParameterDivisionId_Child, SqlParameterFromDate_Child, SqlParameterToDate_Child, SqlParameterCostCenter_Child, SqlParameterLedgerAccountGroup_Child).ToList();

            IEnumerable<TrialBalanceViewModel> TrialBalanceViewModelCombind = TrialBalanceList.Union(ChileTrialBalanceList).ToList().OrderBy(m => m.TopParentLedgerAccountGroupName);

            foreach (var item in TrialBalanceViewModelCombind)
            {
                item.TotalAmtDr = TotalAmtDr;
                item.TotalAmtCr = TotalAmtCr;
            }

            return TrialBalanceViewModelCombind;

        }


        public IEnumerable<TrialBalanceSummaryViewModel> GetTrialBalanceDetailSummaryWithFullHierarchy(FinancialDisplaySettings Settings)
        {
            string SpaceChar = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";

            var SiteSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "Site" select H).FirstOrDefault();
            var DivisionSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "Division" select H).FirstOrDefault();
            var FromDateSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "FromDate" select H).FirstOrDefault();
            var ToDateSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "ToDate" select H).FirstOrDefault();
            var CostCenterSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "CostCenter" select H).FirstOrDefault();
            var IsIncludeZeroBalanceSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "IsIncludeZeroBalance" select H).FirstOrDefault();
            var IsIncludeOpeningSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "IsIncludeOpening" select H).FirstOrDefault();

            var LedgerAccountGroupSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "LedgerAccountGroup" select H).FirstOrDefault();


            string SiteId = SiteSetting.Value;
            string DivisionId = DivisionSetting.Value;
            string FromDate = FromDateSetting.Value;
            string ToDate = ToDateSetting.Value;
            string CostCenterId = CostCenterSetting.Value;
            string IsIncludeZeroBalance = IsIncludeZeroBalanceSetting.Value;
            string IsIncludeOpening = IsIncludeOpeningSetting.Value;
            string LedgerAccountGroup = LedgerAccountGroupSetting.Value;


            SqlParameter SqlParameterSiteId = new SqlParameter("@Site", !string.IsNullOrEmpty(SiteId) ? SiteId : (object)DBNull.Value);
            SqlParameter SqlParameterDivisionId = new SqlParameter("@Division", !string.IsNullOrEmpty(DivisionId) ? DivisionId : (object)DBNull.Value);
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", FromDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", ToDate);
            SqlParameter SqlParameterCostCenter = new SqlParameter("@CostCenter", !string.IsNullOrEmpty(CostCenterId) ? CostCenterId : (object)DBNull.Value);
            SqlParameter SqlParameterLedgerAccountGroup = new SqlParameter("@LedgerAccountGroup", !string.IsNullOrEmpty(LedgerAccountGroup) ? LedgerAccountGroup : (object)DBNull.Value);



            string mQry = GetQryForTrialBalance(SiteId, DivisionId, FromDate, ToDate, CostCenterId, IsIncludeZeroBalance, IsIncludeOpening, LedgerAccountGroup) +
                            @"SELECT H.BaseLedgerAccountGroupId AS LedgerAccountGroupId, '<Strong>' +  Max(H.BaseLedgerAccountGroupName) + '</Strong>' AS LedgerAccountGroupName, 
                                CASE WHEN Sum(isnull(H.Opening,0)) <> 0 THEN Abs(Sum(isnull(H.Opening,0))) ELSE NULL END AS Opening,
                                CASE WHEN Sum(isnull(H.Opening,0)) > 0 THEN 'Dr' 
	                                 WHEN Sum(isnull(H.Opening,0)) < 0 THEN 'Cr' 
                                ELSE NULL END AS OpeningDrCr,
                                CASE WHEN Sum(isnull(H.AmtDr,0)) <> 0 Then Sum(isnull(H.AmtDr,0)) Else Null End AS AmtDr,
                                CASE WHEN Sum(isnull(H.AmtCr,0)) <> 0 Then Sum(isnull(H.AmtCr,0)) Else Null End AS AmtCr,
                                CASE WHEN Abs(Sum(isnull(H.Balance,0))) <> 0 Then Abs(Sum(isnull(H.Balance,0))) Else Null End As Balance,
                                CASE WHEN Sum(isnull(H.Balance,0)) > 0 THEN 'Dr' 
	                                 WHEN Sum(isnull(H.Balance,0)) < 0 THEN 'Cr' 
                                ELSE NULL END AS BalanceDrCr,
                                NULL AS LedgerAccountId, 'Trial Balance' ReportType, 'Trial Balance' AS OpenReportType, Max(H.BaseLedgerAccountGroupName) As OrderByColumn,
                                Max(BaseLedgerAccountGroupName) As ParentLedgerAccountGroupName, Max(BaseLedgerAccountGroupName) As TopParentLedgerAccountGroupName, 0 As GroupLevel
                                FROM cteAcGroup H 
                                GROUP BY H.BaseLedgerAccountGroupId
                                Having (Sum(isnull(H.Opening,0)) <> 0 Or Sum(isnull(H.AmtDr,0)) <> 0 Or Sum(isnull(H.AmtCr,0)) <> 0 Or Sum(isnull(H.Balance,0)) <> 0) " +
                                (IsIncludeZeroBalance == "False" ? " And Sum(isnull(H.Balance,0)) <> 0 " : "");

            IEnumerable<TrialBalanceSummaryViewModel> TrialBalanceSummaryList = db.Database.SqlQuery<TrialBalanceSummaryViewModel>(mQry, SqlParameterSiteId, SqlParameterDivisionId, SqlParameterFromDate, SqlParameterToDate, SqlParameterCostCenter, SqlParameterLedgerAccountGroup).ToList();


            Decimal TotalAmtDr = 0;
            Decimal TotalAmtCr = 0;
            Decimal TotalOpening = 0;
            string TotalOpeningDrCr = "";
            Decimal TotalBalance = 0;
            string TotalBalanceDrCr = "";

            TotalAmtDr = TrialBalanceSummaryList.Sum(m => m.AmtDr) ?? 0;
            TotalAmtCr = TrialBalanceSummaryList.Sum(m => m.AmtCr) ?? 0;

            foreach (var item in TrialBalanceSummaryList)
            {
                if (item.OpeningDrCr == "Dr")
                    TotalOpening = TotalOpening + (item.Opening ?? 0);
                else
                    TotalOpening = TotalOpening - (item.Opening ?? 0);

                if (item.BalanceDrCr == "Dr")
                    TotalBalance = TotalBalance + (item.Balance ?? 0);
                else
                    TotalBalance = TotalBalance - (item.Balance ?? 0);
            }

            if (TotalOpening > 0)
                TotalOpeningDrCr = "Dr";
            else if (TotalOpening < 0)
                TotalOpeningDrCr = "Cr";

            if (TotalBalance > 0)
                TotalBalanceDrCr = "Dr";
            else if (TotalBalance < 0)
                TotalBalanceDrCr = "Cr";


            //LedgerAccountGroup = string.Join<string>(",", TrialBalanceSummaryList.Select(m => m.LedgerAccountGroupId.ToString()));
            LedgerAccountGroup = string.Join<string>(",", db.LedgerAccountGroup.Select(m => m.LedgerAccountGroupId.ToString()));
            SqlParameter SqlParameterSiteId_Child = new SqlParameter("@Site", !string.IsNullOrEmpty(SiteId) ? SiteId : (object)DBNull.Value);
            SqlParameter SqlParameterDivisionId_Child = new SqlParameter("@Division", !string.IsNullOrEmpty(DivisionId) ? DivisionId : (object)DBNull.Value);
            SqlParameter SqlParameterFromDate_Child = new SqlParameter("@FromDate", FromDate);
            SqlParameter SqlParameterToDate_Child = new SqlParameter("@ToDate", ToDate);
            SqlParameter SqlParameterCostCenter_Child = new SqlParameter("@CostCenter", !string.IsNullOrEmpty(CostCenterId) ? CostCenterId : (object)DBNull.Value);
            SqlParameter SqlParameterLedgerAccountGroup_Child = new SqlParameter("@LedgerAccountGroup", !string.IsNullOrEmpty(LedgerAccountGroup) ? LedgerAccountGroup : (object)DBNull.Value);

            mQry = GetQryForTrialBalance(SiteId, DivisionId, FromDate, ToDate, CostCenterId, IsIncludeZeroBalance, IsIncludeOpening, LedgerAccountGroup) +
                            @"SELECT H.BaseLedgerAccountGroupId AS LedgerAccountGroupId, REPLICATE('" + SpaceChar + @"', Max(Tag.[GroupLevel])) + '<Strong>' +  Max(H.BaseLedgerAccountGroupName) + '</Strong>' AS LedgerAccountGroupName, 
                                CASE WHEN Sum(isnull(H.Opening,0)) <> 0 THEN Abs(Sum(isnull(H.Opening,0))) ELSE NULL END AS Opening,
                                CASE WHEN Sum(isnull(H.Opening,0)) > 0 THEN 'Dr' 
	                                 WHEN Sum(isnull(H.Opening,0)) < 0 THEN 'Cr' 
                                ELSE NULL END AS OpeningDrCr,
                                CASE WHEN Sum(isnull(H.AmtDr,0)) <> 0 Then Sum(isnull(H.AmtDr,0)) Else Null End AS AmtDr,
                                CASE WHEN Sum(isnull(H.AmtCr,0)) <> 0 Then Sum(isnull(H.AmtCr,0)) Else Null End AS AmtCr,
                                CASE WHEN Abs(Sum(isnull(H.Balance,0))) <> 0 Then Abs(Sum(isnull(H.Balance,0))) Else Null End As Balance,
                                CASE WHEN Sum(isnull(H.Balance,0)) > 0 THEN 'Dr' 
	                                 WHEN Sum(isnull(H.Balance,0)) < 0 THEN 'Cr' 
                                ELSE NULL END AS BalanceDrCr,
                                NULL AS LedgerAccountId, 'Trial Balance' ReportType, 'Trial Balance' AS OpenReportType, 
                                Web.FGetAllParentsForChild (H.BaseLedgerAccountGroupId) + Convert(NVARCHAR,Max(Tag.[GroupLevel])) + Max(BaseLedgerAccountGroupName) As OrderByColumn,
                                Max(PAg.LedgerAccountGroupName) AS ParentLedgerAccountGroupName,
                                Max(Tag.TopParentLedgerAccountGroupName) AS TopParentLedgerAccountGroupName, Max(Tag.[GroupLevel]) AS GroupLevel
                                FROM cteAcGroup H 
                                LEFT JOIN Web.LedgerAccountGroups Ag ON H.BaseLedgerAccountGroupId = Ag.LedgerAccountGroupId
                                LEFT JOIN Web.LedgerAccountGroups PAg ON Ag.ParentLedgerAccountGroupId = PAg.LedgerAccountGroupId
                                LEFT JOIN Web.ViewLedgerAccountGroupsWithParent Tag ON Ag.LedgerAccountGroupId = Tag.LedgerAccountGroupId
                                GROUP BY H.BaseLedgerAccountGroupId
                                Having (Sum(isnull(H.Opening,0)) <> 0 Or Sum(isnull(H.AmtDr,0)) <> 0 Or Sum(isnull(H.AmtCr,0)) <> 0 Or Sum(isnull(H.Balance,0)) <> 0) " +
                                (IsIncludeZeroBalance == "False" ? " And Sum(isnull(H.Balance,0)) <> 0 " : "") +

                                @" UNION ALL 

                                SELECT H.LedgerAccountId AS LedgerAccountGroupId, REPLICATE('" + SpaceChar + @"', Tag.[GroupLevel] + 1) + H.LedgerAccountName AS LedgerAccountGroupName, 
                                CASE WHEN isnull(H.Opening,0) <> 0 THEN Abs(isnull(H.Opening,0)) ELSE NULL END AS Opening,
                                CASE WHEN isnull(H.Opening,0) > 0 THEN 'Dr' 
	                                 WHEN isnull(H.Opening,0) < 0 THEN 'Cr' 
                                ELSE NULL END AS OpeningDrCr,
                                CASE WHEN isnull(H.AmtDr,0) <> 0 Then isnull(H.AmtDr,0) Else Null End AS AmtDr,
                                CASE WHEN isnull(H.AmtCr,0) <> 0 Then isnull(H.AmtCr,0) Else Null End AS AmtCr,
                                CASE WHEN Abs(isnull(H.Balance,0)) <> 0 Then Abs(isnull(H.Balance,0)) Else Null End As Balance,
                                CASE WHEN isnull(H.Balance,0) > 0 THEN 'Dr' 
	                                 WHEN isnull(H.Balance,0) < 0 THEN 'Cr' 
                                ELSE NULL END AS BalanceDrCr,
                                H.LedgerAccountId AS LedgerAccountId, 'Trial Balance' ReportType, 'Ledger' AS OpenReportType, 
                                Web.FGetAllParentsForChild (Ag.LedgerAccountGroupId) + Convert(NVARCHAR,Tag.[GroupLevel] + 1) + H.LedgerAccountName As OrderByColumn,
                                Ag.LedgerAccountGroupName AS ParentLedgerAccountGroupName,
                                IsNull(Tag.TopParentLedgerAccountGroupName,Tag.LedgerAccountGroupName) AS TopParentLedgerAccountGroupName, Tag.[GroupLevel] + 1 AS GroupLevel
                                FROM cteLedgerBalance H 
                                LEFT JOIN Web.LedgerAccounts A ON H.LedgerAccountId = A.LedgerAccountId 
                                LEFT JOIN Web.LedgerAccountGroups Ag ON A.LedgerAccountGroupId = Ag.LedgerAccountGroupId 
                                LEFT JOIN Web.ViewLedgerAccountGroupsWithParent Tag ON Ag.LedgerAccountGroupId = Tag.LedgerAccountGroupId
                                Where (isnull(H.Opening,0) <> 0 Or isnull(H.AmtDr,0) <> 0 Or isnull(H.AmtCr,0) <> 0 Or isnull(H.Balance,0) <> 0) " +
                                (IsIncludeZeroBalance == "False" ? " And isnull(H.Balance,0) <> 0 " : "") +
                                @" ORDER BY OrderByColumn ";

            IEnumerable<TrialBalanceSummaryViewModel> ChileTrialBalanceSummaryList = db.Database.SqlQuery<TrialBalanceSummaryViewModel>(mQry, SqlParameterSiteId_Child, SqlParameterDivisionId_Child, SqlParameterFromDate_Child, SqlParameterToDate_Child, SqlParameterCostCenter_Child, SqlParameterLedgerAccountGroup_Child).ToList();

            IEnumerable<TrialBalanceSummaryViewModel> TrialBalanceSummaryViewModelCombind = TrialBalanceSummaryList.Union(ChileTrialBalanceSummaryList).ToList().OrderBy(m => m.TopParentLedgerAccountGroupName);

            foreach (var item in TrialBalanceSummaryViewModelCombind)
            {
                item.TotalAmtDr = TotalAmtDr;
                item.TotalAmtCr = TotalAmtCr;

                item.TotalOpening = Math.Abs(TotalOpening);
                item.TotalOpeningDrCr = TotalOpeningDrCr;
                item.TotalBalance = Math.Abs(TotalBalance);
                item.TotalBalanceDrCr = TotalBalanceDrCr;
            }

            return TrialBalanceSummaryViewModelCombind;

        }

        public IEnumerable<BalanceSheetViewModel> GetBalanceSheet(FinancialDisplaySettings Settings)
        {
            string StrCondition1 = "";
            DataTable DTTemp = new DataTable();
            Decimal DblDebit_Total = 0;
            Decimal DblCredit_Total = 0;
            int I = 0, J = 0;
            Decimal DblNet_Profit_Loss = 0;
            string mQry = "";
            DataTable FGMain = new DataTable();
            List<BalanceSheetViewModel> BalanceSheetViewModelList = new List<BalanceSheetViewModel>();

            var SiteSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "Site" select H).FirstOrDefault();
            var DivisionSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "Division" select H).FirstOrDefault();
            var FromDateSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "FromDate" select H).FirstOrDefault();
            var ToDateSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "ToDate" select H).FirstOrDefault();
            var CostCenterSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "CostCenter" select H).FirstOrDefault();
            var IsIncludeZeroBalanceSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "IsIncludeZeroBalance" select H).FirstOrDefault();
            var IsIncludeOpeningSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "IsIncludeOpening" select H).FirstOrDefault();
            var LedgerAccountGroupSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "LedgerAccountGroup" select H).FirstOrDefault();

            string SiteId = SiteSetting.Value;
            string DivisionId = DivisionSetting.Value;
            string FromDate = FromDateSetting.Value;
            string ToDate = ToDateSetting.Value;
            string CostCenterId = CostCenterSetting.Value;
            string IsIncludeZeroBalance = IsIncludeZeroBalanceSetting.Value;
            string IsIncludeOpening = IsIncludeOpeningSetting.Value;
            string LedgerAccountGroup = LedgerAccountGroupSetting.Value;


            StrCondition1 = " Where H.DocDate <= '" + ToDate + "' ";
            if (SiteId != null)
                StrCondition1 = " And H.SiteId In ('" + SiteId + "') ";
            if (DivisionId != null)
                StrCondition1 = " And H.DivisionId In ('" + DivisionId + "') ";

            //========== For Detail Section =======


            mQry = GetQryForTrialBalance(SiteId, DivisionId, FromDate, ToDate, CostCenterId, IsIncludeZeroBalance, IsIncludeOpening, LedgerAccountGroup) +
                    @"SELECT H.BaseLedgerAccountGroupId AS LedgerAccountGroupId, Max(BaseLedgerAccountGroupName) AS GName, 
                                CASE WHEN Sum(isnull(H.Balance,0)) > 0 THEN Sum(isnull(H.Balance,0)) ELSE 0 END AS AmtDr,
                                CASE WHEN Sum(isnull(H.Balance,0)) < 0 THEN abs(Sum(isnull(H.Balance,0))) ELSE 0 END AS AmtCr,
                                Max(BaseLedgerAccountGroupName) AS ContraGroupName,
                                Max(Ag.LedgerAccountGroupNature) As GroupNature
                                FROM cteAcGroup H 
                                LEFT JOIN Web.LedgerAccountGroups Ag On AG.LedgerAccountGroupId = H.BaseLedgerAccountGroupId
                                Where AG.LedgerAccountGroupNature In ('Asset', 'Liability') 
                                GROUP BY H.BaseLedgerAccountGroupId 
                                Having (Sum(isnull(H.Opening,0)) <> 0 Or Sum(isnull(H.AmtDr,0)) <> 0 Or Sum(isnull(H.AmtCr,0)) <> 0 Or Sum(isnull(H.Balance,0)) <> 0) ";


            //mQry = @"SELECT (Case IsNull(AG1.LedgerAccountGroupId,'') When '' Then IsNull(AG.LedgerAccountGroupId,'') 
            //            Else IsNull(AG1.LedgerAccountGroupId, '') End)  As LedgerAccountGroupId,
            //            Max(Case IsNull(AG1.LedgerAccountGroupName, '') When '' Then IsNull(AG.LedgerAccountGroupName, '')
            //            Else IsNull(AG1.LedgerAccountGroupName, '') End)  As GName,
            //            (Case When(IsNull(Sum(LG.AmtDr), 0) - IsNull(Sum(LG.AmtCr), 0)) > 0 Then
            //            (IsNull(Sum(LG.AmtDr), 0) - IsNull(Sum(LG.AmtCr), 0)) Else 0 End) As AmtDr,
            //            (Case When(IsNull(Sum(LG.AmtCr), 0) - IsNull(Sum(LG.AmtDr), 0)) > 0 Then
            //            (IsNull(Sum(LG.AmtCr), 0) - IsNull(Sum(LG.AmtDr), 0)) Else 0 End) As AmtCr,
            //            Max(Case IsNull(AG1.LedgerAccountGroupId, '') When '' Then IsNull(AG.LedgerAccountGroupName, '') Else IsNull(AG1.LedgerAccountGroupName, '') End)  
            //            As ContraGroupName,
            //            Max(Case IsNull(AG1.LedgerAccountGroupId, '') When '' Then IsNull(AG.LedgerAccountGroupNature, '') Else IsNull(AG1.LedgerAccountGroupNature, '') End)   
            //            As GroupNature
            //            FROM Web.Ledgers LG
            //            LEFT JOIN Web.LedgerHeaders H On Lg.LedgerHeaderId = H.LedgerHeaderId
            //            LEFT Join Web.LedgerAccounts SG On LG.LedgerAccountId = SG.LedgerAccountId
            //            LEFT JOIN Web.LedgerAccountGroups AG On AG.LedgerAccountGroupId = SG.LedgerAccountGroupId
            //            --LEFT JOIN AcGroupPath AGP On AGP.LedgerAccountGroupId = AG.LedgerAccountGroupId
            //            --And AGP.SNo = &IntLevel &
            //            LEFT  JOIN Web.LedgerAccountGroups AG1 On AG1.LedgerAccountGroupId = AG.ParentLedgerAccountGroupId
            //            " + StrCondition1 + @"
            //            AND AG.LedgerAccountGroupNature In ('Asset', 'Liability') 
            //            GROUP By (Case IsNull(AG1.LedgerAccountGroupId, '') When '' Then IsNull(AG.LedgerAccountGroupId, '')
            //            ELSE IsNull(AG1.LedgerAccountGroupId, '') End) 
            //            HAVING(IsNull(Sum(LG.AmtDr), 0) - IsNull(Sum(LG.AmtCr), 0)) <> 0
            //            ORDER By Max(Case IsNull(AG1.LedgerAccountGroupName, '') When '' Then IsNull(AG.LedgerAccountGroupName, '')
            //            ELSE IsNull(AG1.LedgerAccountGroupName, '') End) ";


            DataSet DsTemp = new DataSet();
            string ConnectionString = (string)System.Web.HttpContext.Current.Session["DefaultConnectionString"];

            using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();
                SqlCommand cmd = new SqlCommand(mQry);
                SqlDataAdapter Da = new SqlDataAdapter(mQry, ConnectionString);
                Da.SelectCommand.Parameters.AddWithValue("@Site", !string.IsNullOrEmpty(SiteId) ? SiteId : (object)DBNull.Value);
                Da.SelectCommand.Parameters.AddWithValue("@Division", !string.IsNullOrEmpty(DivisionId) ? DivisionId : (object)DBNull.Value);
                Da.SelectCommand.Parameters.AddWithValue("@FromDate", FromDate);
                Da.SelectCommand.Parameters.AddWithValue("@ToDate", ToDate);
                Da.SelectCommand.Parameters.AddWithValue("@CostCenter", !string.IsNullOrEmpty(CostCenterId) ? CostCenterId : (object)DBNull.Value);
                Da.SelectCommand.Parameters.AddWithValue("@LedgerAccountGroup", !string.IsNullOrEmpty(LedgerAccountGroup) ? LedgerAccountGroup : (object)DBNull.Value);
                Da.Fill(DsTemp);
            }

            DTTemp = DsTemp.Tables[0];

            FGMain.Columns.Add("GRCodeCredit");
            FGMain.Columns.Add("GRNameCredit");
            FGMain.Columns.Add("Credit");
            FGMain.Columns.Add("GRCode");
            FGMain.Columns.Add("GRName");
            FGMain.Columns.Add("Debit");




            DblDebit_Total = 0;
            DblCredit_Total = 0;

            for (I = 0; I <= DTTemp.Rows.Count - 1; I++)
            {
                //FGMain.Rows.Add();
                if (Convert.ToDecimal(DTTemp.Rows[I]["AmtDr"]) > 0)
                {
                    J = FFindEmptyRow(ref FGMain, "GRCodeCredit");
                    FGMain.Rows[J]["GRCodeCredit"] = DTTemp.Rows[I]["LedgerAccountGroupId"];
                    if (DTTemp.Rows[I]["GroupNature"].ToString() == "Asset")
                        FGMain.Rows[J]["GRNameCredit"] = DTTemp.Rows[I]["GName"];
                    else
                        FGMain.Rows[J]["GRNameCredit"] = DTTemp.Rows[I]["ContraGroupName"];
                    FGMain.Rows[J]["Credit"] = DTTemp.Rows[I]["AmtDr"];
                    DblCredit_Total = DblCredit_Total + Convert.ToDecimal(FGMain.Rows[J]["Credit"]);
                }
                else if (Convert.ToDecimal(DTTemp.Rows[I]["AmtCr"]) > 0)
                {
                    J = FFindEmptyRow(ref FGMain, "GRCode");
                    FGMain.Rows[J]["GRCode"] = DTTemp.Rows[I]["LedgerAccountGroupId"];
                    if (DTTemp.Rows[I]["GroupNature"].ToString() == "Asset")
                        FGMain.Rows[J]["GRName"] = DTTemp.Rows[I]["GName"];
                    else
                        FGMain.Rows[J]["GRName"] = DTTemp.Rows[I]["ContraGroupName"];
                    FGMain.Rows[J]["Debit"] = DTTemp.Rows[I]["AmtCr"];
                    DblDebit_Total = DblDebit_Total + Convert.ToDecimal(FGMain.Rows[J]["Debit"]);
                }
                //FGMain.Rows[J]["GGR_SG"] = "Asset";
            }

            DTTemp.Clear();
            DTTemp.Dispose();

            Decimal DHSMain_DblClosingStock = Convert.ToDecimal(FillData(@"SELECT IsNull(Sum(L.ClosingStockValue),0) AS ClosingStockValue
                                    FROM Web.BusinessSessionLines L
                                    LEFT JOIN Web.BusinessSessions H ON L.BusinessSessionId = H.BusinessSessionId
                                    WHERE '" + ToDate + @"' BETWEEN H.FromDate AND H.UptoDate " +
                        (SiteId != null ? " AND L.SiteId IN (SELECT Items FROM[dbo].[Split]('" + SiteId + @"', ','))" : "")).Tables[0].Rows[0]["ClosingStockValue"]);
            if (DHSMain_DblClosingStock > 0)
            {
                J = FFindEmptyRow(ref FGMain, "GRNameCredit");
                FGMain.Rows[J]["GRNameCredit"] = "Closing Stock";
                FGMain.Rows[J]["Credit"] = DHSMain_DblClosingStock;
                DblCredit_Total = DblCredit_Total + DHSMain_DblClosingStock;
            }

            DTTemp = FGetTRDDataTable("", Settings);

            for (I = 0; I <= DTTemp.Rows.Count - 1; I++)
            {
                if (Convert.ToDecimal(DTTemp.Rows[I]["AmtDr"]) > 0)
                    DblNet_Profit_Loss = DblNet_Profit_Loss - Convert.ToDecimal(DTTemp.Rows[I]["AmtDr"]);
                else if (Convert.ToDecimal(DTTemp.Rows[I]["AmtCr"]) > 0)
                    DblNet_Profit_Loss = DblNet_Profit_Loss + Convert.ToDecimal(DTTemp.Rows[I]["AmtCr"]);

            }
            DTTemp.Clear();
            DTTemp.Dispose();

            DTTemp = FGetTRDDataTable("Not", Settings);

            if (DHSMain_DblClosingStock > 0)
                DblNet_Profit_Loss = DblNet_Profit_Loss + DHSMain_DblClosingStock;

            for (I = 0; I <= DTTemp.Rows.Count - 1; I++)
            {
                if (Convert.ToDecimal(DTTemp.Rows[I]["AmtDr"]) > 0)
                    DblNet_Profit_Loss = DblNet_Profit_Loss - Convert.ToDecimal(DTTemp.Rows[I]["AmtDr"]);
                else if (Convert.ToDecimal(DTTemp.Rows[I]["AmtCr"]) > 0)
                    DblNet_Profit_Loss = DblNet_Profit_Loss + Convert.ToDecimal(DTTemp.Rows[I]["AmtCr"]);

            }
            DTTemp.Clear();
            DTTemp.Dispose();

            if (DblNet_Profit_Loss < 0)
            {
                J = FFindEmptyRow(ref FGMain, "GRNameCredit");
                if (J < FFindEmptyRow(ref FGMain, "GRName"))
                    J = FFindEmptyRow(ref FGMain, "GRName");
                FGMain.Rows[J]["GRNameCredit"] = "Net Loss";
                FGMain.Rows[J]["Credit"] = Math.Abs(DblNet_Profit_Loss);
                DblCredit_Total = DblCredit_Total + Math.Abs(DblNet_Profit_Loss);
            }
            else if (DblNet_Profit_Loss > 0)
            {
                J = FFindEmptyRow(ref FGMain, "GRName");
                if (J < FFindEmptyRow(ref FGMain, "GRNameCredit"))
                    J = FFindEmptyRow(ref FGMain, "GRNameCredit");
                FGMain.Rows[J]["GRName"] = "Net Profit";
                FGMain.Rows[J]["Debit"] = Math.Abs(DblNet_Profit_Loss);
                DblDebit_Total = DblDebit_Total + Math.Abs(DblNet_Profit_Loss);
            }


            if (DblDebit_Total - DblCredit_Total > Convert.ToDecimal(0.001))
            {
                J = FFindEmptyRow(ref FGMain, "GRNameCredit");
                FGMain.Rows[J]["GRNameCredit"] = "Difference In Trial Balance";
                FGMain.Rows[J]["Credit"] = DblDebit_Total - DblCredit_Total;
                DblCredit_Total = DblCredit_Total + (DblDebit_Total - DblCredit_Total);
            }
            else if (DblCredit_Total - DblDebit_Total > Convert.ToDecimal(0.001))
            {
                J = FFindEmptyRow(ref FGMain, "GRName");
                FGMain.Rows[J]["GRName"] = "Difference In Trial Balance";
                FGMain.Rows[J]["Debit"] = DblCredit_Total - DblDebit_Total;
                DblDebit_Total = DblDebit_Total + (DblCredit_Total - DblDebit_Total);
            }

            //FGMain.Rows.Add();
            //FGMain.Rows[FGMain.Rows.Count - 1]["Debit"] = DblDebit_Total;
            //FGMain.Rows[FGMain.Rows.Count - 1]["Credit"] = DblCredit_Total;


            for (I = 0; I <= FGMain.Rows.Count - 1; I++)
            {
                BalanceSheetViewModel vm = new BalanceSheetViewModel();
                if (FGMain.Rows[I]["GRCode"] != null && FGMain.Rows[I]["GRCode"] != System.DBNull.Value)
                    vm.GRCode = Convert.ToInt32(FGMain.Rows[I]["GRCode"]);
                if (FGMain.Rows[I]["GRName"] != null && FGMain.Rows[I]["GRName"] != System.DBNull.Value)
                    vm.GRName = "<Strong>" + Convert.ToString(FGMain.Rows[I]["GRName"]) + "</Strong>";
                if (FGMain.Rows[I]["Debit"] != null && FGMain.Rows[I]["Debit"] != System.DBNull.Value)
                    vm.Debit = Convert.ToDecimal(FGMain.Rows[I]["Debit"]);
                if (FGMain.Rows[I]["Credit"] != null && FGMain.Rows[I]["Credit"] != System.DBNull.Value)
                    vm.Credit = Convert.ToDecimal(FGMain.Rows[I]["Credit"]);
                if (FGMain.Rows[I]["GRCodeCredit"] != null && FGMain.Rows[I]["GRCodeCredit"] != System.DBNull.Value)
                    vm.GRCodeCredit = Convert.ToString(FGMain.Rows[I]["GRCodeCredit"]);
                if (FGMain.Rows[I]["GRNameCredit"] != null && FGMain.Rows[I]["GRNameCredit"] != System.DBNull.Value)
                    vm.GRNameCredit = "<Strong>" + Convert.ToString(FGMain.Rows[I]["GRNameCredit"]) + "</Strong>";

                vm.TotalDebit = DblDebit_Total;
                vm.TotalCredit = DblCredit_Total;

                vm.ReportType = "Balance Sheet";
                vm.OpenReportType = "Trial Balance";

                BalanceSheetViewModelList.Add(vm);
            }

            return BalanceSheetViewModelList;
        }



        public DataSet FillData(string Qry)
        {
            DataSet DsTemp = new DataSet();
            string ConnectionString = (string)System.Web.HttpContext.Current.Session["DefaultConnectionString"];

            using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();
                SqlCommand cmd = new SqlCommand(Qry);
                SqlDataAdapter Da = new SqlDataAdapter(Qry, ConnectionString);
                Da.Fill(DsTemp);
            }

            return DsTemp;
        }

        private int FFindEmptyRow(ref DataTable DtMain, string ColName, int IntFindFrom = 0)
        {
            int I;
            bool BlnFlag = true;

            for (I = IntFindFrom; I <= DtMain.Rows.Count - 1; I++)
            {
                if (Convert.ToString(DtMain.Rows[I][ColName]) == "" || DtMain.Rows[I][ColName] == null)
                {
                    BlnFlag = false;
                    break;
                }
            }

            if (BlnFlag == true)
            {
                DtMain.Rows.Add();
                I = DtMain.Rows.Count - 1;
            }

            return I;
        }

        private DataTable FGetTRDDataTable(string ConditionVar, FinancialDisplaySettings Settings)
        {
            string StrCondition1 = "";
            DataTable DTTemp;
            string mQry = "";


            var SiteSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "Site" select H).FirstOrDefault();
            var DivisionSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "Division" select H).FirstOrDefault();
            var FromDateSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "FromDate" select H).FirstOrDefault();
            var ToDateSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "ToDate" select H).FirstOrDefault();
            var CostCenterSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "CostCenter" select H).FirstOrDefault();
            var IsIncludeZeroBalanceSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "IsIncludeZeroBalance" select H).FirstOrDefault();
            var IsIncludeOpeningSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "IsIncludeOpening" select H).FirstOrDefault();
            var LedgerAccountGroupSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "LedgerAccountGroup" select H).FirstOrDefault();

            string SiteId = SiteSetting.Value;
            string DivisionId = DivisionSetting.Value;
            string FromDate = FromDateSetting.Value;
            string ToDate = ToDateSetting.Value;
            string CostCenterId = CostCenterSetting.Value;
            string IsIncludeZeroBalance = IsIncludeZeroBalanceSetting.Value;
            string IsIncludeOpening = IsIncludeOpeningSetting.Value;
            string LedgerAccountGroup = LedgerAccountGroupSetting.Value;


            StrCondition1 = " Where H.DocDate <= '" + ToDate + "' ";
            if (SiteId != null)
                StrCondition1 = " And H.SiteId In ('" + SiteId + "') ";
            if (DivisionId != null)
                StrCondition1 = " And H.DivisionId In ('" + DivisionId + "') ";
            StrCondition1 += " And H.DocDate >= (Case When Ag.LedgerAccountGroupNature in ('Income','Expense') Then '" + FromDate + "' Else '1900/Jan/01' End) ";

            mQry = GetQryForTrialBalance(SiteId, DivisionId, FromDate, ToDate, CostCenterId, IsIncludeZeroBalance, IsIncludeOpening, LedgerAccountGroup) +
                    @"SELECT H.BaseLedgerAccountGroupId AS LedgerAccountGroupId, Max(BaseLedgerAccountGroupName) AS GName, 
                        CASE WHEN Sum(isnull(H.Balance,0)) > 0 THEN Sum(isnull(H.Balance,0)) ELSE 0 END AS AmtDr,
                        CASE WHEN Sum(isnull(H.Balance,0)) < 0 THEN abs(Sum(isnull(H.Balance,0))) ELSE 0 END AS AmtCr,
                        Max(BaseLedgerAccountGroupName) AS ContraGroupName,
                        Max(Ag.LedgerAccountGroupNature) As GroupNature
                        FROM cteAcGroup H 
                        LEFT JOIN Web.LedgerAccountGroups Ag On AG.LedgerAccountGroupId = H.BaseLedgerAccountGroupId
                        Where AG.LedgerAccountGroupNature In ('Income','Expense') 
                        And AG.PLNature " + ConditionVar + @" In ('Direct','Purchase','Sale') 
                        GROUP BY H.BaseLedgerAccountGroupId 
                        Having (Sum(isnull(H.Opening,0)) <> 0 Or Sum(isnull(H.AmtDr,0)) <> 0 Or Sum(isnull(H.AmtCr,0)) <> 0 Or Sum(isnull(H.Balance,0)) <> 0) ";


            //mQry = @" Select (Case IsNull(AG1.LedgerAccountGroupId,'') When '' Then IsNull(AG.LedgerAccountGroupId,'') 
            //            Else IsNull(AG1.LedgerAccountGroupId,'') End)  As LedgerAccountGroupId, 
            //            Max(Case IsNull(AG1.LedgerAccountGroupName,'') When '' Then IsNull(AG.LedgerAccountGroupName,'') 
            //            Else IsNull(AG1.LedgerAccountGroupName,'') End)  As GName, 
            //            (Case When (IsNull(Sum(LG.AmtDr),0)-IsNull(Sum(LG.AmtCr),0))>0 Then  
            //            (IsNull(Sum(LG.AmtDr),0)-IsNull(Sum(LG.AmtCr),0)) Else 0 End) As AmtDr, 
            //            (Case When (IsNull(Sum(LG.AmtCr),0)-IsNull(Sum(LG.AmtDr),0))>0 Then 
            //            (IsNull(Sum(LG.AmtCr),0)-IsNull(Sum(LG.AmtDr),0)) Else 0 End) As AmtCr, 
            //            Max(AG.LedgerAccountGroupName) As ContraLedgerAccountGroupName,Max(AG.LedgerAccountGroupNature) As GroupNature 
            //            From Web.Ledgers  LG 
            //            LEFT JOIN Web.LedgerHeaders H On Lg.LedgerHeaderId = H.LedgerHeaderId
            //            Left Join Web.LedgerAccounts SG On LG.LedgerAccountId = SG.LedgerAccountId
            //            Left Join Web.LedgerAccountGroups AG On AG.LedgerAccountGroupId = SG.LedgerAccountGroupId
            //            --Left Join AcGroupPath AGP On AGP.LedgerAccountGroupId=AG.LedgerAccountGroupId And AGP.SNo= & IntLevel &  
            //            LEFT  JOIN Web.LedgerAccountGroups AG1 On AG1.LedgerAccountGroupId = AG.ParentLedgerAccountGroupId
            //            " + StrCondition1 + @"
            //            And AG.LedgerAccountGroupNature In ('Income','Expense') 
            //            And (AG.LedgerAccountNature " + ConditionVar + @" In ('Direct','Purchase','Sales') Or AG1.LedgerAccountNature " + ConditionVar + @" In ('Direct','Purchase','Sales')) 
            //            Group By (Case IsNull(AG1.LedgerAccountGroupId,'') When '' Then IsNull(AG.LedgerAccountGroupId,'') 
            //            Else IsNull(AG1.LedgerAccountGroupId,'') End) 
            //            Having (IsNull(Sum(LG.AmtDr),0)-IsNull(Sum(LG.AmtCr),0)) <> 0 
            //            Order By Max(Case IsNull(AG1.LedgerAccountGroupName,'') When '' Then IsNull(AG.LedgerAccountGroupName,'') 
            //            Else IsNull(AG1.LedgerAccountGroupName,'') End) ";


            DataSet DsTemp = new DataSet();
            string ConnectionString = (string)System.Web.HttpContext.Current.Session["DefaultConnectionString"];

            using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();
                SqlCommand cmd = new SqlCommand(mQry);
                SqlDataAdapter Da = new SqlDataAdapter(mQry, ConnectionString);
                Da.SelectCommand.Parameters.AddWithValue("@Site", !string.IsNullOrEmpty(SiteId) ? SiteId : (object)DBNull.Value);
                Da.SelectCommand.Parameters.AddWithValue("@Division", !string.IsNullOrEmpty(DivisionId) ? DivisionId : (object)DBNull.Value);
                Da.SelectCommand.Parameters.AddWithValue("@FromDate", FromDate);
                Da.SelectCommand.Parameters.AddWithValue("@ToDate", ToDate);
                Da.SelectCommand.Parameters.AddWithValue("@CostCenter", !string.IsNullOrEmpty(CostCenterId) ? CostCenterId : (object)DBNull.Value);
                Da.SelectCommand.Parameters.AddWithValue("@LedgerAccountGroup", !string.IsNullOrEmpty(LedgerAccountGroup) ? LedgerAccountGroup : (object)DBNull.Value);
                Da.Fill(DsTemp);
            }

            DTTemp = DsTemp.Tables[0];

            return DTTemp;
        }

        public IEnumerable<ProfitAndLossViewModel> GetProfitAndLoss(FinancialDisplaySettings Settings)
        {
            DataTable DTTemp;
            Decimal DblDebit_Total = 0;
            Decimal DblCredit_Total = 0;
            Decimal DblGrossProfit = 0;
            Decimal DblNetProfit = 0;
            int I, J, IntFindRowFrom;
            string mQry = "";
            DataTable FGMain = new DataTable();
            List<ProfitAndLossViewModel> ProfitAndLossViewModelList = new List<ProfitAndLossViewModel>();

            var SiteSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "Site" select H).FirstOrDefault();
            var DivisionSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "Division" select H).FirstOrDefault();
            var FromDateSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "FromDate" select H).FirstOrDefault();
            var ToDateSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "ToDate" select H).FirstOrDefault();
            var CostCenterSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "CostCenter" select H).FirstOrDefault();
            var IsIncludeZeroBalanceSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "IsIncludeZeroBalance" select H).FirstOrDefault();
            var IsIncludeOpeningSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "IsIncludeOpening" select H).FirstOrDefault();
            var LedgerAccountGroupSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "LedgerAccountGroup" select H).FirstOrDefault();

            string SiteId = SiteSetting.Value;
            string DivisionId = DivisionSetting.Value;
            string FromDate = FromDateSetting.Value;
            string ToDate = ToDateSetting.Value;
            string CostCenterId = CostCenterSetting.Value;
            string IsIncludeZeroBalance = IsIncludeZeroBalanceSetting.Value;
            string IsIncludeOpening = IsIncludeOpeningSetting.Value;
            string LedgerAccountGroup = LedgerAccountGroupSetting.Value;


            DTTemp = FGetTRDDataTable("", Settings);

            FGMain.Columns.Add("GRCodeCredit");
            FGMain.Columns.Add("GRNameCredit");
            FGMain.Columns.Add("Credit");
            FGMain.Columns.Add("GRCode");
            FGMain.Columns.Add("GRName");
            FGMain.Columns.Add("Debit");

            DblDebit_Total = 0;
            DblCredit_Total = 0;


            //Decimal DHSMain_DblOpeningStock = Convert.ToDecimal(FillData(@"SELECT IsNull(Sum(L.OpeningStockValue),0) AS OpeningStockValue
            //                        FROM Web.BusinessSessionLines L
            //                        LEFT JOIN Web.BusinessSessions H ON L.BusinessSessionId = H.BusinessSessionId
            //                        WHERE '" + FromDate + @"' BETWEEN H.FromDate AND H.UptoDate " + 
            //                        (SiteId != null ? " AND L.SiteId IN (SELECT Items FROM[dbo].[Split]('" + SiteId + @"', ','))" : "")).Tables[0].Rows[0]["OpeningStockValue"]);
            //if (DHSMain_DblOpeningStock > 0)
            //{
            //    J = FFindEmptyRow(ref FGMain, "GRName");
            //    FGMain.Rows[J]["GRName"] = "Opening Stock";
            //    FGMain.Rows[J]["Debit"] = DHSMain_DblOpeningStock;
            //    DblDebit_Total = DblDebit_Total + DHSMain_DblOpeningStock;
            //}

            for (I = 0; I <= DTTemp.Rows.Count - 1; I++)
            {
                if (Convert.ToDecimal(DTTemp.Rows[I]["AmtCr"]) > 0)
                {
                    J = FFindEmptyRow(ref FGMain, "GRNameCredit");
                    FGMain.Rows[J]["GRCodeCredit"] = DTTemp.Rows[I]["LedgerAccountGroupId"];
                    if (DTTemp.Rows[I]["GroupNature"].ToString() == "Income")
                        FGMain.Rows[J]["GRNameCredit"] = DTTemp.Rows[I]["GName"];
                    else
                        FGMain.Rows[J]["GRNameCredit"] = DTTemp.Rows[I]["ContraGroupName"];
                    FGMain.Rows[J]["Credit"] = DTTemp.Rows[I]["AmtCr"];
                    DblCredit_Total = DblCredit_Total + Convert.ToDecimal(DTTemp.Rows[I]["AmtCr"]);
                }
                else if (Convert.ToDecimal(DTTemp.Rows[I]["AmtDr"]) > 0)
                {
                    J = FFindEmptyRow(ref FGMain, "GRName");
                    FGMain.Rows[J]["GRCode"] = DTTemp.Rows[I]["LedgerAccountGroupId"];
                    if (DTTemp.Rows[I]["GroupNature"].ToString() == "Expense")
                        FGMain.Rows[J]["GRName"] = DTTemp.Rows[I]["GName"];
                    else
                        FGMain.Rows[J]["GRName"] = DTTemp.Rows[I]["ContraGroupName"];
                    FGMain.Rows[J]["Debit"] = DTTemp.Rows[I]["AmtDr"];
                    DblDebit_Total = DblDebit_Total + Convert.ToDecimal(DTTemp.Rows[I]["AmtDr"]);
                }
            }

            Decimal DHSMain_DblClosingStock = Convert.ToDecimal(FillData(@"SELECT IsNull(Sum(L.ClosingStockValue),0) AS ClosingStockValue
                                    FROM Web.BusinessSessionLines L
                                    LEFT JOIN Web.BusinessSessions H ON L.BusinessSessionId = H.BusinessSessionId
                                    WHERE '" + ToDate + @"' BETWEEN H.FromDate AND H.UptoDate " +
                                    (SiteId != null ? " AND L.SiteId IN (SELECT Items FROM[dbo].[Split]('" + SiteId + @"', ','))" : "")).Tables[0].Rows[0]["ClosingStockValue"]);
            if (DHSMain_DblClosingStock > 0)
            {
                J = FFindEmptyRow(ref FGMain, "GRNameCredit");
                FGMain.Rows[J]["GRNameCredit"] = "Closing Stock";
                FGMain.Rows[J]["Credit"] = DHSMain_DblClosingStock;
                DblCredit_Total = DblCredit_Total + DHSMain_DblClosingStock;
            }

            DblGrossProfit = (DblDebit_Total - DblCredit_Total);

            if (DblDebit_Total - DblCredit_Total > 0)
            {
                J = FFindEmptyRow(ref FGMain, "GRNameCredit");
                FGMain.Rows[J]["GRNameCredit"] = "Gross Loss";
                FGMain.Rows[J]["Credit"] = DblDebit_Total - DblCredit_Total;
                DblCredit_Total = DblCredit_Total + (DblDebit_Total - DblCredit_Total);
                DblDebit_Total = DblCredit_Total;
            }
            else if (DblCredit_Total - DblDebit_Total > 0)
            {
                J = FFindEmptyRow(ref FGMain, "GRNameCredit");
                FGMain.Rows[J]["GRName"] = "Gross Profit";
                FGMain.Rows[J]["Debit"] = DblCredit_Total - DblDebit_Total;
                DblDebit_Total = DblDebit_Total + (DblCredit_Total - DblDebit_Total);
                DblCredit_Total = DblDebit_Total;
            }


            if (DblDebit_Total > 0)
            {
                FGMain.Rows.Add();
                FGMain.Rows[FGMain.Rows.Count - 1]["GRCode"] = "0";
                FGMain.Rows.Add();
                FGMain.Rows[FGMain.Rows.Count - 1]["Debit"] = DblDebit_Total;
                FGMain.Rows[FGMain.Rows.Count - 1]["Credit"] = DblCredit_Total;



                FGMain.Rows.Add();
            }

            IntFindRowFrom = FGMain.Rows.Count;
            if (DblGrossProfit > 0)
            {
                J = FFindEmptyRow(ref FGMain, "GRName", IntFindRowFrom);
                FGMain.Rows[J]["GRName"] = "Gross Loss";
                FGMain.Rows[J]["Debit"] = Math.Abs(DblGrossProfit);
            }
            else
            {
                J = FFindEmptyRow(ref FGMain, "GRNameCredit", IntFindRowFrom);
                FGMain.Rows[J]["GRNameCredit"] = "Gross Profit";
                FGMain.Rows[J]["Credit"] = Math.Abs(DblGrossProfit);
            }


            DTTemp = FGetTRDDataTable("Not", Settings);

            DblDebit_Total = 0;
            DblCredit_Total = 0;

            for (I = 0; I <= DTTemp.Rows.Count - 1; I++)
            {
                if (Convert.ToDecimal(DTTemp.Rows[I]["AmtCr"]) > 0)
                {
                    J = FFindEmptyRow(ref FGMain, "GRNameCredit", IntFindRowFrom);
                    FGMain.Rows[J]["GRCodeCredit"] = DTTemp.Rows[I]["LedgerAccountGroupId"];
                    if (DTTemp.Rows[I]["GroupNature"].ToString() == "Income")
                        FGMain.Rows[J]["GRNameCredit"] = DTTemp.Rows[I]["GName"];
                    else
                        FGMain.Rows[J]["GRNameCredit"] = DTTemp.Rows[I]["ContraGroupName"];
                    FGMain.Rows[J]["Credit"] = DTTemp.Rows[I]["AmtCr"];
                    DblCredit_Total = DblCredit_Total + Convert.ToDecimal(DTTemp.Rows[I]["AmtCr"]);
                }
                else if (Convert.ToDecimal(DTTemp.Rows[I]["AmtDr"]) > 0)
                {
                    J = FFindEmptyRow(ref FGMain, "GRName", IntFindRowFrom);
                    FGMain.Rows[J]["GRCode"] = DTTemp.Rows[I]["LedgerAccountGroupId"];
                    if (DTTemp.Rows[I]["GroupNature"].ToString() == "Expense")
                        FGMain.Rows[J]["GRName"] = DTTemp.Rows[I]["GName"];
                    else
                        FGMain.Rows[J]["GRName"] = DTTemp.Rows[I]["ContraGroupName"];
                    FGMain.Rows[J]["Debit"] = DTTemp.Rows[I]["AmtDr"];
                    DblDebit_Total = DblDebit_Total + Convert.ToDecimal(DTTemp.Rows[I]["AmtDr"]);
                }
            }

            DblNetProfit = DblGrossProfit + (DblDebit_Total - DblCredit_Total);
            if (DblNetProfit > 0)
            {
                J = FFindEmptyRow(ref FGMain, "GRNameCredit", IntFindRowFrom);
                FGMain.Rows[J]["GRNameCredit"] = "Net Loss";
                FGMain.Rows[J]["Credit"] = Math.Abs(DblNetProfit);
                DblCredit_Total = DblCredit_Total + Math.Abs(DblNetProfit);
                DblDebit_Total = DblCredit_Total;
            }
            else if (DblNetProfit < 0)
            {
                J = FFindEmptyRow(ref FGMain, "GRName", IntFindRowFrom);
                FGMain.Rows[J]["GRName"] = "Net Profit";
                FGMain.Rows[J]["Debit"] = Math.Abs(DblNetProfit);
                DblDebit_Total = DblDebit_Total + Math.Abs(DblNetProfit);
                DblCredit_Total = DblDebit_Total;
            }



            for (I = 0; I <= FGMain.Rows.Count - 1; I++)
            {
                ProfitAndLossViewModel vm = new ProfitAndLossViewModel();
                if (FGMain.Rows[I]["GRCode"] != null && FGMain.Rows[I]["GRCode"] != System.DBNull.Value)
                    vm.GRCode = Convert.ToInt32(FGMain.Rows[I]["GRCode"]);
                if (FGMain.Rows[I]["GRName"] != null && FGMain.Rows[I]["GRName"] != System.DBNull.Value)
                    vm.GRName = "<Strong>" + Convert.ToString(FGMain.Rows[I]["GRName"]) + "</Strong>";
                if (FGMain.Rows[I]["Debit"] != null && FGMain.Rows[I]["Debit"] != System.DBNull.Value)
                    vm.Debit = Convert.ToDecimal(FGMain.Rows[I]["Debit"]);
                if (FGMain.Rows[I]["Credit"] != null && FGMain.Rows[I]["Credit"] != System.DBNull.Value)
                    vm.Credit = Convert.ToDecimal(FGMain.Rows[I]["Credit"]);
                if (FGMain.Rows[I]["GRCodeCredit"] != null && FGMain.Rows[I]["GRCodeCredit"] != System.DBNull.Value)
                    vm.GRCodeCredit = Convert.ToString(FGMain.Rows[I]["GRCodeCredit"]);
                if (FGMain.Rows[I]["GRNameCredit"] != null && FGMain.Rows[I]["GRNameCredit"] != System.DBNull.Value)
                    vm.GRNameCredit = "<Strong>" + Convert.ToString(FGMain.Rows[I]["GRNameCredit"]) + "</Strong>";

                vm.TotalDebit = DblDebit_Total;
                vm.TotalCredit = DblCredit_Total;

                vm.ReportType = "Profit And Loss";
                vm.OpenReportType = "Trial Balance";

                ProfitAndLossViewModelList.Add(vm);
            }

            return ProfitAndLossViewModelList;
        }




        public void Dispose()
        {
        }

        public IEnumerable<BalanceSheetViewModel> GetBalanceSheetDetail(FinancialDisplaySettings Settings)
        {
            string StrCondition1 = "";
            DataTable DtBalanceSheetSummary = new DataTable();
            Decimal DblDebit_Total = 0;
            Decimal DblCredit_Total = 0;
            int I = 0, J = 0;
            Decimal DblNet_Profit_Loss = 0;
            string mQry = "";
            DataTable FGMain = new DataTable();
            List<BalanceSheetViewModel> BalanceSheetViewModelList = new List<BalanceSheetViewModel>();

            var SiteSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "Site" select H).FirstOrDefault();
            var DivisionSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "Division" select H).FirstOrDefault();
            var FromDateSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "FromDate" select H).FirstOrDefault();
            var ToDateSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "ToDate" select H).FirstOrDefault();
            var CostCenterSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "CostCenter" select H).FirstOrDefault();
            var IsIncludeZeroBalanceSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "IsIncludeZeroBalance" select H).FirstOrDefault();
            var IsIncludeOpeningSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "IsIncludeOpening" select H).FirstOrDefault();
            var LedgerAccountGroupSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "LedgerAccountGroup" select H).FirstOrDefault();

            string SiteId = SiteSetting.Value;
            string DivisionId = DivisionSetting.Value;
            string FromDate = FromDateSetting.Value;
            string ToDate = ToDateSetting.Value;
            string CostCenterId = CostCenterSetting.Value;
            string IsIncludeZeroBalance = IsIncludeZeroBalanceSetting.Value;
            string IsIncludeOpening = IsIncludeOpeningSetting.Value;
            string LedgerAccountGroup = LedgerAccountGroupSetting.Value;


            StrCondition1 = " Where H.DocDate <= '" + ToDate + "' ";
            if (SiteId != null)
                StrCondition1 = " And H.SiteId In ('" + SiteId + "') ";
            if (DivisionId != null)
                StrCondition1 = " And H.DivisionId In ('" + DivisionId + "') ";

            mQry = GetQryForTrialBalance(SiteId, DivisionId, FromDate, ToDate, CostCenterId, IsIncludeZeroBalance, IsIncludeOpening, LedgerAccountGroup) +
                    @"SELECT H.BaseLedgerAccountGroupId AS LedgerAccountGroupId, Max(BaseLedgerAccountGroupName) AS GName, 
                                CASE WHEN Sum(isnull(H.Balance,0)) > 0 THEN Sum(isnull(H.Balance,0)) ELSE 0 END AS AmtDr,
                                CASE WHEN Sum(isnull(H.Balance,0)) < 0 THEN abs(Sum(isnull(H.Balance,0))) ELSE 0 END AS AmtCr,
                                Max(BaseLedgerAccountGroupName) AS ContraGroupName,
                                Max(Ag.LedgerAccountGroupNature) As GroupNature
                                FROM cteAcGroup H 
                                LEFT JOIN Web.LedgerAccountGroups Ag On AG.LedgerAccountGroupId = H.BaseLedgerAccountGroupId
                                Where AG.LedgerAccountGroupNature In ('Asset', 'Liability') 
                                GROUP BY H.BaseLedgerAccountGroupId 
                                Having (Sum(isnull(H.Opening,0)) <> 0 Or Sum(isnull(H.AmtDr,0)) <> 0 Or Sum(isnull(H.AmtCr,0)) <> 0 Or Sum(isnull(H.Balance,0)) <> 0) ";
            DataSet DsBalanceSheetSummary = new DataSet();

            string ConnectionString = (string)System.Web.HttpContext.Current.Session["DefaultConnectionString"];

            using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();
                SqlCommand cmd = new SqlCommand(mQry);
                SqlDataAdapter Da = new SqlDataAdapter(mQry, ConnectionString);
                Da.SelectCommand.Parameters.AddWithValue("@SiteId", !string.IsNullOrEmpty(SiteId) ? SiteId : (object)DBNull.Value);
                Da.SelectCommand.Parameters.AddWithValue("@DivisionId", !string.IsNullOrEmpty(DivisionId) ? DivisionId : (object)DBNull.Value);
                Da.SelectCommand.Parameters.AddWithValue("@FromDate", FromDate);
                Da.SelectCommand.Parameters.AddWithValue("@ToDate", ToDate);
                Da.SelectCommand.Parameters.AddWithValue("@CostCenterId", !string.IsNullOrEmpty(CostCenterId) ? CostCenterId : (object)DBNull.Value);
                Da.SelectCommand.Parameters.AddWithValue("@LedgerAccountGroup", !string.IsNullOrEmpty(LedgerAccountGroup) ? LedgerAccountGroup : (object)DBNull.Value);
                Da.Fill(DsBalanceSheetSummary);
            }

            DtBalanceSheetSummary = DsBalanceSheetSummary.Tables[0];

            DataTable DtDistinctLedgerAccountGroup = DtBalanceSheetSummary.DefaultView.ToTable(true, "LedgerAccountGroupId");
            for (int i = 0; i <= DtDistinctLedgerAccountGroup.Rows.Count - 1; i++)
            {
                if (LedgerAccountGroup == "" || LedgerAccountGroup == null)
                    LedgerAccountGroup = DtDistinctLedgerAccountGroup.Rows[i]["LedgerAccountGroupId"].ToString();
                else
                    LedgerAccountGroup += "," + DtDistinctLedgerAccountGroup.Rows[i]["LedgerAccountGroupId"].ToString();
            }

            FGMain.Columns.Add("GRCodeCredit");
            FGMain.Columns.Add("GRNameCredit");
            FGMain.Columns.Add("Credit");
            FGMain.Columns.Add("GRCode");
            FGMain.Columns.Add("GRName");
            FGMain.Columns.Add("Debit");


            DblDebit_Total = 0;
            DblCredit_Total = 0;

            for (I = 0; I <= DtBalanceSheetSummary.Rows.Count - 1; I++)
            {
                if (Convert.ToDecimal(DtBalanceSheetSummary.Rows[I]["AmtDr"]) > 0)
                {
                    J = FFindEmptyRow(ref FGMain, "GRCodeCredit");
                    FGMain.Rows[J]["GRCodeCredit"] = DtBalanceSheetSummary.Rows[I]["LedgerAccountGroupId"];
                    if (DtBalanceSheetSummary.Rows[I]["GroupNature"].ToString() == "Asset")
                        FGMain.Rows[J]["GRNameCredit"] = DtBalanceSheetSummary.Rows[I]["GName"];
                    else
                        FGMain.Rows[J]["GRNameCredit"] = DtBalanceSheetSummary.Rows[I]["ContraGroupName"];
                    FGMain.Rows[J]["Credit"] = DtBalanceSheetSummary.Rows[I]["AmtDr"];
                    DblCredit_Total = DblCredit_Total + Convert.ToDecimal(FGMain.Rows[J]["Credit"]);
                }
                else if (Convert.ToDecimal(DtBalanceSheetSummary.Rows[I]["AmtCr"]) > 0)
                {
                    J = FFindEmptyRow(ref FGMain, "GRCode");
                    FGMain.Rows[J]["GRCode"] = DtBalanceSheetSummary.Rows[I]["LedgerAccountGroupId"];
                    if (DtBalanceSheetSummary.Rows[I]["GroupNature"].ToString() == "Asset")
                        FGMain.Rows[J]["GRName"] = DtBalanceSheetSummary.Rows[I]["GName"];
                    else
                        FGMain.Rows[J]["GRName"] = DtBalanceSheetSummary.Rows[I]["ContraGroupName"];
                    FGMain.Rows[J]["Debit"] = DtBalanceSheetSummary.Rows[I]["AmtCr"];
                    DblDebit_Total = DblDebit_Total + Convert.ToDecimal(FGMain.Rows[J]["Debit"]);
                }
            }

            DtBalanceSheetSummary.Clear();
            DtBalanceSheetSummary.Dispose();

            Decimal DHSMain_DblClosingStock = Convert.ToDecimal(FillData(@"SELECT IsNull(Sum(L.ClosingStockValue),0) AS ClosingStockValue
                                    FROM Web.BusinessSessionLines L
                                    LEFT JOIN Web.BusinessSessions H ON L.BusinessSessionId = H.BusinessSessionId
                                    WHERE '" + ToDate + @"' BETWEEN H.FromDate AND H.UptoDate " +
                                    (SiteId != null ? " AND L.SiteId IN (SELECT Items FROM[dbo].[Split]('" + SiteId + @"', ','))" : "")).Tables[0].Rows[0]["ClosingStockValue"]);
            if (DHSMain_DblClosingStock > 0)
            {
                J = FFindEmptyRow(ref FGMain, "GRNameCredit");
                FGMain.Rows[J]["GRNameCredit"] = "Closing Stock";
                FGMain.Rows[J]["Credit"] = DHSMain_DblClosingStock;
                DblCredit_Total = DblCredit_Total + DHSMain_DblClosingStock;
            }

            DtBalanceSheetSummary = FGetTRDDataTable("", Settings);

            for (I = 0; I <= DtBalanceSheetSummary.Rows.Count - 1; I++)
            {
                if (Convert.ToDecimal(DtBalanceSheetSummary.Rows[I]["AmtDr"]) > 0)
                    DblNet_Profit_Loss = DblNet_Profit_Loss - Convert.ToDecimal(DtBalanceSheetSummary.Rows[I]["AmtDr"]);
                else if (Convert.ToDecimal(DtBalanceSheetSummary.Rows[I]["AmtCr"]) > 0)
                    DblNet_Profit_Loss = DblNet_Profit_Loss + Convert.ToDecimal(DtBalanceSheetSummary.Rows[I]["AmtCr"]);

            }
            DtBalanceSheetSummary.Clear();
            DtBalanceSheetSummary.Dispose();

            DtBalanceSheetSummary = FGetTRDDataTable("Not", Settings);

            if (DHSMain_DblClosingStock > 0)
                DblNet_Profit_Loss = DblNet_Profit_Loss + DHSMain_DblClosingStock;

            for (I = 0; I <= DtBalanceSheetSummary.Rows.Count - 1; I++)
            {
                if (Convert.ToDecimal(DtBalanceSheetSummary.Rows[I]["AmtDr"]) > 0)
                    DblNet_Profit_Loss = DblNet_Profit_Loss - Convert.ToDecimal(DtBalanceSheetSummary.Rows[I]["AmtDr"]);
                else if (Convert.ToDecimal(DtBalanceSheetSummary.Rows[I]["AmtCr"]) > 0)
                    DblNet_Profit_Loss = DblNet_Profit_Loss + Convert.ToDecimal(DtBalanceSheetSummary.Rows[I]["AmtCr"]);

            }
            DtBalanceSheetSummary.Clear();
            DtBalanceSheetSummary.Dispose();

            if (DblNet_Profit_Loss < 0)
            {
                J = FFindEmptyRow(ref FGMain, "GRNameCredit");
                if (J < FFindEmptyRow(ref FGMain, "GRName"))
                    J = FFindEmptyRow(ref FGMain, "GRName");
                FGMain.Rows[J]["GRNameCredit"] = "Net Loss";
                FGMain.Rows[J]["Credit"] = Math.Abs(DblNet_Profit_Loss);
                DblCredit_Total = DblCredit_Total + Math.Abs(DblNet_Profit_Loss);
            }
            else if (DblNet_Profit_Loss > 0)
            {
                J = FFindEmptyRow(ref FGMain, "GRName");
                if (J < FFindEmptyRow(ref FGMain, "GRNameCredit"))
                    J = FFindEmptyRow(ref FGMain, "GRNameCredit");
                FGMain.Rows[J]["GRName"] = "Net Profit";
                FGMain.Rows[J]["Debit"] = Math.Abs(DblNet_Profit_Loss);
                DblDebit_Total = DblDebit_Total + Math.Abs(DblNet_Profit_Loss);
            }


            if (DblDebit_Total - DblCredit_Total > Convert.ToDecimal(0.001))
            {
                J = FFindEmptyRow(ref FGMain, "GRNameCredit");
                FGMain.Rows[J]["GRNameCredit"] = "Difference In Trial Balance";
                FGMain.Rows[J]["Credit"] = DblDebit_Total - DblCredit_Total;
                DblCredit_Total = DblCredit_Total + (DblDebit_Total - DblCredit_Total);
            }
            else if (DblCredit_Total - DblDebit_Total > Convert.ToDecimal(0.001))
            {
                J = FFindEmptyRow(ref FGMain, "GRName");
                FGMain.Rows[J]["GRName"] = "Difference In Trial Balance";
                FGMain.Rows[J]["Debit"] = DblCredit_Total - DblDebit_Total;
                DblDebit_Total = DblDebit_Total + (DblCredit_Total - DblDebit_Total);
            }

            for (I = 0; I <= FGMain.Rows.Count - 1; I++)
            {
                BalanceSheetViewModel vm = new BalanceSheetViewModel();
                if (FGMain.Rows[I]["GRCode"] != null && FGMain.Rows[I]["GRCode"] != System.DBNull.Value)
                    vm.GRCode = Convert.ToInt32(FGMain.Rows[I]["GRCode"]);
                if (FGMain.Rows[I]["GRName"] != null && FGMain.Rows[I]["GRName"] != System.DBNull.Value)
                    vm.GRName = "<Strong>" + Convert.ToString(FGMain.Rows[I]["GRName"]) + "</Strong>";
                if (FGMain.Rows[I]["Debit"] != null && FGMain.Rows[I]["Debit"] != System.DBNull.Value)
                    vm.Debit = Convert.ToDecimal(FGMain.Rows[I]["Debit"]);
                if (FGMain.Rows[I]["Credit"] != null && FGMain.Rows[I]["Credit"] != System.DBNull.Value)
                    vm.Credit = Convert.ToDecimal(FGMain.Rows[I]["Credit"]);
                if (FGMain.Rows[I]["GRCodeCredit"] != null && FGMain.Rows[I]["GRCodeCredit"] != System.DBNull.Value)
                    vm.GRCodeCredit = Convert.ToString(FGMain.Rows[I]["GRCodeCredit"]);
                if (FGMain.Rows[I]["GRNameCredit"] != null && FGMain.Rows[I]["GRNameCredit"] != System.DBNull.Value)
                    vm.GRNameCredit = "<Strong>" + Convert.ToString(FGMain.Rows[I]["GRNameCredit"]) + "</Strong>";

                vm.TotalDebit = DblDebit_Total;
                vm.TotalCredit = DblCredit_Total;

                vm.ReportType = "Balance Sheet";
                vm.OpenReportType = "Trial Balance";


                vm.RowNumber = I;

                BalanceSheetViewModelList.Add(vm);
            }



            string LedgerAccountGroup_Debit = string.Join<string>(",", BalanceSheetViewModelList.Select(m => m.GRCode.ToString()));
            string LedgerAccountGroup_Credit = string.Join<string>(",", BalanceSheetViewModelList.Select(m => (m.GRCodeCredit ?? "").ToString()));

            SqlParameter SqlParameterSiteId_Child_Debit = new SqlParameter("@Site", !string.IsNullOrEmpty(SiteId) ? SiteId : (object)DBNull.Value);
            SqlParameter SqlParameterDivisionId_Child_Debit = new SqlParameter("@Division", !string.IsNullOrEmpty(DivisionId) ? DivisionId : (object)DBNull.Value);
            SqlParameter SqlParameterFromDate_Child_Debit = new SqlParameter("@FromDate", FromDate);
            SqlParameter SqlParameterToDate_Child_Debit = new SqlParameter("@ToDate", ToDate);
            SqlParameter SqlParameterCostCenter_Child_Debit = new SqlParameter("@CostCenter", !string.IsNullOrEmpty(CostCenterId) ? CostCenterId : (object)DBNull.Value);
            SqlParameter SqlParameterLedgerAccountGroup_Child_Debit = new SqlParameter("@LedgerAccountGroup", !string.IsNullOrEmpty(LedgerAccountGroup_Debit.ToString()) ? LedgerAccountGroup_Debit : (object)DBNull.Value);

            mQry = GetQryForTrialBalance(SiteId, DivisionId, FromDate, ToDate, CostCenterId, IsIncludeZeroBalance, IsIncludeOpening, LedgerAccountGroup) +
                            @"SELECT Max(PAg.LedgerAccountGroupId) As ParentGRCode, H.BaseLedgerAccountGroupId AS GRCode, '&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;' + '<Strong>' +  Max(BaseLedgerAccountGroupName) + '</Strong>' AS GRName, 
                                    -Sum(isnull(H.Balance,0)) AS Debit,
                                    Null As Credit,
                                    NULL AS LedgerAccountId, 'Trial Balance' ReportType, 'Trial Balance' AS OpenReportType, Max(BaseLedgerAccountGroupName) As OrderByColumn,
                                    Max(PAg.LedgerAccountGroupName) AS ParentLedgerAccountGroupName
                                    FROM cteAcGroup H 
                                    LEFT JOIN Web.LedgerAccountGroups Ag ON H.BaseLedgerAccountGroupId = Ag.LedgerAccountGroupId
                                    LEFT JOIN Web.LedgerAccountGroups PAg ON Ag.ParentLedgerAccountGroupId = PAg.LedgerAccountGroupId
                                    GROUP BY H.BaseLedgerAccountGroupId 
                                    Having (Sum(isnull(H.Opening,0)) <> 0 Or Sum(isnull(H.AmtDr,0)) <> 0 Or Sum(isnull(H.AmtCr,0)) <> 0 Or Sum(isnull(H.Balance,0)) <> 0) " +
                            (IsIncludeZeroBalance == "False" ? " And Sum(isnull(H.Balance,0)) <> 0 " : "") +

                            @"UNION ALL 
                
                                    SELECT Ag.LedgerAccountGroupId As ParentGRCode, H.LedgerAccountId AS GRCode, '&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;' + H.LedgerAccountName AS GRName, 
                                    -isnull(H.Balance,0) AS Debit,
                                    Null AS Credit,
                                    H.LedgerAccountId AS LedgerAccountId, 'Trial Balance' ReportType, 'Ledger' AS OpenReportType, H.LedgerAccountName  As OrderByColumn,
                                    Ag.LedgerAccountGroupName AS ParentLedgerAccountGroupName
                                    FROM cteLedgerBalance H 
                                    LEFT JOIN Web.LedgerAccounts A ON H.LedgerAccountId = A.LedgerAccountId 
                                    LEFT JOIN Web.LedgerAccountGroups Ag ON A.LedgerAccountGroupId = Ag.LedgerAccountGroupId 
                                    Where (isnull(H.Opening,0) <> 0 Or isnull(H.AmtDr,0) <> 0 Or isnull(H.AmtCr,0) <> 0 Or isnull(H.Balance,0) <> 0) " +
                            (IsIncludeZeroBalance == "False" ? " And isnull(H.Balance,0) <> 0 " : "") +
                            @" ORDER BY OrderByColumn ";

            List<BalanceSheetViewModel> ChileBalanceSheetList_Debit = db.Database.SqlQuery<BalanceSheetViewModel>(mQry, SqlParameterSiteId_Child_Debit, SqlParameterDivisionId_Child_Debit, SqlParameterFromDate_Child_Debit, SqlParameterToDate_Child_Debit, SqlParameterCostCenter_Child_Debit, SqlParameterLedgerAccountGroup_Child_Debit).ToList();


            SqlParameter SqlParameterSiteId_Child_Credit = new SqlParameter("@Site", !string.IsNullOrEmpty(SiteId) ? SiteId : (object)DBNull.Value);
            SqlParameter SqlParameterDivisionId_Child_Credit = new SqlParameter("@Division", !string.IsNullOrEmpty(DivisionId) ? DivisionId : (object)DBNull.Value);
            SqlParameter SqlParameterFromDate_Child_Credit = new SqlParameter("@FromDate", FromDate);
            SqlParameter SqlParameterToDate_Child_Credit = new SqlParameter("@ToDate", ToDate);
            SqlParameter SqlParameterCostCenter_Child_Credit = new SqlParameter("@CostCenter", !string.IsNullOrEmpty(CostCenterId) ? CostCenterId : (object)DBNull.Value);
            SqlParameter SqlParameterLedgerAccountGroup_Child_Credit = new SqlParameter("@LedgerAccountGroup", !string.IsNullOrEmpty(LedgerAccountGroup_Credit) ? LedgerAccountGroup_Credit : (object)DBNull.Value);

            mQry = GetQryForTrialBalance(SiteId, DivisionId, FromDate, ToDate, CostCenterId, IsIncludeZeroBalance, IsIncludeOpening, LedgerAccountGroup) +
                            @"SELECT Convert(nvarchar,Max(PAg.LedgerAccountGroupId)) As ParentGRCodeCredit, Convert(nvarchar,H.BaseLedgerAccountGroupId) AS GRCodeCredit, '&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;' + '<Strong>' +  Max(BaseLedgerAccountGroupName) + '</Strong>' AS GRNameCredit, 
                                    Null AS Debit,
                                    Sum(isnull(H.Balance,0)) As Credit,
                                    NULL AS LedgerAccountId, 'Trial Balance' ReportType, 'Trial Balance' AS OpenReportType, Max(BaseLedgerAccountGroupName) As OrderByColumn,
                                    Max(PAg.LedgerAccountGroupName) AS ParentLedgerAccountGroupName
                                    FROM cteAcGroup H 
                                    LEFT JOIN Web.LedgerAccountGroups Ag ON H.BaseLedgerAccountGroupId = Ag.LedgerAccountGroupId
                                    LEFT JOIN Web.LedgerAccountGroups PAg ON Ag.ParentLedgerAccountGroupId = PAg.LedgerAccountGroupId
                                    GROUP BY H.BaseLedgerAccountGroupId 
                                    Having (Sum(isnull(H.Opening,0)) <> 0 Or Sum(isnull(H.AmtDr,0)) <> 0 Or Sum(isnull(H.AmtCr,0)) <> 0 Or Sum(isnull(H.Balance,0)) <> 0) " +
                            (IsIncludeZeroBalance == "False" ? " And Sum(isnull(H.Balance,0)) <> 0 " : "") +

                            @"UNION ALL 
                
                                    SELECT Convert(nvarchar,Ag.LedgerAccountGroupId)  As ParentGRCodeCredit, Convert(nvarchar,H.LedgerAccountId) AS GRCodeCredit, '&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;' + H.LedgerAccountName AS GRNameCredit, 
                                    Null AS Debit,
                                    isnull(H.Balance,0) AS Credit,
                                    H.LedgerAccountId AS LedgerAccountId, 'Trial Balance' ReportType, 'Ledger' AS OpenReportType, H.LedgerAccountName  As OrderByColumn,
                                    Ag.LedgerAccountGroupName AS ParentLedgerAccountGroupName
                                    FROM cteLedgerBalance H 
                                    LEFT JOIN Web.LedgerAccounts A ON H.LedgerAccountId = A.LedgerAccountId 
                                    LEFT JOIN Web.LedgerAccountGroups Ag ON A.LedgerAccountGroupId = Ag.LedgerAccountGroupId 
                                    Where (isnull(H.Opening,0) <> 0 Or isnull(H.AmtDr,0) <> 0 Or isnull(H.AmtCr,0) <> 0 Or isnull(H.Balance,0) <> 0) " +
                            (IsIncludeZeroBalance == "False" ? " And isnull(H.Balance,0) <> 0 " : "") +
                            @" ORDER BY OrderByColumn ";

            List<BalanceSheetViewModel> ChileBalanceSheetList_Credit = db.Database.SqlQuery<BalanceSheetViewModel>(mQry, SqlParameterSiteId_Child_Credit, SqlParameterDivisionId_Child_Credit, SqlParameterFromDate_Child_Credit, SqlParameterToDate_Child_Credit, SqlParameterCostCenter_Child_Credit, SqlParameterLedgerAccountGroup_Child_Credit).ToList();


            List<BalanceSheetViewModel> BalanceSheetViewModel_DebitSide = (from H in BalanceSheetViewModelList
                                                                           select new BalanceSheetViewModel
                                                                           {
                                                                               GRCode = H.GRCode,
                                                                               GRName = H.GRName,
                                                                               Debit = H.Debit,
                                                                           }).ToList();

            List<BalanceSheetViewModel> BalanceSheetViewModel_CreditSide = (from H in BalanceSheetViewModelList
                                                                            select new BalanceSheetViewModel
                                                                            {
                                                                                GRCodeCredit = H.GRCodeCredit,
                                                                                GRNameCredit = H.GRNameCredit,
                                                                                Credit = H.Credit,
                                                                            }).ToList();

            for (I = 0; I <= BalanceSheetViewModel_DebitSide.Count - 1; I++)
            {
                if (BalanceSheetViewModel_DebitSide[I].ParentGRCode == null)
                {
                    foreach (var item in ChileBalanceSheetList_Debit)
                    {
                        if (BalanceSheetViewModel_DebitSide[I].GRCode == item.ParentGRCode)
                        {
                            BalanceSheetViewModel Temp = new BalanceSheetViewModel();
                            Temp.GRCode = item.GRCode;
                            Temp.GRName = item.GRName;
                            Temp.Debit = item.Debit;
                            Temp.ParentGRCode = item.ParentGRCode;
                            BalanceSheetViewModel_DebitSide.Insert(I + 1, Temp);
                        }
                    }
                }
            }

            for (I = 0; I <= BalanceSheetViewModel_CreditSide.Count - 1; I++)
            {
                if (BalanceSheetViewModel_CreditSide[I].ParentGRCodeCredit == null)
                {
                    foreach (var item in ChileBalanceSheetList_Credit)
                    {
                        if (BalanceSheetViewModel_CreditSide[I].GRCodeCredit == item.ParentGRCodeCredit)
                        {
                            BalanceSheetViewModel Temp = new BalanceSheetViewModel();
                            Temp.GRCodeCredit = item.GRCodeCredit;
                            Temp.GRNameCredit = item.GRNameCredit;
                            Temp.Credit = item.Credit;
                            Temp.ParentGRCodeCredit = item.ParentGRCodeCredit;
                            BalanceSheetViewModel_CreditSide.Insert(I + 1, Temp);
                        }
                    }
                }
            }


            string StrQry1 = "", StrQry2 = "";
            DataTable DtCombined = new DataTable();
            for (I = 0; I <= BalanceSheetViewModel_DebitSide.Count - 1; I++)
            {
                if (StrQry1 != "")
                    StrQry1 = StrQry1 + " UNION ALL ";

                StrQry1 = StrQry1 + "Select " + I + " As RowNumber, '" + BalanceSheetViewModel_DebitSide[I].GRCode + "' As GRCode, '" + BalanceSheetViewModel_DebitSide[I].GRName + "' As GRName, " + (BalanceSheetViewModel_DebitSide[I].Debit ?? 0) + " As Debit ";
            }

            for (I = 0; I <= BalanceSheetViewModel_CreditSide.Count - 1; I++)
            {
                if (StrQry2 != "")
                    StrQry2 = StrQry2 + " UNION ALL ";

                StrQry2 = StrQry2 + "Select " + I + " As RowNumber, '" + BalanceSheetViewModel_CreditSide[I].GRCodeCredit + "' As GRCodeCredit, '" + BalanceSheetViewModel_CreditSide[I].GRNameCredit + "' As GRNameCredit, " + (BalanceSheetViewModel_CreditSide[I].Credit ?? 0) + " As Credit ";
            }

            mQry = "Select GRCode, GRName, Debit, GRCodeCredit, GRNameCredit, Credit From (" + StrQry1 + ") As V1 FULL JOIN (" + StrQry2 + ") As V2 On V1.RowNumber = V2.RowNumber ";
            using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();
                SqlCommand cmd = new SqlCommand(mQry);
                SqlDataAdapter Da = new SqlDataAdapter(mQry, ConnectionString);
                Da.Fill(DtCombined);
            }


            List<BalanceSheetViewModel> BalanceSheetViewModelList_Combined = new List<BalanceSheetViewModel>();
            for (I = 0; I <= DtCombined.Rows.Count - 1; I++)
            {
                BalanceSheetViewModel vm = new BalanceSheetViewModel();
                if (DtCombined.Rows[I]["GRCode"] != null && DtCombined.Rows[I]["GRCode"] != System.DBNull.Value)
                    vm.GRCode = Convert.ToInt32(DtCombined.Rows[I]["GRCode"]);
                if (DtCombined.Rows[I]["GRName"] != null && DtCombined.Rows[I]["GRName"] != System.DBNull.Value)
                    vm.GRName = Convert.ToString(DtCombined.Rows[I]["GRName"]);
                if (DtCombined.Rows[I]["Debit"] != null && DtCombined.Rows[I]["Debit"] != System.DBNull.Value)
                    if (Convert.ToDecimal(DtCombined.Rows[I]["Debit"]) != 0)
                        vm.Debit = Convert.ToDecimal(DtCombined.Rows[I]["Debit"]);
                if (DtCombined.Rows[I]["Credit"] != null && DtCombined.Rows[I]["Credit"] != System.DBNull.Value)
                    if (Convert.ToDecimal(DtCombined.Rows[I]["Credit"]) != 0)
                        vm.Credit = Convert.ToDecimal(DtCombined.Rows[I]["Credit"]);
                if (DtCombined.Rows[I]["GRCodeCredit"] != null && DtCombined.Rows[I]["GRCodeCredit"] != System.DBNull.Value)
                    vm.GRCodeCredit = Convert.ToString(DtCombined.Rows[I]["GRCodeCredit"]);
                if (DtCombined.Rows[I]["GRNameCredit"] != null && DtCombined.Rows[I]["GRNameCredit"] != System.DBNull.Value)
                    vm.GRNameCredit = Convert.ToString(DtCombined.Rows[I]["GRNameCredit"]);

                vm.TotalDebit = DblDebit_Total;
                vm.TotalCredit = DblCredit_Total;

                vm.ReportType = "Balance Sheet";
                vm.OpenReportType = "Trial Balance";

                BalanceSheetViewModelList_Combined.Add(vm);
            }

            return BalanceSheetViewModelList_Combined;
        }

        public IEnumerable<ProfitAndLossViewModel> GetProfitAndLossDetail(FinancialDisplaySettings Settings)
        {
            DataTable DTTemp;
            Decimal DblDebit_Total = 0;
            Decimal DblCredit_Total = 0;
            Decimal DblGrossProfit = 0;
            Decimal DblNetProfit = 0;
            int I, J, IntFindRowFrom;
            string mQry = "";
            DataTable FGMain = new DataTable();
            List<ProfitAndLossViewModel> ProfitAndLossViewModelList = new List<ProfitAndLossViewModel>();

            var SiteSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "Site" select H).FirstOrDefault();
            var DivisionSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "Division" select H).FirstOrDefault();
            var FromDateSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "FromDate" select H).FirstOrDefault();
            var ToDateSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "ToDate" select H).FirstOrDefault();
            var CostCenterSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "CostCenter" select H).FirstOrDefault();
            var IsIncludeZeroBalanceSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "IsIncludeZeroBalance" select H).FirstOrDefault();
            var IsIncludeOpeningSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "IsIncludeOpening" select H).FirstOrDefault();
            var LedgerAccountGroupSetting = (from H in Settings.FinancialDisplayParameters where H.ParameterName == "LedgerAccountGroup" select H).FirstOrDefault();

            string SiteId = SiteSetting.Value;
            string DivisionId = DivisionSetting.Value;
            string FromDate = FromDateSetting.Value;
            string ToDate = ToDateSetting.Value;
            string CostCenterId = CostCenterSetting.Value;
            string IsIncludeZeroBalance = IsIncludeZeroBalanceSetting.Value;
            string IsIncludeOpening = IsIncludeOpeningSetting.Value;


            DTTemp = FGetTRDDataTable("", Settings);

            FGMain.Columns.Add("GRCodeCredit");
            FGMain.Columns.Add("GRNameCredit");
            FGMain.Columns.Add("Credit");
            FGMain.Columns.Add("GRCode");
            FGMain.Columns.Add("GRName");
            FGMain.Columns.Add("Debit");

            DblDebit_Total = 0;
            DblCredit_Total = 0;

            //Decimal DHSMain_DblOpeningStock = Convert.ToDecimal(FillData(@"SELECT IsNull(Sum(L.OpeningStockValue),0) AS OpeningStockValue
            //                        FROM Web.BusinessSessionLines L
            //                        LEFT JOIN Web.BusinessSessions H ON L.BusinessSessionId = H.BusinessSessionId
            //                        WHERE '" + FromDate + @"' BETWEEN H.FromDate AND H.UptoDate " +
            //                        (SiteId != null ? " AND L.SiteId IN (SELECT Items FROM[dbo].[Split]('" + SiteId + @"', ','))" : "")).Tables[0].Rows[0]["OpeningStockValue"]);
            //if (DHSMain_DblOpeningStock > 0)
            //{
            //    J = FFindEmptyRow(ref FGMain, "GRName");
            //    FGMain.Rows[J]["GRName"] = "Opening Stock";
            //    FGMain.Rows[J]["Debit"] = DHSMain_DblOpeningStock;
            //    DblDebit_Total = DblDebit_Total + DHSMain_DblOpeningStock;
            //}

            for (I = 0; I <= DTTemp.Rows.Count - 1; I++)
            {
                if (Convert.ToDecimal(DTTemp.Rows[I]["AmtCr"]) > 0)
                {
                    J = FFindEmptyRow(ref FGMain, "GRNameCredit");
                    FGMain.Rows[J]["GRCodeCredit"] = DTTemp.Rows[I]["LedgerAccountGroupId"];
                    if (DTTemp.Rows[I]["GroupNature"].ToString() == "Income")
                        FGMain.Rows[J]["GRNameCredit"] = DTTemp.Rows[I]["GName"];
                    else
                        FGMain.Rows[J]["GRNameCredit"] = DTTemp.Rows[I]["ContraGroupName"];
                    FGMain.Rows[J]["Credit"] = DTTemp.Rows[I]["AmtCr"];
                    DblCredit_Total = DblCredit_Total + Convert.ToDecimal(DTTemp.Rows[I]["AmtCr"]);
                }
                else if (Convert.ToDecimal(DTTemp.Rows[I]["AmtDr"]) > 0)
                {
                    J = FFindEmptyRow(ref FGMain, "GRName");
                    FGMain.Rows[J]["GRCode"] = DTTemp.Rows[I]["LedgerAccountGroupId"];
                    if (DTTemp.Rows[I]["GroupNature"].ToString() == "Expense")
                        FGMain.Rows[J]["GRName"] = DTTemp.Rows[I]["GName"];
                    else
                        FGMain.Rows[J]["GRName"] = DTTemp.Rows[I]["ContraGroupName"];
                    FGMain.Rows[J]["Debit"] = DTTemp.Rows[I]["AmtDr"];
                    DblDebit_Total = DblDebit_Total + Convert.ToDecimal(DTTemp.Rows[I]["AmtDr"]);
                }
            }

            Decimal DHSMain_DblClosingStock = Convert.ToDecimal(FillData(@"SELECT IsNull(Sum(L.ClosingStockValue),0) AS ClosingStockValue
                                    FROM Web.BusinessSessionLines L
                                    LEFT JOIN Web.BusinessSessions H ON L.BusinessSessionId = H.BusinessSessionId
                                    WHERE '" + ToDate + @"' BETWEEN H.FromDate AND H.UptoDate " +
                                    (SiteId != null ? " AND L.SiteId IN (SELECT Items FROM[dbo].[Split]('" + SiteId + @"', ','))" : "")).Tables[0].Rows[0]["ClosingStockValue"]);
            if (DHSMain_DblClosingStock > 0)
            {
                J = FFindEmptyRow(ref FGMain, "GRNameCredit");
                FGMain.Rows[J]["GRNameCredit"] = "Closing Stock";
                FGMain.Rows[J]["Credit"] = DHSMain_DblClosingStock;
                DblCredit_Total = DblCredit_Total + DHSMain_DblClosingStock;
            }

            DblGrossProfit = (DblDebit_Total - DblCredit_Total);

            if (DblDebit_Total - DblCredit_Total > 0)
            {
                J = FFindEmptyRow(ref FGMain, "GRNameCredit");
                FGMain.Rows[J]["GRNameCredit"] = "Gross Loss";
                FGMain.Rows[J]["Credit"] = DblDebit_Total - DblCredit_Total;
                DblCredit_Total = DblCredit_Total + (DblDebit_Total - DblCredit_Total);
                DblDebit_Total = DblCredit_Total;
            }
            else if (DblCredit_Total - DblDebit_Total > 0)
            {
                J = FFindEmptyRow(ref FGMain, "GRNameCredit");
                FGMain.Rows[J]["GRName"] = "Gross Profit";
                FGMain.Rows[J]["Debit"] = DblCredit_Total - DblDebit_Total;
                DblDebit_Total = DblDebit_Total + (DblCredit_Total - DblDebit_Total);
                DblCredit_Total = DblDebit_Total;
            }


            if (DblDebit_Total > 0)
            {
                FGMain.Rows.Add();
                FGMain.Rows[FGMain.Rows.Count - 1]["GRCode"] = "0";
                FGMain.Rows.Add();
                FGMain.Rows[FGMain.Rows.Count - 1]["Debit"] = DblDebit_Total;
                FGMain.Rows[FGMain.Rows.Count - 1]["Credit"] = DblCredit_Total;

                //Code For Showing Totals Parellal
                FGMain.Rows[FGMain.Rows.Count - 1]["GRCode"] = "-1";
                FGMain.Rows[FGMain.Rows.Count - 1]["GRCodeCredit"] = "-1";
                FGMain.Rows.Add();
            }

            IntFindRowFrom = FGMain.Rows.Count;
            if (DblGrossProfit > 0)
            {
                J = FFindEmptyRow(ref FGMain, "GRNameCredit", IntFindRowFrom);
                FGMain.Rows[J]["GRName"] = "Gross Loss";
                FGMain.Rows[J]["Debit"] = Math.Abs(DblGrossProfit);
            }
            else
            {
                J = FFindEmptyRow(ref FGMain, "GRNameCredit", IntFindRowFrom);
                FGMain.Rows[J]["GRNameCredit"] = "Gross Profit";
                FGMain.Rows[J]["Credit"] = Math.Abs(DblGrossProfit);
            }


            DTTemp = FGetTRDDataTable("Not", Settings);

            DblDebit_Total = 0;
            DblCredit_Total = 0;

            for (I = 0; I <= DTTemp.Rows.Count - 1; I++)
            {
                if (Convert.ToDecimal(DTTemp.Rows[I]["AmtCr"]) > 0)
                {
                    J = FFindEmptyRow(ref FGMain, "GRNameCredit", IntFindRowFrom);
                    FGMain.Rows[J]["GRCodeCredit"] = DTTemp.Rows[I]["LedgerAccountGroupId"];
                    if (DTTemp.Rows[I]["GroupNature"].ToString() == "Income")
                        FGMain.Rows[J]["GRNameCredit"] = DTTemp.Rows[I]["GName"];
                    else
                        FGMain.Rows[J]["GRNameCredit"] = DTTemp.Rows[I]["ContraGroupName"];
                    FGMain.Rows[J]["Credit"] = DTTemp.Rows[I]["AmtCr"];
                    DblCredit_Total = DblCredit_Total + Convert.ToDecimal(DTTemp.Rows[I]["AmtCr"]);
                }
                else if (Convert.ToDecimal(DTTemp.Rows[I]["AmtDr"]) > 0)
                {
                    J = FFindEmptyRow(ref FGMain, "GRName", IntFindRowFrom);
                    FGMain.Rows[J]["GRCode"] = DTTemp.Rows[I]["LedgerAccountGroupId"];
                    if (DTTemp.Rows[I]["GroupNature"].ToString() == "Expense")
                        FGMain.Rows[J]["GRName"] = DTTemp.Rows[I]["GName"];
                    else
                        FGMain.Rows[J]["GRName"] = DTTemp.Rows[I]["ContraGroupName"];
                    FGMain.Rows[J]["Debit"] = DTTemp.Rows[I]["AmtDr"];
                    DblDebit_Total = DblDebit_Total + Convert.ToDecimal(DTTemp.Rows[I]["AmtDr"]);
                }
            }

            DblNetProfit = DblGrossProfit + (DblDebit_Total - DblCredit_Total);
            if (DblNetProfit > 0)
            {
                J = FFindEmptyRow(ref FGMain, "GRNameCredit", IntFindRowFrom);
                FGMain.Rows[J]["GRNameCredit"] = "Net Loss";
                FGMain.Rows[J]["Credit"] = Math.Abs(DblNetProfit);
                DblCredit_Total = DblCredit_Total + Math.Abs(DblNetProfit);
                DblDebit_Total = DblCredit_Total;
            }
            else if (DblNetProfit < 0)
            {
                J = FFindEmptyRow(ref FGMain, "GRName", IntFindRowFrom);
                FGMain.Rows[J]["GRName"] = "Net Profit";
                FGMain.Rows[J]["Debit"] = Math.Abs(DblNetProfit);
                DblDebit_Total = DblDebit_Total + Math.Abs(DblNetProfit);
                DblCredit_Total = DblDebit_Total;
            }



            for (I = 0; I <= FGMain.Rows.Count - 1; I++)
            {
                ProfitAndLossViewModel vm = new ProfitAndLossViewModel();
                if (FGMain.Rows[I]["GRCode"] != null && FGMain.Rows[I]["GRCode"] != System.DBNull.Value)
                    vm.GRCode = Convert.ToInt32(FGMain.Rows[I]["GRCode"]);
                if (FGMain.Rows[I]["GRName"] != null && FGMain.Rows[I]["GRName"] != System.DBNull.Value)
                    vm.GRName = "<Strong>" + Convert.ToString(FGMain.Rows[I]["GRName"]) + "</Strong>";
                if (FGMain.Rows[I]["Debit"] != null && FGMain.Rows[I]["Debit"] != System.DBNull.Value)
                    vm.Debit = Convert.ToDecimal(FGMain.Rows[I]["Debit"]);
                if (FGMain.Rows[I]["Credit"] != null && FGMain.Rows[I]["Credit"] != System.DBNull.Value)
                    vm.Credit = Convert.ToDecimal(FGMain.Rows[I]["Credit"]);
                if (FGMain.Rows[I]["GRCodeCredit"] != null && FGMain.Rows[I]["GRCodeCredit"] != System.DBNull.Value)
                    vm.GRCodeCredit = Convert.ToString(FGMain.Rows[I]["GRCodeCredit"]);
                if (FGMain.Rows[I]["GRNameCredit"] != null && FGMain.Rows[I]["GRNameCredit"] != System.DBNull.Value)
                    vm.GRNameCredit = "<Strong>" + Convert.ToString(FGMain.Rows[I]["GRNameCredit"]) + "</Strong>";

                vm.TotalDebit = DblDebit_Total;
                vm.TotalCredit = DblCredit_Total;

                vm.ReportType = "Profit And Loss";
                vm.OpenReportType = "Trial Balance";

                ProfitAndLossViewModelList.Add(vm);
            }

            string ConnectionString = (string)System.Web.HttpContext.Current.Session["DefaultConnectionString"];

            string LedgerAccountGroup_Debit = string.Join<string>(",", ProfitAndLossViewModelList.Select(m => m.GRCode.ToString()));
            string LedgerAccountGroup_Credit = string.Join<string>(",", ProfitAndLossViewModelList.Select(m => (m.GRCodeCredit ?? "").ToString()));

            SqlParameter SqlParameterSiteId_Child_Debit = new SqlParameter("@Site", !string.IsNullOrEmpty(SiteId) ? SiteId : (object)DBNull.Value);
            SqlParameter SqlParameterDivisionId_Child_Debit = new SqlParameter("@Division", !string.IsNullOrEmpty(DivisionId) ? DivisionId : (object)DBNull.Value);
            SqlParameter SqlParameterFromDate_Child_Debit = new SqlParameter("@FromDate", FromDate);
            SqlParameter SqlParameterToDate_Child_Debit = new SqlParameter("@ToDate", ToDate);
            SqlParameter SqlParameterCostCenter_Child_Debit = new SqlParameter("@CostCenter", !string.IsNullOrEmpty(CostCenterId) ? CostCenterId : (object)DBNull.Value);
            SqlParameter SqlParameterLedgerAccountGroup_Child_Debit = new SqlParameter("@LedgerAccountGroup", !string.IsNullOrEmpty(LedgerAccountGroup_Debit.ToString()) ? LedgerAccountGroup_Debit : (object)DBNull.Value);

            mQry = GetQryForTrialBalance(SiteId, DivisionId, FromDate, ToDate, CostCenterId, IsIncludeZeroBalance, IsIncludeOpening, LedgerAccountGroup_Debit) +
                            @"SELECT Max(PAg.LedgerAccountGroupId) As ParentGRCode, H.BaseLedgerAccountGroupId AS GRCode, '&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;' + '<Strong>' +  Max(BaseLedgerAccountGroupName) + '</Strong>' AS GRName, 
                                    Sum(isnull(H.Balance,0)) AS Debit,
                                    Null As Credit,
                                    NULL AS LedgerAccountId, 'Trial Balance' ReportType, 'Trial Balance' AS OpenReportType, Max(BaseLedgerAccountGroupName) As OrderByColumn,
                                    Max(PAg.LedgerAccountGroupName) AS ParentLedgerAccountGroupName
                                    FROM cteAcGroup H 
                                    LEFT JOIN Web.LedgerAccountGroups Ag ON H.BaseLedgerAccountGroupId = Ag.LedgerAccountGroupId
                                    LEFT JOIN Web.LedgerAccountGroups PAg ON Ag.ParentLedgerAccountGroupId = PAg.LedgerAccountGroupId
                                    GROUP BY H.BaseLedgerAccountGroupId 
                                    Having (Sum(isnull(H.Opening,0)) <> 0 Or Sum(isnull(H.AmtDr,0)) <> 0 Or Sum(isnull(H.AmtCr,0)) <> 0 Or Sum(isnull(H.Balance,0)) <> 0) " +
                            (IsIncludeZeroBalance == "False" ? " And Sum(isnull(H.Balance,0)) <> 0 " : "") +

                            @"UNION ALL 
                
                                    SELECT Ag.LedgerAccountGroupId As ParentGRCode, H.LedgerAccountId AS GRCode, '&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;' + H.LedgerAccountName AS GRName, 
                                    isnull(H.Balance,0) AS Debit,
                                    Null AS Credit,
                                    H.LedgerAccountId AS LedgerAccountId, 'Trial Balance' ReportType, 'Ledger' AS OpenReportType, H.LedgerAccountName  As OrderByColumn,
                                    Ag.LedgerAccountGroupName AS ParentLedgerAccountGroupName
                                    FROM cteLedgerBalance H 
                                    LEFT JOIN Web.LedgerAccounts A ON H.LedgerAccountId = A.LedgerAccountId 
                                    LEFT JOIN Web.LedgerAccountGroups Ag ON A.LedgerAccountGroupId = Ag.LedgerAccountGroupId 
                                    Where (isnull(H.Opening,0) <> 0 Or isnull(H.AmtDr,0) <> 0 Or isnull(H.AmtCr,0) <> 0 Or isnull(H.Balance,0) <> 0) " +
                            (IsIncludeZeroBalance == "False" ? " And isnull(H.Balance,0) <> 0 " : "") +
                            @" ORDER BY OrderByColumn ";

            List<ProfitAndLossViewModel> ChileProfitAndLossList_Debit = db.Database.SqlQuery<ProfitAndLossViewModel>(mQry, SqlParameterSiteId_Child_Debit, SqlParameterDivisionId_Child_Debit, SqlParameterFromDate_Child_Debit, SqlParameterToDate_Child_Debit, SqlParameterCostCenter_Child_Debit, SqlParameterLedgerAccountGroup_Child_Debit).ToList();


            SqlParameter SqlParameterSiteId_Child_Credit = new SqlParameter("@Site", !string.IsNullOrEmpty(SiteId) ? SiteId : (object)DBNull.Value);
            SqlParameter SqlParameterDivisionId_Child_Credit = new SqlParameter("@Division", !string.IsNullOrEmpty(DivisionId) ? DivisionId : (object)DBNull.Value);
            SqlParameter SqlParameterFromDate_Child_Credit = new SqlParameter("@FromDate", FromDate);
            SqlParameter SqlParameterToDate_Child_Credit = new SqlParameter("@ToDate", ToDate);
            SqlParameter SqlParameterCostCenter_Child_Credit = new SqlParameter("@CostCenter", !string.IsNullOrEmpty(CostCenterId) ? CostCenterId : (object)DBNull.Value);
            SqlParameter SqlParameterLedgerAccountGroup_Child_Credit = new SqlParameter("@LedgerAccountGroup", !string.IsNullOrEmpty(LedgerAccountGroup_Credit) ? LedgerAccountGroup_Credit : (object)DBNull.Value);

            mQry = GetQryForTrialBalance(SiteId, DivisionId, FromDate, ToDate, CostCenterId, IsIncludeZeroBalance, IsIncludeOpening, LedgerAccountGroup_Credit) +
                            @"SELECT Convert(nvarchar,Max(PAg.LedgerAccountGroupId)) As ParentGRCodeCredit, Convert(nvarchar,H.BaseLedgerAccountGroupId) AS GRCodeCredit, '&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;' + '<Strong>' +  Max(BaseLedgerAccountGroupName) + '</Strong>' AS GRNameCredit, 
                                    Null AS Debit,
                                    -Sum(isnull(H.Balance,0)) As Credit,
                                    NULL AS LedgerAccountId, 'Trial Balance' ReportType, 'Trial Balance' AS OpenReportType, Max(BaseLedgerAccountGroupName) As OrderByColumn,
                                    Max(PAg.LedgerAccountGroupName) AS ParentLedgerAccountGroupName
                                    FROM cteAcGroup H 
                                    LEFT JOIN Web.LedgerAccountGroups Ag ON H.BaseLedgerAccountGroupId = Ag.LedgerAccountGroupId
                                    LEFT JOIN Web.LedgerAccountGroups PAg ON Ag.ParentLedgerAccountGroupId = PAg.LedgerAccountGroupId
                                    GROUP BY H.BaseLedgerAccountGroupId 
                                    Having (Sum(isnull(H.Opening,0)) <> 0 Or Sum(isnull(H.AmtDr,0)) <> 0 Or Sum(isnull(H.AmtCr,0)) <> 0 Or Sum(isnull(H.Balance,0)) <> 0) " +
                            (IsIncludeZeroBalance == "False" ? " And Sum(isnull(H.Balance,0)) <> 0 " : "") +

                            @"UNION ALL 
                
                                    SELECT Convert(nvarchar,Ag.LedgerAccountGroupId)  As ParentGRCodeCredit, Convert(nvarchar,H.LedgerAccountId) AS GRCodeCredit, '&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;' + H.LedgerAccountName AS GRNameCredit, 
                                    Null AS Debit,
                                    -isnull(H.Balance,0) AS Credit,
                                    H.LedgerAccountId AS LedgerAccountId, 'Trial Balance' ReportType, 'Ledger' AS OpenReportType, H.LedgerAccountName  As OrderByColumn,
                                    Ag.LedgerAccountGroupName AS ParentLedgerAccountGroupName
                                    FROM cteLedgerBalance H 
                                    LEFT JOIN Web.LedgerAccounts A ON H.LedgerAccountId = A.LedgerAccountId 
                                    LEFT JOIN Web.LedgerAccountGroups Ag ON A.LedgerAccountGroupId = Ag.LedgerAccountGroupId 
                                    Where (isnull(H.Opening,0) <> 0 Or isnull(H.AmtDr,0) <> 0 Or isnull(H.AmtCr,0) <> 0 Or isnull(H.Balance,0) <> 0) " +
                            (IsIncludeZeroBalance == "False" ? " And isnull(H.Balance,0) <> 0 " : "") +
                            @" ORDER BY OrderByColumn ";

            List<ProfitAndLossViewModel> ChildProfitAndLossList_Credit = db.Database.SqlQuery<ProfitAndLossViewModel>(mQry, SqlParameterSiteId_Child_Credit, SqlParameterDivisionId_Child_Credit, SqlParameterFromDate_Child_Credit, SqlParameterToDate_Child_Credit, SqlParameterCostCenter_Child_Credit, SqlParameterLedgerAccountGroup_Child_Credit).ToList();


            List<ProfitAndLossViewModel> ProfitAndLossViewModel_DebitSide = (from H in ProfitAndLossViewModelList
                                                                             select new ProfitAndLossViewModel
                                                                             {
                                                                                 GRCode = H.GRCode,
                                                                                 GRName = H.GRName,
                                                                                 Debit = H.Debit,
                                                                             }).ToList();

            List<ProfitAndLossViewModel> ProfitAndLossViewModel_CreditSide = (from H in ProfitAndLossViewModelList
                                                                              select new ProfitAndLossViewModel
                                                                              {
                                                                                  GRCodeCredit = H.GRCodeCredit,
                                                                                  GRNameCredit = H.GRNameCredit,
                                                                                  Credit = H.Credit,
                                                                              }).ToList();

            for (I = 0; I <= ProfitAndLossViewModel_DebitSide.Count - 1; I++)
            {
                if (ProfitAndLossViewModel_DebitSide[I].ParentGRCode == null)
                {
                    foreach (var item in ChileProfitAndLossList_Debit)
                    {
                        if (ProfitAndLossViewModel_DebitSide[I].GRCode == item.ParentGRCode)
                        {
                            ProfitAndLossViewModel Temp = new ProfitAndLossViewModel();
                            Temp.GRCode = item.GRCode;
                            Temp.GRName = item.GRName;
                            Temp.Debit = item.Debit;
                            Temp.ParentGRCode = item.ParentGRCode;
                            ProfitAndLossViewModel_DebitSide.Insert(I + 1, Temp);
                        }
                    }
                }
            }

            for (I = 0; I <= ProfitAndLossViewModel_CreditSide.Count - 1; I++)
            {
                if (ProfitAndLossViewModel_CreditSide[I].ParentGRCodeCredit == null)
                {
                    foreach (var item in ChildProfitAndLossList_Credit)
                    {
                        if (ProfitAndLossViewModel_CreditSide[I].GRCodeCredit == item.ParentGRCodeCredit)
                        {
                            ProfitAndLossViewModel Temp = new ProfitAndLossViewModel();
                            Temp.GRCodeCredit = item.GRCodeCredit;
                            Temp.GRNameCredit = item.GRNameCredit;
                            Temp.Credit = item.Credit;
                            Temp.ParentGRCodeCredit = item.ParentGRCodeCredit;
                            ProfitAndLossViewModel_CreditSide.Insert(I + 1, Temp);
                        }
                    }
                }
            }

            int MaxLines = 0;
            int DebitSiteLineNum = 0;
            int CreditSiteLineNum = 0;
            string StrQry1 = "", StrQry2 = "";
            int M = 0;
            DataTable DtCombined = new DataTable();
            for (I = 0; I <= ProfitAndLossViewModel_DebitSide.Count - 1; I++)
            {
                if (ProfitAndLossViewModel_DebitSide[I].GRCode == -1)
                {
                    MaxLines = I;
                    DebitSiteLineNum = I;
                    break;
                }

                if (StrQry1 != "")
                    StrQry1 = StrQry1 + " UNION ALL ";

                StrQry1 = StrQry1 + "Select " + I + " As RowNumber, '" + ProfitAndLossViewModel_DebitSide[I].GRCode + "' As GRCode, '" + ProfitAndLossViewModel_DebitSide[I].GRName + "' As GRName, " + (ProfitAndLossViewModel_DebitSide[I].Debit ?? 0) + " As Debit ";
            }

            for (I = 0; I <= ProfitAndLossViewModel_CreditSide.Count - 1; I++)
            {
                if (ProfitAndLossViewModel_CreditSide[I].GRCodeCredit == "-1")
                {
                    if (I > MaxLines)
                    {
                        MaxLines = I;
                        CreditSiteLineNum = I;
                    }
                    break;
                }

                if (StrQry2 != "")
                    StrQry2 = StrQry2 + " UNION ALL ";

                StrQry2 = StrQry2 + "Select " + I + " As RowNumber, '" + ProfitAndLossViewModel_CreditSide[I].GRCodeCredit + "' As GRCodeCredit, '" + ProfitAndLossViewModel_CreditSide[I].GRNameCredit + "' As GRNameCredit, " + (ProfitAndLossViewModel_CreditSide[I].Credit ?? 0) + " As Credit ";
            }

            M = 0;
            for (I = DebitSiteLineNum; I <= ProfitAndLossViewModel_DebitSide.Count - 1; I++)
            {
                if (StrQry1 != "")
                    StrQry1 = StrQry1 + " UNION ALL ";

                StrQry1 = StrQry1 + "Select " + (MaxLines + M) + " As RowNumber, '" + ProfitAndLossViewModel_DebitSide[I].GRCode + "' As GRCode, '" + ProfitAndLossViewModel_DebitSide[I].GRName + "' As GRName, " + (ProfitAndLossViewModel_DebitSide[I].Debit ?? 0) + " As Debit ";
                M = M + 1;
            }


            M = 0;
            for (I = CreditSiteLineNum; I <= ProfitAndLossViewModel_CreditSide.Count - 1; I++)
            {
                if (StrQry2 != "")
                    StrQry2 = StrQry2 + " UNION ALL ";

                StrQry2 = StrQry2 + "Select " + (MaxLines + M) + " As RowNumber, '" + ProfitAndLossViewModel_CreditSide[I].GRCodeCredit + "' As GRCodeCredit, '" + ProfitAndLossViewModel_CreditSide[I].GRNameCredit + "' As GRNameCredit, " + (ProfitAndLossViewModel_CreditSide[I].Credit ?? 0) + " As Credit ";
                M = M + 1;
            }




            mQry = "Select GRCode, GRName, Debit, GRCodeCredit, GRNameCredit, Credit From (" + StrQry1 + ") As V1 FULL JOIN (" + StrQry2 + ") As V2 On V1.RowNumber = V2.RowNumber ";
            using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();
                SqlCommand cmd = new SqlCommand(mQry);
                SqlDataAdapter Da = new SqlDataAdapter(mQry, ConnectionString);
                Da.Fill(DtCombined);
            }


            List<ProfitAndLossViewModel> ProfitAndLossViewModelList_Combined = new List<ProfitAndLossViewModel>();
            for (I = 0; I <= DtCombined.Rows.Count - 1; I++)
            {
                ProfitAndLossViewModel vm = new ProfitAndLossViewModel();
                if (DtCombined.Rows[I]["GRCode"] != null && DtCombined.Rows[I]["GRCode"] != System.DBNull.Value)
                    vm.GRCode = Convert.ToInt32(DtCombined.Rows[I]["GRCode"]);
                if (DtCombined.Rows[I]["GRName"] != null && DtCombined.Rows[I]["GRName"] != System.DBNull.Value)
                    vm.GRName = Convert.ToString(DtCombined.Rows[I]["GRName"]);
                if (DtCombined.Rows[I]["Debit"] != null && DtCombined.Rows[I]["Debit"] != System.DBNull.Value)
                    if (Convert.ToDecimal(DtCombined.Rows[I]["Debit"]) != 0)
                        vm.Debit = Convert.ToDecimal(DtCombined.Rows[I]["Debit"]);
                if (DtCombined.Rows[I]["Credit"] != null && DtCombined.Rows[I]["Credit"] != System.DBNull.Value)
                    if (Convert.ToDecimal(DtCombined.Rows[I]["Credit"]) != 0)
                        vm.Credit = Convert.ToDecimal(DtCombined.Rows[I]["Credit"]);
                if (DtCombined.Rows[I]["GRCodeCredit"] != null && DtCombined.Rows[I]["GRCodeCredit"] != System.DBNull.Value)
                    vm.GRCodeCredit = Convert.ToString(DtCombined.Rows[I]["GRCodeCredit"]);
                if (DtCombined.Rows[I]["GRNameCredit"] != null && DtCombined.Rows[I]["GRNameCredit"] != System.DBNull.Value)
                    vm.GRNameCredit = Convert.ToString(DtCombined.Rows[I]["GRNameCredit"]);

                vm.TotalDebit = DblDebit_Total;
                vm.TotalCredit = DblCredit_Total;

                vm.ReportType = "Profit And Loss";
                vm.OpenReportType = "Trial Balance";

                ProfitAndLossViewModelList_Combined.Add(vm);
            }

            return ProfitAndLossViewModelList_Combined;
        }
    }
}
