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
        protected List<ObjetoGeometria> objetosLista = new List<ObjetoGeometria>();
        private ObjetoGeometria objetoSelecionado = null;
        private char objetoId = '@';
        private bool bBoxDesenhar = false;
        private bool mouseMoverPto = false;
        private Ponto4D pto1, pto2, pto3, pto4, pto5, pto_click, vertice_movendo;
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

            // pto_click_1 = new Ponto4D(40, 65);
            // point_obj_1 = new Point(objetoId, null, pto_click_1);
            // this.objetosLista.Add(point_obj_1);
            //
            // objetoId = Utilitario.charProximo(objetoId);
            // pto_click_2 = new Ponto4D(100, 65);
            // point_obj_2 = new Point(objetoId, null, pto_click_2);
            // this.objetosLista.Add(point_obj_2);
            //
            // objetoId = Utilitario.charProximo(objetoId);
            // pto_click_3 = new Ponto4D(160, 65);
            // point_obj_3 = new Point(objetoId, null, pto_click_3);
            // this.objetosLista.Add(point_obj_3);

            //objetoId = Utilitario.charProximo(objetoId);
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

        protected int GetIndexsVerticeMiasPertoMouse()
        {
            int index_i = -1;
            double distancia_menor = Double.MaxValue;


            for (int j = 0; j < objetoSelecionado.pontosLista.Count; j++)
            {
                double distacia = Matematica.DistaciaEntrePontos(x_mouse, y_mouse, objetoSelecionado.pontosLista[j].X,
                    objetoSelecionado.pontosLista[j].Y);

                if (distacia < distancia_menor)
                {
                    distancia_menor = distacia;
                    index_i = j;
                }
            }

            return index_i;
        }

        protected override void OnKeyDown(OpenTK.Input.KeyboardKeyEventArgs e)
        {
            if (e.Key == Key.H)
            {
                Utilitario.AjudaTeclado();
            }
            else if (e.Key == Key.Escape)
            {
                Console.WriteLine("Como que encerra?");
            }
            else if (e.Key == Key.E)
            {
                Console.WriteLine("--- Objetos / Pontos: ");
                for (var i = 0; i < objetosLista.Count; i++)
                {
                    Console.WriteLine(objetosLista[i]);
                }
            }
            else if (e.Key == Key.O)
            {
                bBoxDesenhar = !bBoxDesenhar;
            }
            else if (e.Key == Key.Enter)
            {
                if (construindo_poligono)
                {
                    objetoSelecionado.PontosRemoverUltimo();
                    construindo_poligono = false;
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
                    objetoId = Utilitario.charProximo(objetoId);
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
            else if (e.Key == Key.A)
            {
                pto_click = new Ponto4D(x_mouse, y_mouse);

                for (var i = 0; i < objetosLista.Count; i++)
                {
                    if (objetosLista[i].BBox.estaDentro(pto_click))
                    {
                        Console.WriteLine("Dentro BBox " + objetosLista[i].getRotulo());
                        if (objetosLista[i].estaDentro(pto_click))
                        {
                            Console.WriteLine("Dentro Poligono (objeto novo selecionado)" + objetosLista[i].getRotulo());
                            objetoSelecionado = objetosLista[i];
                            break;
                        }
                    }
                }
                
            }
            else if (e.Key == Key.M)
            {
                Console.WriteLine(objetoSelecionado.matriz);
            }
            else if (e.Key == Key.P)
            {
                Console.WriteLine(objetoSelecionado);
            }
            else if (e.Key == Key.I)
            {
                objetoSelecionado.AtribuirIdentidade();
                Console.WriteLine("AtribuirIdentidade ao objetoSelecionado");
            }
            else if (e.Key == Key.Left)
                objetoSelecionado.TranslacaoXYZ(-10, 0, 0);
            else if (e.Key == Key.Right)
                objetoSelecionado.TranslacaoXYZ(10, 0, 0);
            else if (e.Key == Key.Up)
                objetoSelecionado.TranslacaoXYZ(0, 10, 0);
            else if (e.Key == Key.Down)
                objetoSelecionado.TranslacaoXYZ(0, -10, 0);
            else if (e.Key == Key.PageDown)
                objetoSelecionado.EscalaXYZ(1.2, 1.2, 1.2);
            else if (e.Key == Key.PageUp)
                objetoSelecionado.EscalaXYZ(0.8, 0.8, 0.8);
            else if (e.Key == Key.Home)
            {
                
            }
            else if (e.Key == Key.End)
            {
            }
            else if (e.Key == Key.Number1)
            {
            }
            else if (e.Key == Key.Number2)
            {
            }
            else if (e.Key == Key.Number3)
            {
            }
            else if (e.Key == Key.Number4)
            {
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
            else if (e.Key == Key.S)
            {
                if (objetoSelecionado.PrimitivaTipo == PrimitiveType.LineLoop)
                    objetoSelecionado.PrimitivaTipo = PrimitiveType.LineStrip;
                else
                    objetoSelecionado.PrimitivaTipo = PrimitiveType.LineLoop;
            }
            else if (e.Key == Key.D)
            {
                var index = GetIndexsVerticeMiasPertoMouse();
                if (index >= 0)
                    objetoSelecionado.PontosRemoverNoIndex(index);
            }
            else if (e.Key == Key.V)
            {
                if (vertice_movendo != null)
                {
                    vertice_movendo = null;
                }
                else
                {
                    var index = GetIndexsVerticeMiasPertoMouse();
                    if (index >= 0)
                    {
                        vertice_movendo = objetoSelecionado.pontosLista[index];
                        vertice_movendo.X = x_mouse;
                        vertice_movendo.Y = y_mouse;
                    }
                }
            }
            else if (e.Key == Key.C)
            {
                objetosLista.Remove(objetoSelecionado);
            }
            else if (e.Key == Key.X)
            {
            }
            else if (e.Key == Key.Y)
            {
            }
            else if (e.Key == Key.Z)
            {
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

            if (vertice_movendo != null)
            {
                vertice_movendo.X = x_mouse;
                vertice_movendo.Y = y_mouse;
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