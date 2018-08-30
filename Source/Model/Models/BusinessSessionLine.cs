using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Models
{
    public class BusinessSessionLine : EntityBase, IHistoryLog
    {
        [Key]
        public int BusinessSessionLineId { get; set; }
        public int BusinessSessionId { get; set; }
        public int SiteId { get; set; }
        public int DivisionId { get; set; }
        public decimal? OpeningStockValue { get; set; }
        public decimal? ClosingStockValue { get; set; }
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
