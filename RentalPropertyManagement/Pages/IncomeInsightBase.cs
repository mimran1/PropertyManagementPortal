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
    public class IncomeInsightBase:ComponentBase
    {
        [Inject]
        public DBService DBService { get; set; }
        [Inject]
        public IJSRuntime JSRuntime { get; set; }
        public IEnumerable<IncomeByRoom> MonthlyIncomeByRooms_2020 { get; set; }
        public IEnumerable<IncomeByRoom> MonthlyIncomeByRooms_2021 { get; set; }
        [Inject]
        public PropertyService PropertyService { get; set; }
        public Property Property { get; set; }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                try
                {
                    Property = PropertyService.Property;
                    MonthlyIncomeByRooms_2020 = (from incomeByRoom in (await DBService.GetIncomeByRoom()).ToList()
                                                       where incomeByRoom.Year == 2020
                                                       select new IncomeByRoom
                                                       {
                                                           Year = incomeByRoom.Year,
                                                           Month = incomeByRoom.Month,
                                                           Room = incomeByRoom.Room,
                                                           Amount = incomeByRoom.Amount
                                                       }).ToList();
                    MonthlyIncomeByRooms_2021 = (from incomeByRoom in (await DBService.GetIncomeByRoom()).ToList()
                                                       where incomeByRoom.Year == 2021
                                                       select new IncomeByRoom
                                                       {
                                                           Year = incomeByRoom.Year,
                                                           Month = incomeByRoom.Month,
                                                           Room = incomeByRoom.Room,
                                                           Amount = incomeByRoom.Amount
                                                       }).ToList();
                    StateHasChanged();
                    await JSRuntime.InvokeVoidAsync("incomeInsight_2020", JsonConvert.SerializeObject(MonthlyIncomeByRooms_2020));
                    //await JSRuntime.InvokeVoidAsync("incomeInsight_2021", JsonConvert.SerializeObject(MonthlyIncomeByRooms_2021));
                }
                catch (Exception e)
                {

                }

            }

        }
    }
}
