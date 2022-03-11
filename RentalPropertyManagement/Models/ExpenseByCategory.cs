using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentalPropertyManagement.Models
{
    public class ExpenseByCategory
    {
        public int Year { get; set; }
        public string Month { get; set; }
        public string Category { get; set; }
        public decimal Amount { get; set; }
        //Holds reference to all the expense items for given month/year (used in FinancialDetailYearMonth)
        public IEnumerable<ExpenseItem> expenseItems;
        public string ShortName { get; set; }
        public int PropertyId { get; set; }
    }
}
