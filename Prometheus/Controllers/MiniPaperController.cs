using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Domino.Models;

namespace Domino.Controllers
{
    public class MiniPaperController : Controller
    {
        // GET: MiniPaper
        public ActionResult ViewAll()
        {
            var vm = new List<List<DominoVM>>();
            var templist = new List<DominoVM>();

            var tempitem = new DominoVM();
            tempitem.CardColor = DominoCardColor.green;
            tempitem.CardContent = "ECO Pending";
            tempitem.CardNo = "1";
            tempitem.CardModalName = "_ecosignoff1modal";
            templist.Add(tempitem);

            tempitem = new DominoVM();
            tempitem.CardColor = DominoCardColor.blue;
            tempitem.CardContent = "ECO signoff_1";
            tempitem.CardNo = "2";
            tempitem.CardModalName = "_ecosignoff1modal";
            templist.Add(tempitem);

            vm.Add(templist);


            return View(vm);
        }

        public ActionResult CallCardModal(string ModalName)
        {
            var vm = new List<List<DominoVM>>();
            var templist = new List<DominoVM>();

            var tempitem = new DominoVM();
            tempitem.CardColor = DominoCardColor.green;
            tempitem.CardContent = "ECO Pending";
            tempitem.CardNo = "1";
            tempitem.CardModalName = "_ecosignoff1modal";
            templist.Add(tempitem);

            tempitem = new DominoVM();
            tempitem.CardColor = DominoCardColor.blue;
            tempitem.CardContent = "ECO signoff_1";
            tempitem.CardNo = "2";
            tempitem.CardModalName = "_ecosignoff1modal";
            templist.Add(tempitem);

            vm.Add(templist);

            var taglist = new List<string>();
            taglist.Add("A");
            taglist.Add("B");
            taglist.Add("C");
            taglist.Add("D");
            ViewBag.tobechoosetags = taglist;
            ViewBag.modalname = ModalName;

            return View("ViewAll", vm);
        }

        public ActionResult AddOne()
        {
            var vm = new List<List<DominoVM>>();
            var templist = new List<DominoVM>();

            var tempitem = new DominoVM();
            tempitem.CardColor = DominoCardColor.green;
            tempitem.CardContent = "ECO Pending";
            tempitem.CardNo = "1";
            tempitem.CardModalName = "_ecosignoff1modal";
            templist.Add(tempitem);

            tempitem = new DominoVM();
            tempitem.CardColor = DominoCardColor.blue;
            tempitem.CardContent = "ECO signoff_1";
            tempitem.CardNo = "2";
            tempitem.CardModalName = "_ecosignoff1modal";
            templist.Add(tempitem);

            tempitem = new DominoVM();
            tempitem.CardColor = DominoCardColor.red;
            tempitem.CardContent = "ECO complete";
            tempitem.CardNo = "3";
            templist.Add(tempitem);

            vm.Add(templist);

            return View("ViewAll",vm);
        }
    }
}