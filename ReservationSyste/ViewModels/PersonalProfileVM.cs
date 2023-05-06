using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReservationSyste.ViewModels
{
    public class PersonalProfileVM
    {
        public int Id { get; set; }
        [NotMapped]
        public string Name { get; set; } = string.Empty;
        public int RoomId { get; set; }
        [Required]
        [DisplayName("First Name")]
        public string FirstName { get; set; }
        [Required]
        [DisplayName("Last Name")]
        public string LastName { get; set; }
        [Required]
        public string Sex { get; set; }
        [Required]
        [DisplayName("Date Of Birth ")]
        public string DateOfBirth { get; set; }
        [Required]
        public string Occupation { get; set; }

        [Required]
        public string Phone { get; set; }
        public string Email { get; set; }
        [Required]
        [DisplayName("Address Line 1")]
        public string AddressLine1 { get; set; }
        [DisplayName("Address Line 2")]
        public string AddressLine2 { get; set; }
        [Required]
        public string City { get; set; }

        [Required]
        public string State { get; set; }

        [DisplayName("Arrival Time")]
        public DateTime ArrivalTime { get; set; }
        public string Purpose { get; set; }
        [DisplayName("Additional Requirement")]
        public string AdditionalRequirement { get; set; }
        [Required]
        public bool Term { get; set; }
        public string SiteKey { get; set; }
        public string Error { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public decimal Price { get; set; }
    }
}
