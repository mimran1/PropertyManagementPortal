using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentalPropertyManagement.Models
{
    public class ConfirmItems
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public Dictionary<string,object> Items { get; set; }
    }
}
