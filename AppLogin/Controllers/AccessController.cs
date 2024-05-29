using Microsoft.AspNetCore.Mvc;
using AppLogin.Data;
using AppLogin.Models;
using Microsoft.EntityFrameworkCore;
using AppLogin.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;


namespace AppLogin.Controllers
{
    public class AccessController : Controller
    {
        private readonly AppDbContext _context;
        public AccessController(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }

        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(UserVM userModel)
        {
            if (userModel.Password != userModel.ConfirmPasword)
            {
                ViewData["Message"] = "The passwords are different";
                return View();
            }
            //creating object
            User user = new User()
            {
                Name = userModel.Name,
                LastName = userModel.LastName,
                Email = userModel.Email,
                Password = userModel.Password
            };
            //saving changes on DB
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            if (user.Id != 0)
            {
                return RedirectToAction("Login","Access");
            }

            ViewData["Message"] = "User can not be created. Fatal Error :(";
            return View();
        }


        [HttpGet]
        public IActionResult LogIn()
        {
            if (User.Identity!.IsAuthenticated) return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LogIn(LoginVM userModel)
        {
            User? userCatch = await _context.Users
                .Where(u => 
                        u.Email == userModel.Email &&
                        u.Password == userModel.Password
                ).FirstOrDefaultAsync();

            if (userCatch == null) 
            {
                ViewData["Message"] = "User not found :(";
                return View();
            }

            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, userCatch.Name)
            };
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims,CookieAuthenticationDefaults.AuthenticationScheme);
            AuthenticationProperties properties = new AuthenticationProperties()
            {
                AllowRefresh = true
            };
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                properties
                );

            return RedirectToAction("Index", "Home");
        }
    }
}
