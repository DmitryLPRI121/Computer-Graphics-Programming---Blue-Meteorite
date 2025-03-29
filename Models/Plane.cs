using OpenTK.Graphics.OpenGL4;

namespace Computer_Graphics_Programming_Blue_Meteorite
{
    public class Plane : TransformableObject
    {
        private readonly float[] vertices = {
        // Позиции           // Нормали         // Текстурные координаты
        // Плоскость (квадрат в плоскости XZ)
        -0.5f, 0.0f, -0.5f,  0.0f, 1.0f, 0.0f,  0.0f, 0.0f,
         0.5f, 0.0f, -0.5f,  0.0f, 1.0f, 0.0f,  1.0f, 0.0f,
         0.5f, 0.0f,  0.5f,  0.0f, 1.0f, 0.0f,  1.0f, 1.0f,
        -0.5f, 0.0f,  0.5f,  0.0f, 1.0f, 0.0f,  0.0f, 1.0f
    };

        private readonly uint[] indices = {
        0, 1, 2,
        0, 2, 3
    };

        private int VBO, VAO, IBO;
        private Texture texture;

        public Plane(string texturePath)
        {
            InitializeBuffers();
            texture = new Texture(texturePath);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }

        public void InitializeBuffers()
        {
            // Создаем и заполняем VBO для вершин и VAO
            GL.GenBuffers(1, out VBO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

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
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        protected override void Draw(Shader shader)
        {
            // Привязываем VAO (который автоматически привязывает буфер индексов IBO)
            GL.BindVertexArray(VAO);

            // Активируем текстуру
            texture.Use(TextureUnit.Texture0);

            // Используем DrawElements для отрисовки с индексами
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);

            // Отключаем привязку VAO для безопасности
            GL.BindVertexArray(0);
        }
    }
}