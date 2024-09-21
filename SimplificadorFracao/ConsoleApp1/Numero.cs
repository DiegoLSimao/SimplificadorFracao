using System;

namespace SimplificadorFracao
{
    internal class Numero
    {
        public uint Numerador;
        public uint Denominador;
        public double CoeficienteLinear;
        public uint MaximoDivisorComum;
        public uint MinimoMultiploComum;
        public double ErroPercentual;
        

        //*** Construtores
        public Numero()
        {
            Numerador = 0;
            Denominador = 1;
            CoeficienteLinear = 0.0;
        }
        public Numero(uint numerador, uint denominador, double erroPercentual)
        {
            //***Atribui
            Numerador = numerador;
            Denominador = denominador;
            CoeficienteLinear = (double)numerador / (double)denominador;
            MaximoDivisorComum = CalcularMDC(numerador, denominador);
            MinimoMultiploComum = CalcularMMC(numerador, denominador, MaximoDivisorComum);
            ErroPercentual = erroPercentual;
        }

        private uint CalcularMDC(uint a, uint b)
        {
            //*** Máximo Divisor Comum, ALgoritimo de euclides
            while (b != 0)
            {
                uint temp = b;
                b = a % b;
                a = temp;
            }

            return a;

        }

        private uint CalcularMMC(uint a, uint b, uint mdc)
        {
            return (uint)Math.Abs(a * b) / mdc;
        }

    }
}
