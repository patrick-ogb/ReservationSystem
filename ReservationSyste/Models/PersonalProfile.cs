using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ReservationSyste.Models
{
    public class PersonalProfile
    {
            public int Id { get; set; }
            [Required]
            public string  Name { get; set; }
            [Required]
            [DisplayName("First Name")]
            public string  FirstName { get; set; }
            [Required]
            [DisplayName("Last Name")]
            public string  LastName { get; set; }
            [Required]
            public string  Sex { get; set; }
            [Required]
            [DisplayName("Date Of Birth ")]
            public string DateOfBirth { get; set; }
            [Required]
            public string  Occupation { get; set; }
       
            [Required]
            public string  Phone { get; set; }
            [Required]
            public string Email { get; set; }
            [Required]
            [DisplayName("Address Line 1")]
            public string AddressLine1 { get; set; }
            [DisplayName("Address Line 2")]
            public string  AddressLine2 { get; set; }
            [Required]
            public string  City { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
            [DisplayName("Arrival Time")]
            public DateTime ArrivalTime { get; set; }
            public string  Purpose { get; set; }
            [DisplayName("Additional Requirement")]
            public string  AdditionalRequirement { get; set; }
            [Required]
            public bool Captcha { get; set; }
            [Required]
            public bool Term { get; set; }
    }
}
