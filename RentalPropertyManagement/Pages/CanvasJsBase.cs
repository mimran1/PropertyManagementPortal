using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using RentalPropertyManagement.Models;
using RentalPropertyManagement.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentalPropertyManagement.Pages
{
    public class CanvasJsBase: ComponentBase
    {
        public IEnumerable<MonthlyFinancial> MonthlyFinancial_2020 { get; set; }
        [Inject]
        public IJSRuntime JSRuntime { get; set; }
        [Inject]
        public DBService DBService { get; set; }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                MonthlyFinancial_2020 = (await DBService.GetMonthlyFinancial(1,"2020")).ToList();
                await JSRuntime.InvokeVoidAsync("canvasChart", JsonConvert.SerializeObject(MonthlyFinancial_2020));
            }
        }
    }
}
