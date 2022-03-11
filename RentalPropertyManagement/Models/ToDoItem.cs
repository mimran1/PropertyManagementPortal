using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentalPropertyManagement.Models
{
    public class ToDoItem
    {
        public int ID { get; set; }
        public int ProprtyId { get; set; }
        public string Description { get; set; }
        public bool IsDone { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
