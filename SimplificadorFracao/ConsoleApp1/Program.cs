using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Diagnostics;
using AutoUpdaterDotNET;
using System.IO;
using System.Xml.Linq;
using SimplificadorFracao;

namespace Simplificador
{
    class Program
    {
        //todo implementar atualizador automático

        static bool Cabecalho = false;
        static bool Loop = false;
        static string Metodo = string.Empty;
        static bool MenorErro = true;
        static byte Estado = 0;

        static int Numerador;
        static int Denominador;
        static double PercentualErro;

    
        static void Main(string[] args)
        {
#if !DEBUG
            VerificarAtualizacao();
#endif
            while (!Loop)
            {
                switch (Estado)
                {
                    case 0:
                        {
                            if (!Cabecalho) EscreverCabecalho();
                            Estado = 1;
                            break;
                        }

                    case 1:
                        {
                            var MetodoEscolhido = Definicao_Metodo();
                            if (MetodoEscolhido == 3 || MetodoEscolhido == 6) Estado = 0;
                            else if (MetodoEscolhido == 4) return;
                            else if (MetodoEscolhido == 5) break;
                            else Estado = 2;
                            break;
                        }

                    case 2:
                        {
                            var validado = EntradaDados();
                            if (validado) Estado = 3;
                            else Estado = 2;
                            break;
                        }

                    case 3:
                        {
                            RealizarCalculo();
                            Estado = 4;
                            break;
                        }

                    case 4:
                        {
                            GerenciadorLoop();
                            Estado = 0;
                            break;
                        }

                    default:
                        {
                            Console.WriteLine($"Estado={Estado} --> ERRO GRAVE, Tem que tá vendo isso!");
                            Console.ReadLine();
                            Estado = 0;
                            break;
                        }
                };                 
            }// fim while loop

        }// fim main

        private static void VerificarAtualizacao()
        {
            try
            {
                string servidor = string.Concat("file://", LerConfigAtualizacao());
                AutoUpdater.RunUpdateAsAdmin = false;
                AutoUpdater.Start(servidor);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Não foi possível verificar atualização!!!\r\n{ex.Message}");
            }
           
        }

        private static string LerConfigAtualizacao()
        {
            try
            {
                // Combine o diretório do executável com o nome do arquivo
                string caminhoArquivoConfig = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.xml");

                XDocument doc = XDocument.Load(caminhoArquivoConfig);
                return doc.Element("Configuracao").Element("CaminhoAtualizacao").Value;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao ler o arquivo de configuração: {ex.Message}");
                return string.Empty;
            }
        }

        static void RealizarCalculo()
        {
            if (MenorErro)
                FracaoSimplificada_MenorErro(Numerador, Denominador, PercentualErro);
            else
                FracaoSimplificada_MenorFracao(Numerador, Denominador);
        }


        static void FracaoSimplificada_MenorErro(int numerador, int denominador, double percentualErro)
        {
            double valorOriginal = (double)Numerador / (double)Denominador;
            bool encontrouSolucao = false;
            bool exibiuMelhorSolucao = false;
            int MelhorNumerador = 0;
            int MelhorDenominador = 0;
            double MelhorErroPercentual = 0.0;
            List<Numero> listNumeros = new List<Numero>();


            #region PROCESSAMENTO DO ERRO
            //*** Processamento de erro
            for (int divisor = 2; divisor <= denominador; divisor++)
            {
                if (numerador == 1)
                    break;

                int novoNumerador = numerador / divisor;
                int novoDenominador = denominador / divisor;
                double novoValor = (double)novoNumerador / novoDenominador;
                double erroPercentual = Math.Abs((novoValor - valorOriginal) / valorOriginal) * 100;


                if (erroPercentual <= percentualErro)
                {
                    encontrouSolucao = true;

                    //*** Lista com todos os resultados dentro do percentual desejado
                    Numero num = new Numero(novoNumerador, novoDenominador, erroPercentual);
                    listNumeros.Add(num);

                    //*** Se numerador for igual a 1 finaliza
                    if (novoNumerador == 1)
                        break;
                }
            }
            #endregion


            //***Se possuir mais de uma solução faz os cálculos de menor numerador e denominador e menor erro percentual
            if ((encontrouSolucao==true) && (listNumeros.Count > 0))
            {
                #region CALCULO MENOR NUMERADOR E DENOMINADOR
                int menorNumDen = 0;
                //***Encontra menor Numerador E menor Denominador
                for (int i = 0; i < listNumeros.Count; i++)
                {
                    if ((listNumeros[i].Numerador < 65536) && (listNumeros[i].Denominador < 65536))
                    {
                        menorNumDen = i; // salva o indice do melhor resultado
                    }
                }
                #endregion

                #region CALCULO DO MENOR ERRO PERCENTUAL
                double menorErro = percentualErro;
                int menorPercent = 0;

                //*** Encontra menor erro percentual na lista
                for (int i = 0; i < listNumeros.Count; i++)
                {
                    if ((listNumeros[i].Numerador < 65536) && (listNumeros[i].Denominador < 65536))
                    {
                        if (listNumeros[i].ErroPercentual < menorErro)
                        {
                            menorErro = listNumeros[i].ErroPercentual;
                            menorPercent = i; // salva o indice do melhor resultado
                        }
                    }
                }
                #endregion

                #region EXIBIÇÃO PARA O USUÁRIO
                bool ExibirUnico = (listNumeros[menorNumDen].ErroPercentual == listNumeros[menorPercent].ErroPercentual);

                if(ExibirUnico)
                {
                    Console.WriteLine("");
                    Console.WriteLine("");
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write($"---------------------------------------------");
                    Imprimir_Resultados(encontrouSolucao, numerador, denominador, null, listNumeros[menorNumDen].Numerador, listNumeros[menorNumDen].Denominador, listNumeros[menorNumDen].ErroPercentual);
                }
                else
                {
                    //Menor numerador e Denominador
                    Console.WriteLine("");
                    Console.WriteLine("");
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($"---------------------------------------------");
                    Console.Write($"------- MENOR NUMERADOR E DENOMINADOR -------");
                    Imprimir_Resultados(encontrouSolucao, numerador, denominador, null, listNumeros[menorNumDen].Numerador, listNumeros[menorNumDen].Denominador, listNumeros[menorNumDen].ErroPercentual);


                    //Menor Erro Percentual
                    Console.WriteLine("");
                    Console.WriteLine("");
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($"---------------------------------------------");
                    Console.Write($"---------- MENOR ERRO PERCENTUAL ------------");
                    Imprimir_Resultados(encontrouSolucao, numerador, denominador, null, listNumeros[menorPercent].Numerador, listNumeros[menorPercent].Denominador, listNumeros[menorPercent].ErroPercentual);

                }
                #endregion

            }
            else
            {
                //*** Se não encontrou uma solução apresenta o valor informado somente
                Console.WriteLine("");
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write($"---------------------------------------------");
                Imprimir_Resultados(encontrouSolucao, numerador, denominador, null, MelhorNumerador, MelhorDenominador, MelhorErroPercentual);
            }

            //*** Limpa lista para o Garbage Colector
            listNumeros.Clear();
            listNumeros = null;
        }
      
        static void EscreverCabecalho()
        {
            var ver = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Console.WriteLine("===============================================================");
            Console.WriteLine($"==================== SimpliFração - {ver} ===================");
            Console.WriteLine("===============================================================");
            Console.WriteLine("");
            Cabecalho = true;
        }

        static byte Definicao_Metodo()
        {
            byte ret = 0;
            //*** Pede qual método
            if (Metodo == string.Empty)
            {
                Console.WriteLine("Escolha o Método:");
                Console.WriteLine("\t1. Opção 1, Cálculo de Menor Fração");
                Console.WriteLine("\t2. Opção 2, Cálculo de Menor Erro Admitido.");
                Console.WriteLine("\t3. Opção 3, Ajuda.");
                Console.WriteLine("\t4. Opção 4, Fechar.");
                Console.Write("Selecione: ");
                Metodo = Console.ReadLine();

                //*** Método maior precisão
                if (string.Equals(Metodo, "1"))
                {
                    Console.WriteLine("Método Maior Precisão selecionado!");
                    MenorErro = false;
                    Console.WriteLine("===============================================================");
                    Console.WriteLine("");
                    ret= 1;
                }
                //*** Método menor erro
                else if (string.Equals(Metodo, "2"))
                {
                    MenorErro = true;
                    Console.WriteLine("Método Menor Erro selecionado!");
                    Console.WriteLine("===============================================================");
                    Console.WriteLine("");
                    ret = 2;
                }
                //*** Ajuda com informações
                else if (string.Equals(Metodo, "3"))
                {
                    Informacoes();
                    ret = 3;
                }
                //*** Sair da aplicação
                else if (string.Equals(Metodo, "4"))
                {
                    ret = 4;
                }
                //*** Sobre
                else if (string.Equals(Metodo, "?"))
                {
                    Sobre();
                    ret = 6;
                }
                else //Recomeçar, usuário não ecolheu uma opção válida
                {
                    //*** Escrever em vermelho
                    Console.ForegroundColor = ConsoleColor.Red;

                    Console.WriteLine("Opção inválida !!!\r\n");
                    Metodo = string.Empty;
                    ret = 5;
                    // Restaurar a cor padrão após a impressão
                    Console.ResetColor();
                }                
            }
            return ret;
        }

        static bool ValidacaoDados(string num, string den, string erro)
        {
            bool ret;
            try
            {
                Numerador = Convert.ToInt32(num);
                Denominador = Convert.ToInt32(den);
                PercentualErro = Convert.ToDouble(erro);
                Check_EntradasBurras(Numerador, Denominador);

                //*** Se entrada numerador e denominador for par simplifica
                while ((Numerador % 2 == 0) && (Denominador % 2 == 0))
                {
                    Numerador /= 2;
                    Denominador /= 2;
                }//fim while

                ret = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Mensagem: {ex.Message}");
                Console.WriteLine("REPITA A OPERAÇÃO!");
                Console.WriteLine("");
                ret = false;
            }

            return ret;
        }

        static bool EntradaDados()
        {
            string num, den, erro;

            //todo implementar operação quando o sinal de igual for digitado Ex: =44+55 assumiria Numerador = 99

            Console.Write("Numerador: ");
            num = Console.ReadLine();
            Console.Write("Denominador: ");
            den = Console.ReadLine();

            if(MenorErro)
            {
                Console.Write("Erro Admitido (%): ");
                erro = Console.ReadLine();
            }
            else
            {
                erro = "0";
            }
            return ValidacaoDados(num, den, erro);
        }

        static void GerenciadorLoop()
        {
            Console.WriteLine("");
            Console.Write("Repetir Operação? Tecle: ([S]im / [N]ão / [L]impar Tela / [T]rocar Método) / [F]echar: ");
            string repetir = Console.ReadLine();
            if (string.Equals(repetir, "s") || string.Equals(repetir, "S"))
            {
                //*** Repetir operação
                Loop = false;
                Console.WriteLine("");
                Console.WriteLine("===============================================================");
            }
            else if (string.Equals(repetir, "l") || string.Equals(repetir, "L"))
            {
                //*** Limpar tela
                Loop = false;
                Console.Clear();
            }
            else if (string.Equals(repetir, "n") || string.Equals(repetir, "N"))
            {
                Cabecalho =false;
                Metodo = string.Empty;
            }
            else if (string.Equals(repetir, "f") || string.Equals(repetir, "F"))
            {
                //*** Fechar aplicação
                Environment.Exit(0);
            }
            else if (string.Equals(repetir, "t") || string.Equals(repetir, "T"))
            {
                //*** Trocar Metodo
                //*** Limpar tela
                Loop = false;
                Metodo = string.Empty;
                Console.WriteLine("===============================================================");
            }
            else
            {
                GerenciadorLoop();
            }
        }

        static void FracaoSimplificada_MenorFracao(int numerador, int denominador)
        {
            Stopwatch sw = new Stopwatch();
            double valorOriginal = (double)numerador / denominador;
            bool encontrouSolucao = false;
            int MelhorNumerador = 0;
            int MelhorDenominador = 0;
            double MelhorErroPercentual = 100.0;


            sw.Start(); //Inicia contagem de tempo

            //*** Cálculo principal
            for (int divisor = 2; divisor <= denominador; divisor++)
            {
                int novoNumerador = numerador / divisor;
                int novoDenominador = denominador / divisor;
                double novoValor = (double)novoNumerador / novoDenominador;

                double erroPercentual = Math.Abs((novoValor - valorOriginal) / valorOriginal) * 100;

                if (erroPercentual <= MelhorErroPercentual && novoNumerador <= 65535)
                { 
                    encontrouSolucao = true;
                    MelhorNumerador = novoNumerador;
                    MelhorDenominador = novoDenominador;
                    MelhorErroPercentual = erroPercentual;

                    //*** Se novo numerador for igual a 1 finaliza
                    if (novoNumerador == 1)
                     break;
                }
            }

            sw.Stop(); // Finaliza contagem de tempo
            Imprimir_Resultados(encontrouSolucao, numerador ,denominador, sw, MelhorNumerador, MelhorDenominador, MelhorErroPercentual);

        }

        static void Imprimir_Resultados(bool encontrouSolucao, int numerador, int denominador, Stopwatch sw, int MelhorNumerador, int MelhorDenominador,double MelhorErroPercentual)
        {
            Console.WriteLine("");
            if (encontrouSolucao)
            {
                //*** Encontrou pelo menos uma solução

                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"------------ RESULTADO CALCULADO ------------");
                
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Inc. Orig.: {(double)numerador / denominador}");
                Console.WriteLine($"Inc. Erro : {(double)MelhorNumerador / MelhorDenominador}");
                Console.WriteLine($"Erro percentual: {MelhorErroPercentual.ToString("0.0000")}%");
                Console.WriteLine("");
                Console.WriteLine($"Fração simplificada: {MelhorNumerador}/{MelhorDenominador}");

                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"--------------------------------------------");
                Console.ResetColor();

            }
            else
            {
                //*** Encontrou pelo menos uma solução
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"------------ MELHOR RESULTADO ------------");

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Inc.: {(double)numerador / denominador}");
                Console.WriteLine("");
                Console.WriteLine($"Fração simplificada: {numerador}/{denominador}");

                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"--------------------------------------------");
                Console.ResetColor();
            }
        }

        static void Check_EntradasBurras(int numerador, int denominador)
        {
            if (numerador == 1)     throw new Exception("Sem Simplificação! Numerador = 1.");
            if (numerador == 0)     throw new Exception("Sem Simplificação! Numerador = 0, Resultado Sempre Zero!");
            if (denominador == 1)   throw new Exception("Sem Simplificação! Denominador = 1, Resultado é o Próprio Numerador!");
            if (denominador == 0)   throw new Exception("Primeiro Mandamento da Matemática: \"Não Dividirás Por Zero\"");
            if (denominador == numerador) throw new Exception("Numerador Igual ao Denominador, Resultado é 1");
        }
 
        static void Informacoes()
        {
            Console.Clear();

            //*** informações
            Console.WriteLine("Opção 1, Menor Fração:\r\nO usuário deve informar o numerador e denominador da fração e o sistema\r\nirá simplificar e encontrar a menor fração mais precisa possível.\r\nO usuário deve analizar o percentual de erro e decidir se faz sentido utilizar ou não!\r\n\r\n");
            Console.WriteLine("Opção 2, Menor Erro Admitido:\r\nO usuário deve informar o numerador e o denominador da fração\r\nem seguida o percentual de erro admitido (Aceita casas decimais). O sistema irá calcular\r\na menor fração dentro do percentual de erro admitido.\r\n\r\n");
            Console.WriteLine("Obs.: Numerador e denominador obrigatóriamente deve ser números inteiro.\r\nPercentual aceita casas decimais no método Menor Erro.\r\n\r\n");
            Console.WriteLine("No Resultado será apresentado a inclinação da reta original e a inclinação da reta considerando o erro percentual.\r\nA fração será aprensetada no formato [a/b], onde [a] é o numerador (multiplica) e [b] é o denominador (divide).\r\n\r\n");
            
            Console.WriteLine("Presione Enter para continuar!");
            Console.ReadLine();

            //*** Limpar tela
            Metodo = string.Empty;
            Console.Clear();
        }

        static void Sobre()
        {
            var user = Environment.UserName;
            var maquina = Environment.MachineName;
            var diretorio = Environment.CurrentDirectory;
            var os = Environment.OSVersion.ToString();
            var programador = "Eng. Diego de Lima Simão [simaold@gmail.com]";
            Version versionAss = Assembly.GetEntryAssembly().GetName().Version;


            Console.Clear();
            Console.WriteLine("---------- SimpliFração ----------\r\n");

            var mensagem = $"Usuário: {user} \n\nMáquina: {maquina} \n\nCaminho: {diretorio}\n\nOS: {os} \n\nVersão Assembly: {versionAss}\n\nProgramador: {programador}";
            Console.WriteLine(mensagem);

            Console.WriteLine("Repositório: https://github.com/DiegoLSimao/SimplificadorFracao");

            Console.WriteLine("\r\nPresione Enter para continuar!");
            Console.ReadLine();

            

            //*** Limpar tela
            Metodo = string.Empty;
            Console.Clear();
        }
    }


   
}
