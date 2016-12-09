using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Domino.Models
{
    public class DominoCardColor
    {
        public static string red = "red";
        public static string blue = "blue";
        public static string green = "green";
    }

    public class DominoVM
    {
        public string CardNo { set; get; }
        public string CardContent { set; get; }
        public string CardColor { set; get; }
        public string CardModalName { set; get; }
    }
}