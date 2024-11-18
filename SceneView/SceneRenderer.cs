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
        private Earth_Plane plane;
        private InputHandler inputHandler;

        public SceneRenderer(SimpleOpenGlControl glControl)
        {
            this.glControl = glControl;
            camera = new Camera();
            inputHandler = new InputHandler(camera); // создаем InputHandler
            plane = new Earth_Plane();
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
            plane.Render();
            glControl.Refresh();
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
