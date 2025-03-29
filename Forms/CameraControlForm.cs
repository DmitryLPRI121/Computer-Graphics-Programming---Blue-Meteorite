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
        private NumericUpDown jumpHeightInput;
        private NumericUpDown jumpRiseSpeedInput;
        private NumericUpDown fallSpeedInput;
        private NumericUpDown gravityInput;

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
            this.Size = new Size(300, 400);

            // Movement Speed Control
            Label movementSpeedLabel = new Label
            {
                Text = "Movement Speed:",
                Location = new Point(10, 20),
                AutoSize = true
            };

            movementSpeedInput = new NumericUpDown
            {
                Location = new Point(10, 50),
                Size = new Size(120, 20),
                DecimalPlaces = 2,
                Minimum = 0.1m,
                Maximum = 100m,
                Increment = 0.1m,
                Value = 10m
            };

            // Jump Height Control
            Label jumpHeightLabel = new Label
            {
                Text = "Jump Height:",
                Location = new Point(10, 90),
                AutoSize = true
            };

            jumpHeightInput = new NumericUpDown
            {
                Location = new Point(10, 120),
                Size = new Size(120, 20),
                DecimalPlaces = 2,
                Minimum = 0.1m,
                Maximum = 100m,
                Increment = 0.1m,
                Value = 5m
            };

            // Jump Rise Speed Control
            Label jumpRiseSpeedLabel = new Label
            {
                Text = "Jump Rise Speed:",
                Location = new Point(10, 160),
                AutoSize = true
            };

            jumpRiseSpeedInput = new NumericUpDown
            {
                Location = new Point(10, 190),
                Size = new Size(120, 20),
                DecimalPlaces = 2,
                Minimum = 0.1m,
                Maximum = 100m,
                Increment = 0.1m,
                Value = 5m
            };

            // Fall Speed Control
            Label fallSpeedLabel = new Label
            {
                Text = "Fall Speed:",
                Location = new Point(10, 230),
                AutoSize = true
            };

            fallSpeedInput = new NumericUpDown
            {
                Location = new Point(10, 260),
                Size = new Size(120, 20),
                DecimalPlaces = 2,
                Minimum = 0.1m,
                Maximum = 100m,
                Increment = 0.1m,
                Value = 9.8m
            };

            // Gravity Control
            Label gravityLabel = new Label
            {
                Text = "Gravity:",
                Location = new Point(10, 300),
                AutoSize = true
            };

            gravityInput = new NumericUpDown
            {
                Location = new Point(10, 330),
                Size = new Size(120, 20),
                DecimalPlaces = 2,
                Minimum = 0.1m,
                Maximum = 100m,
                Increment = 0.1m,
                Value = 9.8m
            };

            // Add value changed handlers
            movementSpeedInput.ValueChanged += (s, e) => UpdateCameraParameters();
            jumpHeightInput.ValueChanged += (s, e) => UpdateCameraParameters();
            jumpRiseSpeedInput.ValueChanged += (s, e) => UpdateCameraParameters();
            fallSpeedInput.ValueChanged += (s, e) => UpdateCameraParameters();
            gravityInput.ValueChanged += (s, e) => UpdateCameraParameters();

            // Add controls to form
            this.Controls.AddRange(new Control[] {
                movementSpeedLabel, movementSpeedInput,
                jumpHeightLabel, jumpHeightInput,
                jumpRiseSpeedLabel, jumpRiseSpeedInput,
                fallSpeedLabel, fallSpeedInput,
                gravityLabel, gravityInput
            });
        }

        private void UpdateInputs()
        {
            if (camera != null)
            {
                movementSpeedInput.Value = (decimal)camera.MovementSpeed;
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
                camera.MovementSpeed = (float)movementSpeedInput.Value;
            }

            if (cameraDynamic != null)
            {
                // Update jump parameters
                cameraDynamic.JumpForce = (float)jumpHeightInput.Value;
                cameraDynamic.JumpRiseSpeed = (float)jumpRiseSpeedInput.Value;
                cameraDynamic.FallSpeed = (float)fallSpeedInput.Value;
            }

            if (sceneState != null)
            {
                sceneState.GravityStrength = (float)gravityInput.Value;
            }
        }
    }
} 