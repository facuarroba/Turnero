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
using System.Collections.ObjectModel;

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
        private static int spanQuery = 500;
        private static int spanOnErrorQuery = 2000;

        DispatcherTimer queueTimer;

        public ObservableCollection<Turno> listaTurnos { get; set; }
        public ObservableCollection<ItemTurno> listaLlamados { get; set; }

        ServiceQuery serviceQuery;

        public MainWindow()
        {
            InitializeComponent();

            serviceQuery = ServiceQuery.Instance;
            serviceQuery.server = ConfigManager.ReadConnectionSetting("TurneroMedico.Properties.Settings.Servidor");
            serviceQuery.urlPath = ConfigManager.ReadConnectionSetting("TurneroMedico.Properties.Settings.ServicePath");
            ID_TERMINAL = ConfigManager.readStringSetting("idTerminal");

            listaTurnos = new ObservableCollection<Turno>();
            listaLlamados = new ObservableCollection<ItemTurno>();

            queueTimer = new DispatcherTimer();
            NoError();
            queueTimer.Tick += new EventHandler(queueTimer_Tick);
            queueTimer.Start();
        }


        void queueTimer_Tick(object sender, EventArgs e)
        {

            object item = lbTurnos.SelectedItem;
            PopulateListTurnos();
            if (item != null)
                findSelected2(lbTurnos, ((Turno)item).idTurno);

            item = lbLlamados.SelectedItem;
            PopulateListLlamados();
            if (item != null)
                findSelected(lbLlamados, ((ItemTurno)item).Turno.idTurno);

            
        }

        private void findSelected(ListBox lb, String id)
        {
            foreach (ItemTurno item in lb.Items)
            {
                if (item.Turno.idTurno.Equals(id))
                    lb.SelectedItem = item;
            }
        }

        private void findSelected2(ListBox lb, String id)
        {
            foreach (Turno item in lb.Items)
            {
                if (item.idTurno.Equals(id))
                    lb.SelectedItem = item;
            }
        }

        public void PopulateListTurnos()
        {
            Turnos turnos = serviceQuery.consultarTurnos(ID_TERMINAL);
            listaTurnos.Clear();

            if (turnos != null)
            {
                if (turnos.turnos != null)
                {
                    foreach (Turno t in serviceQuery.ordenarTurnos(turnos.turnos))
                    {
                        listaTurnos.Add(t);
                    }
                }
            }
            this.DataContext = this;
        }

        public void PopulateListLlamados()
        {
            Turnos turnos = serviceQuery.consultarTurnosAtendidos(ID_TERMINAL);
            listaLlamados.Clear();

            if (turnos != null)
            {
                if (turnos.turnos != null)
                {
                    foreach (Turno t in serviceQuery.ordenarTurnos(turnos.turnos))
                    {
                         ItemTurno item = new ItemTurno();
                        item.Turno = t;
                        lbLlamados.Items.Add(item);
                    }
                }
            }

            
            this.DataContext = this;
        }
        private void btnAtender_Click(object sender, RoutedEventArgs e)
        {
            if (lbLlamados.Items.Count == 1)
                lbLlamados.SelectedItem = lbLlamados.Items[0];
            if (lbLlamados.SelectedItem != null)
            {
                AtiendeTurno at = serviceQuery.atenderTurno(((ItemTurno)lbLlamados.SelectedItem).Turno.idTurno);
                if (at.resultado == "error")
                    MessageBox.Show(at.msg);

                lbLlamados.SelectedItem = null;
            }
            else
            {
                MessageBox.Show("Debe seleccionar un turno");
            }
            Keyboard.Focus(lbLlamados);
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            if (lbLlamados.Items.Count == 1)
                lbLlamados.SelectedItem = lbLlamados.Items[0];
            if (lbLlamados.SelectedItem != null)
            {
                Turno turno = ((ItemTurno)lbLlamados.SelectedItem).Turno;
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
            Keyboard.Focus(lbLlamados);
        }

        private void btnFinalizar_Click(object sender, RoutedEventArgs e)
        {
            if (lbLlamados.Items.Count == 1)
                lbLlamados.SelectedItem = lbLlamados.Items[0];
            if (lbLlamados.SelectedItem != null)
            {
                Turno turno = ((ItemTurno)lbLlamados.SelectedItem).Turno;
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
            Keyboard.Focus(lbLlamados);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (lbTurnos.Items.Count == 1)
                lbTurnos.SelectedItem = lbLlamados.Items[0];
            if (lbTurnos.SelectedItem != null)
            {
                Turno t = (Turno)lbTurnos.SelectedItem;

                LlamaTurno llamado = serviceQuery.llamarTurno(t.idTurno, ID_TERMINAL);
                if (llamado.resultado == "error")
                {
                    MessageBox.Show("Se ha producido un error. Intente nuevamente");
                }

                lbTurnos.SelectedItem = null;
            }
            else
            {
                MessageBox.Show("Debe seleccionar un turno");
            }
            Keyboard.Focus(lbTurnos);
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
