using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
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
using TurneroClassLibrary;

namespace TurneroCustomControlLibrary
{
    /// <summary>
    /// Realice los pasos 1a o 1b y luego 2 para usar este control personalizado en un archivo XAML.
    ///
    /// Paso 1a) Usar este control personalizado en un archivo XAML existente en el proyecto actual.
    /// Agregue este atributo XmlNamespace al elemento raíz del archivo de marcado en el que 
    /// se va a utilizar:
    ///
    ///     xmlns:MyNamespace="clr-namespace:TurneroCustomControlLibrary"
    ///
    ///
    /// Paso 1b) Usar este control personalizado en un archivo XAML existente en otro proyecto.
    /// Agregue este atributo XmlNamespace al elemento raíz del archivo de marcado en el que 
    /// se va a utilizar:
    ///
    ///     xmlns:MyNamespace="clr-namespace:TurneroCustomControlLibrary;assembly=TurneroCustomControlLibrary"
    ///
    /// Tendrá también que agregar una referencia de proyecto desde el proyecto en el que reside el archivo XAML
    /// hasta este proyecto y recompilar para evitar errores de compilación:
    ///
    ///     Haga clic con el botón secundario del mouse en el proyecto de destino en el Explorador de soluciones y seleccione
    ///     "Agregar referencia"->"Proyectos"->[seleccione este proyecto]
    ///
    ///
    /// Paso 2)
    /// Prosiga y utilice el control en el archivo XAML.
    ///
    ///     <MyNamespace:CustomControl1/>
    ///
    /// </summary>
    public class WindowBase : Window
    {
        private string ID_TERMINAL;
        private string appName = "default";
        DispatcherTimer timer = null;
        ServiceQuery serviceQuery;
        NLogLogger logger;

        

        int errorSpan = 5000;
        int successSpan = 500;
        int maxErrorsCount = 5;
        int errorsCount = 0;

        static WindowBase()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WindowBase), new FrameworkPropertyMetadata(typeof(WindowBase)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            #region Minimize, maximize y close
            //accedemos al boton cerrar definido en el estilo
            Button ButtonClose = GetTemplateChild("btClose") as Button;

            if (ButtonClose != null)
            {
                //nos suscribimos al evento click del botón cerrar
                ButtonClose.Click += ButtonClose_Click;

            }

            //accedemos al boton minimizar definido en el estilo
            Button ButtonMinimize = GetTemplateChild("btMinimize") as Button;

            if (ButtonMinimize != null)
            {
                //suscripcion al click del boton minimizar
                ButtonMinimize.Click += ButtonMinimize_Click;
            }

            //accedemos al boton maximizar definido en el estilo
            Button ButtonMaximize = GetTemplateChild("btMaximize") as Button;

            if (ButtonMaximize != null)
            {
                //suscripcion al click del boton maximizar
                ButtonMaximize.Click += ButtonMaximize_Click;
            }
            #endregion

            //accedemos al boton maximizar definido en el estilo
            Border TopBorder = GetTemplateChild("TopBorder") as Border;

            if (TopBorder != null)
            {
                //suscripcion al mousebuttondown del border de la ventana para permitir mover la ventana
                TopBorder.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(TopBorder_MouseLeftButtonDown);
            }
        }

        #region Manejadores de eventos de la ventana
        void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        void ButtonMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }

        void ButtonMaximize_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == System.Windows.WindowState.Maximized)
            {
                this.WindowState = System.Windows.WindowState.Normal;
            }
            else
            {
                this.WindowState = System.Windows.WindowState.Maximized;
            }
        }

        /// <summary>
        /// Manejador del mousedown para poder mover la ventana
        /// </summary>
        /// <param name="sender">
        /// <param name="e">
        void TopBorder_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        #endregion

        #region turnero
        public string IdTerminal
        {
            get { return ID_TERMINAL; }
            set { ID_TERMINAL = value; }
        }

        public ServiceQuery ServiceQuery
        {
            get 
            {
                if (serviceQuery == null)
                    serviceQuery = new ServiceQuery();
                return serviceQuery; 
            }
            set { serviceQuery = value; }
        }

        public DispatcherTimer Timer
        {
            get
            {
                if (timer == null)
                {
                    timer = new DispatcherTimer();
                }
                return timer;
            }
            set { timer = value; }
        }

        public string AppName
        {
            get { return appName; }
            set { appName = value; }
        }

        public NLogLogger Logger
        {
            get 
            {
                if(logger==null)
                    logger = new NLogLogger();
                return logger; 
            }
            set { logger = value; }
        }

        protected void setTimer(EventHandler evento)
        {
            Timer.Interval = TimeSpan.FromMilliseconds(successSpan);
            Timer.Tick += evento;
        }

        protected void loadSettings()
        {
            this.ServiceQuery.server = ConfigManager.ReadConnectionSetting(AppName, "Servidor");
            this.ServiceQuery.urlPath = ConfigManager.ReadConnectionSetting(AppName, "ServicePath");
            this.IdTerminal = ConfigManager.readStringSetting("idTerminal");
        }

        protected void onError()
        {
            errorsCount++;
            if (errorsCount >= maxErrorsCount)
            {
                Timer.Interval = TimeSpan.FromMilliseconds(errorSpan);
                Logger.Error("set errorSpan");
            }
        }

        protected void onSuccess()
        {
            if (errorsCount > 0)
            {
                errorsCount = 0;
                Timer.Interval = TimeSpan.FromMilliseconds(successSpan);
                Logger.Info("set successSpan");
            }
        }

        protected void playSound(string sound)
        {
            SoundPlayer player = new SoundPlayer(sound);
            player.Play();
        }
        #endregion
    }
}
