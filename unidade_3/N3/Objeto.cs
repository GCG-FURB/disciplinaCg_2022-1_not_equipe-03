/**
  Autor: Dalton Solano dos Reis
**/

using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using CG_Biblioteca;

namespace gcgcg
{
  internal abstract class Objeto
  {
    protected char rotulo;
    private Cor objetoCor = new Cor(255, 255, 255, 255);
    public Cor ObjetoCor { get => objetoCor; set => objetoCor = value; }
    private PrimitiveType primitivaTipo = PrimitiveType.LineLoop;
    public PrimitiveType PrimitivaTipo { get => primitivaTipo; set => primitivaTipo = value; }
    private float primitivaTamanho = 1;
    public float PrimitivaTamanho { get => primitivaTamanho; set => primitivaTamanho = value; }
    private BBox bBox = new BBox();
    public BBox BBox { get => bBox; set => bBox = value; }
    private List<Objeto> objetosLista = new List<Objeto>();
    public Transformacao4D matriz = new Transformacao4D();
    public Transformacao4D matrizTmpRotacao = new Transformacao4D();
    public Transformacao4D matrizTmpTranslacao = new Transformacao4D();
    public Transformacao4D matrizTmpTranslacaoInversa = new Transformacao4D();
    public Transformacao4D matrizGlobal = new Transformacao4D();
    public char eixoRotacao = 'x';
    
    public Objeto(char rotulo, Objeto paiRef)
    {
      this.rotulo = rotulo;
    }

    public char getRotulo()
    {
      return this.rotulo;
    }

    public void Desenhar()
    {
      GL.PushMatrix();
      GL.MultMatrix(matriz.ObterDados());
      GL.Color3(objetoCor.CorR, objetoCor.CorG, objetoCor.CorB);
      GL.LineWidth(primitivaTamanho);
      GL.PointSize(primitivaTamanho);
      DesenharGeometria();
      for (var i = 0; i < objetosLista.Count; i++)
      {
        objetosLista[i].Desenhar();
      }
      GL.PopMatrix();
    }
    
    public void RotacaoEixo(double angulo)
    {
      switch (eixoRotacao)
      {
        case 'x':
          matrizTmpRotacao.AtribuirRotacaoX(Transformacao4D.DEG_TO_RAD * angulo);
          break;
        case 'y':
          matrizTmpRotacao.AtribuirRotacaoY(Transformacao4D.DEG_TO_RAD * angulo);
          break;
        case 'z':
          matrizTmpRotacao.AtribuirRotacaoZ(Transformacao4D.DEG_TO_RAD * angulo);
          break;
      }
    }
    
    protected abstract void DesenharGeometria();
    public void FilhoAdicionar(Objeto filho)
    {
      this.objetosLista.Add(filho);
    }
    public void FilhoRemover(Objeto filho)
    {
      this.objetosLista.Remove(filho);
    }
    public List<Objeto> GetFilhos()
    {
      return this.objetosLista;
    }
    
    public void TranslacaoXYZ(double tx, double ty, double tz)
    {
      Transformacao4D t = new Transformacao4D();
      t.AtribuirTranslacao(tx, ty, tz);
      matriz = t.MultiplicarMatriz(matriz);
    }
    
    public void EscalaXYZ(double sx, double sy, double sz)
    {
      Transformacao4D t = new Transformacao4D();
      t.AtribuirEscala(sx, sy, sz);
      matriz = t.MultiplicarMatriz(matriz);
    }
    
    public void EscalaXYZBBox(double sx, double sy, double sz)
    {
      Transformacao4D t = new Transformacao4D();
      Transformacao4D t1 = new Transformacao4D();
      Transformacao4D t2 = new Transformacao4D();
      Transformacao4D t3 = new Transformacao4D();
      Ponto4D pto_centro = bBox.obterCentro;
      
      t1.AtribuirTranslacao(-pto_centro.X, -pto_centro.Y, -pto_centro.Z);
      t = t1.MultiplicarMatriz(t);
      
      t2.AtribuirEscala(sx, sy, sz);
      t = t2.MultiplicarMatriz(t);
      
      t3.AtribuirTranslacao(pto_centro.X, pto_centro.Y, pto_centro.Z);
      t = t3.MultiplicarMatriz(t);

      matriz = matriz.MultiplicarMatriz(t);
    }

    public void AtribuirIdentidade()
    { 
      matriz.AtribuirIdentidade();
    }
    
    
    public void rotacaoOrigem(double angulo)
    {
      Transformacao4D matrizGeralTemporaria = new Transformacao4D();
      Transformacao4D matrizRotacaoTemporaria = new Transformacao4D();

      matrizGeralTemporaria.AtribuirIdentidade();

      matrizRotacaoTemporaria.AtribuirRotacaoZ(Transformacao4D.DEG_TO_RAD * angulo);
      matrizGeralTemporaria = matrizRotacaoTemporaria.MultiplicarMatriz(matrizGeralTemporaria);


      matriz = matriz.MultiplicarMatriz(matrizGeralTemporaria);
    }

    public void rotacaoBBox(double angulo)
    {
      Transformacao4D matrizGeralTemporaria = new Transformacao4D();
      Transformacao4D matrizRotacaoTemporaria = new Transformacao4D();
      Transformacao4D matrizTranslacaoTemporaria2 = new Transformacao4D();
      Transformacao4D matrizTranslacaoTemporaria = new Transformacao4D();
      
      matrizGeralTemporaria.AtribuirIdentidade();
      Ponto4D pontoPivo = bBox.obterCentro;

      matrizTranslacaoTemporaria.AtribuirTranslacao(-pontoPivo.X, -pontoPivo.Y, -pontoPivo.Z);
      matrizGeralTemporaria = matrizTranslacaoTemporaria.MultiplicarMatriz(matrizGeralTemporaria);

      matrizRotacaoTemporaria.AtribuirRotacaoZ(Transformacao4D.DEG_TO_RAD * angulo);
      matrizGeralTemporaria = matrizRotacaoTemporaria.MultiplicarMatriz(matrizGeralTemporaria);

      matrizTranslacaoTemporaria2.AtribuirTranslacao(pontoPivo.X, pontoPivo.Y, pontoPivo.Z);
      matrizGeralTemporaria = matrizTranslacaoTemporaria2.MultiplicarMatriz(matrizGeralTemporaria);

      matriz = matriz.MultiplicarMatriz(matrizGeralTemporaria);
    }
    
  }
}