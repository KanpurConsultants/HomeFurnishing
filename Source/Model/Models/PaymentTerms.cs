using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model.Models
{
    public class PaymentTerms : EntityBase, IHistoryLog
    {
        public PaymentTerms()
        {
        }

        [Key]
        public int PaymentTermsId { get; set; }
        [Display (Name="Payment Terms")]
        [MaxLength(50, ErrorMessage = "Paymentterms Name cannot exceed 50 characters"), Required]
        [Index("IX_PaymentTerms_PaymentTermsName", IsUnique = true)]
        public string PaymentTermsName { get; set; }

        [MaxLength(50), Required]
        public string PrintingDescription { get; set; }

        [Display(Name = "Is Active ?")]
        public Boolean IsActive { get; set; }

        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        [MaxLength(50)]
        public string OMSId { get; set; }
    }
}
