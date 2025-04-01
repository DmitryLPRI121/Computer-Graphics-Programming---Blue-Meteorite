using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Computer_Graphics_Programming_Blue_Meteorite
{
    public class Prism : TransformableObject
    {
        private float[] vertices;
        private readonly uint[] indices = {
            // Переднее основание
            0, 1, 2,

            // Заднее основание
            3, 5, 4,

            // Нижняя грань
            6, 7, 8,
            6, 8, 9,

            // Правая грань
            10, 11, 12,
            10, 12, 13,

            // Левая грань
            14, 15, 16,
            14, 16, 17,
        };

        private int VBO, VAO, IBO;
        private Texture texture;

        public Prism(string texturePath, float textureRepeat = 1.0f)
        {
            GenerateVertices();
            InitializeBuffers();
            texture = new Texture(texturePath, textureRepeat);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }

        private void GenerateVertices()
        {
            // Создаем матрицу вращения
            Matrix4 rotationMatrix = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(Rotation.X)) *
                                   Matrix4.CreateRotationY(MathHelper.DegreesToRadians(Rotation.Y)) *
                                   Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(Rotation.Z));

            // Базовые вершины без поворота
            Vector3[] baseVertices = {
                // Переднее основание (треугольник)
                new Vector3(-0f, -0.5f, -0.5f),
                new Vector3(1f, -0.5f, -0.5f),
                new Vector3(0.0f, 0.5f, -0.5f),

                // Заднее основание (треугольник)
                new Vector3(-0f, -0.5f, 0.5f),
                new Vector3(1f, -0.5f, 0.5f),
                new Vector3(0.0f, 0.5f, 0.5f),

                // Нижняя грань (прямоугольник)
                new Vector3(-0f, -0.5f, -0.5f),
                new Vector3(1f, -0.5f, -0.5f),
                new Vector3(1f, -0.5f, 0.5f),
                new Vector3(-0f, -0.5f, 0.5f),

                // Правая грань (прямоугольник)
                new Vector3(1f, -0.5f, -0.5f),
                new Vector3(0.0f, 0.5f, -0.5f),
                new Vector3(0.0f, 0.5f, 0.5f),
                new Vector3(1f, -0.5f, 0.5f),

                // Левая грань (прямоугольник)
                new Vector3(0.0f, 0.5f, -0.5f),
                new Vector3(-0f, -0.5f, -0.5f),
                new Vector3(-0f, -0.5f, 0.5f),
                new Vector3(0.0f, 0.5f, 0.5f),
            };

            // Базовые нормали без поворота
            Vector3[] baseNormals = {
                // Переднее основание
                new Vector3(0.0f, 0.0f, -1.0f),
                new Vector3(0.0f, 0.0f, -1.0f),
                new Vector3(0.0f, 0.0f, -1.0f),

                // Заднее основание
                new Vector3(0.0f, 0.0f, 1.0f),
                new Vector3(0.0f, 0.0f, 1.0f),
                new Vector3(0.0f, 0.0f, 1.0f),

                // Нижняя грань
                new Vector3(0.0f, -1.0f, 0.0f),
                new Vector3(0.0f, -1.0f, 0.0f),
                new Vector3(0.0f, -1.0f, 0.0f),
                new Vector3(0.0f, -1.0f, 0.0f),

                // Правая грань
                new Vector3(1.0f, 0.0f, 0.0f),
                new Vector3(1.0f, 0.0f, 0.0f),
                new Vector3(1.0f, 0.0f, 0.0f),
                new Vector3(1.0f, 0.0f, 0.0f),

                // Левая грань
                new Vector3(-1.0f, 0.0f, 0.0f),
                new Vector3(-1.0f, 0.0f, 0.0f),
                new Vector3(-1.0f, 0.0f, 0.0f),
                new Vector3(-1.0f, 0.0f, 0.0f),
            };

            // Текстурные координаты
            float[] texCoords = {
                // Переднее основание
                0.0f, 0.0f,
                1.0f, 0.0f,
                0.5f, 1.0f,

                // Заднее основание
                0.0f, 0.0f,
                1.0f, 0.0f,
                0.5f, 1.0f,

                // Нижняя грань
                0.0f, 0.0f,
                1.0f, 0.0f,
                1.0f, 1.0f,
                0.0f, 1.0f,

                // Правая грань
                0.0f, 0.0f,
                1.0f, 0.0f,
                1.0f, 1.0f,
                0.0f, 1.0f,

                // Левая грань
                0.0f, 0.0f,
                1.0f, 0.0f,
                1.0f, 1.0f,
                0.0f, 1.0f,
            };

            // Создаем массив вершин с учетом поворота
            vertices = new float[baseVertices.Length * 8]; // 8 компонентов на вершину (3 позиции + 3 нормали + 2 текстурные координаты)
            int vertexIndex = 0;

            for (int i = 0; i < baseVertices.Length; i++)
            {
                // Применяем поворот к вершине
                Vector3 rotatedVertex = Vector3.TransformPosition(baseVertices[i], rotationMatrix);
                Vector3 rotatedNormal = Vector3.TransformVector(baseNormals[i], rotationMatrix);

                // Добавляем позицию
                vertices[vertexIndex++] = rotatedVertex.X;
                vertices[vertexIndex++] = rotatedVertex.Y;
                vertices[vertexIndex++] = rotatedVertex.Z;

                // Добавляем нормаль
                vertices[vertexIndex++] = rotatedNormal.X;
                vertices[vertexIndex++] = rotatedNormal.Y;
                vertices[vertexIndex++] = rotatedNormal.Z;

                // Добавляем текстурные координаты
                vertices[vertexIndex++] = texCoords[i * 2];
                vertices[vertexIndex++] = texCoords[i * 2 + 1];
            }
        }

        public void InitializeBuffers()
        {
            GL.GenBuffers(1, out VBO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            GL.GenVertexArrays(1, out VAO);
            GL.BindVertexArray(VAO);

            // Позиции
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);

            // Нормали
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));

            // Текстурные координаты
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));

            GL.GenBuffers(1, out IBO);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        protected override void Draw(Shader shader)
        {
            GL.BindVertexArray(VAO);
            texture.Use(TextureUnit.Texture0);
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
            texture.Detach();
        }
    }
}