using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace ServidorConciliacion
{
    class Program
    {
        static void Main(string[] args)
        {
            SistemaConciliacion.PublicarServicioConciliacion();
            TcpChannel channel = new TcpChannel(5000);
            ChannelServices.RegisterChannel(channel, false);
            RemotingConfiguration.RegisterWellKnownServiceType(
               typeof(OperacionConciliacion), "OperacionConciliacion",
               WellKnownObjectMode.Singleton);
            Console.WriteLine("Remote server is running");
            //Console.ReadLine();
           
            var connected = true;
            while (connected)
            {
                ImprimirMenu();
                var opcion = Console.ReadLine();
                if (opcion == "1")
                {
                    var estadisticasRegistro = SistemaConciliacion.listaRegistroActividad;
                    var estadisticasConciliacion = SistemaConciliacion.listaEventos;
                    Console.WriteLine("Estadisticas del Servidor de Registro");
                    SistemaConciliacion.generarEstadisticas(estadisticasRegistro);
                    Console.WriteLine("Estadisticas del Servidor de Conciliacion");
                    SistemaConciliacion.generarEstadisticas(estadisticasConciliacion);
                }
                else if (opcion == "2")
                {
                    connected = false;
                }
                else
                {
                    Console.WriteLine("ERROR. Ingrese un comando valido");
                }
            }
            ChannelServices.UnregisterChannel(channel);
        }

        public static void ImprimirMenu()
        {
            Console.WriteLine("MENU DEL SERVIDOR DE CONCILIACION");
            Console.WriteLine("1 - Obtener estadisiticas y comparar");
            Console.WriteLine("2 - Salir");
        }
    }
}
