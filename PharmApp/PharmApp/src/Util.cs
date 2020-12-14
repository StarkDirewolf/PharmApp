using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmApp.src
{
    class Util
    {

        public static string TrimTrailingZeros(string decimalStr)
        {
            if (decimalStr.EndsWith(".0000"))
            {
                return decimalStr.Substring(0, decimalStr.Length - 5);
            }

            return decimalStr.Substring(0, decimalStr.Length - 2);
        }
    }
}
