using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clubmates.Web.Controllers
{
    public class ClubsController : Controller
    {
        [Authorize(Policy = "MustbeGuest")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
