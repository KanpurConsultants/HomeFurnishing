using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Models
{
    public class CustomLine : EntityBase, IHistoryLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("CustomLineId")]
        public int CustomLineId { get; set; }

        [Display(Name = "Custom"), Required]
        [ForeignKey("CustomHeader")]
        public int CustomHeaderId { get; set; }
        public virtual CustomHeader CustomHeader { get; set; }

        

        [Display(Name = "Remark")]
        public string Remark { get; set; }



        [Display(Name = "Lock Reason")]
        public string LockReason { get; set; }

        public int? Sr { get; set; }

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
