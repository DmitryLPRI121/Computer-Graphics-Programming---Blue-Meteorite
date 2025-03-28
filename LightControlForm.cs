using OpenTK.Mathematics;

namespace Computer_Graphics_Programming_Blue_Meteorite
{
    public partial class LightControlForm : Form
    {
        private SceneState sceneState;

        public LightControlForm(SceneState state)
        {
            sceneState = state;
            InitializeComponent();
            InitializeUI();
        }

        private void InitializeComponent()
        {
            this.Text = "Light Control";
            this.Size = new System.Drawing.Size(400, 400);

            // Position controls
            Label positionLabel = new Label
            {
                Text = "Light Position:",
                Location = new System.Drawing.Point(10, 10),
                AutoSize = true
            };

            Label xLabel = new Label { Text = "X:", Location = new System.Drawing.Point(10, 40), AutoSize = true };
            LightXCord = new TextBox { Location = new System.Drawing.Point(30, 40), Size = new System.Drawing.Size(60, 20) };

            Label yLabel = new Label { Text = "Y:", Location = new System.Drawing.Point(100, 40), AutoSize = true };
            LightYCord = new TextBox { Location = new System.Drawing.Point(120, 40), Size = new System.Drawing.Size(60, 20) };

            Label zLabel = new Label { Text = "Z:", Location = new System.Drawing.Point(190, 40), AutoSize = true };
            LightZCord = new TextBox { Location = new System.Drawing.Point(210, 40), Size = new System.Drawing.Size(60, 20) };

            // Intensity controls
            Label intensityLabel = new Label
            {
                Text = "Light Intensity:",
                Location = new System.Drawing.Point(10, 70),
                AutoSize = true
            };

            Label ambientLabel = new Label { Text = "Ambient:", Location = new System.Drawing.Point(10, 100), AutoSize = true };
            ambientTrackBar = new TrackBar
            {
                Location = new System.Drawing.Point(70, 100),
                Size = new System.Drawing.Size(200, 45),
                Minimum = 0,
                Maximum = 500,
                TickFrequency = 50
            };

            Label diffuseLabel = new Label { Text = "Diffuse:", Location = new System.Drawing.Point(10, 150), AutoSize = true };
            diffuseTrackBar = new TrackBar
            {
                Location = new System.Drawing.Point(70, 150),
                Size = new System.Drawing.Size(200, 45),
                Minimum = 0,
                Maximum = 500,
                TickFrequency = 50
            };

            Label specularLabel = new Label { Text = "Specular:", Location = new System.Drawing.Point(10, 200), AutoSize = true };
            specularTrackBar = new TrackBar
            {
                Location = new System.Drawing.Point(70, 200),
                Size = new System.Drawing.Size(200, 45),
                Minimum = 0,
                Maximum = 500,
                TickFrequency = 50
            };

            // Color controls
            Label colorLabel = new Label
            {
                Text = "Light Color:",
                Location = new System.Drawing.Point(10, 250),
                AutoSize = true
            };

            Label rLabel = new Label { Text = "R:", Location = new System.Drawing.Point(10, 280), AutoSize = true };
            lightRComponent = new TrackBar
            {
                Location = new System.Drawing.Point(30, 280),
                Size = new System.Drawing.Size(200, 45),
                Minimum = 0,
                Maximum = 100,
                TickFrequency = 10
            };

            Label gLabel = new Label { Text = "G:", Location = new System.Drawing.Point(10, 320), AutoSize = true };
            lightGComponent = new TrackBar
            {
                Location = new System.Drawing.Point(30, 320),
                Size = new System.Drawing.Size(200, 45),
                Minimum = 0,
                Maximum = 100,
                TickFrequency = 10
            };

            Label bLabel = new Label { Text = "B:", Location = new System.Drawing.Point(10, 360), AutoSize = true };
            lightBComponent = new TrackBar
            {
                Location = new System.Drawing.Point(30, 360),
                Size = new System.Drawing.Size(200, 45),
                Minimum = 0,
                Maximum = 100,
                TickFrequency = 10
            };

            // Add controls to form
            this.Controls.AddRange(new Control[] {
                positionLabel, xLabel, yLabel, zLabel, LightXCord, LightYCord, LightZCord,
                intensityLabel, ambientLabel, diffuseLabel, specularLabel,
                ambientTrackBar, diffuseTrackBar, specularTrackBar,
                colorLabel, rLabel, gLabel, bLabel,
                lightRComponent, lightGComponent, lightBComponent
            });

            // Add event handlers
            LightXCord.TextChanged += LightChanged;
            LightYCord.TextChanged += LightChanged;
            LightZCord.TextChanged += LightChanged;
            ambientTrackBar.ValueChanged += LightChanged;
            diffuseTrackBar.ValueChanged += LightChanged;
            specularTrackBar.ValueChanged += LightChanged;
            lightRComponent.ValueChanged += LightChanged;
            lightGComponent.ValueChanged += LightChanged;
            lightBComponent.ValueChanged += LightChanged;
        }

        private void InitializeUI()
        {
            // Set initial values
            LightXCord.Text = sceneState.LightSettings.Position.X.ToString();
            LightYCord.Text = sceneState.LightSettings.Position.Y.ToString();
            LightZCord.Text = sceneState.LightSettings.Position.Z.ToString();

            ambientTrackBar.Value = (int)(sceneState.LightSettings.AmbientIntensity * 100);
            diffuseTrackBar.Value = (int)(sceneState.LightSettings.DiffuseIntensity * 100);
            specularTrackBar.Value = (int)(sceneState.LightSettings.SpecularIntensity * 100);

            lightRComponent.Value = (int)(sceneState.LightSettings.Color.R * 100);
            lightGComponent.Value = (int)(sceneState.LightSettings.Color.G * 100);
            lightBComponent.Value = (int)(sceneState.LightSettings.Color.B * 100);
        }

        private void LightChanged(object sender, EventArgs e)
        {
            lock (sceneState)
            {
                var textBox = sender as TextBox;
                if (textBox != null && float.TryParse(textBox.Text.Replace('.', ','), out float value))
                {
                    switch (textBox.Name)
                    {
                        case "LightXCord":
                            sceneState.LightSettings.Position = new Vector3(value, sceneState.LightSettings.Position.Y, sceneState.LightSettings.Position.Z);
                            break;
                        case "LightYCord":
                            sceneState.LightSettings.Position = new Vector3(sceneState.LightSettings.Position.X, value, sceneState.LightSettings.Position.Z);
                            break;
                        case "LightZCord":
                            sceneState.LightSettings.Position = new Vector3(sceneState.LightSettings.Position.X, sceneState.LightSettings.Position.Y, value);
                            break;
                    }
                }

                sceneState.LightSettings.AmbientIntensity = ambientTrackBar.Value / 100.0f;
                sceneState.LightSettings.DiffuseIntensity = diffuseTrackBar.Value / 100.0f;
                sceneState.LightSettings.SpecularIntensity = specularTrackBar.Value / 100.0f;

                sceneState.LightSettings.Color = new Color4(
                    lightRComponent.Value / 100.0f,
                    lightGComponent.Value / 100.0f,
                    lightBComponent.Value / 100.0f,
                    1.0f
                );
            }
        }

        private TextBox LightXCord;
        private TextBox LightYCord;
        private TextBox LightZCord;
        private TrackBar ambientTrackBar;
        private TrackBar diffuseTrackBar;
        private TrackBar specularTrackBar;
        private TrackBar lightRComponent;
        private TrackBar lightGComponent;
        private TrackBar lightBComponent;
    }
} 