﻿using Model;
using System;
using System.ComponentModel.DataAnnotations;

namespace Model.ViewModel
{
    public class UserRoleViewModel : EntityBase
    {        
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string FromUserId { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public string Email { get; set; }
        public string RolesList { get; set; }
        public string RoleIdList { get; set; }
        public DateTime? ExpiryDate { get; set; }

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Modified By")]
        public string ModifiedBy { get; set; }

        [Display(Name = "Modified Date")]
        public DateTime ModifiedDate { get; set; }

    }

    public class UserRoleIndexViewModel 
    {
        public int UserRoleId { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public string SiteName { get; set; }
        public string DivisionName { get; set; }
    }
}
