﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Models
{
    public class PurchaseInvoiceLineCharge : CalculationLineCharge
    {
        [ForeignKey("PurchaseInvoiceLine")]
        public int LineTableId { get; set; }
        public virtual PurchaseInvoiceLine PurchaseInvoiceLine { get; set; }

    }
}
