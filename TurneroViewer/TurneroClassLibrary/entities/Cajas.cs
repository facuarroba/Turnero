using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TurneroClassLibrary.entities
{
    [Serializable]
    [XmlType("cajas")]
    public class Cajas
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

        private Caja[] itemsField;

        [XmlElementAttribute("caja")]
        public Caja[] cajas
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
    }

    [Serializable]
    [XmlType("caja")]
    public class Caja
    {
        [XmlAttribute(AttributeName = "tipo_atencion")]
        public string tipoAtencion { get; set; }

        [XmlAttribute(AttributeName = "descripcion")]
        public string nombre { get; set; }

        public override string ToString()
        {
            return nombre;
        }
    }
}
