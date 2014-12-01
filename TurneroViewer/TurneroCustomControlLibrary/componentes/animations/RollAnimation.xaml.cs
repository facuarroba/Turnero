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
    /// Interaction logic for RollAnimation.xaml
    /// </summary>
    public partial class RollAnimation : UserControl
    {
        DispatcherTimer _timer = null;
        double _rollTopLeft = 0;
        double _imageWidth = 400;
        double _imageHeight = 300;
        double _offset = 3;
        double _3DSize = 50;
        Image _tempimage = null;
        public RollAnimation()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(Window1_Loaded);
        }
        void Window1_Loaded(object sender, RoutedEventArgs e)
        {
            _tempimage = new Image();
            _tempimage.Width = _imageWidth;
            _tempimage.Height = _imageHeight;
            _tempimage.Source = loadBitmap(null);//cargar imagen...
            ScaleTransform scaleTransform = new ScaleTransform(1, -1, 0, 0);
            _tempimage.LayoutTransform = scaleTransform;

            image.Source = loadBitmap(null);//cargar imagen...
            image.Clip = new RectangleGeometry(new Rect(0, 0, _imageWidth, _rollTopLeft));


            LinearGradientBrush maskBrush = new LinearGradientBrush();
            maskBrush.StartPoint = new Point(0, 0);
            maskBrush.EndPoint = new Point(0, 1);

            GradientStop BlackStop1 = new GradientStop(Color.FromArgb(30, 255, 255, 255), 0.0);
            GradientStop BlackStop3 = new GradientStop(Colors.Black, 0.0);
            GradientStop BlackStop4 = new GradientStop(Colors.Black, .9);
            GradientStop transparentStop = new GradientStop(Color.FromArgb(175, 255, 255, 255), .5);
            GradientStop BlackStop2 = new GradientStop(Color.FromArgb(100, 255, 255, 255), 1);

            maskBrush.GradientStops.Add(BlackStop1);
            maskBrush.GradientStops.Add(transparentStop);
            maskBrush.GradientStops.Add(BlackStop2);
            maskBrush.GradientStops.Add(BlackStop3);
            maskBrush.GradientStops.Add(BlackStop4);

            CanAni.OpacityMask = maskBrush;

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(.1);
            _timer.Tick += new EventHandler(_timer_Tick);
            _timer.IsEnabled = true;
        }

        void _timer_Tick(object sender, EventArgs e)
        {
            _rollTopLeft = _rollTopLeft + _offset;
            image.Clip = new RectangleGeometry(new Rect(0, 0, _imageWidth, _rollTopLeft));
            double yPosition = _rollTopLeft + _3DSize;
            double height = _3DSize;
            if (Math.Abs((_imageHeight - _rollTopLeft)) < _3DSize)
            {
                height = Math.Abs(image.Height - _rollTopLeft);
                CanAni.Height = height;
                CanWrap.Height = height;
                yPosition = _rollTopLeft;
                Linetwo.Y1 = height;
                Linetwo.Y2 = height;
            }
            Lineone.Visibility = Visibility.Visible;
            Linetwo.Visibility = Visibility.Visible;
            if (_imageHeight == _rollTopLeft)
            {
                _timer.IsEnabled = false;
                Lineone.Visibility = Visibility.Collapsed;
                Linetwo.Visibility = Visibility.Collapsed;
            }
            _tempimage.Clip = new RectangleGeometry(new Rect(0, yPosition, _imageWidth, height));
            VisualBrush vb = new VisualBrush(_tempimage as Visual);
            CanAni.Background = vb;
            Canvas.SetTop(CanWrap, _rollTopLeft);

        }
        public static BitmapSource loadBitmap(System.Drawing.Bitmap source)
        {
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(source.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty,
                System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
        }
    }
}
