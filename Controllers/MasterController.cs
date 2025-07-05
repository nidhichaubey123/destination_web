using Microsoft.AspNetCore.Mvc;

namespace DMCPortal.Web.Controllers
{
    public class MasterController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
