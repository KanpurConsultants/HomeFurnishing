using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Models
{
    public class CostingHeader : EntityBase, IHistoryLog
    {
        public CostingHeader()
        {
            CostingLines = new List<CostingLine>();
        }

        [Key]
        public int CostingHeaderId { get; set; }
                        
        [Display(Name = "Costing Type"),Required]
        [ForeignKey("DocType")]
        [Index("IX_CostingHeader_DocID", IsUnique = true, Order = 1)]
        public int DocTypeId { get; set; }
        public virtual DocumentType DocType { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}")]                
        [Display(Name = "Costing Date"),Required ]
        public DateTime DocDate { get; set; }
        
        [Display(Name = "Costing No"),Required,MaxLength(20) ]
        [Index("IX_CostingHeader_DocID", IsUnique = true, Order = 2)]
        public string DocNo { get; set; }

        [Display(Name = "Division"),Required ]
        [ForeignKey("Division")]
        [Index("IX_CostingHeader_DocID", IsUnique = true, Order = 3)]
        public int DivisionId { get; set; }
        public virtual  Division  Division { get; set; }

        [Display(Name = "Site"), Required]
        [ForeignKey("Site")]
        [Index("IX_CostingHeader_DocID", IsUnique = true, Order = 4)]
        public int SiteId { get; set; }
        public virtual Site Site { get; set; }


        [ForeignKey("Person"), Display(Name = "Person"), Required]
        public int PersonId { get; set; }
        public virtual Person Person { get; set; }


        [Display(Name = "Remark")]
        public string Remark { get; set; }

        [Display(Name = "Lock Reason")]
        public string LockReason { get; set; }

        public int Status { get; set; }
        public int? ReviewCount { get; set; }
        public string ReviewBy { get; set; }

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [Display(Name = "Modified By")]
        public string ModifiedBy { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Modified Date")]
        public DateTime ModifiedDate { get; set; }

        public ICollection<CostingLine> CostingLines { get; set; }

        [MaxLength(50)]
        public string OMSId { get; set; }

    }
}
