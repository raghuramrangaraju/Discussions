using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Discussions.Models;

namespace Discussions.Controllers
{
    public class HomeController : Controller
    {
        StructuredDiscussionsEntities context = new StructuredDiscussionsEntities();
        public ActionResult Index()
        {
           var questions = context.Questions.ToList().OrderByDescending(x=>x.CreateAt);
           

            return View(questions);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Claim(int QuestionId = 0) {

            return View();
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


        public JsonResult askquestion(string question="")
        {
            //To check the user first and get the username 
            Question newdata = new Question();
            newdata.Question1 = question;
            newdata.UserId = 1;
            newdata.CreateAt = DateTime.Now;
            newdata.Status = true;
            context.Questions.Add(newdata);
            context.SaveChanges();
            return Json(newdata, JsonRequestBehavior.AllowGet);
        }

        public JsonResult submitclaim() {

            return Json(1, JsonRequestBehavior.AllowGet);
        }
    }
}