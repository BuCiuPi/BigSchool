
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
            var upcommingCourse = context.Courses.ToList();

            var userID = User.Identity.GetUserId();
            foreach(Course i in upcommingCourse)

            {
                //tìm Name của user từ lectureid
                ApplicationUser user =

                System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>(
                ).FindById(i.LectureId);
                i.Name = user.Name;
                //lấy ds tham gia khóa học
                if (userID != null)

                {
                    i.isLogin = true;
                    //ktra user đó chưa tham gia khóa học

                    Attendance find = context.Attendances.FirstOrDefault(p =>

                    p.CouseId == i.Id && p.Attendee == userID);
                    if (find == null)
                        i.isShowGoing = true;
                    //ktra user đã theo dõi giảng viên của khóa học ?

                    following findFollow = context.followings.FirstOrDefault(p =>

                    p.followerId == userID && p.followeeId == i.LectureId);

                    if (findFollow == null)
                        i.isShowFollow = true;
                }
            }
            return View(upcommingCourse);
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
                 objCourse.Name = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().
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
                temp.Name = currentUser.Name;
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

        public ActionResult LectureIamGoing()
        {
            ApplicationUser currentUser =
           System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>()
            .FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            BigSchoolContext context = new BigSchoolContext();


            //danh sách giảng viên được theo dõi bởi người dùng (đăng nhập) hiện tại
            var listFollwee = context.followings.Where(p => p.followerId ==
            currentUser.Id).ToList();

            //danh sách các khóa học mà người dùng đã đăng ký
            var listAttendances = context.Attendances.Where(p => p.Attendee ==
            currentUser.Id).ToList();

            var courses = new List<Course>();
            foreach (var course in listAttendances)
            {
                foreach (var item in listFollwee)
                {
                    if (item.followeeId == course.Course.LectureId)
                    {
                        Course objCourse = course.Course;
                        objCourse.Name =
                       System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>()
                        .FindById(objCourse.LectureId).Name;
                        courses.Add(objCourse);
                    }
                }

            }
            return View(courses);
        }
    }
}