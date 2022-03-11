using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RentalPropertyManagement.Models
{
    public class FormsModel
    {
        public string Valid { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Oh noes! that name is already taken")]
        public string InValid { get; set; }
        public string Blank { get; set; }
        public string Email { get; set; } = "email@example.com";
        public DateTime Date { get; set; } = DateTime.Now;
    }
}
