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
            var baseinfo = new ECOBaseInfo();
            baseinfo.ECOKey = DominoVM.GetUniqKey();
            baseinfo.ECONum = "97807";
            baseinfo.PNDesc = "FCBG410QB1C10-FC";
            baseinfo.Customer = "MRV";
            baseinfo.PE = "Jessica Zheng";

            var vm = new List<List<DominoVM>>();
            var templist = new List<DominoVM>();

            var tempitem = new DominoVM();
            tempitem.ECOkey = baseinfo.ECOKey;
            tempitem.Cardkey = DominoVM.GetUniqKey();
            tempitem.CardStatus = DominoCardStatus.done;
            tempitem.CardContent = "ECO Pending";
            tempitem.CardNo = "1";
            tempitem.CardModalName = "_ecopendingmodal";
            tempitem.EBaseInfo = baseinfo;
            templist.Add(tempitem);

            //tempitem = new DominoVM();
            //tempitem.CardStatus = DominoCardStatus.working;
            //tempitem.CardContent = "ECO signoff_1";
            //tempitem.CardNo = "2";
            //tempitem.CardModalName = "_ecosignoff1modal";
            //tempitem.EBaseInfo = baseinfo;
            //templist.Add(tempitem);

            //tempitem = new DominoVM();
            //tempitem.CardStatus = DominoCardStatus.working;
            //tempitem.CardContent = "ECO signoff_1";
            //tempitem.CardNo = "2";
            //tempitem.CardModalName = "_ecosignoff1modal";
            //tempitem.EBaseInfo = baseinfo;
            //templist.Add(tempitem);
            //tempitem = new DominoVM();
            //tempitem.CardStatus = DominoCardStatus.working;
            //tempitem.CardContent = "ECO signoff_1";
            //tempitem.CardNo = "2";
            //tempitem.CardModalName = "_ecosignoff1modal";
            //templist.Add(tempitem);
            //tempitem = new DominoVM();
            //tempitem.CardStatus = DominoCardStatus.working;
            //tempitem.CardContent = "ECO signoff_1";
            //tempitem.CardNo = "2";
            //tempitem.CardModalName = "_ecosignoff1modal";
            //templist.Add(tempitem);
            //tempitem = new DominoVM();
            //tempitem.CardStatus = DominoCardStatus.working;
            //tempitem.CardContent = "ECO signoff_1";
            //tempitem.CardNo = "2";
            //tempitem.CardModalName = "_ecosignoff1modal";
            //templist.Add(tempitem);
            //tempitem = new DominoVM();
            //tempitem.CardStatus = DominoCardStatus.working;
            //tempitem.CardContent = "ECO signoff_1";
            //tempitem.CardNo = "2";
            //tempitem.CardModalName = "_ecosignoff1modal";
            //templist.Add(tempitem);
            //tempitem = new DominoVM();
            //tempitem.CardStatus = DominoCardStatus.working;
            //tempitem.CardContent = "ECO signoff_1";
            //tempitem.CardNo = "2";
            //tempitem.CardModalName = "_ecosignoff1modal";
            //templist.Add(tempitem);
            //tempitem = new DominoVM();
            //tempitem.CardStatus = DominoCardStatus.working;
            //tempitem.CardContent = "ECO signoff_1";
            //tempitem.CardNo = "2";
            //tempitem.CardModalName = "_ecosignoff1modal";
            //templist.Add(tempitem);
            //tempitem = new DominoVM();
            //tempitem.CardStatus = DominoCardStatus.working;
            //tempitem.CardContent = "ECO signoff_1";
            //tempitem.CardNo = "2";
            //tempitem.CardModalName = "_ecosignoff1modal";
            //templist.Add(tempitem);
            //tempitem = new DominoVM();
            //tempitem.CardStatus = DominoCardStatus.working;
            //tempitem.CardContent = "ECO signoff_1";
            //tempitem.CardNo = "2";
            //tempitem.CardModalName = "_ecosignoff1modal";
            //templist.Add(tempitem);
            //tempitem = new DominoVM();
            //tempitem.CardStatus = DominoCardStatus.working;
            //tempitem.CardContent = "ECO signoff_1";
            //tempitem.CardNo = "2";
            //tempitem.CardModalName = "_ecosignoff1modal";
            //templist.Add(tempitem);
            //tempitem = new DominoVM();
            //tempitem.CardStatus = DominoCardStatus.working;
            //tempitem.CardContent = "ECO signoff_1";
            //tempitem.CardNo = "2";
            //tempitem.CardModalName = "_ecosignoff1modal";
            //templist.Add(tempitem);
            vm.Add(templist);

            //for (var idx = 0; idx < 10; idx++)
            //{
            //    templist = new List<DominoVM>();
            //    tempitem = new DominoVM();
            //    tempitem.CardStatus = DominoCardStatus.done;
            //    tempitem.CardContent = "ECO Pending";
            //    tempitem.CardNo = "1";
            //    tempitem.CardModalName = "_ecosignoff1modal";
            //    tempitem.EBaseInfo = baseinfo;
            //    templist.Add(tempitem);

            //    tempitem = new DominoVM();
            //    tempitem.CardStatus = DominoCardStatus.working;
            //    tempitem.CardContent = "ECO signoff_1";
            //    tempitem.CardNo = "2";
            //    tempitem.CardModalName = "_ecosignoff1modal";
            //    tempitem.EBaseInfo = baseinfo;
            //    templist.Add(tempitem);

            //    tempitem = new DominoVM();
            //    tempitem.CardStatus = DominoCardStatus.working;
            //    tempitem.CardContent = "ECO signoff_1";
            //    tempitem.CardNo = "2";
            //    tempitem.CardModalName = "_ecosignoff1modal";
            //    tempitem.EBaseInfo = baseinfo;
            //    templist.Add(tempitem);
            //    tempitem = new DominoVM();
            //    tempitem.CardStatus = DominoCardStatus.working;
            //    tempitem.CardContent = "ECO signoff_1";
            //    tempitem.CardNo = "2";
            //    tempitem.CardModalName = "_ecosignoff1modal";
            //    templist.Add(tempitem);
            //    tempitem = new DominoVM();
            //    tempitem.CardStatus = DominoCardStatus.working;
            //    tempitem.CardContent = "ECO signoff_1";
            //    tempitem.CardNo = "2";
            //    tempitem.CardModalName = "_ecosignoff1modal";
            //    templist.Add(tempitem);
            //    tempitem = new DominoVM();
            //    tempitem.CardStatus = DominoCardStatus.working;
            //    tempitem.CardContent = "ECO signoff_1";
            //    tempitem.CardNo = "2";
            //    tempitem.CardModalName = "_ecosignoff1modal";
            //    templist.Add(tempitem);
            //    tempitem = new DominoVM();
            //    tempitem.CardStatus = DominoCardStatus.working;
            //    tempitem.CardContent = "ECO signoff_1";
            //    tempitem.CardNo = "2";
            //    tempitem.CardModalName = "_ecosignoff1modal";
            //    templist.Add(tempitem);
            //    tempitem = new DominoVM();
            //    tempitem.CardStatus = DominoCardStatus.working;
            //    tempitem.CardContent = "ECO signoff_1";
            //    tempitem.CardNo = "2";
            //    tempitem.CardModalName = "_ecosignoff1modal";
            //    templist.Add(tempitem);
            //    tempitem = new DominoVM();
            //    tempitem.CardStatus = DominoCardStatus.working;
            //    tempitem.CardContent = "ECO signoff_1";
            //    tempitem.CardNo = "2";
            //    tempitem.CardModalName = "_ecosignoff1modal";
            //    templist.Add(tempitem);
            //    tempitem = new DominoVM();
            //    tempitem.CardStatus = DominoCardStatus.working;
            //    tempitem.CardContent = "ECO signoff_1";
            //    tempitem.CardNo = "2";
            //    tempitem.CardModalName = "_ecosignoff1modal";
            //    templist.Add(tempitem);
            //    tempitem = new DominoVM();
            //    tempitem.CardStatus = DominoCardStatus.working;
            //    tempitem.CardContent = "ECO signoff_1";
            //    tempitem.CardNo = "2";
            //    tempitem.CardModalName = "_ecosignoff1modal";
            //    templist.Add(tempitem);
            //    tempitem = new DominoVM();
            //    tempitem.CardStatus = DominoCardStatus.working;
            //    tempitem.CardContent = "ECO signoff_1";
            //    tempitem.CardNo = "2";
            //    tempitem.CardModalName = "_ecosignoff1modal";
            //    templist.Add(tempitem);
            //    tempitem = new DominoVM();
            //    tempitem.CardStatus = DominoCardStatus.working;
            //    tempitem.CardContent = "ECO signoff_1";
            //    tempitem.CardNo = "2";
            //    tempitem.CardModalName = "_ecosignoff1modal";
            //    templist.Add(tempitem);
            //    vm.Add(templist);

            //}

            return View(vm);
        }

        public ActionResult CallCardModal(string ModalName,string ECOKey,string CardKey)
        {
            var routedict = new RouteValueDictionary();
            routedict.Add("ModalName", ModalName);
            routedict.Add("ECOKey", ECOKey);
            routedict.Add("CardKey", CardKey);

            if (string.Compare(ModalName, "_ecopendingmodal") == 0)
            {
                return RedirectToAction("ECOPending", "MiniPaper", routedict);
            }

            return View("ViewAll");

        }

        public ActionResult ECOPending(string ModalName, string ECOKey, string CardKey)
        {
            var baseinfo = new ECOBaseInfo();
            baseinfo.ECOKey = ECOKey;
            baseinfo.ECONum = "97807";
            baseinfo.PNDesc = "FCBG410QB1C10-FC";
            baseinfo.Customer = "MRV";
            baseinfo.PE = "Jessica Zheng";

            var vm = new List<List<DominoVM>>();
            var templist = new List<DominoVM>();

            var tempitem = new DominoVM();
            tempitem.ECOkey = ECOKey;
            tempitem.Cardkey = CardKey;
            tempitem.CardStatus = DominoCardStatus.done;
            tempitem.CardContent = "ECO Pending";
            tempitem.CardNo = "1";
            tempitem.CardModalName = ModalName;
            tempitem.EBaseInfo = baseinfo;
            templist.Add(tempitem);
            vm.Add(templist);

            ViewBag.modalname = ModalName;
            ViewBag.ECOKey = ECOKey;
            ViewBag.CardKey = CardKey;

            return View("SingalECO", vm);


            //var baseinfo = new ECOBaseInfo();
            //baseinfo.ECONum = "97807";
            //baseinfo.PNDesc = "FCBG410QB1C10-FC";
            //baseinfo.Customer = "MRV";
            //baseinfo.PE = "Jessica Zheng";


            //var vm = new List<List<DominoVM>>();
            //var templist = new List<DominoVM>();

            //var tempitem = new DominoVM();
            //tempitem.CardStatus = DominoCardStatus.done;
            //tempitem.CardContent = "ECO Pending";
            //tempitem.CardNo = "1";
            //tempitem.CardModalName = "_ecosignoff1modal";
            //tempitem.EBaseInfo = baseinfo;
            //templist.Add(tempitem);

            //tempitem = new DominoVM();
            //tempitem.CardStatus = DominoCardStatus.working;
            //tempitem.CardContent = "ECO signoff_1";
            //tempitem.CardNo = "2";
            //tempitem.CardModalName = "_ecosignoff1modal";
            //tempitem.EBaseInfo = baseinfo;
            //templist.Add(tempitem);

            //tempitem = new DominoVM();
            //tempitem.CardStatus = DominoCardStatus.pending;
            //tempitem.CardContent = "ECO complete";
            //tempitem.CardNo = "3";
            //tempitem.EBaseInfo = baseinfo;
            //templist.Add(tempitem);

            //vm.Add(templist);

            //return View("ViewAll",vm);
        }

        [HttpPost, ActionName("ECOPending")]
        [ValidateAntiForgeryToken]
        public ActionResult ECOPendingPost()
        {
            var ECOKey = Request.Form["ECOKey"];
            var CardKey = Request.Form["CardKey"];

            var baseinfo = new ECOBaseInfo();
            baseinfo.ECOKey = ECOKey;
            baseinfo.ECONum = "97807";
            baseinfo.PNDesc = "FCBG410QB1C10-FC";
            baseinfo.Customer = "MRV";
            baseinfo.PE = "Jessica Zheng";

            var vm = new List<List<DominoVM>>();
            var templist = new List<DominoVM>();

            var tempitem = new DominoVM();
            tempitem.ECOkey = ECOKey;
            tempitem.Cardkey = CardKey;
            tempitem.CardStatus = DominoCardStatus.done;
            tempitem.CardContent = "ECO Pending";
            tempitem.CardNo = "1";
            tempitem.CardModalName = "_ecopendingmodal";
            tempitem.EBaseInfo = baseinfo;
            templist.Add(tempitem);

            tempitem = new DominoVM();
            tempitem.ECOkey = ECOKey;
            tempitem.Cardkey = DominoVM.GetUniqKey();
            tempitem.CardStatus = DominoCardStatus.pending;
            tempitem.CardContent = "ECO Sign off 1";
            tempitem.CardNo = "2";
            tempitem.CardModalName = "_ecosignoff1modal";
            tempitem.EBaseInfo = baseinfo;
            templist.Add(tempitem);

            vm.Add(templist);

            //ViewBag.modalname = "_ecopendingmodal";
            //ViewBag.ECOKey = ECOKey;
            //ViewBag.CardKey = CardKey;

            return View("SingalECO", vm);
        }

    }
}