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
        private readonly IUserService _userService;
        private readonly IBaseService<Class> _classService;

        public SearchController(IBaseService<TeacherUser> teacherUserService, IUserService userService,
                                IBaseService<Class> classService)
        {
            _teacherUserService = teacherUserService;
            _userService = userService;
            _classService = classService;
        }

        //
        // GET: /Search/
        
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetTeacherUsers()
        {
            try
            {
                var data = _teacherUserService.GetAll();

                var records = data.Select(t => new TeacherUserViewModel
                {
                    Id = t.Id,
                    FirstName = t.User.FirstName,
                    LastName = t.User.LastName,
                    Email = t.User.Email,
                    ClassRate = t.ClassRate,
                    HourlyRate = t.HourlyRate
                });
                
                             
                return Json(new { Result = "OK", Records = records });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public ActionResult CreateTeacherUser(TeacherUser teacherUser)
        {
            try
            {
                _teacherUserService.Insert(teacherUser);
                
                var data = teacherUser;

                return Json(new { Result = "OK", Record = data });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public ActionResult GetClasses()
        {
            try
            {
                var data = _classService.GetAll();

                return Json(new { Result = "OK", Records = data });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
