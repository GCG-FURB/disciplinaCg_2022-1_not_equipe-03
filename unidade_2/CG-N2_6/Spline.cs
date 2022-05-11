using OpenTK.Graphics.OpenGL;
using CG_Biblioteca;
using System;
using System.Collections.Generic;

namespace gcgcg
{
    internal class Spline : ObjetoGeometria
    {

        public Ponto4D pto1, pto2, pto3, pto4;
        public int quantidade_pontos;

        public Spline(char rotulo, Objeto paiRef, Ponto4D pto1, Ponto4D pto2, Ponto4D pto3, Ponto4D pto4, int quantidade_pontos) : base(rotulo, paiRef)
        {
            this.pto1 = pto1;
            this.pto2 = pto2;
            this.pto3 = pto3;
            this.pto4 = pto4;
            this.quantidade_pontos = quantidade_pontos;
        }

        protected override void DesenharObjeto()
        {
            var i = 1d / this.quantidade_pontos;
            var ponto_aux = pto1;

            for (double t = i; t <= 1.0000000001; t += i)
            {
                var pto1_pto2 = Calcular(pto1, pto2, t);
                var pto2_pto3 = Calcular(pto2, pto3, t);
                var pto3_pto4 = Calcular(pto3, pto4, t);

                var pto1_pto2_pto3 = Calcular(pto1_pto2, pto2_pto3, t);
                var pto2_pto3_pto4 = Calcular(pto2_pto3, pto3_pto4, t);

                var pto_final = Calcular(pto1_pto2_pto3, pto2_pto3_pto4, t);

                GL.Begin(PrimitiveType.Lines);
                GL.Vertex2(ponto_aux.X, ponto_aux.Y);
                GL.Vertex2(pto_final.X, pto_final.Y);
                GL.End();

                ponto_aux = pto_final;
            }
        }

        private Ponto4D Calcular(Ponto4D ptoA, Ponto4D ptoB, double t)
        {
            var pto = new Ponto4D();
            pto.X = ptoA.X + ((ptoB.X - ptoA.X) * t);
            pto.Y = ptoA.Y + ((ptoB.Y - ptoA.Y) * t);
            return pto;
        }
    }
}