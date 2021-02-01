using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmApp.src.Product_Info
{
    class OrderHistoryLine
    {
        public readonly string description;
        public readonly DateTime date;
        public readonly string statusComment;
        public readonly string supplier;
        public readonly decimal orderQty;
        public readonly decimal receivedQty;

        public OrderHistoryLine(string description, DateTime date, string supplier, decimal orderQty, decimal receivedQty, string statusComment)
        {
            this.description = description;
            this.date = date;
            this.supplier = supplier;
            this.orderQty = orderQty;
            this.receivedQty = receivedQty;
            this.statusComment = statusComment;
        }

        public bool DeliveryDue()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            string str = date.ToString("g") + " - ";
            str += description + " "; 
            str += receivedQty.ToString().Replace(".000", "") + "/" + orderQty.ToString().Replace(".000", "") + " (" + supplier;
            if (statusComment != null)
            {
                str += " - " + statusComment;
            }
            str += ")";
            return str;
        }

    }
}
