using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KStore.Calc._2.Model
{
    public class Calculation
    {
        public static int Sum( int left, int right )
        {
            return left + right;
        }
    }

    internal class Util
    {
        internal static int StringToInt(string src)
        {
            int ret = 0;
            if (int.TryParse(src, out ret))
            {
                return ret;
            }
            throw new ArgumentException("src" + src);
        }

        internal static string IntToString(int src)
        {
            return src.ToString();
        }
    }
}
