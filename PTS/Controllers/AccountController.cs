using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ClassLibrary1.Helpers;
using Core.Helpers;
using Core.Helpers.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using Newtonsoft.Json.Serialization;
using WebMatrix.WebData;
using PTS.Filters;
using PTS.Models;
using Service.Interfaces;
using Core.Domains;
using PTS.Infrastructure;
using CommonHelper = Core.CommonHelper;
using NewCommonHelper = Core.Helpers.CommonHelper;
using System.Net.Mail;

namespace PTS.Controllers {
    //[Authorize]
    //[InitializeSimpleMembership]
    public class AccountController : BaseController {

        #region fields
        private readonly IBaseService<Class> _classService;
        private readonly IBaseService<Location> _locationService;
        private readonly ILoginService _loginService;
        private readonly IBaseService<Request> _requestService;
        private readonly IBaseService<TeacherUser> _teacherUserService;
        private readonly IBaseService<Teacher_Offers> _teacherOfferService;
        private readonly IUserService _userService;
        private readonly IBaseService<Subject> _subjectService;
        private readonly IBaseService<Class_Meeting_Dates> _classMeetingDatesService;
        private readonly IBaseService<Enrolled> _enrolledService;
        private readonly IBaseService<Tutors> _tutorsService;
        private readonly IBaseService<Payment> _paymentsService;
        private readonly IBaseService<Schedule> _scheduleService;
        private readonly IEmailService _emailService;
        #endregion

        #region constructor
        public AccountController(IBaseService<Schedule> scheduleServie, IBaseService<Payment> paymentsService, IBaseService<Enrolled> enrolledService, IBaseService<Class_Meeting_Dates> classMeetingDatesService, IBaseService<Subject> subjectService, IUserService userService, IBaseService<Class> classService, IBaseService<Location> locationService,
            IBaseService<TeacherUser> teacherUserService, ILoginService loginService, IBaseService<Request> requestService, IBaseService<Teacher_Offers> teacherOfferService, IBaseService<Tutors> tutorsService,
            IBaseService<Schedule> scheduleService, IEmailService emailService) {
            _userService = userService;
            _classService = classService;
            _locationService = locationService;
            _loginService = loginService;
            _requestService = requestService;
            _teacherUserService = teacherUserService;
            _userService = userService;
            _subjectService = subjectService;
            _classMeetingDatesService = classMeetingDatesService;
            _enrolledService = enrolledService;
            _teacherOfferService = teacherOfferService;
            _tutorsService = tutorsService;
            _paymentsService = paymentsService;
            _scheduleService = scheduleService;
            _emailService = emailService;
        }
        #endregion
        //
        // GET: /Account/Login


        [AllowAnonymous]
        public ActionResult Login(string returnUrl) {
            ViewBag.ReturnUrl = returnUrl;
            var model = new LoginModel();
            return View(model);
        }

        //
        // POST: /Account/Login

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginModel loginModel, string returnUrl) {
            try {
                if (_userService.ValidateLogin(loginModel.UserName, loginModel.Password)) {
                    var user = _userService.GetUserByEmail(loginModel.UserName);

                    FormsAuthentication.SetAuthCookie(loginModel.UserName, loginModel.RememberMe);
                    SessionDataHelper.Username = user.Email;
                    SessionDataHelper.UserId = user.Id;
                    SessionDataHelper.UserRole = user.Role;
                    SessionDataHelper.Latitude = user.Location.Latitude;
                    SessionDataHelper.Longitude = user.Location.Longitude;
                    SessionDataHelper.ZipCode = user.Location.ZipCode;
                    SessionDataHelper.SessionId = System.Web.HttpContext.Current.Session.SessionID;

                    if (SessionDataHelper.UserId != 1) {
                        _loginService.LogUser(SessionDataHelper.UserId, SessionDataHelper.SessionId);
                    }

                    if (Url.IsLocalUrl(returnUrl)) {
                        return Redirect(returnUrl);
                    }

                    return RedirectToAction("Index", "Home");
                }
                throw new Exception("Your username/password combination was incorrect");
            } catch (Exception ex) {
                ModelState.AddModelError("Error", ex.Message);
            }
            return View(loginModel);
        }

        public ActionResult SaveUserLocation(Location loc) {
            loc.Id = SessionDataHelper.UserId;

            _locationService.Update(loc);
            return Json(new {
                Result = "OK"
            });
        }

        public ActionResult SaveTeacherUser(TeacherUser teach) {
            teach.Id = SessionDataHelper.UserId;
            teach.ScheduleId = teach.Schedule.Id = _teacherUserService.GetById(SessionDataHelper.UserId).ScheduleId;
            _scheduleService.Update(teach.Schedule);
            _teacherUserService.Update(teach);
            return Json(new {
                Result = "OK"
            });
        }

        public ActionResult SaveUser(User user) {
            try {
                user.Id = SessionDataHelper.UserId;
                var tempUserData = _userService.GetById(user.Id);
                var updateUser = new User {
                    Id = user.Id,
                    Email = user.Email,
                    SSN = tempUserData.SSN,
                    Education = user.Education,
                    DOB = tempUserData.DOB,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    LocationId = tempUserData.LocationId,
                    Major = user.Major,
                    Role = tempUserData.Role,
                    PassWord = tempUserData.PassWord,
                    PasswordSalt = tempUserData.PasswordSalt
                };

                _userService.Update(updateUser);
                return Json(new {
                    Result = "OK"
                });
            } catch (Exception ex) {
                throw new Exception(ex.Message);
            }

        }

        [Authorize]
        public ActionResult ManageAccount() {
            var model = _userService.GetById(SessionDataHelper.UserId);
            var user = new AccountUser();
            var loc = new LocationVM();


            int locid = model.LocationId;
            var getlocation = _locationService.GetById(locid);
            loc = new LocationVM {
                Address = getlocation.Address,
                City = getlocation.City,
                Country = getlocation.Country,
                LocationId = getlocation.Id,
                State = getlocation.State,
                ZipCode = getlocation.ZipCode
            };


            if (model.Role == UserRole.Student) {
                user = new AccountUser {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Education = model.Education,
                    Email = model.Email,
                    Id = model.Id,
                    Major = model.Major,
                    Location = loc,
                    Role = UserRole.Student
                };
            } else if (model.Role == UserRole.Admin) {
                user = new AccountUser {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Id = model.Id,
                    Location = loc,
                    Education = model.Education,
                    Major = model.Major,
                    Role = UserRole.Admin
                };
            } else if (model.Role == UserRole.Teacher) {
                var teacher = _teacherUserService.GetById(SessionDataHelper.UserId);
                user = new AccountUser {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Id = model.Id,
                    Location = loc,
                    Education = model.Education,
                    Major = model.Major,
                    ClassRate = teacher.ClassRate,
                    HourlyRate = teacher.HourlyRate,
                    Role = UserRole.Teacher,
                    Schedule = teacher.Schedule
                };
            }

            return View(user);
        }

        [HttpPost]
        public ActionResult LogOff() {
            FormsAuthentication.SignOut();
            SessionHelper.Abandon();
            Session.Abandon();
            //WebSecurity.Logout();

            FormsAuthentication.RedirectToLoginPage();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Register() {
            var user = new User() {
                DOB = DateTime.Today,
                Role = UserRole.Student,
                Location = new Location()
            };
            return View(user);
        }

        //
        // POST: /Account/Register

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Register(string confirmPassword, User user, TeacherScheduleModel teacherUser, int subject) {
            try {
                if (!CommonHelper.IsValidEmail(user.Email)) {
                    throw new Exception("Username must be a valid email address");
                }
                if (confirmPassword.Equals(user.PassWord, StringComparison.Ordinal)) {
                    var salt = "";
                    var email = user.Email;
                    var hashedPassword = SecurityHelper.HashPassword(user.PassWord, ref salt);
                    user.PassWord = hashedPassword;
                    user.PasswordSalt = salt;
                    _locationService.Insert(user.Location);
                    var location = user.Location;
                    user.LocationId = location.Id;
                    user.Location = null;

                    if (teacherUser.Teacher.HourlyRate != 0 || teacherUser.Teacher.ClassRate != 0) {
                        teacherUser.Teacher.Active = false;
                        teacherUser.Teacher.User = user;
                        user = null;
                        _scheduleService.Insert(teacherUser.Schedule);
                        teacherUser.Teacher.ScheduleId = teacherUser.Schedule.Id;
                        _teacherUserService.Insert(teacherUser.Teacher);
                        //_emailService.SendNewUserEmail(teacherUser.Teacher.User, confirmPassword);
                        teacherUser.Teacher.Schedule = teacherUser.Schedule;
                        var teacherOffer = new Teacher_Offers() {
                            TeacherId = teacherUser.Teacher.Id,
                            SubjectId = subject
                        };
                        _teacherOfferService.Insert(teacherOffer);
                    } else {
                        teacherUser = null;
                        _userService.Insert(user);
                        //_emailService.SendNewUserEmail(user, confirmPassword);
                    }

                    var loginModel = new LoginModel() {
                        UserName = email,
                        Password = confirmPassword,
                        RememberMe = false
                    };


                    RegisterLogin(loginModel);
                    //return RedirectToAction("Index", "Home");
                    return Redirect("/Home/Index");
                }
                throw new Exception("Passwords do not match");
            } catch (Exception ex) {
                Error(ex.Message);
                return View(user);
            }
        }

        [AllowAnonymous]
        public void RegisterLogin(LoginModel loginModel) {
            try {
                    var user = _userService.GetUserByEmail(loginModel.UserName);

                    FormsAuthentication.SetAuthCookie(loginModel.UserName, loginModel.RememberMe);
                    SessionDataHelper.Username = user.Email;
                    SessionDataHelper.UserId = user.Id;
                    SessionDataHelper.UserRole = user.Role;
                    SessionDataHelper.Latitude = user.Location.Latitude;
                    SessionDataHelper.Longitude = user.Location.Longitude;
                    SessionDataHelper.ZipCode = user.Location.ZipCode;
                    SessionDataHelper.SessionId = System.Web.HttpContext.Current.Session.SessionID;

                    if (SessionDataHelper.UserId != 1) {
                        _loginService.LogUser(SessionDataHelper.UserId, SessionDataHelper.SessionId);
                    }

            } catch (Exception ex) {
                ModelState.AddModelError("Error", ex.Message);
            }
        }

        [HttpGet]
        public ActionResult SendEmail(string email) {
            var user = _userService.GetById(SessionDataHelper.UserId);
            var model = new EmailModel {
                To = email,
                From = user.Email,
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult SendEmail(EmailModel email) {
            MailMessage emailSender = new MailMessage();
            emailSender.To.Add(email.To);
            emailSender.Subject = "New Message - PTS: " + email.Subject;
            emailSender.SubjectEncoding = System.Text.Encoding.UTF8;
            emailSender.From = new System.Net.Mail.MailAddress("prospecttutoringsystems@gmail.com", "One Ghost", System.Text.Encoding.UTF8);
            emailSender.Body = "New Message From " + email.From + ".\n\n\n"
                + email.Body;
            emailSender.BodyEncoding = System.Text.Encoding.UTF8;

            //_emailService.SmtpSend(emailSender);
            return RedirectToAction("DisplaySessions", "Account");
        }



        [HttpGet]
        public ActionResult LoadRequestSession(int teacherId) {
            try {
                var model = new RequestModel() {
                    Teacher = _teacherUserService.GetById(teacherId),
                    Request = new Request() {
                        TeacherId = teacherId,
                        StudentId = SessionDataHelper.UserId,
                        Status = "Pending",
                    }
                };
                return View("RegisterForTeacher", model);
            } catch (Exception ex) {
                throw new Exception(ex.Message);
            }

        }

        [HttpPost]
        public ActionResult LoadRequestSession(Request request) {
            try {
                _requestService.Insert(request);
                return RedirectToAction("ProcessTutorPayment", "Payment", new { studentId = request.StudentId, tutorId = request.TeacherId });
            } catch (Exception ex) {
                throw new Exception(ex.Message);
            }
        }

        [Authorize]
        public ActionResult CreateClass() {
            var subjects = _subjectService.GetAll();
            var today = DateTime.Today.DayOfWeek;
            var model = new ClassViewModel {
                SubjectId = 1,
                Subjects = new SelectList(subjects, "Id", "Name")
            };


            return View(model);
        }

        [HttpPost]
        public ActionResult SaveClass(ClassViewModel classModel) {
            classModel.Duration += ((double)DateTime.Parse(classModel.EndTime).Subtract(DateTime.Parse(classModel.StartTime)).TotalMinutes / 60);

            var insertLocation = new Location {
                Address = classModel.Address,
                City = classModel.City,
                Country = classModel.Country,
                ZipCode = classModel.ZipCode,
                State = classModel.State
            };

            _locationService.Insert(insertLocation);


            var insertClass = new Class {
                Description = classModel.Description,
                StartTime = TimeSpan.Parse(classModel.StartTime),
                EndTime = TimeSpan.Parse(classModel.EndTime),
                Duration = classModel.Duration,
                TeacherId = SessionDataHelper.UserId,
                Active = false,
                SubjectID = classModel.SubjectId,
                LocationId = insertLocation.Id
            };
            _classService.Insert(insertClass);


            DateTime start = classModel.DateStart;
            DateTime end = classModel.DateEnd;
            while (start.DayOfYear <= end.DayOfYear) {
                Class_Meeting_Dates insertMeetingDate = new Class_Meeting_Dates();

                if (classModel.Monday && start.DayOfWeek == System.DayOfWeek.Monday) {
                    insertMeetingDate = new Class_Meeting_Dates {
                        ClassId = insertClass.Id,
                        Date = start
                    };
                } else if (classModel.Tuesday && start.DayOfWeek == System.DayOfWeek.Tuesday) {
                    insertMeetingDate = new Class_Meeting_Dates {
                        ClassId = insertClass.Id,
                        Date = start
                    };
                } else if (classModel.Wednesday && start.DayOfWeek == System.DayOfWeek.Wednesday) {
                    insertMeetingDate = new Class_Meeting_Dates {
                        ClassId = insertClass.Id,
                        Date = start
                    };
                } else if (classModel.Thursday && start.DayOfWeek == System.DayOfWeek.Thursday) {
                    insertMeetingDate = new Class_Meeting_Dates {
                        ClassId = insertClass.Id,
                        Date = start
                    };
                } else if (classModel.Friday && start.DayOfWeek == System.DayOfWeek.Friday) {
                    insertMeetingDate = new Class_Meeting_Dates {
                        ClassId = insertClass.Id,
                        Date = start
                    };
                }
                if (insertMeetingDate.ClassId != 0) {
                    _classMeetingDatesService.Insert(insertMeetingDate);
                }
                start = start.AddDays(1);
            }

            return RedirectToAction("DisplayClasses", "Account");
        }

        [Authorize]
        public ActionResult DisplayClasses() {
            var model = "";

            return View(model);
        }

        public ActionResult getClassesToDisplay() {
            var teacherClasses = _classService.GetTableQuery().Where(c => c.TeacherId == SessionDataHelper.UserId);

            var tables = new List<ClassViewModel>();

            foreach (var c in teacherClasses) {
                var meetingDates = _classMeetingDatesService.GetTableQuery().Where(m => m.ClassId == c.Id).OrderBy(o => o.Date);
                var loc = _locationService.GetById(c.Id);
                var teacher = _teacherUserService.GetById(c.TeacherId);
                var classModel = new ClassViewModel {
                    LocationId = c.LocationId,
                    EndTime = c.EndTime.ToString(),
                    StartTime = c.StartTime.ToString(),
                    Duration = c.Duration,
                    Description = c.Description,
                    Id = c.Id,
                    SubjectId = c.SubjectID,
                    TeacherId = c.TeacherId,
                    Status = c.Active ? "Active" : "Pending",

                };
                if (meetingDates.Count() > 0) {
                    classModel.DateStart = meetingDates.First().Date;
                    classModel.DateEnd = meetingDates.OrderByDescending(x => x.Date).First().Date;
                }
                if (loc != null) {
                    classModel.Address = loc.Address;
                    classModel.City = loc.City;
                    classModel.Country = loc.Country;
                    classModel.State = loc.State;
                    classModel.ZipCode = loc.ZipCode;
                }
                if (teacher != null) {
                    classModel.TeacherName = teacher.User.FirstName + " " + teacher.User.LastName;
                }
                tables.Add(classModel);
            }

            return Json(new { Result = "OK", Records = tables });
        }

        [Authorize]
        public ActionResult DisplaySessions() {
            var model = _userService.GetById(SessionDataHelper.UserId);
            return View(model);
        }

        [HttpPost]
        public JsonResult GetSessions() {
            try {
                if (SessionDataHelper.UserRole == UserRole.Student) {
                    var requests = _requestService.GetAll().Where(x => x.StudentId == SessionDataHelper.UserId);
                    var teacher = _teacherUserService.GetAll().Where(x => requests.Select(i => i.TeacherId).Contains(x.Id)).ToList();

                    //var subject = _teacherOfferService.GetAll().Where(x => tutors.Contains(x.TeacherId)).ToList();

                    var results = teacher.Select(p => new {
                        p.Id,
                        p.User.FirstName,
                        p.User.LastName,
                        p.User.Email,
                        Rate = "$" + p.HourlyRate,
                        Role = SessionDataHelper.UserRole,
                        Status = requests.FirstOrDefault(x => x.TeacherId == p.Id).Status,
                        Review = p.ReviewTeacher.SingleOrDefault(x => x.StudentId == SessionDataHelper.UserId),
                        AverageRating = p.ReviewTeacher.FirstOrDefault() != null ? Math.Round(p.ReviewTeacher.Average(a => a.Rating), 1).ToString() : "No Ratings",
                    }).ToArray();
                    return Json(new { Result = "OK", Records = results, TotalRecordCount = requests.Count() });
                } else {
                    var requests = _requestService.GetAll().Where(x => x.TeacherId == SessionDataHelper.UserId);
                    var student = _userService.GetAll().Where(x => requests.Select(i => i.StudentId).Contains(x.Id)).ToList();
                    var user = _teacherUserService.GetById(SessionDataHelper.UserId);

                    var results = student.Select(p => new {
                        p.Id,
                        p.FirstName,
                        p.LastName,
                        p.Email,
                        Rate = "$" + user.HourlyRate,
                        Status = requests.FirstOrDefault(x => x.StudentId == p.Id).Status,
                        RequestId = requests.FirstOrDefault(x => x.StudentId == p.Id).Id,
                        AverageRating = user.ReviewTeacher.FirstOrDefault() != null ? Math.Round(user.ReviewTeacher.Average(a => a.Rating), 1).ToString() : "No Ratings",
                    }).ToArray();
                    return Json(new { Result = "OK", Records = results, TotalRecordCount = results.Count() });
                }
            } catch (Exception ex) {
                throw new Exception(ex.Message);
            }
        }

        public void ApproveRequest(int requestId) {
            var request = _requestService.GetById(requestId);
            request.Status = "Approved";
            _requestService.Update(request);
            var user = _userService.GetById(request.StudentId);
            var teacher = _userService.GetById(request.TeacherId).FirstName;
            //vice.SendApprovedEmail(user, teacher);
        }

        [HttpPost]
        public JsonResult GetSchedule(int otherId) {
            try {
                if (SessionDataHelper.UserRole == UserRole.Teacher) {
                    var schedule = _requestService.GetAll().Where(x => x.TeacherId == SessionDataHelper.UserId && x.StudentId == otherId);
                    var results = schedule.Select(p => new {
                        Sunday = (p.Sunday == false) ? "N/A" : "Scheduled",
                        Monday = (p.Monday == false) ? "N/A" : "Scheduled",
                        Tuesday = (p.Tuesday == false) ? "N/A" : "Scheduled",
                        Wednesday = (p.Wednesday == false) ? "N/A" : "Scheduled",
                        Thursday = (p.Thursday == false) ? "N/A" : "Scheduled",
                        Friday = (p.Friday == false) ? "N/A" : "Scheduled",
                        Saturday = (p.Saturday == false) ? "N/A" : "Scheduled",
                    }).ToArray();

                    return Json(new { Result = "OK", Records = results, TotalRecordCount = results.Count() });
                } else {
                    var schedule = _requestService.GetAll().Where(x => x.StudentId == SessionDataHelper.UserId && x.TeacherId == otherId);
                    var results = schedule.Select(p => new {
                        Sunday = (p.Sunday == false) ? "N/A" : "Scheduled",
                        Monday = (p.Monday == false) ? "N/A" : "Scheduled",
                        Tuesday = (p.Tuesday == false) ? "N/A" : "Scheduled",
                        Wednesday = (p.Wednesday == false) ? "N/A" : "Scheduled",
                        Thursday = (p.Thursday == false) ? "N/A" : "Scheduled",
                        Friday = (p.Friday == false) ? "N/A" : "Scheduled",
                        Saturday = (p.Saturday == false) ? "N/A" : "Scheduled",
                    }).ToArray();

                    return Json(new { Result = "OK", Records = results, TotalRecordCount = results.Count() });
                }
            } catch (Exception ex) {
                throw new Exception(ex.Message);
            }
        }

        public ActionResult GetStudentClassesToDisplay() {
            var enrolled = _enrolledService.GetTableQuery().Where(e => e.StudentId == SessionDataHelper.UserId);


            var tables = new List<ClassViewModel>();

            foreach (var c in enrolled) {
                var enrolledClass = _classService.GetById(c.ClassId);
                if (enrolledClass != null) {
                    var meetingDates = _classMeetingDatesService.GetTableQuery().Where(m => m.ClassId == enrolledClass.Id).OrderBy(o => o.Date);

                    var loc = _locationService.GetById(enrolledClass.Id);
                    var teacher = _teacherUserService.GetById(enrolledClass.TeacherId);
                    var classModel = new ClassViewModel {
                        LocationId = enrolledClass.LocationId,
                        EndTime = enrolledClass.EndTime.ToString(),
                        StartTime = enrolledClass.StartTime.ToString(),
                        Review = enrolledClass.ReviewClass.SingleOrDefault(x => x.StudentId == SessionDataHelper.UserId),
                        AverageRating = enrolledClass.ReviewClass.FirstOrDefault() != null ? Math.Round(enrolledClass.ReviewClass.Average(a => a.Rating), 1).ToString() : "No Ratings",
                        Description = enrolledClass.Description,
                        Email = teacher.User.Email,
                        Id = enrolledClass.Id,
                        SubjectId = enrolledClass.SubjectID,
                        TeacherId = enrolledClass.TeacherId,
                    };
                    if (meetingDates.Count() > 0) {
                        classModel.DateStart = meetingDates.First().Date;
                        classModel.DateEnd = meetingDates.OrderByDescending(x => x.Date).First().Date;
                    }
                    if (loc != null) {
                        classModel.Address = loc.Address;
                        classModel.City = loc.City;
                        classModel.Country = loc.Country;
                        classModel.State = loc.State;
                        classModel.ZipCode = loc.ZipCode;
                    }
                    if (teacher != null) {
                        classModel.TeacherName = teacher.User.FirstName + " " + teacher.User.LastName;
                    }
                    tables.Add(classModel);
                }

            }

            return Json(new { Result = "OK", Records = tables });
        }

        public ActionResult Transactions() {

            return View("");
        }
        [HttpPost]
        public ActionResult GetPayments() {
            var payments = _paymentsService.GetTableQuery().Where(p => p.StudentId == SessionDataHelper.UserId);
            var results = new List<TransactionsVM>();

            foreach (var p in payments) {
                var studName = _userService.GetById(p.StudentId);
                var teacherName = _userService.GetById(p.TeacherId);

                var trans = new TransactionsVM {
                    Amount = p.Amount,
                    Date = p.Date,
                    StudentID = p.StudentId,
                    StudentName = studName.FirstName + " " + studName.LastName,
                    TeacherID = p.TeacherId,
                    TeacherName = teacherName.FirstName + " " + teacherName.LastName,
                    Description = p.Description

                };
                results.Add(trans);
            }

            return Json(new { Result = "OK", Records = results });
        }

        [HttpPost]
        public ActionResult GetPaymentsReceived() {
            var received = _paymentsService.GetTableQuery().Where(p => p.TeacherId == SessionDataHelper.UserId);
            var results = new List<TransactionsVM>();

            foreach (var p in received) {
                var studName = _userService.GetById(p.StudentId);
                var teacherName = _userService.GetById(p.TeacherId);

                var trans = new TransactionsVM {
                    Amount = p.Amount,
                    Date = p.Date,
                    StudentID = p.StudentId,
                    StudentName = studName.FirstName + " " + studName.LastName,
                    TeacherID = p.TeacherId,
                    TeacherName = teacherName.FirstName + " " + teacherName.LastName,
                    Description = p.Description

                };
                results.Add(trans);
            }

            return Json(new { Result = "OK", Records = results });
        }

        public ActionResult Calendar() {

            return View("");
        }

        [HttpPost]
        public ActionResult GetCalendar() {
            var results = new List<CalendarEvent>();
            var enrolled = _enrolledService.GetTableQuery().Where(e => e.StudentId == SessionDataHelper.UserId).ToList();

            foreach (var c in enrolled) {
                var studentClass = _classService.GetById(c.ClassId);
                foreach (var m in studentClass.ClassMeetingDates) {
                    var temp = new CalendarEvent {
                        allday = false,
                        id = c.ClassId,
                        start = m.Date.AddMinutes(studentClass.StartTime.Minutes).ToString(),
                        end = m.Date.AddMinutes(studentClass.EndTime.Minutes).ToString(),
                        title = "Student: " + studentClass.Description
                    };
                    results.Add(temp);
                }
            }

            var classes = _classService.GetTableQuery().Where(c => c.TeacherId == SessionDataHelper.UserId).ToList();

            foreach (var c in classes) {
                foreach (var m in c.ClassMeetingDates) {
                    var temp = new CalendarEvent {
                        allday = false,
                        id = c.Id,
                        start = m.Date.AddMinutes(c.StartTime.Minutes).ToString(),
                        end = m.Date.AddMinutes(c.EndTime.Minutes).ToString(),
                        title = "Student: " + c.Description
                    };
                    results.Add(temp);


                }

            }
            return Json(new { Result = "OK", Records = results });
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ResetPassword(LoginModel model, string returnUrl = "") {
            try {
                //User user = _userService.GetUserByEmail(model.UserName);
                User user = _userService.GetAll().FirstOrDefault(x => x.Email == model.UserName);
                if (user != null) {
                    var salt = "";
                    var tempPassword = NewCommonHelper.GenerateRandomString();
                    var hashedPassword = SecurityHelper.HashPassword(tempPassword, ref salt);
                    user.PassWord = hashedPassword;
                    user.PasswordSalt = salt;
                    var user2 = new User();
                    user2 = user;
                    _userService.Update(user2);
                    //_emailService.SendResetPasswordEmail(user2, tempPassword);
                } else {
                    throw new Exception("Invalid username.");
                }
            } catch (Exception ex) {
                //ModelState.AddModelError("Error", ex.Message);
                //Error("WRONG!");
                return RedirectToAction("Login", ex.Message);
                //return Mode("_ForgotPassword",model);
            }
            return RedirectToAction("Login", "Account");
        }

        [AllowAnonymous]
        public ActionResult LoadForgotPassword() {
            //var model = new UserSignInModel();
            var model = new LoginModel();
            return View(model);
        }

        [AllowAnonymous]
        public bool CheckUsername(string username) {
            var test = _userService.GetUserByEmail(username);
            return (test == null) ? false : true;
        }

        [HttpGet]
        [Authorize]
        public ActionResult ChangePassword() {
            var model = new LocalPasswordModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult ChangePassword(LocalPasswordModel password) {
            var user = _userService.GetById(SessionDataHelper.UserId);
            var salt = "";
            var hashedPassword = SecurityHelper.HashPassword(password.NewPassword, ref salt);
            user.PassWord = hashedPassword;
            user.PasswordSalt = salt;
            _userService.Update(user);
            return RedirectToAction("ManageAccount", "Account");
        }
    }
}

