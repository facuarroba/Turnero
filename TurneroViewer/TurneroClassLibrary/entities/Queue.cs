using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public string nombre { get; set; }

        [XmlAttribute(AttributeName = "estado")]
        public string estado { get; set; }

        public String numeroString()
        {
            return tipoAtencion + " " + numeracion;
        }
        
        public override string ToString()
        {
            string result = "Nombre: "+ nombre + " - Numero: " + numeroString();
            return result;
        }

    }
}
