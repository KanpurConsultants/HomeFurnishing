using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Models
{
    public class CustomHeader : EntityBase, IHistoryLog
    {
        public CustomHeader()
        {            
            CustomLines = new List<CustomLine>();
        }

        [Key]        
        public int CustomHeaderId { get; set; }
                        
        [Display(Name = "Custom Type"),Required]
        [ForeignKey("DocType")]
        [Index("IX_CustomHeader_DocID", IsUnique = true, Order = 1)]
        public int DocTypeId { get; set; }
        public virtual DocumentType DocType { get; set; }
                
        [Display(Name = "Custom Date"),Required ]
        public DateTime  DocDate { get; set; }
        
        [Display(Name = "Custom No"),Required,MaxLength(20) ]
        [Index("IX_CustomHeader_DocID", IsUnique = true, Order = 2)]
        public string DocNo { get; set; }

        [Display(Name = "Division"),Required ]
        [ForeignKey("Division")]
        [Index("IX_CustomHeader_DocID", IsUnique = true, Order = 3)]
        public int DivisionId { get; set; }
        public virtual  Division  Division { get; set; }

        [Display(Name = "Site"), Required]
        [ForeignKey("Site")]
        [Index("IX_CustomHeader_DocID", IsUnique = true, Order = 4)]
        public int SiteId { get; set; }
        public virtual Site Site { get; set; }

        
        public int Status { get; set; }

        
        [Display(Name = "Remark")]
        public string Remark { get; set; }

        [Display(Name = "Lock Reason")]
        public string LockReason { get; set; }
        
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }
        [Display(Name = "Modified By")]
        public string ModifiedBy { get; set; }
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
        [Display(Name = "Modified Date")]
        public DateTime ModifiedDate { get; set; }

        public ICollection<CustomLine> CustomLines { get; set; }

        [MaxLength(50)]
        public string OMSId { get; set; }
    }
}
