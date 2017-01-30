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
        public string PE { set; get; }
        public string Depart { set; get; }
        public string Customer { set; get; }
        public string Quarter { set; get; }
        public string Month { set; get; }

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
                if (string.Compare(DateTime.Parse(eco.InitRevison).ToString("yyyy-MM-dd"), "1982-05-06") == 0)
                {

                }
                else
                {
                    var tempworkload = new WorkLoadData();
                    tempworkload.ECOKey = eco.ECOKey;
                    tempworkload.Customer = eco.Customer;
                    if (!eco.PE.Contains("@"))
                    {
                        tempworkload.PE = (eco.PE.Trim().Replace(" ", ".") + "@finisar.com").ToUpper();
                    }
                    else
                    {
                        tempworkload.PE = eco.PE;
                    }
                    
                    tempworkload.InitReceiveDate = DateTime.Parse(DateTime.Parse(eco.InitRevison).ToString("yyyy-MM-dd") + " 07:30:00");

                    if (string.Compare(DateTime.Parse(eco.ECOSubmit).ToString("yyyy-MM-dd"), "1982-05-06") == 0)
                    {
                        tempworkload.ECOSubmitDate = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd") + " 07:30:00").AddDays(30);
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
                            tempworkload.ECOCompleteDate = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd") + " 07:30:00").AddDays(60);
                        }
                        else
                        {
                            tempworkload.ECOCompleteDate = DateTime.Parse(DateTime.Parse(cardinfo.ECOCompleteDate).ToString("yyyy-MM-dd") + " 07:30:00");
                        }
                    }
                    else
                    {
                        tempworkload.ECOCompleteDate = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd") + " 07:30:00").AddDays(60);
                    }

                    //tempworkload.HoldStartDate = DateTime.Parse(eco.ECOHoldStartDate);
                    //tempworkload.HoldEndDate = DateTime.Parse(eco.ECOHoldEndDate);

                    tempworkload.HoldStartDate = tempworkload.InitReceiveDate.AddDays(3);
                    tempworkload.HoldEndDate = tempworkload.ECOSubmitDate.AddDays(-3);

                    if (tempworkload.HoldStartDate > tempworkload.HoldEndDate)
                    {
                        tempworkload.HoldEndDate = tempworkload.HoldStartDate;
                    }

                    allworkload.Add(tempworkload);
                }//end else
            }//end foreach

            return allworkload;
        }

        private static List<WorkLoadData> RetrieveWorkLoadData(DateTime startdated, DateTime enddated)
        {
            var startdate = DateTime.Parse(startdated.ToString("yyyy-MM-dd") + " 07:30:00");
            var enddate = DateTime.Parse(enddated.ToString("yyyy-MM-dd") + " 07:30:00");

            var allworkload = CollectAllWorkLoadTimePoint();

            var filterworkload = new List<WorkLoadData>();
            foreach (var wkload in allworkload)
            {
                var status = WorkLoadStatus(startdate, enddate, wkload);
                if (string.Compare(status, DominoCardType.None) != 0)
                {
                    wkload.Status = status;
                    filterworkload.Add(wkload);
                }
            }//end foreach

            return filterworkload;
        }

        public static Dictionary<string, WorkLoadField> RetrieveDepartWorkLoadData(DateTime startdate, DateTime enddate)
        {
            var udlist = DominoUserViewModels.RetrieveAllUserDepart();
            var uddict = new Dictionary<string, string>();
            foreach (var ud in udlist)
            {
                uddict.Add(ud.UserName, ud.Depart);
            }

            var wkloads = RetrieveWorkLoadData(startdate, enddate);

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

        public static Dictionary<string, WorkLoadField> RetrievePEWorkLoadData(DateTime startdate, DateTime enddate)
        {
            var pewkloads = RetrieveWorkLoadData(startdate, enddate);

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

            var ecodone = ECOBaseInfo.RetrieveCompletedECOBaseInfo();
            foreach (var eco in ecodone)
            {
                var tempcycle = new CycleTimeData();
                try
                {
                    var completecard = DominoVM.RetrieveSpecialCard(eco, DominoCardType.ECOComplete);
                    if (completecard.Count > 0)
                    {
                        var cardinfo = DominoVM.RetrieveECOCompleteInfo(completecard[0].CardKey);
                        tempcycle.ECOCompleteDate = ConvertDate(cardinfo.ECOCompleteDate);

                        if (!string.IsNullOrEmpty(tempcycle.ECOCompleteDate))
                        {
                            var completedate = DateTime.Parse(tempcycle.ECOCompleteDate);
                            if (completedate >= startdate && completedate <= enddate)
                            {
                                tempcycle.Month = completedate.Month.ToString();
                                tempcycle.Quarter = QuartStr(completedate.Month, DateTime.Parse(tempcycle.ECOCompleteDate));
                            }
                            else
                            {
                                continue;
                            }

                        }
                        else
                        {
                            continue;
                        }
                    }//end if


                    tempcycle.ECOKey = eco.ECOKey;
                    tempcycle.Customer = eco.Customer;
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


        private static List<CycleTimeData> CalculateCycleTimePoints(DateTime startdate, DateTime enddate)
        {
            var cyclelist = CollectAllCycleTimePoint(startdate, enddate);

            var calculatedlist = new List<CycleTimeData>();
            foreach (var cycle in cyclelist)
            {
                try
                {
                    if (string.IsNullOrEmpty(cycle.OpsEntry))
                    {
                        cycle.MiniPIPApprovalAging = DOMINOCYCLETIMEVAL.DInvalidVAL;
                    }
                    else
                    {
                        if (DateTime.Parse(cycle.OpsEntry) >= DateTime.Parse(cycle.FinalRevison))
                        {
                            cycle.MiniPIPApprovalAging = CountWorkDays(DateTime.Parse(cycle.FinalRevison), DateTime.Parse(cycle.OpsEntry)) - 1;
                        }
                        else
                        {
                            cycle.MiniPIPApprovalAging = CountWorkDays(DateTime.Parse(cycle.InitRevison), DateTime.Parse(cycle.OpsEntry)) - 1;
                        }
                    }

                    if (string.IsNullOrEmpty(cycle.FinalRevison))
                    {
                        cycle.ChangeDelayAging = DOMINOCYCLETIMEVAL.DInvalidVAL;
                    }
                    else
                    {
                        cycle.ChangeDelayAging = CountWorkDays(DateTime.Parse(cycle.InitRevison), DateTime.Parse(cycle.FinalRevison)) - 1;
                    }


                    if (string.IsNullOrEmpty(cycle.ECOSubmit))
                    {
                        cycle.EngineeringAging = DOMINOCYCLETIMEVAL.DInvalidVAL;
                    }
                    else
                    {
                        var datelist = new List<string>();
                        datelist.Add(cycle.InitRevison);
                        datelist.Add(cycle.FinalRevison);
                        datelist.Add(cycle.TLAAvailable);
                        datelist.Add(cycle.OpsEntry);
                        datelist.Add(cycle.TestModification);

                        cycle.EngineeringAging = CountWorkDays(MaxDate(datelist), DateTime.Parse(cycle.ECOSubmit)) - 1;
                    }

                    if (string.IsNullOrEmpty(cycle.ECOTRSignoff))
                    {
                        cycle.TechReviewAging = DOMINOCYCLETIMEVAL.DInvalidVAL;
                    }
                    else
                    {
                        cycle.TechReviewAging = CountWorkDays(DateTime.Parse(cycle.ECOSubmit), DateTime.Parse(cycle.ECOTRSignoff)) - 1;
                    }

                    if (string.IsNullOrEmpty(cycle.ECOCCBSignoff))
                    {
                        cycle.CCBSignoffAging = DOMINOCYCLETIMEVAL.DInvalidVAL;
                    }
                    else
                    {
                        cycle.CCBSignoffAging = CountWorkDays(DateTime.Parse(cycle.ECOTRSignoff), DateTime.Parse(cycle.ECOCCBSignoff)) - 1;
                    }

                    if (string.IsNullOrEmpty(cycle.SampleShipDate))
                    {
                        cycle.SampleShipAging = DOMINOCYCLETIMEVAL.DInvalidVAL;
                    }
                    else
                    {
                        cycle.SampleShipAging = CountWorkDays(DateTime.Parse(cycle.InitRevison), DateTime.Parse(cycle.SampleShipDate)) - 1;
                    }

                    calculatedlist.Add(cycle);
                }
                catch (Exception ex) { }
            }
            return calculatedlist;
        }


        public static Dictionary<string, CycleTimeDataField> RetrieveDepartCycleTimeData(DateTime startdate, DateTime enddate)
        {
            var udlist = DominoUserViewModels.RetrieveAllUserDepart();
            var uddict = new Dictionary<string, string>();
            foreach (var ud in udlist)
            {
                uddict.Add(ud.UserName, ud.Depart);
            }

            var cycletimes = CalculateCycleTimePoints(startdate, enddate);

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

        public static Dictionary<string, CycleTimeDataField> RetrievPECycleTimeData(DateTime startdate, DateTime enddate)
        {
            var cycletimes = CalculateCycleTimePoints(startdate, enddate);

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

        private static string QuartStr(int month,DateTime completedate)
        {
            if (month >= 5 && month <= 7)
            {
                return "Q1/" + completedate.ToString("yy");
            }
            else if (month >= 8 && month <= 10)
            {
                return "Q2/" + completedate.ToString("yy");
            }
            else if (month >= 2 && month <= 4)
            {
                return "Q4" + completedate.AddYears(-1).ToString("yy");
            }
            else
            {
                if (month >= 11 && month <= 12)
                {
                    return "Q3/" + completedate.ToString("yy");
                }
                else
                {
                    return "Q3/" + completedate.AddYears(-1).ToString("yy");
                }
            }
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




    }
}