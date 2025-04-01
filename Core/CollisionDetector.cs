using OpenTK.Mathematics;
using System;
using System.Collections.Generic;

namespace Computer_Graphics_Programming_Blue_Meteorite
{
    public class CollisionDetector
    {
        // Класс для хранения информации о столкновении
        public class CollisionInfo
        {
            public TransformableObject ObjectA { get; set; }
            public TransformableObject ObjectB { get; set; }
            public Vector3 ContactPoint { get; set; }
            public Vector3 Normal { get; set; }
            public float PenetrationDepth { get; set; }
        }

        // Проверка коллизии между двумя объектами
        public static bool CheckCollision(TransformableObject a, TransformableObject b, out CollisionInfo collisionInfo)
        {
            collisionInfo = null;

            // Добавим проверку для камеры
            if (a is Camera || b is Camera)
            {
                Camera camera = a is Camera ? a as Camera : b as Camera;
                TransformableObject other = a is Camera ? b : a;
                
                bool result = CheckCameraCollision(camera, other, out collisionInfo);
                
                // Если a и b поменялись местами, нужно переставить их в collisionInfo
                if (b is Camera && result)
                {
                    collisionInfo.ObjectA = b;
                    collisionInfo.ObjectB = a;
                    collisionInfo.Normal = -collisionInfo.Normal;
                }
                
                return result;
            }

            // Для начала проверяем по типам объектов
            if (a is Sphere && b is Sphere)
            {
                return CheckSphereSphereCollision(a as Sphere, b as Sphere, out collisionInfo);
            }
            else if (a is Cube && b is Cube)
            {
                return CheckCubeCubeCollision(a as Cube, b as Cube, out collisionInfo);
            }
            else if ((a is Sphere && b is Cube) || (a is Cube && b is Sphere))
            {
                Sphere sphere = a is Sphere ? a as Sphere : b as Sphere;
                Cube cube = a is Cube ? a as Cube : b as Cube;
                bool result = CheckSphereCubeCollision(sphere, cube, out collisionInfo);
                
                // Если a и b поменялись местами, нужно переставить их в collisionInfo
                if (a is Cube && result)
                {
                    collisionInfo.ObjectA = b;
                    collisionInfo.ObjectB = a;
                    collisionInfo.Normal = -collisionInfo.Normal;
                }
                
                return result;
            }
            else if ((a is Sphere && b is Plane) || (a is Plane && b is Sphere))
            {
                Sphere sphere = a is Sphere ? a as Sphere : b as Sphere;
                Plane plane = a is Plane ? a as Plane : b as Plane;
                bool result = CheckSpherePlaneCollision(sphere, plane, out collisionInfo);
                
                // Если a и b поменялись местами, нужно переставить их в collisionInfo
                if (a is Plane && result)
                {
                    collisionInfo.ObjectA = b;
                    collisionInfo.ObjectB = a;
                    collisionInfo.Normal = -collisionInfo.Normal;
                }
                
                return result;
            }
            else if ((a is Cube && b is Plane) || (a is Plane && b is Cube))
            {
                Cube cube = a is Cube ? a as Cube : b as Cube;
                Plane plane = a is Plane ? a as Plane : b as Plane;
                bool result = CheckCubePlaneCollision(cube, plane, out collisionInfo);
                
                // Если a и b поменялись местами, нужно переставить их в collisionInfo
                if (a is Plane && result)
                {
                    collisionInfo.ObjectA = b;
                    collisionInfo.ObjectB = a;
                    collisionInfo.Normal = -collisionInfo.Normal;
                }
                
                return result;
            }
            else if ((a is Prism && b is Sphere) || (a is Sphere && b is Prism))
            {
                Sphere sphere = a is Sphere ? a as Sphere : b as Sphere;
                Prism prism = a is Prism ? a as Prism : b as Prism;
                bool result = CheckSpherePrismCollision(sphere, prism, out collisionInfo);
                
                // Если a и b поменялись местами, нужно переставить их в collisionInfo
                if (a is Prism && result)
                {
                    collisionInfo.ObjectA = b;
                    collisionInfo.ObjectB = a;
                    collisionInfo.Normal = -collisionInfo.Normal;
                }
                
                return result;
            }

            // По умолчанию коллизии нет
            return false;
        }

        // Проверка столкновения сфера-сфера (самая простая)
        private static bool CheckSphereSphereCollision(Sphere a, Sphere b, out CollisionInfo collisionInfo)
        {
            collisionInfo = new CollisionInfo
            {
                ObjectA = a,
                ObjectB = b
            };

            // Получаем радиусы сфер (масштабированные)
            float radiusA = 0.5f * MathF.Max(a.Scale.X, MathF.Max(a.Scale.Y, a.Scale.Z));
            float radiusB = 0.5f * MathF.Max(b.Scale.X, MathF.Max(b.Scale.Y, b.Scale.Z));
            float radiusSum = radiusA + radiusB;

            // Вычисляем вектор между центрами сфер
            Vector3 direction = b.Position - a.Position;
            float distance = direction.Length;

            // Проверяем, есть ли пересечение
            if (distance < radiusSum)
            {
                // Нормализуем направление
                direction = distance > 0 ? direction / distance : new Vector3(0, 1, 0);

                collisionInfo.Normal = direction;
                collisionInfo.PenetrationDepth = radiusSum - distance;
                collisionInfo.ContactPoint = a.Position + direction * (radiusA - collisionInfo.PenetrationDepth * 0.5f);

                return true;
            }

            return false;
        }

        // Проверка столкновения куб-куб
        private static bool CheckCubeCubeCollision(Cube a, Cube b, out CollisionInfo collisionInfo)
        {
            collisionInfo = new CollisionInfo
            {
                ObjectA = a,
                ObjectB = b
            };

            // Получаем половины размеров каждого куба
            Vector3 halfSizeA = a.Scale * 0.5f;
            Vector3 halfSizeB = b.Scale * 0.5f;

            // Вычисляем вектор между центрами кубов
            Vector3 centerDiff = b.Position - a.Position;

            // Проверяем перекрытие по каждой оси
            float overlapX = halfSizeA.X + halfSizeB.X - MathF.Abs(centerDiff.X);
            float overlapY = halfSizeA.Y + halfSizeB.Y - MathF.Abs(centerDiff.Y);
            float overlapZ = halfSizeA.Z + halfSizeB.Z - MathF.Abs(centerDiff.Z);

            // Если есть положительное перекрытие по всем осям, то есть коллизия
            if (overlapX > 0 && overlapY > 0 && overlapZ > 0)
            {
                // Определяем наименьшее перекрытие и соответствующую ось
                Vector3 normal = Vector3.Zero;
                
                if (overlapX <= overlapY && overlapX <= overlapZ)
                {
                    normal = new Vector3(centerDiff.X > 0 ? 1 : -1, 0, 0);
                    collisionInfo.PenetrationDepth = overlapX;
                }
                else if (overlapY <= overlapX && overlapY <= overlapZ)
                {
                    normal = new Vector3(0, centerDiff.Y > 0 ? 1 : -1, 0);
                    collisionInfo.PenetrationDepth = overlapY;
                }
                else
                {
                    normal = new Vector3(0, 0, centerDiff.Z > 0 ? 1 : -1);
                    collisionInfo.PenetrationDepth = overlapZ;
                }

                collisionInfo.Normal = normal;
                collisionInfo.ContactPoint = a.Position + centerDiff * 0.5f;

                return true;
            }

            return false;
        }

        // Проверка столкновения сфера-куб
        private static bool CheckSphereCubeCollision(Sphere sphere, Cube cube, out CollisionInfo collisionInfo)
        {
            collisionInfo = new CollisionInfo
            {
                ObjectA = sphere,
                ObjectB = cube
            };

            // Получаем радиус сферы и половину размера куба
            float radius = 0.5f * MathF.Max(sphere.Scale.X, MathF.Max(sphere.Scale.Y, sphere.Scale.Z));
            Vector3 halfSize = cube.Scale * 0.5f;

            // Находим ближайшую к сфере точку на кубе
            Vector3 closestPoint = Vector3.Zero;
            Vector3 cubeToSphere = sphere.Position - cube.Position;

            // Ограничиваем по каждой оси
            closestPoint.X = MathHelper.Clamp(cubeToSphere.X, -halfSize.X, halfSize.X);
            closestPoint.Y = MathHelper.Clamp(cubeToSphere.Y, -halfSize.Y, halfSize.Y);
            closestPoint.Z = MathHelper.Clamp(cubeToSphere.Z, -halfSize.Z, halfSize.Z);

            // Конвертируем обратно в мировые координаты
            closestPoint += cube.Position;

            // Проверяем расстояние между ближайшей точкой и центром сферы
            Vector3 direction = closestPoint - sphere.Position;
            float distance = direction.Length;

            if (distance < radius)
            {
                // Нормализуем направление
                direction = distance > 0 ? direction / distance : new Vector3(0, 1, 0);

                // Определяем, с какой гранью куба произошло столкновение
                Vector3 normal = Vector3.Zero;
                float penetrationDepth = radius - distance;

                // Проверяем, с какой гранью произошло столкновение
                if (MathF.Abs(closestPoint.X - cube.Position.X) > MathF.Abs(closestPoint.Y - cube.Position.Y) &&
                    MathF.Abs(closestPoint.X - cube.Position.X) > MathF.Abs(closestPoint.Z - cube.Position.Z))
                {
                    // Столкновение с боковой гранью по X
                    normal = new Vector3(MathF.Sign(closestPoint.X - cube.Position.X), 0, 0);
                }
                else if (MathF.Abs(closestPoint.Y - cube.Position.Y) > MathF.Abs(closestPoint.Z - cube.Position.Z))
                {
                    // Столкновение с верхней/нижней гранью
                    normal = new Vector3(0, MathF.Sign(closestPoint.Y - cube.Position.Y), 0);
                }
                else
                {
                    // Столкновение с боковой гранью по Z
                    normal = new Vector3(0, 0, MathF.Sign(closestPoint.Z - cube.Position.Z));
                }

                collisionInfo.Normal = normal;
                collisionInfo.PenetrationDepth = penetrationDepth;
                collisionInfo.ContactPoint = closestPoint;

                return true;
            }

            return false;
        }

        // Проверка столкновения сфера-плоскость
        private static bool CheckSpherePlaneCollision(Sphere sphere, Plane plane, out CollisionInfo collisionInfo)
        {
            collisionInfo = new CollisionInfo
            {
                ObjectA = sphere,
                ObjectB = plane
            };

            // Получаем радиус сферы
            float radius = 0.5f * MathF.Max(sphere.Scale.X, MathF.Max(sphere.Scale.Y, sphere.Scale.Z));
            
            // Для простоты считаем, что нормаль плоскости всегда (0, 1, 0)
            Vector3 planeNormal = new Vector3(0, 1, 0);

            // Расстояние от центра сферы до плоскости
            float distance = MathF.Abs(sphere.Position.Y - plane.Position.Y);

            if (distance < radius)
            {
                collisionInfo.Normal = planeNormal;
                collisionInfo.PenetrationDepth = radius - distance;
                collisionInfo.ContactPoint = sphere.Position - planeNormal * distance;

                return true;
            }

            return false;
        }

        // Проверка столкновения куб-плоскость
        private static bool CheckCubePlaneCollision(Cube cube, Plane plane, out CollisionInfo collisionInfo)
        {
            collisionInfo = new CollisionInfo
            {
                ObjectA = cube,
                ObjectB = plane
            };

            // Получаем половину высоты куба
            float halfHeight = cube.Scale.Y * 0.5f;
            
            // Для простоты считаем, что нормаль плоскости всегда (0, 1, 0)
            Vector3 planeNormal = new Vector3(0, 1, 0);

            // Нижняя точка куба
            float lowestPointY = cube.Position.Y - halfHeight;
            
            // Расстояние от нижней точки куба до плоскости
            float distance = lowestPointY - plane.Position.Y;

            if (distance < 0)
            {
                collisionInfo.Normal = planeNormal;
                collisionInfo.PenetrationDepth = -distance;
                
                // Точка контакта - проекция центра куба на плоскость
                collisionInfo.ContactPoint = new Vector3(
                    cube.Position.X, 
                    plane.Position.Y, 
                    cube.Position.Z);

                return true;
            }

            return false;
        }

        // Проверка столкновения сфера-призма с более точным определением коллизии
        private static bool CheckSpherePrismCollision(Sphere sphere, Prism prism, out CollisionInfo collisionInfo)
        {
            collisionInfo = new CollisionInfo
            {
                ObjectA = sphere,
                ObjectB = prism
            };

            // Получаем радиус сферы
            float radius = 0.5f * MathF.Max(sphere.Scale.X, MathF.Max(sphere.Scale.Y, sphere.Scale.Z));
            
            // Параметры призмы (прямоугольная основа и треугольные грани)
            Vector3 halfSize = prism.Scale * 0.5f;
            
            // Создаем матрицу вращения призмы
            Matrix4 prismRotation = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(prism.Rotation.X)) *
                                   Matrix4.CreateRotationY(MathHelper.DegreesToRadians(prism.Rotation.Y)) *
                                   Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(prism.Rotation.Z));
            
            // Создаем обратную матрицу вращения для преобразования в локальное пространство призмы
            Matrix4 inversePrismRotation = Matrix4.Invert(prismRotation);
            
            // Вектор от призмы к сфере в мировых координатах
            Vector3 prismToSphere = sphere.Position - prism.Position;
            
            // Преобразуем вектор в локальное пространство призмы
            Vector3 localPrismToSphere = Vector3.TransformVector(prismToSphere, inversePrismRotation);
            
            // Проверка коллизии с основанием призмы (прямоугольная часть)
            
            // Сначала проверим вертикальные стенки призмы
            // Ограничиваем по XZ плоскости (основание призмы)
            Vector3 closestPointXZ = new Vector3(
                MathHelper.Clamp(localPrismToSphere.X, -halfSize.X, halfSize.X),
                0,
                MathHelper.Clamp(localPrismToSphere.Z, -halfSize.Z, halfSize.Z)
            );
            
            // Проверяем, находится ли сфера в пределах прямоугольного основания по XZ
            bool insideRectBase = MathF.Abs(localPrismToSphere.X) <= halfSize.X && 
                                  MathF.Abs(localPrismToSphere.Z) <= halfSize.Z;
            
            // Проверяем коллизию с нижней гранью (прямоугольник)
            if (insideRectBase && localPrismToSphere.Y < 0 && MathF.Abs(localPrismToSphere.Y) < radius + halfSize.Y)
            {
                // Коллизия с нижней гранью
                Vector3 localNormal = new Vector3(0, -1, 0);
                // Преобразуем нормаль обратно в мировые координаты
                collisionInfo.Normal = Vector3.TransformVector(localNormal, prismRotation);
                collisionInfo.PenetrationDepth = radius + halfSize.Y - MathF.Abs(localPrismToSphere.Y);
                
                // Точка контакта в локальных координатах
                Vector3 localContactPoint = new Vector3(
                    localPrismToSphere.X,
                    -halfSize.Y,
                    localPrismToSphere.Z
                );
                // Преобразуем точку контакта в мировые координаты
                collisionInfo.ContactPoint = Vector3.TransformPosition(localContactPoint, prismRotation) + prism.Position;
                return true;
            }
            
            // Проверяем коллизию с верхней наклонной гранью (треугольник)
            if (insideRectBase && localPrismToSphere.Y > 0)
            {
                // Нормаль к верхней треугольной грани (направлена вверх под углом)
                Vector3 localTopNormal = Vector3.Normalize(new Vector3(0, halfSize.Y, 0));
                
                // Расстояние от сферы до верхней грани по нормали
                float distToTopFace = Vector3.Dot(localPrismToSphere, localTopNormal) - halfSize.Y;
                
                // Если сфера достаточно близко к верхней грани
                if (distToTopFace < radius)
                {
                    // Преобразуем нормаль в мировые координаты
                    collisionInfo.Normal = Vector3.TransformVector(localTopNormal, prismRotation);
                    collisionInfo.PenetrationDepth = radius - distToTopFace;
                    
                    // Точка контакта в локальных координатах
                    Vector3 localContactPoint = localPrismToSphere - localTopNormal * distToTopFace;
                    // Преобразуем точку контакта в мировые координаты
                    collisionInfo.ContactPoint = Vector3.TransformPosition(localContactPoint, prismRotation) + prism.Position;
                    return true;
                }
            }
            
            // Проверяем коллизию с боковыми гранями (четыре трапеции)
            if (!insideRectBase || localPrismToSphere.Y > 0)
            {
                // Определяем ближайшую точку на боковой грани
                Vector3 closestPointOnSide = Vector3.Zero;
                Vector3 localSideNormal = Vector3.Zero;
                float minDistance = float.MaxValue;
                
                // Проверяем каждую боковую грань
                for (int side = 0; side < 4; side++)
                {
                    Vector3 localEdgeNormal;
                    float edgeHalfSize;
                    
                    // Выбираем параметры в зависимости от номера грани
                    if (side == 0 || side == 2) // X-грани
                    {
                        localEdgeNormal = new Vector3(side == 0 ? -1 : 1, 0, 0);
                        edgeHalfSize = halfSize.X;
                    }
                    else // Z-грани
                    {
                        localEdgeNormal = new Vector3(0, 0, side == 1 ? 1 : -1);
                        edgeHalfSize = halfSize.Z;
                    }
                    
                    // Расстояние от сферы до грани по нормали
                    float distToEdge;
                    
                    if (side == 0) // -X грань
                        distToEdge = -localPrismToSphere.X - halfSize.X;
                    else if (side == 1) // +Z грань
                        distToEdge = localPrismToSphere.Z - halfSize.Z;
                    else if (side == 2) // +X грань
                        distToEdge = localPrismToSphere.X - halfSize.X;
                    else // -Z грань
                        distToEdge = -localPrismToSphere.Z - halfSize.Z;
                    
                    // Если сфера потенциально пересекает эту грань
                    if (distToEdge < radius)
                    {
                        // Проверяем, не выходит ли сфера за пределы этой грани в другом направлении
                        bool withinEdgeBounds = false;
                        
                        if (side == 0 || side == 2) // X-грани, проверяем пределы по Z
                            withinEdgeBounds = MathF.Abs(localPrismToSphere.Z) < halfSize.Z;
                        else // Z-грани, проверяем пределы по X
                            withinEdgeBounds = MathF.Abs(localPrismToSphere.X) < halfSize.X;
                        
                        // Также проверяем, находится ли сфера в пределах высоты боковой грани
                        withinEdgeBounds = withinEdgeBounds && localPrismToSphere.Y >= -halfSize.Y && 
                                          localPrismToSphere.Y <= halfSize.Y;
                        
                        if (withinEdgeBounds && MathF.Abs(distToEdge) < minDistance)
                        {
                            minDistance = MathF.Abs(distToEdge);
                            localSideNormal = localEdgeNormal;
                            
                            // Ближайшая точка на боковой грани
                            closestPointOnSide = Vector3.Zero;
                            
                            if (side == 0) // -X грань
                                closestPointOnSide.X = -halfSize.X;
                            else if (side == 1) // +Z грань
                                closestPointOnSide.Z = halfSize.Z;
                            else if (side == 2) // +X грань
                                closestPointOnSide.X = halfSize.X;
                            else // -Z грань
                                closestPointOnSide.Z = -halfSize.Z;
                            
                            if (side == 0 || side == 2) // X-грани
                                closestPointOnSide.Z = localPrismToSphere.Z;
                            else // Z-грани
                                closestPointOnSide.X = localPrismToSphere.X;
                            
                            closestPointOnSide.Y = MathHelper.Clamp(localPrismToSphere.Y, 
                                                                 -halfSize.Y,
                                                                 halfSize.Y);
                        }
                    }
                }
                
                // Если нашли ближайшую боковую грань
                if (minDistance < float.MaxValue && minDistance < radius)
                {
                    // Преобразуем нормаль в мировые координаты
                    collisionInfo.Normal = Vector3.TransformVector(-localSideNormal, prismRotation);
                    collisionInfo.PenetrationDepth = radius - minDistance;
                    
                    // Преобразуем точку контакта в мировые координаты
                    collisionInfo.ContactPoint = Vector3.TransformPosition(closestPointOnSide, prismRotation) + prism.Position;
                    return true;
                }
            }
            
            // Если все предыдущие проверки не дали результат,
            // используем упрощенный подход с bounding box
            Vector3 localClosestPointBB = Vector3.Zero;
            
            // Ограничиваем по каждой оси
            localClosestPointBB.X = MathHelper.Clamp(localPrismToSphere.X, -halfSize.X, halfSize.X);
            localClosestPointBB.Y = MathHelper.Clamp(localPrismToSphere.Y, -halfSize.Y, halfSize.Y);
            localClosestPointBB.Z = MathHelper.Clamp(localPrismToSphere.Z, -halfSize.Z, halfSize.Z);
            
            // Преобразуем точку в мировые координаты
            Vector3 closestPointBB = Vector3.TransformPosition(localClosestPointBB, prismRotation) + prism.Position;
            
            // Проверяем расстояние между ближайшей точкой и центром сферы
            Vector3 direction = closestPointBB - sphere.Position;
            float distance = direction.Length;
            
            if (distance < radius)
            {
                // Нормализуем направление
                direction = distance > 0 ? direction / distance : new Vector3(0, 1, 0);
                
                collisionInfo.Normal = -direction; // От призмы к сфере
                collisionInfo.PenetrationDepth = radius - distance;
                collisionInfo.ContactPoint = closestPointBB;
                
                return true;
            }
            
            return false;
        }

        // Проверка коллизий для всех объектов сцены
        public static List<CollisionInfo> DetectAllCollisions(List<TransformableObject> objects)
        {
            List<CollisionInfo> collisions = new List<CollisionInfo>();

            // Перебираем все пары объектов
            for (int i = 0; i < objects.Count; i++)
            {
                for (int j = i + 1; j < objects.Count; j++)
                {
                    CollisionInfo collision;
                    if (CheckCollision(objects[i], objects[j], out collision))
                    {
                        collisions.Add(collision);
                    }
                }
            }

            return collisions;
        }
        
        // Разрешение коллизий с возвратом объектов
        public static void ResolveCollisions(List<CollisionInfo> collisions)
        {
            foreach (var collision in collisions)
            {
                ResolveCollision(collision);
            }
        }

        // Разрешение коллизии для конкретной пары объектов
        private static void ResolveCollision(CollisionInfo collision)
        {
            var dynamicObject = collision.ObjectA;
            var staticObject = collision.ObjectB;

            // Проверяем, является ли первый объект динамическим
            if (dynamicObject.SelfDynamic == null)
            {
                // Если первый объект статический, меняем объекты местами
                dynamicObject = collision.ObjectB;
                staticObject = collision.ObjectA;
                collision.Normal = -collision.Normal;
            }

            var dynamicBody = dynamicObject.SelfDynamic;
            
            // Специальная обработка для сферы с кубом
            if (dynamicObject is Sphere && (staticObject is Cube || staticObject is Sphere))
            {
                // Вычисляем нормаль от статического к динамическому объекту
                Vector3 normal = -collision.Normal; // От статического к динамическому
                
                // Получаем текущую скорость
                Vector3 velocity = dynamicBody.Velocity;
                
                // Проекция скорости на нормаль
                float velocityProjection = Vector3.Dot(velocity, normal);
                
                // Если объект движется в направлении от статического объекта, игнорируем коллизию
                if (velocityProjection > 0)
                    return;
                
                // Коэффициент восстановления (упругости)
                float restitution = 0.5f; // Уменьшаем упругость для более реалистичного поведения
                
                // Импульс = проекция скорости на нормаль с учетом коэффициента восстановления
                Vector3 impulse = -(1 + restitution) * velocityProjection * (-normal);
                
                // Добавляем небольшой буфер для предотвращения застревания
                float bufferFactor = 1.05f;
                dynamicObject.Position += normal * collision.PenetrationDepth * bufferFactor;
                
                // Изменяем скорость в динамическом теле
                dynamicBody.ApplyImpulse(impulse);
                
                return;
            }

            // Обрабатываем только если у обоих объектов есть физика
            if (collision.ObjectA.SelfDynamic == null || collision.ObjectB.SelfDynamic == null)
            {
                // Если один статичный, а другой динамический
                if (collision.ObjectA.SelfDynamic != null)
                {
                    ResolveDynamicWithStatic(collision.ObjectA, collision.ObjectB, collision);
                }
                else if (collision.ObjectB.SelfDynamic != null)
                {
                    ResolveDynamicWithStatic(collision.ObjectB, collision.ObjectA, 
                        new CollisionInfo {
                            ObjectA = collision.ObjectB,
                            ObjectB = collision.ObjectA,
                            ContactPoint = collision.ContactPoint,
                            Normal = -collision.Normal,
                            PenetrationDepth = collision.PenetrationDepth
                        });
                }
                return;
            }

            // Если оба объекта динамические, разрешаем коллизию между ними
            ResolveDynamicWithDynamic(collision);
        }

        // Разрешение коллизии между динамическим и статическим объектами
        private static void ResolveDynamicWithStatic(TransformableObject dynamicObject, TransformableObject staticObject, CollisionInfo collision)
        {
            var dynamicBody = dynamicObject.SelfDynamic;
            
            // Специальная обработка для персонажа (камеры)
            if (dynamicObject is Camera camera)
            {
                // Вычисляем нормаль от статического к динамическому объекту
                Vector3 normal = -collision.Normal; // От статического к динамическому
                
                if (staticObject is Plane)
                {
                    // Персонаж на плоскости, устанавливаем его "приземленным"
                    camera.Position = new Vector3(
                        camera.Position.X,
                        staticObject.Position.Y + camera.CollisionRadius, // Позиция плоскости + радиус коллизии
                        camera.Position.Z
                    );
                    
                    // Обнуляем вертикальную скорость
                    if (dynamicBody._velocity.Y < 0)
                    {
                        dynamicBody._velocity.Y = 0;
                    }
                    
                    // Устанавливаем опорную плоскость для проверки IsGrounded
                    dynamicBody.floor_y = staticObject.Position.Y;
                    
                    return;
                }
                else if (staticObject is Cube || staticObject is Prism || staticObject is Sphere)
                {
                    // Проверяем, стоит ли персонаж на объекте (нормаль направлена вверх)
                    bool isStandingOn = normal.Y > 0.7f; // Нормаль направлена вверх (с запасом)
                    
                    if (isStandingOn)
                    {
                        // Определяем высоту, на которой должен стоять персонаж
                        float objectTop;
                        
                        if (staticObject is Sphere sphere)
                        {
                            float sphereRadius = 0.5f * MathF.Max(sphere.Scale.X, MathF.Max(sphere.Scale.Y, sphere.Scale.Z));
                            Vector3 cameraToSphere = sphere.Position - camera.Position;
                            cameraToSphere.Y = 0; // Убираем вертикальную составляющую
                            float horizontalDistance = cameraToSphere.Length;
                            
                            // Вычисляем высоту на сфере с учетом расстояния от центра
                            if (horizontalDistance < sphereRadius)
                            {
                                // Используем пифагорову теорему для нахождения высоты точки на сфере
                                float heightOffset = MathF.Sqrt(sphereRadius * sphereRadius - horizontalDistance * horizontalDistance);
                                objectTop = sphere.Position.Y + heightOffset;
                            }
                            else
                            {
                                // Если мы за пределами сферы по горизонтали, используем стандартную логику
                                objectTop = sphere.Position.Y + sphereRadius;
                            }
                        }
                        else
                        {
                            // Для куба и призмы просто берем верхнюю грань
                            objectTop = staticObject.Position.Y + staticObject.Scale.Y * 0.5f;
                        }
                        
                        // Устанавливаем позицию камеры над объектом
                        camera.Position = new Vector3(
                            camera.Position.X,
                            objectTop + camera.CollisionRadius,
                            camera.Position.Z
                        );
                        
                        // Обнуляем вертикальную скорость, если она направлена вниз
                        if (dynamicBody._velocity.Y < 0)
                        {
                            dynamicBody._velocity.Y = 0;
                        }
                        
                        // Устанавливаем опорный объект для проверки IsGrounded
                        dynamicBody.floor_y = objectTop;
                        
                        return;
                    }
                }
                
                // Если это не случай стояния на объекте, отталкиваемся от него
                // с дополнительным буфером, чтобы избежать застревания
                float bufferDistance = 0.05f;
                camera.Position += normal * (collision.PenetrationDepth + bufferDistance);
                
                // Отражаем скорость от поверхности объекта
                float restitution = 0.3f; // Коэффициент восстановления (упругости)
                Vector3 velocity = dynamicBody.Velocity;
                
                // Проекция скорости на нормаль
                float velocityProjection = Vector3.Dot(velocity, -normal);
                
                // Отражаем только если объект движется в направлении статического
                if (velocityProjection > 0)
                {
                    // Импульс = проекция скорости на нормаль с учетом коэффициента восстановления
                    Vector3 impulse = -(1 + restitution) * velocityProjection * (-normal);
                    
                    // Изменяем скорость в динамическом теле
                    dynamicBody.ApplyImpulse(impulse);
                }
                
                return;
            }
            
            // Проверка специального случая для куба на плоскости
            if (dynamicObject is Cube && staticObject is Plane)
            {
                // Устанавливаем позицию так, чтобы нижняя грань куба была точно на плоскости
                float halfHeight = dynamicObject.Scale.Y * 0.5f;
                dynamicObject.Position = new Vector3(
                    dynamicObject.Position.X,
                    staticObject.Position.Y + halfHeight, // Позиция плоскости + половина высоты куба
                    dynamicObject.Position.Z
                );
                
                // Обнуляем вертикальную скорость
                if (dynamicBody._velocity.Y < 0)
                {
                    dynamicBody._velocity.Y = 0;
                }
                
                return;
            }
            // Проверка специального случая для сферы на плоскости
            else if (dynamicObject is Sphere && staticObject is Plane)
            {
                // Устанавливаем позицию так, чтобы нижняя точка сферы была точно на плоскости
                float radius = 0.5f * MathF.Max(dynamicObject.Scale.X, MathF.Max(dynamicObject.Scale.Y, dynamicObject.Scale.Z));
                dynamicObject.Position = new Vector3(
                    dynamicObject.Position.X,
                    staticObject.Position.Y + radius, // Позиция плоскости + радиус сферы
                    dynamicObject.Position.Z
                );
                
                // Обнуляем вертикальную скорость или отражаем ее с затуханием
                if (dynamicBody._velocity.Y < 0)
                {
                    // Отражаем с коэффициентом затухания
                    float sphereRestitution = 0.5f; // Коэффициент восстановления для сферы (можно настроить)
                    dynamicBody._velocity.Y = -dynamicBody._velocity.Y * sphereRestitution;
                    
                    // Если скорость очень маленькая, просто останавливаем
                    if (MathF.Abs(dynamicBody._velocity.Y) < 0.1f)
                    {
                        dynamicBody._velocity.Y = 0;
                    }
                }
                
                return;
            }
            
            // Общий случай - корректируем позицию с дополнительным буфером
            float bufferFactor = 1.05f; // Немного больший буфер, чтобы избежать застревания
            dynamicObject.Position += collision.Normal * collision.PenetrationDepth * bufferFactor;
            
            // Отражаем скорость
            float generalRestitution = 0.3f; // Коэффициент восстановления (упругости)
            Vector3 generalVelocity = dynamicBody.Velocity;
            
            // Проекция скорости на нормаль
            float generalVelocityProjection = Vector3.Dot(generalVelocity, collision.Normal);
            
            // Отражаем только если объект движется в направлении статического
            if (generalVelocityProjection < 0)
            {
                // Импульс = проекция скорости на нормаль с учетом коэффициента восстановления
                Vector3 impulse = -(1 + generalRestitution) * generalVelocityProjection * collision.Normal;
                
                // Изменяем скорость в динамическом теле
                dynamicBody.ApplyImpulse(impulse);
            }
        }

        // Разрешение коллизии между двумя динамическими объектами
        private static void ResolveDynamicWithDynamic(CollisionInfo collision)
        {
            var bodyA = collision.ObjectA.SelfDynamic;
            var bodyB = collision.ObjectB.SelfDynamic;
            
            // Специальная обработка для камеры
            if (collision.ObjectA is Camera || collision.ObjectB is Camera)
            {
                // Определяем, кто есть камера, а кто другой объект
                bool aIsCamera = collision.ObjectA is Camera;
                DynamicBody cameraBody = aIsCamera ? bodyA : bodyB;
                DynamicBody otherBody = aIsCamera ? bodyB : bodyA;
                TransformableObject camera = aIsCamera ? collision.ObjectA : collision.ObjectB;
                TransformableObject other = aIsCamera ? collision.ObjectB : collision.ObjectA;
                
                // Приводим к типу Camera для доступа к его специфическим свойствам
                Camera cameraObj = camera as Camera;
                
                // Нормаль всегда от камеры к объекту
                Vector3 normal = aIsCamera ? collision.Normal : -collision.Normal;
                
                // Единая обработка коллизий для всех типов объектов
                Vector3 pushDirection;
                float pushDistance;
                
                if (other is Sphere sphere)
                {
                    // Для сфер используем простое направление от центра
                    float sphereRadius = 0.5f * MathF.Max(sphere.Scale.X, MathF.Max(sphere.Scale.Y, sphere.Scale.Z));
                    
                    // Вычисляем вектор направления от сферы к камере
                    Vector3 sphereToCamera = camera.Position - sphere.Position;
                    float distance = sphereToCamera.Length;
                    
                    // Нормализуем направление
                    pushDirection = distance > 0 ? sphereToCamera / distance : new Vector3(0, 1, 0);
                    
                    // Вычисляем минимальное необходимое расстояние
                    float minDistance = sphereRadius + cameraObj.CollisionRadius;
                    pushDistance = minDistance + 0.05f; // Дополнительный буфер
                }
                else if (other is Cube cube)
                {
                    // Для кубов находим ближайшую точку на поверхности или направление выталкивания
                    Vector3 halfSize = cube.Scale * 0.5f;
                    Vector3 cubeToCamera = camera.Position - cube.Position;
                    
                    // Проверяем, находится ли камера внутри куба
                    bool insideX = MathF.Abs(cubeToCamera.X) < halfSize.X;
                    bool insideY = MathF.Abs(cubeToCamera.Y) < halfSize.Y;
                    bool insideZ = MathF.Abs(cubeToCamera.Z) < halfSize.Z;
                    
                    if (insideX && insideY && insideZ)
                    {
                        // Если камера внутри куба, находим ближайшую грань для выхода
                        float distToXFace = MathF.Min(halfSize.X - MathF.Abs(cubeToCamera.X), halfSize.X + MathF.Abs(cubeToCamera.X));
                        float distToYFace = MathF.Min(halfSize.Y - MathF.Abs(cubeToCamera.Y), halfSize.Y + MathF.Abs(cubeToCamera.Y));
                        float distToZFace = MathF.Min(halfSize.Z - MathF.Abs(cubeToCamera.Z), halfSize.Z + MathF.Abs(cubeToCamera.Z));
                        
                        if (distToXFace <= distToYFace && distToXFace <= distToZFace)
                        {
                            pushDirection = new Vector3(cubeToCamera.X > 0 ? 1 : -1, 0, 0);
                            pushDistance = halfSize.X + cameraObj.CollisionRadius + 0.05f;
                        }
                        else if (distToYFace <= distToXFace && distToYFace <= distToZFace)
                        {
                            pushDirection = new Vector3(0, cubeToCamera.Y > 0 ? 1 : -1, 0);
                            pushDistance = halfSize.Y + cameraObj.CollisionRadius + 0.05f;
                        }
                        else
                        {
                            pushDirection = new Vector3(0, 0, cubeToCamera.Z > 0 ? 1 : -1);
                            pushDistance = halfSize.Z + cameraObj.CollisionRadius + 0.05f;
                        }
                    }
                    else
                    {
                        // Если камера снаружи, используем нормаль коллизии
                        pushDirection = normal;
                        pushDistance = collision.PenetrationDepth + 0.05f;
                    }
                }
                else if (other is Prism prism)
                {
                    // Для призм используем упрощенный подход похожий на кубы
                    Vector3 halfSize = prism.Scale * 0.5f;
                    
                    // Создаем матрицу вращения призмы
                    Matrix4 prismRotation = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(prism.Rotation.X)) *
                                           Matrix4.CreateRotationY(MathHelper.DegreesToRadians(prism.Rotation.Y)) *
                                           Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(prism.Rotation.Z));
                    
                    // Создаем обратную матрицу вращения для преобразования в локальное пространство призмы
                    Matrix4 inversePrismRotation = Matrix4.Invert(prismRotation);
                    
                    // Вектор от призмы к камере в мировых координатах
                    Vector3 prismToCamera = camera.Position - prism.Position;
                    
                    // Преобразуем вектор в локальное пространство призмы
                    Vector3 localPrismToCamera = Vector3.TransformVector(prismToCamera, inversePrismRotation);
                    
                    // Проверяем, находится ли камера внутри призмы (упрощенно через bounding box)
                    bool insideX = MathF.Abs(localPrismToCamera.X) < halfSize.X;
                    bool insideY = MathF.Abs(localPrismToCamera.Y) < halfSize.Y;
                    bool insideZ = MathF.Abs(localPrismToCamera.Z) < halfSize.Z;
                    
                    // Проверка на нахождение внутри нижней части призмы (прямоугольная часть)
                    bool insideRectBase = insideX && insideZ && localPrismToCamera.Y < 0 && localPrismToCamera.Y > -halfSize.Y;
                    
                    // Проверка на нахождение внутри верхней части призмы (треугольная часть)
                    bool insideTopPart = insideX && insideZ && localPrismToCamera.Y >= 0 && localPrismToCamera.Y <= halfSize.Y;
                    
                    // Расчет максимальной высоты в точке расположения камеры (для верхней треугольной части)
                    float maxYAtPosition = 0;
                    if (insideTopPart)
                    {
                        // Для простоты считаем, что верхняя часть имеет форму пирамиды
                        float normalizedX = MathF.Abs(localPrismToCamera.X) / halfSize.X;
                        float normalizedZ = MathF.Abs(localPrismToCamera.Z) / halfSize.Z;
                        
                        // Линейная интерполяция высоты в зависимости от расстояния от центра
                        maxYAtPosition = halfSize.Y * (1.0f - MathF.Max(normalizedX, normalizedZ));
                    }
                    
                    // Определяем, находится ли камера внутри призмы
                    bool insidePrism = insideRectBase || (insideTopPart && localPrismToCamera.Y < maxYAtPosition);
                    
                    if (insidePrism)
                    {
                        // Камера внутри призмы, находим ближайшую грань для выхода
                        float distToXFace = halfSize.X - MathF.Abs(localPrismToCamera.X);
                        float distToZFace = halfSize.Z - MathF.Abs(localPrismToCamera.Z);
                        float distToBottomFace = halfSize.Y + localPrismToCamera.Y; // Расстояние до нижней грани
                        
                        // Расстояние до верхней наклонной грани
                        float distToTopFace = float.MaxValue;
                        Vector3 topNormal = Vector3.Zero;
                        
                        // Определяем радиус коллизии камеры один раз для всего метода
                        float collisionRadius = (camera as Camera)?.CollisionRadius ?? 0.5f;
                        
                        if (localPrismToCamera.Y >= 0)
                        {
                            // Для простоты: верхняя грань имеет нормаль с компонентами X и Z, 
                            // направленными от центра к краям основания пирамиды
                            float normalizedX = localPrismToCamera.X / halfSize.X;
                            float normalizedZ = localPrismToCamera.Z / halfSize.Z;
                            
                            // Создаем нормаль к наклонной грани в точке над камерой
                            topNormal = Vector3.Normalize(new Vector3(normalizedX, 0.5f, normalizedZ));
                            
                            // Расстояние до верхней грани (приблизительное)
                            float currentMaxY = halfSize.Y * (1.0f - MathF.Max(normalizedX, normalizedZ));
                            distToTopFace = currentMaxY - localPrismToCamera.Y;
                        }
                        
                        Vector3 localNormal;
                        float penetrationDepth;
                        Vector3 localContactPoint;
                        
                        // Определяем ближайшую грань
                        if (distToXFace <= distToZFace && distToXFace <= distToBottomFace && distToXFace <= distToTopFace)
                        {
                            // Ближайшая X-грань
                            localNormal = new Vector3(localPrismToCamera.X > 0 ? 1 : -1, 0, 0);
                            penetrationDepth = distToXFace + collisionRadius;
                            localContactPoint = new Vector3(
                                localPrismToCamera.X > 0 ? halfSize.X : -halfSize.X,
                                localPrismToCamera.Y,
                                localPrismToCamera.Z
                            );
                        }
                        else if (distToZFace <= distToXFace && distToZFace <= distToBottomFace && distToZFace <= distToTopFace)
                        {
                            // Ближайшая Z-грань
                            localNormal = new Vector3(0, 0, localPrismToCamera.Z > 0 ? 1 : -1);
                            penetrationDepth = distToZFace + collisionRadius;
                            localContactPoint = new Vector3(
                                localPrismToCamera.X,
                                localPrismToCamera.Y,
                                localPrismToCamera.Z > 0 ? halfSize.Z : -halfSize.Z
                            );
                        }
                        else if (distToBottomFace <= distToXFace && distToBottomFace <= distToZFace && distToBottomFace <= distToTopFace)
                        {
                            // Ближайшая нижняя грань
                            localNormal = new Vector3(0, -1, 0);
                            penetrationDepth = distToBottomFace + collisionRadius;
                            localContactPoint = new Vector3(
                                localPrismToCamera.X,
                                -halfSize.Y,
                                localPrismToCamera.Z
                            );
                        }
                        else
                        {
                            // Ближайшая верхняя наклонная грань
                            localNormal = topNormal;
                            float currentMaxY = halfSize.Y * (1.0f - MathF.Max(
                                MathF.Abs(localPrismToCamera.X) / halfSize.X,
                                MathF.Abs(localPrismToCamera.Z) / halfSize.Z
                            ));
                            penetrationDepth = distToTopFace + collisionRadius;
                            localContactPoint = new Vector3(
                                localPrismToCamera.X,
                                currentMaxY,
                                localPrismToCamera.Z
                            );
                        }
                        
                        // Преобразуем нормаль и точку контакта в мировые координаты
                        Vector3 worldNormal = Vector3.TransformVector(localNormal, prismRotation);
                        Vector3 worldContactPoint = Vector3.TransformPosition(localContactPoint, prismRotation) + prism.Position;
                        
                        // Применяем результаты к объекту collision
                        collision.Normal = worldNormal;
                        collision.PenetrationDepth = penetrationDepth;
                        collision.ContactPoint = worldContactPoint;
                        
                        return;
                    }
                }
                else
                {
                    // Для других типов объектов используем обычную нормаль
                    pushDirection = normal;
                    pushDistance = collision.PenetrationDepth + 0.05f;
                }
                
                // Проверка, стоит ли персонаж на объекте (нормаль направлена вниз)
                Vector3 collisionNormal = collision.Normal;
                float collisionPenetration = collision.PenetrationDepth;
                
                if (other is Sphere)
                {
                    // Для сфер не позволяем персонажу становиться на них
                    // Вместо этого усиливаем эффект толкания
                    float sphereRadius = 0.5f * MathF.Max(other.Scale.X, MathF.Max(other.Scale.Y, other.Scale.Z));
                    float effectiveSphereRadius = sphereRadius * 1.2f;
                    
                    // Вычисляем вектор от центра сферы к персонажу
                    Vector3 sphereToCamera = camera.Position - other.Position;
                    float distance = sphereToCamera.Length;
                    
                    // Нормализуем направление
                    Vector3 spherePushDirection = distance > 0 ? sphereToCamera / distance : new Vector3(0, 1, 0);
                    
                    // Устанавливаем позицию персонажа на безопасном расстоянии от сферы
                    camera.Position = other.Position + spherePushDirection * (effectiveSphereRadius + cameraObj.CollisionRadius + 0.05f);
                    
                    // Если персонаж двигался в сторону сферы, применяем усиленный импульс
                    float sphereImpactVelocity = Vector3.Dot(cameraBody._velocity, -spherePushDirection);
                    if (sphereImpactVelocity > 0)
                    {
                        // Увеличиваем силу удара
                        float kickForce = sphereImpactVelocity * 4.0f; // Увеличенный множитель силы удара
                        
                        // Добавляем дополнительный импульс в направлении движения персонажа
                        Vector3 kickDirection = cameraBody._velocity;
                        if (kickDirection.Length > 0)
                        {
                            kickDirection = kickDirection / kickDirection.Length;
                        }
                        Vector3 kickImpulse = kickDirection * kickForce;
                        
                        // Добавляем вертикальный компонент к импульсу, чтобы сфера подпрыгивала
                        kickImpulse += new Vector3(0, kickForce * 0.5f, 0);
                        
                        // Применяем импульс к сфере
                        otherBody.ApplyImpulse(kickImpulse);
                        
                        // Добавляем более сильный случайный вращательный момент
                        float randomTorque = (float)new Random().NextDouble() * 1.0f - 0.5f;
                        Vector3 randomAxis = new Vector3(
                            (float)new Random().NextDouble() * 2 - 1,
                            (float)new Random().NextDouble() * 2 - 1,
                            (float)new Random().NextDouble() * 2 - 1
                        );
                        if (randomAxis.Length > 0)
                        {
                            randomAxis = randomAxis / randomAxis.Length;
                        }
                        otherBody.ApplyAngularImpulse(randomAxis * randomTorque);
                        
                        // Отталкиваем персонажа в противоположном направлении
                        cameraBody._velocity -= spherePushDirection * sphereImpactVelocity * 0.5f;
                    }
                    
                    return;
                }
                
                if (collisionNormal.Y < -0.7f) // Если нормаль направлена вниз (с запасом)
                {
                    // Персонаж стоит на объекте, корректируем его позицию вверх
                    // и делаем его "приземленным" в этом случае
                    camera.Position = new Vector3(
                        camera.Position.X,
                        other.Position.Y + other.Scale.Y * 0.5f + cameraObj.CollisionRadius, // Стоять на верхней грани объекта
                        camera.Position.Z
                    );
                    
                    // Устанавливаем скорость по Y в 0, чтобы избежать проваливания
                    cameraBody._velocity.Y = 0;
                    
                    // Вместо прямой установки IsGrounded, обновляем floor_y, 
                    // это позволит DynamicBody самостоятельно определить IsGrounded
                    cameraBody.floor_y = other.Position.Y + other.Scale.Y * 0.5f;
                    
                    // Устанавливаем объект как опору для камеры и запоминаем его текущую позицию
                    cameraBody.GroundObject = other;
                    cameraBody.LastGroundObjectPosition = other.Position;
                    
                    // Применяем трение по X и Z к объекту под ногами
                    float frictionFactor = 0.8f; // Коэффициент трения
                    Vector3 horizontalVelocity = new Vector3(cameraBody._velocity.X, 0, cameraBody._velocity.Z);
                    otherBody.ApplyImpulse(horizontalVelocity * frictionFactor);
                    
                    return;
                }
                
                // Если не стоим на объекте, выталкиваем камеру от объекта
                camera.Position = other.Position + collisionNormal * (collisionPenetration + 0.05f);
                
                // Если персонаж двигался в сторону объекта, гасим его скорость в этом направлении
                float velocityTowardsObject = Vector3.Dot(cameraBody._velocity, -collisionNormal);
                if (velocityTowardsObject > 0)
                {
                    // Применяем импульс к сфере при столкновении с персонажем
                    if (other is Sphere)
                    {
                        // Вычисляем силу удара на основе скорости персонажа
                        float kickForce = velocityTowardsObject * 2.0f; // Множитель силы удара
                        
                        // Добавляем дополнительный импульс в направлении движения персонажа
                        Vector3 kickDirection = cameraBody._velocity;
                        if (kickDirection.Length > 0)
                        {
                            kickDirection = kickDirection / kickDirection.Length;
                        }
                        Vector3 kickImpulse = kickDirection * kickForce;
                        
                        // Применяем импульс к сфере
                        otherBody.ApplyImpulse(kickImpulse);
                        
                        // Добавляем небольшой случайный вращательный момент для более реалистичного поведения
                        float randomTorque = (float)new Random().NextDouble() * 0.5f - 0.25f;
                        Vector3 randomAxis = new Vector3(
                            (float)new Random().NextDouble() * 2 - 1,
                            (float)new Random().NextDouble() * 2 - 1,
                            (float)new Random().NextDouble() * 2 - 1
                        );
                        if (randomAxis.Length > 0)
                        {
                            randomAxis = randomAxis / randomAxis.Length;
                        }
                        otherBody.ApplyAngularImpulse(randomAxis * randomTorque);
                    }
                    
                    // Гасим скорость персонажа в направлении объекта
                    cameraBody._velocity -= collisionNormal * velocityTowardsObject;
                }
                
                return;
            }
            
            // Стандартная обработка для других динамических объектов
            // Корректируем позиции обоих объектов
            float totalMass = bodyA.Mass + bodyB.Mass;
            float ratioA = bodyB.Mass / totalMass;
            float ratioB = bodyA.Mass / totalMass;
            
            collision.ObjectA.Position -= collision.Normal * collision.PenetrationDepth * ratioA;
            collision.ObjectB.Position += collision.Normal * collision.PenetrationDepth * ratioB;
            
            // Рассчитываем импульс для обоих объектов
            float restitution = 0.3f; // Коэффициент восстановления
            
            // Относительная скорость
            Vector3 relativeVelocity = bodyB.Velocity - bodyA.Velocity;
            float velocityProjection = Vector3.Dot(relativeVelocity, collision.Normal);
            
            // Если объекты удаляются друг от друга, не применяем импульс
            if (velocityProjection > 0)
                return;
            
            // Расчет импульса
            float impulseScalar = -(1 + restitution) * velocityProjection;
            impulseScalar /= (1/bodyA.Mass) + (1/bodyB.Mass);
            
            Vector3 impulse = impulseScalar * collision.Normal;
            
            // Применяем импульс через специальный метод
            bodyA.ApplyImpulse(-impulse);
            bodyB.ApplyImpulse(impulse);
        }

        // Проверка столкновения камеры с другим объектом
        private static bool CheckCameraCollision(Camera camera, TransformableObject other, out CollisionInfo collisionInfo)
        {
            collisionInfo = new CollisionInfo
            {
                ObjectA = camera,
                ObjectB = other
            };
            
            // Используем сферическую коллизию для камеры с заданным радиусом
            float cameraRadius = camera.CollisionRadius;
            
            if (other is Sphere sphere)
            {
                // Проверка камера-сфера (используем алгоритм сфера-сфера)
                // Получаем радиус сферы с учетом масштаба
                float sphereRadius = 0.5f * MathF.Max(sphere.Scale.X, MathF.Max(sphere.Scale.Y, sphere.Scale.Z));
                
                // Увеличиваем область коллизии сферы на 20% для более надежного определения столкновений
                float effectiveSphereRadius = sphereRadius * 1.2f;
                float radiusSum = cameraRadius + effectiveSphereRadius;
                
                // Вычисляем вектор между центрами
                Vector3 direction = sphere.Position - camera.Position;
                float distance = direction.Length;
                
                // Проверяем, есть ли пересечение
                if (distance < radiusSum)
                {
                    // Нормализуем направление
                    direction = distance > 0 ? direction / distance : new Vector3(0, 1, 0);
                    
                    // Учитываем наклон поверхности сферы для более реалистичной коллизии
                    Vector3 sphereSurfaceNormal = direction;
                    
                    // Если персонаж находится над сферой, усиливаем вертикальную составляющую нормали
                    if (direction.Y > 0)
                    {
                        sphereSurfaceNormal = Vector3.Normalize(new Vector3(
                            direction.X * 0.5f,
                            direction.Y,
                            direction.Z * 0.5f
                        ));
                    }
                    
                    collisionInfo.Normal = sphereSurfaceNormal;
                    collisionInfo.PenetrationDepth = radiusSum - distance;
                    collisionInfo.ContactPoint = camera.Position + direction * (cameraRadius - collisionInfo.PenetrationDepth * 0.5f);
                    
                    return true;
                }
            }
            else if (other is Cube cube)
            {
                // Проверка камера-куб с учетом возможности нахождения камеры внутри куба
                Vector3 halfSize = cube.Scale * 0.5f;
                Vector3 cubeToCamera = camera.Position - cube.Position;
                
                // Проверяем, находится ли камера внутри куба
                bool insideX = MathF.Abs(cubeToCamera.X) < halfSize.X;
                bool insideY = MathF.Abs(cubeToCamera.Y) < halfSize.Y;
                bool insideZ = MathF.Abs(cubeToCamera.Z) < halfSize.Z;
                
                if (insideX && insideY && insideZ)
                {
                    // Камера внутри куба, находим ближайшую грань для выхода
                    float distToXFace = MathF.Min(halfSize.X - MathF.Abs(cubeToCamera.X), halfSize.X + MathF.Abs(cubeToCamera.X));
                    float distToYFace = MathF.Min(halfSize.Y - MathF.Abs(cubeToCamera.Y), halfSize.Y + MathF.Abs(cubeToCamera.Y));
                    float distToZFace = MathF.Min(halfSize.Z - MathF.Abs(cubeToCamera.Z), halfSize.Z + MathF.Abs(cubeToCamera.Z));
                    
                    Vector3 normal = Vector3.Zero;
                    float penetrationDepth = 0;
                    
                    if (distToXFace <= distToYFace && distToXFace <= distToZFace)
                    {
                        normal = new Vector3(cubeToCamera.X > 0 ? 1 : -1, 0, 0);
                        penetrationDepth = distToXFace + cameraRadius;
                    }
                    else if (distToYFace <= distToXFace && distToYFace <= distToZFace)
                    {
                        normal = new Vector3(0, cubeToCamera.Y > 0 ? 1 : -1, 0);
                        penetrationDepth = distToYFace + cameraRadius;
                    }
                    else
                    {
                        normal = new Vector3(0, 0, cubeToCamera.Z > 0 ? 1 : -1);
                        penetrationDepth = distToZFace + cameraRadius;
                    }
                    
                    collisionInfo.Normal = normal;
                    collisionInfo.PenetrationDepth = penetrationDepth;
                    
                    // Вычисляем точку контакта как проекцию на ближайшую грань
                    Vector3 faceCenter = cube.Position + Vector3.Multiply(normal, halfSize);
                    collisionInfo.ContactPoint = faceCenter;
                    
                    return true;
                }
                
                // Стандартный алгоритм для случая, когда камера снаружи куба
                // Находим ближайшую к камере точку на кубе
                Vector3 closestPoint = Vector3.Zero;
                
                // Ограничиваем по каждой оси
                closestPoint.X = MathHelper.Clamp(cubeToCamera.X, -halfSize.X, halfSize.X);
                closestPoint.Y = MathHelper.Clamp(cubeToCamera.Y, -halfSize.Y, halfSize.Y);
                closestPoint.Z = MathHelper.Clamp(cubeToCamera.Z, -halfSize.Z, halfSize.Z);
                
                // Конвертируем обратно в мировые координаты
                closestPoint += cube.Position;
                
                // Проверяем расстояние между ближайшей точкой и камерой
                Vector3 direction = closestPoint - camera.Position;
                float distance = direction.Length;
                
                if (distance < cameraRadius)
                {
                    // Нормализуем направление
                    direction = distance > 0 ? direction / distance : new Vector3(0, 1, 0);
                    
                    collisionInfo.Normal = -direction; // От куба к камере
                    collisionInfo.PenetrationDepth = cameraRadius - distance;
                    collisionInfo.ContactPoint = closestPoint;
                    
                    return true;
                }
            }
            else if (other is Prism prism)
            {
                // Проверка камера-призма с учетом возможности нахождения камеры внутри призмы
                Vector3 halfSize = prism.Scale * 0.5f;
                
                // Создаем матрицу вращения призмы
                Matrix4 prismRotation = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(prism.Rotation.X)) *
                                       Matrix4.CreateRotationY(MathHelper.DegreesToRadians(prism.Rotation.Y)) *
                                       Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(prism.Rotation.Z));
                
                // Создаем обратную матрицу вращения для преобразования в локальное пространство призмы
                Matrix4 inversePrismRotation = Matrix4.Invert(prismRotation);
                
                // Вектор от призмы к камере в мировых координатах
                Vector3 prismToCamera = camera.Position - prism.Position;
                
                // Преобразуем вектор в локальное пространство призмы
                Vector3 localPrismToCamera = Vector3.TransformVector(prismToCamera, inversePrismRotation);
                
                // Проверяем, находится ли камера внутри bounding box призмы
                bool insideX = MathF.Abs(localPrismToCamera.X) < halfSize.X;
                bool insideY = MathF.Abs(localPrismToCamera.Y) < halfSize.Y;
                bool insideZ = MathF.Abs(localPrismToCamera.Z) < halfSize.Z;
                
                // Проверка на нахождение внутри нижней части призмы (прямоугольная часть)
                bool insideRectBase = insideX && insideZ && localPrismToCamera.Y < 0 && localPrismToCamera.Y > -halfSize.Y;
                
                // Проверка на нахождение внутри верхней части призмы (треугольная часть)
                bool insideTopPart = insideX && insideZ && localPrismToCamera.Y >= 0 && localPrismToCamera.Y <= halfSize.Y;
                
                // Расчет максимальной высоты в точке расположения камеры (для верхней треугольной части)
                float maxYAtPosition = 0;
                if (insideTopPart)
                {
                    // Для простоты считаем, что верхняя часть имеет форму пирамиды
                    float normalizedX = MathF.Abs(localPrismToCamera.X) / halfSize.X;
                    float normalizedZ = MathF.Abs(localPrismToCamera.Z) / halfSize.Z;
                    
                    // Линейная интерполяция высоты в зависимости от расстояния от центра
                    maxYAtPosition = halfSize.Y * (1.0f - MathF.Max(normalizedX, normalizedZ));
                }
                
                // Определяем, находится ли камера внутри призмы
                bool insidePrism = insideRectBase || (insideTopPart && localPrismToCamera.Y < maxYAtPosition);
                
                if (insidePrism)
                {
                    // Камера внутри призмы, находим ближайшую грань для выхода
                    float distToXFace = halfSize.X - MathF.Abs(localPrismToCamera.X);
                    float distToZFace = halfSize.Z - MathF.Abs(localPrismToCamera.Z);
                    float distToBottomFace = halfSize.Y + localPrismToCamera.Y; // Расстояние до нижней грани
                    
                    // Расстояние до верхней наклонной грани
                    float distToTopFace = float.MaxValue;
                    Vector3 topNormal = Vector3.Zero;
                    
                    // Определяем радиус коллизии камеры один раз для всего метода
                    float collisionRadius = (camera as Camera)?.CollisionRadius ?? 0.5f;
                    
                    if (localPrismToCamera.Y >= 0)
                    {
                        // Для простоты: верхняя грань имеет нормаль с компонентами X и Z, 
                        // направленными от центра к краям основания пирамиды
                        float normalizedX = localPrismToCamera.X / halfSize.X;
                        float normalizedZ = localPrismToCamera.Z / halfSize.Z;
                        
                        // Создаем нормаль к наклонной грани в точке над камерой
                        topNormal = Vector3.Normalize(new Vector3(normalizedX, 0.5f, normalizedZ));
                        
                        // Расстояние до верхней грани (приблизительное)
                        float currentMaxY = halfSize.Y * (1.0f - MathF.Max(normalizedX, normalizedZ));
                        distToTopFace = currentMaxY - localPrismToCamera.Y;
                    }
                    
                    Vector3 localNormal;
                    float penetrationDepth;
                    Vector3 localContactPoint;
                    
                    // Определяем ближайшую грань
                    if (distToXFace <= distToZFace && distToXFace <= distToBottomFace && distToXFace <= distToTopFace)
                    {
                        // Ближайшая X-грань
                        localNormal = new Vector3(localPrismToCamera.X > 0 ? 1 : -1, 0, 0);
                        penetrationDepth = distToXFace + collisionRadius;
                        localContactPoint = new Vector3(
                            localPrismToCamera.X > 0 ? halfSize.X : -halfSize.X,
                            localPrismToCamera.Y,
                            localPrismToCamera.Z
                        );
                    }
                    else if (distToZFace <= distToXFace && distToZFace <= distToBottomFace && distToZFace <= distToTopFace)
                    {
                        // Ближайшая Z-грань
                        localNormal = new Vector3(0, 0, localPrismToCamera.Z > 0 ? 1 : -1);
                        penetrationDepth = distToZFace + collisionRadius;
                        localContactPoint = new Vector3(
                            localPrismToCamera.X,
                            localPrismToCamera.Y,
                            localPrismToCamera.Z > 0 ? halfSize.Z : -halfSize.Z
                        );
                    }
                    else if (distToBottomFace <= distToXFace && distToBottomFace <= distToZFace && distToBottomFace <= distToTopFace)
                    {
                        // Ближайшая нижняя грань
                        localNormal = new Vector3(0, -1, 0);
                        penetrationDepth = distToBottomFace + collisionRadius;
                        localContactPoint = new Vector3(
                            localPrismToCamera.X,
                            -halfSize.Y,
                            localPrismToCamera.Z
                        );
                    }
                    else
                    {
                        // Ближайшая верхняя наклонная грань
                        localNormal = topNormal;
                        float currentMaxY = halfSize.Y * (1.0f - MathF.Max(
                            MathF.Abs(localPrismToCamera.X) / halfSize.X,
                            MathF.Abs(localPrismToCamera.Z) / halfSize.Z
                        ));
                        penetrationDepth = distToTopFace + collisionRadius;
                        localContactPoint = new Vector3(
                            localPrismToCamera.X,
                            currentMaxY,
                            localPrismToCamera.Z
                        );
                    }
                    
                    // Преобразуем нормаль и точку контакта в мировые координаты
                    Vector3 worldNormal = Vector3.TransformVector(localNormal, prismRotation);
                    Vector3 worldContactPoint = Vector3.TransformPosition(localContactPoint, prismRotation) + prism.Position;
                    
                    // Применяем результаты к объекту collision
                    collisionInfo.Normal = worldNormal;
                    collisionInfo.PenetrationDepth = penetrationDepth;
                    collisionInfo.ContactPoint = worldContactPoint;
                    
                    return true;
                }
                
                // Если камера снаружи призмы, проверяем столкновение со всеми гранями
                
                // Проверка коллизии с нижней гранью
                if (insideX && insideZ && localPrismToCamera.Y < -halfSize.Y && localPrismToCamera.Y > -halfSize.Y - cameraRadius)
                {
                    Vector3 localNormal = new Vector3(0, -1, 0);
                    collisionInfo.Normal = Vector3.TransformVector(localNormal, prismRotation);
                    collisionInfo.PenetrationDepth = cameraRadius - MathF.Abs(localPrismToCamera.Y + halfSize.Y);
                    collisionInfo.ContactPoint = Vector3.TransformPosition(new Vector3(
                        localPrismToCamera.X,
                        -halfSize.Y,
                        localPrismToCamera.Z
                    ), prismRotation) + prism.Position;
                    return true;
                }
                
                // Проверка коллизии с верхними наклонными гранями
                if (insideX && insideZ && localPrismToCamera.Y > 0 && localPrismToCamera.Y < halfSize.Y + cameraRadius)
                {
                    // Вычисляем нормаль к верхней грани в точке над камерой
                    float normalizedX = localPrismToCamera.X / halfSize.X;
                    float normalizedZ = localPrismToCamera.Z / halfSize.Z;
                    Vector3 localSlopeNormal = Vector3.Normalize(new Vector3(normalizedX, 1.0f, normalizedZ));
                    
                    // Вычисляем расстояние до верхней грани по нормали
                    float distToSlope = maxYAtPosition - localPrismToCamera.Y;
                    
                    if (distToSlope < cameraRadius)
                    {
                        collisionInfo.Normal = Vector3.TransformVector(localSlopeNormal, prismRotation);
                        collisionInfo.PenetrationDepth = cameraRadius - distToSlope;
                        collisionInfo.ContactPoint = Vector3.TransformPosition(new Vector3(
                            localPrismToCamera.X,
                            maxYAtPosition,
                            localPrismToCamera.Z
                        ), prismRotation) + prism.Position;
                        return true;
                    }
                }
                
                // Проверка коллизии с боковыми гранями
                Vector3 localClosestPoint = Vector3.Zero;
                
                // Ограничиваем точку по бокам призмы
                localClosestPoint.X = MathHelper.Clamp(localPrismToCamera.X, -halfSize.X, halfSize.X);
                localClosestPoint.Z = MathHelper.Clamp(localPrismToCamera.Z, -halfSize.Z, halfSize.Z);
                
                // Определяем, к какому треугольнику (верхнему или нижнему) ближе точка
                if (localPrismToCamera.Y >= 0)
                {
                    // Для верхней части (треугольной)
                    float normalizedX = MathF.Abs(localClosestPoint.X) / halfSize.X;
                    float normalizedZ = MathF.Abs(localClosestPoint.Z) / halfSize.Z;
                    localClosestPoint.Y = MathHelper.Clamp(localPrismToCamera.Y, 0, halfSize.Y * (1.0f - MathF.Max(normalizedX, normalizedZ)));
                }
                else
                {
                    // Для нижней части (прямоугольной)
                    localClosestPoint.Y = MathHelper.Clamp(localPrismToCamera.Y, -halfSize.Y, 0);
                }
                
                // Преобразуем точку в мировые координаты
                Vector3 closestPoint = Vector3.TransformPosition(localClosestPoint, prismRotation) + prism.Position;
                
                // Проверяем расстояние между ближайшей точкой и камерой
                Vector3 direction = closestPoint - camera.Position;
                float distance = direction.Length;
                
                if (distance < cameraRadius)
                {
                    // Нормализуем направление
                    direction = distance > 0 ? direction / distance : new Vector3(0, 1, 0);
                    
                    collisionInfo.Normal = -direction; // От призмы к камере
                    collisionInfo.PenetrationDepth = cameraRadius - distance;
                    collisionInfo.ContactPoint = closestPoint;
                    
                    return true;
                }
            }
            else if (other is Plane plane)
            {
                // Проверка камера-плоскость
                // Для простоты считаем, что нормаль плоскости всегда (0, 1, 0)
                Vector3 planeNormal = new Vector3(0, 1, 0);
                
                // Расстояние от камеры до плоскости
                float distanceToPlane = camera.Position.Y - plane.Position.Y;
                
                // Если камера ниже плоскости или слишком близко к ней
                if (distanceToPlane < cameraRadius)
                {
                    collisionInfo.Normal = planeNormal;
                    collisionInfo.PenetrationDepth = cameraRadius - distanceToPlane;
                    collisionInfo.ContactPoint = new Vector3(
                        camera.Position.X,
                        plane.Position.Y,
                        camera.Position.Z
                    );
                    
                    return true;
                }
            }
            
            return false;
        }
    }
} 