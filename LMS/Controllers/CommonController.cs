using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS.Controllers
{
  public class CommonController : Controller
  {

    /*******Begin code to modify********/

    // TODO: Uncomment and change 'X' after you have scaffoled
    
    protected Team63LMSContext db;

    public CommonController()
    {
      db = new Team63LMSContext();
    }
    

    /*
     * WARNING: This is the quick and easy way to make the controller
     *          use a different LibraryContext - good enough for our purposes.
     *          The "right" way is through Dependency Injection via the constructor 
     *          (look this up if interested).
    */

    // TODO: Uncomment and change 'X' after you have scaffoled
    
    public void UseLMSContext(Team63LMSContext ctx)
    {
      db = ctx;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        db.Dispose();
      }
      base.Dispose(disposing);
    }

    /// <summary>
    /// Retreive a JSON array of all departments from the database.
    /// Each object in the array should have a field called "name" and "subject",
    /// where "name" is the department name and "subject" is the subject abbreviation.
    /// </summary>
    /// <returns>The JSON array</returns>
    public IActionResult GetDepartments()
    {
        var query = from d in db.Departments
                    select new
                    {
                        name = d.Name,
                        subject = d.Subject
                    };

        return Json(query.ToArray());
    }



    /// <summary>
    /// Returns a JSON array representing the course catalog.
    /// Each object in the array should have the following fields:
    /// "subject": The subject abbreviation, (e.g. "CS")
    /// "dname": The department name, as in "Computer Science"
    /// "courses": An array of JSON objects representing the courses in the department.
    ///            Each field in this inner-array should have the following fields:
    ///            "number": The course number (e.g. 5530)
    ///            "cname": The course name (e.g. "Database Systems")
    /// </summary>
    /// <returns>The JSON array</returns>
    public IActionResult GetCatalog()
    {
            var query = from d in db.Departments
                        select new
                        {
                            subject = d.Subject,
                            dname = d.Name,
                            courses = from cr in db.Courses where d.Subject == cr.Listing
                                      select new
                                      {
                                          number = cr.Number,
                                          cname = cr.Name
                                       }
                    };

        return Json(query.ToArray());
    }

    /// <summary>
    /// Returns a JSON array of all class offerings of a specific course.
    /// Each object in the array should have the following fields:
    /// "season": the season part of the semester, such as "Fall"
    /// "year": the year part of the semester
    /// "location": the location of the class
    /// "start": the start time in format "hh:mm:ss"
    /// "end": the end time in format "hh:mm:ss"
    /// "fname": the first name of the professor
    /// "lname": the last name of the professor
    /// </summary>
    /// <param name="subject">The subject abbreviation, as in "CS"</param>
    /// <param name="number">The course number, as in 5530</param>
    /// <returns>The JSON array</returns>
    public IActionResult GetClassOfferings(string subject, int number)
    {
            var query = from cr in db.Courses
                        join c in db.Classes
                        on new { A = cr.CatalogId, B = cr.Listing, C = (int) cr.Number }
                        equals new { A = c.CategoryId, B = subject, C = number} into joined
                        from j in joined.DefaultIfEmpty()
                        select j;

            var query2 = from q in query
                            join p in db.Professors
                            on q.TaughtBy equals p.UId
                            select new
                            {
                                season = splitSem(q.Semester, 0),
                                year = splitSem(q.Semester, 1),
                                location = q.Loc,
                                start = q.Start,
                                end = q.End,
                                fname = p.FName,
                                lname = p.LName 
                            };

            return Json(query2.ToArray());
    }

    /// <summary>
    /// This method does NOT return JSON. It returns plain text (containing html).
    /// Use "return Content(...)" to return plain text.
    /// Returns the contents of an assignment.
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The name of the assignment category in the class</param>
    /// <param name="asgname">The name of the assignment in the category</param>
    /// <returns>The assignment contents</returns>
    public IActionResult GetAssignmentContents(string subject, int num, string season, int year, string category, string asgname)
    {

        var query = from cr in db.Courses
                    join c in db.Classes
                    on new { A = cr.CatalogId, B = cr.Listing, C = (int)cr.Number }
                    equals new { A = c.CategoryId, B = subject, C = num } into joined
                    from j in joined
                    select j;

        var query2 = from q in query
                        join ac in db.AssignmentCategories
                        on new { A = q.ClassId, B = (string) splitSem(q.Semester, 0), C = (int) splitSem(q.Semester, 1)} 
                        equals new { A = ac.ClassId, B = season, C = year } into joined
                        from j in joined.DefaultIfEmpty()
                        select j;

        var query3 = from q in query2
                        join a in db.Assignments
                        on new { A = q.AssignCatId, B = q.Name, C = asgname } equals new { A = a.AssignCatId, B = category, C = a.Name }
                        select a.Contents;

        try
        {
            return Content(query3.SingleOrDefault());
        }
        catch
        {
            return Content(null);
        }
    }


    /// <summary>
    /// This method does NOT return JSON. It returns plain text (containing html).
    /// Use "return Content(...)" to return plain text.
    /// Returns the contents of an assignment submission.
    /// Returns the empty string ("") if there is no submission.
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The name of the assignment category in the class</param>
    /// <param name="asgname">The name of the assignment in the category</param>
    /// <param name="uid">The uid of the student who submitted it</param>
    /// <returns>The submission text</returns>
    public IActionResult GetSubmissionText(string subject, int num, string season, int year, string category, string asgname, string uid)
    {

        var query = from cr in db.Courses
                    join c in db.Classes
                    on new { A = cr.CatalogId, B = cr.Listing, C = (int)cr.Number }
                    equals new { A = c.CategoryId, B = subject, C = num } into joined
                    from j in joined
                    select j;

        var query2 = from q in query
                        join ac in db.AssignmentCategories
                        on new { A = q.ClassId, B = (string)splitSem(q.Semester, 0), C = (int)splitSem(q.Semester, 1) }
                        equals new { A = ac.ClassId, B = season, C = year } into joined
                        from j in joined
                        select j;

        var query3 = from q in query2
                        join a in db.Assignments
                        on new { A = q.AssignCatId, B = q.Name, C = asgname } equals new { A = a.AssignCatId, B = category, C = a.Name } into joined
                        from j in joined
                        select j;

        var query4 = from q in query3
                        join s in db.Submission
                        on new { A = q.AId, B = uid } equals new { A = s.AId, B = s.UId }
                        select s.Contents;

            try
            {
                return Content(query4.SingleOrDefault());
            }
            catch
            {
                return Content(null);
            }
    }


    /// <summary>
    /// Gets information about a user as a single JSON object.
    /// The object should have the following fields:
    /// "fname": the user's first name
    /// "lname": the user's last name
    /// "uid": the user's uid
    /// "department": (professors and students only) the name (such as "Computer Science") of the department for the user. 
    ///               If the user is a Professor, this is the department they work in.
    ///               If the user is a Student, this is the department they major in.    
    ///               If the user is an Administrator, this field is not present in the returned JSON
    /// </summary>
    /// <param name="uid">The ID of the user</param>
    /// <returns>
    /// The user JSON object 
    /// or an object containing {success: false} if the user doesn't exist
    /// </returns>
    public IActionResult GetUser(string uid)
    {
        var query = from p in db.Professors where p.UId == uid
                    select p;

        if(query.Count() > 0)
        {
            var query2 = from p in db.Professors
                            where p.UId == uid
                            select new
                            {
                                fname = p.FName,
                                lname = p.LName,
                                uid = p.UId,
                                department = p.WorksIn
                            };
            return Json(query2.SingleOrDefault());
        }

        var query3 = from s in db.Students
                        where s.UId == uid
                        select s;
        if (query3.Count() > 0)
        {
            var query4 = from s in db.Students
                            where s.UId == uid
                            select new
                        {
                            fname = s.FName,
                            lname = s.LName,
                            uid = s.UId,
                            department = s.Major
                        };

            return Json(query4.SingleOrDefault());
        }

        var query5 = from a in db.Administrators
                        where a.UId == uid
                        select a;   

        if (query5.Count() > 0)
        {
            var query6 = from a in db.Administrators
                            where a.UId == uid
                            select new
                            {
                                fname = a.FName,
                                lname = a.LName,
                                uid = a.UId,
                            };
            return Json(query5.SingleOrDefault());
        }
        else
        {
            return Json(new { success = false });
        }
}

    /* Private helper method that splits the semester's
     * season and year.
     * @ret object - season or year
     * @param semester - semester to be split into season/year
     * @param seasonYear - condition to return season/year
     */
    private object splitSem(string semester, int seasonYear)
    {
        string[] s = semester.Split(' ');

        if(seasonYear == 0)
        {
            return s[0];
        }
        else
        {
            return Int32.Parse(s[1]);
        }
    }

    /*******End code to modify********/

  }
}