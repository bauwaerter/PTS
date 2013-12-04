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

namespace PTS.Controllers
{
    //[Authorize]
    //[InitializeSimpleMembership]
    public class AccountController : BaseController
    {

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
        public AccountController(IBaseService<Schedule> scheduleServie, IBaseService<Payment> paymentsService, IBaseService<Enrolled> enrolledService, IBaseService<Class_Meeting_Dates> classMeetingDatesService, IBaseService<Subject> subjectService, IUserService userService, IBaseService<StudentUser> studentUserService, IBaseService<Class> classService, IBaseService<Location> locationService, 
            IBaseService<TeacherUser> teacherUserService, ILoginService loginService, IBaseService<Request> requestService,IBaseService<Teacher_Offers> teacherOfferService, IBaseService<Tutors> tutorsService,
            IBaseService<Schedule> scheduleService, IEmailService emailService)
        {
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
            try{
                if (_userService.ValidateLogin(loginModel.UserName, loginModel.Password)){
                    var user = _userService.GetUserByEmail(loginModel.UserName);

                    FormsAuthentication.SetAuthCookie(loginModel.UserName, loginModel.RememberMe);
                    SessionDataHelper.Username = user.Email;
                    SessionDataHelper.UserId = user.Id;
                    SessionDataHelper.UserRole = user.Role;
                    SessionDataHelper.Latitude = user.Location.Latitude;
                    SessionDataHelper.Longitude = user.Location.Longitude;
                    SessionDataHelper.ZipCode = user.Location.ZipCode;
                    SessionDataHelper.SessionId = System.Web.HttpContext.Current.Session.SessionID;

                    if (SessionDataHelper.UserId != 1){
                        _loginService.LogUser(SessionDataHelper.UserId, SessionDataHelper.SessionId);
                    }

                    if (Url.IsLocalUrl(returnUrl)){
                        return Redirect(returnUrl);
                    }

                    return RedirectToAction("Index", "Home");
                }
                throw new Exception("Your username/password combination was incorrect");
            }catch (Exception ex){
                ModelState.AddModelError("Error", ex.Message);
            }
            return View(loginModel);
        }

        public ActionResult SaveUserLocation(Location loc)
        {
            loc.Id = SessionDataHelper.UserId;

            _locationService.Update(loc);
            return Json(new
            {
                Result = "OK"
            });
        }

        public ActionResult SaveTeacherUser(TeacherUser teach)
        {
            teach.Id = SessionDataHelper.UserId;
            teach.ScheduleId = teach.Schedule.Id =_teacherUserService.GetById(SessionDataHelper.UserId).ScheduleId;
            _scheduleService.Update(teach.Schedule);
            _teacherUserService.Update(teach);
            return Json(new
            {
                Result = "OK"
            });
        }

        public ActionResult SaveUser(User user)
        {
            try
            {
                user.Id = SessionDataHelper.UserId;
                var tempUserData=_userService.GetById(user.Id);
                var updateUser = new User
                {
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
                return Json(new
                {
                    Result="OK"
                });
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
            
        }

        [Authorize]
        public ActionResult ManageAccount()
        {
            var model = _userService.GetById(SessionDataHelper.UserId);
            var user = new AccountUser();
            var loc = new LocationVM();
                       
            
           int locid = model.LocationId;
           var getlocation = _locationService.GetById(locid);
           loc = new LocationVM 
                {
                    Address = getlocation.Address,
                    City = getlocation.City,
                    Country = getlocation.Country,
                    LocationId = getlocation.Id,
                    State = getlocation.State,
                    ZipCode = getlocation.ZipCode
                };
            

            if(model.Role == UserRole.Student){
                user = new AccountUser
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Education = model.Education,
                    Email = model.Email,
                    Id = model.Id,
                    Major = model.Major,
                    Location = loc,
                    Role = UserRole.Student
                };
            }
            else if (model.Role == UserRole.Admin)
            {
                user = new AccountUser
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Id = model.Id,
                    Location = loc,
                    Education = model.Education,
                    Major = model.Major,
                    Role = UserRole.Admin
                };
            }
            else if (model.Role == UserRole.Teacher)
            {
                var teacher = _teacherUserService.GetById(SessionDataHelper.UserId);
                user = new AccountUser
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Id = model.Id,
                    Location = loc,
                    Education = model.Education,
                    Major = model.Major,
                    ClassRate= teacher.ClassRate,
                    HourlyRate=teacher.HourlyRate,
                    Role = UserRole.Teacher,
                    Schedule= teacher.Schedule
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
        public ActionResult Register(){
            var user = new User(){
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
                        _emailService.SendNewUserEmail(user, confirmPassword);
                        teacherUser.Teacher.Schedule = teacherUser.Schedule;
                        var teacherOffer = new Teacher_Offers(){
                            TeacherId = teacherUser.Teacher.Id,
                            SubjectId = subject
                        };
                        _teacherOfferService.Insert(teacherOffer);
                    }
                    else{
                        teacherUser = null;
                        _userService.Insert(user);
                        _emailService.SendNewUserEmail(user, confirmPassword);
                    }

                    var loginModel = new LoginModel(){
                        UserName = email,
                        Password = confirmPassword,
                        RememberMe = false
                    };

                    
                    Login(loginModel, "");
                    return RedirectToAction("Index", "Home");
                }
            throw new Exception("Passwords do not match");
            } catch (Exception ex) {
                Error(ex.Message);
                return View(user);
            }
        }

        [HttpGet]
        public ActionResult LoadRequestSession(int teacherId)
        {
            try{
                var model = new RequestModel(){
                    Teacher = _teacherUserService.GetById(teacherId),
                    Request = new Request(){
                        TeacherId = teacherId,
                        StudentId = SessionDataHelper.UserId,
                        Status = "Pending",
                    }
                };
                return View("RegisterForTeacher", model);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        [HttpPost]
        public ActionResult LoadRequestSession(Request request) {
            try{
                _requestService.Insert(request);
                return RedirectToAction("ProcessTutorPayment", "Payment", new {studentId = request.StudentId, tutorId = request.TeacherId});
            } catch (Exception ex) {
                throw new Exception(ex.Message);
            }
        }

        [Authorize]
        public ActionResult CreateClass()
        {
            var subjects = _subjectService.GetAll();
            var today=DateTime.Today.DayOfWeek;
            var model = new ClassViewModel
            {
                SubjectId = 1,
                Subjects = new SelectList(subjects, "Id", "Name")
            };
            

            return View(model);
        }

        [HttpPost]
        public ActionResult SaveClass(ClassViewModel classModel)
        {
            classModel.Duration += ((double)DateTime.Parse(classModel.EndTime).Subtract(DateTime.Parse(classModel.StartTime)).TotalMinutes / 60);
            
            var insertLocation = new Location
            {
                Address = classModel.Address,
                City = classModel.City,
                Country = classModel.Country,
                ZipCode = classModel.ZipCode,
                State = classModel.State
            };

            _locationService.Insert(insertLocation);


            var insertClass = new Class
            {
                Description = classModel.Description,
                StartTime = TimeSpan.Parse(classModel.StartTime),
                EndTime = TimeSpan.Parse(classModel.EndTime),
                Duration = classModel.Duration,
                TeacherId= SessionDataHelper.UserId,
                Active=false,
                SubjectID=classModel.SubjectId,
                LocationId=insertLocation.Id
            };
            _classService.Insert(insertClass);


            DateTime start = classModel.DateStart;
            DateTime end = classModel.DateEnd;
            while (start.DayOfYear <= end.DayOfYear)
            {
                Class_Meeting_Dates insertMeetingDate = new Class_Meeting_Dates();

                if (classModel.Monday && start.DayOfWeek == System.DayOfWeek.Monday)
                {
                    insertMeetingDate = new Class_Meeting_Dates
                    {
                        ClassId = insertClass.Id,
                        Date = start
                    };
                }
                else if (classModel.Tuesday && start.DayOfWeek == System.DayOfWeek.Tuesday)
                {
                    insertMeetingDate = new Class_Meeting_Dates
                    {
                        ClassId = insertClass.Id,
                        Date = start
                    };
                }
                else if (classModel.Wednesday && start.DayOfWeek == System.DayOfWeek.Wednesday)
                {
                    insertMeetingDate = new Class_Meeting_Dates
                    {
                        ClassId = insertClass.Id,
                        Date = start
                    };
                }
                else if (classModel.Thursday && start.DayOfWeek == System.DayOfWeek.Thursday)
                {
                    insertMeetingDate = new Class_Meeting_Dates
                    {
                        ClassId = insertClass.Id,
                        Date = start
                    };
                }
                else if (classModel.Friday && start.DayOfWeek == System.DayOfWeek.Friday)
                {
                    insertMeetingDate = new Class_Meeting_Dates
                    {
                        ClassId = insertClass.Id,
                        Date = start
                    };
                }
                if(insertMeetingDate.ClassId!=0)
                { 
                    _classMeetingDatesService.Insert(insertMeetingDate);
                }
                start=start.AddDays(1);
            }

            return RedirectToAction("DisplayClasses","Account");
        }

        [Authorize]
        public ActionResult DisplayClasses()
        {
            var model = "";

            return View(model);
        }

        public ActionResult getClassesToDisplay()
        {
            var teacherClasses = _classService.GetTableQuery().Where(c => c.TeacherId == SessionDataHelper.UserId);
            
            var tables = new List<ClassViewModel>();

            foreach (var c in teacherClasses)
            {
                var meetingDates = _classMeetingDatesService.GetTableQuery().Where(m => m.ClassId==c.Id).OrderBy(o=>o.Date);
                var loc = _locationService.GetById(c.Id);
                var teacher = _teacherUserService.GetById(c.TeacherId);
                var classModel = new ClassViewModel
                {
                    LocationId = c.LocationId,
                    EndTime = c.EndTime.ToString(),
                    StartTime = c.StartTime.ToString(),
                    Duration = c.Duration,
                    Description = c.Description,
                    Id = c.Id,
                    SubjectId = c.SubjectID,
                    TeacherId=c.TeacherId,

                };
                if(meetingDates.Count()>0)
                {
                    classModel.DateStart = meetingDates.First().Date;
                    classModel.DateEnd = meetingDates.OrderByDescending(x=>x.Date).First().Date;
                }
                if(loc!=null)
                {
                    classModel.Address=loc.Address;
                    classModel.City=loc.City;
                    classModel.Country=loc.Country;
                    classModel.State=loc.State;
                    classModel.ZipCode = loc.ZipCode;
                }
                if(teacher!=null)
                {
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
        public JsonResult GetSessions(){
            try{
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
                        Status = requests.FirstOrDefault(x => x.TeacherId == p.Id).Status
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
                        Rate = "$"+ user.HourlyRate,
                        Status = requests.FirstOrDefault(x => x.StudentId == p.Id).Status,
                        RequestId = requests.FirstOrDefault(x => x.StudentId == p.Id).Id
                    }).ToArray();
                    return Json(new { Result = "OK", Records = results, TotalRecordCount = results.Count() });
                }
            }
            catch (Exception ex){
                throw new Exception(ex.Message);
            }
        }

        public void ApproveRequest(int requestId) {
            var request = _requestService.GetById(requestId);
            request.Status = "Approved";
            _requestService.Update(request);
            var user = _userService.GetById(request.StudentId);
            var teacher = _userService.GetById(request.TeacherId).FirstName;
            _emailService.SendApprovedEmail(user, teacher);
        }

        [HttpPost]
        public JsonResult GetSchedule() {
            try {
                var schedule = _requestService.GetAll().Where(x => x.Status == "Approved");
                var results = schedule.Select(p => new{
                    Sunday = (p.Sunday == false) ? "N/A" : "Scheduled",
                    Monday = (p.Monday == false) ? "N/A" : "Scheduled",
                    Tuesday = (p.Tuesday == false) ? "N/A" : "Scheduled",
                    Wednesday = (p.Wednesday == false) ? "N/A" : "Scheduled",
                    Thursday = (p.Thursday == false) ? "N/A" : "Scheduled",
                    Friday = (p.Friday == false) ? "N/A" : "Scheduled",
                    Saturday = (p.Saturday == false) ? "N/A" : "Scheduled",
                }).ToArray();
                
                return Json(new { Result = "OK", Records = results, TotalRecordCount = results.Count() });
            } catch (Exception ex) {
                throw new Exception(ex.Message);
            }

        } 

        public ActionResult GetStudentClassesToDisplay()
        {
            var enrolled = _enrolledService.GetTableQuery().Where(e => e.StudentId == SessionDataHelper.UserId);


            var tables = new List<ClassViewModel>();

            foreach (var c in enrolled)
            {
                var enrolledClass = _classService.GetById(c.ClassId);
                if (enrolledClass != null)
                {
                    var meetingDates = _classMeetingDatesService.GetTableQuery().Where(m => m.ClassId == enrolledClass.Id).OrderBy(o => o.Date);

                    var loc = _locationService.GetById(enrolledClass.Id);
                    var teacher = _teacherUserService.GetById(enrolledClass.TeacherId);
                    var classModel = new ClassViewModel
                    {
                        LocationId = enrolledClass.LocationId,
                        EndTime = enrolledClass.EndTime.ToString(),
                        StartTime = enrolledClass.StartTime.ToString(),
                        Duration = enrolledClass.Duration,
                        Description = enrolledClass.Description,
                        Id = enrolledClass.Id,
                        SubjectId = enrolledClass.SubjectID,
                        TeacherId = enrolledClass.TeacherId,

                    };
                    if (meetingDates.Count() > 0)
                    {
                        classModel.DateStart = meetingDates.First().Date;
                        classModel.DateEnd = meetingDates.OrderByDescending(x => x.Date).First().Date;
                    }
                    if (loc != null)
                    {
                        classModel.Address = loc.Address;
                        classModel.City = loc.City;
                        classModel.Country = loc.Country;
                        classModel.State = loc.State;
                        classModel.ZipCode = loc.ZipCode;
                    }
                    if (teacher != null)
                    {
                        classModel.TeacherName = teacher.User.FirstName + " " + teacher.User.LastName;
                    }
                    tables.Add(classModel);
                }
                
            }

            return Json(new { Result = "OK", Records = tables });
        }

        public ActionResult Transactions()
        {

            return View("");
        }
        [HttpPost]
        public ActionResult GetPayments()
        {
            var payments = _paymentsService.GetTableQuery().Where(p => p.StudentId == SessionDataHelper.UserId);
            var results = new List<TransactionsVM>();

            foreach (var p in payments)
            {
                var studName=_userService.GetById(p.StudentId);
                var teacherName =_userService.GetById(p.TeacherId);
                
                var trans = new TransactionsVM
                {
                    Amount=p.Amount,
                    Date=p.Date,
                    StudentID=p.StudentId,
                    StudentName=studName.FirstName+ " " + studName.LastName,
                    TeacherID=p.TeacherId,
                    TeacherName=teacherName.FirstName+ " " + teacherName.LastName,
                    Description=p.Description
                
                };
                results.Add(trans);
            }

            return Json(new { Result = "OK", Records = results});
        }

        [HttpPost]
        public ActionResult GetPaymentsReceived()
        {
            var received = _paymentsService.GetTableQuery().Where(p => p.TeacherId == SessionDataHelper.UserId);
            var results = new List<TransactionsVM>();

            foreach (var p in received)
            {
                var studName = _userService.GetById(p.StudentId);
                var teacherName = _userService.GetById(p.TeacherId);

                var trans = new TransactionsVM
                {
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

        //
        // POST: /Account/Disassociate

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Disassociate(string provider, string providerUserId)
        //{
        //    string ownerAccount = OAuthWebSecurity.GetUserName(provider, providerUserId);
        //    ManageMessageId? message = null;

        //    // Only disassociate the account if the currently logged in user is the owner
        //    if (ownerAccount == User.Identity.Name)
        //    {
        //        // Use a transaction to prevent the user from deleting their last login credential
        //        using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
        //        {
        //            bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
        //            if (hasLocalAccount || OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name).Count > 1)
        //            {
        //                OAuthWebSecurity.DeleteAccount(provider, providerUserId);
        //                scope.Complete();
        //                message = ManageMessageId.RemoveLoginSuccess;
        //            }
        //        }
        //    }

        //    return RedirectToAction("Manage", new { Message = message });
        //}

        //
        // GET: /Account/Manage

        //public ActionResult Manage(ManageMessageId? message)
        //{
        //    ViewBag.StatusMessage =
        //        message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
        //        : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
        //        : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
        //        : "";
        //    ViewBag.HasLocalPassword = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
        //    ViewBag.ReturnUrl = Url.Action("Manage");
        //    return View();
        //}

        //
        // POST: /Account/Manage

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Manage(LocalPasswordModel model)
        //{
        //    bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
        //    ViewBag.HasLocalPassword = hasLocalAccount;
        //    ViewBag.ReturnUrl = Url.Action("Manage");
        //    if (hasLocalAccount)
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            // ChangePassword will throw an exception rather than return false in certain failure scenarios.
        //            bool changePasswordSucceeded;
        //            try
        //            {
        //                changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
        //            }
        //            catch (Exception)
        //            {
        //                changePasswordSucceeded = false;
        //            }

        //            if (changePasswordSucceeded)
        //            {
        //                return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
        //            }
        //            else
        //            {
        //                ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
        //            }
        //        }
        //    }
        //    else
        //    {
        //        // User does not have a local password so remove any validation errors caused by a missing
        //        // OldPassword field
        //        ModelState state = ModelState["OldPassword"];
        //        if (state != null)
        //        {
        //            state.Errors.Clear();
        //        }

        //        if (ModelState.IsValid)
        //        {
        //            try
        //            {
        //                WebSecurity.CreateAccount(User.Identity.Name, model.NewPassword);
        //                return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
        //            }
        //            catch (Exception)
        //            {
        //                ModelState.AddModelError("", String.Format("Unable to create local account. An account with the name \"{0}\" may already exist.", User.Identity.Name));
        //            }
        //        }
        //    }

        //    // If we got this far, something failed, redisplay form
        //    return View(model);
        //}

        //
        // POST: /Account/ExternalLogin

    //    [HttpPost]
    //    [AllowAnonymous]
    //    [ValidateAntiForgeryToken]
    //    public ActionResult ExternalLogin(string provider, string returnUrl)
    //    {
    //        return new ExternalLoginResult(provider, Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
    //    }

    //    //
    //    // GET: /Account/ExternalLoginCallback

    //    [AllowAnonymous]
    //    public ActionResult ExternalLoginCallback(string returnUrl)
    //    {
    //        AuthenticationResult result = OAuthWebSecurity.VerifyAuthentication(Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
    //        if (!result.IsSuccessful)
    //        {
    //            return RedirectToAction("ExternalLoginFailure");
    //        }

    //        if (OAuthWebSecurity.Login(result.Provider, result.ProviderUserId, createPersistentCookie: false))
    //        {
    //            return RedirectToLocal(returnUrl);
    //        }

    //        if (User.Identity.IsAuthenticated)
    //        {
    //            // If the current user is logged in add the new account
    //            OAuthWebSecurity.CreateOrUpdateAccount(result.Provider, result.ProviderUserId, User.Identity.Name);
    //            return RedirectToLocal(returnUrl);
    //        }
    //        else
    //        {
    //            // User is new, ask for their desired membership name
    //            string loginData = OAuthWebSecurity.SerializeProviderUserId(result.Provider, result.ProviderUserId);
    //            ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(result.Provider).DisplayName;
    //            ViewBag.ReturnUrl = returnUrl;
    //            return View("ExternalLoginConfirmation", new RegisterExternalLoginModel { UserName = result.UserName, ExternalLoginData = loginData });
    //        }
    //    }

    //    //
    //    // POST: /Account/ExternalLoginConfirmation

    //    [HttpPost]
    //    [AllowAnonymous]
    //    [ValidateAntiForgeryToken]
    //    public ActionResult ExternalLoginConfirmation(RegisterExternalLoginModel model, string returnUrl)
    //    {
    //        string provider = null;
    //        string providerUserId = null;

    //        if (User.Identity.IsAuthenticated || !OAuthWebSecurity.TryDeserializeProviderUserId(model.ExternalLoginData, out provider, out providerUserId))
    //        {
    //            return RedirectToAction("Manage");
    //        }

    //        if (ModelState.IsValid)
    //        {
    //            // Insert a new user into the database
    //            using (UsersContext db = new UsersContext())
    //            {
    //                UserProfile user = db.UserProfiles.FirstOrDefault(u => u.UserName.ToLower() == model.UserName.ToLower());
    //                // Check if user already exists
    //                if (user == null)
    //                {
    //                    // Insert name into the profile table
    //                    db.UserProfiles.Add(new UserProfile { UserName = model.UserName });
    //                    db.SaveChanges();

    //                    OAuthWebSecurity.CreateOrUpdateAccount(provider, providerUserId, model.UserName);
    //                    OAuthWebSecurity.Login(provider, providerUserId, createPersistentCookie: false);

    //                    return RedirectToLocal(returnUrl);
    //                }
    //                else
    //                {
    //                    ModelState.AddModelError("UserName", "User name already exists. Please enter a different user name.");
    //                }
    //            }
    //        }

    //        ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(provider).DisplayName;
    //        ViewBag.ReturnUrl = returnUrl;
    //        return View(model);
    //    }

    //    //
    //    // GET: /Account/ExternalLoginFailure

    //    [AllowAnonymous]
    //    public ActionResult ExternalLoginFailure()
    //    {
    //        return View();
    //    }

    //    [AllowAnonymous]
    //    [ChildActionOnly]
    //    public ActionResult ExternalLoginsList(string returnUrl)
    //    {
    //        ViewBag.ReturnUrl = returnUrl;
    //        return PartialView("_ExternalLoginsListPartial", OAuthWebSecurity.RegisteredClientData);
    //    }

    //    [ChildActionOnly]
    //    public ActionResult RemoveExternalLogins()
    //    {
    //        ICollection<OAuthAccount> accounts = OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name);
    //        List<ExternalLogin> externalLogins = new List<ExternalLogin>();
    //        foreach (OAuthAccount account in accounts)
    //        {
    //            AuthenticationClientData clientData = OAuthWebSecurity.GetOAuthClientData(account.Provider);

    //            externalLogins.Add(new ExternalLogin
    //            {
    //                Provider = account.Provider,
    //                ProviderDisplayName = clientData.DisplayName,
    //                ProviderUserId = account.ProviderUserId,
    //            });
    //        }

    //        ViewBag.ShowRemoveButton = externalLogins.Count > 1 || OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
    //        return PartialView("_RemoveExternalLoginsPartial", externalLogins);
    //    }

    //    #region Helpers
    //    private ActionResult RedirectToLocal(string returnUrl)
    //    {
    //        if (Url.IsLocalUrl(returnUrl))
    //        {
    //            return Redirect(returnUrl);
    //        }
    //        else
    //        {
    //            return RedirectToAction("Index", "Home");
    //        }
    //    }

    //    public enum ManageMessageId
    //    {
    //        ChangePasswordSuccess,
    //        SetPasswordSuccess,
    //        RemoveLoginSuccess,
    //    }

    //    internal class ExternalLoginResult : ActionResult
    //    {
    //        public ExternalLoginResult(string provider, string returnUrl)
    //        {
    //            Provider = provider;
    //            ReturnUrl = returnUrl;
    //        }

    //        public string Provider { get; private set; }
    //        public string ReturnUrl { get; private set; }

    //        public override void ExecuteResult(ControllerContext context)
    //        {
    //            OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
    //        }
    //    }

    //    private static string ErrorCodeToString(MembershipCreateStatus createStatus)
    //    {
    //        // See http://go.microsoft.com/fwlink/?LinkID=177550 for
    //        // a full list of status codes.
    //        switch (createStatus)
    //        {
    //            case MembershipCreateStatus.DuplicateUserName:
    //                return "User name already exists. Please enter a different user name.";

    //            case MembershipCreateStatus.DuplicateEmail:
    //                return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

    //            case MembershipCreateStatus.InvalidPassword:
    //                return "The password provided is invalid. Please enter a valid password value.";

    //            case MembershipCreateStatus.InvalidEmail:
    //                return "The e-mail address provided is invalid. Please check the value and try again.";

    //            case MembershipCreateStatus.InvalidAnswer:
    //                return "The password retrieval answer provided is invalid. Please check the value and try again.";

    //            case MembershipCreateStatus.InvalidQuestion:
    //                return "The password retrieval question provided is invalid. Please check the value and try again.";

    //            case MembershipCreateStatus.InvalidUserName:
    //                return "The user name provided is invalid. Please check the value and try again.";

    //            case MembershipCreateStatus.ProviderError:
    //                return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

    //            case MembershipCreateStatus.UserRejected:
    //                return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

    //            default:
    //                return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
    //        }
    //    }
    //    #endregion
    }
}
