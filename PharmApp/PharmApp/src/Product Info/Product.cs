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
        private SQLOrderHistory orders;
        public decimal dtPrice = 0.0M;
        public string deliveryNotes;
        private SQLOrderPadLine currentlyOnOrder;

        public OrderHistory GetPlacedOrders(DateTime fromDate, DateTime endDate)
        {
            if (orders == null || orders.fromDate != fromDate || orders.endDate != endDate)
            {
                if (isGeneric)
                {
                    orders = new SQLOrderHistory(genericID, fromDate, endDate, true);
                }
                else
                {
                    orders = new SQLOrderHistory(pipcode, fromDate, endDate, false);
                }
            }

            return orders.GetData();
        }

        public bool DeliveryDue()
        {
            OrderHistory history = GetRecentOrders();
            return history.DeliveryDue();
        }

        public bool IsOrdering()
        {
            OrderHistory history = GetRecentOrders();
            return history.IsOrdering();
        }

        public OrderHistory GetRecentOrders()
        {
            return GetPlacedOrders(DateTime.Today.AddDays(-5), DateTime.Today);
        }

        public override string ToString()
        {
            return description + " (" + Util.TrimTrailingZeros(unitsPerPack.ToString()) + ") - £" + Util.TrimTrailingZeros(dtPrice.ToString());
        }
    }
}
