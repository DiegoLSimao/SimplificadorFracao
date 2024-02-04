using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimplificadorFracao
{
    internal class Numero
    {
        public int Numerador;
        public int Denominador;
        public double CoeficienteLinear;
        public int MaximoDivisorComum;
        public int MinimoMultiploComum;
        public double ErroPercentual;

        //*** Construtores
        public Numero()
        {
            Numerador = 0;
            Denominador = 1;
            CoeficienteLinear = 0.0;
        }
        public Numero(int numerador, int denominador, double erroPercentual)
        {
            //*** Se entrada numerador e denominador for par simplifica
            while ((numerador % 2 == 0) && (denominador % 2 == 0))
            {
                numerador /= 2;
                denominador /= 2;
            }//fim while

            //***Atribui
            Numerador = numerador;
            Denominador = denominador;
            CoeficienteLinear = (double)numerador / (double)denominador;
            MaximoDivisorComum = CalcularMDC(numerador, denominador);
            MinimoMultiploComum = CalcularMMC(numerador, denominador, MaximoDivisorComum);
            ErroPercentual = erroPercentual;
        }

        private int CalcularMDC(int a, int b)
        {
            //*** Máximo Divisor Comum, ALgoritimo de euclides
            while (b != 0)
            {
                int temp = b;
                b = a % b;
                a = temp;
            }

            return a;

        }

        private int CalcularMMC(int a, int b, int mdc)
        {
            return Math.Abs(a * b) / mdc;
        }




    }
}
