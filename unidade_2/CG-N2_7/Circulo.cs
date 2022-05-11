using OpenTK.Graphics.OpenGL;
using CG_Biblioteca;
using System;

namespace gcgcg
{
    internal class Circulo : ObjetoGeometria{

        double radius;
        Ponto4D pto, ptoCentro;

        public Circulo(char rotulo, Objeto paiRef, Ponto4D ptoCentro, double radius) : base(rotulo, paiRef)
        {
            base.PontosAdicionar(ptoCentro);
            this.ptoCentro = ptoCentro;
            this.radius = radius;
        }

        protected override void DesenharObjeto()
        {
            // GL.Enable(EnableCap.Blend);
            // GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.Begin(base.PrimitivaTipo);

            for (int i = 0; i < 360; i+=5)
            {
                pto = Matematica.GerarPtosCirculo(i, this.radius);
                GL.Vertex2(pto.X + ptoCentro.X, pto.Y + ptoCentro.Y);
            }
            GL.End();
        }
    }
}