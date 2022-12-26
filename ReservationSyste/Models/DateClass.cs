using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ReservationSyste.Models
{
    public class DateClass
    {
        public string ImagePath { get; set; }
        public string Id { get; set; }

        [Required]
        public string Email { get; set; }

        
        [ Required, DisplayName("Check out Date")]
        public string DateCheckOut { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }

        [DisplayName("Room Type")]
        public string RoomName { get; set; }
        public double Price { get; set; }
        public double RoomCount { get; set; }
    }
}
