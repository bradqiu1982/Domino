using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Domino.Models
{
    public class CfgUtility
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

        public static Dictionary<string, string> GetNPIMachine(Controller ctrl)
        {
            var lines = System.IO.File.ReadAllLines(ctrl.Server.MapPath("~/Scripts/npidepartmentmachine.cfg"));
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
                    if (!ret.ContainsKey(kvpair[0].Trim()))
                    {
                        ret.Add(kvpair[0].Trim().ToUpper(), kvpair[1].Trim());
                    }
                }//end if
            }//end foreach
            return ret;
        }
    }
}