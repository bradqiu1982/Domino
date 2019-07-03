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
    }
}