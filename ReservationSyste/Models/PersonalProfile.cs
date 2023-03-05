using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ReservationSyste.Models
{
    public class PersonalProfile
    {
            public int Id { get; set; }
            public string  FirstName { get; set; }
            public string  LastName { get; set; }
            public string  Sex { get; set; }
            public string DateOfBirth { get; set; }
            public string  Occupation { get; set; }
            public string  Phone { get; set; }
            public string Email { get; set; }
            public string AddressLine1 { get; set; }
            public string  AddressLine2 { get; set; }
        public string Purpose { get; set; }
        public string  City { get; set; }
            public string State { get; set; }
            public bool Term { get; set; }
            public DateTime CreatedDate { get; set; }
            public DateTime ModifiedDate { get; set; }

    }
}
