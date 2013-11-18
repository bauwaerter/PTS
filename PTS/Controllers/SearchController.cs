using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Core.Domains;
using Service.Interfaces;
using PTS.Infrastructure;
using PTS.Models;

namespace PTS.Views.Search
{
    public class SearchController : Controller
    {
        private readonly IBaseService<TeacherUser> _teacherUserService;

        public SearchController(IBaseService<TeacherUser> teacherUserService)
        {
            _teacherUserService = teacherUserService;
        }

        //
        // GET: /Search/
        
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetTeacherUsers()
        {
            var data = _teacherUserService.GetAll().ToList();

            return Json(new { Results = "OK", Records = data });
        }
    }
}
