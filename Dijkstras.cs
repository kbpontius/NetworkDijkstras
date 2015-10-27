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
        }

        // MARK: PRIMARY METHODS
        public List<Edge> CalculateAllPaths()
        {
            pq = new PQ(points.Count);
            populateAllPaths(startIndex);

            int currentIndex = CalculateResults(startIndex);

            return CompileResults(currentIndex);
        }

        public List<Edge> CalculateOnePath()
        {
            pq = new PQ(points.Count);

            populateOnePath(startIndex);
            int currentIndex = pq.PopMin();

            currentIndex = CalculateResults(currentIndex);

            return CompileResults(currentIndex);
        }

        // MARK: HELPER METHODS
        public static double GetDistance(PointF p1, PointF p2)
        {
            return Math.Sqrt(Math.Pow(p2.Y - p1.Y, 2) + Math.Pow(p2.X - p1.X, 2));
        }

        private int CalculateResults(int currentIndex)
        {
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

                if (currentIndex == startIndex)
                {
                    currentIndex = pq.PopMin();
                }
            }

            return currentIndex;
        }

        private List<Edge> CompileResults(int currentIndex)
        {
            List<Edge> edgeList = new List<Edge>();

            if (currentIndex == endIndex)
            {
                while (currentIndex != startIndex)
                {
                    int backPointer = pq.GetBackPointer(currentIndex);
                    edgeList.Add(new Edge(points[currentIndex], points[backPointer]));
                    currentIndex = backPointer;
                }

                return edgeList;
            }
            else
            {
                return edgeList;
            }
        }

        private void ReevalutePathCost(int index)
        {
            double currentCost = pq.GetPathCost(index);

            foreach (int connection in adjacencyList[index])
            {
                double pathCost = pq.GetPathCost(connection) + currentCost;
            }
        }

        private void populateAllPaths(int index)
        {
            for (int i = 0; i < points.Count; i++)
            {
                if (i == startIndex)
                {
                    pq.Add(i, 0, i);
                }
                else
                {
                    pq.Add(i, Double.MaxValue, -1);
                }
            }
        }

        private void populateOnePath(int index)
        {
            foreach (int connection in adjacencyList[index])
            {
                pq.Add(connection, GetDistance(points[this.startIndex], points[connection]), index);
            }
        }
    }
}
