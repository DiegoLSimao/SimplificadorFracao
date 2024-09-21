using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimplificadorFracao
{
    internal static class LogHelper
    {
        public static void CriarArquivoLog()
        {
            string servidor = LerXML.LerConfiguracao("servidor");
            string logLocal = AppDomain.CurrentDomain.BaseDirectory + "logs";
            string ServidorLog = servidor + $"\\logs";

            if (servidor != null)
            {
                if (!Directory.Exists(ServidorLog))
                {
                    //Criar Pasta
                    Directory.CreateDirectory(ServidorLog);

                    //Criar Arquivo
                    var user = Environment.UserName;
                    string arquivolog = servidor + $"\\logs\\{Environment.UserName}.txt";
                    if (!File.Exists(arquivolog))
                    {
                        // Arquivo não existe
                        File.Create(arquivolog);
                    }
                }
            }

            //Cria pasta local 
            if (!Directory.Exists(logLocal))
            {
                //Criar Pasta
                Directory.CreateDirectory(logLocal);

                //Criar Arquivo
                var user = Environment.UserName;
                string arquivolog = logLocal + $"\\{Environment.UserName}.txt";
                if (!File.Exists(arquivolog))
                {
                    // Arquivo não existe
                    File.Create(arquivolog);
                }
            }
        }

        public static void EscreverLog(string message)
        {
            string local = AppDomain.CurrentDomain.BaseDirectory;
            string servidor = LerXML.LerConfiguracao("servidor");


            string logServidor = servidor + $"\\logs\\{Environment.UserName}.txt";
            string logLocal = local + $"logs\\{Environment.UserName}.txt";
            try
            {
                //log local
                using (StreamWriter writer = new StreamWriter(logLocal, true))
                {
                    writer.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
                }

                //Log servidor
                if (servidor != null)
                {
                    using (StreamWriter writer = new StreamWriter(logServidor, true))
                    {
                        writer.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
                    }
                }
            }
            catch
            {
            }

        }
    }
}
