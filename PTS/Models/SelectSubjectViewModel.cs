using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PTS.Models
{
    public class SelectSubjectViewModel
    {
        [Display(Name = "User Role")]
        public int SelectedSubjectId { get; set; }
        public IEnumerable<SelectListItem> Subjects { get; set; }
    }
}