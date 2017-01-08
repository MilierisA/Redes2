using Notificaciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace ServidorConciliacion
{
    public static class SistemaConciliacion
    {
        public static List<ObjetoNotificacion> listaEventos { get; set; }
        public static List<ObjetoNotificacion> listaRegistroActividad { get; set; }

        public static List<ObjetoNotificacion> GetListaEventos()
        {
            if (listaEventos == null)
                listaEventos = new List<ObjetoNotificacion>();

            return listaEventos;
        }

        public static void generarEstadisticas(List<ObjetoNotificacion> lista)
        {
            if(lista != null)
            {
                var inicios = lista.Where(n => n.accion == 1);
                if (inicios != null && inicios.Count() > 0)
                    Console.WriteLine("Inicios de Sesión:");
                foreach (var i in inicios)
                {
                    Console.WriteLine("El ususario " + i.usuario + " inicio sesion");
                }
                var descargas = lista.Where(n => n.accion == 2);
                if (descargas != null && descargas.Count() > 0)
                    Console.WriteLine("Descarga de archivos:");
                foreach (var i in descargas)
                {
                    Console.WriteLine("El ususario " + i.usuario + " descargo un archivo");
                }
                var cargas = lista.Where(n => n.accion == 3);
                if (cargas != null && cargas.Count() > 0)
                    Console.WriteLine("Carga de archivos:");
                foreach (var i in cargas)
                {
                    Console.WriteLine("El ususario " + i.usuario + " cargo un archivo");
                }
                var cierres = lista.Where(n => n.accion == 4);
                if (cierres != null && cierres.Count() > 0)
                    Console.WriteLine("Cierres de Sesión:");
                foreach (var i in cierres)
                {
                    Console.WriteLine("El ususario " + i.usuario + " cerro sesion");
                }
            }
        }

        public static void PublicarServicioConciliacion()
        {
            ServiceHost servicioConciliacion = null;
            try
            {
                //Base Address for Service
                Uri httpBaseAddress = new Uri("http://localhost:4322/servicioConciliacion");

                //Instantiate ServiceHost
                servicioConciliacion = new ServiceHost(typeof(ServidorConciliacion.ServicioConciliacion),
                    httpBaseAddress);

                //Add Endpoint to Host
                servicioConciliacion.AddServiceEndpoint(typeof(ServidorConciliacion.IServicioConciliacion),
                                                        new WSHttpBinding(), "");

                //Metadata Exchange
                ServiceMetadataBehavior serviceBehavior = new ServiceMetadataBehavior();
                serviceBehavior.HttpGetEnabled = true;
                servicioConciliacion.Description.Behaviors.Add(serviceBehavior);

                //Open
                servicioConciliacion.Open();
                Console.WriteLine("Servicio de conciliacion disponible en: {0}", httpBaseAddress);
            }
            catch (Exception ex)
            {
                servicioConciliacion = null;
                Console.WriteLine("There is an issue with servicioConciliacion " + ex.Message);
            }
        }
    }
}
