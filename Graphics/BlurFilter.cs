using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Computer_Graphics_Programming_Blue_Meteorite.Graphics
{
    public class BlurFilter
    {
        private bool isEnabled;
        private int shaderProgram;
        private int quadVAO, quadVBO;
        private int screenTexture;
        private float blurStrength = 1.0f;

        public bool IsEnabled
        {
            get => isEnabled;
            set => isEnabled = value;
        }

        public float BlurStrength
        {
            get => blurStrength;
            set => blurStrength = value;
        }

        public BlurFilter()
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
                uniform float blurStrength;
                uniform vec2 screenSize;
                uniform bool horizontal;

                void main()
                {
                    vec2 texelSize = 1.0 / screenSize;
                    vec4 result = vec4(0.0);
                    
                    // Gaussian weights
                    float weights[5] = float[] (0.227027, 0.1945946, 0.1216216, 0.054054, 0.016216);
                    
                    // Center pixel
                    result += texture(screenTexture, TexCoords) * weights[0];
                    
                    // Blur in one direction
                    for(int i = 1; i < 5; i++)
                    {
                        vec2 offset = vec2(0.0);
                        if(horizontal)
                            offset = vec2(texelSize.x * i * blurStrength, 0.0);
                        else
                            offset = vec2(0.0, texelSize.y * i * blurStrength);
                            
                        result += texture(screenTexture, TexCoords + offset) * weights[i];
                        result += texture(screenTexture, TexCoords - offset) * weights[i];
                    }
                    
                    FragColor = result;
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

            int blurStrengthLoc = GL.GetUniformLocation(shaderProgram, "blurStrength");
            int screenSizeLoc = GL.GetUniformLocation(shaderProgram, "screenSize");
            int horizontalLoc = GL.GetUniformLocation(shaderProgram, "horizontal");

            GL.Uniform1(blurStrengthLoc, blurStrength);
            GL.Uniform2(screenSizeLoc, screenSize);
            GL.Uniform1(horizontalLoc, 1);

            GL.BindVertexArray(quadVAO);
            GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);
            GL.BindVertexArray(0);

            GL.Uniform1(horizontalLoc, 0);
            GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);
        }

        public void Cleanup()
        {
            GL.DeleteVertexArray(quadVAO);
            GL.DeleteBuffer(quadVBO);
            GL.DeleteProgram(shaderProgram);
        }
    }
} 