using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Computer_Graphics_Programming_Blue_Meteorite.Graphics
{
    public class NightVisionFilter
    {
        private bool isEnabled;
        private int shaderProgram;
        private int quadVAO, quadVBO;
        private int screenTexture;
        private float noiseStrength = 0.3f;
        private float time = 0.0f;

        public bool IsEnabled
        {
            get => isEnabled;
            set => isEnabled = value;
        }

        public float NoiseStrength
        {
            get => noiseStrength;
            set => noiseStrength = value;
        }

        public NightVisionFilter()
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
                uniform bool nightVisionEnabled;
                uniform float noiseStrength;
                uniform float time;

                // Hash function for noise
                float hash(vec2 p)
                {
                    return fract(sin(dot(p, vec2(12.9898, 78.233))) * 43758.5453);
                }

                void main()
                {
                    vec4 color = texture(screenTexture, TexCoords);
                    
                    if (nightVisionEnabled)
                    {
                        // Increase brightness
                        color.rgb = color.rgb * 1.5;
                        
                        // Apply green tint
                        float luminance = dot(color.rgb, vec3(0.299, 0.587, 0.114));
                        color.rgb = vec3(0.0, luminance * 1.2, 0.0);
                        
                        // Add noise
                        vec2 noiseCoord = TexCoords * 300.0 + time;
                        float noise = hash(noiseCoord) * noiseStrength;
                        color.rgb += vec3(noise);
                        
                        // Add vignette effect
                        vec2 center = vec2(0.5, 0.5);
                        float distance = length(TexCoords - center);
                        float vignette = smoothstep(0.8, 0.4, distance);
                        color.rgb *= vignette;
                        
                        // Add scan lines
                        float scanLine = sin(TexCoords.y * 800.0) * 0.03;
                        color.rgb += vec3(scanLine);
                    }
                    
                    FragColor = color;
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

        public void Apply(int screenTexture)
        {
            this.screenTexture = screenTexture;
            
            time += 0.01f;
            
            GL.UseProgram(shaderProgram);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, screenTexture);

            int nightVisionEnabledLoc = GL.GetUniformLocation(shaderProgram, "nightVisionEnabled");
            GL.Uniform1(nightVisionEnabledLoc, isEnabled ? 1 : 0);

            int noiseStrengthLoc = GL.GetUniformLocation(shaderProgram, "noiseStrength");
            GL.Uniform1(noiseStrengthLoc, noiseStrength);
            
            int timeLoc = GL.GetUniformLocation(shaderProgram, "time");
            GL.Uniform1(timeLoc, time);

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