using Microsoft.AspNetCore.Mvc;

namespace ReservationSyste.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public double Price { get; set; }
        public int RoomCount { get; set; }
        public string RoomText { get; set; }
        public string SmokingAllowed { get; set; }
        public string ButlerServerAvailable { get; set; }
        public string AirConditioning { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public int CreatedBy { get; set; }
        public int ModifiedBy { get; set; }
        public string ImageUrl { get; set; }
        public int ReservationStatus { get; set; }
    }


    public class ReservationModel
    {
        public  ReservationModel()
        {
            Reservations = new List<Reservation>();
           // DateClasses = new DateClass();
        }
       public IEnumerable<Reservation> Reservations { get; set; }

        

        //public DateClass DateClasses { get; set; }
    }

}


