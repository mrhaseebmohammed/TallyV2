using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using boardreview.Entities;
using boardreview.Models;
using DevOne.Security.Cryptography.BCrypt;

namespace boardreview.Controllers
{
    public class UsersController : Controller
    {
        private boardreviewEntities db = new boardreviewEntities();

        // GET: Users/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "UserID,Username,Password,Salt,Email,IsEmailVerified,IsActive")] User user)
        public ActionResult Create([Bind(Include = "Email,Password,ConfirmPassword")] UserCreateViewModel userVM)
        {
            if (db.Users.Any(u => u.Email == userVM.Email))
            {
                ModelState.AddModelError("Email", "Email in use");
            }

            if (ModelState.IsValid)
            {

                User user = new User();
                user.Email = userVM.Email;
                string pwdToHash = userVM.Password + "*)&h9"; //Security through obscurity
                user.Password = BCryptHelper.HashPassword(pwdToHash, BCryptHelper.GenerateSalt());
                FormsAuthentication.SetAuthCookie(userVM.Email, false);
                db.Users.Add(user);
                db.SaveChanges();

                return RedirectToAction("Index", "Home");
            }
            return View(userVM);
        }

        [Authorize]
        public ActionResult Logout()
        {
            if (User.Identity.Name != null)
            {
                FormsAuthentication.SignOut();
            }
            return RedirectToAction("Index", "Home");

        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login([Bind(Include = "Email, Password")] UserLoginViewModel userLVM)
        {
            if (ModelState.IsValid)
            {
                if(db.Users.Any(x=>x.Email == userLVM.Email))
                {
                    if (BCryptHelper.CheckPassword(userLVM.Password + "*)&h9", db.Users.First(u => u.Email == userLVM.Email).Password))
                    {
                        FormsAuthentication.SetAuthCookie(userLVM.Email, false);
                        return RedirectToAction("Index", "Home");
                    }
                }
                
                ModelState.AddModelError("", "Invalid username/password.");
            }
            return View(userLVM);
        }
        
    }
}
