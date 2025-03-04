using OpenTK;
using OpenTK.GLControl;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Computer_Graphics_Programming_Blue_Meteorite
{
    public partial class Form1 : Form
    {
        private GLControl glControl;
        private Camera camera;
        private HashSet<Keys> pressedKeys = new HashSet<Keys>(); // Хранит нажатые клавиши

        private int shaderProgram;
        private int vao, vbo;
        private int modelLocation, viewLocation, projectionLocation;

        private bool isMouseCaptured = false;
        private Point lastMousePosition;
        private float deltaTime = 0.01f; // Временной интервал для движения
        private float mouseSensitivity = 0.1f; // Чувствительность мыши

        public Form1()
        {
            InitializeComponent();
            InitializeGLControl();
        }

        private void InitializeGLControl()
        {
            glControl = new GLControl { Dock = DockStyle.Fill };
            this.Controls.Add(glControl);

            glControl.Load += OnLoad;
            glControl.Paint += OnRender;
            glControl.KeyDown += OnKeyDown;
            glControl.KeyUp += OnKeyUp;
            glControl.MouseMove += OnMouseMove;
            glControl.Focus(); // Захватываем фокус для клавиатуры

            Application.Idle += (_, __) => glControl.Invalidate(); // Вызываем перерисовку постоянно
        }

        private void OnLoad(object sender, EventArgs e)
        {
            GL.ClearColor(Color4.Black);
            GL.Enable(EnableCap.DepthTest);

            camera = new Camera(new Vector3(0.0f, 0.0f, 3.0f));

            shaderProgram = CreateProgram(
                LoadShader("Shaders/vertex_shader.glsl"),
                LoadShader("Shaders/fragment_shader.glsl"));

            float[] vertices =
            {
                -0.5f, -0.5f, -0.5f,
                 0.5f, -0.5f, -0.5f,
                 0.0f,  0.5f, -0.5f
            };

            vao = GL.GenVertexArray();
            vbo = GL.GenBuffer();

            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            modelLocation = GL.GetUniformLocation(shaderProgram, "model");
            viewLocation = GL.GetUniformLocation(shaderProgram, "view");
            projectionLocation = GL.GetUniformLocation(shaderProgram, "projection");

            GL.UseProgram(shaderProgram);
        }

        private void OnRender(object sender, PaintEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            ProcessKeyboardInput();

            Matrix4 model = Matrix4.Identity;
            Matrix4 view = camera.GetViewMatrix();
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), Width / (float)Height, 0.1f, 100.0f);

            GL.UniformMatrix4(modelLocation, false, ref model);
            GL.UniformMatrix4(viewLocation, false, ref view);
            GL.UniformMatrix4(projectionLocation, false, ref projection);

            GL.BindVertexArray(vao);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

            glControl.SwapBuffers();

            UpdateCameraPositionLabels(); // Обновление значений координат камеры
        }

        private void ProcessKeyboardInput()
        {
            Vector3 right = Vector3.Cross(camera.Front, camera.Up).Normalized();

            if (pressedKeys.Contains(Keys.W)) camera.Move(camera.Front, deltaTime);
            if (pressedKeys.Contains(Keys.S)) camera.Move(-camera.Front, deltaTime);
            if (pressedKeys.Contains(Keys.A)) camera.Move(-right, deltaTime);
            if (pressedKeys.Contains(Keys.D)) camera.Move(right, deltaTime);

            if (pressedKeys.Contains(Keys.Space)) camera.Move(camera.Up, deltaTime);
            if (pressedKeys.Contains(Keys.ControlKey)) camera.Move(-camera.Up, deltaTime);
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            pressedKeys.Add(e.KeyCode);
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            pressedKeys.Remove(e.KeyCode);
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!isMouseCaptured)
            {
                lastMousePosition = e.Location;
                isMouseCaptured = true;
                return;
            }

            float offsetX = (e.X - lastMousePosition.X) * mouseSensitivity;
            float offsetY = (lastMousePosition.Y - e.Y) * mouseSensitivity;

            lastMousePosition = e.Location;
            camera.Rotate(offsetX, offsetY);
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

        private void UpdateCameraPositionLabels()
        {
            cameraPositionX.Text = $"x: {camera.Position.X:F2}";
            cameraPositionY.Text = $"y: {camera.Position.Y:F2}";
            cameraPositionZ.Text = $"z: {camera.Position.Z:F2}";
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}