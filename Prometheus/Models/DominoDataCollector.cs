using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.Mvc;
using System.Text;

namespace Domino.Models
{
    public class DominoDataCollector
    {

        private static Dictionary<string, string> GetSysConfig(Controller ctrl)
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
            var baseinfos = ECOBaseInfo.RetrieveAllECOBaseInfo();

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

                    if (string.IsNullOrEmpty(initialmini)
                        || DateTime.Parse(initialmini) > DateTime.Parse("2016-9-15 10:00:00")
                        || DateTime.Parse(initialmini) < DateTime.Parse("2016-9-7 10:00:00"))
                    {
                        continue;
                    }

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

                                var MiniPIPDocFolder = syscfgdict["MINIPIPECOFOLDER"] + "\\" + baseinfo.Customer + "\\" + baseinfo.PNDesc;
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

                        var MiniPIPDocFolder = syscfgdict["MINIPIPECOFOLDER"] + "\\" + baseinfo.Customer + "\\" + baseinfo.PNDesc;
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
                        if (line[19].ToUpper().Contains(baseinfo.PNDesc))
                        {
                            if (lineiddict.ContainsKey(line[38]))
                            {
                                continue;
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
                        }//end if
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
                            if (string.Compare(line[3], "903") == 0
                                && line[1].Contains(baseinfo.PNDesc))
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


        public static void UpdateEEPROM2NDFromExcel(Controller ctrl, ECOBaseInfo baseinfo, string cardkey)
        {
            var syscfgdict = GetSysConfig(ctrl);

            var currentcard = DominoVM.RetrieveCard(cardkey);

            string datestring = DateTime.Now.ToString("yyyyMMdd");
            string imgdir = ctrl.Server.MapPath("~/userfiles") + "\\docs\\" + datestring + "\\";
            var desfile = syscfgdict["EEPROM2NDCHECK"];
            var sheetname = syscfgdict["EEPROM2NDCHECKSHEET"];

            var jolist = new List<DominoVM>();

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

                        var data = ExcelReader.RetrieveDataFromExcel(fn, sheetname);
                        foreach (var line in data)
                        {
                            if (string.Compare(line[7],baseinfo.ECONum,true) == 0
                                && string.Compare(line[4], "EEPROM", true) == 0
                                && line[8].Contains(baseinfo.PNDesc))
                            {
                                var tempinfo = new DominoVM();
                                tempinfo.BdEEPROM2NDDate = line[0] + "-" + line[1] + "-" + line[2];
                                tempinfo.BdEEPROM2NDPE = line[5];

                                if (!string.IsNullOrEmpty(line[9]))
                                {
                                    tempinfo.BdEEPROM2NDRESULT = line[9];
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(line[12]))
                                    {
                                        try
                                        {
                                            var val = Convert.ToInt32(line[12]);
                                            if (val > 0)
                                            {
                                                tempinfo.BdEEPROM2NDRESULT = "fail";
                                            }
                                            else
                                            {
                                                tempinfo.BdEEPROM2NDRESULT = "pass";
                                            }
                                        }
                                        catch (Exception ex) { tempinfo.BdEEPROM2NDRESULT = "fail"; }
                                    }
                                    else
                                    {
                                        tempinfo.BdEEPROM2NDRESULT = "fail";
                                    }
                                }
                                
                                tempinfo.UpdateBDEEPROM2ND(cardkey);
                                jolist.Add(tempinfo);
                                break;
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

                }
            }
            catch (Exception ex) { }

        }


    }
}