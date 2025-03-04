using OpenTK.Mathematics;

namespace Computer_Graphics_Programming_Blue_Meteorite
{
    public class Camera
    {
        public Vector3 Position { get; private set; }
        public Vector3 Front { get; private set; }
        public Vector3 Up { get; private set; }
        public Vector3 Right { get; private set; }
        public float Yaw { get; private set; } = -90.0f;
        public float Pitch { get; private set; } = 0.0f;
        private float speed = 2.5f;
        private float sensitivity = 0.1f;

        public Camera(Vector3 position)
        {
            Position = position;
            Front = new Vector3(0.0f, 0.0f, -1.0f);
            Up = new Vector3(0.0f, 1.0f, 0.0f);
            UpdateVectors();
        }

        public Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(Position, Position + Front, Up);
        }

        public void Move(Vector3 direction, float deltaTime)
        {
            Position += direction * (speed * deltaTime);
        }

        public void Rotate(float offsetX, float offsetY)
        {
            offsetX *= sensitivity;
            offsetY *= sensitivity;

            Yaw += offsetX;
            Pitch += offsetY;

            if (Pitch > 89.0f) Pitch = 89.0f;
            if (Pitch < -89.0f) Pitch = -89.0f;

            UpdateVectors();
        }

        private void UpdateVectors()
        {
            Vector3 front;
            front.X = MathF.Cos(MathHelper.DegreesToRadians(Yaw)) * MathF.Cos(MathHelper.DegreesToRadians(Pitch));
            front.Y = MathF.Sin(MathHelper.DegreesToRadians(Pitch));
            front.Z = MathF.Sin(MathHelper.DegreesToRadians(Yaw)) * MathF.Cos(MathHelper.DegreesToRadians(Pitch));

            Front = front.Normalized();
            Right = Vector3.Cross(Front, new Vector3(0.0f, 1.0f, 0.0f)).Normalized();
            Up = Vector3.Cross(Right, Front).Normalized();
        }
    }
}