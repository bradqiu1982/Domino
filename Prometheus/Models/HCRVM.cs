using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Domino.Models
{
    public class HCRVM
    {

        private static string RMSpectialCh(string str)
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


        public static void LoadHCRVMData(Controller ctrl)
        {
            var HCRList = new List<HCRVM>();
            var syscfg = CfgUtility.GetSysConfig(ctrl);
            var data = DominoDataCollector.RetrieveDataFromExcel(ctrl, syscfg["HCRFOLED"], null);
            var idx = 0;
            foreach (var line in data)
            {
                if (idx == 0)
                { idx++; continue; }
                if (string.IsNullOrEmpty(line[7]))
                { continue; }

                var tempvm = new HCRVM();
                tempvm.ECONum = line[6];
                tempvm.ChangeItems = line[7];
                tempvm.DueDate = UT.O2T(line[8]).ToString("yyyy-MM-dd HH:mm:dd");
                tempvm.HCRStatus = line[9].ToUpper();

                var lastitem = HCRList.Last();

                tempvm.HCRName = line[2];
                if (string.IsNullOrEmpty(tempvm.HCRName))
                { tempvm.HCRName = lastitem.HCRName; }

                tempvm.CreateDate = line[1];
                if (string.IsNullOrEmpty(tempvm.CreateDate))
                { tempvm.CreateDate = lastitem.CreateDate; }

                tempvm.ProductAffect = line[3];
                if (string.IsNullOrEmpty(tempvm.ProductAffect))
                { tempvm.ProductAffect = lastitem.ProductAffect; }

                tempvm.PM = line[4];
                if (string.IsNullOrEmpty(tempvm.PM))
                { tempvm.PM = lastitem.PM; }

                tempvm.ECOOwner = line[5];
                if (string.IsNullOrEmpty(tempvm.ECOOwner))
                { tempvm.ECOOwner = lastitem.ECOOwner; }

                var nkey = tempvm.HCRName;
                if (nkey.Length > 30)
                { nkey = nkey.Substring(0, 30); }

                var ckey = tempvm.ChangeItems;
                if (ckey.Length > 30)
                { ckey = ckey.Substring(0, 30); }

                tempvm.HCRKey = RMSpectialCh(nkey)+"_"+RMSpectialCh(ckey);

                HCRList.Add(tempvm);
            }

            foreach (var item in HCRList)
            { item.StoreData(); }

            foreach (var item in HCRList)
            { item.SendDueDateWarningEmail(ctrl); }

        }

        public void SendDueDateWarningEmail(Controller ctrl)
        {
            var startdate = DateTime.Now.AddDays(-3);
            var enddate = DateTime.Now.AddDays(3);
            var ddate = UT.O2T(DueDate);
            if ((HCRStatus.Contains("ONGO") || HCRStatus.Contains("PEND"))
                && ddate >= startdate && ddate <= enddate)
            {
                var syscfg = CfgUtility.GetSysConfig(ctrl);
                var hcradmin = syscfg["HCRADMIN"].Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                var towholist = new List<string>(hcradmin);
                if (!string.IsNullOrEmpty(PM))
                { towholist.Add(PM.Replace(" ", ".") + "@II-VI.COM"); }
                if (!string.IsNullOrEmpty(ECOOwner))
                { towholist.Add(ECOOwner.Replace(" ", ".") + "@II-VI.COM"); }

                var table = new List<List<string>>();
                var line = new List<string>();
                line.Add("Created Date");line.Add("HCR name"); line.Add("Product"); line.Add("PM");
                line.Add("ECO Owner"); line.Add("ECO Num"); line.Add("Change Item");
                line.Add("Due Date"); line.Add("Status"); table.Add(line);
                line = new List<string>();
                line.Add(CreateDate); line.Add(HCRName); line.Add(ProductAffect); line.Add(PM);
                line.Add(ECOOwner); line.Add(ECONum); line.Add(ChangeItems);
                line.Add(DueDate); line.Add(HCRStatus); table.Add(line);
                var comment = "This HCR is close to/over its due date,please check it.<br>HCR Link:<br>" +
                    "http://wuxinpi.chn.ii-vi.net:8080/Domino/MiniPIP/ShowHCR?HCRKey=" + HCRKey;

                var content = EmailUtility.CreateTableHtml("Hi Guys", "This is a HCR Due Date Waring",comment, table);
                EmailUtility.SendEmail(ctrl, "HCR DUE DATE WARNING:" + HCRName,towholist, content);
                new System.Threading.ManualResetEvent(false).WaitOne(500);
            }
        }

        public static void SendHCRHistoryWarningEmail(string ECOKey,string PE,string HCRKey,Controller ctrl)
        {
            var hcr = GetHCRByKey(HCRKey)[0];

            var syscfg = CfgUtility.GetSysConfig(ctrl);
            var hcradmin = syscfg["HCRADMIN"].Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            var towholist = new List<string>(hcradmin);
            if (!string.IsNullOrEmpty(PE))
            { towholist.Add(PE.Replace(" ", ".") + "@II-VI.COM"); }
            if (!string.IsNullOrEmpty(hcr.PM))
            { towholist.Add(hcr.PM.Replace(" ", ".") + "@II-VI.COM"); }
            if (!string.IsNullOrEmpty(hcr.ECOOwner))
            { towholist.Add(hcr.ECOOwner.Replace(" ", ".") + "@II-VI.COM"); }


            var table = new List<List<string>>();
            var line = new List<string>();
            line.Add("Created Date"); line.Add("HCR name");line.Add("Product"); line.Add("PM");
            line.Add("ECO Owner"); line.Add("ECO Num"); line.Add("Change Item");
            line.Add("Due Date"); line.Add("Status"); table.Add(line);
            line = new List<string>();
            line.Add(hcr.CreateDate); line.Add(hcr.HCRName);line.Add(hcr.ProductAffect); line.Add(hcr.PM);
            line.Add(hcr.ECOOwner); line.Add(hcr.ECONum); line.Add(hcr.ChangeItems);
            line.Add(hcr.DueDate); line.Add(hcr.HCRStatus); table.Add(line);

            var comment = "Please make sure your current MiniPIP follow this updated HCR.<br>HCR Link:<br>"+
                "http://wuxinpi.chn.ii-vi.net:8080/Domino/MiniPIP/ShowHCR?HCRKey=" + hcr.HCRKey
                + "<br>Current MiniPIP link:<br>"
                + "http://wuxinpi.chn.ii-vi.net:8080/Domino/MiniPIP/ShowMiniPIP?ECOKey=" + ECOKey;

            var content = EmailUtility.CreateTableHtml("Hi Guys", "This is an updated HCR of current product: "+hcr.ProductAffect, comment , table);
            EmailUtility.SendEmail(ctrl, "HCR HISTORY WARNING: " + hcr.HCRName, towholist, content);
            new System.Threading.ManualResetEvent(false).WaitOne(500);
        }

        private void StoreData()
        {
            if (HasData())
            { UpdateData(); }
            else
            { InsertData(); }
        }

        private bool HasData()
        {
            var sql = "select HCRKey from HCRVM where HCRKey = @HCRKey";
            var dict = new Dictionary<string, string>();
            dict.Add("@HCRKey", HCRKey);
            var dbret = DBUtility.ExeLocalSqlWithRes(sql,dict);
            if (dbret.Count > 0)
            { return true; }

            return false;
        }

        private void UpdateData()
        {
            var sql = "update HCRVM set ECONum=@ECONum,DueDate=@DueDate,HCRStatus=@HCRStatus,ProductAffect=@ProductAffect,PM=@PM,CreateDate=@CreateDate,ECOOwner=@ECOOwner where  HCRKey = @HCRKey";
            var dict = new Dictionary<string, string>();
            dict.Add("@HCRKey", HCRKey);
            dict.Add("@ECONum", ECONum);
            dict.Add("@DueDate", DueDate);
            dict.Add("@HCRStatus", HCRStatus);
            dict.Add("@ProductAffect", ProductAffect);
            dict.Add("@PM", PM);
            dict.Add("@ECOOwner", ECOOwner);
            dict.Add("@CreateDate", CreateDate);
            DBUtility.ExeLocalSqlNoRes(sql, dict);
        }

        private void InsertData()
        {
            var sql = @"insert into HCRVM(HCRKey,HCRName,ProductAffect,PM,ECOOwner,ECONum,ChangeItems,CreateDate,DueDate,HCRStatus) 
                        values(@HCRKey,@HCRName,@ProductAffect,@PM,@ECOOwner,@ECONum,@ChangeItems,@CreateDate,@DueDate,@HCRStatus)";
            var dict = new Dictionary<string, string>();
            dict.Add("@HCRKey", HCRKey);
            dict.Add("@HCRName", HCRName);
            dict.Add("@ProductAffect", ProductAffect);
            dict.Add("@PM", PM);
            dict.Add("@ECOOwner", ECOOwner);

            dict.Add("@ECONum", ECONum);
            dict.Add("@ChangeItems", ChangeItems);
            dict.Add("@CreateDate", CreateDate);
            dict.Add("@DueDate", DueDate);
            dict.Add("@HCRStatus", HCRStatus);
            DBUtility.ExeLocalSqlNoRes(sql, dict);
        }

        public static List<HCRVM> GetAllHCR()
        {
            var ret = new List<HCRVM>();

            var sql = "select HCRKey,HCRName,ProductAffect,PM,ECOOwner,ECONum,ChangeItems,CreateDate,DueDate,HCRStatus from HCRVM order by DueDate desc";
            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            foreach (var line in dbret)
            {
                ret.Add(new HCRVM(UT.O2S(line[0]), UT.O2S(line[1]), UT.O2S(line[2]), UT.O2S(line[3])
                    , UT.O2S(line[4]), UT.O2S(line[5]), UT.O2S(line[6]), UT.O2S(line[7])
                    , UT.O2T(line[8]).ToString("yyyy-MM-dd"), UT.O2S(line[9])));
            }
            return ret;
        }

        public static List<HCRVM> GetHCRByKey(string HCRKey)
        {
            var ret = new List<HCRVM>();

            var sql = "select HCRKey,HCRName,ProductAffect,PM,ECOOwner,ECONum,ChangeItems,CreateDate,DueDate,HCRStatus from HCRVM where  HCRKey = @HCRKey";
            var dict = new Dictionary<string, string>();
            dict.Add("@HCRKey", HCRKey);
            var dbret = DBUtility.ExeLocalSqlWithRes(sql,dict);
            foreach (var line in dbret)
            {
                ret.Add(new HCRVM(UT.O2S(line[0]), UT.O2S(line[1]), UT.O2S(line[2]), UT.O2S(line[3])
                    , UT.O2S(line[4]), UT.O2S(line[5]), UT.O2S(line[6]), UT.O2S(line[7])
                    , UT.O2T(line[8]).ToString("yyyy-MM-dd"), UT.O2S(line[9])));
            }
            return ret;
        }

        public HCRVM()
        {
            HCRKey = "";
            HCRName = "";
            ProductAffect = "";
            PM = "";
            ECOOwner = "";
            ECONum = "";
            ChangeItems = "";
            CreateDate = "";
            DueDate = "";
            HCRStatus = "";
        }

        public HCRVM(string hk,string hn,string pd,string pm,string eo,string en,string ci,string cd,string dd,string hs)
        {
            HCRKey = hk;
            HCRName = hn;
            ProductAffect = pd;
            PM = pm;
            ECOOwner = eo;
            ECONum = en;
            ChangeItems = ci;
            CreateDate = cd;
            DueDate = dd;
            HCRStatus = hs;
        }

        public string HCRKey { set; get; }
        public string HCRName { set; get; }
        public string ProductAffect { set; get; }
        public string PM { set; get; }
        public string ECOOwner { set; get; }
        public string ECONum { set; get; }
        public string ChangeItems { set; get; }
        public string CreateDate { set; get; }
        public string DueDate { set; get; }
        public string HCRStatus { set; get; }
    }
}