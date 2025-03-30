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

                collisionInfo.Normal = -direction; // От куба к сфере
                collisionInfo.PenetrationDepth = radius - distance;
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
            
            // Вектор от призмы к сфере
            Vector3 prismToSphere = sphere.Position - prism.Position;
            
            // Проверка коллизии с основанием призмы (прямоугольная часть)
            
            // Сначала проверим вертикальные стенки призмы
            // Ограничиваем по XZ плоскости (основание призмы)
            Vector3 closestPointXZ = new Vector3(
                MathHelper.Clamp(prismToSphere.X, -halfSize.X, halfSize.X),
                0,
                MathHelper.Clamp(prismToSphere.Z, -halfSize.Z, halfSize.Z)
            );
            
            // Проверяем, находится ли сфера в пределах прямоугольного основания по XZ
            bool insideRectBase = MathF.Abs(prismToSphere.X) <= halfSize.X && 
                                  MathF.Abs(prismToSphere.Z) <= halfSize.Z;
            
            // Проверяем коллизию с нижней гранью (прямоугольник)
            if (insideRectBase && prismToSphere.Y < 0 && MathF.Abs(prismToSphere.Y) < radius + halfSize.Y)
            {
                // Коллизия с нижней гранью
                collisionInfo.Normal = new Vector3(0, -1, 0);
                collisionInfo.PenetrationDepth = radius + halfSize.Y - MathF.Abs(prismToSphere.Y);
                collisionInfo.ContactPoint = new Vector3(
                    sphere.Position.X,
                    prism.Position.Y - halfSize.Y,
                    sphere.Position.Z
                );
                return true;
            }
            
            // Проверяем коллизию с верхней наклонной гранью (треугольник)
            if (insideRectBase && prismToSphere.Y > 0)
            {
                // Нормаль к верхней треугольной грани (направлена вверх под углом)
                Vector3 topNormal = Vector3.Normalize(new Vector3(0, halfSize.Y, 0));
                
                // Расстояние от сферы до верхней грани по нормали
                float distToTopFace = Vector3.Dot(prismToSphere, topNormal) - halfSize.Y;
                
                // Если сфера достаточно близко к верхней грани
                if (distToTopFace < radius)
                {
                    collisionInfo.Normal = topNormal;
                    collisionInfo.PenetrationDepth = radius - distToTopFace;
                    collisionInfo.ContactPoint = sphere.Position - topNormal * distToTopFace;
                    return true;
                }
            }
            
            // Проверяем коллизию с боковыми гранями (четыре трапеции)
            if (!insideRectBase || prismToSphere.Y > 0)
            {
                // Определяем ближайшую точку на боковой грани
                Vector3 closestPointOnSide = Vector3.Zero;
                Vector3 sideNormal = Vector3.Zero;
                float minDistance = float.MaxValue;
                
                // Проверяем каждую боковую грань
                for (int side = 0; side < 4; side++)
                {
                    Vector3 edgeNormal;
                    float edgeHalfSize;
                    
                    // Выбираем параметры в зависимости от номера грани
                    if (side == 0 || side == 2) // X-грани
                    {
                        edgeNormal = new Vector3(side == 0 ? -1 : 1, 0, 0);
                        edgeHalfSize = halfSize.X;
                    }
                    else // Z-грани
                    {
                        edgeNormal = new Vector3(0, 0, side == 1 ? 1 : -1);
                        edgeHalfSize = halfSize.Z;
                    }
                    
                    // Расстояние от сферы до грани по нормали
                    float distToEdge;
                    
                    if (side == 0) // -X грань
                        distToEdge = -prismToSphere.X - halfSize.X;
                    else if (side == 1) // +Z грань
                        distToEdge = prismToSphere.Z - halfSize.Z;
                    else if (side == 2) // +X грань
                        distToEdge = prismToSphere.X - halfSize.X;
                    else // -Z грань
                        distToEdge = -prismToSphere.Z - halfSize.Z;
                    
                    // Если сфера потенциально пересекает эту грань
                    if (distToEdge < radius)
                    {
                        // Проверяем, не выходит ли сфера за пределы этой грани в другом направлении
                        bool withinEdgeBounds = false;
                        
                        if (side == 0 || side == 2) // X-грани, проверяем пределы по Z
                            withinEdgeBounds = MathF.Abs(prismToSphere.Z) < halfSize.Z;
                        else // Z-грани, проверяем пределы по X
                            withinEdgeBounds = MathF.Abs(prismToSphere.X) < halfSize.X;
                        
                        // Также проверяем, находится ли сфера в пределах высоты боковой грани
                        withinEdgeBounds = withinEdgeBounds && prismToSphere.Y >= -halfSize.Y && 
                                          prismToSphere.Y <= halfSize.Y;
                        
                        if (withinEdgeBounds && MathF.Abs(distToEdge) < minDistance)
                        {
                            minDistance = MathF.Abs(distToEdge);
                            sideNormal = edgeNormal;
                            
                            // Ближайшая точка на боковой грани
                            closestPointOnSide = prism.Position;
                            
                            if (side == 0) // -X грань
                                closestPointOnSide.X -= halfSize.X;
                            else if (side == 1) // +Z грань
                                closestPointOnSide.Z += halfSize.Z;
                            else if (side == 2) // +X грань
                                closestPointOnSide.X += halfSize.X;
                            else // -Z грань
                                closestPointOnSide.Z -= halfSize.Z;
                            
                            if (side == 0 || side == 2) // X-грани
                                closestPointOnSide.Z = sphere.Position.Z;
                            else // Z-грани
                                closestPointOnSide.X = sphere.Position.X;
                            
                            closestPointOnSide.Y = MathHelper.Clamp(sphere.Position.Y, 
                                                                 prism.Position.Y - halfSize.Y,
                                                                 prism.Position.Y + halfSize.Y);
                        }
                    }
                }
                
                // Если нашли ближайшую боковую грань
                if (minDistance < float.MaxValue && minDistance < radius)
                {
                    collisionInfo.Normal = -sideNormal; // От призмы к сфере
                    collisionInfo.PenetrationDepth = radius - minDistance;
                    collisionInfo.ContactPoint = closestPointOnSide;
                    return true;
                }
            }
            
            // Проверяем коллизию с рёбрами
            if (!insideRectBase && prismToSphere.Y < 0)
            {
                // Проверяем четыре нижних ребра
                Vector3 closestPoint = prism.Position - new Vector3(0, halfSize.Y, 0);
                float minDistance = float.MaxValue;
                
                // Перебираем 4 ребра нижней грани
                for (int edge = 0; edge < 4; edge++)
                {
                    Vector3 edgePoint = closestPoint;
                    
                    // Координаты вершин нижней грани
                    if (edge == 0) // нижняя левая задняя
                    {
                        edgePoint.X -= halfSize.X;
                        edgePoint.Z -= halfSize.Z;
                    }
                    else if (edge == 1) // нижняя правая задняя
                    {
                        edgePoint.X += halfSize.X;
                        edgePoint.Z -= halfSize.Z;
                    }
                    else if (edge == 2) // нижняя правая передняя
                    {
                        edgePoint.X += halfSize.X;
                        edgePoint.Z += halfSize.Z;
                    }
                    else // нижняя левая передняя
                    {
                        edgePoint.X -= halfSize.X;
                        edgePoint.Z += halfSize.Z;
                    }
                    
                    float edgeDistance = (sphere.Position - edgePoint).Length;
                    
                    if (edgeDistance < minDistance)
                    {
                        minDistance = edgeDistance;
                        closestPoint = edgePoint;
                    }
                }
                
                if (minDistance < radius)
                {
                    Vector3 edgeDirection = sphere.Position - closestPoint;
                    float edgeToSphereDistance = edgeDirection.Length;
                    edgeDirection = edgeToSphereDistance > 0 ? edgeDirection / edgeToSphereDistance : new Vector3(0, 1, 0);
                    
                    collisionInfo.Normal = -edgeDirection; // От призмы к сфере
                    collisionInfo.PenetrationDepth = radius - minDistance;
                    collisionInfo.ContactPoint = closestPoint;
                    return true;
                }
            }
            
            // Если все предыдущие проверки не дали результат,
            // используем упрощенный подход с bounding box
            Vector3 closestPointBB = Vector3.Zero;
            
            // Ограничиваем по каждой оси
            closestPointBB.X = MathHelper.Clamp(prismToSphere.X, -halfSize.X, halfSize.X);
            closestPointBB.Y = MathHelper.Clamp(prismToSphere.Y, -halfSize.Y, halfSize.Y);
            closestPointBB.Z = MathHelper.Clamp(prismToSphere.Z, -halfSize.Z, halfSize.Z);
            
            // Конвертируем обратно в мировые координаты
            closestPointBB += prism.Position;
            
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
                    Vector3 prismToCamera = camera.Position - prism.Position;
                    
                    // Проверяем, находится ли камера внутри призмы (упрощенно через bounding box)
                    bool insideX = MathF.Abs(prismToCamera.X) < halfSize.X;
                    bool insideY = MathF.Abs(prismToCamera.Y) < halfSize.Y;
                    bool insideZ = MathF.Abs(prismToCamera.Z) < halfSize.Z;
                    
                    if (insideX && insideY && insideZ)
                    {
                        // Если камера внутри призмы, находим ближайшую грань для выхода
                        float distToXFace = MathF.Min(halfSize.X - MathF.Abs(prismToCamera.X), halfSize.X + MathF.Abs(prismToCamera.X));
                        float distToYFace = MathF.Min(halfSize.Y - MathF.Abs(prismToCamera.Y), halfSize.Y + MathF.Abs(prismToCamera.Y));
                        float distToZFace = MathF.Min(halfSize.Z - MathF.Abs(prismToCamera.Z), halfSize.Z + MathF.Abs(prismToCamera.Z));
                        
                        if (distToXFace <= distToYFace && distToXFace <= distToZFace)
                        {
                            pushDirection = new Vector3(prismToCamera.X > 0 ? 1 : -1, 0, 0);
                            pushDistance = halfSize.X + cameraObj.CollisionRadius + 0.05f;
                        }
                        else if (distToYFace <= distToXFace && distToYFace <= distToZFace)
                        {
                            pushDirection = new Vector3(0, prismToCamera.Y > 0 ? 1 : -1, 0);
                            pushDistance = halfSize.Y + cameraObj.CollisionRadius + 0.05f;
                        }
                        else
                        {
                            pushDirection = new Vector3(0, 0, prismToCamera.Z > 0 ? 1 : -1);
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
                else
                {
                    // Для других типов объектов используем обычную нормаль
                    pushDirection = normal;
                    pushDistance = collision.PenetrationDepth + 0.05f;
                }
                
                // Проверка, стоит ли персонаж на объекте (нормаль направлена вниз)
                bool isStandingOnObject = pushDirection.Y < -0.7f; // Если нормаль направлена вниз (с запасом)
                
                if (isStandingOnObject)
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
                camera.Position = other.Position + pushDirection * pushDistance;
                
                // Если персонаж двигался в сторону объекта, гасим его скорость в этом направлении
                float velocityTowardsObject = Vector3.Dot(cameraBody._velocity, -pushDirection);
                if (velocityTowardsObject > 0)
                {
                    cameraBody._velocity -= -pushDirection * velocityTowardsObject;
                }
                
                // Если объект движется в сторону персонажа, отталкиваем его
                if (otherBody != null)
                {
                    float objectVelocityTowardsCamera = Vector3.Dot(otherBody._velocity, pushDirection);
                    if (objectVelocityTowardsCamera > 0)
                    {
                        float repulsionFactor = 1.2f; // Усиленный отталкивающий фактор
                        otherBody.ApplyImpulse(-pushDirection * objectVelocityTowardsCamera * repulsionFactor);
                    }
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
                // Получаем радиус сферы
                float sphereRadius = 0.5f * MathF.Max(sphere.Scale.X, MathF.Max(sphere.Scale.Y, sphere.Scale.Z));
                float radiusSum = cameraRadius + sphereRadius;
                
                // Вычисляем вектор между центрами
                Vector3 direction = sphere.Position - camera.Position;
                float distance = direction.Length;
                
                // Проверяем, есть ли пересечение
                if (distance < radiusSum)
                {
                    // Нормализуем направление
                    direction = distance > 0 ? direction / distance : new Vector3(0, 1, 0);
                    
                    collisionInfo.Normal = direction;
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
                Vector3 prismToCamera = camera.Position - prism.Position;
                
                // Проверяем, находится ли камера внутри bounding box призмы
                bool insideX = MathF.Abs(prismToCamera.X) < halfSize.X;
                bool insideY = MathF.Abs(prismToCamera.Y) < halfSize.Y;
                bool insideZ = MathF.Abs(prismToCamera.Z) < halfSize.Z;
                
                // Проверка на нахождение внутри нижней части призмы (прямоугольная часть)
                bool insideRectBase = insideX && insideZ && prismToCamera.Y < 0 && prismToCamera.Y > -halfSize.Y;
                
                // Проверка на нахождение внутри верхней части призмы (треугольная часть)
                bool insideTopPart = insideX && insideZ && prismToCamera.Y >= 0 && prismToCamera.Y <= halfSize.Y;
                
                // Расчет максимальной высоты в точке расположения камеры (для верхней треугольной части)
                float maxYAtPosition = 0;
                if (insideTopPart)
                {
                    // Для простоты считаем, что верхняя часть имеет форму пирамиды
                    float normalizedX = MathF.Abs(prismToCamera.X) / halfSize.X;
                    float normalizedZ = MathF.Abs(prismToCamera.Z) / halfSize.Z;
                    
                    // Линейная интерполяция высоты в зависимости от расстояния от центра
                    maxYAtPosition = halfSize.Y * (1.0f - MathF.Max(normalizedX, normalizedZ));
                }
                
                // Определяем, находится ли камера внутри призмы
                bool insidePrism = insideRectBase || (insideTopPart && prismToCamera.Y < maxYAtPosition);
                
                if (insidePrism)
                {
                    // Камера внутри призмы, находим ближайшую грань для выхода
                    float distToXFace = halfSize.X - MathF.Abs(prismToCamera.X);
                    float distToZFace = halfSize.Z - MathF.Abs(prismToCamera.Z);
                    float distToBottomFace = halfSize.Y + prismToCamera.Y; // Расстояние до нижней грани
                    
                    // Расстояние до верхней наклонной грани
                    float distToTopFace = float.MaxValue;
                    Vector3 topNormal = Vector3.Zero;
                    
                    if (prismToCamera.Y >= 0)
                    {
                        // Для простоты: верхняя грань имеет нормаль с компонентами X и Z, 
                        // направленными от центра к краям основания пирамиды
                        float normalizedX = prismToCamera.X / halfSize.X;
                        float normalizedZ = prismToCamera.Z / halfSize.Z;
                        
                        // Создаем нормаль к наклонной грани в точке над камерой
                        topNormal = Vector3.Normalize(new Vector3(normalizedX, 0.5f, normalizedZ));
                        
                        // Расстояние до верхней грани (приблизительное)
                        distToTopFace = maxYAtPosition - prismToCamera.Y;
                    }
                    
                    Vector3 normal;
                    float penetrationDepth;
                    Vector3 contactPoint;
                    
                    // Определяем ближайшую грань
                    if (distToXFace <= distToZFace && distToXFace <= distToBottomFace && distToXFace <= distToTopFace)
                    {
                        // Ближайшая X-грань
                        normal = new Vector3(prismToCamera.X > 0 ? 1 : -1, 0, 0);
                        penetrationDepth = distToXFace + cameraRadius;
                        contactPoint = prism.Position + new Vector3(
                            prismToCamera.X > 0 ? halfSize.X : -halfSize.X,
                            prismToCamera.Y,
                            prismToCamera.Z
                        );
                    }
                    else if (distToZFace <= distToXFace && distToZFace <= distToBottomFace && distToZFace <= distToTopFace)
                    {
                        // Ближайшая Z-грань
                        normal = new Vector3(0, 0, prismToCamera.Z > 0 ? 1 : -1);
                        penetrationDepth = distToZFace + cameraRadius;
                        contactPoint = prism.Position + new Vector3(
                            prismToCamera.X,
                            prismToCamera.Y,
                            prismToCamera.Z > 0 ? halfSize.Z : -halfSize.Z
                        );
                    }
                    else if (distToBottomFace <= distToXFace && distToBottomFace <= distToZFace && distToBottomFace <= distToTopFace)
                    {
                        // Ближайшая нижняя грань
                        normal = new Vector3(0, -1, 0);
                        penetrationDepth = distToBottomFace + cameraRadius;
                        contactPoint = prism.Position + new Vector3(
                            prismToCamera.X,
                            -halfSize.Y,
                            prismToCamera.Z
                        );
                    }
                    else
                    {
                        // Ближайшая верхняя наклонная грань
                        normal = topNormal;
                        penetrationDepth = distToTopFace + cameraRadius;
                        contactPoint = prism.Position + new Vector3(
                            prismToCamera.X,
                            maxYAtPosition,
                            prismToCamera.Z
                        );
                    }
                    
                    collisionInfo.Normal = normal;
                    collisionInfo.PenetrationDepth = penetrationDepth;
                    collisionInfo.ContactPoint = contactPoint;
                    
                    return true;
                }
                
                // Если камера снаружи призмы, проверяем столкновение со всеми гранями
                
                // Проверка коллизии с нижней гранью
                if (insideX && insideZ && prismToCamera.Y < -halfSize.Y && prismToCamera.Y > -halfSize.Y - cameraRadius)
                {
                    collisionInfo.Normal = new Vector3(0, -1, 0);
                    collisionInfo.PenetrationDepth = cameraRadius - MathF.Abs(prismToCamera.Y + halfSize.Y);
                    collisionInfo.ContactPoint = new Vector3(
                        camera.Position.X,
                        prism.Position.Y - halfSize.Y,
                        camera.Position.Z
                    );
                    return true;
                }
                
                // Проверка коллизии с верхними наклонными гранями
                if (insideX && insideZ && prismToCamera.Y > 0 && prismToCamera.Y < halfSize.Y + cameraRadius)
                {
                    // Вычисляем нормаль к верхней грани в точке над камерой
                    float normalizedX = prismToCamera.X / halfSize.X;
                    float normalizedZ = prismToCamera.Z / halfSize.Z;
                    Vector3 slopeNormal = Vector3.Normalize(new Vector3(normalizedX, 1.0f, normalizedZ));
                    
                    // Вычисляем расстояние до верхней грани по нормали
                    float distToSlope = maxYAtPosition - prismToCamera.Y;
                    
                    if (distToSlope < cameraRadius)
                    {
                        collisionInfo.Normal = slopeNormal;
                        collisionInfo.PenetrationDepth = cameraRadius - distToSlope;
                        collisionInfo.ContactPoint = camera.Position - slopeNormal * distToSlope;
                        return true;
                    }
                }
                
                // Проверка коллизии с боковыми гранями
                Vector3 closestPoint = Vector3.Zero;
                
                // Ограничиваем точку по бокам призмы
                closestPoint.X = MathHelper.Clamp(prismToCamera.X, -halfSize.X, halfSize.X);
                closestPoint.Z = MathHelper.Clamp(prismToCamera.Z, -halfSize.Z, halfSize.Z);
                
                // Определяем, к какому треугольнику (верхнему или нижнему) ближе точка
                if (prismToCamera.Y >= 0)
                {
                    // Для верхней части (треугольной)
                    float normalizedX = MathF.Abs(closestPoint.X) / halfSize.X;
                    float normalizedZ = MathF.Abs(closestPoint.Z) / halfSize.Z;
                    closestPoint.Y = MathHelper.Clamp(prismToCamera.Y, 0, halfSize.Y * (1.0f - MathF.Max(normalizedX, normalizedZ)));
                }
                else
                {
                    // Для нижней части (прямоугольной)
                    closestPoint.Y = MathHelper.Clamp(prismToCamera.Y, -halfSize.Y, 0);
                }
                
                // Конвертируем обратно в мировые координаты
                closestPoint += prism.Position;
                
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