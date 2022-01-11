using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WpfApp2
{
    public static class VectorCalculations
    {
        public static float CountCosunis(Vector3 v1, Vector3 v2)
        {   
            return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
        }

        public static Vector3 GetVectorR(Vector3 N, Vector3 L, float NLcos)
        {
            return new Vector3(2 * NLcos * N.X - L.X, 2 * NLcos * N.Y - L.Y, 2 * NLcos * N.Z - L.Z);
        }

        public static Vector3 GetVectorL(int x, int y, Vector3 lightSourcePos)
        {
            lightSourcePos.X -= x;
            lightSourcePos.Y -= y;
            return Vector3.Normalize(lightSourcePos);
        }

        //https://stackoverflow.com/questions/1966587/given-3-points-how-do-i-calculate-the-normal-vector
        /*public static Vector3 GetNormalVector(Vector3 a, Vector3 b, Vector3 c)
        {
            Vector3 dir = Vector3.Cross(c - a, b - a);
            if (dir.Z < 0) dir = -dir;
            Vector3 norm = Vector3.Normalize(dir);
            return norm;
        }*/

        public static Vector3 GetNormalVector(float x, float y, Triangle triangle, Vector3 sphereCenter)
        {
            Vector3 res = new Vector3(x, y, GetZForXYInTriangle(x, y, triangle)) - sphereCenter;
            return Vector3.Normalize(res); ;
        }

        //https://math.stackexchange.com/questions/851742/calculate-coordinate-of-any-point-on-triangle-in-3d-plane
        public static float GetZForXYInTriangle(float x, float y, Triangle triangle)
        {
            float x1 = triangle.left.X;
            float x2 = triangle.right.X;
            float x3 = triangle.vertical.X;
            float y1 = triangle.left.Y;
            float y2 = triangle.right.Y;
            float y3 = triangle.vertical.Y;
            float z1 = triangle.left.Z;
            float z2 = triangle.right.Z;
            float z3 = triangle.vertical.Z;

            return z1 + (((x2 - x1) * (z3 - z1) - (x3 - x1) * (z2 - z1)) / ((x2 - x1) * (y3 - y1) - (x3 - x1) * (y2 - y1))) *
                (y - y1) - (x - x1) * (((y2 - y1) * (z3 - z1) - (y3 - y1) * (z2 - z1)) / ((x2 - x1) * (y3 - y1) - (x3 - x1) * (y2 - y1)));
        }

        public static Vector3 GetMixedNormalVector(int x, int y, float k, Vector3 sphereNormalVector, System.Drawing.Color[,] textureColors)
        {
            Vector3 textureNormalVector = GetNormalVectorFromTexture(x, y, textureColors);
            //textureNormalVector.Z = Math.Abs(textureNormalVector.Z);
            Vector3[] M = GetMatrix(sphereNormalVector);

            Vector3 multipliedVector = MultiplyMatrixByVector(M, textureNormalVector);

            if (k == 0)
            {
                if (multipliedVector.X == 0 && multipliedVector.Y == 0 && multipliedVector.Z == 0)
                    return multipliedVector;
            }

            return Vector3.Normalize( k * sphereNormalVector + (1 - k) *
                MultiplyMatrixByVector(M, textureNormalVector)
                );
        }

        private static Vector3 GetNormalVectorFromTexture(int x, int y, System.Drawing.Color[,] textureColors)
        {
            System.Drawing.Color color = textureColors[x, y];
            
            float X = (float)(color.R - 127) / 127;
            float Y = - (float)(color.G - 127) / 127;
            float Z = (float)(color.B - 127) / 127;
            return Vector3.Normalize(new Vector3(X, Y, Z));
        }

        private static Vector3[] GetMatrix(Vector3 N)
        {
            Vector3 B = Vector3.Normalize(N * new Vector3(0, 0, 1));
            if (N.X == 0 && N.Y == 0 && N.Z == 1)
                B = new Vector3(0, 1, 0);
            Vector3 T = Vector3.Normalize(B * N);

            Vector3[] res = new Vector3[3];
            res[0] = T;
            res[1] = B;
            res[2] = N;
            return res;
        }

        private static Vector3 MultiplyMatrixByVector(Vector3[] M, Vector3 N)
        {
            /*return new Vector3(
                M[0].X * N.X + M[0].Y * N.Y + M[0].Z * N.Y,
                M[1].X * N.X + M[1].Y * N.Y + M[1].Z * N.Y,
                M[2].X * N.X + M[2].Y * N.Y + M[2].Z * N.Y);*/

            return new Vector3(
                M[0].X * N.X + M[1].X * N.Y + M[2].X * N.Z,
                M[0].Y * N.X + M[1].Y * N.Y + M[2].Y * N.Z,
                M[0].Z * N.X + M[1].Z * N.Y + M[2].Z * N.Z);
        }
    }
}
