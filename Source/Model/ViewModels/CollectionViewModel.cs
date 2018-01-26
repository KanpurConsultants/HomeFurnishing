using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Model.ViewModels;

namespace Model.ViewModels
{
    
    public partial class CollectionViewModel
    {
        public int LedgerHeaderId { get; set; }
        public int LedgerLineId { get; set; }
        public int SiteId { get; set; }
        public int DivisionId { get; set; }
        public int PersonId { get; set; }
        public int LedgerAccountId { get; set; }
        public int PaymentModeLedgerAccountId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int GodownId { get; set; }
        public string GodownName { get; set; }
        public int? BinLocationId { get; set; }
        public string BinLocationName { get; set; }
        public int? AreaId { get; set; }
        public string AreaName { get; set; }
        public string HouseNo { get; set; }
        public string FatherName { get; set; }
        public Decimal? TotalPropertyArea { get; set; }
        public Decimal? TotalTaxableArea { get; set; }
        public Decimal? TotalARV { get; set; }
        public Decimal? TotalTax { get; set; }
        public int DocTypeId { get; set; }
        public string DocTypeName { get; set; }
        public int AgentDocTypeId { get; set; }
        public string DocNo { get; set; }
        public string PartyDocNo { get; set; }
        public DateTime DocDate { get; set; }
        public string ManualDocNo { get; set; }
        public Decimal IntrestBalance { get; set; }
        public Decimal ArearBalance { get; set; }
        public Decimal ExcessBalance { get; set; }
        public Decimal CurrentYearBalance { get; set; }
        public Decimal NetOutstanding { get; set; }
        public Decimal ReceivedAmount { get; set; }
        public int PaymentModeId { get; set; }
        public string PaymentModeName { get; set; }
        public int? AgentId { get; set; }
        public string AgentName { get; set; }

        public int? ReferenceLedgerAccountId { get; set; }
        public string ReferenceLedgerAccountName { get; set; }

        public string ChqNo { get; set; }
        public DateTime? ChqDate { get; set; }
        public int? ReviewCount { get; set; }
        public string ReviewBy { get; set; }
        public bool? Reviewed { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int Status { get; set; }
        public Decimal? DiscountAmount { get; set; }
        public CollectionSettingsViewModel CollectionSettings { get; set; }
    }
}
