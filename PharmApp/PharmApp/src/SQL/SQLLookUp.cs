using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmApp.src.SQL
{
    abstract class SQLLookUp<T>
    {
        private T data;
        private Stopwatch timer;
        private const int CACHE_TIME_MS = 10000;

        public T GetData()
        {
            if (timer == null || data == null)
            {
                data = QueryFunction();
                timer = new Stopwatch();
                timer.Start();
            }
            else if (timer.ElapsedMilliseconds > CACHE_TIME_MS)
            {
                data = QueryFunction();
                timer.Restart();
            }

            return data;
        }

        abstract protected T QueryFunction();

        public override string ToString()
        {
            return GetData().ToString();
        }
    }
}
