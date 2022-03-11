using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentalPropertyManagement.Models
{
    public class MonthlyFinancial
    {
        public int Year { get; set; }
        public string Month { get; set; }
        public decimal Expenditure { get; set; }
        public decimal Income { get; set; }
        public decimal Net { get; set; }
        public string ShortName { get; set; }
        public int PropertyId { get; set; }
        public int MonthNumber { get; set; }
    }
}
