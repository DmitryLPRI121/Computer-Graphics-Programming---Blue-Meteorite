using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;

namespace Computer_Graphics_Programming_Blue_Meteorite
{
    public class ParticleSystem
    {
        private List<Particle> particles = new List<Particle>();
        public SceneObject TargetObject { get; private set; }
        private float emissionRate = 100f;
        private float timeSinceLastEmission = 0f;
        private float duration = 5f;
        private float currentDuration = 0f;
        private bool isActive = false;

        private int VAO, VBO;
        private Shader particleShader;
        private float[] particleData;
        private Random random = new Random();
        private bool isInitialized = false;
        public string Name { get; set; }

        // Константы для настройки частиц
        private const float MIN_PARTICLE_SIZE = 20.0f;
        private const float MAX_PARTICLE_SIZE = 40.0f;
        private const float PARTICLE_SPREAD = 2.0f;
        private const float PARTICLE_SPEED = 2.0f;
        private const float PARTICLE_LIFE_MIN = 1.0f;
        private const float PARTICLE_LIFE_MAX = 2.0f;

        public ParticleSystem(SceneObject target)
        {
            TargetObject = target;
        }

        public void Initialize()
        {
            if (!isInitialized)
            {
                InitializeBuffers();
                particleShader = new Shader("shaders/particle.vert", "shaders/particle.frag");
                GL.Enable(EnableCap.PointSprite);
                GL.Enable(EnableCap.ProgramPointSize);
                isInitialized = true;
            }
        }

        private void InitializeBuffers()
        {
            GL.GenVertexArrays(1, out VAO);
            GL.GenBuffers(1, out VBO);

            GL.BindVertexArray(VAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);

            // Позиция
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 7 * sizeof(float), 0);

            // Цвет
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 7 * sizeof(float), 3 * sizeof(float));

            // Размер
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 1, VertexAttribPointerType.Float, false, 7 * sizeof(float), 6 * sizeof(float));

            GL.BindVertexArray(0);
        }

        public void Start()
        {
            isActive = true;
            currentDuration = 0f;
            particles.Clear();
        }

        public void Stop()
        {
            isActive = false;
            ClearParticles();
        }

        public void ClearParticles()
        {
            particles.Clear();
            if (particleData != null)
            {
                particleData = new float[0];
            }
        }

        public void Update(float deltaTime)
        {
            if (!isActive) return;

            currentDuration += deltaTime;
            if (currentDuration >= duration)
            {
                Stop();
                return;
            }

            // Обновляем существующие частицы
            for (int i = particles.Count - 1; i >= 0; i--)
            {
                particles[i].Update(deltaTime);
                if (particles[i].Life <= 0)
                {
                    particles.RemoveAt(i);
                }
            }

            // Создаем новые частицы
            timeSinceLastEmission += deltaTime;
            float emissionInterval = 1f / emissionRate;
            while (timeSinceLastEmission >= emissionInterval)
            {
                EmitParticle();
                timeSinceLastEmission -= emissionInterval;
            }
        }

        private void EmitParticle()
        {
            // Создаем частицу в позиции объекта
            Vector3 basePosition = TargetObject.Position;
            
            // Добавляем случайное смещение
            float offsetX = (float)(random.NextDouble() - 0.5) * PARTICLE_SPREAD;
            float offsetY = (float)(random.NextDouble() - 0.5) * PARTICLE_SPREAD;
            float offsetZ = (float)(random.NextDouble() - 0.5) * PARTICLE_SPREAD;
            
            Vector3 position = basePosition + new Vector3(offsetX, offsetY, offsetZ);

            // Создаем случайную скорость
            float speedX = (float)(random.NextDouble() - 0.5) * PARTICLE_SPEED;
            float speedY = (float)random.NextDouble() * PARTICLE_SPEED;
            float speedZ = (float)(random.NextDouble() - 0.5) * PARTICLE_SPEED;
            
            Vector3 velocity = new Vector3(speedX, speedY, speedZ);

            // Создаем случайный цвет (синий)
            float r = (float)random.NextDouble() * 0.3f;
            float g = (float)random.NextDouble() * 0.5f + 0.2f;
            float b = 1.0f;
            Vector3 color = new Vector3(r, g, b);

            // Создаем частицу с увеличенным размером
            float size = (float)random.NextDouble() * (MAX_PARTICLE_SIZE - MIN_PARTICLE_SIZE) + MIN_PARTICLE_SIZE;
            float life = (float)random.NextDouble() * (PARTICLE_LIFE_MAX - PARTICLE_LIFE_MIN) + PARTICLE_LIFE_MIN;
            particles.Add(new Particle(position, velocity, color, size, life));
        }

        public void Render(Shader shader, Matrix4 projection, Matrix4 view, Vector3 cameraPos)
        {
            if (particles.Count == 0) return;

            particleShader.Use();

            // Обновляем данные частиц
            particleData = new float[particles.Count * 7];
            for (int i = 0; i < particles.Count; i++)
            {
                var particle = particles[i];
                int baseIndex = i * 7;

                // Позиция
                particleData[baseIndex] = particle.Position.X;
                particleData[baseIndex + 1] = particle.Position.Y;
                particleData[baseIndex + 2] = particle.Position.Z;

                // Цвет
                particleData[baseIndex + 3] = particle.Color.X;
                particleData[baseIndex + 4] = particle.Color.Y;
                particleData[baseIndex + 5] = particle.Color.Z;

                // Размер
                particleData[baseIndex + 6] = particle.Size;
            }

            // Обновляем буфер
            GL.BindVertexArray(VAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, particleData.Length * sizeof(float), particleData, BufferUsageHint.DynamicDraw);

            // Устанавливаем матрицы проекции и вида
            particleShader.SetMatrix4("projection", projection);
            particleShader.SetMatrix4("view", view);
            particleShader.SetVector3("cameraPos", cameraPos);

            // Включаем смешивание для прозрачности
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            // Рендерим частицы
            GL.DrawArrays(PrimitiveType.Points, 0, particles.Count);

            // Отключаем смешивание
            GL.Disable(EnableCap.Blend);

            GL.BindVertexArray(0);
        }
    }
} 