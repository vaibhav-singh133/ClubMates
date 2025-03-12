using ClubMates.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static System.Net.Mime.MediaTypeNames;

namespace ClubMates.Controllers
{
    public class StudentController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            Student student = new Student()
            {
                StudentId = 1,
                StudentName = "John",
                DateOfBirth = new DateTime(1990, 1, 1),
                Height = 1.75m,
                weight = 68
            };

            List<Student> students = new List<Student>()
                {
                    new Student() { StudentId = 1, StudentName = "Vaibhav Raj Singh", DateOfBirth = new DateTime(1990, 1, 1), Height = 1.75m, weight = 68 },
                    new Student() { StudentId = 2, StudentName = "Steve", DateOfBirth = new DateTime(1992, 1, 1), Height = 1.80m, weight = 75 },
                    new Student() { StudentId = 3, StudentName = "Bill", DateOfBirth = new DateTime(1993, 1, 1), Height = 1.85m, weight = 80 }
                };

            return View(students);
        }

        public ActionResult AddStudent()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddStudent(Student student)
        {
            if (ModelState.IsValid)
            {
                
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}