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
        protected const int CACHE_TIME_MS = 5000;

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
                //UpdateDataAsync();
                data = QueryFunction();
                timer.Restart();
            }

            return data;
        }

        public SQLLookUp()
        {
            //data = DefaultData();
        }

        private async void UpdateDataAsync()
        {
            data = await Task.Run(() => QueryFunction());
        }

        abstract protected T QueryFunction();
        abstract protected T DefaultData();

        public override string ToString()
        {
            return GetData().ToString();
        }

        public void SetData(T data)
        {
            if (timer == null)
            {
                timer = new Stopwatch();
                timer.Start();
            }
            else
            {
                timer.Restart();
            }

            this.data = data;
        }

    }
}
