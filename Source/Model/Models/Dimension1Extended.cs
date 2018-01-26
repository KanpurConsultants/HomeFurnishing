using Model;
using Model.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model.Models
{
    public class Dimension1Extended : EntityBase
    {
        [Key]
        [ForeignKey("Dimension1")]
        public int Dimension1Id { get; set; }
        public Dimension1 Dimension1 { get; set; }
        public Decimal Multiplier { get; set; }
        public int CostCenterId { get; set; }
        public virtual CostCenter CostCenter { get; set; }
    }
}
