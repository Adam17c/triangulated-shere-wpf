using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WpfApp2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        float kdVal;
        float ksVal;
        float kVal;
        float mVal;

        const int lightMovementRadius = 250;

        bool animation = false;

        bool drawGrid = false;
        bool interpolatedFilling = false;

        bool textureDrawing = false;

        TrianglesGrid trianglesGrid;

        bool contentRendered = false;

        BmpPixelSnoop drawBitmapSnoop;

        Vector3 lightSource;

        System.Drawing.Point prevMousePos;

        System.Drawing.Color[,] textureColors;

        public MainWindow()
        {
            InitializeComponent();            
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            trianglesGrid = new TrianglesGrid(new Point(canvas.ActualWidth / 2, canvas.ActualHeight / 2), 200, (int)triangulationSlider.Value);

            zSlider.Minimum = trianglesGrid.sphereRadius;
            zSlider.Maximum = trianglesGrid.sphereRadius + 200;

            GettextureColors();

            lightSource = new Vector3(trianglesGrid.sphereCenter.X + lightMovementRadius, trianglesGrid.sphereCenter.Y, (float)zSlider.Value);

            contentRendered = true;

            InitializeTimer();

            BmpPixelSnoopDrawing(trianglesGrid);

        }

        private void GettextureColors()
        {
            System.Drawing.Bitmap textureBitmap = new System.Drawing.Bitmap("BrickNormal.png");
            //System.Drawing.Bitmap textureBitmap = new System.Drawing.Bitmap("TygerClawsNormal2.png");
            
            //System.Drawing.Bitmap textureBitmap = new System.Drawing.Bitmap("bubbles_normal.png");
            BmpPixelSnoop textureBitmapSnoop;

            int Xoffset = (int)(trianglesGrid.Xoffset + 0.5);
            int Yoffset = (int)(trianglesGrid.Yoffset + 0.5);
            int diameter = 2 * (int)(trianglesGrid.sphereRadius + 1);

            //textureColors = new System.Drawing.Color[Xoffset + diameter + 1, Yoffset + diameter + 1];
            textureColors = new System.Drawing.Color[Xoffset + diameter, Yoffset + diameter];

            using (textureBitmapSnoop = new BmpPixelSnoop(textureBitmap))
            {
                for (int i = 0; i < Math.Min(diameter, textureBitmap.Width); i++)
                    for (int j = 0; j < Math.Min(diameter, textureBitmap.Height); j++)
                        textureColors[i + Xoffset, j + Yoffset] = textureBitmapSnoop.GetPixel(i, j);
            }
        }

        private void triangulationSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            trianglesGrid = new TrianglesGrid(new Point(canvas.ActualWidth / 2, canvas.ActualHeight / 2), 200, (int)triangulationSlider.Value);
            BmpPixelSnoopDrawing(trianglesGrid);
        }

        private void zSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            lightSource.Z = (float)zSlider.Value;
            BmpPixelSnoopDrawing(trianglesGrid);
        }

        private void ksSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ksVal = (float)ksSlider.Value;
            BmpPixelSnoopDrawing(trianglesGrid);
        }

        private void kdSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            kdVal = (float)kdSlider.Value;
            BmpPixelSnoopDrawing(trianglesGrid);
        }

        private void kSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            kVal = (float)kSlider.Value;
            BmpPixelSnoopDrawing(trianglesGrid);
        }

        private void mSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mVal = (float)mSlider.Value;
            BmpPixelSnoopDrawing(trianglesGrid);
        }

        private void copulaRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            textureDrawing = false;
            BmpPixelSnoopDrawing(trianglesGrid);
        }

        private void textureRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            textureDrawing = true;
            BmpPixelSnoopDrawing(trianglesGrid);
        }

        private void objectColorButton_Click(object sender, RoutedEventArgs e)
        {
            ColorDialog dialog = new ColorDialog();
            if(dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                objectColor = dialog.Color;
                objectColorRectangle.Fill = new SolidColorBrush(Color.FromArgb(dialog.Color.A,dialog.Color.R,dialog.Color.G,dialog.Color.B));
            }
            BmpPixelSnoopDrawing(trianglesGrid);
        }

        private void lightColorButton_Click(object sender, RoutedEventArgs e)
        {
            ColorDialog dialog = new ColorDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                lightColor = dialog.Color;
                lightColorRectangle.Fill = new SolidColorBrush(Color.FromArgb(dialog.Color.A, dialog.Color.R, dialog.Color.G, dialog.Color.B));
            }
            BmpPixelSnoopDrawing(trianglesGrid);
        }

        private void animationButton_Click(object sender, RoutedEventArgs e)
        {
            if (!animation)
            {
                Timer.Start();
                stopWatch.Start();
                animation = true;
                animationButton.Content = "Zatrzymaj animację swiatła";
            }
            else
            {
                Timer.Stop();
                stopWatch.Stop();
                animation = false;
                animationButton.Content = "Włącz animację światła";
            }
        }


        private void preciseDrawingCheckBox_Click(object sender, RoutedEventArgs e)
        {
            interpolatedFilling = false;
            BmpPixelSnoopDrawing(trianglesGrid);
        }

        private void interpolatedDrawingCheckbox_Click(object sender, RoutedEventArgs e)
        {
            interpolatedFilling = true;
            BmpPixelSnoopDrawing(trianglesGrid);
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            drawGrid = !drawGrid;
            BmpPixelSnoopDrawing(trianglesGrid);
        }

        private void canvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            System.Drawing.Point mousePos = new System.Drawing.Point((int)Mouse.GetPosition(canvas).X, (int)Mouse.GetPosition(canvas).Y);
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                if (mousePos.X >= trianglesGrid.Xoffset && mousePos.X <= trianglesGrid.Xoffset + 2 * trianglesGrid.sphereRadius &&
                    mousePos.Y >= trianglesGrid.Yoffset && mousePos.Y <= trianglesGrid.Yoffset + 2 * trianglesGrid.sphereRadius)
                    foreach (Triangle triangle in trianglesGrid.triangles)
                    if (triangle.VertexClicked(mousePos))
                            triangle.MoveVertex(prevMousePos, mousePos);
            }
            prevMousePos = mousePos;
            BmpPixelSnoopDrawing(trianglesGrid);
        }

    }
}
