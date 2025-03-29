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
        private bool isCyclic = false;

        public bool IsActive => isActive;
        public bool IsCyclic
        {
            get => isCyclic;
            set
            {
                isCyclic = value;
                if (value)
                {
                    isActive = true;
                }
            }
        }

        private int VAO, VBO;
        private Shader particleShader;
        private float[] particleData;
        private Random random = new Random();
        private bool isInitialized = false;
        public string Name { get; set; }

        // Настраиваемые параметры системы частиц
        public float MinParticleSize { get; set; } = 20.0f;
        public float MaxParticleSize { get; set; } = 40.0f;
        public float ParticleSpread { get; set; } = 2.0f;
        public float ParticleSpeed { get; set; } = 2.0f;
        public float ParticleLifeMin { get; set; } = 1.0f;
        public float ParticleLifeMax { get; set; } = 2.0f;
        public float ColorVariation { get; set; } = 0.3f;
        public ParticleEffectType EffectType { get; set; } = ParticleEffectType.Basic;
        public ParticleShape Shape { get; set; } = ParticleShape.Circle;

        // Цвета для разных эффектов
        public Vector3 BasicColor { get; set; } = new Vector3(0.3f, 0.7f, 1.0f);
        public Vector3 FireColor { get; set; } = new Vector3(1.0f, 0.3f, 0.0f);
        public Vector3 SparkleColor { get; set; } = new Vector3(1.0f, 1.0f, 1.0f);
        public Vector3 FountainColor { get; set; } = new Vector3(0.0f, 0.5f, 1.0f);

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
            if (!isCyclic && currentDuration >= duration)
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
            Vector3 basePosition = TargetObject.Position;
            Vector3 position;
            Vector3 velocity;
            Vector3 color;
            float size;
            float life;

            switch (EffectType)
            {
                case ParticleEffectType.Basic:
                    (position, velocity, color, size, life) = CreateBasicParticle(basePosition);
                    break;
                case ParticleEffectType.Fire:
                    (position, velocity, color, size, life) = CreateFireParticle(basePosition);
                    break;
                case ParticleEffectType.Sparkle:
                    (position, velocity, color, size, life) = CreateSparkleParticle(basePosition);
                    break;
                case ParticleEffectType.Fountain:
                    (position, velocity, color, size, life) = CreateFountainParticle(basePosition);
                    break;
                default:
                    (position, velocity, color, size, life) = CreateBasicParticle(basePosition);
                    break;
            }

            particles.Add(new Particle(position, velocity, color, size, life));
        }

        private (Vector3 pos, Vector3 vel, Vector3 col, float size, float life) CreateBasicParticle(Vector3 basePosition)
        {
            float offsetX = (float)(random.NextDouble() - 0.5) * ParticleSpread;
            float offsetY = (float)(random.NextDouble() - 0.5) * ParticleSpread;
            float offsetZ = (float)(random.NextDouble() - 0.5) * ParticleSpread;
            Vector3 position = basePosition + new Vector3(offsetX, offsetY, offsetZ);

            float speedX = (float)(random.NextDouble() - 0.5) * ParticleSpeed;
            float speedY = (float)random.NextDouble() * ParticleSpeed;
            float speedZ = (float)(random.NextDouble() - 0.5) * ParticleSpeed;
            Vector3 velocity = new Vector3(speedX, speedY, speedZ);

            float colorVar = (float)random.NextDouble() * ColorVariation;
            Vector3 color = BasicColor + new Vector3(colorVar, colorVar, colorVar);

            float size = (float)random.NextDouble() * (MaxParticleSize - MinParticleSize) + MinParticleSize;
            float life = (float)random.NextDouble() * (ParticleLifeMax - ParticleLifeMin) + ParticleLifeMin;

            return (position, velocity, color, size, life);
        }

        private (Vector3 pos, Vector3 vel, Vector3 col, float size, float life) CreateFireParticle(Vector3 basePosition)
        {
            float offsetX = (float)(random.NextDouble() - 0.5) * ParticleSpread * 0.5f;
            float offsetY = (float)random.NextDouble() * ParticleSpread * 0.5f;
            float offsetZ = (float)(random.NextDouble() - 0.5) * ParticleSpread * 0.5f;
            Vector3 position = basePosition + new Vector3(offsetX, offsetY, offsetZ);

            float speedX = (float)(random.NextDouble() - 0.5) * ParticleSpeed * 0.5f;
            float speedY = (float)random.NextDouble() * ParticleSpeed * 2f;
            float speedZ = (float)(random.NextDouble() - 0.5) * ParticleSpeed * 0.5f;
            Vector3 velocity = new Vector3(speedX, speedY, speedZ);

            float colorVar = (float)random.NextDouble() * ColorVariation;
            Vector3 color = FireColor + new Vector3(colorVar, colorVar, colorVar);

            float size = (float)random.NextDouble() * (MaxParticleSize - MinParticleSize) + MinParticleSize;
            float life = (float)random.NextDouble() * (ParticleLifeMax - ParticleLifeMin) + ParticleLifeMin;

            return (position, velocity, color, size, life);
        }

        private (Vector3 pos, Vector3 vel, Vector3 col, float size, float life) CreateSparkleParticle(Vector3 basePosition)
        {
            float offsetX = (float)(random.NextDouble() - 0.5) * ParticleSpread * 2f;
            float offsetY = (float)(random.NextDouble() - 0.5) * ParticleSpread * 2f;
            float offsetZ = (float)(random.NextDouble() - 0.5) * ParticleSpread * 2f;
            Vector3 position = basePosition + new Vector3(offsetX, offsetY, offsetZ);

            float speedX = (float)(random.NextDouble() - 0.5) * ParticleSpeed * 0.5f;
            float speedY = (float)(random.NextDouble() - 0.5) * ParticleSpeed * 0.5f;
            float speedZ = (float)(random.NextDouble() - 0.5) * ParticleSpeed * 0.5f;
            Vector3 velocity = new Vector3(speedX, speedY, speedZ);

            float colorVar = (float)random.NextDouble() * ColorVariation;
            Vector3 color = SparkleColor + new Vector3(colorVar, colorVar, colorVar);

            float size = (float)random.NextDouble() * (MaxParticleSize - MinParticleSize) * 0.5f + MinParticleSize * 0.5f;
            float life = (float)random.NextDouble() * (ParticleLifeMax - ParticleLifeMin) * 0.5f + ParticleLifeMin * 0.5f;

            return (position, velocity, color, size, life);
        }

        private (Vector3 pos, Vector3 vel, Vector3 col, float size, float life) CreateFountainParticle(Vector3 basePosition)
        {
            float offsetX = (float)(random.NextDouble() - 0.5) * ParticleSpread * 0.2f;
            float offsetY = (float)random.NextDouble() * ParticleSpread * 0.2f;
            float offsetZ = (float)(random.NextDouble() - 0.5) * ParticleSpread * 0.2f;
            Vector3 position = basePosition + new Vector3(offsetX, offsetY, offsetZ);

            float speedX = (float)(random.NextDouble() - 0.5) * ParticleSpeed * 0.3f;
            float speedY = (float)random.NextDouble() * ParticleSpeed * 2f;
            float speedZ = (float)(random.NextDouble() - 0.5) * ParticleSpeed * 0.3f;
            Vector3 velocity = new Vector3(speedX, speedY, speedZ);

            float colorVar = (float)random.NextDouble() * ColorVariation;
            Vector3 color = FountainColor + new Vector3(colorVar, colorVar, colorVar);

            float size = (float)random.NextDouble() * (MaxParticleSize - MinParticleSize) + MinParticleSize;
            float life = (float)random.NextDouble() * (ParticleLifeMax - ParticleLifeMin) + ParticleLifeMin;

            return (position, velocity, color, size, life);
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
            particleShader.SetInt("particleShape", (int)Shape);

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

    public enum ParticleEffectType
    {
        Basic,
        Fire,
        Sparkle,
        Fountain
    }

    public enum ParticleShape
    {
        Circle,
        Square,
        Star,
        Cross
    }
} 