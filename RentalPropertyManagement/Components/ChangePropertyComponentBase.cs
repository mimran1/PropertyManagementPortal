using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components;
using RentalPropertyManagement.Models;
using RentalPropertyManagement.Services;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentalPropertyManagement.Components
{
    public class ChangePropertyComponentBase : ComponentBase
    {
        [Inject]
        public PropertyService PropertyService { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        public DBService DBService { get; set; }
        [Inject]
        public PageHistoryService PageHistoryService { get; set; }
        public string CurrentlySelected { get; set; } = "Currently Selected";
        [Inject]
        public ISessionStorageService SessionStorageService { get; set; }
        public string FirstVisit { get; set; }
        public bool RetrievingData { get; set; } = true;
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                Log.Information("Retrieving b4: " + RetrievingData);
                RetrievingData = true;
                FirstVisit = await SessionStorageService.GetItemAsync<string>("FirstVisit");
                if (FirstVisit == null)
                {
                    FirstVisit = "Y";
                }
                else
                {
                    //This was done because clicking using window.location in JS was calling the
                    //constructor of PropertyService again and resetting current selected property to "All Properties"
                    PropertyService.Property = await SessionStorageService.GetItemAsync<Property>("Property");
                    //To see session storage variables in Chrome: Open DevTools > Application > Session Storage (left pane)
                    await SessionStorageService.SetItemAsync<Property>("Property", PropertyService.Property);
                    
                    Log.Information("Retrieving after: " + RetrievingData);
                }
                RetrievingData = false;
                StateHasChanged();
            }
        }

        
        public async void OnClick(int propId)
        {
            //Set the property to what user selected
            //ADD THIS LINE
            PropertyService.SetProperty(propId);
            //Set the Property value of the session to what the user just selected
            await SessionStorageService.SetItemAsync<Property>("Property", PropertyService.Property);
            //Get the last page that was visited and navigate to it. If last page visited is nothing, then navigate home
            string prevPage = await SessionStorageService.GetItemAsync<string>("LastPage");
            //FirstVisit is used to the currently selected property in italics. If first time visting then the FirstVist is set to "Y" and non of the property is in italics.
            FirstVisit = await SessionStorageService.GetItemAsync<string>("FirstVisit");
            if(prevPage == null)
            {
                NavigationManager.NavigateTo("/home", false);
            }
            else
            {
                NavigationManager.NavigateTo(prevPage, false);
            }

        }
    }
}
