using System.ComponentModel.DataAnnotations;

// New namespace imports:
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using Model.Models;
using System;
using Microsoft.AspNet.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using Model.ViewModel;

namespace Model.ViewModels
{
    [Serializable]
    public class SalaryLineReferenceViewModel
    {
        public int SalaryLineId { get; set; }
        public int ReferenceDocTypeId { get; set; }
        public int ReferenceDocId { get; set; }
        public int ReferenceDocLineId { get; set; }
        public int PersonId { get; set; }
    }

    public class SalaryLineReferenceIndexViewModel : SalaryLineReferenceViewModel
    {
        public string SalaryHeaderDocNo { get; set; }
        public string PersonName { get; set; }
        public string DocumentTypeName { get; set; }
        public string DocumentNo { get; set; }

    }

    public class SalaryLineReferenceSummaryViewModel
    {
        public List<SalaryLineReferenceIndexViewModel> SalaryLineReferenceSummaryVM { get; set; }

    }
}
