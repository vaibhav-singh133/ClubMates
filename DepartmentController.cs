using ClubMates.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace ClubMates.Controllers
{
    public class DepartmentController : Controller
    {
        private static List<Department> departments = new List<Department>();

        // GET: Department
        public ActionResult Index()
        {
            return View(departments);
        }

        // GET: Department/AddDepartment
        public ActionResult AddDepartment()
        {
            return View();
        }

        // POST: Department/AddDepartment
        [HttpPost]
        public ActionResult AddDepartment(Department department)
        {
            if (ModelState.IsValid)
            {
                departments.Add(department);
                return RedirectToAction("Index");
            }
            return View(department);
        }
    }
}
