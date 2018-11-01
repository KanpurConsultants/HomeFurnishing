using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Models
{
    public class CostingLine : EntityBase, IHistoryLog
    {
        public CostingLine()
        {
        }

        [Key]
        public int CostingLineId { get; set; }

        [Display(Name = "Costing"), Required]
        [ForeignKey("CostingHeader")]
        public int CostingHeaderId { get; set; }
        public virtual CostingHeader CostingHeader { get; set; }


        [Display(Name = "ProductGroup"), Required]
        [ForeignKey("ProductGroup")]
        public int ProductGroupId { get; set; }
        public virtual ProductGroup ProductGroup { get; set; }

        [Display(Name = "Colour")]
        [ForeignKey("Colour")]
        public int? ColourId { get; set; }
        public virtual Colour Colour { get; set; }

        [Display(Name = "Size")]
        [ForeignKey("Size")]
        public int? SizeId { get; set; }
        public virtual Size Size { get; set; }

        [Display(Name = "Remark")]
        public string Remark { get; set; }

        [Display(Name = "Qty"), Required]
        public Decimal Qty { get; set; }

        [Display(Name = "Pile Weight")]
        public Decimal? PileWeight { get; set; }


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

        [MaxLength(50)]
        public string OMSId { get; set; }

    }
}
