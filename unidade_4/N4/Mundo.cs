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
        private int JOGADOR_X, JOGADOR_Z;
        private int BARREIRA = 1;
        private int FINAL = 5;
        private int VIDAS = 5;
        private int switch_color_counter = 0;
        private bool primeira_pessoa = false;

        int[][] map = new[]
        {
            new[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
            new[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            new[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1 },
            new[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            new[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1 },
            new[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1 },
            new[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1 },
            new[] { 1, 0, 0, 0, 0, 0, 3, 0, 0, 0, 1, 0, 0, 0, 1 },
            new[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1 },
            new[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 1 },
            new[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1 },
            new[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 1 },
            new[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 5, 0, 1 },
            new[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1 },
            new[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
        };

        protected List<Cubo> cubos = new List<Cubo>();
        protected Retangulo chao;
        protected Cubo jogador;

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
            cameraAerea();

            //Console.WriteLine(" --- Ajuda / Teclas: ");
            //Console.WriteLine(" [  H     ] mostra teclas usadas. ");

            objetoId = Utilitario.charProximo(objetoId);
            objetoSelecionado = null;

            // Chao
            Chao chao = new Chao(objetoId, null);
            chao.TranslacaoXYZ(0, -2, 0);
            chao.EscalaXYZ(1000, 0, 1000);
            chao.ObjetoCor.CorR = 147;
            chao.ObjetoCor.CorG = 194;
            chao.ObjetoCor.CorB = 141;
            objetosLista.Add(chao);

            var tx = 0;
            var tz = 0;

            for (int i = 0; i < map.Length; i++)
            {
                for (int j = 0; j < map[i].Length; j++)
                {
                    if (map[i][j] == BARREIRA)
                    {
                        objetoId = Utilitario.charProximo(objetoId);
                        var cubo = new Cubo(objetoId, null);
                        cubo.TranslacaoXYZ(tx, 0, tz);
                        switch_color(cubo);
                        cubos.Add(cubo);
                    }
                    else if (map[i][j] == JOGADOR)
                    {
                        objetoId = Utilitario.charProximo(objetoId);
                        jogador = new Cubo(objetoId, null);
                        jogador.ObjetoCor.CorR = 10;
                        jogador.ObjetoCor.CorG = 166;
                        jogador.ObjetoCor.CorB = 201;
                        JOGADOR_X = i;
                        JOGADOR_Z = j;
                    }
                    else if (map[i][j] == FINAL)
                    {
                        objetoId = Utilitario.charProximo(objetoId);
                        var cubo = new Cubo(objetoId, null);
                        cubo.EscalaXYZ(0.3, 0.3, 0.3);
                        cubo.TranslacaoXYZ(tx, 0, tz);
                        cubo.ObjetoCor.CorR = 250;
                        cubo.ObjetoCor.CorG = 227;
                        cubo.ObjetoCor.CorB = 17;
                        cubos.Add(cubo);
                    }

                    tz += 2;
                }

                tz = 0;
                tx += 2;
            }

            Console.WriteLine("Bem Vindo ao labirinto !!!!");
            Console.WriteLine($"Voce possui {VIDAS} vidas!");


            GL.ClearColor(0.5f, 0.5f, 0.5f, 1.0f);
        }

        protected void switch_color(Cubo cubo)
        {
            var cor = Convert.ToByte(50 * switch_color_counter);
            cubo.ObjetoCor.CorR = cor;
            cubo.ObjetoCor.CorG = cor;
            cubo.ObjetoCor.CorB = cor;

            switch_color_counter++;

            if (switch_color_counter > 4)
            {
                switch_color_counter = 0;
            }
        }

        protected void cameraAerea()
        {
            fovy = (float)Math.PI / 4;
            aspect = Width / (float)Height;
            near = 1.0f;
            far = 500.0f;
            eye = new Vector3(15, 40, 50);
            at = new Vector3(15, 0, 10);
            up = new Vector3(0, 1, 0);
        }
        
        protected void cameraPrimeiraPessoa()
        {
            fovy = (float)Math.PI / 4;
            aspect = Width / (float)Height;
            near = 1.0f;
            far = 500.0f;
            eye = new Vector3(JOGADOR_X, 1, JOGADOR_Z);
            at = new Vector3(JOGADOR_X, 1, JOGADOR_Z + 1);
            up = new Vector3(0, 1, 0);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            //GL.MatrixMode(MatrixMode.Projection);
            //GL.LoadIdentity();
            //GL.Ortho(camera.xmin, camera.xmax, camera.ymin, camera.ymax, camera.zmin, camera.zmax);
        }

        protected void desenhaJogador()
        {
            jogador.AtribuirIdentidade();
            jogador.EscalaXYZ(0.7, 0.7, 0.7);
            jogador.TranslacaoXYZ(2 * JOGADOR_X, 0, 2 * JOGADOR_Z);
            jogador.Desenhar();
            if (primeira_pessoa)
            {
                cameraPrimeiraPessoa();
            }
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

            desenhaJogador();

            this.SwapBuffers();
        }

        protected void colisao()
        {
            VIDAS--;
            Console.WriteLine("Voce acaba de ter uma colisao. Perdeu uma vida!!!");
            Console.WriteLine($"Vidas Restantes: {VIDAS} vidas");

            if (VIDAS == 0)
            {
                Console.WriteLine($"Voce Perdeu!!!!");
                // TODO: Encerrar
            }
        }

        protected void moverJogadorSemPerspectiva(bool is_eixo_x, int new_point)
        {
            if (is_eixo_x)
            {
                if (map[new_point][JOGADOR_Z] == BARREIRA)
                {
                    colisao();
                    return;
                }

                if (map[new_point][JOGADOR_Z] == FINAL)
                {
                    ganhou();
                    return;
                }

                map[JOGADOR_X][JOGADOR_Z] = 0;
                JOGADOR_X = new_point;
                map[JOGADOR_X][JOGADOR_Z] = JOGADOR;
            }
            else
            {
                if (map[JOGADOR_X][new_point] == BARREIRA)
                {
                    colisao();
                    return;
                }

                if (map[JOGADOR_X][new_point] == FINAL)
                {
                    ganhou();
                    return;
                }

                map[JOGADOR_X][JOGADOR_Z] = 0;
                JOGADOR_Z = new_point;
                map[JOGADOR_X][JOGADOR_Z] = JOGADOR;
            }
        }

        protected void ganhou()
        {
            Console.WriteLine($"Voce Ganhou!!!!");
            // TODO: Encerrar
        }

        protected override void OnKeyDown(OpenTK.Input.KeyboardKeyEventArgs e)
        {
            if (e.Key == Key.H)
                Utilitario.AjudaTeclado();
            else if (e.Key == Key.Left)
                moverJogadorSemPerspectiva(true, JOGADOR_X - 1);
            else if (e.Key == Key.Right)
                moverJogadorSemPerspectiva(true, JOGADOR_X + 1);
            else if (e.Key == Key.Up)
                moverJogadorSemPerspectiva(false, JOGADOR_Z - 1);
            else if (e.Key == Key.Down)
                moverJogadorSemPerspectiva(false, JOGADOR_Z + 1);
            else if (e.Key == Key.Number1)
            {
                primeira_pessoa = false;
                cameraAerea();
            }
            else if (e.Key == Key.Number2)
            {
                primeira_pessoa = true;
            }
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
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(fovy, aspect, near, far);
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