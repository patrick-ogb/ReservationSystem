using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace ReservationSyste.ViewModels
{
    public class ReservationVM
    { 
        public ReservationVM()
        {
            AllowSmoking = new List<AllowSmoking>();
            ButlerServices = new List<ButlerService>();
            AirConditions = new List<AirCondition>();
            YearListRange = new List<GenerateYear>();

        }

        [DataType(DataType.Date), DisplayFormat(DataFormatString ="{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime TestDate { get; set; }

        public int Id { get; set; }
        [Required]
        public string ProductId { get; set; } = "";
        [Required]
        public string Name { get; set; }
        public string DateCreated { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        public double Price { get; set; }
        [Required]
        [DisplayName("Number of rooms")]
        public int RoomCount { get; set; }
        public DateTime LeavingDate { get; set; } = Convert.ToDateTime(DateTime.Now.ToString("yyy-MM-dd"));
        [Required]
        [DisplayName("Room photo")]
        public IFormFile Image { get; set; }
        public string ImageUrl { get; set; }

        //[Required]
        [DisplayName("Smoking Allowed")]
        public string SmokingAllowed { get; set; }
        public List<AllowSmoking> AllowSmoking { get; set; }
        //[Required]
        [DisplayName("Butller Services Available")]
        public string ButlerServiceAvailable { get; set; }
        public List<ButlerService> ButlerServices { get; set; }

        //[Required]
        [DisplayName("Air Conditioning")]
        public string AirConditioning { get; set; }
        public List<AirCondition> AirConditions { get; set; }


        public string Year { get;set; }
        public List<GenerateYear> YearListRange { get; set; }

        public string Month { get; set; }
        public List<GenerateYear> MonthListRange { get; set; }

    }





    public class GenerateYear
    {
        public int YearId { get; set; }
        public int Year { get; set; }

        public int monthId { get; set; }
        public int Month { get; set; }
        public string MonthName { get;set; }
        public static List<GenerateYear> GetYear()
        {
            List<GenerateYear> yearList = new List<GenerateYear>();
            int count = 0;
            for (int year = 2018; year <= DateTime.Now.Year; year++)
            {
                count++;
                yearList.Add(new GenerateYear { Year = year, YearId = count });
            }
            return yearList;
        }

        public static List<GenerateYear> GetMonth()
        {
            List<GenerateYear> monthList = new List<GenerateYear>();
            for (int month = 1; month <= 12; month++ )
            {
                string name = DateTimeFormatInfo.CurrentInfo.GetMonthName(month);
                monthList.Add(new GenerateYear { Month = month, monthId = month, MonthName = name });
            }
            return monthList;
        }
    }



    public class AllowSmoking
    {
        public int Id { get; set; }
        public string SmokingStatus { get; set; }

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
        public string ButlerServiceAv { get; set; }

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
        public string AirConditioningAv { get; set; }
        
        public static List<AirCondition> GetAirConditions()
        {
            return new List<AirCondition>
            {
                new AirCondition{AirConditioningAv = "Air Condition Available"},
                new AirCondition{AirConditioningAv = "Air Condition Not Available"},
            };
        }
    }

   
    public class Product
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public bool IsAvalable { get; set; }

        public static List<Product> GetProducts()
        {
            return new List<Product>
            {
                new Product{Id = 1, ProductName = "Shoes", IsAvalable= true},
                new Product{Id = 2, ProductName = "SmartTV", IsAvalable= false},
                new Product{Id = 3, ProductName = "Phone", IsAvalable= true},
                new Product{Id = 4, ProductName = "Car", IsAvalable= false},
                new Product{Id = 5, ProductName = "Laptop", IsAvalable= true},
            };
        }
    }

}
