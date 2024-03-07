using System;
using System.Data;
using System.Threading.Tasks;
using Captivlink.Infrastructure.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Captivlink.Api.Identity.Account
{
    [AllowAnonymous]
    public class RegisterController : Controller
    {

        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public RegisterController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            this._signInManager = signInManager;
            this._userManager = userManager;
        }

        public IActionResult Setup()
        {
            return View("RegisterSetup");
        }

        [HttpGet]
        public IActionResult Details(string role)
        {
            if (role.ToLower() != "contentcreator" && role.ToLower() != "business")
            {
                return RedirectToAction("Setup");
            }

            return View("RegisterDetails", new RegisterInputModel() {Role = role});
        }

        [HttpPost]
        public async Task<IActionResult> Details(RegisterInputModel model)
        {
            if (model.Role.ToLower() != "contentcreator" && model.Role.ToLower() != "business")
            {
                return RedirectToAction("Setup");
            }

            if (ModelState.IsValid)
            {
                var result = await RegisterUserInternalAsync(model);
                if (result.Succeeded)
                {
                    
                    return Redirect(Program.Application.FrontendUrl);
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View("RegisterDetails", model);
        }

        public async Task<IdentityResult> RegisterUserInternalAsync(RegisterInputModel request)
        {
            if (await _userManager.Users.AnyAsync(x => x.NormalizedEmail == request.Email.ToUpperInvariant()))
            {
                return IdentityResult.Failed(new IdentityError { Description = "User with that email already exists" });
            }

            return await CreateUserInternalAsync(request);
        }

        private async Task<IdentityResult> CreateUserInternalAsync(RegisterInputModel request)
        {
            var user = new ApplicationUser
            {
                UserName = request.Email?.Trim(),
                Email = request.Email?.Trim(),
                EmailConfirmed = true,
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, request.Role);
                await _signInManager.SignInAsync(user, false, "Register");
            }

            return result;
        }
    }
}
