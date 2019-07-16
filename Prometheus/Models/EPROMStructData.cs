using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Domino.Models
{
    public class EPROMStructData
    {
        private static bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }

        private static byte GetByteByBit(int pos, int length, byte val)
        {
            var orgbtarray = new byte[1];
            orgbtarray[0] = val;
            var orgarray = new BitArray(orgbtarray);

            byte ret = 0;
            var sidx = 0;
            for (var bidx = pos; bidx < pos + length; bidx++)
            {
                if (orgarray.Get(bidx))
                {
                    ret += (byte)Math.Pow(2, length - sidx - 1);
                }
                sidx++;
            }
            return ret;
        }

        public static List<EPROMStructData> ParseEEPROMFile(string EEPROMTemplateFile, string EEPROMMaskFile, string EEPROMContentFile, Controller ctrl)
        {
            var epromtemplatelist = EPROMStructData.LoadTemplateData(ctrl, EEPROMTemplateFile);
            var eprommaskcontent = EPROMContentData.LoadContentData(EEPROMMaskFile);
            var epromcontent = EPROMContentData.LoadContentData(EEPROMContentFile);

            foreach (var template in epromtemplatelist)
            {
                if (template.BitCount >= 8)
                {
                    var bytecount = template.BitCount / 8;
                    for (var b = 0; b < bytecount; b++)
                    {
                        var key = "IDX:" + template.TableNo + "-" + (template.ByteIndx + b);
                        if (eprommaskcontent.ContainsKey(key) && (eprommaskcontent[key].Val == 255))
                        {
                            template.NeedCheck = true;
                            template.CheckIdxs.Add(b);
                        }
                    }
                }
                else
                {
                    var key = "IDX:" + template.TableNo + "-" + template.ByteIndx;
                    if (eprommaskcontent.ContainsKey(key) && (eprommaskcontent[key].Val == 255))
                    {
                        template.NeedCheck = true;
                        template.CheckIdxs.Add(0);
                    }
                }
            }

            foreach (var template in epromtemplatelist)
            {
                if (template.BitCount >= 8)
                {
                    var bytecount = template.BitCount / 8;
                    for (var b = 0; b < bytecount; b++)
                    {
                        var key = "IDX:" + template.TableNo + "-" + (template.ByteIndx + b);
                        if (epromcontent.ContainsKey(key))
                        { template.ByteList.Add(epromcontent[key].Val); }

                    }
                }
                else
                {
                    var key = "IDX:" + template.TableNo + "-" + template.ByteIndx;
                    if (epromcontent.ContainsKey(key))
                    {
                        template.ByteList.Add(GetByteByBit(template.BitPos, template.BitCount, epromcontent[key].Val));
                    }
                }
            }

            return epromtemplatelist;
        }

        public static Dictionary<string, EPROMStructData> Convert2DictData(List<EPROMStructData> datalist)
        {
            var ret = new Dictionary<string, EPROMStructData>();
            foreach (var item in datalist)
            {
                var key = item.TableNo + "_" + item.ByteIndx + "_" + item.BitPos + "_" + item.BitCount;
                if (!ret.ContainsKey(key))
                {
                    ret.Add(key, item);
                }
            }
            return ret;
        }

        private static List<EPROMStructData> LoadTemplateData(Controller ctrl,string tempfile)
        {
            var ret = new List<EPROMStructData>();
            var table0 = DominoDataCollector.RetrieveDataFromExcel(ctrl, tempfile, "Table 0", 10);
            var table1 = DominoDataCollector.RetrieveDataFromExcel(ctrl, tempfile, "Table 1", 10);
            var table2 = DominoDataCollector.RetrieveDataFromExcel(ctrl, tempfile, "Table 2", 10);
            var table3 = DominoDataCollector.RetrieveDataFromExcel(ctrl, tempfile, "Table 3", 10);

            var tablelist = new List<List<List<string>>>();
            tablelist.Add(table0);
            tablelist.Add(table1);
            tablelist.Add(table2);
            tablelist.Add(table3);


            var tabno = 0;
            foreach (var tab in tablelist)
            {
                foreach (var line in tab)
                {
                    if (IsDigitsOnly(line[0]) && IsDigitsOnly(line[2]) && IsDigitsOnly(line[3]))
                    {
                        if (!string.IsNullOrEmpty(line[4])
                           && !line[4].ToUpper().Contains("RESERVED")
                           && !line[5].ToUpper().Contains("RESERVED"))
                        {
                            var tempvm = new EPROMStructData();

                            tempvm.TableNo = tabno;
                            tempvm.ByteIndx = Convert.ToInt32(line[0]);
                            tempvm.BitPos = Convert.ToInt32(line[2]);
                            tempvm.BitCount = Convert.ToInt32(line[3]);
                            tempvm.ParamName = line[4];
                            tempvm.ParamDesc = line[5];

                            ret.Add(tempvm);
                        }
                    }
                }//end foreach
                tabno += 1;
            }//end foreach
            return ret;
        }

        public EPROMStructData()
        {
            ParamName = string.Empty;
            ParamDesc = string.Empty;
            TableNo = -1;
            ByteIndx = -1;
            BitPos = -1;
            BitCount = -1;
            ByteList = new List<byte>();
            ActualValue = "N/A";
            NeedCheck = false;
            CheckIdxs = new List<int>();
        }

        public string ParamName { set; get; }
        public string ParamDesc { set; get; }
        public int TableNo { set; get; }
        public int ByteIndx { set; get; }
        public int BitPos { set; get; }
        public int BitCount { set; get; }
        
        public List<byte> ByteList { set; get; }
        public string ParamValue {
            get {
                if (ByteList != null)
                {
                    var vallist = new List<string>();
                    foreach (var item in ByteList)
                    {
                        var ival = Convert.ToInt32(item).ToString("X2");
                        vallist.Add(ival);
                    }

                    if (vallist.Count == 0)
                    { return "N/A"; }

                    return string.Join(",", vallist);
                }
                return "N/A";
            }
        }
        
        public string ActualValue { set; get; }
        public bool NeedCheck { set; get; }
        public List<int> CheckIdxs { set; get; }

        public string CheckPass {
            get {
                    if (NeedCheck)
                    {
                        if (string.Compare(ActualValue, "N/A") == 0 
                            || string.Compare(ParamValue, "N/A") == 0)
                        { return "FALSE"; }

                        var actualvalarray = ActualValue.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        var actualbytes = new List<byte>();
                        foreach (var a in actualvalarray)
                        { actualbytes.Add((byte)Convert.ToInt32(a,16)); }

                        var pass = "TRUE";
                        foreach (var idx in CheckIdxs)
                        {
                            if (ByteList[idx] != actualbytes[idx])
                            { pass = "FALSE"; }
                        }
                        return pass;
                    }
                    else
                    { return "TRUE"; }
            }
        }




    }
}