using log4net;
using PharmApp.src.SQL;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace PharmApp.src.Product_Info
{
    class OrderPad
    {
        private static OrderPad obj;
        private ConcurrentDictionary<string, Product> products = new ConcurrentDictionary<string, Product>();
        private List<Product> productsWithComments = new List<Product>();
        private SQLOrderPadPIPs query = new SQLOrderPadPIPs();
        private const int UPDATE_PERIOD = 5000;
        private Timer updateTimer;
        private volatile bool isRunning = false;

        private OrderPad()
        {
            updateTimer = new Timer(new TimerCallback(UpdateTimerTick), null, 0, UPDATE_PERIOD);
        }

        public static OrderPad Get()
        {
            if (obj == null)
            {
                obj = new OrderPad();
            }
            return obj;
        }

        private void UpdateTimerTick(object state)
        {
            if (isRunning) return;
            isRunning = true;

            Stopwatch tickTime = new Stopwatch();
            tickTime.Start();

            List<string> pips = query.GetData();


            //var toDeleteQuery =
            //    from prod in products.AsParallel()
            //    where !pips.Contains(prod.pipcode)
            //    select prod;

            //List<Product> forDeleting = toDeleteQuery.ToList();

            //forDeleting.AsParallel().ForAll(prod => products.Remove(prod));

            List<string> forDeleting = new List<string>();

            // Delete absent pips
            foreach (string savedPip in products.Keys)
            {
                if (pips.Contains(savedPip))
                {
                    pips.Remove(savedPip);
                }
                else
                {
                    forDeleting.Add(savedPip);
                }
            }

            foreach (string pip in forDeleting)
            {
                Product removedProduct;
                products.TryRemove(pip, out removedProduct);
            }

            foreach (string pip in pips)
            {
                AddProduct(pip);
            }

            //List<Product> forDeleting = new List<Product>();
            //foreach (Product product in products)
            //{
            //    if (!pips.Contains(product.pipcode))
            //    {
            //        forDeleting.Add(product);
            //    }
            //}
            //forDeleting.ForEach(p => products.Remove(p));

            // Add new products found
            //foreach (string pip in pips)
            //{
            //    AddProduct(pip);
            //}

            UpdateProductsWithComments();

            LogManager.GetLogger(typeof(Program)).Debug("Orderpad update tick took: " + tickTime.ElapsedMilliseconds + "ms");
            isRunning = false;
        }

        // Adds a product to the list so long as a product with the same pip isn't already on there
        public void AddProduct(string pip)
        {
            if (!products.ContainsKey(pip))
            {

                products.TryAdd(pip, ProductLookup.Get().FindByPIP(pip));
            }

            //int existingProductIndex = products.FindIndex(p => p.pipcode.Equals(pip));

            //if (existingProductIndex == -1)
            //{
            //    products.Add(ProductLookup.Get().FindByPIP(pip));
            //}
            
        }

        public void UpdateProductsWithComments()
        {
            Stopwatch test = new Stopwatch();
            test.Start();
            var parallelQuery =
                from prod in products.AsParallel()
                where prod.Value.orderingNote != null && prod.Value.orderingNote.requiresAction && prod.Value.GetCurrentOrders().IsOnOrder()
                select prod.Value;

            productsWithComments = parallelQuery.ToList();
            LogManager.GetLogger(typeof(Program)).Debug("Comment updating took: " + test.ElapsedMilliseconds + "ms");
            
        }

        public List<Product> GetProductsRequiringAction()
        {

            //var parallelQuery =
            //    from prod in products.AsParallel()
            //    where prod.orderingNote != null && prod.orderingNote.requiresAction && prod.GetCurrentOrders().IsOnOrder()
            //    select prod;

            //return parallelQuery.ToList();

            //return products.FindAll(p => p.orderingNote != null && p.orderingNote.requiresAction && p.GetCurrentOrders().IsOnOrder());
            return productsWithComments;
        }
    }
}
