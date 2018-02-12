using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model.ViewModels
{
    public class PropertyLineViewModel
    {
        public int ProductBuyerId { get; set; }
        public int PersonId { get; set; }
        public DateTime DateOfConsutruction { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Dimension1Id { get; set; }
        public string Dimension1Name { get; set; }
        public int? DiscountTypeId { get; set; }
        public string DiscountTypeName { get; set; }
        public Decimal DiscountRate { get; set; }
        public Decimal? PropertyArea { get; set; }
        public Decimal? TaxableArea { get; set; }
        public Decimal? ARV { get; set; }
        public string TenantName { get; set; }
        public string BillingType { get; set; }
        public string Description { get; set; }
        public Decimal? CoveredArea { get; set; }
        public Decimal? GarageArea { get; set; }
        public Decimal? BalconyArea { get; set; }
        public bool IsRented { get; set; }
        public DateTime WEF { get; set; }
        public Decimal? TaxAmount { get; set; }
        public Decimal? TaxPercentage { get; set; }

        public Decimal? WaterTaxAmount { get; set; }
        public Decimal? WaterTaxPercentage { get; set; }
        public DateTime? NewWEF { get; set; }
        public string ModifyRemark { get; set; }
        public Decimal? OldARV { get; set; }
    }
}
