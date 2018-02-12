using Model;
using Model.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model.PropertyTax.Models
{
    public class ProductBuyerLog : EntityBase
    {
        [Key]
        public int ProductBuyerLogId { get; set; }

        [ForeignKey("ProductBuyer")]
        public int ProductBuyerId { get; set; }
        public ProductBuyer ProductBuyer { get; set; }
        [ForeignKey("Product")]
        [Display(Name = "Product")]
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        [ForeignKey("Buyer")]
        [Display(Name = "Buyer")]
        public int BuyerId { get; set; }
        public virtual Person Buyer { get; set; }

        [MaxLength(50)]
        public string BuyerSku { get; set; }

        [MaxLength(50)]
        public string BuyerProductCode { get; set; }

        [MaxLength(20)]
        public string BuyerUpcCode { get; set; }

        [MaxLength(50)]
        public string BuyerSpecification { get; set; }

        [MaxLength(50)]
        public string BuyerSpecification1 { get; set; }

        [MaxLength(50)]
        public string BuyerSpecification2 { get; set; }

        [MaxLength(50)]
        public string BuyerSpecification3 { get; set; }

        [MaxLength(50)]
        public string BuyerSpecification4 { get; set; }

        [MaxLength(50)]
        public string BuyerSpecification5 { get; set; }

        [MaxLength(50)]
        public string BuyerSpecification6 { get; set; }

        public DateTime DateOfConsutruction { get; set; }

        [ForeignKey("DiscountType"), Display(Name = "DiscountType")]
        public int? DiscountTypeId { get; set; }
        public virtual DiscountType DiscountType { get; set; }
        public Decimal? PropertyArea { get; set; }
        public Decimal? TaxableArea { get; set; }
        public Decimal? ARV { get; set; }
        public string TenantName { get; set; }
        public string BillingType { get; set; }
        public string Description { get; set; }
        public Decimal? CoveredArea { get; set; }
        public Decimal? GarageArea { get; set; }
        public Decimal? BalconyArea { get; set; }
        public bool? IsRented { get; set; }
        public DateTime WEF { get; set; }
        public Decimal? TaxAmount { get; set; }
        public Decimal? TaxPercentage { get; set; }
        public Decimal? WaterTaxAmount { get; set; }
        public Decimal? WaterTaxPercentage { get; set; }
        public string ModifyRemark { get; set; }

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
