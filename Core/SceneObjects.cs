using OpenTK.Mathematics;

namespace Computer_Graphics_Programming_Blue_Meteorite
{

    public class SceneObjects
    {
        public float GravityStrength { get; set; } = 30f;

        // Состояние света
        public List<LightSettings> LightSettings { get; set; } = new List<LightSettings>
        {
            new LightSettings()
            {
                Position = new Vector3(0, 140, -340),
                AmbientIntensity = 10.0f,
            },

            // Звезды
            new LightSettings()
            {
                Position = new Vector3(0, 100, 0),
                AmbientIntensity = 150.0f,
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
            var meteorite = new SceneObject
            {
                Name = "Метеорит",
                Type = ObjectTypes.Sphere,
                Texture = "textures/meteorite.jpg",
                Position = new Vector3(0f, 150f, -350f), 
                Rotation = new Vector3(0, 0, 0),
                Scale = new Vector3(50f, 50f, 50f),
                IsDynamic = false,
            };
            Objects.Add(meteorite);

            // Добавляем анимацию вращения для метеорита
            var meteoriteRotation = new RotationAnimation
            {
                Name = "MeteoriteRotation",
                TargetObject = meteorite,
                RotationSpeed = new Vector3(3, 2, 5)
            };
            meteoriteRotation.Start();
            Animations.Add(meteoriteRotation);

            // Поверхность Земли
            Objects.Add(new SceneObject
            {
                Name = "Поверхность Земли",
                Type = ObjectTypes.Plane,
                Texture = "textures/grass.jpg",
                TextureRepeat = 1000.0f,
                Position = new Vector3(0f, 0f, 0f),
                Rotation = new Vector3(0, 0, 0),
                Scale = new Vector3(10000f, 1f, 10000f),
                IsDynamic = false,
            });

            // Футбольное поле
            Objects.Add(new SceneObject
            {
                Name = "Футбольное поле",
                Type = ObjectTypes.Plane,
                Texture = "textures/footballfield.jpg",
                Position = new Vector3(0f, 0.1f, 0f),
                Rotation = new Vector3(0, 90, 0),
                Scale = new Vector3(120f, 1f, 60f),
                IsDynamic = false,
            });

            // Перегородки (используем призмы)
            Objects.Add(new SceneObject
            {
                Name = "Перегородка Левая",
                Type = ObjectTypes.Prism,
                Texture = "textures/tribuna.jpg",
                Position = new Vector3(-30f, 1f, 0f),
                Rotation = new Vector3(0, 0, 0),
                Scale = new Vector3(1f, 5f, 120f),
                IsDynamic = false,
            });
            Objects.Add(new SceneObject
            {
                Name = "Перегородка Правая",
                Type = ObjectTypes.Prism,
                Texture = "textures/tribuna.jpg",
                Position = new Vector3(30f, 1f, 0f),
                Rotation = new Vector3(0, 180, 0),
                Scale = new Vector3(1f, 5f, 120f),
                IsDynamic = false,
            });
            Objects.Add(new SceneObject
            {
                Name = "Перегородка Передняя",
                Type = ObjectTypes.Prism,
                Texture = "textures/tribuna.jpg",
                Position = new Vector3(0f, 1f, -60f),
                Rotation = new Vector3(0, 270, 0),
                Scale = new Vector3(1f, 5f, 60f),
                IsDynamic = false,
            });
            Objects.Add(new SceneObject
            {
                Name = "Перегородка Задняя",
                Type = ObjectTypes.Prism,
                Texture = "textures/tribuna.jpg",
                Position = new Vector3(0f, 1f, 60f),
                Rotation = new Vector3(0, 90, 0),
                Scale = new Vector3(1f, 5f, 60f),
                IsDynamic = false,
            });

            // Фрактальный объект
            Objects.Add(new SceneObject
            {
                Name = "Фрактал",
                Type = ObjectTypes.DiamondFractal,
                Texture = "textures/abstract1.jpg",
                Position = new Vector3(0f, 70f, 0),
                Rotation = new Vector3(0, 0, 0),
                Scale = new Vector3(3f, 3f, 3f),
                IsDynamic = false,
            });

            // Здания
            Objects.Add(new SceneObject
            {
                Name = "Здание Справа Даль",
                Type = ObjectTypes.Cube,
                Texture = "textures/build.jpg",
                TextureRepeat = 2.5f,
                Position = new Vector3(150f, 45f, -70f),
                Rotation = new Vector3(0, 0, 0),
                Scale = new Vector3(50f, 90f, 100f),
                IsDynamic = false,
            });
            Objects.Add(new SceneObject
            {
                Name = "Здание Справа Середина",
                Type = ObjectTypes.Cube,
                Texture = "textures/build.jpg",
                TextureRepeat = 1.5f,
                Position = new Vector3(150.1f, 62.5f, -14f),
                Rotation = new Vector3(0, 0, 0),
                Scale = new Vector3(50f, 55f, 50f),
                IsDynamic = false,
            });
            Objects.Add(new SceneObject
            {
                Name = "Здание Справа Близь",
                Type = ObjectTypes.Cube,
                Texture = "textures/build.jpg",
                TextureRepeat = 2.5f,
                Position = new Vector3(150f, 45f, 60f),
                Rotation = new Vector3(0, 0, 0),
                Scale = new Vector3(50f, 90f, 100f),
                IsDynamic = false,
            });
            Objects.Add(new SceneObject
            {
                Name = "Здание Спереди Нач",
                Type = ObjectTypes.Cube,
                Texture = "textures/build.jpg",
                TextureRepeat = 2.5f,
                Position = new Vector3(131f, 45f, -180f),
                Rotation = new Vector3(0, 0, 0),
                Scale = new Vector3(90f, 90f, 50f),
                IsDynamic = false,
            });
            Objects.Add(new SceneObject
            {
                Name = "Здание Спереди Середина",
                Type = ObjectTypes.Cube,
                Texture = "textures/build.jpg",
                TextureRepeat = 1.5f,
                Position = new Vector3(150.1f, 62.5f, -147f),
                Rotation = new Vector3(0, 0, 0),
                Scale = new Vector3(50f, 55f, 60f),
                IsDynamic = false,
            }); Objects.Add(new SceneObject
            {
                Name = "Здание Спереди Кон",
                Type = ObjectTypes.Cube,
                Texture = "textures/build.jpg",
                TextureRepeat = 2.5f,
                Position = new Vector3(191f, 45f, -180.1f),
                Rotation = new Vector3(0, 0, 0),
                Scale = new Vector3(90f, 90f, 50f),
                IsDynamic = false,
            });
            Objects.Add(new SceneObject
            {
                Name = "Здание Слева Даль",
                Type = ObjectTypes.Cube,
                Texture = "textures/build.jpg",
                TextureRepeat = 2.5f,
                Position = new Vector3(-140f, 45f, -150f),
                Rotation = new Vector3(0, 0, 0),
                Scale = new Vector3(50f, 90f, 120f),
                IsDynamic = false,
            });
            Objects.Add(new SceneObject
            {
                Name = "Здание Слева Середина",
                Type = ObjectTypes.Cube,
                Texture = "textures/build.jpg",
                TextureRepeat = 2.5f,
                Position = new Vector3(-140f, 45f, -30f),
                Rotation = new Vector3(0, 180, 0),
                Scale = new Vector3(50f, 90f, 120f),
                IsDynamic = false,
            });
            Objects.Add(new SceneObject
            {
                Name = "Здание Слева Близь",
                Type = ObjectTypes.Cube,
                Texture = "textures/build.jpg",
                TextureRepeat = 2.5f,
                Position = new Vector3(-140f, 45f, 90f),
                Rotation = new Vector3(0, 0, 0),
                Scale = new Vector3(50f, 90f, 120f),
                IsDynamic = false,
            });

            //Школа
            Objects.Add(new SceneObject
            {
                Name = "Школа",
                Type = ObjectTypes.Cube,
                Texture = "textures/school.jpg",
                TextureRepeat = 5f,
                Position = new Vector3(0f, 30f, -300f),
                Rotation = new Vector3(0, 0, 0),
                Scale = new Vector3(150f, 60f, 50f),
                IsDynamic = false,
            });
            Objects.Add(new SceneObject
            {
                Name = "Школа 1",
                Type = ObjectTypes.Cube,
                Texture = "textures/school1.jpg",
                Position = new Vector3(0f, 25f, -260f),
                Rotation = new Vector3(0, 0, 0),
                Scale = new Vector3(40f, 4f, 30f),
                IsDynamic = false,
            });
            Objects.Add(new SceneObject
            {
                Name = "Школа 2",
                Type = ObjectTypes.Cube,
                Texture = "textures/school1.jpg",
                Position = new Vector3(-15f, 10f, -250f),
                Rotation = new Vector3(0, 0, 0),
                Scale = new Vector3(5f, 30f, 5f),
                IsDynamic = false,
            });
            Objects.Add(new SceneObject
            {
                Name = "Школа 3",
                Type = ObjectTypes.Cube,
                Texture = "textures/school1.jpg",
                Position = new Vector3(15f, 10f, -250f),
                Rotation = new Vector3(0, 0, 0),
                Scale = new Vector3(5f, 30f, 5f),
                IsDynamic = false,
            });
            Objects.Add(new SceneObject
            {
                Name = "Школа 4",
                Type = ObjectTypes.Cube,
                Texture = "textures/abstract.jpg",
                TextureRepeat = 5f,
                Position = new Vector3(0f, 5f, -270f),
                Rotation = new Vector3(0, 0, 0),
                Scale = new Vector3(12f, 20f, 1f),
                IsDynamic = false,
            });

            // Детский сад
            Objects.Add(new SceneObject
            {
                Name = "Детский сад",
                Type = ObjectTypes.Cube,
                Texture = "textures/build2.jpg",
                TextureRepeat = 2f,
                Position = new Vector3(-20f, 25f, 250f),
                Rotation = new Vector3(0, 0, 0),
                Scale = new Vector3(100f, 50f, 55f),
                IsDynamic = false,
            });
            Objects.Add(new SceneObject
            {
                Name = "Детский сад 1",
                Type = ObjectTypes.Cube,
                Texture = "textures/build2.jpg",
                TextureRepeat = 2f,
                Position = new Vector3(30f, 25f, 250.1f),
                Rotation = new Vector3(0, 0, 0),
                Scale = new Vector3(100f, 50f, 55f),
                IsDynamic = false,
            });
            Objects.Add(new SceneObject
            {
                Name = "Детский сад 2",
                Type = ObjectTypes.Cube,
                Texture = "textures/build2.jpg",
                TextureRepeat = 2f,
                Position = new Vector3(80f, 25f, 250f),
                Rotation = new Vector3(0, 0, 0),
                Scale = new Vector3(100f, 50f, 55f),
                IsDynamic = false,
            });

            // Офис
            Objects.Add(new SceneObject
            {
                Name = "Офис",
                Type = ObjectTypes.Cube,
                Texture = "textures/build3.jpg",
                TextureRepeat = 2f,
                Position = new Vector3(-130f, 35f, 250.1f),
                Rotation = new Vector3(0, 0, 0),
                Scale = new Vector3(60f, 70f, 100f),
                IsDynamic = false,
            });

            // Администрация
            Objects.Add(new SceneObject
            {
                Name = "Администрация",
                Type = ObjectTypes.Cube,
                Texture = "textures/build4.jpg",
                TextureRepeat = 2f,
                Position = new Vector3(180f, 45f, 230f),
                Rotation = new Vector3(0, 0, 0),
                Scale = new Vector3(60f, 90f, 100f),
                IsDynamic = false,
            });

            // Детали сцены
            Objects.Add(new SceneObject
            {
                Name = "Портфель 1",
                Type = ObjectTypes.Cube,
                Texture = "textures/portfel.jpg",
                Position = new Vector3(-27f, 1f, -55f),
                Rotation = new Vector3(0, -90, 0),
                Scale = new Vector3(3f, 0.8f, 2.5f),
                IsDynamic = true,
            });
            Objects.Add(new SceneObject
            {
                Name = "Портфель 2",
                Type = ObjectTypes.Cube,
                Texture = "textures/portfel.jpg",
                Position = new Vector3(-27f, 3f, -55f),
                Rotation = new Vector3(-70, -90, 0),
                Scale = new Vector3(3f, 0.8f, 2.5f),
                IsDynamic = true,
            });
            Objects.Add(new SceneObject
            {
                Name = "Портфель 3",
                Type = ObjectTypes.Cube,
                Texture = "textures/portfel.jpg",
                Position = new Vector3(-27f, 2f, -52.65f),
                Rotation = new Vector3(-70, 180, 0),
                Scale = new Vector3(3f, 0.8f, 2.5f),
                IsDynamic = true,
            });

            // Добавляем футбольный мяч
            Objects.Add(new SceneObject
            {
                Name = "Футбольный мяч",
                Type = ObjectTypes.Sphere,
                Texture = "textures/footballball.jpg",
                Position = new Vector3(-3, 2f, -15),
                Rotation = new Vector3(0, 0, 0),
                Scale = new Vector3(1.8f, 1.8f, 1.8f),
                IsDynamic = true,
            });

            // Добавляем системы частиц для всех объектов в цикле
            foreach (var obj in Objects)
            {
                ParticleSystems.Add(new ParticleSystem(obj) { Name = obj.Name });
            }
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
        Prism,
        DiamondFractal
    }

    // Класс для представления объектов на сцене
    public class SceneObject
    {
        public string Parent { get; set; }
        public string? Texture { get; set; }  // Делаем Texture nullable
        public float TextureRepeat { get; set; } = 1.0f; // Коэффициент повторения текстуры
        public string Name { get; set; }

        public bool IsDynamic { get; set; }

        public ObjectTypes Type { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Color { get; set; } = new Vector3(1f, 1f, 1f); // Цвет по умолчанию - белый

        public Vector3 AppliedForce { get; set; }
        public bool isForceApplied { get; set; }

        public Vector3 Rotation { get; set; }
        public Vector3 Scale { get; set; }

        // Конструктор для инициализации свойств, связанных с силой
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
            // Для динамических объектов устанавливаем флаг для применения силы во время следующей синхронизации
            // Также обновляем позицию напрямую, чтобы изменения были видны немедленно в интерфейсе
            if (IsDynamic)
            {
                AppliedForce = vector3 * 50.0f; // Значительно увеличиваем силу, чтобы движение было более заметным
                isForceApplied = true;
                
                // Также обновляем позицию напрямую, чтобы изменения в интерфейсе появлялись немедленно
                Position += vector3;
            }
            else
            {
                // Для статических объектов обновляем позицию напрямую
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
    }
}