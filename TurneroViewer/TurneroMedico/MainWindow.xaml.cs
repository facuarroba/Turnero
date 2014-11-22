using System;
using System.Collections.Generic;
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
using TurneroAtencion2.componentes;

namespace TurneroMedico
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
     public partial class MainWindow : Window
    {
        private string ID_TERMINAL = "101";
        private int errorCount = 0;
        private static int maxErrorCount = 5;
        private static int spanQuery = 800;
        private static int spanOnErrorQuery = 10000;

        private static string emptyVal = "- --";

        private long count = 0;
        DispatcherTimer queueTimer;

        ServiceQuery serviceQuery;

        public MainWindow()
        {
            InitializeComponent();

            serviceQuery = ServiceQuery.Instance;
            serviceQuery.server = ConfigManager.ReadConnectionSetting("TurneroMedico.Properties.Settings.Servidor");
            serviceQuery.urlPath = ConfigManager.ReadConnectionSetting("TurneroMedico.Properties.Settings.ServicePath");
            ID_TERMINAL = ConfigManager.readStringSetting("idTerminal");

            queueTimer = new DispatcherTimer();
            queueTimer.Tick += new EventHandler(queueTimer_Tick);
            queueTimer.Start();
        }


        void queueTimer_Tick(object sender, EventArgs e)
        {
            if (lbTurnos.SelectedItem == null)
                PopulateListTurnos();

            object item = lbLlamados.SelectedItem;
            PopulateListLlamados();
            if (item != null)
                findSelected(((ItemTurno)item).Turno.idTurno);
        }

        private void findSelected(String id)
        {
            foreach (ItemTurno item in lbLlamados.Items)
            {
                if (item.Turno.idTurno.Equals(id))
                    lbLlamados.SelectedItem = item;
            }
        }

        public void PopulateListTurnos()
        {
            Turnos turnos = serviceQuery.consultarTurnos(ID_TERMINAL);
            lbTurnos.Items.Clear();

            if (turnos != null)
            {
                if (turnos.turnos != null)
                {
                    foreach (Turno t in serviceQuery.ordenarTurnos(turnos.turnos))
                    {
                            lbTurnos.Items.Add(t);
                    }
                }
            }
        }

        public void PopulateListLlamados()
        {
            Turnos turnos = serviceQuery.consultarTurnos(ID_TERMINAL, "3");
            lbLlamados.Items.Clear();

            if (turnos != null)
            {
                if (turnos.turnos != null)
                {
                    foreach (Turno t in turnos.turnos)
                    {
                        
                        ItemTurno item = new ItemTurno();
                        item.Turno = t;
                        lbLlamados.Items.Add(item);

                    }
                }
            }

            turnos = serviceQuery.consultarTurnos(ID_TERMINAL, "4");

            if (turnos != null)
            {
                if (turnos.turnos != null)
                {
                    foreach (Turno t in turnos.turnos)
                    {

                        ItemTurno item = new ItemTurno();
                        item.Turno = t;
                        lbLlamados.Items.Add(item);

                    }
                }
            }
        }
        private void btnAtender_Click(object sender, RoutedEventArgs e)
        {
            if (lbLlamados.SelectedItem != null)
            {
                AtiendeTurno at = serviceQuery.atenderTurno(((ItemTurno)lbLlamados.SelectedItem).Turno.idTurno);
                if (at.resultado == "error")
                    MessageBox.Show(at.msg);

                lbLlamados.SelectedItem = null;
            }
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            Turno turno = ((ItemTurno)lbLlamados.SelectedItem).Turno;
            if (turno != null)
            {
                if (MessageBox.Show("Va a cancelar el llamado de" + turno.nombre + ". ¿Está ud. seguro?", "Mensaje", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    CancelaLlamado t = serviceQuery.cancelarLlamado(turno.idTurno);
                    if (t.resultado == "error")
                        MessageBox.Show(t.msg);
                }
            }
            else
            {
                MessageBox.Show("Debe seleccionar un turno");
            }
        }

        private void btnFinalizar_Click(object sender, RoutedEventArgs e)
        {
            Turno turno = ((ItemTurno)lbLlamados.SelectedItem).Turno;
            if (turno != null)
            {
                if (MessageBox.Show("Va a finalizar el turno de" + turno.nombre + ". ¿Está ud. seguro?", "Mensaje", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    FinalizaTurno t = serviceQuery.finalizaTurno(turno.idTurno);
                    if (t.resultado == "error")
                        MessageBox.Show(t.msg);
                }
            }
            else
            {
                MessageBox.Show("Debe seleccionar un turno");
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (lbTurnos.SelectedItem != null)
            {
                Turno t = (Turno)lbTurnos.SelectedItem;

                LlamaTurno llamado = serviceQuery.llamarTurno(t.idTurno, ID_TERMINAL);
                if (llamado.resultado == "error")
                {
                    MessageBox.Show("Se ha producido un error. Intente nuevamente");
                    //lbLlamados.Items.Add(t);
                }
                //else
                //    MessageBox.Show("Llamando a : " + t.ToString());
                lbTurnos.SelectedItem = null;

            }
        }

        private void Error()
        {
            errorCount++;
            if (errorCount > maxErrorCount)
            {
                queueTimer.Interval = TimeSpan.FromMilliseconds(spanOnErrorQuery);
            }
        }

        private void NoError()
        {
            errorCount = 0;
            queueTimer.Interval = TimeSpan.FromMilliseconds(spanQuery);
        }
        
    }
}
