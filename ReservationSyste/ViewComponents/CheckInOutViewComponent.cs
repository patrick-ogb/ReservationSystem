using Microsoft.AspNetCore.Mvc;
using ReservationSyste.Services.Interfices;

namespace ReservationSyste.ViewComponents
{
    public class CheckInOutViewComponent : ViewComponent
    {
        private readonly IReservationService _reservationService;

        public CheckInOutViewComponent(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
           //var result =  await _reservationService.GetCheckInOutAsync();
            return View();
        }
    }
}
