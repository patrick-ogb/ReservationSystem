using ReservationSyste.Models;

namespace ReservationSyste.ViewModels
{
    public class DateTimeVM : DateClass
    {
        public string MonthName { get; set; }
        public DateTime CheckOut { get; set; }
        public DateTime CheckIn { get; set; }
        public int Days { get; set; }
        public string SiteKey { get; set; }
        public string Error { get; set; }
        public int RoomId { get; set; }
        public double TatalAmount { get; set; }
    }
}
