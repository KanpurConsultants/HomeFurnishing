﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model.Models
{
    public class Courier : EntityBase
    {
        public Courier()
        {
           
        }

        [Key]
        [ForeignKey("Person"), Display(Name = "Person")]
        public int PersonID { get; set; }
        public virtual Person Person { get; set; }

        [MaxLength(50)]
        public string OMSId { get; set; }
    }
}
