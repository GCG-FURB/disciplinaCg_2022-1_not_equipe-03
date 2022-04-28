using OpenTK.Graphics.OpenGL;
using CG_Biblioteca;
using System;
using System.Collections.Generic;

namespace gcgcg
{
    internal class SegReta : ObjetoGeometria
    {

        public Ponto4D ptoInicial, ptoFinal;

        public SegReta(char rotulo, Objeto paiRef, Ponto4D ptoInicial, Ponto4D ptoFinal) : base(rotulo, paiRef)
        {
            this.ptoInicial = ptoInicial;
            this.ptoFinal = ptoFinal;
        }

        protected override void DesenharObjeto()
        {
            GL.Begin(PrimitiveType.LineStrip);

            GL.Vertex2(this.ptoInicial.X, this.ptoInicial.Y);
            GL.Vertex2(this.ptoFinal.X, this.ptoFinal.Y);

            GL.End();
        }
    }
}