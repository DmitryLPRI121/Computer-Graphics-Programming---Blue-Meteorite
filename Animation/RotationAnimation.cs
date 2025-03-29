using OpenTK.Mathematics;

namespace Computer_Graphics_Programming_Blue_Meteorite
{
    public class RotationAnimation : AnimationSystem
    {
        public string Name { get; set; }
        public SceneObject TargetObject { get; set; }
        public Vector3 RotationSpeed { get; set; } // Degrees per second for each axis
        private bool isCompleted = false;

        public bool IsFinished => isCompleted;

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