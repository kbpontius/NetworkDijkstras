using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NetworkRouting
{
    public partial class Form1 : Form
    {
        Pen pen = new Pen(Color.Red, 1f);
        Stopwatch stopwatch = new Stopwatch();
        double onePathTime = 0;
        double allPathsTime = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void generateButton_Click(object sender, EventArgs e)
        {
            int randomSeed = int.Parse(randomSeedBox.Text);
            int size = int.Parse(sizeBox.Text);

            Random rand = new Random(randomSeed);
            seedUsedLabel.Text = "Random Seed Used: " + randomSeed.ToString();

            this.adjacencyList = generateAdjacencyList(size, rand);
            List<PointF> points = generatePoints(size, rand);
            resetImageToPoints(points);
            this.points = points;
            startNodeIndex = -1;
            stopNodeIndex = -1;
            sourceNodeBox.Text = "";
            targetNodeBox.Text = "";
            sourceNodeBox.Enabled = true;
            targetNodeBox.Enabled = true;
        }

        // Generates the distance matrix.  Values of -1 indicate a missing edge.  Loopbacks are at a cost of 0.
        private const int MIN_WEIGHT = 1;
        private const int MAX_WEIGHT = 100;
        private const double PROBABILITY_OF_DELETION = 0.35;

        private const int NUMBER_OF_ADJACENT_POINTS = 3;

        private List<HashSet<int>> generateAdjacencyList(int size, Random rand)
        {
            List<HashSet<int>> adjacencyList = new List<HashSet<int>>();

            for (int i = 0; i < size; i++)
            {
                HashSet<int> adjacentPoints = new HashSet<int>();
                while (adjacentPoints.Count < 3)
                {
                    int point = rand.Next(size);
                    if (point != i) adjacentPoints.Add(point);
                }
                adjacencyList.Add(adjacentPoints);
            }

            return adjacencyList;
        }

        private List<PointF> generatePoints(int size, Random rand)
        {
            List<PointF> points = new List<PointF>();
            for (int i = 0; i < size; i++)
            {
                points.Add(new PointF((float) (rand.NextDouble() * pictureBox.Width), (float) (rand.NextDouble() * pictureBox.Height)));
            }
            return points;
        }

        private void resetImageToPoints(List<PointF> points)
        {
            pictureBox.Image = new Bitmap(pictureBox.Width, pictureBox.Height);
            Graphics graphics = Graphics.FromImage(pictureBox.Image);
            foreach (PointF point in points)
            {
                graphics.DrawEllipse(new Pen(Color.Blue), point.X, point.Y, 2, 2);
            }

            this.graphics = graphics;
            pictureBox.Invalidate();
        }

        // These variables are instantiated after the "Generate" button is clicked
        private List<PointF> points = new List<PointF>();
        private Graphics graphics;
        private List<HashSet<int>> adjacencyList;

        // Use this to generate routing tables for every node
        private void solveButton_Click(object sender, EventArgs e)
        {
            LoadStartEndIndices();

            if (startNodeIndex != -1 && stopNodeIndex != -1 && startNodeIndex != stopNodeIndex)
            {
                Dijkstras dk = new Dijkstras(startNodeIndex, stopNodeIndex, points, adjacencyList);

                stopwatch = Stopwatch.StartNew();
                List<Edge> backPointerList = dk.CalculateOnePath();
                stopwatch.Stop();

                onePathTime = stopwatch.Elapsed.TotalMilliseconds;

                stopwatch = Stopwatch.StartNew();
                dk.CalculateAllPaths();
                stopwatch.Stop();

                allPathsTime = stopwatch.Elapsed.TotalMilliseconds;

                oneTimeBox.Text = Convert.ToString(onePathTime / 1000);
                allTimeBox.Text = Convert.ToString(allPathsTime / 1000);
                differenceBox.Text = Convert.ToString((onePathTime / allPathsTime) * 100);

                if (backPointerList.Count > 0)
                {
                    Tuple<double, List<Edge>> calculations = CalculatePathCosts(backPointerList);
                    backPointerList = calculations.Item2;
                    double totalCost = calculations.Item1;
                    pathCostBox.Text = Convert.ToString(totalCost);

                    DrawLineList(backPointerList);
                }
                else
                {
                    pathCostBox.Text = "Unreachable.";
                }
            }
            else
            {
                seedUsedLabel.Text = "Invalid start/end nodes.";
            }
        }

        private void LoadStartEndIndices()
        {
            if (sourceNodeBox.Text.Length > 0 && targetNodeBox.Text.Length > 0)
            {
                int desiredStartIndex = Convert.ToInt32(sourceNodeBox.Text);
                int desiredStopIndex = Convert.ToInt32(targetNodeBox.Text);

                if (desiredStartIndex < points.Count
                        && desiredStartIndex >= 0)
                {
                    startNodeIndex = desiredStartIndex;
                }

                if (desiredStopIndex < points.Count
                        && desiredStopIndex >= 0)
                {
                    stopNodeIndex = desiredStopIndex;
                }

                paintStartStopPoints();
            }
        }

        public double FormatDecimal(double modifyMe, int numberOfDecimals)
        {
            double decimalPlaceMultiplier = Math.Pow(10, numberOfDecimals);
            return Convert.ToInt32(modifyMe * decimalPlaceMultiplier) / decimalPlaceMultiplier;
        }

        private void DrawLineList(List<Edge> pointerList)
        {
            for (int i = 0; i < pointerList.Count; i++)
            {
                DrawLine(pointerList[i].startPoint, pointerList[i].endPoint);
                DrawPathCost(GetMidpoint(pointerList[i].startPoint, pointerList[i].endPoint), FormatDecimal(pointerList[i].pathCost, 2));
            }

            pictureBox.Refresh();
        }

        private Tuple<double, List<Edge>> CalculatePathCosts(List<Edge> pointerList)
        {
            double totalCost = 0;

            foreach (Edge edge in pointerList)
            {
                edge.pathCost = Dijkstras.GetDistance(edge.startPoint, edge.endPoint);
                totalCost += edge.pathCost;
            }

            return Tuple.Create(totalCost, pointerList);
        }

        private void DrawLine(PointF point1, PointF point2)
        {
            graphics.DrawLine(pen, point1, point2);
        }

        private void DrawPathCost(PointF midpoint, double pathCost)
        {
            graphics.DrawString(Convert.ToString(pathCost), new Font(FontFamily.GenericSansSerif, 10, FontStyle.Regular), new SolidBrush(Color.Red), midpoint);
        }

        private PointF GetMidpoint(PointF p1, PointF p2)
        {
            PointF midpoint = new PointF();
            midpoint.X = ((p2.X - p1.X) / 2) + p1.X;
            midpoint.Y = ((p2.Y - p1.Y) / 2) + p1.Y;

            return midpoint;
        }

        private Boolean startStopToggle = true;
        private int startNodeIndex = -1;
        private int stopNodeIndex = -1;
        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (points.Count > 0)
            {
                Point mouseDownLocation = new Point(e.X, e.Y);
                int index = ClosestPoint(points, mouseDownLocation);
                if (startStopToggle)
                {
                    startNodeIndex = index;
                    sourceNodeBox.Text = "" + index;
                }
                else
                {
                    stopNodeIndex = index;
                    targetNodeBox.Text = "" + index;
                }
                startStopToggle = !startStopToggle;

                resetImageToPoints(points);
                paintStartStopPoints();
            }
        }

        private void paintStartStopPoints()
        {
            if (startNodeIndex > -1)
            {
                Graphics graphics = Graphics.FromImage(pictureBox.Image);
                graphics.DrawEllipse(new Pen(Color.Green, 6), points[startNodeIndex].X, points[startNodeIndex].Y, 1, 1);
                this.graphics = graphics;
                pictureBox.Invalidate();
            }

            if (stopNodeIndex > -1)
            {
                Graphics graphics = Graphics.FromImage(pictureBox.Image);
                graphics.DrawEllipse(new Pen(Color.Red, 2), points[stopNodeIndex].X - 3, points[stopNodeIndex].Y - 3, 8, 8);
                this.graphics = graphics;
                pictureBox.Invalidate();
            }
        }

        private int ClosestPoint(List<PointF> points, Point mouseDownLocation)
        {
            double minDist = double.MaxValue;
            int minIndex = 0;

            for (int i = 0; i < points.Count; i++)
            {
                double dist = Math.Sqrt(Math.Pow(points[i].X-mouseDownLocation.X,2) + Math.Pow(points[i].Y - mouseDownLocation.Y,2));
                if (dist < minDist)
                {
                    minIndex = i;
                    minDist = dist;
                }
            }

            return minIndex;
        }
    }
}
