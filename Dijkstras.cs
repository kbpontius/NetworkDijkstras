using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;


namespace NetworkRouting
{
    class Dijkstras
    {
        private PQ pq;
        private List<PointF> points = new List<PointF>();
        private List<HashSet<int>> adjacencyList;
        private int startIndex = -1;
        private int endIndex = -1;

        // MARK: INITIALIZER
        public Dijkstras(int startIndex, int endIndex, List<PointF> points, List<HashSet<int>> adjacencyList)
        {
            this.startIndex = startIndex;
            this.endIndex = endIndex;
            this.points = points;
            this.adjacencyList = adjacencyList;

            pq = new PQ(points.Count);
        }

        // MARK: PRIMARY METHODS
        public List<int> ResolvePath()
        {
            List<int> backPointerList = new List<int>();

            populateOnePath(startIndex);
            int currentIndex = startIndex;
            currentIndex = pq.PopMin();

            while (!pq.IsEmpty())
            {
                if (currentIndex == endIndex) { break; }

                foreach (int lookupIndex in adjacencyList[currentIndex])
                {
                    double newPathCost = pq.GetPathCost(currentIndex) + GetDistance(points[currentIndex], points[lookupIndex]);
                    double oldPathCost = pq.GetPathCost(lookupIndex);

                    if (oldPathCost == -1)
                    {
                        pq.Add(lookupIndex, newPathCost, currentIndex);
                    }
                    else if (oldPathCost > newPathCost && !pq.NodeIsVisited(lookupIndex))
                    {
                        pq.DecreaseKey(lookupIndex, newPathCost, currentIndex);
                    }
                }

                currentIndex = pq.PopMin();
            }

            if (currentIndex == endIndex)
            {
                int backPointer = -1;
                backPointerList.Add(currentIndex);

                while (currentIndex != startIndex)
                {
                    backPointer = pq.GetBackPointer(currentIndex);
                    currentIndex = backPointer;
                    backPointerList.Add(currentIndex);
                }

                return backPointerList;
            }
            else
            {
                return backPointerList;
            }
        }

        // MARK: HELPER METHODS
        private void ReevalutePathCost(int index)
        {
            double currentCost = pq.GetPathCost(index);

            foreach (int connection in adjacencyList[index])
            {
                double pathCost = pq.GetPathCost(connection) + currentCost;
            }
        }

        private double GetDistance(PointF p1, PointF p2)
        {
            return Math.Sqrt(Math.Pow(p2.Y - p1.Y, 2) + Math.Pow(p2.X - p1.X, 2));
        }

        private void populateOnePath(int index)
        {
            foreach (int connection in adjacencyList[index])
            {
                pq.Add(connection, GetDistance(points[startIndex], points[connection]), index);
            }
        }
    }
}
