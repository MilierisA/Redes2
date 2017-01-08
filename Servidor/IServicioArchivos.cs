using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace Servidor
{
    [ServiceContract]
    public interface IServicioArchivos
    {
        [OperationContract]
        List<String> ListarArchivos();

        [OperationContract]
        bool EliminarArchivo(string nombreArchivo);
    }
}
