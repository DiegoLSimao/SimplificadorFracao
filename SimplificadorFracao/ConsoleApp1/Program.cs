using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Diagnostics;

namespace Simplificador
{
    class Program
    {
        static bool Cabecalho = false;
        static bool Validado = false;
        static bool Loop = false;
        static string Metodo = string.Empty;
        static bool MenorErro = true;

        static int Numerador;
        static int Denominador;
        static double PercentualErro;

    
        static void Main(string[] args)
        {
            while (!Loop)
            {
                while (!Validado)
                {
                    EscreverCabecalho();
                    Definicao_Metodo();
                    EntradaDados();
                    if(Validado) 
                        RealizarCalculo();
                }// fim while validado

                GerenciadorLoop();       
                
            }// fim while loop

        }// fim main

        static void RealizarCalculo()
        {
            if (MenorErro)
                FracaoSimplificada_MenorErro(Numerador, Denominador, PercentualErro);
            else
                FracaoSimplificada_MaiorPrecisao(Numerador, Denominador);
        }

        static void FracaoSimplificada_MenorErro(int numerador, int denominador, double percentualErro)
        {
            Stopwatch sw = new Stopwatch();
            double valorOriginal = (double)Numerador / Denominador;
            bool encontrouSolucao = false;
            int MelhorNumerador = 0;
            int MelhorDenominador = 0;
            double MelhorErroPercentual = 0.0;

            List<int> MelhoresNumeradores = new List<int>();
            List<int> MelhoresDenominadores = new List<int>();
            List<double> MelhoresErroPercentual = new List<double>();

            sw.Start(); //Inicia contagem de tempo

            //*** Se entrada numerador e denominador for par simplifica antes do calculo de erro
            while ((numerador % 2 == 0) && (denominador % 2 == 0))
            {
                numerador /= 2;
                denominador /= 2;
            }//fim while


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
                    //*** Lista com todos os resultados dentro do percentual desejado
                    MelhoresNumeradores.Add(novoNumerador);
                    MelhoresDenominadores.Add(novoDenominador);
                    MelhoresErroPercentual.Add(erroPercentual);

                    //*** Armazena a ultima solução dentro do percentual desejado
                    encontrouSolucao = true;
                    MelhorNumerador = novoNumerador;
                    MelhorDenominador = novoDenominador;
                    MelhorErroPercentual = erroPercentual;

                    //percentualErro = erroPercentual;

                    //*** Se numerador for igual a 1 finaliza
                    if (novoNumerador == 1)
                        break;
                }
            }
            sw.Stop(); //Finaliza contagem de tempo
            Console.WriteLine("");
            //*** ultima solução encontrada
            Imprimir_Resultados(encontrouSolucao, numerador, denominador, sw, MelhorNumerador, MelhorDenominador, MelhorErroPercentual);


/* MENOR ERRO ENCONTRADO
            Console.WriteLine("");
            Console.Write("Menor erro percentual:");
            double menor = percentualErro;
            int i=0,j=0;
            foreach (var item in MelhoresErroPercentual)
            {
                if(item < menor)
                {
                    menor = item;
                    i = j;
                }
                j++;
            }

            if(MelhoresErroPercentual[i]< MelhorErroPercentual)
            {
                //*** Menor erro encontrado
                Imprimir_Resultados(encontrouSolucao, numerador, denominador, sw, MelhoresNumeradores[i], MelhoresDenominadores[i], MelhoresErroPercentual[i]);
            }
*/
        }
      
        static void EscreverCabecalho()
        {
            if (Cabecalho) 
                return;

            var ver = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Console.WriteLine("===============================================================");
            Console.WriteLine($"========== Simplificador Denigres de Fração - {ver} =========");
            Console.WriteLine("===============================================================");
            Console.WriteLine("");
            Cabecalho = true;
        }

        static void Definicao_Metodo()
        {
            //*** Pede qual método
            if (Metodo == string.Empty)
            {
                Console.WriteLine("Escolha o Método:");
                Console.WriteLine("1. Opção 1, Cálculo de Maior Precisão");
                Console.WriteLine("2. Opção 2, Cálculo de Menor Erro");
                Console.Write("Selecione: ");
                Metodo = Console.ReadLine();

                //*** Letra P ou p digitada seleciona o cálculo pelo método Maior precisão
                if (string.Equals(Metodo, "1"))
                {
                    Console.WriteLine("Método Maior Precisão selecionado!");
                    MenorErro = false;
                }
                else // qualquer outra coisa seleciona o metodo menor erro
                {
                    MenorErro = true;
                    Console.WriteLine("Método Menor Erro selecionado!");
                }
                Console.WriteLine("===============================================================");
                Console.WriteLine("");
            }
        }

        static void ValidacaoDados(string num, string den, string erro)
        {
            try
            {
                Numerador = Convert.ToInt32(num);
                Denominador = Convert.ToInt32(den);
                PercentualErro = Convert.ToDouble(erro);
                Check_EntradasBurras(Numerador, Denominador);
                Validado = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Mensagem: {ex.Message}");
                Console.WriteLine("REPITA A OPERAÇÃO!");
                Console.WriteLine("");
                Validado = false;
            }
        }

        static void EntradaDados()
        {
            string num, den, erro;

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
            ValidacaoDados(num, den, erro);
        }

        static void GerenciadorLoop()
        {
            Console.WriteLine("");
            Console.Write("Repetir Operação? (S/N/L/T):  ");
            string repetir = Console.ReadLine();
            if (string.Equals(repetir, "s") || string.Equals(repetir, "S"))
            {
                //*** Repetir operação
                Loop = false;
                Validado = false;
                Console.WriteLine("");
                Console.WriteLine("===============================================================");
            }
            else if (string.Equals(repetir, "l") || string.Equals(repetir, "L"))
            {
                //*** Limpar tela
                Loop = false;
                Validado = false;
                Console.Clear();
            }
            else if (string.Equals(repetir, "n") || string.Equals(repetir, "N"))
            {
                //*** Sair do sistema
                Environment.Exit(0);
            }
            else if (string.Equals(repetir, "t") || string.Equals(repetir, "T"))
            {
                //*** Trocar Metodo
                //*** Limpar tela
                Loop = false;
                Validado = false;
                Console.Clear();
                Metodo = string.Empty;
            }
            else
            {
                GerenciadorLoop();
            }
        }

        static void FracaoSimplificada_MaiorPrecisao(int numerador, int denominador)
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
                Console.WriteLine($"*** RESULTADO !!! [{sw.ElapsedMilliseconds}ms]***");
                Console.WriteLine($"Inc. Orig.: {(double)numerador / denominador}");
                Console.WriteLine($"Inc. Erro : {(double)MelhorNumerador / MelhorDenominador}");
                Console.WriteLine("");
                Console.WriteLine($"Fração simplificada: {MelhorNumerador}/{MelhorDenominador}");
                Console.WriteLine($"Erro percentual: {MelhorErroPercentual.ToString("0.0000")}%");
            }
            else
            {
                //*** Encontrou pelo menos uma solução
                Console.WriteLine($"*** MELHOR RESULTADO !!! [{sw.ElapsedMilliseconds}ms]***");
                Console.WriteLine($"Inc.: {(double)numerador / denominador}");
                Console.WriteLine("");
                Console.WriteLine($"Fração simplificada: {numerador}/{denominador}");
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
    }


   
}
