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
    public class StudentControllerUnitTests : LMSUnitTests
    {

        [Fact]
        private void testGetMyClasses()
        {
            /*
            var db = AddStudentstoMultipleClasses();

            StudentController controller = new StudentController();
            controller.UseLMSContext(db);

            var result1 = controller.GetMyClasses("u0000003") as JsonResult;
            var result2 = controller.GetMyClasses("u0000002") as JsonResult;
            var result3 = controller.GetMyClasses("u0000001") as JsonResult;

            dynamic jdata1 = result1.Value;
            dynamic jdata2 = result2.Value;
            dynamic jdata3 = result3.Value;

            Assert.Equal(1, jdata1.count);
            Assert.Equal(2, jdata2.count);
            Assert.Equal(3, jdata3.count);
            */
        }

    }
}
