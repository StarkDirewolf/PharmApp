using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmApp.src.Product_Info
{
    class OrderPadQuantities
    {
        public enum Supplier
        {
            ECASS,
            AAH
        }

        private Dictionary<Supplier, Decimal> onOrder = new Dictionary<Supplier, Decimal>();

        public OrderPadQuantities(Decimal eCassQty, Decimal aahQty)
        {
            onOrder.Add(Supplier.ECASS, eCassQty);
            onOrder.Add(Supplier.AAH, aahQty);
        }

        public Decimal GetAAH()
        {
            Decimal returnValue;
            onOrder.TryGetValue(Supplier.AAH, out returnValue);
            string returnValueString = returnValue.ToString();
            if (returnValueString.Substring(returnValueString.Length - 4) == ".000") returnValue = Decimal.Truncate(returnValue);
            return returnValue;
        }

        public Decimal GetECass()
        {
            Decimal returnValue;
            onOrder.TryGetValue(Supplier.ECASS, out returnValue);
            string returnValueString = returnValue.ToString();
            if (returnValueString.Length > 4 && returnValueString.Substring(returnValueString.Length - 4) == ".000") returnValue = Decimal.Truncate(returnValue);
            return returnValue;
        }

        public bool IsOnOrder()
        {
            if (GetECass() > 0 || GetAAH() > 0)
                return true;

            return false;
        }
    }
}
