using Model.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DatabaseViews
{


    [Table("ViewCustomHeaderAttribute")]
    public class ViewCustomHeaderAttribute
    {
        [Key]
        public int CustomHeaderId { get; set; }
        public int SiteId { get; set; }
        public int DivisionId { get; set; }
        public int DocTypeId { get; set; }
        public DateTime DocDate { get; set; }
        public string DocNo { get; set; }
        public string Remark { get; set; }
    }

}
