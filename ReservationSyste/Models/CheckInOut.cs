namespace ReservationSyste.Models
{
    public class CheckInOut
    {
        public int Id { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int Duration { get; set; }
        public int RoomCount { get; set; }
        public string RoomType { get; set; }
        public int TotalLengthOfStay { get; set; }
        public double TotalPrice { get; set; }
    }
}
