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

            mQry = @"SELECT E.EmployeeId, P.Name AS EmployeeName, 30.00 AS Days, 0.00 AS Additions, 0.00 AS Deductions, 0.00 AS LoanEMI, 
                    @DocDate As DocDate, 
                    @DocTypeId As DocTypeId
                    FROM Web.Employees E
                    LEFT JOIN Web.People P ON E.PersonID = P.PersonID  ";

            //mQry = mQry + "UNION ALL " + mQry;
            //mQry = mQry + "UNION ALL " + mQry;

            IEnumerable<SalaryWizardResultViewModel> SalaryWizardResultViewModel = db.Database.SqlQuery<SalaryWizardResultViewModel>(mQry, SqlParameterDocDate, SqlParameterDocTypeId).ToList();

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
}
