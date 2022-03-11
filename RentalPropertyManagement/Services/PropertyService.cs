using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components;
using RentalPropertyManagement.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentalPropertyManagement.Services
{
    public class PropertyService
    {
        
        private readonly DBService _dbservice;
        public Property Property { get; set; }
        public IEnumerable<Property> Properties { get; set; }
        public bool IsAllPropertySelected { get; set; } = true;
        public bool IsSinglePropertySelected { get; set; } = false;
        [Inject]
        public ISessionStorageService SessionStorageService { get; set; }

        // Use Constructor Injection to set defualt value of Property
        // Note that setting a Property value is actually done in ChangePropertyComponentBase
        public PropertyService(DBService dBService)
        {
            _dbservice = dBService;
            Properties = _dbservice.GetAllProperties();
            IsAllPropertySelected = true;
            IsSinglePropertySelected = false;
            Property = new Property();
            Property.ID = -1;
            Property.ShortName = "All Properties";
            Log.Information("PropertyService contructor called.");
        }

        public void SetProperty(int id)
        {
            if (id == -1)
            {
                Properties = _dbservice.GetAllProperties();
                IsAllPropertySelected = true;
                IsSinglePropertySelected = false;
                Property.ID = -1;
                Property.ShortName = "All Properties";
            }
            else
            {
                Property = _dbservice.GetPropertyDetail(id);
                IsAllPropertySelected = false;
                IsSinglePropertySelected = true;
            }
        }

    }
}
