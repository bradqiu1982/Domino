using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Domino.Models
{
    public class NewLoadMiniPIP
    {
        public NewLoadMiniPIP()
        {
            MiniPIPID = "";
            OrderInfo = "";
            SpecailMaterial = "";
            PN = "";
            PE = "";
            Removed = "";
            AlertDate = DateTime.Parse("1982-05-06 10:00:00");
            ResponseDate = DateTime.Parse("1982-05-06 10:00:00");
        }

        public NewLoadMiniPIP(string mid, string oi, string sm, string pn, string pe, DateTime alertdate)
        {
            MiniPIPID = mid;
            OrderInfo = oi;
            SpecailMaterial = sm;
            PN = pn;
            PE = pe;
            AlertDate = alertdate;
        }

        public static void RegistNewLoadMiniPIP(string mid, string pn, string pe)
        {
            var sql = "insert into NewLoadMiniPIP(MiniPIPID,PN,PE) values('<MiniPIPID>','<PN>','<PE>')";
            sql = sql.Replace("<MiniPIPID>", mid).Replace("<PN>", pn).Replace("<PE>", pe);
            DBUtility.ExeLocalSqlNoRes(sql);
        }

        public static void ReceiveResponsed(string mid,string machine)
        {
            var sql = "update NewLoadMiniPIP set Removed = 'TRUE',ResponseMachine = '<ResponseMachine>',ResponseDate = '<ResponseDate>' where MiniPIPID = '<MiniPIPID>'";
            sql = sql.Replace("<MiniPIPID>", mid).Replace("<ResponseMachine>",machine).Replace("<ResponseDate>",DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            DBUtility.ExeLocalSqlNoRes(sql);
        }

        public static string RMSpectialCh(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'))
                {
                    sb.Append(c);
                }
            }
            return sb.ToString().ToUpper();
        }

        public static Dictionary<string, bool> NewLoadPIPToShow()
        {
            var ret = new Dictionary<string, bool>();
            var sql = "select MiniPIPID from NewLoadMiniPIP where Removed <> 'TRUE' and SpecailMaterial = ''";
            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            foreach (var line in dbret)
            {
                var mid = Convert.ToString(line[0]);
                if (!ret.ContainsKey(mid))
                {
                    ret.Add(mid, true);
                }
            }//end foreach
            return ret;
        }

        public static Dictionary<string, bool> NewLoadNeedOrderInfo()
        {
            var ret = new Dictionary<string, bool>();
            var sql = "select MiniPIPID from NewLoadMiniPIP where Removed <> 'TRUE' and OrderInfo = ''";
            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            foreach (var line in dbret)
            {
                var mid = Convert.ToString(line[0]);
                if (!ret.ContainsKey(mid))
                {
                    ret.Add(mid, true);
                }
            }//end foreach
            return ret;
        }

        public static Dictionary<string, bool> NewLoadPIPModalToShow()
        {
            var ret = new Dictionary<string, bool>();
            var sql = "select MiniPIPID from NewLoadMiniPIP where SpecailMaterial = '' and Removed <> 'TRUE' and OrderInfo <> ''";
            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            foreach (var line in dbret)
            {
                var mid = Convert.ToString(line[0]);
                if (!ret.ContainsKey(mid))
                {
                    ret.Add(mid, true);
                }
            }//end foreach
            return ret;
        }

        public static void UpdateOrderInfo(string mid, string orderinfo)
        {
            var ord = RMSpectialCh(orderinfo);
            if (ord.Contains("NA"))
            {
                ReceiveResponsed(mid,"NA");
            }

            var sql = "update NewLoadMiniPIP set OrderInfo = '<OrderInfo>' where MiniPIPID = '<MiniPIPID>'";
            sql = sql.Replace("<MiniPIPID>", mid).Replace("<OrderInfo>",ord.Replace("'",""));
            DBUtility.ExeLocalSqlNoRes(sql);
        }

        public static List<NewLoadMiniPIP> RetrieveNewLoadMiniPIP(string mid)
        {
            var alertlist = new List<NewLoadMiniPIP>();

            var sql = "select MiniPIPID,OrderInfo,SpecailMaterial,PN,PE,AlertDate from NewLoadMiniPIP where MiniPIPID = '<MiniPIPID>'";
            sql = sql.Replace("<MiniPIPID>",mid);
            var dbret = DBUtility.ExeLocalSqlWithRes(sql);

            foreach (var line in dbret)
            {

                    var tempvm = new NewLoadMiniPIP();
                    tempvm.MiniPIPID = Convert.ToString(line[0]);
                    tempvm.OrderInfo = Convert.ToString(line[1]);
                    tempvm.SpecailMaterial = Convert.ToString(line[2]);
                    tempvm.PN = Convert.ToString(line[3]);
                    tempvm.PE = Convert.ToString(line[4]);

                    alertlist.Add(tempvm);

            }//end foreach

            return alertlist;
        }

        public static void UpdateMaterial(string mid, string material,Controller ctrl)
        {
            if (string.IsNullOrEmpty(material))
            {
                ReceiveResponsed(mid, "NA");
            }

            var sql = "update NewLoadMiniPIP set SpecailMaterial = '<SpecailMaterial>' where MiniPIPID = '<MiniPIPID>'";
            sql = sql.Replace("<MiniPIPID>", mid).Replace("<SpecailMaterial>", material.Replace("'", ""));
            DBUtility.ExeLocalSqlNoRes(sql);

            if (!string.IsNullOrEmpty(material))
            {
                var alertlist = RetrieveNewLoadMiniPIP(mid);
                SendNoticEmail(ctrl,alertlist);
            }
        }

        public static void SendNoticEmail(Controller ctrl, List<NewLoadMiniPIP> alertlist)
        {
            var alertdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var cfg = CfgUtility.GetSysConfig(ctrl);
            var tolist = cfg["SPECIALMATERIALEMAIL"].Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToList();

            StringBuilder sb = new StringBuilder(64 * (alertlist.Count + 5));
            sb.Append("('");
            foreach (var tempvm in alertlist)
            {
                sb.Append(tempvm.MiniPIPID + "','");
            }

            if (alertlist.Count > 0)
            {
                var midstr = sb.ToString();
                var midcond = midstr.Substring(0, midstr.Length - 2) + ")";

                var sql = "update NewLoadMiniPIP set AlertDate = '<AlertDate>' where MiniPIPID in <MIDCOND>";
                sql = sql.Replace("<MIDCOND>", midcond).Replace("<AlertDate>", alertdate);
                DBUtility.ExeLocalSqlNoRes(sql);

                var emailstr = "Hi Lance Chen\r\n Below is the specail material list for you to approve:\r\n";
                foreach (var item in alertlist)
                {
                    var tempdata = "";
                    tempdata = tempdata + "PN: " + item.PN + " PE: " + item.PE + " Order: " + item.OrderInfo + " Special Material: " + item.SpecailMaterial + "\r\n";
                    tempdata = tempdata + "http://wuxinpi.chn.ii-vi.net:8080/Domino/MiniPIP/ApproveMaterial?MID=" + item.MiniPIPID + "  \r\n";
                    emailstr = emailstr + tempdata;
                }

                EmailUtility.SendEmail(ctrl,"MiniPIP Special Material OA", tolist, emailstr);
                new System.Threading.ManualResetEvent(false).WaitOne(1000);
            }
        }

        public static void SendNoticEmail(Controller ctrl)
        {
            var today = DateTime.Now.ToString("yyyy-MM-dd");
            var alertlist = new List<NewLoadMiniPIP>();

            var sql = "select MiniPIPID,OrderInfo,SpecailMaterial,PN,PE,AlertDate from NewLoadMiniPIP where SpecailMaterial <> '' and Removed <> 'TRUE' and OrderInfo <> ''";
            var dbret = DBUtility.ExeLocalSqlWithRes(sql);

            foreach (var line in dbret)
            {
                var prealertdate = Convert.ToDateTime(line[5]).ToString("yyyy-MM-dd");
                if (string.Compare(today, prealertdate) != 0)
                {
                    var tempvm = new NewLoadMiniPIP();
                    tempvm.MiniPIPID = Convert.ToString(line[0]);
                    tempvm.OrderInfo = Convert.ToString(line[1]);
                    tempvm.SpecailMaterial = Convert.ToString(line[2]);
                    tempvm.PN = Convert.ToString(line[3]);
                    tempvm.PE = Convert.ToString(line[4]);

                    alertlist.Add(tempvm);
                }
            }//end foreach

            SendNoticEmail(ctrl, alertlist);
        }

        public string MiniPIPID { set; get; }
        public string OrderInfo { set; get; }
        public string SpecailMaterial { set; get; }
        public string PN { set; get; }
        public string PE { set; get; }
        public string Removed { set; get; }
        public DateTime AlertDate { set; get; }
        public DateTime ResponseDate { set; get; }
        public string ResponseMachine { set; get; }
    }
}