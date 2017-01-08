using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servidor
{
    public class ServicioArchivos : IServicioArchivos
    {
        private const string directorioServidor = @"../../../Servidor/directorioServidor";
        public bool EliminarArchivo(string nombreArchivo)
        {
            List<string> archivos = ListarArchivos();
            nombreArchivo = directorioServidor + @"\" + nombreArchivo;

            if (!archivos.Contains(nombreArchivo))
            { //No existe el archivo
                return false;
            }

            File.Delete(archivos.FirstOrDefault(a => a == nombreArchivo));
            return true;
        }

        public List<string> ListarArchivos()
        {
            List<string> archivos = new List<string>();
            string directorio = directorioServidor;
            string[] fileEntries = Directory.GetFiles(directorio);
            foreach (var file in fileEntries)
            {
                archivos.Add(file);
            }
            return archivos;
        }
    }
}
