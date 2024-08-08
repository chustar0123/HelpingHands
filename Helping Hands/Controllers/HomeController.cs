using Helping_Hands.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Helping_Hands.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Grp0410HelpingHandsContext _dbContext;

        public HomeController(ILogger<HomeController> logger, Grp0410HelpingHandsContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }
        public IActionResult ErrorPage()
        {
            var errorMessage = TempData["ErrorMessage"] as string;
            if (string.IsNullOrEmpty(errorMessage))
            {
                errorMessage = "An error occurred.";
            }

            ViewBag.ErrorMessage = errorMessage;
            return View();
        }
        public IActionResult Index()
        {
            var businesses = _dbContext.Businesses.ToList();
            return View(businesses);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}