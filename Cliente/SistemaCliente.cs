using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Protocolo;
using System.IO;
using System.Messaging;

namespace DominioCliente
{
    public static class SistemaCliente
    {
        private const string directorioCliente = @"../../../Cliente/directorioCliente";
        public static List<string> archivosBajados;
        private const string pathMsgQueue = @".\private$\ClientQueue";

        public static MessageQueue GetMsgQueue()
        {
            if (MessageQueue.Exists(pathMsgQueue))
            {
                return new MessageQueue(pathMsgQueue);
            }
            else
            {
                return MessageQueue.Create(pathMsgQueue);
            }
        }
        public static int darLargo(byte[] data)
        {
            Protocolo.Protocolo prot = Protocolo.Protocolo.BytesAProtocolo(data);
            return prot.DarLargo();
        }

        public static byte[] getDatos(byte[] data)
        {

            Protocolo.Protocolo prot = Protocolo.Protocolo.BytesAProtocolo(data);
            return prot.darBytes();
        }

        public static byte[] ProcesarPedido(string comando, string informacion = "")
        {
            if (comando == "1")
            {
                return IniciarSesion(informacion);
            }
            if (comando == "2")
            {
                return SolicitarArchivos();
            }
            if (comando == "3")
            {
                return CargarArchivo(informacion);
            }
            if (comando == "4")
            {
                return SolicitarDescargarArchivo(informacion);
            }
            if (comando == "5")
            {
                return SincronizarServidor(informacion); 
            }
            if (comando == "6")
            {
                return CerrarSesion();
            }
            if (comando == "8")
            {
                return DescargarPaqueteDeArchivo(informacion);
            }


            return null;
        }

        public static byte[] CerrarSesion()
        {
            Protocolo.Protocolo prot = new Protocolo.Protocolo("REQ", "6", 9 ,new byte[0]);
            String protocoloString = prot.ToString();
            byte[] bytes = Encoding.ASCII.GetBytes(protocoloString);
            return bytes;
        }

        public static byte[] IniciarSesion(string nombreUsuario)
        {
            Protocolo.Protocolo prot = new Protocolo.Protocolo("REQ", "1", 9 + nombreUsuario.Length, Encoding.ASCII.GetBytes(nombreUsuario));
            String protocoloString = prot.ToString();
            byte[] bytes = Encoding.ASCII.GetBytes(protocoloString);
            return bytes;
        }

        public static string ProcesarRespuesta(byte[] respuesta)
        {
            Protocolo.Protocolo protocoloRespuesta = Protocolo.Protocolo.BytesAProtocolo(respuesta);
            byte[] bytesRespuesta = protocoloRespuesta.darBytes();
            string ret = Encoding.UTF8.GetString(bytesRespuesta).TrimEnd().Replace("\0", string.Empty);
            return ret;
        }

        public static byte[] SolicitarArchivos()
        {
            Protocolo.Protocolo prot = new Protocolo.Protocolo("REQ", "2", 9, new byte[0]);
            String protocoloString = prot.ToString();
            byte[] bytes = Encoding.ASCII.GetBytes(protocoloString);
            return bytes;
        }

        public static byte[] CargarArchivo(string nombreArchivo)
        {
            int cantPaquetes = 0;
            try
            {
                int largo = File.ReadAllBytes(directorioCliente + "\\" + nombreArchivo).Length;
                cantPaquetes = (largo / 9990) + 1; // minimo 1
            }
            catch (FileNotFoundException e)
            {
                
            }
            catch (DirectoryNotFoundException e)
            {

            }
            string archivoYCantPaquetes = nombreArchivo + "#" + cantPaquetes;
            Protocolo.Protocolo prot = new Protocolo.Protocolo("REQ", "3", 9 + archivoYCantPaquetes.Length, Encoding.ASCII.GetBytes(archivoYCantPaquetes));
            String protocoloString = prot.ToString();
            byte[] bytes = Encoding.ASCII.GetBytes(protocoloString);
            return bytes;
        }

        public static byte[] CargarPaqueteDeArchivo(byte[] paqueteActual)
        {
            int largoTotal = 9 + paqueteActual.Length;
            string primeraParteProtocolo = "REQ07" + largoTotal.ToString("0000");
            byte[] primeraParteProtocoloBytes = Encoding.ASCII.GetBytes(primeraParteProtocolo);
            //Protocolo.Protocolo prot = new Protocolo.Protocolo("REQ", "7", 9 + paqueteActual.Length, paqueteActual);
            byte[] bytes = new byte[primeraParteProtocoloBytes.Length + paqueteActual.Length];
            primeraParteProtocoloBytes.CopyTo(bytes, 0);
            paqueteActual.CopyTo(bytes, primeraParteProtocoloBytes.Length);
            return bytes;
            //Este tiene que cargar el proximo paquete y enviarlo
        }

        public static byte[] SolicitarDescargarArchivo(string nombreArchivo)
        {
            string mensajeRet = "";

            //Verificamos que no exista un archivo con ese nombre
            if (archivosBajados == null)
                archivosBajados = new List<string>();

            lock (archivosBajados)
            {
                if (archivosBajados.Contains(nombreArchivo))
                {
                    mensajeRet = "ERROR";
                }
                else
                {
                    mensajeRet = "OK";
                    archivosBajados.Add(nombreArchivo);
                }
            }

            string mensaje = nombreArchivo + "#" + mensajeRet; 
            Protocolo.Protocolo prot = new Protocolo.Protocolo("REQ", "4", 9 + Encoding.ASCII.GetBytes(mensaje).Length, Encoding.ASCII.GetBytes(mensaje));
            string protocoloString = prot.ToString();
            byte[] bytes = Encoding.ASCII.GetBytes(protocoloString);
            return bytes;
        }

        public static byte[] DescargarPaqueteDeArchivo(string paqueteActual)
        {
            Protocolo.Protocolo prot = new Protocolo.Protocolo("REQ", "8", 9 + Encoding.ASCII.GetBytes(paqueteActual).Length, Encoding.ASCII.GetBytes(paqueteActual));
            string protocoloString = prot.ToString();
            byte[] bytes = Encoding.ASCII.GetBytes(protocoloString);
            return bytes;
        }

        public static void GuardarPaqueteDeArchivo(FileStream archivo, byte[] paquete, string nombreArchivo)
        {
            archivo.Write(paquete, 0, paquete.Length);
        }

        public static byte[] SincronizarServidor(string datosServidor)
        {
            Protocolo.Protocolo prot = new Protocolo.Protocolo("REQ", "5", 9 + Encoding.ASCII.GetBytes(datosServidor).Length, Encoding.ASCII.GetBytes(datosServidor));
            string protocoloString = prot.ToString();
            byte[] bytes = Encoding.ASCII.GetBytes(protocoloString);
            return bytes;
        }
    }
}
