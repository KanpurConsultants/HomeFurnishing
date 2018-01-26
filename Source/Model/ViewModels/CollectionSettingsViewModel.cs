using System.ComponentModel.DataAnnotations;

namespace Model.ViewModels
{
    public class CollectionSettingsViewModel
    {
        public int CollectionSettingsId { get; set; }        
        public int DocTypeId { get; set; }
        public string DocTypeName { get; set; }


        public bool IsVisibleIntrestBalance { get; set; }
        public bool IsVisibleArearBalance { get; set; }
        public bool IsVisibleExcessBalance { get; set; }
        public bool IsVisibleCurrentYearBalance { get; set; }
        public bool IsVisibleNetOutstanding { get; set; }
        public bool IsVisibleReason { get; set; }



    }
}
