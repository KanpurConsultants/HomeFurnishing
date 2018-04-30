﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Models
{
    public class Narration : EntityBase, IHistoryLog
    {
        public Narration()
        {
        }

        [Key]
        public int NarrationId { get; set; }

        [ForeignKey("DocType")]
        public int DocTypeId { get; set; }
        public virtual DocumentType DocType { get; set; }


        [Display (Name="Narration")]
        [Required]
        [Index("IX_Narration_NarrationName", IsUnique = true)]
        public string NarrationName { get; set; }

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
