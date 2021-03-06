﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Models
{
    public class ProductGroup : EntityBase, IHistoryLog
    {
        public ProductGroup()
        {
            Products = new List<Product>();
            RateDecimalPlaces = 2;
        }

        [Key]
        public int ProductGroupId { get; set; }

        [Display (Name="Product Group Name")]
        [MaxLength(50, ErrorMessage = "ProductGroup Name cannot exceed 50 characters"), Required]
        [Index("IX_ProductGroup_ProductGroupName", IsUnique = true)]
        public string ProductGroupName { get; set; }

        [ForeignKey("ProductType")]
        [Display(Name = "Product Type")]
        public int ProductTypeId { get; set; }
        public virtual ProductType ProductType { get; set; }

        [ForeignKey("DefaultSalesTaxProductCode")]
        [Display(Name = "Default Sales Tax Product Code")]
        public int? DefaultSalesTaxProductCodeId { get; set; }
        public virtual SalesTaxProductCode DefaultSalesTaxProductCode { get; set; }

        [ForeignKey("DefaultSalesTaxGroupProduct")]
        [Display(Name = "Default Sales Tax Group Product")]
        public int? DefaultSalesTaxGroupProductId { get; set; }
        public virtual ChargeGroupProduct DefaultSalesTaxGroupProduct { get; set; }

        public Byte RateDecimalPlaces { get; set; }


        [Display(Name = "Is System Define ?")]
        public Boolean IsSystemDefine { get; set; }

        [Display(Name = "Is Active ?")]
        public Boolean IsActive { get; set; }
        [MaxLength(100)]
        public string ImageFolderName { get; set; }

        [MaxLength(100)]
        public string ImageFileName { get; set; }

        public ICollection<Product> Products { get; set; }

        public Decimal ? Sr { get; set; }

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
