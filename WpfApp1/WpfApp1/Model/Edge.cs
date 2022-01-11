using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2
{
    public class Edge
    {
        public Vector3 startVector;
        public Vector3 endVector;
        public int minX { set; get; }
        public int maxX { set; get; }
        public int minY { set; get; }
        public int maxY { set; get; }
        public double m { set; get; }
        public double currX { set; get; }
        private static int clickArea = 5;

        public Edge(Vector3 startVector, Vector3 endVector)
        {
            this.startVector = startVector;
            this.endVector = endVector;
            SetParameters();
        }

        public Edge(Point startPoint, Point endPoint)
        {
            this.startVector = new Vector3(startPoint.X, startPoint.Y, 0);
            this.endVector = new Vector3(endPoint.X, endPoint.Y, 0); ;
            SetParameters();
        }

        private void SetParameters()
        {
            currX = startVector.Y < endVector.Y ? (int)(startVector.X + 0.5) : (int)(endVector.X + 0.5);
            minX = (int)(Math.Min(startVector.X, endVector.X)+ 0.5);
            maxX = (int)(Math.Max(startVector.X, endVector.X) + 0.5);
            minY = (int)(Math.Min(startVector.Y, endVector.Y) + 0.5);
            maxY = (int)(Math.Max(startVector.Y, endVector.Y) + 0.5);
            double dx = startVector.X - endVector.X, dy = startVector.Y - endVector.Y;
            m = dy / dx;
        }

        public Point? WhichEndpointClicked(Point clickPoint)
        {
            if(EndpointClicked(clickPoint, startVector))
                return new Point((int)(startVector.X + 0.5), (int)(startVector.Y + 0.5));
            if (EndpointClicked(clickPoint, endVector))
                return new Point((int)(endVector.X + 0.5), (int)(endVector.Y + 0.5));
            return null;
        }

        public static bool EndpointClicked(Point clickPoint, Vector3 endpoint)
        {
            return clickPoint.X > (int)(endpoint.X + 0.5) - clickArea && clickPoint.X < (int)(endpoint.X + 0.5) + clickArea
                && clickPoint.Y > (int)(endpoint.Y + 0.5) - clickArea && clickPoint.Y < (int)(endpoint.Y + 0.5) + clickArea;
        }
    }
}
