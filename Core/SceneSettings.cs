using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using Computer_Graphics_Programming_Blue_Meteorite.Graphics;

namespace Computer_Graphics_Programming_Blue_Meteorite
{
    public class SceneSettings : GameWindow
    {
        private Shader shader;
        public Skybox skybox;
        public List<TransformableObject> sceneObjects = new List<TransformableObject>();
        public List<DynamicBody> physicalStates = new List<DynamicBody>();

        public Camera camera;
        private List<Light> lights;
        public GlobalLight globalLight;
        SceneObjects sceneState;

        private Vector2 lastMousePosition;
        private bool firstMove = true;
        
        // Добавляем переменные для сохранения позиции курсора
        private float savedYaw;
        private float savedPitch;
        private bool hasSavedCameraState = false;

        // Post-processing framebuffer
        private int postProcessFBO;
        private int postProcessTexture;
        public GrayscaleFilter grayscaleFilter;
        public SepiaFilter sepiaFilter;
        public BlurFilter blurFilter;
        public PixelizedFilter pixelizedFilter;
        public NightVisionFilter nightVisionFilter;
        public SharpnessFilter sharpnessFilter;

        public SceneSettings(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings, SceneObjects ss)
            : base(gameWindowSettings, nativeWindowSettings)
        {
            sceneState = ss;
            camera = new Camera(new Vector3(0.0f, 2.0f, 3.0f), Vector3.UnitY);

            DynamicBody cameraDynamic = new DynamicBody(camera, Vector3.UnitY);
            cameraDynamic.floor_y = 4;
            camera.SelfDynamic = cameraDynamic;

            physicalStates.Add(cameraDynamic);
            lights = new List<Light>();
            globalLight = new GlobalLight(sceneState);
            
            // Инициализация настроек света
            if (sceneState.LightSettings != null)
            {
                foreach (var lightSettings in sceneState.LightSettings)
                {
                    var light = new Light();
                    light.Position = lightSettings.Position;
                    light.LookAt = lightSettings.LookAt;
                    light.Ambient = new Vector3(1.0f, 1.0f, 1.0f);
                    light.Diffuse = new Vector3(1.0f, 1.0f, 1.0f);
                    light.Specular = new Vector3(1.0f, 1.0f, 1.0f);
                    light.SetAttenuation(lightSettings.AttenuationA, lightSettings.AttenuationB, lightSettings.AttenuationC);
                    lights.Add(light);
                }
            }
        }

        public void InitializeFilters()
        {
            // Инициализируем фреймбуфер для пост-обработки
            InitializePostProcessFramebuffer();
            
            // Инициализируем фильтры
            grayscaleFilter = new GrayscaleFilter();
            sepiaFilter = new SepiaFilter();
            blurFilter = new BlurFilter();
            pixelizedFilter = new PixelizedFilter();
            nightVisionFilter = new NightVisionFilter();
            sharpnessFilter = new SharpnessFilter();
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            CursorState = CursorState.Grabbed;

            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Texture2D);

            shader = new Shader("shaders/base.vert", "shaders/base.frag");

            // Инициализируем траву
            GrassFractal grass = new GrassFractal("textures/grass2.jpg", 2, 0.8f, 0.08f, 0.3f, 10000, 100.0f);
            grass.Name = "Grass";
            grass.Position = new Vector3(0, 0, 0);
            sceneObjects.Add(grass);

            lock (sceneState)
            {
                // Инициализируем системы частиц
                foreach (var particleSystem in sceneState.ParticleSystems)
                {
                    particleSystem.Initialize();
                }

                // Инициализируем skybox
                skybox = new Skybox(sceneState);
                skybox.SetTimeOfDay(sceneState.SkyboxTimeOfDay);
                skybox.SetAutoUpdate(sceneState.SkyboxAutoUpdate);

                // Инициализируем глобальное освещение
                globalLight.SetTimeOfDay(sceneState.SkyboxTimeOfDay);
                globalLight.SetAutoUpdate(sceneState.SkyboxAutoUpdate);

                // Ищем корневые объекты
                foreach (var obj in sceneState.Objects.Where(o => o.Parent == null))
                {
                    sceneObjects.Add(recursiveBuild(obj));
                }
            }
        }

        private void InitializePostProcessFramebuffer()
        {
            // Создаем фреймбуфер
            postProcessFBO = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, postProcessFBO);

            // Создаем текстуру для цветового буфера
            postProcessTexture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, postProcessTexture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, Size.X, Size.Y, 0, PixelFormat.Rgb, PixelType.UnsignedByte, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            // Привязываем текстуру к фреймбуферу
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, postProcessTexture, 0);

            // Создаем и привязываем буфер глубины
            int rbo = GL.GenRenderbuffer();
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, rbo);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8, Size.X, Size.Y);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, rbo);

            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
            {
                throw new Exception("Post-process framebuffer is not complete!");
            }

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        private TransformableObject recursiveBuild(SceneObject obj)
        {
            TransformableObject current;
            switch (obj.Type)
            {
                case ObjectTypes.Cube:
                    current = new Cube(obj.Texture, obj.TextureRepeat);
                    break;
                case ObjectTypes.Sphere:
                    current = new Sphere(obj.Texture, 32, 16, obj.TextureRepeat);
                    break;
                case ObjectTypes.Plane:
                    current = new Plane(obj.Texture, obj.TextureRepeat);
                    break;
                case ObjectTypes.Prism:
                    current = new Prism(obj.Texture, obj.TextureRepeat);
                    break;
                case ObjectTypes.DiamondFractal:
                    current = new DiamondFractal(obj.Texture, 3, 1.0f, 0.3f, 1, null, obj.TextureRepeat);
                    break;
                default:
                    throw new Exception($"Unknown object type: {obj.Type}");
            }

            current.Name = obj.Name;
            current.Position = obj.Position;
            current.Rotation = obj.Rotation;
            current.Scale = obj.Scale;
            current.Color = new Vector3(1.0f, 1.0f, 1.0f);
            current.TextureRepeat = obj.TextureRepeat;

            if (obj.IsDynamic)
            {
                DynamicBody db = new DynamicBody(current, Vector3.UnitY);
                current.SelfDynamic = db;
                physicalStates.Add(db);
            }

            foreach (var rel in sceneState.Objects.Where(o => o.Parent != null && o.Parent.Equals(obj.Name)))
            {
                current.AddChild(recursiveBuild(rel));
            }

            return current;
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            SyncWithSceneState();

            // Управление камерой с помощью клавиатуры
            if (KeyboardState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.W))
                camera.ProcessKeyboard(CameraMovement.Forward, (float)e.Time);
            if (KeyboardState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.S))
                camera.ProcessKeyboard(CameraMovement.Backward, (float)e.Time);
            if (KeyboardState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.A))
                camera.ProcessKeyboard(CameraMovement.Left, (float)e.Time);
            if (KeyboardState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.D))
                camera.ProcessKeyboard(CameraMovement.Right, (float)e.Time);

            // Обработка прыжка
            if (KeyboardState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Space))
            {
                if (camera.SelfDynamic != null && camera.SelfDynamic.IsGrounded)
                {
                    camera.SelfDynamic.Jump(2.0f); // Уменьшено с 5.0f до 2.0f
                }
            }

            if (KeyboardState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Escape))
            {
                // Если курсор захвачен, сохраняем состояние камеры перед освобождением
                if (CursorState == CursorState.Grabbed && camera.SelfDynamic != null)
                {
                    savedYaw = ((DynamicBody)camera.SelfDynamic).Yaw;
                    savedPitch = ((DynamicBody)camera.SelfDynamic).Pitch;
                    hasSavedCameraState = true;
                }
                CursorState = CursorState.Normal;
            }

            // Физическое обновление всех динамических объектов
            foreach (var st in physicalStates)
            {
                // Применяем гравитацию ко всем динамическим объектам, которые не на земле
                if (!st.IsGrounded)
                {
                    st.ApplyGravity(sceneState.GravityStrength, (float)e.Time);
                }

                // Обновляем физическое состояние
                st.Update((float)e.Time);
            }

            // Обработка коллизий
            // Создаем список объектов, включая камеру
            var objectsWithCamera = new List<TransformableObject>(sceneObjects);
            objectsWithCamera.Add(camera); // Добавляем камеру в список объектов для коллизий
            
            var collisions = CollisionDetector.DetectAllCollisions(objectsWithCamera);
            if (collisions.Count > 0)
            {
                CollisionDetector.ResolveCollisions(collisions);
                
                // Дополнительная проверка после разрешения коллизий для всех типов объектов
                // чтобы убедиться, что персонаж не застрял внутри объектов
                foreach (var obj in sceneObjects)
                {
                    float cameraRadius = camera.CollisionRadius;
                    float objRadius = 0;
                    Vector3 closestPoint = Vector3.Zero;
                    bool needCheck = false;
                    
                    if (obj is Sphere sphere)
                    {
                        // Для сфер используем радиус
                        objRadius = 0.5f * MathF.Max(sphere.Scale.X, MathF.Max(sphere.Scale.Y, sphere.Scale.Z));
                        closestPoint = sphere.Position;
                        needCheck = true;
                    }
                    else if (obj is Cube cube)
                    {
                        // Для кубов находим ближайшую точку на поверхности куба
                        Vector3 halfSize = cube.Scale * 0.5f;
                        Vector3 cubeToCamera = camera.Position - cube.Position;
                        
                        // Ограничиваем по каждой оси
                        Vector3 clampedPoint = Vector3.Zero;
                        clampedPoint.X = MathHelper.Clamp(cubeToCamera.X, -halfSize.X, halfSize.X);
                        clampedPoint.Y = MathHelper.Clamp(cubeToCamera.Y, -halfSize.Y, halfSize.Y);
                        clampedPoint.Z = MathHelper.Clamp(cubeToCamera.Z, -halfSize.Z, halfSize.Z);
                        
                        // Проверяем, находится ли камера внутри куба
                        bool insideX = MathF.Abs(cubeToCamera.X) < halfSize.X;
                        bool insideY = MathF.Abs(cubeToCamera.Y) < halfSize.Y;
                        bool insideZ = MathF.Abs(cubeToCamera.Z) < halfSize.Z;
                        
                        if (insideX && insideY && insideZ)
                        {
                            // Если камера внутри куба, находим ближайшую грань
                            float distToXFace = MathF.Min(halfSize.X - MathF.Abs(cubeToCamera.X), halfSize.X + MathF.Abs(cubeToCamera.X));
                            float distToYFace = MathF.Min(halfSize.Y - MathF.Abs(cubeToCamera.Y), halfSize.Y + MathF.Abs(cubeToCamera.Y));
                            float distToZFace = MathF.Min(halfSize.Z - MathF.Abs(cubeToCamera.Z), halfSize.Z + MathF.Abs(cubeToCamera.Z));
                            
                            if (distToXFace <= distToYFace && distToXFace <= distToZFace)
                            {
                                clampedPoint = new Vector3(
                                    cubeToCamera.X > 0 ? halfSize.X : -halfSize.X,
                                    cubeToCamera.Y,
                                    cubeToCamera.Z);
                            }
                            else if (distToYFace <= distToXFace && distToYFace <= distToZFace)
                            {
                                clampedPoint = new Vector3(
                                    cubeToCamera.X,
                                    cubeToCamera.Y > 0 ? halfSize.Y : -halfSize.Y,
                                    cubeToCamera.Z);
                            }
                            else
                            {
                                clampedPoint = new Vector3(
                                    cubeToCamera.X,
                                    cubeToCamera.Y,
                                    cubeToCamera.Z > 0 ? halfSize.Z : -halfSize.Z);
                            }
                        }
                        
                        closestPoint = cube.Position + clampedPoint;
                        objRadius = 0; // Используем фактическую точку на поверхности
                        needCheck = true;
                    }
                    else if (obj is Prism)
                    {
                        // Отключаем коллизии камеры с призмами
                        continue;
                    }
                    else if (obj is Prism prism)
                    {
                        // Для призм используем улучшенный подход с учетом наклонной верхней грани
                        Vector3 halfSize = prism.Scale * 0.5f;
                        Vector3 prismToCamera = camera.Position - prism.Position;
                        
                        // Проверяем позицию камеры относительно призмы
                        bool insideX = MathF.Abs(prismToCamera.X) < halfSize.X;
                        bool insideZ = MathF.Abs(prismToCamera.Z) < halfSize.Z;
                        bool insideY = prismToCamera.Y > -halfSize.Y && 
                                    prismToCamera.Y < halfSize.Y;
                        
                        // Определяем, находится ли камера в прямоугольной части призмы (нижняя часть)
                        bool insideRectBase = insideX && insideZ && prismToCamera.Y < 0 && insideY;
                        
                        // Определяем, находится ли камера в треугольной части призмы (верхняя часть)
                        bool potentiallyInsideTop = insideX && insideZ && prismToCamera.Y >= 0 && insideY;
                        
                        if (potentiallyInsideTop)
                        {
                            // Рассчитываем максимальную высоту в данной точке XZ
                            float normalizedX = MathF.Abs(prismToCamera.X) / halfSize.X;
                            float normalizedZ = MathF.Abs(prismToCamera.Z) / halfSize.Z;
                            float maxYAtPosition = halfSize.Y * (1.0f - MathF.Max(normalizedX, normalizedZ));
                            
                            // Проверяем, находится ли камера под наклонной гранью
                            bool insideTopTriangle = prismToCamera.Y < maxYAtPosition;
                            
                            if (insideTopTriangle)
                            {
                                // Камера внутри треугольной части, нужно вытолкнуть наружу
                                
                                // Найдем ближайшую грань - боковую или верхнюю
                                float distToXEdge = halfSize.X - MathF.Abs(prismToCamera.X);
                                float distToZEdge = halfSize.Z - MathF.Abs(prismToCamera.Z);
                                float distToTop = maxYAtPosition - prismToCamera.Y;
                                
                                Vector3 pushDirection;
                                float pushDistance;
                                
                                if (distToXEdge <= distToZEdge && distToXEdge <= distToTop)
                                {
                                    // Ближе к X-грани
                                    pushDirection = new Vector3(prismToCamera.X > 0 ? 1 : -1, 0, 0);
                                    pushDistance = distToXEdge + cameraRadius + 0.1f;
                                }
                                else if (distToZEdge <= distToXEdge && distToZEdge <= distToTop)
                                {
                                    // Ближе к Z-грани
                                    pushDirection = new Vector3(0, 0, prismToCamera.Z > 0 ? 1 : -1);
                                    pushDistance = distToZEdge + cameraRadius + 0.1f;
                                }
                                else
                                {
                                    // Ближе к верхней грани - вычисляем нормаль к наклонной поверхности
                                    float nx = prismToCamera.X / halfSize.X;
                                    float nz = prismToCamera.Z / halfSize.Z;
                                    pushDirection = Vector3.Normalize(new Vector3(nx, 1.0f, nz));
                                    pushDistance = distToTop + cameraRadius + 0.1f;
                                }
                                
                                // Выталкиваем камеру
                                camera.Position = prism.Position + pushDirection * pushDistance;
                                
                                // Корректируем скорость, чтобы избежать повторного проникновения
                                if (camera.SelfDynamic != null)
                                {
                                    float velocityTowardsPrism = Vector3.Dot(camera.SelfDynamic._velocity, -pushDirection);
                                    if (velocityTowardsPrism > 0)
                                    {
                                        camera.SelfDynamic._velocity -= -pushDirection * velocityTowardsPrism;
                                    }
                                }
                                
                                needCheck = false; // Уже обработали этот случай
                            }
                        }
                        else if (insideRectBase)
                        {
                            // Камера внутри прямоугольной части, нужно вытолкнуть наружу
                            
                            // Найдем ближайшую грань
                            float distToXEdge = halfSize.X - MathF.Abs(prismToCamera.X);
                            float distToZEdge = halfSize.Z - MathF.Abs(prismToCamera.Z);
                            float distToBottom = halfSize.Y + prismToCamera.Y;
                            float distToTop = -prismToCamera.Y; // Расстояние до верхней плоскости основания
                            
                            Vector3 pushDirection;
                            float pushDistance;
                            
                            if (distToXEdge <= distToZEdge && distToXEdge <= distToBottom && distToXEdge <= distToTop)
                            {
                                // Ближе к X-грани
                                pushDirection = new Vector3(prismToCamera.X > 0 ? 1 : -1, 0, 0);
                                pushDistance = distToXEdge + cameraRadius + 0.1f;
                            }
                            else if (distToZEdge <= distToXEdge && distToZEdge <= distToBottom && distToZEdge <= distToTop)
                            {
                                // Ближе к Z-грани
                                pushDirection = new Vector3(0, 0, prismToCamera.Z > 0 ? 1 : -1);
                                pushDistance = distToZEdge + cameraRadius + 0.1f;
                            }
                            else if (distToBottom <= distToXEdge && distToBottom <= distToZEdge && distToBottom <= distToTop)
                            {
                                // Ближе к нижней грани
                                pushDirection = new Vector3(0, -1, 0);
                                pushDistance = distToBottom + cameraRadius + 0.1f;
                            }
                            else
                            {
                                // Ближе к верхней плоскости основания
                                pushDirection = new Vector3(0, 1, 0);
                                pushDistance = distToTop + cameraRadius + 0.1f;
                            }
                            
                            // Выталкиваем камеру
                            camera.Position = prism.Position + pushDirection * pushDistance;
                            
                            // Корректируем скорость, чтобы избежать повторного проникновения
                            if (camera.SelfDynamic != null)
                            {
                                float velocityTowardsPrism = Vector3.Dot(camera.SelfDynamic._velocity, -pushDirection);
                                if (velocityTowardsPrism > 0)
                                {
                                    camera.SelfDynamic._velocity -= -pushDirection * velocityTowardsPrism;
                                }
                            }
                            
                            needCheck = false; // Уже обработали этот случай
                        }
                        
                        if (needCheck)
                        {
                            // Для случаев, когда камера снаружи, но может быть близко к призме
                            // Используем стандартный подход с ближайшей точкой
                            Vector3 clampedPoint = Vector3.Zero;
                            
                            // Ограничиваем по X и Z
                            clampedPoint.X = MathHelper.Clamp(prismToCamera.X, -halfSize.X, halfSize.X);
                            clampedPoint.Z = MathHelper.Clamp(prismToCamera.Z, -halfSize.Z, halfSize.Z);
                            
                            // Для Y координаты учитываем форму призмы
                            if (prismToCamera.Y >= 0)
                            {
                                // Верхняя треугольная часть
                                float normalizedX = MathF.Abs(clampedPoint.X) / halfSize.X;
                                float normalizedZ = MathF.Abs(clampedPoint.Z) / halfSize.Z;
                                float maxY = halfSize.Y * (1.0f - MathF.Max(normalizedX, normalizedZ));
                                clampedPoint.Y = MathHelper.Clamp(prismToCamera.Y, 0, maxY);
                            }
                            else
                            {
                                // Нижняя прямоугольная часть
                                clampedPoint.Y = MathHelper.Clamp(prismToCamera.Y, -halfSize.Y, 0);
                            }
                            
                            closestPoint = prism.Position + clampedPoint;
                            objRadius = 0; // Используем фактическую точку на поверхности
                        }
                    }
                    
                    if (needCheck)
                    {
                        // Вектор от объекта к камере
                        Vector3 dirToCamera = camera.Position - closestPoint;
                        float distance = dirToCamera.Length;
                        
                        // Минимальное допустимое расстояние между объектом и камерой
                        float minAllowedDistance = objRadius + cameraRadius + 0.05f; // Добавляем буфер
                        
                        // Если камера оказалась слишком близко или внутри объекта
                        if (distance < minAllowedDistance)
                        {
                            // Нормализуем направление
                            Vector3 dirNormalized = distance > 0.01f ? dirToCamera / distance : new Vector3(0, 1, 0);
                            
                            // Если направление близко к вертикальному и объект под нами, не выталкиваем
                            bool isUpwardsDir = dirNormalized.Y > 0.95f;
                            bool isStandingOnObject = camera.Position.Y > obj.Position.Y + obj.Scale.Y * 0.4f;
                            
                            if (!(isUpwardsDir && isStandingOnObject))
                            {
                                // Корректируем позицию камеры, выталкивая ее наружу
                                camera.Position = closestPoint + dirNormalized * minAllowedDistance;
                                
                                // Если камера двигалась в сторону объекта, останавливаем это движение
                                if (camera.SelfDynamic != null)
                                {
                                    float velocityTowardsObject = Vector3.Dot(camera.SelfDynamic._velocity, -dirNormalized);
                                    if (velocityTowardsObject > 0)
                                    {
                                        camera.SelfDynamic._velocity -= -dirNormalized * velocityTowardsObject;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // Обновление систем частиц
            lock (sceneState)
            {
                foreach (var particleSystem in sceneState.ParticleSystems)
                {
                    particleSystem.Update((float)e.Time);
                }

                // Обновление анимаций
                foreach (var animation in sceneState.Animations)
                {
                    animation.Update((float)e.Time);
                }
            }

            // Обновление skybox и глобального освещения
            if (skybox != null)
            {
                lock (sceneState)
                {
                    if (sceneState.SkyboxAutoUpdate)
                    {
                        skybox.Update((float)e.Time);
                        // Синхронизируем время с sceneState
                        sceneState.SkyboxTimeOfDay = skybox.GetTimeOfDay();
                    }
                    else
                    {
                        skybox.SetTimeOfDay(sceneState.SkyboxTimeOfDay);
                    }
                }
                // Обновляем глобальное освещение после skybox
                globalLight.Update((float)e.Time);
            }
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);
            if (CursorState != CursorState.Grabbed) return;
            if (firstMove)
            {
                lastMousePosition = new Vector2(e.X, e.Y);
                firstMove = false;
            }
            else
            {
                float deltaX = e.X - lastMousePosition.X;
                float deltaY = lastMousePosition.Y - e.Y;
                lastMousePosition = new Vector2(e.X, e.Y);

                camera.ProcessMouseMovement(deltaX, deltaY);
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if ((e.Button == OpenTK.Windowing.GraphicsLibraryFramework.MouseButton.Button1 ||
                 e.Button == OpenTK.Windowing.GraphicsLibraryFramework.MouseButton.Left) &&
                CursorState != CursorState.Grabbed)
            {
                CursorState = CursorState.Grabbed;
                
                // Восстанавливаем сохранённое положение камеры при повторном захвате
                if (hasSavedCameraState && camera.SelfDynamic != null)
                {
                    ((DynamicBody)camera.SelfDynamic).RestoreRotation(savedYaw, savedPitch);
                    firstMove = true;
                }
            }
            // Обработка нажатия на колесико мыши для переключения полноэкранного режима
            else if ((e.Button == OpenTK.Windowing.GraphicsLibraryFramework.MouseButton.Button3 ||
                      e.Button == OpenTK.Windowing.GraphicsLibraryFramework.MouseButton.Middle) &&
                     CursorState == CursorState.Grabbed)
            {
                // Переключаем режим окна между обычным и полноэкранным
                if (WindowState == WindowState.Normal)
                {
                    WindowState = WindowState.Fullscreen;
                }
                else
                {
                    WindowState = WindowState.Normal;
                }
            }
            base.OnMouseDown(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            // Включаем блендинг для прозрачности
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            // Создаем матрицы проекции и вида
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(camera.GetZoom()), Size.X / (float)Size.Y, 0.1f, 1000.0f);
            Matrix4 view = camera.GetViewMatrix();

            // Синхронизируем источники света перед рендерингом
            SyncWithSceneState();

            // Обновляем тени для всех источников света
            foreach (var light in lights)
            {
                light.ReCompute(this);
            }

            // Привязываем фреймбуфер пост-обработки
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, postProcessFBO);
            GL.Viewport(0, 0, Size.X, Size.Y);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            shader.Use();

            camera.SetCameraUniforms(shader, Size);
            
            // Устанавливаем количество источников света
            shader.SetInt("numLights", lights.Count);
            
            // Устанавливаем параметры для всех источников света, если они есть
            if (lights.Count > 0)
            {
                lock (sceneState)
                {
                    for (int i = 0; i < lights.Count; i++)
                    {
                        if (i >= sceneState.LightSettings.Count)
                            break;
                            
                        var light = lights[i];
                        var lightSettings = sceneState.LightSettings[i];
                        
                        light.Ambient = Vector3.Multiply(new Vector3(lightSettings.Color.R, lightSettings.Color.G, lightSettings.Color.B), lightSettings.AmbientIntensity);
                        light.Diffuse = Vector3.Multiply(new Vector3(lightSettings.Color.R, lightSettings.Color.G, lightSettings.Color.B), lightSettings.DiffuseIntensity);
                        light.Specular = Vector3.Multiply(new Vector3(lightSettings.Color.R, lightSettings.Color.G, lightSettings.Color.B), lightSettings.SpecularIntensity);
                        light.Position = lightSettings.Position;
                        
                        light.SetLightUniforms(shader, i);
                    }
                }
            }
            
            globalLight.SetLightUniforms(shader);

            foreach (var obj in sceneObjects)
            {
                obj.Render(shader);
            }

            // Рендеринг систем частиц
            lock (sceneState)
            {
                foreach (var particleSystem in sceneState.ParticleSystems)
                {
                    particleSystem.Render(shader, projection, view, camera.Position);
                }
            }

            // Рендерим skybox последним
            if (skybox != null)
            {
                skybox.Render(projection, view);
            }

            // Отключаем блендинг после рендеринга
            GL.Disable(EnableCap.Blend);

            // Привязываем основной фреймбуфер
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Viewport(0, 0, Size.X, Size.Y);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Применяем пост-обработку
            if (nightVisionFilter.IsEnabled)
            {
                nightVisionFilter.Apply(postProcessTexture);
            }
            else if (pixelizedFilter.IsEnabled)
            {
                pixelizedFilter.Apply(postProcessTexture, new Vector2(Size.X, Size.Y));
            }
            else if (blurFilter.IsEnabled)
            {
                blurFilter.Apply(postProcessTexture, new Vector2(Size.X, Size.Y));
            }
            else if (sepiaFilter.IsEnabled)
            {
                sepiaFilter.Apply(postProcessTexture);
            }
            else if (grayscaleFilter.IsEnabled)
            {
                grayscaleFilter.Apply(postProcessTexture);
            }
            else if (sharpnessFilter.IsEnabled)
            {
                sharpnessFilter.Apply(postProcessTexture, new Vector2(Size.X, Size.Y));
            }
            else
            {
                // Если фильтры отключены, используем шейдер grayscaleFilter для отображения текстуры
                grayscaleFilter.Apply(postProcessTexture);
            }

            SwapBuffers();
        }

        private void SyncWithSceneState()
        {
            lock (sceneState)
            {
                // Обновляем все параметры света
                if (sceneState.LightSettings != null)
                {
                    // Удаляем лишние источники света
                    while (lights.Count > sceneState.LightSettings.Count)
                    {
                        lights.RemoveAt(lights.Count - 1);
                    }

                    // Добавляем новые источники света
                    while (lights.Count < sceneState.LightSettings.Count)
                    {
                        lights.Add(new Light());
                    }

                    // Обновляем параметры существующих источников света
                    for (int i = 0; i < lights.Count; i++)
                    {
                        var light = lights[i];
                        var lightSettings = sceneState.LightSettings[i];
                        
                        light.Position = lightSettings.Position;
                        light.LookAt = lightSettings.LookAt;
                        light.Ambient = Vector3.Multiply(new Vector3(lightSettings.Color.R, lightSettings.Color.G, lightSettings.Color.B), lightSettings.AmbientIntensity);
                        light.Diffuse = Vector3.Multiply(new Vector3(lightSettings.Color.R, lightSettings.Color.G, lightSettings.Color.B), lightSettings.DiffuseIntensity);
                        light.Specular = Vector3.Multiply(new Vector3(lightSettings.Color.R, lightSettings.Color.G, lightSettings.Color.B), lightSettings.SpecularIntensity);
                        light.SetAttenuation(lightSettings.AttenuationA, lightSettings.AttenuationB, lightSettings.AttenuationC);
                    }
                }

                // Sync transformable objects from sceneState (but preserve physics for dynamic objects)
                foreach (var obj in sceneObjects)
                {
                    recursiveSync(obj);
                }
                
                // After physics calculations, sync the positions of dynamic objects back to sceneState
                foreach (var obj in sceneObjects)
                {
                    if (obj.SelfDynamic != null)
                    {
                        var stateObj = sceneState.Objects.Find(o => o.Name.Equals(obj.Name));
                        if (stateObj != null && stateObj.IsDynamic)
                        {
                            // Update the scene state with the physics-calculated position
                            stateObj.Position = obj.Position;
                        }
                    }
                }
            }
        }

        private void recursiveSync(TransformableObject obj)
        {
            foreach (var child in obj.Children)
            {
                recursiveSync(child);
            }

            var stateObj = sceneState.Objects.Find(o => o.Name.Equals(obj.Name));
            if (stateObj != null)
            {
                if (stateObj.IsDynamic && obj.SelfDynamic != null)
                {
                    // Always apply rotation and scale from sceneState
                    obj.Rotation = stateObj.Rotation;
                    obj.Scale = stateObj.Scale;
                    
                    // For direct position setting from the transform panel,
                    // we need to detect if position has changed significantly
                    Vector3 positionDifference = stateObj.Position - obj.Position;
                    bool positionChangedSignificantly = positionDifference.Length > 1.0f;
                    
                    // If position was explicitly set in the UI (large change) or the object is idle,
                    // update position directly
                    if (positionChangedSignificantly || obj.SelfDynamic.Velocity.Length < 0.05f)
                    {
                        obj.Position = stateObj.Position;
                        // Reset velocity when position is explicitly set
                        if (positionChangedSignificantly)
                        {
                            obj.SelfDynamic._velocity = Vector3.Zero;
                        }
                    }

                    // Apply forces from UI controls
                    if (stateObj.isForceApplied)
                    {
                        // Reset the object's velocity to make the force application more direct
                        obj.SelfDynamic._velocity = Vector3.Zero;
                        
                        // Apply the force from UI control with higher magnitude for more immediate effect
                        obj.SelfDynamic.ApplyForce(stateObj.AppliedForce, 0.1f);
                        stateObj.isForceApplied = false; // Reset force flag after applying
                    }
                }
                else
                {
                    // For non-dynamic objects, simply update all properties from sceneState
                    obj.Position = stateObj.Position;
                    obj.Rotation = stateObj.Rotation;
                    obj.Scale = stateObj.Scale;
                }
            }
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Size.X, Size.Y);

            // Обновляем размер текстуры пост-обработки
            GL.BindTexture(TextureTarget.Texture2D, postProcessTexture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, Size.X, Size.Y, 0, PixelFormat.Rgb, PixelType.UnsignedByte, IntPtr.Zero);

            // Обновляем размер буфера глубины
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, postProcessFBO);
            int rbo = GL.GenRenderbuffer();
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, rbo);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8, Size.X, Size.Y);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, rbo);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        protected override void OnUnload()
        {
            base.OnUnload();

            // Освобождаем ресурсы травы
            foreach (var obj in sceneObjects)
            {
                if (obj is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }

            // Освобождаем ресурсы пост-обработки
            GL.DeleteFramebuffer(postProcessFBO);
            GL.DeleteTexture(postProcessTexture);
            grayscaleFilter.Cleanup();
            sepiaFilter.Cleanup();
            blurFilter.Cleanup();
            pixelizedFilter.Cleanup();
            nightVisionFilter.Cleanup();
            sharpnessFilter.Cleanup();

            shader.Dispose();
            skybox.Dispose();
        }
    }
}