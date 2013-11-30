using Core.Domains;
using Service.Interfaces;
using Service.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PTS.Views.Review
{
    public class ReviewController : Controller
    {


        #region fields

        //private readonly IBaseService<StudentUser> _studentUserService;
        private readonly IUserService _userService;
        //private readonly IStudentUserService _studentUserService;
        private readonly IBaseService<Enrolled> _enrolledService;
        private readonly IBaseService<StudentUser> _studentUserService;
        private readonly IBaseService<TeacherUser> _teacherUserService;
        private readonly IBaseService<Class> _classService;
        private readonly IBaseService<Location> _locationService;

        #endregion
        //
        // GET: /Review/

        public ReviewController(IBaseService<Enrolled> enrolledService, IUserService userService, IBaseService<StudentUser> studentUserService, 
                                IBaseService<Class> classService, IBaseService<Location> locationService, IBaseService<TeacherUser> teacherUserService)
        {
            _enrolledService = enrolledService;
            
        }

        public ActionResult Index()
        {
            var ClassesTaken = _enrolledService.GetTableQuery().Where(e => e.StudentId == 15).ToList();

            return View();
        }

    }
}
