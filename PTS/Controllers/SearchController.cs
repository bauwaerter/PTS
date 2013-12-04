using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Core.Domains;
using Service.Interfaces;
using PTS.Infrastructure;
using PTS.Models;
using PTS.Helpers;

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
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult GetTeacherUsers(int jtStartIndex, int jtPageSize, string textSearch = "")
        {
            try
            {
                var teacherList = (from teacher in _teacherUserService.GetTableQuery().Where(a => a.Active)
                                   select new
                                   {
                                       teacher.Id,
                                       teacher.User.FirstName,
                                       teacher.User.LastName,
                                       teacher.User.Email,
                                       teacher.ReviewTeacher,
                                       teacher.ClassRate,
                                       teacher.HourlyRate,
                                   }).ToList();

                
                var records = teacherList.Select(t => new TeacherUserViewModel
                {
                    Id = t.Id,
                    FirstName = t.FirstName,
                    LastName = t.LastName,
                    Email = t.Email,
                    AverageRating = t.ReviewTeacher.FirstOrDefault() !=  null ? Math.Round(t.ReviewTeacher.Average(a => a.Rating), 1).ToString() : "No Ratings",
                    ClassRate = t.ClassRate,
                    HourlyRate = t.HourlyRate
                });

                if(!string.IsNullOrWhiteSpace(textSearch))
                {
                    var oldRecords = records;
                    records = oldRecords.Where(r => 
                                (ListHelper.CheckIndexOf(r.FirstName, textSearch)) || 
                                (ListHelper.CheckIndexOf(r.LastName, textSearch)) ||
                                (ListHelper.CheckIndexOf(r.Email, textSearch)) ||
                                (ListHelper.CheckIndexOf(r.AverageRating, textSearch)) ||
                                (ListHelper.CheckIndexOf(r.ClassRate.ToString(), textSearch)) ||
                                (ListHelper.CheckIndexOf(r.HourlyRate.ToString(), textSearch)));
                }
            
                return Json(new { Result = "OK", Records = records.Skip(jtStartIndex).Take(jtPageSize).ToList(), TotalRecordCount = records.Count() });
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
                
                if (tutor.Schedule != null)
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
                        ScheduleId = tutor.Id,
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
        public ActionResult LoadTeacherMap(int teacherId)
        {
            var user = _userService.GetById(teacherId);
            return View("TeacherMap", user);
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult GetClasses(int jtStartIndex, int jtPageSize, string textSearch = "")
        {
            try
            {
                var classList = (from classes in _classService.GetTableQuery().Where(a => a.Active)
                                 select new
                                 {
                                     classes.Id,
                                     classes.Teacher.User,
                                     classes.ReviewClass,
                                     classes.StartTime,
                                     classes.EndTime,
                                     classes.Duration,
                                     classes.Location,
                                     classes.SubjectID,
                                     classes.Description,
                                     classes.Subject.Name,
                                 }).ToList();

                var records = classList.Select(d => new ClassViewModel
                {
                    Id = d.Id,
                    LocationId = d.Location.Id,
                    SubjectId = d.SubjectID,
                    TeacherName = d.User.FirstName + " " + d.User.LastName,
                    SubjectName = d.Name,
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
                                (ListHelper.CheckIndexOf(r.Description, textSearch)) ||
                                (ListHelper.CheckIndexOf(r.TeacherName, textSearch)) ||
                                (ListHelper.CheckIndexOf(r.AverageRating, textSearch)) ||
                                (ListHelper.CheckIndexOf(r.StartTime.ToString(), textSearch)) ||
                                (ListHelper.CheckIndexOf(r.EndTime.ToString(), textSearch)) ||
                                (ListHelper.CheckIndexOf(r.Duration.ToString(), textSearch)) ||
                                (ListHelper.CheckIndexOf(r.Address, textSearch)) ||
                                (ListHelper.CheckIndexOf(r.City, textSearch)) ||
                                (ListHelper.CheckIndexOf(r.State, textSearch)) ||
                                (ListHelper.CheckIndexOf(r.ZipCode.ToString(), textSearch)) ||
                                (ListHelper.CheckIndexOf(r.Country, textSearch)) ||
                                (ListHelper.CheckIndexOf(r.SubjectName, textSearch)));
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
