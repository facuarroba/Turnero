using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TurneroClassLibrary.entities
{
    [Serializable]
    [XmlType("terminal")]
    public class Terminal
    {
        [XmlAttribute(AttributeName = "resultado")]
        public string resultado { get; set; }

        [XmlAttribute(AttributeName = "msg")]
        public string msg { get; set; }

        public override string ToString()
        {
            return msg;
        }
    }
}
