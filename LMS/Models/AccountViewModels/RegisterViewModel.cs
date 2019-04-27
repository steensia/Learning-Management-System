using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Models.AccountViewModels
{
  public class RegisterViewModel
  {

    public string Role { get; set; }

    public List<SelectListItem> Roles { get; } = new List<SelectListItem>
    {
       new SelectListItem { Value = "Student", Text = "Student" },
       new SelectListItem { Value = "Professor", Text = "Professor" },
       new SelectListItem { Value = "Administrator", Text = "Administrator"  }
    };

    public string Department { get; set; }

    public List<SelectListItem> Departments { get; set; } = new List<SelectListItem>
    {
      new SelectListItem{Value = "None", Text = "NONE"}
    };

    [Required]
    [Display(Name = "First Name")]
    public string FirstName { get; set; }

    [Required]
    [Display(Name = "Last Name")]
    public string LastName { get; set; }

    [Required]
    [Display(Name = "Date of Birth")]
    [DataType(DataType.Date)]
    public System.DateTime DOB { get; set; }

    [Required]
    //[StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }
  }
}
