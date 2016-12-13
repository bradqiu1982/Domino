using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Domino.Models;
using System.Reflection;
using System.Web.Routing;

namespace Domino.Controllers
{
    public class MiniPaperController : Controller
    {
        // GET: MiniPapers
        public ActionResult ViewAll()
        {
            DominoVM.CleanDB();

            var baseinfo = new ECOBaseInfo();
            baseinfo.ECOKey = DominoVM.GetUniqKey();
            baseinfo.ECONum = "97807";
            baseinfo.PNDesc = "FCBG410QB1C10-FC";
            baseinfo.Customer = "MRV";
            baseinfo.PE = "Jessica Zheng";
            baseinfo.CreateECO();

            DominoVM.CreateCard(baseinfo.ECOKey, DominoVM.GetUniqKey(), DominoCardType.ECOPending, DominoCardStatus.pending);


            var baseinfos = ECOBaseInfo.RetrieveAllECOBaseInfo();
            var vm = new List<List<DominoVM>>();
            foreach (var item in baseinfos)
            {
                var templist = DominoVM.RetrieveECOCards(item);
                vm.Add(templist);
            }

            return View(vm);
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



        public ActionResult ECOPending(string ECOKey, string CardKey)
        {
            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfo(ECOKey);
            var vm = new List<List<DominoVM>>();
            foreach (var item in baseinfos)
            {
                var templist = DominoVM.RetrieveECOCards(item);
                vm.Add(templist);
            }

            var ecotypes = new string[] { DominoECOType.DVS,DominoECOType.DVNS,DominoECOType.RVS,DominoECOType.RVNS };
            var asilist = new List<string>();
            asilist.AddRange(ecotypes);
            ViewBag.ECOTypeList = CreateSelectList(asilist, "");

            ViewBag.CardDetailPage = DominoCardType.ECOPending;
            ViewBag.ECOKey = ECOKey;
            ViewBag.CardKey = CardKey;

            return View("SingalECO", vm);
        }

        [HttpPost, ActionName("ECOPending")]
        [ValidateAntiForgeryToken]
        public ActionResult ECOPendingPost()
        {
            var ECOKey = Request.Form["ECOKey"];
            var CardKey = Request.Form["CardKey"];
            var ECOType = Request.Form["ECOTypeList"].ToString();

            DominoVM.UpdateCard(ECOKey, CardKey, DominoCardStatus.done);

            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfo(ECOKey);
            if (baseinfos.Count > 0)
            {
                baseinfos[0].ECOType = ECOType;
                if (string.Compare(ECOType,DominoECOType.DVS) == 0
                    || string.Compare(ECOType, DominoECOType.RVS) == 0)
                {
                    baseinfos[0].FirstArticleNeed = DominoFirstArticle.FirstArticleNeed;
                }
                else if (string.Compare(ECOType,DominoECOType.DVNS) == 0
                    ||string.Compare(ECOType, DominoECOType.RVNS) == 0)
                {
                    baseinfos[0].FirstArticleNeed = DominoFirstArticle.NoFirstArticleNeed;
                }
                else
                {
                    baseinfos[0].FirstArticleNeed = DominoFirstArticle.FirstArticleNeed;
                }
                baseinfos[0].UpdateECO();
            }

            var newcardkey = DominoVM.GetUniqKey();

            if (string.Compare(ECOType, DominoECOType.DVS) == 0
                || string.Compare(ECOType, DominoECOType.DVNS) == 0)
            {
                DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.ECOSignoff1, DominoCardStatus.pending);
                var dict = new RouteValueDictionary();
                dict.Add("ECOKey", ECOKey);
                dict.Add("CardKey", newcardkey);
                return RedirectToAction(DominoCardType.ECOSignoff1, "MiniPaper", dict);
            }
            else if (string.Compare(ECOType, DominoECOType.RVS) == 0
                || string.Compare(ECOType, DominoECOType.RVNS) == 0)
            {
                DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.ECOSignoff2, DominoCardStatus.pending);
                var dict = new RouteValueDictionary();
                dict.Add("ECOKey", ECOKey);
                dict.Add("CardKey", newcardkey);
                return RedirectToAction(DominoCardType.ECOSignoff2, "MiniPaper", dict);
            }
            else
            {
                DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.ECOSignoff1, DominoCardStatus.pending);
                var dict = new RouteValueDictionary();
                dict.Add("ECOKey", ECOKey);
                dict.Add("CardKey", newcardkey);
                return RedirectToAction(DominoCardType.ECOSignoff1, "MiniPaper", dict);
            }
        }

        public ActionResult ECOSignoff1(string ECOKey, string CardKey)
        {
            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfo(ECOKey);
            var vm = new List<List<DominoVM>>();
            foreach (var item in baseinfos)
            {
                var templist = DominoVM.RetrieveECOCards(item);
                vm.Add(templist);
            }

            ViewBag.CardDetailPage = DominoCardType.ECOSignoff1;
            ViewBag.ECOKey = ECOKey;
            ViewBag.CardKey = CardKey;

            return View("SingalECO", vm);

        }

        [HttpPost, ActionName("ECOSignoff1")]
        [ValidateAntiForgeryToken]
        public ActionResult ECOSignoff1Post()
        {
            var ECOKey = Request.Form["ECOKey"];
            var CardKey = Request.Form["CardKey"];
            DominoVM.UpdateCard(ECOKey, CardKey, DominoCardStatus.done);

            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfo(ECOKey);
            if (baseinfos.Count > 0)
            {
                baseinfos[0].InitRevison = "4/13/2016";
                baseinfos[0].UpdateECO();
            }

            var newcardkey = DominoVM.GetUniqKey();
            DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.ECOComplete, DominoCardStatus.pending);

            var dict = new RouteValueDictionary();
            dict.Add("ECOKey", ECOKey);
            dict.Add("CardKey", newcardkey);
            return RedirectToAction(DominoCardType.ECOComplete, "MiniPaper", dict);
        }


        public ActionResult ECOComplete(string ECOKey, string CardKey)
        {
            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfo(ECOKey);
            var vm = new List<List<DominoVM>>();
            foreach (var item in baseinfos)
            {
                var templist = DominoVM.RetrieveECOCards(item);
                vm.Add(templist);
            }

            ViewBag.CardDetailPage = DominoCardType.ECOComplete;
            ViewBag.ECOKey = ECOKey;
            ViewBag.CardKey = CardKey;

            return View("SingalECO", vm);
        }

        [HttpPost, ActionName("ECOComplete")]
        [ValidateAntiForgeryToken]
        public ActionResult ECOCompletePost()
        {
            var ECOKey = Request.Form["ECOKey"];
            var CardKey = Request.Form["CardKey"];
            DominoVM.UpdateCard(ECOKey, CardKey, DominoCardStatus.done);

            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfo(ECOKey);

            var newcardkey = DominoVM.GetUniqKey();
            if (string.Compare(baseinfos[0].ECOType, DominoECOType.DVS) == 0
                || string.Compare(baseinfos[0].ECOType, DominoECOType.RVNS) == 0)
            {
                DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.SampleOrdering, DominoCardStatus.pending);
                var dict = new RouteValueDictionary();
                dict.Add("ECOKey", ECOKey);
                dict.Add("CardKey", newcardkey);
                return RedirectToAction(DominoCardType.SampleOrdering, "MiniPaper", dict);
            }
            else if (string.Compare(baseinfos[0].ECOType, DominoECOType.DVNS) == 0)
            {
                DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.FACustomerApproval, DominoCardStatus.pending);
                var dict = new RouteValueDictionary();
                dict.Add("ECOKey", ECOKey);
                dict.Add("CardKey", newcardkey);
                return RedirectToAction(DominoCardType.FACustomerApproval, "MiniPaper", dict);
            }
            else if (string.Compare(baseinfos[0].ECOType, DominoECOType.RVS) == 0)
            {
                DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.MiniPIPComplete, DominoCardStatus.pending);
                var dict = new RouteValueDictionary();
                dict.Add("ECOKey", ECOKey);
                dict.Add("CardKey", newcardkey);
                return RedirectToAction(DominoCardType.MiniPIPComplete, "MiniPaper", dict);
            }
            else
            {
                DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.SampleOrdering, DominoCardStatus.pending);
                var dict = new RouteValueDictionary();
                dict.Add("ECOKey", ECOKey);
                dict.Add("CardKey", newcardkey);
                return RedirectToAction(DominoCardType.SampleOrdering, "MiniPaper", dict);
            }

        }

        public ActionResult FACustomerApproval(string ECOKey, string CardKey)
        {
            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfo(ECOKey);
            var vm = new List<List<DominoVM>>();
            foreach (var item in baseinfos)
            {
                var templist = DominoVM.RetrieveECOCards(item);
                vm.Add(templist);
            }

            ViewBag.CardDetailPage = DominoCardType.FACustomerApproval;
            ViewBag.ECOKey = ECOKey;
            ViewBag.CardKey = CardKey;

            return View("SingalECO", vm);
        }

        [HttpPost, ActionName("FACustomerApproval")]
        [ValidateAntiForgeryToken]
        public ActionResult FACustomerApprovalPost()
        {
            var ECOKey = Request.Form["ECOKey"];
            var CardKey = Request.Form["CardKey"];
            DominoVM.UpdateCard(ECOKey, CardKey, DominoCardStatus.done);

            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfo(ECOKey);

            var newcardkey = DominoVM.GetUniqKey();
            if (string.Compare(baseinfos[0].ECOType, DominoECOType.DVNS) == 0)
            {
                DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.SampleOrdering, DominoCardStatus.pending);
                var dict = new RouteValueDictionary();
                dict.Add("ECOKey", ECOKey);
                dict.Add("CardKey", newcardkey);
                return RedirectToAction(DominoCardType.SampleOrdering, "MiniPaper", dict);
            }
            else if (string.Compare(baseinfos[0].ECOType, DominoECOType.RVNS) == 0)
            {
                DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.ECOComplete, DominoCardStatus.pending);
                var dict = new RouteValueDictionary();
                dict.Add("ECOKey", ECOKey);
                dict.Add("CardKey", newcardkey);
                return RedirectToAction(DominoCardType.ECOComplete, "MiniPaper", dict);
            }
            else
            {
                DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.SampleOrdering, DominoCardStatus.pending);
                var dict = new RouteValueDictionary();
                dict.Add("ECOKey", ECOKey);
                dict.Add("CardKey", newcardkey);
                return RedirectToAction(DominoCardType.SampleOrdering, "MiniPaper", dict);
            }

        }

        public ActionResult ECOSignoff2(string ECOKey, string CardKey)
        {
            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfo(ECOKey);
            var vm = new List<List<DominoVM>>();
            foreach (var item in baseinfos)
            {
                var templist = DominoVM.RetrieveECOCards(item);
                vm.Add(templist);
            }

            ViewBag.CardDetailPage = DominoCardType.ECOSignoff2;
            ViewBag.ECOKey = ECOKey;
            ViewBag.CardKey = CardKey;

            return View("SingalECO", vm);

        }

        [HttpPost, ActionName("ECOSignoff2")]
        [ValidateAntiForgeryToken]
        public ActionResult ECOSignoff2Post()
        {
            var ECOKey = Request.Form["ECOKey"];
            var CardKey = Request.Form["CardKey"];
            DominoVM.UpdateCard(ECOKey, CardKey, DominoCardStatus.done);

            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfo(ECOKey);
            if (baseinfos.Count > 0)
            {
                baseinfos[0].InitRevison = "4/13/2016";
                baseinfos[0].UpdateECO();
            }

            var newcardkey = DominoVM.GetUniqKey();
            DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.CustomerApprovalHold, DominoCardStatus.pending);

            var dict = new RouteValueDictionary();
            dict.Add("ECOKey", ECOKey);
            dict.Add("CardKey", newcardkey);
            return RedirectToAction(DominoCardType.CustomerApprovalHold, "MiniPaper", dict);
        }


        public ActionResult CustomerApprovalHold(string ECOKey, string CardKey)
        {
            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfo(ECOKey);
            var vm = new List<List<DominoVM>>();
            foreach (var item in baseinfos)
            {
                var templist = DominoVM.RetrieveECOCards(item);
                vm.Add(templist);
            }

            ViewBag.CardDetailPage = DominoCardType.CustomerApprovalHold;
            ViewBag.ECOKey = ECOKey;
            ViewBag.CardKey = CardKey;

            return View("SingalECO", vm);

        }

        [HttpPost, ActionName("CustomerApprovalHold")]
        [ValidateAntiForgeryToken]
        public ActionResult CustomerApprovalHoldPost()
        {
            var ECOKey = Request.Form["ECOKey"];
            var CardKey = Request.Form["CardKey"];
            DominoVM.UpdateCard(ECOKey, CardKey, DominoCardStatus.done);

            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfo(ECOKey);

            var newcardkey = DominoVM.GetUniqKey();

            if (string.Compare(baseinfos[0].ECOType, DominoECOType.RVS) == 0)
            {
                DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.SampleOrdering, DominoCardStatus.pending);
                var dict = new RouteValueDictionary();
                dict.Add("ECOKey", ECOKey);
                dict.Add("CardKey", newcardkey);
                return RedirectToAction(DominoCardType.SampleOrdering, "MiniPaper", dict);
            }
            else if (string.Compare(baseinfos[0].ECOType, DominoECOType.RVNS) == 0)
            {
                DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.FACustomerApproval, DominoCardStatus.pending);
                var dict = new RouteValueDictionary();
                dict.Add("ECOKey", ECOKey);
                dict.Add("CardKey", newcardkey);
                return RedirectToAction(DominoCardType.FACustomerApproval, "MiniPaper", dict);
            }
            else
            {
                DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.SampleOrdering, DominoCardStatus.pending);
                var dict = new RouteValueDictionary();
                dict.Add("ECOKey", ECOKey);
                dict.Add("CardKey", newcardkey);
                return RedirectToAction(DominoCardType.SampleOrdering, "MiniPaper", dict);
            }
        }


        public ActionResult SampleOrdering(string ECOKey, string CardKey)
        {
            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfo(ECOKey);
            var vm = new List<List<DominoVM>>();
            foreach (var item in baseinfos)
            {
                var templist = DominoVM.RetrieveECOCards(item);
                vm.Add(templist);
            }

            ViewBag.CardDetailPage = DominoCardType.SampleOrdering;
            ViewBag.ECOKey = ECOKey;
            ViewBag.CardKey = CardKey;

            return View("SingalECO", vm);

        }

        [HttpPost, ActionName("SampleOrdering")]
        [ValidateAntiForgeryToken]
        public ActionResult SampleOrderingPost()
        {
            var ECOKey = Request.Form["ECOKey"];
            var CardKey = Request.Form["CardKey"];
            DominoVM.UpdateCard(ECOKey, CardKey, DominoCardStatus.done);

            var newcardkey = DominoVM.GetUniqKey();
            DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.SampleBuilding, DominoCardStatus.pending);

            var dict = new RouteValueDictionary();
            dict.Add("ECOKey", ECOKey);
            dict.Add("CardKey", newcardkey);
            return RedirectToAction(DominoCardType.SampleBuilding, "MiniPaper", dict);
        }


        public ActionResult SampleBuilding(string ECOKey, string CardKey)
        {
            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfo(ECOKey);
            var vm = new List<List<DominoVM>>();
            foreach (var item in baseinfos)
            {
                var templist = DominoVM.RetrieveECOCards(item);
                vm.Add(templist);
            }

            ViewBag.CardDetailPage = DominoCardType.SampleBuilding;
            ViewBag.ECOKey = ECOKey;
            ViewBag.CardKey = CardKey;

            return View("SingalECO", vm);

        }

        [HttpPost, ActionName("SampleBuilding")]
        [ValidateAntiForgeryToken]
        public ActionResult SampleBuildingPost()
        {
            var ECOKey = Request.Form["ECOKey"];
            var CardKey = Request.Form["CardKey"];
            DominoVM.UpdateCard(ECOKey, CardKey, DominoCardStatus.done);

            var newcardkey = DominoVM.GetUniqKey();
            DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.SampleShipment, DominoCardStatus.pending);

            var dict = new RouteValueDictionary();
            dict.Add("ECOKey", ECOKey);
            dict.Add("CardKey", newcardkey);
            return RedirectToAction(DominoCardType.SampleShipment, "MiniPaper", dict);
        }


        public ActionResult SampleShipment(string ECOKey, string CardKey)
        {
            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfo(ECOKey);
            var vm = new List<List<DominoVM>>();
            foreach (var item in baseinfos)
            {
                var templist = DominoVM.RetrieveECOCards(item);
                vm.Add(templist);
            }

            ViewBag.CardDetailPage = DominoCardType.SampleShipment;
            ViewBag.ECOKey = ECOKey;
            ViewBag.CardKey = CardKey;

            return View("SingalECO", vm);

        }

        [HttpPost, ActionName("SampleShipment")]
        [ValidateAntiForgeryToken]
        public ActionResult SampleShipmentPost()
        {
            var ECOKey = Request.Form["ECOKey"];
            var CardKey = Request.Form["CardKey"];
            DominoVM.UpdateCard(ECOKey, CardKey, DominoCardStatus.done);

            var newcardkey = DominoVM.GetUniqKey();
            DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.SampleCustomerApproval, DominoCardStatus.pending);

            var dict = new RouteValueDictionary();
            dict.Add("ECOKey", ECOKey);
            dict.Add("CardKey", newcardkey);
            return RedirectToAction(DominoCardType.SampleCustomerApproval, "MiniPaper", dict);
        }

        public ActionResult SampleCustomerApproval(string ECOKey, string CardKey)
        {
            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfo(ECOKey);
            var vm = new List<List<DominoVM>>();
            foreach (var item in baseinfos)
            {
                var templist = DominoVM.RetrieveECOCards(item);
                vm.Add(templist);
            }

            ViewBag.CardDetailPage = DominoCardType.SampleCustomerApproval;
            ViewBag.ECOKey = ECOKey;
            ViewBag.CardKey = CardKey;

            return View("SingalECO", vm);

        }

        [HttpPost, ActionName("SampleCustomerApproval")]
        [ValidateAntiForgeryToken]
        public ActionResult SampleCustomerApprovalPost()
        {
            var ECOKey = Request.Form["ECOKey"];
            var CardKey = Request.Form["CardKey"];
            DominoVM.UpdateCard(ECOKey, CardKey, DominoCardStatus.done);

            var newcardkey = DominoVM.GetUniqKey();
            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfo(ECOKey);

            if (string.Compare(baseinfos[0].ECOType, DominoECOType.RVS) == 0)
            {
                DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.ECOComplete, DominoCardStatus.pending);
                var dict = new RouteValueDictionary();
                dict.Add("ECOKey", ECOKey);
                dict.Add("CardKey", newcardkey);
                return RedirectToAction(DominoCardType.ECOComplete, "MiniPaper", dict);
            }
            else
            {
                DominoVM.CreateCard(ECOKey, newcardkey, DominoCardType.MiniPIPComplete, DominoCardStatus.pending);
                var dict = new RouteValueDictionary();
                dict.Add("ECOKey", ECOKey);
                dict.Add("CardKey", newcardkey);
                return RedirectToAction(DominoCardType.MiniPIPComplete, "MiniPaper", dict);
            }
        }


        public ActionResult MiniPIPComplete(string ECOKey, string CardKey)
        {
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

        [HttpPost, ActionName("MiniPIPComplete")]
        [ValidateAntiForgeryToken]
        public ActionResult MiniPIPCompletePost()
        {
            var ECOKey = Request.Form["ECOKey"];
            var CardKey = Request.Form["CardKey"];
            DominoVM.UpdateCard(ECOKey, CardKey, DominoCardStatus.done);


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