using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using System.Diagnostics;

namespace Computer_Graphics_Programming_Blue_Meteorite
{
    public class Scene : GameWindow
    {
        private Shader shader;
        public Skybox skybox;
        public List<TransformableObject> sceneObjects = new List<TransformableObject>();
        public List<DynamicBody> physicalStates = new List<DynamicBody>();

        private Camera camera;
        Light light;
        SceneState sceneState;

        private Vector2 lastMousePosition;
        private bool firstMove = true;

        public Scene(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings, SceneState ss)
            : base(gameWindowSettings, nativeWindowSettings)
        {
            sceneState = ss;
            camera = new Camera(new Vector3(0.0f, 0.0f, 3.0f), Vector3.UnitY);

            DynamicBody cameraDynamic = new DynamicBody(camera);
            cameraDynamic.floor_y = 1;
            camera.SelfDynamic = cameraDynamic;

            physicalStates.Add(cameraDynamic);
            light = new Light();
            
            // Инициализация настроек света
            if (sceneState.LightSettings != null)
            {
                light.Position = sceneState.LightSettings.Position;
                light.LookAt = sceneState.LightSettings.LookAt;
                light.Ambient = Vector3.Multiply(new Vector3(sceneState.LightSettings.Color.R, sceneState.LightSettings.Color.G, sceneState.LightSettings.Color.B), sceneState.LightSettings.AmbientIntensity);
                light.Diffuse = Vector3.Multiply(new Vector3(sceneState.LightSettings.Color.R, sceneState.LightSettings.Color.G, sceneState.LightSettings.Color.B), sceneState.LightSettings.DiffuseIntensity);
                light.Specular = Vector3.Multiply(new Vector3(sceneState.LightSettings.Color.R, sceneState.LightSettings.Color.G, sceneState.LightSettings.Color.B), sceneState.LightSettings.SpecularIntensity);
                light.SetAttenuation(sceneState.LightSettings.AttenuationA, sceneState.LightSettings.AttenuationB, sceneState.LightSettings.AttenuationC);
            }
            else
            {
                // Устанавливаем значения по умолчанию для света
                sceneState.LightSettings = new LightSettings
                {
                    Position = new Vector3(0, 5, 10),
                    LookAt = new Vector3(0, 0, 0),
                    AmbientIntensity = 1.0f,
                    DiffuseIntensity = 2.0f,
                    SpecularIntensity = 1.0f,
                    Color = new Color4(1.0f, 1.0f, 1.0f, 1.0f),
                    AttenuationA = 1.0f,
                    AttenuationB = 0.09f,
                    AttenuationC = 0.032f
                };
                
                light.Position = sceneState.LightSettings.Position;
                light.LookAt = sceneState.LightSettings.LookAt;
                light.Ambient = Vector3.Multiply(new Vector3(sceneState.LightSettings.Color.R, sceneState.LightSettings.Color.G, sceneState.LightSettings.Color.B), sceneState.LightSettings.AmbientIntensity);
                light.Diffuse = Vector3.Multiply(new Vector3(sceneState.LightSettings.Color.R, sceneState.LightSettings.Color.G, sceneState.LightSettings.Color.B), sceneState.LightSettings.DiffuseIntensity);
                light.Specular = Vector3.Multiply(new Vector3(sceneState.LightSettings.Color.R, sceneState.LightSettings.Color.G, sceneState.LightSettings.Color.B), sceneState.LightSettings.SpecularIntensity);
                light.SetAttenuation(sceneState.LightSettings.AttenuationA, sceneState.LightSettings.AttenuationB, sceneState.LightSettings.AttenuationC);
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

            lock (sceneState)
            {
                // Инициализация настроек света
                if (sceneState.LightSettings != null)
                {
                    // Устанавливаем значения по умолчанию, если они не были установлены
                    if (sceneState.LightSettings.Position == Vector3.Zero)
                    {
                        sceneState.LightSettings.Position = new Vector3(0, 5, 10);
                    }
                    if (sceneState.LightSettings.LookAt == Vector3.Zero)
                    {
                        sceneState.LightSettings.LookAt = new Vector3(0, 0, 0);
                    }
                    if (sceneState.LightSettings.AmbientIntensity == 0)
                    {
                        sceneState.LightSettings.AmbientIntensity = 1.0f;
                    }
                    if (sceneState.LightSettings.DiffuseIntensity == 0)
                    {
                        sceneState.LightSettings.DiffuseIntensity = 2.0f;
                    }
                    if (sceneState.LightSettings.SpecularIntensity == 0)
                    {
                        sceneState.LightSettings.SpecularIntensity = 1.0f;
                    }
                    if (sceneState.LightSettings.Color.R == 0 && sceneState.LightSettings.Color.G == 0 && sceneState.LightSettings.Color.B == 0)
                    {
                        sceneState.LightSettings.Color = new Color4(1.0f, 1.0f, 1.0f, 1.0f);
                    }

                    // Применяем настройки света
                    light.Position = sceneState.LightSettings.Position;
                    light.LookAt = sceneState.LightSettings.LookAt;
                    light.Ambient = Vector3.Multiply(new Vector3(sceneState.LightSettings.Color.R, sceneState.LightSettings.Color.G, sceneState.LightSettings.Color.B), sceneState.LightSettings.AmbientIntensity);
                    light.Diffuse = Vector3.Multiply(new Vector3(sceneState.LightSettings.Color.R, sceneState.LightSettings.Color.G, sceneState.LightSettings.Color.B), sceneState.LightSettings.DiffuseIntensity);
                    light.Specular = Vector3.Multiply(new Vector3(sceneState.LightSettings.Color.R, sceneState.LightSettings.Color.G, sceneState.LightSettings.Color.B), sceneState.LightSettings.SpecularIntensity);
                    light.SetAttenuation(sceneState.LightSettings.AttenuationA, sceneState.LightSettings.AttenuationB, sceneState.LightSettings.AttenuationC);
                }

                // Инициализируем системы частиц
                foreach (var particleSystem in sceneState.ParticleSystems)
                {
                    particleSystem.Initialize();
                }

                // Инициализируем skybox
                skybox = new Skybox();
                skybox.SetTimeOfDay(sceneState.SkyboxTimeOfDay);
                skybox.SetAutoUpdate(sceneState.SkyboxAutoUpdate);

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
                    camera.SelfDynamic.Jump(5.0f); // Сила прыжка
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
            }

            // Обновление skybox
            if (skybox != null)
            {
                skybox.Update((float)e.Time);
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

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            camera.ProcessMouseScroll(e.OffsetY);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            // Render skybox first
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(camera.GetZoom()), Size.X / (float)Size.Y, 0.1f, 1000.0f);
            Matrix4 view = camera.GetViewMatrix();
            skybox.Render(projection, view);

            // Rest of the rendering
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Viewport(0, 0, Size.X, Size.Y);
            GL.Clear(ClearBufferMask.DepthBufferBit);

            shader.Use();

            camera.SetCameraUniforms(shader, Size);
            light.SetLightUniforms(shader);

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

            SwapBuffers();
        }

        private void SyncWithSceneState()
        {
            lock (sceneState)
            {
                // Обновляем все параметры света
                if (sceneState.LightSettings != null)
                {
                    light.Position = sceneState.LightSettings.Position;
                    light.LookAt = sceneState.LightSettings.LookAt;
                    light.Ambient = Vector3.Multiply(new Vector3(sceneState.LightSettings.Color.R, sceneState.LightSettings.Color.G, sceneState.LightSettings.Color.B), sceneState.LightSettings.AmbientIntensity);
                    light.Diffuse = Vector3.Multiply(new Vector3(sceneState.LightSettings.Color.R, sceneState.LightSettings.Color.G, sceneState.LightSettings.Color.B), sceneState.LightSettings.DiffuseIntensity);
                    light.Specular = Vector3.Multiply(new Vector3(sceneState.LightSettings.Color.R, sceneState.LightSettings.Color.G, sceneState.LightSettings.Color.B), sceneState.LightSettings.SpecularIntensity);
                    light.SetAttenuation(sceneState.LightSettings.AttenuationA, sceneState.LightSettings.AttenuationB, sceneState.LightSettings.AttenuationC);
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
            shader.Dispose();
            skybox.Dispose();
        }
    }
}