using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClubMates.Controllers
{
    public class HomeController : Controller
    {
        // GET: Student
        public ActionResult Index()
        {
            return View();
        }
    }
}