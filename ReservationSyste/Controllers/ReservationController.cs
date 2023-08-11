using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PayStack.Net;
using ReservationSyste.Data;
using ReservationSyste.Enums;
using ReservationSyste.Helper;
using ReservationSyste.Models;
using ReservationSyste.Services.Interfices;
using ReservationSyste.Utility;
using ReservationSyste.ViewModels;
using System.Text.Json;

namespace ReservationSyste.Controllers
{

    public class ReservationController : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IReservationService _reservationService;
        private readonly IHttpContextAccessor _httpContext;
        private readonly ApplicationDbContext _dbContext;
        private readonly RecaptchaOption _option;
        private readonly RecaptchaHelper _helper;
        private readonly string token;
        //private readonly IConfiguration _configuration;
        private PayStackApi Paystack { get; set; }

        public ReservationController(IWebHostEnvironment webHostEnvironment,
            IReservationService reservationService, IHttpContextAccessor httpContext,
            IOptions<RecaptchaOption> option, ApplicationDbContext dbContext, IConfiguration configuration)
        {
            _webHostEnvironment = webHostEnvironment;
            _reservationService = reservationService;
            _httpContext = httpContext;
            _dbContext = dbContext;
            _option = option.Value;
            _helper = new RecaptchaHelper(option);
            token = configuration["Payment:PaystackSK"];
            //token = configuration.GetSection("Payment")["PaystackSK"];
            Paystack = new PayStackApi(token);
        }

        public IActionResult CreateReservation()
        {
            ViewBag.Employees = Product.GetProducts();
            TempData.Keep("noble");
            return View(new ReservationVM
            {
                AllowSmoking = AllowSmoking.GetAllowSmokings(),
                ButlerServices = ButlerService.GetButlerServices(),
                AirConditions = AirCondition.GetAirConditions(),
                YearListRange = GenerateYear.GetYear(),
                MonthListRange = GenerateYear.GetMonth()
            }) ;
        }

        [HttpPost]
        public async Task<IActionResult> CreateReservation(ReservationVM reservationVM)
        {
            if (!ModelState.IsValid)
            {
                return View(new ReservationVM
                {
                    AllowSmoking = AllowSmoking.GetAllowSmokings(),
                    ButlerServices = ButlerService.GetButlerServices(),
                    AirConditions = AirCondition.GetAirConditions(),
                    YearListRange = GenerateYear.GetYear(),
                    MonthListRange = GenerateYear.GetMonth()
                });
            }
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
                   // reservationVM.YearListRange = GenerateYear.GetYear();
                    if (result > 0)
                    {
                        //ReservationVM rvm = new ReservationVM
                        //{
                        //    Name = "",
                        //    Content = "",
                        //    Price = 0,
                        //    RoomCount = 0,
                        //    AllowSmoking = AllowSmoking.GetAllowSmokings(),
                        //    ButlerServices = ButlerService.GetButlerServices(),
                        //    AirConditions = AirCondition.GetAirConditions(),
                        //};
                        BasicNotification("Reservation created", NotificationType.Error, "Successful!");
                        return RedirectToAction("Index", "Home");
                    }
                    reservationVM.AllowSmoking = AllowSmoking.GetAllowSmokings();
                    reservationVM.ButlerServices = ButlerService.GetButlerServices();
                    reservationVM.AirConditions = AirCondition.GetAirConditions();
                    return View(reservationVM);
                }
            }

            ModelState.AddModelError(string.Empty, "Unable to save reservation");
            reservationVM.AllowSmoking = AllowSmoking.GetAllowSmokings();
            reservationVM.ButlerServices = ButlerService.GetButlerServices();
            reservationVM.AirConditions = AirCondition.GetAirConditions();
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


        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PersonalProfile(DateClass model)
        {
            DateTimeVM dateTimeVM = new DateTimeVM();
            int DecisionParam = Convert.ToInt32(DecissionParameter.FormPage);
            var nDays = (Convert.ToDateTime(model.DateCheckOut) - DateTime.Now.Date).Days;
            var TotalAmount = (model.Price * nDays);

            if (model.ActionNane == "CreatePersonalProfile")
            {
                DecisionParam = Convert.ToInt32(DecissionParameter.PaymentPage);
                ViewBag.DecisionParam = DecisionParam;

                var jSonObj = _httpContext.HttpContext.Session.GetString("dateTimeVM");
                var dataTimeVM = JsonSerializer.Deserialize<DateTimeVM>(jSonObj);

                var user = await _reservationService.GetUserName(model.Email);

                BigProfileVM bigProfileVM2 = new BigProfileVM();
                bigProfileVM2.PaymentOption = new PaymentOption
                {
                    PaymentOptionId = dataTimeVM.Id,
                    Email = model.Email,
                    Name = $"{user.FirstName} {user.LastName}",
                    Amount = Convert.ToDecimal(TotalAmount)
                };

                return View(bigProfileVM2);
            }

            ViewBag.htmlText = "<h1>This is text from user with email: " + model.Email + " passed from PersonProfile action </h1>";
            //if (model.DateCheckOut is null || model.Email is null)
            //    return RedirectToAction("Index", "Home");

            BigProfileVM bigProfileVM = new BigProfileVM();
            //ViewBag.RecaptchaValue = _option;

            dateTimeVM.Email = model.Email;
            dateTimeVM.Id = model.Id;
            dateTimeVM.DateCheckOut = model.DateCheckOut;
            dateTimeVM.ImagePath = model.ImagePath;
            dateTimeVM.MonthName = Convert.ToDateTime(model.DateCheckOut).ToMonthName();
            dateTimeVM.RoomName = model.RoomName;
            dateTimeVM.Price = model.Price;
            dateTimeVM.CheckOut = Convert.ToDateTime(model.DateCheckOut);
            dateTimeVM.CheckIn = DateTime.Now;
            dateTimeVM.Days = nDays;
            dateTimeVM.RoomCount = (int)model.RoomCount;
            dateTimeVM.SiteKey = _option.SiteKey;
            dateTimeVM.TatalAmount = TotalAmount;
            ViewBag.Error = "Commence Captcha";
            ViewBag.RecaptchaValue = _option;
            ViewBag.DecisionParam = DecisionParam;
            ViewBag.Datetimevm = dateTimeVM;

            var jSonObject = JsonSerializer.Serialize<DateTimeVM>(dateTimeVM);
            _httpContext.HttpContext.Session.SetString("dateTimeVM", jSonObject);

            if (_reservationService.VerifyEmail(model.Email))
            {
                DecisionParam = Convert.ToInt32(DecissionParameter.ComplementoryPage);

                bigProfileVM.ComplementaryVM = new ComplementaryVM
                {
                    PersonalProfileId = (await _reservationService.GetUserName(model.Email)).Id,
                    CheckOut = model.DateCheckOut,
                    RoomId = model.Id,
                    Price = model.Price
                };
                ViewBag.DecisionParam = DecisionParam;
                return View(bigProfileVM);
            }

            bigProfileVM.PersonalProfileVMs = new PersonalProfileVM
            {
                Email = model.Email,
                Price = model.Price,
            };

            var userName = await _reservationService.GetUserName(model.Email);

            if (userName is not null)
            {
                bigProfileVM.PaymentOption = new PaymentOption();
                bigProfileVM.PaymentOption.PaymentOptionId = model.Id;
                bigProfileVM.PaymentOption.Email = model.Email;
                bigProfileVM.PaymentOption.Name = userName.FirstName + " " + userName.LastName;
                bigProfileVM.PaymentOption.Amount = Convert.ToDecimal(model.Price * nDays);
            }

            return View(bigProfileVM);
        }

        public async Task<ActionResult> CreatePersonalProfile(BigProfileVM bigProfileVM)
        {
            int DecisionParam = Convert.ToInt32(DecissionParameter.ComplementoryPage);

            string captchaResponse = Request.Form["g-recaptcha-response"].ToString();
            var validate = _helper.ValidateCaptcha(captchaResponse);
            if (!validate.Success)
            {
                bigProfileVM.PersonalProfileVMs.Error = "Finish captcha";
                return RedirectToAction("PersonalProfile", "Reservation");
            }

            var jSonObj = _httpContext.HttpContext.Session.GetString("dateTimeVM");
            var dataTimeVM = JsonSerializer.Deserialize<DateTimeVM>(jSonObj);


            if (bigProfileVM.ComplementaryVM is not null)
            {
                PersonalProfileRoom personalProfileRoom = new PersonalProfileRoom
                {
                    PersonalProfileId = bigProfileVM.ComplementaryVM.PersonalProfileId,
                    RoomId = bigProfileVM.ComplementaryVM.RoomId,
                    CheckOut = Convert.ToDateTime(bigProfileVM.ComplementaryVM.CheckOut),
                    CheckIn = DateTime.Now,
                    AdditionalRequirment = bigProfileVM.ComplementaryVM.AdditionalRequirement,
                    Purpose = bigProfileVM.ComplementaryVM.Purpose,
                    ProfileStatus = Convert.ToInt32(ProfileStatus.Open)
                };
                var response = await _reservationService.CreatePersonalProfileRoomAsync(personalProfileRoom, _dbContext);
                if (response > 0)
                {
                    var update = await _reservationService.UpdateReservationStatusAsync(dataTimeVM.Id, (int)ReservationStatusEnum.Requested, _dbContext);
                    if (update > 0)
                    {
                        DecisionParam = Convert.ToInt32(DecissionParameter.PaymentPage);
                        ViewBag.DecisionParam = DecisionParam;
                        return RedirectToAction("PersonalProfile", "Reservation",
                            new DateClass
                            {
                                ActionNane = "CreatePersonalProfile",
                                Email = dataTimeVM.Email,
                                Id = dataTimeVM.Id,
                                Price = bigProfileVM.ComplementaryVM.Price,
                                DateCheckOut = dataTimeVM.DateCheckOut
                            });
                    }
                    ViewBag.DecisionParam = DecisionParam;
                    return RedirectToAction("PersonalProfile", "Reservation");
                }
                return RedirectToAction("Payment");
            }



            bigProfileVM.PersonalProfileVMs.Email = dataTimeVM.Email;
            bigProfileVM.PersonalProfileVMs.RoomId = Convert.ToInt32(dataTimeVM.Id);

            if (bigProfileVM.PersonalProfileVMs is not null)
            {
                PersonalProfile personalProfile = new PersonalProfile
                {
                    Email = dataTimeVM.Email,
                    Phone = bigProfileVM.PersonalProfileVMs.Phone,
                    Occupation = bigProfileVM.PersonalProfileVMs.Occupation,
                    DateOfBirth = bigProfileVM.PersonalProfileVMs.DateOfBirth,
                    Sex = bigProfileVM.PersonalProfileVMs.Sex,
                    CreatedDate = DateTime.Now,
                    LastName = bigProfileVM.PersonalProfileVMs.LastName,
                    FirstName = bigProfileVM.PersonalProfileVMs.FirstName,
                    Term = bigProfileVM.PersonalProfileVMs.Term,
                    AddressLine1 = bigProfileVM.PersonalProfileVMs.AddressLine1,
                    AddressLine2 = bigProfileVM.PersonalProfileVMs.AddressLine2,
                    City = bigProfileVM.PersonalProfileVMs.City,
                    State = bigProfileVM.PersonalProfileVMs.State,
                    Purpose = bigProfileVM.PersonalProfileVMs.Purpose
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
                            RoomId = dataTimeVM.Id,
                            CheckIn = dataTimeVM.CheckIn,
                            CheckOut = dataTimeVM.CheckOut,
                            Purpose = bigProfileVM.PersonalProfileVMs.Purpose,
                            AdditionalRequirment = bigProfileVM.PersonalProfileVMs.AdditionalRequirement,
                            ArrivalTime = bigProfileVM.PersonalProfileVMs.ArrivalTime,
                            ProfileStatus = Convert.ToInt32(ProfileStatus.Open)
                        };
                        var response = await _reservationService.CreatePersonalProfileRoomAsync(personalProfileRoom, _dbContext);
                        if (response > 0)
                        {
                            var update = await _reservationService.UpdateReservationStatusAsync(dataTimeVM.Id, (int)ReservationStatusEnum.Requested, _dbContext);
                            if (update > 0)
                            {
                                await transaction.CommitAsync();
                                DecisionParam = Convert.ToInt32(DecissionParameter.PaymentPage);
                                ViewBag.DecisionParam = DecisionParam;

                                return RedirectToAction("PersonalProfile", "Reservation",
                                   new DateClass
                                   {
                                       ActionNane = "CreatePersonalProfile",
                                       Email = dataTimeVM.Email,
                                       Id = dataTimeVM.Id,
                                       Price = bigProfileVM.PersonalProfileVMs.Price,
                                       DateCheckOut = dataTimeVM.DateCheckOut
                                   });
                            }

                            ViewBag.DecisionParam = DecisionParam;
                            return RedirectToAction("PersonalProfile", "Reservation");
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
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

        public async Task<IActionResult> Payment(BigProfileVM model)
        {
            TransactionInitializeRequest request = new()
            {
                AmountInKobo = Convert.ToInt32(model.PaymentOption.Amount * 100),
                Email = model.PaymentOption.Email,
                Reference = Generate().ToString(),
                Currency = "NGN",
                CallbackUrl = "https://localhost:7068/Reservation/verify"
            };

            TransactionInitializeResponse response = Paystack.Transactions.Initialize(request);
            if (response.Status)
            {
                var transaction = new TransactionModel()
                {
                    Amount = Convert.ToInt32(model.PaymentOption.Amount),
                    Email = model.PaymentOption.Email,
                    TrxRef = request.Reference,
                    Name = model.PaymentOption.Name,
                    RoomId = model.PaymentOption.PaymentOptionId
                };
                await _dbContext.Transactions.AddAsync(transaction);
                await _dbContext.SaveChangesAsync();
                return Redirect(response.Data.AuthorizationUrl);
            }

            ViewData["error"] = response.Message;
            var msg = response.Message;
            BasicNotification(msg, NotificationType.Error, "Failed to complete payment!");
            return RedirectToAction("Index", "Home"); ;
        }

        public async Task<IActionResult> Verify(string reference)
        {
            TransactionVerifyResponse response = Paystack.Transactions.Verify(reference);
            if (response.Data.Status == "success")
            {
                var transaction = _dbContext.Transactions.Where(x => x.TrxRef == reference).FirstOrDefault();
                if (transaction != null)
                {
                    transaction.Status = true;
                    _dbContext.Transactions.Update(transaction);
                    await _dbContext.SaveChangesAsync();
                    var roomId = await _reservationService.GetRoomId(reference);
                    var update = await _reservationService.UpdateReservationStatusAsync(Convert.ToInt32(roomId), (int)ReservationStatusEnum.Booked, _dbContext);
                    BasicNotification("Thanks for your Patronage", NotificationType.Success, "Payment Successful!");
                    return RedirectToAction("Index", "Home");
                }
            }
            var resp = response.Data.GatewayResponse;
            ViewData["error"] = resp;
            BasicNotification(resp, NotificationType.Success, "Payment Failed!");
            return RedirectToAction("Index");
        }

        public static int Generate()
        {
            Random rand = new Random((int)DateTime.Now.Ticks);
            return rand.Next(100000000, 999999999);
        }

        public JsonResult SaveProducts(string selectedProducts)
        {
            var productList = selectedProducts.Split(',');
            return Json("", productList);
        }
     }

}






public class EditReservationVM : Reservation
{
    public IFormFile Image { get; set; }
}

