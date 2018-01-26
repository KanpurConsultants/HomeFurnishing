using Model;
using Model.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model.Models
{
    public class CollectionSettings : EntityBase, IHistoryLog
    {

        [Key]
        public int CollectionSettingsId { get; set; }

        [ForeignKey("DocType"), Display(Name = "Order Type")]
        public int DocTypeId { get; set; }
        public virtual DocumentType DocType { get; set; }      


        public bool? IsVisibleIntrestBalance { get; set; }
        public bool? IsVisibleArearBalance { get; set; }
        public bool? IsVisibleExcessBalance { get; set; }
        public bool? IsVisibleCurrentYearBalance { get; set; }
        public bool? IsVisibleNetOutstanding { get; set; }        
        public bool? IsVisibleReason { get; set; }


        /// <summary>
        /// DocId will be passed as a parameter in specified procedure.
        /// Procedure should have only one parameter of type int.
        /// </summary>
        [MaxLength(100)]
        public string SqlProcDocumentPrint { get; set; }

        /// <summary>
        /// DocId will be passed as a parameter in specified procedure.
        /// Procedure should have only one parameter of type int.
        /// </summary>
        [MaxLength(100)]
        public string SqlProcDocumentPrint_AfterSubmit { get; set; }

        /// <summary>
        /// DocId will be passed as a parameter in specified procedure.
        /// Procedure should have only one parameter of type int.
        /// </summary>
        [MaxLength(100)]
        public string SqlProcDocumentPrint_AfterApprove { get; set; }

        [MaxLength(100)]
        public string DocumentPrint { get; set; }


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
