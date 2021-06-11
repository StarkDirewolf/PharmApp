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
        private SynchronizedCollection<Product> products = new SynchronizedCollection<Product>();
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

            List<Product> forDeleting = new List<Product>();

            // Delete absent pips
            foreach (Product product in products)
            {
                if (pips.Contains(product.pipcode))
                {
                    pips.Remove(product.pipcode);
                }
                else
                {
                    forDeleting.Add(product);
                }
            }

            foreach (Product product in forDeleting)
            {
                products.Remove(product);
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

            LogManager.GetLogger(typeof(Program)).Debug("Orderpad update tick took: " + tickTime.ElapsedMilliseconds + "ms");
            isRunning = false;
        }

        // Adds a product to the list so long as a product with the same pip isn't already on there
        public void AddProduct(string pip)
        {
            if (!products.Any(prod => prod.pipcode.Equals(pip)))
            {
                products.Add(ProductLookup.Get().FindByPIP(pip));
            }

            //int existingProductIndex = products.FindIndex(p => p.pipcode.Equals(pip));

            //if (existingProductIndex == -1)
            //{
            //    products.Add(ProductLookup.Get().FindByPIP(pip));
            //}
            
        }

        public List<Product> GetProductsRequiringAction()
        {
            var parallelQuery =
                from prod in products.AsParallel()
                where prod.orderingNote != null && prod.orderingNote.requiresAction && prod.GetCurrentOrders().IsOnOrder()
                select prod;

            return parallelQuery.ToList();

            //return products.FindAll(p => p.orderingNote != null && p.orderingNote.requiresAction && p.GetCurrentOrders().IsOnOrder());
        }
    }
}
