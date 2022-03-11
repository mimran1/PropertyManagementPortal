using Microsoft.AspNetCore.Components;
using RentalPropertyManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentalPropertyManagement.Components
{
    public class RoomGridComponentBase : ComponentBase
    {
        [Parameter]
        public IEnumerable<RoomOccupancy> RoomOccupancies { get; set; }
        [Parameter]
        public Property Property { get; set; }

        public string CellDisplayValue(decimal val)
        {
            //https://www.csharp-examples.net/string-format-double/
            if (val == 0)
                return "-";
            else
                return "$" + String.Format("{0:0.##}", val);
        }
    }


}
