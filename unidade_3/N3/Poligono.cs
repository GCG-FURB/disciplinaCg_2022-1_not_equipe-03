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
        
    }
}