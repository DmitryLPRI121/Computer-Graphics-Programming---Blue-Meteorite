using OpenTK.Mathematics;

namespace Computer_Graphics_Programming_Blue_Meteorite
{ 
    public class Camera : TransformableObject
    {
        private readonly float CAMERA_FLOOR = 1.0f;
        private Vector3 front;
        private Vector3 up;
        private Vector3 right;
        private Vector3 worldUp;

        private float yaw;
        private float pitch;

        private float _movementSpeed;
        private float mouseSensitivity;

        public float MovementSpeed
        {
            get => _movementSpeed;
            set => _movementSpeed = value;
        }

        public Camera(Vector3 startPosition, Vector3 upDirection, float startYaw = -90.0f, float startPitch = 0.0f) : base()
        {
            Position = startPosition;
            worldUp = upDirection;
            yaw = startYaw;
            pitch = startPitch;

            front = new Vector3(0.0f, 0.0f, -1.0f);
            _movementSpeed = 10f;
            mouseSensitivity = 0.1f;

            UpdateCameraVectors();
        }

        public Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(Position, Position + front, up);
        }

        public float GetZoom()
        {
            return 45.0f; // Fixed zoom value
        }

        public void ProcessKeyboard(CameraMovement direction, float deltaTime)
        {
            Vector3 frontProject = new Vector3(front.X, 0, front.Z).Normalized();
            Vector3 rightProject = new Vector3(right.X, 0, right.Z).Normalized();

            float velocity = _movementSpeed * deltaTime;
            if (direction == CameraMovement.Forward)
                Position += frontProject * velocity;
            if (direction == CameraMovement.Backward)
                Position -= frontProject * velocity;
            if (direction == CameraMovement.Left)
                Position -= rightProject * velocity;
            if (direction == CameraMovement.Right)
                Position += rightProject * velocity;
            if (direction == CameraMovement.Jump)
                if (Position.Y <= CAMERA_FLOOR + 0.1f)
                    SelfDynamic.ApplyForce(new Vector3(0,400,0),0.1f);
        }

        public void ProcessMouseMovement(float xoffset, float yoffset, bool constrainPitch = true)
        {
            xoffset *= mouseSensitivity;
            yoffset *= mouseSensitivity;

            yaw += xoffset;
            pitch += yoffset;

            if (constrainPitch)
            {
                if (pitch > 89.0f)
                    pitch = 89.0f;
                if (pitch < -89.0f)
                    pitch = -89.0f;
            }

            UpdateCameraVectors();
        }

        internal void SetCameraUniforms(Shader shader, Vector2 size)
        {
            Matrix4 view = GetViewMatrix();
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(GetZoom()), size.X / (float)size.Y, 0.1f, 1000.0f);
            shader.SetMatrix4("view", view);
            shader.SetMatrix4("projection", projection);
            // Установим параметры камеры (viewPos — позиция наблюдателя)
            shader.SetVector3("viewPos", Position);
        }

        private void UpdateCameraVectors()
        {
            Vector3 tempFront;
            tempFront.X = MathF.Cos(MathHelper.DegreesToRadians(yaw)) * MathF.Cos(MathHelper.DegreesToRadians(pitch));
            tempFront.Y = MathF.Sin(MathHelper.DegreesToRadians(pitch));
            tempFront.Z = MathF.Sin(MathHelper.DegreesToRadians(yaw)) * MathF.Cos(MathHelper.DegreesToRadians(pitch));
            front = Vector3.Normalize(tempFront);
            right = Vector3.Normalize(Vector3.Cross(front, worldUp));
            up = Vector3.Normalize(Vector3.Cross(right, front));
        }
    }

    public enum CameraMovement
    {
        Forward,
        Backward,
        Left,
        Right,
        Jump
    }
}