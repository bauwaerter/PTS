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
        private readonly IBaseService<Location> _locationService;

        public SearchController(IBaseService<TeacherUser> teacherUserService, IUserService userService,
                                IBaseService<Class> classService, IBaseService<Location> locationService)
        {
            _teacherUserService = teacherUserService;
            _userService = userService;
            _classService = classService;
            _locationService = locationService;
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
        public ActionResult GetTutorAvailibility(int tutorUserId)
        {
            try
            {
                var schedule = _teacherUserService.GetById(tutorUserId).Schedule;
                var data = new TutorAvailabilityViewModel
                {
                    ScheduleId = schedule.Id,
                    Sunday = schedule.SunStartTime + " - " + schedule.SunEndTime,
                    Monday = schedule.MonStartTime + " - " + schedule.MonEndTime,
                    Tuesday = schedule.TuesStartTime + " - " + schedule.TuesEndTime,
                    Wednesday = schedule.WedStartTime + " - " + schedule.WedEndTime,
                    Thursday = schedule.ThursStartTime + " - " + schedule.ThursEndTime,
                    Friday = schedule.FriStartTime + " - " + schedule.FriEndTime,
                    Saturday = schedule.SatStartTime + " - " + schedule.SatEndTime,
                };

                return Json(new { Result = "OK", Records =  data });
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
                var data = _classService.GetAll();

                var records = data.Select(d => new ClassViewModel
                {
                    Id = d.Id,
                    LocationId = d.LocationId,
                    SubjectId = d.SubjectID,
                    Description = d.Description,
                    StartTime = d.StartTime.ToString(),
                    EndTime = d.EndTime.ToString(),
                    Duration = d.Duration,
                    City = d.Location.City,
                    State = d.Location.State,
                    Address = d.Location.Address,
                    ZipCode = d.Location.ZipCode,
                    Country = d.Location.Country,
                });

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
