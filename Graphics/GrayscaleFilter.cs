using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Computer_Graphics_Programming_Blue_Meteorite.Graphics
{
    public class GrayscaleFilter
    {
        private bool isEnabled;
        private int shaderProgram;
        private int quadVAO, quadVBO;
        private int screenTexture;

        public bool IsEnabled
        {
            get => isEnabled;
            set => isEnabled = value;
        }

        public GrayscaleFilter()
        {
            InitializeShader();
            InitializeQuad();
        }

        private void InitializeShader()
        {
            string vertexShaderSource = @"
                #version 330 core
                layout (location = 0) in vec3 aPos;
                layout (location = 1) in vec2 aTexCoords;
                out vec2 TexCoords;
                void main()
                {
                    gl_Position = vec4(aPos, 1.0);
                    TexCoords = aTexCoords;
                }";

            string fragmentShaderSource = @"
                #version 330 core
                out vec4 FragColor;
                in vec2 TexCoords;
                uniform sampler2D screenTexture;
                uniform bool grayscaleEnabled;
                void main()
                {
                    vec4 color = texture(screenTexture, TexCoords);
                    if (grayscaleEnabled)
                    {
                        float gray = dot(color.rgb, vec3(0.299, 0.587, 0.114));
                        FragColor = vec4(vec3(gray), color.a);
                    }
                    else
                    {
                        FragColor = color;
                    }
                }";

            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexShaderSource);
            GL.CompileShader(vertexShader);

            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentShaderSource);
            GL.CompileShader(fragmentShader);

            shaderProgram = GL.CreateProgram();
            GL.AttachShader(shaderProgram, vertexShader);
            GL.AttachShader(shaderProgram, fragmentShader);
            GL.LinkProgram(shaderProgram);

            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);
        }

        private void InitializeQuad()
        {
            float[] quadVertices = {
                // positions        // texture Coords
                -1.0f,  1.0f, 0.0f, 0.0f, 1.0f,
                -1.0f, -1.0f, 0.0f, 0.0f, 0.0f,
                 1.0f,  1.0f, 0.0f, 1.0f, 1.0f,
                 1.0f, -1.0f, 0.0f, 1.0f, 0.0f,
            };

            quadVAO = GL.GenVertexArray();
            quadVBO = GL.GenBuffer();

            GL.BindVertexArray(quadVAO);

            GL.BindBuffer(BufferTarget.ArrayBuffer, quadVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, quadVertices.Length * sizeof(float), quadVertices, BufferUsageHint.StaticDraw);

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
        }

        public void Apply(int screenTexture)
        {
            this.screenTexture = screenTexture;
            GL.UseProgram(shaderProgram);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, screenTexture);

            int grayscaleEnabledLoc = GL.GetUniformLocation(shaderProgram, "grayscaleEnabled");
            GL.Uniform1(grayscaleEnabledLoc, isEnabled ? 1 : 0);

            GL.BindVertexArray(quadVAO);
            GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);
            GL.BindVertexArray(0);
        }

        public void Cleanup()
        {
            GL.DeleteVertexArray(quadVAO);
            GL.DeleteBuffer(quadVBO);
            GL.DeleteProgram(shaderProgram);
        }
    }
} 