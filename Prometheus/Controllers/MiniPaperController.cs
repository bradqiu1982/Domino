using System;
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
    public class MiniPaperController : Controller
    {
        // GET: MiniPapers
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
                ck.Add("logonredirectctrl", "MiniPaper");
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
            DominoVM.RefreshSystem(this);
            return RedirectToAction("ViewAll", "MiniPaper");
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
                ck.Add("logonredirectctrl", "MiniPaper");
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

                foreach (var card in cardlist)
                {
                    if (string.Compare(card.CardType, DominoCardType.ECOPending) == 0)
                    {
                        ViewBag.CurrentCard = card;
                        break;
                    }
                }

                ViewBag.ECOKey = ECOKey;
                ViewBag.CardKey = CardKey;
                ViewBag.CardDetailPage = DominoCardType.ECOPending;
                
                return View("SingalECO", vm);
            }

            return RedirectToAction("ViewAll", "MiniPaper");
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

        private void StoreAttachAndComment(string CardKey,string updater)
        {
            if (!string.IsNullOrEmpty(Request.Form["attachmentupload"]))
            {
                var urls = ReceiveRMAFiles();
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

            if (!string.IsNullOrEmpty(Request.Form["commenteditor"]))
            {
                var rootcause = Server.HtmlDecode(Request.Form["commenteditor"]);
                var dbstr = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(rootcause));
                DominoVM.StoreCardComment(CardKey, dbstr, updater, DateTime.Now.ToString());
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

                baseinfos[0].UpdateECO();

                StoreAttachAndComment(CardKey, updater);

                DominoVM.UpdateCardStatus(ECOKey, CardKey, DominoCardStatus.done);

                var newcardkey = DominoVM.GetUniqKey();

                if (string.Compare(baseinfos[0].ECOType, DominoECOType.DVS) == 0
                    || string.Compare(baseinfos[0].ECOType, DominoECOType.DVNS) == 0)
                {
                        var realcardkey = DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.ECOSignoff1, DominoCardStatus.pending);
                        var dict = new RouteValueDictionary();
                        dict.Add("ECOKey", ECOKey);
                        dict.Add("CardKey", realcardkey);
                        return RedirectToAction(DominoCardType.ECOSignoff1, "MiniPaper", dict);
                }
                else if (string.Compare(baseinfos[0].ECOType, DominoECOType.RVS) == 0
                    || string.Compare(baseinfos[0].ECOType, DominoECOType.RVNS) == 0)
                {
                        var realcardkey = DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.ECOSignoff2, DominoCardStatus.pending);
                        var dict = new RouteValueDictionary();
                        dict.Add("ECOKey", ECOKey);
                        dict.Add("CardKey", realcardkey);
                        return RedirectToAction(DominoCardType.ECOSignoff2, "MiniPaper", dict);
                }
                else
                {
                        var realcardkey = DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.ECOSignoff1, DominoCardStatus.pending);
                        var dict = new RouteValueDictionary();
                        dict.Add("ECOKey", ECOKey);
                        dict.Add("CardKey", realcardkey);
                        return RedirectToAction(DominoCardType.ECOSignoff1, "MiniPaper", dict);
                }
            }
            else
            {
                var dict = new RouteValueDictionary();
                dict.Add("ECOKey", ECOKey);
                dict.Add("CardKey", CardKey);
                return RedirectToAction(DominoCardType.ECOPending, "MiniPaper", dict);
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

                ViewBag.ECOKey = ECOKey;
                ViewBag.CardKey = CardKey;
                ViewBag.CardDetailPage = DominoCardType.ECOSignoff1;

                return View("SingalECO", vm);
            }

            return RedirectToAction("ViewAll", "MiniPaper");

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

                StoreAttachAndComment(CardKey, updater);

                DominoVM.UpdateCardStatus(ECOKey, CardKey, DominoCardStatus.done);

                var newcardkey = DominoVM.GetUniqKey();

                var realcardkey = DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.ECOComplete, DominoCardStatus.pending);
                var dict = new RouteValueDictionary();
                dict.Add("ECOKey", ECOKey);
                dict.Add("CardKey", realcardkey);
                return RedirectToAction(DominoCardType.ECOComplete, "MiniPaper", dict);
            }
            else
            {
                var dict = new RouteValueDictionary();
                dict.Add("ECOKey", ECOKey);
                dict.Add("CardKey", CardKey);
                return RedirectToAction(DominoCardType.ECOSignoff1, "MiniPaper", dict);
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

                ViewBag.ECOKey = ECOKey;
                ViewBag.CardKey = CardKey;
                ViewBag.CardDetailPage = DominoCardType.ECOComplete;

                return View("SingalECO", vm);
            }

            return RedirectToAction("ViewAll", "MiniPaper");

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

                StoreAttachAndComment(CardKey, updater);

                DominoVM.UpdateCardStatus(ECOKey, CardKey, DominoCardStatus.done);

                var newcardkey = DominoVM.GetUniqKey();
                if (string.Compare(baseinfos[0].ECOType, DominoECOType.DVS) == 0
                    || string.Compare(baseinfos[0].ECOType, DominoECOType.RVNS) == 0)
                {
                    var realcardkey = DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.SampleOrdering, DominoCardStatus.pending);
                    var dict = new RouteValueDictionary();
                    dict.Add("ECOKey", ECOKey);
                    dict.Add("CardKey", realcardkey);
                    return RedirectToAction(DominoCardType.SampleOrdering, "MiniPaper", dict);
                }
                else if (string.Compare(baseinfos[0].ECOType, DominoECOType.DVNS) == 0)
                {
                    var realcardkey = DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.FACustomerApproval, DominoCardStatus.pending);
                    var dict = new RouteValueDictionary();
                    dict.Add("ECOKey", ECOKey);
                    dict.Add("CardKey", realcardkey);
                    return RedirectToAction(DominoCardType.FACustomerApproval, "MiniPaper", dict);
                }
                else if (string.Compare(baseinfos[0].ECOType, DominoECOType.RVS) == 0)
                {
                    var realcardkey = DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.MiniPIPComplete, DominoCardStatus.pending);
                    var dict = new RouteValueDictionary();
                    dict.Add("ECOKey", ECOKey);
                    dict.Add("CardKey", realcardkey);
                    return RedirectToAction(DominoCardType.MiniPIPComplete, "MiniPaper", dict);
                }
                else
                {
                    var realcardkey = DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.SampleOrdering, DominoCardStatus.pending);
                    var dict = new RouteValueDictionary();
                    dict.Add("ECOKey", ECOKey);
                    dict.Add("CardKey", realcardkey);
                    return RedirectToAction(DominoCardType.SampleOrdering, "MiniPaper", dict);
                }
            }
            else
            {
                var dict = new RouteValueDictionary();
                dict.Add("ECOKey", ECOKey);
                dict.Add("CardKey", CardKey);
                return RedirectToAction(DominoCardType.ECOComplete, "MiniPaper", dict);
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

                return View("SingalECO", vm);
            }

            return RedirectToAction("ViewAll", "MiniPaper");
        }

        [HttpPost, ActionName("FACustomerApproval")]
        [ValidateAntiForgeryToken]
        public ActionResult FACustomerApprovalPost()
        {
            var ckdict = CookieUtility.UnpackCookie(this);
            var updater = ckdict["logonuser"].Split(new char[] { '|' })[0];

            var ECOKey = Request.Form["ECOKey"];
            var CardKey = Request.Form["CardKey"];
            DominoVM.UpdateCardStatus(ECOKey, CardKey, DominoCardStatus.done);

            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfo(ECOKey);

            var newcardkey = DominoVM.GetUniqKey();
            if (string.Compare(baseinfos[0].ECOType, DominoECOType.DVNS) == 0)
            {
                var realcardkey = DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.SampleOrdering, DominoCardStatus.pending);
                var dict = new RouteValueDictionary();
                dict.Add("ECOKey", ECOKey);
                dict.Add("CardKey", realcardkey);
                return RedirectToAction(DominoCardType.SampleOrdering, "MiniPaper", dict);
            }
            else if (string.Compare(baseinfos[0].ECOType, DominoECOType.RVNS) == 0)
            {
                var realcardkey = DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.ECOComplete, DominoCardStatus.pending);
                var dict = new RouteValueDictionary();
                dict.Add("ECOKey", ECOKey);
                dict.Add("CardKey", realcardkey);
                return RedirectToAction(DominoCardType.ECOComplete, "MiniPaper", dict);
            }
            else
            {
                var realcardkey = DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.SampleOrdering, DominoCardStatus.pending);
                var dict = new RouteValueDictionary();
                dict.Add("ECOKey", ECOKey);
                dict.Add("CardKey", realcardkey);
                return RedirectToAction(DominoCardType.SampleOrdering, "MiniPaper", dict);
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

                ViewBag.ECOKey = ECOKey;
                ViewBag.CardKey = CardKey;
                ViewBag.CardDetailPage = DominoCardType.ECOSignoff2;

                return View("SingalECO", vm);
            }

            return RedirectToAction("ViewAll", "MiniPaper");

        }

        [HttpPost, ActionName("ECOSignoff2")]
        [ValidateAntiForgeryToken]
        public ActionResult ECOSignoff2Post()
        {
            var ckdict = CookieUtility.UnpackCookie(this);
            var updater = ckdict["logonuser"].Split(new char[] { '|' })[0];

            var ECOKey = Request.Form["ECOKey"];
            var CardKey = Request.Form["CardKey"];
            DominoVM.UpdateCardStatus(ECOKey, CardKey, DominoCardStatus.done);

            var newcardkey = DominoVM.GetUniqKey();
            var realcardkey = DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.CustomerApprovalHold, DominoCardStatus.pending);

            var dict = new RouteValueDictionary();
            dict.Add("ECOKey", ECOKey);
            dict.Add("CardKey", realcardkey);
            return RedirectToAction(DominoCardType.CustomerApprovalHold, "MiniPaper", dict);
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

                ViewBag.ECOKey = ECOKey;
                ViewBag.CardKey = CardKey;
                ViewBag.CardDetailPage = DominoCardType.CustomerApprovalHold;

                return View("SingalECO", vm);
            }

            return RedirectToAction("ViewAll", "MiniPaper");

        }

        [HttpPost, ActionName("CustomerApprovalHold")]
        [ValidateAntiForgeryToken]
        public ActionResult CustomerApprovalHoldPost()
        {
            var ckdict = CookieUtility.UnpackCookie(this);
            var updater = ckdict["logonuser"].Split(new char[] { '|' })[0];

            var ECOKey = Request.Form["ECOKey"];
            var CardKey = Request.Form["CardKey"];
            DominoVM.UpdateCardStatus(ECOKey, CardKey, DominoCardStatus.done);

            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfo(ECOKey);

            var newcardkey = DominoVM.GetUniqKey();

            if (string.Compare(baseinfos[0].ECOType, DominoECOType.RVS) == 0)
            {
                var realcardkey = DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.SampleOrdering, DominoCardStatus.pending);
                var dict = new RouteValueDictionary();
                dict.Add("ECOKey", ECOKey);
                dict.Add("CardKey", realcardkey);
                return RedirectToAction(DominoCardType.SampleOrdering, "MiniPaper", dict);
            }
            else if (string.Compare(baseinfos[0].ECOType, DominoECOType.RVNS) == 0)
            {
                var realcardkey = DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.FACustomerApproval, DominoCardStatus.pending);
                var dict = new RouteValueDictionary();
                dict.Add("ECOKey", ECOKey);
                dict.Add("CardKey", realcardkey);
                return RedirectToAction(DominoCardType.FACustomerApproval, "MiniPaper", dict);
            }
            else
            {
                var realcardkey = DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.SampleOrdering, DominoCardStatus.pending);
                var dict = new RouteValueDictionary();
                dict.Add("ECOKey", ECOKey);
                dict.Add("CardKey", realcardkey);
                return RedirectToAction(DominoCardType.SampleOrdering, "MiniPaper", dict);
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

                return View("SingalECO", vm);
            }

            return RedirectToAction("ViewAll", "MiniPaper");

        }

        [HttpPost, ActionName("SampleOrdering")]
        [ValidateAntiForgeryToken]
        public ActionResult SampleOrderingPost()
        {
            var ckdict = CookieUtility.UnpackCookie(this);
            var updater = ckdict["logonuser"].Split(new char[] { '|' })[0];

            var ECOKey = Request.Form["ECOKey"];
            var CardKey = Request.Form["CardKey"];
            DominoVM.UpdateCardStatus(ECOKey, CardKey, DominoCardStatus.done);

            var newcardkey = DominoVM.GetUniqKey();
            var realcardkey = DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.SampleBuilding, DominoCardStatus.pending);

            var dict = new RouteValueDictionary();
            dict.Add("ECOKey", ECOKey);
            dict.Add("CardKey", realcardkey);
            return RedirectToAction(DominoCardType.SampleBuilding, "MiniPaper", dict);
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

                return View("SingalECO", vm);
            }

            return RedirectToAction("ViewAll", "MiniPaper");

        }

        [HttpPost, ActionName("SampleBuilding")]
        [ValidateAntiForgeryToken]
        public ActionResult SampleBuildingPost()
        {
            var ckdict = CookieUtility.UnpackCookie(this);
            var updater = ckdict["logonuser"].Split(new char[] { '|' })[0];

            var ECOKey = Request.Form["ECOKey"];
            var CardKey = Request.Form["CardKey"];
            DominoVM.UpdateCardStatus(ECOKey, CardKey, DominoCardStatus.done);

            var newcardkey = DominoVM.GetUniqKey();
            var realcardkey = DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.SampleShipment, DominoCardStatus.pending);

            var dict = new RouteValueDictionary();
            dict.Add("ECOKey", ECOKey);
            dict.Add("CardKey", realcardkey);
            return RedirectToAction(DominoCardType.SampleShipment, "MiniPaper", dict);
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

                return View("SingalECO", vm);
            }

            return RedirectToAction("ViewAll", "MiniPaper");

        }

        [HttpPost, ActionName("SampleShipment")]
        [ValidateAntiForgeryToken]
        public ActionResult SampleShipmentPost()
        {
            var ckdict = CookieUtility.UnpackCookie(this);
            var updater = ckdict["logonuser"].Split(new char[] { '|' })[0];

            var ECOKey = Request.Form["ECOKey"];
            var CardKey = Request.Form["CardKey"];
            DominoVM.UpdateCardStatus(ECOKey, CardKey, DominoCardStatus.done);

            var newcardkey = DominoVM.GetUniqKey();
            var realcardkey = DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.SampleCustomerApproval, DominoCardStatus.pending);

            var dict = new RouteValueDictionary();
            dict.Add("ECOKey", ECOKey);
            dict.Add("CardKey", realcardkey);
            return RedirectToAction(DominoCardType.SampleCustomerApproval, "MiniPaper", dict);
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

                return View("SingalECO", vm);
            }

            return RedirectToAction("ViewAll", "MiniPaper");

        }

        [HttpPost, ActionName("SampleCustomerApproval")]
        [ValidateAntiForgeryToken]
        public ActionResult SampleCustomerApprovalPost()
        {
            var ckdict = CookieUtility.UnpackCookie(this);
            var updater = ckdict["logonuser"].Split(new char[] { '|' })[0];

            var ECOKey = Request.Form["ECOKey"];
            var CardKey = Request.Form["CardKey"];
            DominoVM.UpdateCardStatus(ECOKey, CardKey, DominoCardStatus.done);

            var newcardkey = DominoVM.GetUniqKey();
            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfo(ECOKey);

            if (string.Compare(baseinfos[0].ECOType, DominoECOType.RVS) == 0)
            {
                var realcardkey = DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.ECOComplete, DominoCardStatus.pending);
                var dict = new RouteValueDictionary();
                dict.Add("ECOKey", ECOKey);
                dict.Add("CardKey", realcardkey);
                return RedirectToAction(DominoCardType.ECOComplete, "MiniPaper", dict);
            }
            else
            {
                var realcardkey = DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.MiniPIPComplete, DominoCardStatus.pending);
                var dict = new RouteValueDictionary();
                dict.Add("ECOKey", ECOKey);
                dict.Add("CardKey", realcardkey);
                return RedirectToAction(DominoCardType.MiniPIPComplete, "MiniPaper", dict);
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

                return View("SingalECO", vm);
            }

            return RedirectToAction("ViewAll", "MiniPaper");

        }

        [HttpPost, ActionName("MiniPIPComplete")]
        [ValidateAntiForgeryToken]
        public ActionResult MiniPIPCompletePost()
        {
            var ckdict = CookieUtility.UnpackCookie(this);
            var updater = ckdict["logonuser"].Split(new char[] { '|' })[0];

            var ECOKey = Request.Form["ECOKey"];
            var CardKey = Request.Form["CardKey"];
            DominoVM.UpdateCardStatus(ECOKey, CardKey, DominoCardStatus.done);


            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfo(ECOKey);
            var vm = new List<List<DominoVM>>();
            foreach (var item in baseinfos)
            {
                var templist = DominoVM.RetrieveECOCards(item);
                vm.Add(templist);
            }
            ViewBag.CardDetailPage = DominoCardType.MiniPIPComplete;
            ViewBag.ECOKey = ECOKey;
            ViewBag.CardKey = CardKey;

            return View("SingalECO", vm);
        }

    }
}