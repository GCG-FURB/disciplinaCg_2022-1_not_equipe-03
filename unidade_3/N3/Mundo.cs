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

        private Mundo(int width, int height) : base(width, height)
        {
        }

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
        int mouseX, mouseY; //TODO: achar método MouseDown para não ter variável Global
        private bool mouseMoverPto = false;
        private Ponto4D pto1, pto2, pto3, pto4, pto5, pto_click_1, pto_click_2, pto_click_3;
        private Poligono bandeirinha;
        private Point point_obj_1, point_obj_2, point_obj_3;
        private bool construindo_poligono = false;
        private int x_mouse, y_mouse;

#if CG_Privado
    private Privado_SegReta obj_SegReta;
    private Privado_Circulo obj_Circulo;
#endif

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            camera.xmin = 0;
            camera.xmax = 600;
            camera.ymin = 0;
            camera.ymax = 600;

            Console.WriteLine(" --- Ajuda / Teclas: ");
            Console.WriteLine(" [  H     ] mostra teclas usadas. ");
            objetoId = Utilitario.charProximo(objetoId);

            pto1 = new Ponto4D(50, 50);
            pto2 = new Ponto4D(50, 150);
            pto3 = new Ponto4D(150, 150);
            pto4 = new Ponto4D(150, 50);
            pto5 = new Ponto4D(100, 100);

            pto_click_1 = new Ponto4D(40, 65);
            point_obj_1 = new Point(objetoId, null, pto_click_1);
            this.objetosLista.Add(point_obj_1);

            pto_click_2 = new Ponto4D(100, 65);
            point_obj_2 = new Point(objetoId, null, pto_click_2);
            this.objetosLista.Add(point_obj_2);

            pto_click_3 = new Ponto4D(160, 65);
            point_obj_3 = new Point(objetoId, null, pto_click_3);
            this.objetosLista.Add(point_obj_3);

            bandeirinha = new Poligono(objetoId, null, new List<Ponto4D>() { pto1, pto2, pto3, pto4, pto5 });
            bandeirinha.PrimitivaTamanho = 3;
            bandeirinha.ObjetoCor.CorR = 0;
            bandeirinha.ObjetoCor.CorG = 0;
            bandeirinha.ObjetoCor.CorB = 255;

            this.objetosLista.Add(bandeirinha);
            objetoSelecionado = bandeirinha;

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

        protected override void OnKeyDown(OpenTK.Input.KeyboardKeyEventArgs e)
        {
            if (e.Key == Key.E)
            {
                Console.WriteLine("--- Objetos / Pontos: ");
                for (var i = 0; i < objetosLista.Count; i++)
                {
                    Console.WriteLine(objetosLista[i]);
                }
            }
            else if (e.Key == Key.Space)
            {
                if (construindo_poligono)
                {
                    objetoSelecionado.PontosAdicionar(new Ponto4D(x_mouse, y_mouse));
                }
                else
                {
                    Poligono poligono = new Poligono(objetoId, null, new List<Ponto4D>());
                    poligono.PrimitivaTamanho = 3;
                    poligono.ObjetoCor.CorR = 255;
                    poligono.ObjetoCor.CorG = 255;
                    poligono.ObjetoCor.CorB = 255;
                    poligono.PontosAdicionar(new Ponto4D(x_mouse, y_mouse));
                    poligono.PontosAdicionar(new Ponto4D(x_mouse, y_mouse));

                    objetosLista.Add(poligono);
                    objetoSelecionado = poligono;
                    construindo_poligono = true;
                }
            }
            else if (e.Key == Key.Enter && construindo_poligono)
            {
                objetoSelecionado.PontosRemoverUltimo();
                construindo_poligono = false;
            }
            else if (e.Key == Key.K)
            {
                List<Ponto4D> alo = new List<Ponto4D>() { pto_click_1, pto_click_2, pto_click_3 };

                foreach (var pto in alo)
                {
                    Console.WriteLine(pto.ToString());
                    if (bandeirinha.BBox.estaDentro(pto))
                    {
                        Console.WriteLine("esta DENTRO da BBox");
                        if (bandeirinha.estaDentro(pto))
                        {
                            Console.WriteLine("esta DENTRO do POLIGONO");
                        }
                    }
                }
            }
            else if (e.Key == Key.R)
            {
                objetoSelecionado.ObjetoCor.CorR = 255;
                objetoSelecionado.ObjetoCor.CorG = 0;
                objetoSelecionado.ObjetoCor.CorB = 0;
            }
            else if (e.Key == Key.G)
            {
                objetoSelecionado.ObjetoCor.CorR = 0;
                objetoSelecionado.ObjetoCor.CorG = 255;
                objetoSelecionado.ObjetoCor.CorB = 0;
            }
            else if (e.Key == Key.B)
            {
                objetoSelecionado.ObjetoCor.CorR = 0;
                objetoSelecionado.ObjetoCor.CorG = 0;
                objetoSelecionado.ObjetoCor.CorB = 255;
            }
            else
            {
                Console.WriteLine(" __ Tecla não implementada.");
                Console.WriteLine(e.Key);
            }
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            //Console.WriteLine("Mouse X" + e.Position.X + " - Y" + (600 - e.Position.Y)); // Inverti eixo Y
            x_mouse = e.Position.X;
            y_mouse = (600 - e.Position.Y);

            if (construindo_poligono)
            {
                objetoSelecionado.PontosUltimo().X = x_mouse;
                objetoSelecionado.PontosUltimo().Y = y_mouse;
            }
        }

#if CG_Gizmo
        private void Sru3D()
        {
            GL.LineWidth(5);
            GL.Begin(PrimitiveType.Lines);
            // GL.Color3(1.0f,0.0f,0.0f);
            GL.Color3(Convert.ToByte(255), Convert.ToByte(0), Convert.ToByte(0));
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(200, 0, 0);
            // GL.Color3(0.0f,1.0f,0.0f);
            GL.Color3(Convert.ToByte(0), Convert.ToByte(255), Convert.ToByte(0));
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, 200, 0);
            // GL.Color3(0.0f,0.0f,1.0f);
            GL.Color3(Convert.ToByte(0), Convert.ToByte(0), Convert.ToByte(255));
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, 0, 200);
            GL.End();
        }
#endif
    }

    class Program
    {
        static void Main(string[] args)
        {
            Mundo window = Mundo.GetInstance(600, 600);
            window.Title = "CG_N3";
            window.Run(1.0 / 60.0);
        }
    }
}