using BigSchoolProject.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BigSchoolProject.Controllers
{
    public class AttendancesController : ApiController
    {
        protected void Application_Start()
        {

        }
        // add atten to data basse / them lop hoc duoc theo doi database
        public IHttpActionResult Attend(Course attendanceDto)
        {
            var userID = User.Identity.GetUserId();
            BigSchoolContext context = new BigSchoolContext();
            if (context.Attendances.Any(p => p.Attendee == userID && p.CouseId == attendanceDto.Id))
            {
                return BadRequest(" the attendance already exist!");
            }
            var attendance = new Attendance() { CouseId = attendanceDto.Id, Attendee = User.Identity.GetUserId() };
            context.Attendances.Add(attendance);
            context.SaveChanges();

            return Ok();
        }
    }
}
