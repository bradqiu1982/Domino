using System;
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

namespace Domino.Controllers
{
    public class DominoUserController : Controller
    {

        private bool checkexistuser(string user)
        {
            return false;
        }


        public ActionResult RegisterUser()
        {
            return View();
        }
         
        private void SendActiveEmail(string username,string updatetime)
        {
            var routevalue = new RouteValueDictionary();
            routevalue.Add("validatestr", Convert.ToBase64String(UTF8Encoding.UTF8.GetBytes(username + "||" + updatetime)));
            //send validate email
            string scheme = this.Url.RequestContext.HttpContext.Request.Url.Scheme;
            string validatestr = this.Url.Action("ActiveUser", "DominoUser", routevalue, scheme);

            var netcomputername = "";
            try { netcomputername = System.Net.Dns.GetHostName(); }
            catch (Exception ex) { }
            validatestr = validatestr.Replace("//localhost", "//" + netcomputername);

            var toaddrs = new List<string>();
            toaddrs.Add(username);
            EmailUtility.SendEmail(this,"NPI Website Active Link",toaddrs, validatestr);
        }

        [HttpPost, ActionName("RegisterUser")]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterUserPOST()
        {

           if (DominoUserViewModels.CheckUserExist(Request.Form["Email"]))
            {
                var createerror = "<h3><font color=\"red\">Fail to create User: User Exist</font></h3>";
                ViewBag.CreateError = createerror;
                return View();
            }

            var username = Request.Form["Email"];
            var password = Request.Form["Password"];
            string updatetime = DateTime.Now.ToString();

            var user = new DominoUserViewModels();
            user.Email = username.ToUpper();
            user.Password = password;
            user.UpdateDate = DateTime.Parse(updatetime);
            user.RegistUser();

            SendActiveEmail(username, updatetime);

            return RedirectToAction("ValidateNoticeA");
        }

        public ActionResult ValidateNoticeA()
        {
            return View();
        }

        public ActionResult ValidateNoticeB()
        {
            return View();
        }

        public ActionResult ResetNoticeA()
        {
            return View();
        }

        public ActionResult ResetNoticeB()
        {
            return View();
        }

        public ActionResult ActiveUser(string validatestr)
        {
            if (string.IsNullOrEmpty(validatestr))
            {
                var createerror = "<h3><font color=\"red\">Fail to active User: active string is empty</font></h3>";
                ViewBag.CreateError = createerror;
                RedirectToAction("RegisterUser");
            }
            
            var bs = Convert.FromBase64String(validatestr);
            var val = UTF8Encoding.UTF8.GetString(bs);
            var username = val.Split(new string[] { "||" },StringSplitOptions.None)[0];
            var updatetime = val.Split(new string[] { "||" }, StringSplitOptions.None)[1];
            DominoUserViewModels.ActiveUser(username);
            return RedirectToAction("ValidateNoticeB");
        }

        public ActionResult ResetUser(string resetstr)
        {
            if (string.IsNullOrEmpty(resetstr))
            {
                var createerror = "<h3><font color=\"red\">Fail to reset User: reset string is empty</font></h3>";
                ViewBag.CreateError = createerror;
                return RedirectToAction("RegisterUser");
            }

            try
            {
                var bs = Convert.FromBase64String(resetstr);
                var val = UTF8Encoding.UTF8.GetString(bs);
                var username = val.Split(new string[] { "||" }, StringSplitOptions.None)[0];
                var updatetime = val.Split(new string[] { "||" }, StringSplitOptions.None)[1];
                var vm = new DominoUserViewModels();
                vm.Email = username;
                vm.Password = "";
                vm.ConfirmPassword = "";
                return View(vm);
            }
            catch (Exception ex)
            {
                var createerror = "<h3><font color=\"red\">Fail to reset User: reset string is wrong</font></h3>";
                ViewBag.CreateError = createerror;
                return RedirectToAction("RegisterUser");
            }

        }

        [HttpPost, ActionName("ResetUser")]
        [ValidateAntiForgeryToken]
        public ActionResult ResetUserPOST()
        {
            var username = Request.Form["Email"];
            var password = Request.Form["Password"];
            DominoUserViewModels.RestPwd(username, password);
            DominoUserViewModels.ActiveUser(username);
            return RedirectToAction("ResetNoticeB");
        }


        private ActionResult SendResetEmail(string username)
        {
            string updatetime = DateTime.Now.ToString();
            var routevalue = new RouteValueDictionary();
            routevalue.Add("resetstr", Convert.ToBase64String(UTF8Encoding.UTF8.GetBytes(username + "||" + updatetime)));
            //send validate email
            string scheme = this.Url.RequestContext.HttpContext.Request.Url.Scheme;
            string validatestr = this.Url.Action("ResetUser", "DominoUser", routevalue, scheme);
            var netcomputername = "";
            try { netcomputername = System.Net.Dns.GetHostName(); }
            catch (Exception ex) { }
            validatestr = validatestr.Replace("//localhost", "//" + netcomputername);

            var toaddrs = new List<string>();
            toaddrs.Add(username);
            EmailUtility.SendEmail(this,"NPI Website Active Link", toaddrs, validatestr);
            return RedirectToAction("ResetNoticeA");
        }

        public ActionResult LoginUser()
        {
            return View();
        }

        private ActionResult NormalLogin(string username,string dbpwd,string inputpwd)
        {
            if (string.Compare(dbpwd, inputpwd) != 0)
            {
                var loginerror = "<h3><font color=\"red\">Fail to login: password not correct</font></h3>";
                ViewBag.loginerror = loginerror;
                return View("LoginUser");
            }

            var ckdict = CookieUtility.UnpackCookie(this);
            if (ckdict.ContainsKey("logonredirectctrl") 
                && ckdict.ContainsKey("logonredirectact")
                && !string.IsNullOrEmpty(ckdict["logonredirectact"])
                && !string.IsNullOrEmpty(ckdict["logonredirectctrl"]))
            {
                var logonredirectact = ckdict["logonredirectact"];
                var logonredirectctrl = ckdict["logonredirectctrl"];

                //verify user information
                string logonuser = username + "||" + DateTime.Now.ToString();
                var ck = new Dictionary<string, string>();
                ck.Add("logonuser", logonuser);
                ck.Add("logonredirectact","");
                ck.Add("logonredirectctrl","");
                CookieUtility.SetCookie(this, ck);

                //UserRankViewModel.UpdateUserRank(username,1);

                return RedirectToAction(logonredirectact, logonredirectctrl);
            }
            else
            {
                //verify user information
                string logonuser = username + "||" + DateTime.Now.ToString();
                var ck = new Dictionary<string, string>();
                ck.Add("logonuser", logonuser);
                CookieUtility.SetCookie(this, ck);

                //UserRankViewModel.UpdateUserRank(username, 1);

                return RedirectToAction("ViewAll", "MiniPIP");
            }
        }

        public static void RegisterUserAuto(string name)
        {
            var dbret = DominoUserViewModels.RetrieveUser(name);
            if (dbret == null)
            {
                var user = new DominoUserViewModels();
                user.Email = name.ToUpper();
                user.Password = "abc@123";
                user.UpdateDate = DateTime.Now;
                user.RegistUser();
                DominoUserViewModels.ActiveUser(user.Email);
            }
        }

        [HttpPost, ActionName("LoginUser")]
        [ValidateAntiForgeryToken]
        public ActionResult LoginUserPOST()
        {
            var username = Request.Form["Email"].ToUpper();
            var password = Request.Form["Password"];

            var dbret = DominoUserViewModels.RetrieveUser(username);
            if (dbret == null)
            {
                if (string.Compare(password, "abc@123", true) == 0)
                {
                    var user = new DominoUserViewModels();
                    user.Email = username.ToUpper();
                    user.Password = password;
                    user.UpdateDate = DateTime.Now;
                    user.RegistUser();
                    DominoUserViewModels.ActiveUser(user.Email);

                    dbret = DominoUserViewModels.RetrieveUser(username);
                }
                else
                {
                    var loginerror = "<h3><font color=\"red\">Fail to login: user not exist</font></h3>";
                    ViewBag.loginerror = loginerror;
                    return View();
                }
            }

            if (dbret.Validated == 0)
            {
                var loginerror = "<h3><font color=\"red\">Fail to login: user is not actived</font></h3>";
                ViewBag.loginerror = loginerror;
                string updatetime = DateTime.Now.ToString();
                DominoUserViewModels.UpdateUserTime(username, DateTime.Parse(updatetime));
                SendActiveEmail(username, updatetime);
                return RedirectToAction("ValidateNoticeA");
            }

            if (Request.Form["Login"] != null)
            {
                return NormalLogin(username, dbret.Password, password);
            }
            else if (Request.Form["ForgetPassword"] != null)
            {
                return SendResetEmail(username);
            }
            else
            {
                return View();
            }
        }


        public ActionResult LoginOutUser(string ctrl, string action)
        {
            var val = CookieUtility.UnpackCookie(this);
            val["logonuser"] = "";
            CookieUtility.SetCookie(this, val);
            return RedirectToAction("ViewAll", "MiniPIP");
        }

        public ActionResult UserCenter()
        {
            return View();
        }

        [HttpPost, ActionName("SaveCacheInfo")]
        public string SaveCacheInfo()
        {
            var ckdict = CookieUtility.UnpackCookie(this);
            var updater = ckdict["logonuser"].Split(new char[] { '|' })[0];

            foreach (var key in Request.Form.Keys)
            {
                DominoUserCacheVM.InsertCacheInfo(updater, Request.Form.Get(key.ToString()));
            }

            return "SAVED";
        }

        public ActionResult UserCachedInfo()
        {
            var ckdict = CookieUtility.UnpackCookie(this);
            if (ckdict.ContainsKey("logonuser") && !string.IsNullOrEmpty(ckdict["logonuser"]))
            {

            }
            else
            {
                var ck = new Dictionary<string, string>();
                ck.Add("logonredirectctrl", "User");
                ck.Add("logonredirectact", "UserCachedInfo");
                CookieUtility.SetCookie(this, ck);
                return RedirectToAction("LoginUser", "User");
            }

            var updater = ckdict["logonuser"].Split(new char[] { '|' })[0];
            var vm = DominoUserCacheVM.RetrieveCacheInfo(updater);

            return View(vm);
        }

    }
}