using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmApp.src.Requests
{
    class UniqueID
    {
        private readonly int id;

        public UniqueID (int id)
        {
            this.id = id;
        }

        public static bool operator == (UniqueID obj1, UniqueID obj2)
        {
            if (object.ReferenceEquals(obj1, null) && !object.ReferenceEquals(obj2, null)) return false;
            if (object.ReferenceEquals(obj2, null) && !object.ReferenceEquals(obj1, null)) return false;
            if (object.ReferenceEquals(obj1, null) && object.ReferenceEquals(obj2, null)) return true;
            if (obj1.id == obj2.id) return true;
            return false;
        }

        public static bool operator != (UniqueID obj1, UniqueID obj2)
        {
            if (object.ReferenceEquals(obj1, null) && object.ReferenceEquals(obj2, null)) return false;
            if (object.ReferenceEquals(obj2, null) && !object.ReferenceEquals(obj1, null)) return true;
            if (object.ReferenceEquals(obj1, null) && !object.ReferenceEquals(obj2, null)) return true;
            if (obj1.id == obj2.id) return false;
            return true;
        }

        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(obj, null)) return false;
            if (obj is UniqueID && ((UniqueID)obj).id == id) return true;
            return false;
        }

        public override int GetHashCode()
        {
            return id;
        }
    }
}
