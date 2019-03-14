using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AllAboutLogging.Models;
using Microsoft.Extensions.Logging;

namespace AllAboutLogging.Controllers
{
    public class HomeController : Controller
    {

        private ILogger<HomeController> _logger;
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Login()
        {
            _logger.LogInformation("User request to load the Login Page");
            return View();
        }

        public IActionResult Login(LoginModel model)
        {
            _logger.LogInformation("Requested login  for {@User} from address {IpAddress}", model, Request.HttpContext.Connection.RemoteIpAddress);
            return View();
        }
        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
