using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Audio.OpenAL;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using StbImageSharp;

using ComputerGraphics_lab2;

namespace ComputerGraphics_lab2
{
    internal class Game : GameWindow
    {
        
        private Shader shaderProgram;
        private Camera camera;
        private bool showStats = false;

        private Texture trainTexture;
        private Texture railTexture;

        private int width, height;

        private List<Vector3> vertices = new List<Vector3>()
        {
            //front face
			new Vector3(-0.5f,  0.5f, 0.5f), //top-left vertice
			new Vector3( 0.5f,  0.5f, 0.5f), //top-right vertice
			new Vector3( 0.5f, -0.5f, 0.5f), //bottom-right vertice
			new Vector3(-0.5f, -0.5f, 0.5f), //botom-left vertice
			//right face
			new Vector3( 0.5f,  0.5f, 0.5f), //top-left vertice
			new Vector3( 0.5f,  0.5f, -0.5f), //top-right vertice
			new Vector3( 0.5f, -0.5f, -0.5f), //bottom-right vertice
			new Vector3( 0.5f, -0.5f, 0.5f), //botom-left vertice
			//back face
			new Vector3(-0.5f,  0.5f, -0.5f), //top-left vertice
			new Vector3( 0.5f,  0.5f, -0.5f), //top-right vertice
			new Vector3( 0.5f, -0.5f, -0.5f), //bottom-right vertice
			new Vector3(-0.5f, -0.5f, -0.5f), //botom-left vertice
			//left face
			new Vector3( -0.5f,  0.5f, 0.5f), //top-left vertice
			new Vector3( -0.5f,  0.5f, -0.5f), //top-right vertice
			new Vector3( -0.5f, -0.5f, -0.5f), //bottom-right vertice
			new Vector3( -0.5f, -0.5f, 0.5f), //botom-left vertice
			// top face
			new Vector3(-0.5f,  0.5f, -0.5f), //top-left vertice
			new Vector3( 0.5f,  0.5f, -0.5f), //top-right vertice
			new Vector3( 0.5f, 0.5f, 0.5f), //bottom-right vertice
			new Vector3(-0.5f, 0.5f, 0.5f), //botom-left vertice
			//bottom face
			new Vector3(-0.5f,  -0.5f, -0.5f), //top-left vertice
			new Vector3( 0.5f,  -0.5f, -0.5f), //top-right vertice
			new Vector3( 0.5f, -0.5f, 0.5f), //bottom-right vertice
			new Vector3(-0.5f, -0.5f, 0.5f), //botom-left vertice
        }; // вершины граней (список векторов)

        private List<Vector2> texCoords = new List<Vector2>()
        {
            new Vector2(0f, 1f),
            new Vector2(1f, 1f),
            new Vector2(1f, 0f),
            new Vector2(0f, 0f),

            new Vector2(0f, 1f),
            new Vector2(1f, 1f),
            new Vector2(1f, 0f),
            new Vector2(0f, 0f),

            new Vector2(0f, 1f),
            new Vector2(1f, 1f),
            new Vector2(1f, 0f),
            new Vector2(0f, 0f),

            new Vector2(0f, 1f),
            new Vector2(1f, 1f),
            new Vector2(1f, 0f),
            new Vector2(0f, 0f),

            new Vector2(0f, 1f),
            new Vector2(1f, 1f),
            new Vector2(1f, 0f),
            new Vector2(0f, 0f),

            new Vector2(0f, 1f),
            new Vector2(1f, 1f),
            new Vector2(1f, 0f),
            new Vector2(0f, 0f)
        }; // координаты текстуры

        private uint[] indices =
        {
            0, 1, 2, // top triangle
            2, 3, 0, // bottom triangle

            4, 5, 6,
            6, 7, 4,

            8, 9, 10,
            10, 11, 8,

            12, 13, 14,
            14, 15, 12,

            16, 17, 18,
            18, 19, 16,

            20, 21, 22,
            22, 23, 20,
        }; // порядок отрисовки вершин

        private List<Matrix4> railTransforms = new List<Matrix4>(); // рельсы

        private int VAO; // объект массива вершин (дескриптор)
        private int VBO; // объект буфера вершин (дескриптор)
        private int EBO; // объект буфера элементов (дескриптор)
        private int textureVBO; // объект буфера вершин для текстуры (дескриптор)
        private float yRot = 0f;

        private float trainSpeed = 0f;
        private float trainPosition = 0f;
        private float acceleration = 500f;
        private float maxSpeed = 1000f;
        private float friction = 0.98f;
        private float railCount = 100; // число рельсов
        private float spacing = 1.0f; // расстояние между рельсами

        public Game(int width, int height) : base
        (GameWindowSettings.Default, new NativeWindowSettings() { Title = "3D Train Game" })
        {
            this.CenterWindow(new Vector2i(width, height)); //  центрируется окно - двумерный вектор int-ов
            this.height = height;
            this.width = width;
        }

        private void InitializeBuffers()
        {
            VAO = GL.GenVertexArray(); // создание объекта массива вершин
            VBO = GL.GenBuffer(); // cоздание объекта буффера вершин
            EBO = GL.GenBuffer(); // cоздание объекта буффера элементов
            textureVBO = GL.GenBuffer(); // cоздание объекта буфера вершин для текстуры


            // вершины
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO); // привязка VBO к целевому буфферу вершин
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Count * Vector3.SizeInBytes, vertices.ToArray(), BufferUsageHint.StaticDraw);
            // копирует данные вершин в GPU-память с использованим параметра StaticDraw
            GL.BindVertexArray(VAO); // привязка VAO 
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(0); // включение атрибута вершин
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0); // Отвязка VBO

            // примитивы
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO); // привязка EBO к целевому буфферу элементов
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);
            // копирует данные вершин в GPU-память с использованим параметра StaticDraw
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0); // отвязка EBO

            // текстурные координаты
            GL.BindBuffer(BufferTarget.ArrayBuffer, textureVBO); // привязка VBO к целевому буферу вершин
            GL.BufferData(BufferTarget.ArrayBuffer, texCoords.Count * Vector2.SizeInBytes, texCoords.ToArray(), BufferUsageHint.StaticDraw);
            // копирует данные вершин в GPU-память с использованим параметра StaticDraw 
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0); // Настройка атрибута вершин (показываем на слот номер 1)
            GL.EnableVertexAttribArray(1); // включение атрибута вершин

            GL.BindVertexArray(0); // Отвязка VAO
        }

        protected override void OnLoad()
        {
            InitializeBuffers();

            shaderProgram = new Shader(); // cоздание шейдерной программы
            shaderProgram.LoadShaders(); // загрузка шейдеров

            trainTexture = new Texture("../../../Textures/bombardiro_crocodilo.jpg");
            railTexture = new Texture("../../../Textures/wall.jpg");

            GL.Enable(EnableCap.DepthTest); // выполнение depth-testing

            camera = new Camera(width, height, new Vector3(0f, 2f, 5f)); // cмотрим сверху и немного сзади

            for (int i = 0; i <= railCount; i++)
            {
                // Matrix4 rail = Matrix4.CreateScale(1f, 0.05f, 0.5f) * Matrix4.CreateTranslation(i, -0.55f, -1.5f);
                Matrix4 transform = Matrix4.CreateTranslation(0f, 0f, -i * spacing);
                railTransforms.Add(transform);
            }

            CursorState = CursorState.Grabbed;
            // ShowVersionInfo();
        }


        protected override void OnUnload()
        {
            base.OnUnload();

            GL.DeleteBuffer(VAO);
            GL.DeleteBuffer(VBO);
            GL.DeleteBuffer(EBO);

            trainTexture.Delete();
            railTexture.Delete();

            shaderProgram.DeleteShader();
        }

        protected override void OnRenderFrame(FrameEventArgs args) // рендеринг каждого кадра
        {
            GL.ClearColor(0f, 0.75f, 0.9f, 1f); // цвет фона
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            shaderProgram.UseShader(); // активирует шейдерную программу
            shaderProgram.SetMatrix4("view", camera.GetViewMatrix());
            shaderProgram.SetMatrix4("projection", camera.GetProjection());

            //     // Создание матриц
            //     // Matrix4 model = Matrix4.Identity;
            // Matrix4 view = camera.GetViewMatrix();
            // Matrix4 projection = camera.GetProjection();
            // Matrix4 translation = Matrix4.CreateTranslation(trainPosition, 0f, -1.5f);


            // отправка данных в uniforms
            // GL.UniformMatrix4(modelLocation, true, ref model);
            // GL.UniformMatrix4(viewLocation, true, ref view);
            // GL.UniformMatrix4(projectionLocation, true, ref projection);

            GL.BindVertexArray(VAO); // привязка VAO
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            // GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);


            // Рендер паровоза
            trainTexture.Use(TextureUnit.Texture0); // использование текстуры паровоза
            shaderProgram.SetInt("texture0", 0);
            Matrix4 trainTransform = Matrix4.CreateTranslation(0f, 0f, -trainPosition);
            shaderProgram.SetMatrix4("model", trainTransform); // установка матрицы модели
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);

            // Рендер рельс
            railTexture.Use(TextureUnit.Texture1); // использование текстуры рельсов
            shaderProgram.SetInt("texture0", 1);
            foreach (var railModel in railTransforms)
            {
                shaderProgram.SetMatrix4("model", railModel); // установка матрицы модели
                GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
            }

            /*
            Matrix4 trainModel = Matrix4.CreateRotationY(yRot) * Matrix4.CreateTranslation(0f, 0f, trainPosition);
            int trainModelLoc = GL.GetUniformLocation(shaderProgram.shaderHandle, "model");
            GL.UniformMatrix4(trainModelLoc, true, ref trainModel);

            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
            */
            GL.BindVertexArray(0);

            Context.SwapBuffers();
            base.OnRenderFrame(args);
        }

        protected override void OnUpdateFrame(FrameEventArgs args) // обновление каждого кадра
        {
            if (KeyboardState.IsKeyPressed(Keys.Tab))
                showStats = !showStats; 
            if (showStats)
            {
                Console.Clear();
                Console.WriteLine($"Position: {trainPosition:F2}");
                Console.WriteLine($"Speed: {trainSpeed:F2}");
                Console.WriteLine($"FPS: {(1.0 / args.Time):F2}");
            }

            if (KeyboardState.IsKeyDown(Keys.Escape)) // выход по нажатию клавиши Escape
                Close();

            MouseState mouse = MouseState;
            KeyboardState input = KeyboardState;

            if (input.IsKeyDown(Keys.W) || input.IsKeyDown(Keys.Up)) // обработка движения паровоза
            {
                trainSpeed += acceleration * (float)args.Time;
                if (trainSpeed > maxSpeed)
                    trainSpeed = maxSpeed;
            }
            else if (input.IsKeyDown(Keys.S) || input.IsKeyDown(Keys.Down))
            {
                trainSpeed -= acceleration * (float)args.Time;
                if (trainSpeed < 0f)
                    trainSpeed = 0f;
            }

            trainSpeed *= friction; // трение (замедление)

            trainPosition += trainSpeed * (float)args.Time; // обновление позиции

            // camera.Update(input, mouse, args);

            base.OnUpdateFrame(args);
        }

        protected override void OnResize(ResizeEventArgs e) // изменение размера окна
        {
            base.OnResize(e);
            GL.Viewport(0, 0, e.Width, e.Height);
            this.width = width;
            this.height = height;
        }

        public void ShowVersionInfo()
        {
            Console.WriteLine($"OpenGL:{GL.GetString(StringName.Version)}"); // версия OpenGL
            Console.WriteLine($"GLSL:{GL.GetString(StringName.ShadingLanguageVersion)}"); // версия GLSL
        }

        /*
        private void DrawBox(Matrix4 transform)
        {
            shaderProgram.UseShader();

            // устновка uniform-переменных
            shaderProgram.SetMatrix4("model", transform);
            shaderProgram.SetMatrix4("view", camera.GetViewMatrix());
            shaderProgram.SetMatrix4("projection", camera.GetProjection());

            GL.BindVertexArray(VAO); // привязка VAO куба

            if (texture != 0) // Активируем текстуру, если она используется
            {
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, texture);
                shaderProgram.SetInt("tex0", 0); // если uniform sampler2D tex0
            }

            GL.DrawArrays(PrimitiveType.Triangles, 0, 36); // рисуем куб

            GL.BindVertexArray(0); // отвязываем VAO
        }
        */
    }
}
