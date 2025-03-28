using OpenTK.Graphics.OpenGL4;

namespace Computer_Graphics_Programming_Blue_Meteorite
{
    public class Sphere : TransformableObject
    {

        private List<float> vertices = new List<float>();
        private List<uint> indices = new List<uint>();

        private int VBO, VAO, IBO;
        private Texture texture;

        private int segments;
        private int rings;

        public Sphere(string texturePath, int segments = 32, int rings = 16)
        {
            this.segments = segments;
            this.rings = rings;

            GenerateSphereData();
            InitializeBuffers();
            texture = new Texture(texturePath);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }

        private void GenerateSphereData()
        {
            // Генерация сферы с использованием сферических координат
            for (int ring = 0; ring <= rings; ring++)
            {
                // Угол phi идет от 0 до Pi (верх-низ)
                float phi = (float)ring / rings * MathF.PI;

                for (int segment = 0; segment <= segments; segment++)
                {
                    // Угол theta идет от 0 до 2*Pi (обход вокруг сферы)
                    float theta = (float)segment / segments * 2.0f * MathF.PI;

                    // Сферические координаты
                    float x = MathF.Sin(phi) * MathF.Cos(theta);
                    float y = MathF.Cos(phi);
                    float z = MathF.Sin(phi) * MathF.Sin(theta);

                    // Позиция вершины
                    vertices.Add(x * 0.5f);  // масштабируем до размера 0.5 (как у куба)
                    vertices.Add(y * 0.5f);
                    vertices.Add(z * 0.5f);

                    // Нормаль (для сферы нормаль - это просто нормализованный вектор к центру)
                    vertices.Add(x);
                    vertices.Add(y);
                    vertices.Add(z);

                    // Текстурные координаты
                    vertices.Add((float)segment / segments);
                    vertices.Add((float)ring / rings);
                }
            }

            // Создаем индексы для соединения вершин в треугольники
            for (int ring = 0; ring < rings; ring++)
            {
                for (int segment = 0; segment < segments; segment++)
                {
                    uint current = (uint)(ring * (segments + 1) + segment);
                    uint next = (uint)(current + 1);
                    uint bottom = (uint)((ring + 1) * (segments + 1) + segment);
                    uint bottomNext = (uint)(bottom + 1);

                    // Верхний треугольник
                    indices.Add(current);
                    indices.Add(next);
                    indices.Add(bottom);

                    // Нижний треугольник
                    indices.Add(bottom);
                    indices.Add(next);
                    indices.Add(bottomNext);
                }
            }
        }

        public void InitializeBuffers()
        {
            // Создаем и заполняем VBO для вершин и VAO
            GL.GenBuffers(1, out VBO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Count * sizeof(float), vertices.ToArray(), BufferUsageHint.StaticDraw);

            GL.GenVertexArrays(1, out VAO);
            GL.BindVertexArray(VAO);

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);

            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));

            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));

            // Создаем буфер индексов
            GL.GenBuffers(1, out IBO);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Count * sizeof(uint), indices.ToArray(), BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        protected override void Draw(Shader shader)
        {
            // Привязываем VAO (который автоматически привязывает буфер индексов IBO)
            GL.BindVertexArray(VAO);

            // Активируем текстуру
            texture.Use(TextureUnit.Texture0);

            // Используем DrawElements для отрисовки с индексами
            GL.DrawElements(PrimitiveType.Triangles, indices.Count, DrawElementsType.UnsignedInt, 0);

            // Отключаем привязку VAO для безопасности
            GL.BindVertexArray(0);
        }
    }
}