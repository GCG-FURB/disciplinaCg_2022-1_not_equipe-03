using OpenTK.Graphics.OpenGL;
using CG_Biblioteca;
using System;

namespace gcgcg
{
    internal class Linha : ObjetoGeometria
    {
        int tamanho;
        Ponto4D ptoInicial;
        bool vertical;

        public Linha(char rotulo, Objeto paiRef, Ponto4D ptoInicial, int tamanho, bool vertical = false) : base(rotulo, paiRef)
        {
            base.PontosAdicionar(ptoInicial);
            this.ptoInicial = ptoInicial;
            this.tamanho = tamanho;
            this.vertical = vertical;
        }

        protected override void DesenharObjeto()
        {
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex2(ptoInicial.X, ptoInicial.Y);

            if (vertical == true)
            {
                GL.Vertex2(ptoInicial.X, ptoInicial.Y + this.tamanho);
            }
            else
            {
                GL.Vertex2(ptoInicial.X + this.tamanho, ptoInicial.Y);
            }
            GL.End();
        }
    }
}