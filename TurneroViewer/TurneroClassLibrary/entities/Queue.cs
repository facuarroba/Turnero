using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Xml.Serialization;

namespace TurneroClassLibrary.entities
{
    [Serializable]
    [XmlType("turnos")]
    public class Turnos
    {
        [XmlAttribute(AttributeName = "resultado")]
        public string resultado { get; set; }

        [XmlAttribute(AttributeName = "cantidad")]
        public string cantidad { get; set; }

        [XmlAttribute(AttributeName = "msg")]
        public string msg { get; set; }

        public int count
        {
            get
            {
                return Convert.ToInt32(cantidad);
            }
        }

        private Turno[] itemsField;

        [XmlElementAttribute("turno")]
        public Turno[] turnos
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }
        /*
        public List<Turno> listTurnos
        {
            get { return this.itemsField.ToList(); }
        }*/
    }

    [Serializable]
    [XmlType("turno")]
    public class Turno
    {
        private string[] nombreArray;

        [XmlAttribute(AttributeName = "idTurno")]
        public string idTurno { get; set; }

        [XmlAttribute(AttributeName = "hc")]
        public string hc { get; set; }

        [XmlAttribute(AttributeName = "descripcion_terminal")]
        public string terminal { get; set; }

        [XmlAttribute(AttributeName = "tipo_atencion")]
        public string tipoAtencion { get; set; }

        [XmlAttribute(AttributeName = "numeracion")]
        public string numeracion { get; set; }

        [XmlAttribute(AttributeName = "descripcion")]
        public string descripcion { get; set; }

        [XmlAttribute(AttributeName = "ts_accion")]
        public string ts_accion { get; set; }

        [XmlAttribute(AttributeName = "nombre")]
        public string nombreXML { get; set; }

        [XmlAttribute(AttributeName = "estado")]
        public string estado { get; set; }

        [XmlAttribute(AttributeName = "prioridad")]
        public string prioridad { get; set; }

        [XmlAttribute(AttributeName = "demora")]
        public string demora { get; set; }

        public String numeroString()
        {
            return tipoAtencion + " " + numeracion;
        }

        public string nombre 
        {
            get
            {
                nombreArray = nombreXML.Split('*');
                return nombreArray[0];
            }
            set
            {
                nombreArray[0] = value;
                nombreXML = nombreArray[0] + "*" + nombreArray[1];
            } 
        }

        public string nota
        {
            get
            {
                string res = "";
                nombreArray = nombreXML.Split('*');
                if (nombreArray.Count() > 1)
                    res = nombreArray[1];
                return res;
            }
            set
            {
                nombreArray[1] = value;
                nombreXML = nombreArray[0] + "*" + nombreArray[1];
            }
        }
        public int idTurnoInt
        {   
            get 
            {
                return Convert.ToInt32(idTurno);
            }
            
        }

        public Brush ForegroundColor
        {
            get
            {
                Brush res;
                int numPrioridad=1;
                if(prioridad != null)
                    numPrioridad = Convert.ToInt32(prioridad);
            
                if (numPrioridad > 1)
                    res = Brushes.Red;
                else
                    res = Brushes.Black;

                return res;
            }
        }

        public System.Windows.FontWeight FontWeight
        {
            get
            {
                System.Windows.FontWeight res;
                int numPrioridad = 1;
                if (prioridad != null)
                    numPrioridad = Convert.ToInt32(prioridad);

                if (numPrioridad > 1)
                    res = System.Windows.FontWeights.Bold;
                else
                    res = System.Windows.FontWeights.Normal;

                return res;
            }
        }
        public String NombreMostrar
        {
            get
            {
                return ToString();
            }
        }

        public override string ToString()
        {
            string result = numeroString()  +" - " + nombre + "(" + nota + ") - " + demora + " min.";
            return result;
        }

    }
}
