using OpenTK.Graphics.OpenGL;
using CG_Biblioteca;
using System;
using System.Collections.Generic;

namespace gcgcg
{
    internal class PrimitivasGeometricas : ObjetoGeometria
    {

        public PrimitivasGeometricas(char rotulo, Objeto paiRef, List<Ponto4D> listaPontos) : base(rotulo, paiRef)
        {
            this.pontosLista = listaPontos;
        }

        protected override void DesenharObjeto()
        {
            GL.Begin(base.PrimitivaTipo);

            foreach (Ponto4D pto in this.pontosLista)
            {
                GL.Vertex2(pto.X, pto.Y);
            }
            GL.End();
        }
    }
}