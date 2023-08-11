using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ReservationSyste.Models;
using ReservationSyste.Services.Interfices;
using ReservationSyste.ViewModels;
using System.Diagnostics;
using System.Text.Json;

namespace ReservationSyste.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IReservationService _reservationService;
        private readonly IHttpContextAccessor _httpContext;

        public HomeController(ILogger<HomeController> logger, IReservationService reservationService,
            IHttpContextAccessor httpContext)
        {
            _logger = logger;
            _reservationService = reservationService;
            _httpContext = httpContext;
        }

        public async Task<IActionResult> Index(string SearchTerm = null)
        {
            List<Reservation> reservations = new List<Reservation>();
            if (SearchTerm != null)
            {
                reservations = await _reservationService.SearchReservationAsync(SearchTerm);
            }
            else
            {
                reservations= await _reservationService.GetAllReservationAsync();
             }

            TempData["noble"] = "Sir Noble";
            TempData.Keep("noble");
            ViewBag.ReservationVM = new ReservationModel {Reservations = reservations };
            BasicNotification("Geetings", NotificationType.Success, "Completed Successfully!");
            return View(new DateClass());
        }

        public IActionResult Create()
        {
            //el metodo de crear retorno true
            if (true)
            {
                BasicNotification("Geetings", NotificationType.Success, "Completed Successfully!");
            }
            return RedirectToAction(nameof(Index));

        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        


        public IActionResult CheckInOut(string SessionValue)
        {
            _httpContext.HttpContext.Session.SetString("sessionRoomValue", SessionValue);

            return PartialView("_CheckInOut", new DateClass { Id = Convert.ToInt32(SessionValue.Split("!!!")[0]), ImagePath = SessionValue.Split("!!!")[1] });
        }

        public JsonResult CheckInOutJSon(int ReservationId, string ImageUrl) => Json(System.Text.Json.JsonSerializer.Serialize<DateClass>(new DateClass { Id = ReservationId, ImagePath = ImageUrl }));
      
        public ActionResult Adminlte()
        {
            return View();
        }

        public ActionResult ChartGraph()
        {
            
            ChartDataViewModel vm = new ChartDataViewModel();
            var vmJsonObj = JsonConvert.SerializeObject(ChartData.GetChartDatas());

            ChartData chartData = new ChartData();
            
            return View(chartData);
        }




        class DBData
        {
            public Data[] Data { get; set; } // Data is an array of data rows.
        }

        class Data
        {
            [JsonProperty("Comments")]   // <- Mind that these are redundant.
            public string Comments { get; set; } // <- Also mind, the props should be of type string

            [JsonProperty("TimeStamp")]
            public string TimeStamp { get; set; }

            [JsonProperty("UserName")]
            public string UserName { get; set; }
        }





    }
}