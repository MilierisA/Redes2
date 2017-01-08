using Notificaciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServidorConciliacion
{
    [ServiceContract]
    public interface IServicioConciliacion
    {
        [OperationContract]
        List<ObjetoNotificacion> ObtenerEstadisticas (string nombreUsuario);
    }
}
