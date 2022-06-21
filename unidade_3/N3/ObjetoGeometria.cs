/**
  Autor: Dalton Solano dos Reis
**/

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using CG_Biblioteca;

namespace gcgcg
{
    internal abstract class ObjetoGeometria : Objeto
    {
        public List<Ponto4D> pontosLista = new List<Ponto4D>();

        public ObjetoGeometria(char rotulo, Objeto paiRef) : base(rotulo, paiRef)
        {
        }

        protected override void DesenharGeometria()
        {
            DesenharObjeto();
        }

        protected abstract void DesenharObjeto();

        public void PontosAdicionar(Ponto4D pto)
        {
            pontosLista.Add(pto);
            if (pontosLista.Count.Equals(1))
                base.BBox.Atribuir(pto);
            else
                base.BBox.Atualizar(pto);
            base.BBox.ProcessarCentro();
        }

        public void PontosRemoverUltimo()
        {
            pontosLista.RemoveAt(pontosLista.Count - 1);
        }

        public void PontosRemoverNoIndex(int index)
        {
            pontosLista.RemoveAt(index);
        }
        
        public void PontosRemover(Ponto4D pto)
        {
            pontosLista.Remove(pto);
        }

        protected void PontosRemoverTodos()
        {
            pontosLista.Clear();
        }

        public Ponto4D PontosUltimo()
        {
            return pontosLista[pontosLista.Count - 1];
        }

        public void PontosAlterar(Ponto4D pto, int posicao)
        {
            pontosLista[posicao] = pto;
        }

        public override string ToString()
        {
            string retorno;
            retorno = "__ Objeto: " + base.rotulo + "\n";
            for (var i = 0; i < pontosLista.Count; i++)
            {
                retorno += "P" + i + "[" + pontosLista[i].X + "," + pontosLista[i].Y + "," + pontosLista[i].Z + "," +
                           pontosLista[i].W + "]" + "\n";
            }

            return (retorno);
        }

        public bool estaDentro(Ponto4D pto)
        {
            int cruzamentos = 0;
            for (int i = 0; i < pontosLista.Count; i++)
            {
                int index_prox_pto = i + 1;
                if (index_prox_pto >= pontosLista.Count)
                    index_prox_pto = 0;
                
                Ponto4D pto_1 = pontosLista[i];
                Ponto4D pto_2 = pontosLista[index_prox_pto];

                double ti = Matematica.intercecao_scan_line(pto.Y, pto_1.Y, pto_2.Y);

                if (ti >= 0 && ti <= 1)
                {
                    double xi = Matematica.calcula_xi_scan_line(pto_1.X, pto_2.X, ti);

                    if (xi > pto.X)
                    {
                        cruzamentos += 1;
                    }
                    
                }
            }

            return cruzamentos % 2 != 0;
        }
    }
}