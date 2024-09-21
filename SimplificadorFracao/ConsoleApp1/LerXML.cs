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
        public static string LerConfiguracao(string element)
        {
            try
            {
                // Combine o diretório do executável com o nome do arquivo
                string caminhoArquivoConfig = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.xml");

                if (File.Exists(caminhoArquivoConfig))
                {
                    XDocument doc = XDocument.Load(caminhoArquivoConfig);
                    return doc.Element("Configuracao").Element(element).Value;
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao ler o arquivo de configuração: {ex.Message}");
                return null;
            }
        }
    }
}
