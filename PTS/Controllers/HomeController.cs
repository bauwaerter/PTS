using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Core.Domains;
using Service.Interfaces;
using System.Web.Mvc;
using PTS.Infrastructure;
using Service.Interfaces;
using PTS.Models;

namespace PTS.Controllers
{
    public class HomeController : BaseController
    {
        #region fields

        //private readonly IBaseService<StudentUser> _studentUserService;
        private readonly IUserService _userService;

        #endregion

        //public HomeController() { }

        //public HomeController(IBaseService<StudentUser> studentUserService)
        //{
        //    _studentUserService = studentUserService;
        //}

        public HomeController(
            IUserService userService)
        {
            _userService = userService;
        }

        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            //var studentUser = new StudentUser{
            //    Education = "BSCS",
            //    Major = "CS"
                
            //};
            try
            {
                var user = new User
                {
                    FirstName = "first",
                    LastName = "last",
                    DOB = DateTime.Now,
                    SSN = 123456789,
                    Email = "test@test.com",
                    PassWord = "TestPassword"
                };

                _userService.Save(user);

                var users = _userService.GetAll();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            //_studentUserService.Insert(studentUser);

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
