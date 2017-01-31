using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.Mvc;
using System.Text;
using System.Diagnostics;

namespace Domino.Models
{

    public class DOMINOAGILEDOWNLOADTYPE
    {
        public static string ATTACH = "ATTACH";
        public static string ATTACHNAME = "ATTACHNAME";
        public static string WORKFLOW = "WORKFLOW";
    }

    public class DominoDataCollector
    {

        public static Dictionary<string, string> GetSysConfig(Controller ctrl)
        {
            var lines = System.IO.File.ReadAllLines(ctrl.Server.MapPath("~/Scripts/DominoCfg.txt"));
            var ret = new Dictionary<string, string>();
            foreach (var line in lines)
            {
                if (line.Contains("##"))
                {
                    continue;
                }

                if (line.Contains(":::"))
                {
                    var kvpair = line.Split(new string[] { ":::" }, StringSplitOptions.RemoveEmptyEntries);
                    ret.Add(kvpair[0].Trim(), kvpair[1].Trim());
                }
            }
            return ret;
        }

        public static void DownloadAgile(List<string> ecolist, Controller ctrl,string downloadtype)
        {
            var syscfgdict = GetSysConfig(ctrl);
            var AGILEURL = syscfgdict["AGILEURL"];
            var LOCALSITEPORT = syscfgdict["LOCALSITEPORT"];
            var SAVELOCATION = syscfgdict["SAVELOCATION"];

            var ecostr = string.Empty;
            foreach (var eco in ecolist)
            {
                ecostr = ecostr + " " + eco+" ";
            }

            var args = downloadtype+" " + AGILEURL + " " + LOCALSITEPORT + " " + SAVELOCATION + " " + ecostr;

            using (Process myprocess = new Process())
            {
                myprocess.StartInfo.FileName = Path.Combine(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath, @"Scripts\agiledownloadwraper\AgileDownload.exe").Replace("\\", "/");
                myprocess.StartInfo.Arguments = args;
                myprocess.StartInfo.CreateNoWindow = true;
                myprocess.Start();
            }
        }

        public static void UpdateECOWeeklyUpdate(Controller ctrl, ECOBaseInfo baseinfo, string cardkey)
        {
            var syscfgdict = GetSysConfig(ctrl);

            string datestring = DateTime.Now.ToString("yyyyMMdd");
            string imgdir = ctrl.Server.MapPath("~/userfiles") + "\\docs\\" + datestring + "\\";

            var desfolder = syscfgdict["WEEKLYUPDATE"];
            var sheetname = syscfgdict["MINIPIPSHEETNAME"];
            try
            {
                if (!Directory.Exists(imgdir))
                {
                    Directory.CreateDirectory(imgdir);
                }

                var pendinghistory = new List<ECOPendingUpdate>();

                if (Directory.Exists(desfolder))
                {
                    var fds = Directory.EnumerateFiles(desfolder);
                    foreach (var fd in fds)
                    {
                        try
                        {
                            var timestrs = Path.GetFileNameWithoutExtension(fd).Split(new string[] {" "},StringSplitOptions.RemoveEmptyEntries);
                            var timestamp = timestrs[timestrs.Length - 1];
                            if (timestamp.Length != 8)
                            {
                                continue;
                            }

                            var fn = imgdir + Path.GetFileName(fd);
                            System.IO.File.Copy(fd, fn, true);
                            var data = ExcelReader.RetrieveDataFromExcel(fn, sheetname);
                            foreach (var line in data)
                            {
                                if (string.Compare(line[2], baseinfo.PNDesc, true) == 0
                                    && string.Compare(DateTime.Parse(line[12]).ToString("yyyy-MM-dd"), DateTime.Parse(baseinfo.InitRevison).ToString("yyyy-MM-dd"), true) == 0)
                                {
                                    var tempinfo = new ECOPendingUpdate();
                                    tempinfo.CardKey = cardkey;
                                    tempinfo.History = line[85];
                                    tempinfo.UpdateTime = timestamp.Substring(0,4)+"-"+timestamp.Substring(4,2)+"-"+timestamp.Substring(6,2)+" 07:30:00";
                                    pendinghistory.Add(tempinfo);
                                }//end if
                            }//end foreach
                        }
                        catch (Exception ex) { }
                    }//end foreach

                    DominoVM.UpdateHistoryInfo(pendinghistory, cardkey);
                }
            }
            catch (Exception ex) { }

        }

        private static string ConvertToDate(string datestr)
        {
            if (string.IsNullOrEmpty(datestr))
            {
                return string.Empty;
            }
            try
            {
                return DateTime.Parse(datestr).ToString();
            }
            catch (Exception ex) { return string.Empty; }
        }

        private static void updateecolist(List<List<string>> data, Controller ctrl, string localdir, string urlfolder)
        {
            var syscfgdict = GetSysConfig(ctrl);
            var baseinfos = ECOBaseInfo.RetrieveAllExistECOBaseInfo();

            foreach (var line in data)
            {
                if (!string.IsNullOrEmpty(line[2])
                    && !string.IsNullOrEmpty(line[12])
                    && !string.IsNullOrEmpty(line[23])
                    && (string.Compare(line[23], "C", true) == 0
                    || string.Compare(line[23], "W", true) == 0)
                    && string.Compare(line[81], "canceled", true) != 0)
                {
                    var initialmini = string.Empty;
                    try
                    {
                        initialmini = DateTime.Parse(line[12]).ToString();
                    }
                    catch (Exception ex)
                    { initialmini = string.Empty; }

                    //if (string.IsNullOrEmpty(initialmini)
                    //    || DateTime.Parse(initialmini) > DateTime.Parse("2016-9-30 10:00:00")
                    //    || DateTime.Parse(initialmini) < DateTime.Parse("2016-9-1 10:00:00"))
                    //{
                    //    continue;
                    //}

                    var baseinfo = new ECOBaseInfo();
                    baseinfo.PNDesc = line[2];
                    baseinfo.Customer = line[5];
                    baseinfo.Complex = line[6];
                    baseinfo.RSM = line[7];
                    baseinfo.PE = line[8];
                    baseinfo.RiskBuild = line[9];

                    baseinfo.InitRevison = ConvertToDate(line[12]);
                    baseinfo.FinalRevison = ConvertToDate(line[13]);
                    baseinfo.TLAAvailable = ConvertToDate(line[14]);
                    baseinfo.OpsEntry = ConvertToDate(line[17]);
                    baseinfo.TestModification = ConvertToDate(line[18]);
                    baseinfo.ECOSubmit = ConvertToDate(line[19]);
                    baseinfo.ECOReviewSignoff = ConvertToDate(line[79]);
                    baseinfo.ECOCCBSignoff = ConvertToDate(line[80]);
                    baseinfo.ECONum = line[81];
                    baseinfo.QTRInit = line[82];

                    bool ecoexist = false;
                    foreach (var item in baseinfos)
                    {
                        if (string.Compare(item.PNDesc, baseinfo.PNDesc, true) == 0
                            && string.Compare(DateTime.Parse(item.InitRevison).ToString("yyyy-MM-dd"), DateTime.Parse(baseinfo.InitRevison).ToString("yyyy-MM-dd"), true) == 0)
                        {
                            ecoexist = true;

                            if (string.Compare(item.MiniPIPStatus, DominoMiniPIPStatus.working) != 0)
                            {
                                break;
                            }

                            item.Customer = baseinfo.Customer;
                            item.Complex = baseinfo.Complex;
                            item.RSM = baseinfo.RSM;
                            item.PE = baseinfo.PE;
                            item.RiskBuild = baseinfo.RiskBuild;

                            item.FinalRevison = baseinfo.FinalRevison;
                            item.TLAAvailable = baseinfo.TLAAvailable;
                            item.OpsEntry = baseinfo.OpsEntry;
                            item.TestModification = baseinfo.TestModification;
                            item.ECOSubmit = baseinfo.ECOSubmit;
                            item.ECOReviewSignoff = baseinfo.ECOReviewSignoff;
                            item.ECOCCBSignoff = baseinfo.ECOCCBSignoff;
                            item.ECONum = baseinfo.ECONum;
                            item.QTRInit = baseinfo.QTRInit;
                            item.UpdateECO();

                            var pendingcard = DominoVM.RetrieveSpecialCard(item, DominoCardType.ECOPending);

                            if (pendingcard.Count > 0)
                            {
                                if (!string.IsNullOrEmpty(item.ECONum)
                                && string.Compare(pendingcard[0].CardStatus, DominoCardStatus.working) == 0)
                                {
                                    DominoVM.UpdateCardStatus(pendingcard[0].CardKey, DominoCardType.ECOPending);
                                }

                                var allattach = DominoVM.RetrieveCardExistedAttachment(pendingcard[0].CardKey);

                                var customerfold = new List<string>();
                                var allcustomerfolder = Directory.EnumerateDirectories(syscfgdict["MINIPIPCUSTOMERFOLDER"]);
                                foreach (var cf in allcustomerfolder)
                                {
                                    var lastlevelfd = cf.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
                                    var lastfd = lastlevelfd[lastlevelfd.Length - 1];
                                    if (lastfd.ToUpper().Contains(baseinfo.Customer.ToUpper())
                                        || baseinfo.Customer.ToUpper().Contains(lastfd.ToUpper()))
                                    {
                                        customerfold.Add(cf);
                                    }
                                }

                                    foreach (var cf in customerfold)
                                    { 
                                        var MiniPIPDocFolder = cf + "\\" + baseinfo.PNDesc;

                                        if (Directory.Exists(MiniPIPDocFolder))
                                        {
                                            var minidocfiles = Directory.EnumerateFiles(MiniPIPDocFolder);
                                            foreach (var minifile in minidocfiles)
                                            {
                                                var fn = System.IO.Path.GetFileName(minifile);
                                                fn = fn.Replace(" ", "_").Replace("#", "")
                                                        .Replace("&", "").Replace("?", "").Replace("%", "").Replace("+", "");

                                                bool attachexist = false;
                                                foreach (var att in allattach)
                                                {
                                                    if (att.Contains(fn))
                                                    {
                                                        attachexist = true;
                                                        break;
                                                    }
                                                }//end foreach

                                                if (!attachexist)
                                                {
                                                    var desfile = localdir + fn;
                                                    try
                                                    {
                                                        System.IO.File.Copy(minifile, desfile, true);
                                                        var url = "/userfiles/docs/" + urlfolder + "/" + fn;
                                                        DominoVM.StoreCardAttachment(pendingcard[0].CardKey, url);
                                                    }
                                                    catch (Exception ex) { }
                                                }
                                            }//end foreach
                                        }//try to get mini doc file
                                }

                            }//end if

                            break;
                        }
                    }

                    if (!ecoexist)
                    {
                        baseinfo.ECOKey = DominoVM.GetUniqKey();
                        baseinfo.CreateECO();

                        var CardKey = DominoVM.GetUniqKey();

                        if (string.IsNullOrEmpty(baseinfo.ECONum))
                        {
                            DominoVM.CreateCard(baseinfo.ECOKey, CardKey, DominoCardType.ECOPending, DominoCardStatus.working);
                        }
                        else
                        {
                            DominoVM.CreateCard(baseinfo.ECOKey, CardKey, DominoCardType.ECOPending, DominoCardStatus.pending);
                        }

                        var customerfold = new List<string>();
                        var allcustomerfolder = Directory.EnumerateDirectories(syscfgdict["MINIPIPCUSTOMERFOLDER"]);
                        foreach (var cf in allcustomerfolder)
                        {
                            var lastlevelfd = cf.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
                            var lastfd = lastlevelfd[lastlevelfd.Length - 1];
                            if (lastfd.ToUpper().Contains(baseinfo.Customer.ToUpper())
                                || baseinfo.Customer.ToUpper().Contains(lastfd.ToUpper()))
                            {
                                customerfold.Add(cf);
                            }
                        }

                        foreach (var cf in customerfold)
                        {
                            var MiniPIPDocFolder = cf + "\\" + baseinfo.PNDesc;
                            if (Directory.Exists(MiniPIPDocFolder))
                            {
                                var minidocfiles = Directory.EnumerateFiles(MiniPIPDocFolder);
                                foreach (var minifile in minidocfiles)
                                {
                                    var fn = System.IO.Path.GetFileName(minifile);
                                    fn = fn.Replace(" ", "_").Replace("#", "")
                                            .Replace("&", "").Replace("?", "").Replace("%", "").Replace("+", "");
                                    var desfile = localdir + fn;

                                    try
                                    {
                                        System.IO.File.Copy(minifile, desfile, true);
                                        var url = "/userfiles/docs/" + urlfolder + "/" + fn;
                                        DominoVM.StoreCardAttachment(CardKey, url);
                                    }
                                    catch (Exception ex) { }

                                }//end foreach
                            }//try to get mini doc file
                        }

                    }

                }
            }
        }

        public static void SetForceECORefresh(Controller ctrl)
        {
            var syscfgdict = GetSysConfig(ctrl);
            string datestring = DateTime.Now.ToString("yyyyMMdd");
            string imgdir = ctrl.Server.MapPath("~/userfiles") + "\\docs\\" + datestring + "\\";
            var desfile = imgdir + syscfgdict["MINIPIPECOFILENAME"];
            if (System.IO.File.Exists(desfile))
            {
                try
                {
                    System.IO.File.Delete(desfile);
                }
                catch (Exception ex) { }
            }
        }

        public static void RefreshECOList(Controller ctrl)
        {
            var syscfgdict = GetSysConfig(ctrl);

            string datestring = DateTime.Now.ToString("yyyyMMdd");
            string imgdir = ctrl.Server.MapPath("~/userfiles") + "\\docs\\" + datestring + "\\";
            var desfile = imgdir + syscfgdict["MINIPIPECOFILENAME"];
            try
            {
                if (!Directory.Exists(imgdir) || !System.IO.File.Exists(desfile))
                {
                    if (!Directory.Exists(imgdir))
                        Directory.CreateDirectory(imgdir);

                    var srcfile = syscfgdict["MINIPIPECOFOLDER"] + "\\" + syscfgdict["MINIPIPECOFILENAME"];

                    if (System.IO.File.Exists(srcfile) && !System.IO.File.Exists(desfile))
                    {
                        System.IO.File.Copy(srcfile, desfile, true);
                    }
                }

                if (System.IO.File.Exists(desfile))
                {
                    var data = ExcelReader.RetrieveDataFromExcel(desfile, syscfgdict["MINIPIPSHEETNAME"]);
                    updateecolist(data, ctrl, imgdir, datestring);
                }
            }
            catch (Exception ex) { }
        }

        public static void RefreshECOPendingAttachInfo(Controller ctrl, string ECOKey)
        {

            var syscfgdict = GetSysConfig(ctrl);
            string datestring = DateTime.Now.ToString("yyyyMMdd");
            string imgdir = ctrl.Server.MapPath("~/userfiles") + "\\docs\\" + datestring + "\\";
            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfo(ECOKey);

            foreach (var item in baseinfos)
            {
                    if (string.Compare(item.MiniPIPStatus, DominoMiniPIPStatus.working) != 0)
                    {
                        break;
                    }

                    var pendingcard = DominoVM.RetrieveSpecialCard(item, DominoCardType.ECOPending);

                    if (pendingcard.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(item.ECONum)
                        && string.Compare(pendingcard[0].CardStatus, DominoCardStatus.working) == 0)
                        {
                            DominoVM.UpdateCardStatus(pendingcard[0].CardKey, DominoCardType.ECOPending);
                        }

                        var allattach = DominoVM.RetrieveCardExistedAttachment(pendingcard[0].CardKey);

                        var customerfold = new List<string>();
                        var allcustomerfolder = Directory.EnumerateDirectories(syscfgdict["MINIPIPCUSTOMERFOLDER"]);
                        foreach (var cf in allcustomerfolder)
                        {
                            var lastlevelfd = cf.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
                            var lastfd = lastlevelfd[lastlevelfd.Length - 1];
                            if (lastfd.ToUpper().Contains(item.Customer.ToUpper())
                                || item.Customer.ToUpper().Contains(lastfd.ToUpper()))
                            {
                                customerfold.Add(cf);
                            }
                        }

                        foreach (var cf in customerfold)
                        {
                            var MiniPIPDocFolder = cf + "\\" + item.PNDesc;

                            if (Directory.Exists(MiniPIPDocFolder))
                            {
                                var minidocfiles = Directory.EnumerateFiles(MiniPIPDocFolder);
                                foreach (var minifile in minidocfiles)
                                {
                                    var fn = System.IO.Path.GetFileName(minifile);
                                    fn = fn.Replace(" ", "_").Replace("#", "")
                                            .Replace("&", "").Replace("?", "").Replace("%", "").Replace("+", "");

                                    bool attachexist = false;
                                    foreach (var att in allattach)
                                    {
                                        if (att.Contains(fn))
                                        {
                                            attachexist = true;
                                            break;
                                        }
                                    }//end foreach

                                    if (!attachexist)
                                    {
                                        var desfile = imgdir + fn;
                                        try
                                        {
                                            System.IO.File.Copy(minifile, desfile, true);
                                            var url = "/userfiles/docs/" + datestring + "/" + fn;
                                            DominoVM.StoreCardAttachment(pendingcard[0].CardKey, url);
                                        }
                                        catch (Exception ex) { }
                                    }
                                }//end foreach
                            }//try to get mini doc file
                        }

                    }//end if

                break;
            }//end foreach

        }


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
            return sb.ToString();
        }

        private static void RefreshQAEEPROMFAI(ECOBaseInfo baseinfo, string CardKey, Controller ctrl)
        {

            var syscfgdict = GetSysConfig(ctrl);
            var srcrootfolder = syscfgdict["QAEEPROMFAI"];
            var eepromfilter = syscfgdict["QAEEPROMCHECKLISTFILTER"];

            if (Directory.Exists(srcrootfolder))
            {
                var currentcard = DominoVM.RetrieveCard(CardKey);
                if (currentcard.Count == 0)
                    return;

                var allattach = DominoVM.RetrieveCardExistedAttachment(currentcard[0].CardKey);

                var destfolderlist = new List<string>();
                var srcfolders = Directory.EnumerateDirectories(srcrootfolder);
                foreach (var fd in srcfolders)
                {
                    if (fd.ToUpper().Contains(baseinfo.PNDesc.ToUpper()))
                    {
                        destfolderlist.Add(fd);
                    }
                }//end foreach get folder contains PNDESC

                var destfiles = new List<string>();
                foreach (var fd in destfolderlist)
                {
                    var qafiles = Directory.EnumerateFiles(fd);
                    foreach (var qaf in qafiles)
                    {
                        if (Path.GetFileName(qaf).ToUpper().Contains(eepromfilter))
                        {
                            destfiles.Add(qaf);
                        }
                    }
                }

                foreach (var desf in destfiles)
                {
                    var fn = Path.GetFileName(desf);
                    fn = fn.Replace(" ", "_").Replace("#", "")
                            .Replace("&", "").Replace("?", "").Replace("%", "").Replace("+", "");

                    var pathstrs = desf.Split(Path.DirectorySeparatorChar);
                    var uplevel = pathstrs[pathstrs.Length - 2];
                    var prefix = RMSpectialCh(uplevel);

                    var attfn = prefix + "_" + fn;

                    string datestring = DateTime.Now.ToString("yyyyMMdd");
                    string imgdir = ctrl.Server.MapPath("~/userfiles") + "\\docs\\" + datestring + "\\";
                    if (!Directory.Exists(imgdir))
                        Directory.CreateDirectory(imgdir);

                    var attpath = imgdir + attfn;
                    var url = "/userfiles/docs/" + datestring + "/" + attfn;

                    var attexist = false;
                    foreach (var att in allattach)
                    {
                        if (att.Contains(attfn))
                        {
                            attexist = true;
                            break;
                        }
                    }

                    if (!attexist)
                    {
                        try
                        {
                            System.IO.File.Copy(desf, attpath, true);
                            DominoVM.StoreCardAttachment(CardKey, url);
                        }
                        catch (Exception ex) { }
                    }

                }//end foreach

            }//end if

        }

        private static void RefreshQALabelFAI(ECOBaseInfo baseinfo, string CardKey, Controller ctrl)
        {
            var syscfgdict = GetSysConfig(ctrl);
            var srcrootfolder = syscfgdict["QALABELFAI"];
            var labelfilter = syscfgdict["QALABELFILTER"];

            if (Directory.Exists(srcrootfolder))
            {
                var currentcard = DominoVM.RetrieveCard(CardKey);
                if (currentcard.Count == 0)
                    return;

                var allattach = DominoVM.RetrieveCardExistedAttachment(currentcard[0].CardKey);

                var firstleveldirs = new List<string>();
                var ffold = Directory.EnumerateDirectories(srcrootfolder);
                firstleveldirs.AddRange(ffold);

                var seconfleveldir = new List<string>();
                foreach (var ffd in ffold)
                {
                    var sfd = Directory.EnumerateDirectories(ffd);
                    seconfleveldir.AddRange(sfd);
                }

                var destfolderlist = new List<string>();
                foreach (var desf in seconfleveldir)
                {
                    if (desf.ToUpper().Contains(baseinfo.PNDesc.ToUpper()))
                    {
                        destfolderlist.Add(desf);
                    }
                }

                var destfiles = new List<string>();
                foreach (var fd in destfolderlist)
                {
                    var qafiles = Directory.EnumerateFiles(fd);
                    foreach (var qaf in qafiles)
                    {
                        if (Path.GetFileName(qaf).ToUpper().Contains(labelfilter))
                        {
                            destfiles.Add(qaf);
                        }
                    }
                }

                foreach (var desf in destfiles)
                {
                    var fn = Path.GetFileName(desf);
                    fn = fn.Replace(" ", "_").Replace("#", "")
                            .Replace("&", "").Replace("?", "").Replace("%", "").Replace("+", "");

                    var pathstrs = desf.Split(Path.DirectorySeparatorChar);
                    var uplevel = pathstrs[pathstrs.Length - 2];
                    var prefix = RMSpectialCh(uplevel);

                    var attfn = prefix + "_" + fn;

                    string datestring = DateTime.Now.ToString("yyyyMMdd");
                    string imgdir = ctrl.Server.MapPath("~/userfiles") + "\\docs\\" + datestring + "\\";
                    if (!Directory.Exists(imgdir))
                        Directory.CreateDirectory(imgdir);

                    var attpath = imgdir + attfn;
                    var url = "/userfiles/docs/" + datestring + "/" + attfn;

                    var attexist = false;
                    foreach (var att in allattach)
                    {
                        if (att.Contains(attfn))
                        {
                            attexist = true;
                            break;
                        }
                    }

                    if (!attexist)
                    {
                        try
                        {
                            System.IO.File.Copy(desf, attpath, true);
                            DominoVM.StoreCardAttachment(CardKey, url);
                        }
                        catch (Exception ex) { }
                    }
                }//end foreach

            }//end if

        }

        public static void RefreshQAFAI(ECOBaseInfo baseinfo, string CardKey, Controller ctrl)
        {
            RefreshQAEEPROMFAI(baseinfo, CardKey, ctrl);
            RefreshQALabelFAI(baseinfo, CardKey, ctrl);
        }

        private static bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }

        public static void UpdateOrderInfoFromExcel(Controller ctrl, ECOBaseInfo baseinfo, string cardkey)
        {
            var syscfgdict = GetSysConfig(ctrl);

            var currentcard = DominoVM.RetrieveCard(cardkey);

            var lineiddict = new Dictionary<string, bool>();
            string datestring = DateTime.Now.ToString("yyyyMMdd");
            string imgdir = ctrl.Server.MapPath("~/userfiles") + "\\docs\\" + datestring + "\\";

            var desfolder = syscfgdict["ORDERINFOPATH"];
            
            try
            {
                var backlogfiles = new List<string>();
                if (!Directory.Exists(imgdir))
                {
                    Directory.CreateDirectory(imgdir);
                }

                if (Directory.Exists(desfolder))
                {
                    var fds = Directory.EnumerateFiles(desfolder);
                    foreach (var fd in fds)
                    {
                        try
                        {
                            var ogfn = Path.GetFileName(fd);
                            if (ogfn.Contains("_0630"))
                            {
                                var fn = imgdir + Path.GetFileName(fd);
                                System.IO.File.Copy(fd, fn, true);
                                backlogfiles.Add(fn);
                            }
                        }
                        catch (Exception ex) { }
                    }//end foreach

                    var alldata = new List<List<string>>();
                    foreach (var bcklog in backlogfiles)
                    {
                        var data = ExcelReader.RetrieveDataFromExcel(bcklog, null);
                        alldata.AddRange(data);
                    }

                    var ordinfos = new List<MiniPIPOrdeInfo>();
                    foreach (var line in alldata)
                    {
                        var baseinfopn = string.Empty;
                        var excelpn = string.Empty;

                        var expns = line[19].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        foreach(var expn in expns)
                        {
                            if (baseinfo.PNDesc.Contains("xx"))
                            {
                                var lastidx = baseinfo.PNDesc.LastIndexOf("xx");
                                baseinfopn = baseinfo.PNDesc.Remove(lastidx, 2);

                                if (expn.Length > (lastidx + 2))
                                {
                                   excelpn = expn.Remove(lastidx, 2);
                                }
                                else
                                {
                                    excelpn = expn;
                                }
                            }
                            else
                            {
                                 baseinfopn= baseinfo.PNDesc;
                                 excelpn = expn;
                            }


                            if (excelpn.ToUpper().Contains(baseinfopn.ToUpper()))
                            {
                                if (lineiddict.ContainsKey(line[38]))
                                {
                                    break;
                                }
                                else
                                {
                                    lineiddict.Add(line[38], true);
                                }

                                var tempinfo = new MiniPIPOrdeInfo();
                                tempinfo.CardKey = cardkey;
                                tempinfo.OrderType = line[0];
                                tempinfo.OrderDate = line[1];
                                tempinfo.SONum = line[15];
                                tempinfo.Item = line[17];
                                tempinfo.Description = line[19];
                                tempinfo.QTY = line[21];
                                tempinfo.SSD = line[25];
                                tempinfo.Planner = line[27];
                                tempinfo.LineID = line[38];

                                ordinfos.Add(tempinfo);
                                break;
                            }//end if
                        }//end foreach
                    }//end foreach

                    if (ordinfos.Count > 0 && currentcard.Count > 0)
                    {
                        if (string.Compare(currentcard[0].CardStatus, DominoCardStatus.working) == 0)
                        {
                            DominoVM.UpdateCardStatus(cardkey, DominoCardStatus.pending);
                        }
                    }

                    DominoVM.UpdateOrderInfo(ordinfos, cardkey);
                }
            }
            catch (Exception ex) { }

        }

        public static void UpdateJOInfoFromExcel(Controller ctrl, ECOBaseInfo baseinfo, string cardkey)
        {
            var syscfgdict = GetSysConfig(ctrl);

            var currentcard = DominoVM.RetrieveCard(cardkey);

            string datestring = DateTime.Now.ToString("yyyyMMdd");
            string imgdir = ctrl.Server.MapPath("~/userfiles") + "\\docs\\" + datestring + "\\";
            var desfile = syscfgdict["JOORDERINFO"];
            var jolist = new List<ECOJOOrderInfo>();           

            try
            {
                var backlogfiles = new List<string>();
                if (!Directory.Exists(imgdir))
                {
                    Directory.CreateDirectory(imgdir);
                }

                if (System.IO.File.Exists(desfile))
                {
                    try
                    {
                        var fn = imgdir + Path.GetFileName(desfile);
                        System.IO.File.Copy(desfile, fn, true);
                        var data = ExcelReader.RetrieveDataFromExcel(fn, null);
                        foreach (var line in data)
                        {
                            var baseinfopn = string.Empty;
                            var excelpn = string.Empty;

                            var expns = line[1].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (var expn in expns)
                            {
                                if (baseinfo.PNDesc.Contains("xx"))
                                {
                                    var lastidx = baseinfo.PNDesc.LastIndexOf("xx");
                                    baseinfopn = baseinfo.PNDesc.Remove(lastidx, 2);

                                    if (expn.Length > (lastidx + 2))
                                    {
                                        excelpn = expn.Remove(lastidx, 2);
                                    }
                                    else
                                    {
                                        excelpn = expn;
                                    }
                                }
                                else
                                {
                                    baseinfopn = baseinfo.PNDesc;
                                    excelpn = expn;
                                }

                                if ((string.Compare(line[3], "903") == 0 || string.Compare(line[3], "919") == 0)
                                && excelpn.ToUpper().Contains(baseinfopn.ToUpper()))
                                {
                                    var tempinfo = new ECOJOOrderInfo();
                                    tempinfo.Description = line[1];
                                    tempinfo.Category = line[3];
                                    tempinfo.WipJob = line[11];
                                    tempinfo.JobStatus = line[14];
                                    tempinfo.SSTD = line[15];
                                    tempinfo.JOQTY = line[20];
                                    tempinfo.Planner = line[37];
                                    tempinfo.Creator = line[38];
                                    jolist.Add(tempinfo);
                                    break;
                                }
                            }
                        }
                    }
                    catch (Exception ex) { }


                    if (jolist.Count > 0 && currentcard.Count > 0)
                    {
                        if (string.Compare(currentcard[0].CardStatus, DominoCardStatus.working) == 0)
                        {
                            DominoVM.UpdateCardStatus(cardkey, DominoCardStatus.pending);
                        }
                    }

                    DominoVM.UpdateJOInfo(jolist, cardkey);
                }
            }
            catch (Exception ex) { }

        }

        public static void Update1STJOCheckFromExcel(Controller ctrl, ECOBaseInfo baseinfo, string cardkey)
        {
            var JoTable = DominoVM.RetrieveJOInfo(cardkey);
            if (JoTable.Count > 0)
            {
                var jodict = new Dictionary<string, bool>();
                foreach (var jo in JoTable)
                {
                    if (!jodict.ContainsKey(jo.WipJob))
                    {
                        jodict.Add(jo.WipJob, true);
                    }
                }

                var syscfgdict = GetSysConfig(ctrl);
                var currentcard = DominoVM.RetrieveCard(cardkey);

                string datestring = DateTime.Now.ToString("yyyyMMdd");
                string imgdir = ctrl.Server.MapPath("~/userfiles") + "\\docs\\" + datestring + "\\";
                var desfile = syscfgdict["JO1STCHECKINFO"];

                var jochecklist = new List<ECOJOCheck>();

                try
                {
                    if (!Directory.Exists(imgdir))
                    {
                        Directory.CreateDirectory(imgdir);
                    }

                    if (System.IO.File.Exists(desfile))
                    {
                        try
                        {
                            var fn = imgdir + Path.GetFileName(desfile);
                            System.IO.File.Copy(desfile, fn, true);

                            var data = ExcelReader.RetrieveDataFromExcel(fn, null);
                            foreach (var line in data)
                            {
                                if (jodict.ContainsKey(line[1]))
                                {
                                    var tempinfo = new ECOJOCheck();
                                    tempinfo.CardKey = cardkey;
                                    tempinfo.CheckType = DOMINOJOCHECKTYPE.ENGTYPE;
                                    tempinfo.JO = line[1];
                                    tempinfo.EEPROM = line[4];
                                    tempinfo.EEPROMDT = line[6];
                                    tempinfo.Label = line[7];
                                    tempinfo.LabelDT = line[8];
                                    tempinfo.TestData = line[9];
                                    tempinfo.TestDateDT = line[10];
                                    tempinfo.Costemic = line[11];
                                    tempinfo.CostemicDT = line[12];
                                    tempinfo.Status = line[13];
                                    jochecklist.Add(tempinfo);
                                }
                            }
                        }
                        catch (Exception ex) { }

                        if (jochecklist.Count > 0)
                        {
                            DominoVM.UpdateJOCheckInfo(jochecklist, cardkey);
                        }

                        if (jochecklist.Count > 0 && currentcard.Count > 0)
                        {
                            if (string.Compare(currentcard[0].CardStatus, DominoCardStatus.working) == 0)
                            {
                                DominoVM.UpdateCardStatus(cardkey, DominoCardStatus.pending);
                            }
                        }

                        if (jochecklist.Count == 0 && currentcard.Count > 0)
                        {
                            if (string.Compare(currentcard[0].CardStatus, DominoCardStatus.pending) == 0)
                            {
                                DominoVM.UpdateCardStatus(cardkey, DominoCardStatus.working);
                            }
                        }
                    }
                }
                catch (Exception ex) { }
            }
        }

        public static void Update2NDJOCheckFromExcel(Controller ctrl, ECOBaseInfo baseinfo, string cardkey)
        {
            var JoTable = DominoVM.RetrieveJOInfo(cardkey);
            if (JoTable.Count > 0)
            {
                var jodict = new Dictionary<string, bool>();
                foreach (var jo in JoTable)
                {
                    if (!jodict.ContainsKey(jo.WipJob))
                    {
                        jodict.Add(jo.WipJob, true);
                    }
                }

                var syscfgdict = GetSysConfig(ctrl);
                var currentcard = DominoVM.RetrieveCard(cardkey);

                string datestring = DateTime.Now.ToString("yyyyMMdd");
                string imgdir = ctrl.Server.MapPath("~/userfiles") + "\\docs\\" + datestring + "\\";
                var desfolder = syscfgdict["JO2NDCHECKINFO"];

                var jochecklist = new List<ECOJOCheck>();

                try
                {
                    if (!Directory.Exists(imgdir))
                    {
                        Directory.CreateDirectory(imgdir);
                    }

                    var fs = Directory.EnumerateFiles(desfolder);
                    var desfile = string.Empty;
                    foreach (var f in fs)
                    {
                        if (Path.GetFileName(f).Contains("FAI OQC"))
                        {
                            desfile = f;
                            break;
                        }
                    }

                    if (System.IO.File.Exists(desfile))
                    {
                        try
                        {
                            var fn = imgdir + "QA_FAI_OCQ"+Path.GetExtension(desfile);
                            System.IO.File.Copy(desfile, fn, true);

                            var data = ExcelReader.RetrieveDataFromExcel(fn, null);
                            foreach (var line in data)
                            {
                                var jos = line[9].Split(new string[] { "\r", "\n"," ","," }, StringSplitOptions.RemoveEmptyEntries);
                                foreach (var j in jos)
                                {
                                    if (jodict.ContainsKey(j.Trim()))
                                    {
                                        var tempinfo = new ECOJOCheck();
                                        tempinfo.CardKey = cardkey;
                                        tempinfo.CheckType = DOMINOJOCHECKTYPE.QATYPE;
                                        tempinfo.JO = j.Trim();
                                        tempinfo.EEPROM = line[5];
                                        tempinfo.EEPROMDT = line[0] + "-" + line[1] + "-" + line[2];
                                        jochecklist.Add(tempinfo);
                                    }
                                }

                            }
                        }
                        catch (Exception ex) { }

                        if (jochecklist.Count > 0)
                        {
                            DominoVM.UpdateJOCheckInfo(jochecklist, cardkey);
                        }

                        if (jochecklist.Count > 0 && currentcard.Count > 0)
                        {
                            if (string.Compare(currentcard[0].CardStatus, DominoCardStatus.working) == 0)
                            {
                                DominoVM.UpdateCardStatus(cardkey, DominoCardStatus.pending);
                            }
                        }

                        if (jochecklist.Count == 0 && currentcard.Count > 0)
                        {
                            if (string.Compare(currentcard[0].CardStatus, DominoCardStatus.pending) == 0)
                            {
                                DominoVM.UpdateCardStatus(cardkey, DominoCardStatus.working);
                            }
                        }
                    }
                }
                catch (Exception ex) { }
            }

        }

        public static void UpdateJOMainStoreFromExcel(Controller ctrl, ECOBaseInfo baseinfo, string cardkey)
        {
            var JoTable = DominoVM.RetrieveJOInfo(cardkey);
            if (JoTable.Count > 0)
            {
                var jodict = new Dictionary<string, bool>();
                foreach (var jo in JoTable)
                {
                    if (!jodict.ContainsKey(jo.WipJob))
                    {
                        jodict.Add(jo.WipJob, false);
                    }
                }

                var syscfgdict = GetSysConfig(ctrl);
                var currentcard = DominoVM.RetrieveCard(cardkey);

                string datestring = DateTime.Now.ToString("yyyyMMdd");
                string imgdir = ctrl.Server.MapPath("~/userfiles") + "\\docs\\" + datestring + "\\";
                var desfile = syscfgdict["JO2MAINSTORE"];

                try
                {
                    if (!Directory.Exists(imgdir))
                    {
                        Directory.CreateDirectory(imgdir);
                    }

                    if (System.IO.File.Exists(desfile))
                    {
                        try
                        {
                            var fn = imgdir + Path.GetFileName(desfile);
                            System.IO.File.Copy(desfile, fn, true);

                            var data = ExcelReader.RetrieveDataFromExcel(fn, null);
                            foreach (var line in data)
                            {
                                if (jodict.ContainsKey(line[11]))
                                {
                                    if (!line[14].ToUpper().Contains("COMPLETE"))
                                    {
                                        jodict[line[11]] = true;
                                    }
                                }
                            }


                            foreach (var kv in jodict)
                            {
                                if (kv.Value)
                                {
                                    DominoVM.UpdateJOMainStore(cardkey, kv.Key, DOMINOJOSTORESTATUS.WIP);
                                }
                                else
                                {
                                    DominoVM.UpdateJOMainStore(cardkey, kv.Key, DOMINOJOSTORESTATUS.STORED);
                                }
                            }//end foreach
                        }
                        catch (Exception ex) { }
                    }
                }
                catch (Exception ex) { }
            }
        }

        public static void UpdateShipInfoFromExcel(Controller ctrl, ECOBaseInfo baseinfo, string cardkey)
        {
            var syscfgdict = GetSysConfig(ctrl);

            var ordercard = DominoVM.RetrieveSpecialCard(baseinfo, DominoCardType.SampleOrdering);
            var orderinfos = DominoVM.RetrieveOrderInfo(ordercard[0].CardKey);
            var sodict = new Dictionary<string, bool>();
            foreach (var so in orderinfos)
            {
                if (!sodict.ContainsKey(so.SONum))
                {
                    sodict.Add(so.SONum, true);
                }
            }

            var currentcard = DominoVM.RetrieveCard(cardkey);

            string datestring = DateTime.Now.ToString("yyyyMMdd");
            string imgdir = ctrl.Server.MapPath("~/userfiles") + "\\docs\\" + datestring + "\\";

            //var desfile = syscfgdict["SHIPMENTINFOPATH"];
            //var sheetname = syscfgdict["SHIPMENTINFOSHEETNAME"];

            var desfolder = syscfgdict["SHIPMENTINFOPATH"];

            var jolist = new List<ECOShipInfo>();

            try
            {
                if (!Directory.Exists(imgdir))
                {
                    Directory.CreateDirectory(imgdir);
                }

                var fs = Directory.EnumerateFiles(desfolder);
                var desfile = string.Empty;
                foreach (var f in fs)
                {
                    if (Path.GetFileName(f).Contains("Finisar - OTS"))
                    {
                        desfile = f;
                        break;
                    }
                }
                

                if (System.IO.File.Exists(desfile))
                {
                    var sheetname = Path.GetFileNameWithoutExtension(desfile);
                    try
                    {
                        var fn = imgdir + Path.GetFileName(desfile);
                        System.IO.File.Copy(desfile, fn, true);

                        var data = ExcelReader.RetrieveDataFromExcel(fn, sheetname);
                        foreach (var line in data)
                        {
                            if (sodict.ContainsKey(line[8]))
                            {
                                var tempinfo = new ECOShipInfo();
                                tempinfo.ShipSONum = line[8];
                                tempinfo.ShipLine = line[9];
                                tempinfo.ShipOrderQTY = line[13];
                                tempinfo.ShippedQTY = line[14];
                                tempinfo.ShipDate = line[19];
                                jolist.Add(tempinfo);
                            }
                        }
                    }
                    catch (Exception ex) { }


                    if (jolist.Count > 0 && currentcard.Count > 0)
                    {
                        if (string.Compare(currentcard[0].CardStatus, DominoCardStatus.working) == 0)
                        {
                            DominoVM.UpdateCardStatus(cardkey, DominoCardStatus.pending);
                        }
                    }

                    DominoVM.UpdateShipInfo(jolist, cardkey);
                }
            }
            catch (Exception ex) { }

        }

        public static List<QACheckData> RetrieveAllQACheckInfo(Controller ctrl)
        {
            var ret = new List<QACheckData>();

            var syscfgdict = GetSysConfig(ctrl);
            var srcfile = syscfgdict["QAFACHECKCHART"];
            var sheetname = syscfgdict["QAFACHECKCHARTSHEET"];
            string datestring = DateTime.Now.ToString("yyyyMMdd");
            string imgdir = ctrl.Server.MapPath("~/userfiles") + "\\docs\\" + datestring + "\\";

            try
            {
                if (!Directory.Exists(imgdir))
                {
                    Directory.CreateDirectory(imgdir);
                }

                if (System.IO.File.Exists(srcfile))
                {
                    try
                    {
                        var fn = imgdir + Path.GetFileNameWithoutExtension(srcfile) + DateTime.Now.ToString("yyyyMMddhhmmss")+ Path.GetExtension(srcfile);
                        System.IO.File.Copy(srcfile, fn, true);
                        var data = ExcelReader.RetrieveDataFromExcel(fn, sheetname);
                        foreach (var line in data)
                        {
                            try
                            {
                                var tempinfo = new QACheckData();
                                tempinfo.QADate = DateTime.Parse(line[0] + "-" + line[1] + "-" + line[2] + " 07:30:00");
                                if (line[5].Contains("@"))
                                {
                                    tempinfo.PE = line[5].ToUpper();
                                }
                                else
                                {
                                    tempinfo.PE = (line[5].Replace(" ", ".") + "@finisar.com").ToUpper();
                                }

                                if (line[4].ToUpper().Contains("EEPROM") && line[4].ToUpper().Contains("FLI"))
                                {
                                    if (!string.IsNullOrEmpty(line[11]))
                                    {
                                        tempinfo.EEPROMPASS = Convert.ToInt32(line[11]);
                                        tempinfo.FLIPASS = Convert.ToInt32(line[11]);
                                    }
                                    if (!string.IsNullOrEmpty(line[12]))
                                    {
                                        tempinfo.EEPROMFAIL = Convert.ToInt32(line[12]);
                                        tempinfo.FLIFAIL = Convert.ToInt32(line[12]);
                                    }
                                    
                                }
                                else if (line[4].ToUpper().Contains("EEPROM"))
                                {
                                    if (!string.IsNullOrEmpty(line[11]))
                                    {
                                        tempinfo.EEPROMPASS = Convert.ToInt32(line[11]);
                                    }
                                    if (!string.IsNullOrEmpty(line[12]))
                                    {
                                        tempinfo.EEPROMFAIL = Convert.ToInt32(line[12]);
                                    }
                                }
                                else if (line[4].ToUpper().Contains("FLI"))
                                {
                                    if (!string.IsNullOrEmpty(line[11]))
                                    {
                                        tempinfo.FLIPASS = Convert.ToInt32(line[11]);
                                    }
                                    if (!string.IsNullOrEmpty(line[12]))
                                    {
                                        tempinfo.FLIFAIL = Convert.ToInt32(line[12]);
                                    }
                                }
                                ret.Add(tempinfo);
                            }
                            catch (Exception ex) { }
                        }//end foreach
                    }
                    catch (Exception ex) { }

                }
            }
            catch (Exception ex) { }

            return ret;
        }

    }
}