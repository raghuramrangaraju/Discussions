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
        [HttpGet]
        public ActionResult Claim(int QuestionId = 14) {
            ViewBag.questionid = QuestionId;
            //  var claim = context.Claims.Where(x => x.QuestionId == QuestionId).ToList();
            ViewBag.yestance = context.Claims.Where(x => x.QuestionId == QuestionId && x.Status == true).ToList();
            ViewBag.notance = context.Claims.Where(x => x.QuestionId == QuestionId && x.Status == false).ToList();


            return View();
        }
        [HttpPost]
        public ActionResult Claim(Claim claim)
        {
            Claim newclaim = new Claim();
            newclaim.Claim1 = claim.Claim1;
            newclaim.CreatedAt = DateTime.Now;
            newclaim.Evidence = claim.Evidence;
            newclaim.Source = claim.Source;
            newclaim.Status = false; // For no stance and yes for new stance
            newclaim.UserId = 1; // Its has to be changed 
            newclaim.QuestionId = 14;
            context.Claims.Add(newclaim);
            context.SaveChanges();
            return RedirectToAction("Claim");
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

        public JsonResult submitclaim(string question="") {

            return Json(1, JsonRequestBehavior.AllowGet);
        }
    }
}