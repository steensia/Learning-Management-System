using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Controllers
{
  [Authorize(Roles = "Professor")]
  public class ProfessorController : CommonController
  {
    public IActionResult Index()
    {
      return View();
    }

    public IActionResult Students(string subject, string num, string season, string year)
    {
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      ViewData["season"] = season;
      ViewData["year"] = year;
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

    public IActionResult Categories(string subject, string num, string season, string year)
    {
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      ViewData["season"] = season;
      ViewData["year"] = year;
      return View();
    }

    public IActionResult CatAssignments(string subject, string num, string season, string year, string cat)
    {
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      ViewData["season"] = season;
      ViewData["year"] = year;
      ViewData["cat"] = cat;
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

    public IActionResult Submissions(string subject, string num, string season, string year, string cat, string aname)
    {
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      ViewData["season"] = season;
      ViewData["year"] = year;
      ViewData["cat"] = cat;
      ViewData["aname"] = aname;
      return View();
    }

    public IActionResult Grade(string subject, string num, string season, string year, string cat, string aname, string uid)
    {
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      ViewData["season"] = season;
      ViewData["year"] = year;
      ViewData["cat"] = cat;
      ViewData["aname"] = aname;
      ViewData["uid"] = uid;
      return View();
    }

    /*******Begin code to modify********/


    /// <summary>
    /// Returns a JSON array of all the students in a class.
    /// Each object in the array should have the following fields:
    /// "fname" - first name
    /// "lname" - last name
    /// "uid" - user ID
    /// "dob" - date of birth
    /// "grade" - the student's grade in this class
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <returns>The JSON array</returns>
    public IActionResult GetStudentsInClass(string subject, int num, string season, int year)
    {

        var query = from cr in db.Courses
                    join c in db.Classes
                    on new { A = cr.CatalogId, B = cr.Listing, C = cr.Number } equals new { A = c.CategoryId, B = subject, C = (ushort?) num } into joined
                    from j in joined
                    select j;

        var query2 = from q in query
                        join e in db.Enrolled
                        on new { A = q.ClassId, B = (string)splitSem(q.Semester, 0), C = (int)splitSem(q.Semester, 1) } equals new { A = e.ClassId, B = season, C = year }
                        into joined
                        from j in joined.DefaultIfEmpty()
                        select j;

        var query3 = from q in query2
                        join s in db.Students
                        on q.UId equals s.UId
                        select new
                        {
                            fname = s.FName,
                            lname = s.LName,
                            uid = s.UId,
                            dob = s.Dob,
                            grade = q.Grade
                        };

            try
            {
                return Json(query3.ToArray());
            }
            catch
            {
                return Json(null);
            }
    }


    /// <summary>
    /// Returns a JSON array with all the assignments in an assignment category for a class.
    /// If the "category" parameter is null, return all assignments in the class.
    /// Each object in the array should have the following fields:
    /// "aname" - The assignment name
    /// "cname" - The assignment category name.
    /// "due" - The due DateTime
    /// "submissions" - The number of submissions to the assignment
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The name of the assignment category in the class, 
    /// or null to return assignments from all categories</param>
    /// <returns>The JSON array</returns>
    public IActionResult GetAssignmentsInCategory(string subject, int num, string season, int year, string category)
    {
        var query = from cr in db.Courses
                    join c in db.Classes on new { A = cr.CatalogId, B = cr.Listing, C = (int)cr.Number, D = season, E = year }
                    equals new { A = c.CategoryId, B = subject, C = num, D = (string)splitSem(c.Semester, 0), E = (int)splitSem(c.Semester, 1) } into joined
                    from j in joined
                    select j;

        if (category == null)
        {
            var query2 = from q in query
                            join ac in db.AssignmentCategories on q.ClassId equals ac.ClassId into joined
                            from j in joined
                            select j;

            var query3 = from q in query2
                            join a in db.Assignments on q.AssignCatId equals a.AssignCatId into joined
                            from j in joined
                            select j;
            var query0 = from q in query3
                            join s in db.Submission
                            on q.AId equals s.AId
                            select q;

            var query4 = from q in query3
                            select new {
                                aname = q.Name,
                                cname = q.AssignCat.Name,
                                due = q.Due,
                                submissions = (from q0 in query0
                                            where q.AId == q0.AId
                                            select q0).Count()
                        };
            try
            {
                return Json(query4.ToArray());
            }
            catch
            {
                //Do nothing for now
            }
            return Json(null);
        }
        else
        {
            var query2 = from q in query
                        join ac in db.AssignmentCategories on new { A = q.ClassId, B = category } equals new { A = ac.ClassId, B = ac.Name } into joined
                        from j in joined
                        select j;

            var query3 = from q in query2
                            join a in db.Assignments on q.AssignCatId equals a.AssignCatId into joined
                            from j in joined
                            select j;

            var query0 = from q in query3
                            join s in db.Submission
                            on q.AId equals s.AId
                            select q;

            var query4 = from q in query3
                            select new
                            {
                                aname = q.Name,
                                cname = q.AssignCat.Name,
                                due = q.Due,
                                submissions = (from q0 in query0
                                               where q.AId == q0.AId
                                               select q0).Count()
                            };

            try
            {
                return Json(query4.ToArray());
            }
            catch
            {
                //Do nothing for now
            }
            return Json(null);
        }

    }


    /// <summary>
    /// Returns a JSON array of the assignment categories for a certain class.
    /// Each object in the array should have the folling fields:
    /// "name" - The category name
    /// "weight" - The category weight
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The name of the assignment category in the class</param>
    /// <returns>The JSON array</returns>
    public IActionResult GetAssignmentCategories(string subject, int num, string season, int year)
    {

        var query = from cr in db.Courses
                    join c in db.Classes on new { A = cr.CatalogId, B = cr.Listing, C = (int)cr.Number, D = season, E = year }
                    equals new { A = c.CategoryId, B = subject, C = num, D = (string)splitSem(c.Semester, 0), E = (int)splitSem(c.Semester, 1) } into joined
                    from j in joined.DefaultIfEmpty()
                    select j;

        var query2 = from q in query
                        join ac in db.AssignmentCategories on q.ClassId equals ac.ClassId
                        select new
                        {
                            name = ac.Name,
                            weight = ac.Weight
                    };

        return Json(query2.ToArray());
    }

        /// <summary>
        /// Creates a new assignment category for the specified class.
        /// If a category of the given class with the given name already exists, return success = false.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The new category name</param>
        /// <param name="catweight">The new category weight</param>
        /// <returns>A JSON object containing {success = true/false} </returns>
        public IActionResult CreateAssignmentCategory(string subject, int num, string season, int year, string category, int catweight)
        {
        
            var query = from cr in db.Courses
                        join c in db.Classes on new { A = cr.CatalogId, B = cr.Listing, C = (int)cr.Number, D = season, E = year }
                        equals new { A = c.CategoryId, B = subject, C = num, D = (string)splitSem(c.Semester, 0), E = (int)splitSem(c.Semester, 1) } into joined
                        from j in joined
                        select j;

            if (query.SingleOrDefault() == null)
            {
                return Json(new { success = false });
            }

            AssignmentCategories a = new AssignmentCategories();
            a.ClassId = (from q in query select q.ClassId).FirstOrDefault();
            a.Name = category;
            a.Weight = (byte)catweight;

            db.AssignmentCategories.Add(a);
            db.SaveChanges();

            return Json(new { success = true });
        }

    /// <summary>
    /// Creates a new assignment for the given class and category.
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The name of the assignment category in the class</param>
    /// <param name="asgname">The new assignment name</param>
    /// <param name="asgpoints">The max point value for the new assignment</param>
    /// <param name="asgdue">The due DateTime for the new assignment</param>
    /// <param name="asgcontents">The contents of the new assignment</param>
    /// <returns>A JSON object containing success = true/false</returns>
    public IActionResult CreateAssignment(string subject, int num, string season, int year, string category, string asgname, int asgpoints, DateTime asgdue, string asgcontents)
    {
            var query = from asg in db.Assignments
                        where asgname == asg.Name
                        select asg;
            if(query.Count() > 0 && query.SingleOrDefault() != null)
            {
                return Json(new { success = false });
            }

            var query1 = from cr in db.Courses
                    join c in db.Classes on new { A = cr.CatalogId, B = cr.Listing, C = (int)cr.Number, D = season, E = year }
                    equals new { A = c.CategoryId, B = subject, C = num, D = (string)splitSem(c.Semester, 0), E = (int)splitSem(c.Semester, 1) } into joined
                    from j in joined
                    select j;

            var query2 = from q in query1
                         join ac in db.AssignmentCategories on new { A = q.ClassId, B = category } equals new { A = ac.ClassId, B = ac.Name } into joined
                         from j in joined.DefaultIfEmpty()
                         select j.AssignCatId;

            Assignments a = new Assignments();
            a.AssignCatId = query2.SingleOrDefault();
            a.Name = asgname;
            a.Points = (uint)asgpoints;
            a.Due = asgdue;
            a.Contents = asgcontents;

            db.Assignments.Add(a);
            db.SaveChanges();


            //Update Grades for all students
            var updateGrades = from e in db.Enrolled
                               where query1.FirstOrDefault().ClassId == e.ClassId
                               select e.UId;

            foreach(string u in updateGrades)
            {
                //Requery for where the students grade is in the enrolled table
                var gradeQuery = from e in db.Enrolled
                                 where e.UId == u && query1.FirstOrDefault().ClassId == e.ClassId
                                 select e;

                    Enrolled eObj = gradeQuery.SingleOrDefault();
                    eObj.Grade = updateGrade(subject, num, season, year, u);
                    db.SaveChanges(); //Resave changes with new grade
            }


            return Json(new { success = true });
    }


    /// <summary>
    /// Gets a JSON array of all the submissions to a certain assignment.
    /// Each object in the array should have the following fields:
    /// "fname" - first name
    /// "lname" - last name
    /// "uid" - user ID
    /// "time" - DateTime of the submission
    /// "score" - The score given to the submission
    /// 
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The name of the assignment category in the class</param>
    /// <param name="asgname">The name of the assignment</param>
    /// <returns>The JSON array</returns>
    public IActionResult GetSubmissionsToAssignment(string subject, int num, string season, int year, string category, string asgname)
    {

        var query = from cr in db.Courses
                    join c in db.Classes on new { A = cr.CatalogId, B = cr.Listing, C = (int)cr.Number, D = season, E = year }
                    equals new { A = c.CategoryId, B = subject, C = num, D = (string)splitSem(c.Semester, 0), E = (int)splitSem(c.Semester, 1) } into joined
                    from j in joined
                    select j;

        var query2 = from q in query
                        join ac in db.AssignmentCategories on new { A = q.ClassId, B = category } equals new { A = ac.ClassId, B = ac.Name } into joined
                        from j in joined
                        select j;

        var query3 = from q in query2
                    join a in db.Assignments on new { A = q.AssignCatId, B = asgname } equals new { A = a.AssignCatId, B = a.Name}  into joined
                    from j in joined
                    select j;

        var query4 = from q in query3
                        join s in db.Submission on q.AId equals s.AId into joined
                        from j in joined
                        select new
                        {
                            fname = j.U.FName,
                            lname = j.U.LName,
                            uid = j.UId,
                            time = j.Time,
                            score = j.Score
                        };

            return Json(query4.ToArray());
 
    }


    /// <summary>
    /// Set the score of an assignment submission
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The name of the assignment category in the class</param>
    /// <param name="asgname">The name of the assignment</param>
    /// <param name="uid">The uid of the student who's submission is being graded</param>
    /// <param name="score">The new score for the submission</param>
    /// <returns>A JSON object containing success = true/false</returns>
    public IActionResult GradeSubmission(string subject, int num, string season, int year, string category, string asgname, string uid, int score)
    {

        var query = from cr in db.Courses
                    join c in db.Classes on new { A = cr.CatalogId, B = cr.Listing, C = (int)cr.Number, D = season, E = year }
                    equals new { A = c.CategoryId, B = subject, C = num, D = (string)splitSem(c.Semester, 0), E = (int)splitSem(c.Semester, 1) } into joined
                    from j in joined
                    select j;

        var query2 = from q in query
                        join ac in db.AssignmentCategories on new { A = q.ClassId, B = category } equals new { A = ac.ClassId, B = ac.Name } into joined
                        from j in joined.DefaultIfEmpty()
                        select j;

        var query3 = from q in query2
                        join a in db.Assignments on new { A = q.AssignCatId, B = asgname } equals new { A = a.AssignCatId, B = a.Name } into joined
                        from j in joined.DefaultIfEmpty()
                        select j;

        var query4 = from q in query3
                        join s in db.Submission on new { A = q.AId, B = uid } equals new { A = s.AId, B = s.UId } into joined
                        from j in joined.DefaultIfEmpty()
                        select j;

            Submission sub;
            try
            {
                sub = query4.SingleOrDefault();
            }
            catch
            {
                sub = null;
            }

        if (sub != null)
        {
            sub.Score = (uint)score;

            db.SaveChanges(); //Need to save changes before so our grading is accurate and not one save behind.

                var gradeQuery = from e in db.Enrolled
                                 where e.UId == uid && query.FirstOrDefault().ClassId == e.ClassId
                                 select e;

                    Enrolled eObj = gradeQuery.SingleOrDefault();
                    eObj.Grade = updateGrade(subject, num, season, year, uid);
                    db.SaveChanges(); //Resave changes with new grade
  
                return Json(new { success = true });
                }
        return Json(new { success = false });
    }


    /// <summary>
    /// Returns a JSON array of the classes taught by the specified professor
    /// Each object in the array should have the following fields:
    /// "subject" - The subject abbreviation of the class (such as "CS")
    /// "number" - The course number (such as 5530)
    /// "name" - The course name
    /// "season" - The season part of the semester in which the class is taught
    /// "year" - The year part of the semester in which the class is taught
    /// </summary>
    /// <param name="uid">The professor's uid</param>
    /// <returns>The JSON array</returns>
    public IActionResult GetMyClasses(string uid)
    {
        var query = from c in db.Classes
                    join cr in db.Courses on new { A = c.CategoryId, B = c.TaughtBy} equals new { A = cr.CatalogId, B = uid} 
                    select new
                    {
                        subject = cr.Listing,
                        number = cr.Number,
                        name = cr.Name,
                        season = splitSem(c.Semester, 0),
                        year = splitSem(c.Semester, 1),
                    };

        return Json(query.ToArray());
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

        /// <summary>
        /// This method takes in a query and returns a letter grade char.
        /// </summary>
        /// <returns>A Character representing a letter grade</returns>
     private string updateGrade(string subject, int num, string season, int year, string uid)
        {
            double pointsEarned = 0; 
            double totalPointsPossible = 0;
            double weightSum = 0;
            double finalPercentage;

            double scalingFactor;

            double? runningAssignmentWeightTotal = 0;

            var query = from cr in db.Courses
                        join c in db.Classes on new { A = cr.CatalogId, B = cr.Listing, C = (int)cr.Number, D = season, E = year }
                        equals new { A = c.CategoryId, B = subject, C = num, D = (string)splitSem(c.Semester, 0), E = (int)splitSem(c.Semester, 1) } into joined
                        from j in joined
                        select j;

            var query2 = from q in query
                         select q.AssignmentCategories;

         
            foreach (AssignmentCategories ac in query2.FirstOrDefault())
            {
                var assignmentsQuery = from a in db.Assignments
                                       where a.AssignCatId == ac.AssignCatId
                                       select a;

                if (assignmentsQuery.FirstOrDefault() != null)
                {
                    weightSum += (double)ac.Weight;

                    
                    foreach (Assignments a in assignmentsQuery)
                    {
                        totalPointsPossible += (double)a.Points;

                        var submissionQuery = from s in db.Submission
                                              where a.AId == s.AId && uid == s.UId
                                              select s;
                        foreach (Submission s in submissionQuery) //For each unneccesary but protects us from accessing null data
                        {
                            pointsEarned += (double)s.Score;
                        }
                    }

                    runningAssignmentWeightTotal += (ac.Weight * (pointsEarned / totalPointsPossible));

                    //Reset these values for next Assignment Category
                    pointsEarned = 0;
                    totalPointsPossible = 0;
                }
            }

            //Compute scaling factor
            scalingFactor = 100 / weightSum;

            finalPercentage = scalingFactor * (double)runningAssignmentWeightTotal;

            if (finalPercentage >= 93)
            {
                return "A";
            }
            else if (finalPercentage >= 90)
            {
                return "A-";
            }
            else if (finalPercentage >= 87)
            {
                return "B+";
            }
            else if (finalPercentage >= 83)
            {
                return "B";
            }
            else if (finalPercentage >= 80)
            {
                return "B-";
            }
            else if (finalPercentage >= 77)
            {
                return "C+";
            }
            else if (finalPercentage >= 73)
            {
                return "C";
            }
            else if (finalPercentage >= 70)
            {
                return "C-";
            }
            else if (finalPercentage >= 67)
            {
                return "D+";
            }
            else if (finalPercentage >= 63)
            {
                return "D";
            }
            else if (finalPercentage >= 60)
            {
                return "D-";
            }
            else return "E";
        }

    /*******End code to modify********/

    }
}