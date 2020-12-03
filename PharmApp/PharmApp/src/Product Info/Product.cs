using PharmApp.src.Product_Info;
using PharmApp.src.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmApp.src
{
    class Product
    {
        public string pipcode = "0";
        public string description = "NO DESCRIPTION";
        public string genericID = "0";
        public decimal unitsPerPack = 1.0M;
        //public string preparationCode;
        public bool isGeneric = false;
        public string supplier = "NO SUPPLIER";
        private SQLLookUp<OrderHistory> orders;

        public void GetPlacedOrders(DateTime fromDate, DateTime endDate)
        {
            if (orders == null)
            {
                orders = new SQLLookUp<OrderHistory>(SQLQueryer.GetOrderHistory);
            }
        }
    }
}
