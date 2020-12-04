using PharmApp.src.Product_Info;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmApp.src.SQL
{
    class SQLOrderHistory : SQLLookUp<OrderHistory>
    {
        private string pip;
        public DateTime fromDate, endDate;

        public SQLOrderHistory(string pip, DateTime fromDate, DateTime endDate)
        {
            this.pip = pip;
            this.fromDate = fromDate;
            this.endDate = endDate;
        }

        protected override OrderHistory QueryFunction()
        {
            return SQLQueryer.GetOrderHistory(pip, fromDate, endDate);
        }

        public override string ToString()
        {
            return base.ToString();) 
        }
    }
}
