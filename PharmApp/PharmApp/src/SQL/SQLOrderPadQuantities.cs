using PharmApp.src.Product_Info;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmApp.src.SQL
{
    class SQLOrderPadQuantities : SQLLookUp<OrderPadQuantities>
    {
        private string pip;

        public SQLOrderPadQuantities(string pip)
        {
            this.pip = pip;
        }

        protected override OrderPadQuantities QueryFunction()
        {
            return SQLQueryer.GetOrderPadLine(pip);
        }
    }
}
