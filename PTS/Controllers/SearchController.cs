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
                IList<int> userIds = new List<int>();
                IList<TeacherUserViewModel> records = new List<TeacherUserViewModel>();
                foreach (var userId in data)
                {
                    int id = userId.Id;
                    userIds.Add(id);
                    records.Add(new TeacherUserViewModel
                    {
                        Id = userId.Id,
                        UserId = userId.Id,
                        FirstName = _userService.GetById(userIds.LastOrDefault()).FirstName,
                        LastName =  _userService.GetById(userIds.LastOrDefault()).LastName,
                        Email = _userService.GetById(userIds.LastOrDefault()).Email,
                        ClassRate = userId.ClassRate,
                        HourlyRate = userId.HourlyRate
                    });
                    
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
