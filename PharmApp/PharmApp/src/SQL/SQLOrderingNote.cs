using PharmApp.src.Product_Info;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmApp.src.SQL
{
    class SQLOrderingNote : SQLLookUp<OrderingNote>
    {

        private string pipcode;

        public SQLOrderingNote(string pipcode) : base()
        {
            this.pipcode = pipcode;
        }

        protected override OrderingNote QueryFunction()
        {
            return SQLQueryer.GetOrderingNote(pipcode);
        }

        public bool IsEmpty()
        {
            if (GetData() == null)
            {
                return true;
            }
            return false;
        }

        public void Clear()
        {
            SetData(null);
        }

        public bool NoteEquals(string note)
        {
            if (GetData() == null)
            {
                return false;
            }
            else if (GetData().note == note) return true;
            else return false;
        }

        public bool RequiresActionEquals(bool requiresAction)
        {
            if (GetData() == null)
            {
                return false;
            }
            else if (GetData().requiresAction == requiresAction) return true;
            else return false;
        }

        protected override OrderingNote DefaultData()
        {
            return new OrderingNote("", false);
        }
    }
}
