using ePharma_asp_mvc.Data;
using ePharma_asp_mvc.Data.ViewModels;
using ePharma_asp_mvc.Models;
using ePharma_asp_mvc.Models.Static;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ePharma_asp_mvc.Controllers
{
    public class ApplicationUserController : Controller
    {
        private readonly AppDbContext _context;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly SignInManager<ApplicationUser> _signInManager;

        public ApplicationUserController(
            AppDbContext context,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // ============================
        // Users List
        // ============================

        [Authorize(Roles = UsersRoles.Admin)]
        public async Task<IActionResult> Index()
        {
            var data = await _context.Users
                .AsNoTracking()
                .ToListAsync();

            return View(data);
        }

        // ============================
        // Login
        // ============================

        [HttpGet]
        public IActionResult LogIn(string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;

            return View(new LogInViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogIn(
            LogInViewModel logInData,
            string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(logInData);
            }

            var user = await _userManager
                .FindByEmailAsync(logInData.EmailAddress);

            if (user == null)
            {
                TempData["Error"] = "Invalid email or password";
                return View(logInData);
            }

            var passwordCheck = await _userManager
                .CheckPasswordAsync(user, logInData.Password);

            if (!passwordCheck)
            {
                TempData["Error"] = "Invalid email or password";
                return View(logInData);
            }

            var result = await _signInManager
                .PasswordSignInAsync(
                    user,
                    logInData.Password,
                    false,
                    false);

            if (result.Succeeded)
            {
                // لو فيه returnUrl
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                // Check Roles
                if (await _userManager.IsInRoleAsync(user, UsersRoles.Admin))
                {
                    return RedirectToAction("Index", "Admin");
                }

                if (await _userManager.IsInRoleAsync(user, UsersRoles.User))
                {
                    return RedirectToAction("Index", "Home");
                }

                // Default
                return RedirectToAction("Index", "Home");
            }

            TempData["Error"] = "Invalid email or password";

            return View(logInData);
        }

        // ============================
        // Register
        // ============================

        [HttpGet]
        public IActionResult SignUp()
        {
            return View(new SignUpViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp(SignUpViewModel registerData)
        {
            if (!ModelState.IsValid)
            {
                return View(registerData);
            }

            // Check Existing Email
            var existingUser = await _userManager
                .FindByEmailAsync(registerData.EmailAddress);

            if (existingUser != null)
            {
                TempData["Error"] = "This email already exists";

                return View(registerData);
            }

            // Create User
            var newUser = new ApplicationUser
            {
                UserName = registerData.EmailAddress,
                Email = registerData.EmailAddress,
                FirstName = registerData.FirstName,
                LastName = registerData.LastName,
                Address = registerData.Address,
                PhoneNumber = registerData.PhoneNumber
            };

            var response = await _userManager
                .CreateAsync(newUser, registerData.Password);

            if (response.Succeeded)
            {
                await _userManager
                    .AddToRoleAsync(newUser, UsersRoles.User);

                return View("Login");
            }

            foreach (var error in response.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(registerData);
        }

        // ============================
        // Logout
        // ============================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }
    }
}