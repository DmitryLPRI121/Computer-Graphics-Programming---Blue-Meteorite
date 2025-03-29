using OpenTK.Graphics.OpenGL4;
using System.Drawing.Imaging;

namespace Computer_Graphics_Programming_Blue_Meteorite
{ 
    public class Texture
    {
        public int Handle;
        TextureUnit currentUnit;
        public Texture(string? path)
        {
            Handle = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, Handle);

            // Настройка параметров текстуры
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            if (path != null)
            {
                // Загрузка изображения
                Bitmap bmp = new Bitmap(path);
                BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                    ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp.Width, bmp.Height, 0,
                    OpenTK.Graphics.OpenGL4.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

                bmp.UnlockBits(data);
                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            }
            else
            {
                // Создаем текстуру по умолчанию (1x1 пиксель белого цвета)
                byte[] defaultTexture = new byte[] { 255, 255, 255, 255 };
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, 1, 1, 0,
                    OpenTK.Graphics.OpenGL4.PixelFormat.Rgba, PixelType.UnsignedByte, defaultTexture);
            }
        }

        public void Use(TextureUnit unit)
        {
            currentUnit = unit;
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, Handle);
        }

        internal void Detach()
        {
            GL.ActiveTexture(currentUnit);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }
    }
}