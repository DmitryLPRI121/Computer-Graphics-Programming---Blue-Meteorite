using OpenTK.Mathematics;

namespace Computer_Graphics_Programming_Blue_Meteorite
{
    public partial class LightControlForm : Form
    {
        private SceneObjects sceneState;
        private bool isInitializing = false;
        private ComboBox lightSelector;
        private Button addLightBtn;
        private Button removeLightBtn;
        private Button setPositionBtn;

        public LightControlForm(SceneObjects state)
        {
            sceneState = state;
            InitializeComponent();
            InitializeUI();
        }

        private void InitializeComponent()
        {
            this.Text = "Light Control";
            this.Size = new System.Drawing.Size(400, 500);

            // Light selection controls
            Label selectLabel = new Label
            {
                Text = "Select Light:",
                Location = new System.Drawing.Point(10, 10),
                AutoSize = true
            };

            lightSelector = new ComboBox
            {
                Location = new System.Drawing.Point(10, 35),
                Size = new System.Drawing.Size(200, 20),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            addLightBtn = new Button
            {
                Text = "Add Light",
                Location = new System.Drawing.Point(220, 35),
                Size = new System.Drawing.Size(80, 20)
            };

            removeLightBtn = new Button
            {
                Text = "Remove Light",
                Location = new System.Drawing.Point(310, 35),
                Size = new System.Drawing.Size(80, 20)
            };

            // Position controls
            Label positionLabel = new Label
            {
                Text = "Light Position:",
                Location = new System.Drawing.Point(10, 65),
                AutoSize = true
            };

            Label xLabel = new Label { Text = "X:", Location = new System.Drawing.Point(10, 95), AutoSize = true };
            LightXCord = new TextBox { Location = new System.Drawing.Point(30, 95), Size = new System.Drawing.Size(60, 20) };

            Label yLabel = new Label { Text = "Y:", Location = new System.Drawing.Point(100, 95), AutoSize = true };
            LightYCord = new TextBox { Location = new System.Drawing.Point(120, 95), Size = new System.Drawing.Size(60, 20) };

            Label zLabel = new Label { Text = "Z:", Location = new System.Drawing.Point(190, 95), AutoSize = true };
            LightZCord = new TextBox { Location = new System.Drawing.Point(210, 95), Size = new System.Drawing.Size(60, 20) };

            setPositionBtn = new Button
            {
                Text = "Set Position",
                Location = new System.Drawing.Point(280, 95),
                Size = new System.Drawing.Size(110, 20)
            };

            // Intensity controls
            Label intensityLabel = new Label
            {
                Text = "Light Intensity:",
                Location = new System.Drawing.Point(10, 125),
                AutoSize = true
            };

            Label ambientLabel = new Label { Text = "Ambient:", Location = new System.Drawing.Point(10, 155), AutoSize = true };
            ambientTrackBar = new TrackBar
            {
                Location = new System.Drawing.Point(70, 155),
                Size = new System.Drawing.Size(200, 45),
                Minimum = 0,
                Maximum = 1000,
                TickFrequency = 100
            };

            Label diffuseLabel = new Label { Text = "Diffuse:", Location = new System.Drawing.Point(10, 205), AutoSize = true };
            diffuseTrackBar = new TrackBar
            {
                Location = new System.Drawing.Point(70, 205),
                Size = new System.Drawing.Size(200, 45),
                Minimum = 0,
                Maximum = 1000,
                TickFrequency = 100
            };

            Label specularLabel = new Label { Text = "Specular:", Location = new System.Drawing.Point(10, 255), AutoSize = true };
            specularTrackBar = new TrackBar
            {
                Location = new System.Drawing.Point(70, 255),
                Size = new System.Drawing.Size(200, 45),
                Minimum = 0,
                Maximum = 1000,
                TickFrequency = 100
            };

            // Color controls
            Label colorLabel = new Label
            {
                Text = "Light Color:",
                Location = new System.Drawing.Point(10, 305),
                AutoSize = true
            };

            Label rLabel = new Label { Text = "R:", Location = new System.Drawing.Point(10, 335), AutoSize = true };
            lightRComponent = new TrackBar
            {
                Location = new System.Drawing.Point(30, 335),
                Size = new System.Drawing.Size(200, 45),
                Minimum = 0,
                Maximum = 100,
                TickFrequency = 10
            };

            Label gLabel = new Label { Text = "G:", Location = new System.Drawing.Point(10, 375), AutoSize = true };
            lightGComponent = new TrackBar
            {
                Location = new System.Drawing.Point(30, 375),
                Size = new System.Drawing.Size(200, 45),
                Minimum = 0,
                Maximum = 100,
                TickFrequency = 10
            };

            Label bLabel = new Label { Text = "B:", Location = new System.Drawing.Point(10, 415), AutoSize = true };
            lightBComponent = new TrackBar
            {
                Location = new System.Drawing.Point(30, 415),
                Size = new System.Drawing.Size(200, 45),
                Minimum = 0,
                Maximum = 100,
                TickFrequency = 10
            };

            // Add controls to form
            this.Controls.AddRange(new Control[] {
                selectLabel, lightSelector, addLightBtn, removeLightBtn,
                positionLabel, xLabel, yLabel, zLabel, LightXCord, LightYCord, LightZCord, setPositionBtn,
                intensityLabel, ambientLabel, diffuseLabel, specularLabel,
                ambientTrackBar, diffuseTrackBar, specularTrackBar,
                colorLabel, rLabel, gLabel, bLabel,
                lightRComponent, lightGComponent, lightBComponent
            });

            // Add event handlers
            lightSelector.SelectedIndexChanged += LightSelector_SelectedIndexChanged;
            addLightBtn.Click += AddLightBtn_Click;
            removeLightBtn.Click += RemoveLightBtn_Click;
            setPositionBtn.Click += SetPositionBtn_Click;
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
            isInitializing = true;
            UpdateLightSelector();
            if (sceneState.LightSettings.Count > 0)
            {
                lightSelector.SelectedIndex = 0;
                UpdateControlsForSelectedLight();
            }
            isInitializing = false;
        }

        private void UpdateLightSelector()
        {
            lightSelector.Items.Clear();
            lock (sceneState)
            {
                for (int i = 0; i < sceneState.LightSettings.Count; i++)
                {
                    lightSelector.Items.Add($"Light {i + 1}");
                }
            }
        }

        private void UpdateControlsForSelectedLight()
        {
            lock (sceneState)
            {
                if (lightSelector.SelectedIndex < 0 || lightSelector.SelectedIndex >= sceneState.LightSettings.Count)
                    return;

                var light = sceneState.LightSettings[lightSelector.SelectedIndex];
                LightXCord.Text = light.Position.X.ToString();
                LightYCord.Text = light.Position.Y.ToString();
                LightZCord.Text = light.Position.Z.ToString();

                ambientTrackBar.Value = (int)(light.AmbientIntensity * 100);
                diffuseTrackBar.Value = (int)(light.DiffuseIntensity * 100);
                specularTrackBar.Value = (int)(light.SpecularIntensity * 100);

                lightRComponent.Value = (int)(light.Color.R * 100);
                lightGComponent.Value = (int)(light.Color.G * 100);
                lightBComponent.Value = (int)(light.Color.B * 100);
            }
        }

        private void LightSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isInitializing) return;
            UpdateControlsForSelectedLight();
        }

        private void AddLightBtn_Click(object sender, EventArgs e)
        {
            lock (sceneState)
            {
                // Создаем новый источник света с позицией (0,0,0)
                sceneState.LightSettings.Add(new LightSettings()
                {
                    Position = new Vector3(0, 0, 0),
                    LookAt = new Vector3(0, 0, 0),
                    AmbientIntensity = 2.0f,
                    DiffuseIntensity = 2.0f,
                    SpecularIntensity = 1.0f,
                    AttenuationA = 1.0f,
                    AttenuationB = 0.09f,
                    AttenuationC = 0.032f,
                    Color = new Color4(1.0f, 1.0f, 1.0f, 1.0f)
                });
            }
            UpdateLightSelector();
            lightSelector.SelectedIndex = sceneState.LightSettings.Count - 1;
        }

        private void RemoveLightBtn_Click(object sender, EventArgs e)
        {
            if (lightSelector.SelectedIndex >= 0)
            {
                int selectedIndex = lightSelector.SelectedIndex;
                
                // Удаляем источник света из коллекции
                lock (sceneState)
                {
                    sceneState.LightSettings.RemoveAt(selectedIndex);
                    
                    // Не будем автоматически добавлять источник света, если удалены все
                    // Позволим удалить все источники света
                }
                
                // Обновляем интерфейс
                UpdateLightSelector();
                
                // Выбираем новый индекс, если есть источники света
                if (lightSelector.Items.Count > 0)
                {
                    // Если был удален не последний элемент, выбираем элемент с тем же индексом
                    // Иначе выбираем последний элемент
                    lightSelector.SelectedIndex = Math.Min(selectedIndex, lightSelector.Items.Count - 1);
                }
            }
        }

        private void SetPositionBtn_Click(object sender, EventArgs e)
        {
            lock (sceneState)
            {
                if (lightSelector.SelectedIndex < 0 || lightSelector.SelectedIndex >= sceneState.LightSettings.Count)
                    return;

                if (float.TryParse(LightXCord.Text.Replace('.', ','), out float x) &&
                    float.TryParse(LightYCord.Text.Replace('.', ','), out float y) &&
                    float.TryParse(LightZCord.Text.Replace('.', ','), out float z))
                {
                    sceneState.LightSettings[lightSelector.SelectedIndex].Position = new Vector3(x, y, z);
                }
            }
        }

        private void LightChanged(object sender, EventArgs e)
        {
            if (isInitializing || lightSelector.SelectedIndex < 0)
                return;
            
            lock (sceneState)
            {
                if (lightSelector.SelectedIndex >= sceneState.LightSettings.Count)
                    return;
                
                var light = sceneState.LightSettings[lightSelector.SelectedIndex];
                var textBox = sender as TextBox;
                if (textBox != null && float.TryParse(textBox.Text.Replace('.', ','), out float value))
                {
                    switch (textBox.Name)
                    {
                        case "LightXCord":
                            light.Position = new Vector3(value, light.Position.Y, light.Position.Z);
                            break;
                        case "LightYCord":
                            light.Position = new Vector3(light.Position.X, value, light.Position.Z);
                            break;
                        case "LightZCord":
                            light.Position = new Vector3(light.Position.X, light.Position.Y, value);
                            break;
                    }
                }

                light.AmbientIntensity = ambientTrackBar.Value / 100.0f;
                light.DiffuseIntensity = diffuseTrackBar.Value / 100.0f;
                light.SpecularIntensity = specularTrackBar.Value / 100.0f;

                light.Color = new Color4(
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