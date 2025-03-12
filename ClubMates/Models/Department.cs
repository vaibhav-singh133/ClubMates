using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ClubMates.Models
{
    public class Department
    {
        [Key]
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
    }
}
