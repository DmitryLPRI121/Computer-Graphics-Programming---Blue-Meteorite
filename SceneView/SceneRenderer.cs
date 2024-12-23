// SceneView/SceneRenderer.cs
using System.Windows.Forms;
using Tao.OpenGl;
using Tao.Platform.Windows;

namespace Computer_Graphics_Programming___Blue_Meteorite
{
    public class SceneRenderer
    {
        private readonly SimpleOpenGlControl glControl;
        private Camera camera;
        private InputHandler inputHandler;

        public SceneRenderer(SimpleOpenGlControl glControl)
        {
            this.glControl = glControl;
            camera = new Camera();
            inputHandler = new InputHandler(camera);
        }

        public void Initialize()
        {
            Gl.glEnable(Gl.GL_DEPTH_TEST);
            Gl.glClearColor(0.1f, 0.1f, 0.1f, 1.0f);
        }

        public void Render()
        {
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
            camera.Apply();

            // The scene objects render
            RenderSceneObjects();

            glControl.Refresh();
        }

        private void RenderSceneObjects()
        {
            // Cube
            Object3D cube = CreateObject.Cube(1.0f);
            cube.SetPosition(0.0f, 0.0f, -5.0f)
                .SetRotation(30.0f, 45.0f, 0.0f)
                .SetScale(1.5f);

            // Sphere
            Object3D sphere = CreateObject.Sphere(0.5f, 20, 20);
            sphere.SetPosition(2.0f, 0.0f, -5.0f)
                  .SetScale(1.2f);

            cube.ChangeScale(1.0f);
        }

        public void HandleResize(int width, int height)
        {
            Gl.glViewport(0, 0, width, height);
            camera.UpdateProjection(width, height);
        }

        public void HandleKeyDown(KeyEventArgs e) => inputHandler.HandleKeyDown(e);
        public void HandleKeyUp(KeyEventArgs e) => inputHandler.HandleKeyUp(e);
        public void HandleMouseMove(MouseEventArgs e) => inputHandler.HandleMouseMove(e);
        public void HandleMouseDown(MouseEventArgs e) => inputHandler.HandleMouseDown(e);
        public void HandleMouseUp(MouseEventArgs e) => inputHandler.HandleMouseUp(e);
    }
}