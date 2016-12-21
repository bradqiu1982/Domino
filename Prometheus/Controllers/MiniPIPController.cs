﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Domino.Models;
using System.Reflection;
using System.Web.Routing;
using System.IO;


namespace Domino.Controllers
{
    public class MiniPIPController : Controller
    {
        // GET: MiniPIPs
        public ActionResult ViewAll()
        {
            //DominoVM.CleanDB();

            //var baseinfo = new ECOBaseInfo();
            //baseinfo.ECOKey = DominoVM.GetUniqKey();
            //baseinfo.ECONum = "97807";
            //baseinfo.PNDesc = "FCBG410QB1C10-FC";
            //baseinfo.Customer = "MRV";
            //baseinfo.PE = "Jessica Zheng";
            //baseinfo.CreateECO();

            //DominoVM.CreateCard(baseinfo.ECOKey, DominoVM.GetUniqKey(), DominoCardType.ECOPending, DominoCardStatus.pending);

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
                return RedirectToAction("LoginUser", "User");
            }

            var baseinfos = ECOBaseInfo.RetrieveAllECOBaseInfo();
            var vm = new List<List<DominoVM>>();
            foreach (var item in baseinfos)
            {
                var templist = DominoVM.RetrieveECOCards(item);
                vm.Add(templist);
            }

            return View(vm);
        }

        public ActionResult RefreshSys()
        {
            DominoVM.SetForceECORefresh(this);
            DominoVM.RefreshECOList(this);
            return RedirectToAction("ViewAll", "MiniPIP");
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

        public ActionResult ECOPending(string ECOKey, string CardKey)
        {
            var ckdict = CookieUtility.UnpackCookie(this);
            if (!LoginSystem(ckdict, ECOKey, CardKey))
            {
                return RedirectToAction("LoginUser", "User");
            }
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

                foreach (var card in cardlist)
                {
                    if (string.Compare(card.CardType, DominoCardType.ECOPending) == 0)
                    {
                        ViewBag.CurrentCard = card;
                        break;
                    }
                }

                DominoVM cardinfo = DominoVM.RetrieveECOPendingInfo(ViewBag.CurrentCard.CardKey);
                if (string.IsNullOrEmpty(cardinfo.CardKey))
                {
                    DominoVM.StoreECOPendingInfo(ViewBag.CurrentCard.CardKey, DominoYESNO.NO);
                }

                DominoVM.UpdateECOWeeklyUpdate(this, baseinfos[0], CardKey);

                cardinfo = DominoVM.RetrieveECOPendingInfo(ViewBag.CurrentCard.CardKey);
                ViewBag.CurrentCard.MiniPIPHold = cardinfo.MiniPIPHold;
                ViewBag.CurrentCard.MiniPIPWeeklyUpdate = cardinfo.MiniPIPWeeklyUpdate;

                
                var pipholds = new string[] { DominoYESNO.NO,DominoYESNO.YES};
                asilist = new List<string>();
                asilist.AddRange(pipholds);
                ViewBag.MiniPIPHoldList = CreateSelectList(asilist, cardinfo.MiniPIPHold);

                ViewBag.ECOKey = ECOKey;
                ViewBag.CardKey = CardKey;
                ViewBag.CardDetailPage = DominoCardType.ECOPending;

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
                            .Replace(" ", "_").Replace("#", "")
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

        private void StoreAttachAndComment(string CardKey,string updater,DominoVM cardinfo=null)
        {
            var urls = ReceiveRMAFiles();

            if (!string.IsNullOrEmpty(Request.Form["attachmentupload"]))
            {
                var internalreportfile = Request.Form["attachmentupload"];
                var originalname = Path.GetFileNameWithoutExtension(internalreportfile)
                    .Replace(" ", "_").Replace("#", "")
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
                if (!string.IsNullOrEmpty(Request.Form["qrfileupload"]))
                {
                    var internalreportfile = Request.Form["qrfileupload"];
                    var originalname = Path.GetFileNameWithoutExtension(internalreportfile)
                        .Replace(" ", "_").Replace("#", "")
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
                        cardinfo.ECOQRFile = url;
                    }
                }

                if (!string.IsNullOrEmpty(Request.Form["peerfileupload"]))
                {
                    var internalreportfile = Request.Form["peerfileupload"];
                    var originalname = Path.GetFileNameWithoutExtension(internalreportfile)
                        .Replace(" ", "_").Replace("#", "")
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
                        cardinfo.EEPROMPeerReview = url;
                    }
                }

                if (!string.IsNullOrEmpty(Request.Form["traceviewfileupload"]))
                {
                    var internalreportfile = Request.Form["traceviewfileupload"];
                    var originalname = Path.GetFileNameWithoutExtension(internalreportfile)
                        .Replace(" ", "_").Replace("#", "")
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
                        cardinfo.ECOTraceview = url;
                    }
                }
                if (!string.IsNullOrEmpty(Request.Form["speccomfileupload"]))
                {
                    var internalreportfile = Request.Form["speccomfileupload"];
                    var originalname = Path.GetFileNameWithoutExtension(internalreportfile)
                        .Replace(" ", "_").Replace("#", "")
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
                        cardinfo.SpecCompresuite = url;
                    }
                }

                if (!string.IsNullOrEmpty(Request.Form["codefileupload"]))
                {
                    var internalreportfile = Request.Form["codefileupload"];
                    var originalname = Path.GetFileNameWithoutExtension(internalreportfile)
                        .Replace(" ", "_").Replace("#", "")
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
                        cardinfo.AgileCodeFile = url;
                    }
                }
                if (!string.IsNullOrEmpty(Request.Form["specfileupload"]))
                {
                    var internalreportfile = Request.Form["specfileupload"];
                    var originalname = Path.GetFileNameWithoutExtension(internalreportfile)
                        .Replace(" ", "_").Replace("#", "")
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
                        cardinfo.AgileSpecFile = url;
                    }
                }

                if (!string.IsNullOrEmpty(Request.Form["testingfileupload"]))
                {
                    var internalreportfile = Request.Form["testingfileupload"];
                    var originalname = Path.GetFileNameWithoutExtension(internalreportfile)
                        .Replace(" ", "_").Replace("#", "")
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
                        cardinfo.AgileTestFile = url;
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
                    && string.Compare(baseinfos[0].FlowInfo, "Default", true) == 0)
                {
                    baseinfos[0].ECOType = DominoECOType.DVS;
                }
                else if (IsDigitsOnly(baseinfos[0].FirstArticleNeed)
                    && string.Compare(baseinfos[0].FlowInfo, "Revise", true) == 0)
                {
                    baseinfos[0].ECOType = DominoECOType.RVS;
                }
                else if (!IsDigitsOnly(baseinfos[0].FirstArticleNeed)
                    && string.Compare(baseinfos[0].FlowInfo, "Default", true) == 0)
                {
                    baseinfos[0].ECOType = DominoECOType.DVNS;
                }
                else if (!IsDigitsOnly(baseinfos[0].FirstArticleNeed)
                    && string.Compare(baseinfos[0].FlowInfo, "Revise", true) == 0)
                {
                    baseinfos[0].ECOType = DominoECOType.RVNS;
                }

                if (!string.IsNullOrEmpty(Request.Form["MCOIssued"]))
                {
                    baseinfos[0].MCOIssued = Request.Form["MCOIssued"];
                }

                baseinfos[0].PNImplement = Request.Form["PNImplementList"].ToString();

                baseinfos[0].UpdateECO();

                var ecohold = Request.Form["MiniPIPHoldList"].ToString();
                DominoVM.UpdateECOPendingInfo(CardKey, ecohold);
                
                StoreAttachAndComment(CardKey, updater);

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

                DominoVM.UpdateCardStatus(CardKey, DominoCardStatus.done);

                var newcardkey = DominoVM.GetUniqKey();

                if (string.Compare(baseinfos[0].ECOType, DominoECOType.DVS) == 0
                    || string.Compare(baseinfos[0].ECOType, DominoECOType.DVNS) == 0)
                {
                        var realcardkey = DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.ECOSignoff1, DominoCardStatus.pending);
                        var dict = new RouteValueDictionary();
                        dict.Add("ECOKey", ECOKey);
                        dict.Add("CardKey", realcardkey);
                        return RedirectToAction(DominoCardType.ECOSignoff1, "MiniPIP", dict);
                }
                else if (string.Compare(baseinfos[0].ECOType, DominoECOType.RVS) == 0
                    || string.Compare(baseinfos[0].ECOType, DominoECOType.RVNS) == 0)
                {
                        var realcardkey = DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.ECOSignoff2, DominoCardStatus.pending);
                        var dict = new RouteValueDictionary();
                        dict.Add("ECOKey", ECOKey);
                        dict.Add("CardKey", realcardkey);
                        return RedirectToAction(DominoCardType.ECOSignoff2, "MiniPIP", dict);
                }
                else
                {
                        var realcardkey = DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.ECOSignoff1, DominoCardStatus.pending);
                        var dict = new RouteValueDictionary();
                        dict.Add("ECOKey", ECOKey);
                        dict.Add("CardKey", realcardkey);
                        return RedirectToAction(DominoCardType.ECOSignoff1, "MiniPIP", dict);
                }
            }
            else
            {
                var dict = new RouteValueDictionary();
                dict.Add("ECOKey", ECOKey);
                dict.Add("CardKey", CardKey);
                return RedirectToAction(DominoCardType.ECOPending, "MiniPIP", dict);
            }
        }

        public ActionResult ECOSignoff1(string ECOKey, string CardKey)
        {
            var ckdict = CookieUtility.UnpackCookie(this);
            if (!LoginSystem(ckdict, ECOKey, CardKey))
            {
                return RedirectToAction("LoginUser", "User");
            }
            if (string.IsNullOrEmpty(ECOKey))
                ECOKey = ckdict["ECOKey"];
            if (string.IsNullOrEmpty(CardKey))
                CardKey = ckdict["CardKey"];

            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfo(ECOKey);
            if (baseinfos.Count > 0)
            {
                var currentcard = DominoVM.RetrieveSpecialCard(baseinfos[0], DominoCardType.ECOSignoff1);
                if (currentcard.Count > 0)
                {
                    if (string.Compare(currentcard[0].CardStatus, DominoCardStatus.done, true) != 0
                        && !string.IsNullOrEmpty(baseinfos[0].PNDesc))
                    {
                        DominoVM.RefreshQAFAI(baseinfos[0], CardKey, this);
                    }//if card is not finished,we refresh qa folder to get files
                }
                
                var vm = new List<List<DominoVM>>();
                var cardlist = DominoVM.RetrieveECOCards(baseinfos[0]);
                vm.Add(cardlist);

                foreach (var card in cardlist)
                {
                    if (string.Compare(card.CardType, DominoCardType.ECOSignoff1) == 0)
                    {
                        ViewBag.CurrentCard = card;
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
                asilist.AddRange(yesno);
                ViewBag.QAEEPROMCheckList = CreateSelectList(asilist, cardinfo.QAEEPROMCheck);
              
                asilist = new List<string>();
                asilist.AddRange(yesno);
                ViewBag.QALabelCheckList = CreateSelectList(asilist, cardinfo.QALabelCheck);

                var alluser = UserViewModels.RetrieveAllUser();
                asilist = new List<string>();
                asilist.Add("NONE");
                asilist.AddRange(alluser);
                ViewBag.PeerReviewEngineerList = CreateSelectList(asilist, cardinfo.PeerReviewEngineer);

                asilist = new List<string>();
                asilist.AddRange(yesno);
                ViewBag.PeerReviewList = CreateSelectList(asilist, cardinfo.PeerReview);

                asilist = new List<string>();
                asilist.AddRange(yesno);
                ViewBag.ECOAttachmentCheckList = CreateSelectList(asilist, cardinfo.ECOAttachmentCheck);

                asilist = new List<string>();
                asilist.AddRange(yesno);
                ViewBag.MiniPVTCheckList = CreateSelectList(asilist, cardinfo.MiniPVTCheck);

                var facats = new string[] { DominoFACategory.EEPROMFA, DominoFACategory.LABELFA, DominoFACategory.LABELEEPROMFA };
                asilist = new List<string>();
                asilist.AddRange(facats);
                ViewBag.FACategoryList = CreateSelectList(asilist, cardinfo.FACategory);

                ViewBag.ECOKey = ECOKey;
                ViewBag.CardKey = CardKey;
                ViewBag.CardDetailPage = DominoCardType.ECOSignoff1;

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

                StoreAttachAndComment(CardKey, updater, cardinfo);

                cardinfo.UpdateSignoffInfo(CardKey);

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

                DominoVM.UpdateCardStatus(CardKey, DominoCardStatus.done);

                var newcardkey = DominoVM.GetUniqKey();

                var realcardkey = DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.ECOComplete, DominoCardStatus.pending);
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


        public ActionResult ECOComplete(string ECOKey, string CardKey)
        {
            var ckdict = CookieUtility.UnpackCookie(this);
            if (!LoginSystem(ckdict, ECOKey, CardKey))
            {
                return RedirectToAction("LoginUser", "User");
            }
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
                ViewBag.CurrentCard.ECOSubmitDate = cardinfo.ECOSubmitDate;
                ViewBag.CurrentCard.ECOTRApprovedDate = cardinfo.ECOTRApprovedDate;
                ViewBag.CurrentCard.ECOCCBApprovedDate = cardinfo.ECOCCBApprovedDate;
                ViewBag.CurrentCard.ECOCompleteDate = cardinfo.ECOCompleteDate;

                if (!string.IsNullOrEmpty(cardinfo.ECOSubmitDate))
                {
                    try
                    {
                        ViewBag.CurrentCard.ECOSubmitDate = DateTime.Parse(cardinfo.ECOSubmitDate).ToString("yyyy-MM-dd");
                    }
                    catch (Exception ex) { }
                }
                if (!string.IsNullOrEmpty(cardinfo.ECOTRApprovedDate))
                {
                    try
                    {
                        ViewBag.CurrentCard.ECOTRApprovedDate = DateTime.Parse(cardinfo.ECOTRApprovedDate).ToString("yyyy-MM-dd");
                    }
                    catch (Exception ex) { }
                }
                if (!string.IsNullOrEmpty(cardinfo.ECOCCBApprovedDate))
                {
                    try
                    {
                        ViewBag.CurrentCard.ECOCCBApprovedDate = DateTime.Parse(cardinfo.ECOCCBApprovedDate).ToString("yyyy-MM-dd");
                    }
                    catch (Exception ex) { }
                }
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
                cardinfo.ECOSubmitDate = ConvertToDate(Request.Form["ECOSubmitDate"]);
                cardinfo.ECOTRApprovedDate = ConvertToDate(Request.Form["ECOTRApprovedDate"]);
                cardinfo.ECOCCBApprovedDate = ConvertToDate(Request.Form["ECOCCBApprovedDate"]);
                cardinfo.ECOCompleteDate = ConvertToDate(Request.Form["ECOCompleteDate"]);
                cardinfo.UpdateECOCompleteInfo(CardKey);

                var allchecked = true;
                if (string.Compare(cardinfo.ECOCompleted, DominoYESNO.NO) == 0)
                {
                    SetNoticeInfo("ECO should be completed");
                    allchecked = false;
                }
                else if (string.IsNullOrEmpty(cardinfo.ECOSubmitDate))
                {
                    SetNoticeInfo("ECO Submit Date is needed");
                    allchecked = false;
                }
                else if (string.IsNullOrEmpty(cardinfo.ECOTRApprovedDate))
                {
                    SetNoticeInfo("ECO TR Approved Date is needed");
                    allchecked = false;
                }
                else if (string.IsNullOrEmpty(cardinfo.ECOCCBApprovedDate))
                {
                    SetNoticeInfo("ECO CCB Approved Date is needed");
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

                StoreAttachAndComment(CardKey, updater);

                DominoVM.UpdateCardStatus(CardKey, DominoCardStatus.done);

                var newcardkey = DominoVM.GetUniqKey();
                if (string.Compare(baseinfos[0].ECOType, DominoECOType.DVS) == 0
                    || string.Compare(baseinfos[0].ECOType, DominoECOType.RVNS) == 0)
                {
                    var realcardkey = DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.SampleOrdering, DominoCardStatus.working);
                    var dict = new RouteValueDictionary();
                    dict.Add("ECOKey", ECOKey);
                    dict.Add("CardKey", realcardkey);
                    return RedirectToAction(DominoCardType.SampleOrdering, "MiniPIP", dict);
                }
                else if (string.Compare(baseinfos[0].ECOType, DominoECOType.DVNS) == 0)
                {
                    var realcardkey = DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.FACustomerApproval, DominoCardStatus.pending);
                    var dict = new RouteValueDictionary();
                    dict.Add("ECOKey", ECOKey);
                    dict.Add("CardKey", realcardkey);
                    return RedirectToAction(DominoCardType.FACustomerApproval, "MiniPIP", dict);
                }
                else if (string.Compare(baseinfos[0].ECOType, DominoECOType.RVS) == 0)
                {
                    var realcardkey = DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.MiniPIPComplete, DominoCardStatus.pending);
                    var dict = new RouteValueDictionary();
                    dict.Add("ECOKey", ECOKey);
                    dict.Add("CardKey", realcardkey);
                    return RedirectToAction(DominoCardType.MiniPIPComplete, "MiniPIP", dict);
                }
                else
                {
                    var realcardkey = DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.SampleOrdering, DominoCardStatus.working);
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

        public ActionResult FACustomerApproval(string ECOKey, string CardKey)
        {
            var ckdict = CookieUtility.UnpackCookie(this);
            if (!LoginSystem(ckdict, ECOKey, CardKey))
            {
                return RedirectToAction("LoginUser", "User");
            }
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
                    if (string.Compare(card.CardType, DominoCardType.FACustomerApproval) == 0)
                    {
                        ViewBag.CurrentCard = card;
                        break;
                    }
                }

                ViewBag.ECOKey = ECOKey;
                ViewBag.CardKey = CardKey;
                ViewBag.CardDetailPage = DominoCardType.FACustomerApproval;

                return View("CurrentECO", vm);
            }

            return RedirectToAction("ViewAll", "MiniPIP");
        }

        [HttpPost, ActionName("FACustomerApproval")]
        [ValidateAntiForgeryToken]
        public ActionResult FACustomerApprovalPost()
        {
            var ckdict = CookieUtility.UnpackCookie(this);
            var updater = ckdict["logonuser"].Split(new char[] { '|' })[0];

            var ECOKey = Request.Form["ECOKey"];
            var CardKey = Request.Form["CardKey"];

            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfo(ECOKey);
            if (baseinfos.Count > 0)
            {

                StoreAttachAndComment(CardKey, updater);

                DominoVM.UpdateCardStatus(CardKey, DominoCardStatus.done);

                var newcardkey = DominoVM.GetUniqKey();
                if (string.Compare(baseinfos[0].ECOType, DominoECOType.DVNS) == 0)
                {
                    var realcardkey = DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.SampleOrdering, DominoCardStatus.working);
                    var dict = new RouteValueDictionary();
                    dict.Add("ECOKey", ECOKey);
                    dict.Add("CardKey", realcardkey);
                    return RedirectToAction(DominoCardType.SampleOrdering, "MiniPIP", dict);
                }
                else if (string.Compare(baseinfos[0].ECOType, DominoECOType.RVNS) == 0)
                {
                    var realcardkey = DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.ECOComplete, DominoCardStatus.pending);
                    var dict = new RouteValueDictionary();
                    dict.Add("ECOKey", ECOKey);
                    dict.Add("CardKey", realcardkey);
                    return RedirectToAction(DominoCardType.ECOComplete, "MiniPIP", dict);
                }
                else
                {
                    var realcardkey = DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.SampleOrdering, DominoCardStatus.working);
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
                return RedirectToAction(DominoCardType.FACustomerApproval, "MiniPIP", dict);
            }

        }

        public ActionResult ECOSignoff2(string ECOKey, string CardKey)
        {
            var ckdict = CookieUtility.UnpackCookie(this);
            if (!LoginSystem(ckdict, ECOKey, CardKey))
            {
                return RedirectToAction("LoginUser", "User");
            }
            if (string.IsNullOrEmpty(ECOKey))
                ECOKey = ckdict["ECOKey"];
            if (string.IsNullOrEmpty(CardKey))
                CardKey = ckdict["CardKey"];

            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfo(ECOKey);
            if (baseinfos.Count > 0)
            {
                var currentcard = DominoVM.RetrieveSpecialCard(baseinfos[0], DominoCardType.ECOSignoff2);
                if (currentcard.Count > 0)
                {
                    if (string.Compare(currentcard[0].CardStatus, DominoCardStatus.done, true) != 0
                        && !string.IsNullOrEmpty(baseinfos[0].PNDesc))
                    {
                        DominoVM.RefreshQAFAI(baseinfos[0], CardKey, this);
                    }//if card is not finished,we refresh qa folder to get files
                }

                var vm = new List<List<DominoVM>>();
                var cardlist = DominoVM.RetrieveECOCards(baseinfos[0]);
                vm.Add(cardlist);

                foreach (var card in cardlist)
                {
                    if (string.Compare(card.CardType, DominoCardType.ECOSignoff2) == 0)
                    {
                        ViewBag.CurrentCard = card;
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
                asilist.AddRange(yesno);
                ViewBag.QAEEPROMCheckList = CreateSelectList(asilist, cardinfo.QAEEPROMCheck);

                asilist = new List<string>();
                asilist.AddRange(yesno);
                ViewBag.QALabelCheckList = CreateSelectList(asilist, cardinfo.QALabelCheck);

                var alluser = UserViewModels.RetrieveAllUser();
                asilist = new List<string>();
                asilist.Add("NONE");
                asilist.AddRange(alluser);
                ViewBag.PeerReviewEngineerList = CreateSelectList(asilist, cardinfo.PeerReviewEngineer);

                asilist = new List<string>();
                asilist.AddRange(yesno);
                ViewBag.PeerReviewList = CreateSelectList(asilist, cardinfo.PeerReview);

                asilist = new List<string>();
                asilist.AddRange(yesno);
                ViewBag.ECOAttachmentCheckList = CreateSelectList(asilist, cardinfo.ECOAttachmentCheck);

                asilist = new List<string>();
                asilist.AddRange(yesno);
                ViewBag.MiniPVTCheckList = CreateSelectList(asilist, cardinfo.MiniPVTCheck);

                var facats = new string[] { DominoFACategory.EEPROMFA, DominoFACategory.LABELFA, DominoFACategory.LABELEEPROMFA };
                asilist = new List<string>();
                asilist.AddRange(facats);
                ViewBag.FACategoryList = CreateSelectList(asilist, cardinfo.FACategory);

                ViewBag.ECOKey = ECOKey;
                ViewBag.CardKey = CardKey;
                ViewBag.CardDetailPage = DominoCardType.ECOSignoff2;

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

                StoreAttachAndComment(CardKey, updater, cardinfo);

                cardinfo.UpdateSignoffInfo(CardKey);

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
                    return RedirectToAction(DominoCardType.ECOSignoff2, "MiniPIP", dict1);
                }

                DominoVM.UpdateCardStatus(CardKey, DominoCardStatus.done);

                var newcardkey = DominoVM.GetUniqKey();
                var realcardkey = DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.CustomerApprovalHold, DominoCardStatus.pending);

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


        public ActionResult CustomerApprovalHold(string ECOKey, string CardKey)
        {
            var ckdict = CookieUtility.UnpackCookie(this);
            if (!LoginSystem(ckdict, ECOKey, CardKey))
            {
                return RedirectToAction("LoginUser", "User");
            }
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
                ViewBag.CurrentCard.CustomerApproveHoldDate = cardinfo.CustomerApproveHoldDate;
                if (!string.IsNullOrEmpty(cardinfo.CustomerApproveHoldDate))
                {
                    try
                    {
                        ViewBag.CurrentCard.CustomerApproveHoldDate = DateTime.Parse(cardinfo.CustomerApproveHoldDate).ToString("yyyy-MM-dd");
                    }
                    catch (Exception ex) { }
                }


                ViewBag.ECOKey = ECOKey;
                ViewBag.CardKey = CardKey;
                ViewBag.CardDetailPage = DominoCardType.CustomerApprovalHold;

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
                cardinfo.CustomerApproveHoldDate = Request.Form["CustomerApproveHoldDate"];
                cardinfo.UpdateCustomerApproveHoldInfo(CardKey);

                if (string.IsNullOrEmpty(cardinfo.CustomerApproveHoldDate))
                {
                    SetNoticeInfo("Customer Aprrove Hold Date should not be empty");
                    var dict = new RouteValueDictionary();
                    dict.Add("ECOKey", ECOKey);
                    dict.Add("CardKey", CardKey);
                    return RedirectToAction(DominoCardType.CustomerApprovalHold, "MiniPIP", dict);
                }

                DominoVM.UpdateCardStatus(CardKey, DominoCardStatus.done);

                var newcardkey = DominoVM.GetUniqKey();

                if (string.Compare(baseinfos[0].ECOType, DominoECOType.RVS) == 0)
                {
                    var realcardkey = DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.SampleOrdering, DominoCardStatus.working);
                    var dict = new RouteValueDictionary();
                    dict.Add("ECOKey", ECOKey);
                    dict.Add("CardKey", realcardkey);
                    return RedirectToAction(DominoCardType.SampleOrdering, "MiniPIP", dict);
                }
                else if (string.Compare(baseinfos[0].ECOType, DominoECOType.RVNS) == 0)
                {
                    var realcardkey = DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.FACustomerApproval, DominoCardStatus.pending);
                    var dict = new RouteValueDictionary();
                    dict.Add("ECOKey", ECOKey);
                    dict.Add("CardKey", realcardkey);
                    return RedirectToAction(DominoCardType.FACustomerApproval, "MiniPIP", dict);
                }
                else
                {
                    var realcardkey = DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.SampleOrdering, DominoCardStatus.working);
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


        public ActionResult SampleOrdering(string ECOKey, string CardKey)
        {
            var ckdict = CookieUtility.UnpackCookie(this);
            if (!LoginSystem(ckdict, ECOKey, CardKey))
            {
                return RedirectToAction("LoginUser", "User");
            }
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
                    if (string.Compare(card.CardType, DominoCardType.SampleOrdering) == 0)
                    {
                        ViewBag.CurrentCard = card;
                        break;
                    }
                }

                ViewBag.ECOKey = ECOKey;
                ViewBag.CardKey = CardKey;
                ViewBag.CardDetailPage = DominoCardType.SampleOrdering;

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

                DominoVM.UpdateCardStatus(CardKey, DominoCardStatus.done);

                var newcardkey = DominoVM.GetUniqKey();
                var realcardkey = DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.SampleBuilding, DominoCardStatus.pending);

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


        public ActionResult SampleBuilding(string ECOKey, string CardKey)
        {
            var ckdict = CookieUtility.UnpackCookie(this);
            if (!LoginSystem(ckdict, ECOKey, CardKey))
            {
                return RedirectToAction("LoginUser", "User");
            }
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
                    if (string.Compare(card.CardType, DominoCardType.SampleBuilding) == 0)
                    {
                        ViewBag.CurrentCard = card;
                        break;
                    }
                }

                ViewBag.ECOKey = ECOKey;
                ViewBag.CardKey = CardKey;
                ViewBag.CardDetailPage = DominoCardType.SampleBuilding;

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

                DominoVM.UpdateCardStatus(CardKey, DominoCardStatus.done);

                var newcardkey = DominoVM.GetUniqKey();
                var realcardkey = DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.SampleShipment, DominoCardStatus.pending);

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


        public ActionResult SampleShipment(string ECOKey, string CardKey)
        {
            var ckdict = CookieUtility.UnpackCookie(this);
            if (!LoginSystem(ckdict, ECOKey, CardKey))
            {
                return RedirectToAction("LoginUser", "User");
            }
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
                    if (string.Compare(card.CardType, DominoCardType.SampleShipment) == 0)
                    {
                        ViewBag.CurrentCard = card;
                        break;
                    }
                }

                ViewBag.ECOKey = ECOKey;
                ViewBag.CardKey = CardKey;
                ViewBag.CardDetailPage = DominoCardType.SampleShipment;

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

                DominoVM.UpdateCardStatus(CardKey, DominoCardStatus.done);

                var newcardkey = DominoVM.GetUniqKey();
                var realcardkey = DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.SampleCustomerApproval, DominoCardStatus.pending);

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

        public ActionResult SampleCustomerApproval(string ECOKey, string CardKey)
        {
            var ckdict = CookieUtility.UnpackCookie(this);
            if (!LoginSystem(ckdict, ECOKey, CardKey))
            {
                return RedirectToAction("LoginUser", "User");
            }
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

                ViewBag.ECOKey = ECOKey;
                ViewBag.CardKey = CardKey;
                ViewBag.CardDetailPage = DominoCardType.SampleCustomerApproval;

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

                DominoVM.UpdateCardStatus(CardKey, DominoCardStatus.done);

                var newcardkey = DominoVM.GetUniqKey();

                if (string.Compare(baseinfos[0].ECOType, DominoECOType.RVS) == 0)
                {
                    var realcardkey = DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.ECOComplete, DominoCardStatus.pending);
                    var dict = new RouteValueDictionary();
                    dict.Add("ECOKey", ECOKey);
                    dict.Add("CardKey", realcardkey);
                    return RedirectToAction(DominoCardType.ECOComplete, "MiniPIP", dict);
                }
                else
                {
                    var realcardkey = DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.MiniPIPComplete, DominoCardStatus.pending);
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


        public ActionResult MiniPIPComplete(string ECOKey, string CardKey)
        {
            var ckdict = CookieUtility.UnpackCookie(this);
            if (!LoginSystem(ckdict, ECOKey, CardKey))
            {
                return RedirectToAction("LoginUser", "User");
            }
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

                ViewBag.ECOKey = ECOKey;
                ViewBag.CardKey = CardKey;
                ViewBag.CardDetailPage = DominoCardType.MiniPIPComplete;

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
                if (!string.IsNullOrEmpty(baseinfos[0].MCOIssued))
                {
                    DominoVM.UpdateCardStatus(CardKey, DominoCardStatus.done);
                }
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

    }
}