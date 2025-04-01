using OpenTK.Mathematics;

namespace Computer_Graphics_Programming_Blue_Meteorite
{
    public class RotationAnimation : AnimationSystem
    {
        public string Name { get; set; }
        public SceneObject TargetObject { get; set; }
        public Vector3 RotationSpeed { get; set; } // Градусы в секунду для каждой оси
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
                TargetObject.Rotation += RotationSpeed * deltaTime;
            }
        }
    }
} 