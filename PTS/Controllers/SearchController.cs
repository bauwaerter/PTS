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

        [HttpPost]
        public ActionResult GetTeacherUsers(string textSearch = "")
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

                if(!string.IsNullOrWhiteSpace(textSearch))
                {
                    var oldRecords = records;
                    records = oldRecords.Where(r => 
                                r.FirstName.Contains(textSearch) || 
                                r.LastName.Contains(textSearch));
                }
            
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
        
        [HttpPost]
        public ActionResult GetClasses(string textSearch = "")
        {
            try
            {
                var records = _classService.GetAll();

                if (!string.IsNullOrWhiteSpace(textSearch))
                {
                    var oldRecords = records;
                    records = oldRecords.Where(r =>
                                r.Description.Contains(textSearch));
                }

                return Json(new { Result = "OK", Records = records });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
