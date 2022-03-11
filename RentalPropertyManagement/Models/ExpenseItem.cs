using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace RentalPropertyManagement.Models
{
    public class ExpenseItem
    {
        [Required(ErrorMessage = "Vendor is required")]
        public string Vendor { get; set; }
        [Required(ErrorMessage = "Date of expense is required")]
        public int Year { get; set; }
        public string Month { get; set; }
        
        [Required(ErrorMessage = "Category is required")]
        public string Category { get; set; }
        [Required(ErrorMessage = "Amount is required")]
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public DateTime DateOfExpense { get; set; }
        public int ID { get; set; }
        public int PropertyId { get; set; }
        public string ShortName { get; set; }
        
    }
}
