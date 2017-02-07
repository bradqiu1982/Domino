using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Http;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Domino
{

    public class MyActionFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //if (filterContext.HttpContext.Request.Path.ToUpper().Contains("/MINIPIP/VIEWALL")
            //    ||string.Compare(filterContext.HttpContext.Request.Path.ToUpper(),"/DOMINO") == 0)
            //{
            //    base.OnActionExecuting(filterContext);
            //    return;
            //}

            var fooCookie = filterContext.HttpContext.Request.Cookies["activenpi"];

            if (fooCookie != null)
            {
                var ret = new Dictionary<string, string>();
                try
                {
                    foreach (var key in fooCookie.Values.AllKeys)
                    {
                        ret.Add(key, UTF8Encoding.UTF8.GetString(Convert.FromBase64String(fooCookie.Values[key])));
                    }
                }
                catch (Exception ex)
                {
                    ret.Clear();
                }

                if (ret.ContainsKey("logonuser") && !string.IsNullOrEmpty(ret["logonuser"]))
                {
                    var username = ret["logonuser"].Split(new char[] { '|' })[0];
                    if (!Domino.Models.DominoUserViewModels.UserExist(username))
                    {
                        //filterContext.Result = new RedirectToRouteResult("Domino", new RouteValueDictionary(new { action = "ViewAll", controller = "MiniPIP" }));
                        filterContext.Result = new HttpUnauthorizedResult();
                        return;
                    }
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            AreaRegistration.RegisterAllAreas();

            RouteConfig.RegisterRoutes(RouteTable.Routes);

            GlobalFilters.Filters.Add(new MyActionFilterAttribute());
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            try
            {
                using (Process myprocess = new Process())
                {
                    myprocess.StartInfo.FileName = Path.Combine(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath, @"Scripts\HeartBeat4Domino.exe").Replace("\\", "/");
                    //System.Windows.MessageBox.Show(myprocess.StartInfo.FileName);
                    //myprocess.StartInfo.CreateNoWindow = true;
                    myprocess.Start();
                }
            }
            catch (Exception ex)
            { }
        }
    }
}
