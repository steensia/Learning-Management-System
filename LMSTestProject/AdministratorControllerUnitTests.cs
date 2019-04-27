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
    public class AdministratorControllerUnitTests : LMSUnitTests
    {
        [Fact]
        public void TestAddClass()
        {
            var db = AddClasses();

            var query = from c in db.Classes
                        select c;

            Assert.Equal(1, query.Count());
            Assert.Equal("WEB 1250", query.ToArray()[0].Loc);
            Assert.Equal(new TimeSpan(7, 9, 16), query.ToArray()[0].Start);
            Assert.Equal(new TimeSpan(9, 16, 25), query.ToArray()[0].End);
            Assert.Equal("Spring 2019", query.ToArray()[0].Semester);
            Assert.Equal("u0000000", query.ToArray()[0].TaughtBy);
            Assert.Equal(1, (int)query.ToArray()[0].CategoryId);
        }

        [Fact]
        public void TestAddClass2()
        {
            var db = AddClasses2();

            AdministratorController controller = new AdministratorController();
            controller.UseLMSContext(db);

            var query0 = from cr in db.Courses
                         select cr;

            var result = controller.CreateClass("CS", 1, "Spring", 2019, new DateTime(2009, 05, 30, 7, 9, 16),
                        new DateTime(2009, 05, 30, 9, 16, 25), "WEB 1250", "u0000000") as JsonResult;

            var query = from c in db.Classes
                        select c;

            dynamic jdata = result.Value;

            Assert.Equal(true, jdata.ToString().Contains("True"));

            Assert.Equal(1, query.Count());
            Assert.Equal("WEB 1250", query.ToArray()[0].Loc);
            Assert.Equal(new TimeSpan(7, 9, 16), query.ToArray()[0].Start);
            Assert.Equal(new TimeSpan(9, 16, 25), query.ToArray()[0].End);
            Assert.Equal("Spring 2019", query.ToArray()[0].Semester);
            Assert.Equal("u0000000", query.ToArray()[0].TaughtBy);
           //Assert.Equal(1, (int)query.ToArray()[0].CategoryId);
        }

        [Fact]
        public void TestAddClassExistingClass()
        {
            var db = AddClasses();

            AdministratorController controller = new AdministratorController();
            controller.UseLMSContext(db);

            var result = controller.CreateClass("CS", 1, "Spring", 2019, new DateTime(2009, 05, 30, 7, 9, 16),
                        new DateTime(2009, 05, 30, 9, 16, 25), "WEB 1250", "u0000000") as JsonResult;

            dynamic jdata = result.Value;

            var query = from c in db.Classes
                        select c;

            Assert.Equal(true, jdata.ToString().Contains("False"));

            Assert.Equal(1, query.Count());
            Assert.Equal("WEB 1250", query.ToArray()[0].Loc);
            Assert.Equal(new TimeSpan(7, 9, 16), query.ToArray()[0].Start);
            Assert.Equal(new TimeSpan(9, 16, 25), query.ToArray()[0].End);
            Assert.Equal("Spring 2019", query.ToArray()[0].Semester);
            Assert.Equal("u0000000", query.ToArray()[0].TaughtBy);
            Assert.Equal(1, (int)query.ToArray()[0].CategoryId);
        }

        [Fact]
        public void TestAddClassStartConflict()
        {
            var db = AddClasses();

            AdministratorController controller = new AdministratorController();
            controller.UseLMSContext(db);

            var result = controller.CreateClass("CS", 1, "Spring", 2019, new DateTime(2009, 05, 30, 7, 25, 49),
                        new DateTime(2009, 05, 30, 9, 25, 49), "MEB 3232", "u0000000") as JsonResult;

            dynamic jdata = result.Value;

            var query = from c in db.Classes
                        select c;

            //Assert.Equal(true, jdata.ToString().Contains("False"));
            //Assert.Equal(1, query.Count());
        }

        [Fact]
        public void TestAddClassEndConflict()
        {
            var db = AddClasses();

            AdministratorController controller = new AdministratorController();
            controller.UseLMSContext(db);

            var result = controller.CreateClass("CS", 1, "Spring", 2019, new DateTime(2009, 05, 30, 4, 7, 9),
                        new DateTime(2009, 05, 30, 7, 9, 16), "MEB 3232", "u0000000") as JsonResult;

            dynamic jdata = result.Value;

            var query = from c in db.Classes
                        select c;

            //Assert.Equal(true, jdata.ToString().Contains("False"));
            //Assert.Equal(1, query.Count());
        }
        [Fact]
        public void TestAddCourse()
        {
            var db = AddOneCourse();

            var query = from c in db.Courses
                        select c;

            Assert.Equal(1, query.Count());
        }

        [Fact]
        public void TestAddExistingCourse()
        {
            var db = AddExistingCourse();

            var query = from c in db.Courses
                        select c;

            Assert.Equal(2, query.Count());
        }

        [Fact]
        public void TestCreateCourseAddCourse()
        {
            var db = mockDB();

            AdministratorController controller = new AdministratorController();
            controller.UseLMSContext(db);

            var result = controller.CreateCourse("HF", 1550, "Classical Mythology") as JsonResult;

            dynamic jdata = result.Value;

            Assert.Equal(true, jdata.ToString().Contains("True"));
        }

        [Fact]
        public void TestCreateCourseAddExistingCourse()
        {
            var db = mockDB();

            AdministratorController controller = new AdministratorController();
            controller.UseLMSContext(db);

            var result = controller.CreateCourse("HF", 1550, "Classical Mythology") as JsonResult;

            dynamic jdata = result.Value;

            Assert.Equal(true, jdata.ToString().Contains("True"));

            // Return false if course already exists
            var result2 = controller.CreateCourse("HF", 1550, "Classical Mythology") as JsonResult;

            dynamic jdata2 = result2.Value;

            Assert.Equal(true, jdata2.ToString().Contains("False"));
        }

        [Fact]
        public void TestGetCourses()
        {
            var db = AddCourses();

            AdministratorController controller = new AdministratorController();
            controller.UseLMSContext(db);

            var result = controller.GetCourses("CS") as JsonResult;

            dynamic jdata = result.Value;

            Assert.Equal(3, jdata.Length);
        }

        [Fact]
        public void TestGetCoursesDoNotExist()
        {
            var db = AddCourses();

            AdministratorController controller = new AdministratorController();
            controller.UseLMSContext(db);

            var result = controller.GetCourses("EE") as JsonResult;

            dynamic jdata = result.Value;

            Assert.Equal(0, jdata.Length);
        }

        [Fact]
        public void TestAddAdministrator()
        {
            var db = AddAdministrator();

            var query = from a in db.Administrators
                        select a;

            Assert.Equal("Magnus", query.ToArray()[0].FName);
            Assert.Equal("Carlsen", query.ToArray()[0].LName);
            Assert.Equal("u0000000", query.ToArray()[0].UId);
            Assert.Contains("10/23/1997", query.ToArray()[0].Dob.ToString());
        }

        [Fact]
        public void TestGetProfessors()
        {
            var db = AddProfessor();

            AdministratorController controller = new AdministratorController();
            controller.UseLMSContext(db);

            var result = controller.GetProfessors("CS") as JsonResult;

            dynamic jdata = result.Value;

            Assert.Equal(1, jdata.Length);


            var query = from p in db.Professors
                        select p;

            Assert.Equal("John", query.ToArray()[0].FName);
            Assert.Equal("Doe", query.ToArray()[0].LName);
            Assert.Equal("u0000000", query.ToArray()[0].UId);
            Assert.Equal("CS", query.ToArray()[0].WorksIn);
            Assert.Contains("10/23/1997", query.ToArray()[0].Dob.ToString());
        }

        [Fact]
        public void TestGetProfessors2()
        {
            var db = AddProfessor2();

            AdministratorController controller = new AdministratorController();
            controller.UseLMSContext(db);

            var result = controller.GetProfessors("EE") as JsonResult;

            dynamic jdata = result.Value;

            Assert.Equal(2, jdata.Length);

            var query = from p in db.Professors
                        select p;

            Assert.Equal("Ron", query.ToArray()[0].FName);
            Assert.Equal("Weasley", query.ToArray()[0].LName);
            Assert.Equal("u0000000", query.ToArray()[0].UId);
            Assert.Equal("EE", query.ToArray()[0].WorksIn);
            Assert.Contains("9/22/1994", query.ToArray()[0].Dob.ToString());

            Assert.Equal("Kanny", query.ToArray()[1].FName);
            Assert.Equal("Dopta", query.ToArray()[1].LName);
            Assert.Equal("u0000001", query.ToArray()[1].UId);
            Assert.Equal("EE", query.ToArray()[1].WorksIn);
            Assert.Contains("10/23/1992", query.ToArray()[1].Dob.ToString());
        }
    }
}
