using Microsoft.AspNetCore.Mvc;
using ReservationSyste.Models;
using ReservationSyste.Services.Interfices;
using ReservationSyste.ViewModels;
using System.Diagnostics;
using System.Text.Json;

namespace ReservationSyste.Controllers
{
    public class HomeController : Controller
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
            List<Reservation> reservations;
            if (SearchTerm != null)
            {
                reservations = await _reservationService.SearchReservationAsync(SearchTerm);
            }
            else
            {
                reservations = await _reservationService.GetAllReservationAsync();
            }

            ViewBag.ReservationVM = new ReservationModel {Reservations = reservations };

            return View(new DateClass());
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        


        public IActionResult CheckInOut(string SessionValue)
        {
            _httpContext.HttpContext.Session.SetString("sessionRoomValue", SessionValue);

            return PartialView("_CheckInOut", new DateClass { Id = SessionValue.Split("!!!")[0], ImagePath = SessionValue.Split("!!!")[1] });
        }

        public JsonResult CheckInOutJSon(int ReservationId, string ImageUrl) => Json(JsonSerializer.Serialize<DateClass>(new DateClass { Id = ReservationId.ToString(), ImagePath = ImageUrl }));
      

    }
}