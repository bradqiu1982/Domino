using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Domino.Models
{
    public class DOMINOCYCLETIMEVAL
    {
        public static double DInvalidVAL = -9999;
        public static int IInvalidVAL = -9999;
    }

    public class WorkLoadData
    {
        public string ECOKey { set; get; }
        public string PE { set; get; }
        public string Depart { set; get; }
        public string Customer { set; get; }
        public string Status { set; get; }

        public string ECONUM { set; get; }
        public string PN { set; get; }
        public string Complex { set; get; }
        public string RSM { set; get; }


        public DateTime InitReceiveDate { set; get; }
        public DateTime HoldStartDate { set; get; }
        public DateTime HoldEndDate { set; get; }
        public DateTime ECOSubmitDate { set; get; }
        public DateTime ECOCompleteDate { set; get; }
    }


    public class CycleTimeData
    {
        public CycleTimeData()
        {
            InitRevison = string.Empty;
            FinalRevison =string.Empty;
            TLAAvailable=string.Empty;
            OpsEntry=string.Empty;
            TestModification=string.Empty;
            ECOSubmit=string.Empty;
            ECOTRSignoff=string.Empty;
            ECOCCBSignoff=string.Empty;
            ECOCompleteDate=string.Empty;
            SampleShipDate=string.Empty;
        }

        public string ECOKey { set; get; }
        public string ECONUM { set; get; }
        public string PN { set; get; }
        public string PE { set; get; }
        public string Depart { set; get; }
        public string Customer { set; get; }
        public string Complex { set; get; }
        public string RSM { set; get; }
        //public string Quarter { set; get; }
        //public string Month { set; get; }

        public string InitRevison { set; get; }
        public string FinalRevison { set; get; }
        public string TLAAvailable { set; get; }
        public string OpsEntry { set; get; }
        public string TestModification { set; get; }
        public string ECOSubmit { set; get; }
        public string ECOTRSignoff { set; get; }
        public string ECOCCBSignoff { set; get; }
        public string ECOCompleteDate { set; get; }
        public string SampleShipDate { set; get; }

        public double MiniPIPApprovalAging { set; get; }
        public double ChangeDelayAging { set; get; }
        public double EngineeringAging { set; get; }
        public double TechReviewAging { set; get; }
        public double CCBSignoffAging { set; get; }
        public double SampleShipAging { set; get; }

    }


    public class CycleTimeDataField
    {
        public CycleTimeDataField()
        {
            MiniPIPApprovalAging = 0.0;
            ChangeDelayAging = 0.0;
            EngineeringAging = 0.0;
            TechReviewAging = 0.0;
            CCBSignoffAging = 0.0;
            SampleShipAging = 0.0;

            MiniPIPApprovalAgingTM = 0;
            ChangeDelayAgingTM = 0;
            EngineeringAgingTM = 0;
            TechReviewAgingTM = 0;
            CCBSignoffAgingTM = 0;
            SampleShipAgingTM = 0;

            TotalMiniPIPs = 0;
        }

        public double MiniPIPApprovalAging { set; get; }
        public double ChangeDelayAging { set; get; }
        public double EngineeringAging { set; get; }
        public double TechReviewAging { set; get; }
        public double CCBSignoffAging { set; get; }
        public double SampleShipAging { set; get; }

        public int TotalMiniPIPs { set; get; }

        private int MiniPIPApprovalAgingTM { set; get; }
        private int ChangeDelayAgingTM { set; get; }
        private int EngineeringAgingTM { set; get; }
        private int TechReviewAgingTM { set; get; }
        private int CCBSignoffAgingTM { set; get; }
        public int SampleShipAgingTM { set; get; }

        public double MiniPIPApprovalAgingAVG
        {
            get
            {
                if (MiniPIPApprovalAgingTM == 0)
                {
                    return 0.0;
                }
                return MiniPIPApprovalAging / (double)MiniPIPApprovalAgingTM;
            }
        }
        public double ChangeDelayAgingAVG
        {
            get
            {
                if (ChangeDelayAgingTM == 0)
                {
                    return 0.0;
                }
                return ChangeDelayAging / (double)ChangeDelayAgingTM;
            }
        }
        public double EngineeringAgingAVG
        {
            get
            {
                if (EngineeringAgingTM == 0)
                {
                    return 0.0;
                }
                return EngineeringAging / (double)EngineeringAgingTM;
            }
        }
        public double TechReviewAgingAVG
        {
            get
            {
                if (TechReviewAgingTM == 0)
                {
                    return 0.0;
                }
                return TechReviewAging / (double)TechReviewAgingTM;
            }
        }
        public double CCBSignoffAgingAVG
        {
            get
            {
                if (CCBSignoffAgingTM == 0)
                {
                    return 0.0;
                }
                return CCBSignoffAging / (double)CCBSignoffAgingTM;
            }
        }

        public double SampleShipAgingAVG
        {
            get
            {
                if (SampleShipAgingTM == 0)
                {
                    return 0.0;
                }
                return SampleShipAging / (double)SampleShipAgingTM;
            }
        }

        public void appendcycledata(CycleTimeData cycledata)
        {
            TotalMiniPIPs = TotalMiniPIPs + 1;

            if (cycledata.MiniPIPApprovalAging != DOMINOCYCLETIMEVAL.DInvalidVAL)
            {
                MiniPIPApprovalAging = cycledata.MiniPIPApprovalAging + MiniPIPApprovalAging;
                MiniPIPApprovalAgingTM = MiniPIPApprovalAgingTM + 1;
            }

            if (cycledata.ChangeDelayAging != DOMINOCYCLETIMEVAL.DInvalidVAL)
            {
                ChangeDelayAging = cycledata.ChangeDelayAging + ChangeDelayAging;
                ChangeDelayAgingTM = ChangeDelayAgingTM + 1;
            }

            if (cycledata.EngineeringAging != DOMINOCYCLETIMEVAL.DInvalidVAL)
            {
                EngineeringAging = cycledata.EngineeringAging + EngineeringAging;
                EngineeringAgingTM = EngineeringAgingTM + 1;
            }

            if (cycledata.TechReviewAging != DOMINOCYCLETIMEVAL.DInvalidVAL)
            {
                TechReviewAging = cycledata.TechReviewAging + TechReviewAging;
                TechReviewAgingTM = TechReviewAgingTM + 1;
            }

            if (cycledata.CCBSignoffAging != DOMINOCYCLETIMEVAL.DInvalidVAL)
            {
                CCBSignoffAging = cycledata.CCBSignoffAging + CCBSignoffAging;
                CCBSignoffAgingTM = CCBSignoffAgingTM + 1;
            }

            if (cycledata.SampleShipAging != DOMINOCYCLETIMEVAL.DInvalidVAL)
            {
                SampleShipAging = cycledata.SampleShipAging + SampleShipAging;
                SampleShipAgingTM = SampleShipAgingTM + 1;
            }
        }

    }


    public class WorkLoadField
    {
        public WorkLoadField()
        {
            Hold = 0;
            Operation = 0;
            SignOff = 0;
            Complete = 0;
        }

        public int Hold { set; get; }
        public int Operation { set; get; }
        public int SignOff { set; get; }
        public int Complete { set; get; }

        public void SetStatus(string stat)
        {
            if (string.Compare(stat, DominoCardType.ECOPending) == 0)
            {
                Operation = Operation + 1;
            }

            if (string.Compare(stat, DominoCardType.Hold) == 0)
            {
                Hold = Hold + 1;
            }

            if (string.Compare(stat, DominoCardType.ECOSignoff1) == 0)
            {
                SignOff = SignOff + 1;
            }

            if (string.Compare(stat, DominoCardType.ECOComplete) == 0)
            {
                Complete = Complete + 1;
            }
        }
    }

    public class ComplexData
    {
        public ComplexData()
        {
            E = 0;
            M = 0;
            S = 0;
        }

        public string ECOKey { set; get; }
        public string PE { set; get; }
        public string Depart { set; get; }
        public string Customer { set; get; }

        public DateTime InitReceiveDate { set; get; }
        public DateTime ECOCompleteDate { set; get; }


        public void SetComplexType(string c)
        {
            if (string.IsNullOrEmpty(c))
                return;

            if (string.Compare(c.Substring(0, 1).ToUpper(), "E") == 0)
            {
                E = 1;
            }

            if (string.Compare(c.Substring(0, 1).ToUpper(), "M") == 0)
            {
                M = 1;
            }

            if (string.Compare(c.Substring(0, 1).ToUpper(), "S") == 0)
            {
                S = 1;
            }
        }

        public int E { set; get; }
        public int M { set; get; }
        public int S { set; get; }

        public void AppendComplexData(ComplexData cd)
        {
            E = E + cd.E;
            M = M + cd.M;
            S = S + cd.S;
        }
    }

    public class RevenueData {
        public string ECONUM { set; get; }
        public string PN { set; get; }
        public string PE { set; get; }
        public string Depart { set; get; }
        public string Customer { set; get; }
        public string Status { set; get; }
        public double Revenue { set; get; }
        public string ECOType { set; get; }

        public DateTime InitReceiveDate { set; get; }
        public DateTime ECOCompleteDate { set; get; }

        public void AppendRevenueData(RevenueData rvdata)
        {
            Revenue += rvdata.Revenue;
        }

        public RevenueData()
        {
            ECONUM = "";
            PN = "";
            PE = "";
            Depart = "";
            Customer = "";
            Status = "";
            Revenue = 0;
            ECOType = "";
        }
    }


    public class QACheckData
    {
        public QACheckData()
        {
            EEPROMPASS = 0;
            EEPROMFAIL = 0;
            FLIPASS = 0;
            FLIFAIL = 0;
    }

        public DateTime QADate { set; get; }
        public string PE { set; get; }
        public string Depart { set; get; }

        public int EEPROMPASS { set; get; }
        public int EEPROMFAIL { set; get; }
        public int FLIPASS { set; get;}
        public int FLIFAIL { set; get; }

        public void AppendQAData(QACheckData qa)
        {
            EEPROMPASS = EEPROMPASS+qa.EEPROMPASS;
            EEPROMFAIL = EEPROMFAIL+qa.EEPROMFAIL;
            FLIPASS = FLIPASS+qa.FLIPASS;
            FLIFAIL = FLIFAIL+qa.FLIFAIL;
        }
    }


    public class DominoRPVM
    {
        private static string WorkLoadStatus(DateTime startdate, DateTime enddate, WorkLoadData workloaddata)
        {
            if (workloaddata.InitReceiveDate >= startdate && workloaddata.InitReceiveDate <= enddate)
            {
                if (workloaddata.HoldStartDate  > workloaddata.InitReceiveDate 
                    && workloaddata.HoldEndDate < workloaddata.ECOSubmitDate
                    && workloaddata.HoldEndDate > workloaddata.HoldStartDate)
                {
                    if (enddate >= workloaddata.InitReceiveDate && enddate < workloaddata.HoldStartDate)
                    {
                        return DominoCardType.ECOPending;
                    }

                    if (enddate >= workloaddata.HoldStartDate && enddate < workloaddata.HoldEndDate)
                    {
                        return DominoCardType.Hold;
                    }

                    if (enddate >= workloaddata.HoldEndDate && enddate < workloaddata.ECOSubmitDate)
                    {
                        return DominoCardType.ECOPending;
                    }
                }
                else
                {
                    if (enddate >= workloaddata.InitReceiveDate && enddate < workloaddata.ECOSubmitDate)
                    {
                        return DominoCardType.ECOPending;
                    }
                }


                if (enddate >= workloaddata.ECOSubmitDate && enddate < workloaddata.ECOCompleteDate)
                {
                    return DominoCardType.ECOSignoff1;
                }

                if (enddate >= workloaddata.ECOCompleteDate)
                {
                    return DominoCardType.ECOComplete;
                }

                return DominoCardType.None;

            }
            else
            {
                if (startdate > workloaddata.InitReceiveDate && startdate <= workloaddata.ECOCompleteDate)
                {

                    if (workloaddata.HoldStartDate > workloaddata.InitReceiveDate
                    && workloaddata.HoldEndDate < workloaddata.ECOSubmitDate
                    && workloaddata.HoldEndDate > workloaddata.HoldStartDate)
                    {
                        if (enddate >= workloaddata.InitReceiveDate && enddate < workloaddata.HoldStartDate)
                        {
                            return DominoCardType.ECOPending;
                        }

                        if (enddate >= workloaddata.HoldStartDate && enddate < workloaddata.HoldEndDate)
                        {
                            return DominoCardType.Hold;
                        }

                        if (enddate >= workloaddata.HoldEndDate && enddate < workloaddata.ECOSubmitDate)
                        {
                            return DominoCardType.ECOPending;
                        }
                    }
                    else
                    {
                        if (enddate >= workloaddata.InitReceiveDate && enddate < workloaddata.ECOSubmitDate)
                        {
                            return DominoCardType.ECOPending;
                        }
                    }

                    if (enddate >= workloaddata.ECOSubmitDate && enddate < workloaddata.ECOCompleteDate)
                    {
                        return DominoCardType.ECOSignoff1;
                    }

                    if (enddate >= workloaddata.ECOCompleteDate)
                    {
                        return DominoCardType.ECOComplete;
                    }
                }

                return DominoCardType.None;
            }
        }

        private static List<WorkLoadData> CollectAllWorkLoadTimePoint()
        {
            var allworkload = new List<WorkLoadData>();

            var alleco = ECOBaseInfo.RetrieveAllNotDeleteECOBaseInfo();
            foreach (var eco in alleco)
            {
                try
                {
                    if (string.Compare(DateTime.Parse(eco.InitRevison).ToString("yyyy-MM-dd"), "1982-05-06") == 0)
                    {

                    }
                    else
                    {
                        var tempworkload = new WorkLoadData();
                        tempworkload.ECOKey = eco.ECOKey;
                        tempworkload.Customer = eco.Customer;

                        tempworkload.ECONUM = eco.ECONum;
                        tempworkload.PN = eco.PNDesc;
                        tempworkload.Complex = eco.Complex;
                        tempworkload.RSM = eco.RSM;


                        if (!eco.PE.Contains("@"))
                        {
                            tempworkload.PE = (eco.PE.Trim().Replace(" ", ".") + "@finisar.com").ToUpper();
                        }
                        else
                        {
                            tempworkload.PE = eco.PE;
                        }
                    
                        tempworkload.InitReceiveDate = DateTime.Parse(DateTime.Parse(eco.InitRevison).ToString("yyyy-MM-dd") + " 07:30:00");

                        if (string.IsNullOrEmpty(eco.ECOSubmit) || string.Compare(DateTime.Parse(eco.ECOSubmit).ToString("yyyy-MM-dd"), "1982-05-06") == 0)
                        {
                            tempworkload.ECOSubmitDate = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd") + " 07:30:00").AddMonths(1);
                        }
                        else
                        {
                            tempworkload.ECOSubmitDate = DateTime.Parse(DateTime.Parse(eco.ECOSubmit).ToString("yyyy-MM-dd") + " 07:30:00");
                        }

                        var completecard = DominoVM.RetrieveSpecialCard(eco, DominoCardType.ECOComplete);
                        if (completecard.Count > 0)
                        {
                            var cardinfo = DominoVM.RetrieveECOCompleteInfo(completecard[0].CardKey);
                            if (string.IsNullOrEmpty(cardinfo.ECOCompleteDate))
                            {
                                tempworkload.ECOCompleteDate = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd") + " 07:30:00").AddMonths(2);
                            }
                            else
                            {
                                tempworkload.ECOCompleteDate = DateTime.Parse(DateTime.Parse(cardinfo.ECOCompleteDate).ToString("yyyy-MM-dd") + " 07:30:00");
                            }
                        }
                        else
                        {
                            tempworkload.ECOCompleteDate = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd") + " 07:30:00").AddMonths(2);
                        }

                        tempworkload.HoldStartDate = DateTime.Parse(eco.ECOHoldStartDate);
                        tempworkload.HoldEndDate = DateTime.Parse(eco.ECOHoldEndDate);

                        //tempworkload.HoldStartDate = tempworkload.InitReceiveDate.AddDays(3);
                        //tempworkload.HoldEndDate = tempworkload.ECOSubmitDate.AddDays(-3);

                        if (string.Compare(tempworkload.HoldStartDate.ToString("yyyy-MM-dd"), "1982-05-06") != 0)
                        {
                            if (string.Compare(tempworkload.HoldEndDate.ToString("yyyy-MM-dd"), "1982-05-06") == 0)
                            {
                                tempworkload.HoldEndDate = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd") + " 07:30:00").AddMonths(1);
                                tempworkload.ECOSubmitDate = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd") + " 07:30:00").AddMonths(2);
                                tempworkload.ECOCompleteDate = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd") + " 07:30:00").AddMonths(3);
                            }
                            else
                            {
                                if (tempworkload.HoldStartDate > tempworkload.HoldEndDate)
                                {
                                    tempworkload.HoldEndDate = tempworkload.HoldStartDate;
                                }
                            }
                        }
                        else
                        {
                            tempworkload.HoldEndDate = DateTime.Parse("1982-05-06 07:30:00");
                        }

                        allworkload.Add(tempworkload);
                    }//end else
                }
                catch (Exception ex)
                { }
            }//end foreach

            return allworkload;
        }

        private static string WkloadConvert(string stat)
        {
            if (string.Compare(stat, DominoCardType.ECOPending) == 0)
            {
                return "Operation";
            }

            if (string.Compare(stat, DominoCardType.Hold) == 0)
            {
                return "Hold";
            }

            if (string.Compare(stat, DominoCardType.ECOSignoff1) == 0)
            {
                return "SignOff";
            }

            if (string.Compare(stat, DominoCardType.ECOComplete) == 0)
            {
                return "Complete";
            }

            return string.Empty;
        }

        private static List<WorkLoadData> RetrieveWorkLoadData(DateTime startdated, DateTime enddated,string filename)
        {
            var udlist = DominoUserViewModels.RetrieveAllUserDepart();
            var uddict = new Dictionary<string, string>();
            foreach (var ud in udlist)
            {
                uddict.Add(ud.UserName, ud.Depart);
            }

            var startdate = DateTime.Parse(startdated.ToString("yyyy-MM-dd") + " 07:30:00");
            var enddate = DateTime.Parse(enddated.ToString("yyyy-MM-dd") + " 07:30:00");

            var allworkload = CollectAllWorkLoadTimePoint();

            logreportinfo(filename, "PN,ECONUM,Customer,type,RSM,PE,Depart,Status\r\n");

            var filterworkload = new List<WorkLoadData>();
            foreach (var wkload in allworkload)
            {
                var status = WorkLoadStatus(startdate, enddate, wkload);
                if (string.Compare(status, DominoCardType.None) != 0)
                {
                    var depart = "";

                    if (uddict.ContainsKey(wkload.PE))
                    {
                        depart = uddict[wkload.PE];
                    }

                    logreportinfo(filename, wkload.PN.Replace(",","")+","+wkload.ECONUM+","+wkload.Customer.Replace(",", "")
                        + "," + wkload.Complex + "," + wkload.RSM + "," 
                        + wkload.PE.Split(new string[] { "@" }, StringSplitOptions.RemoveEmptyEntries)[0] + ","+ depart + ","+ WkloadConvert(status) + "\r\n");

                    wkload.Status = status;
                    filterworkload.Add(wkload);
                }
            }//end foreach

            return filterworkload;
        }

        public static Dictionary<string, WorkLoadField> RetrieveDepartWorkLoadData(DateTime startdate, DateTime enddate,string filename)
        {
            var udlist = DominoUserViewModels.RetrieveAllUserDepart();
            var uddict = new Dictionary<string, string>();
            foreach (var ud in udlist)
            {
                uddict.Add(ud.UserName, ud.Depart);
            }

            var wkloads = RetrieveWorkLoadData(startdate, enddate,filename);

            var departworkload = new List<WorkLoadData>();
            foreach (var wkl in wkloads)
            {
                if (uddict.ContainsKey(wkl.PE))
                {
                    wkl.Depart = uddict[wkl.PE];
                    departworkload.Add(wkl);
                }
            }

            var ret = new Dictionary<string, WorkLoadField>();

            var departs = DominoUserViewModels.RetrieveAllDepartment();
            foreach (var dpt in departs)
            {
                foreach (var wkl in departworkload)
                {
                    if (string.Compare(dpt, wkl.Depart) == 0)
                    {
                        if (ret.ContainsKey(dpt))
                        {
                            ret[dpt].SetStatus(wkl.Status);
                        }
                        else
                        {
                            ret.Add(dpt, new WorkLoadField());
                            ret[dpt].SetStatus(wkl.Status);
                        }
                    }//end if
                }//foreach
            }//foreach

            return ret;
        }

        public static Dictionary<string, WorkLoadField> RetrievePEWorkLoadData(DateTime startdate, DateTime enddate,string filename)
        {
            var pewkloads = RetrieveWorkLoadData(startdate, enddate,filename);

            var pedict = new Dictionary<string, bool>();
            foreach (var w in pewkloads)
            {
                if (!pedict.ContainsKey(w.PE))
                {
                    pedict.Add(w.PE, true);
                }
            }

            var ret = new Dictionary<string, WorkLoadField>();

            var pes = pedict.Keys.ToList();
            foreach (var pe in pes)
            {
                foreach (var wkl in pewkloads)
                {
                    if (string.Compare(pe, wkl.PE) == 0)
                    {
                        if (ret.ContainsKey(pe))
                        {
                            ret[pe].SetStatus(wkl.Status);
                        }
                        else
                        {
                            ret.Add(pe, new WorkLoadField());
                            ret[pe].SetStatus(wkl.Status);
                        }
                    }//end if
                }//foreach
            }//foreach

            return ret;
        }

        public static Dictionary<string, WorkLoadField> RetrieveCustomerWorkLoadData(DateTime startdate, DateTime enddate,string filename)
        {
            var pewkloads = RetrieveWorkLoadData(startdate, enddate,filename);

            var custdict = new Dictionary<string, bool>();
            foreach (var w in pewkloads)
            {
                if (!custdict.ContainsKey(w.Customer))
                {
                    custdict.Add(w.Customer, true);
                }
            }

            var ret = new Dictionary<string, WorkLoadField>();

            var custs = custdict.Keys.ToList();
            foreach (var cu in custs)
            {
                foreach (var wkl in pewkloads)
                {
                    if (string.Compare(cu, wkl.Customer) == 0)
                    {
                        if (ret.ContainsKey(cu))
                        {
                            ret[cu].SetStatus(wkl.Status);
                        }
                        else
                        {
                            ret.Add(cu, new WorkLoadField());
                            ret[cu].SetStatus(wkl.Status);
                        }
                    }//end if
                }//foreach
            }//foreach

            return ret;
        }

        private static List<DateTime> RetrieveDateSpanByMonth(string startdate, string enddate)
        {
            var ret = new List<DateTime>();
            var sdate = DateTime.Parse(DateTime.Parse(startdate).ToString("yyyy-MM-dd") + " 07:30:00");
            ret.Add(sdate);

            var temptimepoint = sdate;
            var edate = DateTime.Parse(enddate);

            temptimepoint = temptimepoint.AddMonths(1);
            temptimepoint = DateTime.Parse(temptimepoint.ToString("yyyy-MM") + "-01 07:30:00");


            if (temptimepoint > edate)
            {
                ret.Add(DateTime.Parse(DateTime.Parse(enddate).ToString("yyyy-MM-dd") + " 07:30:00"));
                return ret;
            }
            else
            {
                ret.Add(temptimepoint);
            }

            while (temptimepoint < edate)
            {
                temptimepoint = temptimepoint.AddMonths(1);
                if (temptimepoint > edate)
                {
                    ret.Add(DateTime.Parse(DateTime.Parse(enddate).ToString("yyyy-MM-dd") + " 07:30:00"));
                    return ret;
                }
                else
                {
                    ret.Add(DateTime.Parse(temptimepoint.ToString("yyyy-MM-dd") + " 07:30:00"));
                }
            }
            return ret;
        }

        private static string QuartStr(DateTime date)
        {
            int month = date.Month;

            if (month >= 5 && month <= 7)
            {
                return date.AddYears(1).ToString("yy") + "/Q1";
            }
            else if (month >= 8 && month <= 10)
            {
                return date.AddYears(1).ToString("yy") + "/Q2";
            }
            else if (month >= 2 && month <= 4)
            {
                return date.ToString("yy") + "/Q4";
            }
            else
            {
                if (month >= 11 && month <= 12)
                {
                    return date.AddYears(1).ToString("yy") + "/Q3";
                }
                else
                {
                    return date.ToString("yy") + "/Q3";
                }
            }
        }

        private static List<DateTime> RetrieveDateSpanByQuart(string startdate, string enddate)
        {
            var ret = new List<DateTime>();
            var sdate = DateTime.Parse(DateTime.Parse(startdate).ToString("yyyy-MM-dd") + " 07:30:00");
            ret.Add(sdate);
            var edate = DateTime.Parse(DateTime.Parse(enddate).ToString("yyyy-MM-dd") + " 07:30:00");

            var quartendday = sdate;

            if (sdate.Month >= 5 && sdate.Month <= 7)
            {
                quartendday = DateTime.Parse(sdate.ToString("yyyy") + "-08-1 07:30:00");
            }
            else if (sdate.Month >= 8 && sdate.Month <= 10)
            {
                quartendday = DateTime.Parse(sdate.ToString("yyyy") + "-11-01 07:30:00");
            }
            else if (sdate.Month >= 11 && sdate.Month <= 12)
            {
                quartendday = DateTime.Parse(sdate.AddYears(1).ToString("yyyy") + "-02-01 07:30:00");
            }
            else if (sdate.Month == 1)
            {
                quartendday = DateTime.Parse(sdate.ToString("yyyy") + "-02-01 07:30:00");
            }
            else if (sdate.Month >= 2 && sdate.Month <= 4)
            {
                quartendday = DateTime.Parse(sdate.ToString("yyyy") + "-05-01 07:30:00");
            }

            if (quartendday >= edate)
            {
                ret.Add(edate);
                return ret;
            }
            else
            {
                ret.Add(quartendday);
            }

            while (quartendday < edate)
            {
                quartendday = quartendday.AddMonths(3);
                if (quartendday > edate)
                {
                    ret.Add(edate);
                    return ret;
                }
                else
                {
                    ret.Add(quartendday);
                }
            }

            return ret;

        }


        public static Dictionary<string, WorkLoadField> RetrieveMonthlyWorkLoadData(DateTime startdate, DateTime enddate,string filename)
        {
            var ret = new Dictionary<string, WorkLoadField>();

            var ldate = RetrieveDateSpanByMonth(startdate.ToString(), enddate.ToString());
            for (int idx = 0; idx < ldate.Count - 1; idx++)
            {
                var pewkloads = RetrieveWorkLoadData(ldate[idx], ldate[idx + 1],filename);
                foreach (var wkl in pewkloads)
                {
                    if (ret.ContainsKey(ldate[idx].ToString("yy/MM")))
                    {
                        ret[ldate[idx].ToString("yy/MM")].SetStatus(wkl.Status);
                    }
                    else
                    {
                        ret.Add(ldate[idx].ToString("yy/MM"), new WorkLoadField());
                        ret[ldate[idx].ToString("yy/MM")].SetStatus(wkl.Status);
                    }
                }//foreach
            }

            return ret;
        }

        public static Dictionary<string, WorkLoadField> RetrieveQuartWorkLoadData(DateTime startdate, DateTime enddate,string filename)
        {
            var ret = new Dictionary<string, WorkLoadField>();

            var ldate = RetrieveDateSpanByQuart(startdate.ToString(), enddate.ToString());
            for (int idx = 0; idx < ldate.Count - 1; idx++)
            {
                var pewkloads = RetrieveWorkLoadData(ldate[idx], ldate[idx + 1],filename);
                foreach (var wkl in pewkloads)
                {
                    var key = QuartStr(ldate[idx]);
                    if (ret.ContainsKey(key))
                    {
                        ret[key].SetStatus(wkl.Status);
                    }
                    else
                    {
                        ret.Add(key, new WorkLoadField());
                        ret[key].SetStatus(wkl.Status);
                    }
                }//foreach
            }

            return ret;
        }

        private static string ConvertDate(string date)
        {
            try
            {
                if (string.IsNullOrEmpty(date))
                {
                    return string.Empty;
                }
                if (string.Compare(DateTime.Parse(date).ToString("yyyy-MM-dd"), "1982-05-06") == 0)
                {
                    return string.Empty;
                }
                else
                {
                    return DateTime.Parse(date).ToString("yyyy-MM-dd") + " 07:30:00";
                }
            }
            catch (Exception ex) { return string.Empty; }
        }


        private static List<CycleTimeData> CollectAllCycleTimePoint(DateTime startdate, DateTime enddate)
        {
            var allcycletime = new List<CycleTimeData>();

            var ecodone = ECOBaseInfo.RetrieveECOCompletedBaseInfo();
            foreach (var eco in ecodone)
            {
                var tempcycle = new CycleTimeData();
                try
                {
                    var ecosubmitdate = DateTime.Parse(eco.ECOSubmit);
                    if (ecosubmitdate < startdate || ecosubmitdate > enddate)
                    {
                        continue;
                    }

                    //var completecard = DominoVM.RetrieveSpecialCard(eco, DominoCardType.ECOComplete);
                    //if (completecard.Count > 0)
                    //{
                    //    var cardinfo = DominoVM.RetrieveECOCompleteInfo(completecard[0].CardKey);
                    //    tempcycle.ECOCompleteDate = ConvertDate(cardinfo.ECOCompleteDate);

                    //    if (!string.IsNullOrEmpty(tempcycle.ECOCompleteDate))
                    //    {
                    //        var completedate = DateTime.Parse(tempcycle.ECOCompleteDate);
                    //        if (completedate >= startdate && completedate <= enddate)
                    //        {
                    //            //tempcycle.Month = completedate.Month.ToString();
                    //            //tempcycle.Quarter = QuartStr(completedate.Month, DateTime.Parse(tempcycle.ECOCompleteDate));
                    //        }
                    //        else
                    //        {
                    //            continue;
                    //        }

                    //    }
                    //    else
                    //    {
                    //        continue;
                    //    }
                    //}//end if


                    tempcycle.ECOKey = eco.ECOKey;
                    tempcycle.ECONUM = eco.ECONum;
                    tempcycle.PN = eco.PNDesc;
                    tempcycle.Customer = eco.Customer;
                    tempcycle.Complex = eco.Complex;
                    tempcycle.RSM = eco.RSM;
                    
                    if (!eco.PE.Contains("@"))
                    {
                        tempcycle.PE = (eco.PE.Trim().Replace(" ", ".") + "@finisar.com").ToUpper();
                    }
                    else
                    {
                        tempcycle.PE = eco.PE;
                    }

                    tempcycle.InitRevison = ConvertDate(eco.InitRevison);
                    tempcycle.FinalRevison = ConvertDate(eco.FinalRevison);
                    tempcycle.TLAAvailable = ConvertDate(eco.TLAAvailable);
                    tempcycle.OpsEntry = ConvertDate(eco.OpsEntry);
                    tempcycle.TestModification = ConvertDate(eco.TestModification);
                    tempcycle.ECOSubmit = ConvertDate(eco.ECOSubmit);
                    tempcycle.ECOTRSignoff = ConvertDate(eco.ECOReviewSignoff);
                    tempcycle.ECOCCBSignoff = ConvertDate(eco.ECOCCBSignoff);

                    var shipcare = DominoVM.RetrieveSpecialCard(eco, DominoCardType.SampleShipment);
                    if (shipcare.Count > 0)
                    {
                        var ShipTable = DominoVM.RetrieveShipInfo(shipcare[0].CardKey);
                        if (ShipTable.Count > 0)
                        {
                            var datelist = new List<DateTime>();
                            foreach (var shipinfo in ShipTable)
                            {
                                if (!string.IsNullOrEmpty(shipinfo.ShipDate))
                                {
                                    datelist.Add(DateTime.Parse(shipinfo.ShipDate));
                                }
                            }

                            if (datelist.Count > 0)
                            {
                                datelist.Sort();
                                tempcycle.SampleShipDate = ConvertDate(datelist[0].ToString());
                            }
                        }
                    }//end if

                    allcycletime.Add(tempcycle);
                }
                catch (Exception ex) { }
                
            }//end foreach

            return allcycletime;
        }

        private static void logreportinfo(string filename,string info)
        {
            try
            {
                if (System.IO.File.Exists(filename))
                {
                    var content = System.IO.File.ReadAllText(filename);
                    content = content + info;
                    System.IO.File.WriteAllText(filename, content);
                }
                else
                {
                    System.IO.File.WriteAllText(filename, info);
                }
            }
            catch (Exception ex) { }
        }

        private static List<CycleTimeData> CalculateCycleTimePoints(DateTime startdate, DateTime enddate,string filename)
        {
            var cyclelist = CollectAllCycleTimePoint(startdate, enddate);
            var calculatedlist = new List<CycleTimeData>();


            var udlist = DominoUserViewModels.RetrieveAllUserDepart();
            var uddict = new Dictionary<string, string>();
            foreach (var ud in udlist)
            {
                uddict.Add(ud.UserName, ud.Depart);
            }

            logreportinfo(filename, "PN,ECONUM,Customer,type,RSM,PE,Depart,InitRevison,FinalRevison,OpsLogEntry,TLAAvailable,TestModification,ECOSubmit,ECOTRSignoff,ECOCCBSignoff,SampleShipDate");
            logreportinfo(filename, ",MiniPIPApprovalAging,ChangeDelayAging,EngineeringAging,TechReviewAging,CCBSignoffAging,SampleShipAging\r\n");

            foreach (var cycle in cyclelist)
            {
                var depart = "";
                if (uddict.ContainsKey(cycle.PE))
                {
                    depart = uddict[cycle.PE];
                }

                logreportinfo(filename, cycle.PN+","+ cycle.ECONUM + "," + cycle.Customer + "," + cycle.Complex + "," + cycle.RSM
                    + "," +  cycle.PE.Split(new string[] { "@"},StringSplitOptions.RemoveEmptyEntries)[0]+ "," +  depart
                    + "," + cycle.InitRevison + "," + cycle.FinalRevison + "," + cycle.OpsEntry
                    + "," + cycle.TLAAvailable + "," + cycle.TestModification + "," + cycle.ECOSubmit
                    + "," + cycle.ECOTRSignoff + "," + cycle.ECOCCBSignoff + "," + cycle.SampleShipDate);

                try
                {

                    if (string.IsNullOrEmpty(cycle.OpsEntry))
                    {
                        cycle.MiniPIPApprovalAging = DOMINOCYCLETIMEVAL.DInvalidVAL;
                        logreportinfo(filename, ",NA");
                    }
                    else
                    {
                        if (DateTime.Parse(cycle.OpsEntry) >= DateTime.Parse(cycle.FinalRevison))
                        {
                            cycle.MiniPIPApprovalAging = CountWorkDays(DateTime.Parse(cycle.FinalRevison), DateTime.Parse(cycle.OpsEntry)) - 1;
                            logreportinfo(filename, ","+ cycle.MiniPIPApprovalAging.ToString());
                        }
                        else
                        {
                            cycle.MiniPIPApprovalAging = CountWorkDays(DateTime.Parse(cycle.InitRevison), DateTime.Parse(cycle.OpsEntry)) - 1;
                            logreportinfo(filename, "," + cycle.MiniPIPApprovalAging.ToString());
                        }
                    }
                    

                    if (string.IsNullOrEmpty(cycle.FinalRevison))
                    {
                        cycle.ChangeDelayAging = DOMINOCYCLETIMEVAL.DInvalidVAL;
                        logreportinfo(filename, ",NA");
                    }
                    else
                    {
                        cycle.ChangeDelayAging = CountWorkDays(DateTime.Parse(cycle.InitRevison), DateTime.Parse(cycle.FinalRevison)) - 1;
                        logreportinfo(filename, "," + cycle.ChangeDelayAging.ToString());
                    }


                    if (string.IsNullOrEmpty(cycle.ECOSubmit))
                    {
                        cycle.EngineeringAging = DOMINOCYCLETIMEVAL.DInvalidVAL;
                        logreportinfo(filename, ",NA");
                    }
                    else
                    {
                        var datelist = new List<string>();
                        datelist.Add(cycle.InitRevison);
                        datelist.Add(cycle.FinalRevison);
                        datelist.Add(cycle.TLAAvailable);
                        datelist.Add(cycle.OpsEntry);
                        datelist.Add(cycle.TestModification);

                        var maxdate = MaxDate(datelist);
                        cycle.EngineeringAging = CountWorkDays(maxdate, DateTime.Parse(cycle.ECOSubmit)) - 1;
                        logreportinfo(filename, "," + cycle.EngineeringAging.ToString());
                    }

                    if (string.IsNullOrEmpty(cycle.ECOTRSignoff))
                    {
                        cycle.TechReviewAging = DOMINOCYCLETIMEVAL.DInvalidVAL;
                        logreportinfo(filename, ",NA");
                    }
                    else
                    {
                        cycle.TechReviewAging = CountWorkDays(DateTime.Parse(cycle.ECOSubmit), DateTime.Parse(cycle.ECOTRSignoff)) - 1;
                        logreportinfo(filename, "," + cycle.TechReviewAging.ToString());
                    }

                    if (string.IsNullOrEmpty(cycle.ECOCCBSignoff))
                    {
                        cycle.CCBSignoffAging = DOMINOCYCLETIMEVAL.DInvalidVAL;
                        logreportinfo(filename, ",NA");
                    }
                    else
                    {
                        cycle.CCBSignoffAging = CountWorkDays(DateTime.Parse(cycle.ECOTRSignoff), DateTime.Parse(cycle.ECOCCBSignoff)) - 1;
                        logreportinfo(filename, "," + cycle.CCBSignoffAging.ToString());
                    }

                    if (string.IsNullOrEmpty(cycle.SampleShipDate))
                    {
                        cycle.SampleShipAging = DOMINOCYCLETIMEVAL.DInvalidVAL;
                        logreportinfo(filename, ",NA\r\n");
                    }
                    else
                    {
                        cycle.SampleShipAging = CountWorkDays(DateTime.Parse(cycle.InitRevison), DateTime.Parse(cycle.SampleShipDate)) - 1;
                        logreportinfo(filename, "," + cycle.SampleShipAging.ToString()+"\r\n");
                    }

                    calculatedlist.Add(cycle);
                }
                catch (Exception ex) { }
            }
            return calculatedlist;
        }


        public static Dictionary<string, CycleTimeDataField> RetrieveDepartCycleTimeData(DateTime startdate, DateTime enddate,string filename)
        {
            var udlist = DominoUserViewModels.RetrieveAllUserDepart();
            var uddict = new Dictionary<string, string>();
            foreach (var ud in udlist)
            {
                uddict.Add(ud.UserName, ud.Depart);
            }

            var cycletimes = CalculateCycleTimePoints(startdate, enddate, filename);

            var departcycle = new List<CycleTimeData>();
            foreach (var wkl in cycletimes)
            {
                if (uddict.ContainsKey(wkl.PE))
                {
                    wkl.Depart = uddict[wkl.PE];
                    departcycle.Add(wkl);
                }
            }

            var ret = new Dictionary<string, CycleTimeDataField>();

            var departs = DominoUserViewModels.RetrieveAllDepartment();
            foreach (var dpt in departs)
            {
                foreach (var wkl in departcycle)
                {
                    if (string.Compare(dpt, wkl.Depart) == 0)
                    {
                        if (ret.ContainsKey(dpt))
                        {
                            ret[dpt].appendcycledata(wkl);
                        }
                        else
                        {
                            ret.Add(dpt, new CycleTimeDataField());
                            ret[dpt].appendcycledata(wkl);
                        }
                    }//end if
                }//foreach
            }//foreach

            return ret;
        }

        public static Dictionary<string, CycleTimeDataField> RetrievePECycleTimeData(DateTime startdate, DateTime enddate,string filename)
        {
            var cycletimes = CalculateCycleTimePoints(startdate, enddate,filename);

            var pedict = new Dictionary<string, bool>();
            foreach (var w in cycletimes)
            {
                if (!pedict.ContainsKey(w.PE))
                {
                    pedict.Add(w.PE, true);
                }
            }

            var ret = new Dictionary<string, CycleTimeDataField>();

            var pes = pedict.Keys.ToList();
            foreach (var pe in pes)
            {
                foreach (var wkl in cycletimes)
                {
                    if (string.Compare(pe, wkl.PE) == 0)
                    {
                        if (ret.ContainsKey(pe))
                        {
                            ret[pe].appendcycledata(wkl);
                        }
                        else
                        {
                            ret.Add(pe, new CycleTimeDataField());
                            ret[pe].appendcycledata(wkl);
                        }
                    }//end if
                }//foreach
            }//foreach

            return ret;
        }


        public static Dictionary<string, CycleTimeDataField> RetrieveCustomerCycleTimeData(DateTime startdate, DateTime enddate,string filename)
        {
            var cycletimes = CalculateCycleTimePoints(startdate, enddate, filename);

            var custdict = new Dictionary<string, bool>();
            foreach (var w in cycletimes)
            {
                if (!custdict.ContainsKey(w.Customer))
                {
                    custdict.Add(w.Customer, true);
                }
            }

            var ret = new Dictionary<string, CycleTimeDataField>();

            var custs = custdict.Keys.ToList();
            foreach (var cu in custs)
            {
                foreach (var wkl in cycletimes)
                {
                    if (string.Compare(cu, wkl.Customer) == 0)
                    {
                        if (ret.ContainsKey(cu))
                        {
                            ret[cu].appendcycledata(wkl);
                        }
                        else
                        {
                            ret.Add(cu, new CycleTimeDataField());
                            ret[cu].appendcycledata(wkl);
                        }
                    }//end if
                }//foreach
            }//foreach

            return ret;
        }


        public static Dictionary<string, CycleTimeDataField> RetrieveMonthlyCycleTimeData(DateTime startdate, DateTime enddate,string filename)
        {
            var ret = new Dictionary<string, CycleTimeDataField>();
            var ldate = RetrieveDateSpanByMonth(startdate.ToString(), enddate.ToString());
            for (int idx = 0; idx < ldate.Count - 1; idx++)
            {
                var cycletimes = CalculateCycleTimePoints(ldate[idx], ldate[idx + 1], filename);
                foreach (var wkl in cycletimes)
                {

                        if (ret.ContainsKey(ldate[idx].ToString("yy/MM")))
                        {
                            ret[ldate[idx].ToString("yy/MM")].appendcycledata(wkl);
                        }
                        else
                        {
                            ret.Add(ldate[idx].ToString("yy/MM"), new CycleTimeDataField());
                            ret[ldate[idx].ToString("yy/MM")].appendcycledata(wkl);
                        }

                }//foreach
            }

            return ret;
        }

        public static Dictionary<string, CycleTimeDataField> RetrieveQuartCycleTimeData(DateTime startdate, DateTime enddate,string filename)
        {
            var ret = new Dictionary<string, CycleTimeDataField>();
            var ldate = RetrieveDateSpanByQuart(startdate.ToString(), enddate.ToString());
            for (int idx = 0; idx < ldate.Count - 1; idx++)
            {
                var cycletimes = CalculateCycleTimePoints(ldate[idx], ldate[idx + 1], filename);
                foreach (var wkl in cycletimes)
                {
                    var key = QuartStr(ldate[idx]);
                    if (ret.ContainsKey(key))
                    {
                        ret[key].appendcycledata(wkl);
                    }
                    else
                    {
                        ret.Add(key, new CycleTimeDataField());
                        ret[key].appendcycledata(wkl);
                    }

                }//foreach
            }

            return ret;
        }

        private static int CountWorkDays(DateTime startDate, DateTime endDate)
        {
            int dayCount = 0;

            DateTime tmpDate = startDate;
            DateTime finiDate = endDate;
            if (startDate > endDate)
            {
                tmpDate = endDate;
                finiDate = startDate;
            }
            
            while (tmpDate <= finiDate)
            {
                if (tmpDate.DayOfWeek != DayOfWeek.Sunday 
                    && tmpDate.DayOfWeek != DayOfWeek.Saturday)
                {
                     dayCount = dayCount+1;
                }

                // Move onto next day
                tmpDate = tmpDate.AddDays(1);
            }

            if (startDate > endDate)
            {
                return 0 - dayCount;
            }
            else
            {
                return dayCount;
            }
         }

        private static DateTime MaxDate(List<string> days)
        {
            DateTime initdate = DateTime.Parse("1982-05-06 07:30:00");
            foreach (var d in days)
            {
                if (!string.IsNullOrEmpty(d))
                {
                    if (DateTime.Parse(d) > initdate)
                        initdate = DateTime.Parse(d);
                }
            }
            return initdate;
        }



        private static List<ComplexData> RetrieveAllComplexData(DateTime startDate, DateTime endDate)
        {
            var allcomplex = new List<ComplexData>();

            var alleco = ECOBaseInfo.RetrieveAllNotDeleteECOBaseInfo();
            foreach (var eco in alleco)
            {
                if (string.Compare(DateTime.Parse(eco.InitRevison).ToString("yyyy-MM-dd"), "1982-05-06") == 0)
                {

                }
                else
                {
                    var complexdata = new ComplexData();
                    complexdata.ECOKey = eco.ECOKey;
                    complexdata.Customer = eco.Customer;
                    if (!eco.PE.Contains("@"))
                    {
                        complexdata.PE = (eco.PE.Trim().Replace(" ", ".") + "@finisar.com").ToUpper();
                    }
                    else
                    {
                        complexdata.PE = eco.PE;
                    }

                    complexdata.InitReceiveDate = DateTime.Parse(DateTime.Parse(eco.InitRevison).ToString("yyyy-MM-dd") + " 07:30:00");


                    var completecard = DominoVM.RetrieveSpecialCard(eco, DominoCardType.ECOComplete);
                    if (completecard.Count > 0)
                    {
                        var cardinfo = DominoVM.RetrieveECOCompleteInfo(completecard[0].CardKey);
                        if (string.IsNullOrEmpty(cardinfo.ECOCompleteDate))
                        {
                            complexdata.ECOCompleteDate = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd") + " 07:30:00").AddDays(60);
                        }
                        else
                        {
                            complexdata.ECOCompleteDate = DateTime.Parse(DateTime.Parse(cardinfo.ECOCompleteDate).ToString("yyyy-MM-dd") + " 07:30:00");
                        }
                    }
                    else
                    {
                        complexdata.ECOCompleteDate = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd") + " 07:30:00").AddDays(60);
                    }

                    if (startDate <= complexdata.InitReceiveDate && complexdata.InitReceiveDate <= endDate)
                    {
                        complexdata.SetComplexType(eco.Complex);
                        allcomplex.Add(complexdata);
                    }
                    else if (startDate >= complexdata.InitReceiveDate && startDate <= complexdata.ECOCompleteDate)
                    {
                        complexdata.SetComplexType(eco.Complex);
                        allcomplex.Add(complexdata);
                    }

                }//end else
            }//end foreach

            return allcomplex;
        }

        public static Dictionary<string, ComplexData> RetrieveDepartComplexData(DateTime startDate, DateTime endDate)
        {
            var allcomplexdata = RetrieveAllComplexData(startDate, endDate);

            var udlist = DominoUserViewModels.RetrieveAllUserDepart();
            var uddict = new Dictionary<string, string>();
            foreach (var ud in udlist)
            {
                uddict.Add(ud.UserName, ud.Depart);
            }

            var departcomplex = new List<ComplexData>();
            foreach (var wkl in allcomplexdata)
            {
                if (uddict.ContainsKey(wkl.PE))
                {
                    wkl.Depart = uddict[wkl.PE];
                    departcomplex.Add(wkl);
                }
            }

            var ret = new Dictionary<string, ComplexData>();

            var departs = DominoUserViewModels.RetrieveAllDepartment();
            foreach (var dpt in departs)
            {
                foreach (var wkl in departcomplex)
                {
                    if (string.Compare(dpt, wkl.Depart) == 0)
                    {
                        if (ret.ContainsKey(dpt))
                        {
                            ret[dpt].AppendComplexData(wkl);
                        }
                        else
                        {
                            ret.Add(dpt, new ComplexData());
                            ret[dpt].AppendComplexData(wkl);
                        }
                    }//end if
                }//foreach
            }//foreach

            return ret;
        }

        public static Dictionary<string, ComplexData> RetrievePEComplexData(DateTime startDate, DateTime endDate)
        {
            var allcomplexdata = RetrieveAllComplexData(startDate, endDate);

            var pedict = new Dictionary<string, bool>();
            foreach (var w in allcomplexdata)
            {
                if (!pedict.ContainsKey(w.PE))
                {
                    pedict.Add(w.PE, true);
                }
            }


            var ret = new Dictionary<string, ComplexData>();

            var pes = pedict.Keys.ToList();
            
            foreach (var pe in pes)
            {
                foreach (var wkl in allcomplexdata)
                {
                    if (string.Compare(pe, wkl.PE) == 0)
                    {
                        if (ret.ContainsKey(pe))
                        {
                            ret[pe].AppendComplexData(wkl);
                        }
                        else
                        {
                            ret.Add(pe, new ComplexData());
                            ret[pe].AppendComplexData(wkl);
                        }
                    }//end if
                }//foreach
            }//foreach

            return ret;
        }


        public static Dictionary<string, ComplexData> RetrieveCustomerComplexData(DateTime startDate, DateTime endDate)
        {
            var allcomplexdata = RetrieveAllComplexData(startDate, endDate);

            var custdict = new Dictionary<string, bool>();
            foreach (var w in allcomplexdata)
            {
                if (!custdict.ContainsKey(w.Customer))
                {
                    custdict.Add(w.Customer, true);
                }
            }

            var ret = new Dictionary<string, ComplexData>();

            var custs = custdict.Keys.ToList();

            foreach (var cu in custs)
            {
                foreach (var wkl in allcomplexdata)
                {
                    if (string.Compare(cu, wkl.Customer) == 0)
                    {
                        if (ret.ContainsKey(cu))
                        {
                            ret[cu].AppendComplexData(wkl);
                        }
                        else
                        {
                            ret.Add(cu, new ComplexData());
                            ret[cu].AppendComplexData(wkl);
                        }
                    }//end if
                }//foreach
            }//foreach

            return ret;
        }

        public static Dictionary<string, ComplexData> RetrieveMonthlyComplexData(DateTime startDate, DateTime endDate)
        {
            var ret = new Dictionary<string, ComplexData>();
            var ldate = RetrieveDateSpanByMonth(startDate.ToString(), endDate.ToString());
            for (int idx = 0; idx < ldate.Count - 1; idx++)
            {
                var allcomplexdata = RetrieveAllComplexData(ldate[idx], ldate[idx + 1]);
                foreach (var wkl in allcomplexdata)
                {
                        if (ret.ContainsKey(ldate[idx].ToString("yy/MM")))
                        {
                            ret[ldate[idx].ToString("yy/MM")].AppendComplexData(wkl);
                        }
                        else
                        {
                            ret.Add(ldate[idx].ToString("yy/MM"), new ComplexData());
                            ret[ldate[idx].ToString("yy/MM")].AppendComplexData(wkl);
                        }

                }//foreach
            }
                
            return ret;
        }


        public static Dictionary<string, ComplexData> RetrieveQuartComplexData(DateTime startDate, DateTime endDate)
        {
            var ret = new Dictionary<string, ComplexData>();
            var ldate = RetrieveDateSpanByQuart(startDate.ToString(), endDate.ToString());
            for (int idx = 0; idx < ldate.Count - 1; idx++)
            {
                var allcomplexdata = RetrieveAllComplexData(ldate[idx], ldate[idx + 1]);
                foreach (var wkl in allcomplexdata)
                {
                    var key = QuartStr(ldate[idx]);
                    if (ret.ContainsKey(key))
                    {
                        ret[key].AppendComplexData(wkl);
                    }
                    else
                    {
                        ret.Add(key, new ComplexData());
                        ret[key].AppendComplexData(wkl);
                    }

                }//foreach
            }

            return ret;
        }


        private static List<RevenueData> RetrieveAllRevenueData(DateTime startDate, DateTime endDate,Dictionary<string,string> departdict,string filename)
        {

            var allrevenue = new List<RevenueData>();
            var alleco = ECOBaseInfo.RetrieveAllNotDeleteECOBaseInfo();
            foreach (var eco in alleco)
            {
                if (eco.ECORevenue == 0)
                { continue; }

                if (string.Compare(DateTime.Parse(eco.InitRevison).ToString("yyyy-MM-dd"), "1982-05-06") == 0)
                {

                }
                else
                {
                    var revenue = new RevenueData();

                    revenue.InitReceiveDate = DateTime.Parse(DateTime.Parse(eco.InitRevison).ToString("yyyy-MM-dd") + " 07:30:00");
                    var completecard = DominoVM.RetrieveSpecialCard(eco, DominoCardType.ECOComplete);
                    if (completecard.Count > 0)
                    {
                        var cardinfo = DominoVM.RetrieveECOCompleteInfo(completecard[0].CardKey);
                        if (string.IsNullOrEmpty(cardinfo.ECOCompleteDate))
                        {
                            revenue.ECOCompleteDate = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd") + " 07:30:00").AddDays(60);
                        }
                        else
                        {
                            revenue.ECOCompleteDate = DateTime.Parse(DateTime.Parse(cardinfo.ECOCompleteDate).ToString("yyyy-MM-dd") + " 07:30:00");
                        }
                    }
                    else
                    {
                        revenue.ECOCompleteDate = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd") + " 07:30:00").AddDays(60);
                    }

                    if (startDate <= revenue.InitReceiveDate && revenue.InitReceiveDate <= endDate)
                    {
                        revenue.ECONUM = eco.ECONum;
                        revenue.PN = eco.PNDesc;
                        revenue.Revenue = eco.ECORevenue;
                        revenue.Customer = eco.Customer;
                        if (!eco.PE.Contains("@"))
                        {
                            revenue.PE = (eco.PE.Trim().Replace(" ", ".") + "@finisar.com").ToUpper();
                        }
                        else
                        {
                            revenue.PE = eco.PE;
                        }
                        revenue.Status = eco.MiniPIPStatus;
                        revenue.ECOType = eco.ECOType;
                        if (departdict.ContainsKey(revenue.PE))
                        { revenue.Depart = departdict[revenue.PE]; }

                        allrevenue.Add(revenue);
                    }
                    else if (startDate >= revenue.InitReceiveDate && startDate <= revenue.ECOCompleteDate)
                    {
                        revenue.ECONUM = eco.ECONum;
                        revenue.PN = eco.PNDesc;
                        revenue.Revenue = eco.ECORevenue;
                        revenue.Customer = eco.Customer;
                        if (!eco.PE.Contains("@"))
                        {
                            revenue.PE = (eco.PE.Trim().Replace(" ", ".") + "@finisar.com").ToUpper();
                        }
                        else
                        {
                            revenue.PE = eco.PE;
                        }
                        revenue.Status = eco.MiniPIPStatus;
                        revenue.ECOType = eco.ECOType;
                        if (departdict.ContainsKey(revenue.PE))
                        { revenue.Depart = departdict[revenue.PE]; }

                        allrevenue.Add(revenue);
                    }

                }//end else
            }//end foreach

            logreportinfo(filename, "ECONum,PN,PE,Customer,Revenue,ECOType,Status,InitReceiveDate,ECOCompleteDate\r\n");
            foreach (var revenue in allrevenue)
            {
                logreportinfo(filename, revenue.ECONUM+","+ revenue.PN.Replace(",","")+","+ revenue.PE+","+ revenue.Customer.Replace(",", "") + ","+revenue.Revenue.ToString()+","+
                    revenue.ECOType+","+revenue.Status+","+revenue.InitReceiveDate.ToString("yyyy-MM-dd")+","+revenue.ECOCompleteDate.ToString("yyyy-MM-dd")+"\r\n");
            }

            return allrevenue;
        }


        public static Dictionary<string, RevenueData> RetrieveDepartRevenueData(DateTime startDate, DateTime endDate,string filename)
        {
            var udlist = DominoUserViewModels.RetrieveAllUserDepart();
            var uddict = new Dictionary<string, string>();
            foreach (var ud in udlist)
            {
                uddict.Add(ud.UserName, ud.Depart);
            }

            var allrevenuedata = RetrieveAllRevenueData(startDate, endDate,uddict,filename);

            var ret = new Dictionary<string, RevenueData>();

            foreach (var rvdata in allrevenuedata)
            {
                if (string.IsNullOrEmpty(rvdata.Depart))
                { continue; }
                if (ret.ContainsKey(rvdata.Depart))
                {
                    ret[rvdata.Depart].AppendRevenueData(rvdata);
                }
                else
                {
                    ret.Add(rvdata.Depart, new RevenueData());
                    ret[rvdata.Depart].AppendRevenueData(rvdata);
                }
            }//foreach


            return ret;
        }

        public static Dictionary<string, RevenueData> RetrievePERevenueData(DateTime startDate, DateTime endDate, string filename)
        {
            var udlist = DominoUserViewModels.RetrieveAllUserDepart();
            var uddict = new Dictionary<string, string>();
            foreach (var ud in udlist)
            {
                uddict.Add(ud.UserName, ud.Depart);
            }

            var allrevenuedata = RetrieveAllRevenueData(startDate, endDate, uddict, filename);

            var ret = new Dictionary<string, RevenueData>();

            foreach (var rvdata in allrevenuedata)
            {
                if (string.IsNullOrEmpty(rvdata.PE))
                { continue; }

                if (ret.ContainsKey(rvdata.PE))
                {
                    ret[rvdata.PE].AppendRevenueData(rvdata);
                }
                else
                {
                    ret.Add(rvdata.PE, new RevenueData());
                    ret[rvdata.PE].AppendRevenueData(rvdata);
                }
            }//foreach

            return ret;
        }

        public static Dictionary<string, RevenueData> RetrieveCustomerRevenueData(DateTime startDate, DateTime endDate, string filename)
        {
            var udlist = DominoUserViewModels.RetrieveAllUserDepart();
            var uddict = new Dictionary<string, string>();
            foreach (var ud in udlist)
            {
                uddict.Add(ud.UserName, ud.Depart);
            }

            var allrevenuedata = RetrieveAllRevenueData(startDate, endDate, uddict, filename);

            var ret = new Dictionary<string, RevenueData>();

            foreach (var rvdata in allrevenuedata)
            {
                if (string.IsNullOrEmpty(rvdata.Customer))
                { continue; }

                if (ret.ContainsKey(rvdata.Customer))
                {
                    ret[rvdata.Customer].AppendRevenueData(rvdata);
                }
                else
                {
                    ret.Add(rvdata.Customer, new RevenueData());
                    ret[rvdata.Customer].AppendRevenueData(rvdata);
                }
            }//foreach

            return ret;
        }

        public static Dictionary<string, RevenueData> RetrieveMonthlyRevenueData(DateTime startDate, DateTime endDate, string filename)
        {
            var udlist = DominoUserViewModels.RetrieveAllUserDepart();
            var uddict = new Dictionary<string, string>();
            foreach (var ud in udlist)
            {
                uddict.Add(ud.UserName, ud.Depart);
            }

            var ret = new Dictionary<string, RevenueData>();
            var ldate = RetrieveDateSpanByMonth(startDate.ToString(), endDate.ToString());
            for (int idx = 0; idx < ldate.Count - 1; idx++)
            {
                var allrevenuedata = RetrieveAllRevenueData(ldate[idx], ldate[idx + 1], uddict, filename);
                foreach (var rvdata in allrevenuedata)
                {
                    if (ret.ContainsKey(ldate[idx].ToString("yy/MM")))
                    {
                        ret[ldate[idx].ToString("yy/MM")].AppendRevenueData(rvdata);
                    }
                    else
                    {
                        ret.Add(ldate[idx].ToString("yy/MM"), new RevenueData());
                        ret[ldate[idx].ToString("yy/MM")].AppendRevenueData(rvdata);
                    }

                }//foreach
            }

            return ret;
        }

        public static Dictionary<string, RevenueData> RetrieveQuartRevenueData(DateTime startDate, DateTime endDate, string filename)
        {
            var udlist = DominoUserViewModels.RetrieveAllUserDepart();
            var uddict = new Dictionary<string, string>();
            foreach (var ud in udlist)
            {
                uddict.Add(ud.UserName, ud.Depart);
            }

            var ret = new Dictionary<string, RevenueData>();
            var ldate = RetrieveDateSpanByMonth(startDate.ToString(), endDate.ToString());
            for (int idx = 0; idx < ldate.Count - 1; idx++)
            {
                var allrevenuedata = RetrieveAllRevenueData(ldate[idx], ldate[idx + 1], uddict, filename);
                foreach (var rvdata in allrevenuedata)
                {
                    var key = QuartStr(ldate[idx]);
                    if (ret.ContainsKey(key))
                    {
                        ret[key].AppendRevenueData(rvdata);
                    }
                    else
                    {
                        ret.Add(key, new RevenueData());
                        ret[key].AppendRevenueData(rvdata);
                    }

                }//foreach
            }

            return ret;
        }


        public static Dictionary<string, QACheckData> RetrieveDepartQACheckData(Controller ctrl,DateTime StartDate,DateTime EndDate)
        {
            var alllist = DominoDataCollector.RetrieveAllQACheckInfo(ctrl);
            var datelist = new List<QACheckData>();
            foreach (var qacheck in alllist)
            {
                if (qacheck.QADate >= StartDate && qacheck.QADate <= EndDate)
                {
                    datelist.Add(qacheck);
                }
            }

            var udlist = DominoUserViewModels.RetrieveAllUserDepart();
            var uddict = new Dictionary<string, string>();
            foreach (var ud in udlist)
            {
                uddict.Add(ud.UserName, ud.Depart);
            }

            var departqa = new List<QACheckData>();
            foreach (var wkl in datelist)
            {
                if (uddict.ContainsKey(wkl.PE))
                {
                    wkl.Depart = uddict[wkl.PE];
                    departqa.Add(wkl);
                }
            }

            var ret = new Dictionary<string, QACheckData>();
            var departs = DominoUserViewModels.RetrieveAllDepartment();
            foreach (var dpt in departs)
            {
                foreach (var wkl in departqa)
                {
                    if (string.Compare(dpt, wkl.Depart) == 0)
                    {
                        if (ret.ContainsKey(dpt))
                        {
                            ret[dpt].AppendQAData(wkl);
                        }
                        else
                        {
                            ret.Add(dpt, new QACheckData());
                            ret[dpt].AppendQAData(wkl);
                        }
                    }//end if
                }//foreach
            }//foreach
            return ret;
        }

        public static Dictionary<string, QACheckData> RetrievePEQACheckData(Controller ctrl, DateTime StartDate, DateTime EndDate)
        {
            var alllist = DominoDataCollector.RetrieveAllQACheckInfo(ctrl);
            var datelist = new List<QACheckData>();
            foreach (var qacheck in alllist)
            {
                if (qacheck.QADate >= StartDate && qacheck.QADate <= EndDate)
                {
                    datelist.Add(qacheck);
                }
            }

            var pedict = new Dictionary<string, bool>();
            foreach (var w in datelist)
            {
                if (!pedict.ContainsKey(w.PE))
                {
                    pedict.Add(w.PE, true);
                }
            }

            var ret = new Dictionary<string, QACheckData>();
            var pes = pedict.Keys.ToList();
            foreach (var pe in pes)
            {
                foreach (var wkl in datelist)
                {
                    if (string.Compare(pe, wkl.PE) == 0)
                    {
                        if (ret.ContainsKey(pe))
                        {
                            ret[pe].AppendQAData(wkl);
                        }
                        else
                        {
                            ret.Add(pe, new QACheckData());
                            ret[pe].AppendQAData(wkl);
                        }
                    }//end if
                }//foreach
            }//foreach
            return ret;
        }


        private static List<QACheckData> getperiodqacheck(Controller ctrl, DateTime StartDate, DateTime EndDate)
        {
            var alllist = DominoDataCollector.RetrieveAllQACheckInfo(ctrl);
            var datelist = new List<QACheckData>();
            foreach (var qacheck in alllist)
            {
                if (qacheck.QADate >= StartDate && qacheck.QADate <= EndDate)
                {
                    datelist.Add(qacheck);
                }
            }
            return datelist;
        }

        public static Dictionary<string, QACheckData> RetrieveMonthlyQACheckData(Controller ctrl, DateTime StartDate, DateTime EndDate)
        {
            var ret = new Dictionary<string, QACheckData>();
            var ldate = RetrieveDateSpanByMonth(StartDate.ToString(), EndDate.ToString());
            for (int idx = 0; idx < ldate.Count - 1; idx++)
            {
                var datelist = getperiodqacheck(ctrl,ldate[idx], ldate[idx + 1]);
                foreach (var wkl in datelist)
                {
                        if (ret.ContainsKey(ldate[idx].ToString("yy/MM")))
                        {
                            ret[ldate[idx].ToString("yy/MM")].AppendQAData(wkl);
                        }
                        else
                        {
                            ret.Add(ldate[idx].ToString("yy/MM"), new QACheckData());
                            ret[ldate[idx].ToString("yy/MM")].AppendQAData(wkl);
                        }
                }//foreach
            }

            return ret;
        }

        public static Dictionary<string, QACheckData> RetrieveQuartQACheckData(Controller ctrl, DateTime StartDate, DateTime EndDate)
        {
            var ret = new Dictionary<string, QACheckData>();
            var ldate = RetrieveDateSpanByQuart(StartDate.ToString(), EndDate.ToString());
            for (int idx = 0; idx < ldate.Count - 1; idx++)
            {
                var datelist = getperiodqacheck(ctrl, ldate[idx], ldate[idx + 1]);
                foreach (var wkl in datelist)
                {
                    var key = QuartStr(ldate[idx]);
                    if (ret.ContainsKey(key))
                    {
                        ret[key].AppendQAData(wkl);
                    }
                    else
                    {
                        ret.Add(key, new QACheckData());
                        ret[key].AppendQAData(wkl);
                    }
                }//foreach
            }

            return ret;
        }

    }
}