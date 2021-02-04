using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmApp.src.SQL
{
    class ProductLookup
    {
        private static ProductLookup singleton;
        private List<Product> cache = new List<Product>();

        public static ProductLookup Get()
        {
            if (singleton == null)
            {
                singleton = new ProductLookup();
            }
            return singleton;
        }

        private ProductLookup()
        {

        }

        public Product FindByPIP(string pip)
        {
            foreach (Product product in cache) {
                if (product.pipcode == pip) return product;
            }

            Product result = SQLQueryer.PopulateFromPIP(pip);
            cache.Add(result);
            return result;
        }
    }
}
