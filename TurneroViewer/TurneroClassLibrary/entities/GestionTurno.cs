using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace TurneroClassLibrary.entities
{
    [Serializable]
    [XmlType("llamado")]
    public class LlamaTurno
    {
        [XmlAttribute(AttributeName = "resultado")]
        public string resultado { get; set; }

        [XmlAttribute(AttributeName = "msg")]
        public string msg { get; set; }
    }

    [Serializable]
    [XmlType("cancelar")]
    public class CancelaLlamado
    {
        [XmlAttribute(AttributeName = "resultado")]
        public string resultado { get; set; }

        [XmlAttribute(AttributeName = "msg")]
        public string msg { get; set; }
    }

    [Serializable]
    [XmlType("atender")]
    public class AtiendeTurno
    {
        [XmlAttribute(AttributeName = "resultado")]
        public string resultado { get; set; }

        [XmlAttribute(AttributeName = "msg")]
        public string msg { get; set; }
    }

    [Serializable]
    [XmlType("finalizar")]
    public class FinalizaTurno
    {
        [XmlAttribute(AttributeName = "resultado")]
        public string resultado { get; set; }

        [XmlAttribute(AttributeName = "msg")]
        public string msg { get; set; }
    }
}
