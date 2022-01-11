using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp2
{
    partial class MainWindow : Window
    {
        private void FillPolygon(List<Edge> edges, IFillableShape filledShape)
        {            
            List<Edge>[] ET;
            int ETcount = 0; // liczba krawędzi w ET
            ET = DivideIntoBuckets(edges, ref ETcount);
            if (ETcount==0) return;

            List<Edge> AET = new List<Edge>();
            int ymin = ET[0].First().minY;
            int y = ymin;

            while (AET.Count > 0 || ETcount > 0)
            {
                if (y - ymin < ET.Length && ET[y - ymin].Count > 0)
                {
                    AET.AddRange(ET[y - ymin]);

                    ETcount -= ET[y - ymin].Count;

                    AET = SortByX(AET);
                }

                List<Edge> newAET = new List<Edge>(AET);
                foreach (Edge edge in AET)
                {
                    if (y == edge.maxY)
                        newAET.Remove(edge);
                }                         
                AET = newAET;

                AET = SortByX(AET);

                if (interpolatedFilling) InterpolatedScanLineFill(AET, y, (Triangle)filledShape);
                else ScanLineFill(AET, y, filledShape);
                
                y++;

                foreach (Edge edge in AET)
                {
                    if (edge.m != 0)
                        edge.currX += (1.0 / edge.m);
                }
            }
        }

        private List<Edge>[] DivideIntoBuckets(List<Edge> edges, ref int ETcount)
        {
            int minValue = edges.First().minY;
            int maxValue = minValue;

            foreach (Edge edge in edges)
            {
                if (edge.minY > maxValue)
                    maxValue = edge.minY;
                if (edge.minY < minValue)
                    minValue = edge.minY;
            }

            List<Edge>[] buckets = new List<Edge>[maxValue - minValue + 1];

            for (int i = 0; i < buckets.Length; i++)
            {
                buckets[i] = new List<Edge>();
            }

            foreach (Edge edge in edges)
            {
                if (edge.maxY != edge.minY)
                {
                    buckets[edge.minY - minValue].Add(edge);
                    ETcount++;
                }
            }

            return buckets;
        }

        private List<Edge> SortByX(List<Edge> AET)
        {
            return AET.OrderBy(e => e.currX).ThenBy(e => e.maxX).ThenBy(e =>e.minX).ToList();
        }

        private void ScanLineFill(List<Edge> AET, int y, IFillableShape filledShape)
        {
            Edge startEdge = null, endEdge;

            foreach (Edge edge in AET)
            {
                if (startEdge == null)
                    startEdge = edge;
                else
                {
                    endEdge = edge;

                    for (int x = (int)(startEdge.currX + 0.5); x <= (endEdge.currX + 0.5); x++)
                    {
                        Color color = GetColor(x, y, filledShape);
                        double R = color.R;
                        SetPixel(x, y, color);
                    }

                    startEdge = null;
                }
            }
        }

        private void InterpolatedScanLineFill(List<Edge> AET, int y, Triangle filledTriangle)
        {
            Edge startEdge = null, endEdge;

            System.Drawing.Point[] vertices = new System.Drawing.Point[3];
            vertices[0] = filledTriangle.GetPointA();
            vertices[1] = filledTriangle.GetPointB();
            vertices[2] = filledTriangle.GetPointC();

            Color[] verticesColors = new Color[3];
            verticesColors[0] = GetColor(vertices[0].X, vertices[0].Y, filledTriangle);
            verticesColors[1] = GetColor(vertices[1].X, vertices[1].Y, filledTriangle);
            verticesColors[2] = GetColor(vertices[2].X, vertices[2].Y, filledTriangle);

            foreach (Edge edge in AET)
            {
                if (startEdge == null)
                    startEdge = edge;
                else
                {
                    endEdge = edge;

                    for (int x = (int)(startEdge.currX + 0.5); x <= (endEdge.currX + 0.5); x++)
                    {
                        SetPixel(x, y, GetInterpolatedColor(x, y, vertices, verticesColors));
                    }

                    startEdge = null;
                }
            }
        }
    }
}
