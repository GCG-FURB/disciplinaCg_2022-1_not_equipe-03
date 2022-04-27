using OpenTK.Graphics.OpenGL;
using CG_Biblioteca;
using System;

namespace gcgcg
{
    internal class SrPalito : ObjetoGeometria
    {
        int angulo, raio;
        Ponto4D ptoInicial, ptoFinal;

        public SrPalito(char rotulo, Objeto paiRef, Ponto4D ptoInicial, int angulo, int raio) : base(rotulo, paiRef)
        {
            base.PontosAdicionar(ptoInicial);
            this.ptoInicial = ptoInicial;
            this.angulo = angulo;
            this.raio = raio;
        }

        public void MaisAngulo() {
            this.angulo += 1;
        }

        public void MenosAngulo()
        {
            this.angulo -= 1;
        }

        public void MaisRaio()
        {
            this.raio += 1;
        }
        public void MenosRaio()
        {
            this.raio -= 1;
        }

        protected override void DesenharObjeto()
        {
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex2(ptoInicial.X, ptoInicial.Y);
            ptoFinal = Matematica.GerarPtosCirculo((double)this.angulo, (double)this.raio);
            GL.Vertex2(ptoFinal.X, ptoFinal.Y);
            GL.End();
        }
    }
}