using Microsoft.AspNetCore.Mvc;

namespace WebhookInbox.Mvc.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
