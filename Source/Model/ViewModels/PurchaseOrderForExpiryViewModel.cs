﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Models;
using System.ComponentModel.DataAnnotations;

namespace Model.ViewModel
{
    public class PurchaseOrderForExpiryViewModel
    {
        public int PurchaseOrderHeaderId { get; set; }
        public string PurchaseOrderNo { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}")]
        public DateTime DocDate { get; set; }
        public string SDocDate { get; set; }
        [DisplayFormat(DataFormatString="{0:dd/MMM/yyyy}")]
        public DateTime ? DueDate { get; set; }
        public string SDueDate { get; set; }
        public string Revised { get; set; }
        public DateTime ? NewDueDate { get; set; }
        public string Reason { get; set; }
        public string DocTypeName { get; set; }
    }
}