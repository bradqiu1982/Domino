﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Domino.Controllers
{
    public class ShareDataController : ApiController
    {
        [HttpGet]
        public Dictionary<string, List<string>> DataMap()
        {
            var plist = new Dictionary<string, List<string>>();
            var slist = new List<string>();
            slist.Add("apple");
            slist.Add("banana");
            plist.Add("fruit", slist);
            slist = new List<string>();
            slist.Add("rice");
            slist.Add("fish");
            plist.Add("food", slist);

            return plist;
        }
    }
}
