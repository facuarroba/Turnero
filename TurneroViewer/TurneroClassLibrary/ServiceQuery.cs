using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TurneroClassLibrary.entities;

namespace TurneroClassLibrary
{
    public class ServiceQuery
    {
        private String SERVER = "http://192.168.1.55:8080";
        private String URL_PATH = "Turnos/servicios/";

        private String WEATHER_SERVER = "http://api.openweathermap.org";
        private String WEATHER_PATH = "data/2.5/weather?q=mar%20del%20plata&mode=xml";


        private static ServiceQuery instance;

        private NLogLogger logger;
        private ServiceQuery() 
        {
            logger = new NLogLogger();
        }

        public static ServiceQuery Instance
        {
            get 
            {
                if (instance == null)
                {
                    instance = new ServiceQuery();
                }
                return instance;
            }
        }

        public String server
        {
            get { return SERVER; }
            set { SERVER = value; }
        }

        public String urlPath
        {
            get { return URL_PATH; }
            set { URL_PATH = value; }
        }

        public String weatherServer
        {
            get { return WEATHER_SERVER; }
            set { WEATHER_SERVER = value; }
        }

        public String weatherPath
        {
            get { return WEATHER_PATH; }
            set { WEATHER_PATH = value; }
        }

        public CurrentWeather getClima()
        {
            var client = new RestClient(weatherServer);

            RestRequest request = new RestRequest(weatherPath, Method.GET);
            IRestResponse queryResult = client.Execute(request);


            CurrentWeather clima = null;

            XmlSerializer mySerializer = new XmlSerializer(typeof(CurrentWeather));

            if (queryResult.Content != "")
            {
                using (TextReader reader = new StringReader(queryResult.Content))
                {
                    clima = (CurrentWeather)mySerializer.Deserialize(reader);
                }
            }
            return clima;
        }

        public Turnos consultarTurnos(String idTerminal, String estado = "0")
        {
            String ServiceName = "consultarTurnos.jsp";
            String XML = "";
            Turnos turnos;
            String URL_parameters = "idTerminal="+idTerminal+"&estado=" + estado ;
            String URL = URL_PATH + ServiceName + "?" + URL_parameters;

            var client = new RestClient(SERVER);
            client.Timeout = 1000;
            logger.Info(ServiceName + " - URL: " + URL);
            try
            {
                RestRequest request = new RestRequest(URL, Method.GET);
                
                IRestResponse queryResult = client.Execute(request);

                XML = queryResult.Content;

                logger.Info(ServiceName + " - Parametros: idTerminal = " + idTerminal + " - estado = " + estado + " - Respuesta: " + XML);
                XmlSerializer mySerializer = new XmlSerializer(typeof(Turnos));
                using (TextReader reader = new StringReader(XML))
                {
                    turnos = (Turnos)mySerializer.Deserialize(reader);
                }
                return turnos;
            }
            catch (Exception e)
            {
                Turnos error = new Turnos();
                error.resultado = "error";
                error.msg= "internal error: " + e.Message;
                logger.Error(ServiceName + " - Msg: " + e.Message);
                return error;
            }
            
        }


        public Turnos consultarTurnosAtendidos(String idTerminal)
        {
            String ServiceName = "getLlamadosAtendidos.jsp";
            String XML = "";
            Turnos turnos;
            String URL_parameters = "idTerminal=" + idTerminal;
            String URL = URL_PATH + ServiceName + "?" + URL_parameters;

            var client = new RestClient(SERVER);
            client.Timeout = 1000;
            logger.Info(ServiceName + " - URL: " + URL);
            try
            {
                RestRequest request = new RestRequest(URL, Method.GET);

                IRestResponse queryResult = client.Execute(request);

                XML = queryResult.Content;

                logger.Info(ServiceName + " - Parametros: idTerminal = " + idTerminal + " - Respuesta: " + XML);
                XmlSerializer mySerializer = new XmlSerializer(typeof(Turnos));
                using (TextReader reader = new StringReader(XML))
                {
                    turnos = (Turnos)mySerializer.Deserialize(reader);
                }
                return turnos;
            }
            catch (Exception e)
            {
                Turnos error = new Turnos();
                error.resultado = "error";
                error.msg = "internal error: " + e.Message;
                logger.Error(ServiceName + " - Msg: " + e.Message);
                return error;
            }

        }

        public List<Turno> ordenarTurnos(Turno[] lista)
        {
            List<Turno> res = lista.OrderBy(o => o.idTurnoInt).ToList();
            return res;
        }

        //public Cajas getCajasHabilitadas()
        //{
        //    String ServiceName = "getCajasHabilitadas.jsp";
        //    String XML = "";
        //    Cajas cajas = new Cajas();
        //    String URL = URL_PATH + ServiceName;

        //    var client = new RestClient(SERVER);
        //    client.Timeout = 1000;
        //    logger.Info(ServiceName + " - URL: " + URL);
        //    try 
        //    {
        //        RestRequest request = new RestRequest(URL, Method.GET);
        //        IRestResponse queryResult = client.Execute(request);

        //        XML = queryResult.Content;
        //        logger.Info(ServiceName + " - Respuesta: " + XML);
        //        XmlSerializer mySerializer = new XmlSerializer(typeof(Cajas));
        //        using (TextReader reader = new StringReader(XML))
        //        {           
        //            cajas = (Cajas)mySerializer.Deserialize(reader);
        //        }
        //        return cajas;
        //    }
        //    catch(Exception e)
        //    {
        //        cajas.resultado = "error";
        //        cajas.msg = "internal error: " + e.Message;
        //        logger.Error(ServiceName + " - Msg: " + e.Message);
        //        return cajas;
        //    }

        //}

        public Registro registrarTurno(Cola cola, String hc, String nombre = "")
        {
            String ServiceName = "registrarTurno.jsp";
            Registro registro = null;
            String XML = "";
            String URL_parameters = "";
            if (hc == "") hc = "0";
            URL_parameters = "hc=" + hc + "&nombre=" + nombre + "&tipo_atencion=" + cola.tipoAtencion;

            var client = new RestClient(SERVER);
            client.Timeout = 1000;
            try
            {
                RestRequest request = new RestRequest(URL_PATH + ServiceName + "?" + URL_parameters, Method.GET);
                IRestResponse queryResult = client.Execute(request);

                XML = queryResult.Content;

                logger.Info(ServiceName + " - Parametros: Caja = " + cola + " - nombre = "+ nombre +" - Respuesta: " + XML);
                XmlSerializer mySerializer = new XmlSerializer(typeof(Registro));
                using (TextReader reader = new StringReader(XML))
                {
                    registro = (Registro)mySerializer.Deserialize(reader);
                }
                return registro;
            }
            catch(Exception e)
            {
                Registro error = new Registro();
                error.resultado = "error";
                error.msg = "internal error: " + e.Message;
                logger.Error(ServiceName + " - Msg: " + e.Message);
                return error;
            }
        }

        public LlamaTurno llamarTurno(String idTurno,String idTerminal )
        {
            String ServiceName = "llamarTurno.jsp";
            String XML = "";
            LlamaTurno objeto;
            String URL_parameters = "";
            URL_parameters = "idTerminal=" + idTerminal + "&idTurno=" + idTurno;

            var client = new RestClient(SERVER);
            client.Timeout = 1000;
            try
            {
                RestRequest request = new RestRequest(URL_PATH + ServiceName + "?" + URL_parameters, Method.GET);
                IRestResponse queryResult = client.Execute(request);

                XML = queryResult.Content;

                logger.Info(ServiceName + " - Parametros: idTurno = " + idTurno + " - idTerminal = " + idTerminal + " - Respuesta: " + XML);
                XmlSerializer mySerializer = new XmlSerializer(typeof(LlamaTurno));
                using (TextReader reader = new StringReader(XML))
                {
                    objeto = (LlamaTurno)mySerializer.Deserialize(reader);
                }
                return objeto;
            }
            catch (Exception e)
            {
                LlamaTurno error = new LlamaTurno();
                error.resultado = "error";
                error.msg = "internal error: " + e.Message;
                logger.Error(ServiceName + " - Msg: " + e.Message);
                return error;
            }

        }

        public AtiendeTurno atenderTurno(String idTurno)
        {
            String ServiceName = "atenderTurno.jsp";
            String XML = "";
            AtiendeTurno objeto;
            String URL_parameters = "";
            URL_parameters = "idTurno=" + idTurno;

            var client = new RestClient(SERVER);
            client.Timeout = 1000;

            try
            {
                RestRequest request = new RestRequest(URL_PATH + ServiceName + "?" + URL_parameters, Method.GET);
                IRestResponse queryResult = client.Execute(request);

                XML = queryResult.Content;

                logger.Info(ServiceName + " - Parametros: idTurno = " + idTurno + " - Respuesta: " + XML);

                XmlSerializer mySerializer = new XmlSerializer(typeof(AtiendeTurno));
                using (TextReader reader = new StringReader(XML))
                {
                    objeto = (AtiendeTurno)mySerializer.Deserialize(reader);
                }
                return objeto;
            }
            catch (Exception e)
            {
                AtiendeTurno error = new AtiendeTurno();
                error.resultado = "error";
                error.msg = "internal error: " + e.Message;
                logger.Error(ServiceName + " - Msg: " + e.Message);
                return error;
            }

        }

        public FinalizaTurno finalizaTurno(String idTurno)
        {
            String ServiceName = "finalizarTurno.jsp";
            String XML = "";
            FinalizaTurno objeto;
            String URL_parameters = "";
            URL_parameters = "idTurno=" + idTurno;

            var client = new RestClient(SERVER);
            client.Timeout = 1000;
            try
            {
                RestRequest request = new RestRequest(URL_PATH + ServiceName +"?" + URL_parameters, Method.GET);
                IRestResponse queryResult = client.Execute(request);

                XML = queryResult.Content;

                logger.Info(ServiceName + " - Parametros: idTurno = " + idTurno + " - Respuesta: " + XML);

                XmlSerializer mySerializer = new XmlSerializer(typeof(FinalizaTurno));
                using (TextReader reader = new StringReader(XML))
                {
                    objeto = (FinalizaTurno)mySerializer.Deserialize(reader);
                }
                return objeto;
            }
            catch (Exception e)
            {
                FinalizaTurno error = new FinalizaTurno();
                error.resultado = "error";
                error.msg = "internal error: " + e.Message;
                logger.Error(ServiceName + " - Msg: " + e.Message);
                return error;
            }

        }

        public Colas getColas(String recepcion = "1")
        {
            String ServiceName = "getColas.jsp";
            String XML = "";
            Colas colas;
            String URL_parameters = "recepcion=" + recepcion;
            String URL = URL_PATH + ServiceName + "?" + URL_parameters;

            var client = new RestClient(SERVER);
            client.Timeout = 1000;
            logger.Info(ServiceName + " - URL: " + URL);
            try
            {
                RestRequest request = new RestRequest(URL, Method.GET);

                IRestResponse queryResult = client.Execute(request);

                XML = queryResult.Content;

                logger.Info(ServiceName + " - Parametros: recepcion = " + recepcion + " - Respuesta: " + XML);
                XmlSerializer mySerializer = new XmlSerializer(typeof(Colas));
                using (TextReader reader = new StringReader(XML))
                {
                    colas = (Colas)mySerializer.Deserialize(reader);
                }
                return colas;
            }
            catch (Exception e)
            {
                Colas error = new Colas();
                error.resultado = "error";
                error.msg = "internal error: " + e.Message;
                logger.Error(ServiceName + " - Msg: " + e.Message);
                return error;
            }
        }

        public CancelaLlamado cancelarLlamado(String idTurno)
        {
            String ServiceName = "cancelarLlamado.jsp";
            String XML = "";
            CancelaLlamado objeto;
            String URL_parameters = "";
            URL_parameters = "idTurno=" + idTurno;

            var client = new RestClient(SERVER);
            client.Timeout = 1000;
            try
            {
                RestRequest request = new RestRequest(URL_PATH + ServiceName +"?" + URL_parameters, Method.GET);
                IRestResponse queryResult = client.Execute(request);

                XML = queryResult.Content;

                logger.Info(ServiceName + " - Parametros: idTurno = " + idTurno + " - Respuesta: " + XML);

                XmlSerializer mySerializer = new XmlSerializer(typeof(CancelaLlamado));
                using (TextReader reader = new StringReader(XML))
                {
                    objeto = (CancelaLlamado)mySerializer.Deserialize(reader);
                }
                return objeto;
            }
            catch (Exception e)
            {
                CancelaLlamado error = new CancelaLlamado();
                error.resultado = "error";
                error.msg = "internal error: " + e.Message;
                logger.Error(ServiceName + " - Msg: " + e.Message);
                return error;
            }

        }

    }
}
