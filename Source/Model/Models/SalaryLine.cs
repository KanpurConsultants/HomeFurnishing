using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Models
{
    public class SalaryLine : EntityBase, IHistoryLog
    {
        [Key]
        public int SalaryLineId { get; set; }

        [ForeignKey("SalaryHeader")]
        [Index("IX_SalaryLine_Unique", IsUnique = true, Order = 1)]
        public int SalaryHeaderId { get; set; }
        public virtual SalaryHeader SalaryHeader { get; set; }

        [ForeignKey("Employee"), Display(Name = "Employee")]
        [Index("IX_SalaryLine_Unique", IsUnique = true, Order = 2)]
        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }

        public decimal Days { get; set; }
        public decimal BasicSalary { get; set; }
        public decimal? OtherAddition { get; set; }
        public decimal? OtherDeduction { get; set; }
        public decimal? LoanEMI { get; set; }
        public decimal? Advance { get; set; }
        public decimal NetPayable { get; set; }


        [Display(Name = "Remark")]
        public string Remark { get; set; }

        public int Sr { get; set; }

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [Display(Name = "Modified By")]
        public string ModifiedBy { get; set; }

        [Display(Name = "Created Date"), DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Modified Date"), DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}")]
        public DateTime ModifiedDate { get; set; }

        [Display(Name = "Lock Reason")]
        public string LockReason { get; set; }


        [MaxLength(50)]
        public string OMSId { get; set; }



    }
}
