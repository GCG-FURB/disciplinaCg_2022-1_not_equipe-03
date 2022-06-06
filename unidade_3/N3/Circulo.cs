using OpenTK.Graphics.OpenGL;
using CG_Biblioteca;
using System;

namespace gcgcg
{
    internal class Circulo : ObjetoGeometria
    {

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

            for (int i = 0; i < 360; i += 5)
            {
                pto = Matematica.GerarPtosCirculo(i, this.radius);
                GL.Vertex2(pto.X + ptoCentro.X, pto.Y + ptoCentro.Y);
            }
            GL.End();
        }

        public bool estaDentro(Ponto4D pto)
        {
            double lado_x = Math.Pow((double)this.ptoCentro.X - pto.X, 2);
            double lado_y = Math.Pow((double)this.ptoCentro.Y - pto.Y, 2);

            double valorRadicando = lado_x + lado_y;

            double distacia = Math.Sqrt(valorRadicando);

            if(distacia > radius){
                return false;
            }
            return true;
        }
    }
}