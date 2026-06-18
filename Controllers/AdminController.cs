using Microsoft.AspNetCore.Mvc;

namespace EBookStore.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserType") == "Admin" || HttpContext.Session.GetString("UserType") == "Viewer") { }
            else
                return RedirectToAction("Index", "Home");

            return View();
        }
    }
}
