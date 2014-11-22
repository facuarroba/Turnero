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
using TurneroClassLibrary.entities;

namespace TurneroViewer.componentes
{
    /// <summary>
    /// Lógica de interacción para DisplayTipo2.xaml
    /// </summary>
    public partial class DisplayTipo2 : UserControl
    {
        public DisplayTipo2()
        {
            InitializeComponent();
        }

        public DisplayTipo2(Turno turno)
        {
            InitializeComponent();
            setTextNumber (turno.nombre);
            setTextInferior("");
            setTextSuperior(turno.descripcion);
        }

        public void setTextSuperior(String value)
        {
            if (value == null) value = "";
            this.textSuperior.Content = value;
        }

        public String getTextSuperior()
        {
            return this.textSuperior.Content.ToString();
        }

        public void setTextInferior(String value)
        {
            if (value == null) value = "";
            this.textInferior.Content = value;
        }

        public String getTextInferior()
        {
            return this.textInferior.Content.ToString();
        }

        public void setTextNumber(String value)
        {
            if (value == null) value = "";
            this.textNumber.Content = value;
        }

        public String getTextNumber()
        {
            return this.textNumber.Content.ToString();
        }
    }
}
