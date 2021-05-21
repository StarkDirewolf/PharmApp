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

        public SQLOrderPadQuantities(string pip) : base()
        {
            this.pip = pip;
        }

        protected override OrderPadQuantities DefaultData()
        {
            return new OrderPadQuantities(0, 0);
        }

        protected override OrderPadQuantities QueryFunction()
        {
            return SQLQueryer.GetOrderPadLine(pip);
        }
    }
}
