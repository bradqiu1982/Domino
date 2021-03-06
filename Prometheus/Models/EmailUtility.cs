﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Net;
using System.Net.Mail;
using System.Web.Routing;
using System.Collections.Specialized;
using Domino.Models;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Threading;
using System.Web.UI.WebControls;

namespace Domino.Models
{
    public class EmailUtility
    {
        private static void logthdinfo(string info)
        {
            try
            {
                var filename = "d:\\log\\emailexception-" + DateTime.Now.ToString("yyyy-MM-dd");
                if (System.IO.File.Exists(filename))
                {
                    var content = System.IO.File.ReadAllText(filename);
                    content = content + "\r\n" + DateTime.Now.ToString() + " : " + info;
                    System.IO.File.WriteAllText(filename, content);
                }
                else
                {
                    System.IO.File.WriteAllText(filename, DateTime.Now.ToString() + " : " + info);
                }
            }
            catch (Exception ex)
            {
            }
        }

        public static bool IsEmaileValid(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public static bool SendEmail(Controller ctrl, string title, List<string> tolist, string content, bool isHtml = true, string attachpath = null)
        {
            try
            {
                var syscfgdict = CfgUtility.GetSysConfig(ctrl);

                var message = new MailMessage();
                if (!string.IsNullOrEmpty(attachpath))
                {
                    var attach = new Attachment(attachpath);
                    message.Attachments.Add(attach);
                }

                message.IsBodyHtml = isHtml;
                message.From = new MailAddress(syscfgdict["APPEMAILADRESS"]);
                foreach (var item in tolist)
                {
                    if (!item.Contains("@"))
                        continue;

                    try
                    {
                        if (item.Contains(";") || item.Contains("/"))
                        {
                            var ts = item.Split(new string[] { ";","/" }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (var t in ts)
                            {
                                if (IsEmaileValid(t))
                                {
                                    message.To.Add(t);
                                }
                            }
                        }
                        else
                        {
                            if (IsEmaileValid(item))
                            {
                                message.To.Add(item);
                            }
                        }
                    }
                    catch (Exception e) { logthdinfo("Address exception: " + e.Message); }
                }

                message.Subject = title;
                message.Body = content.Replace("\r\n", "<br>").Replace("\r", "<br>");

                SmtpClient client = new SmtpClient();
                client.Host = syscfgdict["EMAILSERVER"];

                if (syscfgdict["EMAILSSL"].Contains("TRUE"))
                { client.EnableSsl = true; }
                else
                { client.EnableSsl = false; }

                client.Timeout = 60000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(syscfgdict["APPEMAILADRESS"], syscfgdict["APPEMAILPWD"]);

                ServicePointManager.ServerCertificateValidationCallback
                    = delegate (object s, X509Certificate certificate, X509Chain chain
                    , SslPolicyErrors sslPolicyErrors) { return true; };

                new Thread(() => {
                    try
                    {
                        client.Send(message);
                    }
                    catch (SmtpFailedRecipientsException ex)
                    {
                        logthdinfo("SmtpFailedRecipientsException exception: " + ex.Message);
                        try
                        {
                            message.To.Clear();
                            foreach (var item in tolist)
                            {
                                if (ex.Message.Contains(item))
                                {
                                    try
                                    {
                                        message.To.Add(item);
                                    }
                                    catch (Exception e) { logthdinfo("Address exception2: " + e.Message); }
                                }
                            }
                            client.Send(message);
                        }
                        catch (Exception ex1)
                        {
                            logthdinfo("nest exception1: " + ex1.Message);
                        }
                    }
                    catch (Exception ex)
                    {
                        logthdinfo("Domino1 send exception: " + ex.Message);
                    }
                }).Start();
            }
            catch (Exception ex)
            {
                logthdinfo("main exception: " + ex.Message);
                return false;
            }
            return true;
        }

        public static bool SendEmailWithAttach(Controller ctrl, string title, List<string> tolist, string content, List<string> attaches, bool isHtml = true)
        {
            try
            {
                var syscfgdict = CfgUtility.GetSysConfig(ctrl);

                var message = new MailMessage();
                foreach (var att in attaches)
                {
                    var attach = new Attachment(att);
                    message.Attachments.Add(attach);
                }

                message.IsBodyHtml = isHtml;
                message.From = new MailAddress(syscfgdict["APPEMAILADRESS"]);
                foreach (var item in tolist)
                {
                    if (!item.Contains("@"))
                        continue;

                    try
                    {
                        if (item.Contains(";") || item.Contains("/"))
                        {
                            var ts = item.Split(new string[] { ";", "/" }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (var t in ts)
                            {
                                if (IsEmaileValid(t))
                                {
                                    message.To.Add(t);
                                }
                            }
                        }
                        else
                        {
                            if (IsEmaileValid(item))
                            {
                                message.To.Add(item);
                            }
                        }
                    }
                    catch (Exception e) { logthdinfo("Address exception: " + e.Message); }
                }

                message.Subject = title;
                message.Body = content.Replace("\r\n", "<br>").Replace("\r", "<br>");

                SmtpClient client = new SmtpClient();
                client.Host = syscfgdict["EMAILSERVER"];

                if (syscfgdict["EMAILSSL"].Contains("TRUE"))
                { client.EnableSsl = true; }
                else
                { client.EnableSsl = false; }

                client.Timeout = 60000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(syscfgdict["APPEMAILADRESS"], syscfgdict["APPEMAILPWD"]);

                ServicePointManager.ServerCertificateValidationCallback
                    = delegate (object s, X509Certificate certificate, X509Chain chain
                    , SslPolicyErrors sslPolicyErrors) { return true; };

                new Thread(() => {
                    try
                    {
                        client.Send(message);
                    }
                    catch (SmtpFailedRecipientsException ex)
                    {
                        logthdinfo("SmtpFailedRecipientsException exception: " + ex.Message);
                        try
                        {
                            message.To.Clear();
                            foreach (var item in tolist)
                            {
                                if (ex.Message.Contains(item))
                                {
                                    try
                                    {
                                        message.To.Add(item);
                                    }
                                    catch (Exception e) { logthdinfo("Address exception2: " + e.Message); }
                                }
                            }
                            client.Send(message);
                        }
                        catch (Exception ex1)
                        {
                            logthdinfo("nest exception1: " + ex1.Message);
                        }
                    }
                    catch (Exception ex)
                    {
                        logthdinfo("Domino2 send exception: " + ex.Message);
                    }
                }).Start();
            }
            catch (Exception ex)
            {
                logthdinfo("main exception: " + ex.Message);
                return false;
            }
            return true;
        }

        public static string CreateTableHtml(string greetig, string description, string comment, List<List<string>> table)
        {
            var idx = 0;
            var content = "<!DOCTYPE html>";
            content += "<html>";
            content += "<head>";
            content += "<meta http-equiv='Content-Type' content='text/html; charset=UTF-8' />";
            content += "<title></title>";
            content += "</head>";
            content += "<body>";
            content += "<div><p>" + greetig + ",</p></div>";
            content += "<div><p>" + description + ".</p></div>";
            if (!string.IsNullOrEmpty(comment))
            {
                content += "<div><p>" + comment + "</p>";
            }
            if (table != null)
            {
                content += "<div><br>";
                content += "<table border='1' cellpadding='0' cellspacing='0' width='100%'>";
                content += "<thead style='background-color: #006DC0; color: #fff;'>";
                foreach (var th in table[0])
                {
                    content += "<th>" + th + "</th>";
                }
                content += "</thead>";
                foreach (var tr in table)
                {
                    if (idx > 0)
                    {
                        content += "<tr>";
                        foreach (var td in tr)
                        {
                            content += "<td>" + td + "</td>";
                        }
                        content += "</tr>";
                    }
                    idx++;
                }
                content += "</table>";
                content += "</div>";
            }
            content += "<br><br>";
            content += "<div><p style='font-size: 12px; font-style: italic;'>This is a system generated message, please remove WXNPI.Trace when replying.</p></div>";
            content += "</body>";
            content += "</html>";

            return content;
        }

        public static bool SendHtmlEmail(string title, List<string> tolist, string content)
        {
            try
            {
                MailDefinition md = new MailDefinition();
                md.From = "brad.qiu@II-VI.COM";
                md.Subject = title;
                md.IsBodyHtml = true;

                var message = md.CreateMailMessage("brad.qiu@II-VI.COM", new Dictionary<string, string>(), content, new System.Web.UI.Control());
                //message.From = new MailAddress("brad.qiu@II-VI.COM");
                //foreach (var item in tolist)
                //{
                //    message.To.Add(item);
                //}
                //message.Subject = title;
                //message.Body = content;

                SmtpClient client = new SmtpClient();
                client.Host = "wmail.finisar.com";
                client.EnableSsl = true;
                client.Timeout = 60000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential("brad.qiu@II-VI.COM", "wangle@4321");

                ServicePointManager.ServerCertificateValidationCallback
                    = delegate (object s, X509Certificate certificate, X509Chain chain
                    , SslPolicyErrors sslPolicyErrors) { return true; };

                new Thread(() => {
                    try
                    {
                        client.Send(message);
                    }
                    catch (Exception ex)
                    { }
                }).Start();
            }
            catch (Exception ex)
            {
                //System.Windows.MessageBox.Show(ex.ToString());
                return false;
            }
            return true;
        }
    }
}