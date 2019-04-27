using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Controllers
{
  [Authorize(Roles = "Student")]
  public class StudentController : CommonController
  {

    public IActionResult Index()
    {
      return View();
    }

    public IActionResult Catalog()
    {
      return View();
    }

    public IActionResult Class(string subject, string num, string season, string year)
    {
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      ViewData["season"] = season;
      ViewData["year"] = year;
      return View();
    }

    public IActionResult Assignment(string subject, string num, string season, string year, string cat, string aname)
    {
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      ViewData["season"] = season;
      ViewData["year"] = year;
      ViewData["cat"] = cat;
      ViewData["aname"] = aname;
      return View();
    }


    public IActionResult ClassListings(string subject, string num)
    {
      System.Diagnostics.Debug.WriteLine(subject + num);
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      return View();
    }


    /*******Begin code to modify********/

    /// <summary>
    /// Returns a JSON array of the classes the given student is enrolled in.
    /// Each object in the array should have the following fields:
    /// "subject" - The subject abbreviation of the class (such as "CS")
    /// "number" - The course number (such as 5530)
    /// "name" - The course name
    /// "season" - The season part of the semester
    /// "year" - The year part of the semester
    /// "grade" - The grade earned in the class, or "--" if one hasn't been assigned
    /// </summary>
    /// <param name="uid">The uid of the student</param>
    /// <returns>The JSON array</returns>
    public IActionResult GetMyClasses(string uid)
    {
        var query = from cr in db.Courses
                    join c in db.Classes on cr.CatalogId equals c.CategoryId
                    select new
                    {
                        Courses = cr,
                        Classes = c
                    };

        var query2 = from q in query
                        join e in db.Enrolled
                        on new { A = q.Classes.ClassId, B = uid } equals new { A = e.ClassId, B = e.UId }
                        select new
                        {
                            subject = q.Courses.Listing,
                            number = q.Courses.Number,
                            name = q.Courses.Name,
                            season = splitSem(q.Classes.Semester, 0),
                            year = splitSem(q.Classes.Semester, 1),
                            grade = e.Grade
                        };

        return Json(query2.ToArray());
    }

    /// <summary>
    /// Returns a JSON array of all the assignments in the given class that the given student is enrolled in.
    /// Each object in the array should have the following fields:
    /// "aname" - The assignment name
    /// "cname" - The category name that the assignment belongs to
    /// "due" - The due Date/Time
    /// "score" - The score earned by the student, or null if the student has not submitted to this assignment.
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="uid"></param>
    /// <returns>The JSON array</returns>
    public IActionResult GetAssignmentsInClass(string subject, int num, string season, int year, string uid)
    {
        var query1 = from cr in db.Courses
                        where cr.Listing == subject && cr.Number == num
                        select cr;

        var query2 = from q in query1
                        join c in db.Classes
                        on new { A = q.CatalogId, B = season, C = year } equals new { A = c.CategoryId, B = (string)splitSem(c.Semester, 0), C = (int)splitSem(c.Semester, 1) }
                        into joined
                        from j in joined.DefaultIfEmpty()
                        select j;

        var query3 = from q in query2
                        join ac in db.AssignmentCategories
                        on q.ClassId equals ac.ClassId
                        into joined
                        from j in joined.DefaultIfEmpty()
                        select j;

        var query4 = from q in query3
                        join a in db.Assignments
                        on q.AssignCatId equals a.AssignCatId
                        into joined
                        from j in joined
                        select new
                        {
                            aname = j.Name,
                            cname = j.AssignCat.Name,
                            due = j.Due,
                            score = from s in db.Submission
                                    where new {A = s.AId, B = s.UId} == new {A = j.AId, B = uid}
                                    select s.Score
                        };
        try
        {
            return Json(query4.ToArray());
        }
        catch
        {
            return Json(null);
        }
    }


    /// <summary>
    /// Adds a submission to the given assignment for the given student
    /// The submission should use the current time as its DateTime
    /// You can get the current time with DateTime.Now
    /// The score of the submission should start as 0 until a Professor grades it
    /// If a Student submits to an assignment again, it should replace the submission contents
    /// and the submission time (the score should remain the same).
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The name of the assignment category in the class</param>
    /// <param name="asgname">The new assignment name</param>
    /// <param name="uid">The student submitting the assignment</param>
    /// <param name="contents">The text contents of the student's submission</param>
    /// <returns>A JSON object containing {success = true/false}</returns>
    public IActionResult SubmitAssignmentText(string subject, int num, string season, int year, 
      string category, string asgname, string uid, string contents)
    {
        var query1 = from cr in db.Courses
                        where cr.Listing == subject && cr.Number == num
                        select cr;

        var query2 = from q in query1
                        join c in db.Classes
                        on new { A = q.CatalogId, B = season, C = year } equals new { A = c.CategoryId, B = (string)splitSem(c.Semester, 0), C = (int)splitSem(c.Semester, 1) }
                        into joined
                        from j in joined.DefaultIfEmpty()
                        select j;

        var query3 = from q in query2
                        join ac in db.AssignmentCategories
                        on  new {A = q.ClassId, B = category } equals new {A = ac.ClassId, B = ac.Name }
                        into joined
                        from j in joined.DefaultIfEmpty()
                        select j;

        var query4 = from q in query3
                        join a in db.Assignments
                        on new { A = q.AssignCatId, B = asgname } equals new { A = a.AssignCatId, B = a.Name }
                        into joined
                        from j in joined.DefaultIfEmpty()
                        select j;

        var query5 = from q in query4
                        join s in db.Submission
                        on new { A = q.AId, B = uid } equals new { A = s.AId, B = s.UId }
                        into joined
                        from j in joined.DefaultIfEmpty()
                        select j;

        if(query5.ToArray().Count() > 0 && query5.SingleOrDefault() != null) //Student has already submitted before, UPDATE
        {
            Submission s = query5.SingleOrDefault();
            s.Contents = contents;
            s.Time = DateTime.Now;

            db.SaveChanges();

            return Json(new { success = true });
        }
        else //Student has not submitted before, ADD
        {
            Submission s = new Submission();
            s.Contents = contents;
            s.AId = (from q in query4 select q.AId).FirstOrDefault();
            s.UId = uid;
            s.Score = 0;
            s.Time = DateTime.Now;

            db.Add(s);
            db.SaveChanges();

            return Json(new { success = true });
        }    
    }

    
    /// <summary>
    /// Enrolls a student in a class.
    /// </summary>
    /// <param name="subject">The department subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester</param>
    /// <param name="year">The year part of the semester</param>
    /// <param name="uid">The uid of the student</param>
    /// <returns>A JSON object containing {success = {true/false}. 
    /// false if the student is already enrolled in the class, true otherwise.</returns>
    public IActionResult Enroll(string subject, int num, string season, int year, string uid)
    {
        var query1 = from cr in db.Courses
                        where cr.Listing == subject && cr.Number == num
                        select cr;

        var query2 = from q in query1
                        join c in db.Classes
                        on new { A = q.CatalogId, B = season, C = year } equals new { A = c.CategoryId, B = (string)splitSem(c.Semester, 0), C = (int)splitSem(c.Semester, 1) }
                        into joined
                        from j in joined.DefaultIfEmpty()
                        select j;

        var query3 = from q in query2
                        join e in db.Enrolled
                        on new { A = q.ClassId, B = uid } equals new { A = e.ClassId, B = e.UId }
                        into joined
                        from j in joined.DefaultIfEmpty()
                        select j;

        if (query3.Count() > 0 && query3.FirstOrDefault() != null)
        {
            return Json(new { success = false });
        }
        else
        {
            var query4 = from q in query2
                            select q;

            Enrolled en = new Enrolled();
            en.UId = uid;
            en.ClassId = (from q in query4 select q.ClassId).FirstOrDefault();
            en.Grade = "--";

            db.Enrolled.Add(en);
            db.SaveChanges();

            return Json(new { success = true });
        } 
    }



    /// <summary>
    /// Calculates a student's GPA
    /// A student's GPA is determined by the grade-point representation of the average grade in all their classes.
    /// Assume all classes are 4 credit hours.
    /// If a student does not have a grade in a class ("--"), that class is not counted in the average.
    /// If a student is not enrolled in any classes, they have a GPA of 0.0.
    /// Otherwise, the point-value of a letter grade is determined by the table on this page:
    /// https://advising.utah.edu/academic-standards/gpa-calculator-new.php
    /// </summary>
    /// <param name="uid">The uid of the student</param>
    /// <returns>A JSON object containing a single field called "gpa" with the number value</returns>
    public IActionResult GetGPA(string uid)
    {     
    var query = from e in db.Enrolled
                where uid == e.UId
                select e.Grade;
        if(query.Count() > 0 && query.FirstOrDefault() != null)
        {
            //dynamic gpa = new JsonResult(calcGPA(query));
            var gpaQuery = from q in query
                            select new
                            {
                                gpa = calcGPA(query)
                            };

            return Json(gpaQuery.FirstOrDefault()); // Takes the first GPA (assumes it is the highest)
        }
        else
        {
            //dynamic gpa = new JsonResult(0.0);
            var gpaQuery = from q in query
                            select new
                            {
                                gpa = 0.0
                            };

            return Json(gpaQuery.SingleOrDefault());
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

            if (seasonYear == 0)
            {
                return s[0];
            }
            else
            {
                return Int32.Parse(s[1]);
            }
        }

    /* Private helper method to calculate the GPA of 
        * a student.
        * 
        * @ret double - GPA of student
        * @param query - grades to calculate GPA
        */
    private double calcGPA(IQueryable<string> query)
    {
        int count = 0;
        double GPA = 0;

        foreach (var grade in query)
        {
            count++;
            switch (grade)
            {
                case "--":
                    count--;
                    break;
                case "A":
                    GPA += 4.0;
                    break;
                case "A-":
                    GPA += 3.7;
                    break;
                case "B+":
                    GPA += 3.3;
                    break;
                case "B":
                    GPA += 3.0;
                    break;
                case "B-":
                    GPA += 2.7;
                    break;
                case "C+":
                    GPA += 2.3;
                    break;
                case "C":
                    GPA += 2.0;
                    break;
                case "C-":
                    GPA += 1.7;
                    break;
                case "D+":
                    GPA += 1.3;
                    break;
                case "D":
                    GPA += 1.0;
                    break;
                case "D-":
                    GPA += 0.7;
                    break;
                case "E":
                    GPA += 0.0;
                    break;
            }
        }
        if(GPA == 0)
            {
                return 0;
            }
        return (GPA / count);
    }
        /*******End code to modify********/

  }
}