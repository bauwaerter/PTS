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
        private readonly IUserService _userService;

        #endregion

        #region constructor 

        public PaymentController(IBaseService<Class> classService, IBaseService<Location> locationService,
            IBaseService<Payment> paymentService, IUserService userService){
            _classService = classService;
            _locationService = locationService;
            _paymentService = paymentService;
            _userService = userService;

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
                }
            };
            payment.Location = _locationService.GetById((int)locId);
            return View(payment);
        }

        [HttpPost]
        public ActionResult ProcessPayment(PaymentModel model){
            model.Location = null;
            _paymentService.Insert(model.Payment);
            return RedirectToAction("Index", "Search");
        }

    }
}

