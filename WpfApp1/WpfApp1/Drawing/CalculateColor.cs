using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp2
{
    partial class MainWindow : Window
    {
        private Color GetColor(int x, int y, IFillableShape filledShape)
        {
            //Vector3 normalVector = Vector3.Normalize(new Vector3(x - trianglesGrid.sphereCenter.X, y - trianglesGrid.sphereCenter.Y, 0));

            //Vector3 normalVector = VectorCalculations.CalculateNormalSphereVector(VectorCalculations.PointInWorld(x, y, (Triangle)filledShape), trianglesGrid.sphereCenter);
            //Vector3 lightVersor = VectorCalculations.GetVectorL(x, y, lightSource);

            Vector3 normalVector = VectorCalculations.GetNormalVector(x, y, (Triangle)filledShape, trianglesGrid.sphereCenter);
            if (textureDrawing) normalVector = VectorCalculations.GetMixedNormalVector(x, y, kVal, normalVector, textureColors);
            
            

            Vector3 lightVersor = VectorCalculations.GetVectorL(x, y, lightSource);

            if (textureDrawing && kVal == 0)
                /*Title =*/
                kVal.ToString();

            return CalculateColor(kdVal, ksVal, mVal, lightColor, objectColor, normalVector, lightVersor, x, y);
        }

        private Color CalculateColor(float kd, float ks, float m, Color lightColor, Color objectColor, Vector3 N, Vector3 L, int x, int y)
        {
            if (textureDrawing && kVal == 0)
                /*Title =*/ kVal.ToString();

            if (textureDrawing) objectColor = textureColors[x, y];
            
            Vector3 V = new Vector3(0, 0, 1);
            float cosNL = CutTo01(VectorCalculations.CountCosunis(N, L));
            Vector3 R = Vector3.Normalize(VectorCalculations.GetVectorR(N, L, cosNL));
            double cosVR = CutTo01(VectorCalculations.CountCosunis(V, R));
            float Ir = CutTo01(kd * ((float)lightColor.R / 255) * ((float)objectColor.R / 255) * cosNL + ks * ((float)lightColor.R / 255) * ((float)objectColor.R / 255) * (float)Math.Pow(cosVR, m));
            float Ig = CutTo01(kd * ((float)lightColor.G / 255) * ((float)objectColor.G / 255) * cosNL + ks * ((float)lightColor.G / 255) * ((float)objectColor.G / 255) * (float)Math.Pow(cosVR, m));
            float Ib = CutTo01(kd * ((float)lightColor.B / 255) * ((float)objectColor.B / 255) * cosNL + ks * ((float)lightColor.B / 255) * ((float)objectColor.B / 255) * (float)Math.Pow(cosVR, m));
            return Color.FromArgb((int)(Ir * 255), (int)(Ig * 255), (int)(Ib * 255));
        }

        public float GetZForXY(float x, float y)
        {
            float res;
            try
            {
                res = (float)Math.Sqrt(trianglesGrid.sphereRadius * trianglesGrid.sphereRadius - (x * x + y * y));
            }
            catch (Exception)
            {
                return 0;
            }
            return res;
        }

        public  float CutTo01(float val)
        {
            return val > 1 ? 1 : val < 0 ? 0 : val;
        }
    }
}
