using Core.Domains;
using PTS.Infrastructure;
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
        
        //private readonly IStudentUserService _studentUserService;
        private readonly IBaseService<Enrolled> _enrolledService;
        
        private readonly IBaseService<TeacherUser> _teacherUserService;
        private readonly IBaseService<Class> _classService;
        
        private readonly IBaseService<ReviewClass> _reviewClassService;
        private readonly IBaseService<Payment> _paymentService;
        private readonly IBaseService<ReviewTeacher> _reviewTutorService;


        #endregion
        //
        // GET: /Review/

        public ReviewController(IBaseService<ReviewTeacher> reviewTutorService,IBaseService<Payment> paymentService, IBaseService<ReviewClass> reviewClass,IBaseService<Enrolled> enrolledService, IUserService userService, 
                                IBaseService<Class> classService, IBaseService<Location> locationService, IBaseService<TeacherUser> teacherUserService)
        {
            _enrolledService = enrolledService;
            _classService = classService;
            _reviewClassService = reviewClass;
            _paymentService = paymentService;
            _reviewTutorService = reviewTutorService;
            _teacherUserService = teacherUserService;
        }

        public ActionResult Index()
        {
            
            return View();
        }

        public ActionResult ReviewTutor()
        {
            return View();
        }

        public ActionResult ReviewClassesView(int classId)
        {
            var reviewClass = _classService.GetById(classId);
            return View(reviewClass);
        }

        public ActionResult ReviewTeacherView(int teacherId)
        {
            var reivewTeacher = _teacherUserService.GetById(teacherId);
            return View(reivewTeacher);
        }
        [HttpPost]
        public ActionResult AddUpdateTutorsToReview(ReviewTutorViewModel review)
        {
            var reviewTutor = _reviewTutorService.GetTableQuery().Where(r => r.StudentId == review.StudentID).Where(r=> r.TeacherId==review.TeacherID).ToList();

            var record = new ReviewTutorViewModel();
            if(reviewTutor.Count()>0)
            {
                var update = new ReviewTeacher
                {
                    Id = reviewTutor.Single().Id,
                    Comment = review.Comment,
                    Date = DateTime.Today,
                    Rating = review.Rating,
                    StudentId = review.StudentID,
                    TeacherId = review.TeacherID
                };
                _reviewTutorService.Update(update);

                record = new ReviewTutorViewModel
                {
                    TeacherID=review.TeacherID,
                    TutorName=review.TutorName,
                    Comment = review.Comment,
                    Date = DateTime.Today,
                    StudentID = review.StudentID,
                    Rating = review.Rating,
                   
                };
            }
            else
            {
                var insert = new ReviewTeacher
                {
                    Comment = review.Comment,
                    Date = DateTime.Today,
                    Rating = review.Rating,
                    StudentId = review.StudentID,
                    TeacherId = review.TeacherID
                };
                _reviewTutorService.Insert(insert);

                record = new ReviewTutorViewModel
                {
                    TeacherID = review.TeacherID,
                    TutorName = review.TutorName,
                    Comment = review.Comment,
                    Date = DateTime.Today,
                    StudentID = review.StudentID,
                    Rating = review.Rating,

                };
            }

            
            return Json(new { Result = "OK", Records = record });
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

        [HttpPost]
        public ActionResult GetTutorsToReview()
        {
            var payments = _paymentService.GetTableQuery().Where(p => p.StudentId == SessionDataHelper.UserId).Distinct().ToList();
            var Class = new List<Class>();
            var Reviews = new List<ReviewTutorViewModel>();

            foreach(var p in payments)
            {
                
                var temp = new ReviewTutorViewModel();
                var tempTutorReview = _reviewTutorService.GetTableQuery().Where(r => r.StudentId == SessionDataHelper.UserId).Where(r => r.TeacherId == p.TeacherId).SingleOrDefault();
                var tempTutor = _teacherUserService.GetById(p.TeacherId);
                temp.Date = DateTime.Today;
                temp.StudentID = SessionDataHelper.UserId;
                temp.TeacherID = p.TeacherId;
                temp.TutorName = tempTutor.User.FirstName + " " + tempTutor.User.LastName;
                if(tempTutorReview!=null)
                {
                    temp.Comment=tempTutorReview.Comment;
                    temp.Rating = tempTutorReview.Rating;
                }
                else
                {
                    temp.Comment = "Review This Tutor!";
                    temp.Rating=0;
                }

                Reviews.Add(temp);

            }

            var records=Reviews;
            return Json(new { Result = "OK", Records = records });
        }
        


        public ActionResult GetClassesToReview()
        {
            var EnrolledClasses = _enrolledService.GetTableQuery().Where(e => e.StudentId == SessionDataHelper.UserId).ToList();
            var Class = new List<Class>();
            var Reviews = new List<ReviewClassViewModel>();
            
            foreach(var c in EnrolledClasses)
            {
                var temp =new ReviewClassViewModel();
                var tempClassReview = _reviewClassService.GetTableQuery().Where(r => r.ClassId == c.ClassId).Where(r => r.StudentId == SessionDataHelper.UserId).SingleOrDefault();
                var tempClass = _classService.GetById(c.ClassId);
                temp.Description = tempClass.Description;
                temp.ClassID = tempClass.Id;
                temp.StudentID = SessionDataHelper.UserId;
                if (tempClassReview != null)
                {
                    temp.TeacherName = tempClass.Teacher.User.FirstName+ " " +tempClass.Teacher.User.LastName;
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
