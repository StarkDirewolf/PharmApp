using PharmApp.src.Product_Info;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmApp.src.SQL
{
    class SQLOrderPadPIPs : SQLLookUp<List<string>>
    {

        public SQLOrderPadPIPs() : base()
        {
        }

        protected override List<string> DefaultData()
        {
            return new List<string>();
        }

        protected override List<string> QueryFunction()
        {
            return SQLQueryer.GetOrderPadPIPs();
        }
    }
}
