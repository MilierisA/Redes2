using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Notificaciones
{
    [Serializable]
    [DataContract]
    public class ObjetoNotificacion
    {
        [DataMember]
        public string usuario { get; set; }
        [DataMember]
        public string msg { get; set; }
        [DataMember]
        public int accion { get; set; } // 1 - Inicio de sesión, 2 - Descarga Archivo, 3 - Carga Archivo, 4 - Cierre de sesión

        public ObjetoNotificacion(string usu, string mensaje, int acc)
        {
            this.usuario = usu;
            this.msg = mensaje;
            this.accion = acc;
        }

        public ObjetoNotificacion()
        {

        }
    }
}
