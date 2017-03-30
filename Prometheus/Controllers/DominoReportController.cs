using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Domino.Models;
using System.IO;

namespace Domino.Controllers
{
    public class DominoReportController : Controller
    {

        private List<SelectListItem> CreateSelectList(List<string> valist, string defVal)
        {
            bool selected = false;
            var pslist = new List<SelectListItem>();
            foreach (var p in valist)
            {
                var pitem = new SelectListItem();
                pitem.Text = p;
                pitem.Value = p;
                if (!string.IsNullOrEmpty(defVal) && string.Compare(defVal, p, true) == 0)
                {
                    pitem.Selected = true;
                    selected = true;
                }
                pslist.Add(pitem);
            }

            if (!selected && pslist.Count > 0)
            {
                pslist[0].Selected = true;
            }

            return pslist;
        }

        private void GetAdminAuth()
        {
            ViewBag.badmin = false;
            var ckdict = CookieUtility.UnpackCookie(this);
            if (ckdict.ContainsKey("logonuser"))
            {
                ViewBag.badmin = DominoUserViewModels.IsAdmin(ckdict["logonuser"].Split(new char[] { '|' })[0]);
            }
        }

        public ActionResult WorkLoad()
        {
            GetAdminAuth();

            var charts = new string[]{ DominoChartType.Department, DominoChartType.PE, DominoChartType.Monthly, DominoChartType.Quarter, DominoChartType.Customer };
            var chartlist = new List<string>();
            chartlist.AddRange(charts);
            ViewBag.charttypelist = CreateSelectList(chartlist, "");

            return View();
        }

        private void DepartWorkLoad(string filename)
        {
            var startdate = DateTime.Parse(Request.Form["StartDate"]);
            var enddate = DateTime.Parse(Request.Form["EndDate"]);
            var title = Request.Form["ChartTitle"];

            var workloaddata = DominoRPVM.RetrieveDepartWorkLoadData(startdate, enddate,filename);
            if (workloaddata.Count == 0)
            {
                return;
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
                    int tempmax = 0;

                    ChartxAxisValues = ChartxAxisValues+"'" + dpt + "',";

                    if (workloaddata[dpt].Complete == 0)
                    {
                        CompleteAmount = CompleteAmount + "null,";
                    }
                    else
                    {
                        CompleteAmount = CompleteAmount + workloaddata[dpt].Complete.ToString() + ",";
                        tempmax = tempmax + workloaddata[dpt].Complete;
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
                        tempmax = tempmax + workloaddata[dpt].SignOff;
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
                        tempmax = tempmax + workloaddata[dpt].Operation;
                        all = all + workloaddata[dpt].Operation;
                    }

                    if (workloaddata[dpt].Hold == 0)
                    {
                        HoldAmount = HoldAmount + "null,";
                    }
                    else
                    {
                        HoldAmount = HoldAmount + workloaddata[dpt].Hold.ToString() + ",";
                        tempmax = tempmax + workloaddata[dpt].Hold;
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

                    if (tempmax > AmountMAX)
                        AmountMAX = tempmax;
                }
            }

            ChartxAxisValues = ChartxAxisValues.Substring(0, ChartxAxisValues.Length - 1);
            CompleteAmount = CompleteAmount.Substring(0, CompleteAmount.Length - 1);
            SignoffAmount = SignoffAmount.Substring(0, SignoffAmount.Length - 1);
            OperationAmount = OperationAmount.Substring(0, OperationAmount.Length - 1);
            HoldAmount = HoldAmount.Substring(0, HoldAmount.Length - 1);
            FinishYield = FinishYield.Substring(0, FinishYield.Length - 1);

            var temptitle = "Department WorkLoad " + startdate.ToString("yyyy/MM/dd") + "-" + enddate.ToString("yyyy/MM/dd");
            if (!string.IsNullOrEmpty(title.Trim()))
                temptitle = title.Trim();

            var tempscript = System.IO.File.ReadAllText(Server.MapPath("~/Scripts/DominoWorkLoad.xml"));
            ViewBag.workloadchart = tempscript.Replace("#ElementID#", "workloadchart")
                .Replace("#Title#", temptitle)
                .Replace("#ChartxAxisValues#", ChartxAxisValues)
                .Replace("#AmountMAX#", (AmountMAX + 1).ToString())
                .Replace("#CompleteAmount#", CompleteAmount)
                .Replace("#SignoffAmount#", SignoffAmount)
                .Replace("#OperationAmount#", OperationAmount)
                .Replace("#HoldAmount#", HoldAmount)
                .Replace("#FinishYield#", FinishYield);
        }

        private void DictWorkLoad(string charttype,string filename)
        {
            var startdate = DateTime.Parse(Request.Form["StartDate"]);
            var enddate = DateTime.Parse(Request.Form["EndDate"]);
            var title = Request.Form["ChartTitle"];


            var workloaddata = new Dictionary<string, WorkLoadField>();

            if (string.Compare(charttype, DominoChartType.PE) == 0)
            {
                workloaddata = DominoRPVM.RetrievePEWorkLoadData(startdate, enddate,filename);
            }
            else if (string.Compare(charttype, DominoChartType.Customer) == 0)
            {
                workloaddata = DominoRPVM.RetrieveCustomerWorkLoadData(startdate, enddate, filename);
            }
            else if (string.Compare(charttype, DominoChartType.Monthly) == 0)
            {
                workloaddata = DominoRPVM.RetrieveMonthlyWorkLoadData(startdate, enddate, filename);
            }
            else if (string.Compare(charttype, DominoChartType.Quarter) == 0)
            {
                workloaddata = DominoRPVM.RetrieveQuartWorkLoadData(startdate, enddate, filename);
            }

            if (workloaddata.Count == 0)
            {
                return;
            }

            var ChartxAxisValues = string.Empty;
            int AmountMAX = 0;
            var CompleteAmount = string.Empty;
            var SignoffAmount = string.Empty;
            var OperationAmount = string.Empty;
            var HoldAmount = string.Empty;
            var FinishYield = string.Empty;

            var pes = new List<string>();
            if (string.Compare(charttype, DominoChartType.Monthly) == 0
                || string.Compare(charttype, DominoChartType.Quarter) == 0)
            {
                pes = workloaddata.Keys.ToList();
                pes.Sort(delegate (string sk1, string sk2)
                {
                    int ik1 = Convert.ToInt32(sk1.Replace("/", "").Replace("Q",""));
                    int ik2 = Convert.ToInt32(sk2.Replace("/", "").Replace("Q",""));
                    if (ik1 > ik2) return 1;
                    if (ik1 < ik2) return -1;
                    return 0;
                });
            }
            else
            {
                pes = workloaddata.Keys.ToList();
                pes.Sort();
            }

            foreach (var pe in pes)
            {
                if (workloaddata.ContainsKey(pe))
                {
                    int tempmax = 0;
                    double complete = 0;
                    double all = 0;

                    ChartxAxisValues = ChartxAxisValues + "'" + pe.Split(new string[] { "@" },StringSplitOptions.RemoveEmptyEntries)[0] + "',";

                    if (workloaddata[pe].Complete == 0)
                    {
                        CompleteAmount = CompleteAmount + "null,";
                    }
                    else
                    {
                        CompleteAmount = CompleteAmount + workloaddata[pe].Complete.ToString() + ",";
                        tempmax = tempmax + workloaddata[pe].Complete;
                        complete = complete + workloaddata[pe].Complete;
                        all = all + workloaddata[pe].Complete;
                    }

                    if (workloaddata[pe].SignOff == 0)
                    {
                        SignoffAmount = SignoffAmount + "null,";
                    }
                    else
                    {
                        SignoffAmount = SignoffAmount + workloaddata[pe].SignOff.ToString() + ",";
                        tempmax = tempmax + workloaddata[pe].SignOff;
                        complete = complete + workloaddata[pe].SignOff;
                        all = all + workloaddata[pe].SignOff;
                    }

                    if (workloaddata[pe].Operation == 0)
                    {
                        OperationAmount = OperationAmount + "null,";
                    }
                    else
                    {
                        OperationAmount = OperationAmount + workloaddata[pe].Operation.ToString() + ",";
                        tempmax = tempmax + workloaddata[pe].Operation;
                        all = all + workloaddata[pe].Operation;
                    }

                    if (workloaddata[pe].Hold == 0)
                    {
                        HoldAmount = HoldAmount + "null,";
                    }
                    else
                    {
                        HoldAmount = HoldAmount + workloaddata[pe].Hold.ToString() + ",";
                        tempmax = tempmax + workloaddata[pe].Hold;
                        all = all + workloaddata[pe].Hold;
                    }

                    if ((int)all == 0)
                    {
                        FinishYield = FinishYield + "0.00,";
                    }
                    else
                    {
                        FinishYield = FinishYield + ((complete / all) * 100.0).ToString("0.00") + ",";
                    }

                    if (tempmax > AmountMAX)
                        AmountMAX = tempmax;
                }
            }

            ChartxAxisValues = ChartxAxisValues.Substring(0, ChartxAxisValues.Length - 1);
            CompleteAmount = CompleteAmount.Substring(0, CompleteAmount.Length - 1);
            SignoffAmount = SignoffAmount.Substring(0, SignoffAmount.Length - 1);
            OperationAmount = OperationAmount.Substring(0, OperationAmount.Length - 1);
            HoldAmount = HoldAmount.Substring(0, HoldAmount.Length - 1);
            FinishYield = FinishYield.Substring(0, FinishYield.Length - 1);

            var temptitle = charttype + " WorkLoad " + startdate.ToString("yyyy/MM/dd") + "-" + enddate.ToString("yyyy/MM/dd");
            if (!string.IsNullOrEmpty(title.Trim()))
                temptitle = title.Trim();

            var tempscript = System.IO.File.ReadAllText(Server.MapPath("~/Scripts/DominoWorkLoad.xml"));
            ViewBag.workloadchart = tempscript.Replace("#ElementID#", "workloadchart")
                .Replace("#Title#", temptitle)
                .Replace("#ChartxAxisValues#", ChartxAxisValues)
                .Replace("#AmountMAX#", (AmountMAX+1).ToString())
                .Replace("#CompleteAmount#", CompleteAmount)
                .Replace("#SignoffAmount#", SignoffAmount)
                .Replace("#OperationAmount#", OperationAmount)
                .Replace("#HoldAmount#", HoldAmount)
                .Replace("#FinishYield#", FinishYield);
        }


        [HttpPost, ActionName("WorkLoad")]
        [ValidateAntiForgeryToken]
        public ActionResult WorkLoadPose()
        {
            GetAdminAuth();

            var charts = new string[] { DominoChartType.Department, DominoChartType.PE, DominoChartType.Monthly, DominoChartType.Quarter, DominoChartType.Customer };
            var chartlist = new List<string>();
            chartlist.AddRange(charts);
            ViewBag.charttypelist = CreateSelectList(chartlist, "");

            if (string.IsNullOrEmpty(Request.Form["StartDate"])
                || string.IsNullOrEmpty(Request.Form["EndDate"])
                || (DateTime.Parse(Request.Form["StartDate"]) > DateTime.Parse(Request.Form["EndDate"])))
            {
                return View();
            }

            var fn = "Workload-data" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
            string datestring = DateTime.Now.ToString("yyyyMMdd");
            string imgdir = Server.MapPath("~/userfiles") + "\\docs\\" + datestring + "\\";
            if (!Directory.Exists(imgdir))
            {
                Directory.CreateDirectory(imgdir);
            }
            var realpath = imgdir + fn;
            ViewBag.workloadurl = "/userfiles/docs/" + datestring + "/" + fn;

            var charttype = Request.Form["charttypelist"].ToString();
            if (string.Compare(charttype, DominoChartType.Department) == 0)
            {
                DepartWorkLoad(realpath);
            }
            else
            {
                DictWorkLoad(charttype, realpath);
            }

            return View();
        }


        public ActionResult CycleTime()
        {
            GetAdminAuth();

            var charts = new string[] { DominoChartType.Department, DominoChartType.PE, DominoChartType.Monthly, DominoChartType.Quarter, DominoChartType.Customer };
            var chartlist = new List<string>();
            chartlist.AddRange(charts);
            ViewBag.charttypelist = CreateSelectList(chartlist, "");

            return View();
        }

        private void DepartCycleTime(string filename)
        {
            var startdate = DateTime.Parse(Request.Form["StartDate"]);
            var enddate = DateTime.Parse(Request.Form["EndDate"]);
            var title = Request.Form["ChartTitle"];


            var cycletimedict = DominoRPVM.RetrieveDepartCycleTimeData(startdate, enddate,filename);
            if (cycletimedict.Count == 0)
            {
                return;
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
                    double tempmaxday = 0.0;

                    ChartxAxisValues = ChartxAxisValues + "'" + dpt + "',";
                    //AmountMAX = cycletimedict[dpt].TotalMiniPIPs + AmountMAX;
                    if (cycletimedict[dpt].TotalMiniPIPs > AmountMAX)
                        AmountMAX = cycletimedict[dpt].TotalMiniPIPs;

                    if (cycletimedict[dpt].CCBSignoffAgingAVG == 0)
                    {
                        CCBSignoffAging = CCBSignoffAging + "null,";
                    }
                    else
                    {
                        CCBSignoffAging = CCBSignoffAging + cycletimedict[dpt].CCBSignoffAgingAVG.ToString("0.0") + ",";
                        tempmaxday = tempmaxday + cycletimedict[dpt].CCBSignoffAgingAVG;

                        if (cycletimedict[dpt].CCBSignoffAgingAVG < 0 && cycletimedict[dpt].CCBSignoffAgingAVG < DayMIN)
                            DayMIN = cycletimedict[dpt].CCBSignoffAgingAVG;
                    }

                    if (cycletimedict[dpt].TechReviewAgingAVG == 0)
                    {
                        TechReviewAging = TechReviewAging + "null,";
                    }
                    else
                    {
                        TechReviewAging = TechReviewAging + cycletimedict[dpt].TechReviewAgingAVG.ToString("0.0") + ",";
                        tempmaxday = tempmaxday + cycletimedict[dpt].TechReviewAgingAVG;

                        if (cycletimedict[dpt].TechReviewAgingAVG < 0 && cycletimedict[dpt].TechReviewAgingAVG < DayMIN)
                            DayMIN =  cycletimedict[dpt].TechReviewAgingAVG;
                    }

                    if (cycletimedict[dpt].EngineeringAgingAVG == 0)
                    {
                        EngineeringAging = EngineeringAging + "null,";
                    }
                    else
                    {
                        EngineeringAging = EngineeringAging + cycletimedict[dpt].EngineeringAgingAVG.ToString("0.0") + ",";
                        tempmaxday = tempmaxday + cycletimedict[dpt].EngineeringAgingAVG;

                        if (cycletimedict[dpt].EngineeringAgingAVG < 0 && cycletimedict[dpt].EngineeringAgingAVG < DayMIN)
                            DayMIN =  cycletimedict[dpt].EngineeringAgingAVG;
                    }

                    if (cycletimedict[dpt].ChangeDelayAgingAVG == 0)
                    {
                        ChangeDelayAging = ChangeDelayAging + "null,";
                    }
                    else
                    {
                        ChangeDelayAging = ChangeDelayAging + cycletimedict[dpt].ChangeDelayAgingAVG.ToString("0.0") + ",";
                        tempmaxday = tempmaxday + cycletimedict[dpt].ChangeDelayAgingAVG;

                        if (cycletimedict[dpt].ChangeDelayAgingAVG < 0 && cycletimedict[dpt].ChangeDelayAgingAVG < DayMIN)
                            DayMIN =  cycletimedict[dpt].ChangeDelayAgingAVG;
                    }

                    if (cycletimedict[dpt].MiniPIPApprovalAgingAVG == 0)
                    {
                        ApprovalAging = ApprovalAging + "null,";
                    }
                    else
                    {
                        ApprovalAging = ApprovalAging + cycletimedict[dpt].MiniPIPApprovalAgingAVG.ToString("0.0") + ",";
                        tempmaxday = tempmaxday + cycletimedict[dpt].MiniPIPApprovalAgingAVG;

                        if (cycletimedict[dpt].MiniPIPApprovalAgingAVG < 0 && cycletimedict[dpt].MiniPIPApprovalAgingAVG < DayMIN)
                            DayMIN =  cycletimedict[dpt].MiniPIPApprovalAgingAVG;
                    }

                    TotalMiniPIPs = TotalMiniPIPs + cycletimedict[dpt].TotalMiniPIPs.ToString() + ",";

                    if (tempmaxday > DayMAX)
                        DayMAX = tempmaxday;
                }
            }

            ChartxAxisValues = ChartxAxisValues.Substring(0, ChartxAxisValues.Length - 1);
            CCBSignoffAging = CCBSignoffAging.Substring(0, CCBSignoffAging.Length - 1);
            TechReviewAging = TechReviewAging.Substring(0, TechReviewAging.Length - 1);
            EngineeringAging = EngineeringAging.Substring(0, EngineeringAging.Length - 1);
            ChangeDelayAging = ChangeDelayAging.Substring(0, ChangeDelayAging.Length - 1);
            ApprovalAging = ApprovalAging.Substring(0, ApprovalAging.Length - 1);
            TotalMiniPIPs = TotalMiniPIPs.Substring(0, TotalMiniPIPs.Length - 1);

            var temptitle = "Department CycleTime " + startdate.ToString("yyyy/MM/dd") + "-" + enddate.ToString("yyyy/MM/dd");
            if (!string.IsNullOrEmpty(title.Trim()))
                temptitle = title.Trim();

            var tempscript = System.IO.File.ReadAllText(Server.MapPath("~/Scripts/DominoCycleTime.xml"));
            ViewBag.cycletimechart = tempscript.Replace("#ElementID#", "cycletimechart")
                .Replace("#Title#", temptitle)
                .Replace("#ChartxAxisValues#", ChartxAxisValues)
                .Replace("#AmountMAX#", (AmountMAX+1).ToString())
                .Replace("#DayMIN#", DayMIN.ToString())
                .Replace("#DayMAX#", (DayMAX+3.0).ToString())
                .Replace("#CCBSignoffAging#", CCBSignoffAging)
                .Replace("#TechReviewAging#", TechReviewAging)
                .Replace("#EngineeringAging#", EngineeringAging)
                .Replace("#ChangeDelayAging#", ChangeDelayAging)
                .Replace("#ApprovalAging#", ApprovalAging)
                .Replace("#TotalMiniPIPs#", TotalMiniPIPs);
        }

        private void DictCycleTime(string charttype,string filename)
        {
            var startdate = DateTime.Parse(Request.Form["StartDate"]);
            var enddate = DateTime.Parse(Request.Form["EndDate"]);
            var title = Request.Form["ChartTitle"];

            var cycletimedict = new Dictionary<string, CycleTimeDataField>();

            if (string.Compare(charttype, DominoChartType.PE) == 0)
            {
                cycletimedict = DominoRPVM.RetrievePECycleTimeData(startdate, enddate,filename);
            }
            else if (string.Compare(charttype, DominoChartType.Customer) == 0)
            {
                cycletimedict = DominoRPVM.RetrieveCustomerCycleTimeData(startdate, enddate,filename);
            }
            else if (string.Compare(charttype, DominoChartType.Monthly) == 0)
            {
                cycletimedict = DominoRPVM.RetrieveMonthlyCycleTimeData(startdate, enddate,filename);
            }
            else if (string.Compare(charttype, DominoChartType.Quarter) == 0)
            {
                cycletimedict = DominoRPVM.RetrieveQuartCycleTimeData(startdate, enddate,filename);
            }

            if (cycletimedict.Count == 0)
            {
                return;
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

            var pes = new List<string>();
            if (string.Compare(charttype, DominoChartType.Monthly) == 0
                || string.Compare(charttype, DominoChartType.Quarter) == 0)
            {
                pes = cycletimedict.Keys.ToList();
                pes.Sort(delegate (string sk1, string sk2)
                {
                    int ik1 = Convert.ToInt32(sk1.Replace("/", "").Replace("Q",""));
                    int ik2 = Convert.ToInt32(sk2.Replace("/", "").Replace("Q",""));
                    if (ik1 > ik2) return 1;
                    if (ik1 < ik2) return -1;
                    return 0;
                });
            }
            else
            {
                pes = cycletimedict.Keys.ToList();
                pes.Sort();
            }

            foreach (var pe in pes)
            {
                if (cycletimedict.ContainsKey(pe))
                {
                    double tempmaxday = 0.0;

                    ChartxAxisValues = ChartxAxisValues + "'" + pe.Split(new string[] { "@" }, StringSplitOptions.RemoveEmptyEntries)[0] + "',";
                    //AmountMAX = cycletimedict[dpt].TotalMiniPIPs + AmountMAX;
                    if (cycletimedict[pe].TotalMiniPIPs > AmountMAX)
                        AmountMAX = cycletimedict[pe].TotalMiniPIPs;

                    if (cycletimedict[pe].CCBSignoffAgingAVG == 0)
                    {
                        CCBSignoffAging = CCBSignoffAging + "null,";
                    }
                    else
                    {
                        CCBSignoffAging = CCBSignoffAging + cycletimedict[pe].CCBSignoffAgingAVG.ToString("0.0") + ",";
                        tempmaxday = tempmaxday + cycletimedict[pe].CCBSignoffAgingAVG;

                        if (cycletimedict[pe].CCBSignoffAgingAVG < 0 && cycletimedict[pe].CCBSignoffAgingAVG < DayMIN)
                            DayMIN = cycletimedict[pe].CCBSignoffAgingAVG;
                    }

                    if (cycletimedict[pe].TechReviewAgingAVG == 0)
                    {
                        TechReviewAging = TechReviewAging + "null,";
                    }
                    else
                    {
                        TechReviewAging = TechReviewAging + cycletimedict[pe].TechReviewAgingAVG.ToString("0.0") + ",";
                        tempmaxday = tempmaxday + cycletimedict[pe].TechReviewAgingAVG;

                        if (cycletimedict[pe].TechReviewAgingAVG < 0 && cycletimedict[pe].TechReviewAgingAVG < DayMIN)
                            DayMIN =  cycletimedict[pe].TechReviewAgingAVG;
                    }

                    if (cycletimedict[pe].EngineeringAgingAVG == 0)
                    {
                        EngineeringAging = EngineeringAging + "null,";
                    }
                    else
                    {
                        EngineeringAging = EngineeringAging + cycletimedict[pe].EngineeringAgingAVG.ToString("0.0") + ",";
                        tempmaxday = tempmaxday + cycletimedict[pe].EngineeringAgingAVG;

                        if (cycletimedict[pe].EngineeringAgingAVG < 0 && cycletimedict[pe].EngineeringAgingAVG < DayMIN)
                            DayMIN =  cycletimedict[pe].EngineeringAgingAVG;
                    }

                    if (cycletimedict[pe].ChangeDelayAgingAVG == 0)
                    {
                        ChangeDelayAging = ChangeDelayAging + "null,";
                    }
                    else
                    {
                        ChangeDelayAging = ChangeDelayAging + cycletimedict[pe].ChangeDelayAgingAVG.ToString("0.0") + ",";
                        tempmaxday = tempmaxday + cycletimedict[pe].ChangeDelayAgingAVG;

                        if (cycletimedict[pe].ChangeDelayAgingAVG < 0 && cycletimedict[pe].ChangeDelayAgingAVG < DayMIN)
                            DayMIN =  cycletimedict[pe].ChangeDelayAgingAVG;
                    }

                    if (cycletimedict[pe].MiniPIPApprovalAgingAVG == 0)
                    {
                        ApprovalAging = ApprovalAging + "null,";
                    }
                    else
                    {
                        ApprovalAging = ApprovalAging + cycletimedict[pe].MiniPIPApprovalAgingAVG.ToString("0.0") + ",";
                        tempmaxday = tempmaxday + cycletimedict[pe].MiniPIPApprovalAgingAVG;

                        if (cycletimedict[pe].MiniPIPApprovalAgingAVG < 0 && cycletimedict[pe].MiniPIPApprovalAgingAVG < DayMIN)
                            DayMIN =  cycletimedict[pe].MiniPIPApprovalAgingAVG;
                    }

                    TotalMiniPIPs = TotalMiniPIPs + cycletimedict[pe].TotalMiniPIPs.ToString() + ",";

                    if (tempmaxday > DayMAX)
                        DayMAX = tempmaxday;
                }
            }

            ChartxAxisValues = ChartxAxisValues.Substring(0, ChartxAxisValues.Length - 1);
            CCBSignoffAging = CCBSignoffAging.Substring(0, CCBSignoffAging.Length - 1);
            TechReviewAging = TechReviewAging.Substring(0, TechReviewAging.Length - 1);
            EngineeringAging = EngineeringAging.Substring(0, EngineeringAging.Length - 1);
            ChangeDelayAging = ChangeDelayAging.Substring(0, ChangeDelayAging.Length - 1);
            ApprovalAging = ApprovalAging.Substring(0, ApprovalAging.Length - 1);
            TotalMiniPIPs = TotalMiniPIPs.Substring(0, TotalMiniPIPs.Length - 1);

            var temptitle = charttype + " CycleTime " + startdate.ToString("yyyy/MM/dd") + "-" + enddate.ToString("yyyy/MM/dd");
            if (!string.IsNullOrEmpty(title.Trim()))
                temptitle = title.Trim();

            var tempscript = System.IO.File.ReadAllText(Server.MapPath("~/Scripts/DominoCycleTime.xml"));
            ViewBag.cycletimechart = tempscript.Replace("#ElementID#", "cycletimechart")
                .Replace("#Title#", temptitle)
                .Replace("#ChartxAxisValues#", ChartxAxisValues)
                .Replace("#AmountMAX#", (AmountMAX+1).ToString())
                .Replace("#DayMIN#", DayMIN.ToString())
                .Replace("#DayMAX#", (DayMAX+3.0).ToString())
                .Replace("#CCBSignoffAging#", CCBSignoffAging)
                .Replace("#TechReviewAging#", TechReviewAging)
                .Replace("#EngineeringAging#", EngineeringAging)
                .Replace("#ChangeDelayAging#", ChangeDelayAging)
                .Replace("#ApprovalAging#", ApprovalAging)
                .Replace("#TotalMiniPIPs#", TotalMiniPIPs);
        }

        [HttpPost, ActionName("CycleTime")]
        [ValidateAntiForgeryToken]
        public ActionResult CycleTimePose()
        {
            GetAdminAuth();

            var charts = new string[] { DominoChartType.Department, DominoChartType.PE, DominoChartType.Monthly, DominoChartType.Quarter, DominoChartType.Customer };
            var chartlist = new List<string>();
            chartlist.AddRange(charts);
            ViewBag.charttypelist = CreateSelectList(chartlist, "");

            if (string.IsNullOrEmpty(Request.Form["StartDate"])
                || string.IsNullOrEmpty(Request.Form["EndDate"])
                || (DateTime.Parse(Request.Form["StartDate"]) > DateTime.Parse(Request.Form["EndDate"])))
            {
                return View();
            }

            var fn = "CycleTime-data" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
            string datestring = DateTime.Now.ToString("yyyyMMdd");
            string imgdir = Server.MapPath("~/userfiles") + "\\docs\\" + datestring + "\\";
            if (!Directory.Exists(imgdir))
            {
                Directory.CreateDirectory(imgdir);
            }
            var realpath = imgdir + fn;
            ViewBag.cycleurl = "/userfiles/docs/" + datestring + "/" + fn;


            var charttype = Request.Form["charttypelist"].ToString();

            if (string.Compare(charttype, DominoChartType.Department) == 0)
            {
                DepartCycleTime(realpath);
            }
            else
            {
                DictCycleTime(charttype, realpath);
            }

            return View();
        }


        public ActionResult FABuilding()
        {
            GetAdminAuth();

            var charts = new string[] { DominoChartType.Department, DominoChartType.PE, DominoChartType.Monthly, DominoChartType.Quarter, DominoChartType.Customer };
            var chartlist = new List<string>();
            chartlist.AddRange(charts);
            ViewBag.charttypelist = CreateSelectList(chartlist, "");

            return View();
        }

        private void DepartFABuilding(string filename)
        {
            var startdate = DateTime.Parse(Request.Form["StartDate"]);
            var enddate = DateTime.Parse(Request.Form["EndDate"]);
            var title = Request.Form["ChartTitle"];

            var cycletimedict = DominoRPVM.RetrieveDepartCycleTimeData(startdate, enddate,filename);
            if (cycletimedict.Count == 0)
            {
                return;
            }

            var ChartxAxisValues = string.Empty;
            double DAYMAX = 0;
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
                        if (cycletimedict[dpt].SampleShipAgingTM > AmountMAX)
                            AmountMAX = cycletimedict[dpt].SampleShipAgingTM;

                        SampleShipAging = SampleShipAging + cycletimedict[dpt].SampleShipAgingAVG.ToString("0.0") + ",";

                        if (cycletimedict[dpt].SampleShipAgingAVG > DAYMAX)
                            DAYMAX = cycletimedict[dpt].SampleShipAgingAVG;

                        if (cycletimedict[dpt].SampleShipAgingAVG < 0)
                            DayMIN = DayMIN + cycletimedict[dpt].SampleShipAgingAVG;

                        TotalMiniPIPs = TotalMiniPIPs + cycletimedict[dpt].SampleShipAgingTM.ToString() + ",";
                    }
                }
            }

            if (string.IsNullOrEmpty(ChartxAxisValues))
            {
                return;
            }

            ChartxAxisValues = ChartxAxisValues.Substring(0, ChartxAxisValues.Length - 1);
            SampleShipAging = SampleShipAging.Substring(0, SampleShipAging.Length - 1);
            TotalMiniPIPs = TotalMiniPIPs.Substring(0, TotalMiniPIPs.Length - 1);

            var temptitle = "Depart FA Build Time " + startdate.ToString("yyyy/MM/dd") + "-" + enddate.ToString("yyyy/MM/dd");
            if (!string.IsNullOrEmpty(title.Trim()))
                temptitle = title.Trim();

            var tempscript = System.IO.File.ReadAllText(Server.MapPath("~/Scripts/DominoFABuilding.xml"));
            ViewBag.fabuildingchart = tempscript.Replace("#ElementID#", "fabuildingchart")
                .Replace("#Title#", temptitle)
                .Replace("#ChartxAxisValues#", ChartxAxisValues)
                .Replace("#AmountMAX#", (AmountMAX + 1).ToString())
                .Replace("#DayMIN#", DayMIN.ToString())
                .Replace("#DayMAX#", (DAYMAX + 1.0).ToString())
                .Replace("#SampleShipAging#", SampleShipAging)
                .Replace("#TotalMiniPIPs#", TotalMiniPIPs);
        }

        private void DictFABuilding(string charttype,string filename)
        {
            var startdate = DateTime.Parse(Request.Form["StartDate"]);
            var enddate = DateTime.Parse(Request.Form["EndDate"]);
            var title = Request.Form["ChartTitle"];

            var cycletimedict = new Dictionary<string, CycleTimeDataField>();
            if (string.Compare(charttype, DominoChartType.PE) == 0)
            {
                cycletimedict = DominoRPVM.RetrievePECycleTimeData(startdate, enddate,filename);
            }
            else if (string.Compare(charttype, DominoChartType.Customer) == 0)
            {
                cycletimedict = DominoRPVM.RetrieveCustomerCycleTimeData(startdate, enddate,filename);
            }
            else if (string.Compare(charttype, DominoChartType.Monthly) == 0)
            {
                cycletimedict = DominoRPVM.RetrieveMonthlyCycleTimeData(startdate, enddate,filename);
            }
            else if (string.Compare(charttype, DominoChartType.Quarter) == 0)
            {
                cycletimedict = DominoRPVM.RetrieveQuartCycleTimeData(startdate, enddate,filename);
            }

            if (cycletimedict.Count == 0)
            {
                return;
            }

            var ChartxAxisValues = string.Empty;
            double DAYMAX = 0;
            int AmountMAX = 0;
            double DayMIN = 0;

            var SampleShipAging = string.Empty;
            var TotalMiniPIPs = string.Empty;

            var pes = new List<string>();
            if (string.Compare(charttype, DominoChartType.Monthly) == 0
                || string.Compare(charttype, DominoChartType.Quarter) == 0)
            {
                pes = cycletimedict.Keys.ToList();
                pes.Sort(delegate (string sk1, string sk2)
                {
                    int ik1 = Convert.ToInt32(sk1.Replace("/", "").Replace("Q",""));
                    int ik2 = Convert.ToInt32(sk2.Replace("/", "").Replace("Q",""));
                    if (ik1 > ik2) return 1;
                    if (ik1 < ik2) return -1;
                    return 0;
                });
            }
            else
            {
                pes = cycletimedict.Keys.ToList();
                pes.Sort();
            }

            foreach (var pe in pes)
            {
                if (cycletimedict.ContainsKey(pe))
                {
                    if (cycletimedict[pe].SampleShipAgingAVG == 0)
                    {
                        //SampleShipAging = SampleShipAging + "null,";
                    }
                    else
                    {
                        ChartxAxisValues = ChartxAxisValues + "'" + pe.Split(new string[] { "@" }, StringSplitOptions.RemoveEmptyEntries)[0] + "',";

                        if (cycletimedict[pe].SampleShipAgingTM > AmountMAX)
                            AmountMAX = cycletimedict[pe].SampleShipAgingTM;

                        SampleShipAging = SampleShipAging + cycletimedict[pe].SampleShipAgingAVG.ToString("0.0") + ",";

                        if (cycletimedict[pe].SampleShipAgingAVG > DAYMAX)
                            DAYMAX = cycletimedict[pe].SampleShipAgingAVG;

                        if (cycletimedict[pe].SampleShipAgingAVG < 0)
                            DayMIN = DayMIN + cycletimedict[pe].SampleShipAgingAVG;

                        TotalMiniPIPs = TotalMiniPIPs + cycletimedict[pe].SampleShipAgingTM.ToString() + ",";
                    }
                }
            }

            if (string.IsNullOrEmpty(ChartxAxisValues))
            {
                return;
            }

            ChartxAxisValues = ChartxAxisValues.Substring(0, ChartxAxisValues.Length - 1);
            SampleShipAging = SampleShipAging.Substring(0, SampleShipAging.Length - 1);
            TotalMiniPIPs = TotalMiniPIPs.Substring(0, TotalMiniPIPs.Length - 1);

            var temptitle = charttype+" FA Build Time " + startdate.ToString("yyyy/MM/dd") + "-" + enddate.ToString("yyyy/MM/dd");
            if (!string.IsNullOrEmpty(title.Trim()))
                temptitle = title.Trim();

            var tempscript = System.IO.File.ReadAllText(Server.MapPath("~/Scripts/DominoFABuilding.xml"));
            ViewBag.fabuildingchart = tempscript.Replace("#ElementID#", "fabuildingchart")
                .Replace("#Title#", temptitle)
                .Replace("#ChartxAxisValues#", ChartxAxisValues)
                .Replace("#AmountMAX#", (AmountMAX + 1).ToString())
                .Replace("#DayMIN#", DayMIN.ToString())
                .Replace("#DayMAX#", (DAYMAX + 1.0).ToString())
                .Replace("#SampleShipAging#", SampleShipAging)
                .Replace("#TotalMiniPIPs#", TotalMiniPIPs);
        }

        [HttpPost, ActionName("FABuilding")]
        [ValidateAntiForgeryToken]
        public ActionResult FABuildingPose()
        {
            GetAdminAuth();

            var charts = new string[] { DominoChartType.Department, DominoChartType.PE, DominoChartType.Monthly, DominoChartType.Quarter, DominoChartType.Customer };
            var chartlist = new List<string>();
            chartlist.AddRange(charts);
            ViewBag.charttypelist = CreateSelectList(chartlist, "");

            if (string.IsNullOrEmpty(Request.Form["StartDate"])
                || string.IsNullOrEmpty(Request.Form["EndDate"])
                || (DateTime.Parse(Request.Form["StartDate"]) > DateTime.Parse(Request.Form["EndDate"])))
            {
                return View();
            }

            var fn = "CycleTime-data" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
            string datestring = DateTime.Now.ToString("yyyyMMdd");
            string imgdir = Server.MapPath("~/userfiles") + "\\docs\\" + datestring + "\\";
            if (!Directory.Exists(imgdir))
            {
                Directory.CreateDirectory(imgdir);
            }
            var realpath = imgdir + fn;
            ViewBag.cycleurl = "/userfiles/docs/" + datestring + "/" + fn;

            var charttype = Request.Form["charttypelist"].ToString();
            if (string.Compare(charttype, DominoChartType.Department) == 0)
            {
                DepartFABuilding(realpath);
            }
            else
            {
                DictFABuilding(charttype, realpath);
            }

            return View();
        }
        

        public ActionResult Complex()
        {
            GetAdminAuth();

            var charts = new string[] { DominoChartType.Department, DominoChartType.PE, DominoChartType.Monthly, DominoChartType.Quarter, DominoChartType.Customer };
            var chartlist = new List<string>();
            chartlist.AddRange(charts);
            ViewBag.charttypelist = CreateSelectList(chartlist, "");

            return View();
        }

        private void DepartComplex()
        {
            var startdate = DateTime.Parse(Request.Form["StartDate"]);
            var enddate = DateTime.Parse(Request.Form["EndDate"]);
            var title = Request.Form["ChartTitle"];

            var complexdict = DominoRPVM.RetrieveDepartComplexData(startdate, enddate);
            if (complexdict.Count == 0)
            {
                return;
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
                    int tempmaxamount = 0;
                    ChartxAxisValues = ChartxAxisValues + "'" + dpt + "',";

                    if (complexdict[dpt].E == 0)
                    {
                        EXPEDITE = EXPEDITE + "null,";
                    }
                    else
                    {
                        EXPEDITE = EXPEDITE + complexdict[dpt].E.ToString() + ",";
                        tempmaxamount = tempmaxamount + complexdict[dpt].E;
                    }

                    if (complexdict[dpt].M == 0)
                    {
                        MEDIUM = MEDIUM + "null,";
                    }
                    else
                    {
                        MEDIUM = MEDIUM + complexdict[dpt].M.ToString() + ",";
                        tempmaxamount = tempmaxamount + complexdict[dpt].M;
                    }

                    if (complexdict[dpt].S == 0)
                    {
                        SMALL = SMALL + "null,";
                    }
                    else
                    {
                        SMALL = SMALL + complexdict[dpt].S.ToString() + ",";
                        tempmaxamount = tempmaxamount + complexdict[dpt].S;
                    }

                    if (tempmaxamount > AmountMAX)
                        AmountMAX = tempmaxamount;
                }
            }

            ChartxAxisValues = ChartxAxisValues.Substring(0, ChartxAxisValues.Length - 1);
            EXPEDITE = EXPEDITE.Substring(0, EXPEDITE.Length - 1);
            MEDIUM = MEDIUM.Substring(0, MEDIUM.Length - 1);
            SMALL = SMALL.Substring(0, SMALL.Length - 1);

            var temptitle = "Department Type " + startdate.ToString("yyyy/MM/dd") + "-" + enddate.ToString("yyyy/MM/dd");
            if (!string.IsNullOrEmpty(title.Trim()))
                temptitle = title.Trim();

            var tempscript = System.IO.File.ReadAllText(Server.MapPath("~/Scripts/DominoComplex.xml"));
            ViewBag.complexchart = tempscript.Replace("#ElementID#", "complexchart")
                .Replace("#Title#", temptitle)
                .Replace("#ChartxAxisValues#", ChartxAxisValues)
                .Replace("#AmountMAX#", (AmountMAX+1).ToString())
                .Replace("#EXPEDITE#", EXPEDITE)
                .Replace("#MEDIUM#", MEDIUM)
                .Replace("#SMALL#", SMALL);
        }

        private void DictComplex(string charttype)
        {
            var startdate = DateTime.Parse(Request.Form["StartDate"]);
            var enddate = DateTime.Parse(Request.Form["EndDate"]);
            var title = Request.Form["ChartTitle"];

            var complexdict = new Dictionary<string, ComplexData>();

            if (string.Compare(charttype, DominoChartType.PE) == 0)
            {
                complexdict = DominoRPVM.RetrievePEComplexData(startdate, enddate);
            }
            else if (string.Compare(charttype, DominoChartType.Customer) == 0)
            {
                complexdict = DominoRPVM.RetrieveCustomerComplexData(startdate, enddate);
            }
            else if (string.Compare(charttype, DominoChartType.Monthly) == 0)
            {
                complexdict = DominoRPVM.RetrieveMonthlyComplexData(startdate, enddate);
            }
            else if (string.Compare(charttype, DominoChartType.Quarter) == 0)
            {
                complexdict = DominoRPVM.RetrieveQuartComplexData(startdate, enddate);
            }

            if (complexdict.Count == 0)
            {
                return;
            }

            var ChartxAxisValues = string.Empty;
            int AmountMAX = 0;

            var EXPEDITE = string.Empty;
            var MEDIUM = string.Empty;
            var SMALL = string.Empty;



            var pes = new List<string>();
            if (string.Compare(charttype, DominoChartType.Monthly) == 0
                || string.Compare(charttype, DominoChartType.Quarter) == 0)
            {
                pes = complexdict.Keys.ToList();
                pes.Sort(delegate (string sk1, string sk2)
                {
                    int ik1 = Convert.ToInt32(sk1.Replace("/", "").Replace("Q",""));
                    int ik2 = Convert.ToInt32(sk2.Replace("/", "").Replace("Q",""));
                    if (ik1 > ik2) return 1;
                    if (ik1 < ik2) return -1;
                    return 0;
                });
            }
            else
            {
                pes = complexdict.Keys.ToList();
                pes.Sort();
            }

            foreach (var pe in pes)
            {
                if (complexdict.ContainsKey(pe))
                {
                    int tempmaxamount = 0;
                    ChartxAxisValues = ChartxAxisValues + "'" + pe.Split(new string[] { "@" }, StringSplitOptions.RemoveEmptyEntries)[0] + "',";

                    if (complexdict[pe].E == 0)
                    {
                        EXPEDITE = EXPEDITE + "null,";
                    }
                    else
                    {
                        EXPEDITE = EXPEDITE + complexdict[pe].E.ToString() + ",";
                        tempmaxamount = tempmaxamount + complexdict[pe].E;
                    }

                    if (complexdict[pe].M == 0)
                    {
                        MEDIUM = MEDIUM + "null,";
                    }
                    else
                    {
                        MEDIUM = MEDIUM + complexdict[pe].M.ToString() + ",";
                        tempmaxamount = tempmaxamount + complexdict[pe].M;
                    }

                    if (complexdict[pe].S == 0)
                    {
                        SMALL = SMALL + "null,";
                    }
                    else
                    {
                        SMALL = SMALL + complexdict[pe].S.ToString() + ",";
                        tempmaxamount = tempmaxamount + complexdict[pe].S;
                    }

                    if (tempmaxamount > AmountMAX)
                        AmountMAX = tempmaxamount;
                }
            }

            ChartxAxisValues = ChartxAxisValues.Substring(0, ChartxAxisValues.Length - 1);
            EXPEDITE = EXPEDITE.Substring(0, EXPEDITE.Length - 1);
            MEDIUM = MEDIUM.Substring(0, MEDIUM.Length - 1);
            SMALL = SMALL.Substring(0, SMALL.Length - 1);

            var temptitle = charttype+" Type " + startdate.ToString("yyyy/MM/dd") + "-" + enddate.ToString("yyyy/MM/dd");
            if (!string.IsNullOrEmpty(title.Trim()))
                temptitle = title.Trim();

            var tempscript = System.IO.File.ReadAllText(Server.MapPath("~/Scripts/DominoComplex.xml"));
            ViewBag.complexchart = tempscript.Replace("#ElementID#", "complexchart")
                .Replace("#Title#", temptitle)
                .Replace("#ChartxAxisValues#", ChartxAxisValues)
                .Replace("#AmountMAX#", (AmountMAX + 1).ToString())
                .Replace("#EXPEDITE#", EXPEDITE)
                .Replace("#MEDIUM#", MEDIUM)
                .Replace("#SMALL#", SMALL);
        }

        [HttpPost, ActionName("Complex")]
        [ValidateAntiForgeryToken]
        public ActionResult ComplexPose()
        {
            GetAdminAuth();

            var charts = new string[] { DominoChartType.Department, DominoChartType.PE, DominoChartType.Monthly, DominoChartType.Quarter, DominoChartType.Customer };
            var chartlist = new List<string>();
            chartlist.AddRange(charts);
            ViewBag.charttypelist = CreateSelectList(chartlist, "");

            if (string.IsNullOrEmpty(Request.Form["StartDate"])
                || string.IsNullOrEmpty(Request.Form["EndDate"])
                || (DateTime.Parse(Request.Form["StartDate"]) > DateTime.Parse(Request.Form["EndDate"])))
            {
                return View();
            }



            var charttype = Request.Form["charttypelist"].ToString();

            if (string.Compare(charttype, DominoChartType.Department) == 0)
            {
                DepartComplex();
            }
            else
            {
                DictComplex(charttype);
            }

            return View();
        }


        public ActionResult QAFACheck()
        {
            GetAdminAuth();

            var charts = new string[] { DominoChartType.Department, DominoChartType.PE, DominoChartType.Monthly, DominoChartType.Quarter};
            var chartlist = new List<string>();
            chartlist.AddRange(charts);
            ViewBag.charttypelist = CreateSelectList(chartlist, "");

            return View();
        }

        private void DepartQAFACheck()
        {
            var startdate = DateTime.Parse(Request.Form["StartDate"]);
            var enddate = DateTime.Parse(Request.Form["EndDate"]);
            var title = Request.Form["ChartTitle"];

            var qafadict = DominoRPVM.RetrieveDepartQACheckData(this,startdate, enddate);
            if (qafadict.Count == 0)
            {
                return;
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
                    int tempmax = 0;
                    ChartxAxisValues = ChartxAxisValues + "'" + dpt + "',";

                    if (qafadict[dpt].EEPROMPASS == 0)
                    {
                        PassQTY = PassQTY + "null,";
                    }
                    else
                    {
                        PassQTY = PassQTY + qafadict[dpt].EEPROMPASS.ToString() + ",";
                        tempmax = tempmax + qafadict[dpt].EEPROMPASS;
                    }

                    if (qafadict[dpt].EEPROMFAIL == 0)
                    {
                        FailQTY = FailQTY + "null,";
                    }
                    else
                    {
                        FailQTY = FailQTY + qafadict[dpt].EEPROMFAIL.ToString() + ",";
                        tempmax = tempmax + qafadict[dpt].EEPROMFAIL;
                    }

                    if ((qafadict[dpt].EEPROMPASS+ qafadict[dpt].EEPROMFAIL) == 0)
                    {
                        FailRate = FailRate + "100.0,";
                    }
                    else
                    {
                        FailRate = FailRate + (((double)qafadict[dpt].EEPROMFAIL/(double)(qafadict[dpt].EEPROMPASS + qafadict[dpt].EEPROMFAIL))*100.0).ToString("0.0") + ",";
                    }

                    if (tempmax > AmountMAX)
                        AmountMAX = tempmax;
                }
            }

            ChartxAxisValues = ChartxAxisValues.Substring(0, ChartxAxisValues.Length - 1);
            PassQTY = PassQTY.Substring(0, PassQTY.Length - 1);
            FailQTY = FailQTY.Substring(0, FailQTY.Length - 1);
            FailRate = FailRate.Substring(0, FailRate.Length - 1);

            var temptitle = "Depart EEPROM QA Fail Rate/Fail QTY " + startdate.ToString("yyyy/MM/dd") + "-" + enddate.ToString("yyyy/MM/dd");
            if (!string.IsNullOrEmpty(title.Trim()))
                temptitle = title.Trim();

            var tempscript = System.IO.File.ReadAllText(Server.MapPath("~/Scripts/DominoQAFACheck.xml"));
            ViewBag.qafachart = tempscript.Replace("#ElementID#", "qafachart")
                .Replace("#Title#", temptitle)
                .Replace("#ChartxAxisValues#", ChartxAxisValues)
                .Replace("#AmountMAX#", (AmountMAX+1).ToString())
                .Replace("#PassQTY#", PassQTY)
                .Replace("#FailQTY#", FailQTY)
                .Replace("#FailRate#", FailRate);
        }

        private void DictQAFACheck(string charttype)
        {
            var startdate = DateTime.Parse(Request.Form["StartDate"]);
            var enddate = DateTime.Parse(Request.Form["EndDate"]);
            var title = Request.Form["ChartTitle"];

            var qafadict = new Dictionary<string, QACheckData>();

            if (string.Compare(charttype, DominoChartType.PE) == 0)
            {
                qafadict = DominoRPVM.RetrievePEQACheckData(this, startdate, enddate);
            }
            else if (string.Compare(charttype, DominoChartType.Monthly) == 0)
            {
                qafadict = DominoRPVM.RetrieveMonthlyQACheckData(this, startdate, enddate);
            }
            else if (string.Compare(charttype, DominoChartType.Quarter) == 0)
            {
                qafadict = DominoRPVM.RetrieveQuartQACheckData(this, startdate, enddate);
            }


            if (qafadict.Count == 0)
            {
                return;
            }

            var ChartxAxisValues = string.Empty;
            int AmountMAX = 0;

            var PassQTY = string.Empty;
            var FailQTY = string.Empty;
            var FailRate = string.Empty;


            var pes = new List<string>();
            if (string.Compare(charttype, DominoChartType.Monthly) == 0
                || string.Compare(charttype, DominoChartType.Quarter) == 0)
            {
                pes = qafadict.Keys.ToList();
                pes.Sort(delegate (string sk1, string sk2)
                {
                    int ik1 = Convert.ToInt32(sk1.Replace("/", "").Replace("Q",""));
                    int ik2 = Convert.ToInt32(sk2.Replace("/", "").Replace("Q",""));
                    if (ik1 > ik2) return 1;
                    if (ik1 < ik2) return -1;
                    return 0;
                });
            }
            else
            {
                pes = qafadict.Keys.ToList();
                pes.Sort();
            }

            foreach (var pe in pes)
            {
                if (qafadict.ContainsKey(pe))
                {
                    int tempmax = 0;
                    ChartxAxisValues = ChartxAxisValues + "'" + pe.Split(new string[] { "@" }, StringSplitOptions.RemoveEmptyEntries)[0] + "',";

                    if (qafadict[pe].EEPROMPASS == 0)
                    {
                        PassQTY = PassQTY + "null,";
                    }
                    else
                    {
                        PassQTY = PassQTY + qafadict[pe].EEPROMPASS.ToString() + ",";
                        tempmax = tempmax + qafadict[pe].EEPROMPASS;
                    }

                    if (qafadict[pe].EEPROMFAIL == 0)
                    {
                        FailQTY = FailQTY + "null,";
                    }
                    else
                    {
                        FailQTY = FailQTY + qafadict[pe].EEPROMFAIL.ToString() + ",";
                        tempmax = tempmax + qafadict[pe].EEPROMFAIL;
                    }

                    if ((qafadict[pe].EEPROMPASS + qafadict[pe].EEPROMFAIL) == 0)
                    {
                        FailRate = FailRate + "100.0,";
                    }
                    else
                    {
                        FailRate = FailRate + (((double)qafadict[pe].EEPROMFAIL / (double)(qafadict[pe].EEPROMPASS + qafadict[pe].EEPROMFAIL)) * 100.0).ToString("0.0") + ",";
                    }

                    if (tempmax > AmountMAX)
                        AmountMAX = tempmax;
                }
            }

            ChartxAxisValues = ChartxAxisValues.Substring(0, ChartxAxisValues.Length - 1);
            PassQTY = PassQTY.Substring(0, PassQTY.Length - 1);
            FailQTY = FailQTY.Substring(0, FailQTY.Length - 1);
            FailRate = FailRate.Substring(0, FailRate.Length - 1);

            var temptitle = charttype + " EEPROM QA Fail Rate/Fail QTY " + startdate.ToString("yyyy/MM/dd") + "-" + enddate.ToString("yyyy/MM/dd");
            if (!string.IsNullOrEmpty(title.Trim()))
                temptitle = title.Trim();

            var tempscript = System.IO.File.ReadAllText(Server.MapPath("~/Scripts/DominoQAFACheck.xml"));
            ViewBag.qafachart = tempscript.Replace("#ElementID#", "qafachart")
                .Replace("#Title#", temptitle)
                .Replace("#ChartxAxisValues#", ChartxAxisValues)
                .Replace("#AmountMAX#", (AmountMAX + 1).ToString())
                .Replace("#PassQTY#", PassQTY)
                .Replace("#FailQTY#", FailQTY)
                .Replace("#FailRate#", FailRate);
        }

        [HttpPost, ActionName("QAFACheck")]
        [ValidateAntiForgeryToken]
        public ActionResult QAFACheckPose()
        {
            GetAdminAuth();

            var charts = new string[] { DominoChartType.Department, DominoChartType.PE, DominoChartType.Monthly, DominoChartType.Quarter};
            var chartlist = new List<string>();
            chartlist.AddRange(charts);
            ViewBag.charttypelist = CreateSelectList(chartlist, "");

            if (string.IsNullOrEmpty(Request.Form["StartDate"])
                || string.IsNullOrEmpty(Request.Form["EndDate"])
                || (DateTime.Parse(Request.Form["StartDate"]) > DateTime.Parse(Request.Form["EndDate"])))
            {
                return View();
            }

            var charttype = Request.Form["charttypelist"].ToString();

            if (string.Compare(charttype, DominoChartType.Department) == 0)
            {
                DepartQAFACheck();
            }
            else
            {
                DictQAFACheck(charttype);
            }

            return View();
        }

    }

}