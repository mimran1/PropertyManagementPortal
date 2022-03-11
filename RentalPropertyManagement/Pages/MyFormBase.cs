using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using RentalPropertyManagement.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RentalPropertyManagement.Pages
{
    public class MyFormBase : ComponentBase
    {
        public PersonModel Person = new PersonModel();

        public string LastSubmitResult { get; set; }

        public void ValidFormSubmitted(EditContext editContext)
        {
            LastSubmitResult = "OnValidSubmit was executed";
        }
        public void InvalidFormSubmitted(EditContext editContext)
        {
            LastSubmitResult = "OnInvalidSubmit was executed";
        }

        

    }
}
