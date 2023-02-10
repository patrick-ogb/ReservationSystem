using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ReservationSyste.Data;
using ReservationSyste.Enums;
using ReservationSyste.Helper;
using ReservationSyste.Models;
using ReservationSyste.Services.Interfices;
using ReservationSyste.Utility;
using ReservationSyste.ViewModels;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace ReservationSyste.Controllers
{
    public class ReservationController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IReservationService _reservationService;
        private readonly IHttpContextAccessor _httpContext;
        private readonly ApplicationDbContext _dbContext;
        private readonly RecaptchaOption _option;
        private readonly RecaptchaHelper _helper;
        public ReservationController(IWebHostEnvironment webHostEnvironment, 
            IReservationService reservationService, IHttpContextAccessor httpContext,
            IOptions<RecaptchaOption> option, ApplicationDbContext dbContext)
        {
            _webHostEnvironment = webHostEnvironment;
            _reservationService = reservationService;
            _httpContext = httpContext;
            _dbContext = dbContext;
            _option = option.Value;
            _helper = new RecaptchaHelper(option);

        }

        public IActionResult CreateReservation() =>
                 View(new ReservationVM{ AllowSmoking = AllowSmoking.GetAllowSmokings(), ButlerServices = ButlerService.GetButlerServices(), AirConditions = AirCondition.GetAirConditions(), });
        


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

        public IActionResult PersonalProfile(DateClass model)
        {
            if (model.DateCheckOut is null || model.Email is null)
                return RedirectToAction("Index", "Home");

            if (_reservationService.VerifyEmail(model.Email))
            {
                //Code to book room
                return PartialView("_PaymentPartialView");
            }

            DateTimeVM dateTimeVM = new DateTimeVM();
                dateTimeVM.Email = model.Email;
                dateTimeVM.Id = model.Id;
                dateTimeVM.DateCheckOut = model.DateCheckOut;
                dateTimeVM.ImagePath = model.ImagePath;
                dateTimeVM.MonthName = Convert.ToDateTime(model.DateCheckOut).ToMonthName();
                dateTimeVM.RoomName = model.RoomName;
                dateTimeVM.Price = model.Price;
                dateTimeVM.CheckOut = Convert.ToDateTime(model.DateCheckOut);
                dateTimeVM.CheckIn = DateTime.Now;
                dateTimeVM.Days = (Convert.ToDateTime(model.DateCheckOut) - DateTime.Now).Days;
                dateTimeVM.RoomCount = (int)model.RoomCount;
                dateTimeVM.SiteKey = _option.SiteKey;
                ViewBag.Error = "Commence Captcha";
                ViewBag.RecaptchaValue = _option;


            var jSonObject  = JsonSerializer.Serialize<DateTimeVM>(dateTimeVM);

            _httpContext.HttpContext.Session.SetString("dateTimeVM", jSonObject);
            return View();
        }

        public async Task<ActionResult> CreatePersonalProfile(PersonalProfileVM personalProfileVM)
        {
            if (!ModelState.IsValid)
                return View(personalProfileVM);

            if (personalProfileVM.Term is not true)
                return RedirectToAction("PersonalProfile");

           
            string captchaResponse = Request.Form["g-recaptcha-response"].ToString();
            var validate = _helper.ValidateCaptcha(captchaResponse);
            if (!validate.Success)
            {
                personalProfileVM.Error = "Finish captcha";
                return RedirectToAction("PersonalProfile", "Reservation" );
            }

            var jSonObj = _httpContext.HttpContext.Session.GetString("dateTimeVM");
            var dataTimeVM = JsonSerializer.Deserialize<DateTimeVM>(jSonObj);

            personalProfileVM.Email = dataTimeVM.Email;
            personalProfileVM.RoomId = Convert.ToInt32(dataTimeVM.Id);

            PersonalProfile personalProfile = new PersonalProfile
            {
                Email = dataTimeVM.Email,
                Phone = personalProfileVM.Phone,
                Occupation = personalProfileVM.Occupation,
                DateOfBirth = personalProfileVM.DateOfBirth,
                Sex = personalProfileVM.Sex,
                CreatedDate = DateTime.Now,
                LastName = personalProfileVM.LastName,
                FirstName = personalProfileVM.FirstName,
                Term = personalProfileVM.Term,
                AddressLine1 = personalProfileVM.AddressLine1,
                AddressLine2 = personalProfileVM.AddressLine2,
                City = personalProfileVM.City,
                State = personalProfileVM.State,
            };

            using var transaction = _dbContext.Database.BeginTransaction();
            try
            {
                var result = await _reservationService.CreatePersonalProfileAsync(personalProfile, _dbContext);
                if (result > 0)
                {
                    PersonalProfileRoom personalProfileRoom = new PersonalProfileRoom
                    {
                        PersonalProfileId = result,
                        RoomId = Convert.ToInt32(dataTimeVM.Id),
                        CheckIn = dataTimeVM.CheckIn,
                        CheckOut = dataTimeVM.CheckOut,
                        Purpose = personalProfileVM.Purpose,
                        AdditionalRequirment = personalProfileVM.AdditionalRequirement,
                        ArrivalTime = personalProfileVM.ArrivalTime,
                    };

                    var response = await _reservationService.CreatePersonalProfileRoomAsync(personalProfileRoom, _dbContext);
                    if (response > 0)
                    {
                        var update = await _reservationService.UpdateReservationStatusAsync(Convert.ToInt32(dataTimeVM.Id), (int)ReservationStatusEnum.Requested, _dbContext);
                        if (update > 0)
                        {
                           await  transaction.CommitAsync();

                            return PartialView("_PaymentPartialView");
                        }
                        return RedirectToAction("PersonalProfile", "Reservation");
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
           

            return RedirectToAction("PersonalProfile");
        }



        public void HttpRequestDemo()
        {
            //Debug.Write("");
            //var request = new HttpRequestMessage(HttpMethod.Post, "token");
            //request.Headers.Authorization = new AuthenticationHeaderValue("Basic",
            //    Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_configuration["client_id"]}:{_configuration["client_secrate"]}")));
            //request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            //{
            //    { "grant_type", "application/json" }
            //});
            //HttpClient client = new HttpClient();
            //var response = await client.SendAsync(request);
            //response.EnsureSuccessStatusCode();
            //var streamResponse = await response.Content.ReadAsStreamAsync();
            //var authObject = await JsonSerializer.DeserializeAsync<AuthResponse>(streamResponse);
        }

    }

}





public class EditReservationVM : Reservation
{
    public IFormFile Image { get; set; }
}