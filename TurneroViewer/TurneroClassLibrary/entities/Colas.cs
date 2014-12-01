using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace TurneroClassLibrary.entities
{
    [Serializable]
    [XmlType("colas")]
    public class Colas
    {
        private String mensaje;

        [XmlAttribute(AttributeName = "resultado")]
        public string resultado { get; set; }

         [XmlAttribute(AttributeName = "msg")]
        public string msg 
        { 
            get 
            {return mensaje;}
            set 
            { mensaje = value;}
        }

        public int count
        {
            get
            {
                int res = 0;
                if (itemsField != null) 
                    res = itemsField.Count();
                return res;
            }
        }

        private Cola[] itemsField;

        [XmlElementAttribute("cola")]
        public Cola[] colas
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
    [XmlType("cola")]
    public class Cola
    {
        [XmlAttribute(AttributeName = "tipo_atencion")]
        public string tipoAtencion { get; set; }

        [XmlAttribute(AttributeName = "descripcion")]
        public string descripcion { get; set; }

        public override string ToString()
        {
            return descripcion;
        }

    }
}
