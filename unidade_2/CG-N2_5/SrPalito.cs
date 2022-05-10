using OpenTK.Graphics.OpenGL;
using CG_Biblioteca;
using System;

namespace gcgcg
{
    internal class SrPalito : ObjetoGeometria
    {
        int angulo, raio;
        Ponto4D ptoInicial, ptoFinal;
        SegReta seg_reta;

        public SrPalito(char rotulo, Objeto paiRef, Ponto4D ptoInicial, int angulo, int raio) : base(rotulo, paiRef)
        {
            base.PontosAdicionar(ptoInicial);
            this.ptoInicial = ptoInicial;
            this.angulo = angulo;
            this.raio = raio;
        }

        public void irDireita()
        {
            this.ptoInicial.X += 1;
            this.ptoInicial.X += 1;
        }

        public void irEsquerda()
        {
            this.ptoInicial.X -= 1;
        }

        public void MaisAngulo()
        {
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
            ptoFinal = Matematica.GerarPtosCirculo((double)this.angulo, (double)this.raio);
            ptoFinal = new Ponto4D(ptoInicial.X + ptoFinal.X, ptoInicial.Y + ptoFinal.Y);

            seg_reta = new SegReta(this.rotulo, null, ptoInicial, ptoFinal);
            seg_reta.ObjetoCor.CorR = 0;
            seg_reta.ObjetoCor.CorG = 0;
            seg_reta.ObjetoCor.CorB = 0;
            seg_reta.PrimitivaTamanho = 3;
            seg_reta.Desenhar();
        }
    }
}