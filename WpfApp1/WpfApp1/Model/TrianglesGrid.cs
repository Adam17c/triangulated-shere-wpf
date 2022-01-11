using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2
{
    class TrianglesGrid
    {
        public List<Triangle> triangles = new List<Triangle>();
        public float Xoffset, Yoffset, sphereRadius;
        public Vector3 sphereCenter;

        public TrianglesGrid(Point center, float radius, int precision) 
        {
            Xoffset = (float)center.X - radius;
            Yoffset = (float)center.Y - radius;
            this.sphereRadius = radius;
            InitializeGrid(precision);
        }

        //http://blog.andreaskahler.com/2009/06/creating-icosphere-mesh-in-code.html
        private void InitializeGrid(int precision)
        {
            Vector3 baseA = new Vector3(Xoffset + sphereRadius, Yoffset, 0);
            Vector3 baseB = new Vector3(Xoffset + 2 * sphereRadius, Yoffset + sphereRadius, 0);
            Vector3 baseC = new Vector3(Xoffset + sphereRadius, Yoffset + 2 * sphereRadius, 0);
            Vector3 baseD = new Vector3(Xoffset, Yoffset + sphereRadius, 0);
            Vector3 top = new Vector3(Xoffset + sphereRadius, Yoffset + sphereRadius, sphereRadius);

            sphereCenter = new Vector3(Xoffset + sphereRadius, Yoffset + sphereRadius, 0);

            triangles.Add(new Triangle(baseB, baseA, top));
            triangles.Add(new Triangle(baseC, baseB, top));
            triangles.Add(new Triangle(baseD, baseC, top));
            triangles.Add(new Triangle(baseD, baseA, top));

            List<Triangle> oldTriangles;
            List<Triangle> newTriangles;

            for (int i = 0; i < precision; i++)
            {
                oldTriangles = triangles;
                newTriangles = new List<Triangle>();
                foreach (Triangle triangle in triangles)
                {
                    Vector3 leftEdgeMid = GetMiddleVector(triangle.left, triangle.vertical);
                    Vector3 rightEdgeMid = GetMiddleVector(triangle.right, triangle.vertical);
                    Vector3 HorizontalEdgeMid = GetMiddleVector(triangle.left, triangle.right);

                    NormalizePoint(ref leftEdgeMid);
                    NormalizePoint(ref rightEdgeMid);
                    NormalizePoint(ref HorizontalEdgeMid);

                    newTriangles.Add(new Triangle(leftEdgeMid, rightEdgeMid, triangle.vertical));
                    newTriangles.Add(new Triangle(leftEdgeMid, rightEdgeMid, HorizontalEdgeMid));
                    newTriangles.Add(new Triangle(triangle.left, HorizontalEdgeMid, leftEdgeMid));
                    newTriangles.Add(new Triangle(HorizontalEdgeMid, triangle.right, rightEdgeMid));

                }
                triangles = triangles.Except(oldTriangles).ToList();
                triangles.AddRange(newTriangles);
            }
        }

        private void NormalizePoint(ref Vector3 point)
        {
            Vector3 diff = point - sphereCenter;
            point = sphereCenter + diff / diff.Length() * sphereRadius;
        }

        private Vector3 GetMiddleVector(Vector3 start, Vector3 end)
        {
            float newX = start.X + (end.X - start.X) / 2;
            float newY = start.Y + (end.Y - start.Y) / 2;
            float newZ = start.Z + (end.Z - start.Z) / 2;

            return new Vector3(newX, newY, newZ);
        }

    }
}
