using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ClienteInspeccion
{
    class Program
    {
        static void Main(string[] args)
        {
            ServicioArchivos.IServicioArchivos servicioArchivos = new ServicioArchivos.ServicioArchivosClient();
            ServicioConciliacion.IServicioConciliacion servicioConciliacion = new ServicioConciliacion.ServicioConciliacionClient();
            var connected = true;
            while (connected)
            {
                ImprimirMenu();
                var opcion = Console.ReadLine();
                if (opcion == "1")
                {
                    try
                    {
                        var listaArchivos = servicioArchivos.ListarArchivos();
                        Console.WriteLine("ARCHIVOS DEL SERVIDOR");
                        foreach (var file in listaArchivos)
                        {
                            Console.WriteLine(file);
                        }
                    }
                    catch (EndpointNotFoundException e)
                    {
                        Console.WriteLine("Error en la conexion al servicio de archivos");
                    }
                    catch (CommunicationObjectFaultedException e)
                    {
                        Console.WriteLine("Error en la conexion al servicio de archivos");
                    }
                }
                else if (opcion == "2")
                {
                    Console.WriteLine("Ingrese el nombre del archivo que desea eliminar");
                    var nombreArchivo = Console.ReadLine();
                    try
                    {
                        var eliminarArchivo = servicioArchivos.EliminarArchivo(nombreArchivo);
                        if (eliminarArchivo)
                            Console.WriteLine("El archivo " + nombreArchivo + " ha sido eliminado exitosamente");
                        else
                            Console.WriteLine("ERROR. El archivo " + nombreArchivo + " no pertenece al servidor");
                    }
                    catch (EndpointNotFoundException e)
                    {
                        Console.WriteLine("Error en la conexion al servicio de archivos");
                    }
                    catch (CommunicationObjectFaultedException e)
                    {
                        Console.WriteLine("Error en la conexion al servicio de archivos");
                    }
                    
                }
                else if (opcion == "3")
                {
                    Console.WriteLine("Ingrese el nombre del usuario del que desea obtener las notificaciones");
                    var nombreUsuario = Console.ReadLine();
                    try
                    {
                        var notificaciones = servicioConciliacion.ObtenerEstadisticas(nombreUsuario);
                        if (notificaciones.Count() == 0)
                            Console.WriteLine("No existen notificaciones para este usuario");
                        else
                            Console.WriteLine("Notificaciones del usuario " + nombreUsuario);
                        foreach (var not in notificaciones)
                        {
                            Console.WriteLine(not.msg);
                        }
                    }
                    catch (EndpointNotFoundException e)
                    {
                        Console.WriteLine("Error en la conexion al servicio de conciliacion");
                    }
                    catch (CommunicationObjectFaultedException e)
                    {
                        Console.WriteLine("Error en la conexion al servicio de conciliacion");
                    }
                }
                else if (opcion == "4")
                {
                    connected = false;
                }
                else
                {
                    Console.WriteLine("ERROR. Ingrese un comando valido");
                }
            }
        }

        public static void ImprimirMenu()
        {
            Console.WriteLine("MENU DEL CLIENTE DE INSPECCION");
            Console.WriteLine("Seleccione la opcion de la accion que desea realizar");
            Console.WriteLine("1 - Solicitar lista de archivos del servidor de archivos");
            Console.WriteLine("2 - Eliminar archivo del servidor de archivos");
            Console.WriteLine("3 - Obtener estadisticas por usuario del servidor de conciliacion");
            Console.WriteLine("4 - Salir");
        }
    }
}
