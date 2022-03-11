using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace RentalPropertyManagement.Models
{
    public class GeneralLedgerItem
    {
        public string Vendor { get; set; }
        public DateTime DateOfExpense { get; set; }
        public string Category { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
