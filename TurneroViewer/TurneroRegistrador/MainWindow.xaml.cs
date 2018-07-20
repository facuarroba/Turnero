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
using TurneroClassLibrary;
using TurneroClassLibrary.entities;
using System.Configuration;
using TurneroCustomControlLibrary;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace TurneroRegistrador
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : WindowBase
    {
        private int errorCount = 0;
        private static int maxErrorCount = 5;
        private static int spanQuery = 1000;
        private static int spanOnErrorQuery = 60000;

        private int SelectedPriority = 1;
        private int defaultPriority = 1;
        private static string emptyVal = "- --";

        private long count = 0;
        DispatcherTimer queueTimer;

        public ObservableCollection<Turno> listaTurnos { get; set; }
        
        public MainWindow()
        {
            InitializeComponent();

            listaTurnos = new ObservableCollection<Turno>();

            ServiceQuery = ServiceQuery.Instance;
            ServiceQuery.server = ConfigManager.ReadConnectionSetting("TurneroRegistrador","Servidor");
            ServiceQuery.urlPath = ConfigManager.ReadConnectionSetting("TurneroRegistrador", "ServicePath");
            this.IdTerminal = ConfigManager.readStringSetting("idTerminal");
            defaultPriority = Convert.ToInt32(ConfigManager.readStringSetting("defaultPriority"));
            spanOnErrorQuery = Convert.ToInt32(ConfigManager.readStringSetting("updateOnErrorSpan"));
            spanQuery= Convert.ToInt32(ConfigManager.readStringSetting("updateSpan"));

            queueTimer = new DispatcherTimer();
            NoError();
            queueTimer.Tick += new EventHandler(queueTimer_Tick);
            queueTimer.Start();

            if (defaultPriority.Equals("1"))
                cmbPrioridad.SelectedIndex = 0;
            else
                cmbPrioridad.SelectedIndex = 1;
        }

        void queueTimer_Tick(object sender, EventArgs e)
        {
            if (!labelNumber.Content.Equals(emptyVal))
                count++;
            if (count % 4 == 0)
            {
                labelNumber.Content = emptyVal;
                count = 0;
            }

            object item = lbTurnos.SelectedItem;
            PopulateListTurnos();
            if (item != null)
                findSelected(lbTurnos, ((Turno)item).idTurno);

            if (listColas.SelectedItem == null)
                updateList();
        }

        private void findSelected(ListBox lb, String id)
        {
            foreach (Turno item in lb.Items)
            {
                if (item.idTurno.Equals(id))
                    lb.SelectedItem = item;
            }
        }
        public void PopulateListTurnos()
        {
            Turnos turnos = this.ServiceQuery.consultarTurnos(this.IdTerminal);
            listaTurnos.Clear();

            if (turnos != null)
            {
                if (turnos.turnos != null)
                {
                    //foreach (Turno t in this.ServiceQuery.ordenarTurnos(turnos.turnos))
                    foreach (Turno t in turnos.turnos)
                    {
                        listaTurnos.Add(t);
                    }
                }
            }
            this.DataContext = this;
        }

        private void updateList()
        {
            listColas.Items.Clear();
            Colas colas = ServiceQuery.getColas(IdTerminal,"L");

            if (colas.resultado == "ok")
            {
                this.Title = "Turnero Registrador - " + TerminalName;
                foreach (Cola c in colas.colas)
                    listColas.Items.Add(c);
            }
            else
            {
                Error();
                this.Title = colas.msg;
            }
        }

        private void btnFinalizar_Click(object sender, RoutedEventArgs e)
        {
            if (lbTurnos.Items.Count == 1)
                lbTurnos.SelectedItem = lbTurnos.Items[0];
            Turno turno = null;
            if (lbTurnos.SelectedItem != null)
                turno = (Turno)lbTurnos.SelectedItem;
            if (turno != null)
            {
                if (MessageBox.Show("Va a finalizar el turno de " + turno.nombre + ". ¿Está ud. seguro?", "Mensaje", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    FinalizaTurno t = this.ServiceQuery.finalizaTurno(turno.idTurno);
                    if (t.resultado == "error")
                        MessageBox.Show(t.msg);
                }
            }
            else
            {
                MessageBox.Show("Debe seleccionar un turno");
            }
            Keyboard.Focus(lbTurnos);
        }

        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {
            Turno t = null;
            if (lbTurnos.SelectedItem != null)
            {
                t = (Turno)lbTurnos.SelectedItem;
                EditTurnoWindow et = new EditTurnoWindow(t);
                et.ShowDialog();
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Registro registro;
            String Nombre = "";
            String Nota = "";
            String HC = "";
            HC = txtHC.Text.Trim();
            Nombre = txtName.Text.Trim() +"*"+ txtNota.Text.Trim();
            Nota = txtNota.Text.Trim();

            Cola selCola = (Cola)listColas.SelectedItem;
            if (selCola != null)
            {
                if (Nombre.Equals(string.Empty))
                {
                    MessageBox.Show("Debe ingresar un nombre para el turno");
                    Keyboard.Focus(txtName);
                    return;
                }

                //if (HC.Equals(""))
                //{
                //    if (MessageBox.Show("No ingresó un número de historia clínica. ¿Desea continuar?", "Mensaje", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                //    {
                //        Keyboard.Focus(txtHC);
                //        return;
                //    }
                //}

//                if (MessageBox.Show("Va a otorgar un turno a " + txtName.Text.Trim() + ", con historia clinica n° " + HC + ". ¿Está ud. seguro?", "Mensaje", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
//                {
                    registro = ServiceQuery.registrarTurno(selCola, HC, Nombre,SelectedPriority);
                    if (registro.resultado == "ok")
                    {
                        int i = Convert.ToInt16(registro.msg);
                        labelNumber.Content = selCola.tipoAtencion + " " + i.ToString("00");
                        txtHC.Text = "";
                        txtName.Text = "";
                        txtNota.Text = "";
                    }
                    else
                    {
                        MessageBox.Show("Error al intentar registrar el turno. Msg: " + registro.msg); 
                    }
                    Keyboard.Focus(txtName);
                    if (defaultPriority > 1)
                        cmbPrioridad.SelectedIndex = 1;
                    else
                        cmbPrioridad.SelectedIndex = 0;
//                }
            }            
            
            else
            {
                MessageBox.Show("Debe seleccionar un tipo de atención");
                Keyboard.Focus(listColas);
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

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedPriority = ((ComboBox)sender).SelectedIndex + 1;
                        
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^A-Za-z0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

    }
}
