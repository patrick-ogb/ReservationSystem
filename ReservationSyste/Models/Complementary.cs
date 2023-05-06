using System.ComponentModel.DataAnnotations.Schema;

namespace ReservationSyste.Models
{
    public class Complementary
    {
        public int Id { get; set; }
        public string Purpose { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string AdditionalRequirement { get; set; }
        public string SiteKey { get; set; }
    }
}
