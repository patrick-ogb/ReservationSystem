using Microsoft.AspNetCore.Mvc;
using ReservationSyste.Services.Interfices;
using ReservationSyste.ViewModels;
using System.Text.Json;

namespace ReservationSyste.ViewComponents
{
    public class CheckInOutViewComponent : ViewComponent
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IReservationService _reservationService;

        public CheckInOutViewComponent(IHttpContextAccessor httpContextAccessor,
            IReservationService reservationService)
        {
            _httpContextAccessor = httpContextAccessor;
            _reservationService = reservationService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var dateTimeVM = _httpContextAccessor.HttpContext.Session.GetString("dateTimeVM");
            var jSonObject = JsonSerializer.Deserialize<DateTimeVM>(dateTimeVM);
            //var result =  await _reservationService.GetCheckInOutAsync();
            return View(jSonObject);
        }
    }
}
