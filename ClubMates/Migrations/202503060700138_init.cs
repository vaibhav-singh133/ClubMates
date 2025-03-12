namespace ClubMates.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Departments",
                c => new
                    {
                        DepartmentId = c.Int(nullable: false, identity: true),
                        DepartmentName = c.String(),
                    })
                .PrimaryKey(t => t.DepartmentId);
            
            CreateTable(
                "dbo.Students",
                c => new
                    {
                        StudentId = c.Int(nullable: false, identity: true),
                        StudentName = c.String(),
                        DateOfBirth = c.DateTime(nullable: false),
                        Height = c.Decimal(nullable: false, precision: 18, scale: 2),
                        weight = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.StudentId);
            
            CreateTable(
                "dbo.Teachers",
                c => new
                    {
                        TeacherId = c.Int(nullable: false, identity: true),
                        TeacherName = c.String(),
                        TeacherEmail = c.String(),
                        TeacherPhone = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.TeacherId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Teachers");
            DropTable("dbo.Students");
            DropTable("dbo.Departments");
        }
    }
}
