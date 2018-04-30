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
    public class NarrationIndexViewModel
    {
        public int NarrationId { get; set; }
        public string NarrationName { get; set; }
        public string DocumentTypeName { get; set; }
    }
}
