﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Discussions.Models;

namespace Discussions.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        StructuredDiscussionsEntities context = new StructuredDiscussionsEntities();
        public ActionResult Index()
        {
            var questions = context.Questions.ToList().OrderByDescending(x => x.CreateAt);
            return View(questions);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        [HttpGet]
        public ActionResult Claim(int QuestionId = 0)
        {
            ViewBag.questionid = QuestionId;
            ViewBag.question = context.Questions.Where(x => x.QuestionId == QuestionId).Select(x => x.Question1).FirstOrDefault();
            var stance = (from a in context.Questions
                          join b in context.StanceTypes on a.QuestionId equals b.QuestionId
                          where a.QuestionId == QuestionId 
                            select new 
                            {
                                b.StanceTypeName,
                                b.StanceTypeId,
                                b.QuestionId
                            }).ToList();

            // -----------------  To find the recorded Claim for the question -------

            List<claimstance> mainclaimstance = new List<claimstance>();

            foreach (var item in stance) {
                claimstance claimstance = new claimstance();
                claimstance.StanceTypeName = item.StanceTypeName;
                claimstance.StanceTypeId = item.StanceTypeId;
                claimstance.QuestionId = item.QuestionId;
                claimstance.Claim = context.Claims.Where(x => x.QuestionId == item.QuestionId && x.StanceTypeId == item.StanceTypeId).ToList();
                mainclaimstance.Add(claimstance);
            }
            ViewBag.dynamicstance = mainclaimstance;

            //var stanceExample = context.ClaimStanceFunct(QuestionId).ToList();

            ViewBag.yestance = context.Claims.Where(x => x.QuestionId == QuestionId && x.Status == true).OrderByDescending(x => x.Rating).ToList();
            ViewBag.notance = context.Claims.Where(x => x.QuestionId == QuestionId && x.Status == false).OrderByDescending(x => x.Rating).ToList();
            //  var yes = context.Claims.Where(x => x.QuestionId == QuestionId && x.Status == true).ToList();
            //  ViewBag.yestance = yes.OrderBy(x => x.Rating);
            //var no = context.Claims.Where(x => x.QuestionId == QuestionId && x.Status == false).ToList();
            //  ViewBag.notance = no;
            return View();
        }
        [HttpPost]
        public ActionResult Claim(Claim claim, FormCollection value)
        {
            Claim newclaim = new Claim();
            newclaim.Claim1 = claim.Claim1;
            newclaim.CreatedAt = DateTime.UtcNow;
            newclaim.Evidence = claim.Evidence;
            newclaim.Source = claim.Source;
            newclaim.StanceTypeId = Convert.ToInt64(claim.StanceTypeId); // For no stance and yes for new stance
            newclaim.UserId = Convert.ToInt64(Session["UserId"]);  // Its has to be changed 
            newclaim.QuestionId = claim.QuestionId;
            newclaim.Rating = 0; // Initial rating will be zero
            newclaim.Stance = value["claimcheckbox"];
            context.Claims.Add(newclaim);
            context.SaveChanges();
            return RedirectToAction("Claim", new { QuestionId = newclaim.QuestionId });
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


        public JsonResult askquestion(string question = "", string Stance = "")
        {



            //To check the user first and get the username 
            var newdata = new Question();
            newdata.Question1 = question;
            newdata.UserId = Convert.ToInt64(Session["UserId"]);
            newdata.CreateAt = DateTime.UtcNow;
            newdata.Status = true;
            context.Entry(newdata).State = EntityState.Added;
            context.SaveChanges();

            // Parse the string 
            string[] stringbuffer = Stance.Split('$');
            foreach (var item in stringbuffer)
            {
                if (item != "" && item != " ")
                {
                    var newStanceType = new StanceType();
                    newStanceType.CreatedAt = DateTime.UtcNow;
                    newStanceType.QuestionId = newdata.QuestionId;
                    newStanceType.StanceTypeName = item;
                    context.Entry(newStanceType).State = EntityState.Added;
                    context.SaveChanges();
                }
            }
            return Json(newdata.QuestionId, JsonRequestBehavior.AllowGet);
        }

        public JsonResult vote(int? questionid, int? claimid, bool? status, bool? stance)
        {
            var userid = Convert.ToInt64(Session["UserId"]);
            var data = context.Votes.Where(x => x.QuestionId == questionid && x.ClaimId == claimid && x.UserId == userid && x.Stance == stance && x.UserId == userid).FirstOrDefault();
            if (data == null)
            { // new value
                Vote newdata = new Vote();
                newdata.ClaimId = claimid;
                newdata.CreatedAt = DateTime.UtcNow;
                newdata.QuestionId = questionid;
                newdata.UserId = userid;
                newdata.Vote1 = status;
                newdata.Stance = stance;
                newdata.Status = true;
                context.Entry(newdata).State = EntityState.Added;
                context.SaveChanges();
            }
            else {
                data.Vote1 = status;
                context.SaveChanges();
            }


            // Updating the rating value in the claim table 
            var yesstance = context.Votes.Where(x => x.QuestionId == questionid && x.ClaimId == claimid && x.Vote1 == true && x.UserId == userid && x.Stance == stance).ToList().Count();
            var nostance = context.Votes.Where(x => x.QuestionId == questionid && x.ClaimId == claimid && x.Vote1 == false && x.UserId == userid && x.Stance == stance).ToList().Count();
            var rating = (yesstance / (yesstance + nostance)) * 100;
            var claim = context.Claims.Where(x => x.ClaimId == claimid && x.QuestionId == questionid).FirstOrDefault();
            claim.Rating = rating;
            context.SaveChanges();

            return Json(1, JsonRequestBehavior.AllowGet);
        }


        public JsonResult subevidence(int claimid, string subevidence, string subevidencesource, string stance)
        {
            SubEvidence sub_evidence = new SubEvidence();
            sub_evidence.ClaimId = Convert.ToInt64(claimid);
            sub_evidence.SubEvidence1 = subevidence;
            sub_evidence.Source = subevidencesource;
            sub_evidence.UserId = Convert.ToInt64(Session["UserId"]);
            sub_evidence.Status = true;
            sub_evidence.CreatedAt = DateTime.UtcNow;
            sub_evidence.Rating = 0;
            sub_evidence.Stance = stance;
            context.SubEvidences.Add(sub_evidence);
            context.SaveChanges();



            return Json(1, JsonRequestBehavior.AllowGet);
        }
    }
}