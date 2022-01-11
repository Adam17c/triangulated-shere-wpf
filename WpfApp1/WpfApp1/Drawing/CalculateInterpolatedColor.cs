using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2
{
    partial class MainWindow : System.Windows.Window
    {
        private Color GetInterpolatedColor(int x, int y, Point[] vertices, Color[] verticesColors)
        {
            Point P = new Point(x, y);

            double triangleArea = Triangle.Get2DTriangleArea(vertices[0], vertices[1], vertices[2]);

            double Pa = Triangle.Get2DTriangleArea(P, vertices[1], vertices[2]);
            double Pb = Triangle.Get2DTriangleArea(P, vertices[0], vertices[2]);
            double Pc = Triangle.Get2DTriangleArea(P, vertices[0], vertices[1]);

            if (triangleArea == 0)
            {
                double minDistance = double.MaxValue;
                int vertice = 0;
                for (int i = 0; i < vertices.Length; i++)
                {
                    double distance = Math.Pow(vertices[i].X - x, 2) + Math.Pow(vertices[i].Y - y, 2);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        vertice = i;
                    }
                }
                return verticesColors[vertice];
            }
            
            double alfa = Pa / triangleArea;
            double beta = Pb / triangleArea;
            double gamma = Pc / triangleArea;

            alfa = CutTo01((float)alfa);
            beta = CutTo01((float)beta);
            gamma = CutTo01((float)gamma);

            if (alfa == 1 && beta == 1 && gamma == 1)
            {
                double minDistance = double.MaxValue;
                int vertice = 0;
                for (int i = 0; i < vertices.Length; i++)
                {
                    double distance = Math.Sqrt(Math.Pow(vertices[i].X - x, 2) + Math.Pow(vertices[i].Y - y, 2));
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        vertice = i;
                    }
                }
                return verticesColors[vertice];
            }

            float Ir = CutTo01((float)alfa * verticesColors[0].R / 255 + (float)beta * verticesColors[1].R / 255 + (float)gamma * verticesColors[2].R / 255);
            float Ig = CutTo01((float)alfa * verticesColors[0].G / 255 + (float)beta * verticesColors[1].G / 255 + (float)gamma * verticesColors[2].G / 255);
            float Ib = CutTo01((float)alfa * verticesColors[0].B / 255 + (float)beta * verticesColors[1].B / 255 + (float)gamma * verticesColors[2].B / 255);
            return Color.FromArgb((int)(Ir * 255), (int)(Ig * 255), (int)(Ib * 255));
        }
    }
}
