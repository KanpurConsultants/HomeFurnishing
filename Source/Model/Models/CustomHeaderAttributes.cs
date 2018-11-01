using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Models
{
    public class CustomHeaderAttributes : DocumentHeaderAttributes 
    {

        [ForeignKey("CustomHeader")]
        public int HeaderTableId { get; set; }
        public virtual CustomHeader CustomHeader { get; set; }


    }
}
