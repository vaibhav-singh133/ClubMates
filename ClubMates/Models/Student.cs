using System;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ClubMates.Models
{
	public class Student 
	{
        [Key]

        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public decimal Height { get; set; }
        public int weight { get; set; }

        public int Age
        {
            get
            {
                return DateTime.Now.Year - DateOfBirth.Year;
            }
        }


    }
}