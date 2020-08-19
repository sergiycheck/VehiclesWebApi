using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vehicles.Helpers
{
    public class CustomPair
    {
        public int First { get; private set; }
        public int Second { get; private set; }
        public CustomPair(int first, int second)
        {
            this.First = first;
            this.Second = second;
        }

        public override bool Equals(object obj)
        {
            if (Object.ReferenceEquals(this, obj))
            {
                return true;
            }
            CustomPair instance = obj as CustomPair;
            if (instance == null)
            {
                return false;
            }
            return this.First == instance.First && this.Second == instance.Second/* ||
                   this.First == instance.Second && this.Second == instance.First*/;
        }

        public override int GetHashCode()
        {
            return this.First.GetHashCode() ^ this.Second.GetHashCode();
        }
    }

}
