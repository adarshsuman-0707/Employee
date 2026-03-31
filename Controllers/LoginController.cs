using Microsoft.AspNetCore.Mvc;
using EmployeeManagement.Models;

namespace EmployeeManagement.Controllers
{
    public class LoginController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(Login model)
        {
       
            if (model.Username == "admin" && model.Password == "123456789")
            {
                HttpContext.Session.SetString("Username", model.Username);
                return RedirectToAction("Index", "Employee");
            }

          
            ViewBag.ErrorMessage = "Invalid Username or Password!";
            return View(model);
        }
       
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("Username");
            return RedirectToAction("Index", "Home");

        }
    }
}
