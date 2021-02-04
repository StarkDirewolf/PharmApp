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

        private Dictionary<Supplier, int> onOrder = new Dictionary<Supplier, int>();

        public OrderPadQuantities(int eCassQty, int aahQty)
        {
            onOrder.Add(Supplier.ECASS, eCassQty);
            onOrder.Add(Supplier.AAH, aahQty);
        }

        public int GetAAH()
        {
            int returnValue;
            onOrder.TryGetValue(Supplier.AAH, out returnValue);
            return returnValue;
        }

        public int GetECass()
        {
            int returnValue;
            onOrder.TryGetValue(Supplier.ECASS, out returnValue);
            return returnValue;
        }
    }
}
