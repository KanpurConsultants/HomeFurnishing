using Model;
using Model.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model.Models
{
    public class Religion : EntityBase, IHistoryLog
    {
        [Key]
        [Display(Name = "Religion Id")]
        public int ReligionId { get; set; }

        [ForeignKey("DocType")]
        public int DocTypeId { get; set; }
        public virtual DocumentType DocType { get; set; }

        [MaxLength(50, ErrorMessage = "Religion Name cannot exceed 50 characters"), Required]
        [Index("IX_Religion_Religion", IsUnique = true)]
        [Display(Name = "Religion Name")]
        public string ReligionName { get; set; }

        
        [Display(Name = "Is Active ?")]
        public Boolean IsActive { get; set; }

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [Display(Name = "Modified By")]
        public string ModifiedBy { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Modified Date")]
        public DateTime ModifiedDate { get; set; }

    }
}
