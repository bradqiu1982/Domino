using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Domino.Models
{
    public class UT
    {
        public static double O2D(string obj)
        {
            if (string.IsNullOrEmpty(obj))
            { return 0; }
            try
            {
                return Convert.ToDouble(obj);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public static List<List<T>> SplitList<T>(List<T> me, int size = 5000)
        {
            var list = new List<List<T>>();
            try
            {
                for (int i = 0; i < me.Count; i += size)
                {
                    list.Add(me.GetRange(i, Math.Min(size, me.Count - i)));
                }
            }
            catch (Exception ex) { }
            return list;
        }

        public static string O2S(object obj)
        {
            if (obj != null)
            {
                try
                {
                    return Convert.ToString(obj);
                }
                catch (Exception ex) { return string.Empty; }
            }
            return string.Empty;
        }

        public static DateTime O2T(object obj)
        {
            if (obj != null && !string.IsNullOrEmpty(obj.ToString()))
            {
                try
                {
                    return Convert.ToDateTime(obj);
                }
                catch (Exception ex) { return DateTime.Now; }
            }

            return DateTime.Now;
        }

    }
}