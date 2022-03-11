using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentalPropertyManagement.Models
{
    public class Property
    {
        public int ID { get; set; }
        public string Address { get; set; }
        public string ShortName { get; set; }
        public DateTime DateOfPurchase { get; set; }
        public decimal PurchasePrice { get; set; }
        public int NumBeds { get; set; }
        public int NumBaths { get; set; }
    }
}
