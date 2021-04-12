using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PharmApp.src
{
    class Util
    {

        public static string TrimTrailingZeros(string decimalStr)
        {
            if (decimalStr.EndsWith(".0000"))
                return decimalStr.Substring(0, decimalStr.Length - 5);

            if (decimalStr.EndsWith(".000"))
                return decimalStr.Substring(0, decimalStr.Length - 4);

            if (decimalStr.EndsWith(".500"))
                return decimalStr.Substring(0, decimalStr.Length - 2);

            return decimalStr;
        }

        public static string NOTFOUND = "NOT FOUND";

        public static void ShowError(string str)
        {
            MessageBox.Show(str, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
