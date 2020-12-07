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
        private bool byGenericID;

        public SQLOrderHistory(string pip, DateTime fromDate, DateTime endDate, bool byGenericID)
        {
            this.pip = pip;
            this.fromDate = fromDate;
            this.endDate = endDate;
            this.byGenericID = byGenericID;
        }

        protected override OrderHistory QueryFunction()
        {
            return SQLQueryer.GetOrderHistory(pip, fromDate, endDate, byGenericID);
        }
    }
}
