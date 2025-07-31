using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VnptSmsBrandName.Models;
using VnptSmsBrandName.Models.Identity;
using VnptSmsBrandName.ViewModel;
using System.Diagnostics;

namespace VnptSmsBrandName.Controllers
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
