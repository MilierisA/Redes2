using DominioServidor;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Servidor
{
    public static class SistemaServidor
    {
        private const string directorioServidor = @"../../../Servidor/directorioServidor";
        public static List<string> archivosEnServidor;

        public static void PublicarServicio()
        {
            ServiceHost servicioArchivos = null;
            try
            {
                //Base Address for Service
                Uri httpBaseAddress = new Uri("http://localhost:4321/ServicioArchivos");

                //Instantiate ServiceHost
                servicioArchivos = new ServiceHost(typeof(Servidor.ServicioArchivos),
                    httpBaseAddress);

                //Add Endpoint to Host
                servicioArchivos.AddServiceEndpoint(typeof(Servidor.IServicioArchivos),
                                                        new WSHttpBinding(), "");

                //Metadata Exchange
                ServiceMetadataBehavior serviceBehavior = new ServiceMetadataBehavior();
                serviceBehavior.HttpGetEnabled = true;
                servicioArchivos.Description.Behaviors.Add(serviceBehavior);

                //Open
                servicioArchivos.Open();
                Console.WriteLine("Servicio de archivos disponible en: {0}", httpBaseAddress);
            }
            catch (Exception ex)
            {
                servicioArchivos = null;
                Console.WriteLine("There is an issue with servicioArchivos " + ex.Message);
            }
        }

        public static string getComando(byte[] data)
        {
            Protocolo.Protocolo prot = Protocolo.Protocolo.BytesAProtocolo(data);
            return prot.darComando();
        }
        public static byte[] getDatos(byte[] data)
        {

            Protocolo.Protocolo prot = Protocolo.Protocolo.BytesAProtocolo(data);
            return prot.darBytes();
        }

        public static int darLargo(byte[] data)
        {
            Protocolo.Protocolo prot = Protocolo.Protocolo.BytesAProtocolo(data);
            return prot.DarLargo();
        }

        public static byte[] IniciarSesion(byte[] data)
        {
            //Procesar la info y validar el usuario
            Protocolo.Protocolo prot = Protocolo.Protocolo.BytesAProtocolo(data);
            var nombreUsuario = Encoding.UTF8.GetString(prot.darBytes()).TrimEnd().Replace("\0", string.Empty);
            bool valido = false;

            XmlDocument datosServer = new XmlDocument();
            datosServer.Load("../../../Servidor/UsuariosHabilitados.xml");

            XmlNodeList users = datosServer.GetElementsByTagName("usuarios");

            //Primero verificamos que exista una conexion para este cliente y luego que este habilitado
            if (!ConexionServidor.YaEstaConectado(nombreUsuario))
            {
                foreach (XmlElement user in users)
                {
                    var nombresUsuarios = user.ChildNodes;
                    foreach (XmlElement usuario in nombresUsuarios)
                    {
                        string nombre = usuario.FirstChild.Value;
                        if (nombre == nombreUsuario)
                            valido = true;
                    }

                }
            }

            string respuesta = nombreUsuario + "#" + valido.ToString();
            Protocolo.Protocolo ProtRespuesta = new Protocolo.Protocolo("RES", "1", 9 + respuesta.ToString().Length, Encoding.ASCII.GetBytes(respuesta.ToString()));
            return Encoding.ASCII.GetBytes(ProtRespuesta.ToString());
        }

        public static byte[] ListarArchivos()
        {
            string directorio = directorioServidor;
            string[] fileEntries = Directory.GetFiles(directorio);
            string archivos = "";
            foreach (var file in fileEntries)
            {
                archivos += file + "#";
            }
            string comando = "2";
            string header = "RES";
            int largo = 9 + archivos.Length;

            Protocolo.Protocolo protocoloRespuesta = new Protocolo.Protocolo(header, comando, largo, Encoding.ASCII.GetBytes(archivos));
            return Encoding.ASCII.GetBytes(protocoloRespuesta.ToString());
        }

        public static byte[] SubirArchivo(string datos)
        {
            string nombreArchivoYCantPaquetes = datos.TrimEnd().Replace("\0", string.Empty);
            string nombreArchivo = nombreArchivoYCantPaquetes.Split('#')[0];
            int cantPaquetes = Int16.Parse(nombreArchivoYCantPaquetes.Split('#')[1]);
            string mensajeRet = "";

            //Verificamos que no exista un archivo con ese nombre
            if (archivosEnServidor == null)
                archivosEnServidor = new List<string>();

            lock (archivosEnServidor)
            {
                if (archivosEnServidor.Contains(nombreArchivo))
                {
                    mensajeRet = "ERROR";
                }
                else
                {
                    mensajeRet = "OK";
                    archivosEnServidor.Add(nombreArchivo);
                }
            }

            Protocolo.Protocolo protocoloRespuesta = new Protocolo.Protocolo("RES", "3", 9 + mensajeRet.Length, Encoding.ASCII.GetBytes(mensajeRet));
            return Encoding.ASCII.GetBytes(protocoloRespuesta.ToString());
        }

        public static byte[] DescargarArchivo(byte[] datos)
        {
            string directorio = directorioServidor;
            string nombreArchivo = Encoding.UTF8.GetString(datos).TrimEnd().Replace("\0", string.Empty).Split('#')[0];
            int cantPaquetes = 0;
            //Aca hay que verificar que exista el archivo
            try
            {
                int largo = File.ReadAllBytes(directorio + "\\" + nombreArchivo).Length;
                cantPaquetes = (largo / 9990) + 1; // minimo 1
            }
            catch (FileNotFoundException e)
            {

            }
            string archivoYCantPaquetes = nombreArchivo + "#" + cantPaquetes;
            Protocolo.Protocolo protocoloRespuesta = new Protocolo.Protocolo("RES", "4", 9 + archivoYCantPaquetes.Length, Encoding.ASCII.GetBytes(archivoYCantPaquetes));
            string protocoloString = protocoloRespuesta.ToString();
            byte[] bytes = Encoding.ASCII.GetBytes(protocoloString);
            return bytes;
        }

        public static byte[] DescargarPaqueteDeArchivo(byte[] paqueteActual)
        {

            int largoTotal = 9 + paqueteActual.Length;
            string primeraParteProtocolo = "RES08" + largoTotal.ToString("0000");
            byte[] primeraParteProtocoloBytes = Encoding.ASCII.GetBytes(primeraParteProtocolo);
            byte[] bytes = new byte[primeraParteProtocoloBytes.Length + paqueteActual.Length];
            primeraParteProtocoloBytes.CopyTo(bytes, 0);
            paqueteActual.CopyTo(bytes, primeraParteProtocoloBytes.Length);
            return bytes;
            //Este tiene que cargar el proximo paquete y enviarlo
        }

        public static byte[] SubirPaqueteDeArchivo(byte[] datos)
        {
            string datosString = Encoding.UTF8.GetString(datos).TrimEnd().Replace("\0", string.Empty);
            string mensajeOK = "OK";
            Protocolo.Protocolo protocoloRespuesta = new Protocolo.Protocolo("RES", "7", 9 + mensajeOK.Length, Encoding.ASCII.GetBytes(mensajeOK));
            return Encoding.ASCII.GetBytes(protocoloRespuesta.ToString());
        }

        public static void GuardarPaqueteDeArchivo(FileStream archivo, byte[] paquete, string nombreArchivo)
        {
            archivo.Write(paquete, 0, paquete.Length);
        }

        public static byte[] SincronizarServidor(string server)
        {
            string[] datosServidor = server.Split('#');
            string ipServidor = datosServidor[0];
            string puertoServidor = datosServidor[1];
            string directorioServidor = datosServidor[2];
            return null;
        }

        public static byte[] NotificarSincronizacion()
        {
            string mensajeOK = "OK";
            Protocolo.Protocolo protocoloRespuesta = new Protocolo.Protocolo("RES", "5", 9 + mensajeOK.Length, Encoding.ASCII.GetBytes(mensajeOK));
            return Encoding.ASCII.GetBytes(protocoloRespuesta.ToString());
        }

    }
}
