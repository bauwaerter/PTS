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
        private readonly IBaseService<Subject> _subjectService;
        

        public SearchController(IBaseService<TeacherUser> teacherUserService, IUserService userService,
                                IBaseService<Class> classService, IBaseService<Location> locationService,
                                IBaseService<ReviewTeacher> reviewTeacherService, IBaseService<Subject> subjectService)
        {
            _teacherUserService = teacherUserService;
            _userService = userService;
            _classService = classService;
            _locationService = locationService;
            _reviewTeacherService = reviewTeacherService;
            _subjectService = subjectService;
           
        }

        //
        // GET: /Search/
        [AllowAnonymous]
        public ActionResult Index(int teacherId = 0)
        {
            ViewBag.Subjects = _subjectService.GetTableQuery().ToList();

            if (teacherId != 0)
            {
                var user = _teacherUserService.GetById(teacherId).User;
                ViewBag.TeacherName = user.FirstName + " " + user.LastName;
                return View();
            }
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult GetTeacherUsers(int jtStartIndex, int jtPageSize, string textSearch = "", double lat1 = 0, double lon1 = 0, int miles = 0)
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
                                       teacher.HourlyRate,
                                       teacher.User.Location.Latitude,
                                       teacher.User.Location.Longitude,
                                   }).ToList();

                

                var records = teacherList.Select(t => new TeacherUserViewModel
                {
                    Id = t.Id,
                    Name = t.FirstName + " " + t.LastName,
                    Email = t.Email,
                    AverageRating = t.ReviewTeacher.FirstOrDefault() !=  null ? Math.Round(t.ReviewTeacher.Average(a => a.Rating), 1).ToString() : "No Ratings",
                    HourlyRate = t.HourlyRate,
                    Latitude = t.Latitude,
                    Longitude = t.Longitude
                });

                if (miles != 0)
                {
                    var oldTeacherList = records;
                    records = oldTeacherList.Where(d => (ListHelper.CalculateDistance(lat1, lon1, d.Latitude, d.Longitude, miles)));
                }


                if(!string.IsNullOrWhiteSpace(textSearch))
                {
                    var oldRecords = records;
                    records = oldRecords.Where(r => 
                                (ListHelper.CheckIndexOf(r.Name, textSearch)) ||
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
        public ActionResult GetTutorSubjects(int tutorUserId)
        {
            try
            {
                var tutor = _teacherUserService.GetById(tutorUserId);
                var subjects = tutor.TeacherOffers.Select(s => s.Subject);

                var records = subjects.Select(s => new TutorSubjectViewModel
                {
                    SubjectId = s.Id,
                    Name = s.Name,
                    Description = s.Description
                });

                return Json(new { Result = "OK", Records = records, TotalRecordCount = records.Count() });
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
        public ActionResult AllTutorsMap()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult AllClassesMap()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult LoadClassesMap(int classesId)
        {
            var classes = _classService.GetById(classesId);
            return View("ClassesMap", classes);
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult GetClasses(int jtStartIndex, int jtPageSize, string textSearch = "", int subjectSearch = 0, double lat1 = 0, double lon1 = 0, int miles = 0)
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
                                     classes.Location.Latitude,
                                     classes.Location.Longitude
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
                    Latitude = d.Latitude,
                    Longitude = d.Longitude
                });

                if (miles != 0)
                {
                    var oldClassesList = records;
                    records = oldClassesList.Where(d => (ListHelper.CalculateDistance(lat1, lon1, d.Latitude, d.Longitude, miles)));
                }

                if (subjectSearch != 0)
                {
                    var subjectSearchRecords = records;
                    records =  subjectSearchRecords.Where(s => (s.SubjectId == subjectSearch));
                }

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
