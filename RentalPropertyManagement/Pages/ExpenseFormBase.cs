using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components;
using Radzen;
using RentalPropertyManagement.Components;
using RentalPropertyManagement.Models;
using RentalPropertyManagement.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RentalPropertyManagement.Pages
{
    public class ExpenseFormBase : ComponentBase
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
        public ExpenseItem ExpenseItem = new ExpenseItem();
        public List<string> Categories = new List<string>()
        {
            "Property Management",
            "Insurance",
            "Material",
            "Labor",
            "Taxes",
            "Utilities",
            "Misc"
        };
        public List<int> YearList = new List<int>()
        {
            2022,
            2021
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

        public string LastSubmitResult { get; set; }
        public string Message { get; set; } = "";
        public decimal? Amount { get; set; }
        public bool popup = false;
        public ConfirmBase ConfirmRef { get; set; }
        public int RowsAffected { get; set; } = -99;

        public string ConfirmationMessage { get; set; }
        public ConfirmItems ConfirmItems { get; set; }
        [Inject]
        public ISessionStorageService SessionStorageService { get; set; }
        public bool PropertySet { get; set; } = false;
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
                ExpenseItem.Vendor = "Walmart";
                ExpenseItem.DateOfExpense = DateTime.Now;
                ExpenseItem.Category = "Material";
                ExpenseItem.Amount = 45.80m;
                ExpenseItem.Description = "Testing form";
                
                ConfirmItems = new ConfirmItems();

                StateHasChanged();
            }
                
        }

        public void Submit(ExpenseItem arg)
        {
            ConfirmItems.Title = "Are you sure?";
            ConfirmItems.Message = "Enter following transaction: ";
            ConfirmItems.Items = new Dictionary<string, object>();
            ConfirmItems.Items.Add("Property", Property.ShortName);
            ConfirmItems.Items.Add("Vendor", ExpenseItem.Vendor);
            ConfirmItems.Items.Add("Date Of Expense", ExpenseItem.DateOfExpense.ToString("MM/dd/yyyy"));
            ConfirmItems.Items.Add("Year", ExpenseItem.Year);
            ConfirmItems.Items.Add("Month", ExpenseItem.Month);
            ConfirmItems.Items.Add("Category", ExpenseItem.Category);
            ConfirmItems.Items.Add("Amount", ExpenseItem.Amount);
            ConfirmItems.Items.Add("Description", ExpenseItem.Description);
            
            ConfirmRef.Show();
        }


        public void ConfirmSubmit(bool val)
        {
            if (val)
            {
                RowsAffected = DBService.InsertIntoGeneralLedger(Property.ID, ExpenseItem);
                if (RowsAffected > 0)
                {
                    Message = "Transaction successfully submitted.";
                    ClearForm();
                }
                else
                    Message = "Something went wrong. Transaction was not submitted.";
            }
            else
                Message = "Transaction cancelled";
        }

        public void OnInvalidSubmit(FormInvalidSubmitEventArgs args)
        {
            LastSubmitResult = "OnInvalidSubmit was executed";
        }

        public void ClearForm()
        {
            ExpenseItem.Vendor = "";
            ExpenseItem.DateOfExpense = DateTime.Now;
            ExpenseItem.Category = "";
            ExpenseItem.Amount = 0.00m;
            ExpenseItem.Description = "";
        }
    }
}
