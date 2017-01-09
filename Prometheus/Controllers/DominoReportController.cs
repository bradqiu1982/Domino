﻿using System;
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
                .Replace("#Title#", "Department WorkLoad "+startdate.ToString("yyyy/MM/dd")+"-"+enddate.ToString("yyyy/MM/dd"))
                .Replace("#ChartxAxisValues#", ChartxAxisValues)
                .Replace("#AmountMAX#", AmountMAX.ToString())
                .Replace("#CompleteAmount#", CompleteAmount)
                .Replace("#SignoffAmount#", SignoffAmount)
                .Replace("#OperationAmount#", OperationAmount)
                .Replace("#HoldAmount#", HoldAmount)
                .Replace("#FinishYield#", FinishYield);

            return View();
        }


        public ActionResult CycleTime()
        {
            return View();
        }

        [HttpPost, ActionName("CycleTime")]
        [ValidateAntiForgeryToken]
        public ActionResult CycleTimePose()
        {
            if (string.IsNullOrEmpty(Request.Form["StartDate"])
                || string.IsNullOrEmpty(Request.Form["EndDate"])
                || (DateTime.Parse(Request.Form["StartDate"]) > DateTime.Parse(Request.Form["EndDate"])))
            {
                return View();
            }

            var startdate = DateTime.Parse(Request.Form["StartDate"]);
            var enddate = DateTime.Parse(Request.Form["EndDate"]);
            var cycletimedict = DominoRPVM.RetrieveDepartCycleTimeData(startdate, enddate);
            if (cycletimedict.Count == 0)
            {
                return View();
            }

            var ChartxAxisValues = string.Empty;
            double DayMAX = 0;
            int AmountMAX = 0;
            double DayMIN = 0;

            var CCBSignoffAging = string.Empty;
            var TechReviewAging = string.Empty;
            var EngineeringAging = string.Empty;
            var ChangeDelayAging = string.Empty;
            var ApprovalAging = string.Empty;
            var TotalMiniPIPs = string.Empty;

            var departs = DominoUserViewModels.RetrieveAllDepartment();
            foreach (var dpt in departs)
            {
                if (cycletimedict.ContainsKey(dpt))
                {
                    ChartxAxisValues = ChartxAxisValues + "'" + dpt + "',";
                    AmountMAX = cycletimedict[dpt].TotalMiniPIPs + AmountMAX;

                    if (cycletimedict[dpt].CCBSignoffAgingAVG == 0)
                    {
                        CCBSignoffAging = CCBSignoffAging + "null,";
                    }
                    else
                    {
                        CCBSignoffAging = CCBSignoffAging + cycletimedict[dpt].CCBSignoffAgingAVG.ToString("0.0") + ",";
                        DayMAX = DayMAX + cycletimedict[dpt].CCBSignoffAgingAVG;

                        if (cycletimedict[dpt].CCBSignoffAgingAVG < 0)
                            DayMIN = DayMIN + cycletimedict[dpt].CCBSignoffAgingAVG;
                    }

                    if (cycletimedict[dpt].TechReviewAgingAVG == 0)
                    {
                        TechReviewAging = TechReviewAging + "null,";
                    }
                    else
                    {
                        TechReviewAging = TechReviewAging + cycletimedict[dpt].TechReviewAgingAVG.ToString("0.0") + ",";
                        DayMAX = DayMAX + cycletimedict[dpt].TechReviewAgingAVG;

                        if (cycletimedict[dpt].TechReviewAgingAVG < 0)
                            DayMIN = DayMIN + cycletimedict[dpt].TechReviewAgingAVG;
                    }

                    if (cycletimedict[dpt].EngineeringAgingAVG == 0)
                    {
                        EngineeringAging = EngineeringAging + "null,";
                    }
                    else
                    {
                        EngineeringAging = EngineeringAging + cycletimedict[dpt].EngineeringAgingAVG.ToString("0.0") + ",";
                        DayMAX = DayMAX + cycletimedict[dpt].EngineeringAgingAVG;

                        if (cycletimedict[dpt].EngineeringAgingAVG < 0)
                            DayMIN = DayMIN + cycletimedict[dpt].EngineeringAgingAVG;
                    }

                    if (cycletimedict[dpt].ChangeDelayAgingAVG == 0)
                    {
                        ChangeDelayAging = ChangeDelayAging + "null,";
                    }
                    else
                    {
                        ChangeDelayAging = ChangeDelayAging + cycletimedict[dpt].ChangeDelayAgingAVG.ToString("0.0") + ",";
                        DayMAX = DayMAX + cycletimedict[dpt].ChangeDelayAgingAVG;

                        if (cycletimedict[dpt].ChangeDelayAgingAVG < 0)
                            DayMIN = DayMIN + cycletimedict[dpt].ChangeDelayAgingAVG;
                    }

                    if (cycletimedict[dpt].MiniPIPApprovalAgingAVG == 0)
                    {
                        ApprovalAging = ApprovalAging + "null,";
                    }
                    else
                    {
                        ApprovalAging = ApprovalAging + cycletimedict[dpt].MiniPIPApprovalAgingAVG.ToString("0.0") + ",";
                        DayMAX = DayMAX + cycletimedict[dpt].MiniPIPApprovalAgingAVG;

                        if (cycletimedict[dpt].MiniPIPApprovalAgingAVG < 0)
                            DayMIN = DayMIN + cycletimedict[dpt].MiniPIPApprovalAgingAVG;
                    }

                    TotalMiniPIPs = TotalMiniPIPs + cycletimedict[dpt].TotalMiniPIPs.ToString() + ",";
                }
            }

            ChartxAxisValues = ChartxAxisValues.Substring(0, ChartxAxisValues.Length - 1);
            CCBSignoffAging = CCBSignoffAging.Substring(0, CCBSignoffAging.Length - 1);
            TechReviewAging = TechReviewAging.Substring(0, TechReviewAging.Length - 1);
            EngineeringAging = EngineeringAging.Substring(0, EngineeringAging.Length - 1);
            ChangeDelayAging = ChangeDelayAging.Substring(0, ChangeDelayAging.Length - 1);
            ApprovalAging = ApprovalAging.Substring(0, ApprovalAging.Length - 1);
            TotalMiniPIPs = TotalMiniPIPs.Substring(0, TotalMiniPIPs.Length - 1);

            var tempscript = System.IO.File.ReadAllText(Server.MapPath("~/Scripts/DominoCycleTime.xml"));
            ViewBag.cycletimechart = tempscript.Replace("#ElementID#", "cycletimechart")
                .Replace("#Title#", "Department CycleTime " + startdate.ToString("yyyy/MM/dd") + "-" + enddate.ToString("yyyy/MM/dd"))
                .Replace("#ChartxAxisValues#", ChartxAxisValues)
                .Replace("#AmountMAX#", AmountMAX.ToString())
                .Replace("#DayMIN#", DayMIN.ToString())
                .Replace("#DayMAX#", DayMAX.ToString())
                .Replace("#CCBSignoffAging#", CCBSignoffAging)
                .Replace("#TechReviewAging#", TechReviewAging)
                .Replace("#EngineeringAging#", EngineeringAging)
                .Replace("#ChangeDelayAging#", ChangeDelayAging)
                .Replace("#ApprovalAging#", ApprovalAging)
                .Replace("#TotalMiniPIPs#", TotalMiniPIPs);

            return View();
        }


        public ActionResult Complex()
        {
            return View();
        }

        [HttpPost, ActionName("Complex")]
        [ValidateAntiForgeryToken]
        public ActionResult ComplexPose()
        {
            if (string.IsNullOrEmpty(Request.Form["StartDate"])
                || string.IsNullOrEmpty(Request.Form["EndDate"])
                || (DateTime.Parse(Request.Form["StartDate"]) > DateTime.Parse(Request.Form["EndDate"])))
            {
                return View();
            }

            return View();
        }


    }

}