
using BigSchoolProject.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace BigSchoolProject.Controllers
{
    public class CourseController : Controller
    {

        // GET: Course
        public ActionResult Index()
        {
            BigSchoolContext context = new BigSchoolContext();
            ApplicationDbContext au = new ApplicationDbContext();
            var couses = context.Courses.ToList();
            foreach (Course item in couses)
            {
                item.LectureName = au.Users.FirstOrDefault(s => s.Id == item.LectureId).Name;
            }

            return View(couses);
        }

        public ActionResult Create()
        {
            BigSchoolContext context = new BigSchoolContext();
            Course objCourse = new Course();
            objCourse.ListCategory = context.Categories.ToList();
            return View(objCourse);
        }
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Course input)
        {
            BigSchoolContext context = new BigSchoolContext();

            ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            input.LectureId = user.Id;

            context.Courses.Add(input);
            context.SaveChanges();
            return RedirectToAction("Index", "Course");
        }

        public ActionResult Attending()
        {
            BigSchoolContext context = new BigSchoolContext(); 
            ApplicationUser currentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().
                FindById(System.Web.HttpContext.Current.User.Identity.GetUserId()); var listAttendances = context.Attendances.Where(p => p.Attendee == currentUser.Id).ToList();
            var courses = new List<Course>();
            foreach (Attendance temp in listAttendances)
            {
                 Course objCourse = temp.Course;
                 objCourse.LectureName = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().
                 FindById(objCourse.LectureId).Name; courses.Add(objCourse);
            }

            return View(courses);
        }

        public ActionResult Mine()
        {
            BigSchoolContext context = new BigSchoolContext();
            ApplicationUser currentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().
                FindById(System.Web.HttpContext.Current.User.Identity.GetUserId()); 
            var courses = context.Courses.Where(c => c.LectureId == currentUser.Id).ToList();
            foreach (Course temp in courses)
            {
                temp.LectureName = currentUser.Name;
            }

            return View(courses);
        }
    }
}