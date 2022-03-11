using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentalPropertyManagement.Models
{
    public class ExpenseByYearMonth
    {
        public int Year { get; set; }
        public string Month { get; set; }
        public decimal PropertyMangement { get; set; }
        public decimal Labor { get; set; }
        public decimal Material { get; set; }
        public decimal CapitalImprovements { get; set; }
        public decimal Utilities { get; set; }
        public decimal Taxes { get; set; }
        public decimal Insurance { get; set; }
        public decimal Misc { get; set; }
        public decimal Total { get; set; }
        public int PropertyId { get; set; }
    }
}
