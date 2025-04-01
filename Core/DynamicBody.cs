using OpenTK.Mathematics;
using System;

namespace Computer_Graphics_Programming_Blue_Meteorite
{
    public class DynamicBody
    {
        public float floor_y { get; set; } = 0;
        public bool IsGrounded { get; private set; }
        public float GroundCheckDistance { get; set; } = 0.1f;

        // Добавляем объект-опору
        public TransformableObject GroundObject { get; set; } = null;
        public Vector3 LastGroundObjectPosition { get; set; } = Vector3.Zero;
        
        // Добавляем настраиваемые коэффициенты для передачи скорости
        public float HorizontalVelocityTransferFactor { get; set; } = 0.7f;
        public float VerticalVelocityTransferFactor { get; set; } = 0.5f;

        public Vector3 _velocity;
        private Vector3 _acceleration;
        private float _jumpRiseSpeed = 15.0f;
        private float _movementSpeed = 20f;
        private float _mouseSensitivity = 0.1f;

        private Vector3 front;
        private Vector3 up;
        private Vector3 right;
        private Vector3 worldUp;

        // Делаем yaw и pitch публичными свойствами
        private float yaw;
        private float pitch;
        
        // Добавляем публичные свойства для доступа к углам
        public float Yaw => yaw;
        public float Pitch => pitch;

        public Vector3 Velocity => _velocity;
        public float Mass { get; set; } = 1.0f;
        public float Drag { get; set; } = 0.5f;
        public float MovementSpeed
        {
            get => _movementSpeed;
            set => _movementSpeed = value;
        }

        public float JumpRiseSpeed
        {
            get => _jumpRiseSpeed;
            set => _jumpRiseSpeed = value;
        }

        public float MouseSensitivity
        {
            get => _mouseSensitivity;
            set => _mouseSensitivity = value;
        }

        public Vector3 Front => front;
        public Vector3 Up => up;
        public Vector3 Right => right;

        public TransformableObject Target { get; set; }

        public DynamicBody(TransformableObject target, Vector3 upDirection, float startYaw = -90.0f, float startPitch = 0.0f)
        {
            _velocity = Vector3.Zero;
            _acceleration = Vector3.Zero;
            Target = target;
            
            worldUp = upDirection;
            yaw = startYaw;
            pitch = startPitch;

            front = new Vector3(0.0f, 0.0f, -1.0f);
            UpdateVectors();
        }

        public void ApplyImpulse(Vector3 impulse)
        {
            _velocity += impulse / Mass;
        }

        public void ApplyAngularImpulse(Vector3 angularImpulse)
        {
            // Применяем вращательный импульс к объекту
            // Для простоты используем линейную скорость для создания эффекта вращения
            float rotationFactor = 0.5f; // Коэффициент для настройки силы вращения
            _velocity += angularImpulse * rotationFactor;
        }

        public void ProcessKeyboard(CameraMovement direction, float deltaTime)
        {
            Vector3 frontProject = new Vector3(front.X, 0, front.Z).Normalized();
            Vector3 rightProject = new Vector3(right.X, 0, right.Z).Normalized();

            float velocity = _movementSpeed * deltaTime;
            if (direction == CameraMovement.Forward)
                Target.Position += frontProject * velocity;
            if (direction == CameraMovement.Backward)
                Target.Position -= frontProject * velocity;
            if (direction == CameraMovement.Left)
                Target.Position -= rightProject * velocity;
            if (direction == CameraMovement.Right)
                Target.Position += rightProject * velocity;
            if (direction == CameraMovement.Jump)
                if (IsGrounded)
                    Jump(1.0f);
        }

        public void ProcessMouseMovement(float xoffset, float yoffset, bool constrainPitch = true)
        {
            xoffset *= _mouseSensitivity;
            yoffset *= _mouseSensitivity;

            yaw += xoffset;
            pitch += yoffset;

            if (constrainPitch)
            {
                if (pitch > 89.0f)
                    pitch = 89.0f;
                if (pitch < -89.0f)
                    pitch = -89.0f;
            }

            UpdateVectors();
        }

        public void ApplyGravity(float gravityForce, float deltaTime)
        {
            // Применяем гравитацию как изменение ускорения
            if (!IsGrounded)
            {
                // Применяем гравитацию даже если мы движемся вверх, чтобы симулировать реальную физику
                _acceleration.Y -= gravityForce * Mass;
                
                // Убеждаемся, что у нас есть минимальная скорость, чтобы гравитация была заметна
                // Это предотвращает "застревание" объектов в воздухе
                if (Math.Abs(_velocity.Y) < 0.001f)
                {
                    _velocity.Y = -0.01f; // Небольшая начальная скорость вниз
                }
            }
            else
            {
                // Когда объект на земле, обеспечиваем нулевую вертикальную скорость
                _velocity.Y = 0;
            }
        }

        public void ApplyForce(Vector3 force, float deltaTime)
        {
            // Вычисляем величину силы
            float magnitude = force.Length;
            
            // Прямое изменение скорости для немедленной реакции
            if (magnitude > 10.0f) // Для больших сил от пользовательского интерфейса
            {
                // Применяем большее немедленное изменение скорости для сильных сил (от UI)
                Vector3 direction = force.Normalized();
                _velocity = direction * magnitude * 0.1f;
                
                // Также добавляем к ускорению для продолжения движения
                _acceleration += force / Mass;
            }
            else // Для меньших игровых сил
            {
                // Стандартное применение силы для обычного геймплея
                _velocity += force * deltaTime * 0.5f;
                _acceleration += force / Mass;
            }
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

            // Если мы стоим на динамическом объекте, учитываем его движение
            if (IsGrounded && GroundObject != null && GroundObject.SelfDynamic != null)
            {
                // Вычисляем разницу в позиции объекта-опоры
                Vector3 groundMovement = GroundObject.Position - LastGroundObjectPosition;
                
                // Применяем это смещение к нашему объекту (только по XZ, чтобы не было конфликтов по Y)
                Target.Position += new Vector3(groundMovement.X, 0, groundMovement.Z);
                
                // Обновляем последнюю позицию объекта-опоры
                LastGroundObjectPosition = GroundObject.Position;
                
                // Предотвращаем проваливание в объект, на котором стоим
                if (Target is Camera camera)
                {
                    // Вычисляем объект, на котором стоим
                    Vector3 objectToCamera = new Vector3(
                        Target.Position.X - GroundObject.Position.X,
                        0,
                        Target.Position.Z - GroundObject.Position.Z
                    );
                    
                    float horizontalDistance = objectToCamera.Length;
                    
                    if (GroundObject is Sphere sphere)
                    {
                        // Для сфер проверяем, что мы не проваливаемся в нее по горизонтали
                        float sphereRadius = 0.5f * MathF.Max(sphere.Scale.X, MathF.Max(sphere.Scale.Y, sphere.Scale.Z));
                        
                        // Если мы оказались внутри сферы по горизонтали
                        if (horizontalDistance < sphereRadius * 0.95f) // С небольшим допуском
                        {
                            // Выталкиваем персонажа наружу по горизонтали
                            if (horizontalDistance > 0.01f) // Избегаем деления на близкий к нулю вектор
                            {
                                Vector3 outDirection = objectToCamera / horizontalDistance;
                                Target.Position = new Vector3(
                                    sphere.Position.X + outDirection.X * sphereRadius,
                                    Target.Position.Y,
                                    sphere.Position.Z + outDirection.Z * sphereRadius
                                );
                            }
                            else
                            {
                                // Если мы прямо над центром сферы, выталкиваем в случайном направлении
                                float randomAngle = (float)new Random().NextDouble() * MathF.PI * 2;
                                Target.Position = new Vector3(
                                    sphere.Position.X + MathF.Cos(randomAngle) * sphereRadius,
                                    Target.Position.Y,
                                    sphere.Position.Z + MathF.Sin(randomAngle) * sphereRadius
                                );
                            }
                        }
                    }
                    else if (GroundObject is Cube cube)
                    {
                        // Для кубов проверяем, что мы находимся над поверхностью куба
                        Vector3 halfSize = cube.Scale * 0.5f;
                        
                        // Если мы вышли за пределы верхней грани куба
                        bool outsideX = MathF.Abs(objectToCamera.X) > halfSize.X;
                        bool outsideZ = MathF.Abs(objectToCamera.Z) > halfSize.Z;
                        
                        if (outsideX || outsideZ)
                        {
                            // Ограничиваем позицию по XZ, чтобы не выпасть за края куба
                            float clampedX = MathHelper.Clamp(objectToCamera.X, -halfSize.X, halfSize.X);
                            float clampedZ = MathHelper.Clamp(objectToCamera.Z, -halfSize.Z, halfSize.Z);
                            
                            Target.Position = new Vector3(
                                cube.Position.X + clampedX,
                                Target.Position.Y,
                                cube.Position.Z + clampedZ
                            );
                        }
                    }
                    else if (GroundObject is Prism prism)
                    {
                        // Для призм используем улучшенный подход с учетом наклонной верхней грани
                        Vector3 halfSize = prism.Scale * 0.5f;
                        
                        // Нормализованные координаты относительно центра призмы в плоскости XZ
                        float normalizedX = MathF.Abs(objectToCamera.X) / halfSize.X;
                        float normalizedZ = MathF.Abs(objectToCamera.Z) / halfSize.Z;
                        
                        // Проверяем, находится ли персонаж над призмой в плоскости XZ
                        bool insideXZ = normalizedX <= 1.0f && normalizedZ <= 1.0f;
                        
                        if (insideXZ)
                        {
                            // Если мы находимся на наклонной части (верхней грани)
                            if (Target.Position.Y > prism.Position.Y)
                            {
                                // Рассчитываем, насколько персонаж близок к краю наклонной грани
                                float distanceFromCenter = MathF.Max(normalizedX, normalizedZ);
                                
                                // Если мы слишком близко к краю, добавляем небольшую силу к центру
                                // чтобы предотвратить соскальзывание
                                if (distanceFromCenter > 0.8f)
                                {
                                    // Вычисляем направление к центру призмы в плоскости XZ
                                    Vector3 directionToCenter = new Vector3(
                                        prism.Position.X - Target.Position.X,
                                        0,
                                        prism.Position.Z - Target.Position.Z
                                    );
                                    
                                    if (directionToCenter.Length > 0.01f)
                                    {
                                        directionToCenter = Vector3.Normalize(directionToCenter);
                                        
                                        // Сила притяжения к центру увеличивается ближе к краю
                                        float pullFactor = (distanceFromCenter - 0.8f) * 5.0f; // от 0 до 1
                                        
                                        // Применяем небольшое смещение к центру призмы
                                        Target.Position += directionToCenter * pullFactor * 0.01f;
                                    }
                                }
                                
                                // Расчет максимальной высоты наклонной грани в текущей позиции
                                float heightAtCurrentPosition = prism.Position.Y + 
                                    halfSize.Y * (1.0f - MathF.Max(normalizedX, normalizedZ));
                                
                                // Устанавливаем высоту персонажа чуть выше поверхности наклонной грани
                                if (Target.Position.Y < heightAtCurrentPosition + 0.1f)
                                {
                                    Target.Position = new Vector3(
                                        Target.Position.X,
                                        heightAtCurrentPosition + 0.1f,
                                        Target.Position.Z
                                    );
                                }
                            }
                        }
                        else
                        {
                            // Если персонаж вышел за границы призмы в плоскости XZ,
                            // ограничиваем его положение, чтобы он не выпал за край
                            float clampedX = MathHelper.Clamp(objectToCamera.X, -halfSize.X, halfSize.X);
                            float clampedZ = MathHelper.Clamp(objectToCamera.Z, -halfSize.Z, halfSize.Z);
                            
                            Target.Position = new Vector3(
                                prism.Position.X + clampedX,
                                Target.Position.Y,
                                prism.Position.Z + clampedZ
                            );
                        }
                    }
                }
            }

            // Обновляем позицию с учетом скорости (важно для динамических объектов)
            Vector3 velocityChange = _velocity * deltaTime;
            if (velocityChange.Length > 0.001f)
            {
                Target.Position += velocityChange;
            }

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
            else if (!IsGrounded)
            {
                // Если не на земле, сбрасываем объект-опору
                GroundObject = null;
            }
            
            // Sync back to SceneObject if the target is a dynamic object
            // Find the corresponding SceneObject in the SceneObjects list and update its position
            if (Target != null && Target.Name != null)
            {
                // Note: This requires access to the sceneState object which might not be accessible here
                // In a full implementation, we would need to pass a reference to SceneObjects or use an event system
                // For now, we rely on the SceneSettings class to handle this synchronization from TransformableObject to SceneObject
                
                // If the object has been moved by physics, make sure its DynamicBody is in sync
                if (Target.SelfDynamic == this)
                {
                    // Ensure that any object with this DynamicBody has its position in sync
                    Target.SelfDynamic.Target.Position = Target.Position;
                }
            }
        }

        public void Jump(float jumpForce)
        {
            if (IsGrounded)
            {
                // Применяем только скорость взлета
                _velocity.Y = _jumpRiseSpeed;
                
                // Если прыгаем с динамического объекта, добавляем его скорость к нашей
                if (GroundObject != null && GroundObject.SelfDynamic != null)
                {
                    // Учитываем горизонтальную скорость объекта при прыжке
                    Vector3 objectVelocity = GroundObject.SelfDynamic.Velocity;
                    Vector3 horizontalVelocity = new Vector3(objectVelocity.X, 0, objectVelocity.Z);
                    
                    // Добавляем часть горизонтальной скорости объекта к скорости персонажа
                    float transferFactor = HorizontalVelocityTransferFactor;
                    _velocity.X += horizontalVelocity.X * transferFactor;
                    _velocity.Z += horizontalVelocity.Z * transferFactor;
                    
                    // Если объект движется вверх, добавляем его вертикальную скорость
                    if (objectVelocity.Y > 0)
                    {
                        _velocity.Y += objectVelocity.Y * VerticalVelocityTransferFactor;
                    }
                }
                
                IsGrounded = false;
                // Сбрасываем объект-опору при прыжке
                GroundObject = null;
            }
        }

        private void UpdateVectors()
        {
            Vector3 tempFront;
            tempFront.X = MathF.Cos(MathHelper.DegreesToRadians(yaw)) * MathF.Cos(MathHelper.DegreesToRadians(pitch));
            tempFront.Y = MathF.Sin(MathHelper.DegreesToRadians(pitch));
            tempFront.Z = MathF.Sin(MathHelper.DegreesToRadians(yaw)) * MathF.Cos(MathHelper.DegreesToRadians(pitch));
            front = Vector3.Normalize(tempFront);
            right = Vector3.Normalize(Vector3.Cross(front, worldUp));
            up = Vector3.Normalize(Vector3.Cross(right, front));
        }

        // Добавляем метод для восстановления углов поворота
        public void RestoreRotation(float newYaw, float newPitch, bool constrainPitch = true)
        {
            yaw = newYaw;
            pitch = newPitch;
            
            if (constrainPitch)
            {
                if (pitch > 89.0f)
                    pitch = 89.0f;
                if (pitch < -89.0f)
                    pitch = -89.0f;
            }
            
            UpdateVectors();
        }
    }
}