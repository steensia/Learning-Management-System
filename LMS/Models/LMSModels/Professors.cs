using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Professors
    {
        public Professors()
        {
            Classes = new HashSet<Classes>();
        }

        public string UId { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public DateTime? Dob { get; set; }
        public string WorksIn { get; set; }

        public virtual Departments WorksInNavigation { get; set; }
        public virtual ICollection<Classes> Classes { get; set; }
    }
}
