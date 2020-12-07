using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmApp.src.Product_Info
{
    class OrderHistory
    {

        private List<OrderLine> orderLines = new List<OrderLine>();

        public void AddOrderLine(OrderLine line)
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
            foreach (OrderLine line in orderLines)
            {
                if (line.DeliveryDue()) return true;
            }

            return false;
        }

        public bool IsOrdering()
        {
            foreach (OrderLine line in orderLines)
            {
                if (line.receivedQty > 0) return true;
            }

            return false;
        }
    }
}
