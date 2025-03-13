using Clubmates.Web.Models.AccountViewModel;
using Clubmates.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Clubmates.Web.Controllers
{
    public class AdminController(UserManager<ClubmatesUser> usermanager) : Controller
    {

        private readonly UserManager<ClubmatesUser> _userManager = usermanager;
        public IActionResult Index()
        {
            return View();

        }

        [Authorize(Policy = "MustbeSuperAdmin")]

        public async Task<IActionResult> ManageUsers()
        {
            return View(await GetUserstoManage());
        }
        private async Task<List<UserViewModel>> GetUserstoManage()
        {
            var listofUserAccounts = await _userManager.Users
                .Where(x => x.ClubmatesRole != ClubmatesRole.SuperAdmin)
                .Select(x => new UserViewModel
                {
                    Name = x.UserName,
                    EmailId = x.Email,
                    Role = x.ClubmatesRole
                }).ToListAsync();
            return listofUserAccounts;
        }
        public async Task<IActionResult> EditUser(string email)
        {
            var accountUser = await _userManager.FindByEmailAsync(email);
            if (accountUser != null)
            {
                UserViewModel userViewModel = new()
                {
                    Name = accountUser.UserName,
                    EmailId = accountUser.Email,
                    Role = accountUser.ClubmatesRole,
                    Roles = Enum.GetValues<ClubmatesRole>().Select(x => new SelectListItem
                    {
                        Text = Enum.GetName(x),
                        Value = x.ToString()
                    })
                };
                return View(userViewModel);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> EditUser(UserViewModel userViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(userViewModel);
            }
            else
            {
                try
                {
                    ClubmatesUser? clubmatesUser = await _userManager.FindByEmailAsync(userViewModel.EmailId ?? "");
                    if (clubmatesUser != null)
                    {
                        clubmatesUser.ClubmatesRole = userViewModel.Role;
                        var claims = await _userManager.GetClaimsAsync(clubmatesUser);
                        var RemoveResult = await _userManager.RemoveClaimsAsync(clubmatesUser, claims);
                        if (!RemoveResult.Succeeded)
                        {
                            ModelState.AddModelError("", "Failed to update user claims, Removing claim failed");
                            return View(userViewModel);
                        }
                        var claimRequired = new List<Claim>
                            {
                                new (ClaimTypes.Name, userViewModel.Name ?? ""),
                                new (ClaimTypes.Role, Enum.GetName(userViewModel.Role ?? ClubmatesRole.User) ?? ""),
                                new (ClaimTypes.Email, userViewModel.EmailId ?? ""),
                                new (ClaimTypes.NameIdentifier, clubmatesUser.Id)
                            };

                        var AddClaims = await _userManager.AddClaimsAsync(clubmatesUser, claimRequired);
                        if (!AddClaims.Succeeded)
                        {
                            ModelState.AddModelError("", "Failed to update user claims, Adding claim failed");
                            return View(userViewModel);
                        }
                        var UserUpdatedResult = await _userManager.UpdateAsync(clubmatesUser);
                        if (!UserUpdatedResult.Succeeded)
                        {
                            ModelState.AddModelError("", "Failed to update user");
                            return View(userViewModel);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    return View(userViewModel);
                }
                return RedirectToAction("ManageUsers", await GetUserstoManage());
            }

        }
        public async Task<IActionResult> DeleteUser(string email)
        {
            var accountUser = await _userManager.FindByEmailAsync(email);
            if (accountUser != null)
            {
                await _userManager.DeleteAsync(accountUser);
                return View("ManageUsers", await GetUserstoManage());
            }
            return NotFound();
        }

        public IActionResult CreateUser()
        {
            return View(new CreateUserViewModel());

        }
        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (model != null && model.EmailId != null && !model.EmailId.Equals(model.ConfirmEmailId))
            {
                ModelState.AddModelError("", "Email and Confirm Email do not match");
                return View(model);
            }
            if (model != null && model.Password != null && !model.Password.Equals(model.ConfirmPassword))
            {
                ModelState.AddModelError("", "Password and Confirm Password do not match");
                return View(model);
            }
            if (model != null)
                if (model != null)
                {
                    ClubmatesUser clubMatesUser = new ClubmatesUser
                    {
                        UserName = model.Name,
                        Email = model.EmailId,
                        ClubmatesRole = model.Role
                    };
                    var createduser = await _userManager.CreateAsync(clubMatesUser, model.Password ?? "Password-1");
                    if (!createduser.Succeeded)
                    {
                        foreach (var error in createduser.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                        return View(model);
                    }
                    List<Claim> claims = new()
                {
                    new(ClaimTypes.Name, model.Name ?? ""),
                    new(ClaimTypes.Email, model.EmailId ?? ""),
                    new(ClaimTypes.Role, Enum.GetName(model.Role) ?? ""),
                    new(ClaimTypes.NameIdentifier, clubMatesUser.Id)
                };
                    return View("ManageUsers", await GetUserstoManage());
                }
            return View(new CreateUserViewModel());

        }
    }
}
