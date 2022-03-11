using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using RentalPropertyManagement.Models;
using RentalPropertyManagement.Services;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentalPropertyManagement.Pages
{
    public class LedgerBase : ComponentBase
    {
        [Inject]
        public DBService DBService { get; set; }
        public IEnumerable<ExpenseItem> ExpenseItems { get;set; }
        public IEnumerable<IncomeByRoom> IncomeByRooms { get; set; }
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

        protected override void OnInitialized()
        {
            PageHistoryService.AddPageToHistory(NavigationManager.Uri);
        }
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
                    if(Property.ID==-1)
                    {
                        ExpenseItems = (await DBService.GetGeneralLedgerItem()).ToList();
                    }
                    else
                    {
                        ExpenseItems = (await DBService.GetGeneralLedgerItem())
                        .Where(x => x.PropertyId == Property.ID)
                        .ToList();
                    }

                    if (Property.ID == -1)
                    {
                        IncomeByRooms = (from incomeByRoom in (await DBService.GetIncomeByRoom()).ToList()
                                         //group incomeByRoom by new { incomeByRoom.Year, incomeByRoom.Month, incomeByRoom.Room } into eGroup
                                         select new IncomeByRoom
                                         {
                                             ID = incomeByRoom.ID,
                                             Year = incomeByRoom.Year,
                                             Month = incomeByRoom.Month,
                                             Room = incomeByRoom.Room,
                                             Amount = incomeByRoom.Amount,
                                             Description = incomeByRoom.Description,
                                             ShortName = incomeByRoom.ShortName
                                         }).ToList();
                    }
                    else
                    {
                        IncomeByRooms = (from incomeByRoom in (await DBService.GetIncomeByRoom()).ToList()
                                         where incomeByRoom.PropertyId == Property.ID
                                         //group incomeByRoom by new { incomeByRoom.Year, incomeByRoom.Month, incomeByRoom.Room } into eGroup
                                         select new IncomeByRoom
                                         {
                                             ID = incomeByRoom.ID,
                                             Year = incomeByRoom.Year,
                                             Month = incomeByRoom.Month,
                                             Room = incomeByRoom.Room,
                                             Amount = incomeByRoom.Amount,
                                             Description = incomeByRoom.Description,
                                             ShortName = incomeByRoom.ShortName
                                         }).ToList();
                    }
                    
                    StateHasChanged();
                    
                }
                catch(Exception e)
                {
                    Log.Error(e.Message);
                }
                
            }
        }
    }
}
