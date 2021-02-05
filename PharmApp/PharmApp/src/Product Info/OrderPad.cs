using PharmApp.src.SQL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmApp.src.Product_Info
{
    class OrderPad
    {
        private static OrderPad obj;
        private List<Product> products = new List<Product>();
        private SQLOrderPadPIPs query = new SQLOrderPadPIPs();
        private const int minUpdateTime = 5000;
        private Stopwatch timePassed = new Stopwatch();

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
            // Start timer if it's not running (should only happen first update)
            if (!timePassed.IsRunning)
            {
                timePassed.Start();
            }
            // Cancel update if minimum time hasn't passed
            else if (timePassed.ElapsedMilliseconds < minUpdateTime) return;

            List<string> pips = query.GetData();

            // Delete absent pips
            List<Product> forDeleting = new List<Product>();
            foreach (Product product in products)
            {
                if (!pips.Contains(product.pipcode))
                {
                    forDeleting.Add(product);
                }
            }
            forDeleting.ForEach(p => products.Remove(p));

            // Add new products found
            foreach (string pip in pips)
            {
                AddProduct(pip);
            }
        }

        // Adds a product to the list so long as a product with the same pip isn't already on there
        public void AddProduct(string pip)
        {
            int existingProductIndex = products.FindIndex(p => p.pipcode.Equals(pip));

            if (existingProductIndex == -1)
            {
                products.Add(ProductLookup.Get().FindByPIP(pip));
            }
            
        }

        public List<Product> GetProductsWithNotes()
        {
            return products.FindAll(p => p.deliveryNotes != null);
        }
    }
}
