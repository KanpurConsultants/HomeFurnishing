using Model;
using Model.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model.PropertyTax.Models
{
    public class PersonExtended : EntityBase
    {
        [Key]
        [ForeignKey("Person")]
        public int PersonId { get; set; }
        public Person Person { get; set; }
        public string GISId { get; set; }
        
        [ForeignKey("Godown"), Display(Name = "Godown")]
        public int GodownId { get; set; }
        public virtual Godown Godown { get; set; }

        [ForeignKey("BinLocation"), Display(Name = "Bin Location")]
        public int? BinLocationId { get; set; }
        public virtual BinLocation BinLocation { get; set; }
        public string HouseNo { get; set; }
        public string OldHouseNo { get; set; }

        [ForeignKey("Area"), Display(Name = "Area")]
        public int? AreaId { get; set; }
        public virtual Area Area { get; set; }

        public string FatherName { get; set; }

        [MaxLength(50)]
        public string AadharNo { get; set; }


        

        [ForeignKey("Caste"), Display(Name = "Caste")]
        public int? CasteId { get; set; }
        public virtual Caste Caste { get; set; }

        [ForeignKey("Religion"), Display(Name = "Religion")]
        public int? ReligionId { get; set; }
        public virtual Religion Religion { get; set; }

        [ForeignKey("PersonRateGroup"), Display(Name = "PersonRateGroup")]
        public int? PersonRateGroupId { get; set; }
        public virtual PersonRateGroup PersonRateGroup { get; set; }
        public Decimal? TotalPropertyArea { get; set; }
        public Decimal? TotalTaxableArea { get; set; }
        public Decimal? TotalARV { get; set; }
        public Decimal? TotalTax { get; set; }
    }
}
