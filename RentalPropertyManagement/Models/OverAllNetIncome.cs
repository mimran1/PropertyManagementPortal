using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentalPropertyManagement.Models
{
    public class OverAllNetIncome
    {
        public int PropertyId { get; set; }
        public decimal NetIncome { get; set; }
        public string ShortName { get; set; }
        public int Year { get; set; }
    }
}
