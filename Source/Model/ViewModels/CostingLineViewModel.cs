using System.ComponentModel.DataAnnotations;

// New namespace imports:
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using Model.Models;
using System;
using Microsoft.AspNet.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using Model.ViewModel;

namespace Model.ViewModels
{
    public class CostingLineViewModel
    {
        [Key]
        public int CostingLineId { get; set; }

        [ForeignKey("CostingHeader")]
        public int CostingHeaderId { get; set; }
        public virtual CostingHeader CostingHeader { get; set; }
        public string CostingDocNo { get; set; }

        [ForeignKey("Product"), Display(Name = "Product")]
        public int? ProductId { get; set; }
        public virtual Product Product { get; set; }
        public string ProductName { get; set; }

        [ForeignKey("ProductGroup"), Display(Name = "ProductGroup"),Required(ErrorMessage= "Please select a Product Group")]
        public int ? ProductGroupId { get; set; }
        public virtual ProductGroup ProductGroup { get; set; }
        public string ProductGroupName { get; set; }


        public decimal ? Qty { get; set; }


        public decimal ? PileWeight { get; set; }

        [Display(Name = "Remark")]
        public string Remark { get; set; }

        public DocumentTypeSettingsViewModel DocumentTypeSettings { get; set; }

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [Display(Name = "Modified By")]
        public string ModifiedBy { get; set; }

        [Display(Name = "Created Date"), DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Modified Date"), DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}")]
        public DateTime ModifiedDate { get; set; }

        
        public int? ColourId { get; set; }
        public string ColourName { get; set; }

        public int? SizeId { get; set; }
        public string SizeName { get; set; }

    }

    public class CostingLineIndexViewModel : CostingLineViewModel
    {
        public string CostingHeaderDocNo { get; set; }
        public int ProgressPerc { get; set; }
        public int unitDecimalPlaces { get; set; }



    }
    public class CostingLineDetail
    {
        public decimal? PileWeight { get; set; }
        public int? ProductGroupId { get; set; }
        public string ProductGroupName { get; set; }

        public int? ColourId { get; set; }
        public string ColourName { get; set; }

        public int? SizeId { get; set; }
        public string SizeName { get; set; }

    }

    public class CostingLineBalance
    {
        public int CostingLineId { get; set; }
        public string CostingDocNo { get; set; }

    }

    public class PendingCostingFromProc
    {


        public int CostingLineId { get; set; }
        public decimal BalanceQty { get; set; }
        public decimal Rate { get; set; }
        public decimal BalanceAmount { get; set; }
        public int CostingHeaderId { get; set; }
        public string CostingNo { get; set; }
        public int ProductId { get; set; }
        public int BuyerId { get; set; }
        public string BuyerName { get; set; }
        public DateTime OrderDate { get; set; }
        public string ProductName { get; set; }
        public int ProductGroupId { get; set; }
        public string UnitName { get; set; }
        public string Specification { get; set; }

        public bool BomDetailExists { get; set; }

        [Display(Name = "Dimension1")]
        [ForeignKey("Dimension1")]
        public int? Dimension1Id { get; set; }
        public virtual Dimension1 Dimension1 { get; set; }
        public string Dimension1Name { get; set; }

        [Display(Name = "Dimension2")]
        [ForeignKey("Dimension2")]
        public int? Dimension2Id { get; set; }
        public virtual Dimension2 Dimension2 { get; set; }

        public string Dimension2Name { get; set; }

        public int? Sr { get; set; }

    }
}
