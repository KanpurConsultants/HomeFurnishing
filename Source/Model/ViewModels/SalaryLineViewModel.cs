using System.ComponentModel.DataAnnotations;

// New namespace imports:
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using Model.Models;
using System;
using Microsoft.AspNet.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using Model.ViewModel;

namespace Model.ViewModels
{
    public class SalaryLineViewModel
    {
        [Key]
        public int SalaryLineId { get; set; }

        [ForeignKey("SalaryHeader")]
        public int SalaryHeaderId { get; set; }
        public virtual SalaryHeader SalaryHeader { get; set; }
        public string SalaryDocNo { get; set; }

        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }

        public decimal Days { get; set; }
        public decimal BasicSalary { get; set; }
        public decimal? OtherAddition { get; set; }
        public decimal? OtherDeduction { get; set; }
        public decimal? LoanEMI { get; set; }
        public decimal? Advance { get; set; }
        public decimal NetPayable { get; set; }
        public SalarySettingsViewModel SalarySettings { get; set; }
        public DocumentTypeSettingsViewModel DocumentTypeSettings { get; set; }

        

        [Display(Name = "Remark")]
        public string Remark { get; set; }


        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [Display(Name = "Modified By")]
        public string ModifiedBy { get; set; }

        [Display(Name = "Created Date"), DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Modified Date"), DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}")]
        public DateTime ModifiedDate { get; set; }


        public string LockReason { get; set; }

        public int Sr { get; set; }


        public List<SalaryLineCharge> linecharges { get; set; }
        public List<SalaryHeaderCharge> footercharges { get; set; }

        public int DocTypeId { get; set; }
        public string DocTypeName { get; set; }

        [Display(Name = "Division"), Required]
        public int DivisionId { get; set; }
        public string DivisionName { get; set; }

        [Display(Name = "Site"), Required]
        public int SiteId { get; set; }
        public string SiteName { get; set; }
    }

    public class SalaryLineIndexViewModel : SalaryLineViewModel
    {
        public string SalaryHeaderDocNo { get; set; }
        public int ProgressPerc { get; set; }
        public int unitDecimalPlaces { get; set; }
    }
}
