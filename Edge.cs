using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace NetworkRouting
{
    class Edge
    {
        public PointF startPoint;
        public PointF endPoint;
        public double pathCost;

        public Edge(PointF startPoint)
        {
            this.startPoint = startPoint;
        }

        public Edge(PointF startPoint, PointF endpoint)
        {
            this.startPoint = startPoint;
            this.endPoint = endpoint;
        }
    }
}
