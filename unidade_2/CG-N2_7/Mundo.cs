
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
        private Circulo circulo_obj_grande, circulo_obj_pequeno;
        private Ponto4D pto_central_circulo_grande, pto_circulo_pequeno, point_1, point_2;
        private SegReta seg_reta_1, seg_reta_2, seg_reta_3, seg_reta_4;
        private PrimitivasGeometricas primitivasGeometricas;
        private int primitivaIndex = 0;
        private Spline spline;
        private Point point_visual_centro;

        private SrPalito sr_palito;

        private List<PrimitiveType> listaDePrimitivos = new List<PrimitiveType>();

#if CG_Privado
    private Privado_SegReta obj_SegReta;
    private Privado_Circulo obj_Circulo;
#endif

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            camera.xmin = -100; camera.xmax = 500; camera.ymin = -100; camera.ymax = 500;

            Console.WriteLine(" --- Ajuda / Teclas: ");
            Console.WriteLine(" [  H     ] mostra teclas usadas. ");
            objetoId = Utilitario.charProximo(objetoId);

            // Circulo grande
            pto_central_circulo_grande = new Ponto4D(200, 200);
            circulo_obj_grande = new Circulo(objetoId, null, pto_central_circulo_grande, 150);
            circulo_obj_grande.ObjetoCor.CorR = 0;
            circulo_obj_grande.ObjetoCor.CorG = 0;
            circulo_obj_grande.ObjetoCor.CorB = 0;
            circulo_obj_grande.PrimitivaTamanho = 3;
            objetosLista.Add(circulo_obj_grande);

            // Retangulo Interno
            point_1 = Matematica.GerarPtosCirculo(45.0, 150);
            point_1.X += pto_central_circulo_grande.X;
            point_1.Y += pto_central_circulo_grande.Y;

            point_2 = Matematica.GerarPtosCirculo(225.0, 150);
            point_2.X += pto_central_circulo_grande.X;
            point_2.Y += pto_central_circulo_grande.Y;

            ret_1 = new Retangulo(objetoId, null, point_1, point_2);
            ret_1.ObjetoCor.CorR = 255;
            ret_1.ObjetoCor.CorG = 0;
            ret_1.ObjetoCor.CorB = 0;
            ret_1.PrimitivaTamanho = 3;
            objetosLista.Add(ret_1);


            // Circulo Pequeno
            pto_circulo_pequeno = new Ponto4D(200, 200);

            circulo_obj_pequeno = new Circulo(objetoId, null, pto_circulo_pequeno, 50);
            circulo_obj_pequeno.ObjetoCor.CorR = 0;
            circulo_obj_pequeno.ObjetoCor.CorG = 0;
            circulo_obj_pequeno.ObjetoCor.CorB = 0;
            circulo_obj_pequeno.PrimitivaTamanho = 3;

            objetosLista.Add(circulo_obj_pequeno);

            // Point
            point_visual_centro = new Point(objetoId, null, pto_circulo_pequeno);
            point_visual_centro.ObjetoCor.CorR = 0;
            point_visual_centro.ObjetoCor.CorG = 0;
            point_visual_centro.ObjetoCor.CorB = 0;
            point_visual_centro.PrimitivaTamanho = 5;

            objetosLista.Add(point_visual_centro);
            objetoSelecionado = ret_1;



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

        public bool podeMoverCirculo()
        {
            if (ret_1.BBox.estaDentro(pto_circulo_pequeno))
            {
                Console.WriteLine("Dentro Retangulo");
                ret_1.ObjetoCor.CorR = 255;
                ret_1.ObjetoCor.CorG = 0;
                ret_1.ObjetoCor.CorB = 0;
                return true;
            }
            else if (circulo_obj_grande.estaDentro(pto_circulo_pequeno))
            {
                Console.WriteLine("Dentro Circulo");
                ret_1.ObjetoCor.CorR = 0;
                ret_1.ObjetoCor.CorG = 0;
                ret_1.ObjetoCor.CorB = 255;
                return true;
            }
            else
            {
                Console.WriteLine("FORAA");
                return false;
            }
        }

        protected override void OnKeyDown(OpenTK.Input.KeyboardKeyEventArgs e)
        {
            if (e.Key == Key.W)
            {
                pto_circulo_pequeno.Y += 4;

                if (!this.podeMoverCirculo())
                    pto_circulo_pequeno.Y -= 4;
            }
            else if (e.Key == Key.A && this.podeMoverCirculo())
            {
                pto_circulo_pequeno.X -= 4;

                if (!this.podeMoverCirculo())
                    pto_circulo_pequeno.X += 4;
            }
            else if (e.Key == Key.S && this.podeMoverCirculo())
            {
                pto_circulo_pequeno.Y -= 4;

                if (!this.podeMoverCirculo())
                    pto_circulo_pequeno.Y += 4;

            }
            else if (e.Key == Key.D && this.podeMoverCirculo())
            {
                pto_circulo_pequeno.X += 4;

                if (!this.podeMoverCirculo())
                    pto_circulo_pequeno.X -= 4;
            }
            else if (e.Key == Key.O)
                bBoxDesenhar = !bBoxDesenhar;
            else if (e.Key == Key.E)
            {
                Console.WriteLine("--- Objetos / Pontos: ");
                for (var i = 0; i < objetosLista.Count; i++)
                {
                    Console.WriteLine(objetosLista[i]);
                }
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
            window.Title = "CG_N2_7";
            window.Run(1.0 / 60.0);
        }
    }
}
