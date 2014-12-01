using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using RestSharp;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TurneroClassLibrary.entities;
using TurneroClassLibrary;
using System.IO;
using System.Xml.Serialization;
using System.Reflection;
using System.Windows.Threading;
using System.ComponentModel;
using TurneroViewer.componentes;
using System.Media;
using System.Configuration;
using Vlc.DotNet.Wpf;
using Vlc.DotNet.Core.Medias;


namespace TurneroViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window 
    {
        double count = 0;
        private string ID_TERMINAL = "901";
        private string TXT_MSG = "";
 
        private static int numbersRefresh = 500;
        
        DispatcherTimer queueTimer;

        private ServiceQuery serviceQuery;

        private Turnos buffer=null;
        ILogger _logger;
        public MainWindow()
        {
            InitializeComponent();
            this.Cursor = Cursors.None;

            string showBar = ConfigManager.readStringSetting("showBar");
            if (showBar.Equals("1"))
                this.WindowStyle = System.Windows.WindowStyle.SingleBorderWindow;
            else
                this.WindowStyle = System.Windows.WindowStyle.None;

            this.DataContext = this;

            serviceQuery = ServiceQuery.Instance;
            serviceQuery.server = ConfigManager.ReadConnectionSetting("TurneroViewer.Properties.Settings.Servidor");
            serviceQuery.urlPath = ConfigManager.ReadConnectionSetting("TurneroViewer.Properties.Settings.ServicePath");
            serviceQuery.weatherServer = ConfigManager.ReadConnectionSetting("TurneroViewer.Properties.Settings.ClimaServer");
            serviceQuery.weatherPath = ConfigManager.ReadConnectionSetting("TurneroViewer.Properties.Settings.ClimaPath");

            ID_TERMINAL = ConfigManager.readStringSetting("idTerminal");
            TXT_MSG = ConfigManager.readStringSetting("msg");

            DispatcherTimer timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate
            {
                this.labelHora.Content = DateTime.Now.ToString("dd/MM/yyyy HH:mm    ");
            }, this.Dispatcher);

            NLogLogger.ConfigureLogger("C:\\","Display");
            _logger = new NLogLogger();
            _logger.Info("Application starting.");

            queueTimer = new DispatcherTimer();
            queueTimer.Interval = TimeSpan.FromMilliseconds(numbersRefresh);
            queueTimer.Tick += new EventHandler(queueTimer_Tick);
            queueTimer.Start();
        }

        void queueTimer_Tick(object sender, EventArgs e)
        {
            UpdateScreen();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            setVideo();
            //setTV();
            UpdateScreen();
        }

        private void setVideo()
        {
            this.mediaPlayer.Source = new Uri(ConfigManager.ReadConnectionSetting("TurneroViewer.Properties.Settings.VideoPath"), UriKind.RelativeOrAbsolute);
            this.mediaPlayer.Play();
        }


        private void mediaPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            this.mediaPlayer.Position = new TimeSpan(0, 0, 1);
            this.mediaPlayer.Play();
        }

        
        private void UpdateScreen()
        {
            if (count % 3600 == 0)
            {
                CurrentWeather clima = serviceQuery.getClima();
                if (clima != null)
                {
                    tempLabel.Content = clima.toString();
                    BitmapImage logo = new BitmapImage(new Uri("pack://application:,,,/TurneroViewer;component/imagenes/iconos/" + clima.clima.icono + ".png", UriKind.RelativeOrAbsolute));
                    icono.Source = logo;
                }
            }
            count++;

            if (count % 2 == 0)
            {
                Turnos turnos = serviceQuery.consultarTurnos(ID_TERMINAL,"3");
                if (turnos != null)
                {
                    UpdateNumeros(turnos);
                }
            }
        }

        private void UpdateNumeros(Turnos turnos)
        {
            List<int> cambios = new List<int>();
            if (turnos.resultado == "ok")
            {
                this.numbersGrid.Children.Clear();
                if (turnos.cantidad != "0")
                    cambios = compareList(turnos);
                for (int i = 0; i < turnos.count; i++)
                {
                    Turno t = turnos.turnos[i];
                    var d = new DisplayTipo4(t);
                    if (cambios.Contains(Convert.ToUInt16(t.idTurno)))
                        d.Background = Brushes.GreenYellow;
                    Grid.SetRow(d, i);
                    this.numbersGrid.Children.Add(d);
                }
                if (cambios.Count > 0)
                    PlaySound();
                buffer = turnos;
                queueTimer.Interval = TimeSpan.FromMilliseconds(numbersRefresh);

                updateAtendidos();
            }
            else
            {
                queueTimer.Interval = queueTimer.Interval + queueTimer.Interval;
            }
        }

        private void updateAtendidos()
        {
            Turnos turnos = serviceQuery.consultarTurnos(ID_TERMINAL, "4");

            atendidosGrid.Children.Clear();
            
            List<int> cambios = new List<int>();
            if (turnos.resultado == "ok")
            {
                for (int i = 0; i < turnos.count; i++)
                {
                    Turno t = turnos.turnos[i];
                    var d = new DisplaySlim();
                    d.Turno = t;
                    Grid.SetRow(d, i);
                    atendidosGrid.Children.Add(d);
                }
            }
            
        }

        private List<int> compareList(Turnos turnos)
        {
            int res = 0;
            List<int> ids = new List<int>();

            if ((buffer != null) && (turnos != null))
            {
                foreach (Turno t in turnos.turnos)
                {
                    if (buffer.cantidad != "0")
                    {
                        foreach (Turno b in buffer.turnos)
                        {
                            if (t.idTurno == b.idTurno)
                            {
                                res = 0;
                                break;
                            }
                            else
                                res = Convert.ToInt16(t.idTurno);
                        }
                    }
                    else
                    {
                        res = Convert.ToInt16(t.idTurno);
                    }
                    if (res != 0)
                        ids.Add(res);
                }
            }
            return ids;
        }

        private void PlaySound()
        {
            SoundPlayer player = new SoundPlayer("sounds/line.wav");
            player.Play();
        }

        private void setTV()
        {

            VlcControl vlcPlayer = new VlcControl();

            multimediaBorder.Child = vlcPlayer;

            // WPF dark magic ahead: run-time interpreted data binding
            // When the VLC video changes (is loaded for example), the grid displays the new video image
            Binding vlcBinding = new Binding("VideoSource");
            vlcBinding.Source = vlcPlayer;

            // VLC paints into a WPF image
            Image vImage = new Image();
            vImage.SetBinding(Image.SourceProperty, vlcBinding);

            // The WPF image is used by a WPF brush
            VisualBrush vBrush = new VisualBrush();
            vBrush.TileMode = TileMode.None;
            vBrush.Stretch = Stretch.Uniform;
            vBrush.Visual = vImage;

            // The WPF brush is used by the grid element background
            multimediaBorder.Background = vBrush;

            //TELEFE HD: rtmp://sl100tb.cxnlive.com/live/telefe.stream 
            //EL TRECE HD: rtsp://stream.eltrecetv.com.ar/live13/13tv/13tv1 
            //AMERICA TV HD: rtmp://sl100tb.cxnlive.com/live/america.stream 
            //CANAL 9 HD: rtmp://sl100tb.cxnlive.com/live/canal9.stream 
            //TV PUBLICA HD: rtmp://sl100tb.cxnlive.com/live/tvpublica.stream 
            //TyC Sports HD: rtmp://sl100tb.cxnlive.com/live/tyc.stream 
            //TN HD: rtsp://stream.tn.com.ar/live/tnhd1 
            //Canal 26 HD: rtsp://live-edge01.telecentro.net.ar:80/live/26hd-360 
            //Deportv hd: rtmp://sl100tb.cxnlive.com/live/deportv.stream 
            //Magazine TV: rtsp://stream.mgzn.tv/live/mgzntv/mgzntv 
            //FOX SPORT: rtmp://wdc.cxnlive.com/live/foxsd.stream 
            //ESPN: rtmp://wdc.cxnlive.com/live/espn.stream 
            //CRONICA TV: rtmp://wdc.cxnlive.com/live/cronica.stream 
            //CANAL 22 Mexico: rtmp://origen.cloudapp.net/live/envivopc
            //LATINO:VTV URUGUAY rtmp://sl100tb.cxnlive.com/live/vtv.stream 
            //LATINO:VTV+ URUGUAY rtmp://sl100tb.cxnlive.com/live/vtvmas.stream 
            //LATINO:MONTECARLO URUGUAY HD rtmp://sl100tb.cxnlive.com/live/montecarlo.stream 
            //LATINO:CANAL 10 URUGUAY HD rtmp://sl100tb.cxnlive.com/live/canal10.stream 
            //LATINO:TELEDOCE URUGUAY HD rtmp://sl100tb.cxnlive.com/live/teledoce.stream 
            //VTV uruguay (futbol local - Seleccion): rtmp://sl100tb.cxnlive.com/live//vtv.stream 
            //Deportv HD: rtmp://sl100tb.cxnlive.com/live/deportv.stream 
            //America TV: rtmp://wdc.cxnlive.com/live/americasd.stream 
            //Q musica: "mms://streamqm.uigc.net/qmusica"
            var media1 = new LocationMedia("rtsp://stream.tn.com.ar/live/tnhd1");

            vlcPlayer.Media = media1;
        }
    }
}
