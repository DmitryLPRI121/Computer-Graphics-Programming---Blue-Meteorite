// SceneView/Camera.cs
using System;
using System.Windows.Forms;
using Tao.OpenGl;

namespace Computer_Graphics_Programming___Blue_Meteorite
{
    public class Camera
    {
        private float x, y, z;
        private float yaw, pitch;
        private float speed = 0.1f;
        private float sensitivity = 0.1f;

        public Camera()
        {
            x = 0; y = 5; z = 10;
            yaw = 0; pitch = 0;
        }

        public void Apply()
        {
            Gl.glLoadIdentity();
            Glu.gluLookAt(x, y, z, x + (float)Math.Cos(yaw), y + (float)Math.Sin(pitch), z + (float)Math.Sin(yaw), 0, 1, 0);
        }

        public void MoveForward(float distance)
        {
            x += distance * (float)Math.Cos(yaw);
            z += distance * (float)Math.Sin(yaw);
        }

        public void MoveBackward(float distance)
        {
            x -= distance * (float)Math.Cos(yaw);
            z -= distance * (float)Math.Sin(yaw);
        }

        public void Rotate(float deltaX, float deltaY)
        {
            yaw += deltaX * sensitivity;
            pitch -= deltaY * sensitivity;

            if (pitch > Math.PI / 2) pitch = (float)(Math.PI / 2);
            if (pitch < -Math.PI / 2) pitch = (float)(-Math.PI / 2);
        }

        // Методы для обработки ввода (Keyboard and Mouse)
        public void HandleKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W)
            {
                MoveForward(1.0f);
            }
            else if (e.KeyCode == Keys.S)
            {
                MoveBackward(1.0f);
            }
        }

        public void HandleKeyUp(KeyEventArgs e)
        {
            // Может быть использован для дальнейшей логики
        }

        public void HandleMouseMove(MouseEventArgs e)
        {
            // Обрабатываем движение мыши
        }

        public void UpdateProjection(int width, int height)
        {
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            Glu.gluPerspective(60, (double)width / height, 0.1, 1000);
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
        }
    }
}