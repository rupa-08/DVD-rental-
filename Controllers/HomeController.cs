using AppDevGroupCoursework.Models;
using AppDevGroupCoursework.Request;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AppDevGroupCoursework.Controllers
{
    public class HomeController : Controller
    {
        private readonly DatabaseContext dataBaseContext;
        private readonly ILogger<HomeController> _logger;

        public object Session { get; private set; }

        public HomeController(ILogger<HomeController> logger, DatabaseContext db)
        {
            _logger = logger;
            dataBaseContext = db;
        }
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginRequest objUser)
        {
            if (ModelState.IsValid)
            {
                var obj = dataBaseContext.User.Where(a => a.username.Equals(objUser.Username) && a.password.Equals(objUser.Password)).FirstOrDefault();
                if (obj != null)
                {
                    return RedirectToAction("Index");
                }
            }
            return View(objUser);
        }
        public IActionResult Index()
        {
            return View();
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