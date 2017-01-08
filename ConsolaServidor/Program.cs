using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DominioServidor;
using System.Xml;
using Servidor;

namespace ConsolaServidor
{
    class Program
    {
        static void Main(string[] args)
        {

            XmlDocument datosServer = new XmlDocument();

            //La ruta del documento XML permite rutas relativas 
            //respecto del ejecutable!

            datosServer.Load("../../DataServer.xml");

            XmlNodeList user = datosServer.GetElementsByTagName("servidor");
            string ip = "";
            int puerto = 0;
            foreach (XmlElement nodo in user)
            {

                XmlNodeList nIP =
                nodo.GetElementsByTagName("ip");

                XmlNodeList nPuerto =
                nodo.GetElementsByTagName("puerto");

                ip = nIP[0].InnerText;
                puerto = Int32.Parse(nPuerto[0].InnerText);


            }
            SistemaServidor.PublicarServicio();
            ConexionServidor.AceptarConexiones(ip,puerto);
        }
    }
}
