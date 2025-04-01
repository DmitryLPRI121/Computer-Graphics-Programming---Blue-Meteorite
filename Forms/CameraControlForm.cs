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
            this.ClientSize = new Size(300, 200);
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
                Text = "Jump Speed:",
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

            this.Controls.AddRange(new Control[] {
                movementSpeedLabel, movementSpeedInput,
                jumpRiseSpeedLabel, jumpRiseSpeedInput,
                gravityLabel, gravityInput,
                collisionRadiusLabel, collisionRadiusInput
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
            }

            if (sceneState != null)
            {
                gravityInput.Value = 35;
            }
        }

        private void UpdateCameraParameters()
        {
            if (camera != null)
            {
                // Movement speed affects only horizontal movement
                camera.MovementSpeed = (float)movementSpeedInput.Value;
                // Collision radius
                camera.CollisionRadius = (float)collisionRadiusInput.Value;
            }

            if (cameraDynamic != null)
            {
                // Jump rise speed
                cameraDynamic.JumpRiseSpeed = (float)jumpRiseSpeedInput.Value;
            }

            if (sceneState != null)
            {
                // Gravity affects falling speed
                sceneState.GravityStrength = (float)gravityInput.Value;
            }
        }
    }
} 