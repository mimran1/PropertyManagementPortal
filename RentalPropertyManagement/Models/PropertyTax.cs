using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentalPropertyManagement.Models
{
    public class PropertyTax
    {
        public int PropertyId;
        public int Year;
        public string TaxType;
        public decimal Amount;
    }
}
