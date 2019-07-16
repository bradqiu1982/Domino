using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;


namespace Domino.Models
{
    public class EPROMContentData
    {
        public static Dictionary<string, EPROMContentData> LoadContentData(string cfile)
        {
            var vallist = new List<EPROMContentData>();

            var lines = File.ReadAllLines(cfile);
            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line.Trim()))
                { continue; }

                var items = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                if (items[0].Contains("0:") && items.Count == 17)
                {
                    var tabno = Convert.ToInt32(items[0].Substring(0, 2), 16);
                    var byteidx = Convert.ToInt32(items[0].Substring(2, 2), 16);
                    for (var idx = 0; idx < 16; idx++)
                    {
                        var tempvm = new EPROMContentData();
                        tempvm.TableNo = tabno;
                        tempvm.ByteIndx = byteidx + idx;
                        tempvm.Val =Convert.ToByte(Convert.ToInt32(items[idx + 1], 16));
                        vallist.Add(tempvm);
                    }
                }
            }//end foreach

            var retdict = new Dictionary<string, EPROMContentData>();
            foreach (var item in vallist)
            {
                var key = "IDX:" + item.TableNo + "-" + item.ByteIndx;
                if (!retdict.ContainsKey(key)) {
                    retdict.Add(key, item);
                }
            }
            return retdict;
        }

        public EPROMContentData()
        {
            TableNo = -1;
            ByteIndx = -1;
            Val = 0;
        }

        public int TableNo { set; get; }
        public int ByteIndx { set; get; }
        public byte Val { set; get; }

    }
}