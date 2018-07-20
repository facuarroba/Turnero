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
using System.Windows.Shapes;
using TurneroClassLibrary;
using TurneroClassLibrary.entities;

namespace TurneroCustomControlLibrary
{
    /// <summary>
    /// Lógica de interacción para Window1.xaml
    /// </summary>
    public partial class EditTurnoWindow : Window
    {
        Turno res;
        public EditTurnoWindow()
        {
            InitializeComponent();
        }
        public EditTurnoWindow(Turno t)
        {
            InitializeComponent();
            res = t;
            int priority = 1;

            txtNota.Text = res.nota;
            txtName.Text = res.nombre;

            txtHC.Text = res.hc;
            priority = Convert.ToInt16(res.prioridad);
            if (priority > 1)
                cmbPrioridad.SelectedIndex = 1;
            else
                cmbPrioridad.SelectedIndex = 0;
            
        }

        public Turno editTurno
        {
            get { return res; }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            ServiceQuery SQ = ServiceQuery.Instance;
            Modificacion registro;
            String Nombre = "";
            String HC = "";
            int SelectedPriority = SelectedPriority = cmbPrioridad.SelectedIndex + 1; ;
            HC = txtHC.Text.Trim();
            Nombre = txtName.Text.Trim() + "*" + txtNota.Text.Trim();

            if (Nombre.Equals(""))
            {
                MessageBox.Show("Debe ingresar un nombre para el turno");
                Keyboard.Focus(txtName);
                return;
            }

            registro = SQ.modificarTurno(res.idTurno,HC, Nombre, SelectedPriority);
            if (registro.resultado == "ok")
            {
                txtHC.Text = "";
                txtName.Text = "";
                txtNota.Text = "";
                this.Close();
            }
            else
            {
                MessageBox.Show("Error al intentar registrar el turno. Msg: " + registro.msg);
            }
            Keyboard.Focus(txtName);

        }
    }
}
