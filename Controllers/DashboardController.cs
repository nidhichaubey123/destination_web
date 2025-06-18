using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
public class DashboardController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult AddUser()
    {
        return RedirectToAction("Index", "Register");
    }
}
