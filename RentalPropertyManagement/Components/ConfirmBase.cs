using BlazorStrap;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using RentalPropertyManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentalPropertyManagement.Components
{
    //See this video for how to set up chile to parent communicatio: https://www.youtube.com/watch?v=Caw5hmq4dEY&t=349s
    //Note I modified it to some extent
    public class ConfirmBase: ComponentBase
    {
        protected bool ShowConfirmation { get; set; }
        public BSModal VerticallyCentered;
        
        [Parameter]
        public ConfirmItems ConfirmItems { get; set; }
        public ConfirmItems ConfirmItemsPleaseWork { get; set; }

        public List<string> confirmationMessageList { get; set; }

        protected override void OnParametersSet()
        {
            
        }

        protected override void OnAfterRender(bool firstRender)
        {
            //if (firstRender)
            //{
            //    ConfirmItemsPleaseWork = ConfirmItems;
            //    StateHasChanged();
            //}
        }
        public void Show()
        {
           ToggleModal();
        }

        [Parameter]
        public EventCallback<bool> ConfirmationChanged { get; set; }

        protected async Task OnConfirmationChange(bool value)
        {
            ToggleModal();
            await ConfirmationChanged.InvokeAsync(value);
        }

        public void ToggleModal()
        {
            VerticallyCentered.Toggle();
        }

        public async void OnNo(MouseEventArgs args)
        {
            ToggleModal();
            await ConfirmationChanged.InvokeAsync(false);
        }

        public async void OnYes(MouseEventArgs args)
        {
            ToggleModal();
            await ConfirmationChanged.InvokeAsync(true);
        }
    }
}
