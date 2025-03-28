using OpenTK.Mathematics;

namespace Computer_Graphics_Programming_Blue_Meteorite
{
    public class DynamicBody
    {
        public float floor_y { get; set; } = 0;
        public bool IsGrounded { get; private set; }
        public float GroundCheckDistance { get; set; } = 0.1f;

        private Vector3 _velocity;
        private Vector3 _acceleration;

        public Vector3 Velocity => _velocity;
        public float Mass { get; set; } = 1.0f;
        public float Drag { get; set; } = 0.5f; // Сопротивление воздуха

        public TransformableObject Target { get; set; }

        public DynamicBody(TransformableObject target)
        {
            _velocity = Vector3.Zero;
            _acceleration = Vector3.Zero;
            Target = target;
        }

        public void ApplyGravity(float gravityForce, float deltaTime)
        {
            if (!IsGrounded)
            {
                _acceleration.Y -= gravityForce * Mass;
            }
        }

        public void ApplyForce(Vector3 force, float deltaTime)
        {
            _acceleration += force / Mass;
        }

        public void Update(float deltaTime)
        {
            // Обновляем скорость с учетом сопротивления (только по XZ, чтобы не мешать гравитации)
            Vector3 horizontalVelocity = new Vector3(_velocity.X, 0, _velocity.Z);
            horizontalVelocity *= (1 - Drag * deltaTime);
            _velocity.X = horizontalVelocity.X;
            _velocity.Z = horizontalVelocity.Z;

            // Применяем ускорение
            _velocity += _acceleration * deltaTime;

            // Обновляем позицию
            Target.Position += _velocity * deltaTime;

            // Проверка нахождения на земле
            IsGrounded = Target.Position.Y <= floor_y + GroundCheckDistance;

            // Сброс ускорения
            _acceleration = Vector3.Zero;

            // Фиксируем позицию при достижении пола
            if (Target.Position.Y < floor_y)
            {
                Target.Position = new Vector3(Target.Position.X, floor_y, Target.Position.Z);
                _velocity.Y = 0;
                IsGrounded = true;
            }
        }

        public void Jump(float jumpForce)
        {
            if (IsGrounded)
            {
                _velocity.Y = jumpForce;
                IsGrounded = false;
            }
        }
    }
}