using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Computer_Graphics_Programming_Blue_Meteorite.Graphics
{
    public class SharpnessFilter
    {
        private bool isEnabled;
        private int shaderProgram;
        private int quadVAO, quadVBO;
        private int screenTexture;
        private float sharpnessStrength = 1.0f;

        public bool IsEnabled
        {
            get => isEnabled;
            set => isEnabled = value;
        }

        public float SharpnessStrength
        {
            get => sharpnessStrength;
            set => sharpnessStrength = value;
        }

        public SharpnessFilter()
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
                uniform float sharpnessStrength;
                uniform vec2 screenSize;

                void main()
                {
                    vec2 texelSize = 1.0 / screenSize;
                    
                    // Sharpening kernel
                    vec4 color = texture(screenTexture, TexCoords);
                    vec4 left = texture(screenTexture, TexCoords - vec2(texelSize.x, 0.0));
                    vec4 right = texture(screenTexture, TexCoords + vec2(texelSize.x, 0.0));
                    vec4 top = texture(screenTexture, TexCoords + vec2(0.0, texelSize.y));
                    vec4 bottom = texture(screenTexture, TexCoords - vec2(0.0, texelSize.y));
                    
                    // Apply sharpening
                    vec4 sharpened = color * (1.0 + 4.0 * sharpnessStrength) - 
                                   (left + right + top + bottom) * sharpnessStrength;
                    
                    FragColor = sharpened;
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

        public void Apply(int screenTexture, Vector2 screenSize)
        {
            this.screenTexture = screenTexture;
            GL.UseProgram(shaderProgram);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, screenTexture);

            int sharpnessStrengthLoc = GL.GetUniformLocation(shaderProgram, "sharpnessStrength");
            int screenSizeLoc = GL.GetUniformLocation(shaderProgram, "screenSize");

            GL.Uniform1(sharpnessStrengthLoc, sharpnessStrength);
            GL.Uniform2(screenSizeLoc, screenSize);

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