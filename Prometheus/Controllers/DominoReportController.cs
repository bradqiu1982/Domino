using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Domino.Models;

namespace Domino.Controllers
{
    public class DominoReportController : Controller
    {
        public ActionResult WorkLoad()
        {
            return View();
        }

        [HttpPost, ActionName("WorkLoad")]
        [ValidateAntiForgeryToken]
        public ActionResult WorkLoadPose()
        {
            if (string.IsNullOrEmpty(Request.Form["StartDate"])
                || string.IsNullOrEmpty(Request.Form["EndDate"])
                || (DateTime.Parse(Request.Form["StartDate"]) > DateTime.Parse(Request.Form["EndDate"])))
            {
                return View();
            }

            var startdate = DateTime.Parse(Request.Form["StartDate"]);
            var enddate = DateTime.Parse(Request.Form["EndDate"]);
            var workloaddata = DominoRPVM.RetrieveDepartWorkLoadData(startdate, enddate);
            if (workloaddata.Count == 0)
            {
                return View();
            }

            var ChartxAxisValues = string.Empty;
            int AmountMAX = 0;
            var CompleteAmount = string.Empty;
            var SignoffAmount = string.Empty;
            var OperationAmount = string.Empty;
            var HoldAmount = string.Empty;
            var FinishYield = string.Empty;

            var departs = DominoUserViewModels.RetrieveAllDepartment();
            foreach (var dpt in departs)
            {
                if (workloaddata.ContainsKey(dpt))
                {
                    double complete = 0;
                    double all = 0;

                    ChartxAxisValues = ChartxAxisValues+"'" + dpt + "',";

                    if (workloaddata[dpt].Complete == 0)
                    {
                        CompleteAmount = CompleteAmount + "null,";
                    }
                    else
                    {
                        CompleteAmount = CompleteAmount + workloaddata[dpt].Complete.ToString() + ",";
                        AmountMAX = AmountMAX + workloaddata[dpt].Complete;
                        complete = complete + workloaddata[dpt].Complete;
                        all = all + workloaddata[dpt].Complete;
                    }

                    if (workloaddata[dpt].SignOff == 0)
                    {
                        SignoffAmount = SignoffAmount + "null,";
                    }
                    else
                    {
                        SignoffAmount = SignoffAmount + workloaddata[dpt].SignOff.ToString() + ",";
                        AmountMAX = AmountMAX + workloaddata[dpt].SignOff;
                        complete = complete + workloaddata[dpt].SignOff;
                        all = all + workloaddata[dpt].SignOff;
                    }

                    if (workloaddata[dpt].Operation == 0)
                    {
                        OperationAmount = OperationAmount +"null,";
                    }
                    else
                    {
                        OperationAmount = OperationAmount + workloaddata[dpt].Operation.ToString() + ",";
                        AmountMAX = AmountMAX + workloaddata[dpt].Operation;
                        all = all + workloaddata[dpt].Operation;
                    }

                    if (workloaddata[dpt].Hold == 0)
                    {
                        HoldAmount = HoldAmount + "null,";
                    }
                    else
                    {
                        HoldAmount = HoldAmount + workloaddata[dpt].Hold.ToString() + ",";
                        AmountMAX = AmountMAX + workloaddata[dpt].Hold;
                        all = all + workloaddata[dpt].Hold;
                    }

                    if ((int)all == 0)
                    {
                        FinishYield = FinishYield + "0.00,";
                    }
                    else
                    {
                        FinishYield = FinishYield + ((complete / all) * 100.0).ToString("0.00")+",";
                    }
                }
            }

            ChartxAxisValues = ChartxAxisValues.Substring(0, ChartxAxisValues.Length - 1);
            CompleteAmount = CompleteAmount.Substring(0, CompleteAmount.Length - 1);
            SignoffAmount = SignoffAmount.Substring(0, SignoffAmount.Length - 1);
            OperationAmount = OperationAmount.Substring(0, OperationAmount.Length - 1);
            HoldAmount = HoldAmount.Substring(0, HoldAmount.Length - 1);
            FinishYield = FinishYield.Substring(0, FinishYield.Length - 1);

            var tempscript = System.IO.File.ReadAllText(Server.MapPath("~/Scripts/DominoWorkLoad.xml"));
            ViewBag.workloadchart = tempscript.Replace("#ElementID#", "workloadchart")
                .Replace("#Title#", "Department WorkLoad")
                .Replace("#ChartxAxisValues#", ChartxAxisValues)
                .Replace("#AmountMAX#", AmountMAX.ToString())
                .Replace("#CompleteAmount#", CompleteAmount)
                .Replace("#SignoffAmount#", SignoffAmount)
                .Replace("#OperationAmount#", OperationAmount)
                .Replace("#HoldAmount#", HoldAmount)
                .Replace("#FinishYield#", FinishYield);

            return View();
        }

    }
}