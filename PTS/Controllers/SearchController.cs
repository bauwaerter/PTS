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
                var users = _userService.GetAll().ToList();
                var teacherUsers = _teacherUserService.GetAll().ToList();

                var data = users.Select(s => new TeacherUserViewModel
                {
                    UserId = s.Id,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    Email = s.Email,

                }).ToList();

                

                return Json(new { Result = "OK", Records = data });
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

                return Json(new { Results = "OK", Record = data });
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
                var data = _classService.GetAll().ToList();

                return Json(new { Results = "OK", Record = data });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
