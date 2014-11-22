using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Globalization;

namespace TurneroClassLibrary.entities
{
    [Serializable]
    [XmlType("current")]
    public class CurrentWeather
    {
        [XmlElement(ElementName = "city")]
        public City city { get; set; }

        [XmlElement(ElementName = "temperature")]
        public Temperatura temperatura { get; set; }

        [XmlElement(ElementName = "humidity")]
        public Humedad humedad { get; set; }

        [XmlElement(ElementName = "pressure")]
        public Presion presion { get; set; }

        [XmlElement(ElementName = "wind")]
        public Viento viento { get; set; }

        [XmlElement(ElementName = "clouds")]
        public Nubes nubes { get; set; }

        [XmlElement(ElementName = "weather")]
        public Clima clima { get; set; }

        [XmlElement(ElementName = "lastupdate")]
        public UltimaActualizacion ultimaActualizacion { get; set; }

        public String toString()
        {
            String res = "T: " + temperatura.toC() + "/ H: " + humedad.valor + humedad.unidad;
            return res;
        }

    }

    [XmlType("city")]
    public class City
    {
        [XmlAttribute(AttributeName = "id")]
        public string id { get; set; }

        [XmlAttribute(AttributeName = "name")]
        public string nombre { get; set; }

        [XmlElement(ElementName = "coord")]
        public Coordenadas coordenadas { get; set; }

        [XmlElement(ElementName = "country")]
        public string pais { get; set; }

        [XmlElement(ElementName = "sun")]
        public Sol sol { get; set; }

    }

    [XmlType("sun")]
    public class Sol
    {
        [XmlAttribute(AttributeName = "rise")]
        public string amanecer { get; set; }

        [XmlAttribute(AttributeName = "set")]
        public string atardecer { get; set; }
    }

    [XmlType("coord")]
    public class Coordenadas
    {
        [XmlAttribute(AttributeName = "lon")]
        public string longitud { get; set; }

        [XmlAttribute(AttributeName = "lat")]
        public string latitud { get; set; }
    }

    [XmlType("temperature")]
    public class Temperatura
    {
        [XmlAttribute(AttributeName = "value")]
        public string valor { get; set; }

        [XmlAttribute(AttributeName = "min")]
        public string minimo { get; set; }

        [XmlAttribute(AttributeName = "max")]
        public string maximo { get; set; }

        [XmlAttribute(AttributeName = "unit")]
        public string unidad { get; set; }

        public string toC()
        {
            float val = float.Parse(valor, CultureInfo.InvariantCulture.NumberFormat);
            val = val - 273.0f;
            return val.ToString("0.0") + " °C";
        }
    }

    [XmlType("humidity")]
    public class Humedad
    {
        [XmlAttribute(AttributeName = "value")]
        public string valor { get; set; }

        [XmlAttribute(AttributeName = "unit")]
        public string unidad { get; set; }
    }

    [XmlType("pressure")]
    public class Presion
    {
        [XmlAttribute(AttributeName = "value")]
        public string valor { get; set; }

        [XmlAttribute(AttributeName = "unit")]
        public string unidad { get; set; }
    }

    [XmlType("wind")]
    public class Viento
    {
        [XmlElement(ElementName = "speed")]
        public Velocidad velocidad { get; set; }

        [XmlElement(ElementName = "direction")]
        public Direccion direccion { get; set; }
    }

    [XmlType("speed")]
    public class Velocidad
    {
        [XmlAttribute(AttributeName = "value")]
        public string valor { get; set; }

        [XmlAttribute(AttributeName = "name")]
        public string nombre { get; set; }
    }

    [XmlType("direction")]
    public class Direccion
    {
        [XmlAttribute(AttributeName = "value")]
        public string valor { get; set; }

        [XmlAttribute(AttributeName = "code")]
        public string codigo { get; set; }

        [XmlAttribute(AttributeName = "name")]
        public string nombre { get; set; }
    }

    [XmlType("clouds")]
    public class Nubes
    {
        [XmlAttribute(AttributeName = "value")]
        public string valor { get; set; }

        [XmlAttribute(AttributeName = "name")]
        public string nombre { get; set; }
    }

    [XmlType("weather")]
    public class Clima
    {
        [XmlAttribute(AttributeName = "number")]
        public string numero { get; set; }

        [XmlAttribute(AttributeName = "value")]
        public string valor { get; set; }

        [XmlAttribute(AttributeName = "icon")]
        public string icono { get; set; }
    }

    [XmlType("lastupdate")]
    public class UltimaActualizacion
    {
        [XmlAttribute(AttributeName = "value")]
        public string valor { get; set; }
    }
}
