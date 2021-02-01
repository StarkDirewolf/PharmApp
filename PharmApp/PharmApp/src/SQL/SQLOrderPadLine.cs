using PharmApp.src.Product_Info;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmApp.src.SQL
{
    class SQLOrderPadLine : SQLLookUp<OrderPadLine>
    {
        private string pip;

        public SQLOrderPadLine(string pip)
        {
            this.pip = pip;
        }

        protected override OrderPadLine QueryFunction()
        {
            return SQLQueryer.GetOrderPadLine(pip);
        }
    }
}
