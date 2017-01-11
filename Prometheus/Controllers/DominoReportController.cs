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


        public ActionResult FABuilding()
        {
            return View();
        }

        [HttpPost, ActionName("FABuilding")]
        [ValidateAntiForgeryToken]
        public ActionResult FABuildingPose()
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

            var SampleShipAging = string.Empty;
            var TotalMiniPIPs = string.Empty;

            var departs = DominoUserViewModels.RetrieveAllDepartment();
            foreach (var dpt in departs)
            {
                if (cycletimedict.ContainsKey(dpt))
                {
                    

                    if (cycletimedict[dpt].SampleShipAgingAVG == 0)
                    {
                        //SampleShipAging = SampleShipAging + "null,";
                    }
                    else
                    {
                        ChartxAxisValues = ChartxAxisValues + "'" + dpt + "',";
                        AmountMAX = cycletimedict[dpt].SampleShipAgingTM + AmountMAX+1;
                        SampleShipAging = SampleShipAging + cycletimedict[dpt].SampleShipAgingAVG.ToString("0.0") + ",";
                        DayMAX = DayMAX + cycletimedict[dpt].SampleShipAgingAVG;

                        if (cycletimedict[dpt].SampleShipAgingAVG < 0)
                            DayMIN = DayMIN + cycletimedict[dpt].SampleShipAgingAVG;

                        TotalMiniPIPs = TotalMiniPIPs + cycletimedict[dpt].SampleShipAgingTM.ToString() + ",";
                    }
                }
            }

            if (string.IsNullOrEmpty(ChartxAxisValues))
            {
                return View();
            }

            ChartxAxisValues = ChartxAxisValues.Substring(0, ChartxAxisValues.Length - 1);
            SampleShipAging = SampleShipAging.Substring(0, SampleShipAging.Length - 1);
            TotalMiniPIPs = TotalMiniPIPs.Substring(0, TotalMiniPIPs.Length - 1);

            var tempscript = System.IO.File.ReadAllText(Server.MapPath("~/Scripts/DominoFABuilding.xml"));
            ViewBag.fabuildingchart = tempscript.Replace("#ElementID#", "fabuildingchart")
                .Replace("#Title#", "Department FA Build Time " + startdate.ToString("yyyy/MM/dd") + "-" + enddate.ToString("yyyy/MM/dd"))
                .Replace("#ChartxAxisValues#", ChartxAxisValues)
                .Replace("#AmountMAX#", AmountMAX.ToString())
                .Replace("#DayMIN#", DayMIN.ToString())
                .Replace("#DayMAX#", DayMAX.ToString())
                .Replace("#SampleShipAging#", SampleShipAging)
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

            var startdate = DateTime.Parse(Request.Form["StartDate"]);
            var enddate = DateTime.Parse(Request.Form["EndDate"]);
            var complexdict = DominoRPVM.RetrieveDepartComplexData(startdate, enddate);
            if (complexdict.Count == 0)
            {
                return View();
            }

            var ChartxAxisValues = string.Empty;
            int AmountMAX = 0;

            var EXPEDITE = string.Empty;
            var MEDIUM = string.Empty;
            var SMALL = string.Empty;

            var departs = DominoUserViewModels.RetrieveAllDepartment();
            foreach (var dpt in departs)
            {
                if (complexdict.ContainsKey(dpt))
                {
                    ChartxAxisValues = ChartxAxisValues + "'" + dpt + "',";

                    if (complexdict[dpt].E == 0)
                    {
                        EXPEDITE = EXPEDITE + "null,";
                    }
                    else
                    {
                        EXPEDITE = EXPEDITE + complexdict[dpt].E.ToString() + ",";
                        AmountMAX = AmountMAX + complexdict[dpt].E;
                    }

                    if (complexdict[dpt].M == 0)
                    {
                        MEDIUM = MEDIUM + "null,";
                    }
                    else
                    {
                        MEDIUM = MEDIUM + complexdict[dpt].M.ToString() + ",";
                        AmountMAX = AmountMAX + complexdict[dpt].M;
                    }

                    if (complexdict[dpt].S == 0)
                    {
                        SMALL = SMALL + "null,";
                    }
                    else
                    {
                        SMALL = SMALL + complexdict[dpt].S.ToString() + ",";
                        AmountMAX = AmountMAX + complexdict[dpt].S;
                    }
                }
            }

            ChartxAxisValues = ChartxAxisValues.Substring(0, ChartxAxisValues.Length - 1);
            EXPEDITE = EXPEDITE.Substring(0, EXPEDITE.Length - 1);
            MEDIUM = MEDIUM.Substring(0, MEDIUM.Length - 1);
            SMALL = SMALL.Substring(0, SMALL.Length - 1);

            var tempscript = System.IO.File.ReadAllText(Server.MapPath("~/Scripts/DominoComplex.xml"));
            ViewBag.complexchart = tempscript.Replace("#ElementID#", "complexchart")
                .Replace("#Title#", "Department Type " + startdate.ToString("yyyy/MM/dd") + "-" + enddate.ToString("yyyy/MM/dd"))
                .Replace("#ChartxAxisValues#", ChartxAxisValues)
                .Replace("#AmountMAX#", AmountMAX.ToString())
                .Replace("#EXPEDITE#", EXPEDITE)
                .Replace("#MEDIUM#", MEDIUM)
                .Replace("#SMALL#", SMALL);


            return View();
        }


        public ActionResult QAFACheck()
        {
            return View();
        }

        [HttpPost, ActionName("QAFACheck")]
        [ValidateAntiForgeryToken]
        public ActionResult QAFACheckPose()
        {
            if (string.IsNullOrEmpty(Request.Form["StartDate"])
                || string.IsNullOrEmpty(Request.Form["EndDate"])
                || (DateTime.Parse(Request.Form["StartDate"]) > DateTime.Parse(Request.Form["EndDate"])))
            {
                return View();
            }

            var startdate = DateTime.Parse(Request.Form["StartDate"]);
            var enddate = DateTime.Parse(Request.Form["EndDate"]);
            var qafadict = DominoRPVM.RetrieveDepartQACheckData(this,startdate, enddate);
            if (qafadict.Count == 0)
            {
                return View();
            }

            var ChartxAxisValues = string.Empty;
            int AmountMAX = 0;

            var PassQTY = string.Empty;
            var FailQTY = string.Empty;
            var FailRate = string.Empty;

            var departs = DominoUserViewModels.RetrieveAllDepartment();
            foreach (var dpt in departs)
            {
                if (qafadict.ContainsKey(dpt))
                {
                    ChartxAxisValues = ChartxAxisValues + "'" + dpt + "',";

                    if (qafadict[dpt].EEPROMPASS == 0)
                    {
                        PassQTY = PassQTY + "null,";
                    }
                    else
                    {
                        PassQTY = PassQTY + qafadict[dpt].EEPROMPASS.ToString() + ",";
                        AmountMAX = AmountMAX + qafadict[dpt].EEPROMPASS;
                    }

                    if (qafadict[dpt].EEPROMFAIL == 0)
                    {
                        FailQTY = FailQTY + "null,";
                    }
                    else
                    {
                        FailQTY = FailQTY + qafadict[dpt].EEPROMFAIL.ToString() + ",";
                        AmountMAX = AmountMAX + qafadict[dpt].EEPROMFAIL;
                    }

                    if ((qafadict[dpt].EEPROMPASS+ qafadict[dpt].EEPROMFAIL) == 0)
                    {
                        FailRate = FailRate + "100.0,";
                    }
                    else
                    {
                        FailRate = FailRate + (((double)qafadict[dpt].EEPROMFAIL/(double)(qafadict[dpt].EEPROMPASS + qafadict[dpt].EEPROMFAIL))*100.0).ToString("0.0") + ",";
                    }
                }
            }

            ChartxAxisValues = ChartxAxisValues.Substring(0, ChartxAxisValues.Length - 1);
            PassQTY = PassQTY.Substring(0, PassQTY.Length - 1);
            FailQTY = FailQTY.Substring(0, FailQTY.Length - 1);
            FailRate = FailRate.Substring(0, FailRate.Length - 1);

            var tempscript = System.IO.File.ReadAllText(Server.MapPath("~/Scripts/DominoQAFACheck.xml"));
            ViewBag.qafachart = tempscript.Replace("#ElementID#", "qafachart")
                .Replace("#Title#", "EEPROM QA Fail Rate/Fail QTY " + startdate.ToString("yyyy/MM/dd") + "-" + enddate.ToString("yyyy/MM/dd"))
                .Replace("#ChartxAxisValues#", ChartxAxisValues)
                .Replace("#AmountMAX#", AmountMAX.ToString())
                .Replace("#PassQTY#", PassQTY)
                .Replace("#FailQTY#", FailQTY)
                .Replace("#FailRate#", FailRate);

            return View();
        }

    }

}