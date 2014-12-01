using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace TurneroViewer.componentes.animations
{
    /// <summary>
    /// Interaction logic for DripAnimation.xaml
    /// </summary>
    public partial class DripAnimation : UserControl
    {
        DispatcherTimer _timer = null;
        Image _tempimage = null;
        double leftposition = 0;
        public DripAnimation()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(Window1_Loaded);
        }

        void Window1_Loaded(object sender, RoutedEventArgs e)
        {


            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(0.03);
            _timer.Tick += new EventHandler(_timer_Tick);
            _timer.IsEnabled = true;
            _tempimage = new Image();
            _tempimage.Width = 300;
            _tempimage.Height = 300;
            _tempimage.Stretch = Stretch.Fill;
            _tempimage.Source = loadBitmap(null); //cargar imagen
            _tempimage.Clip = new RectangleGeometry(new Rect(10, 0, 2, 300));

            gh.Source = loadBitmap(null); //cargar imagen

            VisualBrush vb = new VisualBrush(_tempimage as Visual);
            canani.Background = vb;
            Canvas.SetZIndex(canani, 3);
            Canvas.SetZIndex(gh, 2);

        }
        public static BitmapSource loadBitmap(System.Drawing.Bitmap source)
        {
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(source.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty,
                System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
        }

        void _timer_Tick(object sender, EventArgs e)
        {
            _tempimage.Clip = new RectangleGeometry(new Rect(leftposition, 0, 2, 300));
            VisualBrush vb = new VisualBrush(_tempimage as Visual);
            canani.Background = vb;
            Canvas.SetLeft(canani, leftposition);
            leftposition++;

            if (leftposition >= 300) _timer.IsEnabled = false;
            //canani.Width = canani.Width - leftposition;        

        }
    }
}
