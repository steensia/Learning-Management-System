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
    public class CommonControllerUnitTests : LMSUnitTests
    {
        [Fact]
        private void TestGetDepartments()
        {
            var db = AddDepartment();

            CommonController controller = new CommonController();
            controller.UseLMSContext(db);

            var result = controller.GetDepartments() as JsonResult;

            dynamic jdata = result.Value;

            var query = from d in db.Departments
                        select d;

            Assert.Equal(1, jdata.Length);
            Assert.Equal("Test Department", query.ToArray()[0].Name);
            Assert.Equal("CS", query.ToArray()[0].Subject);

        }

        [Fact]
        private void TestGetDepartmentsEmpty()
        {
            var db = mockDB();

            CommonController controller = new CommonController();
            controller.UseLMSContext(db);

            var result = controller.GetDepartments() as JsonResult;

            dynamic jdata = result.Value;

            var query = from d in db.Departments
                        select d;

            Assert.Equal(0, jdata.Length);
            Assert.Null(query.SingleOrDefault());
        }

        [Fact]
        private void TestGetClassOfferings()
        {
            var db = AddProfessorCourseClass();

            CommonController controller = new CommonController();
            controller.UseLMSContext(db);

            var result = controller.GetClassOfferings("CS", 5530) as JsonResult;

            dynamic jdata = result.Value;

            var query = from c in db.Classes
                        select c;

            //Assert.Equal(1, jdata.Length);
            //Assert.Equal("u0000000", query.SingleOrDefault().TaughtBy);
        }

        [Fact]
        private void TestGetClassOfferingsNoClass()
        {
            var db = mockDB();

            CommonController controller = new CommonController();
            controller.UseLMSContext(db);

            var result = controller.GetClassOfferings("CS", 5530) as JsonResult;

            dynamic jdata = result.Value;

            var query = from c in db.Classes
                        select c;

            Assert.Equal(0, jdata.Length);
            Assert.Null(query.SingleOrDefault());
        }

        [Fact]
        private void TestGetCatalog()
        {
            var db = AddDepartment2();

            CommonController controller = new CommonController();
            controller.UseLMSContext(db);

            var result = controller.GetCatalog() as JsonResult;

            dynamic jdata = result.Value;

            var query = from c in db.Courses
                        select c;

            Assert.Equal(1, jdata.Length);
            Assert.Equal(3, query.Count());
        }
    }
}
