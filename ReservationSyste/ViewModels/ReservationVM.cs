using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ReservationSyste.ViewModels
{
    public class ReservationVM
    {
        public ReservationVM()
        {
            AllowSmoking = new List<AllowSmoking>();
            ButlerServices = new List<ButlerService>();
            AirConditions = new List<AirCondition>();
        }
        public int Id { get; set; }
        
        [Required]
        public string? Name { get; set; }
        public string? DateCreated { get; set; }
        [Required]
        public string? Content { get; set; }
        [Required]
        public double Price { get; set; }
        [Required]
        [DisplayName("Number of rooms")]
        public int RoomCount { get; set; }
        
        [Required]
        [DisplayName("Room photo")]
        public IFormFile? Image { get; set; }
        public string? ImageUrl { get; set; }

        [Required]
        [DisplayName("Smoking Allowed")]
        public string? SmokingAllowed { get; set; }
        public List<AllowSmoking> AllowSmoking { get; set; }
        [Required]
        [DisplayName("Butller Services Available")]
        public string? ButlerServiceAvailable { get; set; }
        public List<ButlerService> ButlerServices { get; set; }

        [Required]
        [DisplayName("Air Conditioning")]
        public string? AirConditioning { get; set; }
        public List<AirCondition> AirConditions { get; set; }
    }




    public class AllowSmoking
    {
        public int Id { get; set; }
        public string? SmokingStatus { get; set; }

        public static List<AllowSmoking> GetAllowSmokings()
        {
            return new List<AllowSmoking>
            {
                new AllowSmoking{Id = 1, SmokingStatus = "Smoking Allowed"},
                new AllowSmoking{Id = 2, SmokingStatus = "Smoking Not Allowed"}
            };
        }
    }


    public class ButlerService
    {
        public string? ButlerServiceAv { get; set; }

        public static List<ButlerService> GetButlerServices()
        {
            return new List<ButlerService>
            {
                new ButlerService{ButlerServiceAv = "Butler ServiceAv Available"},
                new ButlerService{ButlerServiceAv = "Butler ServiceAv Not Available"},
            };
        }
    }


    public class AirCondition
    {
        public string? AirConditioningAv { get; set; }
        
        public static List<AirCondition> GetAirConditions()
        {
            return new List<AirCondition>
            {
                new AirCondition{AirConditioningAv = "Air Condition Available"},
                new AirCondition{AirConditioningAv = "Air Condition Not Available"},
            };
        }
    }
}
