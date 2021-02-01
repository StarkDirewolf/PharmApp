using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmApp.src.Product_Info
{
    class OrderHistory
    {

        private List<OrderHistoryLine> orderLines = new List<OrderHistoryLine>();

        public void AddOrderLine(OrderHistoryLine line)
        {
            orderLines.Add(line);
        }

        public override string ToString()
        {
            string str = "";
            orderLines.ForEach(l => str += l.ToString() + ("\n"));
            return str;
        }

        public bool DeliveryDue()
        {
            foreach (OrderHistoryLine line in orderLines)
            {
                if (line.DeliveryDue()) return true;
            }

            return false;
        }

        public bool IsOrdering()
        {
            foreach (OrderHistoryLine line in orderLines)
            {
                if (line.receivedQty > 0) return true;
            }

            return false;
        }
    }
}
