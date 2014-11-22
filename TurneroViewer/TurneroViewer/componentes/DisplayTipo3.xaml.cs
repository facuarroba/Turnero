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

namespace TurneroViewer.componentes
{
    /// <summary>
    /// Lógica de interacción para DisplayTipo1.xaml
    /// </summary>
    public partial class DisplayTipo3 : UserControl
    {
        public DisplayTipo3()
        {
            InitializeComponent();
        }

        public DisplayTipo3(TurneroClassLibrary.entities.Turno turno)
        {
            InitializeComponent();
            setTextNumber (turno.nombre);
            setTextSuperior(turno.descripcion);
        }

        public void setTextSuperior(String value)
        {
            this.textSuperior.Content = value;
        }

        public String getTextSuperior()
        {
            return this.textSuperior.Content.ToString();
        }

        public void setTextNumber(String value)
        {
            this.textNumber.Content = value;
        }

        public String getTextNumber()
        {
            return this.textNumber.Content.ToString();
        }
    }
}
