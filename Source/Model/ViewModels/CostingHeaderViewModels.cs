using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Model.ViewModel;

namespace Model.ViewModels
{
    public class CostingHeaderIndexViewModel 
    {
        [Key]
        public int CostingHeaderId { get; set; }

        [ForeignKey("DocType"), Display(Name = "Costing Type"),Required(ErrorMessage="Please select a Document type")]
        public int DocTypeId { get; set; }
        public virtual DocumentType DocType { get; set; }

        [Display(Name = "Costing Date"), DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}"),Required(ErrorMessage= "Please select Costing Date")]
        public DateTime DocDate { get; set; }

        [Display(Name = "Costing No"), MaxLength(20, ErrorMessage = "Costing No. can not exceed 20 characters"), Required(ErrorMessage = "The CostingNo Field is Required")]
        public string DocNo { get; set; }

        [ForeignKey("Division"), Display(Name = "Division")]
        public int DivisionId { get; set; }
        public virtual Division Division { get; set; }

        [ForeignKey("Site"), Display(Name = "Site")]
        public int SiteId { get; set; }
        public virtual Site Site { get; set; }



        [ForeignKey("Person"), Display(Name = "Person"),Required(ErrorMessage= "Please select Person"),Range(1,int.MaxValue,ErrorMessage= "Person field is required")]
        public int PersonId { get; set; }
        public virtual Person Person { get; set; }









        public int Status { get; set; }

        [Display(Name = "Remark")]
        public string Remark { get; set; }


        public string CreatedBy { get; set; }

        [Display(Name = "Created Date"), DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Modified By")]
        public string ModifiedBy { get; set; }

        [Display(Name = "Modified Date"), DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}")]
        public DateTime ModifiedDate { get; set; }

         [Display(Name = "Person")]
         public string PersonName { get; set; }

         public string DivisionName { get; set; }
         public string SiteName { get; set; }
        
         [Display(Name="Entry Type")]
         public string DocumentTypeName { get; set; }



         public string ReviewBy { get; set; }
         public bool? Reviewed { get; set; }
         public int? ReviewCount { get; set; }

       public IEnumerable<CostingLine>  CostingLine {get;set;}

    }


    public class CostingLineListViewModel
    {
        public int CostingHeaderId { get; set; }
        public int CostingLineId { get; set; }
        public string DocNo { get; set; }
    }
}
