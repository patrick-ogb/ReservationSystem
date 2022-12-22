using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using ReservationSyste.Enums;
using ReservationSyste.Models;
using ReservationSyste.Services.Interfices;
using ReservationSyste.ViewModels;

namespace ReservationSyste.Controllers
{
    public class ReservationController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IReservationService _reservationService;

        public ReservationController(IWebHostEnvironment webHostEnvironment, IReservationService reservationService)
        {
            _webHostEnvironment = webHostEnvironment;
            _reservationService = reservationService;
        }

        public IActionResult CreateReservation()
        {
            ReservationVM reservationVM = new ReservationVM
            {
                AllowSmoking = AllowSmoking.GetAllowSmokings(),
                ButlerServices = ButlerService.GetButlerServices(),
                AirConditions = AirCondition.GetAirConditions(),
            };
            return View(reservationVM);
        }


        [HttpPost]
        public async Task<IActionResult> CreateReservation(ReservationVM reservationVM)
        {
            if (ModelState.IsValid)
            {
                string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Images");
                string uniqueFileName = string.Empty;
                if (reservationVM.Image is not null && reservationVM.Image.Length > 0)
                {
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + reservationVM.Image?.FileName;
                    string completepath = Path.Combine(uploadFolder, uniqueFileName);

                    using (Stream stream = new FileStream(completepath, FileMode.Create))
                    {
                        await reservationVM.Image!.CopyToAsync(stream);
                    }

                    Reservation reservation = new Reservation
                    {
                        ImageUrl = uniqueFileName,
                        Name = reservationVM.Name,
                        DateCreated = DateTime.Now,
                        SmokingAllowed = reservationVM.SmokingAllowed,
                        AirConditioning = reservationVM.AirConditioning,
                        ButlerServerAvailable = reservationVM.ButlerServiceAvailable,
                        Content = reservationVM.Content,
                        RoomCount = reservationVM.RoomCount,
                        //RoomText = reservationVM.RoomText,
                        Price = reservationVM.Price,
                        ReservationStatus = (int)ReservationStatusEnum.Initiated,

                    };

                    var result = await _reservationService.CreateReservationAsync(reservation);
                    if (result > 0)
                        return RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError(string.Empty, "Unable to save reservation");
            return View(reservationVM);
        }

        public async Task<IActionResult> EditReservation(int Id)
        {
            var reservation = await _reservationService.FindReservationAsync(Id);
            EditReservationVM editReservationVM = new EditReservationVM
            {
                Id = reservation.Id,
                Name = reservation.Name,
                Content = reservation.Content,
                AirConditioning = reservation.AirConditioning,
                RoomCount = reservation.RoomCount,
                ReservationStatus = reservation.ReservationStatus,
                ButlerServerAvailable = reservation.ButlerServerAvailable,
                Price = reservation.Price,
                ImageUrl = reservation.ImageUrl,
            };
            return View(editReservationVM);
        }


        [HttpPost]
        public async Task<IActionResult> EditReservation(EditReservationVM model)
        {
            if (ModelState.IsValid)
            {
                var reservation = await _reservationService.FindReservationAsync(model.Id);

                Reservation reservatn = new Reservation
                {
                    Id = reservation.Id,
                    Name = reservation.Name,
                    Content = reservation.Content,
                    ReservationStatus = reservation.ReservationStatus,
                    RoomCount = reservation.RoomCount,
                    RoomText = reservation.RoomText,
                    Price = reservation.Price,
                    ImageUrl = reservation.ImageUrl,
                    SmokingAllowed = reservation.SmokingAllowed,
                    AirConditioning = reservation.AirConditioning,
                    ButlerServerAvailable = reservation?.ButlerServerAvailable,
                    DateModified = DateTime.Now,
                };
                if (model.Image != null)
                {
                    if (model.ImageUrl != null)
                    {
                        string filePath = Path.Combine(_webHostEnvironment.WebRootPath,
                            "images", model.ImageUrl);
                        System.IO.File.Delete(filePath);
                    }

                    reservatn.ImageUrl = ProcessUploadedFile(model); //1
                }
                    await _reservationService.UpdateReservationAsync(reservation ?? new Reservation());
            }

            return RedirectToAction("index", "Home");
        }

        private string ProcessUploadedFile(EditReservationVM model) //2
        {
            string uniqueFileName = String.Empty;
            if (model.Image != null)
            {
                string upLoadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Image.FileName;
                string filePath = Path.Combine(upLoadFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                    model.Image.CopyTo(fileStream);
            }

            return uniqueFileName;
        }
    }

}


public class EditReservationVM : Reservation
{
    public IFormFile? Image { get; set; }
}