using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using ClubMates.Models;


namespace ClubMates.Db
{
	public class ApplicatioDbContext : DbContext
    {
        public ApplicatioDbContext() : base("DefaultConnection")
        {
           

        }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Department> Departments { get; set; }

    }
}