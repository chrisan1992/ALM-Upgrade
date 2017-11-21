using ALM_Upgrade.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ALM_Upgrade.Controllers
{
    public class HomeController : Controller
    {
        private ALMEntities db = new ALMEntities();

        public ActionResult Index()
        {
            if(Utilities.IsUserLogged())
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
            
        }

        public ActionResult NotFound()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string UserName, string Password)
        {
            String r = Utilities.Encrypt(Password);
            if (ModelState.IsValid)
            {
                try
                {
                    
                    int result = (int)(db.AutenticateUser(UserName, Utilities.Encrypt(Password)).First());
                    Session["user_logged"] = true;
                    Session["user_id"] = result;
                    Users us = db.Users.Find(result);
                    Session["username"] = us.UserName;
                    Session["user_email"] = us.Email;
                    Session["user_pass"] = Password;
                }
                catch (Exception ex)
                {
                    //if no succedded, to the login page
                    Session["message"] = "Please check your credentials and try again";
                    return View("Login");

                }
            }
            //if succedded, to the homepage
            return RedirectToAction("Index");
        }

        public ActionResult LogOut()
        {
            Session["user_logged"] = false;
            return RedirectToAction("Login", "Home");
        }

        /// <summary>
        /// Generic method for guest users
        /// redirects to a marjoincident dashboard with a guest user
        /// </summary>
        /// <returns></returns>
        public ActionResult Dashboard(int? incidentNumber, String user)
        {
            //http://localhost:24062/Home/Dashboard/?incidentNumber=135102&user=Guest
            //put code here
            Session["username"] = "Guest";
            return RedirectToAction("Dashboard", "MajorIncidents",new { id = incidentNumber });
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(string UserName)
        {
            try
            {
                List<Users> users = db.Users.Where(x => x.Email == UserName).ToList();
                if (users.Count == 1)
                {//the email is rgistered in the system
                    String newPass = Utilities.Encrypt(Guid.NewGuid().ToString());
                    users[0].Emailpass = newPass;
                    db.SaveChanges();
                    Utilities.SendRecoveryPassword(UserName, newPass);
                }
                else
                {
                    Session["message"] = "Your emil is not registered into the system.";
                    return View();
                }
                Session["message"] = "A recovery email has been sent. Please check your inbox.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                return View();
            }

        }
    }
}