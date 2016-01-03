using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using boardreview.Entities;
using boardreview.Models;
using DevOne.Security.Cryptography.BCrypt;

namespace boardreview.Controllers
{
    public class HomeController : Controller
    {
        private boardreviewEntities db = new boardreviewEntities(); 
        public ActionResult Index()
        {
            var currUserID = -1;
            if(User.Identity.IsAuthenticated)
            {
                IndexViewModel ivm = new IndexViewModel();
                currUserID = currUserID = db.Users.First(x => x.Email == User.Identity.Name).UserID;
                if (db.Questions.Any(q=>q.CreatedByUserID == currUserID))
                {
                    ivm.shorturls = db.Questions.Where(q => q.CreatedByUserID == currUserID).Select(q => q.ShortURL).ToList();
                    ViewData["shorturls"] = ivm.shorturls.Distinct().ToList();
                }
                
                return View();
            }
            return View();
        }

        public ActionResult PollResults(string shortURL)
        {
            PollViewModel pvm = new PollViewModel();
            Question q = new Question();
            q = db.Questions.First(x => x.ShortURL == shortURL);

            pvm.Question = q;

            pvm.Answers = db.Answers.Where(x => x.QuestionID == q.QuestionID).ToArray();


            return View(pvm);
        }

        public string generateRandom(int maxLength, Random rnd)
        {
            string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789";
            char[] chars = new char[maxLength];
            for (int i = 0; i < maxLength; i++)
            {
                chars[i] = allowedChars[rnd.Next(0, allowedChars.Length)];
            }

            return new string(chars);

        }

        [HttpPost]
        public ActionResult Index([Bind(Include = "Question, Answers, ShortURL")] CreatePollViewModel cpvm)
        {
            var currUserID = -1;

            if (User.Identity.IsAuthenticated)
            {
                currUserID = db.Users.First(x => x.Email == User.Identity.Name).UserID;
            }

            if (db.Questions.Any(q => q.ShortURL == cpvm.ShortURL && q.CreatedByUserID != currUserID))
            {
                ModelState.AddModelError("ShortURL", "ShortURL in use..Maybe you forgot to login?");
            }

            if (ModelState.IsValid)
            {
                Question newQuestion = new Question();

                newQuestion.QuestionText = cpvm.Question.ToString().Replace(System.Environment.NewLine, "<br />");

                Random rnd = new Random();

                if(cpvm.ShortURL == null)
                {
                    newQuestion.ShortURL = generateRandom(6, rnd);
                }
                else
                {
                    newQuestion.ShortURL = cpvm.ShortURL;
                }
                
                newQuestion.Password = generateRandom(6, rnd);

                newQuestion.CreatedDateTime = DateTime.Now;

                if(User.Identity.IsAuthenticated)
                {
                    User u = db.Users.First(x => x.Email == User.Identity.Name);
                    newQuestion.CreatedByUserID = u.UserID;
                }

                newQuestion.Active = true;
                db.Questions.Add(newQuestion);

                db.SaveChanges();

                //int id = newQuestion.Id;

                cpvm.Answers = cpvm.Answers.Where(x => !string.IsNullOrEmpty(x.Value)).ToArray();

                Answer[] newAnswer = new Answer[cpvm.Answers.Length];

                for (int ndx = 0; ndx < cpvm.Answers.Length; ndx++)
                {
                    newAnswer[ndx] = new Answer();

                    newAnswer[ndx].QuestionID = newQuestion.QuestionID;
                    newAnswer[ndx].AnswerText = cpvm.Answers[ndx].Value;

                    db.Answers.Add(newAnswer[ndx]);
                }

                db.SaveChanges();

                //PollViewModel pvm = new PollViewModel();
                //pvm.Question = cpvm.Question;
                //pvm.Answers = cpvm.Answers.Select(x => x.Value).ToArray();
                //return RedirectToAction("AdminPoll", new { password = newQuestion.Password });

                if(User.Identity.IsAuthenticated)
                {
                    return RedirectToAction("ManagePolls");
                }
                else
                {
                    return RedirectToAction("AdminPoll", new { password = newQuestion.Password });
                }
                
            }

            return View();
        }

        public ActionResult AdminPoll(string password)
        {
            PollViewModel pvm = new PollViewModel();
            Question q = new Question();
            q = db.Questions.First(x => x.Password == password);

            pvm.Question = q;

            pvm.Answers = db.Answers.Where(x => x.QuestionID == q.QuestionID).ToArray();


            return View(pvm);
        }

        //[HttpPost]
        //public ActionResult Poll(string shortURL, int Answer)
        //{
        //    Question q = new Question();
        //    q = db.Questions.First(x => x.ShortURL == shortURL);
        //    Answer a = new Answer();
        //    a = db.Answers.First(x => x.AnswerID == Answer);
        //    a.Count = a.Count + 1;

        //    db.SaveChanges();

        //    return RedirectToAction("PollResults", new { shortUrl = q.ShortURL });
        //}


        //public ActionResult Poll(string shortURL)
        //{
        //    PollViewModel pvm = new PollViewModel();
        //    Question q = new Question();
        //    q = db.Questions.First(x => x.ShortURL == shortURL);

        //    pvm.Question = q;

        //    pvm.Answers = db.Answers.Where(x => x.QuestionID == q.QuestionID).ToArray();


        //    return View(pvm);
        //}

        public ActionResult Poll(string shortURL)
        {
            if(!string.IsNullOrEmpty(shortURL))
            {

                AllPollsViewModel allpolls = new AllPollsViewModel();
                List<Answer> temp = new List<Answer>();

                allpolls.Questions = db.Questions.Where(q => q.ShortURL == shortURL && q.Active == true).ToArray();
                foreach (var q in allpolls.Questions)
                {
                    temp.AddRange(db.Answers.Where(a => a.QuestionID == q.QuestionID).ToList());
                }

                allpolls.Answers = temp.ToArray();

                return View(allpolls);
            }
            return RedirectToAction("Index");

        }

        [HttpPost]
        public bool AnswerQuestion(int QuestionID, int AnswerID, string uniqueID)
        {
            if (BCryptHelper.CheckPassword("amazinglyunique", uniqueID))
            {
                if (!db.AnsweredBy.Any(x => x.AnswerID == AnswerID && x.BrowserKey == uniqueID && x.QuestionID == QuestionID) && db.Answers.Any(x=>x.AnswerID ==AnswerID) && db.Questions.Any(x => x.QuestionID == QuestionID))
                {
                    AnsweredBy ab = new AnsweredBy();
                    ab.AnswerID = AnswerID;
                    ab.BrowserKey = uniqueID;
                    ab.QuestionID = QuestionID;
                    db.AnsweredBy.Add(ab);
                    db.SaveChanges();

                    Answer a = new Answer();

                    a = db.Answers.First(x => x.AnswerID == AnswerID);
                    a.Count = a.Count + 1;

                    db.SaveChanges();

                    return true;
                }

                return false;
            }

            return false;

        }

        [HttpPost]
        public bool IsQuestionAnswered(int QuestionID, string uniqueID)
        {
            if (db.AnsweredBy.Any(x => x.BrowserKey == uniqueID && x.QuestionID == QuestionID))
            {
                return true;
            }
            return false;
        }

        [HttpPost]
        [Authorize]
        public void ToggleActive(int QuestionID)
        {
            if (User.Identity.IsAuthenticated)
            {
                User u = db.Users.First(x => x.Email == User.Identity.Name);
                Question q = db.Questions.First(x => x.QuestionID == QuestionID && x.CreatedByUserID == u.UserID);
                q.Active = !q.Active;
                q.ModifiedDateTime = DateTime.Now;
                db.SaveChanges();
               
            }
        }

        [HttpPost]
        [Authorize]
        public void ClearResponses(int QuestionID)
        {
            if (User.Identity.IsAuthenticated)
            {
                User u = db.Users.First(x => x.Email == User.Identity.Name);
                if(db.Questions.Any(x => x.QuestionID == QuestionID && x.CreatedByUserID == u.UserID))
                {
                    Question q = db.Questions.First(x => x.QuestionID == QuestionID && x.CreatedByUserID == u.UserID);
                    var answers = db.Answers.Where(x => x.QuestionID == q.QuestionID);
                    foreach(var a in answers)
                    {
                        db.Answers.Remove(a);
                    }

                    var answersBy = db.AnsweredBy.Where(x => x.QuestionID == q.QuestionID);
                    foreach (var a in answersBy)
                    {
                        db.AnsweredBy.Remove(a);
                    }

                    q.Active = !q.Active;
                    q.ModifiedDateTime = DateTime.Now;


                    db.SaveChanges();

                }
                

            }
        }

        [Authorize]
        public ActionResult ManagePolls()
        {
            if(User.Identity.IsAuthenticated)
            {
                User u = db.Users.First(x => x.Email == User.Identity.Name);

                AllPollsViewModel allpolls = new AllPollsViewModel();
                List<Answer> temp = new List<Answer>();

                allpolls.Questions = db.Questions.Where(q => q.CreatedByUserID == u.UserID).ToArray();
                foreach( var q in allpolls.Questions)
                {
                    temp.AddRange(db.Answers.Where(a => a.QuestionID == q.QuestionID).ToList());
                }

                allpolls.Answers = temp.ToArray();

                return View(allpolls);
            }
            return RedirectToAction("Index");
        }

    }
}