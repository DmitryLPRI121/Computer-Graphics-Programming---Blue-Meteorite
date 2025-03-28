using OpenTK.Mathematics;

namespace Computer_Graphics_Programming_Blue_Meteorite
{
    public class RotationAnimation : IAnimation
    {
        public string Name { get; set; }
        public SceneObject TargetObject { get; set; }
        public Vector3 RotationSpeed { get; set; } // Degrees per second for each axis
        public bool isCompleted { get; set; } = false;

        public bool IsCompleted()
        {
            return isCompleted;
        }

        public void Start()
        {
            isCompleted = false;
        }

        public void Stop()
        {
            isCompleted = true;
        }

        public void Update(float deltaTime)
        {
            if (TargetObject != null && !isCompleted)
            {
                // Apply rotation based on speed and delta time
                TargetObject.Rotation += RotationSpeed * deltaTime;
            }
        }
    }
} 