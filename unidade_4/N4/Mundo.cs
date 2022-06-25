
#define CG_Gizmo
// #define CG_Privado

using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
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
        private Retangulo obj_Retangulo;
        private Circulo circulo_obj;
        private int JOGADOR = 3;
        private int BARREIRA = 1;
        private int FINAL = 5;

        int[][] map = new[]
        {
            new[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
            new[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            new[] { 1, 0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            new[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            new[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            new[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            new[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            new[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            new[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            new[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            new[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            new[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            new[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            new[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            new[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
        };
        
        protected List<Cubo> cubos = new List<Cubo>();
        protected Retangulo chao;

        private float fovy, aspect, near, far;
        private Vector3 eye, at, up;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            camera.xmin = -300;
            camera.xmax = 300;
            camera.ymin = -300;
            camera.ymax = 300;
            
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);

            // ___ parâmetros da câmera sintética
            fovy = (float)Math.PI / 4;
            aspect = Width / (float)Height;
            near = 1.0f;
            far = 50.0f;
            eye = new Vector3(10, 10, 10);
            at = new Vector3(0, 0, 0);
            up = new Vector3(0, 1, 0);

            Console.WriteLine(" --- Ajuda / Teclas: ");
            Console.WriteLine(" [  H     ] mostra teclas usadas. ");

            objetoId = Utilitario.charProximo(objetoId);
            objetoSelecionado = null;
            
            // Retangulo
            Retangulo chao = new Retangulo(objetoId, null,
                new Ponto4D(-100, 100),
                new Ponto4D(100, 100)
                );
            chao.ObjetoCor.CorR = 255;
            objetosLista.Add(chao);
            
            // CUBO
            var cubo = new Cubo(objetoId, null);
            Transformacao4D t = new Transformacao4D();
            t.AtribuirTranslacao(500, 500, 500);

            cubo.ObjetoCor.CorB = 205;
            
            cubos.Add(cubo);

            GL.ClearColor(0.5f, 0.5f, 0.5f, 1.0f);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            //GL.MatrixMode(MatrixMode.Projection);
            //GL.LoadIdentity();
            //GL.Ortho(camera.xmin, camera.xmax, camera.ymin, camera.ymax, camera.zmin, camera.zmax);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            // base.OnRenderFrame(e);
            // GL.Clear(ClearBufferMask.ColorBufferBit);
            // GL.MatrixMode(MatrixMode.Modelview);
            // GL.LoadIdentity();
            //
            // Sru3D();
            //
            // for (var i = 0; i < objetosLista.Count; i++)
            //     objetosLista[i].Desenhar();
            // if (bBoxDesenhar && (objetoSelecionado != null))
            //     objetoSelecionado.BBox.Desenhar();
            // this.SwapBuffers();
            
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Matrix4 modelview = Matrix4.LookAt(eye, at, up);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref modelview);
            Sru3D();
            
            for (var i = 0; i < objetosLista.Count; i++)
                objetosLista[i].Desenhar();

            for (var i = 0; i < cubos.Count; i++)
                cubos[i].Desenhar();

            this.SwapBuffers();
        }

        protected override void OnKeyDown(OpenTK.Input.KeyboardKeyEventArgs e)
        {
            if (e.Key == Key.H)
                Utilitario.AjudaTeclado();
            else if (e.Key == Key.M)
            {
                foreach (var line in map)
                {
                    foreach (var i in line)
                    {
                        Console.Write($"{i} ");
                    }

                    Console.WriteLine();
                }
            }
            else if (e.Key == Key.Escape)
                Exit();
            else if (e.Key == Key.E)
            {
                Console.WriteLine("--- Objetos / Pontos: ");
                for (var i = 0; i < objetosLista.Count; i++)
                {
                    Console.WriteLine(objetosLista[i]);
                }
            }
            else if (e.Key == Key.O)
                bBoxDesenhar = !bBoxDesenhar;
            else if (e.Key == Key.V)
                mouseMoverPto = !mouseMoverPto; //TODO: falta atualizar a BBox do objeto
            else
                Console.WriteLine(" __ Tecla não implementada.");
        }

        private void Sru3D()
        {
            GL.LineWidth(1);
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

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, Width / (float)Height, near, far);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref projection);
        }

    }

    class Program
    {
        static void Main(string[] args)
        {
            Mundo window = Mundo.GetInstance(600, 600);
            window.Title = "CG_N4";
            window.Run(1.0 / 60.0);
        }
    }
}
