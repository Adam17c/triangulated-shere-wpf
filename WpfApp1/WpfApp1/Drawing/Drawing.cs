using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace WpfApp2
{
    public partial class MainWindow : Window
    {

        Color objectColor = Color.White;
        Color lightColor = Color.White;

        private void BmpPixelSnoopDrawing(TrianglesGrid grid)
        {
            if (!contentRendered) return;

            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap((int)canvas.ActualWidth, (int)canvas.ActualHeight);

            using (drawBitmapSnoop = new BmpPixelSnoop(bitmap))
            {
                FillGrid(grid);
                if (drawGrid) DrawGrid(grid);
            }

            image.Source = ConvertBitmapToBitmapImage.Convert(bitmap);
        }

        private void FillGrid(TrianglesGrid grid)
        {    
            /*foreach(Triangle triangle in grid.triangles)
            {
                FillPolygon(triangle.GetEdges(), triangle);
            }*/
            
                Parallel.ForEach(grid.triangles, triangle => {
                    FillPolygon(triangle.GetEdges(), triangle);
                });
        }

        private void DrawGrid(TrianglesGrid grid)
        {
            Parallel.ForEach(grid.triangles, triangle => {
                DrawPolygon(triangle.GetEdges());
            });
        }

        private void SetPixel(int x, int y, Color color)
        {
            drawBitmapSnoop.SetPixel(x, y, color);
        }

        //https://stackoverflow.com/questions/26260654/wpf-converting-bitmap-to-imagesource
        public class ConvertBitmapToBitmapImage
        {
            /// <summary>
            /// Takes a bitmap and converts it to an image that can be handled by WPF ImageBrush
            /// </summary>
            /// <param name="src">A bitmap image</param>
            /// <returns>The image as a BitmapImage for WPF</returns>
            public static BitmapImage Convert(System.Drawing.Bitmap src)
            {
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                ((System.Drawing.Bitmap)src).Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                ms.Seek(0, System.IO.SeekOrigin.Begin);
                image.StreamSource = ms;
                image.EndInit();
                return image;
            }
        }
    }
}
