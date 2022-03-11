using Microsoft.AspNetCore.Components;
using Radzen;
using RentalPropertyManagement.Models;
using RentalPropertyManagement.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentalPropertyManagement.Components
{
    public class ExpenseByCategoryGridComponentBase : ComponentBase
    {
        [Parameter]
        public IEnumerable<ExpenseByYearMonth> ExpenseByYearMonths { get; set; }
        [Inject]
        public DBService DBService { get; set; }
        public string ColWidth { get; set; } = "95px";
        public void RowRender(RowRenderEventArgs<ExpenseByYearMonth> args)
        {
            //args.Attributes.Add("style", $"font-weight: {(args.Data.Room1A > 20 ? "bold" : "normal")};");
        }

        public void CellRender(CellRenderEventArgs<ExpenseByYearMonth> args)
        {
            if (args.Column.Property.Equals("Total"))
            {
                args.Attributes.Add("style", $"font-weight: {(args.Data.Total > 0 ? "bold" : "normal")};");
            }

            if (args.Attributes.ContainsKey("style"))
                args.Attributes["style"] += "text-align: left;";
            else
                args.Attributes.Add("style", "text-align: left");
        }

        public string GetCellValue(decimal val)
        {
            if (val != 0)
                return "$" + val.ToString();

            return "-";
        }
    }
}
