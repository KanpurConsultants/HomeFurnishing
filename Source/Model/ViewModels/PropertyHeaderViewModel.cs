using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Model.ViewModel;

namespace Model.ViewModels
{
    
    public partial class PropertyHeaderViewModel
    {
        public int PersonID { get; set; }
        public int LedgerAccountId { get; set; }
        public int DocTypeId { get; set; }
        public string DocTypeName { get; set; }
        public string Suffix { get; set; }
        public string Code { get; set; }
        public int? ParentId { get; set; }
        public string GISId { get; set; }
        public int GodownId { get; set; }
        public string GodownName { get; set; }
        public int? BinLocationId { get; set; }
        public string BinLocationName { get; set; }
        public int PersonAddressId { get; set; }
        public string Address { get; set; }
        public string ZipCode { get; set; }
        public string HouseNo { get; set; }
        public string OldHouseNo { get; set; }
        public int? AreaId { get; set; }
        public string AreaName { get; set; }
        public string Name { get; set; }
        public string FatherName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public int? CasteId { get; set; }
        public int? ReligionId { get; set; }
        public int? PersonRateGroupId { get; set; }
        public string DivisionIds  { get; set; }
        public string SiteIds { get; set; }
        public Boolean IsActive { get; set; }
        public string ImageFolderName { get; set; }
        public string ImageFileName { get; set; }
        public string AadharNo { get; set; }
        
        public Decimal? TotalPropertyArea { get; set; }
        public Decimal? TotalTaxableArea { get; set; }
        public Decimal? TotalARV { get; set; }
        public Decimal? TotalTax { get; set; }
        public int? ReviewCount { get; set; }
        public string ReviewBy { get; set; }
        public bool? Reviewed { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int Status { get; set; }

        public List<DocumentTypeAttributeViewModel> DocumentTypeAttributes { get; set; }
    }

    public class PropertyHeaderListViewModel
    {
        public int PersonID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class PropertyLineFilterViewModel
    {
        public int JobOrderHeaderId { get; set; }
        public int JobWorkerId { get; set; }
        [Display(Name = "Production Order")]
        public string ProdOrderHeaderId { get; set; }
        [Display(Name = "Product")]
        public string ProductId { get; set; }
        [Display(Name = "Product Group")]
        public string ProductGroupId { get; set; }


        public string Dimension1Id { get; set; }
        public string Dimension2Id { get; set; }

        public string DealUnitId { get; set; }
        public decimal Rate { get; set; }
        public JobOrderSettingsViewModel JobOrderSettings { get; set; }

    }

    public class PropertyLineListViewModel
    {
        public int JobOrderHeaderId { get; set; }
        public int JobOrderLineId { get; set; }
        public string DocNo { get; set; }
        public string Dimension1Name { get; set; }
        public string Dimension2Name { get; set; }
        public string DocumentTypeName { get; set; }
        public string ProductName { get; set; }
        public Decimal? BalanceQty { get; set; }
    }

    public class PropertyMasterDetailModel
    {
        public List<JobOrderLineViewModel> JobOrderLineViewModel { get; set; }
        public JobOrderSettingsViewModel JobOrderSettings { get; set; }
    }

    public class WardIndexViewModel
    {
        public int GodownId { get; set; }
        public string GodownCode { get; set; }
        public string GodownName { get; set; }
        public int PropertyCount { get; set; }
    }

}
