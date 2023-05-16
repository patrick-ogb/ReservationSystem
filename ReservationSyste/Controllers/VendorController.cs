using Microsoft.AspNetCore.Mvc;
using RestSharp;

namespace ReservationSyste.Controllers
{
    public class VendorController : Controller
    {
        private readonly IConfiguration _configuration;

        public VendorController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IActionResult Index()
        {

            return View();
        }



    }

}
