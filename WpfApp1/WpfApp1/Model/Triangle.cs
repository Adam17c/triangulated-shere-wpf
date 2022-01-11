using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2
{
    public class Triangle : IFillableShape
    {
        public Vector3 left;
        public Vector3 right;
        public Vector3 vertical;
        //public Vector3 normalVector;

        private Edge leftEdge;
        private Edge rightEdge;
        private Edge horizontalEdge;


        public Triangle(Vector3 left, Vector3 right, Vector3 vertical)
        {
            this.left = left;
            this.right = right;
            this.vertical = vertical;
            //normalVector = VectorCalculations.GetNormalVector(left, right, vertical);
        }

        public List<Edge> GetEdges()
        {
            List<Edge> edges = new List<Edge>();
            leftEdge = new Edge(left, vertical);
            rightEdge = new Edge(right, vertical);
            horizontalEdge = new Edge(left, right);
            edges.Add(leftEdge);
            edges.Add(rightEdge);
            edges.Add(horizontalEdge);
            return edges;
        }

        public Point GetPointA()
        {
            return new Point((int)(left.X + 0.5), (int)(left.Y + 0.5));
        }

        public Point GetPointB()
        {
            return new Point((int)(right.X + 0.5), (int)(right.Y + 0.5));
        }

        public Point GetPointC()
        {
            return new Point((int)(vertical.X + 0.5), (int)(vertical.Y + 0.5));
        }

        public static double Get2DTriangleArea(Point A, Point B, Point C)
        {
            System.Windows.Vector a = new System.Windows.Vector(B.X - A.X, B.Y - A.Y);
            System.Windows.Vector b = new System.Windows.Vector(C.X - A.X, C.Y - A.Y);

            double product = System.Windows.Vector.CrossProduct(a, b);
            double res = product / 2;

            return Math.Abs(res);
        }

        public bool VertexClicked(Point clickPoint)
        {
            Edge clickedEdge = null;
            foreach (Edge edge in GetEdges())
                if (edge.WhichEndpointClicked(clickPoint) != null)
                    clickedEdge = edge;
            if (clickedEdge != null) 
                return true;
            return false;

        }

        public void MoveVertex(Point startPoint, Point endPoint)
        {
            int dx = endPoint.X - startPoint.X;
            int dy = endPoint.Y - startPoint.Y;

            if (Edge.EndpointClicked(startPoint, left))
            {
                left.X += dx;
                left.Y += dy;
                return;
            }

            if (Edge.EndpointClicked(startPoint, right))
            {
                right.X += dx;
                right.Y += dy;
                return;
            }

            if (Edge.EndpointClicked(startPoint, vertical))
            {
                vertical.X += dx;
                vertical.Y += dy;
                return;
            }
        }

        /*public Vector3 GetNormalVector()
        {
            return normalVector;
        }*/
    }
}
