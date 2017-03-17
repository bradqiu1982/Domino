using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Domino.Models
{
    public class DominoUserCacheVM
    {
        public string UserName { set; get; }
        public string CacheInfo { set; get; }
        public DateTime UpdateTime { set; get; }

        public DominoUserCacheVM()
        {
            UserName = string.Empty;
            CacheInfo = string.Empty;
        }

        public static void InsertCacheInfo(string username, string cacheinfo)
        {
            var sql = "delete from ECOUserCacheInfo where UpdateTime < '<UpdateTime>'";
            sql = sql.Replace("<UpdateTime>", DateTime.Now.AddMonths(-2).ToString());
            DBUtility.ExeLocalSqlNoRes(sql);

            sql = "insert into ECOUserCacheInfo(UserName,CacheInfo,UpdateTime) values('<UserName>','<CacheInfo>','<UpdateTime>')";
            sql = sql.Replace("<UserName>", username.ToUpper()).Replace("<CacheInfo>", cacheinfo).Replace("<UpdateTime>", DateTime.Now.ToString());
            DBUtility.ExeLocalSqlNoRes(sql);
        }

        public static List<DominoUserCacheVM> RetrieveCacheInfo(string username)
        {
            var ret = new List<DominoUserCacheVM>();

            var sql = "select top 10 UserName,CacheInfo,UpdateTime from ECOUserCacheInfo where UserName = '<UserName>' order by UpdateTime DESC";
            sql = sql.Replace("<UserName>", username.ToUpper());
            var dbret = DBUtility.ExeLocalSqlWithRes(sql);

            foreach (var line in dbret)
            {
                var tempinfo = new DominoUserCacheVM();
                tempinfo.UserName = Convert.ToString(line[0]);
                var infobytes = Convert.FromBase64String(Convert.ToString(line[1]));
                tempinfo.CacheInfo = System.Text.Encoding.UTF8.GetString(infobytes);
                tempinfo.UpdateTime = DateTime.Parse(Convert.ToString(line[2]));
                ret.Add(tempinfo);
            }

            return ret;
        }

    }
}