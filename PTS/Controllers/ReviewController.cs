using Core.Domains;
using PTS.Models;
using Service.Interfaces;
using Service.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace PTS.Views.Review
{
    public class ReviewController : Controller
    {


        #region fields

        //private readonly IBaseService<StudentUser> _studentUserService;
        private readonly IUserService _userService;
        //private readonly IStudentUserService _studentUserService;
        private readonly IBaseService<Enrolled> _enrolledService;
        private readonly IBaseService<StudentUser> _studentUserService;
        private readonly IBaseService<TeacherUser> _teacherUserService;
        private readonly IBaseService<Class> _classService;
        private readonly IBaseService<Location> _locationService;
        private readonly IBaseService<ReviewClass> _reviewClassService;


        #endregion
        //
        // GET: /Review/

        public ReviewController(IBaseService<ReviewClass> reviewClass,IBaseService<Enrolled> enrolledService, IUserService userService, IBaseService<StudentUser> studentUserService, 
                                IBaseService<Class> classService, IBaseService<Location> locationService, IBaseService<TeacherUser> teacherUserService)
        {
            _enrolledService = enrolledService;
            _classService = classService;
            _reviewClassService = reviewClass;
        }

        public ActionResult Index()
        {
            
            return View();
        }

        [HttpPost]
        public ActionResult AddUpdateClassesToReview(ReviewClassViewModel review)
        {
            try
            {
                var ReviewClass = _reviewClassService.GetTableQuery().Where(e => e.StudentId == review.StudentID).ToList();
                var classdesc = _classService.GetById(review.ClassID);
                var record = new ReviewClassViewModel();
                if(ReviewClass.Count()>0)
                {
                    var update = new ReviewClass
                    {
                        Id=ReviewClass.SingleOrDefault().Id,
                        ClassId = review.ClassID,
                        Comment = review.Comment,
                        Date = DateTime.Today,
                        StudentId = review.StudentID,
                        Rating = review.Rating,
                    };
                    _reviewClassService.Update(update);

                    record = new ReviewClassViewModel
                    {
                        ClassID = review.ClassID,
                        Comment = review.Comment,
                        Date = DateTime.Today,
                        StudentID = review.StudentID,
                        Rating = review.Rating,
                        Description = classdesc.Description,
                    };
                }
                else
                {
                    var insert = new ReviewClass();
                    insert.ClassId = review.ClassID;
                    insert.Comment = classdesc.Description;
                    insert.Date = DateTime.Today;
                    insert.StudentId = review.StudentID;
                    insert.Rating = review.Rating;
                    _reviewClassService.Insert(insert);

                    record = new ReviewClassViewModel
                    {
                        ClassID = review.ClassID,
                        Comment = review.Comment,
                        Date = DateTime.Today,
                        StudentID = review.StudentID,
                        Rating = review.Rating,
                        Description = classdesc.Description,
                    };
                }


                
                return Json(new { Result = "OK", Record = record });
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public ActionResult GetClassesToReview()
        {
            var EnrolledClasses = _enrolledService.GetTableQuery().Where(e => e.StudentId == 15).ToList();
            var Class = new List<Class>();
            var Reviews = new List<ReviewClassViewModel>();
            
            foreach(var c in EnrolledClasses)
            {
                var temp =new ReviewClassViewModel();
                var tempClassReview = _reviewClassService.GetTableQuery().Where(r => r.ClassId == c.ClassId).SingleOrDefault();
                var tempClass = _classService.GetById(c.ClassId);
                temp.Description = tempClass.Description;
                temp.ClassID = tempClass.Id;
                temp.StudentID = 15;
                if (tempClassReview != null)
                {
                    temp.Comment = tempClassReview.Comment;
                    temp.Date = tempClassReview.Date;
                    temp.Rating = tempClassReview.Rating;
                }
                else
                {
                    temp.Comment = "Review this Class!";
                    temp.Date = DateTime.Today.Date;
                    temp.Rating = 0;
                }

                Reviews.Add( temp);
            }

            


            var records = Reviews;
            return Json(new { Result = "OK", Records = records });
        }
    }
}
