using System.Collections.Generic;
using System.Linq;
using Data;
using Data.Infrastructure;
using Model.Models;
using Core.Common;
using System;
using Model;
using System.Threading.Tasks;
using Data.Models;
using Jobs.Constants.Menu;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Data;
using Model.ViewModels;

namespace Service
{
    public interface ISalaryWizardService : IDisposable
    {
        IEnumerable<SalaryWizardResultViewModel> GetSalaryDetail(SalaryWizardViewModel vm);
        IEnumerable<LoanLedgerIdList> GetLoanList(int LedgerAccountId);
    }

    public class SalaryWizardService : ISalaryWizardService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        string mQry = "";
        string connectionString = (string)System.Web.HttpContext.Current.Session["DefaultConnectionString"];

        public SalaryWizardService()
        {
        }

        public IEnumerable<SalaryWizardResultViewModel> GetSalaryDetail(SalaryWizardViewModel vm)
        {
            SqlParameter SqlParameterDocDate = new SqlParameter("@DocDate", vm.DocDate.ToString());
            SqlParameter SqlParameterDocTypeId = new SqlParameter("@DocTypeId", vm.DocTypeId);

            //mQry = @"SELECT E.EmployeeId, P.Name AS EmployeeName, 30.00 AS Days, 0.00 AS Additions, 0.00 AS Deductions, 0.00 AS LoanEMI, 
            //        @DocDate As DocDate, 
            //        @DocTypeId As DocTypeId
            //        FROM Web.Employees E
            //        LEFT JOIN Web.People P ON E.PersonID = P.PersonID  ";


            mQry = @"SELECT E.EmployeeId, P.Name AS EmployeeName, 30.00 AS Days, 0.00 AS Additions, 0.00 AS Deductions, 
                    IsNull(VAdvance.LoanEMI,0) AS LoanEMI, @DocDate As DocDate, 
                    @DocTypeId As DocTypeId
                    FROM Web.Employees E
                    LEFT JOIN Web.People P ON E.PersonID = P.PersonID
                    LEFT JOIN (
                        SELECT L.EmployeeId
                        FROM Web.SalaryLines L 
                        LEFT JOIN Web.SalaryHeaders H ON L.SalaryHeaderId = H.SalaryHeaderId
                        WHERE H.DocDate = @DocDate
                    ) AS VSalaryLine ON E.EmployeeId = VSalaryLine.EmployeeId
                    LEFT JOIN (
	                    SELECT E.EmployeeId, Sum(CASE WHEN IsNull(L.AmtDr,0) - IsNull(VAdj.AdjustedAmount,0) < LL.BaseRate
	                    THEN IsNull(L.AmtDr,0) - IsNull(VAdj.AdjustedAmount,0)
	                    ELSE LL.BaseRate END) AS LoanEMI, Min(L.LedgerId) As LoanLedgerId
	                    FROM Web.Employees E
	                    LEFT JOIN Web.LedgerAccounts A ON E.PersonID = A.PersonId
	                    LEFT JOIN Web.Ledgers L ON A.LedgerAccountId = L.LedgerAccountId
	                    LEFT JOIN Web.LedgerLines LL ON L.LedgerLineId = LL.LedgerLineId
	                    LEFT JOIN Web.LedgerHeaders H ON L.LedgerHeaderId = H.LedgerHeaderId
	                    LEFT JOIN Web.DocumentTypes D ON H.DocTypeId = D.DocumentTypeId
	                    LEFT JOIN (
		                    SELECT La.DrLedgerId, Sum(La.Amount) AS AdjustedAmount
		                    FROM Web.LedgerAdjs La
		                    GROUP BY La.DrLedgerId
	                    ) AS VAdj ON L.LedgerId = VAdj.DrLedgerId
	                    WHERE D.DocumentTypeName = '" + Jobs.Constants.DocumentType.DocumentTypeConstants.LoansAndAdvances.DocumentTypeName + "'" + @"
                        AND IsNull(L.AmtDr,0) - IsNull(VAdj.AdjustedAmount,0) > 0
	                    GROUP BY E.EmployeeId
                    ) AS VAdvance ON E.EmployeeId = VAdvance.EmployeeId
                    WHERE E.DateOfJoining <= @DocDate AND E.DateOfRelieving IS NULL
                    AND VSalaryLine.EmployeeId IS NULL ";

            //mQry = mQry + "UNION ALL " + mQry;
            //mQry = mQry + "UNION ALL " + mQry;

            IEnumerable<SalaryWizardResultViewModel> SalaryWizardResultViewModel = db.Database.SqlQuery<SalaryWizardResultViewModel>(mQry, SqlParameterDocDate, SqlParameterDocTypeId).ToList();

            return SalaryWizardResultViewModel;

        }



        public IEnumerable<LoanLedgerIdList> GetLoanList(int LedgerAccountId)
        {
            SqlParameter SqlParameterLedgerAccountId = new SqlParameter("@LedgerAccountId", LedgerAccountId);

            mQry = @"SELECT L.LedgerId, IsNull(L.AmtDr,0) - IsNull(VAdj.AdjustedAmount,0) As Amount
	                    FROM Web.Ledgers L 
	                    LEFT JOIN Web.LedgerLines LL ON L.LedgerLineId = LL.LedgerLineId
	                    LEFT JOIN Web.LedgerHeaders H ON L.LedgerHeaderId = H.LedgerHeaderId
	                    LEFT JOIN Web.DocumentTypes D ON H.DocTypeId = D.DocumentTypeId
	                    LEFT JOIN (
		                    SELECT La.DrLedgerId, Sum(La.Amount) AS AdjustedAmount
		                    FROM Web.LedgerAdjs La
		                    GROUP BY La.DrLedgerId
	                    ) AS VAdj ON L.LedgerId = VAdj.DrLedgerId
	                    WHERE L.LedgerAccountId = @LedgerAccountId
                        And D.DocumentTypeName = '" + Jobs.Constants.DocumentType.DocumentTypeConstants.LoansAndAdvances.DocumentTypeName + "'" + @"
                        AND IsNull(L.AmtDr,0) - IsNull(VAdj.AdjustedAmount,0) > 0 ";

            IEnumerable<LoanLedgerIdList> SalaryWizardResultViewModel = db.Database.SqlQuery<LoanLedgerIdList>(mQry, SqlParameterLedgerAccountId).ToList();

            return SalaryWizardResultViewModel;
        }



        public void Dispose()
        {
        }
    }


    public class SalaryWizardResultViewModel
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public int DocTypeId { get; set; }
        public string DocDate { get; set; }
        public Decimal Days { get; set; }
        public Decimal Additions { get; set; }
        public Decimal Deductions { get; set; }
        public Decimal LoanEMI { get; set; }
    }

    public class LoanLedgerIdList
    {
        public int LedgerId { get; set; }
        public Decimal Amount { get; set; }
    }
}
