using Agency.Areas.Admin.ViewModels;
using Agency.Models;
using Agency.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Agency.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AuthController : Controller
    { 
        private readonly UserManager<AppUser> _userManager;

        public AuthController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid) return View();
            AppUser user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == registerVM.UserName || u.Email == registerVM.Email);
            if (user is not null)
            {
                ModelState.AddModelError("UserName", "This user already exists");
                return View();
            }
            user = new()
            {
                UserName = registerVM.UserName,
                Email = registerVM.Email,
                Name = registerVM.Name,
                Surname = registerVM.Surname,
            };
            var result = await _userManager.CreateAsync(user, registerVM.Password);
            if (!result.Succeeded)
            {
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError(String.Empty, error.Description);
                }
                return View();
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
