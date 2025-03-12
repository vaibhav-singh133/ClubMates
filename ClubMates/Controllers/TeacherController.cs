using ClubMates.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClubMates.Controllers
{
    public class TeacherController : Controller
    {
        // GET: Teacher
        public ActionResult Index()
        {
          List<Teacher> teachers = new List<Teacher>()
                {
                new Teacher() { TeacherId = 1, TeacherName = "Ramu", TeacherEmail = "Ramu@gmail.com", TeacherPhone = 9937448739},
                new Teacher() { TeacherId = 2, TeacherName = "Neha", TeacherEmail = "Neha@gmail.com", TeacherPhone = 9933445566},
                new Teacher() { TeacherId = 3, TeacherName = "adesh", TeacherEmail = "Adesh@gmail.com", TeacherPhone = 9931118739 }
                };
            return View(teachers);
        }
        public ActionResult AddTeacher()
        {
            return View();
        }
    }
}