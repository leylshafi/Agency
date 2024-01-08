using Agency.Areas.Admin.ViewModels;
using Agency.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Agency.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AuthController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        public AuthController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid) return View();
            AppUser user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == registerVM.UserName);
            if (user is not null)
            {
                ModelState.AddModelError("UserName", "This user already exists");
                return View();
            }
            user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == registerVM.Email);
            if (user is not null)
            {
                ModelState.AddModelError("Email", "This user already exists");
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
            return RedirectToAction("Login");
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM, string? returnUrl)
        {
            if (!ModelState.IsValid) return View();
            AppUser user = await _userManager.FindByNameAsync(loginVM.UsernameOrEmail);
            if (user is null)
            {
                user = await _userManager.FindByEmailAsync(loginVM.UsernameOrEmail);
                if (user is null)
                {
                    ModelState.AddModelError("UsernameOrEmail", "Username, Email or Password is wrong");
                    return View();
                }
            }
            var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, true, false);
            if (result.IsLockedOut)
            {
                ModelState.AddModelError(String.Empty, "Too many attempts, try again later");
                return View();
            }
            if (!result.Succeeded)
            {
                ModelState.AddModelError(String.Empty, "Username, Email or Password is incorrect");
                return View();
            }
            if (returnUrl is null)
            {
                return Redirect("https://localhost:7271/");
            }
            return Redirect(returnUrl);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Redirect("https://localhost:7271/");
        }
    }
}

