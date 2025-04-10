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

namespace CG_lab2
{
    internal class Game : GameWindow
    {
        private Shader shaderProgram;
        private Camera camera;
        private bool showStats = false;
        // private Train train;

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
        private int textureID; // дескриптор текстуры
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

        protected override void OnLoad()
        {

            VAO = GL.GenVertexArray(); // создание объекта массива вершин
            VBO = GL.GenBuffer(); // cоздание объекта буффера вершин

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO); // привязка VBO к целевому буфферу вершин
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Count * Vector3.SizeInBytes, vertices.ToArray(), BufferUsageHint.StaticDraw);
            // копирует данные вершин в GPU-память с использованим параметра StaticDraw
            GL.BindVertexArray(VAO); // привязка VAO 
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);

            GL.EnableVertexAttribArray(0); // включение атрибута вершин
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0); // Отвязка VBO

            EBO = GL.GenBuffer(); // cоздание объекта буффера элементов
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO); // привязка EBO к целевому буфферу элементов
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);
            // копирует данные вершин в GPU-память с использованим параметра StaticDraw
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0); // отвязка EBO

            textureVBO = GL.GenBuffer(); // cоздание объекта буфера вершин для текстуры
            GL.BindBuffer(BufferTarget.ArrayBuffer, textureVBO); // привязка VBO к целевому буферу вершин
            GL.BufferData(BufferTarget.ArrayBuffer, texCoords.Count * Vector2.SizeInBytes, texCoords.ToArray(), BufferUsageHint.StaticDraw);
            // копирует данные вершин в GPU-память с использованим параметра StaticDraw 
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0); // Настройка атрибута вершин (показываем на слот номер 1)
            GL.EnableVertexAttribArray(1); // включение атрибута вершин

            shaderProgram = new Shader(); // cоздание шейдерной программы
            shaderProgram.LoadShaders(); // загрузка шейдеров

            textureID = GL.GenTexture(); // создание пустой текстуры

            GL.ActiveTexture(TextureUnit.Texture0); // активирует текстурный блок 0
            GL.BindTexture(TextureTarget.Texture2D, textureID); // привязывает текстуру к текстурному блоку

            // параметры текстуры
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            StbImage.stbi_set_flip_vertically_on_load(1); // включение вертикального отражения при загрузке
            ImageResult boxTexture = ImageResult.FromStream(File.OpenRead("../../../Textures/bombardiro_crocodilo.jpg"), ColorComponents.RedGreenBlueAlpha);
            // загружает файл изображения в память и конвертирует его в формат с компонентами RGBA
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, boxTexture.Width, boxTexture.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, boxTexture.Data);
            // передает данные изображения в GPU
            GL.BindTexture(TextureTarget.Texture2D, 0); // отвязка текстуры от текстурного блока

            GL.BindVertexArray(0); // Отвязка VAO

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

            // GL.DeleteProgram(shaderProgram); // высвобождение ресурсов - НИ В КОЕМ СЛУЧАЕ НЕЛЬЗЯ ДЕЛАТЬ
        }

        protected override void OnUnload()
        {
            base.OnUnload();

            GL.DeleteBuffer(VAO);
            GL.DeleteBuffer(VBO);
            GL.DeleteBuffer(EBO);
            GL.DeleteTexture(textureID);

            shaderProgram.DeleteShader();
        }

        protected override void OnRenderFrame(FrameEventArgs args) // рендеринг каждого кадра
        {
            GL.ClearColor(0.3f, 0.3f, 1f, 1f); // цвет фона
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            shaderProgram.UseShader(); // активирует шейдерную программу
            GL.BindTexture(TextureTarget.Texture2D, textureID); // привязка текстуры

            // Создание матриц
            // Matrix4 model = Matrix4.Identity;
            Matrix4 view = camera.GetViewMatrix();
            Matrix4 projection = camera.GetProjection();
            // Matrix4 translation = Matrix4.CreateTranslation(trainPosition, 0f, -1.5f);



            // отправка на GPU
            // int modelLocation = GL.GetUniformLocation(shaderProgram.shaderHandle, "model");
            int viewLocation = GL.GetUniformLocation(shaderProgram.shaderHandle, "view");
            int projectionLocation = GL.GetUniformLocation(shaderProgram.shaderHandle, "projection");

            // отправка данных в uniforms
            // GL.UniformMatrix4(modelLocation, true, ref model);
            GL.UniformMatrix4(viewLocation, true, ref view);
            GL.UniformMatrix4(projectionLocation, true, ref projection);

            GL.BindVertexArray(VAO); // привязка VAO
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            // GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);

            foreach (var railModel in railTransforms)
            {
                Matrix4 model = railModel;
                int modelLocation = GL.GetUniformLocation(shaderProgram.shaderHandle, "model");
                GL.UniformMatrix4(modelLocation, true, ref model);

                GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
            }

            Matrix4 trainModel = Matrix4.CreateRotationY(yRot) * Matrix4.CreateTranslation(0f, 0f, trainPosition);
            int trainModelLoc = GL.GetUniformLocation(shaderProgram.shaderHandle, "model");
            GL.UniformMatrix4(trainModelLoc, true, ref trainModel);

            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);

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
    }
}
