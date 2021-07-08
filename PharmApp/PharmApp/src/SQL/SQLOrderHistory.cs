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
        private const int CACHE_TIME_MS = 180000;

        public SQLOrderHistory(string pip, DateTime fromDate, DateTime endDate, bool byGenericID) : base(CACHE_TIME_MS)
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

        protected override OrderHistory DefaultData()
        {
            return new OrderHistory();
        }
    }
}
