using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocolo
{
    public class Protocolo
    {
        private const int LargoHeader = 3;
        private const int LargoCmd = 2;
        private const int LargoMensaje = 4;
        private string Header { get; set; }
        private string Comando { get; set; }
        private string Largo { get; set; }
        private byte[] Datos { get; set; }

        public Protocolo(string header, string com, int largo, byte[] datos)
        {
            Header = header;
            Comando = com;
            Largo = largo.ToString("0000");
            Datos = datos;
        }

        public static Protocolo BytesAProtocolo(byte[] bytes)
        {
            byte[] todoMenosDatos = bytes.Take(9).ToArray();
            string todoMenosDatosString = Encoding.UTF8.GetString(todoMenosDatos).TrimEnd();
            string header = todoMenosDatosString.Substring(0, 3);
            string com = todoMenosDatosString.Substring(4, 1);
            int largo = Int32.Parse(todoMenosDatosString.Substring(5, 4));
            byte[] datos = bytes.Skip(9).ToArray();
            return new Protocolo(header, com, largo, datos);
        }

        public byte[] darBytes()
        {
            return Datos;
        }

        public string darComando()
        {
            return Comando;
        }

        public int DarLargo()
        {
            return Int32.Parse(Largo);
        }

        public override string ToString()
        {
            return this.Header.ToString() + "0" + this.Comando.ToString() + this.Largo.ToString() + Encoding.UTF8.GetString(Datos);
        }
    }

    public enum Header
    {
        RES,
        REQ
    }

    public enum Comando
    {
        ListaArchivos,
        CargaArchivo,
        DescargaArchivo,
        SyncArchivos
    }

}
