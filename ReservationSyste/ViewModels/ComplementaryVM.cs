using System.ComponentModel.DataAnnotations.Schema;

namespace ReservationSyste.ViewModels
{
    public class ComplementaryVM
    {
        public int Id { get; set; }
        [NotMapped]
        public string Name { get; set; } = string.Empty;
        public int PersonalProfileId { get; set; }
        public int RoomId { get; set; }
        public DateTime CheckIn { get; set; }
        public string CheckOut { get; set; }
        public string Purpose { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string AdditionalRequirement { get; set; }
        public  string SiteKey { get; set; }
        public decimal Price { get; set; }
    }
}
