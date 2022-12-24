using Microsoft.AspNetCore.Mvc;
using ReservationSyste.Models;
using ReservationSyste.Services.Interfices;
using ReservationSyste.ViewModels;
using System.Diagnostics;

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

        public async Task<IActionResult> Index(string? SearchTerm = null)
        {
            List<Reservation> reservations;
            if(SearchTerm != null)
            {
                reservations = await _reservationService.SearchReservationAsync(SearchTerm);
            }
            else
            {
                reservations = await _reservationService.GetAllReservationAsync();
            }

            ReservationModel rservation = new ReservationModel
            {
                Reservations = reservations,
            };

            return View(rservation);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public IActionResult CheckInOut(string SessionValue)
        {
            _httpContext.HttpContext.Session.SetString("sessionRoomValue", SessionValue);

            string[] sses = SessionValue.Split("!!!");
            DateClass dateClass = new DateClass
            {
                Id = sses[0],
                ImagePath = sses[1],
            };

            return PartialView("_CheckInOut", dateClass);
        }

    }
}