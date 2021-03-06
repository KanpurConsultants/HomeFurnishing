﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Surya.India.Model.Models;
using System.ComponentModel.DataAnnotations;

namespace Surya.India.Model.ViewModels
{
   public class SaleOrderAmendmentHeaderDetailsViewModel
   {
       public int ? SaleOrderAmendmentHeaderId { get; set; }

       public int ? DocumentTypeId { get; set; }
       [Display(Name="Entry Type")]
       public string DocumentTypeName { get; set; } 

       [DisplayFormat(DataFormatString="{0:dd/MMM/yyyy}")]
       [Display(Name="Amendment Date")]
       public DateTime DocDate { get; set; }

       public int? SaleOrderHeaderId { get; set; }

       public string Buyer { get; set; }
       [Display(Name = "Amendment No")]
       public string DocNo { get; set; }

       public int ? DivisionId { get; set; }

       public string DivisionName { get; set; }

       public int ? SiteId { get; set; }
       public string SiteName { get; set; }

       public int? Status { get; set; }

       public int ? ReasonId { get; set; }
       public string Reason { get; set; }
       [MinLength(20),Required(ErrorMessage="This Field is required")]
       public string LogReason { get; set; }
       [MinLength(30)]
       public string Remark { get; set; }

       [Display(Name = "Created By")]
       public string CreatedBy { get; set; }
       [Display(Name = "Modified By")]
       public string ModifiedBy { get; set; }
       [Display(Name = "Created Date"), DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}")]
       public DateTime CreatedDate { get; set; }
       [Display(Name = "Modified Date"), DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}")]
       public DateTime ModifiedDate { get; set; }
   }
}
