using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace WpfApp2
{
    public partial class MainWindow : Window
    {
        public DispatcherTimer Timer;
        Stopwatch stopWatch = new Stopwatch();

        private void InitializeTimer()
        {
            Timer = new DispatcherTimer();
            Timer.Tick += TimerOnTick;
            Timer.Interval = new TimeSpan(0, 0, 0, 0, 45);

            stopWatch = new Stopwatch();
        }

        private void TimerOnTick(object sender, object o)
        {
            lightSource.X = trianglesGrid.sphereCenter.X + (float)Math.Cos(stopWatch.Elapsed.TotalSeconds) * lightMovementRadius;
            lightSource.Y =  trianglesGrid.sphereCenter.Y + (float)Math.Sin(stopWatch.Elapsed.TotalSeconds) * lightMovementRadius;

            BmpPixelSnoopDrawing(trianglesGrid);
        }
    }
}
