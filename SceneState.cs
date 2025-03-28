using OpenTK.Mathematics;

namespace Computer_Graphics_Programming_Blue_Meteorite
{

    public class SceneState
    {
        public float GravityStrength { get; set; } = 9.8f;

        // Состояние света
        public LightSettings LightSettings { get; set; } = new LightSettings
        {
            Position = new Vector3(0, 5, 10),
            LookAt = new Vector3(0, 0, 0),
            AmbientIntensity = 0.5f,
            DiffuseIntensity = 1.0f,
            SpecularIntensity = 0.8f,
            AttenuationA = 0.3f,
            AttenuationB = 0.07f,
            AttenuationC = 0.025f,
            Color = new Color4(1.0f, 1.0f, 1.0f, 1.0f)
        };

        // Интерактивность
        public bool IsGrabToolEnabled { get; set; } = false;

        // Активные объекты на сцене
        public List<SceneObject> Objects { get; set; } = new List<SceneObject>();

        // Активные анимации
        public List<IAnimation> Animations { get; set; } = new List<IAnimation>();

        // Системы частиц
        public List<ParticleSystem> ParticleSystems { get; set; } = new List<ParticleSystem>();

        public float SkyboxTimeOfDay { get; set; } = 0.5f;
        public bool SkyboxAutoUpdate { get; set; } = true;

        public SceneState()
        {
            // Инициализация объектов

            // Метеорит
            Objects.Add(new SceneObject
            {
                Name = "Метеорит",
                Type = ObjectTypes.Sphere,
                Texture = "textures/meteorite.jpg",
                Position = new Vector3(0f, 80f, -80f), 
                Rotation = new Vector3(0, 0, 0),
                Scale = new Vector3(15f, 15f, 15f),
                IsDynamic = false,
            });

            // Футбольное поле (используем плоскость)
            Objects.Add(new SceneObject
            {
                Name = "Футбольное поле",
                Type = ObjectTypes.Plane,
                Texture = "textures/grass.jpg",
                Position = new Vector3(0f, 0f, 0f),
                Rotation = new Vector3(0, 0, 0),
                Scale = new Vector3(20f, 1f, 30f), // Размер поля
                IsDynamic = false,
            });

            // Трибуны (используем призмы)
            Objects.Add(new SceneObject
            {
                Name = "Трибуна 1",
                Type = ObjectTypes.Prism,
                Texture = null, // Без текстуры
                Position = new Vector3(-22f, 1f, 0f),
                Rotation = new Vector3(0, 90, 0),
                Scale = new Vector3(2f, 3f, 30f),
                IsDynamic = false,
            });

            Objects.Add(new SceneObject
            {
                Name = "Трибуна 2",
                Type = ObjectTypes.Prism,
                Texture = null, // Без текстуры
                Position = new Vector3(22f, 1f, 0f),
                Rotation = new Vector3(0, -90, 0),
                Scale = new Vector3(2f, 3f, 30f),
                IsDynamic = false,
            });

            Objects.Add(new SceneObject
            {
                Name = "Трибуна 3",
                Type = ObjectTypes.Prism,
                Texture = null, // Без текстуры
                Position = new Vector3(0f, 1f, -32f),
                Rotation = new Vector3(0, 0, 0),
                Scale = new Vector3(20f, 3f, 2f),
                IsDynamic = false,
            });

            // Пятиэтажки (используем кубы)
            for (int i = 0; i < 4; i++)
            {
                Objects.Add(new SceneObject
                {
                    Name = $"Пятиэтажка {i + 1}",
                    Type = ObjectTypes.Cube,
                    Texture = null, // Без текстуры
                    Position = new Vector3(
                        (i % 2 == 0 ? -25f : 25f),
                        7.5f,
                        (i < 2 ? -40f : 40f)
                    ),
                    Rotation = new Vector3(0, 0, 0),
                    Scale = new Vector3(8f, 15f, 8f),
                    IsDynamic = false,
                });
            }

            // Add rotation animation for the meteorite
            Animations.Add(new RotationAnimation
            {
                Name = "MeteoriteRotation",
                TargetObject = Objects[0], // The meteorite is the first object
                RotationSpeed = new Vector3(0, 30, 0) // Rotate 30 degrees per second around Y axis
            });

            // Add particle system for the meteorite
            ParticleSystems.Add(new ParticleSystem(Objects[0])
            {
                Name = "MeteoriteParticles"
            });

        }
        internal SceneObject GetObject(int index)
        {
            return Objects[index];
        }

        internal IAnimation GetAnimation(int index)
        {
            return Animations[index];
        }
    }


    public enum ObjectTypes
    {
        Cube,
        Sphere,
        Plane,
        Prism
    }

    // Класс для представления объектов на сцене
    public class SceneObject
    {
        public string Parent { get; set; }
        public string? Texture { get; set; }  // Making Texture nullable
        public string Name { get; set; }

        public bool IsDynamic { get; set; }

        public ObjectTypes Type { get; set; }
        public Vector3 Position { get; set; }

        public Vector3 AppliedForce { get; set; }
        public bool isForceApplied { get; set; }

        public Vector3 Rotation { get; set; }
        public Vector3 Scale { get; set; }

        internal void ApplyTranslation(Vector3 vector3)
        {
            Position += vector3;
        }

        internal void ApplyRotation(Vector3 vector3)
        {
            Rotation += vector3;
        }

        internal void ApplyScale(Vector3 vector3)
        {
            Scale += vector3;
        }
    }
    public class LightSettings
    {
        public Vector3 Position { get; set; }
        public Vector3 LookAt { get; set; }
        public float AmbientIntensity { get; set; }
        public float DiffuseIntensity { get; set; }
        public float SpecularIntensity { get; set; }
        public float AttenuationA { get; set; }
        public float AttenuationB { get; set; }
        public float AttenuationC { get; set; }
        public Color4 Color { get; set; }

        public void Update(Vector3 position, Vector3 lookAt, float ambient, float diffuse, float specular,
                           float attA, float attB, float attC, Color4 color)
        {
            Position = position;
            LookAt = lookAt;
            AmbientIntensity = ambient;
            DiffuseIntensity = diffuse;
            SpecularIntensity = specular;
            AttenuationA = attA;
            AttenuationB = attB;
            AttenuationC = attC;
            Color = color;
        }
    }
}