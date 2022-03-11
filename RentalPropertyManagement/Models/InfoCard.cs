using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentalPropertyManagement.Models
{
    public class InfoCard
    {
        public string Type { get; set; }
        public decimal Amount { get; set; }
        public Dictionary<string,decimal> PropertyList { get; set; }

        public InfoCard()
        {
            PropertyList = new Dictionary<string, decimal>();
        }
    }
}
