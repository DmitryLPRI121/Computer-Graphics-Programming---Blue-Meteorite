using OpenTK.Mathematics;

namespace Computer_Graphics_Programming_Blue_Meteorite
{
    public class TranslationAnimation : AnimationSystem
    {
        public string Name { get; set; }
        public SceneObject TargetObject { get; set; }
        public Vector3 MovementSpeed { get; set; } // Базовая скорость движения в секунду для каждой оси
        public Vector3 EndPosition { get; set; } // Конечная позиция относительно текущей позиции объекта
        public bool UseLocalSpace { get; set; } = true; // Использовать ли локальные координаты объекта
        public Vector3 UpAcceleration { get; set; } = Vector3.Zero; // Ускорение при движении вверх
        public Vector3 DownAcceleration { get; set; } = Vector3.Zero; // Ускорение при движении вниз
        private bool isCompleted = false;
        private float progress = 0f;
        private bool isMovingForward = true;
        private Vector3 startPosition;
        private Vector3 targetPosition;
        private Vector3 currentSpeed; // Текущая скорость движения
        private float totalDistance; // Общее расстояние для движения

        public bool IsFinished => isCompleted;

        public void Start()
        {
            isCompleted = false;
            progress = 0f;
            isMovingForward = true;
            currentSpeed = MovementSpeed;
            
            if (TargetObject != null)
            {
                startPosition = TargetObject.Position;
                
                if (UseLocalSpace)
                {
                    // Преобразуем конечную позицию из локального пространства в мировое
                    Matrix4 rotationMatrix = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(TargetObject.Rotation.X)) *
                                          Matrix4.CreateRotationY(MathHelper.DegreesToRadians(TargetObject.Rotation.Y)) *
                                          Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(TargetObject.Rotation.Z));
                    
                    // Преобразуем смещение из локального пространства в мировое
                    Vector3 worldSpaceOffset = Vector3.TransformVector(EndPosition, rotationMatrix);
                    targetPosition = startPosition + worldSpaceOffset;
                }
                else
                {
                    // Конечная позиция теперь задается относительно текущей позиции
                    targetPosition = startPosition + EndPosition;
                }

                // Вычисляем общее расстояние для движения
                totalDistance = (targetPosition - startPosition).Length;
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
                if (isMovingForward)
                {
                    // Применяем ускорение при движении вверх
                    currentSpeed += UpAcceleration * deltaTime;
                    
                    // Вычисляем смещение на основе текущей скорости
                    Vector3 direction = (targetPosition - startPosition).Normalized();
                    Vector3 displacement = direction * currentSpeed.Length * deltaTime;
                    
                    // Обновляем позицию
                    TargetObject.Position += displacement;
                    
                    // Проверяем, достигли ли мы целевой позиции
                    if ((TargetObject.Position - targetPosition).Length < 0.01f)
                    {
                        isMovingForward = false;
                        currentSpeed = MovementSpeed;
                    }
                }
                else
                {
                    // Применяем ускорение при движении вниз
                    currentSpeed += DownAcceleration * deltaTime;
                    
                    // Вычисляем смещение на основе текущей скорости
                    Vector3 direction = (startPosition - targetPosition).Normalized();
                    Vector3 displacement = direction * currentSpeed.Length * deltaTime;
                    
                    // Обновляем позицию
                    TargetObject.Position += displacement;
                    
                    // Проверяем, достигли ли мы начальной позиции
                    if ((TargetObject.Position - startPosition).Length < 0.01f)
                    {
                        isMovingForward = true;
                        currentSpeed = MovementSpeed;
                    }
                }
            }
        }
    }
} 