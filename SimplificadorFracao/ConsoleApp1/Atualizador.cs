using AutoUpdaterDotNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimplificadorFracao
{
    internal static class Atualizador
    {
        public static void VerificarAtualizacao()
        {
            string servidor = LerXML.LerConfiguracao("CaminhoAtualizacao");
            if (servidor == null)
            {
                LogHelper.EscreverLog("VerificarAtualizacao(): Não foi possível ler o arquivo config.xml");
                return;
            }
            AutoUpdater.RunUpdateAsAdmin = false;
            AutoUpdater.Start(servidor);
        }
    }
}
