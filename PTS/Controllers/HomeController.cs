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
        private readonly IBaseService<StudentUser> _studentUserService;
        private readonly IBaseService<Class> _classService;

        #endregion

        //public HomeController() { }

        //public HomeController(IBaseService<StudentUser> studentUserService)
        //{
        //    _studentUserService = studentUserService;
        //}

        public HomeController(IUserService userService, IBaseService<StudentUser> studentUserService, IBaseService<Class> classService)
        {
            _userService = userService;
            _studentUserService = studentUserService;
            _classService = classService;
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
