using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components;
using RentalPropertyManagement.Models;
using RentalPropertyManagement.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentalPropertyManagement.Pages
{
    public class FeedbackBase : ComponentBase
    {
        public FeedBackItem FeedBackItem = new FeedBackItem();
        public Property Property { get; set; }
        [Inject]
        public ISessionStorageService SessionStorageService { get; set; }
        [Inject]
        public DBService DBService { get; set; }
        [Inject]
        public PropertyService PropertyService { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        public int RowsAffected { get; set; }
        public string Message { get; set; } = "";
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
                FeedBackItem.comment = "";
                StateHasChanged();
            }

        }
        public void Submit(FeedBackItem arg)
        {

            RowsAffected = DBService.InsertIntoFeedback(FeedBackItem.comment);
            if (RowsAffected > 0)
                Message = "Submitted. Thank you for your feedback. ";
            else
                Message = "Error! Its unfortunate that feedback did not work lol.";
        }
    }
}
