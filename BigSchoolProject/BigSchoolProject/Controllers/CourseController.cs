
using BigSchoolProject.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace BigSchoolProject.Controllers
{
    public class CourseController : Controller
    {

        // see List /
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

        // Create course
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

            ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>()
                .FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            input.LectureId = user.Id;

            context.Courses.Add(input);
            context.SaveChanges();
            return RedirectToAction("Index", "Course");
        }

        // see attending List
        public ActionResult Attending()
        {
            BigSchoolContext context = new BigSchoolContext(); 
            ApplicationUser currentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().
                FindById(System.Web.HttpContext.Current.User.Identity.GetUserId()); var listAttendances = context.Attendances
                .Where(p => p.Attendee == currentUser.Id).ToList();
            var courses = new List<Course>();
            foreach (Attendance temp in listAttendances)
            {
                 Course objCourse = temp.Course;
                 objCourse.LectureName = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().
                 FindById(objCourse.LectureId).Name; courses.Add(objCourse);
            }

            return View(courses);
        }

        // see my Course
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

        public ActionResult Edit(int? Id)
        {
            BigSchoolContext context = new BigSchoolContext();
            Course objCourse = context.Courses.FirstOrDefault(p => p.Id == Id);
            objCourse.ListCategory = context.Categories.ToList();
            return View(objCourse);
        }

        [HttpPost]
        public ActionResult Edit(Course course)
        {
            BigSchoolContext context = new BigSchoolContext();
            if (context.Courses.Find(course.Id)!=null)
            {
                context.Courses.AddOrUpdate(course);
                context.SaveChanges();
            }

            return RedirectToAction("Mine", context.Courses.ToList());
        }

        public ActionResult Delete(int id)
        {
            BigSchoolContext context = new BigSchoolContext();
            return View(context.Courses.FirstOrDefault(p=>p.Id == id));
        }

        [HttpPost]
        public ActionResult Delete(Course course)
        {
            BigSchoolContext context = new BigSchoolContext();
            var dbdel = context.Courses.First(p => p.Id == course.Id);
            if (dbdel != null)
            {
                context.Courses.Remove(dbdel);
                context.SaveChanges();
            }
            return RedirectToAction("Mine", context.Courses.ToList());
        }
    }
}