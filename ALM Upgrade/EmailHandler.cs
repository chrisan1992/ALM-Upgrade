using ALM_Upgrade.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Hosting;

namespace ALM_Upgrade
{
    public class EmailHandler
    {

        public Boolean NotifyCustomer(List<Project_Email> list, int notificationType, String comments, List<HttpPostedFileBase> attachments, Customer_Project c)
        {
            Boolean result = false;
            MailMessage msg = new MailMessage();
            msg.To.Clear();
            foreach (Project_Email p in list)
            {
                MailAddress emailTo = new MailAddress(p.email, p.email);
                if (!msg.To.Contains(emailTo))
                {
                    //to avoid adding duplicate email addresses
                    msg.To.Add(emailTo);
                }
            }
            msg.From = new MailAddress(HttpContext.Current.Session["user_email"].ToString(), HttpContext.Current.Session["username"].ToString());

            int attachIndex = 0;
            foreach (HttpPostedFileBase f in attachments)
            {//go through all the attachments
                var attachment = new Attachment(attachments[attachIndex].InputStream, attachments[attachIndex].FileName);
                msg.Attachments.Add(attachment);
                ++attachIndex;
            }
            //String inhouse = HttpContext.Current.Session["inhouse"].ToString();
            switch (notificationType)
            {
                case 1://initial
                    if (c.upgrade_type == true)
                    {//in house
                        if (c.dry_run)
                        {
                            msg.Subject = c.customer_name + " - " + c.service_id + " - In house to SaaS Dry Run " + c.upgrade_version;
                            msg.Body = System.IO.File.ReadAllText(HostingEnvironment.MapPath(@"~/Content/initial_inhouse.txt"));
                            msg.Body = msg.Body.Replace("migration", "Dry Run");
                        }
                        else
                        {
                            msg.Subject = c.customer_name + " - " + c.service_id + " - In house to SaaS Project Migration " + c.upgrade_version;
                            msg.Body = System.IO.File.ReadAllText(HostingEnvironment.MapPath(@"~/Content/initial_inhouse.txt"));
                        }
                    }
                    else
                    {
                        if (c.dry_run)
                        {
                            msg.Subject = c.customer_name + " - " + c.service_id + " - SaaS to SaaS Dry Run " + c.upgrade_version;
                            msg.Body = System.IO.File.ReadAllText(HostingEnvironment.MapPath(@"~/Content/initial_dry_run.txt"));
                        }
                        else
                        {
                            msg.Subject = c.customer_name + " - " + c.service_id + " - SaaS to SaaS Migration/Upgrade " + c.upgrade_version;
                            msg.Body = System.IO.File.ReadAllText(HostingEnvironment.MapPath(@"~/Content/initial.txt"));
                        }

                    }
                    //check if the keyword Batch appears in the customer name
                    if (c.customer_name.Contains("Batch") || c.customer_name.Contains("batch"))
                    {
                        msg.Body = msg.Body.Replace("@CUSTOMER", c.customer_name.Remove(c.customer_name.IndexOf("Batch"), c.customer_name.Length - c.customer_name.IndexOf("Batch")));
                    }
                    else
                    {
                        msg.Body = msg.Body.Replace("@CUSTOMER", c.customer_name);
                    }
                    msg.Body = msg.Body.Replace("@URL", c.customer_url);
                    msg.Body = msg.Body.Replace("@LINK", "upgradetool.azurewebsites.net/Customer_Project/Status/" + c.customerId);
                    msg.Body = Regex.Replace(msg.Body, @"(\r\n)|\n|\r", "<br/>");
                    break;
                case 2://intermediate
                    if (c.upgrade_type == true)
                    {
                        if (c.dry_run)
                        {
                            msg.Subject = c.customer_name + " - " + c.service_id + " - In house to SaaS Dry Run " + c.upgrade_version;
                            msg.Body = System.IO.File.ReadAllText(HostingEnvironment.MapPath(@"~/Content/intermediate.txt"));
                            msg.Body = msg.Body.Replace("migration", "Dry Run");
                        }
                        else
                        {
                            msg.Subject = c.customer_name + " - " + c.service_id + " - In house to SaaS Project Migration " + c.upgrade_version;
                            msg.Body = System.IO.File.ReadAllText(HostingEnvironment.MapPath(@"~/Content/intermediate.txt"));
                        }

                    }
                    else
                    {
                        if (c.dry_run)
                        {
                            msg.Subject = c.customer_name + " - " + c.service_id + " - SaaS to SaaS Dry Run " + c.upgrade_version;
                            msg.Body = System.IO.File.ReadAllText(HostingEnvironment.MapPath(@"~/Content/intermediate.txt"));
                            msg.Body = msg.Body.Replace("migration", "Dry Run");
                        }
                        else
                        {
                            msg.Subject = c.customer_name + " - " + c.service_id + " - SaaS to SaaS Migration/Upgrade " + c.upgrade_version;
                            msg.Body = System.IO.File.ReadAllText(HostingEnvironment.MapPath(@"~/Content/intermediate.txt"));
                        }

                    }
                    msg.Body = System.IO.File.ReadAllText(HostingEnvironment.MapPath(@"~/Content/intermediate.txt"));

                    //check if the keyword Batch appears in the customer name
                    if (c.customer_name.Contains("Batch") || c.customer_name.Contains("batch"))
                    {
                        msg.Body = msg.Body.Replace("@CUSTOMER", c.customer_name.Remove(c.customer_name.IndexOf("Batch"), c.customer_name.Length - c.customer_name.IndexOf("Batch")));
                    }
                    else
                    {
                        msg.Body = msg.Body.Replace("@CUSTOMER", c.customer_name);
                    }
                    msg.Body = msg.Body.Replace("@URL", c.customer_url);
                    msg.Body = msg.Body.Replace("@CHECKBOXES", GetCheckText(c));
                    comments = Regex.Replace(comments, @"(\r\n)|\n|\r", "<br/>");
                    msg.Body = msg.Body.Replace("@COMMENTS", comments);
                    msg.Body = msg.Body.Replace("@LINK", "upgradetool.azurewebsites.net/Customer_Project/Status/" + c.customerId);
                    msg.Body = Regex.Replace(msg.Body, @"(\r\n)|\n|\r", "<br/>");
                    break;
                case 3://final
                    if (c.upgrade_type == true)
                    {
                        if (c.dry_run)
                        {
                            msg.Subject = c.customer_name + " - " + c.service_id + " - In house to SaaS Dry Run " + c.upgrade_version;
                            msg.Body = System.IO.File.ReadAllText(HostingEnvironment.MapPath(@"~/Content/final.txt"));
                        }
                        else
                        {
                            msg.Subject = c.customer_name + " - " + c.service_id + " - In house to SaaS Project Migration " + c.upgrade_version;
                            msg.Body = System.IO.File.ReadAllText(HostingEnvironment.MapPath(@"~/Content/final.txt"));
                        }
                    }
                    else
                    {
                        if (c.dry_run)
                        {
                            msg.Subject = c.customer_name + " - " + c.service_id + " - SaaS to SaaS Dry Run " + c.upgrade_version;
                            msg.Body = System.IO.File.ReadAllText(HostingEnvironment.MapPath(@"~/Content/final.txt"));
                        }
                        else
                        {
                            msg.Subject = c.customer_name + " - " + c.service_id + " - SaaS to SaaS Migration/Upgrade " + c.upgrade_version;
                            msg.Body = System.IO.File.ReadAllText(HostingEnvironment.MapPath(@"~/Content/final.txt"));
                        }
                    }
                    //check if the keyword Batch appears in the customer name
                    if (c.customer_name.Contains("Batch") || c.customer_name.Contains("batch"))
                    {
                        msg.Body = msg.Body.Replace("@CUSTOMER", c.customer_name.Remove(c.customer_name.IndexOf("Batch"), c.customer_name.Length - c.customer_name.IndexOf("Batch")));
                    }
                    else
                    {

                        msg.Body = msg.Body.Replace("@CUSTOMER", c.customer_name);
                    }
                    msg.Body = msg.Body.Replace("@URL", c.customer_url);
                    msg.Body = msg.Body.Replace("@LINK", "upgradetool.azurewebsites.net/Customer_Project/Status/" + c.customerId);
                    msg.Body = Regex.Replace(msg.Body, @"(\r\n)|\n|\r", "<br/>");
                    break;
            }

            //Reading the email template files in order to send
            String htmlEmailStart = System.IO.File.ReadAllText(HostingEnvironment.MapPath(@"~/Content/email-start.html"));
            String htmlEmailEnd = System.IO.File.ReadAllText(HostingEnvironment.MapPath(@"~/Content/email-end.html"));
            msg.Body = htmlEmailStart + msg.Body + htmlEmailEnd;

            msg.IsBodyHtml = true;

            SmtpClient client = new SmtpClient();
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential(HttpContext.Current.Session["user_email"].ToString(), HttpContext.Current.Session["user_pass"].ToString());
            client.Port = 587; // You can use Port 25 if 587 is blocked
            client.Host = "smtp.office365.com";
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = true;
            try
            {
                client.Send(msg);
                result = true;//good
            }
            catch (Exception ex)
            {
                result = false;//error sending message
            }
            finally
            {
                client.Dispose();//close the connection
            }
            return result;
        }


        public Boolean SendRecoveryPassword(string email, string newPass)
        {
            Boolean result = false;
            MailMessage msg = new MailMessage();
            msg.From = new MailAddress("ALM.migrationtool@gmail.com", "ALM.migrationtool@gmail.com");
            msg.To.Clear();
            msg.To.Add(new MailAddress(email.Replace("@hpe.com", "@microfocus.com")));
            msg.Subject = "ALM Upgrade - Password Recovery";

            msg.Body = "You are receiving this email due to a request to recover your password. Your temporary password is the following:<br/><br/>" + newPass;
            msg.Body += "<br/><br/>Remember to change the password as soon as you login to the system, by clicking the upper right menu and the 'Change your password.<br/><br/>'";
            msg.Body += "Sincerely,<br/>ALM Team";

            //Reading the email template files in order to send
            String htmlEmailStart = System.IO.File.ReadAllText(HostingEnvironment.MapPath(@"~/Content/email-start.html"));
            String htmlEmailEnd = System.IO.File.ReadAllText(HostingEnvironment.MapPath(@"~/Content/email-end.html"));
            msg.Body = htmlEmailStart + msg.Body + htmlEmailEnd;

            msg.IsBodyHtml = true;

            SmtpClient client = new SmtpClient();
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential("ALM.migrationtool@gmail.com", "ALM_migration2017");
            client.Port = 587; // You can use Port 25 if 587 is blocked
            client.Host = "smtp.gmail.com";
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = true;
            try
            {
                client.Send(msg);
                result = true;//good
            }
            catch (Exception ex)
            {
                result = false;//error sending message
            }
            finally
            {
                client.Dispose();//close the connection
            }

            return result;
        }

        private String GetCheckText(Customer_Project c)
        {
            String checks = "<ul>";

            if (c.upgrade_type == false)
            {//SaaS To SaaS
                if (c.deactivation == true)
                {
                    checks += "<li><strong>Deactivate:</strong><ul>";
                    checks += "<li>Deactivate all projects</li>";
                    checks += "</ul></li>";
                }

                if (c.undo_checkouts || c.copy_file || c.copy_db || c.change_file)
                {
                    checks += "<li><strong>Data Copy:</strong><ul>";
                    if (c.undo_checkouts)
                    {
                        checks += "<li>If VC enabled, verify checkouts</li>";
                    }
                    if (c.copy_file)
                    {
                        checks += "<li>Copy repository files</li>";
                    }
                    if (c.copy_db)
                    {
                        checks += "<li>Copy DB schemas</li>";
                    }
                    if (c.change_file)
                    {
                        checks += "<li>Update configuration files</li>";
                    }
                    checks += "</ul></li>";
                }

                if (c.restore_project || c.verify_project || c.upgrade_project)
                {
                    checks += "<li><strong>Upgrade:</strong><ul>";
                    if (c.restore_project)
                    {
                        checks += "<li>Restore the projects from Site Admin</li>";
                    }
                    if (c.verify_project)
                    {
                        checks += "<li>Run Verify tool on projects to be migrated</li>";
                    }
                    if (c.upgrade_project)
                    {
                        checks += "<li>Upgrade projects to desired version</li>";
                    }
                    checks += "</ul></li>";
                }

                if (c.verification)
                {
                    checks += "<li><strong>Verification:</strong><ul>";
                    checks += "<li>Complete internal validation process</li>";
                    checks += "</ul></li>";
                }


                if (c.release)
                {
                    checks += "<li><strong>Release:</strong><ul>";
                    checks += "<li>Projects Released for validation</li>";
                    checks += "</ul></li>";
                }
            }
            else
            {//in house
                if (c.deactivation == true)
                {
                    checks += "<li><strong>Download:</strong><ul>";
                    checks += "<li>Download data from FTP</li>";
                    checks += "</ul></li>";
                }

                if (c.undo_checkouts || c.copy_file || c.copy_db || c.change_file)
                {
                    checks += "<li><strong>Data Copy:</strong><ul>";
                    if (c.copy_file)
                    {
                        checks += "<li>Copy repository files</li>";
                    }
                    if (c.copy_db)
                    {
                        checks += "<li>Copy DB schemas</li>";
                    }
                    if (c.change_file)
                    {
                        checks += "<li>Update configuration files</li>";
                    }
                    if (c.undo_checkouts)
                    {
                        checks += "<li>If VC enabled, verify checkouts</li>";
                    }
                    checks += "</ul></li>";
                }

                if (c.restore_project || c.verify_project || c.upgrade_project)
                {
                    checks += "<li><strong>Upgrade:</strong><ul>";
                    if (c.restore_project)
                    {
                        checks += "<li>Restore the projects from Site Admin</li>";
                    }
                    if (c.verify_project)
                    {
                        checks += "<li>Run Verify tool on projects to be migrated</li>";
                    }
                    if (c.upgrade_project)
                    {
                        checks += "<li>Upgrade projects to desired version</li>";
                    }
                    checks += "</ul></li>";
                }

                if (c.verification)
                {
                    checks += "<li><strong>Verification:</strong><ul>";
                    checks += "<li>Complete internal validation process</li>";
                    checks += "</ul></li>";
                }


                if (c.release)
                {
                    checks += "<li><strong>Release:</strong><ul>";
                    checks += "<li>Projects Released for validation</li>";
                    checks += "</ul></li>";
                }
            }

            checks += "</ul>";
            return checks;
        }

        public bool IsValidUrl(string url)
        {
            if (url == null)
            {
                return false;
            }

            try
            {
                Uri uriResult = new Uri(url);
                return Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute);
            }
            catch
            {
                return false;
            }
        }




    }
}