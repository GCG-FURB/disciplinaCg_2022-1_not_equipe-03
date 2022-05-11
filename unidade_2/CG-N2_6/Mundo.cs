
#define CG_Gizmo
// #define CG_Privado

using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using OpenTK.Input;
using CG_Biblioteca;

namespace gcgcg
{
    class Mundo : GameWindow
    {
        private static Mundo instanciaMundo = null;

        private Mundo(int width, int height) : base(width, height) { }

        public static Mundo GetInstance(int width, int height)
        {
            if (instanciaMundo == null)
                instanciaMundo = new Mundo(width, height);
            return instanciaMundo;
        }

        private CameraOrtho camera = new CameraOrtho();
        protected List<Objeto> objetosLista = new List<Objeto>();
        private ObjetoGeometria objetoSelecionado = null;
        private char objetoId = '@';
        private bool bBoxDesenhar = false;
        int mouseX, mouseY;   //TODO: achar método MouseDown para não ter variável Global
        private bool mouseMoverPto = false;
        private Retangulo ret_1, ret_2, ret_3, ret_4;
        private Circulo circulo_obj;
        private Ponto4D pto1, pto2, pto3, pto4, pto_selecionado;
        private SegReta seg_reta_1, seg_reta_2, seg_reta_3, seg_reta_4;
        private PrimitivasGeometricas primitivasGeometricas;
        private int primitivaIndex = 0;
        private Spline spline;
        private Point point_1, point_2, point_3, point_4, point_selecionado;

        private SrPalito sr_palito;

        private List<PrimitiveType> listaDePrimitivos = new List<PrimitiveType>();

#if CG_Privado
    private Privado_SegReta obj_SegReta;
    private Privado_Circulo obj_Circulo;
#endif

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            camera.xmin = -400; camera.xmax = 400; camera.ymin = -400; camera.ymax = 400;

            Console.WriteLine(" --- Ajuda / Teclas: ");
            Console.WriteLine(" [  H     ] mostra teclas usadas. ");
            objetoId = Utilitario.charProximo(objetoId);

            // Ponto 1
            pto1 = new Ponto4D(-100, -100);

            point_1 = new Point(objetoId, null, pto1);
            point_1.ObjetoCor.CorR = 255; point_1.ObjetoCor.CorG = 0; point_1.ObjetoCor.CorB = 0;
            point_1.PrimitivaTamanho = 10;
            objetosLista.Add(point_1);

            // Ponto 2
            pto2 = new Ponto4D(-100, 100);

            point_2 = new Point(objetoId, null, pto2);
            point_2.ObjetoCor.CorR = 0; point_2.ObjetoCor.CorG = 0; point_2.ObjetoCor.CorB = 0;
            point_2.PrimitivaTamanho = 10;
            objetosLista.Add(point_2);

            // Ponto 3
            pto3 = new Ponto4D(100, 100);
            point_3 = new Point(objetoId, null, pto3);
            point_3.ObjetoCor.CorR = 0; point_3.ObjetoCor.CorG = 0; point_3.ObjetoCor.CorB = 0;
            point_3.PrimitivaTamanho = 10;
            objetosLista.Add(point_3);

            // Ponto 4
            pto4 = new Ponto4D(100, -100);
            point_4 = new Point(objetoId, null, pto4);
            point_4.ObjetoCor.CorR = 0; point_4.ObjetoCor.CorG = 0; point_4.ObjetoCor.CorB = 0;
            point_4.PrimitivaTamanho = 10;
            objetosLista.Add(point_4);


            // Segmento de cima
            seg_reta_1 = new SegReta(objetoId, null, pto2, pto3);
            seg_reta_1.ObjetoCor.CorR = 0; seg_reta_1.ObjetoCor.CorG = 255; seg_reta_1.ObjetoCor.CorB = 255;
            seg_reta_1.PrimitivaTamanho = 3;
            objetosLista.Add(seg_reta_1);

            // Segmento de esquerda
            seg_reta_2 = new SegReta(objetoId, null, pto1, pto2);
            seg_reta_2.ObjetoCor.CorR = 0; seg_reta_2.ObjetoCor.CorG = 255; seg_reta_2.ObjetoCor.CorB = 255;
            seg_reta_2.PrimitivaTamanho = 3;
            objetosLista.Add(seg_reta_2);

            // Segmento de direita
            seg_reta_3 = new SegReta(objetoId, null, pto3, pto4);
            seg_reta_3.ObjetoCor.CorR = 0; seg_reta_3.ObjetoCor.CorG = 255; seg_reta_3.ObjetoCor.CorB = 255;
            seg_reta_3.PrimitivaTamanho = 3;
            objetosLista.Add(seg_reta_3);

            // Spline
            spline = new Spline(objetoId, null, pto1, pto2, pto3, pto4, 10);
            spline.ObjetoCor.CorR = 255; spline.ObjetoCor.CorG = 255; spline.ObjetoCor.CorB = 0;
            spline.PrimitivaTamanho = 3;
            objetosLista.Add(spline);


            point_selecionado = point_1;
            pto_selecionado = pto1;
            objetoSelecionado = seg_reta_1;

#if CG_Privado
      // objetoId = Utilitario.charProximo(objetoId);
      // obj_SegReta = new Privado_SegReta(objetoId, null, new Ponto4D(50, 150), new Ponto4D(150, 250));
      // obj_SegReta.ObjetoCor.CorR = 255; obj_SegReta.ObjetoCor.CorG = 255; obj_SegReta.ObjetoCor.CorB = 0;
      // objetosLista.Add(obj_SegReta);
      // objetoSelecionado = obj_SegReta;

      // objetoId = Utilitario.charProximo(objetoId);
      // obj_Circulo = new Privado_Circulo(objetoId, null, new Ponto4D(100, 300), 50);
      // obj_Circulo.ObjetoCor.CorR = 0; obj_Circulo.ObjetoCor.CorG = 255; obj_Circulo.ObjetoCor.CorB = 255;
      // objetosLista.Add(obj_Circulo);
      // objetoSelecionado = obj_Circulo;
#endif
            GL.ClearColor(0.5f, 0.5f, 0.5f, 1.0f);
        }
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(camera.xmin, camera.xmax, camera.ymin, camera.ymax, camera.zmin, camera.zmax);
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
#if CG_Gizmo
            Sru3D();
#endif
            for (var i = 0; i < objetosLista.Count; i++)
                objetosLista[i].Desenhar();
            if (bBoxDesenhar && (objetoSelecionado != null))
                objetoSelecionado.BBox.Desenhar();
            this.SwapBuffers();
        }

        public void trocaPontoSelecionado(Ponto4D novo_pto_selecionado, Point novo_point){
            this.point_selecionado.ObjetoCor.CorR = 0;

            this.pto_selecionado = novo_pto_selecionado;
            this.point_selecionado = novo_point;

            novo_point.ObjetoCor.CorR = 255;
        }

        protected override void OnKeyDown(OpenTK.Input.KeyboardKeyEventArgs e)
        {
            if (e.Key == Key.Number1){
                trocaPontoSelecionado(pto1, point_1);
            }
            else if (e.Key == Key.Number2)
            {
                trocaPontoSelecionado(pto2, point_2);
            }
            else if (e.Key == Key.Number3)
            {
                trocaPontoSelecionado(pto3, point_3);
            }
            else if (e.Key == Key.Number4)
            {
                trocaPontoSelecionado(pto4, point_4);
            }
            else if (e.Key == Key.C)
            {
                pto_selecionado.Y += 2;
            }
            else if (e.Key == Key.B)
            {
                pto_selecionado.Y -= 2;
            }
            else if (e.Key == Key.E)
            {
                pto_selecionado.X -= 2;
            }
            else if (e.Key == Key.D)
            {
                pto_selecionado.X += 2;
            }
            else if (e.Key == Key.Plus)
            {
                this.spline.quantidade_pontos++;
            }
            else if (e.Key == Key.Minus)
            {
                this.spline.quantidade_pontos--;
            }
            else 
            {
                Console.WriteLine(" __ Tecla não implementada.");
                Console.WriteLine(e.Key);
            }

        }

        //TODO: não está considerando o NDC
        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            mouseX = e.Position.X; mouseY = 600 - e.Position.Y; // Inverti eixo Y
            if (mouseMoverPto && (objetoSelecionado != null))
            {
                objetoSelecionado.PontosUltimo().X = mouseX;
                objetoSelecionado.PontosUltimo().Y = mouseY;
            }
        }

#if CG_Gizmo
        private void Sru3D()
        {
            GL.LineWidth(5);
            GL.Begin(PrimitiveType.Lines);
            // GL.Color3(1.0f,0.0f,0.0f);
            GL.Color3(Convert.ToByte(255), Convert.ToByte(0), Convert.ToByte(0));
            GL.Vertex3(0, 0, 0); GL.Vertex3(200, 0, 0);
            // GL.Color3(0.0f,1.0f,0.0f);
            GL.Color3(Convert.ToByte(0), Convert.ToByte(255), Convert.ToByte(0));
            GL.Vertex3(0, 0, 0); GL.Vertex3(0, 200, 0);
            // GL.Color3(0.0f,0.0f,1.0f);
            GL.Color3(Convert.ToByte(0), Convert.ToByte(0), Convert.ToByte(255));
            GL.Vertex3(0, 0, 0); GL.Vertex3(0, 0, 200);
            GL.End();
        }
#endif
    }
    class Program
    {
        static void Main(string[] args)
        {
            Mundo window = Mundo.GetInstance(600, 600);
            window.Title = "CG_N2_4";
            window.Run(1.0 / 60.0);
        }
    }
}
