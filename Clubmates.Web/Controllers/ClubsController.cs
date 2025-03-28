using Clubmates.Web.AppDbContext;
using Clubmates.Web.Models;
using Clubmates.Web.Models.AdminViewModel;
using Clubmates.Web.Models.ClubViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace Clubmates.Web.Controllers
{
    [Authorize(Policy = "MustbeGuest")]
    public class ClubsController(AppIdentityDbContext dbContext, UserManager<ClubmatesUser> userManager) : Controller
    {
        private readonly AppIdentityDbContext _dbContext = dbContext;
        private readonly UserManager<ClubmatesUser> _userManager = userManager;
        public async Task<IActionResult> Index()
        {
            var clubs = await _dbContext.Clubs.Include(x => x.ClubManager)
                                 .ToListAsync();
            var listofClubs = clubs.Select(x => new CustomerClubViewModel
            {
                ClubId = x.ClubId,
                ClubName = x.ClubName,
                ClubEmail = x.ClubEmail,
                ClubCategory = x.ClubCategory,
                ClubType = x.ClubType,
                ClubRules = x.ClubRules,
                ClubManager = x.ClubManager?.UserName,
                ClubContactNumber = x.ClubContactNumber,
                ClubDescription = x.ClubDescription,
                ClubBanner = x.ClubBanner,
                ClubLogo = x.ClubLogo,
                ClubBackground = x.ClubBackground

            }).ToList();
            return View(listofClubs);
        }
        public async Task<IActionResult> ClubDetails(int clubId)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }
            var clubDetails = await _dbContext.Clubs.Include(x => x.ClubManager)
                                .FirstOrDefaultAsync(x => x.ClubId == clubId);
            if (clubDetails == null)
            {
                return RedirectToAction("Index");
            }
            var clubDetailsViewModel = new CustomerClubViewModel()
            {
                ClubName = clubDetails.ClubName,
                ClubEmail = clubDetails.ClubEmail,
                ClubCategory = clubDetails.ClubCategory,
                ClubType = clubDetails.ClubType,
                ClubRules = clubDetails.ClubRules,
                ClubManager = clubDetails.ClubManager?.UserName,
                ClubContactNumber = clubDetails.ClubContactNumber,
                ClubDescription = clubDetails.ClubDescription,
                ClubLogo = clubDetails.ClubLogo,
                ClubBanner = clubDetails.ClubBanner,
                ClubBackground = clubDetails.ClubBackground
            };
            return View(clubDetailsViewModel);
        }
        public IActionResult CreateClubForCustomer()
        {
            return View(new CustomerClubViewModel());
        }
        [HttpPost]
        public async Task<IActionResult> CreateClubForCustomer(CustomerClubViewModel customerClubViewModel, IFormFile clublogo, IFormFile clubBanner, IFormFile clubBackground)
        {
            if (!ModelState.IsValid)
            {
                return View(customerClubViewModel);
            }
            //who is the Logged in User
            var loggedInUserEmail = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
            //get that user from the database
            if (loggedInUserEmail == null)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = "/Clubs/CreateClubForCustomer" });
            }
            var loggedInUser = await _userManager.FindByEmailAsync(loggedInUserEmail);
            if (customerClubViewModel != null && loggedInUser != null)
            {
                Club club = new()
                {
                    ClubName = customerClubViewModel.ClubName,
                    ClubDescription = customerClubViewModel.ClubDescription,
                    ClubCategory = customerClubViewModel.ClubCategory,
                    ClubType = customerClubViewModel.ClubType,
                    ClubRules = customerClubViewModel.ClubRules,
                    ClubContactNumber = customerClubViewModel.ClubContactNumber,
                    ClubManager = loggedInUser,
                    ClubEmail = customerClubViewModel.ClubEmail
                };
                if (clublogo != null)
                {
                    using var memoryStream = new MemoryStream();
                    await clublogo.CopyToAsync(memoryStream);
                    club.ClubLogo = memoryStream.ToArray();
                }
                if (clubBanner != null)
                {
                    using var memoryStream = new MemoryStream();
                    await clubBanner.CopyToAsync(memoryStream);
                    club.ClubBanner = memoryStream.ToArray();
                }
                if (clubBackground != null)
                {
                    using var memoryStream = new MemoryStream();
                    await clubBackground.CopyToAsync(memoryStream);
                    club.ClubBackground = memoryStream.ToArray();
                }
                var createClubEntity = _dbContext.Clubs.Add(club);
                await _dbContext.SaveChangesAsync();

                if (createClubEntity != null)
                {
                    var createClub = await _dbContext.Clubs.FindAsync(createClubEntity.Entity.ClubId);

                    if (createClub != null)
                    {
                        bool isClubRoleAvaiable = false;
                        if(await _userManager.GetClaimsAsync(loggedInUser) != null)
                        {
                            var userClaims = await _userManager.GetClaimsAsync(loggedInUser);
                            foreach (var claim in userClaims)
                            {
                                if (claim.Value == Enum.GetName(ClubmatesRole.ClubUser))
                                {
                                    isClubRoleAvaiable = true;
                                }
                            }
                            if(!isClubRoleAvaiable)
                            {
                                await _userManager.AddClaimAsync(loggedInUser, new(ClaimTypes.Role, Enum.GetName(ClubmatesRole.ClubUser) ?? ""));
                            }
                        }
                        loggedInUser.ClubmatesRole = ClubmatesRole.ClubUser;
                        //save the user
                        await _userManager.UpdateAsync(loggedInUser);
                        _dbContext.ClubAccesses.Add(new ClubAccess
                        {
                            Club = createClub,
                            ClubmatesUser = loggedInUser,
                            ClubAccessRole = ClubAccessRole.ClubManager
                        });
                        await _dbContext.SaveChangesAsync();
                    }
                }

                return RedirectToAction("Index");
            }
            return View(customerClubViewModel);
        }
    }
}
