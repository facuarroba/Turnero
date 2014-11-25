using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
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
using TurneroAtencion2.componentes;
using TurneroClassLibrary;
using TurneroClassLibrary.entities;

namespace TurneroAtencion2
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
        DispatcherTimer timer;

        private Turno turnoDerivar = null;
        private ServiceQuery serviceQuery;

        public ObservableCollection<checkItemCaja> listaCajas { get; set; }
        public ObservableCollection<Cola> listaCajasDerivar { get; set; }

        public ObservableCollection<Turno> listaTurnos { get; set; }
        public ObservableCollection<ItemTurno> listaLlamados { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            
            serviceQuery = ServiceQuery.Instance;
            serviceQuery.server = ConfigManager.ReadConnectionSetting("TurneroAtencion.Properties.Settings.Servidor");
            serviceQuery.urlPath = ConfigManager.ReadConnectionSetting("TurneroAtencion.Properties.Settings.ServicePath");
            ID_TERMINAL = ConfigManager.readStringSetting("idTerminal");

            listaTurnos = new ObservableCollection<Turno>();
            listaLlamados = new ObservableCollection<ItemTurno>();

            PopulateListCajas();
            PopulateListCajasDerivacion();
            PopulateListTurnos();
            PopulateListLlamados();

            timer = new DispatcherTimer();
            NoError();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            object item = lbTurnos.SelectedItem;
                PopulateListTurnos();
            if(item !=null)
                findSelected2(lbTurnos, ((Turno)item).idTurno);

            item = lbLlamados.SelectedItem;
            PopulateListLlamados();
            if (item != null)
                findSelected(lbLlamados,((ItemTurno) item).Turno.idTurno);

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
                        if (checkTurnoCaja(t))
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
                        if (checkTurnoCaja(t))
                        {
                            ItemTurno item = new ItemTurno();
                            item.Turno = t;
                            listaLlamados.Add(item);
                        }
                    }
                }
            }

            //turnos = serviceQuery.consultarTurnos(ID_TERMINAL, "4");

            //if (turnos != null)
            //{
            //    if (turnos.turnos != null)
            //    {
            //        foreach (Turno t in turnos.turnos)
            //        {
            //            if (checkTurnoCaja(t))
            //            {
            //                ItemTurno item = new ItemTurno();
            //                item.Turno = t;
            //                listaLlamados.Add(item);
            //            }
            //        }
            //    }
            //}

            this.DataContext = this;
        }

        private bool checkTurnoCaja(Turno t)
        {
            bool res = false;
            foreach (checkItemCaja cic in listaCajas)
            {
                if (cic.isChecked == true)
                {
                    if (cic.cola.tipoAtencion == t.tipoAtencion)
                        res = true;
                }
            }
            return res;
        }

        public void PopulateListCajas()
        {
            listaCajas = new ObservableCollection<checkItemCaja>();
            Colas colas = serviceQuery.getColas();
            if (colas.colas != null)
            {
                foreach (Cola c in colas.colas)
                {
                    listaCajas.Add(new checkItemCaja { cola = c, isChecked = true });
                }
            }
            this.DataContext = this;
        }

        public void PopulateListCajasDerivacion()
        {
            listaCajasDerivar = new ObservableCollection<Cola>();
            Colas colas = serviceQuery.getColas("0");
            if (colas.colas != null)
            {
                foreach (Cola c in colas.colas)
                {
                    listaCajasDerivar.Add(c);
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

        private void Error()
        {
            errorCount++;
            if (errorCount > maxErrorCount)
            {
                timer.Interval = TimeSpan.FromMilliseconds(spanOnErrorQuery);
            }
        }

        private void NoError()
        {
            errorCount = 0;
            timer.Interval = TimeSpan.FromMilliseconds(spanQuery);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (lbTurnos.Items.Count == 1)
                lbTurnos.SelectedItem = lbTurnos.Items[0];
            LlamarTurno();
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

        private void btnFinalizar_Click(object sender, RoutedEventArgs e)
        {
            if (lbLlamados.Items.Count == 1)
                lbLlamados.SelectedItem = lbLlamados.Items[0];
            Turno turno = null;
            if (lbLlamados.SelectedItem != null)
                turno = ((ItemTurno)lbLlamados.SelectedItem).Turno;
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
            Keyboard.Focus(lbLlamados);
        }

        private void btnDerivar_Click(object sender, RoutedEventArgs e)
        {
            if (lbLlamados.Items.Count == 1)
                lbLlamados.SelectedItem = lbLlamados.Items[0];
            if (lbLlamados.SelectedItem != null)
            {
                turnoDerivar = ((ItemTurno)lbLlamados.SelectedItem).Turno;
                if (turnoDerivar.estado.Equals("4"))
                {
                    updateTurnoDerivar();
                    lbLlamados.SelectedItem = null;
                }
                else
                {
                    MessageBox.Show("El turno debe estar siendo atendido");
                    turnoDerivar = null;
                }
            }
            else
            {
                MessageBox.Show("Debe seleccionar un turno");
            }
            Keyboard.Focus(lbLlamados);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Registro registro;

            if (turnoDerivar == null)
            {
                MessageBox.Show("Debe seleccionarse un turno");
                return;
            }
            else if (turnoDerivar.nombre.Equals(""))
            {
                MessageBox.Show("Debe seleccionarse un turno");
                return;
            }
            
            Cola selCola = null;
            if(turnoDerivar != null)
            {
                selCola = (Cola)listColas.SelectedItem;
            }

            if (selCola == null)
            {
                MessageBox.Show("Debe seleccionarse una cola para derivar");
                return;
            }

            if (MessageBox.Show("Va a agregar el turno de "+ turnoDerivar.nombre + " a la espera de "+ selCola.ToString() +". ¿Está ud. seguro?", "Mensaje", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                FinalizaTurno ft = serviceQuery.finalizaTurno(turnoDerivar.idTurno);
                if (ft.resultado.Equals("ok"))
                {
                    registro = serviceQuery.registrarTurno(selCola, turnoDerivar.hc, turnoDerivar.nombre);
                    if (registro.resultado == "error")
                    {

                    }
                    listColas.SelectedItem = null;
                    turnoDerivar = null;
                    updateTurnoDerivar();
                }
            }
            Keyboard.Focus(lbLlamados);
        }

        private void updateTurnoDerivar()
        {
            lblHC.Content="";
            lblName.Content="";
            if (turnoDerivar != null)
            {
                lblHC.Content = "H.C.: " + turnoDerivar.hc;
                lblName.Content = turnoDerivar.nombre;
            }
            else
            {
                lblHC.Content = "H.C.: ";
                lblName.Content = "Nombre: ";
            }
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            if (lbLlamados.Items.Count == 1)
                lbLlamados.SelectedItem = lbLlamados.Items[0];
            Turno turno = null;
            if (lbLlamados.SelectedItem != null)
                turno = ((ItemTurno)lbLlamados.SelectedItem).Turno;
            if (turno != null)
            {
                if (MessageBox.Show("Va a cancelar el llamado de " + turno.nombre + ". ¿Está ud. seguro?", "Mensaje", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
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


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (turnoDerivar != null)
            {
                txtHC.Text = turnoDerivar.hc;
                txtName.Text = turnoDerivar.nombre;
                SwapEditVisibility(true);
            }
            else
            {
                MessageBox.Show("Debe haber un turno para editar");
            }
        }

        private void SwapEditVisibility(bool edit)
        {
            if (edit)
            {
                txtName.Visibility = Visibility.Visible;
                txtHC.Visibility = Visibility.Visible;
                lblName.Visibility = Visibility.Hidden;
                lblHC.Visibility = Visibility.Hidden;
            }
            else
            {
                lblName.Visibility = Visibility.Visible;
                lblHC.Visibility = Visibility.Visible;
                txtName.Visibility = Visibility.Hidden;
                txtHC.Visibility = Visibility.Hidden;
            }
        }

        private void btnLlamarSiguiente_Click(object sender, RoutedEventArgs e)
        {
            if (lbTurnos.Items.Count > 1)
                lbTurnos.SelectedItem = lbTurnos.Items[0];
            LlamarTurno();
        }

        private void LlamarTurno()
        {
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
    }
    
    public class checkItemCaja
    {
        public Cola cola { get; set; }
        public bool isChecked { get; set; }
    }
}
