﻿using System;
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

namespace ComputerGraphics_lab2
{
    public class Shader
    {
        public int shaderHandle; // идентификатор шейдерной программы (дескриптор)

        public void LoadShaders()
        {
            shaderHandle = GL.CreateProgram(); // создание пустой шейдерной программы (получение идентификатора)

            int vertexShader = GL.CreateShader(ShaderType.VertexShader); // cоздание вершинного шейдера
            GL.ShaderSource(vertexShader, LoadShaderSource("shader.vert")); // загрузка исходного кода шейдера
            GL.CompileShader(vertexShader); // компиляция шейдера

            GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out int success1);
            if (success1 == 0)
            {
                string infoLog = GL.GetShaderInfoLog(vertexShader);
                Console.WriteLine(infoLog);
            }

            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader); // создание фрагментного шейдера
            GL.ShaderSource(fragmentShader, LoadShaderSource("shader.frag")); // загрузка исходного кода шейдера
            GL.CompileShader(fragmentShader); // компиляция шейдера

            GL.GetShader(fragmentShader, ShaderParameter.CompileStatus, out int success2);
            if (success2 == 0)
            {
                string infoLog = GL.GetShaderInfoLog(fragmentShader);
                Console.WriteLine(infoLog);
            }

            GL.AttachShader(shaderHandle, vertexShader); // прикрепление скомпилированного шейдера к шейдерной программе
            GL.AttachShader(shaderHandle, fragmentShader); // прикрепление скомпилированного шейдера к шейдерной программе
            GL.LinkProgram(shaderHandle); // cвязывает шейдеры в единую исполняемую программу

            GL.GetProgram(shaderHandle, GetProgramParameterName.LinkStatus, out int success3);
            if (success3 == 0)
            {
                string infoLog = GL.GetProgramInfoLog(shaderHandle);
                Console.WriteLine("Program link error: " + infoLog);
            }
        }

        public static string LoadShaderSource(string filepath) // загрузка шейдеров
        {
            string shaderSource = "";
            try
            {
                using (StreamReader reader = new StreamReader("../../../Shaders/" + filepath))
                {
                    shaderSource = reader.ReadToEnd(); // cчитывает весь текст файла в cтроку shaderSource
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to load shader source file:" + e.Message);
            }

            return shaderSource;
        }

        public void UseShader()
        {
            GL.UseProgram(shaderHandle);
        }

        public void DeleteShader()
        {
            GL.DeleteShader(shaderHandle);
        }

        public void SetMatrix4(string name, Matrix4 matrix) //  установка матрицы (uniform-переменной) в шейдер (из C# в GLSL)
        {
            int location = GL.GetUniformLocation(shaderHandle, name);
            if (location == -1)
                Console.WriteLine($"Uniform '{name}' not found in shader.");
            else
                GL.UniformMatrix4(location, true, ref matrix);
        }

        public void SetVector3(string name, Vector3 vector) // установка вектора (uniform-переменной) в шейдер (из C# в GLSL)
        {
            int location = GL.GetUniformLocation(shaderHandle, name);
            if (location == -1)
                Console.WriteLine($"Uniform '{name}' not found in shader.");
            else
                GL.Uniform3(location, vector);
        }

        public void SetFloat(string name, float value) // установка float-числа (uniform-переменной) в шейдер (из C# в GLSL)
        {
            int location = GL.GetUniformLocation(shaderHandle, name);
            if (location == -1)
                Console.WriteLine($"Uniform '{name}' not found in shader.");
            else
                GL.Uniform1(location, value);
        }

        public void SetInt(string name, int value) // установка int-числа (uniform-переменной) в шейдер (из C# в GLSL)
        {
            int location = GL.GetUniformLocation(shaderHandle, name);
            if (location == -1)
                Console.WriteLine($"Uniform '{name}' not found in shader.");
            else
                GL.Uniform1(location, value);
        }
    }
}