﻿using System.ComponentModel.DataAnnotations;

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
    public class EmployeeViewModel
    {
        public int EmployeeId { get; set; }
        public int PersonId { get; set; }

        public int DocTypeId { get; set; }

        [MaxLength(100, ErrorMessage = "{0} can not exceed {1} characters"), Required]
        public string Name { get; set; }

        [MaxLength(20, ErrorMessage = "{0} can not exceed {1} characters"), Required]
        public string Suffix { get; set; }

        [Index("IX_Person_Code", IsUnique = true)]
        [MaxLength(20, ErrorMessage = "{0} can not exceed {1} characters"), Required]
        public string Code { get; set; }

        [MaxLength(11, ErrorMessage = "{0} can not exceed {1} characters")]
        public string Phone { get; set; }

        [MaxLength(10, ErrorMessage = "{0} can not exceed {1} characters")]
        public string Mobile { get; set; }

        [MaxLength(100, ErrorMessage = "{0} can not exceed {1} characters")]
        public string Email { get; set; }

        [Display(Name = "Address"), Required]
        public string Address { get; set; }

        [Display(Name = "City"), Required]
        public int? CityId { get; set; }
        [ForeignKey("CityId")]
        public virtual City City { get; set; }

        [MaxLength(6), Required]
        public string Zipcode { get; set; }

        [Display(Name = "PAN No")]
        [MaxLength(40)]
        public string PanNo { get; set; }

        [ForeignKey("Guarantor"), Display(Name = "Guarantor")]
        public int? GuarantorId { get; set; }
        public virtual Person Guarantor { get; set; }

        [ForeignKey("TdsCategory"), Display(Name = "Tds Category")]
        public int? TdsCategoryId { get; set; }
        public virtual TdsCategory TdsCategory { get; set; }

        [ForeignKey("TdsGroup"), Display(Name = "Tds Group")]
        public int? TdsGroupId { get; set; }
        public virtual TdsGroup TdsGroup { get; set; }

        [Display(Name = "Is Sister Concern ?")]
        public Boolean IsSisterConcern { get; set; }

        [Display(Name = "Date Of Joining"), DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}")]
        public DateTime? DateOfJoining { get; set; }

        [Display(Name = "Date Of Relieving"), DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}")]
        public DateTime? DateOfRelieving { get; set; }

        [MaxLength(10)]
        public string WagesPayType { get; set; }

        [MaxLength(10)]
        public string PaymentType { get; set; }


        [ForeignKey("Designation")]
        [Display(Name = "Person Rate Group")]
        public int? DesignationId { get; set; }
        public virtual Designation Designation { get; set; }

        [ForeignKey("Department")]
        [Display(Name = "Person Rate Group")]
        public int? DepartmentId { get; set; }
        public virtual Department Department { get; set; }

        public int? CalculationId { get; set; }
        public int? CreaditDays { get; set; }

        public Decimal? CreaditLimit { get; set; }

        [Display(Name = "Is Active ?")]
        public Boolean IsActive { get; set; }

        [ForeignKey("SalesTaxGroupParty"), Display(Name = "Sales Tax Group Party")]
        public int? SalesTaxGroupPartyId { get; set; }
        public virtual SalesTaxGroupParty SalesTaxGroupParty { get; set; }

        [ForeignKey("LedgerAccountGroup"), Display(Name = "Account Group"), Required]
        public int LedgerAccountGroupId { get; set; }
        //public virtual AccountGroup AccountGroup { get; set; }

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        public int PersonAddressID { get; set; }

        public int AccountId { get; set; }

        public string DivisionIds { get; set; }

        public string SiteIds { get; set; }
        public string ProcessIds { get; set; }

        //Login Site Id and Login Division Id
        public int DivisionId { get; set; }
        public int SiteId { get; set; }

        public int PersonRegistrationPanNoID { get; set; }

        public Decimal? BasicSalary { get; set; }

        public string Tags { get; set; }

        [MaxLength(100)]
        public string ImageFolderName { get; set; }

        [MaxLength(100)]
        public string ImageFileName { get; set; }
        public PersonSettingsViewModel PersonSettings { get; set; }
        public List<EmployeeCharge> footercharges { get; set; }
        public List<EmployeeLineCharge> linecharges { get; set; }

    }

    public class EmployeeIndexViewModel
    {
        public int EmployeeId { get; set; }
        public int PersonId { get; set; }
        public string Name { get; set; }
        public string Suffix { get; set; }
        public string Code { get; set; }
        public string Department { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }

    }
}
