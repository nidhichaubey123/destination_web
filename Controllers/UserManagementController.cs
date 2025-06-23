using Microsoft.AspNetCore.Mvc;

namespace DMCPortal.Web.Controllers
{
    public class UserManagementController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
