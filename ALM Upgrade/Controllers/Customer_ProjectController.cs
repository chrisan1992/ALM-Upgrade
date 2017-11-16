using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ALM_Upgrade.Models;
using System.IO;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;

namespace ALM_Upgrade.Controllers
{
    public class Customer_ProjectController : Controller
    {
        private ALMEntities db = new ALMEntities();

        // GET: Customer_Project
        public ActionResult Index()
        {
            if (Utilities.IsUserLogged())
            {
                return View(db.Customer_Project.ToList());
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }

        }

        // GET: Customer_Project/Details/5
        public ActionResult Details(int? id)
        {
            if (Utilities.IsUserLogged())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Customer_Project customer_Project = db.Customer_Project.Find(id);
                if (customer_Project == null)
                {
                    return HttpNotFound();
                }
                return View(customer_Project);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        // GET: Customer_Project/Create
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

        // POST: Customer_Project/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,customer_name,customer_url,deactivation,undo_checkouts,copy_file,copy_db,change_file,restore_project,verify_project,upgrade_project,verification,deactivation,release,initialNotification,finalNotification,service_id")] Customer_Project customer_Project)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    customer_Project.customerId = Guid.NewGuid();
                    db.Customer_Project.Add(customer_Project);
                    db.SaveChanges();
                    return RedirectToAction("Edit", new { id = customer_Project.id });
                }
                catch (Exception ex)
                {
                    if (ex.GetBaseException().GetType() == typeof(SqlException))
                    {
                        //Violation of primary key/Unique constraint can be handled here. Also you may //check if Exception Message contains the constraint Name 
                        String m = ex.InnerException.InnerException.Message;
                        if (m.Contains("Violation of UNIQUE KEY constraint"))
                        {
                            Session["message"] = "The customer name is already in use";
                            return View(customer_Project);
                        }
                    }
                }
                
            }

            return View(customer_Project);
        }

        // GET: Customer_Project/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Utilities.IsUserLogged())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Customer_Project customer_Project = db.Customer_Project.Find(id);
                if (customer_Project == null)
                {
                    return HttpNotFound();
                }
                if(customer_Project.deactivation == true && customer_Project.copy_file == true && customer_Project.copy_db == true && 
                    customer_Project.change_file == true && customer_Project.restore_project == true && customer_Project.upgrade_project ==true &&
                    customer_Project.undo_checkouts == true && customer_Project.verify_project ==  true && customer_Project.verification == true &&
                    customer_Project.release == true)
                {
                    ViewBag.allchecks = true;
                }
                else {
                    ViewBag.allchecks = false; 
                }
                return View(customer_Project);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        // POST: Customer_Project/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,customer_name,customer_url,deactivation,undo_checkouts,copy_file,copy_db,change_file,restore_project,verify_project,upgrade_project,verification,release,customerId,initialNotification,finalNotification,service_id")] Customer_Project customer_Project)
        {
            if (ModelState.IsValid)
            {
                db.Entry(customer_Project).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Edit", new { id = customer_Project.id });//return to same view
            }
                return View(customer_Project);
        }

        // GET: Customer_Project/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Utilities.IsUserLogged())
            {
                Customer_Project customer_Project = db.Customer_Project.Find(id);
                db.Customer_Project.Remove(customer_Project);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PartialViewResult Emails(int id)
        {
            List<Project_Email> emails = db.Project_Email.Where(x => x.project_id == id).ToList();
            return PartialView("Emails", emails);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PartialViewResult ProjectList(int id)
        {
            List<ProjectList> projects = db.ProjectList.Where(x => x.customer_project_id == id).ToList();
            return PartialView("ProjectList", projects);
        }

        /// <summary>
        /// Gets tthe list of the finished projects for a customer
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PartialViewResult FinishedProjectList(int id)
        {
            List<ProjectList> projects = db.ProjectList.Where(x => x.customer_project_id == id).Where(x => x.finished == true).OrderBy(x => x.domain_name).ToList();
            return PartialView("FinishedProjectList", projects);
        }

        /// <summary>
        /// saves in the db the status for a specific project
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public JsonResult ProjectFinished(int id, int state)
        {
            try
            {
                ProjectList p = db.ProjectList.Find(id);
                p.finished = (state == 1) ? true : false;
                db.SaveChanges();
                return Json("Correct", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            { return Json("Error", JsonRequestBehavior.AllowGet); }
        }


        [HttpPost]
        public ActionResult AddProjectList(int id)
        {
            //delete any entries for this project id
            db.ProjectList.RemoveRange(db.ProjectList.Where(x => x.customer_project_id == id));
            db.SaveChanges();
            try
            {
                HttpPostedFileBase projectList = Request.Files[0];
                // Use the InputStream to get the actual stream sent.
                StreamReader csvreader = new StreamReader(projectList.InputStream);
                char[] delimiters = new char[] { ';', ',' };
                while (!csvreader.EndOfStream)
                {
                    var line = csvreader.ReadLine();
                    var values = line.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                    ProjectList pl = new ProjectList();
                    pl.domain_name = values[0];
                    pl.project_name = values[1];
                    pl.finished = false;
                    pl.customer_project_id = id;
                    db.ProjectList.Add(pl);
                }
                db.SaveChanges();
                return Json("Project list uploaded successfully!");
            }
            catch (Exception ex)
            {
                //delete any entries for this project id
                db.ProjectList.RemoveRange(db.ProjectList.Where(x => x.customer_project_id == id));
                db.SaveChanges();
                return Json("Error uploading file");
            }
        }

        /// <summary>
        /// View to show the customer the status of the upgrade
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Status(Guid id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer_Project customer_Project;
            try
            {
                customer_Project = db.Customer_Project.Where(x => x.customerId == id).First();
            }
            catch (Exception ex)
            {
                return RedirectToAction("NotFound", "Home");
            }

            //Deactivation
            int result = 0;
            if (customer_Project.deactivation == true)
                result += 100;
            ViewBag.Deactivation = result;

            //Data Copy
            result = 0;
            if (customer_Project.undo_checkouts == true)
                result += 25;
            if (customer_Project.copy_file == true)
                result += 25;
            if (customer_Project.copy_db == true)
                result += 25;
            if (customer_Project.change_file == true)
                result += 25;
            ViewBag.DataCopy = result;

            //Upgrade
            result = 0;
            if (customer_Project.restore_project == true)
                result += 33;
            if (customer_Project.verify_project == true)
                result += 33;
            if (customer_Project.upgrade_project == true)
                result += 34;
            ViewBag.Upgrade = result;

            //Validation
            result = 0;
            if (customer_Project.verification == true)
                result += 100;
            ViewBag.Validation = result;

            //Release
            result = 0;
            if (customer_Project.release == true)
                result += 100;
            ViewBag.Release = result;

            return View(customer_Project);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult DeleteEmail(int id)
        {
            Project_Email email = db.Project_Email.Find(id);
            db.Project_Email.Remove(email);
            db.SaveChanges();

            var results = new List<int>()
                {
                    1
                }.ToList();
            //1 is good
            return Json(results, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Add emails to the database bind to a project
        /// </summary>
        /// <param name="emailList"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult AddEmails(string emailList, int id)
        {
            //every line of the json reads every number
            try
            {
                string[] emails = emailList.Split('\n');
                foreach (string email in emails)
                {
                    if (new EmailAddressAttribute().IsValid(email))
                    {//checks if this is a valid email address
                        Project_Email newEmail = new Project_Email();
                        newEmail.project_id = id;
                        newEmail.email = email;
                        db.Project_Email.Add(newEmail);
                    }
                }
                db.SaveChanges();
                var results = new List<int>()
                {
                    1
                }.ToList();
                //1 is good
                return Json(results, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var results = new List<int>()
                {
                    0
                }.ToList();
                //0 is bad
                return Json(results, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public ActionResult NotifyCustomer(int id, int notificationType, string comments, string inhouse)
        {
            if (inhouse.Equals("true"))
            {
                Session["inhouse"] = "true";
            }
            else
            {
                Session["inhouse"] = "false";
            }
            List<HttpPostedFileBase> attachments = new List<HttpPostedFileBase>();
            try
            {
                HttpFileCollectionBase files = Request.Files;

                for (int i = 0; i < files.Count; i++)
                {
                    //save the attachments
                    attachments.Add(files[i]);
                }
            }
            catch (Exception ex)
            {
                return Json("Error uploading file");
            }

            //get the email list for the specific project
            List<Project_Email> list = db.Project_Email.Where(x => x.project_id == id).ToList();
            Customer_Project c = db.Customer_Project.Find(id);
            if (notificationType == 1)
                c.initialNotification = true;
            else if (notificationType == 3)
                c.finalNotification = true;
            db.SaveChanges();
            int send = Utilities.NotifyCustomer(list, notificationType, comments, attachments,c);
            // Returns message that successfully uploaded  
            return Json("File Uploaded Successfully!");
        }
    }
}
