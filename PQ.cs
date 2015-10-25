using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace NetworkRouting
{
    class PQ
    {
        private List<Node> heap = new List<Node>();
        private List<int> lookupTable = new List<int>();

        // MARK: INITIALIZER
        public PQ(int lookupTableSize)
        {
            for (int i = 0; i < lookupTableSize; i++)
            {
                lookupTable.Add(-1);   
            }
        }

        // MARK: PRIMARY METHODS
        public void Add(int lookupIndex, double value, int backPointer)
        {
            lookupTable[lookupIndex] = heap.Count;
            heap.Add(new Node(lookupIndex, value, backPointer));

            BubbleUp(heap.Count - 1);
        }

        public int PopMin()
        {
            Node minNode = heap[0];
            heap[0] = heap[heap.Count - 1];
            heap.RemoveAt(heap.Count - 1);

            BubbleDown(0);

            return minNode.LOOKUPINDEX;
        }

        public void DecreaseKey(int lookupTableIndex, double newPathValue, int newBackPointer)
        {
            int heapIndex = lookupTable[lookupTableIndex];
            heap[heapIndex].pathCost = newPathValue;
            heap[lookupTable[lookupTableIndex]].backPointer = newBackPointer;

            BubbleUp(heapIndex);
        }

        public bool IsEmpty()
        {
            return heap.Count <= 0;
        }

        public void UpdatePathCost(int lookupIndex, double newPathCost)
        {

        }

        public double GetPathCost(int lookupIndex)
        {

            if (NodeIsAdded(lookupIndex))
            {
                return heap[lookupTable[lookupIndex]].pathCost;
            }

            return -1;
        }

        public int GetBackPointer(int lookupIndex)
        {
            return lookupIndex[]
        }

        public bool NodeIsAdded(int lookupIndex)
        {
            return lookupTable[lookupIndex] != -1;
        }

        public bool NodeIsVisited(int lookupIndex)
        {
            return lookupTable[lookupIndex] == 0 && heap[0].LOOKUPINDEX != lookupIndex;
        }

        // MARK: HELPER METHODS
        private void BubbleUp(int index)
        {
            int currentIndex = index;
            double currentValue = double.MinValue;
            double parentValue = double.MaxValue;

            while (currentIndex != 0 && parentValue > currentValue)
            {
                currentValue = heap[currentIndex].pathCost;
                parentValue = GetParentValue(currentIndex);

                if (currentValue < parentValue)
                {
                    SwapIndices(currentIndex, GetParentIndex(currentIndex));
                    currentIndex = GetParentIndex(currentIndex);
                }
            }
        }

        private void BubbleDown(int index)
        {
            int currentIndex = index;
            int minChildIndex = -1;
            double currentValue = double.MaxValue;
            double minChildValue = double.MinValue;

            while (HasLeftChild(currentIndex) && minChildValue < currentValue)
            {
                currentValue = heap[currentIndex].pathCost;
                minChildIndex = GetMinChildIndex(currentIndex);
                minChildValue = heap[minChildIndex].pathCost;

                if (currentValue > minChildValue)
                {
                    SwapIndices(currentIndex, minChildIndex);
                    currentIndex = minChildIndex;
                }
            }
        }

        private void SwapIndices(int i1, int i2)
        {
            // Swap lookupTable values.
            lookupTable[heap[i1].LOOKUPINDEX] = i2;
            lookupTable[heap[i2].LOOKUPINDEX] = i1;

            Node tempNode = heap[i1];

            // Perform Node swap.
            heap[i1] = heap[i2];
            heap[i2] = tempNode;
        }

        // MARK: INDEX GETTER METHODS
        private int GetParentIndex(int index)
        {
            return (index - 1) / 2;
        }

        private int GetLeftChildIndex(int index)
        {
            return (2 * index) + 1;
        }

        private int GetRightChildIndex(int index)
        {
            return (2 * index) + 2;
        }

        private bool HasLeftChild(int index)
        {
            return GetLeftChildIndex(index) <= heap.Count - 1;
        }

        private bool HasRightChild(int index)
        {
            return GetRightChildIndex(index) <= heap.Count - 1;
        }

        // MARK: VALUE METHODS
        private int GetMinChildIndex(int index)
        {
            double leftChildValue = int.MaxValue;
            double rightChildValue = int.MaxValue;

            if (HasLeftChild(index))
            {
                leftChildValue = GetLeftChildValue(index);
            }

            if (HasRightChild(index))
            {
                rightChildValue = GetRightChildValue(index);
            }

            if (leftChildValue < rightChildValue)
            {
                return GetLeftChildIndex(index);
            }
            else
            {
                return GetRightChildIndex(index);
            }
        }

        private double GetParentValue(int index)
        {
            return heap[GetParentIndex(index)].pathCost;
        }

        private double GetLeftChildValue(int index)
        {
            return heap[GetLeftChildIndex(index)].pathCost;
        }

        private double GetRightChildValue(int index)
        {
            return heap[GetRightChildIndex(index)].pathCost;
        }
    }
}
