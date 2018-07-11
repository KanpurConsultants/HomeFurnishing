using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Model.ViewModel;

namespace Model.ViewModels
{
    public class SalaryHeaderViewModel 
    {
        [Key]
        public int SalaryHeaderId { get; set; }

        public int DocTypeId { get; set; }
        public string DocTypeName { get; set; }

        public DateTime DocDate { get; set; }

        public string DocNo { get; set; }

        public int DivisionId { get; set; }
        public string DivisionName { get; set; }

        public int SiteId { get; set; }
        public string SiteName { get; set; }

        public string WagesPayType { get; set; }
         
        public int Status { get; set; }

        [Display(Name = "Remark")]
        public string Remark { get; set; }

        public int? LedgerHeaderId { get; set; }

        public SalarySettingsViewModel SalarySettings { get; set; }

        public DocumentTypeSettingsViewModel DocumentTypeSettings { get; set; }

        public string CreatedBy { get; set; }

        [Display(Name = "Created Date"), DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Modified By")]
        public string ModifiedBy { get; set; }

        [Display(Name = "Modified Date"), DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}")]
        public DateTime ModifiedDate { get; set; }

        public int? CalculationFooterChargeCount { get; set; }

         public string ReviewBy { get; set; }
         public bool? Reviewed { get; set; }
         public int? ReviewCount { get; set; }

         public string LockReason { get; set; }

       public IEnumerable<SalaryLine>  SalaryLine {get;set;}

    }
    public class SalaryHeaderListViewModel
    {
        public int SalaryHeaderId { get; set; }
        public int SalaryLineId { get; set; }
        public string DocNo { get; set; }

    }

    public class SalaryLineFilterViewModel
    {
        public int SalaryHeaderId { get; set; }


    }

    public class SalaryLineListViewModel
    {
        public int SalaryHeaderId { get; set; }
        public int SalaryLineId { get; set; }
        public string DocNo { get; set; }
    }

    public class SalaryMasterDetailModel
    {
        public List<SalaryLineViewModel> SalaryLineViewModel { get; set; }
        public DocumentTypeSettingsViewModel DocumentTypeSettings { get; set; }
    }



    public class SalaryWizardViewModel
    {
        public DateTime DocDate { get; set; }
        public int DocTypeId { get; set; }
        public string DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string WagesPayType { get; set; }
        public string Remark { get; set; }

        public int SalaryHeaderId { get; set; }
    }
}
