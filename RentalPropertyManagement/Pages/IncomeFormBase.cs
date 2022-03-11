using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components;
using Radzen;
using RentalPropertyManagement.Components;
using RentalPropertyManagement.Models;
using RentalPropertyManagement.Services;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentalPropertyManagement.Pages
{
    public class IncomeFormBase : ComponentBase
    {
        public IncomeByRoom IncomeByRoom = new IncomeByRoom();
        public string ConfirmationMessage { get; set; }
        public ConfirmBase ConfirmRef { get; set; }
        [Inject]
        public DBService DBService { get; set; }
        public string Message { get; set; } = "";
        public int RowsAffected { get; set; } = -99;
        public bool popup { get; set; } = false;
        [Inject]
        public PropertyService PropertyService { get; set; }
        public Property Property { get; set; }
        [Inject]
        public PageHistoryService PageHistoryService { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        public ConfirmItems ConfirmItems { get; set; }
        [Inject]
        public ISessionStorageService SessionStorageService { get; set; }
        public bool PropertySet { get; set; } = false;

        public List<int> YearList = new List<int>()
        {
            2020,
            2021,
            2022,
            2023,
            2024,
            2025
        };
        public List<string> MonthList = new List<string>()
        {
            "January",
            "February",
            "March",
            "April",
            "May",
            "June",
            "July",
            "August",
            "September",
            "October",
            "November",
            "December"
        };
        public List<string> RoomList = new List<string>();

        protected override void OnInitialized()
        {
            PageHistoryService.AddPageToHistory(NavigationManager.Uri);
            IncomeByRoom.Year = DateTime.Now.Year;
            IncomeByRoom.Month = DateTime.Now.ToString("MMMM");
            IncomeByRoom.Room = "Room1A";
            IncomeByRoom.Amount = 450;
            IncomeByRoom.Description = "Test";
            Property = PropertyService.Property;
            //RoomList.Add("Room1A");
            //RoomList.Add("Room1B");
            //if (Property.ID == 1)
            //{
            //    RoomList.Add("Room1C");
            //    RoomList.Add("Room1D");
            //}
            //RoomList.Add("Room2A");
            //RoomList.Add("Room2B");
            //RoomList.Add("Room2C");
            //if (Property.ID == 1)
            //    RoomList.Add("Room2D");

            ConfirmItems = new ConfirmItems();
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
                IncomeByRoom.Year = DateTime.Now.Year;
                IncomeByRoom.Month = DateTime.Now.ToString("MMMM");
                IncomeByRoom.Room = "Room1A";
                IncomeByRoom.Amount = 450;
                IncomeByRoom.Description = "Test";
                
                RoomList.Add("Room1A");
                RoomList.Add("Room1B");
                if (Property.ID == 1)
                {
                    RoomList.Add("Room1C");
                    RoomList.Add("Room1D");
                }
                RoomList.Add("Room2A");
                RoomList.Add("Room2B");
                RoomList.Add("Room2C");
                if (Property.ID == 1)
                    RoomList.Add("Room2D");

                ConfirmItems = new ConfirmItems();
                StateHasChanged();
            }
        }
        public void Submit(IncomeByRoom arg)
        {
            ConfirmItems.Title = "Are you sure?";
            ConfirmItems.Message = "Enter following transaction: ";
            ConfirmItems.Items = new Dictionary<string, object>();
            ConfirmItems.Items.Add("Property", Property.ShortName);
            ConfirmItems.Items.Add("Year", IncomeByRoom.Year);
            ConfirmItems.Items.Add("Month", IncomeByRoom.Month);
            ConfirmItems.Items.Add("Room", IncomeByRoom.Room);
            ConfirmItems.Items.Add("Amount", IncomeByRoom.Amount);
            ConfirmItems.Items.Add("Description", IncomeByRoom.Description);
            ConfirmRef.Show();
        }

        public void OnInvalidSubmit(FormInvalidSubmitEventArgs args)
        {

        }

        public void ConfirmSubmit(bool val)
        {
            if (val)
            {
                RowsAffected = DBService.InsertIntoRentInformationAsync(Property.ID, IncomeByRoom);
                if (RowsAffected == 0)
                    Message = "Error submitting transaction.";
                else if (RowsAffected == -1)
                    Message = "Record already exists.";
                else
                {
                    Message = "Successfully submitted.";
                    //Log.Error("Added new Income item.");
                }
            }
            else
                Message = "Transaction cancelled";
        }
    }
}
