using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Models
{
    public class LedgerSupplementary : EntityBase
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Ledger Id")]
        [ForeignKey("Ledger")]
        public int LedgerId { get; set; }
        public virtual Ledger Ledger { get; set; }

        [ForeignKey("SupplementaryLedger")]
        [Display(Name = "Supplementary Ledger")]
        public int SupplementaryLedgerId { get; set; }
        public virtual Ledger SupplementaryLedger { get; set; }


        public Decimal Amount { get; set; }

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [Display(Name = "Modified By")]
        public string ModifiedBy { get; set; }

        [Display(Name = "Created Date"), DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Modified Date"), DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}")]
        public DateTime ModifiedDate { get; set; }

        [MaxLength(50)]
        public string OMSId { get; set; }


    }
}
