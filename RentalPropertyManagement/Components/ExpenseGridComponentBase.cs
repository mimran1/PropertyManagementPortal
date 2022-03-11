using Microsoft.AspNetCore.Components;
using RentalPropertyManagement.Models;
using RentalPropertyManagement.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentalPropertyManagement.Components
{
    public class ExpenseGridComponentBase : ComponentBase
    {
        [Parameter]
        public IEnumerable<ExpenseByYearMonth> ExpenseByYearMonths { get; set; }
        public string dash { get; set; } = "-";
        protected override void OnInitialized()
        {

        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (ExpenseByYearMonths != null)
            {
                decimal someval = 0;
                foreach (var item in ExpenseByYearMonths)
                {
                    //someval = someval + item.Taxes;
                    item.PropertyMangement = item.PropertyMangement;
                    item.Labor = item.Labor;
                    item.Material = item.Material;
                    item.Insurance = item.Insurance;
                    item.Taxes = item.Taxes;
                    item.Utilities = item.Utilities;
                    item.Total = item.Total;
                }
                
            }
            StateHasChanged();
        }

        public string CellDisplayValue(decimal val)
        {
            //https://www.csharp-examples.net/string-format-double/
            if (val == 0)
                return "-";
            else
                return "$" + String.Format("{0:0}", val);
        }
    }
}
