using System;
using Xunit;
using LMS.Controllers;
using LMS.Models.LMSModels;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace LMSTestProject
{
    public class LMSUnitTests
    {
        private static ServiceProvider NewServiceProvider()
        {
            var serviceProvider = new ServiceCollection()
              .AddEntityFrameworkInMemoryDatabase()
              .BuildServiceProvider();
            return serviceProvider;
        }

        protected Team63LMSContext mockDB()
        {
            var optionsBuilder = new DbContextOptionsBuilder<Team63LMSContext>();
            optionsBuilder.UseInMemoryDatabase().
                UseApplicationServiceProvider(NewServiceProvider());

            Team63LMSContext db = new Team63LMSContext(optionsBuilder.Options);

            return db;
        }

        protected Team63LMSContext AddOneCourse()
        {
            Team63LMSContext db = mockDB();

            Courses c = new Courses();
            c.Listing = "CS";
            c.Number = 4150;
            c.Name = "Algorithms";

            db.Courses.Add(c);

            db.SaveChanges();

            return db;
        }

        protected Team63LMSContext AddExistingCourse()
        {
            Team63LMSContext db = mockDB();

            Courses c = new Courses();
            c.Listing = "CS";
            c.Number = 4150;
            c.Name = "Algorithms";

            db.Courses.Add(c);

            db.SaveChanges();

            Courses c2 = new Courses();
            c2.Listing = "CS";
            c2.Number = 4150;
            c2.Name = "Algorithms";

            db.Courses.Add(c2);
            db.SaveChanges();

            return db;
        }

        protected Team63LMSContext AddCourses()
        {
            Team63LMSContext db = mockDB();

            Courses c = new Courses();
            c.Listing = "CS";
            c.Number = 4150;
            c.Name = "Algorithms";

            db.Courses.Add(c);

            db.SaveChanges();

            Courses c2 = new Courses();
            c2.Listing = "CS";
            c2.Number = 4600;
            c2.Name = "Computer Graphics";

            db.Courses.Add(c2);
            db.SaveChanges();

            Courses c3 = new Courses();
            c3.Listing = "CS";
            c3.Number = 4540;
            c3.Name = "Web Software Architecture";

            db.Courses.Add(c3);
            db.SaveChanges();

            return db;
        }

        protected Team63LMSContext AddAdministrator()
        {
            Team63LMSContext db = mockDB();

            Administrators a = new Administrators();
            a.FName = "Magnus";
            a.LName = "Carlsen";
            a.UId = uIDGen(db);
            a.Dob = new DateTime(1997, 10, 23);

            db.Administrators.Add(a);
            db.SaveChanges();

            return db;
        }

        protected Team63LMSContext AddProfessor()
        {
            Team63LMSContext db = mockDB();

            Professors p = new Professors();
            p.FName = "John";
            p.LName = "Doe";
            p.UId = uIDGen(db);
            p.WorksIn = "CS";
            p.Dob = new DateTime(1997, 10, 23);

            db.Professors.Add(p);
            db.SaveChanges();

            return db;
        }

        protected Team63LMSContext AddProfessor2()
        {
            Team63LMSContext db = mockDB();

            Professors p = new Professors();
            p.FName = "Ron";
            p.LName = "Weasley";
            p.UId = uIDGen(db);
            p.WorksIn = "EE";
            p.Dob = new DateTime(1994, 09, 22);

            db.Professors.Add(p);
            db.SaveChanges();

            Professors p2 = new Professors();
            p2.FName = "Kanny";
            p2.LName = "Dopta";
            p2.UId = uIDGen(db);
            p2.WorksIn = "EE";
            p2.Dob = new DateTime(1992, 10, 23);

            db.Professors.Add(p2);
            db.SaveChanges();

            return db;
        }

        protected Team63LMSContext AddClasses()
        {
            Team63LMSContext db = mockDB();

            AdministratorController controller = new AdministratorController();
            controller.UseLMSContext(db);

            // Create Professor to teach class
            Professors p = new Professors();
            p.FName = "Danny";
            p.LName = "Kopta";
            p.UId = uIDGen(db);
            p.WorksIn = "CS";
            p.Dob = new DateTime(1992, 10, 23);


            db.Professors.Add(p);
            db.SaveChanges();

            controller.CreateCourse("CS", 5530, "Database Systems");

            var query = from pr in db.Professors
                        select pr.UId;

            var query2 = from cr in db.Courses
                         select cr.CatalogId;

            // Create class associated with a CS course
            Classes c = new Classes();
            c.Loc = "WEB 1250";
            c.Start = new TimeSpan(7, 9, 16);
            c.End = new TimeSpan(9, 16, 25);
            c.Semester = "Spring 2019";
            c.TaughtBy = query.SingleOrDefault();
            c.CategoryId = query2.SingleOrDefault();

            db.Classes.Add(c);

            db.SaveChanges();

            return db;
        }

        protected Team63LMSContext AddClasses2()
        {
            Team63LMSContext db = mockDB();

            AdministratorController controller = new AdministratorController();
            controller.UseLMSContext(db);

            // Create Professor to teach class
            Professors p = new Professors();
            p.FName = "Danny";
            p.LName = "Kopta";
            p.UId = uIDGen(db);
            p.WorksIn = "CS";

            db.Professors.Add(p);
            db.SaveChanges();

            controller.CreateCourse("CS", 5530, "Database Systems");

            return db;
        }
        protected Team63LMSContext AddDepartment()
        {
            var db = mockDB();

            Departments d = new Departments();
            d.Name = "Test Department";
            d.Subject = "CS";

            db.Departments.Add(d);
            db.SaveChanges();

            return db;
        }

        protected Team63LMSContext AddDepartment2()
        {
            Team63LMSContext db = mockDB();

            Departments d = new Departments();
            d.Name = "CS Department";
            d.Subject = "CS";

            db.Departments.Add(d);
            db.SaveChanges();

            Courses c = new Courses();
            c.Listing = "CS";
            c.Number = 4150;
            c.Name = "Algorithms";

            db.Courses.Add(c);

            db.SaveChanges();

            Courses c2 = new Courses();
            c2.Listing = "CS";
            c2.Number = 4600;
            c2.Name = "Computer Graphics";

            db.Courses.Add(c2);
            db.SaveChanges();

            Courses c3 = new Courses();
            c3.Listing = "CS";
            c3.Number = 4540;
            c3.Name = "Web Software Architecture";

            db.Courses.Add(c3);
            db.SaveChanges();

            return db;
        }

        protected Team63LMSContext AddProfessorCourseClass()
        {
            Team63LMSContext db = mockDB();

            AdministratorController controller = new AdministratorController();
            controller.UseLMSContext(db);

            // Create Professor to teach class
            Professors p = new Professors();
            p.FName = "Danny";
            p.LName = "Kopta";
            p.UId = uIDGen(db);
            p.WorksIn = "CS";

            db.Professors.Add(p);
            db.SaveChanges();

            controller.CreateCourse("CS", 5530, "Database Systems");

            controller.CreateClass("CS", 1, "Spring", 2019, new DateTime(2009, 05, 30, 7, 9, 16),
                        new DateTime(2009, 05, 30, 9, 16, 25), "WEB 1250", "u0000000");

            return db;
        }

        protected Team63LMSContext AddStudentstoClass()
        {
            Team63LMSContext db = mockDB();
            AdministratorController controller = new AdministratorController();
            controller.UseLMSContext(db);

            StudentController sController = new StudentController();
            sController.UseLMSContext(db);

            // Create Professor to teach class
            Professors p = new Professors();
            p.FName = "Danny";
            p.LName = "Kopta";
            p.UId = uIDGen(db);
            p.WorksIn = "CS";

            db.Professors.Add(p);
            db.SaveChanges();

            controller.CreateCourse("CS", 5530, "Database Systems");

            controller.CreateClass("CS", 1, "Spring", 2019, new DateTime(2009, 05, 30, 7, 9, 16),
                        new DateTime(2009, 05, 30, 9, 16, 25), "WEB 1250", "u0000000");

            //Add student to class
            Students s = new Students();
            s.UId = "u0000001";
            s.FName = "Steen";
            s.LName = "Sia";
            s.Dob = new DateTime(2000, 07, 01);
            s.Major = "Electrical Engineering";

            Students s2 = new Students();
            s.UId = "u0000002";
            s.FName = "Benjamin";
            s.LName = "Button";
            s.Dob = new DateTime(2000, 07, 01);
            s.Major = "Electrical Engineering";

            Students s3 = new Students();
            s.UId = "u0000003";
            s.FName = "J";
            s.LName = "Fish";
            s.Dob = new DateTime(2000, 07, 01);
            s.Major = "Computer Science";

            Students s4 = new Students();
            s.UId = "u0000004";
            s.FName = "Big";
            s.LName = "Mama";
            s.Dob = new DateTime(2000, 07, 01);
            s.Major = "Art";

            Students s5 = new Students();
            s.UId = "u0000005";
            s.FName = "King";
            s.LName = "Arthur";
            s.Dob = new DateTime(2000, 07, 01);
            s.Major = "Medival Studies";

            db.Students.Add(s);
            db.Students.Add(s2);
            db.Students.Add(s3);
            db.Students.Add(s4);
            db.Students.Add(s5);
            db.SaveChanges();

            sController.Enroll("CS", 5530, "Spring", 2019, "u0000001");
            sController.Enroll("CS", 5530, "Spring", 2019, "u0000002");
            sController.Enroll("CS", 5530, "Spring", 2019, "u0000003");
            sController.Enroll("CS", 5530, "Spring", 2019, "u0000004");
            sController.Enroll("CS", 5530, "Spring", 2019, "u0000005");

            return db;
        }

        protected Team63LMSContext AddStudentstoMultipleClasses()
        {
            Team63LMSContext db = mockDB();
            AdministratorController controller = new AdministratorController();
            controller.UseLMSContext(db);

            StudentController sController = new StudentController();
            sController.UseLMSContext(db);

            // Create Professor to teach class
            Professors p = new Professors();
            p.FName = "Danny";
            p.LName = "Kopta";
            p.UId = uIDGen(db);
            p.WorksIn = "CS";

            db.Professors.Add(p);
            db.SaveChanges();

            controller.CreateCourse("CS", 5530, "Database Systems");
            controller.CreateCourse("CS", 4400, "Computer Systems");
            controller.CreateCourse("CS", 3500, "Software Practice I");

            controller.CreateClass("CS", 1, "Spring", 2019, new DateTime(2009, 05, 30, 7, 9, 16),
                        new DateTime(2009, 05, 30, 9, 16, 25), "WEB 1250", "u0000000");

            controller.CreateClass("CS", 4400, "Spring", 2019, new DateTime(2009, 05, 30, 7, 9, 16),
                        new DateTime(2009, 05, 30, 9, 16, 25), "WEB 2000", "u0000000");

            controller.CreateClass("CS", 3500, "Spring", 2019, new DateTime(2009, 05, 30, 7, 9, 16),
                        new DateTime(2009, 05, 30, 9, 16, 25), "WEB 1999", "u0000000");

            //Add student to class
            Students s = new Students();
            s.UId = "u0000001";
            s.FName = "Steen";
            s.LName = "Sia";
            s.Dob = new DateTime(2000, 07, 01);
            s.Major = "Electrical Engineering";

            Students s2 = new Students();
            s.UId = "u0000002";
            s.FName = "Benjamin";
            s.LName = "Button";
            s.Dob = new DateTime(2000, 07, 01);
            s.Major = "Electrical Engineering";

            Students s3 = new Students();
            s.UId = "u0000003";
            s.FName = "J";
            s.LName = "Fish";
            s.Dob = new DateTime(2000, 07, 01);
            s.Major = "Computer Science";

            db.Students.Add(s);
            db.Students.Add(s2);
            db.Students.Add(s3);
            db.SaveChanges();

            sController.Enroll("CS", 5530, "Spring", 2019, "u0000001");
            sController.Enroll("CS", 5530, "Spring", 2019, "u0000002");
            sController.Enroll("CS", 4400, "Spring", 2019, "u0000001");
            sController.Enroll("CS", 4400, "Spring", 2019, "u0000002");
            sController.Enroll("CS", 3500, "Spring", 2019, "u0000003");
            sController.Enroll("CS", 3500, "Spring", 2019, "u0000001");

            return db;
        }
        /* Private helper method to generate an auto-increment uID 
         * for any user. If no user has been created, the first
         * user to register receives uID, u0000000.
         * @ret string - unique uID
         */
        private string uIDGen(Team63LMSContext db)
        {
            string retUID = "";

                // List to store max uIDs for every user
                List<int> uIDs = new List<int>();

                var query = (from a in db.Administrators
                             orderby a.UId descending
                             select a.UId).Take(1);

                addUId(uIDs, query);

                query = (from p in db.Professors
                         orderby p.UId descending
                         select p.UId).Take(1);

                addUId(uIDs, query);

                query = (from s in db.Students
                         orderby s.UId descending
                         select s.UId).Take(1);

                addUId(uIDs, query);

                // First user to register
                if (uIDs.Count == 0) return "u0000000";
                else
                {
                    int maxUID = 0;

                    // Retrieve the max uID available for next useru
                    foreach (var uID in uIDs)
                    {
                        if (maxUID < uID) maxUID = uID;
                    }
                    // Increment uID for new user
                    retUID = (maxUID + 1).ToString("D7"); // Pads 0's
                }
            return 'u' + retUID;
        }

        /* Private helper method that adds existing uIDs to 
         * a list of uIDs for comparison. Otherwise, do nothing.
         * @param uIDs - List to add uIDs
         * @param query - uID to obtain from each user
         */
        private void addUId(List<int> uIDs, IQueryable<string> query)
        {
            // Extracts the 7 digits from uID
            if (query.ToArray().FirstOrDefault() != null)
            {
                Int32.TryParse(query.ToArray().FirstOrDefault().Substring(1), out int temp);
                uIDs.Add(temp);
            }
        }
    }
}
