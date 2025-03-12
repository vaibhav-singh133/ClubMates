using ClubMates.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClubMates.Controllers
{
    public class DepartmentController : Controller
    {
        // GET: Department
        public ActionResult Index()
        {
            List<Department> departments = new List<Department>()
            {
                new Department() { DepartmentId = 1, DepartmentName = "IT" },
                new Department() { DepartmentId = 2, DepartmentName = "IOT" },
                new Department() { DepartmentId = 3, DepartmentName = "CSE" }
            };

            return View(departments);
        }
        public ActionResult AddDepartment()
        {
            return View(new Department());
        }
        [HttpPost]
        public ActionResult AddDepartment(Department department)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}