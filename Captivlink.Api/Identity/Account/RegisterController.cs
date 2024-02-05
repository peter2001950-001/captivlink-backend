using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Captivlink.Api.Identity.Account
{
    [AllowAnonymous]
    public class RegisterController : Controller
    {
        public IActionResult Setup()
        {
            return View();
        }
    }
}
