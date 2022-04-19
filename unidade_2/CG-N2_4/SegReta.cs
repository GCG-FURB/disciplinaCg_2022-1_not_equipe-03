using OpenTK.Graphics.OpenGL;
using CG_Biblioteca;
using System;
using System.Collections.Generic;

namespace gcgcg
{
    internal class SegReta : ObjetoGeometria
    {

        public List<Ponto4D> listaPontos;

        public SegReta(char rotulo, Objeto paiRef, List<Ponto4D> listaPontos) : base(rotulo, paiRef)
        {
            this.listaPontos = listaPontos;
        }

        protected override void DesenharObjeto()
        {
            GL.Begin(PrimitiveType.LineStrip);

            foreach (Ponto4D pto in this.listaPontos)
            {
                GL.Vertex2(pto.X, pto.Y);
            }
            GL.Vertex2(this.listaPontos[0].X, this.listaPontos[0].Y);
            GL.End();
        }
    }
}