using KeycloakExtension;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OnlineStore.Areas.Administrator.Controllers;

[Authorize(Roles = $"{RoleConstants.Root},{RoleConstants.Employee}")]
[Area("Administrator")]
public class HomeController : Controller
{

    public IActionResult Index()
    {
        return View();
    }
}