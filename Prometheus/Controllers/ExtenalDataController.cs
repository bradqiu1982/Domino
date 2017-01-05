using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Domino.Models;

namespace Domino.Controllers
{
    public class ExtenalDataController : Controller
    {
        public ActionResult CommitUserMatrix()
        {
            return View();
        }

        [HttpPost, ActionName("CommitUserMatrix")]
        [ValidateAntiForgeryToken]
        public ActionResult CommitUserMatrixPost()
        {
            var wholefn = "";
            try
            {
                if (!string.IsNullOrEmpty(Request.Form["RMAFileName"]))
                {
                    var customereportfile = Request.Form["RMAFileName"];
                    var originalname = Path.GetFileNameWithoutExtension(customereportfile).Replace(" ", "_").Replace("#", "")
                    .Replace("&", "").Replace("?", "").Replace("%", "").Replace("+", "");

                    foreach (string fl in Request.Files)
                    {
                        if (fl != null && Request.Files[fl].ContentLength > 0)
                        {
                            string fn = Path.GetFileName(Request.Files[fl].FileName).Replace(" ", "_").Replace("#", "")
                    .Replace("&", "").Replace("?", "").Replace("%", "").Replace("+", "");

                            string datestring = DateTime.Now.ToString("yyyyMMdd");
                            string imgdir = Server.MapPath("~/userfiles") + "\\docs\\" + datestring + "\\";

                            if (!Directory.Exists(imgdir))
                            {
                                Directory.CreateDirectory(imgdir);
                            }

                            fn = Path.GetFileNameWithoutExtension(fn) + "-" + DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(fn);
                            Request.Files[fl].SaveAs(imgdir + fn);

                            if (fn.Contains(originalname))
                            {
                                wholefn = imgdir + fn;
                                break;
                            }
                        }//end if
                    }//end foreach

                    if (!string.IsNullOrEmpty(wholefn))
                    {
                        var data = ExcelReader.RetrieveDataFromExcel(wholefn,null);
                        var realdata = new List<List<string>>();
                        if (data.Count > 1)
                        {
                            var templine = new List<string>();
                            templine.Add("Engineer");
                            templine.Add("Department");
                            realdata.Add(templine);

                            for (var idx = 0; idx < data.Count; idx++)
                            {
                                if (idx != 0)
                                {
                                        templine = new List<string>();
                                        
                                        if (!data[idx][0].Contains("@"))
                                        {
                                            templine.Add((data[idx][0].Trim().Replace(" ", ".") + "@finisar.com").ToUpper());
                                        }
                                        else
                                        {
                                            templine.Add(data[idx][0].ToUpper());
                                        }

                                        templine.Add(data[idx][1]);

                                        realdata.Add(templine);
                                }//end if
                            }//end for
                        }

                        if (realdata.Count > 1)
                        {
                            ViewBag.ROWCOUNT = realdata.Count;
                            ViewBag.COLCOUNT = realdata[0].Count;
                            return View("ConfirmUserMatrix", realdata);
                        }
                    }

                }//end if
            }
            catch (Exception ex)
            { }

            return View();

        }

        [HttpPost, ActionName("ConfirmUserMatrix")]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmUserMatrix()
        {
            if (Request.Form["confirmdata"] != null)
            {

                var rowcnt = Convert.ToInt32(Request.Form["rowcount"]);
                var colcnt = Convert.ToInt32(Request.Form["colcount"]);
                var data = new List<List<string>>();
                for (var row = 0; row < rowcnt; row++)
                {
                    var line = new List<string>();
                    for (var col = 0; col < colcnt; col++)
                    {
                        line.Add(Request.Form["row" + row + "col" + col]);
                    }
                    data.Add(line);
                }


                if (data.Count > 1)
                {
                    for (int i = 0; i < data.Count; i++)
                    {
                        if (i != 0)
                        {
                            var username = data[i][0];
                            var depart = data[i][1];
                            DominoUserViewModels.StoreUserMatrix(username, depart);
                        }
                    }//end for
                    return RedirectToAction("ViewAll", "MiniPIP");
                }
            }

            return View();

        }

    }
}