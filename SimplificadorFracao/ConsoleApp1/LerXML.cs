using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SimplificadorFracao
{
    internal static class LerXML
    {
        public static string PathServidor { get; private set; }
        public static string PathAtualizacao { get; private set; }
        public static string PathLocalApp { get; private set; }

        private static string LerConfiguracao(string element)
        {
            try
            {
                string caminhoArquivoConfig = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.xml");

                if (FileHelper.FileExistsWithTimeout(caminhoArquivoConfig).Exists)
                {
                    XDocument doc = XDocument.Load(caminhoArquivoConfig);
                    return doc.Element("Configuracao").Element(element).Value;
                }
                else
                {
                    return null;
                } 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao ler o arquivo de configuração: {ex.Message}");
                return null;
            }
        }


        public static void CarregarConfig()
        {
            PathAtualizacao = LerConfiguracao("CaminhoAtualizacao");
            PathServidor = LerConfiguracao("servidor");
            PathLocalApp = AppDomain.CurrentDomain.BaseDirectory;
        }
    }
}
