using OpenTK.Graphics.OpenGL4;

namespace Computer_Graphics_Programming_Blue_Meteorite
{
    public class Cube : TransformableObject
    {
        private readonly float[] vertices = {
            // Передняя грань (0.0, 0.5) → (0.33, 1.0)
            -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f * -1.0f, 0.5f * -1.0f,
             0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.33f * -1.0f, 0.5f * -1.0f,
             0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.33f * -1.0f, 1.0f * -1.0f,
            -0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f * -1.0f, 1.0f * -1.0f,

            // Задняя грань (0.33, 0.5) → (0.66, 1.0)
            -0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  0.33f * -1.0f, 0.5f * -1.0f,
             0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  0.66f * -1.0f, 0.5f * -1.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  0.66f * -1.0f, 1.0f * -1.0f,
            -0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  0.33f * -1.0f, 1.0f * -1.0f,

            // Левая грань (0.66, 0.5) → (1.0, 1.0)
            -0.5f, -0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  0.66f * -1.0f, 0.5f * -1.0f,
            -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  1.0f * -1.0f, 0.5f * -1.0f,
            -0.5f,  0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  1.0f * -1.0f, 1.0f * -1.0f,
            -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  0.66f * -1.0f, 1.0f * -1.0f,

            // Правая грань (0.0, 0.0) → (0.33, 0.5)
             0.5f, -0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  0.0f * -1.0f, 0.0f * -1.0f,
             0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  0.33f * -1.0f, 0.0f * -1.0f,
             0.5f,  0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  0.33f * -1.0f, 0.5f * -1.0f,
             0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  0.0f * -1.0f, 0.5f * -1.0f,

            // Верхняя грань (0.33, 0.0) → (0.66, 0.5)
            -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  0.33f * -1.0f, 0.0f * -1.0f,
             0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  0.66f * -1.0f, 0.0f * -1.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  0.66f * -1.0f, 0.5f * -1.0f,
            -0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  0.33f * -1.0f, 0.5f * -1.0f,

            // Нижняя грань (0.66, 0.0) → (1.0, 0.5)
            -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  0.66f * -1.0f, 0.0f * -1.0f,
             0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  1.0f * -1.0f, 0.0f * -1.0f,
             0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  1.0f * -1.0f, 0.5f * -1.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  0.66f * -1.0f, 0.5f * -1.0f,
        };

        private readonly uint[] indices = {
        // Передняя грань
        0, 1, 2,
        0, 2, 3,
    
        // Задняя грань
        4, 6, 5,
        4, 7, 6,

        // Левая грань
        8, 9, 10,
        8, 10, 11,

        // Правая грань
        12, 14, 13,
        12, 15, 14,

        // Верхняя грань
        16, 17, 18,
        16, 18, 19,

        // Нижняя грань
        20, 21, 22,
        20, 22, 23,
    };

        private int VBO, VAO, IBO;
        private Texture texture;

        public Cube(string texturePath)
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
            // Активируем текстуру и рендерим куб
            texture.Use(TextureUnit.Texture0);
            // Используем DrawElements для отрисовки с индексами
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);

            // Отключаем привязку VAO
            GL.BindVertexArray(0);
            // Открепляем texture
            texture.Detach();
        }
    }
}