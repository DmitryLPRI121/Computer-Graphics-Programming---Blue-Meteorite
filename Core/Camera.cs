using OpenTK.Mathematics;

namespace Computer_Graphics_Programming_Blue_Meteorite
{ 
    public class Camera : TransformableObject
    {
        private DynamicBody dynamicBody;
        
        // Добавляем свойство для радиуса коллизии камеры
        public float CollisionRadius { get; set; } = 2.65f;

        public float MovementSpeed
        {
            get => dynamicBody.MovementSpeed;
            set => dynamicBody.MovementSpeed = value;
        }

        public Camera(Vector3 startPosition, Vector3 upDirection, float startYaw = -90.0f, float startPitch = 0.0f) : base()
        {
            Position = startPosition;
            dynamicBody = new DynamicBody(this, upDirection, startYaw, startPitch);
        }

        public Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(Position, Position + dynamicBody.Front, dynamicBody.Up);
        }

        public float GetZoom()
        {
            return 45.0f; // Fixed zoom value
        }

        public void ProcessKeyboard(CameraMovement direction, float deltaTime)
        {
            dynamicBody.ProcessKeyboard(direction, deltaTime);
        }

        public void ProcessMouseMovement(float xoffset, float yoffset, bool constrainPitch = true)
        {
            dynamicBody.ProcessMouseMovement(xoffset, yoffset, constrainPitch);
        }

        internal void SetCameraUniforms(Shader shader, Vector2 size)
        {
            Matrix4 view = GetViewMatrix();
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(GetZoom()), size.X / (float)size.Y, 0.1f, 1000.0f);
            shader.SetMatrix4("view", view);
            shader.SetMatrix4("projection", projection);
            shader.SetVector3("viewPos", Position);
        }

        public void Update(float deltaTime)
        {
            dynamicBody.Update(deltaTime);
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