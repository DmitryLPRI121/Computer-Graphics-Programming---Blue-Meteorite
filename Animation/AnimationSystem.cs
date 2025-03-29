namespace Computer_Graphics_Programming_Blue_Meteorite
{
    public interface AnimationSystem
    {
        string Name { get; set; }
        SceneObject TargetObject { get; set; }
        void Start();
        void Update(float deltaTime);
        void Stop();
        bool IsFinished { get; }
    }
}