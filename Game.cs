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
        private Shader skyboxShader;
        private Shader planeShader;
        private Camera camera;
        private bool showStats = false;

        private Texture trainTexture;
        //private Texture railTexture; 
        private Texture railsCenterTexture;
        private Texture groundLeftTexture;
        private Texture groundRightTexture;
        private CubemapTexture cubemapTexture;

        private int width, height;

        private float[] skyboxVertices = 
        {
            
            -1.0f,  1.0f, -1.0f,
            -1.0f, -1.0f, -1.0f,
             1.0f, -1.0f, -1.0f,
             1.0f, -1.0f, -1.0f,
             1.0f,  1.0f, -1.0f,
            -1.0f,  1.0f, -1.0f,

            -1.0f, -1.0f,  1.0f,
            -1.0f, -1.0f, -1.0f,
            -1.0f,  1.0f, -1.0f,
            -1.0f,  1.0f, -1.0f,
            -1.0f,  1.0f,  1.0f,
            -1.0f, -1.0f,  1.0f,

             1.0f, -1.0f, -1.0f,
             1.0f, -1.0f,  1.0f,
             1.0f,  1.0f,  1.0f,
             1.0f,  1.0f,  1.0f,
             1.0f,  1.0f, -1.0f,
             1.0f, -1.0f, -1.0f,

            -1.0f, -1.0f,  1.0f,
            -1.0f,  1.0f,  1.0f,
             1.0f,  1.0f,  1.0f,
             1.0f,  1.0f,  1.0f,
             1.0f, -1.0f,  1.0f,
            -1.0f, -1.0f,  1.0f,

            -1.0f,  1.0f, -1.0f,
             1.0f,  1.0f, -1.0f,
             1.0f,  1.0f,  1.0f,
             1.0f,  1.0f,  1.0f,
            -1.0f,  1.0f,  1.0f,
            -1.0f,  1.0f, -1.0f,

            -1.0f, -1.0f, -1.0f,
            -1.0f, -1.0f,  1.0f,
             1.0f, -1.0f, -1.0f,
             1.0f, -1.0f, -1.0f,
            -1.0f, -1.0f,  1.0f,
             1.0f, -1.0f,  1.0f
            
        }; // вершины скайбокс-куба

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

        private List<Vector3> railPlaneVertices = new List<Vector3>()
        {
            new Vector3(-0.5f, 0.0f,  0.5f),
            new Vector3( 0.5f, 0.0f,  0.5f),
            new Vector3( 0.5f, 0.0f, -0.5f),
            new Vector3(-0.5f, 0.0f, -0.5f)
        }; // вершины плоскости рельсов и земли

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

        private List<Vector2> railPlaneTexCoords = new List<Vector2>()
        {
            new Vector2(0f, 0f),
            new Vector2(1f, 0f),
            new Vector2(1f, 1f),
            new Vector2(0f, 1f)
        }; // координаты текстуры для рельсов и земли

        private uint[] railPlaneIndices = { 0, 1, 2, 2, 3, 0 }; // порядок отрисовки вершин плоскости рельсов и земли

        private uint[] indices =
        {
            // Front face (+Z) - CCW
            0, 2, 1, // Был 0, 1, 2
            2, 0, 3, // Был 2, 3, 0

            // Right face (+X) - CCW
            4, 6, 5, // Был 4, 5, 6
            6, 4, 7, // Был 6, 7, 4

            // Back face (-Z) - CCW
            8, 10, 9, // Был 8, 9, 10
            10, 8, 11,// Был 10, 11, 8

            // Left face (-X) - CCW
            12, 14, 13,// Был 12, 13, 14
            14, 12, 15,// Был 14, 15, 12

            // Top face (+Y) - CCW
            16, 18, 17,// Был 16, 17, 18
            18, 16, 19,// Был 18, 19, 16

            // Bottom face (-Y) - CCW
            20, 22, 21,// Был 20, 21, 22
            22, 20, 23 // Был 22, 23, 20
        }; // порядок отрисовки вершин

        private List<Matrix4> railTransforms = new List<Matrix4>(); // рельсы

        private int VAO; // объект массива вершин (дескриптор)
        private int VBO; // объект буфера вершин (дескриптор)
        private int EBO; // объект буфера элементов (дескриптор)
        private int textureVBO; // объект буфера вершин для текстуры (дескриптор)
        private float yRot = 0f;

        private int skyboxVAO;
        private int skyboxVBO;

        private int railVAO;
        private int railVBO;
        private int railTexCoordVBO;
        private int railEBO;

        private int numSegments = 20; // Количество сегментов (текстур на плоскости)
        private float segmentLength = 100.0f; // Длина одного сегмента
        private float[] segmentZOffsets; // Массив для хранения Z-координаты начала каждого сегмента

        private float trainSpeed = 0f;
        private float trainPosition = 0f;
        private float acceleration = 500f;
        private float maxSpeed = 1000f;
        private float friction = 0.98f;
        private float railCount = 100; // число рельсов
        private float spacing = 1.0f; // расстояние между рельсами
        private float distanceBehind = 12f; // расстояние, на котором камера будет позади поезда
        private bool isInputCaptured = true; // Флаг захвата ввода

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

        private void SetupPlane()
        {
            railVAO = GL.GenVertexArray(); // создание объекта массива вершин
            railVBO = GL.GenBuffer(); // cоздание объекта буффера вершин
            railEBO = GL.GenBuffer(); // cоздание объекта буффера элементов
            railTexCoordVBO = GL.GenBuffer(); // cоздание объекта буфера вершин для текстуры

            // вершины рельсовой плоскости
            GL.BindBuffer(BufferTarget.ArrayBuffer, railVBO); // привязка VBO к целевому буфферу вершин
            GL.BufferData(BufferTarget.ArrayBuffer, railPlaneVertices.Count * Vector3.SizeInBytes, railPlaneVertices.ToArray(), BufferUsageHint.StaticDraw);
            // копирует данные вершин в GPU-память с использованим параметра StaticDraw
            GL.BindVertexArray(railVAO); // привязка VAO 
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(0); // включение атрибута вершин
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0); // Отвязка VBO

            // примитивы
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, railEBO); // привязка EBO к целевому буфферу элементов
            GL.BufferData(BufferTarget.ElementArrayBuffer, railPlaneIndices.Length * sizeof(uint), railPlaneIndices, BufferUsageHint.StaticDraw);
            // копирует данные вершин в GPU-память с использованим параметра StaticDraw
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0); // отвязка EBO

            // текстурные координаты
            GL.BindBuffer(BufferTarget.ArrayBuffer, railTexCoordVBO); // привязка VBO к целевому буферу вершин
            GL.BufferData(BufferTarget.ArrayBuffer, railPlaneTexCoords.Count * Vector2.SizeInBytes, railPlaneTexCoords.ToArray(), BufferUsageHint.StaticDraw);
            // копирует данные вершин в GPU-память с использованим параметра StaticDraw 
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0); // Настройка атрибута вершин (показываем на слот номер 1)
            GL.EnableVertexAttribArray(1); // включение атрибута вершин

            GL.BindVertexArray(0); // Отвязка VAO
        }

        private void SetupSkybox()
        {
            // Генерация VAO и VBO
            skyboxVAO = GL.GenVertexArray();
            skyboxVBO = GL.GenBuffer();

            // Привязываем VAO
            GL.BindVertexArray(skyboxVAO);

            // Привязываем VBO и загружаем данные
            GL.BindBuffer(BufferTarget.ArrayBuffer, skyboxVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, skyboxVertices.Length * sizeof(float), skyboxVertices, BufferUsageHint.StaticDraw);

            // Устанавливаем атрибуты вершины
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0); // layout(location = 0)

            // Отвязываем VAO
            GL.BindVertexArray(0);
        }

        protected override void OnLoad()
        {
            InitializeBuffers();
            SetupPlane();
            SetupSkybox(); 

            shaderProgram = new Shader("Shader.vert", "Shader.frag"); // cоздание шейдерной программы
            skyboxShader = new Shader("Skybox.vert", "Skybox.frag");
            planeShader = new Shader("Plane.vert", "Plane.frag");

            trainTexture = new Texture("../../../Textures/bombardiro_crocodilo.jpg");
            // railTexture = new Texture("../../../Textures/wide_desert_rails.jpg");
            railsCenterTexture = new Texture("../../../Textures/rails_center.jpg");
            groundLeftTexture = new Texture("../../../Textures/ground_left.jpg");
            groundRightTexture = new Texture("../../../Textures/ground_right.jpg");
            string[] faces = new string[]
            {
                "../../../Textures/Skybox/right.jpg",  // GL_TEXTURE_CUBE_MAP_POSITIVE_X (Right +X)
                "../../../Textures/Skybox/left.jpg",   // GL_TEXTURE_CUBE_MAP_NEGATIVE_X (Left -X)
                "../../../Textures/Skybox/top.jpg",    // GL_TEXTURE_CUBE_MAP_POSITIVE_Y (Top +Y)
                "../../../Textures/Skybox/bottom.jpg", // GL_TEXTURE_CUBE_MAP_NEGATIVE_Y (Bottom -Y)
                "../../../Textures/Skybox/front.jpg",  // GL_TEXTURE_CUBE_MAP_POSITIVE_Z (Front +Z)
                "../../../Textures/Skybox/back.jpg"    // GL_TEXTURE_CUBE_MAP_NEGATIVE_Z (Back -Z)
            };
            cubemapTexture = new CubemapTexture(faces);

            GL.Enable(EnableCap.DepthTest); // выполнение depth-testing

            camera = new Camera(width, height, new Vector3(0f, 2f, 5f)); // cмотрим сверху и немного сзади

            segmentZOffsets = new float[numSegments];
            float initialForwardOffset = (float)(numSegments / 2) * segmentLength;
            for (int i = 0; i < numSegments; i++)
            {
                // Расставляем сегменты друг за другом, начиная от initialForwardOffset и уходя в минус
                segmentZOffsets[i] = initialForwardOffset - i * segmentLength; // начало i-ого сегмента
            }

            if (isInputCaptured)
            {
                CursorState = CursorState.Grabbed;
            }
            else
            {
                CursorState = CursorState.Normal;
            }

            camera.ResetFirstMove();
            // ShowVersionInfo();
        }

        protected override void OnUnload()
        {
            base.OnUnload();

            GL.DeleteBuffer(VAO);
            GL.DeleteBuffer(VBO);
            GL.DeleteBuffer(EBO);

            GL.DeleteBuffer(textureVBO);
            GL.DeleteVertexArray(skyboxVAO);
            GL.DeleteBuffer(skyboxVBO);

            trainTexture.Delete();
            // railTexture.Delete();
            railsCenterTexture.Delete();
            groundLeftTexture.Delete();
            groundRightTexture.Delete();
            cubemapTexture.Delete();

            shaderProgram.DeleteShader();
            skyboxShader.DeleteShader();
            planeShader.DeleteShader();
        }

        protected override void OnRenderFrame(FrameEventArgs args) // рендеринг каждого кадра
        {
            GL.Enable(EnableCap.DepthTest);

            GL.ClearColor(0f, 0.75f, 0.9f, 1f); // цвет фона
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Matrix4 viewMatrix = camera.GetViewMatrix();
            Matrix4 projectionMatrix = camera.GetProjection();

            // Рендер паровоза
            shaderProgram.UseShader(); // активирует шейдерную программу
            shaderProgram.SetMatrix4("view", viewMatrix);
            shaderProgram.SetMatrix4("projection", projectionMatrix);
            shaderProgram.SetInt("texture0", 0);
            trainTexture.Use(TextureUnit.Texture0); // использование текстуры паровоза

            GL.BindVertexArray(VAO); // Привязываем VAO паровоза
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);

            // поезд не симметричен относительно центра рельсов (подвину чуть вправо по x)
            Matrix4 trainTransform = Matrix4.CreateScale(4f, 4f, 4f)*Matrix4.CreateTranslation(0.3f, 0f, -trainPosition); 
            shaderProgram.SetMatrix4("model", trainTransform); // установка матрицы модели
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);

            // Рендер плоскости рельсов
            planeShader.UseShader(); // активирует шейдерную программу
            planeShader.SetMatrix4("view", viewMatrix);
            planeShader.SetMatrix4("projection", projectionMatrix);
            planeShader.SetInt("texture0", 0);
            // railTexture.Use(TextureUnit.Texture0); // использование текстуры рельсов

            GL.BindVertexArray(railVAO); // Привязываем VAO плоскости рельсов
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, railEBO); // Привязываем EBO плоскости рельсов

            // --- Параметры плиток ---
            float tileWidthCenter = 30.0f;  // Ширина центральной плитки (рельсы) в мире
            float tileWidthSide = 15.0f;   // Ширина БОКОВОЙ плитки (земля) в мире
            float groundOffsetY = -1.0f; // Чуть ниже рельсов
            float railsOffsetY = -1.0f;  // Положение рельсов
            int numSideTiles = 50; // Сколько плиток земли рисовать С КАЖДОЙ стороны (подбери)

            // --- Масштаб текстурных координат для ОДНОЙ плитки ---
            // Мы НЕ тайлим текстуру ВНУТРИ одной плитки, поэтому масштаб = 1
            planeShader.SetVector2("texScale", Vector2.One);
            // Смещение текстурных координат не нужно для этого способа
            // planeShader.SetVector2("texOffset", Vector2.Zero);

            // --- Основной цикл по СЕГМЕНТАМ ВДОЛЬ пути ---
            for (int segmentIndex = 0; segmentIndex < numSegments; segmentIndex++)
            {
                // Z-координата центра текущего сегмента
                float currentSegmentZ = segmentZOffsets[segmentIndex] - segmentLength / 2.0f;

                // --- 1. Рисуем ЛЕВЫЕ плитки земли ---
                groundLeftTexture.Use(TextureUnit.Texture0); // Используем текстуру левой земли
                Matrix4 leftTileScale = Matrix4.CreateScale(tileWidthSide, 1.0f, segmentLength); // Масштаб для одной левой плитки
                for (int tileIndex = 1; tileIndex <= numSideTiles; tileIndex++)
                {
                    // X-координата центра текущей левой плитки
                    float currentTileX = -(tileWidthCenter / 2.0f) - (tileIndex - 0.5f) * tileWidthSide;
                    Matrix4 leftTileTranslate = Matrix4.CreateTranslation(currentTileX, groundOffsetY, currentSegmentZ);
                    Matrix4 leftTileModel = leftTileScale * leftTileTranslate;
                    planeShader.SetMatrix4("model", leftTileModel);
                    GL.DrawElements(PrimitiveType.Triangles, railPlaneIndices.Length, DrawElementsType.UnsignedInt, 0);
                }

                // --- 2. Рисуем ЦЕНТРАЛЬНУЮ плитку (рельсы) ---
                railsCenterTexture.Use(TextureUnit.Texture0); // Используем текстуру рельсов
                Matrix4 centerTileScale = Matrix4.CreateScale(tileWidthCenter, 1.0f, segmentLength); // Масштаб для центральной плитки
                Matrix4 centerTileTranslate = Matrix4.CreateTranslation(0f, railsOffsetY, currentSegmentZ); // Центр по X=0
                Matrix4 centerTileModel = centerTileScale * centerTileTranslate;
                planeShader.SetMatrix4("model", centerTileModel);
                GL.DrawElements(PrimitiveType.Triangles, railPlaneIndices.Length, DrawElementsType.UnsignedInt, 0);

                // --- 3. Рисуем ПРАВЫЕ плитки земли ---
                groundRightTexture.Use(TextureUnit.Texture0); // Используем текстуру правой земли
                Matrix4 rightTileScale = Matrix4.CreateScale(tileWidthSide, 1.0f, segmentLength); // Масштаб для одной правой плитки
                for (int tileIndex = 1; tileIndex <= numSideTiles; tileIndex++)
                {
                    // X-координата центра текущей правой плитки
                    float currentTileX = (tileWidthCenter / 2.0f) + (tileIndex - 0.5f) * tileWidthSide;
                    Matrix4 rightTileTranslate = Matrix4.CreateTranslation(currentTileX, groundOffsetY, currentSegmentZ);
                    Matrix4 rightTileModel = rightTileScale * rightTileTranslate;
                    planeShader.SetMatrix4("model", rightTileModel);
                    GL.DrawElements(PrimitiveType.Triangles, railPlaneIndices.Length, DrawElementsType.UnsignedInt, 0);
                }
            }

            // Отвязываем VAO после использования
            GL.BindVertexArray(0);


            /*
            float planeScaleX = 10.0f; // Ширина плоскости
            float planeOffsetY = -1f; // Положение по Y (под поездом)

            float tileWidth = planeScaleX;
            float tileHeight = 2.0f;

            Vector2 texScale = new Vector2(planeScaleX / tileWidth, segmentLength / tileHeight);
            planeShader.SetVector2("texScale", texScale); // передаем масштаб текстуры в шейдер

            Matrix4 segmentScaleMatrix = Matrix4.CreateScale(planeScaleX, 1.0f, segmentLength); // матриеца масштабирования для сегмента

            for (int i = 0; i < numSegments; i++)
            {
                Matrix4 segmentTranslateMatrix = Matrix4.CreateTranslation(0f, planeOffsetY, segmentZOffsets[i] - segmentLength / 2.0f);
                Matrix4 segmentModel = segmentScaleMatrix * segmentTranslateMatrix;
                planeShader.SetMatrix4("model", segmentModel); // установка матрицы модели
                GL.DrawElements(PrimitiveType.Triangles, railPlaneIndices.Length, DrawElementsType.UnsignedInt, 0);
            }
            */

            // Рендер скайбокса
            GL.DepthFunc(DepthFunction.Lequal); // включение глубины для отрисовки скайбокса
            GL.Disable(EnableCap.CullFace);

            skyboxShader.UseShader();
            Matrix4 skyboxView = new Matrix4(new Matrix3(viewMatrix)); // удаляем трансляцию

            skyboxShader.SetMatrix4("view", skyboxView);
            skyboxShader.SetMatrix4("projection", projectionMatrix);

            skyboxShader.SetInt("skybox", 0); // Сообщаем шейдеру читать из юнита 0
            cubemapTexture.Use(TextureUnit.Texture0);

            GL.BindVertexArray(skyboxVAO);
            
            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);

            GL.BindVertexArray(0);
            // GL.Enable(EnableCap.CullFace);
            GL.DepthFunc(DepthFunction.Less); // Вернуть обратно, если нужно
            

            Context.SwapBuffers();
            base.OnRenderFrame(args);
        }

        protected override void OnUpdateFrame(FrameEventArgs args) // обновление каждого кадра
        {
            if (!IsFocused) // Не обрабатываем ввод, если окно не в фокусе
            {
                if (isInputCaptured)
                {
                    isInputCaptured = false;
                    CursorState = CursorState.Normal;
                    // Сообщаем камере, что следующее движение будет первым
                    camera.ResetFirstMove();
                }
                return; // Не обрабатываем ввод, если окно не в фокусе
            }

            // Получаем состояния
            MouseState mouse = MouseState;
            KeyboardState input = KeyboardState;

            if (KeyboardState.IsKeyPressed(Keys.F)) // обработак фокуса
            {
                isInputCaptured = !isInputCaptured;

                if (isInputCaptured)
                {
                    CursorState = CursorState.Grabbed; // Захватываем и скрываем курсор
                    camera.ResetFirstMove();
                }
                else
                {
                    CursorState = CursorState.Normal; // Освобождаем и показываем курсор
                }
            }

            if (isInputCaptured)
            {
                camera.UpdateMouseLook(mouse, args);
            }

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

            camera.position = new Vector3(0f, 4f, - trainPosition + distanceBehind);

            // логика переиспользования сегментов рельсов
            float cameraZ = camera.position.Z;
            for (int i = 0; i < numSegments; i++)
            {
                float segmentFarEdgeZ = segmentZOffsets[i] - segmentLength; // дальний край i-ого сегмента
                float segmentNearEdgeZ = segmentZOffsets[i]; // ближний край i-ого сегмента
                float thresholdZ = cameraZ + distanceBehind + segmentLength; // пороговая Z-координата
                if (segmentFarEdgeZ > thresholdZ) 
                {
                    float mostNegativeZ = 0f; // координата самого дальнего сегмента на данный момент
                    for (int j = 0; j < numSegments; j++)
                    {
                        if (segmentZOffsets[j] < mostNegativeZ) // i-ый сегмент дальше
                        { 
                            mostNegativeZ = segmentZOffsets[j];
                        }
                    }
                    segmentZOffsets[i] = mostNegativeZ - segmentLength; // перемещаем сегмент в конец
                }
            }

            base.OnUpdateFrame(args);
        }

        protected override void OnResize(ResizeEventArgs e) // изменение размера окна
        {
            base.OnResize(e);
            GL.Viewport(0, 0, e.Width, e.Height);
            camera?.OnResize(e.Width, e.Height);
            //this.width = width;
            //this.height = height;
        }

        public void ShowVersionInfo()
        {
            Console.WriteLine($"OpenGL:{GL.GetString(StringName.Version)}"); // версия OpenGL
            Console.WriteLine($"GLSL:{GL.GetString(StringName.ShadingLanguageVersion)}"); // версия GLSL
        }
    }
}
