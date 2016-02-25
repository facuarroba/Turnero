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
using TurneroCustomControlLibrary;



namespace TurneroViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : WindowBase 
    {
        double count = 0;
        private Turnos buffer=null;
        private string videoPath;
        private string showBar = "1";
        private string climaIconosPath;
        private string soundPath;

        public MainWindow()
        {
            InitializeComponent();

            AppName = "TurneroViewer";
            loadSettings();
            this.Cursor = Cursors.None;
            
            //if (showBar.Equals("1"))
            //    this.WindowStyle = System.Windows.WindowStyle.SingleBorderWindow;
            //else
            //    this.WindowStyle = System.Windows.WindowStyle.None;

            this.DataContext = this;

            DispatcherTimer horaTimer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate
            {
                this.labelHora.Content = DateTime.Now.ToString("dd/MM/yyyy HH:mm    ");
            }, this.Dispatcher);

            setTimer(new EventHandler(updateScreen));
            Timer.Start();

            setVideo();
            //setTV();
        }

        void loadSettings()
        {
            this.ServiceQuery.server = ConfigManager.ReadConnectionSetting(AppName, "Servidor");
            this.ServiceQuery.urlPath = ConfigManager.ReadConnectionSetting(AppName, "ServicePath");
            this.ServiceQuery.weatherServer = ConfigManager.ReadConnectionSetting(AppName, "ClimaServer");
            this.ServiceQuery.weatherPath = ConfigManager.ReadConnectionSetting(AppName, "ClimaPath");
            videoPath = ConfigManager.ReadConnectionSetting(AppName, "VideoPath");

            this.IdTerminal = ConfigManager.readStringSetting("idTerminal");
            showBar = ConfigManager.readStringSetting("showBar");

            climaIconosPath = "pack://application:,,,/TurneroViewer;component/imagenes/iconos/";
            soundPath = "sounds/line.wav";
        }

        void updateScreen(object sender, EventArgs e)
        {
            if (count % (2 * 60 * 30) == 0)
            {
                CurrentWeather clima = this.ServiceQuery.getClima();
                if (clima != null)
                {
                    tempLabel.Content = clima.toString();
                    BitmapImage logo = new BitmapImage(new Uri(climaIconosPath + clima.clima.icono + ".png", UriKind.RelativeOrAbsolute));
                    icono.Source = logo;
                }
            }
            count++;

            if (count % 2 == 0)
            {
                updateLlamados();
                updateAtendidos();
            }
        }

        private void setVideo()
        {
            this.mediaPlayer.Source = new Uri((videoPath), UriKind.RelativeOrAbsolute);
            this.mediaPlayer.Play();
        }

        private void mediaPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            this.mediaPlayer.Position = new TimeSpan(0, 0, 1);
            this.mediaPlayer.Play();
        }

        private void updateLlamados()
        {
            Turnos turnos = this.ServiceQuery.consultarTurnos(this.IdTerminal, "3");
            if (turnos != null)
            {
                List<int> cambios = new List<int>();
                if (turnos.resultado == "ok")
                {
                    onSuccess();
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
                        playSound(soundPath);
                    buffer = turnos;
                }
                else
                {
                    onError();
                }
            }
        }

        private void updateAtendidos()
        {
            Turnos turnos = this.ServiceQuery.consultarTurnos(this.IdTerminal, "4");

            atendidosGrid.Children.Clear();
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
                                res = Convert.ToInt32(t.idTurno);
                        }
                    }
                    else
                    {
                        res = Convert.ToInt32(t.idTurno);
                    }
                    if (res != 0)
                        ids.Add(res);
                }
            }
            return ids;
        }


        private void setTV()
        {
            try
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
            catch (Exception e)
            {
                MessageBox.Show("Error instanciando VLC. Error: " + e.ToString());
            }
        }
    }
}
