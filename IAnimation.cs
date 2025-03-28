namespace Computer_Graphics_Programming_Blue_Meteorite
{
    public interface IAnimation
    {
        string Name { get; set; }
        SceneObject TargetObject { get; set; }

        public void Start();

        public void Update(float deltaTime);

        public void Stop();
        bool IsCompleted();
    }
}