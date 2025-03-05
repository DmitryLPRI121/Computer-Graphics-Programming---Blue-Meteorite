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
        private Camera camera;
        private HashSet<Keys> pressedKeys = new HashSet<Keys>(); // Хранит нажатые клавиши
        private KeyboardController keyboardController;
        private Renderer renderer;

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
            glControl1.Load += OnLoad;
            glControl1.Paint += OnRender;
            glControl1.KeyDown += OnKeyDown;
            glControl1.KeyUp += OnKeyUp;
            glControl1.MouseMove += OnMouseMove;
            glControl1.Focus(); // Захватываем фокус для клавиатуры

            Application.Idle += (_, __) => glControl1.Invalidate(); // Вызываем перерисовку постоянно
        }

        private void OnLoad(object sender, EventArgs e)
        {
            GL.ClearColor(Color4.Black);
            GL.Enable(EnableCap.DepthTest);

            camera = new Camera(new Vector3(0.0f, 0.0f, 3.0f));

            // Инициализация KeyboardController
            keyboardController = new KeyboardController(camera, pressedKeys, deltaTime);

            // Инициализация рендерера
            renderer = new Renderer(camera);
            renderer.InitializeShaders("Shaders/vertex_shader.glsl", "Shaders/fragment_shader.glsl");
            renderer.InitializeBuffers();
        }

        private void OnRender(object sender, PaintEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Обработка ввода с клавиатуры через KeyboardController
            keyboardController.ProcessKeyboardInput();

            // Рендеринг через Renderer
            renderer.Render();

            glControl1.SwapBuffers();

            UpdateCameraPositionLabels(); // Обновление значений координат камеры
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