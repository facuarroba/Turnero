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
using TurneroClassLibrary.entities;

namespace TurneroAtencion2.componentes
{
    /// <summary>
    /// Lógica de interacción para ItemTurno.xaml
    /// </summary>
    public partial class ItemTurno : UserControl
    {
        private Turno turno;

        private string[] estados = { "esperando", "", "finalizado", "llamado", "atendido" };

        public Turno Turno
        {
            get { return turno; }
            set 
            { 
                turno = value;
                updateItem();
            }
        }

        public ItemTurno()
        {
            InitializeComponent();
        }

        private void updateItem()
        {
            lblName.Content = turno.nombre;
            lblHC.Content = "Historia Clínica: " + turno.hc;
            lblNro.Content = "N° " + turno.numeroString();
            lblEstado.Content = estados[Convert.ToInt16(turno.estado)];
            if (turno.estado.Equals("3"))
                mainGrid.Background = Brushes.Ivory;
            else if (turno.estado.Equals("4"))
                mainGrid.Background = Brushes.Lavender;
        }
    }
}
