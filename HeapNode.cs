using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkRouting
{
    class HeapNode
    {
        public double pathCost = double.MaxValue;
        public int LOOKUPINDEX;
        public int backPointer;

        public HeapNode(int lookupIndex, double pathCost, int backPointer)
        {
            this.LOOKUPINDEX = lookupIndex;
            this.pathCost = pathCost;
            this.backPointer = backPointer;
        }
    }
}
