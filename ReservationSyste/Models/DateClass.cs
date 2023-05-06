using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ReservationSyste.Models
{
    public class DateClass
    {
        public string ImagePath { get; set; }
        public int Id { get; set; }

        [Required]
        public string Email { get; set; }
        
        [ Required, DisplayName("Check out Date")]
        public string DateCheckOut { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }

        [DisplayName("Room Type")]
        public string RoomName { get; set; }
        public decimal Price { get; set; }
        public double RoomCount { get; set; }
        public string ActionNane { get; set; }
    }
}
