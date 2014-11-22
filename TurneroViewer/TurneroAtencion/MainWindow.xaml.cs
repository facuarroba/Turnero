using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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
using TurneroClassLibrary.entities;
using System.Configuration;

namespace TurneroAtencion
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static String ID_TERMINAL = "1";

        private int errorCount = 0;
        private static int maxErrorCount = 5;
        private static int spanQuery = 500;
        private static int spanOnErrorQuery = 60000;
        DispatcherTimer dayTimer;

        private ServiceQuery serviceQuery;

        public ObservableCollection<checkItemCaja> TheList { get; set; }

        public MainWindow()
        {

            InitializeComponent();
            serviceQuery = ServiceQuery.Instance;
            serviceQuery.server = ReadConnectionSetting("TurneroAtencion.Properties.Settings.Servidor");
            serviceQuery.urlPath = ReadConnectionSetting("TurneroAtencion.Properties.Settings.ServicePath");

            PopulateListCajas();
            PopulateListTurnos();

            dayTimer = new DispatcherTimer();
            NoError();
            dayTimer.Tick += new EventHandler(dayTimer_Tick);
            dayTimer.Start();


        }

        void dayTimer_Tick(object sender, EventArgs e)
        {
            if (lbTurnos.SelectedItem == null)
                PopulateListTurnos();
            if (lbLlamados.SelectedItem == null)
                PopulateListLlamados();
        }

        

        public void PopulateListTurnos()
        {
            Turnos turnos = serviceQuery.consultarTurnos(ID_TERMINAL);
            lbTurnos.Items.Clear();

            if(turnos != null)
            {
                if (turnos.turnos != null)
                {
                    foreach (Turno t in ordenarTurnos(turnos.turnos))
                    {
                        if (checkTurnoCaja(t))
                            lbTurnos.Items.Add(t);
                    }
                }
            }
        }

        public void PopulateListLlamados()
        {
            Turnos turnos = serviceQuery.consultarTurnos(ID_TERMINAL,"3");
            lbLlamados.Items.Clear();

            if (turnos != null)
            {
                if (turnos.turnos != null)
                {
                    foreach (Turno t in turnos.turnos)
                    {
                        if (checkTurnoCaja(t))
                            lbLlamados.Items.Add(t);
                    }
                }
            }
        }

        private bool checkTurnoCaja(Turno t)
        {
            bool res = false;
            foreach (checkItemCaja cic in TheList)
            {
                if (cic.isChecked == true)
                {
                    if (cic.caja.tipoAtencion == t.tipoAtencion)
                        res = true;
                }
            }
            return res;
        }

        public void PopulateListCajas()
        {
            TheList = new ObservableCollection<checkItemCaja>();
            Cajas cajas = serviceQuery.getCajasHabilitadas();
            if (cajas.cajas != null)
            {
                foreach (Caja c in cajas.cajas)
                {
                    TheList.Add(new checkItemCaja {caja = c, isChecked = true});
                }
            }
            this.DataContext = this;
        }

        private void CheckBoxZone_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox chkZone = (CheckBox)sender;
            PopulateListTurnos();
        }

        private void CheckBoxZone_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(lbTurnos.SelectedItem != null)
            {
                Turno t = (Turno) lbTurnos.SelectedItem;
                
                LlamaTurno llamado = serviceQuery.llamarTurno(t.idTurno,ID_TERMINAL);
                if(llamado.resultado == "error")
                {
                    lbLlamados.Items.Add(t);
                }
                else
                    MessageBox.Show("Llamando a : " + t.ToString());
                lbTurnos.SelectedItem = null;

            }
            
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (lbLlamados.SelectedItem != null)
            {
                Turno t = (Turno)lbLlamados.SelectedItem;
                AtiendeTurno at = serviceQuery.atenderTurno(t.idTurno);
                if (at.resultado == "error")
                    MessageBox.Show(at.msg);

                lbLlamados.SelectedItem = null;
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            FinalizaTurno t= serviceQuery.finalizaTurno(ID_TERMINAL);
            if (t.resultado == "error")
                MessageBox.Show(t.msg);
        }

        public class checkItemCaja
        {
            public Caja caja { get; set; }
            public bool isChecked { get; set; }
        }

        private void Error()
        {
            errorCount++;
            if (errorCount > maxErrorCount)
            {
                dayTimer.Interval = TimeSpan.FromMilliseconds(spanOnErrorQuery);
            }
        }

        private void NoError()
        {
            errorCount = 0;
            dayTimer.Interval = TimeSpan.FromMilliseconds(spanQuery);
        }

        private void ikritLogo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //
        }

        private String ReadConnectionSetting(string key)
        {
            try
            {
                string result = ConfigurationManager.ConnectionStrings[key].ToString();
                return result;
            }
            catch (Exception e)
            {
                return "";
            }
        }

        private List<Turno> ordenarTurnos(Turno[] lista)
        {
            List<Turno> res = lista.OrderBy(o => o.idTurno).ToList();
            return res;
        }
    }


}