// SceneControls/InputHandler.cs
using System.Windows.Forms;

namespace Computer_Graphics_Programming___Blue_Meteorite
{
    public class InputHandler
    {
        private Camera camera;
        private bool isMousePressed;
        private System.Drawing.Point lastMousePosition;

        public InputHandler(Camera camera)
        {
            this.camera = camera;
        }

        public void HandleKeyDown(KeyEventArgs e)
        {
            // Управление камерой через клавиатуру
            if (e.KeyCode == Keys.W)
            {
                camera.MoveForward(1.0f);
            }
            else if (e.KeyCode == Keys.S)
            {
                camera.MoveBackward(1.0f);
            }
            // Дополнительное управление может быть добавлено сюда
        }

        public void HandleKeyUp(KeyEventArgs e)
        {
            // Здесь можно обрабатывать отпускание клавиш, если необходимо
        }

        public void HandleMouseMove(MouseEventArgs e)
        {
            if (isMousePressed)
            {
                // Вычисление смещения мыши
                var deltaX = e.X - lastMousePosition.X;
                var deltaY = e.Y - lastMousePosition.Y;

                camera.Rotate(deltaX, deltaY);

                // Обновляем позицию мыши
                lastMousePosition = e.Location;
            }
        }

        public void HandleMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isMousePressed = true;
                lastMousePosition = e.Location;
                Cursor.Hide();
            }
        }

        public void HandleMouseUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isMousePressed = false;
                Cursor.Show();
            }
        }

        public void HandleResize(int width, int height)
        {
            camera.UpdateProjection(width, height);
        }
    }
}