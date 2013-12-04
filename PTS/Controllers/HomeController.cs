using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Core.Domains;
using Service.Interfaces;
using PTS.Infrastructure;
using PTS.Models;







namespace PTS.Controllers
{
    public class HomeController : BaseController
    {
        #region fields

        //private readonly IBaseService<StudentUser> _studentUserService;
        private readonly IUserService _userService;
        //private readonly IStudentUserService _studentUserService;
        //private readonly IBaseService<StudentUser> _studentUserService;
        private readonly IBaseService<Class> _classService;
        private readonly IBaseService<TeacherUser> _teacherUserService;

        #endregion

        //public HomeController() { }

        //public HomeController(IBaseService<StudentUser> studentUserService)
        //{
        //    _studentUserService = studentUserService;
        //}

        public HomeController(IUserService userService, IBaseService<StudentUser> studentUserService, IBaseService<Class> classService,
                              IBaseService<TeacherUser> teacherUserService)
        {
            _userService = userService;
            //_studentUserService = studentUserService;
            _classService = classService;
            _teacherUserService = teacherUserService;
        }

        [AllowAnonymous]
        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";
             

            return View();
        }

        [HttpPost]
        public ActionResult SaveUser(User user)
        {
            try
            {
                user.DOB = Convert.ToDateTime(user.DOB);

                _userService.Insert(user);
                                
                return Json(new { Results = "OK" });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpPost]
        public ActionResult GetRandomTeachers()
        {
            var teachers=_teacherUserService.GetAll();
            var adList = new List<TutorAd>();

            foreach(var t in teachers)
            {
                var rand = new Random();
                var reviews = t.ReviewTeacher;
                if (t.ReviewTeacher.Count > 0)
                {
                    var r = reviews.ToArray()[rand.Next(t.ReviewTeacher.Count)];
                    var ad = new TutorAd
                    {
                        Date = r.Date,
                        Rating = r.Rating,
                        Review = r.Comment,
                        TutorName = t.User.FirstName + " " + t.User.LastName
                    };
                    adList.Add(ad);
                }
            }


            return Json(new { Results = adList });
        }

        [AllowAnonymous]
        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Account()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
