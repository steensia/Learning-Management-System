using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LMS.Models;
using Microsoft.AspNetCore.Authorization;

namespace LMS.Controllers
{
  public class HomeController : CommonController
  {

    public IActionResult Index()
    {
      if(User.IsInRole("Student"))
      {
        return Redirect("/Student/Index");
      }
      if(User.IsInRole("Professor"))
      {
        return Redirect("/Professor/Index");
      }
      if(User.IsInRole("Administrator"))
      {
        return Redirect("/Administrator/Index");
      }

      return View();
    }


    public IActionResult About()
    {
      ViewData["Message"] = "Your application description page.";

      return View();
    }

    [Authorize(Roles = "Administrator")]
    public IActionResult Contact()
    {
      ViewData["Message"] = "Your contact page.";

      return View();
    }


    public IActionResult Error()
    {
      return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
  }
}
