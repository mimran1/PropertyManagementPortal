using Microsoft.AspNetCore.Components;
using RentalPropertyManagement.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentalPropertyManagement.Components
{
    public class InfoCardComponentBase : ComponentBase
    {
        [Parameter]
        public InfoCard InfoCard { get; set; }
        public Dictionary<string,string> TypeToColorMapping { get; set; }
        public Dictionary<string, string> ProgressBarColorToHouseMapping { get; set; }
        public string CardLeftBorderColor { get; set; } = "blue";
        public string ProgressBarColo { get; set; }
        public int LastRow { get; set; }
        [Parameter]
        public double FontSize { get; set; } = 1.0;
        
        protected override void OnParametersSet()
        {
            TypeToColorMapping = new Dictionary<string, string>();
            Log.Information("Received: " + InfoCard.Type);
            TypeToColorMapping.Add("Income", "#0479cc");
            TypeToColorMapping.Add("Expense", "#e27076");
            
            TypeToColorMapping.Add("Net", "#00876c");
            CardLeftBorderColor = TypeToColorMapping[InfoCard.Type];
            LastRow = InfoCard.PropertyList.Count;
            //Just so ur dumbass can see how that value is claculated 

            //foreach (KeyValuePair<string, decimal> item in InfoCard.PropertyList)
            //{
            //    decimal result = Math.Round(item.Value / InfoCard.Amount * 100);
            //}
        }

        
    }
}
