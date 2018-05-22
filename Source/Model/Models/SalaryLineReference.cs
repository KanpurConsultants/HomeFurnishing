using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Models
{
    public class SalaryLineReference : EntityBase
    {
        [Key]
        [Column(Order = 0)]
        [ForeignKey("SalaryLine")]
        [Display(Name = "SalaryLine")]
        public int SalaryLineId { get; set; }
        public virtual SalaryLine SalaryLine { get; set; }

        [Key]
        [Column(Order = 1)]
        [ForeignKey("ReferenceDocType"), Display(Name = "ReferenceDocType")]
        public int ReferenceDocTypeId { get; set; }
        public virtual DocumentType ReferenceDocType { get; set; }

        [Key]
        [Column(Order = 2)]
        public int ReferenceDocId { get; set; }
        [Key]
        [Column(Order = 3)]
        public int ReferenceDocLineId { get; set; }
    }
}
