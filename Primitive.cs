using OpenTK.Graphics.OpenGL4;

namespace Computer_Graphics_Programming_Blue_Meteorite
{
    public abstract class Primitive : TransformableObject
    {
        protected int VBO, VAO, IBO;
        protected Texture texture;
        protected float[] vertices;
        protected uint[] indices;

        protected Primitive(string texturePath)
        {
            texture = new Texture(texturePath);
            InitializeGeometry();
            InitializeBuffers();
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }

        protected abstract void InitializeGeometry();

        protected virtual void InitializeBuffers()
        {
            if (vertices == null || indices == null)
            {
                throw new InvalidOperationException("Geometry must be initialized before creating buffers");
            }

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
            if (VAO == 0 || VBO == 0 || IBO == 0)
            {
                throw new InvalidOperationException("Buffers must be initialized before drawing");
            }

            GL.BindVertexArray(VAO);
            texture.Use(TextureUnit.Texture0);
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
            texture.Detach();
        }
    }
} 