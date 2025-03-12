using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ClubMates.Models
{
	public class Teacher
	{
        [Key]
		public int TeacherId { get; set; }
        public string TeacherName { get; set; }
        public string TeacherEmail { get; set; }
        public double TeacherPhone { get; set; }

    }
}