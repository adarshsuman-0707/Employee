using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Controllers
{
    public class PracticeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
