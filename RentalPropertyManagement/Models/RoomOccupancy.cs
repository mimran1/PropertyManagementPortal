using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentalPropertyManagement.Models
{
    public class RoomOccupancy
    {
        public int Year { get; set; }
        public string Month { get; set; }
        public decimal Room1A { get; set; }
        public decimal Room1B { get; set; }
        public decimal Room1C { get; set; }
        public decimal Room1D { get; set; }
        public decimal Room2A { get; set; }
        public decimal Room2B { get; set; }
        public decimal Room2C { get; set; }
        public decimal Room2D { get; set; }
        private decimal _total;
        public decimal Total
        {
            get => _total; 
            set
            {
                _total = Room1A + Room1B + Room1C + Room1D + Room2A + Room2B + Room2C + Room2D;
            }
        }
        public int PropertyId { get; set; }
    }
}
