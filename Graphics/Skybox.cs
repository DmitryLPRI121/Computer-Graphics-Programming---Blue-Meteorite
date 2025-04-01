using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Computer_Graphics_Programming_Blue_Meteorite
{
    public class Skybox
    {
        private readonly float[] vertices = {
            // positions          
            -100.0f,  100.0f, -100.0f,
            -100.0f, -100.0f, -100.0f,
             100.0f, -100.0f, -100.0f,
             100.0f, -100.0f, -100.0f,
             100.0f,  100.0f, -100.0f,
            -100.0f,  100.0f, -100.0f,

            -100.0f, -100.0f,  100.0f,
            -100.0f, -100.0f, -100.0f,
            -100.0f,  100.0f, -100.0f,
            -100.0f,  100.0f, -100.0f,
            -100.0f,  100.0f,  100.0f,
            -100.0f, -100.0f,  100.0f,

             100.0f, -100.0f, -100.0f,
             100.0f, -100.0f,  100.0f,
             100.0f,  100.0f,  100.0f,
             100.0f,  100.0f,  100.0f,
             100.0f,  100.0f, -100.0f,
             100.0f, -100.0f, -100.0f,

            -100.0f, -100.0f,  100.0f,
            -100.0f,  100.0f,  100.0f,
             100.0f,  100.0f,  100.0f,
             100.0f,  100.0f,  100.0f,
             100.0f, -100.0f,  100.0f,
            -100.0f, -100.0f,  100.0f,

            -100.0f,  100.0f, -100.0f,
             100.0f,  100.0f, -100.0f,
             100.0f,  100.0f,  100.0f,
             100.0f,  100.0f,  100.0f,
            -100.0f,  100.0f,  100.0f,
            -100.0f,  100.0f, -100.0f,

            -100.0f, -100.0f, -100.0f,
            -100.0f, -100.0f,  100.0f,
             100.0f, -100.0f, -100.0f,
             100.0f, -100.0f, -100.0f,
            -100.0f, -100.0f,  100.0f,
             100.0f, -100.0f,  100.0f
        };

        private int vao;
        private int vbo;
        private Shader shader;
        private float timeOfDay = 0.5f; // Start at noon
        private bool autoUpdate = true;
        private float cycleSpeed = 60.0f; // Speed of time cycle (in seconds per full cycle)
        private SceneObjects sceneState;

        public Skybox(SceneObjects sceneState = null)
        {
            this.sceneState = sceneState;
            shader = new Shader("shaders/skybox.vert", "shaders/skybox.frag");
            InitializeBuffers();
        }

        private void InitializeBuffers()
        {
            vao = GL.GenVertexArray();
            vbo = GL.GenBuffer();

            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        }

        public void Update(float deltaTime)
        {
            if (autoUpdate)
            {
                float oldTime = timeOfDay;
                timeOfDay = (timeOfDay + deltaTime / cycleSpeed) % 1.0f;
                
                // Обновляем время в sceneState
                if (sceneState != null)
                {
                    lock (sceneState)
                    {
                        sceneState.SkyboxTimeOfDay = timeOfDay;
                    }
                }
            }
        }

        public void SetTimeOfDay(float time)
        {
            float oldTime = timeOfDay;
            timeOfDay = time;
            
            // Синхронизируем время с sceneState
            if (sceneState != null)
            {
                lock (sceneState)
                {
                    sceneState.SkyboxTimeOfDay = timeOfDay;
                }
            }
        }

        public void SetAutoUpdate(bool enabled)
        {
            bool oldValue = autoUpdate;
            autoUpdate = enabled;
        }

        public void SetCycleSpeed(float speed)
        {
            cycleSpeed = speed;
        }

        public void Render(Matrix4 projection, Matrix4 view)
        {
            // Remove translation from view matrix
            view.M41 = 0;
            view.M42 = 0;
            view.M43 = 0;

            shader.Use();
            shader.SetMatrix4("projection", projection);
            shader.SetMatrix4("view", view);
            shader.SetFloat("timeOfDay", timeOfDay);
            shader.SetInt("skybox", 0);

            GL.DepthFunc(DepthFunction.Lequal);
            GL.BindVertexArray(vao);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
            GL.BindVertexArray(0);
            GL.DepthFunc(DepthFunction.Less);
        }

        public float GetTimeOfDay()
        {
            return timeOfDay;
        }

        public void Dispose()
        {
            GL.DeleteVertexArray(vao);
            GL.DeleteBuffer(vbo);
            shader.Dispose();
        }
    }
} 