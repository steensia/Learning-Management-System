using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Controllers
{
  [Authorize(Roles = "Administrator")]
  public class AdministratorController : CommonController
  {
    public IActionResult Index()
    {
      return View();
    }

    public IActionResult Department(string subject)
    {
      ViewData["subject"] = subject;
      return View();
    }

    public IActionResult Course(string subject, string num)
    {
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      return View();
    }

    /*******Begin code to modify********/

    /// <summary>
    /// Returns a JSON array of all the courses in the given department.
    /// Each object in the array should have the following fields:
    /// "number" - The course number (as in 5530)
    /// "name" - The course name (as in "Database Systems")
    /// </summary>
    /// <param name="subject">The department subject abbreviation (as in "CS")</param>
    /// <returns>The JSON result</returns>
    public IActionResult GetCourses(string subject)
    {
        var query = from c in db.Courses where c.Listing == subject
                    select new
                    {
                        number = c.Number,
                        name = c.Name
                    };

        return Json(query.ToArray());
    }



    /// <summary>
    /// Returns a JSON array of all the professors working in a given department.
    /// Each object in the array should have the following fields:
    /// "lname" - The professor's last name
    /// "fname" - The professor's first name
    /// "uid" - The professor's uid
    /// </summary>
    /// <param name="subject">The department subject abbreviation</param>
    /// <returns>The JSON result</returns>
    public IActionResult GetProfessors(string subject)
    {
        var query = from p in db.Professors
                    where p.WorksIn == subject
                    select new
                    {
                        lname = p.LName,
                        fname = p.FName,
                        uid = p.UId
                    };

        return Json(query.ToArray());
    }



    /// <summary>
    /// Creates a course.
    /// A course is uniquely identified by its number + the subject to which it belongs
    /// </summary>
    /// <param name="subject">The subject abbreviation for the department in which the course will be added</param>
    /// <param name="number">The course number</param>
    /// <param name="name">The course name</param>
    /// <returns>A JSON object containing {success = true/false}.
    /// false if the course already exists, true otherwise.</returns>
    public IActionResult CreateCourse(string subject, int number, string name)
    {
        var query = from cr in db.Courses
                    where cr.Listing == subject && cr.Number == number && cr.Name == name
                    select cr;

        var subQuery = from cr in db.Courses
                        where cr.Listing == subject && cr.Number == number
                        select cr;

        if(subQuery.SingleOrDefault() != null)
            {
                return Json(new { success = false });
            }

        if (query.ToArray().Count() == 0)
        {
            Courses c = new Courses();
            c.Listing = subject;
            c.Number = (ushort)number;
            c.Name = name;

            db.Courses.Add(c);
            db.SaveChanges();

            return Json(new { success = true });
        }
        else
        {
            return Json(new { success = false });
        }
    }



    /// <summary>
    /// Creates a class offering of a given course.
    /// </summary>
    /// <param name="subject">The department subject abbreviation</param>
    /// <param name="number">The course number</param>
    /// <param name="season">The season part of the semester</param>
    /// <param name="year">The year part of the semester</param>
    /// <param name="start">The start time</param>
    /// <param name="end">The end time</param>
    /// <param name="location">The location</param>
    /// <param name="instructor">The uid of the professor</param>
    /// <returns>A JSON object containing {success = true/false}. 
    /// false if another class occupies the same location during any time 
    /// within the start-end range in the same semester, or if there is already
    /// a Class offering of the same Course in the same Semester,
    /// true otherwise.</returns>
    public IActionResult CreateClass(string subject, int number, string season, int year, DateTime start, DateTime end, string location, string instructor)
    {

            var query = from cl in db.Classes
                        where cl.Loc == location && (start.TimeOfDay >= cl.Start && start.TimeOfDay <= cl.End) && cl.Semester.Equals(season + " " + year)
                        || cl.Loc == location && (end.TimeOfDay <= cl.End && end.TimeOfDay >= cl.Start) && cl.Semester.Equals(season + " " + year)
                        select cl;

            if (query.ToArray().Count() > 0 && query.SingleOrDefault() != null) return Json(new { success = false });

            var query2 = from cr in db.Courses
                         where subject == cr.Listing && number == cr.Number
                         select cr.CatalogId;

            var catID = query2.ToArray().FirstOrDefault();

            string semester = season + " " + year;

            var query3 = from cl in db.Classes
                         where cl.CategoryId == catID
                         && cl.Semester == semester
                         select cl;

            if (query3.Count() > 0 && query3.SingleOrDefault() == null) return Json(new { success = false });

        Classes c = new Classes();

        c.CategoryId = (uint) catID;
        c.Semester = semester;
        c.Start = start.TimeOfDay;
        c.End = end.TimeOfDay;
        c.Loc = location;
        c.TaughtBy = instructor;

        db.Classes.Add(c);

            try
            {
                db.SaveChanges();

                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
    }


    /*******End code to modify********/

  }
}