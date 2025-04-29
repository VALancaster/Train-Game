using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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

namespace ComputerGraphics_lab2
{
    public class Texture
    {
        public int textureID; // идентификатор текстуры 

        protected Texture() { } // конструктор

        public Texture(string path) // конструктор
        {
            // 1. Генерация и бинд
            textureID = GL.GenTexture(); // создание пустой текстуры
            GL.ActiveTexture(TextureUnit.Texture0); // активирует текстурный блок 0
            GL.BindTexture(TextureTarget.Texture2D, textureID); // привязывает текстуру к текстурному блоку

            // 2. Параметры текстуры
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            // 3. Загрузка изображения
            StbImage.stbi_set_flip_vertically_on_load(1); // включение вертикального отражения при загрузке
            ImageResult boxTexture = ImageResult.FromStream(File.OpenRead(path), ColorComponents.RedGreenBlueAlpha);
            // загружает файл изображения в память и конвертирует его в формат с компонентами RGBA
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, boxTexture.Width, boxTexture.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, boxTexture.Data);
            // передает данные изображения в GPU

            // 4. Генерация мип-карт
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            GL.BindTexture(TextureTarget.Texture2D, 0); // отвязка текстуры от текстурного блока

        }

        public virtual void Use(TextureUnit unit = TextureUnit.Texture0) // использование текстуры
        {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, textureID);
        }

        public virtual void Delete() // удаление текстуры
        {
            GL.DeleteTexture(textureID);
        }
    }

    
    public class CubemapTexture : Texture
    {
        private static readonly TextureTarget[] targets =
        {
            TextureTarget.TextureCubeMapPositiveX,
            TextureTarget.TextureCubeMapNegativeX,
            TextureTarget.TextureCubeMapPositiveY,
            TextureTarget.TextureCubeMapNegativeY,
            TextureTarget.TextureCubeMapPositiveZ,
            TextureTarget.TextureCubeMapNegativeZ
        };

        public CubemapTexture(string[] facePaths) // конструктор
            : base()
        {
            if (facePaths.Length != 6)
            {
                throw new ArgumentException("Cubemap requires 6 images (one for each face)");
            }

            textureID = GL.GenTexture(); // создание пустой текстуры
            GL.BindTexture(TextureTarget.TextureCubeMap, textureID); // привязывает текстуру к текстурному блоку

            for (int i = 0; i < 6; i++)
            {
                var path = facePaths[i];
                StbImage.stbi_set_flip_vertically_on_load(0); // Кубические текстуры не нужно переворачивать
                ImageResult image = ImageResult.FromStream(File.OpenRead(path), ColorComponents.RedGreenBlueAlpha);
                // загружает файл изображения в память и конвертирует его в формат с компонентами RGB
                GL.TexImage2D(targets[i], 0, PixelInternalFormat.Rgba,
                    image.Width, image.Height, 0,
                    PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
                // передает данные изображения в GPU
            }

            // Параметры обёртки и фильтрации
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);

            GL.BindTexture(TextureTarget.TextureCubeMap, 0); // Отвязка
        }

        public override void Use(TextureUnit unit = TextureUnit.Texture0) // использование текстуры
        {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.TextureCubeMap, textureID);
        }
    }
}
