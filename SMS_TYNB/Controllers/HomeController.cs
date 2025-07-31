using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SMS_TYNB.Models;
using SMS_TYNB.Models.Identity;
using SMS_TYNB.ViewModel;
using System.Diagnostics;

namespace SMS_TYNB.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        UserManager<Users> _userManager;


		public HomeController(ILogger<HomeController> logger, UserManager<Users> userManager)
        {
            _logger = logger;
            _userManager = userManager;

		}

        public IActionResult Index()
        {
			return View();
        }
    }
}
