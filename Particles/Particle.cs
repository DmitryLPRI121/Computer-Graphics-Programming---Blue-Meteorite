using OpenTK.Mathematics;

namespace Computer_Graphics_Programming_Blue_Meteorite
{
    public class Particle
    {
        public Vector3 Position { get; set; }
        public Vector3 Velocity { get; set; }
        public Vector3 Color { get; set; }
        public float Size { get; set; }
        public float Life { get; set; }
        public float MaxLife { get; set; }
        public float Alpha { get; set; }

        public Particle(Vector3 position, Vector3 velocity, Vector3 color, float size, float life)
        {
            Position = position;
            Velocity = velocity;
            Color = color;
            Size = size;
            Life = life;
            MaxLife = life;
            Alpha = 1.0f;
        }

        public void Update(float deltaTime)
        {
            Position += Velocity * deltaTime;
            Life -= deltaTime;
            Alpha = Life / MaxLife;
        }
    }
} 