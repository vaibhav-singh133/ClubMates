using Clubmates.Web.Models.AccountViewModel;
using Clubmates.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Clubmates.Web.AppDbContext;
using Clubmates.Web.Models.AdminViewModel;

namespace Clubmates.Web.Controllers
{
    [Authorize(Policy ="MustbeSuperAdmin")]
    public class AdminController(UserManager<ClubmatesUser> usermanager, AppIdentityDbContext dbContext) : Controller
    {

        private readonly UserManager<ClubmatesUser> _userManager = usermanager;
        private readonly AppIdentityDbContext _dbContext = dbContext;
        public IActionResult Index()
        {
            return View();

        }

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
        public async Task<IActionResult> ManageClubs()
        {
            var listOfClubs = await _dbContext.Clubs
                .Include(x => x.ClubManager)
                .ToListAsync();
            List<ClubViewModel> clubViewModels = listOfClubs.Select(x => new ClubViewModel
            {
                ClubId = x.ClubId,
                ClubName = x.ClubName,
                ClubEmail = x.ClubEmail,
                ClubCategory = x.ClubCategory,
                ClubType = x.ClubType,
                ClubRules = x.ClubRules,
                ClubManager = x.ClubManager?.Email,
                ClubContactNumber = x.ClubContactNumber,
                ClubDescription = x.ClubDescription
            }).ToList();
            return View(clubViewModels);
        }

        public IActionResult CreateClub()
        {
            return View(new ClubViewModel());
        }
        [HttpPost]
        public async Task<IActionResult> CreateClub(ClubViewModel clubViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(clubViewModel);
            }
            try
            {
                Club club = new()
                {
                    ClubName = clubViewModel.ClubName,
                    ClubDescription = clubViewModel.ClubDescription,
                    ClubCategory = clubViewModel.ClubCategory,
                    ClubType = clubViewModel.ClubType,
                    ClubRules = clubViewModel.ClubRules,
                    ClubContactNumber = clubViewModel.ClubContactNumber,
                    ClubEmail = clubViewModel.ClubEmail,
                    ClubManager = await _userManager.FindByEmailAsync(clubViewModel.ClubManager ?? "")

                };
                _dbContext.Clubs.Add(club);
                await _dbContext.SaveChangesAsync();
                return RedirectToAction("ManageClubs");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(clubViewModel);
            }
        }
        public async Task<IActionResult> EditClub(int clubId)
        {
            var club = await _dbContext
                .Clubs
                .Include(x => x.ClubManager)
                .FirstOrDefaultAsync(x => x.ClubId == clubId);
            if (club != null)
            {
                ClubViewModel clubViewModel = new()
                {
                    ClubId = club.ClubId,
                    ClubName = club.ClubName,
                    ClubDescription = club.ClubDescription,
                    ClubCategory = club.ClubCategory,
                    ClubType = club.ClubType,
                    ClubRules = club.ClubRules,
                    ClubContactNumber = club.ClubContactNumber,
                    ClubEmail = club.ClubEmail,
                    ClubManager = club.ClubManager?.Email

                };
                return View(clubViewModel);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> EditClub(ClubViewModel clubViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(clubViewModel);
            }
            try
            {
                var club = await _dbContext.Clubs.FindAsync(clubViewModel.ClubId);
                if (club != null)
                {
                    club.ClubName = clubViewModel.ClubName;
                    club.ClubDescription = clubViewModel.ClubDescription;
                    club.ClubCategory = clubViewModel.ClubCategory;
                    club.ClubType = clubViewModel.ClubType;
                    club.ClubRules = clubViewModel.ClubRules;
                    club.ClubContactNumber = clubViewModel.ClubContactNumber;
                    club.ClubEmail = clubViewModel.ClubEmail;
                    club.ClubManager = await _userManager.FindByEmailAsync(clubViewModel.ClubManager ?? "");
                    await _dbContext.SaveChangesAsync();
                    return RedirectToAction("ManageClubs");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(clubViewModel);
            }
            return NotFound();
        }
        public async Task<IActionResult> DeleteClub(int clubId)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("ManageClubs");
            }
            var club = await _dbContext.Clubs.FindAsync(clubId);
            if (club != null)
            {
                var clubAccesses = await _dbContext.ClubAccesses.Where(x => x.Club == club).ToListAsync();

                if (clubAccesses != null && clubAccesses.Count > 0)
                {
                    foreach (var clubAccess in clubAccesses)
                    {
                        _dbContext.Remove(clubAccess);
                    }
                }
                _dbContext.Clubs.Remove(club);
                await _dbContext.SaveChangesAsync();
                return RedirectToAction("ManageClubs");
            }
            return NotFound();
        }

    }
}
