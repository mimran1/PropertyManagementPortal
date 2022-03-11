using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using RentalPropertyManagement.Models;
using RentalPropertyManagement.Services;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentalPropertyManagement.Pages
{
    public class OverAllComponentBase : ComponentBase
    {
        [Inject]
        public DBService DBService { get; set; }

        [Inject]
        public PropertyService PropertyService { get; set; }
        public Property Property { get; set; }
        [Inject]
        public PageHistoryService PageHistoryService { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        public ISessionStorageService SessionStorageService { get; set; }
        public bool PropertySet { get; set; } = false;
        public IEnumerable<OverAllNetIncome> OverAllNetIncomesMaster { get; set; }
        public int OverAllNetIncome { get; set; }
        public int OverAllNetIncome47 { get; set; }
        public int OverAllNetIncome198 { get; set; }
        public IEnumerable<Property> Properties { get; set; }
        public IEnumerable<OverAllNetIncome> OverAllNetIncomesByProperty { get; set; }
        [Inject]
        public IJSRuntime JSRuntime { get; set; }
        public int OverAllNetIncomeMonthly { get; set; }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                Property = await SessionStorageService.GetItemAsync<Property>("Property");
                //Set LastPage variable of the session to the current page. So when u change the property from the ChangePropertyComponent the LastPage variable will to used to navigate.
                await SessionStorageService.SetItemAsync<string>("LastPage", NavigationManager.Uri);
                //Set FirstVisit variable of the session to N. So when u change the propety from the ChangeProperComponent the FirstVisit will be N and the UI will display currently selected Property
                await SessionStorageService.SetItemAsync<string>("FirstVisit", "N");
                if (Property == null)
                {
                    Property = PropertyService.Property; //use defualt (all properties) in the event the session storage has nothing
                }
                PropertySet = true;
                try
                {
                    OverAllNetIncomesMaster = (await DBService.GetOverAllNetIncome()).ToList();
                    OverAllNetIncomesByProperty = (from netAmount in OverAllNetIncomesMaster
                                                   group netAmount by new { netAmount.ShortName } into eGroup
                                                   select new OverAllNetIncome
                                                   {
                                                       ShortName = eGroup.Key.ShortName,
                                                       NetIncome = eGroup.Sum(x => x.NetIncome)
                                                   }).ToList();
                    OverAllNetIncome = (int)(OverAllNetIncomesMaster.Sum(x => x.NetIncome));
                    OverAllNetIncome47 = (int)(OverAllNetIncomesMaster.Where(x => x.PropertyId == 1).Sum(x => x.NetIncome));
                    OverAllNetIncome198 = (int)(OverAllNetIncomesMaster.Where(x => x.PropertyId == 2).Sum(x => x.NetIncome));
                    DateTime now = DateTime.Now;
                    int NumOfMonths = ((now.Year - Constants.FirstRentStartDate.Year) * 12) + now.Month - Constants.FirstRentStartDate.Month;
                    OverAllNetIncomeMonthly = OverAllNetIncome / NumOfMonths;
                    StateHasChanged();
                    await JSRuntime.InvokeVoidAsync("topContributorPieChart", JsonConvert.SerializeObject(OverAllNetIncomesByProperty));
                    await JSRuntime.InvokeVoidAsync("yearlyPerformanceLineChart", JsonConvert.SerializeObject(OverAllNetIncomesMaster));
                }
                catch (Exception e)
                {
                    Log.Error(e.Message);
                }

            }
        }
    }
}
