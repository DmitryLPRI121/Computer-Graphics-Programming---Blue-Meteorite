using OpenTK.Mathematics;

namespace Computer_Graphics_Programming_Blue_Meteorite
{

    public class SceneObjects
    {
        public float GravityStrength { get; set; } = 9.8f + 25.2f;

        // Состояние света
        public List<LightSettings> LightSettings { get; set; } = new List<LightSettings>
        {
            new LightSettings()
            {
                Position = new Vector3(0, 140, -340)
            }
        };

        // Активные объекты на сцене
        public List<SceneObject> Objects { get; set; } = new List<SceneObject>();

        // Активные анимации
        public List<AnimationSystem> Animations { get; set; } = new List<AnimationSystem>();

        // Системы частиц
        public List<ParticleSystem> ParticleSystems { get; set; } = new List<ParticleSystem>();

        public float SkyboxTimeOfDay { get; set; } = 0.5f;
        public bool SkyboxAutoUpdate { get; set; } = true;

        public SceneObjects()
        {
            // Инициализация объектов

            // Метеорит
            Objects.Add(new SceneObject
            {
                Name = "Метеорит",
                Type = ObjectTypes.Sphere,
                Texture = "textures/meteorite.jpg",
                Position = new Vector3(0f, 150f, -350f), 
                Rotation = new Vector3(0, 0, 0),
                Scale = new Vector3(50f, 50f, 50f),
                IsDynamic = false,
            });

            Objects.Add(new SceneObject
            {
                Name = "Поверхность",
                Type = ObjectTypes.Plane,
                Texture = "textures/grass.jpg",
                Position = new Vector3(0f, 0f, 0f),
                Rotation = new Vector3(0, 0, 0),
                Scale = new Vector3(100f, 1f, 100f), // Размер поля
                IsDynamic = false,
            });

            // Добавим несколько динамических объектов для демонстрации коллизий
            Objects.Add(new SceneObject
            {
                Name = "Динамический куб 1",
                Type = ObjectTypes.Cube,
                Texture = "textures/build.jpg",
                Position = new Vector3(-1f, 1f, -2f),
                Rotation = new Vector3(0, 0, 0),
                Scale = new Vector3(3f, 3f, 3f),
                IsDynamic = false,
            });

            //Objects.Add(new SceneObject
            //{
            //    Name = "Динамический куб 3",
            //    Type = ObjectTypes.Cube,
            //    Texture = "textures/build.jpg",
            //    Position = new Vector3(-10f, 20f, -20f),
            //    Rotation = new Vector3(0, 0, 0),
            //    Scale = new Vector3(7f,7f, 7f),
            //    IsDynamic = true,
            //});

            Objects.Add(new SceneObject
            {
                Name = "Динамический куб 2",
                Type = ObjectTypes.Cube,
                Texture = "textures/build.jpg",
                Position = new Vector3(10f, 15f, -20f),
                Rotation = new Vector3(0, 0, 0),
                Scale = new Vector3(5f, 5f, 5f),
                IsDynamic = true,
            });

            Objects.Add(new SceneObject
            {
                Name = "Динамическая сфера",
                Type = ObjectTypes.Sphere,
                Texture = "textures/meteorite.jpg",
                Position = new Vector3(5f, 5f, -10f),
                Rotation = new Vector3(0, 0, 0),
                Scale = new Vector3(5f, 5f, 5f),
                IsDynamic = true,
            });

            // Трибуны (используем призмы)
            Objects.Add(new SceneObject
            {
                Name = "Трибуна 1",
                Type = ObjectTypes.Prism,
                Texture = "textures/tribuna.jpg",
                Position = new Vector3(-22f, 1f, 0f),
                Rotation = new Vector3(0, 90, 0),
                Scale = new Vector3(2f, 3f, 30f),
                IsDynamic = false,
            });

            Objects.Add(new SceneObject
            {
                Name = "Трибуна 2",
                Type = ObjectTypes.Prism,
                Texture = "textures/tribuna.jpg", // Без текстуры
                Position = new Vector3(22f, 1f, 0f),
                Rotation = new Vector3(0, -90, 0),
                Scale = new Vector3(2f, 3f, 30f),
                IsDynamic = false,
            });

            Objects.Add(new SceneObject
            {
                Name = "Трибуна 3",
                Type = ObjectTypes.Prism,
                Texture = "textures/tribuna.jpg", // Без текстуры
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
                    Texture = "textures/build.jpg",
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

        internal AnimationSystem GetAnimation(int index)
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
        public Vector3 Color { get; set; } = new Vector3(1f, 1f, 1f); // Default white color

        public Vector3 AppliedForce { get; set; }
        public bool isForceApplied { get; set; }

        public Vector3 Rotation { get; set; }
        public Vector3 Scale { get; set; }

        // Constructor to ensure force-related properties are initialized
        public SceneObject()
        {
            Position = Vector3.Zero;
            Rotation = Vector3.Zero;
            Scale = Vector3.One;
            AppliedForce = Vector3.Zero;
            isForceApplied = false;
        }

        internal void ApplyTranslation(Vector3 vector3)
        {
            // For dynamic objects, we set a flag to apply force during the next sync
            // We also update the position directly to ensure changes are visible immediately in the UI
            if (IsDynamic)
            {
                AppliedForce = vector3 * 50.0f; // Significantly increase force to make movement more noticeable
                isForceApplied = true;
                
                // Also update position directly so UI changes appear immediately
                Position += vector3;
            }
            else
            {
                // For static objects, update position directly
                Position += vector3;
            }
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

        // Конструктор по умолчанию
        public LightSettings()
        {
            Position = new Vector3(0, 5, 10);
            LookAt = new Vector3(0, 0, 0);
            AmbientIntensity = 2.0f;
            DiffuseIntensity = 2.0f;
            SpecularIntensity = 1.0f;
            AttenuationA = 1.0f;
            AttenuationB = 0.09f;
            AttenuationC = 0.032f;
            Color = new Color4(1.0f, 1.0f, 1.0f, 1.0f);
        }

        // Конструктор копирования для создания нового источника света с теми же параметрами
        public LightSettings(LightSettings source)
        {
            Position = source.Position;
            LookAt = source.LookAt;
            AmbientIntensity = source.AmbientIntensity;
            DiffuseIntensity = source.DiffuseIntensity;
            SpecularIntensity = source.SpecularIntensity;
            AttenuationA = source.AttenuationA;
            AttenuationB = source.AttenuationB;
            AttenuationC = source.AttenuationC;
            Color = source.Color;
        }

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