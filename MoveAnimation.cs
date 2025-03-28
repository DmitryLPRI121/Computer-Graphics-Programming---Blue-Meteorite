using OpenTK.Mathematics;

namespace Computer_Graphics_Programming_Blue_Meteorite
{
    public class MoveAnimation : IAnimation
    {
        private double SmoothStopCoef = 1.5f;

        public string Name { get; set; }
        public SceneObject TargetObject { get; set; }
        public Vector3 StartPos { get; set; }
        public Vector3 Destination { get; set; }
        public float MaxSquareSpeed { get; set; } = 1000.0f;

        public float SpeedScale { get; set; } = 0.1f;

        public bool isCompleted { get; set; } = true;

        public bool IsCompleted()
        {
            return isCompleted;
        }

        public void Start() {
            isCompleted = false;
            TargetObject.Position = StartPos;
        }

        public void Stop()
        {
            isCompleted = true;
            
        }

        public void Update(float deltaTime)
        {
            if (TargetObject != null && !isCompleted)
            {
                Vector3 direction = Destination - TargetObject.Position;
                float speed = Math.Min((float)Math.Pow(direction.Length, SmoothStopCoef) * SpeedScale, MaxSquareSpeed);
                TargetObject.Position += direction.Normalized() * speed * deltaTime;

                
                isCompleted = direction.LengthSquared < 1f;
                if (isCompleted) {
                    Start();
                }
            }
        }

    }
}
