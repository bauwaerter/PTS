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
        private readonly IBaseService<ReviewTeacher> _reviewTeacherService;

        public SearchController(IBaseService<TeacherUser> teacherUserService, IUserService userService,
                                IBaseService<Class> classService, IBaseService<Location> locationService,
                                IBaseService<ReviewTeacher> reviewTeacherService)
        {
            _teacherUserService = teacherUserService;
            _userService = userService;
            _classService = classService;
            _locationService = locationService;
            _reviewTeacherService = reviewTeacherService;   
        }

        //
        // GET: /Search/
        [AllowAnonymous]
        public ActionResult Index()
        {
           
            ViewBag.UserRole = SessionDataHelper.UserRole;
            
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult GetTeacherUsers(int jtStartIndex, int jtPageSize, string textSearch = "")
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
                    AverageRating = t.ReviewTeacher.FirstOrDefault() !=  null ? Math.Round(t.ReviewTeacher.Average(a => a.Rating), 1).ToString() : "No Ratings",
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
            
                return Json(new { Result = "OK", Records = records.Skip(jtStartIndex).Take(jtPageSize).ToList(), TotalRecordCount = records.Count() });
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

        [AllowAnonymous]
        [HttpPost]
        public ActionResult GetTutorAvailibility(int tutorUserId)
        {
            try
            {
                var tutor = _teacherUserService.GetById(tutorUserId);
                var results = new List<object>();
                var zeroTime = new TimeSpan(0);
                
                if (tutor.ScheduleId != null)
                {
                    string[] timeValidation = new string[7];

                    if(tutor.Schedule.SunStartTime.CompareTo(zeroTime) != 0 && tutor.Schedule.SunEndTime.CompareTo(zeroTime) != 0)
                    {
                        timeValidation[0] = (tutor.Schedule.SunStartTime + " - " + tutor.Schedule.SunEndTime);
                    }
                    else
                    {
                        timeValidation[0] = ("N/A");
                    }
                    if(tutor.Schedule.MonStartTime.CompareTo(zeroTime) != 0 && tutor.Schedule.MonEndTime.CompareTo(zeroTime) != 0)
                    {
                        timeValidation[1] = (tutor.Schedule.MonStartTime + " - " + tutor.Schedule.MonEndTime);
                    }
                    else
                    {
                        timeValidation[1] = ("N/A");
                    }
                    if(tutor.Schedule.TuesStartTime.CompareTo(zeroTime) != 0 && tutor.Schedule.TuesEndTime.CompareTo(zeroTime) != 0)
                    {
                        timeValidation[2] = (tutor.Schedule.TuesStartTime + " - " + tutor.Schedule.TuesEndTime);
                    }
                    else
                    {
                        timeValidation[2] = ("N/A");
                    }
                    if(tutor.Schedule.WedStartTime.CompareTo(zeroTime) != 0 && tutor.Schedule.WedEndTime.CompareTo(zeroTime) != 0)
                    {
                        timeValidation[3] = (tutor.Schedule.WedStartTime + " - " + tutor.Schedule.WedEndTime);
                    }
                    else
                    {
                        timeValidation[3] = ("N/A");
                    }
                    if(tutor.Schedule.ThursStartTime.CompareTo(zeroTime) != 0 && tutor.Schedule.ThursEndTime.CompareTo(zeroTime) != 0)
                    {
                        timeValidation[4] = (tutor.Schedule.ThursStartTime + " - " + tutor.Schedule.ThursEndTime);
                    }
                    else
                    {
                        timeValidation[4] = ("N/A");
                    }
                    if(tutor.Schedule.FriStartTime.CompareTo(zeroTime) != 0 && tutor.Schedule.FriEndTime.CompareTo(zeroTime) != 0)
                    {
                        timeValidation[5] = (tutor.Schedule.FriStartTime + " - " + tutor.Schedule.FriEndTime);
                    }
                    else
                    {
                        timeValidation[5] = ("N/A");
                    }
                    if (tutor.Schedule.SatStartTime.CompareTo(zeroTime) != 0 && tutor.Schedule.SatEndTime.CompareTo(zeroTime) != 0)
                    {
                        timeValidation[6] = (tutor.Schedule.SatStartTime + " - " + tutor.Schedule.SatEndTime);
                    }
                    else
                    {
                        timeValidation[6] = ("N/A");
                    }
                    


                    results.Add(new
                    {
                        ScheduleId = tutor.Schedule.Id,
                        Sunday = timeValidation[0],
                        Monday = timeValidation[1],
                        Tuesday = timeValidation[2],
                        Wednesday = timeValidation[3],
                        Thursday = timeValidation[4],
                        Friday = timeValidation[5],
                        Saturday = timeValidation[6],
                    });
                }
                else
                {
                    results.Add(new
                    {
                        ScheduleId = 0,
                        Sunday = "N/A",
                        Monday = "N/A",
                        Tuesday = "N/A",
                        Wednesday = "N/A",
                        Thursday = "N/A",
                        Friday = "N/A",
                        Saturday = "N/A",
                    });
                }
                return Json(new { Result = "OK", Records =  results, TotalRecordCount = results.Count() });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult GetClasses(int jtStartIndex, int jtPageSize, string textSearch = "")
        {
            try
            {
                var data = _classService.GetAll();
                
                var records = data.Select(d => new ClassViewModel
                {
                    Id = d.Id,
                    LocationId = d.LocationId,
                    SubjectId = d.SubjectID != null ? d.SubjectID : null,
                    TeacherName = d.Teacher.User.FirstName + " " + d.Teacher.User.LastName,
                    Description = d.Description,
                    AverageRating = d.ReviewClass.FirstOrDefault() != null ? Math.Round(d.ReviewClass.Average(a => a.Rating), 1).ToString() : "No Ratings",
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

                return Json(new { Result = "OK", Records = records.Skip(jtStartIndex).Take(jtPageSize).ToList(), TotalRecordCount = records.Count() });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
