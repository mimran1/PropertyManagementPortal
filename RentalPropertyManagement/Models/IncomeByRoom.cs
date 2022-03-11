using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentalPropertyManagement.Models
{
    public class IncomeByRoom
    {
        [JsonIgnore]
        public int ID { get; set; }
        [JsonIgnore]
        public int Year { get; set; }
        [JsonIgnore]
        public string Month { get; set; }
        public string Room { get; set; }
        public decimal Amount { get; set; }
        [JsonIgnore]
        public string Description { get; set; }
        
        [JsonIgnore]
        //This was only added so that Radzen inner Grid works on FinancialDetailYearMonth Page
        //It is used to store a reference to the same record as this object
        //So if I have 2020 August Room1A 500 'Rent for Aug 2020' stored in IncomeByRoom object (i.e. this class) then
        //incomeByRooms will have this ONE entry so incomeByRooms[0].Year = 2020, incomeByRooms[0].Month = 'August' etc.
        //Note that this list gets initialized after all the records have been retrieved first (see FinancialDetailYearMonthBase for how its initialized)
        public IEnumerable<IncomeByRoom> incomeByRooms { get; set; }
        public int MonthNumber { get; set; }
        public string ShortName { get; set; }
        public int PropertyId { get; set; }
    }
}
