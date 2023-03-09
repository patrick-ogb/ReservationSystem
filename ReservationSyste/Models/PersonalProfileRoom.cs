namespace ReservationSyste.Models
{
    public class PersonalProfileRoom
    {
        public int Id { get; set; }
        public int PersonalProfileId { get; set; }
        public int RoomId { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public string Purpose { get; set; }
        public string AdditionalRequirment { get; set; }
        public DateTime ArrivalTime { get; set; }
        public int ProfileStatus { get; set; }
    }
}
