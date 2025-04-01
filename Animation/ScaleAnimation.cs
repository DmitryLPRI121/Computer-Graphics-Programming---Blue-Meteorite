using OpenTK.Mathematics;

namespace Computer_Graphics_Programming_Blue_Meteorite
{
    public class ScaleAnimation : AnimationSystem
    {
        public string Name { get; set; }
        public SceneObject TargetObject { get; set; }
        public Vector3 ScaleSpeed { get; set; } // Изменение масштаба в секунду для каждой оси
        public Vector3 MinScale { get; set; } = Vector3.One * 0.5f;
        public Vector3 MaxScale { get; set; } = Vector3.One * 2.0f;
        private bool isCompleted = false;
        private bool isGrowing = true;

        public bool IsFinished => isCompleted;

        public void Start()
        {
            isCompleted = false;
            isGrowing = true;
        }

        public void Stop()
        {
            isCompleted = true;
        }

        public void Update(float deltaTime)
        {
            if (TargetObject != null && !isCompleted)
            {
                Vector3 newScale = TargetObject.Scale;
                
                if (isGrowing)
                {
                    newScale += ScaleSpeed * deltaTime;
                    if (newScale.X >= MaxScale.X && newScale.Y >= MaxScale.Y && newScale.Z >= MaxScale.Z)
                    {
                        isGrowing = false;
                    }
                }
                else
                {
                    newScale -= ScaleSpeed * deltaTime;
                    if (newScale.X <= MinScale.X && newScale.Y <= MinScale.Y && newScale.Z <= MinScale.Z)
                    {
                        isGrowing = true;
                    }
                }

                TargetObject.Scale = new Vector3(
                    MathHelper.Clamp(newScale.X, MinScale.X, MaxScale.X),
                    MathHelper.Clamp(newScale.Y, MinScale.Y, MaxScale.Y),
                    MathHelper.Clamp(newScale.Z, MinScale.Z, MaxScale.Z)
                );
            }
        }
    }
} 