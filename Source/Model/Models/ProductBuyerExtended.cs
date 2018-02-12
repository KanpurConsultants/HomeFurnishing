using Model;
using Model.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model.PropertyTax.Models
{
    public class ProductBuyerExtended : EntityBase
    {
        [Key]
        [ForeignKey("ProductBuyer")]
        public int ProductBuyerId { get; set; }
        public ProductBuyer ProductBuyer { get; set; }
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
    }
}
