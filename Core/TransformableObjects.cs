using OpenTK.Mathematics;

namespace Computer_Graphics_Programming_Blue_Meteorite
{
    public class TransformableObject
    {
        public List<TransformableObject> Children { get; set; } = new List<TransformableObject>();

        public Vector3 Position { get; set; } = Vector3.Zero;
        public Vector3 Rotation { get; set; } = Vector3.Zero;
        public Vector3 Scale { get; set; } = Vector3.One;
        public string Name { get; set; }
        public DynamicBody? SelfDynamic { get; set; }
        public Vector3 Color { get; set; } = Vector3.One;
        public float TextureRepeat { get; set; } = 1.0f;

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
            shader.SetFloat("textureRepeat", TextureRepeat);

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
        
    }
}