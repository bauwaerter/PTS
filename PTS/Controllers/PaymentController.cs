using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using Core.Domains;
using Data.Mappings;
using PTS.Infrastructure;
using PTS.Models;
using Service.Interfaces;

namespace PTS.Controllers{
    public class PaymentController : Controller{
        #region fields

        private readonly IBaseService<Class> _classService;
        private readonly IBaseService<Location> _locationService;
        private readonly IBaseService<Payment> _paymentService;
        private readonly IBaseService<TeacherUser> _teacherUserService; 
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;

        #endregion

        #region constructor 

        public PaymentController(IBaseService<Class> classService, IBaseService<Location> locationService,
            IBaseService<Payment> paymentService, IBaseService<TeacherUser> teacherService, IUserService userService, IEmailService emailService){
            _classService = classService;
            _locationService = locationService;
            _paymentService = paymentService;
            _teacherUserService = teacherService;
            _userService = userService;
            _emailService = emailService;
        }

        #endregion

        //
        // GET: /Payment/
        public ActionResult Index(){
            return View();
        }

        [HttpGet]
        public ActionResult ProcessPayment(int classId){

            var temp = SessionDataHelper.UserId;

            var user = _userService.GetById(temp);
            var newClass = _classService.GetById(classId);
            var locId = user.LocationId;
            var payment = new PaymentModel{
                Payment = new Payment(){
                    StudentId = user.Id,
                    ClassId = newClass.Id,
                    TeacherId = newClass.TeacherId,
                    Date = DateTime.Now,
                    Amount = (int) newClass.Teacher.ClassRate
                },
                Message = "",
            };
            payment.Location = _locationService.GetById((int)locId);
            return View(payment);
        }

        [HttpGet]
        public ActionResult ProcessTutorPayment(int studentId, int tutorId) {

            var user = _userService.GetById(studentId);
            var teacher = _teacherUserService.GetById(tutorId);
            var locId = user.LocationId;
            var payment = new PaymentModel {
                Payment = new Payment() {
                    StudentId = user.Id,
                    ClassId = null,
                    TeacherId = tutorId,
                    Date = DateTime.Now,
                    Amount = (int)teacher.HourlyRate,
                },
                Message = "per hour"
            };
            payment.Location = _locationService.GetById((int)locId);
            return View("ProcessPayment",payment);
        }

        [HttpPost]
        public ActionResult ProcessPayment(PaymentModel model){
            model.Location = null;
            _paymentService.Insert(model.Payment);
            var userFName = _userService.GetById(model.Payment.StudentId).FirstName;
            var teacher = _teacherUserService.GetById(model.Payment.TeacherId);
            _emailService.SendRequestEmail(teacher.User, userFName);
            return RedirectToAction("Index", "Search");
        }
    }
}

