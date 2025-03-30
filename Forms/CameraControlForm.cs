using OpenTK.Mathematics;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Computer_Graphics_Programming_Blue_Meteorite
{
    public class CameraControlForm : Form
    {
        private Camera camera;
        private DynamicBody cameraDynamic;
        private SceneObjects sceneState;

        private NumericUpDown movementSpeedInput;
        private NumericUpDown jumpRiseSpeedInput;
        private NumericUpDown gravityInput;
        private NumericUpDown collisionRadiusInput;
        private NumericUpDown horizontalVelocityTransferInput;
        private NumericUpDown verticalVelocityTransferInput;

        public CameraControlForm(Camera camera, DynamicBody cameraDynamic, SceneObjects sceneState)
        {
            this.camera = camera;
            this.cameraDynamic = cameraDynamic;
            this.sceneState = sceneState;
            InitializeComponent();
            UpdateInputs();
        }

        private void InitializeComponent()
        {
            this.Text = "Camera Control";
            this.ClientSize = new Size(300, 250); // Увеличим размер формы
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;

            // Movement Speed
            Label movementSpeedLabel = new Label
            {
                Text = "Movement Speed:",
                Location = new Point(10, 20),
                Size = new Size(150, 20)
            };

            movementSpeedInput = new NumericUpDown
            {
                Location = new Point(160, 20),
                Size = new Size(100, 20),
                Minimum = 1,
                Maximum = 100,
                DecimalPlaces = 1,
                Increment = 0.5m,
                Value = 20
            };
            movementSpeedInput.ValueChanged += (s, e) => UpdateCameraParameters();

            // Jump Rise Speed
            Label jumpRiseSpeedLabel = new Label
            {
                Text = "Jump Rise Speed:",
                Location = new Point(10, 50),
                Size = new Size(150, 20)
            };

            jumpRiseSpeedInput = new NumericUpDown
            {
                Location = new Point(160, 50),
                Size = new Size(100, 20),
                Minimum = 1,
                Maximum = 50,
                DecimalPlaces = 1,
                Increment = 0.5m,
                Value = 15
            };
            jumpRiseSpeedInput.ValueChanged += (s, e) => UpdateCameraParameters();

            // Gravity
            Label gravityLabel = new Label
            {
                Text = "Gravity Strength:",
                Location = new Point(10, 80),
                Size = new Size(150, 20)
            };

            gravityInput = new NumericUpDown
            {
                Location = new Point(160, 80),
                Size = new Size(100, 20),
                Minimum = 1,
                Maximum = 100,
                DecimalPlaces = 1,
                Increment = 0.5m,
                Value = 9.8m
            };
            gravityInput.ValueChanged += (s, e) => UpdateCameraParameters();
            
            // Collision Radius
            Label collisionRadiusLabel = new Label
            {
                Text = "Collision Radius:",
                Location = new Point(10, 110),
                Size = new Size(150, 20)
            };

            collisionRadiusInput = new NumericUpDown
            {
                Location = new Point(160, 110),
                Size = new Size(100, 20),
                Minimum = 0.1m,
                Maximum = 5,
                DecimalPlaces = 2,
                Increment = 0.1m,
                Value = 1.0m
            };
            collisionRadiusInput.ValueChanged += (s, e) => UpdateCameraParameters();
            
            // Horizontal Velocity Transfer
            Label horizontalVelocityLabel = new Label
            {
                Text = "Horizontal Transfer:",
                Location = new Point(10, 140),
                Size = new Size(150, 20)
            };

            horizontalVelocityTransferInput = new NumericUpDown
            {
                Location = new Point(160, 140),
                Size = new Size(100, 20),
                Minimum = 0m,
                Maximum = 2m,
                DecimalPlaces = 2,
                Increment = 0.1m,
                Value = 0.7m
            };
            horizontalVelocityTransferInput.ValueChanged += (s, e) => UpdateCameraParameters();
            
            // Vertical Velocity Transfer
            Label verticalVelocityLabel = new Label
            {
                Text = "Vertical Transfer:",
                Location = new Point(10, 170),
                Size = new Size(150, 20)
            };

            verticalVelocityTransferInput = new NumericUpDown
            {
                Location = new Point(160, 170),
                Size = new Size(100, 20),
                Minimum = 0m,
                Maximum = 2m,
                DecimalPlaces = 2,
                Increment = 0.1m,
                Value = 0.5m
            };
            verticalVelocityTransferInput.ValueChanged += (s, e) => UpdateCameraParameters();

            this.Controls.AddRange(new Control[] {
                movementSpeedLabel, movementSpeedInput,
                jumpRiseSpeedLabel, jumpRiseSpeedInput,
                gravityLabel, gravityInput,
                collisionRadiusLabel, collisionRadiusInput,
                horizontalVelocityLabel, horizontalVelocityTransferInput,
                verticalVelocityLabel, verticalVelocityTransferInput
            });
        }

        private void UpdateInputs()
        {
            if (camera != null)
            {
                movementSpeedInput.Value = (decimal)camera.MovementSpeed;
                collisionRadiusInput.Value = (decimal)camera.CollisionRadius;
            }

            if (cameraDynamic != null)
            {
                jumpRiseSpeedInput.Value = (decimal)cameraDynamic.JumpRiseSpeed;
                horizontalVelocityTransferInput.Value = (decimal)cameraDynamic.HorizontalVelocityTransferFactor;
                verticalVelocityTransferInput.Value = (decimal)cameraDynamic.VerticalVelocityTransferFactor;
            }

            if (sceneState != null)
            {
                gravityInput.Value = (decimal)sceneState.GravityStrength;
            }
        }

        private void UpdateCameraParameters()
        {
            if (camera != null)
            {
                // Скорость передвижения влияет только на горизонтальное движение
                camera.MovementSpeed = (float)movementSpeedInput.Value;
                // Радиус коллизии
                camera.CollisionRadius = (float)collisionRadiusInput.Value;
            }

            if (cameraDynamic != null)
            {
                // Скорость взлета при прыжке
                cameraDynamic.JumpRiseSpeed = (float)jumpRiseSpeedInput.Value;
                // Коэффициенты передачи скорости при прыжке с динамических объектов
                cameraDynamic.HorizontalVelocityTransferFactor = (float)horizontalVelocityTransferInput.Value;
                cameraDynamic.VerticalVelocityTransferFactor = (float)verticalVelocityTransferInput.Value;
            }

            if (sceneState != null)
            {
                // Гравитация влияет на скорость падения
                sceneState.GravityStrength = (float)gravityInput.Value;
            }
        }
    }
} 