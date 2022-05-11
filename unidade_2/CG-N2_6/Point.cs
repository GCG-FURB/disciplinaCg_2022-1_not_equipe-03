/**
  Autor: Dalton Solano dos Reis
**/

using OpenTK.Graphics.OpenGL;
using CG_Biblioteca;

namespace gcgcg
{
    internal class Point : ObjetoGeometria
    {
        private Ponto4D pto;
        public Point(char rotulo, Objeto paiRef, Ponto4D pto) : base(rotulo, paiRef)
        {
            this.pto = pto;
        }

        protected override void DesenharObjeto()
        {
            GL.Begin(PrimitiveType.Points);
            GL.Vertex2(this.pto.X, this.pto.Y);
            GL.End();
        }

    }
}