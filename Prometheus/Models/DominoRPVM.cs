using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Domino.Models
{
    public class WorkLoadData
    {
        public string ECOKey { set; get; }
        public string PE { set; get; }
        public string Depart { set; get; }
        public string Status { set; get; }

        public DateTime InitReceiveDate { set; get; }
        public DateTime HoldStartDate { set; get; }
        public DateTime HoldEndDate { set; get; }
        public DateTime ECOSubmitDate { set; get; }
        public DateTime ECOCompleteDate { set; get; }
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

    public class DominoRPVM
    {
        private static string WorkLoadStatus(DateTime startdate, DateTime enddate, WorkLoadData workloaddata)
        {
            if (workloaddata.InitReceiveDate >= startdate && workloaddata.InitReceiveDate <= enddate)
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

        private static List<WorkLoadData> RetrieveWorkLoadData(DateTime startdated, DateTime enddated)
        {
            var startdate = DateTime.Parse(startdated.ToString("yyyy-MM-dd") + " 07:30:00");
            var enddate = DateTime.Parse(enddated.ToString("yyyy-MM-dd") + " 07:30:00");

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

                    tempworkload.HoldStartDate = tempworkload.InitReceiveDate.AddDays(3);
                    tempworkload.HoldEndDate = tempworkload.ECOSubmitDate.AddDays(-3);

                    if (tempworkload.HoldStartDate > tempworkload.HoldEndDate)
                    {
                        tempworkload.HoldEndDate = tempworkload.HoldStartDate;
                    }

                    allworkload.Add(tempworkload);
                }//end else
            }//end foreach

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

    }
}