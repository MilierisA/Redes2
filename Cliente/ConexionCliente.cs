using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Linq;
using System.Messaging;
using Notificaciones;

namespace DominioCliente
{
    public class ConexionCliente
    {

        static List<string> ComandosValidos = new List<string>() { "2", "3", "4", "5","6" };
        private const string directorioCliente = @"../../../Cliente/directorioCliente";

        public static void ConectarseAlServidor()
        {
            bool isRunning = true;
            bool loggedIn = false;

            XmlDocument datosServer = new XmlDocument();

            //La ruta del documento XML permite rutas relativas 
            //respecto del ejecutable!

            datosServer.Load("../../../Cliente/DataServer.xml");

            XmlNodeList servidor = datosServer.GetElementsByTagName("usuario");
            string ipServer = "";
            string ipCliente = "";
            int puerto = 0;
            foreach (XmlElement nodo in servidor)
            {

                XmlNodeList nIPCliente = nodo.GetElementsByTagName("ipCliente");

                XmlNodeList nIPServer = nodo.GetElementsByTagName("ipServidor");

                XmlNodeList nPuerto = nodo.GetElementsByTagName("puerto");

                ipCliente = nIPCliente[0].InnerText;
                ipServer = nIPServer[0].InnerText;
                puerto = Int32.Parse(nPuerto[0].InnerText);
            }


            try
            {
                IPEndPoint clientEndPoint =
    new IPEndPoint(IPAddress.Parse(ipCliente), puerto + 1);
                IPEndPoint serverEndPoint =
                    new IPEndPoint(IPAddress.Parse(ipServer), puerto);
                Socket socket = new Socket(AddressFamily.InterNetwork,
                                            SocketType.Stream,
                                            ProtocolType.Tcp);

                socket.Bind(clientEndPoint);
                socket.Connect(serverEndPoint);
                Console.WriteLine("Conectado al servidor");
                Console.WriteLine("Ingrese un nombre para identificar al cliente conectado");
                while (!loggedIn)
                {
                    string nombreCliente = Console.ReadLine();
                    byte[] dataLogin = SistemaCliente.ProcesarPedido("1", nombreCliente);
                    socket.Send(dataLogin);
                    byte[] loginResponse = new byte[256];
                    int response = socket.Receive(loginResponse);
                    loggedIn = Boolean.Parse(SistemaCliente.ProcesarRespuesta(loginResponse).Split('#')[1]);
                    if (!loggedIn)
                    {
                        Console.WriteLine("Nombre de cliente inválido o ya se ha iniciado una sesion con el nombre " + nombreCliente);
                    }
                    else
                    {
                        MessageQueue clientQueue = SistemaCliente.GetMsgQueue();
                        ObjetoNotificacion notificacion = new ObjetoNotificacion(nombreCliente, "Se ha conectado el cliente " + nombreCliente, 1);
                        clientQueue.Send(notificacion);
                        Console.WriteLine("Cliente conectado con éxito!");
                        while (isRunning)
                        {
                            ImprimirMenu();
                            string comando = Console.ReadLine();
                            if (!ComandosValidos.Contains(comando))
                            {
                                Console.WriteLine("Error. Ingrese comando válido");
                            }
                            else
                            {
                                if (comando == "2")
                                {
                                    byte[] data = SistemaCliente.ProcesarPedido(comando, "");
                                    socket.Send(data);
                                    byte[] dataResponse = new byte[9999];
                                    int i = socket.Receive(dataResponse);
                                    string listaArchivos = SistemaCliente.ProcesarRespuesta(dataResponse);
                                    listaArchivos = listaArchivos.Replace("#", "\n");
                                    Console.WriteLine("Lista de archivos");
                                    Console.WriteLine(listaArchivos);
                                }
                                if (comando == "3")
                                {
                                    Console.WriteLine("Ingrese el nombre del archivo a subir");
                                    string nombreArchivo = Console.ReadLine();
                                    byte[] data = SistemaCliente.ProcesarPedido(comando, nombreArchivo);
                                    int cantPaquetesAMandar = Int16.Parse(Encoding.UTF8.GetString(data).Split('#')[1]);

                                    if (cantPaquetesAMandar == 0)
                                    { //No se encontro el archivo
                                        Console.WriteLine("Error. No se encontro el archivo " + nombreArchivo + " en el directorio del cliente");
                                    }
                                    else
                                    {
                                        socket.Send(data);
                                        byte[] dataResponse = new byte[9999];
                                        int i = socket.Receive(dataResponse);
                                        string estadoServidor = SistemaCliente.ProcesarRespuesta(dataResponse).TrimEnd().Replace("\0", string.Empty);
                                        if (estadoServidor == "OK")
                                        {
                                            string rutaArchivo = directorioCliente + "\\" + nombreArchivo;
                                            FileStream fs = new FileStream(rutaArchivo, FileMode.Open);
                                            //El servidor esta listo para que le suba el archivo
                                            int paqueteActual = 1;
                                            while (paqueteActual <= cantPaquetesAMandar)
                                            {
                                                //var stream = new BinaryReader(new StreamReader(rutaArchivo).BaseStream);
                                                int bytesPorDefecto = 9990;
                                                if (paqueteActual == cantPaquetesAMandar)
                                                {
                                                    var largo = fs.Length;
                                                    bytesPorDefecto = (int)largo - (9990 * (paqueteActual - 1));
                                                }
                                                byte[] dataParaEnviar = new byte[bytesPorDefecto];
                                                int iDataRead = fs.Read(dataParaEnviar, 0, bytesPorDefecto);
                                                byte[] dataProtocolo = SistemaCliente.CargarPaqueteDeArchivo(dataParaEnviar);
                                                socket.Send(dataProtocolo);
                                                Console.WriteLine("Subiendo " + nombreArchivo + ". Paquete " + paqueteActual + " de " + cantPaquetesAMandar);
                                                dataResponse = new byte[256];
                                                i = socket.Receive(dataResponse);
                                                estadoServidor = SistemaCliente.ProcesarRespuesta(dataResponse).TrimEnd().Replace("\0", string.Empty);
                                                if (estadoServidor == "OK")
                                                {
                                                    i++; //Else...hacer algo
                                                    paqueteActual++;
                                                }

                                            }
                                            notificacion = new ObjetoNotificacion(nombreCliente, "El cliente " + nombreCliente + " ha cargado el archivo " + nombreArchivo, 3);
                                            clientQueue.Send(notificacion);
                                            fs.Close();
                                        }

                                        else
                                        {
                                            Console.WriteLine("ERROR. Ya se ha subido un archivo con el nombre " + nombreArchivo);
                                        }
                                    }
                                }
                                if (comando == "4")
                                {
                                    Console.WriteLine("Ingrese el nombre del archivo a descargar");
                                    string nombreArchivo = Console.ReadLine();
                                    byte[] dataRequest = SistemaCliente.ProcesarPedido(comando, nombreArchivo);
                                    string datos = Encoding.UTF8.GetString(SistemaCliente.getDatos(dataRequest));
                                    string mensajeConfirmacion = datos.Split('#')[1];
                                    if (mensajeConfirmacion == "ERROR")
                                    {
                                        Console.WriteLine("ERROR. Ya se ha descargado un archivo con el nombre " + nombreArchivo);
                                    }
                                    else {
                                        socket.Send(dataRequest);
                                        byte[] dataResponse = new byte[9999];
                                        int i = socket.Receive(dataResponse);
                                        int cantPaquetes = Int16.Parse(SistemaCliente.ProcesarRespuesta(dataResponse).TrimEnd().Replace("\0", string.Empty).Split('#')[1]);
                                        int paqueteActual = 1;
                                        if (cantPaquetes == 0)
                                        {
                                            Console.WriteLine("No es posible descargar el archivo indicado.");
                                        }
                                        else
                                        {
                                            FileStream fileStream = new FileStream(directorioCliente + "\\" + nombreArchivo, FileMode.Create, FileAccess.Write);
                                            while (paqueteActual <= cantPaquetes)
                                            {
                                                byte[] requestPaquete = SistemaCliente.ProcesarPedido("8", paqueteActual.ToString());
                                                socket.Send(requestPaquete);
                                                byte[] responsePaquete = new byte[9999];
                                                int j = socket.Receive(responsePaquete);
                                                Console.WriteLine("Recibiendo " + nombreArchivo + ". Paquete " + paqueteActual + " de " + cantPaquetes);
                                                int largoPaquete = SistemaCliente.darLargo(responsePaquete) - 9;
                                                byte[] PaqueteArchivo = SistemaCliente.getDatos(responsePaquete);
                                                if (paqueteActual == cantPaquetes)
                                                    PaqueteArchivo = PaqueteArchivo.Take(largoPaquete).ToArray();
                                                SistemaCliente.GuardarPaqueteDeArchivo(fileStream, PaqueteArchivo, nombreArchivo);
                                                //clientQueue.Send("El cliente " + nombreCliente + " ha descargado el paquete " + paqueteActual + " de " + cantPaquetes + " del archivo " + nombreArchivo);
                                                paqueteActual++;
                                            }
                                            notificacion = new ObjetoNotificacion(nombreCliente, "El cliente " + nombreCliente + " ha descargado completamente el archivo " + nombreArchivo, 2);
                                            clientQueue.Send(notificacion);
                                            fileStream.Close();
                                        }
                                    }
                                }
                                if (comando == "5")
                                {
                                    Console.WriteLine("Ingrese la ip del servidor a sincronizar");
                                    string ipServ = Console.ReadLine();
                                    Console.WriteLine("Ingrese el puerto del servidor a sincronizar");
                                    string puertoServidor = Console.ReadLine();
                                    byte[] dataRequest = SistemaCliente.ProcesarPedido(comando, ipServ + "#" + puertoServidor);
                                    socket.Send(dataRequest);
                                    byte[] dataResponse = new byte[9999];
                                    int i = socket.Receive(dataResponse);
                                    SistemaCliente.ProcesarRespuesta(dataResponse).TrimEnd().Replace("\0", string.Empty);
                                    Console.WriteLine("Sincronizacion finalizada");
                                }

                                if (comando == "6")
                                {
                                    byte[] dataLogout = SistemaCliente.ProcesarPedido(comando);
                                    socket.Send(dataLogout);
                                    notificacion = new ObjetoNotificacion(nombreCliente, "El cliente " + nombreCliente +" ha cerrado la sesion", 4);
                                    clientQueue.Send(notificacion);
                                    isRunning = false;
                                    socket.Shutdown(SocketShutdown.Both);
                                    socket.Close();
                                }
                            }

                        }
                    }
                }

            }
            catch (SocketException ex)
            {
                Console.WriteLine("El servidor cerró la conexión");
                Console.ReadLine();
            }
            catch (FormatException e2)
            {
                Console.WriteLine("Error en la configuracion de ip");
                Console.ReadLine();
            }
        }

        public static void ImprimirMenu()
        {
            Console.WriteLine("Ingrese la accion a realizar");
            Console.WriteLine("2: Solicitar lista de archivos");
            Console.WriteLine("3: Solicitar carga de archivo");
            Console.WriteLine("4: Solicitar descarga de archivo");
            Console.WriteLine("5: Sincronizar archivos con otro servidor");
            Console.WriteLine("6: Cerrar sesion");
        }
    }
}
