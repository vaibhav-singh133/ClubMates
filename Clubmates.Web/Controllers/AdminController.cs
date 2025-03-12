using Clubmates.Web.Models.AccountViewModel;
using Clubmates.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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

        public IActionResult ManageUsers()
        {
            var listofUserAccounts = _userManager.Users
                .Where(x => x.ClubmatesRole != ClubmatesRole.SuperAdmin)
                .Select(x => new UserViewModel
                {
                    Name = x.UserName,
                    EmailId = x.Email,
                    Role = Enum.GetName(x.ClubmatesRole)
                }).ToList();
            return View(listofUserAccounts);
        }
    }
}
