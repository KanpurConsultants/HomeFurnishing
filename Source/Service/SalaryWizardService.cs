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
        IEnumerable<AdvanceLedgerIdList> GetAdvanceList(int LedgerAccountId);
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
            if (vm.SalaryHeaderId == 0)
            {

                SqlParameter SqlParameterDocDate = new SqlParameter("@DocDate", vm.DocDate.ToString());
                SqlParameter SqlParameterDocTypeId = new SqlParameter("@DocTypeId", vm.DocTypeId);
                SqlParameter SqlParameterRemark = new SqlParameter("@Remark", (vm.Remark == null) ? DBNull.Value : (object)vm.Remark);
                SqlParameter SqlParameterDepartmentId = new SqlParameter("@DepartmentId", (vm.DepartmentId == null) ? DBNull.Value : (object)vm.DepartmentId);
                SqlParameter SqlParameterWagesPayType = new SqlParameter("@WagesPayType", (vm.WagesPayType == null) ? DBNull.Value : (object)vm.WagesPayType);
                SqlParameter SqlParameterContraDocumentTypeId = new SqlParameter("@ContraDocumentTypeId", (vm.ContraDocumentTypeId == null) ? DBNull.Value : (object)vm.ContraDocumentTypeId);

                SqlParameter SqlParameterDocDate1 = new SqlParameter("@DocDate", vm.DocDate.ToString());
                SqlParameter SqlParameterDocTypeId1 = new SqlParameter("@DocTypeId", vm.DocTypeId);
                SqlParameter SqlParameterRemark1 = new SqlParameter("@Remark", (vm.Remark == null) ? DBNull.Value : (object)vm.Remark);
                SqlParameter SqlParameterDepartmentId1 = new SqlParameter("@DepartmentId", (vm.DepartmentId == null) ? DBNull.Value : (object)vm.DepartmentId);
                SqlParameter SqlParameterWagesPayType1 = new SqlParameter("@WagesPayType", (vm.WagesPayType == null) ? DBNull.Value : (object)vm.WagesPayType);
                SqlParameter SqlParameterContraDocumentTypeId1 = new SqlParameter("@ContraDocumentTypeId", (vm.ContraDocumentTypeId == null) ? DBNull.Value : (object)vm.ContraDocumentTypeId);

                if (vm.WagesPayType == "Jobwork")
                {
                    System.Web.HttpContext.Current.Session["SalaryLineReferenceList"] = null;
                    mQry = @"SELECT 0 As SalaryHeaderId, E.PersonID AS EmployeeId, E.Name+','+E.Suffix AS EmployeeName, Convert(int,E.Code) AS Code, 1.00 AS Days, 0.00 as BasicPay, 0.00 AS TDS, 0.00 AS TDSBaseValue, 0.00 AS Additions, 0.00 AS Deductions, 
                        0.00 as RetensionAmount, IsNull(VLoan.LoanEMI,0) AS LoanEMI, IsNull(VAdvance.Advance,0) AS Advance, @DocDate As DocDate, 
                        @DocTypeId As DocTypeId, @WagesPayType AS WagesPayType, @Remark As HeaderRemark, Convert(Decimal(18,4),DAY(EOMONTH(@DocDate))) - IsNull(VSunday.NoOfSundays,0) AS MonthDays
                        FROM Web.People E
                        LEFT JOIN web.DocumentTypes DT ON DT.DocumentTypeId = E.DocTypeId 
                        LEFT JOIN 
                        (
                            SELECT L.EmployeeId
                            FROM Web.SalaryLines L 
                            LEFT JOIN Web.SalaryHeaders H ON L.SalaryHeaderId = H.SalaryHeaderId
                            WHERE H.DocDate = @DocDate 
                        ) AS VSalaryLine ON E.PersonID = VSalaryLine.EmployeeId
                        LEFT JOIN (
                            SELECT Count(*) AS NoOfSundays
                            FROM master..spt_values 
                            WHERE TYPE ='p' AND DATEDIFF(d, Convert(DATETIME,DATEADD(DAY,1,EOMONTH(@DocDate,-1))),Convert(DATETIME,EOMONTH(@DocDate))) >= number 
                            AND DATENAME(w,Convert(DATETIME,DATEADD(DAY,1,EOMONTH(@DocDate,-1)))+number) = 'Sunday'
                        ) As VSunday On 1 = 1
                        LEFT JOIN (
	                        SELECT E.PersonID, Sum(CASE WHEN IsNull(L.AmtDr,0) - IsNull(VAdj.AdjustedAmount,0) < LL.BaseRate
	                        THEN IsNull(L.AmtDr,0) - IsNull(VAdj.AdjustedAmount,0)
	                        ELSE LL.BaseRate END) AS LoanEMI, Min(L.LedgerId) As LoanLedgerId
	                        FROM Web.People E
	                        LEFT JOIN Web.LedgerAccounts A ON E.PersonID = A.PersonId
	                        LEFT JOIN Web.Ledgers L ON A.LedgerAccountId = L.LedgerAccountId
	                        LEFT JOIN Web.LedgerLines LL ON L.LedgerLineId = LL.LedgerLineId
	                        LEFT JOIN Web.LedgerHeaders H ON L.LedgerHeaderId = H.LedgerHeaderId
	                        LEFT JOIN Web.DocumentTypes D ON H.DocTypeId = D.DocumentTypeId
	                        LEFT JOIN 
                            (
		                        SELECT La.DrLedgerId, Sum(La.Amount) AS AdjustedAmount
		                        FROM Web.LedgerAdjs La
		                        GROUP BY La.DrLedgerId
	                        ) AS VAdj ON L.LedgerId = VAdj.DrLedgerId
	                        WHERE D.DocumentTypeName = '" + Jobs.Constants.DocumentType.DocumentTypeConstants.Loans.DocumentTypeName + "'" + @"
                            AND IsNull(L.AmtDr,0) - IsNull(VAdj.AdjustedAmount,0) > 0
                            AND isnull(LL.DueDate, H.DocDate) <=  @DocDate
	                        GROUP BY E.PersonID
                        ) AS VLoan ON E.PersonID = VLoan.PersonID
                        LEFT JOIN (
	                        SELECT E.PersonID, Sum(IsNull(L.AmtDr,0) - IsNull(VAdj.AdjustedAmount,0)) AS Advance, Min(L.LedgerId) As LoanLedgerId
	                        FROM Web.People E
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
	                        WHERE D.DocumentTypeName = '" + Jobs.Constants.DocumentType.DocumentTypeConstants.Advances.DocumentTypeName + "'" + @"
                            AND IsNull(L.AmtDr,0) - IsNull(VAdj.AdjustedAmount,0) > 0
	                        GROUP BY E.PersonID
                        ) AS VAdvance ON E.PersonID = VAdvance.PersonID
                        WHERE isnull(E.IsActive,1) =1 AND DT.DocumentTypeName = 'Job Worker'" +
                        (vm.DepartmentId != null ? " AND E.PersonId IN (SELECT DISTINCT  PP.PersonId " +
                    " FROM web.Processes P " +
                    " LEFT JOIN web.PersonProcesses PP ON PP.ProcessId = P.ProcessId " +
                    " WHERE PP.PersonId IS NOT NULL " +
                    " AND P.DepartmentId IN (SELECT Items FROM [dbo].[Split] (@DepartmentId, ',')) ) " : "") +
                    " AND VSalaryLine.EmployeeId IS NULL Order By E.Name+','+E.Suffix ";
                }
                else if (vm.WagesPayType == "Jobwork_ByVoucher")
                {
                    mQry = @"SELECT 0 As SalaryLineId, E.PersonID AS PersonID,  VSalaryLine.DocTypeId AS ReferenceDocTypeId, VSalaryLine.LedgerHeaderId AS ReferenceDocId, VSalaryLine.LedgerLineId ReferenceDocLineId
                        FROM Web.People E
                        LEFT JOIN web.DocumentTypes DT ON DT.DocumentTypeId = E.DocTypeId 
                        LEFT JOIN 
                        (
                            SELECT LA.PersonId, H.LedgerHeaderId, H.DocTypeId, L.LedgerLineId
							FROM web.LedgerHeaders H 
							LEFT JOIN web.LedgerLines L ON L.LedgerHeaderId = H.LedgerHeaderId
							LEFT JOIN web.LedgerAccounts LA ON LA.LedgerAccountId = L.LedgerAccountId
							LEFT JOIN web.SalaryLineReferences SLR ON SLR.ReferenceDocTypeId = H.DocTypeId AND H.LedgerHeaderId = SLR.ReferenceDocId AND L.LedgerLineId = SLR.ReferenceDocLineId
							WHERE SLR.SalaryLineId IS NULL " +
                            (vm.ContraDocumentTypeId != null ? " AND H.DocTypeId IN (SELECT Items FROM[dbo].[Split](@ContraDocumentTypeId, ',')) " : "") +
                            @" ) AS VSalaryLine ON E.PersonID = VSalaryLine.PersonId
                        WHERE isnull(E.IsActive,1) =1 AND DT.DocumentTypeName = 'Job Worker'" +
                     (vm.DepartmentId != null ? " AND E.PersonId IN (SELECT DISTINCT  PP.PersonId " +
                    " FROM web.Processes P " +
                    " LEFT JOIN web.PersonProcesses PP ON PP.ProcessId = P.ProcessId " +
                    " WHERE PP.PersonId IS NOT NULL " +
                    " AND P.DepartmentId IN (SELECT Items FROM [dbo].[Split] (@DepartmentId, ',')) ) " : "") +
                    " AND VSalaryLine.PersonId IS NOT NULL Order By E.Name+','+E.Suffix ";
                    IEnumerable<SalaryLineReferenceViewModel> SalaryLineReferenceViewModel = db.Database.SqlQuery<SalaryLineReferenceViewModel>(mQry, SqlParameterDocDate1, SqlParameterDocTypeId1, SqlParameterRemark1, SqlParameterDepartmentId1, SqlParameterWagesPayType1, SqlParameterContraDocumentTypeId1).ToList();

                    System.Web.HttpContext.Current.Session["SalaryLineReferenceList"] = SalaryLineReferenceViewModel;

                    mQry = @"SELECT 0 As SalaryHeaderId, E.PersonID AS EmployeeId, E.Name+','+E.Suffix AS EmployeeName, Convert(int,E.Code) AS Code, 1.00 AS Days, isnull(VSalaryLine.Amount,0) as BasicPay, 0.00 AS TDS, 0.00 AS TDSBaseValue, 0.00 AS Additions, 0.00 AS Deductions, 
                        0.00 as RetensionAmount, IsNull(VLoan.LoanEMI,0) AS LoanEMI, IsNull(VAdvance.Advance,0) AS Advance, @DocDate As DocDate, 
                        @DocTypeId As DocTypeId, @WagesPayType AS WagesPayType, @Remark As HeaderRemark, Convert(Decimal(18,4),DAY(EOMONTH(@DocDate))) - IsNull(VSunday.NoOfSundays,0) AS MonthDays
                        FROM Web.People E
                        LEFT JOIN web.DocumentTypes DT ON DT.DocumentTypeId = E.DocTypeId 
                        LEFT JOIN 
                        (
                            SELECT LA.PersonId, sum(L.Amount) AS Amount 
							FROM web.LedgerHeaders H 
							LEFT JOIN web.LedgerLines L ON L.LedgerHeaderId = H.LedgerHeaderId
							LEFT JOIN web.LedgerAccounts LA ON LA.LedgerAccountId = L.LedgerAccountId
							LEFT JOIN web.SalaryLineReferences SLR ON SLR.ReferenceDocTypeId = H.DocTypeId AND H.LedgerHeaderId = SLR.ReferenceDocId AND L.LedgerLineId = SLR.ReferenceDocLineId
							WHERE 1=1 AND SLR.SalaryLineId IS NULL " +
                            (vm.ContraDocumentTypeId != null ? " AND H.DocTypeId IN (SELECT Items FROM[dbo].[Split](@ContraDocumentTypeId, ',')) " : "") +
                            @" GROUP BY LA.PersonId
                        ) AS VSalaryLine ON E.PersonID = VSalaryLine.PersonId
                        LEFT JOIN (
                            SELECT Count(*) AS NoOfSundays
                            FROM master..spt_values 
                            WHERE TYPE ='p' AND DATEDIFF(d, Convert(DATETIME,DATEADD(DAY,1,EOMONTH(@DocDate,-1))),Convert(DATETIME,EOMONTH(@DocDate))) >= number 
                            AND DATENAME(w,Convert(DATETIME,DATEADD(DAY,1,EOMONTH(@DocDate,-1)))+number) = 'Sunday'
                        ) As VSunday On 1 = 1
                        LEFT JOIN (
	                        SELECT E.PersonID, Sum(CASE WHEN IsNull(L.AmtDr,0) - IsNull(VAdj.AdjustedAmount,0) < LL.BaseRate
	                        THEN IsNull(L.AmtDr,0) - IsNull(VAdj.AdjustedAmount,0)
	                        ELSE LL.BaseRate END) AS LoanEMI, Min(L.LedgerId) As LoanLedgerId
	                        FROM Web.People E
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
	                        WHERE D.DocumentTypeName = '" + Jobs.Constants.DocumentType.DocumentTypeConstants.Loans.DocumentTypeName + "'" + @"
                            AND IsNull(L.AmtDr,0) - IsNull(VAdj.AdjustedAmount,0) > 0
                            AND isnull(LL.DueDate, H.DocDate) <=  @DocDate
	                        GROUP BY E.PersonID
                        ) AS VLoan ON E.PersonID = VLoan.PersonID
                        LEFT JOIN (
	                        SELECT E.PersonID, Sum(IsNull(L.AmtDr,0) - IsNull(VAdj.AdjustedAmount,0)) AS Advance, Min(L.LedgerId) As LoanLedgerId
	                        FROM Web.People E
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
	                        WHERE D.DocumentTypeName = '" + Jobs.Constants.DocumentType.DocumentTypeConstants.Advances.DocumentTypeName + "'" + @"
                            AND IsNull(L.AmtDr,0) - IsNull(VAdj.AdjustedAmount,0) > 0
	                        GROUP BY E.PersonID
                        ) AS VAdvance ON E.PersonID = VAdvance.PersonID
                        WHERE isnull(E.IsActive,1) =1 AND DT.DocumentTypeName = 'Job Worker'" +
                        (vm.DepartmentId != null ? " AND E.PersonId IN (SELECT DISTINCT  PP.PersonId " +
                    " FROM web.Processes P " +
                    " LEFT JOIN web.PersonProcesses PP ON PP.ProcessId = P.ProcessId " +
                    " WHERE PP.PersonId IS NOT NULL " +
                    " AND P.DepartmentId IN (SELECT Items FROM [dbo].[Split] (@DepartmentId, ',')) ) " : "") +
                    " AND VSalaryLine.PersonId IS NOT NULL Order By E.Name+','+E.Suffix ";
                }
                else if (vm.WagesPayType == "Jobwork_ByInvoice")
                {

                    mQry = @"SELECT 0 As SalaryLineId, E.PersonID AS PersonID,  VSalaryLine.DocTypeId AS ReferenceDocTypeId, VSalaryLine.JobInvoiceHeaderId AS ReferenceDocId, VSalaryLine.JobInvoiceLineId ReferenceDocLineId
                                FROM Web.People E
                                LEFT JOIN web.DocumentTypes DT ON DT.DocumentTypeId = E.DocTypeId
                                LEFT JOIN
                                     (
                                       SELECT L.JobWorkerId  PersonId, H.JobInvoiceHeaderId, H.DocTypeId, L.JobInvoiceLineId
                                       FROM web.JobInvoiceHeaders H
                                       LEFT JOIN web.JobInvoiceLines L ON L.JobInvoiceHeaderId = H.JobInvoiceHeaderId
                                       LEFT JOIN web.SalaryLineReferences SLR ON SLR.ReferenceDocTypeId = H.DocTypeId AND H.JobInvoiceHeaderId = SLR.ReferenceDocId AND L.JobInvoiceLineId = SLR.ReferenceDocLineId
                                       WHERE 1 = 1 AND SLR.SalaryLineId IS NULL " +
                                       (vm.ContraDocumentTypeId != null ? " AND H.DocTypeId IN (SELECT Items FROM[dbo].[Split](@ContraDocumentTypeId, ',')) " : "") +
                                       @" ) AS VSalaryLine ON E.PersonID = VSalaryLine.PersonId
                    WHERE isnull(E.IsActive,1) =1 " +
                     (vm.DepartmentId != null ? " AND E.PersonId IN (SELECT DISTINCT  PP.PersonId " +
                    " FROM web.Processes P " +
                    " LEFT JOIN web.PersonProcesses PP ON PP.ProcessId = P.ProcessId " +
                    " WHERE PP.PersonId IS NOT NULL " +
                    " AND P.DepartmentId IN (SELECT Items FROM [dbo].[Split] (@DepartmentId, ',')) ) " : "") +                    
                    " AND VSalaryLine.PersonId IS NOT NULL Order By E.Name + ',' + E.Suffix";

                    IEnumerable<SalaryLineReferenceViewModel> SalaryLineReferenceViewModel = db.Database.SqlQuery<SalaryLineReferenceViewModel>(mQry, SqlParameterDocDate1, SqlParameterDocTypeId1, SqlParameterRemark1, SqlParameterDepartmentId1, SqlParameterWagesPayType1, SqlParameterContraDocumentTypeId1).ToList();

                    System.Web.HttpContext.Current.Session["SalaryLineReferenceList"] = SalaryLineReferenceViewModel;

                    mQry = @"SELECT 0 As SalaryHeaderId, E.PersonID AS EmployeeId, E.Name+','+E.Suffix AS EmployeeName, Convert(int,E.Code) AS Code, 1.00 AS Days, isnull(VSalaryLine.Amount,0) as BasicPay, isnull((SELECT TDSAmount FROM web.FGetTds ( E.PersonID)),0) AS TDS, isnull((SELECT DueAmt - TdsOnAmt FROM web.FGetTds ( E.PersonID)),0) AS TDSBaseValue, 0.00 AS Additions, 0.00 AS Deductions, 
                        isnull(VSalaryLine.RetensionAmount,0) as RetensionAmount, IsNull(VLoan.LoanEMI,0) AS LoanEMI, IsNull(VAdvance.Advance,0) AS Advance, @DocDate As DocDate, 
                        @DocTypeId As DocTypeId, @WagesPayType AS WagesPayType, @Remark As HeaderRemark, Convert(Decimal(18,4),DAY(EOMONTH(@DocDate))) - IsNull(VSunday.NoOfSundays,0) AS MonthDays
                        FROM Web.People E
                        LEFT JOIN web.DocumentTypes DT ON DT.DocumentTypeId = E.DocTypeId 
                        LEFT JOIN 
                        (
                            SELECT L.JobWorkerId AS PersonId, sum(L.Amount) AS Amount , sum(L.RetensionAmount) AS RetensionAmount  
                            FROM web.JobInvoiceHeaders H
                            LEFT JOIN web.JobInvoiceLines L ON L.JobInvoiceHeaderId = H.JobInvoiceHeaderId
                            LEFT JOIN web.SalaryLineReferences SLR ON SLR.ReferenceDocTypeId = H.DocTypeId AND H.JobInvoiceHeaderId = SLR.ReferenceDocId AND L.JobInvoiceLineId = SLR.ReferenceDocLineId
                            WHERE 1 = 1 AND SLR.SalaryLineId IS NULL AND H.DocDate <= @DocDate " +
                            (vm.ContraDocumentTypeId != null ? " AND H.DocTypeId IN (SELECT Items FROM[dbo].[Split](@ContraDocumentTypeId, ',')) " : "") +
                            @" Group By L.JobWorkerId
                        ) AS VSalaryLine ON E.PersonID = VSalaryLine.PersonId
                        LEFT JOIN (
                            SELECT Count(*) AS NoOfSundays
                            FROM master..spt_values 
                            WHERE TYPE ='p' AND DATEDIFF(d, Convert(DATETIME,DATEADD(DAY,1,EOMONTH(@DocDate,-1))),Convert(DATETIME,EOMONTH(@DocDate))) >= number 
                            AND DATENAME(w,Convert(DATETIME,DATEADD(DAY,1,EOMONTH(@DocDate,-1)))+number) = 'Sunday'
                        ) As VSunday On 1 = 1
                        LEFT JOIN (
	                        SELECT E.PersonID, Sum(CASE WHEN IsNull(L.AmtDr,0) - IsNull(VAdj.AdjustedAmount,0) < LL.BaseRate
	                        THEN IsNull(L.AmtDr,0) - IsNull(VAdj.AdjustedAmount,0)
	                        ELSE LL.BaseRate END) AS LoanEMI, Min(L.LedgerId) As LoanLedgerId
	                        FROM Web.People E
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
	                        WHERE D.DocumentTypeName = '" + Jobs.Constants.DocumentType.DocumentTypeConstants.Loans.DocumentTypeName + "'" + @"
                            AND IsNull(L.AmtDr,0) - IsNull(VAdj.AdjustedAmount,0) > 0
                            AND isnull(LL.DueDate, H.DocDate) <=  @DocDate
	                        GROUP BY E.PersonID
                        ) AS VLoan ON E.PersonID = VLoan.PersonID
                        LEFT JOIN (
	                        SELECT E.PersonID, Sum(IsNull(L.AmtDr,0) - IsNull(VAdj.AdjustedAmount,0)) AS Advance, Min(L.LedgerId) As LoanLedgerId
	                        FROM Web.People E
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
	                        WHERE H.AdjustmentType = 'Advance' 
                            AND IsNull(L.AmtDr,0) - IsNull(VAdj.AdjustedAmount,0) > 0
	                        GROUP BY E.PersonID
                        ) AS VAdvance ON E.PersonID = VAdvance.PersonID
                        WHERE isnull(E.IsActive,1) =1 " +
                        (vm.DepartmentId != null ? " AND E.PersonId IN (SELECT DISTINCT  PP.PersonId " +
                    " FROM web.Processes P " +
                    " LEFT JOIN web.PersonProcesses PP ON PP.ProcessId = P.ProcessId " +
                    " WHERE PP.PersonId IS NOT NULL " +
                    " AND P.DepartmentId IN (SELECT Items FROM [dbo].[Split] (@DepartmentId, ',')) ) " : "") +
                    " AND VSalaryLine.PersonId IS NOT NULL Order By E.Name+','+E.Suffix ";
                }
                else
                {
                    System.Web.HttpContext.Current.Session["SalaryLineReferenceList"] = null;
                    mQry = @"SELECT 0 As SalaryHeaderId, E.PersonID AS EmployeeId, P.Name+','+P.Suffix AS EmployeeName, Convert(int,P.Code) AS Code, Convert(Decimal(18,4),DAY(EOMONTH(@DocDate))) - IsNull(VSunday.NoOfSundays,0) AS Days, Isnull(E.BasicSalary,0) as BasicPay, 0.00 AS TDS, 0.00 AS TDSBaseValue, 0.00 AS Additions, 0.00 AS Deductions, 
                        0.00 as RetensionAmount, IsNull(VLoan.LoanEMI,0) AS LoanEMI, IsNull(VAdvance.Advance,0) AS Advance, @DocDate As DocDate, 
                        @DocTypeId As DocTypeId, @WagesPayType AS WagesPayType, @Remark As HeaderRemark, Convert(Decimal(18,4),DAY(EOMONTH(@DocDate))) - IsNull(VSunday.NoOfSundays,0) AS MonthDays
                        FROM Web.Employees E
                        LEFT JOIN Web.People P ON E.PersonID = P.PersonID
                        LEFT JOIN (
                            SELECT L.EmployeeId
                            FROM Web.SalaryLines L 
                            LEFT JOIN Web.SalaryHeaders H ON L.SalaryHeaderId = H.SalaryHeaderId
                            WHERE H.DocDate = @DocDate 
                        ) AS VSalaryLine ON E.EmployeeId = VSalaryLine.EmployeeId
                        LEFT JOIN (
                            SELECT Count(*) AS NoOfSundays
                            FROM master..spt_values 
                            WHERE TYPE ='p' AND DATEDIFF(d, Convert(DATETIME,DATEADD(DAY,1,EOMONTH(@DocDate,-1))),Convert(DATETIME,EOMONTH(@DocDate))) >= number 
                            AND DATENAME(w,Convert(DATETIME,DATEADD(DAY,1,EOMONTH(@DocDate,-1)))+number) = 'Sunday'
                        ) As VSunday On 1 = 1
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
	                        WHERE D.DocumentTypeName = '" + Jobs.Constants.DocumentType.DocumentTypeConstants.Loans.DocumentTypeName + "'" + @"
                            AND IsNull(L.AmtDr,0) - IsNull(VAdj.AdjustedAmount,0) > 0
                            AND isnull(LL.DueDate, H.DocDate) <=  @DocDate
	                        GROUP BY E.EmployeeId
                        ) AS VLoan ON E.EmployeeId = VLoan.EmployeeId
                        LEFT JOIN (
	                        SELECT E.EmployeeId, Sum(IsNull(L.AmtDr,0) - IsNull(VAdj.AdjustedAmount,0)) AS Advance, Min(L.LedgerId) As LoanLedgerId
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
	                        WHERE D.DocumentTypeName = '" + Jobs.Constants.DocumentType.DocumentTypeConstants.Advances.DocumentTypeName + "'" + @"
                            AND IsNull(L.AmtDr,0) - IsNull(VAdj.AdjustedAmount,0) > 0
	                        GROUP BY E.EmployeeId
                        ) AS VAdvance ON E.EmployeeId = VAdvance.EmployeeId
                        WHERE isnull(P.IsActive,1) =1 And E.DateOfJoining <= @DocDate AND E.DateOfRelieving IS NULL " +
                            (vm.DepartmentId != null ? " AND E.DepartmentId IN (SELECT Items FROM [dbo].[Split] (@DepartmentId, ',')) " : "") +
                            (vm.WagesPayType != null ? " AND E.WagesPayType = @WagesPayType" : "") +
                            " AND VSalaryLine.EmployeeId IS NULL AND IsNull(E.BasicSalary,0) > 0 Order By P.Name+','+P.Suffix ";

                }

                IEnumerable<SalaryWizardResultViewModel> SalaryWizardResultViewModel = db.Database.SqlQuery<SalaryWizardResultViewModel>(mQry, SqlParameterDocDate, SqlParameterDocTypeId, SqlParameterRemark, SqlParameterDepartmentId, SqlParameterWagesPayType, SqlParameterContraDocumentTypeId).ToList();
                return SalaryWizardResultViewModel;
            }
            else
            {
                SqlParameter SqlParameterSalaryHeaderId = new SqlParameter("@SalaryHeaderId", vm.SalaryHeaderId.ToString());
                SqlParameter SqlParameterDocDate = new SqlParameter("@DocDate", vm.DocDate.ToString());

                if (vm.WagesPayType == "Jobwork")
                {
                    System.Web.HttpContext.Current.Session["SalaryLineReferenceList"] = null;
                    mQry = @"SELECT H.SalaryHeaderId, E.PersonID AS EmployeeId, E.Name+','+E.Suffix AS EmployeeName, Convert(int,E.Code) AS Code, 1.00 AS Days, 0.00 as BasicPay, 0.00 AS TDS, 0.00 AS TDSBaseValue, L.OtherAddition AS Additions, L.OtherDeduction AS Deductions, 
                        0.00 as RetensionAmount, IsNull(VLoan.LoanEMI,0) AS LoanEMI, IsNull(VAdvance.Advance,0) AS Advance, Convert(nvarchar,H.DocDate) As DocDate,                         
                        H.DocTypeId As DocTypeId, H.WagesPayType WagesPayType, H.Remark As HeaderRemark, Convert(Decimal(18,4),DAY(EOMONTH(H.DocDate))) - IsNull(VSunday.NoOfSundays,0) AS MonthDays
                        FROM Web.SalaryHeaders H
                        LEFT JOIN Web.SalaryLines L on H.SalaryHeaderId = L.SalaryHeaderId
                        LEFT JOIN Web.People E On L.EmployeeId = E.PersonID
                        LEFT JOIN web.DocumentTypes DT ON DT.DocumentTypeId = E.DocTypeId 
                        LEFT JOIN 
                        (
                            SELECT L.EmployeeId
                            FROM Web.SalaryLines L 
                            LEFT JOIN Web.SalaryHeaders H ON L.SalaryHeaderId = H.SalaryHeaderId
                            WHERE H.DocDate = @DocDate 
                        ) AS VSalaryLine ON E.PersonID = VSalaryLine.EmployeeId
                        LEFT JOIN (
                            SELECT Count(*) AS NoOfSundays
                            FROM master..spt_values 
                            WHERE TYPE ='p' AND DATEDIFF(d, Convert(DATETIME,DATEADD(DAY,1,EOMONTH(@DocDate,-1))),Convert(DATETIME,EOMONTH(@DocDate))) >= number 
                            AND DATENAME(w,Convert(DATETIME,DATEADD(DAY,1,EOMONTH(@DocDate,-1)))+number) = 'Sunday'
                        ) As VSunday On 1 = 1
                        LEFT JOIN (
	                        SELECT E.PersonID, Sum(CASE WHEN IsNull(L.AmtDr,0) - IsNull(VAdj.AdjustedAmount,0) < LL.BaseRate
	                        THEN IsNull(L.AmtDr,0) - IsNull(VAdj.AdjustedAmount,0)
	                        ELSE LL.BaseRate END) AS LoanEMI, Min(L.LedgerId) As LoanLedgerId
	                        FROM Web.People E
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
	                        WHERE D.DocumentTypeName = '" + Jobs.Constants.DocumentType.DocumentTypeConstants.Loans.DocumentTypeName + "'" + @"
                            AND IsNull(L.AmtDr,0) - IsNull(VAdj.AdjustedAmount,0) > 0
                            AND isnull(LL.DueDate, H.DocDate) <=  @DocDate
	                        GROUP BY E.PersonID
                        ) AS VLoan ON E.PersonID = VLoan.PersonID
                        LEFT JOIN (
	                        SELECT E.PersonID, Sum(IsNull(L.AmtDr,0) - IsNull(VAdj.AdjustedAmount,0)) AS Advance, Min(L.LedgerId) As LoanLedgerId
	                        FROM Web.People E
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
	                        WHERE D.DocumentTypeName = '" + Jobs.Constants.DocumentType.DocumentTypeConstants.Advances.DocumentTypeName + "'" + @"
                            AND IsNull(L.AmtDr,0) - IsNull(VAdj.AdjustedAmount,0) > 0
	                        GROUP BY E.PersonID
                        ) AS VAdvance ON E.PersonID = VAdvance.PersonID
                        Where L.SalaryHeaderId = @SalaryHeaderId ";
                }
                else if (vm.WagesPayType == "Jobwork_ByVoucher" || vm.WagesPayType == "Jobwork_ByInvoice")
                {
                    mQry = @"SELECT L.SalaryLineId, L.EmployeeId  AS PersonID,  SLR.ReferenceDocTypeId, SLR.ReferenceDocId, SLR.ReferenceDocLineId
                            FROM web.SalaryHeaders H
                            LEFT JOIN web.SalaryLines L ON L.SalaryHeaderId = H.SalaryHeaderId 
                            LEFT JOIN web.SalaryLineReferences SLR ON SLR.SalaryLineId = L.SalaryLineId  
                            WHERE H.SalaryHeaderId =" + vm.SalaryHeaderId + " ";
                    IEnumerable<SalaryLineReferenceViewModel> SalaryLineReferenceViewModel = db.Database.SqlQuery<SalaryLineReferenceViewModel>(mQry).ToList();

                    System.Web.HttpContext.Current.Session["SalaryLineReferenceList"] = SalaryLineReferenceViewModel;

                    mQry = @"SELECT H.SalaryHeaderId, E.PersonID AS EmployeeId, E.Name+','+E.Suffix AS EmployeeName, Convert(int,E.Code) AS Code, 1.00 AS Days, L.BasicSalary as BasicPay, 0.00 AS TDS, 0.00 AS TDSBaseValue, L.OtherAddition AS Additions, L.OtherDeduction AS Deductions, 
                        0.00 as RetensionAmount, IsNull(VLoan.LoanEMI,0) AS LoanEMI, IsNull(VAdvance.Advance,0) AS Advance, Convert(nvarchar,H.DocDate) As DocDate,                         
                        H.DocTypeId As DocTypeId, H.WagesPayType WagesPayType, H.Remark As HeaderRemark, Convert(Decimal(18,4),DAY(EOMONTH(H.DocDate))) - IsNull(VSunday.NoOfSundays,0) AS MonthDays
                        FROM Web.SalaryHeaders H
                        LEFT JOIN Web.SalaryLines L on H.SalaryHeaderId = L.SalaryHeaderId
                        LEFT JOIN Web.People E On L.EmployeeId = E.PersonID
                        LEFT JOIN web.DocumentTypes DT ON DT.DocumentTypeId = E.DocTypeId 
                        LEFT JOIN 
                        (
                            SELECT L.EmployeeId
                            FROM Web.SalaryLines L 
                            LEFT JOIN Web.SalaryHeaders H ON L.SalaryHeaderId = H.SalaryHeaderId
                            WHERE H.DocDate = @DocDate 
                        ) AS VSalaryLine ON E.PersonID = VSalaryLine.EmployeeId
                        LEFT JOIN (
                            SELECT Count(*) AS NoOfSundays
                            FROM master..spt_values 
                            WHERE TYPE ='p' AND DATEDIFF(d, Convert(DATETIME,DATEADD(DAY,1,EOMONTH(@DocDate,-1))),Convert(DATETIME,EOMONTH(@DocDate))) >= number 
                            AND DATENAME(w,Convert(DATETIME,DATEADD(DAY,1,EOMONTH(@DocDate,-1)))+number) = 'Sunday'
                        ) As VSunday On 1 = 1
                        LEFT JOIN (
	                        SELECT E.PersonID, Sum(CASE WHEN IsNull(L.AmtDr,0) - IsNull(VAdj.AdjustedAmount,0) < LL.BaseRate
	                        THEN IsNull(L.AmtDr,0) - IsNull(VAdj.AdjustedAmount,0)
	                        ELSE LL.BaseRate END) AS LoanEMI, Min(L.LedgerId) As LoanLedgerId
	                        FROM Web.People E
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
	                        WHERE D.DocumentTypeName = '" + Jobs.Constants.DocumentType.DocumentTypeConstants.Loans.DocumentTypeName + "'" + @"
                            AND IsNull(L.AmtDr,0) - IsNull(VAdj.AdjustedAmount,0) > 0
                            AND isnull(LL.DueDate, H.DocDate) <=  @DocDate
	                        GROUP BY E.PersonID
                        ) AS VLoan ON E.PersonID = VLoan.PersonID
                        LEFT JOIN (
	                        SELECT E.PersonID, Sum(IsNull(L.AmtDr,0) - IsNull(VAdj.AdjustedAmount,0)) AS Advance, Min(L.LedgerId) As LoanLedgerId
	                        FROM Web.People E
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
	                        WHERE D.DocumentTypeName = '" + Jobs.Constants.DocumentType.DocumentTypeConstants.Advances.DocumentTypeName + "'" + @"
                            AND IsNull(L.AmtDr,0) - IsNull(VAdj.AdjustedAmount,0) > 0
	                        GROUP BY E.PersonID
                        ) AS VAdvance ON E.PersonID = VAdvance.PersonID
                        Where L.SalaryHeaderId = @SalaryHeaderId ";
                }
                else
                {
                    System.Web.HttpContext.Current.Session["SalaryLineReferenceList"] = null;
                    mQry = @"SELECT H.SalaryHeaderId, E.PersonID as EmployeeId, P.Name+','+ P.Suffix AS EmployeeName, Convert(int,P.Code) AS Code, L.Days AS Days, Isnull(E.BasicSalary,0) as BasicPay,  0.00 AS TDS, 0.00 AS TDSBaseValue, L.OtherAddition AS Additions, L.OtherDeduction AS Deductions, 
                        0.00 as RetensionAmount, IsNull(VLoan.LoanEMI,0) AS LoanEMI, IsNull(VAdvance.Advance,0) AS Advance, Convert(nvarchar,H.DocDate) As DocDate, 
                        H.DocTypeId As DocTypeId, H.WagesPayType WagesPayType, H.Remark As HeaderRemark, Convert(Decimal(18,4),DAY(EOMONTH(H.DocDate))) - IsNull(VSunday.NoOfSundays,0) AS MonthDays
                        FROM Web.SalaryHeaders H
                        LEFT JOIN Web.SalaryLines L on H.SalaryHeaderId = L.SalaryHeaderId
                        LEFT JOIN Web.Employees E On L.EmployeeId = E.PersonID
                        LEFT JOIN Web.People P ON E.PersonID = P.PersonID
                        LEFT JOIN (
                            SELECT Count(*) AS NoOfSundays
                            FROM master..spt_values 
                            WHERE TYPE ='p' AND DATEDIFF(d, Convert(DATETIME,DATEADD(DAY,1,EOMONTH(@DocDate,-1))),Convert(DATETIME,EOMONTH(@DocDate))) >= number 
                            AND DATENAME(w,Convert(DATETIME,DATEADD(DAY,1,EOMONTH(@DocDate,-1)))+number) = 'Sunday'
                        ) As VSunday On 1 = 1
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
	                        WHERE D.DocumentTypeName = '" + Jobs.Constants.DocumentType.DocumentTypeConstants.Loans.DocumentTypeName + "'" + @"
                            AND IsNull(L.AmtDr,0) - IsNull(VAdj.AdjustedAmount,0) > 0
                            AND isnull(LL.DueDate, H.DocDate) <=  @DocDate
	                        GROUP BY E.EmployeeId
                        ) AS VLoan ON E.EmployeeId = VLoan.EmployeeId
                        LEFT JOIN (
	                        SELECT E.EmployeeId, Sum(IsNull(L.AmtDr,0) - IsNull(VAdj.AdjustedAmount,0)) AS Advance, Min(L.LedgerId) As LoanLedgerId
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
	                        WHERE D.DocumentTypeName = '" + Jobs.Constants.DocumentType.DocumentTypeConstants.Advances.DocumentTypeName + "'" + @"
                            AND IsNull(L.AmtDr,0) - IsNull(VAdj.AdjustedAmount,0) > 0
	                        GROUP BY E.EmployeeId
                        ) AS VAdvance ON E.EmployeeId = VAdvance.EmployeeId
                        Where L.SalaryHeaderId = @SalaryHeaderId ";

                }

                IEnumerable<SalaryWizardResultViewModel> SalaryWizardResultViewModel = db.Database.SqlQuery<SalaryWizardResultViewModel>(mQry, SqlParameterSalaryHeaderId, SqlParameterDocDate).ToList();
                return SalaryWizardResultViewModel;
            }
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
                        And D.DocumentTypeName = '" + Jobs.Constants.DocumentType.DocumentTypeConstants.Loans.DocumentTypeName + "'" + @"
                        AND IsNull(L.AmtDr,0) - IsNull(VAdj.AdjustedAmount,0) > 0 ";

            IEnumerable<LoanLedgerIdList> SalaryWizardResultViewModel = db.Database.SqlQuery<LoanLedgerIdList>(mQry, SqlParameterLedgerAccountId).ToList();

            return SalaryWizardResultViewModel;
        }



        public IEnumerable<AdvanceLedgerIdList> GetAdvanceList(int LedgerAccountId)
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
                        And D.DocumentTypeName = '" + Jobs.Constants.DocumentType.DocumentTypeConstants.Advances.DocumentTypeName + "'" + @"
                        AND IsNull(L.AmtDr,0) - IsNull(VAdj.AdjustedAmount,0) > 0 ";

            IEnumerable<AdvanceLedgerIdList> SalaryWizardResultViewModel = db.Database.SqlQuery<AdvanceLedgerIdList>(mQry, SqlParameterLedgerAccountId).ToList();

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
        public string DocDate { get; set; }
        public int Code { get; set; }
        public int DocTypeId { get; set; }
        public string WagesPayType { get; set; }
        public Decimal MonthDays { get; set; }
        public Decimal Days { get; set; }
        public Decimal BasicPay { get; set; }
        public Decimal TDS { get; set; }
        public Decimal TDSBaseValue { get; set; }
        public Decimal RetensionAmount { get; set; }        
        public Decimal Additions { get; set; }
        public Decimal Deductions { get; set; }
        public Decimal LoanEMI { get; set; }
        public Decimal Advance { get; set; }
        public string HeaderRemark { get; set; }
        public int? SalaryHeaderId { get; set; }
    }

    public class LoanLedgerIdList
    {
        public int LedgerId { get; set; }
        public Decimal Amount { get; set; }
    }

    public class AdvanceLedgerIdList
    {
        public int LedgerId { get; set; }
        public Decimal Amount { get; set; }
    }


}
