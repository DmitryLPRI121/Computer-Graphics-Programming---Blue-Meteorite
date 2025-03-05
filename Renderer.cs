using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;

namespace Computer_Graphics_Programming_Blue_Meteorite
{
    public class Renderer
    {
        private Camera camera;
        private int shaderProgram;
        private int vao, vbo, ebo;
        private int modelLocation, viewLocation, projectionLocation;
        private int indexCount;

        public Renderer(Camera camera)
        {
            this.camera = camera;
        }

        public void InitializeShaders(string vertexShaderPath, string fragmentShaderPath)
        {
            shaderProgram = CreateProgram(
                LoadShader(vertexShaderPath),
                LoadShader(fragmentShaderPath));
        }

        public void InitializeBuffers()
        {
            List<float> vertices;
            List<int> indices;
            GenerateSphere(1.0f, 36, 18, out vertices, out indices);

            indexCount = indices.Count;

            vao = GL.GenVertexArray();
            vbo = GL.GenBuffer();
            ebo = GL.GenBuffer();

            GL.BindVertexArray(vao);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Count * sizeof(float), vertices.ToArray(), BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Count * sizeof(int), indices.ToArray(), BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            modelLocation = GL.GetUniformLocation(shaderProgram, "model");
            viewLocation = GL.GetUniformLocation(shaderProgram, "view");
            projectionLocation = GL.GetUniformLocation(shaderProgram, "projection");
        }

        public void Render()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.UseProgram(shaderProgram);

            Matrix4 model = Matrix4.Identity;
            Matrix4 view = camera.GetViewMatrix();
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), 800f / 600f, 0.1f, 100.0f);

            GL.UniformMatrix4(modelLocation, false, ref model);
            GL.UniformMatrix4(viewLocation, false, ref view);
            GL.UniformMatrix4(projectionLocation, false, ref projection);

            GL.BindVertexArray(vao);
            GL.DrawElements(PrimitiveType.Triangles, indexCount, DrawElementsType.UnsignedInt, 0);
        }

        private void GenerateSphere(float radius, int sectorCount, int stackCount, out List<float> vertices, out List<int> indices)
        {
            vertices = new List<float>();
            indices = new List<int>();

            for (int i = 0; i <= stackCount; i++)
            {
                float stackAngle = MathHelper.PiOver2 - i * MathHelper.Pi / stackCount;
                float xy = radius * MathF.Cos(stackAngle);
                float z = radius * MathF.Sin(stackAngle);

                for (int j = 0; j <= sectorCount; j++)
                {
                    float sectorAngle = j * MathHelper.TwoPi / sectorCount;
                    float x = xy * MathF.Cos(sectorAngle);
                    float y = xy * MathF.Sin(sectorAngle);

                    vertices.Add(x);
                    vertices.Add(y);
                    vertices.Add(z);

                    vertices.Add(x / radius);
                    vertices.Add(y / radius);
                    vertices.Add(z / radius);
                }
            }

            for (int i = 0; i < stackCount; i++)
            {
                int k1 = i * (sectorCount + 1);
                int k2 = k1 + sectorCount + 1;

                for (int j = 0; j < sectorCount; j++, k1++, k2++)
                {
                    if (i != 0) indices.AddRange(new int[] { k1, k2, k1 + 1 });
                    if (i != (stackCount - 1)) indices.AddRange(new int[] { k1 + 1, k2, k2 + 1 });
                }
            }
        }

        private string LoadShader(string path) => File.ReadAllText(path);

        private int CreateProgram(string vertexShader, string fragmentShader)
        {
            int vertex = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertex, vertexShader);
            GL.CompileShader(vertex);

            int fragment = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragment, fragmentShader);
            GL.CompileShader(fragment);

            int program = GL.CreateProgram();
            GL.AttachShader(program, vertex);
            GL.AttachShader(program, fragment);
            GL.LinkProgram(program);

            return program;
        }
    }
}