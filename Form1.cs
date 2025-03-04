using OpenTK;
using OpenTK.GLControl;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.IO;
using System.Windows.Forms;

namespace Computer_Graphics_Programming_Blue_Meteorite
{
    public partial class Form1 : Form
    {
        private GLControl glControl1;

        private int shaderProgram;
        private int vao;
        private int vbo;

        private int modelLocation;
        private int projectionLocation;

        public Form1()
        {
            InitializeComponent();
            InitializeGLControl();
        }

        private void InitializeGLControl()
        {
            glControl1 = new GLControl
            {
                Dock = DockStyle.Fill
            };

            glControl1.Load += GlControl1_Load;
            glControl1.Paint += GlControl1_Paint;
            glControl1.Resize += GlControl1_Resize;

            this.Controls.Add(glControl1);
        }

        private void GlControl1_Load(object sender, EventArgs e)
        {
            GL.ClearColor(Color4.CornflowerBlue); // Фоновый цвет
            GL.Enable(EnableCap.DepthTest); // Включаем тест глубины

            // Пути к файлам шейдеров
            string vertexShaderPath = Path.Combine(Application.StartupPath, "Shaders", "vertex_shader.glsl");
            string fragmentShaderPath = Path.Combine(Application.StartupPath, "Shaders", "fragment_shader.glsl");

            // Загрузка кода шейдеров
            string vertexShaderSource = LoadShaderSource(vertexShaderPath);
            string fragmentShaderSource = LoadShaderSource(fragmentShaderPath);

            // Компиляция шейдеров
            int vertexShader = CreateShader(ShaderType.VertexShader, vertexShaderSource);
            int fragmentShader = CreateShader(ShaderType.FragmentShader, fragmentShaderSource);
            shaderProgram = CreateProgram(vertexShader, fragmentShader);

            // Вершины треугольника
            float[] vertices = new float[]
            {
                -0.6f, -0.6f, 0.0f,
                 0.6f, -0.6f, 0.0f,
                 0.0f,  0.6f, 0.0f
            };

            // Создание VAO и VBO
            vao = GL.GenVertexArray();
            vbo = GL.GenBuffer();

            GL.BindVertexArray(vao);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);

            // Получение местоположений uniform-переменных
            modelLocation = GL.GetUniformLocation(shaderProgram, "model");
            projectionLocation = GL.GetUniformLocation(shaderProgram, "projection");
        }

        private string LoadShaderSource(string filePath)
        {
            try
            {
                return File.ReadAllText(filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки шейдера:\n{ex.Message}");
                return string.Empty;
            }
        }

        private int CreateShader(ShaderType type, string source)
        {
            int shader = GL.CreateShader(type);
            GL.ShaderSource(shader, source);
            GL.CompileShader(shader);

            GL.GetShader(shader, ShaderParameter.CompileStatus, out int status);
            if (status == 0)
            {
                string infoLog = GL.GetShaderInfoLog(shader);
                MessageBox.Show($"Ошибка компиляции {type} шейдера:\n{infoLog}");
            }

            return shader;
        }

        private int CreateProgram(int vertexShader, int fragmentShader)
        {
            int program = GL.CreateProgram();
            GL.AttachShader(program, vertexShader);
            GL.AttachShader(program, fragmentShader);
            GL.LinkProgram(program);

            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out int status);
            if (status == 0)
            {
                string infoLog = GL.GetProgramInfoLog(program);
                MessageBox.Show($"Ошибка линковки шейдерной программы:\n{infoLog}");
            }

            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);

            return program;
        }

        private void GlControl1_Paint(object sender, PaintEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.UseProgram(shaderProgram);

            // Создание матрицы модели и проекции
            Matrix4 model = Matrix4.CreateRotationZ((float)DateTime.Now.TimeOfDay.TotalSeconds);
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(45),
                glControl1.Width / (float)glControl1.Height,
                0.1f,
                100f
            );

            // Передача матриц в шейдер
            GL.UniformMatrix4(modelLocation, false, ref model);
            GL.UniformMatrix4(projectionLocation, false, ref projection);

            GL.BindVertexArray(vao);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

            glControl1.SwapBuffers();
        }

        private void GlControl1_Resize(object sender, EventArgs e)
        {
            GL.Viewport(0, 0, glControl1.Width, glControl1.Height);
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}