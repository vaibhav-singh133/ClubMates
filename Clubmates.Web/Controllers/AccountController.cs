﻿using Clubmates.Web.Models;
using Clubmates.Web.Models.AccountViewModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Clubmates.Web.Controllers
{
    public class AccountController(UserManager<ClubmatesUser> userManager) : Controller
    {
        private readonly UserManager<ClubmatesUser> _userManager = userManager;
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(registerViewModel);
            }
            if (registerViewModel.Password != registerViewModel.ConfirmPassword)
            {
                ModelState.AddModelError("Password", "Password and Confirm Password do not match");
                return View(registerViewModel);
            }
            ClubmatesUser user = new()
            {
                UserName = registerViewModel.FullName,
                Email = registerViewModel.Email,
                ClubmatesRole = ClubmatesRole.Guest
            };
            var result = await _userManager.CreateAsync(user, registerViewModel.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(registerViewModel);
            }
            else
            {
                var claims = new List<Claim>
                {
                    new (ClaimTypes.Name, registerViewModel.FullName),
                    new (ClaimTypes.Email, user.Email ?? string.Empty),
                    new (ClaimTypes.NameIdentifier, user.Id),
                    new (ClaimTypes.Role, "Guest")
                };
                var claimsResult = await _userManager.AddClaimsAsync(user, claims);
                if (!claimsResult.Succeeded)
                {
                    foreach (var error in claimsResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(registerViewModel);
                }
            }

            return View("Success");
        }
        public IActionResult Login(string returnUrl = "/")
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel());
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel , string ReturnUrl = "/")
        {
            if (!ModelState.IsValid)
            {
                return View(loginViewModel);
            }
            var user = await _userManager.FindByNameAsync(loginViewModel.UserName);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
                return View(loginViewModel);
            }
            var result = await _userManager.CheckPasswordAsync(user, loginViewModel.Password);
            if (!result)
            {
                ModelState.AddModelError(string.Empty, "Invalid Login Attempt, Password is Incorrect");
                return View(loginViewModel);
            }
            else
            {
                var claims = user != null ? await _userManager.GetClaimsAsync(user) : null;
                if (claims != null)
                {
                    var scheme = IdentityConstants.ApplicationScheme;
                    var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, scheme));
                    var AuthenticationProperties = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(20)
                    };
                    await HttpContext.SignInAsync(scheme, principal , AuthenticationProperties);
                    return Redirect(ReturnUrl);
                }
            }
            return View();
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
       
    }
}