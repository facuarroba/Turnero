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

namespace TurneroRegistrador
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string ID_TERMINAL = "101";
        private int errorCount = 0;
        private static int maxErrorCount = 5;
        private static int spanQuery = 500;
        private static int spanOnErrorQuery = 60000;

        private static string emptyVal = "- --";

        private long count = 0;
        DispatcherTimer queueTimer;

        ServiceQuery serviceQuery;

        public MainWindow()
        {
            InitializeComponent();

            serviceQuery = ServiceQuery.Instance;
            serviceQuery.server = ConfigManager.ReadConnectionSetting("TurneroRegistrador.Properties.Settings.Servidor");
            serviceQuery.urlPath = ConfigManager.ReadConnectionSetting("TurneroRegistrador.Properties.Settings.ServicePath");
            ID_TERMINAL = ConfigManager.readStringSetting("idTerminal");

            queueTimer = new DispatcherTimer();
            NoError();
            queueTimer.Tick += new EventHandler(queueTimer_Tick);
            queueTimer.Start();
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

            if (listColas.SelectedItem == null)
                updateList();
        }

        private void updateList()
        {
            listColas.Items.Clear();
            Colas colas = serviceQuery.getColas();

            if (colas.resultado == "ok")
            {
                this.Title = "Turnero Registrador";
                foreach (Cola c in colas.colas)
                    listColas.Items.Add(c);
            }
            else
            {
                Error();
                this.Title = colas.msg;
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Registro registro;
            String Nombre = "";
            String HC = "";
            Nombre = txtName.Text.Trim();
            HC = txtHC.Text.Trim();

            Cola selCola = (Cola)listColas.SelectedItem;
            if (selCola != null)
            {
                if (Nombre.Equals(""))
                {
                    MessageBox.Show("Debe ingresar un nombre para el turno");
                    Keyboard.Focus(txtName);
                    return;
                }

                if (HC.Equals(""))
                {
                    if (MessageBox.Show("No ingresó un número de historia clínica. ¿Desea continuar?", "Mensaje", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                    {
                        Keyboard.Focus(txtHC);
                        return;
                    }
                }

                if (MessageBox.Show("Va a otorgar un turno a " + Nombre + ", con historia clinica n° " + HC + ". ¿Está ud. seguro?", "Mensaje", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    registro = serviceQuery.registrarTurno(selCola, HC, Nombre);
                    if (registro.resultado == "ok")
                    {
                        int i = Convert.ToInt16(registro.msg);
                        labelNumber.Content = selCola.tipoAtencion + " " + i.ToString("00");
                        txtHC.Text = "";
                        txtName.Text = "";
                    }
                    else
                    {
                        MessageBox.Show("Error al intentar registrar el turno. Msg: " + registro.msg); 
                    }
                    Keyboard.Focus(txtName);
                }
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

       
    }
}
