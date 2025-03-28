using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Computer_Graphics_Programming_Blue_Meteorite
{
    public class ShadowMap
    {
        public int DepthMapFBO { get; private set; }
        public int DepthMap { get; private set; }
        public int ShadowWidth { get; private set; }
        public int ShadowHeight { get; private set; }
        public Matrix4 LightSpaceMatrix { get; private set; }

        public ShadowMap(int shadowWidth, int shadowHeight)
        {
            ShadowWidth = shadowWidth;
            ShadowHeight = shadowHeight;

            // 1. Создание фреймбуфера для рендеринга теней
            DepthMapFBO = GL.GenFramebuffer();

            // 2. Создание текстуры для хранения глубины
            DepthMap = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, DepthMap);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent, shadowWidth, shadowHeight, 0, PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            // Настройка граничного цвета для текстуры теней
            float[] borderColor = new float[] { 1.0f, 1.0f, 1.0f, 1.0f };
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, borderColor);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, DepthMapFBO);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, DepthMap, 0);

            GL.DrawBuffer(DrawBufferMode.None); // Мы не рисуем цвет
            GL.ReadBuffer(ReadBufferMode.None);

            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
                throw new Exception("Shadow Framebuffer не был успешно собран!");

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void BindForWriting()
        {
            GL.Viewport(0, 0, ShadowWidth, ShadowHeight);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, DepthMapFBO);
            GL.Clear(ClearBufferMask.DepthBufferBit);
        }

        public void BindForReading(int textureUnit)
        {
            GL.ActiveTexture(TextureUnit.Texture0 + textureUnit);
            GL.BindTexture(TextureTarget.Texture2D, DepthMap);
        }

        public void CalculateLightSpaceMatrix(Vector3 lightPos, Vector3 target, float nearPlane, float farPlane)
        {
            float fov = MathHelper.DegreesToRadians(120f);
            float aspectRatio = (float)ShadowWidth / ShadowHeight;
            Matrix4 lightProjection = Matrix4.CreatePerspectiveFieldOfView(fov, aspectRatio, nearPlane, farPlane);
            Matrix4 lightView = Matrix4.LookAt(lightPos, target, Vector3.UnitY);
            LightSpaceMatrix = lightView * lightProjection;
        }

        public void SetShadowShaderUniforms(Shader shadowShader)
        {
            shadowShader.SetMatrix4("lightSpaceMatrix", LightSpaceMatrix);
        }

        public void SetShaderUniforms(Shader shader)
        {
            shader.SetMatrix4("lightSpaceMatrix", LightSpaceMatrix);

            // Привязываем shadow map
            BindForReading(1);
            shader.SetInt("shadowMap", 1);
        }

        public void Unbind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void Dispose()
        {
            GL.DeleteFramebuffer(DepthMapFBO);
            GL.DeleteTexture(DepthMap);
        }
    }
}
