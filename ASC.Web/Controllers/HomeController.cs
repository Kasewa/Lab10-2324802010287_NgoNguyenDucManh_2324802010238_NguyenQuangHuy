using ASC.Utilities;
using ASC.Web.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ASC.Web.Controllers
{
    public class HomeController : AnonymousController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IOptions<ApplicationSettings> _settings;

        public HomeController(ILogger<HomeController> logger, IOptions<ApplicationSettings> settings)
        {
            _logger = logger;
            _settings = settings;
        }

        public IActionResult Index()
        {
            HttpContext.Session.SetObjectAsJson("Test", _settings.Value);
            ViewBag.Title = _settings.Value.ApplicationTitle;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
