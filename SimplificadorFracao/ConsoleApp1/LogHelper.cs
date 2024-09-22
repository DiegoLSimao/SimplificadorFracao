using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace SimplificadorFracao
{
    internal static class LogHelper
    {
        private static bool CriarArquivoLog(string path)
        {
            bool result = false;
            
            var FileCheck = FileHelper.FileExistsWithTimeout(path);
            if(FileCheck.TimeoutOccurred)
            {
                // não encontrou o arquivo dentro do tempo de 3 segundos
                result = false;
            }
            if (FileCheck.Exists)
            {
                //Confirmou que o arquivo sim existe dentro do tempo de 3 seg
                result = false;
            }
            else
            {
                //Confirmou que o arquivo não existe dentro do tempo de 3 seg
                // Criar Arquivo
                File.Create(path);
                result = true;
            }
            return result;

        }
        private static bool CriarPastaLog(string path)
        {
            bool result = false;

            if (path != null) // Conseguiu obter o caminho do servidor?
            {                     // Sim
                var DirectoryCheck = FileHelper.DirectoryExistsWithTimeout(path);
                if (DirectoryCheck.TimeoutOccurred)
                {
                    //Não encontrou a pasta dentro do tempo de 3 segundos
                    result = false;
                }
                if (DirectoryCheck.Exists)
                {
                    //Confirmou que a pasta sim existe dentro do tempo de 3 seg
                    result = false;
                }
                else
                {
                    //Confirmou que a pasta não existe dentro do tempo de 3 seg

                    //Criar Pasta
                    Directory.CreateDirectory(path);
                    result = true;
                }
            }//Ler arquivo de configuração

            return  result;
        }

        public static void CriarArquivosLog()
        {
            var pasta = "\\logs";
            var arquivo = $"\\{Environment.UserName}.txt";
            //Verifica se pasta no servidor existe, se não cria
            if (!FileHelper.DirectoryExistsWithTimeout(LerXML.PathServidor + pasta).Exists)
            {
                CriarPastaLog(LerXML.PathServidor + pasta);
                CriarArquivoLog(LerXML.PathServidor + pasta + arquivo);
            }

            //Verifica se pasta local se existe, se não cria
            if (!FileHelper.DirectoryExistsWithTimeout(LerXML.PathLocalApp + pasta).Exists)
            {
                CriarPastaLog(LerXML.PathLocalApp + pasta);
                CriarArquivoLog(LerXML.PathLocalApp + pasta + arquivo);
            }
        }

        public static void EscreverLog(string message)
        {
            var pasta = @"\logs";
            var arquivo = @"\" + $"{Environment.UserName}.txt";

            // Caminho do servidor existe
            if (FileHelper.DirectoryExistsWithTimeout(LerXML.PathServidor+pasta).Exists)
            {
                string pathservidor = LerXML.PathServidor + pasta + arquivo;
                if (FileHelper.FileExistsWithTimeout(pathservidor).Exists)
                    Escrever(pathservidor, message);
            }

            // Caminho local existe
            if (FileHelper.DirectoryExistsWithTimeout(LerXML.PathLocalApp+pasta).Exists)
            {
                string pathlocal = LerXML.PathLocalApp + pasta + arquivo;
                if (FileHelper.FileExistsWithTimeout(pathlocal).Exists)
                    Escrever(pathlocal, message);
            }
        }

        private static void Escrever(string path,string message)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(path, true))
                {
                    writer.WriteLine($"{DateTime.Now:dd-MM-yyyy HH:mm:ss} - {message}");
                }
            }
            catch
            {
            }
        }
    }
}
