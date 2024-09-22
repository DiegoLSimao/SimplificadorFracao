using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimplificadorFracao
{
    internal class CheckResult
    {
        // Classe para o retorno com indicação de timeout
        public bool Exists { get; set; } // Indica se o arquivo/diretório existe
        public bool TimeoutOccurred { get; set; } // Indica se houve timeout
        public string LocalPath {get; set; } = null;
    }

    internal class FileHelper
    {
        public static CheckResult FileExistsWithTimeout(string path, int timeout=3000)
        {
            var fileExistsTask = Task.Run(() => File.Exists(path));

            // Aguarda a conclusão da tarefa ou até o timeout (em milissegundos)
            if (fileExistsTask.Wait(timeout))
            {
                var result = new CheckResult { Exists = fileExistsTask.Result, TimeoutOccurred = false };
                if(result.Exists)
                {
                    result.LocalPath = path;
                }
                return result; 
            }
            else
            {
                // Tempo limite estourado
                return new CheckResult { Exists = false, TimeoutOccurred = true, LocalPath = null };
            }
        }

        public static CheckResult DirectoryExistsWithTimeout(string path, int timeout=3000)
        {
            var directoryExistsTask = Task.Run(() => Directory.Exists(path));

            // Aguarda a conclusão da tarefa ou até o timeout (em milissegundos)
            if (directoryExistsTask.Wait(timeout))
            {
                var result = new CheckResult { Exists = directoryExistsTask.Result, TimeoutOccurred = false };
                if(result.Exists)
                {
                    result.LocalPath = path; 
                }
                return result;
            }
            else
            {
                // Tempo limite estourado
                return new CheckResult { Exists = false, TimeoutOccurred = true };
            }
        }






    }
}
