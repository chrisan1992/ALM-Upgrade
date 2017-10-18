using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ALM_Upgrade.Models;

namespace ALM_Upgrade.Controllers
{
    public class UsersController : Controller
    {
        private ALMEntities db = new ALMEntities();

        // GET: Users
        public ActionResult Index()
        {
            if (Utilities.IsUserLogged())
            {
                return View(db.Users.ToList());
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }
        

        // GET: Users/Create
        public ActionResult Create()
        {
            if (Utilities.IsUserLogged())
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Email,UserName,Emailpass")] Users users, String confirmPassword)
        {
            if (Utilities.IsUserLogged())
            {
                if (ModelState.IsValid)
                {
                    if (db.Users.Where(x => x.Email == users.Email).ToList().Count > 0)
                    {
                        Session["message"] = "Email already exist.";
                        return View(users);
                    }
                    else
                    {

                        if (users.Emailpass == confirmPassword)
                        {
                            users.Emailpass = Utilities.Encrypt(users.Emailpass);
                            db.Users.Add(users);
                            db.SaveChanges();//save the new user to the db

                            //return to home after the register
                            return RedirectToAction("Index");
                        }
                        Session["message"] = "Passwords don't match. Please try again.";
                        return View(users);
                    }
                }
                Session["message"] = "User wasn't registered. Please try again.";
                return View(users);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Utilities.IsUserLogged())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Users users = db.Users.Find(id);
                if (users == null)
                {
                    return HttpNotFound();
                }
                return View(users);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Email,UserName,Emailpass")] Users users)
        {
            if (Utilities.IsUserLogged())
            {
                if (ModelState.IsValid)
                {
                    db.Entry(users).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(users);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        // GET: Users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Utilities.IsUserLogged())
            {
                Users users = db.Users.Find(id);
                db.Users.Remove(users);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult ChangePassword(int? id)
        {
            if (Utilities.IsUserLogged())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Users users = db.Users.Find(id);
                if (users == null)
                {
                    return HttpNotFound();
                }
                return View(users);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpPost]
        public ActionResult ChangePassword(String currentPassword, String newPassword, String confirmPassword, String idU)
        {
            if (Utilities.IsUserLogged())
            {
                try
                {
                    int result = (int)(db.AutenticateUser(Session["user_email"].ToString(), Utilities.Encrypt(currentPassword)).First());
                    if (newPassword == confirmPassword)
                    {
                        //call stored procedure to update
                        db.UpdateUserPassword(result, Utilities.Encrypt(newPassword));
                        Session["user_pass"] = newPassword;//now thenew password is stored and ready to be used 
                        return RedirectToAction("Index", "Home");
                    }
                    Session["message"] = "Please check on the new Password.";
                    return View();
                }
                catch (Exception ex)
                {
                    Session["message"] = "The current password doesn't match. Please try again.";
                    return RedirectToAction("ChangePassword", "Users", new { id = idU });
                }
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register([Bind(Include = "Id,Email,UserName,Emailpass")] Users users, String confirmPassword)
        {
            if (ModelState.IsValid)
            {
                if (users.Emailpass == confirmPassword)
                {
                    users.Emailpass = users.Emailpass;
                    db.Users.Add(users);
                    db.SaveChanges();//save the new user to the db

                    //Auth user to save the parameters
                    int result = (int)(db.AutenticateUser(users.Email, users.Emailpass).First());
                    Session["user_logged"] = true;
                    Session["user_id"] = result;
                    Session["username"] = users.UserName;
                    Session["user_email"] = users.Email;
                    Session["user_pass"] = confirmPassword;

                    //return to home after the register
                    return RedirectToAction("Index");
                }
                Session["message"] = "Passwords don't match. Please try again.";
                return View(users);
            }
            Session["message"] = "User wasn't registered. Please try again.";
            return View(users);
        }
    }
}
