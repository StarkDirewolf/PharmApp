using PharmApp.src.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmApp.src.Product_Info
{
    class OrderPad
    {
        private static OrderPad obj;
        private List<Product> products = new List<Product>();

        private OrderPad()
        {

        }

        public static OrderPad Get()
        {
            if (obj == null)
            {
                obj = new OrderPad();
            }
            obj.Update();
            return obj;
        }

        private void Update()
        {
            sqlquery
        }

        public void AddProduct(string pip)
        {
            products.Add(ProductLookup.Get().FindByPIP(pip));
        }

        public List<Product> GetProductsWithNotes()
        {
            return products.FindAll(p => p.deliveryNotes != null);
        }
    }
}
