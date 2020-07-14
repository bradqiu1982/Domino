using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Domino.Models;
using System.Reflection;
using System.Web.Routing;
using System.IO;
using System.Diagnostics;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Net;

namespace Domino.Controllers
{
    public class MiniPIPController : Controller
    {

        private void CreateAllDefaultCards(int cardcount, ECOBaseInfo baseinfo)
        {
            if (cardcount == 0)
                return;

            var currenttime = DateTime.Now;

            if (cardcount < 2)
            {
                new System.Threading.ManualResetEvent(false).WaitOne(2000);
                currenttime = DateTime.Now;
                currenttime = currenttime.AddMinutes(1);
                DominoVM.CreateCard(baseinfo.ECOKey, DominoVM.GetUniqKey(), DominoCardType.ECOSignoff1, currenttime.ToString());
            }
            if (cardcount < 3)
            {
                new System.Threading.ManualResetEvent(false).WaitOne(2000);
                currenttime = DateTime.Now;
                currenttime = currenttime.AddMinutes(1);
                DominoVM.CreateCard(baseinfo.ECOKey, DominoVM.GetUniqKey(), DominoCardType.ECOComplete, currenttime.ToString());
            }
            if (cardcount < 4)
            {
                new System.Threading.ManualResetEvent(false).WaitOne(2000);
                currenttime = DateTime.Now;
                currenttime = currenttime.AddMinutes(1);
                DominoVM.CreateCard(baseinfo.ECOKey, DominoVM.GetUniqKey(), DominoCardType.SampleOrdering, currenttime.ToString());
            }
            if (cardcount < 5)
            {
                new System.Threading.ManualResetEvent(false).WaitOne(2000);
                currenttime = DateTime.Now;
                currenttime = currenttime.AddMinutes(1);
                DominoVM.CreateCard(baseinfo.ECOKey, DominoVM.GetUniqKey(), DominoCardType.SampleBuilding, currenttime.ToString());
            }
            if (cardcount < 6)
            {
                new System.Threading.ManualResetEvent(false).WaitOne(2000);
                currenttime = DateTime.Now;
                currenttime = currenttime.AddMinutes(1);
                DominoVM.CreateCard(baseinfo.ECOKey, DominoVM.GetUniqKey(), DominoCardType.SampleShipment, currenttime.ToString());
            }
            if (cardcount < 7)
            {
                new System.Threading.ManualResetEvent(false).WaitOne(2000);
                currenttime = DateTime.Now;
                currenttime = currenttime.AddMinutes(1);
                DominoVM.CreateCard(baseinfo.ECOKey, DominoVM.GetUniqKey(), DominoCardType.SampleCustomerApproval, currenttime.ToString());
            }
            if (cardcount < 8)
            {
                new System.Threading.ManualResetEvent(false).WaitOne(2000);
                currenttime = DateTime.Now;
                currenttime = currenttime.AddMinutes(1);
                DominoVM.CreateCard(baseinfo.ECOKey, DominoVM.GetUniqKey(), DominoCardType.MiniPIPComplete, currenttime.ToString());
            }
        }

        public static string DetermineCompName(string IP)
        {
            try
            {
                IPAddress myIP = IPAddress.Parse(IP);
                IPHostEntry GetIPHost = Dns.GetHostEntry(myIP);
                List<string> compName = GetIPHost.HostName.ToString().Split('.').ToList();
                return compName.First();
            }
            catch (Exception ex)
            { return string.Empty; }
        }

        static string GetMd5Hash(MD5 md5Hash, string input)
        {

            byte[] data = md5Hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        // GET: MiniPIPs
        public ActionResult ViewAll(string smartkey = null)
        {
            if (smartkey != null)
            {
                var smartkey1 = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(smartkey));
                if (smartkey1.Contains("::"))
                {
                    var splitstr = smartkey1.Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries);
                    var hash1 = splitstr[0];
                    var timestamp = splitstr[1];
                    MD5 md5Hash = MD5.Create();
                    var hash2 = GetMd5Hash(md5Hash, timestamp + "_joke");
                    if (hash1.Contains(hash2))
                    {
                        var now = DateTime.Now;
                        try
                        {
                            var time1 = DateTime.Parse(timestamp);
                            if (time1 > now.AddSeconds(-10))
                            {
                                //time is ok
                            }
                            else
                            {
                                return Redirect("http://wuxinpi.china.ads.finisar.com:8081/");
                            }
                        }
                        catch (Exception ex) { return Redirect("http://wuxinpi.china.ads.finisar.com:8081/"); }

                    }
                    else
                    {
                        return Redirect("http://wuxinpi.china.ads.finisar.com:8081/");
                    }
                }
                else
                {
                    return Redirect("http://wuxinpi.china.ads.finisar.com:8081/");
                }
            }
            else if (Request.Cookies["activenpidomino"] == null && smartkey == null)
            {
                string IP = Request.UserHostName;
                string compName = DetermineCompName(IP).ToUpper();
                var machinedict = CfgUtility.GetNPIMachine(this);
                if (!string.IsNullOrEmpty(compName) && !machinedict.ContainsKey(compName))
                {
                    return Redirect("http://wuxinpi.china.ads.finisar.com:8081/");
                }
            }

             //var baseinfo = new ECOBaseInfo();
            //baseinfo.ECOKey = DominoVM.GetUniqKey();
            //baseinfo.ECONum = "97807";
            //baseinfo.PNDesc = "FCBG410QB1C10-FC";
            //baseinfo.Customer = "MRV";
            //baseinfo.PE = "Jessica Zheng";
            //baseinfo.CreateECO();

            //DominoVM.CreateCard(baseinfo.ECOKey, DominoVM.GetUniqKey(), DominoCardType.ECOPending);

            //DominoVM.RefreshSystem(this);

            var ckdict = CookieUtility.UnpackCookie(this);
            if (ckdict.ContainsKey("logonuser") && !string.IsNullOrEmpty(ckdict["logonuser"]))
            {

            }
            else
            {
                var ck = new Dictionary<string, string>();
                ck.Add("logonredirectctrl", "MiniPIP");
                ck.Add("logonredirectact", "ViewAll");
                CookieUtility.SetCookie(this, ck);
                return RedirectToAction("LoginUser", "DominoUser");
            }

            var updater = GetAdminAuth();

            var DupPNDict = new Dictionary<string, List<ECOBaseInfo>>();
            ViewBag.DupPNList = "";

            var newloaddict = NewLoadMiniPIP.NewLoadPIPToShow();
            ViewBag.newloaddict = newloaddict;

            var baseinfos = ECOBaseInfo.RetrieveAllWorkingECOBaseInfo();
            var vm = new List<List<DominoVM>>();
            foreach (var item in baseinfos)
            {
                if (string.IsNullOrEmpty(item.ECONum) && !newloaddict.ContainsKey(item.ECOKey))
                    continue;

                var loginer = updater.Split(new string[] { "@" }, StringSplitOptions.RemoveEmptyEntries)[0].Replace(".","").Replace(" ", "").ToUpper();
                var pe = item.PE.Split(new string[] { "@" }, StringSplitOptions.RemoveEmptyEntries)[0].Replace(".", "").Replace(" ", "").ToUpper();
                if (string.Compare(loginer, pe) != 0 && !(ViewBag.badmin || ViewBag.demo))
                    continue;

                if (ViewBag.demo)
                {
                    if (!ViewBag.demoecodict.ContainsKey(item.ECONum))
                    {
                        continue;
                    }
                }

                if (DupPNDict.ContainsKey(item.PNDesc))
                {
                    DupPNDict[item.PNDesc].Add(item);
                }
                else
                {
                    var pntemplist = new List<ECOBaseInfo>();
                    pntemplist.Add(item);
                    DupPNDict.Add(item.PNDesc, pntemplist);
                }

                var templist = DominoVM.RetrieveECOCards(item);
                if (templist.Count > 0 && string.Compare(item.FlowInfo, DominoFlowInfo.Default) == 0)
                {
                    CreateAllDefaultCards(templist.Count, item);
                    templist = DominoVM.RetrieveECOCards(item);
                }

                if (string.Compare(item.MiniPIPStatus, DominoMiniPIPStatus.hold) == 0)
                {
                    foreach (var card in templist)
                    {
                        card.CardStatus = DominoCardStatus.working;
                    }
                }

                bool forpereview = false;
                foreach (var card in templist)
                {
                    if (string.Compare(card.CardStatus, DominoCardStatus.pending) == 0)
                    {
                        forpereview = true;
                    }
                }

                if (newloaddict.ContainsKey(item.ECOKey))
                {
                    forpereview = true;
                }

                if (forpereview)
                {
                    vm.Add(templist);
                }
            }

            foreach (var kv in DupPNDict)
            {
                if (kv.Value.Count > 1)
                {
                    ViewBag.DupPNList = ViewBag.DupPNList + " [Product Request: " + kv.Value[0].PNDesc + " PE: " + kv.Value[0].PE + " ECO NUM: ";
                    foreach (var ecoinfo in kv.Value)
                    {
                        ViewBag.DupPNList = ViewBag.DupPNList + ecoinfo.ECONum+",";
                    }
                    ViewBag.DupPNList = ViewBag.DupPNList + "] ;";
                }
            }

            var alluser = ECOBaseInfo.RetrieveAllPE();
            var asilist = new List<string>();
            asilist.Add("NONE");
            asilist.AddRange(alluser);
            ViewBag.FilterPEList = CreateSelectList(asilist, "");

            var allcustomer = ECOBaseInfo.RetrieveAllCustomer();
            asilist = new List<string>();
            asilist.Add("NONE");
            asilist.AddRange(allcustomer);
            ViewBag.CustomerList = CreateSelectList(asilist, "");

            var ecocardtypelistarray = new string[] { DominoCardType.ECOPending, DominoCardType.ECOSignoff1, DominoCardType.CustomerApprovalHold
                                                        ,DominoCardType.ECOComplete,DominoCardType.SampleOrdering,DominoCardType.SampleBuilding
                                                        ,DominoCardType.SampleShipment,DominoCardType.SampleCustomerApproval,DominoCardType.MiniPIPComplete };
            asilist = new List<string>();
            asilist.Add("NONE");
            asilist.AddRange(ecocardtypelistarray);
            ViewBag.ecocardtypelist = CreateSelectList(asilist, "");

            ViewBag.HistoryInfos = DominoUserViewModels.RetrieveUserHistory(updater);

            GetNoticeInfo();

            return View(vm);
        }

        public ActionResult AllPendingWorkingMiniPIP()
        {

            var ckdict = CookieUtility.UnpackCookie(this);
            if (ckdict.ContainsKey("logonuser") && !string.IsNullOrEmpty(ckdict["logonuser"]))
            {

            }
            else
            {
                var ck = new Dictionary<string, string>();
                ck.Add("logonredirectctrl", "MiniPIP");
                ck.Add("logonredirectact", "ViewAll");
                CookieUtility.SetCookie(this, ck);
                return RedirectToAction("LoginUser", "DominoUser");
            }

            var updater = GetAdminAuth();

            var baseinfos = ECOBaseInfo.RetrieveAllWorkingECOBaseInfo();
            var vm = new List<List<DominoVM>>();
            foreach (var item in baseinfos)
            {
                var loginer = updater.Split(new string[] { "@" }, StringSplitOptions.RemoveEmptyEntries)[0].Replace(".", "").Replace(" ", "").ToUpper();
                var pe = item.PE.Split(new string[] { "@" }, StringSplitOptions.RemoveEmptyEntries)[0].Replace(".", "").Replace(" ", "").ToUpper();
                if (string.Compare(loginer, pe) != 0 && !(ViewBag.badmin || ViewBag.demo))
                    continue;

                if (ViewBag.demo)
                {
                    if (!ViewBag.demoecodict.ContainsKey(item.ECONum))
                    {
                        continue;
                    }
                }

                var templist = DominoVM.RetrieveECOCards(item);
                if (templist.Count > 0 && string.Compare(item.FlowInfo, DominoFlowInfo.Default) == 0)
                {
                    CreateAllDefaultCards(templist.Count, item);
                    templist = DominoVM.RetrieveECOCards(item);
                }

                if (string.Compare(item.FlowInfo, DominoFlowInfo.Revise) == 0)
                {
                    if (templist.Count == 1 && string.Compare(item.ECOType, DominoECOType.RVNS) == 0)
                    {
                        CreateRVNSCards(item.ECOKey);
                        templist = DominoVM.RetrieveECOCards(item);
                    }
                    else if (templist.Count == 1 && string.Compare(item.ECOType, DominoECOType.RVS) == 0)
                    {
                        CreateRVSCards(item.ECOKey);
                        templist = DominoVM.RetrieveECOCards(item);
                    }
                }


                if (string.Compare(item.MiniPIPStatus, DominoMiniPIPStatus.hold) == 0)
                {
                    foreach (var card in templist)
                    {
                        card.CardStatus = DominoCardStatus.working;
                    }
                }

                vm.Add(templist);
            }

            var alluser = ECOBaseInfo.RetrieveAllPE();
            var asilist = new List<string>();
            asilist.Add("NONE");
            asilist.AddRange(alluser);
            ViewBag.FilterPEList = CreateSelectList(asilist, "");

            var allcustomer = ECOBaseInfo.RetrieveAllCustomer();
            asilist = new List<string>();
            asilist.Add("NONE");
            asilist.AddRange(allcustomer);
            ViewBag.CustomerList = CreateSelectList(asilist, "");

            var ecocardtypelistarray = new string[] { DominoCardType.ECOPending, DominoCardType.ECOSignoff1, DominoCardType.CustomerApprovalHold
                                                        ,DominoCardType.ECOComplete,DominoCardType.SampleOrdering,DominoCardType.SampleBuilding
                                                        ,DominoCardType.SampleShipment,DominoCardType.SampleCustomerApproval,DominoCardType.MiniPIPComplete };
            asilist = new List<string>();
            asilist.Add("NONE");
            asilist.AddRange(ecocardtypelistarray);
            ViewBag.ecocardtypelist = CreateSelectList(asilist, "");

            ViewBag.HistoryInfos = DominoUserViewModels.RetrieveUserHistory(updater);

            GetNoticeInfo();

            return View("ViewAll", vm);
        }


        public ActionResult SummaryMiniPIP(string CardType)
        {
            var updater = GetAdminAuth();

            var fn = "MiniPIP-Summary-data-" + CardType.ToUpper().Replace(" ", "_") + "-" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
            string datestring = DateTime.Now.ToString("yyyyMMdd");
            string imgdir = Server.MapPath("~/userfiles") + "\\docs\\" + datestring + "\\";
            if (!Directory.Exists(imgdir))
            {
                Directory.CreateDirectory(imgdir);
            }
            var realpath = imgdir + fn;
            var realurl = "/userfiles/docs/" + datestring + "/" + fn;
            logreportinfo(realpath, "Product requested,ECO NUM,customer,type,RSM,PE,Depart\r\n");

            var udlist = DominoUserViewModels.RetrieveAllUserDepart();
            var uddict = new Dictionary<string, string>();
            foreach (var ud in udlist)
            {
                var name = ud.UserName.Split(new string[] { "@" }, StringSplitOptions.RemoveEmptyEntries)[0].ToUpper().Replace(".", "").Replace(" ", "");
                uddict.Add(name, ud.Depart);
            }

            var baseinfos = ECOBaseInfo.RetrieveAllWorkingECOBaseInfo();
            var vm = new List<List<DominoVM>>();

            foreach (var item in baseinfos)
            {
                var depart = "";
                if (uddict.ContainsKey(item.PE.Replace(".", "").Replace(" ", "")))
                {
                    depart = uddict[item.PE.Replace(".", "").Replace(" ", "")];
                }

                var templist = DominoVM.RetrieveECOCards(item);
                if (string.Compare(item.MiniPIPStatus, DominoMiniPIPStatus.hold) == 0)
                {
                    foreach (var card in templist)
                    {
                        card.CardStatus = DominoCardStatus.info;
                    }
                }

                foreach (var card in templist)
                {
                    if (string.Compare(card.CardStatus, DominoCardStatus.pending) == 0
                        || string.Compare(card.CardStatus, DominoCardStatus.working) == 0
                        || string.Compare(card.CardStatus, DominoCardStatus.info) == 0)
                    {
                        if (string.Compare(card.CardType, DominoCardType.ECOPending) == 0)
                        {
                            if (string.IsNullOrEmpty(item.ECONum))
                            {
                                if (string.Compare(CardType, DominoCardType.ECOPending) == 0)
                                {
                                    vm.Add(templist);
                                    logreportinfo(realpath, item.PNDesc + "," + item.ECONum + "," + item.Customer+","+item.Complex + "," + item.RSM+ "," +item.PE + "," + depart + "\r\n");
                                    ViewBag.minipipsummaryurl = realurl;
                                }
                                break;
                            }
                        }

                        if (string.Compare(card.CardType, DominoCardType.ECOSignoff1) == 0
                            || string.Compare(card.CardType, DominoCardType.ECOSignoff2) == 0)
                        {
                            if (string.Compare(CardType, DominoCardType.ECOSignoff1) == 0)
                            {
                                vm.Add(templist);
                                logreportinfo(realpath, item.PNDesc + "," + item.ECONum + "," + item.Customer + "," + item.Complex + "," + item.RSM + "," + item.PE + "," + depart + "\r\n");
                                ViewBag.minipipsummaryurl = realurl;
                            }

                            break;
                        }

                        if (string.Compare(card.CardType, DominoCardType.CustomerApprovalHold) == 0)
                        {
                            DominoVM cardinfo = DominoVM.RetrieveCustomerApproveHoldInfo(card.CardKey);
                            if (!string.IsNullOrEmpty(cardinfo.ECOCustomerHoldStartDate))
                            {
                                if (string.Compare(CardType, DominoCardType.CustomerApprovalHold) == 0)
                                {
                                    vm.Add(templist);
                                    logreportinfo(realpath, item.PNDesc + "," + item.ECONum + "," + item.Customer + "," + item.Complex + "," + item.RSM + "," + item.PE + "," + depart + "\r\n");
                                    ViewBag.minipipsummaryurl = realurl;
                                }

                                break;
                            }
                        }

                        if (string.Compare(card.CardType, DominoCardType.ECOComplete) == 0)
                        {
                            if (string.Compare(CardType, DominoCardType.ECOComplete) == 0)
                            {
                                vm.Add(templist);
                                logreportinfo(realpath, item.PNDesc + "," + item.ECONum + "," + item.Customer + "," + item.Complex + "," + item.RSM + "," + item.PE + "," + depart + "\r\n");
                                ViewBag.minipipsummaryurl = realurl;
                            }
                            break;
                        }

                        if (string.Compare(card.CardType, DominoCardType.SampleOrdering) == 0)
                        {
                            if (string.Compare(CardType, DominoCardType.SampleOrdering) == 0)
                            {
                                vm.Add(templist);
                                logreportinfo(realpath, item.PNDesc + "," + item.ECONum + "," + item.Customer + "," + item.Complex + "," + item.RSM + "," + item.PE + "," + depart + "\r\n");
                                ViewBag.minipipsummaryurl = realurl;
                            }
                            break;
                        }

                        if (string.Compare(card.CardType, DominoCardType.SampleBuilding) == 0)
                        {
                            if (string.Compare(CardType, DominoCardType.SampleBuilding) == 0)
                            {
                                vm.Add(templist);
                                logreportinfo(realpath, item.PNDesc + "," + item.ECONum + "," + item.Customer + "," + item.Complex + "," + item.RSM + "," + item.PE + "," + depart + "\r\n");
                                ViewBag.minipipsummaryurl = realurl;
                            }
                            break;
                        }

                        if (string.Compare(card.CardType, DominoCardType.SampleShipment) == 0)
                        {
                            if (string.Compare(CardType, DominoCardType.SampleShipment) == 0)
                            {
                                vm.Add(templist);
                                logreportinfo(realpath, item.PNDesc + "," + item.ECONum + "," + item.Customer + "," + item.Complex + "," + item.RSM + "," + item.PE + "," + depart + "\r\n");
                                ViewBag.minipipsummaryurl = realurl;
                            }
                            break;
                        }

                        if (string.Compare(card.CardType, DominoCardType.SampleCustomerApproval) == 0)
                        {
                            if (string.Compare(CardType, DominoCardType.SampleCustomerApproval) == 0)
                            {
                                vm.Add(templist);
                                logreportinfo(realpath, item.PNDesc + "," + item.ECONum + "," + item.Customer + "," + item.Complex + "," + item.RSM + "," + item.PE + "," + depart + "\r\n");
                                ViewBag.minipipsummaryurl = realurl;
                            }
                            break;
                        }

                        if (string.Compare(card.CardType, DominoCardType.MiniPIPComplete) == 0)
                        {
                            if (string.Compare(CardType, DominoCardType.MiniPIPComplete) == 0)
                            {
                                vm.Add(templist);
                                logreportinfo(realpath, item.PNDesc + "," + item.ECONum + "," + item.Customer + "," + item.Complex + "," + item.RSM + "," + item.PE + "," + depart + "\r\n");
                                ViewBag.minipipsummaryurl = realurl;
                            }
                            break;
                        }

                        break;
                    }//end if
                }//end foreach

            }//end foreach


            var alluser = ECOBaseInfo.RetrieveAllPE();
            var asilist = new List<string>();
            asilist.Add("NONE");
            asilist.AddRange(alluser);
            ViewBag.FilterPEList = CreateSelectList(asilist, "");

            var allcustomer = ECOBaseInfo.RetrieveAllCustomer();
            asilist = new List<string>();
            asilist.Add("NONE");
            asilist.AddRange(allcustomer);
            ViewBag.CustomerList = CreateSelectList(asilist, "");

            var ecocardtypelistarray = new string[] { DominoCardType.ECOPending, DominoCardType.ECOSignoff1, DominoCardType.CustomerApprovalHold
                                                        ,DominoCardType.ECOComplete,DominoCardType.SampleOrdering,DominoCardType.SampleBuilding
                                                        ,DominoCardType.SampleShipment,DominoCardType.SampleCustomerApproval,DominoCardType.MiniPIPComplete };
            asilist = new List<string>();
            asilist.Add("NONE");
            asilist.AddRange(ecocardtypelistarray);
            ViewBag.ecocardtypelist = CreateSelectList(asilist, "");

            ViewBag.HistoryInfos = DominoUserViewModels.RetrieveUserHistory(updater);
            GetNoticeInfo();

            return View("ViewAll", vm);
        }

        public ActionResult ShowPEWkingMiniPIP(string PE)
        {
            var updater = GetAdminAuth();
            var baseinfos = ECOBaseInfo.RetrievePEWorkingECOBaseInfo(PE);
            var vm = new List<List<DominoVM>>();
            foreach (var item in baseinfos)
            {
                var templist = DominoVM.RetrieveECOCards(item);
                vm.Add(templist);
            }

            var alluser = ECOBaseInfo.RetrieveAllPE();
            var asilist = new List<string>();
            asilist.Add("NONE");
            asilist.AddRange(alluser);
            ViewBag.FilterPEList = CreateSelectList(asilist, PE);

            var allcustomer = ECOBaseInfo.RetrieveAllCustomer();
            asilist = new List<string>();
            asilist.Add("NONE");
            asilist.AddRange(allcustomer);
            ViewBag.CustomerList = CreateSelectList(asilist, "");

            var ecocardtypelistarray = new string[] { DominoCardType.ECOPending, DominoCardType.ECOSignoff1, DominoCardType.CustomerApprovalHold
                                                        ,DominoCardType.ECOComplete,DominoCardType.SampleOrdering,DominoCardType.SampleBuilding
                                                        ,DominoCardType.SampleShipment,DominoCardType.SampleCustomerApproval,DominoCardType.MiniPIPComplete };
            asilist = new List<string>();
            asilist.Add("NONE");
            asilist.AddRange(ecocardtypelistarray);
            ViewBag.ecocardtypelist = CreateSelectList(asilist, "");

            ViewBag.HistoryInfos = DominoUserViewModels.RetrieveUserHistory(updater);
            GetNoticeInfo();

            return View("ViewAll", vm);
        }

        public ActionResult ShowCustomerWorkingMiniPIP(string Customer)
        {
            var updater = GetAdminAuth();
            var baseinfos = ECOBaseInfo.RetrieveCustomerWorkingECOBaseInfo(Customer);
            var vm = new List<List<DominoVM>>();
            foreach (var item in baseinfos)
            {
                var templist = DominoVM.RetrieveECOCards(item);
                vm.Add(templist);
            }

            var alluser = ECOBaseInfo.RetrieveAllPE();
            var asilist = new List<string>();
            asilist.Add("NONE");
            asilist.AddRange(alluser);
            ViewBag.FilterPEList = CreateSelectList(asilist, "");

            var allcustomer = ECOBaseInfo.RetrieveAllCustomer();
            asilist = new List<string>();
            asilist.Add("NONE");
            asilist.AddRange(allcustomer);
            ViewBag.CustomerList = CreateSelectList(asilist, Customer);

            var ecocardtypelistarray = new string[] { DominoCardType.ECOPending, DominoCardType.ECOSignoff1, DominoCardType.CustomerApprovalHold
                                                        ,DominoCardType.ECOComplete,DominoCardType.SampleOrdering,DominoCardType.SampleBuilding
                                                        ,DominoCardType.SampleShipment,DominoCardType.SampleCustomerApproval,DominoCardType.MiniPIPComplete };
            asilist = new List<string>();
            asilist.Add("NONE");
            asilist.AddRange(ecocardtypelistarray);
            ViewBag.ecocardtypelist = CreateSelectList(asilist, "");

            ViewBag.HistoryInfos = DominoUserViewModels.RetrieveUserHistory(updater);
            GetNoticeInfo();

            return View("ViewAll", vm);
        }

        public ActionResult ShowECOMiniPIP(string ECONum)
        {
            var updater = GetAdminAuth();
            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfoWithECONum(ECONum);
            var vm = new List<List<DominoVM>>();
            foreach (var item in baseinfos)
            {
                var templist = DominoVM.RetrieveECOCards(item);
                vm.Add(templist);
            }

            var alluser = ECOBaseInfo.RetrieveAllPE();
            var asilist = new List<string>();
            asilist.Add("NONE");
            asilist.AddRange(alluser);
            ViewBag.FilterPEList = CreateSelectList(asilist, "");

            var allcustomer = ECOBaseInfo.RetrieveAllCustomer();
            asilist = new List<string>();
            asilist.Add("NONE");
            asilist.AddRange(allcustomer);
            ViewBag.CustomerList = CreateSelectList(asilist, "");

            var ecocardtypelistarray = new string[] { DominoCardType.ECOPending, DominoCardType.ECOSignoff1, DominoCardType.CustomerApprovalHold
                                                        ,DominoCardType.ECOComplete,DominoCardType.SampleOrdering,DominoCardType.SampleBuilding
                                                        ,DominoCardType.SampleShipment,DominoCardType.SampleCustomerApproval,DominoCardType.MiniPIPComplete };
            asilist = new List<string>();
            asilist.Add("NONE");
            asilist.AddRange(ecocardtypelistarray);
            ViewBag.ecocardtypelist = CreateSelectList(asilist, "");

            ViewBag.HistoryInfos = DominoUserViewModels.RetrieveUserHistory(updater);
            GetNoticeInfo();

            return View("ViewAll", vm);
        }

        public ActionResult ShowECOMiniPIPByPN(string pndesc)
        {
            var updater = GetAdminAuth();
            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfoWithECONumByPN(pndesc);
            var vm = new List<List<DominoVM>>();
            foreach (var item in baseinfos)
            {
                var templist = DominoVM.RetrieveECOCards(item);
                vm.Add(templist);
            }

            var alluser = ECOBaseInfo.RetrieveAllPE();
            var asilist = new List<string>();
            asilist.Add("NONE");
            asilist.AddRange(alluser);
            ViewBag.FilterPEList = CreateSelectList(asilist, "");

            var allcustomer = ECOBaseInfo.RetrieveAllCustomer();
            asilist = new List<string>();
            asilist.Add("NONE");
            asilist.AddRange(allcustomer);
            ViewBag.CustomerList = CreateSelectList(asilist, "");

            var ecocardtypelistarray = new string[] { DominoCardType.ECOPending, DominoCardType.ECOSignoff1, DominoCardType.CustomerApprovalHold
                                                        ,DominoCardType.ECOComplete,DominoCardType.SampleOrdering,DominoCardType.SampleBuilding
                                                        ,DominoCardType.SampleShipment,DominoCardType.SampleCustomerApproval,DominoCardType.MiniPIPComplete };
            asilist = new List<string>();
            asilist.Add("NONE");
            asilist.AddRange(ecocardtypelistarray);
            ViewBag.ecocardtypelist = CreateSelectList(asilist, "");

            ViewBag.HistoryInfos = DominoUserViewModels.RetrieveUserHistory(updater);
            GetNoticeInfo();

            return View("ViewAll", vm);
        }

        public ActionResult CompletedMiniPIP()
        {
            var updater = GetAdminAuth();

            var baseinfos = ECOBaseInfo.RetrieveAllCompletedECOBaseInfo();
            var vm = new List<List<DominoVM>>();
            foreach (var item in baseinfos)
            {
                var loginer = updater.Split(new string[] { "@" }, StringSplitOptions.RemoveEmptyEntries)[0].Replace(".", "").Replace(" ", "").ToUpper();
                var pe = item.PE.Split(new string[] { "@" }, StringSplitOptions.RemoveEmptyEntries)[0].Replace(".", "").Replace(" ", "").ToUpper();
                if (string.Compare(loginer, pe) != 0 && !(ViewBag.badmin || ViewBag.demo))
                    continue;

                if (ViewBag.demo)
                {
                    if (!ViewBag.demoecodict.ContainsKey(item.ECONum))
                    {
                        continue;
                    }
                }

                var templist = DominoVM.RetrieveECOCards(item);
                if (templist.Count > 0 && string.Compare(item.FlowInfo, DominoFlowInfo.Default) == 0)
                {
                    CreateAllDefaultCards(templist.Count, item);
                    templist = DominoVM.RetrieveECOCards(item);
                }

                vm.Add(templist);
            }

            var alluser = ECOBaseInfo.RetrieveAllPE();
            var asilist = new List<string>();
            asilist.Add("NONE");
            asilist.AddRange(alluser);
            ViewBag.FilterPEList = CreateSelectList(asilist, "");

            GetNoticeInfo();
            return View(vm);
        }

        public ActionResult ShowPECompletedMiniPIP(string PE)
        {
            GetAdminAuth();
            var baseinfos = ECOBaseInfo.RetrievePECompletedECOBaseInfo(PE);
            var vm = new List<List<DominoVM>>();
            foreach (var item in baseinfos)
            {
                var templist = DominoVM.RetrieveECOCards(item);
                vm.Add(templist);
            }

            var alluser = ECOBaseInfo.RetrieveAllPE();
            var asilist = new List<string>();
            asilist.Add("NONE");
            asilist.AddRange(alluser);
            ViewBag.FilterPEList = CreateSelectList(asilist, PE);

            return View("CompletedMiniPIP", vm);
        }



        private void logmaininfo(string info)
        {
            var dominofolder = "d:\\HeartBeat4Domino";
            if (!Directory.Exists(dominofolder))
            {
                Directory.CreateDirectory(dominofolder);
            }

            var filename = dominofolder + "\\weblog" + DateTime.Now.ToString("yyyy-MM-dd");

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

        public ActionResult RefreshSys()
        {
            string datestring = DateTime.Now.ToString("yyyyMMdd");
            string imgdir = Server.MapPath("~/userfiles") + "\\docs\\" + datestring + "\\RefreshSys";
            if(Directory.Exists(imgdir))
                return RedirectToAction("ViewAll", "MiniPIP");

            try
            {
                Directory.CreateDirectory(imgdir);
            }catch (Exception ex) { }

            try
            {
                HCRVM.LoadHCRVMData(this);
            }
            catch (Exception ex) { }

            RefreshAgileInfo();

            RefreshCardsInfo();

            NewLoadMiniPIP.SendNoticEmail(this);

            return RedirectToAction("ViewAll", "MiniPIP");
        }

        private void RefreshCardsInfo()
        {
            try
            {
                DominoDataCollector.SetForceECORefresh(this);
                DominoDataCollector.RefreshECOList(this);

                logmaininfo(DateTime.Now.ToString() + "    ECO base info is refreshed!\r\n");

                var cardtypes = new string[] { DominoCardType.ECOPending,DominoCardType.ECOSignoff1
                                                , DominoCardType.ECOSignoff2, DominoCardType.SampleOrdering
                                                ,DominoCardType.SampleBuilding,DominoCardType.SampleShipment };
                var cardtypelist = new List<string>();
                cardtypelist.AddRange(cardtypes);

                var baseinfos = ECOBaseInfo.RetrieveAllWorkingECOBaseInfo();

                foreach (var cardtype in cardtypelist)
                {
                    var cardlist = new List<DominoVM>();
                    foreach (var bs in baseinfos)
                    {
                        if (string.Compare(bs.MiniPIPStatus, DominoMiniPIPStatus.hold) == 0)
                        {
                            continue;
                        }//for hold card,we will not update info

                        var ret = DominoVM.RetrieveWorkingCard(bs, cardtype);
                        if (!string.IsNullOrEmpty(ret.CardKey))
                        {
                            cardlist.Add(ret);
                        }
                    }

                    foreach (var card in cardlist)
                    {
                        if (string.Compare(cardtype, DominoCardType.ECOPending) == 0)
                        {
                            DominoDataCollector.UpdateECOWeeklyUpdate(this, card.EBaseInfo, card.CardKey);
                            logmaininfo(DateTime.Now.ToString() + " updated ECO "+card.EBaseInfo.ECONum+"  ECOPending info\r\n");
                            DominoVM.CardCanbeUpdate(card.CardKey);
                        }
                        else if (string.Compare(cardtype, DominoCardType.ECOSignoff1) == 0)
                        {
                            DominoDataCollector.RefreshQAFAI(card.EBaseInfo, card.CardKey, this);
                            //DominoDataCollector.RefreshFormatEEPROMFile(card.EBaseInfo, card.CardKey, this);
                            logmaininfo(DateTime.Now.ToString() + " updated ECO " + card.EBaseInfo.ECONum + "  ECOSignoff1 info\r\n");
                            DominoVM.CardCanbeUpdate(card.CardKey);
                        }
                        else if (string.Compare(cardtype, DominoCardType.ECOSignoff2) == 0)
                        {
                            DominoDataCollector.RefreshQAFAI(card.EBaseInfo, card.CardKey, this);
                            //DominoDataCollector.RefreshFormatEEPROMFile(card.EBaseInfo, card.CardKey, this);
                            logmaininfo(DateTime.Now.ToString() + " updated ECO " + card.EBaseInfo.ECONum + "  ECOSignoff2 info\r\n");
                            DominoVM.CardCanbeUpdate(card.CardKey);
                        }
                        else if (string.Compare(cardtype, DominoCardType.SampleOrdering) == 0)
                        {
                            DominoDataCollector.UpdateOrderInfoFromExcel(this, card.EBaseInfo, card.CardKey);
                            logmaininfo(DateTime.Now.ToString() + " updated ECO " + card.EBaseInfo.ECONum + "  SampleOrdering info\r\n");
                            DominoVM.CardCanbeUpdate(card.CardKey);
                        }
                        else if (string.Compare(cardtype, DominoCardType.SampleBuilding) == 0)
                        {
                            //DominoDataCollector.RefreshDumpEEPROMFile(card.EBaseInfo, card.CardKey, this);

                            DominoDataCollector.UpdateJOInfoFromExcel(this, card.EBaseInfo, card.CardKey);
                            DominoDataCollector.Update1STJOCheckFromExcel(this, card.EBaseInfo, card.CardKey);
                            DominoDataCollector.Update2NDJOCheckFromExcel(this, card.EBaseInfo, card.CardKey);
                            DominoDataCollector.UpdateJOMainStoreFromExcel(this, card.EBaseInfo, card.CardKey);
                            DominoDataCollector.RefreshTnuableQAFAI(this, card.EBaseInfo, card.CardKey);

                            logmaininfo(DateTime.Now.ToString() + " updated ECO " + card.EBaseInfo.ECONum + "  SampleBuilding info\r\n");
                            DominoVM.CardCanbeUpdate(card.CardKey);
                        }
                        else if (string.Compare(cardtype, DominoCardType.SampleShipment) == 0)
                        {
                            DominoDataCollector.UpdateShipInfoFromExcel(this, card.EBaseInfo, card.CardKey);
                            logmaininfo(DateTime.Now.ToString() + " updated ECO " + card.EBaseInfo.ECONum + "  SampleShipment info\r\n");
                            DominoVM.CardCanbeUpdate(card.CardKey);
                        }
                    }//end foreach

                    if (cardlist.Count > 0)
                    {
                        logmaininfo(DateTime.Now.ToString() + "    " + cardtype + " info is refreshed!\r\n");
                    }
                }//end foreach
            }catch (Exception ex) { }
        }

        private void RefreshAgileInfo()
        {
            var ecolist = ECOBaseInfo.RetrieveECOUnCompletedBaseInfo();
            var econumlist = new List<string>();
            foreach (var eco in ecolist)
            {
                econumlist.Add(eco);
            }

            try
            {
                if (econumlist.Count > 0)
                {
                    logmaininfo(DateTime.Now.ToString() + "    " + "start refreshing agile workflow.....\r\n");
                    DominoDataCollector.DownloadAgile(econumlist, this, DOMINOAGILEDOWNLOADTYPE.WORKFLOW);
                    logmaininfo(DateTime.Now.ToString() + "    " + "agile workflow is refreshed.....\r\n");
                }
            }
            catch (Exception ex) { }

            //try
            //{
            //    logmaininfo(DateTime.Now.ToString() + "    " + "start refreshing agile attachement name.....\r\n");
            //    DominoDataCollector.DownloadAgile(econumlist, this, DOMINOAGILEDOWNLOADTYPE.ATTACHNAME);
            //    logmaininfo(DateTime.Now.ToString() + "    " + "agile attachement name is refreshed.....\r\n");
            //}
            //catch (Exception ex) { }
        }

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

        private bool LoginSystem(Dictionary<string,string> ckdict, string ECOKey, string CardKey)
        {
            if (ckdict.ContainsKey("logonuser") 
                && !string.IsNullOrEmpty(ckdict["logonuser"]))
            {
                return true;
            }
            else
            {
                var ck = new Dictionary<string, string>();
                ck.Add("logonredirectctrl", "MiniPIP");
                ck.Add("logonredirectact", "ECOPending");
                ck.Add("ECOKey", ECOKey);
                ck.Add("CardKey", CardKey);
                CookieUtility.SetCookie(this, ck);
                return false;
            }
        }

        private string GetAdminAuth()
        {
            ViewBag.badmin = false;
            var ckdict = CookieUtility.UnpackCookie(this);
            if (ckdict.ContainsKey("logonuser"))
            {
                ViewBag.badmin = DominoUserViewModels.IsAdmin(ckdict["logonuser"].Split(new char[] { '|' })[0]);
                ViewBag.demo = DominoUserViewModels.IsDemo(ckdict["logonuser"].Split(new char[] { '|' })[0]);

                var syscfgdict = DominoDataCollector.GetSysConfig(this);
                var demoecolist = syscfgdict["DEMOECONUM"].Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                var demoecodict = new Dictionary<string, bool>();
                foreach (var demo in demoecolist)
                {
                    demoecodict.Add(demo.Trim(), true);
                }
                ViewBag.demoecodict = demoecodict;

                return ckdict["logonuser"].Split(new char[] { '|' })[0];
            }

            return string.Empty;
        }

        public JsonResult UpdateNewLoadOrderInfo()
        {
            var orderinfo = Request.Form["orderinfo"];
            var ecokey = Request.Form["ecokey"];

            NewLoadMiniPIP.UpdateOrderInfo(ecokey, orderinfo);

            var ret = new JsonResult();
            ret.Data = new { success = true };
            return ret;
        }

        public JsonResult UpdateNewLoadSpecialMaterial()
        {
            var spmaterial = Request.Form["spmaterial"];
            var ecokey = Request.Form["ecokey"];

            NewLoadMiniPIP.UpdateMaterial(ecokey, spmaterial,this);

            var ret = new JsonResult();
            ret.Data = new { success = true };
            return ret;
        }

        public ActionResult ApproveMaterial(string MID)
        {
            string IP = Request.UserHostName;
            string compName = DetermineCompName(IP).ToUpper();
            NewLoadMiniPIP.ReceiveResponsed(MID, compName);
            return View();
        }

        public ActionResult ECOPending(string ECOKey, string CardKey,string Refresh="No")
        {
            var ckdict = CookieUtility.UnpackCookie(this);
            if (!LoginSystem(ckdict, ECOKey, CardKey))
            {
                return RedirectToAction("LoginUser", "DominoUser");
            }

            var updater = GetAdminAuth();

            if (string.IsNullOrEmpty(ECOKey))
                ECOKey = ckdict["ECOKey"];
            if (string.IsNullOrEmpty(CardKey))
                CardKey = ckdict["CardKey"];

            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfo(ECOKey);
            if (baseinfos.Count > 0)
            {
                var vm = new List<List<DominoVM>>();
                var cardlist = DominoVM.RetrieveECOCards(baseinfos[0]);
                vm.Add(cardlist);

                var ecoflows = new string[] { DominoFlowInfo.Default, DominoFlowInfo.Revise };
                var asilist = new List<string>();
                asilist.AddRange(ecoflows);
                ViewBag.FlowInfoList = CreateSelectList(asilist, baseinfos[0].FlowInfo);

                var pnimpls = new string[] { DominoPNImplement.NA, DominoPNImplement.Roll, DominoPNImplement.CutOverImm, DominoPNImplement.CutOverAft };
                asilist = new List<string>();
                asilist.AddRange(pnimpls);
                ViewBag.PNImplementList= CreateSelectList(asilist, baseinfos[0].PNImplement);

                var alluser = DominoUserViewModels.RetrieveAllUser();
                asilist = new List<string>();
                asilist.Add("NONE");
                foreach (var u in alluser)
                {
                    asilist.Add(u.Split(new char[] { '@' })[0].Replace(".", " ").ToUpper());
                }
                ViewBag.ActualPEList = CreateSelectList(asilist, baseinfos[0].PE.ToUpper());

                foreach (var card in cardlist)
                {
                    if (string.Compare(baseinfos[0].MiniPIPStatus, DominoMiniPIPStatus.hold) == 0)
                    {
                        card.CardStatus = DominoCardStatus.info;
                    }

                    if (string.Compare(card.CardType, DominoCardType.ECOPending) == 0)
                    {
                        ViewBag.CurrentCard = card;
                        break;
                    }
                }

                DominoVM cardinfo = DominoVM.RetrieveECOPendingInfo(ViewBag.CurrentCard.CardKey);
                if (string.IsNullOrEmpty(cardinfo.CardKey))
                {
                    DominoVM.UpdateECOPendingHoldInfo(ViewBag.CurrentCard.CardKey, DominoYESNO.NO);
                }

                var currentcard = DominoVM.RetrieveSpecialCard(baseinfos[0], DominoCardType.ECOPending);
                if (string.Compare(currentcard[0].CardStatus, DominoCardStatus.done, true) != 0
                    || string.Compare(Refresh, "YES", true) == 0)
                {
                    if (DominoVM.CardCanbeUpdate(CardKey) || string.Compare(Refresh, "YES", true) == 0)
                    {
                        DominoDataCollector.UpdateECOWeeklyUpdate(this, baseinfos[0], CardKey);
                        DominoDataCollector.RefreshECOPendingAttachInfo(this, baseinfos[0].ECOKey);
                    }
                }

                ViewBag.CurrentCard = DominoVM.RetrieveSpecialCard(baseinfos[0], DominoCardType.ECOPending)[0];

                cardinfo = DominoVM.RetrieveECOPendingInfo(CardKey);
                ViewBag.CurrentCard.MiniPIPHold = cardinfo.MiniPIPHold;

                var historys = DominoVM.RetrievePendingHistoryInfo(CardKey);
                ViewBag.CurrentCard.PendingHistoryTable = historys;

                var pipholds = new string[] { DominoYESNO.NO,DominoYESNO.YES};
                asilist = new List<string>();
                asilist.AddRange(pipholds);
                ViewBag.MiniPIPHoldList = CreateSelectList(asilist, cardinfo.MiniPIPHold);

                ViewBag.ECOKey = ECOKey;
                ViewBag.CardKey = CardKey;
                ViewBag.CardDetailPage = DominoCardType.ECOPending;

                if (!string.IsNullOrEmpty(updater))
                {
                    DominoUserViewModels.UpdateUserHistory(updater, baseinfos[0].ECONum, currentcard[0].CardType, currentcard[0].CardKey);
                }

                if (string.IsNullOrEmpty(baseinfos[0].ECONum))
                {
                    ViewBag.PendingDays = "0";
                    if (!string.IsNullOrEmpty(baseinfos[0].FinalRevison))
                    {
                        ViewBag.PendingDays = (DateTime.Now - DateTime.Parse(baseinfos[0].FinalRevison)).Days.ToString();
                    }
                    else
                    {
                        ViewBag.PendingDays = (DateTime.Now - DateTime.Parse(baseinfos[0].InitRevison)).Days.ToString();
                    }
                    
                    DominoVM.UpdateECOPendingPendingDays(ViewBag.CurrentCard.CardKey, ViewBag.PendingDays);
                }
                else
                {
                    if (string.IsNullOrEmpty(cardinfo.ECOPendingDays))
                    {
                        ViewBag.PendingDays = "0";
                        DominoVM.UpdateECOPendingPendingDays(ViewBag.CurrentCard.CardKey, "0");
                    }
                    else
                    {
                        ViewBag.PendingDays = cardinfo.ECOPendingDays;
                    }
                }

                ViewBag.NewLoadNeedOrderInfo = false;
                var newloadshowdict = NewLoadMiniPIP.NewLoadNeedOrderInfo();
                if (newloadshowdict.ContainsKey(ECOKey))
                {
                    ViewBag.NewLoadNeedOrderInfo = true;
                }

                var newloadmodaldict = NewLoadMiniPIP.NewLoadPIPModalToShow();
                ViewBag.shownewloadmodal = false;
                if (newloadmodaldict.ContainsKey(ECOKey))
                {
                    ViewBag.shownewloadmodal = true;
                }
                GetNoticeInfo();
                return View("CurrentECO", vm);
            }

            return RedirectToAction("ViewAll", "MiniPIP");
        }

        private static bool IsDigitsOnly(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }

            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }


        public void SetNoticeInfo(string noticinfo)
        {
            var dict = new Dictionary<string, string>();
            dict.Add("DominoNoticeInfo", noticinfo);
            CookieUtility.SetCookie(this, dict);
        }

        public void GetNoticeInfo()
        {
            var ckdict = CookieUtility.UnpackCookie(this);
            if (ckdict.ContainsKey("DominoNoticeInfo"))
            {
                if (!string.IsNullOrEmpty(ckdict["DominoNoticeInfo"]))
                {
                    ViewBag.DominoNoticeInfo = ckdict["DominoNoticeInfo"];
                    SetNoticeInfo(string.Empty);
                }
                else
                {
                    ViewBag.DominoNoticeInfo = null;
                }
            }
            else
            {
                ViewBag.DominoNoticeInfo = null;
            }
        }

        [HttpPost, ActionName("ECOPending")]
        [ValidateAntiForgeryToken]
        public ActionResult ECOPendingPost()
        {
            var ckdict = CookieUtility.UnpackCookie(this);
            var updater = ckdict["logonuser"].Split(new char[] { '|' })[0];

            var ECOKey = Request.Form["ECOKey"];
            var CardKey = Request.Form["CardKey"];
            
            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfo(ECOKey);
            if (baseinfos.Count > 0)
            {
                if (IsDigitsOnly(Request.Form["OrderInfo"].Trim()))
                {
                    baseinfos[0].FirstArticleNeed = Request.Form["OrderInfo"].Trim();
                }
                else
                {
                    baseinfos[0].FirstArticleNeed = "N/A";
                }

                baseinfos[0].FlowInfo = Request.Form["FlowInfoList"].ToString();

                if (IsDigitsOnly(baseinfos[0].FirstArticleNeed)
                    && string.Compare(baseinfos[0].FlowInfo, DominoFlowInfo.Default, true) == 0)
                {
                    if (!string.IsNullOrEmpty(baseinfos[0].ECOType) 
                        && string.Compare(baseinfos[0].ECOType, DominoECOType.DVS,true) != 0)
                    {
                        DominoVM.RollBack2NextCard(ECOKey, CardKey);
                    }
                    baseinfos[0].ECOType = DominoECOType.DVS;
                }
                else if (IsDigitsOnly(baseinfos[0].FirstArticleNeed)
                    && string.Compare(baseinfos[0].FlowInfo, DominoFlowInfo.Revise, true) == 0)
                {
                    if (!string.IsNullOrEmpty(baseinfos[0].ECOType)
                        && string.Compare(baseinfos[0].ECOType, DominoECOType.RVS, true) != 0)
                    {
                        DominoVM.RollBack2NextCard(ECOKey, CardKey);
                    }
                    baseinfos[0].ECOType = DominoECOType.RVS;
                }
                else if (!IsDigitsOnly(baseinfos[0].FirstArticleNeed)
                    && string.Compare(baseinfos[0].FlowInfo, DominoFlowInfo.Default, true) == 0)
                {
                    if (!string.IsNullOrEmpty(baseinfos[0].ECOType)
                        && string.Compare(baseinfos[0].ECOType, DominoECOType.DVNS, true) != 0)
                    {
                        DominoVM.RollBack2NextCard(ECOKey, CardKey);
                    }
                    baseinfos[0].ECOType = DominoECOType.DVNS;
                }
                else if (!IsDigitsOnly(baseinfos[0].FirstArticleNeed)
                    && string.Compare(baseinfos[0].FlowInfo, DominoFlowInfo.Revise, true) == 0)
                {
                    if (!string.IsNullOrEmpty(baseinfos[0].ECOType)
                        && string.Compare(baseinfos[0].ECOType, DominoECOType.RVNS, true) != 0)
                    {
                        DominoVM.RollBack2NextCard(ECOKey, CardKey);
                    }
                    baseinfos[0].ECOType = DominoECOType.RVNS;
                }

                baseinfos[0].MCOIssued = Request.Form["MCOIssued"];
                baseinfos[0].PNImplement = Request.Form["PNImplementList"].ToString();
                //baseinfos[0].FACustomerApproval = Request.Form["FACustomerApproval"];
                var ecohold = Request.Form["MiniPIPHoldList"].ToString();
                if (string.Compare(ecohold, DominoYESNO.YES) == 0)
                {
                    baseinfos[0].MiniPIPStatus = DominoMiniPIPStatus.hold;
                    if (DateTime.Parse(baseinfos[0].ECOHoldStartDate).ToString("yyyy-MM-dd").Contains("1982-05-06")
                        && DateTime.Parse(baseinfos[0].ECOHoldEndDate).ToString("yyyy-MM-dd").Contains("1982-05-06"))
                    {
                        baseinfos[0].ECOHoldStartDate = DateTime.Now.ToString("yyyy-MM-dd") + " 07:30:00";
                    }
                }
                else
                {
                    baseinfos[0].MiniPIPStatus = DominoMiniPIPStatus.working;
                    if (!DateTime.Parse(baseinfos[0].ECOHoldStartDate).ToString("yyyy-MM-dd").Contains("1982-05-06")
                        && DateTime.Parse(baseinfos[0].ECOHoldEndDate).ToString("yyyy-MM-dd").Contains("1982-05-06"))
                    {
                        baseinfos[0].ECOHoldEndDate = DateTime.Now.ToString("yyyy-MM-dd") + " 07:30:00";
                    }
                }

                if (!string.IsNullOrEmpty(Request.Form["ActualPEList"]))
                {
                    if (string.Compare(Request.Form["ActualPEList"].ToString(), "NONE") != 0)
                    {
                        baseinfos[0].ActualPE = Request.Form["ActualPEList"].ToString();
                    }
                }

                baseinfos[0].UpdateECO();
                DominoVM.UpdateECOPendingHoldInfo(CardKey, ecohold);
                
                StoreAttachAndComment(CardKey, updater);

                if (string.Compare(Request.Form["actionname"], "commitinfo", true) == 0)
                {
                    var redict = new RouteValueDictionary();
                    redict.Add("CardKey", CardKey);
                    return RedirectToAction("GoBackToCardByCardKey", "MiniPIP", redict);
                }


                if (string.Compare(Request.Form["actionname"],"forcecard",true) == 0)
                {
                    if (string.IsNullOrEmpty(baseinfos[0].ECONum))
                    {
                        var dict = new RouteValueDictionary();
                        dict.Add("ECOKey", ECOKey);
                        dict.Add("CardKey", CardKey);
                        SetNoticeInfo("ECO Number is not ready");

                        return RedirectToAction(DominoCardType.ECOPending, "MiniPIP", dict);
                    }

                    if (string.Compare(ecohold, DominoYESNO.YES) == 0)
                    {
                        var dict = new RouteValueDictionary();
                        dict.Add("ECOKey", ECOKey);
                        dict.Add("CardKey", CardKey);
                        SetNoticeInfo("ECO Status is on hold");

                        return RedirectToAction(DominoCardType.ECOPending, "MiniPIP", dict);
                    }
                }


                DominoVM.UpdateCardStatus(CardKey, DominoCardStatus.done);

                var newcardkey = DominoVM.GetUniqKey();

                if (string.Compare(baseinfos[0].ECOType, DominoECOType.DVS) == 0
                    || string.Compare(baseinfos[0].ECOType, DominoECOType.DVNS) == 0)
                {
                    new System.Threading.ManualResetEvent(false).WaitOne(2000);
                    var currenttime = DateTime.Now;

                    currenttime = currenttime.AddMinutes(1);
                    var realcardkey = DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.ECOSignoff1, currenttime.ToString());
                    var dict = new RouteValueDictionary();
                    dict.Add("ECOKey", ECOKey);
                    dict.Add("CardKey", realcardkey);

                    currenttime = currenttime.AddMinutes(1);
                    DominoVM.CreateCard(ECOKey, DominoVM.GetUniqKey(), DominoCardType.ECOComplete, currenttime.ToString());

                    currenttime = currenttime.AddMinutes(1);
                    DominoVM.CreateCard(ECOKey, DominoVM.GetUniqKey(), DominoCardType.SampleOrdering, currenttime.ToString());

                    currenttime = currenttime.AddMinutes(1);
                    DominoVM.CreateCard(ECOKey, DominoVM.GetUniqKey(), DominoCardType.SampleBuilding, currenttime.ToString());

                    currenttime = currenttime.AddMinutes(1);
                    DominoVM.CreateCard(ECOKey, DominoVM.GetUniqKey(), DominoCardType.SampleShipment, currenttime.ToString());

                    currenttime = currenttime.AddMinutes(1);
                    DominoVM.CreateCard(ECOKey, DominoVM.GetUniqKey(), DominoCardType.SampleCustomerApproval, currenttime.ToString());

                    currenttime = currenttime.AddMinutes(1);
                    DominoVM.CreateCard(ECOKey, DominoVM.GetUniqKey(), DominoCardType.MiniPIPComplete, currenttime.ToString());

                    return RedirectToAction(DominoCardType.ECOSignoff1, "MiniPIP", dict);
                }
                else if (string.Compare(baseinfos[0].ECOType, DominoECOType.RVS) == 0)
                {
                    new System.Threading.ManualResetEvent(false).WaitOne(2000);
                    var currenttime = DateTime.Now;

                    currenttime = currenttime.AddMinutes(1);
                    var realcardkey = DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.ECOSignoff2, currenttime.ToString());
                    var dict = new RouteValueDictionary();
                    dict.Add("ECOKey", ECOKey);
                    dict.Add("CardKey", realcardkey);

                    currenttime = currenttime.AddMinutes(1);
                    DominoVM.CreateCard(ECOKey, DominoVM.GetUniqKey(), DominoCardType.CustomerApprovalHold, currenttime.ToString());

                    currenttime = currenttime.AddMinutes(1);
                    DominoVM.CreateCard(ECOKey, DominoVM.GetUniqKey(), DominoCardType.SampleOrdering, currenttime.ToString());

                    currenttime = currenttime.AddMinutes(1);
                    DominoVM.CreateCard(ECOKey, DominoVM.GetUniqKey(), DominoCardType.SampleBuilding, currenttime.ToString());

                    currenttime = currenttime.AddMinutes(1);
                    DominoVM.CreateCard(ECOKey, DominoVM.GetUniqKey(), DominoCardType.SampleShipment, currenttime.ToString());

                    currenttime = currenttime.AddMinutes(1);
                    DominoVM.CreateCard(ECOKey, DominoVM.GetUniqKey(), DominoCardType.SampleCustomerApproval, currenttime.ToString());

                    currenttime = currenttime.AddMinutes(1);
                    DominoVM.CreateCard(ECOKey, DominoVM.GetUniqKey(), DominoCardType.ECOComplete, currenttime.ToString());

                    currenttime = currenttime.AddMinutes(1);
                    DominoVM.CreateCard(ECOKey, DominoVM.GetUniqKey(), DominoCardType.MiniPIPComplete, currenttime.ToString());

                    return RedirectToAction(DominoCardType.ECOSignoff2, "MiniPIP", dict);
                }
                else if (string.Compare(baseinfos[0].ECOType, DominoECOType.RVNS) == 0)
                {
                    new System.Threading.ManualResetEvent(false).WaitOne(2000);
                    var currenttime = DateTime.Now;

                    currenttime = currenttime.AddMinutes(1);

                    var realcardkey = DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.ECOSignoff2, currenttime.ToString());
                    var dict = new RouteValueDictionary();
                    dict.Add("ECOKey", ECOKey);
                    dict.Add("CardKey", realcardkey);

                    currenttime = currenttime.AddMinutes(1);
                    DominoVM.CreateCard(ECOKey, DominoVM.GetUniqKey(), DominoCardType.CustomerApprovalHold, currenttime.ToString());

                    currenttime = currenttime.AddMinutes(1);
                    DominoVM.CreateCard(ECOKey, DominoVM.GetUniqKey(), DominoCardType.ECOComplete, currenttime.ToString());

                    currenttime = currenttime.AddMinutes(1);
                    DominoVM.CreateCard(ECOKey, DominoVM.GetUniqKey(), DominoCardType.SampleOrdering, currenttime.ToString());

                    currenttime = currenttime.AddMinutes(1);
                    DominoVM.CreateCard(ECOKey, DominoVM.GetUniqKey(), DominoCardType.SampleBuilding, currenttime.ToString());

                    currenttime = currenttime.AddMinutes(1);
                    DominoVM.CreateCard(ECOKey, DominoVM.GetUniqKey(), DominoCardType.SampleShipment, currenttime.ToString());

                    currenttime = currenttime.AddMinutes(1);
                    DominoVM.CreateCard(ECOKey, DominoVM.GetUniqKey(), DominoCardType.SampleCustomerApproval, currenttime.ToString());

                    currenttime = currenttime.AddMinutes(1);
                    DominoVM.CreateCard(ECOKey, DominoVM.GetUniqKey(), DominoCardType.MiniPIPComplete, currenttime.ToString());

                    return RedirectToAction(DominoCardType.ECOSignoff2, "MiniPIP", dict);
                }
                //else
                //{
                //    new System.Threading.ManualResetEvent(false).WaitOne(1000);
                //    var currenttime = DateTime.Now;
                //    currenttime = currenttime.AddMinutes(1);

                //    var realcardkey = DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.ECOSignoff1,currenttime.ToString());
                //    var dict = new RouteValueDictionary();
                //    dict.Add("ECOKey", ECOKey);
                //    dict.Add("CardKey", realcardkey);
                //    return RedirectToAction(DominoCardType.ECOSignoff1, "MiniPIP", dict);
                //}
                var dict1 = new RouteValueDictionary();
                dict1.Add("ECOKey", ECOKey);
                dict1.Add("CardKey", CardKey);
                return RedirectToAction(DominoCardType.ECOPending, "MiniPIP", dict1);
            }
            else
            {
                var dict = new RouteValueDictionary();
                dict.Add("ECOKey", ECOKey);
                dict.Add("CardKey", CardKey);
                return RedirectToAction(DominoCardType.ECOPending, "MiniPIP", dict);
            }
        }

        public ActionResult ECOSignoff1(string ECOKey, string CardKey, string Refresh = "No")
        {
            var ckdict = CookieUtility.UnpackCookie(this);
            if (!LoginSystem(ckdict, ECOKey, CardKey))
            {
                return RedirectToAction("LoginUser", "DominoUser");
            }

            var updater = GetAdminAuth();

            if (string.IsNullOrEmpty(ECOKey))
                ECOKey = ckdict["ECOKey"];
            if (string.IsNullOrEmpty(CardKey))
                CardKey = ckdict["CardKey"];

            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfo(ECOKey);
            if (baseinfos.Count > 0)
            {
                    var currentcard = DominoVM.RetrieveSpecialCard(baseinfos[0], DominoCardType.ECOSignoff1);
                    if (string.Compare(currentcard[0].CardStatus, DominoCardStatus.done, true) != 0
                        || string.Compare(Refresh, "YES", true) == 0)
                    {
                        if (DominoVM.CardCanbeUpdate(CardKey) || string.Compare(Refresh, "YES", true) == 0)
                        {
                            DominoDataCollector.RefreshQAFAI(baseinfos[0], CardKey, this);
                            //DominoDataCollector.RefreshFormatEEPROMFile(baseinfos[0], CardKey, this);
                    }
                    }//if card is not finished,we refresh qa folder to get files


                bool eepromattachexist = false;
                bool labelattachexist = false;

                var vm = new List<List<DominoVM>>();
                var cardlist = DominoVM.RetrieveECOCards(baseinfos[0]);
                vm.Add(cardlist);

                foreach (var card in cardlist)
                {
                    if (string.Compare(card.CardType, DominoCardType.ECOSignoff1) == 0)
                    {
                        ViewBag.CurrentCard = card;
                        foreach (var attach in card.AttachList)
                        {
                            if (attach.ToUpper().Contains("EEPROM"))
                            {
                                eepromattachexist = true;
                            }
                            if (attach.ToUpper().Contains("_FAI_"))
                            {
                                labelattachexist = true;
                            }
                        }
                        break;
                    }
                }

                DominoVM cardinfo = DominoVM.RetrieveSignoffInfo(ViewBag.CurrentCard.CardKey);
                ViewBag.CurrentCard.QAEEPROMCheck = cardinfo.QAEEPROMCheck;
                ViewBag.CurrentCard.QALabelCheck = cardinfo.QALabelCheck;
                ViewBag.CurrentCard.PeerReviewEngineer = cardinfo.PeerReviewEngineer;
                ViewBag.CurrentCard.PeerReview = cardinfo.PeerReview;
                ViewBag.CurrentCard.ECOAttachmentCheck = cardinfo.ECOAttachmentCheck;
                ViewBag.CurrentCard.ECOQRFile = cardinfo.ECOQRFile;
                ViewBag.CurrentCard.EEPROMPeerReview = cardinfo.EEPROMPeerReview;
                ViewBag.CurrentCard.ECOTraceview = cardinfo.ECOTraceview;
                ViewBag.CurrentCard.SpecCompresuite = cardinfo.SpecCompresuite;
                ViewBag.CurrentCard.ECOTRApprover = cardinfo.ECOTRApprover;
                ViewBag.CurrentCard.ECOMDApprover = cardinfo.ECOMDApprover;
                ViewBag.CurrentCard.MiniPVTCheck = cardinfo.MiniPVTCheck;
                ViewBag.CurrentCard.AgileCodeFile = cardinfo.AgileCodeFile;
                ViewBag.CurrentCard.AgileSpecFile = cardinfo.AgileSpecFile;
                ViewBag.CurrentCard.AgileTestFile = cardinfo.AgileTestFile;
                ViewBag.CurrentCard.FACategory = cardinfo.FACategory;
                ViewBag.CurrentCard.RSMSendDate = cardinfo.RSMSendDate;
                ViewBag.CurrentCard.RSMApproveDate = cardinfo.RSMApproveDate;
                ViewBag.CurrentCard.EEPROMFormatFile = cardinfo.EEPROMFormatFile;
                ViewBag.CurrentCard.EEPROMExcelTemplate = cardinfo.EEPROMExcelTemplate;
                ViewBag.CurrentCard.EEPROMMaskFile = cardinfo.EEPROMMaskFile;
                ViewBag.CurrentCard.CustomerApproveFile = cardinfo.CustomerApproveFile;


                if (!string.IsNullOrEmpty(cardinfo.RSMSendDate))
                {
                    try
                    {
                        ViewBag.CurrentCard.RSMSendDate = DateTime.Parse(cardinfo.RSMSendDate).ToString("yyyy-MM-dd");
                    }
                    catch (Exception ex) { }
                }

                if (!string.IsNullOrEmpty(cardinfo.RSMApproveDate))
                {
                    try
                    {
                        ViewBag.CurrentCard.RSMApproveDate = DateTime.Parse(cardinfo.RSMApproveDate).ToString("yyyy-MM-dd");
                    }
                    catch (Exception ex) { }
                }

                var yesno = new string[] { DominoYESNO.NO, DominoYESNO.YES };

                var asilist = new List<string>();
                asilist.Add("N/A");
                asilist.AddRange(yesno);

                if (string.IsNullOrEmpty(cardinfo.QAEEPROMCheck) && eepromattachexist)
                {
                    ViewBag.QAEEPROMCheckList = CreateSelectList(asilist,DominoYESNO.YES);
                }
                else
                {
                    ViewBag.QAEEPROMCheckList = CreateSelectList(asilist, cardinfo.QAEEPROMCheck);
                }
                
              
                asilist = new List<string>();
                asilist.Add("N/A");
                asilist.AddRange(yesno);

                if (string.IsNullOrEmpty(cardinfo.QALabelCheck) && labelattachexist)
                {
                    ViewBag.QALabelCheckList = CreateSelectList(asilist, DominoYESNO.YES);
                }
                else
                {
                    ViewBag.QALabelCheckList = CreateSelectList(asilist, cardinfo.QALabelCheck);
                }

                var alluser = DominoUserViewModels.RetrieveAllUser();
                asilist = new List<string>();
                asilist.Add("NONE");
                asilist.AddRange(alluser);
                ViewBag.PeerReviewEngineerList = CreateSelectList(asilist, cardinfo.PeerReviewEngineer);

                asilist = new List<string>();
                asilist.AddRange(yesno);
                ViewBag.PeerReviewList = CreateSelectList(asilist, cardinfo.PeerReview);

                asilist = new List<string>();
                asilist.Add("N/A");
                asilist.AddRange(yesno);
                ViewBag.ECOAttachmentCheckList = CreateSelectList(asilist, cardinfo.ECOAttachmentCheck);


                asilist = new List<string>();
                asilist.Add("N/A");
                asilist.AddRange(yesno);
                ViewBag.MiniPVTCheckList = CreateSelectList(asilist, cardinfo.MiniPVTCheck);

                var facats = new string[] {"N/A",DominoFACategory.EEPROMFA, DominoFACategory.LABELFA, DominoFACategory.LABELEEPROMFA };
                asilist = new List<string>();
                asilist.AddRange(facats);
                ViewBag.FACategoryList = CreateSelectList(asilist, cardinfo.FACategory);

                ViewBag.ECOKey = ECOKey;
                ViewBag.CardKey = CardKey;
                ViewBag.CardDetailPage = DominoCardType.ECOSignoff1;

                if (!string.IsNullOrEmpty(updater))
                {
                    DominoUserViewModels.UpdateUserHistory(updater, baseinfos[0].ECONum, currentcard[0].CardType, currentcard[0].CardKey);
                }

                GetNoticeInfo();
                return View("CurrentECO", vm);
            }

            return RedirectToAction("ViewAll", "MiniPIP");

        }

        [HttpPost, ActionName("ECOSignoff1")]
        [ValidateAntiForgeryToken]
        public ActionResult ECOSignoff1Post()
        {
            var ckdict = CookieUtility.UnpackCookie(this);
            var updater = ckdict["logonuser"].Split(new char[] { '|' })[0];

            var ECOKey = Request.Form["ECOKey"];
            var CardKey = Request.Form["CardKey"];

            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfo(ECOKey);
            if (baseinfos.Count > 0)
            {
                
                var cardinfo = DominoVM.RetrieveSignoffInfo(CardKey);

                cardinfo.QAEEPROMCheck = Request.Form["QAEEPROMCheckList"].ToString();
                cardinfo.QALabelCheck = Request.Form["QALabelCheckList"].ToString();
                cardinfo.PeerReviewEngineer = Request.Form["PeerReviewEngineerList"].ToString();
                cardinfo.PeerReview = Request.Form["PeerReviewList"].ToString();
                cardinfo.ECOAttachmentCheck = Request.Form["ECOAttachmentCheckList"].ToString();
                cardinfo.MiniPVTCheck = Request.Form["MiniPVTCheckList"].ToString();
                cardinfo.FACategory = Request.Form["FACategoryList"].ToString();

                cardinfo.ECOTRApprover = Request.Form["ECOTRApprover"];
                cardinfo.ECOMDApprover = Request.Form["ECOMDApprover"];

                cardinfo.RSMSendDate = Request.Form["RSMSendDate"];
                cardinfo.RSMApproveDate = Request.Form["RSMApproveDate"];

                if (!string.IsNullOrEmpty(cardinfo.RSMApproveDate))
                {
                    baseinfos[0].FACustomerApproval = cardinfo.RSMApproveDate;
                    baseinfos[0].UpdateECO();
                }
                
                StoreAttachAndComment(CardKey, updater, cardinfo);

                cardinfo.UpdateSignoffInfo(CardKey);

                if (Request.Form["commitinfo"] != null)
                {
                    var redict = new RouteValueDictionary();
                    redict.Add("CardKey", CardKey);
                    return RedirectToAction("GoBackToCardByCardKey", "MiniPIP", redict);
                }

                if (Request.Form["forcecard"] == null)
                {
                    var allchecked = true;
                    if (string.Compare(cardinfo.QAEEPROMCheck, DominoYESNO.NO) == 0)
                    {
                        SetNoticeInfo("QA EEPROM is not checked");
                        allchecked = false;
                    }
                    else if (string.Compare(cardinfo.QALabelCheck, DominoYESNO.NO) == 0)
                    {
                        SetNoticeInfo("QA Label is not checked");
                        allchecked = false;
                    }
                    else if (string.Compare(cardinfo.PeerReview, DominoYESNO.NO) == 0)
                    {
                        SetNoticeInfo("Peer Review is not finish");
                        allchecked = false;
                    }
                    else if (string.Compare(cardinfo.ECOAttachmentCheck, DominoYESNO.NO) == 0)
                    {
                        SetNoticeInfo("ECO Attachement is not checked");
                        allchecked = false;
                    }
                    else if (string.Compare(cardinfo.MiniPVTCheck, DominoYESNO.NO) == 0)
                    {
                        SetNoticeInfo("Mini PVT is not checked");
                        allchecked = false;
                    }

                    if (!allchecked)
                    {
                        var dict1 = new RouteValueDictionary();
                        dict1.Add("ECOKey", ECOKey);
                        dict1.Add("CardKey", CardKey);
                        return RedirectToAction(DominoCardType.ECOSignoff1, "MiniPIP", dict1);
                    }
                }

                DominoVM.UpdateCardStatus(CardKey, DominoCardStatus.done);

                var newcardkey = DominoVM.GetUniqKey();

                new System.Threading.ManualResetEvent(false).WaitOne(2000);
                var currenttime = DateTime.Now;
                currenttime = currenttime.AddMinutes(1);
                var realcardkey = DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.ECOComplete,currenttime.ToString());
                var dict = new RouteValueDictionary();
                dict.Add("ECOKey", ECOKey);
                dict.Add("CardKey", realcardkey);
                return RedirectToAction(DominoCardType.ECOComplete, "MiniPIP", dict);
            }
            else
            {
                var dict = new RouteValueDictionary();
                dict.Add("ECOKey", ECOKey);
                dict.Add("CardKey", CardKey);
                return RedirectToAction(DominoCardType.ECOSignoff1, "MiniPIP", dict);
            }
}


        public ActionResult ECOComplete(string ECOKey, string CardKey, string Refresh = "No")
        {
            var ckdict = CookieUtility.UnpackCookie(this);
            if (!LoginSystem(ckdict, ECOKey, CardKey))
            {
                return RedirectToAction("LoginUser", "DominoUser");
            }

            var updater = GetAdminAuth();

            if (string.IsNullOrEmpty(ECOKey))
                ECOKey = ckdict["ECOKey"];
            if (string.IsNullOrEmpty(CardKey))
                CardKey = ckdict["CardKey"];

            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfo(ECOKey);
            if (baseinfos.Count > 0)
            {
                var vm = new List<List<DominoVM>>();
                var cardlist = DominoVM.RetrieveECOCards(baseinfos[0]);
                vm.Add(cardlist);

                foreach (var card in cardlist)
                {
                    if (string.Compare(card.CardType, DominoCardType.ECOComplete) == 0)
                    {
                        ViewBag.CurrentCard = card;
                        break;
                    }
                }

                DominoVM cardinfo = DominoVM.RetrieveECOCompleteInfo(ViewBag.CurrentCard.CardKey);
                ViewBag.CurrentCard.ECOCompleted = cardinfo.ECOCompleted;
                ViewBag.CurrentCard.ECOCompleteDate = cardinfo.ECOCompleteDate;


                if (!string.IsNullOrEmpty(cardinfo.ECOCompleteDate))
                {
                    try
                    {
                        ViewBag.CurrentCard.ECOCompleteDate = DateTime.Parse(cardinfo.ECOCompleteDate).ToString("yyyy-MM-dd");
                    }
                    catch (Exception ex) { }
                }

                var yesno = new string[] { DominoYESNO.NO, DominoYESNO.YES };
                var asilist = new List<string>();
                asilist.AddRange(yesno);
                ViewBag.ECOCompletedList = CreateSelectList(asilist, cardinfo.ECOCompleted);

                ViewBag.ECOKey = ECOKey;
                ViewBag.CardKey = CardKey;
                ViewBag.CardDetailPage = DominoCardType.ECOComplete;

                if (!string.IsNullOrEmpty(updater))
                {
                    DominoUserViewModels.UpdateUserHistory(updater, baseinfos[0].ECONum, ViewBag.CurrentCard.CardType, ViewBag.CurrentCard.CardKey);
                }

                GetNoticeInfo();
                return View("CurrentECO", vm);
            }

            return RedirectToAction("ViewAll", "MiniPIP");

        }

        private static string ConvertToDate(string obj)
        {
            try
            {
                if (string.IsNullOrEmpty(obj.Trim()))
                {
                    return string.Empty;
                }

                var date = DateTime.Parse(Convert.ToString(obj));
                return date.ToString("yyyy-MM-dd");
            }
            catch (Exception ex) { return string.Empty; }
        }

        private static string ConvertUSLocalToDate(string obj)
        {
            try
            {
                if (string.IsNullOrEmpty(obj.Trim()))
                {
                    return string.Empty;
                }
                CultureInfo culture = CultureInfo.GetCultureInfo("en-US");
                var date = DateTime.ParseExact(obj.Trim().Replace("CST", "-6"), "ddd MMM dd HH:mm:ss z yyyy", culture);
                return date.ToString("yyyy-MM-dd hh:mm:ss");
            }
            catch (Exception ex) {
                return string.Empty;
            }
        }


        [HttpPost, ActionName("ECOComplete")]
        [ValidateAntiForgeryToken]
        public ActionResult ECOCompletePost()
        {
            var ckdict = CookieUtility.UnpackCookie(this);
            var updater = ckdict["logonuser"].Split(new char[] { '|' })[0];

            var ECOKey = Request.Form["ECOKey"];
            var CardKey = Request.Form["CardKey"];

            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfo(ECOKey);
            if (baseinfos.Count > 0)
            {
                DominoVM cardinfo = DominoVM.RetrieveECOCompleteInfo(CardKey);
                cardinfo.ECOCompleted = Request.Form["ECOCompletedList"].ToString();
                cardinfo.ECOCompleteDate = ConvertToDate(Request.Form["ECOCompleteDate"]);
                cardinfo.UpdateECOCompleteInfo(CardKey);

                StoreAttachAndComment(CardKey, updater);

                if (Request.Form["commitinfo"] != null)
                {
                    var redict = new RouteValueDictionary();
                    redict.Add("CardKey", CardKey);
                    return RedirectToAction("GoBackToCardByCardKey", "MiniPIP", redict);
                }

                if (Request.Form["forcecard"] == null)
                {
                    var allchecked = true;
                    if (string.Compare(cardinfo.ECOCompleted, DominoYESNO.NO) == 0)
                    {
                        SetNoticeInfo("ECO should be completed");
                        allchecked = false;
                    }
                    else if (string.IsNullOrEmpty(cardinfo.ECOCompleteDate))
                    {
                        SetNoticeInfo("ECO Complete Date is needed");
                        allchecked = false;
                    }

                    if (!allchecked)
                    {
                        var dict1 = new RouteValueDictionary();
                        dict1.Add("ECOKey", ECOKey);
                        dict1.Add("CardKey", CardKey);
                        return RedirectToAction(DominoCardType.ECOComplete, "MiniPIP", dict1);
                    }
                }

                DominoVM.UpdateCardStatus(CardKey, DominoCardStatus.done);

                new System.Threading.ManualResetEvent(false).WaitOne(2000);
                var currenttime = DateTime.Now;
                currenttime = currenttime.AddMinutes(1);

                var newcardkey = DominoVM.GetUniqKey();
                if (string.Compare(baseinfos[0].ECOType, DominoECOType.DVS) == 0
                    || string.Compare(baseinfos[0].ECOType, DominoECOType.RVNS) == 0
                    || string.Compare(baseinfos[0].ECOType, DominoECOType.DVNS) == 0)
                {
                    var realcardkey = DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.SampleOrdering, currenttime.ToString());
                    var dict = new RouteValueDictionary();
                    dict.Add("ECOKey", ECOKey);
                    dict.Add("CardKey", realcardkey);
                    return RedirectToAction(DominoCardType.SampleOrdering, "MiniPIP", dict);
                }
                else if (string.Compare(baseinfos[0].ECOType, DominoECOType.RVS) == 0)
                {
                    var realcardkey = DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.MiniPIPComplete, currenttime.ToString());
                    var dict = new RouteValueDictionary();
                    dict.Add("ECOKey", ECOKey);
                    dict.Add("CardKey", realcardkey);
                    return RedirectToAction(DominoCardType.MiniPIPComplete, "MiniPIP", dict);
                }
                else
                {
                    var realcardkey = DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.SampleOrdering, currenttime.ToString());
                    var dict = new RouteValueDictionary();
                    dict.Add("ECOKey", ECOKey);
                    dict.Add("CardKey", realcardkey);
                    return RedirectToAction(DominoCardType.SampleOrdering, "MiniPIP", dict);
                }
            }
            else
            {
                var dict = new RouteValueDictionary();
                dict.Add("ECOKey", ECOKey);
                dict.Add("CardKey", CardKey);
                return RedirectToAction(DominoCardType.ECOComplete, "MiniPIP", dict);
            }
        }


        public ActionResult ECOSignoff2(string ECOKey, string CardKey, string Refresh = "No")
        {
            var ckdict = CookieUtility.UnpackCookie(this);
            if (!LoginSystem(ckdict, ECOKey, CardKey))
            {
                return RedirectToAction("LoginUser", "DominoUser");
            }

            var updater = GetAdminAuth();

            if (string.IsNullOrEmpty(ECOKey))
                ECOKey = ckdict["ECOKey"];
            if (string.IsNullOrEmpty(CardKey))
                CardKey = ckdict["CardKey"];

            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfo(ECOKey);
            if (baseinfos.Count > 0)
            {
                var currentcard = DominoVM.RetrieveSpecialCard(baseinfos[0], DominoCardType.ECOSignoff2);
                if (string.Compare(currentcard[0].CardStatus, DominoCardStatus.done, true) != 0
                    || string.Compare(Refresh, "YES", true) == 0)
                {
                    if (DominoVM.CardCanbeUpdate(CardKey) || string.Compare(Refresh, "YES", true) == 0)
                    {
                        DominoDataCollector.RefreshQAFAI(baseinfos[0], CardKey, this);
                        //DominoDataCollector.RefreshFormatEEPROMFile(baseinfos[0], CardKey, this);
                    }
                }//if card is not finished,we refresh qa folder to get files
            
                var vm = new List<List<DominoVM>>();
                var cardlist = DominoVM.RetrieveECOCards(baseinfos[0]);
                vm.Add(cardlist);

                bool eepromattachexist = false;
                bool labelattachexist = false;

                foreach (var card in cardlist)
                {
                    if (string.Compare(card.CardType, DominoCardType.ECOSignoff2) == 0)
                    {
                        ViewBag.CurrentCard = card;
                        foreach (var attach in card.AttachList)
                        {
                            if (attach.ToUpper().Contains("EEPROM"))
                            {
                                eepromattachexist = true;
                            }
                            if (attach.ToUpper().Contains("_FAI_"))
                            {
                                labelattachexist = true;
                            }
                        }
                        break;
                    }
                }

                DominoVM cardinfo = DominoVM.RetrieveSignoffInfo(ViewBag.CurrentCard.CardKey);
                ViewBag.CurrentCard.QAEEPROMCheck = cardinfo.QAEEPROMCheck;
                ViewBag.CurrentCard.QALabelCheck = cardinfo.QALabelCheck;
                ViewBag.CurrentCard.PeerReviewEngineer = cardinfo.PeerReviewEngineer;
                ViewBag.CurrentCard.PeerReview = cardinfo.PeerReview;
                ViewBag.CurrentCard.ECOAttachmentCheck = cardinfo.ECOAttachmentCheck;
                ViewBag.CurrentCard.ECOQRFile = cardinfo.ECOQRFile;
                ViewBag.CurrentCard.EEPROMPeerReview = cardinfo.EEPROMPeerReview;
                ViewBag.CurrentCard.ECOTraceview = cardinfo.ECOTraceview;
                ViewBag.CurrentCard.SpecCompresuite = cardinfo.SpecCompresuite;
                ViewBag.CurrentCard.ECOTRApprover = cardinfo.ECOTRApprover;
                ViewBag.CurrentCard.ECOMDApprover = cardinfo.ECOMDApprover;
                ViewBag.CurrentCard.MiniPVTCheck = cardinfo.MiniPVTCheck;
                ViewBag.CurrentCard.AgileCodeFile = cardinfo.AgileCodeFile;
                ViewBag.CurrentCard.AgileSpecFile = cardinfo.AgileSpecFile;
                ViewBag.CurrentCard.AgileTestFile = cardinfo.AgileTestFile;
                ViewBag.CurrentCard.FACategory = cardinfo.FACategory;
                ViewBag.CurrentCard.RSMSendDate = cardinfo.RSMSendDate;
                ViewBag.CurrentCard.RSMApproveDate = cardinfo.RSMApproveDate;
                ViewBag.CurrentCard.ECOCustomerHoldDate = cardinfo.ECOCustomerHoldDate;
                ViewBag.CurrentCard.EEPROMFormatFile = cardinfo.EEPROMFormatFile;
                ViewBag.CurrentCard.EEPROMExcelTemplate = cardinfo.EEPROMExcelTemplate;
                ViewBag.CurrentCard.EEPROMMaskFile = cardinfo.EEPROMMaskFile;
                ViewBag.CurrentCard.CustomerApproveFile = cardinfo.CustomerApproveFile;

                if (!string.IsNullOrEmpty(cardinfo.RSMSendDate))
                {
                    try
                    {
                        ViewBag.CurrentCard.RSMSendDate = DateTime.Parse(cardinfo.RSMSendDate).ToString("yyyy-MM-dd");
                    }
                    catch (Exception ex) { }
                }

                if (!string.IsNullOrEmpty(cardinfo.RSMApproveDate))
                {
                    try
                    {
                        ViewBag.CurrentCard.RSMApproveDate = DateTime.Parse(cardinfo.RSMApproveDate).ToString("yyyy-MM-dd");
                    }
                    catch (Exception ex) { }
                }

                if (!string.IsNullOrEmpty(cardinfo.ECOCustomerHoldDate))
                {
                    try
                    {
                        ViewBag.CurrentCard.ECOCustomerHoldDate = DateTime.Parse(cardinfo.ECOCustomerHoldDate).ToString("yyyy-MM-dd");
                    }
                    catch (Exception ex) { }
                }

                var yesno = new string[] { DominoYESNO.NO, DominoYESNO.YES };

                var asilist = new List<string>();
                asilist.Add("N/A");
                asilist.AddRange(yesno);

                if (string.IsNullOrEmpty(cardinfo.QAEEPROMCheck) && eepromattachexist)
                {
                    ViewBag.QAEEPROMCheckList = CreateSelectList(asilist, DominoYESNO.YES);
                }
                else
                {
                    ViewBag.QAEEPROMCheckList = CreateSelectList(asilist, cardinfo.QAEEPROMCheck);
                }

                asilist = new List<string>();
                asilist.Add("N/A");
                asilist.AddRange(yesno);

                if (string.IsNullOrEmpty(cardinfo.QALabelCheck) && labelattachexist)
                {
                    ViewBag.QALabelCheckList = CreateSelectList(asilist, DominoYESNO.YES);
                }
                else
                {
                    ViewBag.QALabelCheckList = CreateSelectList(asilist, cardinfo.QALabelCheck);
                }

                var alluser = DominoUserViewModels.RetrieveAllUser();
                asilist = new List<string>();
                asilist.Add("NONE");
                asilist.AddRange(alluser);
                ViewBag.PeerReviewEngineerList = CreateSelectList(asilist, cardinfo.PeerReviewEngineer);

                asilist = new List<string>();
                asilist.AddRange(yesno);
                ViewBag.PeerReviewList = CreateSelectList(asilist, cardinfo.PeerReview);

                asilist = new List<string>();
                asilist.Add("N/A");
                asilist.AddRange(yesno);
                ViewBag.ECOAttachmentCheckList = CreateSelectList(asilist, cardinfo.ECOAttachmentCheck);


                asilist = new List<string>();
                asilist.Add("N/A");
                asilist.AddRange(yesno);
                ViewBag.MiniPVTCheckList = CreateSelectList(asilist, cardinfo.MiniPVTCheck);

                var facats = new string[] { "N/A",DominoFACategory.EEPROMFA, DominoFACategory.LABELFA, DominoFACategory.LABELEEPROMFA };
                asilist = new List<string>();
                asilist.AddRange(facats);
                ViewBag.FACategoryList = CreateSelectList(asilist, cardinfo.FACategory);

                ViewBag.ECOKey = ECOKey;
                ViewBag.CardKey = CardKey;
                ViewBag.CardDetailPage = DominoCardType.ECOSignoff2;

                if (!string.IsNullOrEmpty(updater))
                {
                    DominoUserViewModels.UpdateUserHistory(updater, baseinfos[0].ECONum, currentcard[0].CardType, currentcard[0].CardKey);
                }
                GetNoticeInfo();
                return View("CurrentECO", vm);
            }

            return RedirectToAction("ViewAll", "MiniPIP");

        }

        [HttpPost, ActionName("ECOSignoff2")]
        [ValidateAntiForgeryToken]
        public ActionResult ECOSignoff2Post()
        {
            var ckdict = CookieUtility.UnpackCookie(this);
            var updater = ckdict["logonuser"].Split(new char[] { '|' })[0];

            var ECOKey = Request.Form["ECOKey"];
            var CardKey = Request.Form["CardKey"];

            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfo(ECOKey);
            if (baseinfos.Count > 0)
            {

                var cardinfo = DominoVM.RetrieveSignoffInfo(CardKey);

                cardinfo.QAEEPROMCheck = Request.Form["QAEEPROMCheckList"].ToString();
                cardinfo.QALabelCheck = Request.Form["QALabelCheckList"].ToString();
                cardinfo.PeerReviewEngineer = Request.Form["PeerReviewEngineerList"].ToString();
                cardinfo.PeerReview = Request.Form["PeerReviewList"].ToString();
                cardinfo.ECOAttachmentCheck = Request.Form["ECOAttachmentCheckList"].ToString();
                cardinfo.MiniPVTCheck = Request.Form["MiniPVTCheckList"].ToString();
                cardinfo.FACategory = Request.Form["FACategoryList"].ToString();

                cardinfo.ECOTRApprover = Request.Form["ECOTRApprover"];
                cardinfo.ECOMDApprover = Request.Form["ECOMDApprover"];

                cardinfo.RSMSendDate = Request.Form["RSMSendDate"];
                cardinfo.RSMApproveDate = Request.Form["RSMApproveDate"];

                if (!string.IsNullOrEmpty(cardinfo.RSMApproveDate))
                {
                    baseinfos[0].FACustomerApproval = cardinfo.RSMApproveDate;
                    baseinfos[0].UpdateECO();
                }

                cardinfo.ECOCustomerHoldDate = Request.Form["ECOCustomerHoldDate"];
                
                StoreAttachAndComment(CardKey, updater, cardinfo);

                cardinfo.UpdateSignoffInfo(CardKey);

                if (Request.Form["commitinfo"] != null)
                {
                    var redict = new RouteValueDictionary();
                    redict.Add("CardKey", CardKey);
                    return RedirectToAction("GoBackToCardByCardKey", "MiniPIP", redict);
                }

                if (Request.Form["forcecard"] == null)
                {
                    var allchecked = true;
                    if (string.Compare(cardinfo.QAEEPROMCheck, DominoYESNO.NO) == 0)
                    {
                        SetNoticeInfo("QA EEPROM is not checked");
                        allchecked = false;
                    }
                    else if (string.Compare(cardinfo.QALabelCheck, DominoYESNO.NO) == 0)
                    {
                        SetNoticeInfo("QA Label is not checked");
                        allchecked = false;
                    }
                    else if (string.Compare(cardinfo.PeerReview, DominoYESNO.NO) == 0)
                    {
                        SetNoticeInfo("Peer Review is not finish");
                        allchecked = false;
                    }
                    else if (string.Compare(cardinfo.ECOAttachmentCheck, DominoYESNO.NO) == 0)
                    {
                        SetNoticeInfo("ECO Attachement is not checked");
                        allchecked = false;
                    }
                    else if (string.Compare(cardinfo.MiniPVTCheck, DominoYESNO.NO) == 0)
                    {
                        SetNoticeInfo("Mini PVT is not checked");
                        allchecked = false;
                    }
                    //else if (string.IsNullOrEmpty(cardinfo.ECOCustomerHoldDate))
                    //{
                    //    SetNoticeInfo("ECO Customer Hold Date need to be inputed");
                    //    allchecked = false;
                    //}

                    if (!allchecked)
                    {
                        var dict1 = new RouteValueDictionary();
                        dict1.Add("ECOKey", ECOKey);
                        dict1.Add("CardKey", CardKey);
                        return RedirectToAction(DominoCardType.ECOSignoff2, "MiniPIP", dict1);
                    }
                }

                DominoVM.UpdateCardStatus(CardKey, DominoCardStatus.done);

                var newcardkey = DominoVM.GetUniqKey();
                new System.Threading.ManualResetEvent(false).WaitOne(2000);
                var currenttime = DateTime.Now;
                currenttime = currenttime.AddMinutes(1);

                var realcardkey = DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.CustomerApprovalHold, currenttime.ToString());

                var dict = new RouteValueDictionary();
                dict.Add("ECOKey", ECOKey);
                dict.Add("CardKey", realcardkey);
                return RedirectToAction(DominoCardType.CustomerApprovalHold, "MiniPIP", dict);
            }
            else
            {
                var dict = new RouteValueDictionary();
                dict.Add("ECOKey", ECOKey);
                dict.Add("CardKey", CardKey);
                return RedirectToAction(DominoCardType.ECOSignoff2, "MiniPIP", dict);
            }
        }


        public ActionResult CustomerApprovalHold(string ECOKey, string CardKey, string Refresh = "No")
        {
            var ckdict = CookieUtility.UnpackCookie(this);
            if (!LoginSystem(ckdict, ECOKey, CardKey))
            {
                return RedirectToAction("LoginUser", "DominoUser");
            }

            var updater = GetAdminAuth();

            if (string.IsNullOrEmpty(ECOKey))
                ECOKey = ckdict["ECOKey"];
            if (string.IsNullOrEmpty(CardKey))
                CardKey = ckdict["CardKey"];

            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfo(ECOKey);
            if (baseinfos.Count > 0)
            {
                var vm = new List<List<DominoVM>>();
                var cardlist = DominoVM.RetrieveECOCards(baseinfos[0]);
                vm.Add(cardlist);

                foreach (var card in cardlist)
                {
                    if (string.Compare(card.CardType, DominoCardType.CustomerApprovalHold) == 0)
                    {
                        ViewBag.CurrentCard = card;
                        break;
                    }
                }

                DominoVM cardinfo = DominoVM.RetrieveCustomerApproveHoldInfo(ViewBag.CurrentCard.CardKey);
                ViewBag.CurrentCard.ECOCustomerApproveDate = cardinfo.ECOCustomerApproveDate;
                ViewBag.CurrentCard.ECOCustomerHoldStartDate = cardinfo.ECOCustomerHoldStartDate;
                ViewBag.CurrentCard.ECOCustomerHoldAging = cardinfo.ECOCustomerHoldAging;

                if (!string.IsNullOrEmpty(cardinfo.ECOCustomerApproveDate))
                {
                    try
                    {
                        ViewBag.CurrentCard.ECOCustomerApproveDate = DateTime.Parse(cardinfo.ECOCustomerApproveDate).ToString("yyyy-MM-dd");
                    }
                    catch (Exception ex) { }
                }

                if (!string.IsNullOrEmpty(cardinfo.ECOCustomerHoldStartDate))
                {
                    try
                    {
                        ViewBag.CurrentCard.ECOCustomerHoldStartDate = DateTime.Parse(cardinfo.ECOCustomerHoldStartDate).ToString("yyyy-MM-dd");
                    }
                    catch (Exception ex) { }
                }
                else
                {
                    //try to get customeholddate from signoff2 card to retrieve previous stored data
                    var signoffcard = DominoVM.RetrieveSpecialCard(baseinfos[0], DominoCardType.ECOSignoff2);
                    if (signoffcard.Count > 0)
                    {
                        var signoffinfo = DominoVM.RetrieveSignoffInfo(signoffcard[0].CardKey);
                        if (!string.IsNullOrEmpty(signoffinfo.ECOCustomerHoldDate))
                        {
                            try
                            {
                                ViewBag.CurrentCard.ECOCustomerHoldStartDate = DateTime.Parse(signoffinfo.ECOCustomerHoldDate).ToString("yyyy-MM-dd");
                                cardinfo.ECOCustomerHoldStartDate = signoffinfo.ECOCustomerHoldDate;
                                cardinfo.UpdateCustomerApproveHoldStartDate(ViewBag.CurrentCard.CardKey);
                            }
                            catch (Exception ex) { }
                        }
                    }
                }

                if (string.Compare(ViewBag.CurrentCard.CardStatus,DominoCardStatus.done,true) != 0)
                {
                    if (!string.IsNullOrEmpty(cardinfo.ECOCustomerHoldStartDate))
                    {
                        //only hold start date exist, we can compute the hold aging
                        cardinfo.ECOCustomerHoldAging = (DateTime.Now - DateTime.Parse(cardinfo.ECOCustomerHoldStartDate)).Days.ToString();
                        ViewBag.CurrentCard.ECOCustomerHoldAging = cardinfo.ECOCustomerHoldAging;
                        cardinfo.UpdateCustomerApproveHoldAging(ViewBag.CurrentCard.CardKey);
                    }
                }
                else
                {
                    //if card is pass but hold aging is empty, we set it to 0
                    if (string.IsNullOrEmpty(cardinfo.ECOCustomerHoldAging))
                    {
                        cardinfo.ECOCustomerHoldAging = "0";
                        ViewBag.CurrentCard.ECOCustomerHoldAging = cardinfo.ECOCustomerHoldAging;
                        cardinfo.UpdateCustomerApproveHoldAging(ViewBag.CurrentCard.CardKey);
                    }
                }

                ViewBag.ECOKey = ECOKey;
                ViewBag.CardKey = CardKey;
                ViewBag.CardDetailPage = DominoCardType.CustomerApprovalHold;

                if (!string.IsNullOrEmpty(updater))
                {
                    DominoUserViewModels.UpdateUserHistory(updater, baseinfos[0].ECONum, ViewBag.CurrentCard.CardType, ViewBag.CurrentCard.CardKey);
                }

                GetNoticeInfo();
                return View("CurrentECO", vm);
            }

            return RedirectToAction("ViewAll", "MiniPIP");

        }

        [HttpPost, ActionName("CustomerApprovalHold")]
        [ValidateAntiForgeryToken]
        public ActionResult CustomerApprovalHoldPost()
        {
            var ckdict = CookieUtility.UnpackCookie(this);
            var updater = ckdict["logonuser"].Split(new char[] { '|' })[0];

            var ECOKey = Request.Form["ECOKey"];
            var CardKey = Request.Form["CardKey"];

            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfo(ECOKey);
            if (baseinfos.Count > 0)
            {

                StoreAttachAndComment(CardKey, updater);
                DominoVM cardinfo = DominoVM.RetrieveCustomerApproveHoldInfo(CardKey);

                cardinfo.ECOCustomerHoldStartDate = Request.Form["ECOCustomerHoldStartDate"];
                if (!string.IsNullOrEmpty(cardinfo.ECOCustomerHoldStartDate))
                {
                    cardinfo.UpdateCustomerApproveHoldStartDate(CardKey);
                }

                if (Request.Form["commitinfo"] != null)
                {
                    var redict = new RouteValueDictionary();
                    redict.Add("CardKey", CardKey);
                    return RedirectToAction("GoBackToCardByCardKey", "MiniPIP", redict);
                }

                
                //cardinfo.ECOCustomerApproveDate = Request.Form["ECOCustomerApproveDate"];
                //cardinfo.UpdateCustomerApproveHoldInfo(CardKey);
                if (Request.Form["forcecard"] == null)
                {
                    if (string.IsNullOrEmpty(cardinfo.ECOCustomerApproveDate) && string.IsNullOrEmpty(baseinfos[0].FACustomerApproval))
                    {
                        SetNoticeInfo("ECO Sample Approve Date or FAI Approve Date, At least one of them is inputed");
                        var dict = new RouteValueDictionary();
                        dict.Add("ECOKey", ECOKey);
                        dict.Add("CardKey", CardKey);
                        return RedirectToAction(DominoCardType.CustomerApprovalHold, "MiniPIP", dict);
                    }

                    if (string.IsNullOrEmpty(cardinfo.ECOCustomerHoldStartDate))
                    {
                        SetNoticeInfo("ECOCustomerHoldStartDate need to be inputed");
                        var dict = new RouteValueDictionary();
                        dict.Add("ECOKey", ECOKey);
                        dict.Add("CardKey", CardKey);
                        return RedirectToAction(DominoCardType.CustomerApprovalHold, "MiniPIP", dict);
                    }
                }

                DominoVM.UpdateCardStatus(CardKey, DominoCardStatus.done);

                var newcardkey = DominoVM.GetUniqKey();
                new System.Threading.ManualResetEvent(false).WaitOne(2000);
                var currenttime = DateTime.Now;
                currenttime = currenttime.AddMinutes(1);

                if (string.Compare(baseinfos[0].ECOType, DominoECOType.RVS) == 0)
                {
                    var realcardkey = DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.SampleOrdering,currenttime.ToString());
                    var dict = new RouteValueDictionary();
                    dict.Add("ECOKey", ECOKey);
                    dict.Add("CardKey", realcardkey);
                    return RedirectToAction(DominoCardType.SampleOrdering, "MiniPIP", dict);
                }
                else if (string.Compare(baseinfos[0].ECOType, DominoECOType.RVNS) == 0)
                {
                    var realcardkey = DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.ECOComplete, currenttime.ToString());
                    var dict = new RouteValueDictionary();
                    dict.Add("ECOKey", ECOKey);
                    dict.Add("CardKey", realcardkey);
                    return RedirectToAction(DominoCardType.ECOComplete, "MiniPIP", dict);
                }
                else
                {
                    var realcardkey = DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.SampleOrdering, currenttime.ToString());
                    var dict = new RouteValueDictionary();
                    dict.Add("ECOKey", ECOKey);
                    dict.Add("CardKey", realcardkey);
                    return RedirectToAction(DominoCardType.SampleOrdering, "MiniPIP", dict);
                }
            }
            else
            {
                var dict = new RouteValueDictionary();
                dict.Add("ECOKey", ECOKey);
                dict.Add("CardKey", CardKey);
                return RedirectToAction(DominoCardType.CustomerApprovalHold, "MiniPIP", dict);
            }
        }


        public ActionResult SampleOrdering(string ECOKey, string CardKey, string Refresh = "No")
        {
            var ckdict = CookieUtility.UnpackCookie(this);
            if (!LoginSystem(ckdict, ECOKey, CardKey))
            {
                return RedirectToAction("LoginUser", "DominoUser");
            }

            var updater = GetAdminAuth();

            if (string.IsNullOrEmpty(ECOKey))
                ECOKey = ckdict["ECOKey"];
            if (string.IsNullOrEmpty(CardKey))
                CardKey = ckdict["CardKey"];

            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfo(ECOKey);
            if (baseinfos.Count > 0)
            {
                var currentcard = DominoVM.RetrieveSpecialCard(baseinfos[0], DominoCardType.SampleOrdering);
                if (string.Compare(currentcard[0].CardStatus, DominoCardStatus.done, true) != 0
                    || string.Compare(Refresh, "YES", true) == 0)
                {
                    if (DominoVM.CardCanbeUpdate(CardKey) || string.Compare(Refresh, "YES", true) == 0)
                    {
                        DominoDataCollector.UpdateOrderInfoFromExcel(this, baseinfos[0], CardKey);
                    }
                }

                var vm = new List<List<DominoVM>>();
                var cardlist = DominoVM.RetrieveECOCards(baseinfos[0]);
                vm.Add(cardlist);

                foreach (var card in cardlist)
                {
                    if (string.Compare(card.CardType, DominoCardType.SampleOrdering) == 0)
                    {
                        ViewBag.CurrentCard = card;
                        break;
                    }
                }

                var orderinfo = DominoVM.RetrieveOrderInfo(CardKey);
                ViewBag.CurrentCard.OrderTable = orderinfo;

                ViewBag.ECOKey = ECOKey;
                ViewBag.CardKey = CardKey;
                ViewBag.CardDetailPage = DominoCardType.SampleOrdering;

                if (!string.IsNullOrEmpty(updater))
                {
                    DominoUserViewModels.UpdateUserHistory(updater, baseinfos[0].ECONum, currentcard[0].CardType, currentcard[0].CardKey);
                }

                GetNoticeInfo();
                return View("CurrentECO", vm);
            }

            return RedirectToAction("ViewAll", "MiniPIP");

        }

        [HttpPost, ActionName("SampleOrdering")]
        [ValidateAntiForgeryToken]
        public ActionResult SampleOrderingPost()
        {
            var ckdict = CookieUtility.UnpackCookie(this);
            var updater = ckdict["logonuser"].Split(new char[] { '|' })[0];

            var ECOKey = Request.Form["ECOKey"];
            var CardKey = Request.Form["CardKey"];

            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfo(ECOKey);
            if (baseinfos.Count > 0)
            {
                StoreAttachAndComment(CardKey, updater);

                if (Request.Form["commitinfo"] != null)
                {
                    var redict = new RouteValueDictionary();
                    redict.Add("CardKey", CardKey);
                    return RedirectToAction("GoBackToCardByCardKey", "MiniPIP", redict);
                }

                if (Request.Form["forcecard"] == null)
                {
                    var orderinfo = DominoVM.RetrieveOrderInfo(CardKey);
                    if (orderinfo.Count == 0)
                    {
                        SetNoticeInfo("No order information is found");
                        var dict1 = new RouteValueDictionary();
                        dict1.Add("ECOKey", ECOKey);
                        dict1.Add("CardKey", CardKey);
                        return RedirectToAction(DominoCardType.SampleOrdering, "MiniPIP", dict1);
                    }
                }

                DominoVM.UpdateCardStatus(CardKey, DominoCardStatus.done);

                new System.Threading.ManualResetEvent(false).WaitOne(2000);
                var currenttime = DateTime.Now;
                currenttime = currenttime.AddMinutes(1);
                var newcardkey = DominoVM.GetUniqKey();
                var realcardkey = DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.SampleBuilding, currenttime.ToString());

                var dict = new RouteValueDictionary();
                dict.Add("ECOKey", ECOKey);
                dict.Add("CardKey", realcardkey);
                return RedirectToAction(DominoCardType.SampleBuilding, "MiniPIP", dict);
            }
            else
            {
                var dict = new RouteValueDictionary();
                dict.Add("ECOKey", ECOKey);
                dict.Add("CardKey", CardKey);
                return RedirectToAction(DominoCardType.SampleOrdering, "MiniPIP", dict);
            }

        }


        public ActionResult SampleBuilding(string ECOKey, string CardKey, string Refresh = "No")
        {
            var ckdict = CookieUtility.UnpackCookie(this);
            if (!LoginSystem(ckdict, ECOKey, CardKey))
            {
                return RedirectToAction("LoginUser", "DominoUser");
            }

            var updater = GetAdminAuth();

            if (string.IsNullOrEmpty(ECOKey))
                ECOKey = ckdict["ECOKey"];
            if (string.IsNullOrEmpty(CardKey))
                CardKey = ckdict["CardKey"];

            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfo(ECOKey);
            if (baseinfos.Count > 0)
            {
                var currentcard = DominoVM.RetrieveSpecialCard(baseinfos[0], DominoCardType.SampleBuilding);
                if (string.Compare(currentcard[0].CardStatus, DominoCardStatus.done, true) != 0
                    || string.Compare(Refresh, "YES", true) == 0)
                {
                    if (DominoVM.CardCanbeUpdate(CardKey) || string.Compare(Refresh, "YES", true) == 0)
                    {
                        //DominoDataCollector.RefreshDumpEEPROMFile(baseinfos[0], CardKey, this);

                        DominoDataCollector.UpdateJOInfoFromExcel(this, baseinfos[0], CardKey);
                        DominoDataCollector.Update1STJOCheckFromExcel(this, baseinfos[0], CardKey);
                        DominoDataCollector.Update2NDJOCheckFromExcel(this, baseinfos[0], CardKey);
                        DominoDataCollector.UpdateJOMainStoreFromExcel(this, baseinfos[0], CardKey);
                        DominoDataCollector.RefreshTnuableQAFAI(this, baseinfos[0], CardKey);
                    }
                }

                var vm = new List<List<DominoVM>>();
                var cardlist = DominoVM.RetrieveECOCards(baseinfos[0]);
                vm.Add(cardlist);

                foreach (var card in cardlist)
                {
                    if (string.Compare(card.CardType, DominoCardType.SampleBuilding) == 0)
                    {
                        ViewBag.CurrentCard = card;
                        break;
                    }
                }

                ViewBag.CurrentCard.EEPROMFormatFile = "";
                ViewBag.CurrentCard.EEPROMDumpFile = "";
                if (string.Compare(baseinfos[0].FlowInfo, DominoFlowInfo.Default, true) == 0)
                {
                    var signoffcards = DominoVM.RetrieveSpecialCard(baseinfos[0], DominoCardType.ECOSignoff1);
                    if (signoffcards.Count > 0)
                    {
                        var signoffinfo = DominoVM.RetrieveSignoffInfo(signoffcards[0].CardKey);
                        ViewBag.CurrentCard.EEPROMFormatFile = signoffinfo.EEPROMFormatFile;
                        ViewBag.CurrentCard.EEPROMDumpFile = signoffinfo.EEPROMDumpFile;
                        ViewBag.CurrentCard.EEPROMExcelTemplate = signoffinfo.EEPROMExcelTemplate;
                        ViewBag.CurrentCard.EEPROMMaskFile = signoffinfo.EEPROMMaskFile;
                    }
                }
                else
                {
                    var signoffcards = DominoVM.RetrieveSpecialCard(baseinfos[0], DominoCardType.ECOSignoff2);
                    if (signoffcards.Count > 0)
                    {
                        var signoffinfo = DominoVM.RetrieveSignoffInfo(signoffcards[0].CardKey);
                        ViewBag.CurrentCard.EEPROMFormatFile = signoffinfo.EEPROMFormatFile;
                        ViewBag.CurrentCard.EEPROMDumpFile = signoffinfo.EEPROMDumpFile;
                        ViewBag.CurrentCard.EEPROMExcelTemplate = signoffinfo.EEPROMExcelTemplate;
                        ViewBag.CurrentCard.EEPROMMaskFile = signoffinfo.EEPROMMaskFile;
                    }
                }

                ViewBag.CurrentCard.JoTable = DominoVM.RetrieveJOInfo(CardKey);
                ViewBag.CurrentCard.Jo1stCheckTable = DominoVM.RetrieveJOCheck(CardKey, DOMINOJOCHECKTYPE.ENGTYPE);
                ViewBag.CurrentCard.Jo2ndCheckTable = DominoVM.RetrieveJOCheck(CardKey, DOMINOJOCHECKTYPE.QATYPE);
                ViewBag.CurrentCard.JOStoreStautsTable = DominoVM.RetrieveJOMainStore(CardKey);

                ViewBag.ECOKey = ECOKey;
                ViewBag.CardKey = CardKey;
                ViewBag.CardDetailPage = DominoCardType.SampleBuilding;

                if (!string.IsNullOrEmpty(updater))
                {
                    DominoUserViewModels.UpdateUserHistory(updater, baseinfos[0].ECONum, currentcard[0].CardType, currentcard[0].CardKey);
                }
                GetNoticeInfo();
                return View("CurrentECO", vm);
            }

            return RedirectToAction("ViewAll", "MiniPIP");

        }

        [HttpPost, ActionName("SampleBuilding")]
        [ValidateAntiForgeryToken]
        public ActionResult SampleBuildingPost()
        {
            var ckdict = CookieUtility.UnpackCookie(this);
            var updater = ckdict["logonuser"].Split(new char[] { '|' })[0];

            var ECOKey = Request.Form["ECOKey"];
            var CardKey = Request.Form["CardKey"];

            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfo(ECOKey);
            if (baseinfos.Count > 0)
            {
                StoreAttachAndComment(CardKey, updater);

                if (Request.Form["commitinfo"] != null)
                {
                    var redict = new RouteValueDictionary();
                    redict.Add("CardKey", CardKey);
                    return RedirectToAction("GoBackToCardByCardKey", "MiniPIP", redict);
                }

                if (Request.Form["forcecard"] == null)
                {
                    var JoTable = DominoVM.RetrieveJOInfo(CardKey);
                    var FirstCheckTable = DominoVM.RetrieveJOCheck(CardKey, DOMINOJOCHECKTYPE.ENGTYPE);
                    var SecondCheckTable = DominoVM.RetrieveJOCheck(CardKey, DOMINOJOCHECKTYPE.QATYPE);

                    if (JoTable.Count == 0)
                    {
                        SetNoticeInfo("No JO information is found");
                        var dict1 = new RouteValueDictionary();
                        dict1.Add("ECOKey", ECOKey);
                        dict1.Add("CardKey", CardKey);
                        return RedirectToAction(DominoCardType.SampleBuilding, "MiniPIP", dict1);
                    }
                    else if (SecondCheckTable.Count == 0)
                    {
                        SetNoticeInfo("No EEPROM second check information is found");
                        var dict1 = new RouteValueDictionary();
                        dict1.Add("ECOKey", ECOKey);
                        dict1.Add("CardKey", CardKey);
                        return RedirectToAction(DominoCardType.SampleBuilding, "MiniPIP", dict1);
                    }
                    else if (FirstCheckTable.Count == 0)
                    {
                        SetNoticeInfo("No Engineering check information is found");
                        var dict1 = new RouteValueDictionary();
                        dict1.Add("ECOKey", ECOKey);
                        dict1.Add("CardKey", CardKey);
                        return RedirectToAction(DominoCardType.SampleBuilding, "MiniPIP", dict1);
                    }
                }

                DominoVM.UpdateCardStatus(CardKey, DominoCardStatus.done);

                var newcardkey = DominoVM.GetUniqKey();
                new System.Threading.ManualResetEvent(false).WaitOne(2000);
                var currenttime = DateTime.Now;
                currenttime = currenttime.AddMinutes(1);
                var realcardkey = DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.SampleShipment, currenttime.ToString());

                var dict = new RouteValueDictionary();
                dict.Add("ECOKey", ECOKey);
                dict.Add("CardKey", realcardkey);
                return RedirectToAction(DominoCardType.SampleShipment, "MiniPIP", dict);
            }
            else
            {
                var dict = new RouteValueDictionary();
                dict.Add("ECOKey", ECOKey);
                dict.Add("CardKey", CardKey);
                return RedirectToAction(DominoCardType.SampleBuilding, "MiniPIP", dict);
            }
        }


        public ActionResult SampleShipment(string ECOKey, string CardKey, string Refresh = "No")
        {
            var ckdict = CookieUtility.UnpackCookie(this);
            if (!LoginSystem(ckdict, ECOKey, CardKey))
            {
                return RedirectToAction("LoginUser", "DominoUser");
            }

            var updater = GetAdminAuth();

            if (string.IsNullOrEmpty(ECOKey))
                ECOKey = ckdict["ECOKey"];
            if (string.IsNullOrEmpty(CardKey))
                CardKey = ckdict["CardKey"];

            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfo(ECOKey);
            if (baseinfos.Count > 0)
            {
                var currentcard = DominoVM.RetrieveSpecialCard(baseinfos[0], DominoCardType.SampleShipment);
                if (string.Compare(currentcard[0].CardStatus, DominoCardStatus.done, true) != 0
                    || string.Compare(Refresh, "YES", true) == 0)
                {
                    if (DominoVM.CardCanbeUpdate(CardKey) || string.Compare(Refresh, "YES", true) == 0)
                    {
                        DominoDataCollector.UpdateShipInfoFromExcel(this, baseinfos[0], CardKey);
                    }
                }

                var vm = new List<List<DominoVM>>();
                var cardlist = DominoVM.RetrieveECOCards(baseinfos[0]);
                vm.Add(cardlist);

                foreach (var card in cardlist)
                {
                    if (string.Compare(card.CardType, DominoCardType.SampleShipment) == 0)
                    {
                        ViewBag.CurrentCard = card;
                        break;
                    }
                }

                ViewBag.CurrentCard.ShipTable = DominoVM.RetrieveShipInfo(CardKey);

                ViewBag.ECOKey = ECOKey;
                ViewBag.CardKey = CardKey;
                ViewBag.CardDetailPage = DominoCardType.SampleShipment;

                if (!string.IsNullOrEmpty(updater))
                {
                    DominoUserViewModels.UpdateUserHistory(updater, baseinfos[0].ECONum, currentcard[0].CardType, currentcard[0].CardKey);
                }

                GetNoticeInfo();
                return View("CurrentECO", vm);
            }

            return RedirectToAction("ViewAll", "MiniPIP");

        }

        [HttpPost, ActionName("SampleShipment")]
        [ValidateAntiForgeryToken]
        public ActionResult SampleShipmentPost()
        {
            var ckdict = CookieUtility.UnpackCookie(this);
            var updater = ckdict["logonuser"].Split(new char[] { '|' })[0];

            var ECOKey = Request.Form["ECOKey"];
            var CardKey = Request.Form["CardKey"];

            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfo(ECOKey);
            if (baseinfos.Count > 0)
            {
                StoreAttachAndComment(CardKey, updater);

                if (Request.Form["commitinfo"] != null)
                {
                    var redict = new RouteValueDictionary();
                    redict.Add("CardKey", CardKey);
                    return RedirectToAction("GoBackToCardByCardKey", "MiniPIP", redict);
                }

                if (Request.Form["forcecard"] == null)
                {
                    var shipinfo = DominoVM.RetrieveShipInfo(CardKey);
                    if (shipinfo.Count == 0)
                    {
                        SetNoticeInfo("No Shipment information is found");
                        var dict1 = new RouteValueDictionary();
                        dict1.Add("ECOKey", ECOKey);
                        dict1.Add("CardKey", CardKey);
                        return RedirectToAction(DominoCardType.SampleShipment, "MiniPIP", dict1);
                    }
                }

                DominoVM.UpdateCardStatus(CardKey, DominoCardStatus.done);

                var newcardkey = DominoVM.GetUniqKey();
                new System.Threading.ManualResetEvent(false).WaitOne(2000);
                var currenttime = DateTime.Now;
                currenttime = currenttime.AddMinutes(1);

                var realcardkey = DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.SampleCustomerApproval, currenttime.ToString());

                var dict = new RouteValueDictionary();
                dict.Add("ECOKey", ECOKey);
                dict.Add("CardKey", realcardkey);
                return RedirectToAction(DominoCardType.SampleCustomerApproval, "MiniPIP", dict);
            }
            else
            {
                var dict = new RouteValueDictionary();
                dict.Add("ECOKey", ECOKey);
                dict.Add("CardKey", CardKey);
                return RedirectToAction(DominoCardType.SampleShipment, "MiniPIP", dict);
            }
        }

        public ActionResult SampleCustomerApproval(string ECOKey, string CardKey, string Refresh = "No")
        {
            var ckdict = CookieUtility.UnpackCookie(this);
            if (!LoginSystem(ckdict, ECOKey, CardKey))
            {
                return RedirectToAction("LoginUser", "DominoUser");
            }

            var updater = GetAdminAuth();

            if (string.IsNullOrEmpty(ECOKey))
                ECOKey = ckdict["ECOKey"];
            if (string.IsNullOrEmpty(CardKey))
                CardKey = ckdict["CardKey"];

            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfo(ECOKey);
            if (baseinfos.Count > 0)
            {
                var vm = new List<List<DominoVM>>();
                var cardlist = DominoVM.RetrieveECOCards(baseinfos[0]);
                vm.Add(cardlist);

                foreach (var card in cardlist)
                {
                    if (string.Compare(card.CardType, DominoCardType.SampleCustomerApproval) == 0)
                    {
                        ViewBag.CurrentCard = card;
                        break;
                    }
                }

                var cardinfo = DominoVM.RetrieveSampleCustomerApproveInfo(CardKey);
                ViewBag.CurrentCard.SampleCustomerApproveDate = cardinfo.SampleCustomerApproveDate;

                ViewBag.ECOKey = ECOKey;
                ViewBag.CardKey = CardKey;
                ViewBag.CardDetailPage = DominoCardType.SampleCustomerApproval;

                if (!string.IsNullOrEmpty(updater))
                {
                    DominoUserViewModels.UpdateUserHistory(updater, baseinfos[0].ECONum, ViewBag.CurrentCard.CardType, ViewBag.CurrentCard.CardKey);
                }

                GetNoticeInfo();
                return View("CurrentECO", vm);
            }

            return RedirectToAction("ViewAll", "MiniPIP");

        }

        [HttpPost, ActionName("SampleCustomerApproval")]
        [ValidateAntiForgeryToken]
        public ActionResult SampleCustomerApprovalPost()
        {
            var ckdict = CookieUtility.UnpackCookie(this);
            var updater = ckdict["logonuser"].Split(new char[] { '|' })[0];

            var ECOKey = Request.Form["ECOKey"];
            var CardKey = Request.Form["CardKey"];

            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfo(ECOKey);
            if (baseinfos.Count > 0)
            {
                StoreAttachAndComment(CardKey, updater);

                var tempinfo = new DominoVM();
                tempinfo.SampleCustomerApproveDate = Request.Form["SampleCustomerApproveDate"];
                tempinfo.UpdateSampleCustomerApproveInfo(CardKey);

                if (Request.Form["commitinfo"] != null)
                {
                    var redict = new RouteValueDictionary();
                    redict.Add("CardKey", CardKey);
                    return RedirectToAction("GoBackToCardByCardKey", "MiniPIP", redict);
                }


                if (string.IsNullOrEmpty(tempinfo.SampleCustomerApproveDate))
                {
                    if (Request.Form["forcecard"] == null)
                    {
                        SetNoticeInfo("Sample Customer Approve Date is not inputed");
                        var dict1 = new RouteValueDictionary();
                        dict1.Add("ECOKey", ECOKey);
                        dict1.Add("CardKey", CardKey);
                        return RedirectToAction(DominoCardType.SampleCustomerApproval, "MiniPIP", dict1);
                    }
                }
                else
                {
                    var customerholdcard = DominoVM.RetrieveSpecialCard(baseinfos[0], DominoCardType.CustomerApprovalHold);
                    if (customerholdcard.Count > 0)
                    {
                        customerholdcard[0].ECOCustomerApproveDate = tempinfo.SampleCustomerApproveDate;
                        customerholdcard[0].UpdateCustomerApproveHoldInfo(customerholdcard[0].CardKey);
                    }
                }

                DominoVM.UpdateCardStatus(CardKey, DominoCardStatus.done);

                var newcardkey = DominoVM.GetUniqKey();
                new System.Threading.ManualResetEvent(false).WaitOne(2000);
                var currenttime = DateTime.Now;
                currenttime = currenttime.AddMinutes(1);

                if (string.Compare(baseinfos[0].ECOType, DominoECOType.RVS) == 0)
                {
                    var realcardkey = DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.ECOComplete, currenttime.ToString());
                    var dict = new RouteValueDictionary();
                    dict.Add("ECOKey", ECOKey);
                    dict.Add("CardKey", realcardkey);
                    return RedirectToAction(DominoCardType.ECOComplete, "MiniPIP", dict);
                }
                else
                {
                    var realcardkey = DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.MiniPIPComplete, currenttime.ToString());
                    var dict = new RouteValueDictionary();
                    dict.Add("ECOKey", ECOKey);
                    dict.Add("CardKey", realcardkey);
                    return RedirectToAction(DominoCardType.MiniPIPComplete, "MiniPIP", dict);
                }
            }
            else
            {
                var dict = new RouteValueDictionary();
                dict.Add("ECOKey", ECOKey);
                dict.Add("CardKey", CardKey);
                return RedirectToAction(DominoCardType.SampleCustomerApproval, "MiniPIP", dict);
            }
        }


        public ActionResult MiniPIPComplete(string ECOKey, string CardKey, string Refresh = "No")
        {
            var ckdict = CookieUtility.UnpackCookie(this);
            if (!LoginSystem(ckdict, ECOKey, CardKey))
            {
                return RedirectToAction("LoginUser", "DominoUser");
            }

            var updater = GetAdminAuth();

            if (string.IsNullOrEmpty(ECOKey))
                ECOKey = ckdict["ECOKey"];
            if (string.IsNullOrEmpty(CardKey))
                CardKey = ckdict["CardKey"];

            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfo(ECOKey);
            if (baseinfos.Count > 0)
            {
                var vm = new List<List<DominoVM>>();
                var cardlist = DominoVM.RetrieveECOCards(baseinfos[0]);
                vm.Add(cardlist);

                foreach (var card in cardlist)
                {
                    if (string.Compare(card.CardType, DominoCardType.MiniPIPComplete) == 0)
                    {
                        ViewBag.CurrentCard = card;
                        break;
                    }
                }

                ViewBag.MCOIssued = baseinfos[0].MCOIssued;
                ViewBag.FACustomerApproval = baseinfos[0].FACustomerApproval;

                var approvecard = DominoVM.RetrieveSpecialCard(baseinfos[0], DominoCardType.SampleCustomerApproval);
                var approvecardinfo = DominoVM.RetrieveSampleCustomerApproveInfo(approvecard[0].CardKey);
                ViewBag.SampleCustomerApproveDate = approvecardinfo.SampleCustomerApproveDate;

                var cardinfo = DominoVM.RetrieveMinipipCompleteInfo(CardKey);

                var lifecycle = new string[] {"NONE", DominoLifeCycle.FirstArticl, DominoLifeCycle.Prototype,DominoLifeCycle.PreProduct, DominoLifeCycle.Pilot, DominoLifeCycle.Production};
                var asilist = new List<string>();
                asilist.AddRange(lifecycle);
                ViewBag.ECOPartLifeCycleList = CreateSelectList(asilist,cardinfo.ECOPartLifeCycle);

                asilist = new List<string>();
                asilist.AddRange(lifecycle);
                ViewBag.GenericPartLifeCycleList = CreateSelectList(asilist,cardinfo.GenericPartLifeCycle);

                ViewBag.ECOKey = ECOKey;
                ViewBag.CardKey = CardKey;
                ViewBag.CardDetailPage = DominoCardType.MiniPIPComplete;

                if (!string.IsNullOrEmpty(updater))
                {
                    DominoUserViewModels.UpdateUserHistory(updater, baseinfos[0].ECONum, ViewBag.CurrentCard.CardType, ViewBag.CurrentCard.CardKey);
                }

                GetNoticeInfo();
                return View("CurrentECO", vm);
            }

            return RedirectToAction("ViewAll", "MiniPIP");
        }


        [HttpPost, ActionName("MiniPIPComplete")]
        [ValidateAntiForgeryToken]
        public ActionResult MiniPIPCompletePost()
        {
            var ckdict = CookieUtility.UnpackCookie(this);
            var updater = ckdict["logonuser"].Split(new char[] { '|' })[0];

            var ECOKey = Request.Form["ECOKey"];
            var CardKey = Request.Form["CardKey"];

            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfo(ECOKey);
            if (baseinfos.Count > 0)
            {
                StoreAttachAndComment(CardKey, updater);

                var tempinfo = new DominoVM();
                if (string.Compare(Request.Form["ECOPartLifeCycleList"],"NONE",true) != 0)
                {
                    tempinfo.ECOPartLifeCycle = Request.Form["ECOPartLifeCycleList"];
                }
                if (string.Compare(Request.Form["GenericPartLifeCycleList"], "NONE", true) != 0)
                {
                    tempinfo.GenericPartLifeCycle = Request.Form["GenericPartLifeCycleList"];
                }
                tempinfo.UpdateMinipipCompleteInfo(CardKey);

                if (Request.Form["commitinfo"] != null)
                {
                    var redict = new RouteValueDictionary();
                    redict.Add("CardKey", CardKey);
                    return RedirectToAction("GoBackToCardByCardKey", "MiniPIP", redict);
                }

                if (Request.Form["forcecard"] == null)
                {
                    var allchecked = true;

                    if (string.IsNullOrEmpty(baseinfos[0].MCOIssued))
                    {
                        allchecked = false;
                        SetNoticeInfo("MCO number must be input on the ECO Pending card");
                    }
                    else if (string.IsNullOrEmpty(tempinfo.ECOPartLifeCycle))
                    {
                        allchecked = false;
                        SetNoticeInfo("ECO LifeCycle should not be empty");
                    }
                    else if (string.IsNullOrEmpty(tempinfo.GenericPartLifeCycle))
                    {
                        allchecked = false;
                        SetNoticeInfo("Generic LifeCycle should not be empty");
                    }
                    else if (string.Compare(tempinfo.ECOPartLifeCycle, tempinfo.GenericPartLifeCycle, true) != 0)
                    {
                        allchecked = false;
                        SetNoticeInfo("ECO LifeCycle is different from Generic LifeCycle");
                    }
                    else if ((string.Compare(baseinfos[0].ECOType, DominoECOType.DVNS) == 0
                       || string.Compare(baseinfos[0].ECOType, DominoECOType.RVNS) == 0)
                       && string.IsNullOrEmpty(baseinfos[0].FACustomerApproval))
                    {
                        allchecked = false;
                        SetNoticeInfo("FAI Approve Date must be inputed in ECO Signoff1/ECO Signoff2 card");
                    }

                    if (!allchecked)
                    {
                        var dict1 = new RouteValueDictionary();
                        dict1.Add("ECOKey", ECOKey);
                        dict1.Add("CardKey", CardKey);
                        return RedirectToAction(DominoCardType.MiniPIPComplete, "MiniPIP", dict1);
                    }
                }

                DominoVM.UpdateCardStatus(CardKey, DominoCardStatus.done);

                baseinfos[0].MiniPIPStatus = DominoMiniPIPStatus.done;
                baseinfos[0].UpdateECO();
            }

            var dict = new RouteValueDictionary();
            dict.Add("ECOKey", ECOKey);
            dict.Add("CardKey", CardKey);
            return RedirectToAction(DominoCardType.MiniPIPComplete, "MiniPIP", dict);
        }

        public ActionResult DeleteCardAttachment(string CardKey, string FileName)
        {
            DominoVM.DeleteCardAttachment(CardKey, FileName);

            var vm = DominoVM.RetrieveCard(CardKey);
            var dict = new RouteValueDictionary();
            dict.Add("ECOKey", vm[0].ECOKey);
            dict.Add("CardKey", vm[0].CardKey);
            return RedirectToAction(vm[0].CardType, "MiniPIP", dict);
        }

        public ActionResult RefreshCard(string CardKey)
        {
            var vm = DominoVM.RetrieveCard(CardKey);
            var dict = new RouteValueDictionary();
            dict.Add("ECOKey", vm[0].ECOKey);
            dict.Add("CardKey", vm[0].CardKey);
            dict.Add("Refresh", "YES");
            return RedirectToAction(vm[0].CardType, "MiniPIP", dict);
        }

        public ActionResult ShowCardByCardKey(string CardKey)
        {
            var vm = DominoVM.RetrieveCard(CardKey);
            var dict = new RouteValueDictionary();
            dict.Add("ECOKey", vm[0].ECOKey);
            dict.Add("CardKey", vm[0].CardKey);
            return RedirectToAction(vm[0].CardType, "MiniPIP", dict);
        }

        public ActionResult DeleteMiniPIP(string ECOKey)
        {
            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfo(ECOKey);
            if (baseinfos.Count > 0)
            {
                baseinfos[0].MiniPIPStatus = DominoMiniPIPStatus.delete;
                baseinfos[0].UpdateECO();
            }
            return RedirectToAction("ViewAll", "MiniPIP");
        }

        public ActionResult ForceCompleteMiniPIP(string ECOKey)
        {
            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfo(ECOKey);
            if (baseinfos.Count > 0)
            {
                //if (string.Compare(baseinfos[0].CurrentECOProcess.ToUpper(), "COMPLETED") == 0)
                //{
                    baseinfos[0].MiniPIPStatus = DominoMiniPIPStatus.done;
                    baseinfos[0].UpdateECO();
                    SetNoticeInfo("Force complete MiniPIP "+ baseinfos[0].ECONum + " sucessfully !");
                    return RedirectToAction("CompletedMiniPIP", "MiniPIP");
                //}
                //else
                //{
                //    SetNoticeInfo("Fail to Force complete MiniPIP "+baseinfos[0].ECONum+". Current workflow process is "+ baseinfos[0].CurrentECOProcess + " not COMPLETED !");
                //    return RedirectToAction("ViewAll", "MiniPIP");
                //}
            }
            
            return RedirectToAction("ViewAll", "MiniPIP");
        }

        public ActionResult DeleteOrderInfo(string CardKey,string LineID)
        {
            DominoVM.DeleteOrderInfo(CardKey, LineID);

            var vm = DominoVM.RetrieveCard(CardKey);
            var dict = new RouteValueDictionary();
            dict.Add("ECOKey", vm[0].ECOKey);
            dict.Add("CardKey", vm[0].CardKey);
            return RedirectToAction(vm[0].CardType, "MiniPIP", dict);
        }

        public ActionResult DeleteJOInfo(string CardKey, string WipJob)
        {
            DominoVM.DeletJOInfo(CardKey, WipJob);

            var vm = DominoVM.RetrieveCard(CardKey);
            var dict = new RouteValueDictionary();
            dict.Add("ECOKey", vm[0].ECOKey);
            dict.Add("CardKey", vm[0].CardKey);
            return RedirectToAction(vm[0].CardType, "MiniPIP", dict);
        }

        public ActionResult DeleteAllJO(string CardKey)
        {
            DominoVM.DeletJOInfo(CardKey, null);

            var vm = DominoVM.RetrieveCard(CardKey);
            var dict = new RouteValueDictionary();
            dict.Add("ECOKey", vm[0].ECOKey);
            dict.Add("CardKey", vm[0].CardKey);
            return RedirectToAction(vm[0].CardType, "MiniPIP", dict);
        }

        public ActionResult DeleteJOEGCheck(string CardKey, string WipJob)
        {
            DominoVM.DeletJOCheckInfo(CardKey, WipJob, DOMINOJOCHECKTYPE.ENGTYPE);

            var vm = DominoVM.RetrieveCard(CardKey);
            var dict = new RouteValueDictionary();
            dict.Add("ECOKey", vm[0].ECOKey);
            dict.Add("CardKey", vm[0].CardKey);
            return RedirectToAction(vm[0].CardType, "MiniPIP", dict);
        }

        public ActionResult DeleteJOQACheck(string CardKey, string WipJob)
        {
            DominoVM.DeletJOCheckInfo(CardKey, WipJob, DOMINOJOCHECKTYPE.QATYPE);

            var vm = DominoVM.RetrieveCard(CardKey);
            var dict = new RouteValueDictionary();
            dict.Add("ECOKey", vm[0].ECOKey);
            dict.Add("CardKey", vm[0].CardKey);
            return RedirectToAction(vm[0].CardType, "MiniPIP", dict);
        }

        private void CreateRVSCards(string ECOKey)
        {
            var realcardkey = string.Empty;

            new System.Threading.ManualResetEvent(false).WaitOne(2000);
            var currenttime = DateTime.Now;

            currenttime = currenttime.AddMinutes(1);
            realcardkey = DominoVM.CreateCard(ECOKey, DominoVM.GetUniqKey(), DominoCardType.ECOSignoff2, currenttime.ToString());

            currenttime = currenttime.AddMinutes(1);
            realcardkey = DominoVM.CreateCard(ECOKey, DominoVM.GetUniqKey(), DominoCardType.CustomerApprovalHold, currenttime.ToString());

            currenttime = currenttime.AddMinutes(1);
            realcardkey = DominoVM.CreateCard(ECOKey, DominoVM.GetUniqKey(), DominoCardType.SampleOrdering, currenttime.ToString());

            currenttime = currenttime.AddMinutes(1);
            realcardkey = DominoVM.CreateCard(ECOKey, DominoVM.GetUniqKey(), DominoCardType.SampleBuilding, currenttime.ToString());

            currenttime = currenttime.AddMinutes(1);
            realcardkey = DominoVM.CreateCard(ECOKey, DominoVM.GetUniqKey(), DominoCardType.SampleShipment, currenttime.ToString());

            currenttime = currenttime.AddMinutes(1);
            realcardkey = DominoVM.CreateCard(ECOKey, DominoVM.GetUniqKey(), DominoCardType.SampleCustomerApproval, currenttime.ToString());

            currenttime = currenttime.AddMinutes(1);
            realcardkey = DominoVM.CreateCard(ECOKey, DominoVM.GetUniqKey(), DominoCardType.ECOComplete, currenttime.ToString());

            currenttime = currenttime.AddMinutes(1);
            realcardkey = DominoVM.CreateCard(ECOKey, DominoVM.GetUniqKey(), DominoCardType.MiniPIPComplete, currenttime.ToString());

        }

        private void CreateRVNSCards(string ECOKey)
        {
            var realcardkey = string.Empty;

            new System.Threading.ManualResetEvent(false).WaitOne(2000);
            var currenttime = DateTime.Now;

            currenttime = currenttime.AddMinutes(1);
            realcardkey = DominoVM.CreateCard(ECOKey, DominoVM.GetUniqKey(), DominoCardType.ECOSignoff2, currenttime.ToString());

            currenttime = currenttime.AddMinutes(1);
            realcardkey = DominoVM.CreateCard(ECOKey, DominoVM.GetUniqKey(), DominoCardType.CustomerApprovalHold, currenttime.ToString());

            currenttime = currenttime.AddMinutes(1);
            realcardkey = DominoVM.CreateCard(ECOKey, DominoVM.GetUniqKey(), DominoCardType.ECOComplete, currenttime.ToString());

            currenttime = currenttime.AddMinutes(1);
            realcardkey = DominoVM.CreateCard(ECOKey, DominoVM.GetUniqKey(), DominoCardType.SampleOrdering, currenttime.ToString());

            currenttime = currenttime.AddMinutes(1);
            realcardkey = DominoVM.CreateCard(ECOKey, DominoVM.GetUniqKey(), DominoCardType.SampleBuilding, currenttime.ToString());

            currenttime = currenttime.AddMinutes(1);
            realcardkey = DominoVM.CreateCard(ECOKey, DominoVM.GetUniqKey(), DominoCardType.SampleShipment, currenttime.ToString());

            currenttime = currenttime.AddMinutes(1);
            realcardkey = DominoVM.CreateCard(ECOKey, DominoVM.GetUniqKey(), DominoCardType.SampleCustomerApproval, currenttime.ToString());

            currenttime = currenttime.AddMinutes(1);
            realcardkey = DominoVM.CreateCard(ECOKey, DominoVM.GetUniqKey(), DominoCardType.MiniPIPComplete, currenttime.ToString());

        }

        public ActionResult RollBack2ThisCard(string CardKey, string ECOKey)
        {
            
            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfo(ECOKey);
            var vm = DominoVM.RetrieveCard(CardKey);

            DominoVM.RollBack2This(ECOKey, CardKey);

            new System.Threading.ManualResetEvent(false).WaitOne(2000);
            var currenttime = DateTime.Now;
            currenttime = currenttime.AddMinutes(1);

            if (string.Compare(vm[0].CardType, DominoCardType.ECOPending) == 0)
            {
                var newcardkey = DominoVM.CreateCard(ECOKey, DominoVM.GetUniqKey(), vm[0].CardType, currenttime.ToString());
                var dict = new RouteValueDictionary();
                dict.Add("ECOKey", vm[0].ECOKey);
                dict.Add("CardKey", newcardkey);
                return RedirectToAction(vm[0].CardType, "MiniPIP", dict);
            }
            else if (string.Compare(baseinfos[0].ECOType, DominoECOType.RVS) == 0)
            {
                CreateRVSCards(ECOKey);

                var spcard = DominoVM.RetrieveSpecialCard(baseinfos[0], vm[0].CardType);
                var dict = new RouteValueDictionary();
                dict.Add("ECOKey", ECOKey);
                dict.Add("CardKey", spcard[0].CardKey);
                return RedirectToAction(vm[0].CardType, "MiniPIP", dict);
            }
            else if (string.Compare(baseinfos[0].ECOType, DominoECOType.RVNS) == 0)
            {
                CreateRVNSCards(ECOKey);

                var spcard = DominoVM.RetrieveSpecialCard(baseinfos[0], vm[0].CardType);
                var dict = new RouteValueDictionary();
                dict.Add("ECOKey", ECOKey);
                dict.Add("CardKey", spcard[0].CardKey);
                return RedirectToAction(vm[0].CardType, "MiniPIP", dict);
            }
            else
            {
                var newcardkey = DominoVM.CreateCard(ECOKey, DominoVM.GetUniqKey(), vm[0].CardType, currenttime.ToString());
                var dict = new RouteValueDictionary();
                dict.Add("ECOKey", vm[0].ECOKey);
                dict.Add("CardKey", newcardkey);

                if (string.Compare(baseinfos[0].ECOType, DominoECOType.DVNS) == 0
                    || string.Compare(baseinfos[0].ECOType, DominoECOType.DVS) == 0)
                {
                    var templist = DominoVM.RetrieveECOCards(baseinfos[0]);
                    if (templist.Count > 0)
                    {
                        CreateAllDefaultCards(templist.Count, baseinfos[0]);
                    }
                }

                return RedirectToAction(vm[0].CardType, "MiniPIP", dict);
            }
            
        }

        public ActionResult DeleteJOStore(string CardKey, string WipJob)
        {
            DominoVM.DeletJOStore(CardKey, WipJob);

            var vm = DominoVM.RetrieveCard(CardKey);
            var dict = new RouteValueDictionary();
            dict.Add("ECOKey", vm[0].ECOKey);
            dict.Add("CardKey", vm[0].CardKey);
            return RedirectToAction(vm[0].CardType, "MiniPIP", dict);

        }

        public ActionResult DeleteAllJOStore(string CardKey, string WipJob)
        {
            DominoVM.DeletJOStore(CardKey, null);

            var vm = DominoVM.RetrieveCard(CardKey);
            var dict = new RouteValueDictionary();
            dict.Add("ECOKey", vm[0].ECOKey);
            dict.Add("CardKey", vm[0].CardKey);
            return RedirectToAction(vm[0].CardType, "MiniPIP", dict);

        }

        

        public ActionResult GoBackToCardByCardKey(string CardKey)
        {
            var vm = DominoVM.RetrieveCard(CardKey);
            var dict = new RouteValueDictionary();
            dict.Add("ECOKey", vm[0].ECOKey);
            dict.Add("CardKey", vm[0].CardKey);
            return RedirectToAction(vm[0].CardType, "MiniPIP", dict);
        }

        public ActionResult DeleteSPCardComment(string CardKey, string Date)
        {
            var vm = DominoVM.RetrieveCard(CardKey);

            DominoVM.DeleteSPCardComment(CardKey, Date);

            var dict = new RouteValueDictionary();
            dict.Add("ECOKey", vm[0].ECOKey);
            dict.Add("CardKey", vm[0].CardKey);
            return RedirectToAction(vm[0].CardType, "MiniPIP", dict);
        }

        public ActionResult ModifyCardComment(string CardKey, string Date)
        {
            var vm = DominoVM.RetrieveSPCardComment(CardKey, Date);
            if (!string.IsNullOrEmpty(vm.CardKey))
                return View(vm);
            else
                return RedirectToAction("ViewAll", "MiniPIP");
        }

        [HttpPost, ActionName("ModifyCardComment")]
        [ValidateAntiForgeryToken]
        public ActionResult ModifyCardCommentPost()
        {
            var CardKey = Request.Form["HCardKey"];
            var Date = Request.Form["HDate"];

            if (!string.IsNullOrEmpty(Request.Form["commenteditor"]))
            {
                var comm = new ECOCardComments();
                comm.Comment = SeverHtmlDecode.Decode(this, Request.Form["commenteditor"]);
                DominoVM.UpdateSPCardComment(CardKey, Date, comm.dbComment);
            }

            var vm = DominoVM.RetrieveCard(CardKey);
            var dict = new RouteValueDictionary();
            dict.Add("ECOKey", vm[0].ECOKey);
            dict.Add("CardKey", vm[0].CardKey);
            return RedirectToAction(vm[0].CardType, "MiniPIP", dict);
        }

        public ActionResult AgileFileDownload(string CardKey)
        {
            var vm = DominoVM.RetrieveCard(CardKey);

            var baseinfo = ECOBaseInfo.RetrieveECOBaseInfo(vm[0].ECOKey);
            if (baseinfo.Count > 0)
            {
                var ecolist = new List<string>();
                ecolist.Add(baseinfo[0].ECONum);
                DominoDataCollector.DownloadAgile(ecolist, this, DOMINOAGILEDOWNLOADTYPE.ATTACH);
            }

            var dict = new RouteValueDictionary();
            dict.Add("ECOKey", vm[0].ECOKey);
            dict.Add("CardKey", vm[0].CardKey);
            return RedirectToAction(vm[0].CardType, "MiniPIP", dict);
        }

        public ActionResult AgileFileNameDownload(string CardKey)
        {
            var vm = DominoVM.RetrieveCard(CardKey);

            var baseinfo = ECOBaseInfo.RetrieveECOBaseInfo(vm[0].ECOKey);
            if (baseinfo.Count > 0)
            {
                var ecolist = new List<string>();
                ecolist.Add(baseinfo[0].ECONum);
                DominoDataCollector.DownloadAgile(ecolist, this, DOMINOAGILEDOWNLOADTYPE.ATTACHNAME);
            }

            var dict = new RouteValueDictionary();
            dict.Add("ECOKey", vm[0].ECOKey);
            dict.Add("CardKey", vm[0].CardKey);
            return RedirectToAction(vm[0].CardType, "MiniPIP", dict);
        }

        public ActionResult ReNewCard(string CardKey)
        {
            var vm = DominoVM.RetrieveCard(CardKey);

            DominoVM.UpdateCardStatus(CardKey, DominoCardStatus.working);

            if (string.Compare(vm[0].CardType, DominoCardType.MiniPIPComplete) == 0)
            {
                var baseinfo = ECOBaseInfo.RetrieveECOBaseInfo(vm[0].ECOKey);
                if (baseinfo.Count > 0)
                {
                    baseinfo[0].MiniPIPStatus = DominoMiniPIPStatus.working;
                    baseinfo[0].UpdateECO();
                }
            }

            var dict = new RouteValueDictionary();
            dict.Add("ECOKey", vm[0].ECOKey);
            dict.Add("CardKey", vm[0].CardKey);
            return RedirectToAction(vm[0].CardType, "MiniPIP", dict);
        }

        public ActionResult AgileWorkFlowDownload(string CardKey)
        {
            var vm = DominoVM.RetrieveCard(CardKey);

            var baseinfo = ECOBaseInfo.RetrieveECOBaseInfo(vm[0].ECOKey);
            if (baseinfo.Count > 0)
            {
                var ecolist = new List<string>();
                ecolist.Add(baseinfo[0].ECONum);
                DominoDataCollector.DownloadAgile(ecolist, this, DOMINOAGILEDOWNLOADTYPE.WORKFLOW);
            }

            var dict = new RouteValueDictionary();
            dict.Add("ECOKey", vm[0].ECOKey);
            dict.Add("CardKey", vm[0].CardKey);
            return RedirectToAction(vm[0].CardType, "MiniPIP", dict);
        }

        private void StoreAgileAttch(string ECONUM,List<DominoVM> vm)
        {
            var syscfgdict = DominoDataCollector.GetSysConfig(this);
            var SAVELOCATION = (Server.MapPath("~/userfiles") + "\\docs\\Agile");
            Directory.CreateDirectory(SAVELOCATION);

            
            var dir = SAVELOCATION +"\\"+ ECONUM;
            if (Directory.Exists(dir))
            {
                //string datestring = DateTime.Now.ToString("yyyyMMdd");
                string imgdir = Server.MapPath("~/userfiles") + "\\docs\\" + ECONUM + "\\";
                if (!Directory.Exists(imgdir))
                {
                    Directory.CreateDirectory(imgdir);
                }


                var cardinfo = DominoVM.RetrieveSignoffInfo(vm[0].CardKey);

                var files = Directory.EnumerateFiles(dir);
                foreach (var fl in files)
                {
                    var fn = Path.GetFileName(fl);
                    var desfn = imgdir + fn;
                    var url = "/userfiles/docs/" + ECONUM + "/" + fn;

                    if (fl.ToUpper().Contains("QR_")
                        || fl.ToUpper().Contains("QUALIFICATION"))
                    {
                        System.IO.File.Copy(fl, desfn, true);
                        if (!cardinfo.ECOQRFile.Contains(fn))
                        {
                            cardinfo.ECOQRFile = cardinfo.ECOQRFile + url + ":::";
                        }
                    }
                    else if (fl.ToUpper().Contains("PEER")
                        && fl.ToUpper().Contains("REVIEW"))
                    {
                        System.IO.File.Copy(fl, desfn, true);
                        if (!cardinfo.EEPROMPeerReview.Contains(fn))
                        {
                            cardinfo.EEPROMPeerReview = cardinfo.EEPROMPeerReview + url + ":::";
                        }
                    }
                    else if (fl.ToUpper().Contains("TRACEVIEW"))
                    {
                        System.IO.File.Copy(fl, desfn, true);
                        if (!cardinfo.ECOTraceview.Contains(fn))
                        {
                            cardinfo.ECOTraceview = cardinfo.ECOTraceview + url + ":::";
                        }
                    }
                    else if (fl.ToUpper().Contains("COMPARE"))
                    {
                        System.IO.File.Copy(fl, desfn, true);
                        if (!cardinfo.SpecCompresuite.Contains(fn))
                        {
                            cardinfo.SpecCompresuite = cardinfo.SpecCompresuite + url + ":::";
                        }
                    }
                }

                cardinfo.UpdateSignoffInfo(vm[0].CardKey);
            }
        }

        public ActionResult AgileAttach(string ECONUM)
        {
            var ecoinfos = ECOBaseInfo.RetrieveECOBaseInfoWithECONum(ECONUM);
            foreach (var ecoitem in ecoinfos)
            {
                var vm = DominoVM.RetrieveSpecialCard(ecoitem, DominoCardType.ECOSignoff1);
                if (vm.Count > 0)
                {
                    StoreAgileAttch(ECONUM,vm);

                    if (string.Compare(vm[0].CardStatus, DominoCardStatus.working) == 0)
                    {
                        DominoVM.UpdateCardStatus(vm[0].CardKey, DominoCardStatus.pending);
                    }
                }
                else
                {
                    vm = DominoVM.RetrieveSpecialCard(ecoitem, DominoCardType.ECOSignoff2);
                    if (vm.Count > 0)
                    {
                        StoreAgileAttch(ECONUM, vm);

                        if (string.Compare(vm[0].CardStatus, DominoCardStatus.working) == 0)
                        {
                            DominoVM.UpdateCardStatus(vm[0].CardKey, DominoCardStatus.pending);
                        }
                    }
                }
            }
            return View();
        }


        public ActionResult AgileWorkFlow(string ECONUM)
        {
            var ecoinfos = ECOBaseInfo.RetrieveECOBaseInfoWithECONum(ECONUM);
            var workflowinfo = DominoDataCollector.RetrieveAgileWorkFlowData(ECONUM, this);

            foreach (var ecoitem in ecoinfos)
            {
                    if(!string.IsNullOrEmpty(workflowinfo.CurrentProcess))
                    {
                    ecoitem.CurrentECOProcess = workflowinfo.CurrentProcess;
                    ecoitem.CurrentFlowType = workflowinfo.WorkFlowType;
                    ecoitem.UpdateECO();
                    }

                    var vm = DominoVM.RetrieveSpecialCard(ecoitem, DominoCardType.ECOSignoff1);
                    if (vm.Count > 0)
                    {
                        var cardinfo = DominoVM.RetrieveSignoffInfo(vm[0].CardKey);
                        cardinfo.ECOMDApprover = workflowinfo.ECOMDApprover;
                        cardinfo.ECOTRApprover = workflowinfo.ECOTRApprover;
                        cardinfo.UpdateSignoffInfo(vm[0].CardKey);

                        if (string.Compare(vm[0].CardStatus, DominoCardStatus.working) == 0)
                        {
                            DominoVM.UpdateCardStatus(vm[0].CardKey, DominoCardStatus.pending);
                        }
                    }
                    else
                    {
                        vm = DominoVM.RetrieveSpecialCard(ecoitem, DominoCardType.ECOSignoff2);
                        if (vm.Count > 0)
                        {
                            var cardinfo = DominoVM.RetrieveSignoffInfo(vm[0].CardKey);
                            cardinfo.ECOMDApprover = workflowinfo.ECOMDApprover;
                            cardinfo.ECOTRApprover = workflowinfo.ECOTRApprover;
                            cardinfo.UpdateSignoffInfo(vm[0].CardKey);

                            if (string.Compare(vm[0].CardStatus, DominoCardStatus.working) == 0)
                            {
                                DominoVM.UpdateCardStatus(vm[0].CardKey, DominoCardStatus.pending);
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(workflowinfo.CApproveHoldDate))
                    {
                        vm = DominoVM.RetrieveSpecialCard(ecoitem, DominoCardType.CustomerApprovalHold);
                        if (vm.Count > 0)
                        {
                            var cardinfo = DominoVM.RetrieveCustomerApproveHoldInfo(vm[0].CardKey);
                            cardinfo.ECOCustomerHoldStartDate = ConvertUSLocalToDate(workflowinfo.CApproveHoldDate);
                            cardinfo.UpdateCustomerApproveHoldStartDate(vm[0].CardKey);
                        }
                    }

                    if (!string.IsNullOrEmpty(workflowinfo.ECOCompleteDate))
                    {
                        vm = DominoVM.RetrieveSpecialCard(ecoitem, DominoCardType.ECOComplete);
                        if (vm.Count > 0)
                        {
                            vm[0].ECOCompleted = DominoYESNO.YES;
                            vm[0].ECOCompleteDate = ConvertUSLocalToDate(workflowinfo.ECOCompleteDate);
                            vm[0].UpdateECOCompleteInfo(vm[0].CardKey);

                            if (string.Compare(vm[0].CardStatus, DominoCardStatus.working) == 0)
                            {
                                DominoVM.UpdateCardStatus(vm[0].CardKey, DominoCardStatus.pending);
                            }
                        }
                    }
                }

            return View();
        }


        private List<string> ReceiveRMAFiles()
        {
            var ret = new List<string>();

            try
            {
                foreach (string fl in Request.Files)
                {
                    if (fl != null && Request.Files[fl].ContentLength > 0)
                    {
                        string fn = Path.GetFileName(Request.Files[fl].FileName)
                            .Replace(" ", "_").Replace("#", "").Replace("'", "")
                            .Replace("&", "").Replace("?", "").Replace("%", "").Replace("+", "");

                        string datestring = DateTime.Now.ToString("yyyyMMdd");
                        string imgdir = Server.MapPath("~/userfiles") + "\\docs\\" + datestring + "\\";

                        if (!Directory.Exists(imgdir))
                        {
                            Directory.CreateDirectory(imgdir);
                        }

                        fn = Path.GetFileNameWithoutExtension(fn) + "-" + DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(fn);
                        Request.Files[fl].SaveAs(imgdir + fn);

                        var url = "/userfiles/docs/" + datestring + "/" + fn;

                        ret.Add(url);
                    }
                }

            }
            catch (Exception ex)
            { return ret; }

            return ret;
        }

        private void StoreAttachAndComment(string CardKey, string updater, DominoVM cardinfo = null)
        {
            var urls = ReceiveRMAFiles();

            if (!string.IsNullOrEmpty(Request.Form["attachmentupload"]))
            {
                var internalreportfile = Request.Form["attachmentupload"];
                var originalname = Path.GetFileNameWithoutExtension(internalreportfile)
                    .Replace(" ", "_").Replace("#", "").Replace("'", "")
                    .Replace("&", "").Replace("?", "").Replace("%", "").Replace("+", "");

                var url = "";
                foreach (var r in urls)
                {
                    if (r.Contains(originalname))
                    {
                        url = r;
                        break;
                    }
                }

                if (!string.IsNullOrEmpty(url))
                {
                    DominoVM.StoreCardAttachment(CardKey, url);
                }
            }

            if (cardinfo != null)
            {
                if (!string.IsNullOrEmpty(Request.Form["customerapprovefile"]))
                {
                    var internalreportfile = Request.Form["customerapprovefile"];
                    var originalname = Path.GetFileNameWithoutExtension(internalreportfile)
                        .Replace(" ", "_").Replace("#", "").Replace("'", "")
                        .Replace("&", "").Replace("?", "").Replace("%", "").Replace("+", "");

                    var url = "";
                    foreach (var r in urls)
                    {
                        if (r.Contains(originalname))
                        {
                            url = r;
                            break;
                        }
                    }

                    if (!string.IsNullOrEmpty(url))
                    {
                        if (string.IsNullOrEmpty(cardinfo.CustomerApproveFile))
                        {
                            cardinfo.CustomerApproveFile = cardinfo.CustomerApproveFile + url + ":::";
                        }
                        else
                        {
                            var fs = cardinfo.CustomerApproveFile.Split(new string[] { ":::" }, StringSplitOptions.RemoveEmptyEntries);
                            var leftfs = new List<string>();
                            leftfs.Add(url);
                            foreach (var f in fs)
                            {
                                if (f.ToUpper().Contains(originalname.ToUpper()))
                                { continue; }
                                leftfs.Add(f);
                            }
                            cardinfo.CustomerApproveFile = string.Join(":::", leftfs);
                        }
                    }
                }

                if (!string.IsNullOrEmpty(Request.Form["qrfileupload"]))
                {
                    var internalreportfile = Request.Form["qrfileupload"];
                    var originalname = Path.GetFileNameWithoutExtension(internalreportfile)
                        .Replace(" ", "_").Replace("#", "").Replace("'", "")
                        .Replace("&", "").Replace("?", "").Replace("%", "").Replace("+", "");

                    var url = "";
                    foreach (var r in urls)
                    {
                        if (r.Contains(originalname))
                        {
                            url = r;
                            break;
                        }
                    }

                    if (!string.IsNullOrEmpty(url))
                    {
                        if (string.IsNullOrEmpty(cardinfo.ECOQRFile))
                        {
                            cardinfo.ECOQRFile = cardinfo.ECOQRFile + url + ":::";
                        }
                        else
                        {
                            var fs = cardinfo.ECOQRFile.Split(new string[] { ":::" }, StringSplitOptions.RemoveEmptyEntries);
                            var leftfs = new List<string>();
                            leftfs.Add(url);
                            foreach (var f in fs)
                            {
                                if (f.ToUpper().Contains(originalname.ToUpper()))
                                { continue; }
                                leftfs.Add(f);
                            }
                            cardinfo.ECOQRFile = string.Join(":::", leftfs);
                        }
                    }

                }

                if (!string.IsNullOrEmpty(Request.Form["peerfileupload"]))
                {
                    var internalreportfile = Request.Form["peerfileupload"];
                    var originalname = Path.GetFileNameWithoutExtension(internalreportfile)
                        .Replace(" ", "_").Replace("#", "").Replace("'", "")
                        .Replace("&", "").Replace("?", "").Replace("%", "").Replace("+", "");

                    var url = "";
                    foreach (var r in urls)
                    {
                        if (r.Contains(originalname))
                        {
                            url = r;
                            break;
                        }
                    }

                    if (!string.IsNullOrEmpty(url))
                    {
                        if (string.IsNullOrEmpty(cardinfo.EEPROMPeerReview))
                        {
                            cardinfo.EEPROMPeerReview = cardinfo.EEPROMPeerReview + url + ":::";
                        }
                        else
                        {
                            var fs = cardinfo.EEPROMPeerReview.Split(new string[] { ":::" }, StringSplitOptions.RemoveEmptyEntries);
                            var leftfs = new List<string>();
                            leftfs.Add(url);
                            foreach (var f in fs)
                            {
                                if (f.ToUpper().Contains(originalname.ToUpper()))
                                { continue; }
                                leftfs.Add(f);
                            }
                            cardinfo.EEPROMPeerReview = string.Join(":::", leftfs);
                        }
                    }
                }

                if (!string.IsNullOrEmpty(Request.Form["traceviewfileupload"]))
                {
                    var internalreportfile = Request.Form["traceviewfileupload"];
                    var originalname = Path.GetFileNameWithoutExtension(internalreportfile)
                        .Replace(" ", "_").Replace("#", "").Replace("'", "")
                        .Replace("&", "").Replace("?", "").Replace("%", "").Replace("+", "");

                    var url = "";
                    foreach (var r in urls)
                    {
                        if (r.Contains(originalname))
                        {
                            url = r;
                            break;
                        }
                    }

                    if (!string.IsNullOrEmpty(url))
                    {
                        if (string.IsNullOrEmpty(cardinfo.ECOTraceview))
                        {
                            cardinfo.ECOTraceview = cardinfo.ECOTraceview + url + ":::";
                        }
                        else
                        {
                            var fs = cardinfo.ECOTraceview.Split(new string[] { ":::" }, StringSplitOptions.RemoveEmptyEntries);
                            var leftfs = new List<string>();
                            leftfs.Add(url);
                            foreach (var f in fs)
                            {
                                if (f.ToUpper().Contains(originalname.ToUpper()))
                                { continue; }
                                leftfs.Add(f);
                            }
                            cardinfo.ECOTraceview = string.Join(":::", leftfs);
                        }
                    }
                }

                if (!string.IsNullOrEmpty(Request.Form["speccomfileupload"]))
                {
                    var internalreportfile = Request.Form["speccomfileupload"];
                    var originalname = Path.GetFileNameWithoutExtension(internalreportfile)
                        .Replace(" ", "_").Replace("#", "").Replace("'", "")
                        .Replace("&", "").Replace("?", "").Replace("%", "").Replace("+", "");

                    var url = "";
                    foreach (var r in urls)
                    {
                        if (r.Contains(originalname))
                        {
                            url = r;
                            break;
                        }
                    }

                    if (!string.IsNullOrEmpty(url))
                    {
                        if (string.IsNullOrEmpty(cardinfo.SpecCompresuite))
                        {
                            cardinfo.SpecCompresuite = cardinfo.SpecCompresuite + url + ":::";
                        }
                        else
                        {
                            var fs = cardinfo.SpecCompresuite.Split(new string[] { ":::" }, StringSplitOptions.RemoveEmptyEntries);
                            var leftfs = new List<string>();
                            leftfs.Add(url);
                            foreach (var f in fs)
                            {
                                if (f.ToUpper().Contains(originalname.ToUpper()))
                                { continue; }
                                leftfs.Add(f);
                            }
                            cardinfo.SpecCompresuite = string.Join(":::", leftfs);
                        }
                    }

                }

                if (!string.IsNullOrEmpty(Request.Form["codefileupload"]))
                {
                    var internalreportfile = Request.Form["codefileupload"];
                    var originalname = Path.GetFileNameWithoutExtension(internalreportfile)
                        .Replace(" ", "_").Replace("#", "").Replace("'", "")
                        .Replace("&", "").Replace("?", "").Replace("%", "").Replace("+", "");

                    var url = "";
                    foreach (var r in urls)
                    {
                        if (r.Contains(originalname))
                        {
                            url = r;
                            break;
                        }
                    }

                    if (!string.IsNullOrEmpty(url))
                    {
                        if (string.IsNullOrEmpty(cardinfo.AgileCodeFile))
                        {
                            cardinfo.AgileCodeFile = cardinfo.AgileCodeFile + url + ":::";
                        }
                        else
                        {
                            var fs = cardinfo.AgileCodeFile.Split(new string[] { ":::" }, StringSplitOptions.RemoveEmptyEntries);
                            var leftfs = new List<string>();
                            leftfs.Add(url);
                            foreach (var f in fs)
                            {
                                if (f.ToUpper().Contains(originalname.ToUpper()))
                                { continue; }
                                leftfs.Add(f);
                            }
                            cardinfo.AgileCodeFile = string.Join(":::", leftfs);
                        }
                    }

                }

                if (!string.IsNullOrEmpty(Request.Form["specfileupload"]))
                {
                    var internalreportfile = Request.Form["specfileupload"];
                    var originalname = Path.GetFileNameWithoutExtension(internalreportfile)
                        .Replace(" ", "_").Replace("#", "").Replace("'", "")
                        .Replace("&", "").Replace("?", "").Replace("%", "").Replace("+", "");

                    var url = "";
                    foreach (var r in urls)
                    {
                        if (r.Contains(originalname))
                        {
                            url = r;
                            break;
                        }
                    }

                    if (!string.IsNullOrEmpty(url))
                    {
                        if (string.IsNullOrEmpty(cardinfo.AgileSpecFile))
                        {
                            cardinfo.AgileSpecFile = cardinfo.AgileSpecFile + url + ":::";
                        }
                        else
                        {
                            var fs = cardinfo.AgileSpecFile.Split(new string[] { ":::" }, StringSplitOptions.RemoveEmptyEntries);
                            var leftfs = new List<string>();
                            leftfs.Add(url);
                            foreach (var f in fs)
                            {
                                if (f.ToUpper().Contains(originalname.ToUpper()))
                                { continue; }
                                leftfs.Add(f);
                            }
                            cardinfo.AgileSpecFile = string.Join(":::", leftfs);
                        }
                    }

                }

                if (!string.IsNullOrEmpty(Request.Form["testingfileupload"]))
                {
                    var internalreportfile = Request.Form["testingfileupload"];
                    var originalname = Path.GetFileNameWithoutExtension(internalreportfile)
                        .Replace(" ", "_").Replace("#", "").Replace("'", "")
                        .Replace("&", "").Replace("?", "").Replace("%", "").Replace("+", "");

                    var url = "";
                    foreach (var r in urls)
                    {
                        if (r.Contains(originalname))
                        {
                            url = r;
                            break;
                        }
                    }

                    if (!string.IsNullOrEmpty(url))
                    {
                        if (string.IsNullOrEmpty(cardinfo.AgileTestFile))
                        {
                            cardinfo.AgileTestFile = cardinfo.AgileTestFile + url + ":::";
                        }
                        else
                        {
                            var fs = cardinfo.AgileTestFile.Split(new string[] { ":::" }, StringSplitOptions.RemoveEmptyEntries);
                            var leftfs = new List<string>();
                            leftfs.Add(url);
                            foreach (var f in fs)
                            {
                                if (f.ToUpper().Contains(originalname.ToUpper()))
                                { continue; }
                                leftfs.Add(f);
                            }
                            cardinfo.AgileTestFile = string.Join(":::", leftfs);
                        }
                    }
                }
            }


            if (!string.IsNullOrEmpty(Request.Form["commenteditor"]))
            {
                var rootcause = Server.HtmlDecode(Request.Form["commenteditor"]);
                var dbstr = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(rootcause));
                DominoVM.StoreCardComment(CardKey, dbstr, updater, DateTime.Now.ToString());
            }

        }

        private static void logreportinfo(string filename, string info)
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

        public JsonResult NoticePESampleOrder()
        {
            var syscfg = CfgUtility.GetSysConfig(this);
            var noticepeoples = syscfg["SAMPLEORDERNOTICE"].Split(new string[] { ";","," },StringSplitOptions.RemoveEmptyEntries).ToList();

            var cardkey = Request.Form["crtcardkey"];
            var cardinfo = DominoVM.RetrieveCard(cardkey);
            if (cardinfo.Count > 0)
            {
                var baseinfo = ECOBaseInfo.RetrieveECOBaseInfo(cardinfo[0].ECOKey)[0];

                var title = "Sample Ordering for ["+baseinfo.PNDesc+"] for ["
                    + baseinfo.Customer+"] under ["+ baseinfo.ECONum + "]";
                var tolist = new List<string>();
                tolist.Add(baseinfo.PE.Replace(" ",".")+ "@finisar.com");
                if (!string.IsNullOrEmpty(baseinfo.ActualPE))
                { tolist.Add(baseinfo.ActualPE.Replace(" ", ".") + "@finisar.com"); }
                tolist.AddRange(noticepeoples);

                var infotable = new List<List<string>>();
                var templist = new List<string>();
                templist.Add("ECO Number");
                templist.Add(baseinfo.ECONum);
                infotable.Add(templist);
                templist = new List<string>();
                templist.Add("MiniPIP Flow");
                templist.Add(baseinfo.ECOType);
                infotable.Add(templist);
                templist = new List<string>();
                templist.Add("Order Info");
                templist.Add(baseinfo.FirstArticleNeed);
                infotable.Add(templist);

                templist = new List<string>();
                templist.Add("Product Requested");
                templist.Add(baseinfo.PNDesc);
                infotable.Add(templist);
                templist = new List<string>();
                templist.Add("Customer");
                templist.Add(baseinfo.Customer);
                infotable.Add(templist);
                templist = new List<string>();
                templist.Add("Type");
                templist.Add(baseinfo.Complex);
                infotable.Add(templist);

                templist = new List<string>();
                templist.Add("RSM");
                templist.Add(baseinfo.RSM);
                infotable.Add(templist);
                templist = new List<string>();
                templist.Add("PE");
                templist.Add(baseinfo.PE);
                infotable.Add(templist);
                templist = new List<string>();
                templist.Add("Risk Build");
                templist.Add(baseinfo.RiskBuild);
                infotable.Add(templist);
                if (baseinfo.ECORevenue != 0)
                {
                    templist = new List<string>();
                    templist.Add("ECO Revenue");
                    templist.Add("$"+baseinfo.ECORevenue.ToString());
                    infotable.Add(templist);
                }
                if (!string.IsNullOrEmpty(baseinfo.ActualPE))
                {
                    templist = new List<string>();
                    templist.Add("Actual PE");
                    templist.Add(baseinfo.ActualPE);
                    infotable.Add(templist);
                }


                var content = EmailUtility.CreateTableHtml("Hi All", "Below is a notice of sample order:", "",infotable);
                EmailUtility.SendEmail(this,title,tolist,content);
                new System.Threading.ManualResetEvent(false).WaitOne(500);
            }

            var ret = new JsonResult();
            ret.Data = new { sucess = true };
            return ret;
        }

        public JsonResult NoticeQACustomerApproveHold()
        {
            var syscfg = CfgUtility.GetSysConfig(this);
            var noticepeoples = syscfg["CUSTOMEAPPROVENOTICE"].Split(new string[] { ";", "," }, StringSplitOptions.RemoveEmptyEntries).ToList();

            var cardkey = Request.Form["crtcardkey"];
            var cardinfo = DominoVM.RetrieveCard(cardkey);
            if (cardinfo.Count > 0)
            {
                var baseinfo = ECOBaseInfo.RetrieveECOBaseInfo(cardinfo[0].ECOKey)[0];

                var title = "Customer Approval Hold for [" + baseinfo.PNDesc + "] for ["
                    + baseinfo.Customer + "] under [" + baseinfo.ECONum + "]";
                var tolist = new List<string>();
                tolist.Add(baseinfo.PE.Replace(" ", ".") + "@finisar.com");
                if (!string.IsNullOrEmpty(baseinfo.ActualPE))
                { tolist.Add(baseinfo.ActualPE.Replace(" ", ".") + "@finisar.com"); }
                tolist.AddRange(noticepeoples);


                var infotable = new List<List<string>>();
                var templist = new List<string>();
                templist.Add("ECO Number");
                templist.Add(baseinfo.ECONum);
                infotable.Add(templist);
                templist = new List<string>();
                templist.Add("MiniPIP Flow");
                templist.Add(baseinfo.ECOType);
                infotable.Add(templist);
                templist = new List<string>();
                templist.Add("Order Info");
                templist.Add(baseinfo.FirstArticleNeed);
                infotable.Add(templist);

                templist = new List<string>();
                templist.Add("Product Requested");
                templist.Add(baseinfo.PNDesc);
                infotable.Add(templist);
                templist = new List<string>();
                templist.Add("Customer");
                templist.Add(baseinfo.Customer);
                infotable.Add(templist);
                templist = new List<string>();
                templist.Add("Type");
                templist.Add(baseinfo.Complex);
                infotable.Add(templist);

                templist = new List<string>();
                templist.Add("RSM");
                templist.Add(baseinfo.RSM);
                infotable.Add(templist);
                templist = new List<string>();
                templist.Add("PE");
                templist.Add(baseinfo.PE);
                infotable.Add(templist);
                templist = new List<string>();
                templist.Add("Risk Build");
                templist.Add(baseinfo.RiskBuild);
                infotable.Add(templist);
                if (baseinfo.ECORevenue != 0)
                {
                    templist = new List<string>();
                    templist.Add("ECO Revenue");
                    templist.Add("$" + baseinfo.ECORevenue.ToString());
                    infotable.Add(templist);
                }
                if (!string.IsNullOrEmpty(baseinfo.ActualPE))
                {
                    templist = new List<string>();
                    templist.Add("Actual PE");
                    templist.Add(baseinfo.ActualPE);
                    infotable.Add(templist);
                }


                var content = EmailUtility.CreateTableHtml("Hi All", "Below is a notice of Customer Approval Hold:", "", infotable);
                EmailUtility.SendEmail(this, title, tolist, content);
                new System.Threading.ManualResetEvent(false).WaitOne(500);
            }

            var ret = new JsonResult();
            ret.Data = new { sucess = true };
            return ret;
        }

        public List<string> ParseECOSignoffFiles(string paths)
        {
            var ret = new List<string>();
            if (string.IsNullOrEmpty(paths) || !paths.ToUpper().Contains("/userfiles/".ToUpper()))
            { return ret; }

            var ps = paths.Split(new string[] { ":::" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var p in ps)
            {
                ret.Add(Server.MapPath("~/userfiles") + p.Replace("/userfiles", "").Replace("/", "\\"));
            }
            return ret;
        }

        public JsonResult NoticeECOSignOff()
        {
            var syscfg = CfgUtility.GetSysConfig(this);
            var noticepeoples = syscfg["ECOSIGNOFFNOTICE"].Split(new string[] { ";", "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
            var cardkey = Request.Form["crtcardkey"];
            var cardinfo = DominoVM.RetrieveCard(cardkey);
            if (cardinfo.Count > 0)
            {
                var baseinfo = ECOBaseInfo.RetrieveECOBaseInfo(cardinfo[0].ECOKey)[0];
   
                var title = "First Article for [" + baseinfo.PNDesc + "] for ["
                    + baseinfo.Customer + "] under [" + baseinfo.ECONum + "]";

                var tolist = new List<string>();
                tolist.Add(baseinfo.PE.Replace(" ", ".") + "@finisar.com");
                if (!string.IsNullOrEmpty(baseinfo.ActualPE))
                {
                    tolist.Add(baseinfo.ActualPE.Replace(" ", ".") + "@finisar.com");
                }
                tolist.Add(baseinfo.RSM.Replace(" ", ".") + "@finisar.com");
                tolist.AddRange(noticepeoples);

                var signoffinfo = DominoVM.RetrieveSignoffInfo(cardkey);
                var attlist = new List<string>();
                attlist.AddRange(ParseECOSignoffFiles(signoffinfo.CustomerApproveFile));


                var infotable = new List<List<string>>();
                var templist = new List<string>();
                templist.Add("ECO Number");
                templist.Add(baseinfo.ECONum);
                infotable.Add(templist);
                templist = new List<string>();
                templist.Add("MiniPIP Flow");
                templist.Add(baseinfo.ECOType);
                infotable.Add(templist);
                templist = new List<string>();
                templist.Add("Order Info");
                templist.Add(baseinfo.FirstArticleNeed);
                infotable.Add(templist);

                templist = new List<string>();
                templist.Add("Product Requested");
                templist.Add(baseinfo.PNDesc);
                infotable.Add(templist);
                templist = new List<string>();
                templist.Add("Customer");
                templist.Add(baseinfo.Customer);
                infotable.Add(templist);
                templist = new List<string>();
                templist.Add("Type");
                templist.Add(baseinfo.Complex);
                infotable.Add(templist);

                templist = new List<string>();
                templist.Add("RSM");
                templist.Add(baseinfo.RSM);
                infotable.Add(templist);
                templist = new List<string>();
                templist.Add("PE");
                templist.Add(baseinfo.PE);
                infotable.Add(templist);
                templist = new List<string>();
                templist.Add("Risk Build");
                templist.Add(baseinfo.RiskBuild);
                infotable.Add(templist);
                if (baseinfo.ECORevenue != 0)
                {
                    templist = new List<string>();
                    templist.Add("ECO Revenue");
                    templist.Add("$" + baseinfo.ECORevenue.ToString());
                    infotable.Add(templist);
                }
                if (!string.IsNullOrEmpty(baseinfo.ActualPE))
                {
                    templist = new List<string>();
                    templist.Add("Actual PE");
                    templist.Add(baseinfo.ActualPE);
                    infotable.Add(templist);
                }

                if (!string.IsNullOrEmpty(baseinfo.PNImplement))
                {
                    templist = new List<string>();
                    templist.Add("Existed PN Implemented");
                    templist.Add(baseinfo.PNImplement);
                    infotable.Add(templist);
                }


                var commment = "";
                commment += "Please find enclosed FAIR for [" + baseinfo.PNDesc + "] for ["
                    + baseinfo.Customer + "] under [" + baseinfo.ECONum + "] for your review and approval </p><p>";
                commment += "Appreciate if you can reply the team in 48 hours and if need more time, please notify the team so we know this email reach your end. </p><p>";
                commment += "Thank you for your support.";

                var content = EmailUtility.CreateTableHtml("Dear " + baseinfo.RSM + ","+ syscfg["AFTERRSM"] + ",",
                    commment, "", infotable);


                EmailUtility.SendEmailWithAttach(this, title, tolist, content, attlist);
                new System.Threading.ManualResetEvent(false).WaitOne(1500);
            }

            var ret = new JsonResult();
            ret.Data = new { sucess = true };
            return ret;
        }

        public ActionResult ParseEEPROMFile()
        {

            DominoDataCollector.ParseEEPROMFile(@"\\wux-engsys01\d\FTLC1157RGPL-LB_EEPROM_A02.xls", @"\\wux-engsys01\d\MASK_FTLC1157RGPL-LB_EEPROM_A00.txt"
                , @"\\wux-engsys01\d\FTLC1157RGPL-LB_EEPROM_A02.txt", @"\\wux-engsys01\d\FNX0BA9G7_EEPROM_HEX_FTLC1157_20190618_125940.txt", this);
            return View();
        }

        public ActionResult LoadHCRVMData()
        {
            try
            {
                //HCRVM.LoadHCRVMData(this);
                //HCRVM.SendHCRHistoryWarningEmail("8152d5da849a44cdb9b97583309c2d8e", "EMMA XU", "EDRGEN2NEWPCBAVERSIONREL_1276467PCBACAPACITORCHAN", this);
            }
            catch (Exception ex) { }

            return View();
        }

        public ActionResult ShowHCR(string HCRKey)
        {
            ViewBag.HCRKey = "";
            if (!string.IsNullOrEmpty(HCRKey))
            { ViewBag.HCRKey = HCRKey; }
            return View();
        }

        public JsonResult ShowHCRData()
        {
            var hcrkey = Request.Form["hcrkey"];
            var hcrdata = HCRVM.GetHCRByKey(hcrkey);

            var datalist = new List<object>();
            if (hcrdata.Count > 0)
            {
                datalist.Add(new
                { k = "HCR Name", v = hcrdata[0].HCRName });
                datalist.Add(new
                { k = "HCR Create Date", v = hcrdata[0].CreateDate });
                datalist.Add(new
                { k = "HCR PM", v = hcrdata[0].PM });
                datalist.Add(new
                { k = "HCR Product", v = hcrdata[0].ProductAffect });
                datalist.Add(new
                { k = "HCR ECO Owner", v = hcrdata[0].ECOOwner });
                datalist.Add(new
                { k = "HCR ECO Num", v = hcrdata[0].ECONum });
                datalist.Add(new
                { k = "HCR Change Items", v = hcrdata[0].ChangeItems });
                datalist.Add(new
                { k = "HCR Due Date", v = hcrdata[0].DueDate });
                datalist.Add(new
                { k = "HCR Status", v = hcrdata[0].HCRStatus });
            }

            var ret = new JsonResult();
            ret.MaxJsonLength = Int32.MaxValue;
            ret.Data = new
            {
                datalist = datalist
            };
            return ret;
        }

        public ActionResult ShowMiniPIP(string ECOKey)
        {
            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfo(ECOKey);
            var cardlist = DominoVM.RetrieveECOCards(baseinfos[0]);
            var cardkey = "";
            foreach (var card in cardlist)
            {
                if (string.Compare(card.CardType, DominoCardType.ECOPending) == 0)
                {
                    cardkey = card.CardKey;
                    break;
                }
            }

            if (string.IsNullOrEmpty(cardkey))
            { return RedirectToAction("ViewAll", "MiniPIP"); }
            else
            {
                var routedict = new RouteValueDictionary();
                routedict.Add("ECOKey", ECOKey);
                routedict.Add("CardKey", cardkey);
                return RedirectToAction("ECOPending", "MiniPIP", routedict);
            }
        }

    }
}