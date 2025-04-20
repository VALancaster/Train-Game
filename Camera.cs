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

namespace ComputerGraphics_lab2
{
        internal class Camera
        {
            private int SCREENWIDTH;
            private int SCREENHEIGHT;
            private float SENSITIVITY = 3f;

            public Vector3 position;
            Vector3 up = Vector3.UnitY;
            Vector3 front = -Vector3.UnitZ; // ось Z направлена к нам
            Vector3 right = Vector3.UnitX;

            private float pitch;
            private float yaw = -90.0f;

            private bool firstMove = true;
            public Vector2 lastPos;

            public Camera(int width, int height, Vector3 position)
            {
                SCREENWIDTH = width;
                SCREENHEIGHT = height;
                this.position = position;
                UpdateVectors();
            }

            public void ResetFirstMove() // будет сообщать камере, что следующее движение будет первым, чтобы она не прыгнула из-за накопленного смещения
            {
                this.firstMove = true;
            }

            public Matrix4 GetViewMatrix()
            {
                return Matrix4.LookAt(position, position + front, up);
            }

            public Matrix4 GetProjection()
            {
                float aspectRatio = 1.0f;
                if (SCREENHEIGHT > 0)
                {
                    aspectRatio = (float)SCREENWIDTH / SCREENHEIGHT;
                }
                float nearPlane = 0.1f;
                float farPlane = 10000f;
                return Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60f), aspectRatio, nearPlane, farPlane);
            }

            private void UpdateVectors()
            {

                if (pitch > 89.0f)
                {
                    pitch = 89.0f;
                }
                if (pitch < -89.0f)
                {
                    pitch = -89.0f;
                }

                front.X = MathF.Cos(MathHelper.DegreesToRadians(pitch)) * MathF.Cos(MathHelper.DegreesToRadians(yaw));
                front.Y = MathF.Sin(MathHelper.DegreesToRadians(pitch));
                front.Z = MathF.Cos(MathHelper.DegreesToRadians(pitch)) * MathF.Sin(MathHelper.DegreesToRadians(yaw));

                front = Vector3.Normalize(front);
                right = Vector3.Normalize(Vector3.Cross(front, Vector3.UnitY));
                up = Vector3.Normalize(Vector3.Cross(right, front));
            }

            public void UpdateMouseLook(MouseState mouse, FrameEventArgs e)
            {
                if (firstMove)
                {
                    // Инициализируем lastPos текущей позицией мыши
                    lastPos = new Vector2(mouse.X, mouse.Y);
                    firstMove = false;
                    return;
                }
                else
                {
                    // Вычисляем смещение мыши с последнего кадра
                    var deltaX = mouse.X - lastPos.X;
                    var deltaY = mouse.Y - lastPos.Y;
                    lastPos = new Vector2(mouse.X, mouse.Y); // Обновляем последнюю позицию

                    // Применяем чувствительность и время кадра
                    yaw += deltaX * SENSITIVITY * (float)e.Time;
                    pitch -= deltaY * SENSITIVITY * (float)e.Time; // Y инвертирован

                    // Ограничиваем угол pitch, чтобы избежать "переворота"
                    if (pitch > 89.0f)
                        pitch = 89.0f;
                    if (pitch < -89.0f)
                        pitch = -89.0f;

                    // Обновляем векторы front, right и up на основе новых углов yaw и pitch
                    UpdateVectors();
                }
            }

            public void OnResize(int width, int height)
            {
                SCREENWIDTH = width;
                SCREENHEIGHT = height;
                // Сбросим firstMove, чтобы lastPos обновился после возможного изменения координат мыши
                // Хотя при CursorState.Grabbed это может быть не нужно, но не помешает
                firstMove = true;
            }



    }
}