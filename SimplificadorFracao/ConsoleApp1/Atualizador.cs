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
            if(FileHelper.FileExistsWithTimeout(LerXML.PathAtualizacao).Exists)
            {
                AutoUpdater.RunUpdateAsAdmin = false;
                AutoUpdater.Start(LerXML.PathAtualizacao);
            }
            else
            {
                LogHelper.EscreverLog("VerificarAtualizacao(): Não foi possível verificar se tem atualização, o caminho do arquivo InfoAtualizada.xml não foi localizado.");
                return;
            }            
        }
    }
}
