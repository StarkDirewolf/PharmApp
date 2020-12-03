using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmApp.src.SQL
{
    class SQLLookUp<T>
    {
        private T data;
        private Stopwatch timer;
        private Func<T> queryFunction;
        private const int CACHE_TIME_MS = 10000;

        public SQLLookUp(Func<T, T> queryFunction)
        {
            this.queryFunction = queryFunction;
        }

        public T GetData()
        {
            if (timer == null || data == null)
            {
                data = queryFunction.Invoke();
                timer = new Stopwatch();
                timer.Start();
            }
            else if (timer.ElapsedMilliseconds > CACHE_TIME_MS)
            {
                data = queryFunction.Invoke();
            }

            return data;
        }
    }
}
