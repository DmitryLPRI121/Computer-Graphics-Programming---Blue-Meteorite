using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using System.Diagnostics;

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

        public SceneSettings(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings, SceneObjects ss)
            : base(gameWindowSettings, nativeWindowSettings)
        {
            sceneState = ss;
            camera = new Camera(new Vector3(0.0f, 2.0f, 3.0f), Vector3.UnitY);

            DynamicBody cameraDynamic = new DynamicBody(camera);
            cameraDynamic.floor_y = 3;
            camera.SelfDynamic = cameraDynamic;

            physicalStates.Add(cameraDynamic);
            lights = new List<Light>();
            globalLight = new GlobalLight();
            
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

        protected override void OnLoad()
        {
            base.OnLoad();

            CursorState = CursorState.Grabbed;

            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Texture2D);

            shader = new Shader("shaders/base.vert", "shaders/base.frag");

            // Инициализируем траву
            GrassFractal grass = new GrassFractal("textures/grass.jpg", 3, 0.8f, 0.08f, 0.3f, 1000);
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
                skybox = new Skybox();
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

        private TransformableObject recursiveBuild(SceneObject obj)
        {
            TransformableObject current;
            switch (obj.Type)
            {
                case ObjectTypes.Cube:
                    current = new Cube(obj.Texture);
                    break;
                case ObjectTypes.Sphere:
                    current = new Sphere(obj.Texture);
                    break;
                case ObjectTypes.Plane:
                    current = new Plane(obj.Texture);
                    break;
                case ObjectTypes.Prism:
                    current = new Prism(obj.Texture);
                    break;
                default:
                    throw new Exception();
            }

            current.Name = obj.Name;
            current.Position = obj.Position;
            current.Rotation = obj.Rotation;
            current.Scale = obj.Scale;

            foreach (var rel in sceneState.Objects.Where(o => o.Parent != null && o.Parent.Equals(obj.Name)))
            {
                current.AddChild(recursiveBuild(rel));
            }

            if (obj.IsDynamic)
            {
                DynamicBody db = new DynamicBody(current);
                physicalStates.Add(db);
                current.SelfDynamic = db;
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
                CursorState = CursorState.Normal;
            }

            // Физическое обновление
            foreach (var st in physicalStates)
            {
                // Применяем гравитацию
                if (!st.IsGrounded)
                {
                    st.ApplyGravity(sceneState.GravityStrength, (float)e.Time);
                }

                st.Update((float)e.Time);
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
                    }
                    else
                    {
                        skybox.SetTimeOfDay(sceneState.SkyboxTimeOfDay);
                    }
                }
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
            }
            base.OnMouseDown(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            // Включаем блендинг для прозрачности
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            // Обновляем тени для всех источников света
            foreach (var light in lights)
            {
                light.ReCompute(this);
            }

            // Render skybox first
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(camera.GetZoom()), Size.X / (float)Size.Y, 0.1f, 1000.0f);
            Matrix4 view = camera.GetViewMatrix();
            skybox.Render(projection, view);

            // Rest of the rendering
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Viewport(0, 0, Size.X, Size.Y);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            shader.Use();

            camera.SetCameraUniforms(shader, Size);
            
            // Устанавливаем количество источников света
            shader.SetInt("numLights", lights.Count);
            
            // Устанавливаем параметры для всех источников света
            for (int i = 0; i < lights.Count; i++)
            {
                var light = lights[i];
                var lightSettings = sceneState.LightSettings[i];
                
                light.Ambient = Vector3.Multiply(new Vector3(lightSettings.Color.R, lightSettings.Color.G, lightSettings.Color.B), lightSettings.AmbientIntensity);
                light.Diffuse = Vector3.Multiply(new Vector3(lightSettings.Color.R, lightSettings.Color.G, lightSettings.Color.B), lightSettings.DiffuseIntensity);
                light.Specular = Vector3.Multiply(new Vector3(lightSettings.Color.R, lightSettings.Color.G, lightSettings.Color.B), lightSettings.SpecularIntensity);
                light.Position = lightSettings.Position;
                
                light.SetLightUniforms(shader, i);
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

            // Отключаем блендинг после рендеринга
            GL.Disable(EnableCap.Blend);

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

                // Обновляем объекты
                foreach (var obj in sceneObjects)
                {
                    recursiveSync(obj);
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
                if (stateObj.IsDynamic)
                {
                    Matrix4 objMat = obj.GetModelMatrix();
                    stateObj.Position = objMat.ExtractTranslation();
                    stateObj.Rotation = objMat.ExtractRotation().Xyz;
                    stateObj.Scale = objMat.ExtractScale();
                    if (stateObj.isForceApplied)
                    {
                        obj.SelfDynamic.ApplyForce(stateObj.AppliedForce, 0.01f);
                    }
                }
                else
                {
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

            shader.Dispose();
            skybox.Dispose();
        }
    }
}