using OpenTK.Mathematics;

namespace Computer_Graphics_Programming_Blue_Meteorite
{
    public class TranslationAnimation : AnimationSystem
    {
        public string Name { get; set; }
        public SceneObject TargetObject { get; set; }
        public Vector3 MovementSpeed { get; set; } // Movement speed per second for each axis
        public Vector3 StartPosition { get; set; }
        public Vector3 EndPosition { get; set; }
        private bool isCompleted = false;
        private float progress = 0f;
        private bool isMovingForward = true;

        public bool IsFinished => isCompleted;

        public void Start()
        {
            isCompleted = false;
            progress = 0f;
            isMovingForward = true;
            if (TargetObject != null)
            {
                StartPosition = TargetObject.Position;
            }
        }

        public void Stop()
        {
            isCompleted = true;
        }

        public void Update(float deltaTime)
        {
            if (TargetObject != null && !isCompleted)
            {
                progress += deltaTime;
                float t = MathHelper.Clamp(progress, 0f, 1f);

                // Use smoothstep interpolation for smoother movement
                t = t * t * (3f - 2f * t);

                if (isMovingForward)
                {
                    TargetObject.Position = Vector3.Lerp(StartPosition, EndPosition, t);
                    if (progress >= 1f)
                    {
                        isMovingForward = false;
                        progress = 0f;
                    }
                }
                else
                {
                    TargetObject.Position = Vector3.Lerp(EndPosition, StartPosition, t);
                    if (progress >= 1f)
                    {
                        isMovingForward = true;
                        progress = 0f;
                    }
                }
            }
        }
    }
} 