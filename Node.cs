using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkRouting
{
    class Node
    {
        public double pathCost = double.MaxValue;
        public int LOOKUPINDEX;
        public int backPointer;

        public Node(int lookupIndex, double pathCost, int backPointer)
        {
            this.LOOKUPINDEX = lookupIndex;
            this.pathCost = pathCost;
        }
    }
}
