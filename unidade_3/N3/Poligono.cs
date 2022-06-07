using OpenTK.Graphics.OpenGL;
using CG_Biblioteca;
using System;
using System.Collections.Generic;

namespace gcgcg
{
    internal class Poligono : ObjetoGeometria
    {

        public Poligono(char rotulo, Objeto paiRef, List<Ponto4D> points) : base(rotulo, paiRef)
        {
            foreach (var point in points)
            {
                base.PontosAdicionar(point);
            }
        }

        public void AddPonto4D(Ponto4D pto)
        {
            base.PontosAdicionar(pto);
        }

        protected override void DesenharObjeto()
        {
            GL.Begin(base.PrimitivaTipo);

            foreach (var point in pontosLista)
            {
                GL.Vertex2(point.X, point.Y);
            }

            GL.End();
        }

        public double calcula_ti(Ponto4D pto_inicial, Ponto4D pto_final)
        {
            return ((pto_inicial.Y - pto_inicial.Y) / (pto_final.Y - pto_inicial.Y));
        }

        public double x_int(double x1, double x2, double ti)
        {
            return (x1 + (x2 - x1) * ti);
        }

        public bool estaDentro(Ponto4D pto)
        {
            for (int i = 0; i < pontosLista.Count + 1; i++)
            {
                Ponto4D pto_inicial = pontosLista[i];
                Ponto4D pto_final = null;
                
                if (i >= pontosLista.Count)
                    pto_final = this.pontosLista[0];
                else
                    pto_final = this.pontosLista[i + 1];

                double ti = calcula_ti(pto_inicial, pto_final);

                if (ti is < 0 or > 1)
                {
                    return false;
                }

                double xi = x_int(pto_inicial.X, pto_final.X, ti);
                
                // TODO
                return true;
            }

            return false;
        }
    }
}