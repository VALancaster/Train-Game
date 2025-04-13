using System;
using System.Collections.Generic;
using System.Drawing;
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
        public int textureID { get; private set; } // идентификатор текстуры {получить значение можно снаружи, а установить - только внутри}

        public Texture(string path) // конструктор
        {
            // 1. Генерация и бинд
            textureID = GL.GenTexture(); // создание пустой текстуры
            GL.ActiveTexture(TextureUnit.Texture0); // активирует текстурный блок 0
            GL.BindTexture(TextureTarget.Texture2D, textureID); // привязывает текстуру к текстурному блоку

            // 2. Параметры текстуры
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            // 3. Загрузка изображения
            StbImage.stbi_set_flip_vertically_on_load(1); // включение вертикального отражения при загрузке
            ImageResult boxTexture = ImageResult.FromStream(File.OpenRead(path), ColorComponents.RedGreenBlueAlpha);
            // загружает файл изображения в память и конвертирует его в формат с компонентами RGBA
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, boxTexture.Width, boxTexture.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, boxTexture.Data);
            // передает данные изображения в GPU

            GL.BindTexture(TextureTarget.Texture2D, 0); // отвязка текстуры от текстурного блока

        }

        public void Use(TextureUnit unit = TextureUnit.Texture0) // использование текстуры
        {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, textureID);
        }

        public void Delete() // удаление текстуры
        {
            GL.DeleteTexture(textureID);
        }
    }

    /*
    public class CubemapTexture : Texture
    {
        public CubemapTexture(string pathToCrossImage) // конструктор
        {
            Handle = GL.GenTexture();
            GL.BindTexture(TextureTarget.TextureCubeMap, Handle);

            using (var image = new Bitmap(pathToCrossImage))
            {

            }
        }

        public override void Use(TextureUnit unit = TextureUnit.Texture0) // использование текстуры
        {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.TextureCubeMap, Handle);
        }
    }
    */
}
