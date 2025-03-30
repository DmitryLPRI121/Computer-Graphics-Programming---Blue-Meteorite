using OpenTK.Mathematics;
using System.Collections.Generic;

namespace Computer_Graphics_Programming_Blue_Meteorite
{
    public class TransformableObject
    {
        public List<TransformableObject> Children { get; set; } = new List<TransformableObject>();

        public Vector3 Position { get; set; } = Vector3.Zero;
        public Vector3 Rotation { get; set; } = Vector3.Zero; // Углы Эйлера
        public Vector3 Scale { get; set; } = Vector3.One;
        public string Name { get; set; }
        public DynamicBody? SelfDynamic { get; set; }
        public Vector3 Color { get; set; } = Vector3.One;

        public void AddChild(TransformableObject child)
        {
            Children.Add(child);
        }

        public Matrix4 GetModelMatrix()
        {
            Matrix4 model = Matrix4.Identity;

            // Сначала масштабируем объект
            model *= Matrix4.CreateScale(Scale);

            // Затем вращаем вокруг всех осей
            model *= Matrix4.CreateRotationX(MathHelper.DegreesToRadians(Rotation.X));
            model *= Matrix4.CreateRotationY(MathHelper.DegreesToRadians(Rotation.Y));
            model *= Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(Rotation.Z));

            // И наконец, смещаем в пространстве
            model *= Matrix4.CreateTranslation(Position);

            return model;
        }

        public void Render(Shader shader, Matrix4 parentTransform = default)
        {
            // Если parentTransform не задан, используем единичную матрицу
            if (parentTransform == default)
            {
                parentTransform = Matrix4.Identity;
            }

            // Вычисляем глобальную матрицу модели
            Matrix4 model = GetModelMatrix() * parentTransform;

            // Устанавливаем матрицы и параметры в шейдер
            shader.SetMatrix4("model", model);
            shader.SetVector3("objectColor", Color);

            // Рендерим объект
            Draw(shader);

            shader.SetInt("texture_diffuse1", 0);

            // Рендерим дочерние объекты
            foreach (var child in Children)
            {
                child.Render(shader, model);
            }
        }

        protected virtual void Draw(Shader shader) { }
        
        public void ApplyTranslation(Vector3 translation)
        {
            // For dynamic objects, we should apply a force instead of directly changing the position
            if (SelfDynamic != null)
            {
                // Apply a larger force to make the movement more immediate and noticeable
                Vector3 force = translation * 100.0f; // Higher multiplier for more responsive movement
                SelfDynamic.ApplyForce(force, 0.1f);
                
                // Also update position directly to provide immediate visual feedback
                Position += translation;
            }
            else
            {
                // For static objects, directly update position
                Position += translation;
            }
        }
        
        public void ApplyRotation(Vector3 rotation)
        {
            // Create a new Vector3 with the sum of current rotation and new rotation
            Vector3 newRotation = new Vector3(
                Rotation.X + rotation.X,
                Rotation.Y + rotation.Y,
                Rotation.Z + rotation.Z
            );
            
            // Normalize rotation angles to avoid issues
            if (newRotation.X > 360) newRotation.X %= 360;
            if (newRotation.Y > 360) newRotation.Y %= 360;
            if (newRotation.Z > 360) newRotation.Z %= 360;
            
            if (newRotation.X < 0) newRotation.X += 360;
            if (newRotation.Y < 0) newRotation.Y += 360;
            if (newRotation.Z < 0) newRotation.Z += 360;
            
            // Assign the normalized rotation
            Rotation = newRotation;
            
            // For dynamic objects, we should apply a torque or rotational force
            // This is simplified for now, but in a full physics simulation
            // we would calculate proper angular momentum
            if (SelfDynamic != null)
            {
                // In a full implementation, we would apply angular velocity here
                // SelfDynamic.ApplyTorque(rotation);
            }
        }
        
        public void ApplyScale(Vector3 scale)
        {
            Scale += scale;
            
            // Ensure scale doesn't go below a minimum threshold
            Scale = new Vector3(
                MathF.Max(0.1f, Scale.X),
                MathF.Max(0.1f, Scale.Y),
                MathF.Max(0.1f, Scale.Z)
            );
            
            // No need to update DynamicBody here as it handles scale via bounding box calculations
        }
    }
}