using OpenTK.Mathematics;

namespace Computer_Graphics_Programming_Blue_Meteorite
{
    public partial class ParticleSystemControlForm : Form
    {
        private SceneObjects sceneState;
        private ParticleSystem selectedParticleSystem;
        private TableLayoutPanel mainPanel;
        private TableLayoutPanel parametersPanel;
        private ComboBox particleSystemsBox;
        private ComboBox effectTypeComboBox;
        private ComboBox shapeComboBox;
        private Button emitButton;
        private Button stopButton;
        private CheckBox cyclicCheckBox;
        private NumericUpDown minSizeNumeric;
        private NumericUpDown maxSizeNumeric;
        private NumericUpDown spreadNumeric;
        private NumericUpDown speedNumeric;
        private NumericUpDown lifeMinNumeric;
        private NumericUpDown lifeMaxNumeric;
        private NumericUpDown colorVariationNumeric;
        private Button colorButton;

        public ParticleSystemControlForm(SceneObjects state)
        {
            sceneState = state;
            InitializeComponent();
            InitializeUI();
        }

        private void InitializeComponent()
        {
            this.Text = "Particle System Control";
            this.Size = new System.Drawing.Size(500, 600);

            // Main layout
            mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Padding = new Padding(10)
            };
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 200F));
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            // Left panel for controls
            var leftPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                Padding = new Padding(5)
            };
            leftPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            leftPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            leftPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            leftPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            // System selection
            var systemLabel = new Label
            {
                Text = "Select Particle System:",
                Dock = DockStyle.Top,
                Height = 20
            };

            particleSystemsBox = new ComboBox
            {
                Dock = DockStyle.Top,
                Height = 25,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            // Control buttons panel
            var buttonPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                ColumnCount = 1,
                RowCount = 3,
                Height = 120,
                Padding = new Padding(2)
            };
            buttonPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 33F));
            buttonPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 33F));
            buttonPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 34F));

            emitButton = new Button
            {
                Text = "Запустить",
                Dock = DockStyle.Fill,
                Height = 30,
                Enabled = true
            };

            stopButton = new Button
            {
                Text = "Прервать",
                Dock = DockStyle.Fill,
                Height = 30,
                Enabled = false
            };

            cyclicCheckBox = new CheckBox
            {
                Text = "Цикличность",
                Dock = DockStyle.Fill,
                Checked = false,
                Height = 30,
                TextAlign = ContentAlignment.MiddleLeft
            };

            buttonPanel.Controls.Add(emitButton, 0, 0);
            buttonPanel.Controls.Add(stopButton, 0, 1);
            buttonPanel.Controls.Add(cyclicCheckBox, 0, 2);

            // Add controls to left panel
            leftPanel.Controls.Add(systemLabel, 0, 0);
            leftPanel.Controls.Add(particleSystemsBox, 0, 1);
            leftPanel.Controls.Add(buttonPanel, 0, 2);

            // Parameters panel
            parametersPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 10,
                Padding = new Padding(5)
            };
            parametersPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            parametersPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

            // Effect type selection
            var effectLabel = new Label { Text = "Effect Type:" };
            effectTypeComboBox = new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            effectTypeComboBox.Items.AddRange(Enum.GetNames(typeof(ParticleEffectType)));
            effectTypeComboBox.SelectedIndex = 0;

            // Shape selection
            var shapeLabel = new Label { Text = "Particle Shape:" };
            shapeComboBox = new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            shapeComboBox.Items.AddRange(Enum.GetNames(typeof(ParticleShape)));
            shapeComboBox.SelectedIndex = 0;

            // Particle parameters
            AddParameterControl("Min Size:", out minSizeNumeric, 0.1m, 1000m, 0.1m);
            AddParameterControl("Max Size:", out maxSizeNumeric, 0.1m, 1000m, 0.1m);
            AddParameterControl("Spread:", out spreadNumeric, 0.1m, 1000m, 0.1m);
            AddParameterControl("Speed:", out speedNumeric, 0.1m, 1000m, 0.1m);
            AddParameterControl("Life Min:", out lifeMinNumeric, 0.1m, 1000m, 0.1m);
            AddParameterControl("Life Max:", out lifeMaxNumeric, 0.1m, 1000m, 0.1m);
            AddParameterControl("Color Variation:", out colorVariationNumeric, 0m, 1m, 0.01m);

            // Color selection
            var colorLabel = new Label { Text = "Particle Color:" };
            colorButton = new Button
            {
                Text = "Select Color",
                Dock = DockStyle.Fill
            };

            parametersPanel.Controls.Add(effectLabel, 0, 0);
            parametersPanel.Controls.Add(effectTypeComboBox, 1, 0);
            parametersPanel.Controls.Add(shapeLabel, 0, 1);
            parametersPanel.Controls.Add(shapeComboBox, 1, 1);
            parametersPanel.Controls.Add(colorLabel, 0, 7);
            parametersPanel.Controls.Add(colorButton, 1, 7);

            // Add panels to main panel
            mainPanel.Controls.Add(leftPanel, 0, 0);
            mainPanel.Controls.Add(parametersPanel, 1, 0);

            this.Controls.Add(mainPanel);

            // Add event handlers
            particleSystemsBox.SelectedIndexChanged += particleSystemsBox_SelectedIndexChanged;
            emitButton.Click += emitButton_Click;
            stopButton.Click += stopButton_Click;
            cyclicCheckBox.CheckedChanged += cyclicCheckBox_CheckedChanged;
            effectTypeComboBox.SelectedIndexChanged += effectTypeComboBox_SelectedIndexChanged;
            shapeComboBox.SelectedIndexChanged += shapeComboBox_SelectedIndexChanged;
            colorButton.Click += colorButton_Click;

            // Initialize numeric controls
            minSizeNumeric.ValueChanged += (s, e) => UpdateParticleSystem();
            maxSizeNumeric.ValueChanged += (s, e) => UpdateParticleSystem();
            spreadNumeric.ValueChanged += (s, e) => UpdateParticleSystem();
            speedNumeric.ValueChanged += (s, e) => UpdateParticleSystem();
            lifeMinNumeric.ValueChanged += (s, e) => UpdateParticleSystem();
            lifeMaxNumeric.ValueChanged += (s, e) => UpdateParticleSystem();
            colorVariationNumeric.ValueChanged += (s, e) => UpdateParticleSystem();
        }

        private void AddParameterControl(string label, out NumericUpDown numeric, decimal min, decimal max, decimal increment = 1)
        {
            var labelControl = new Label { Text = label };
            numeric = new NumericUpDown
            {
                Minimum = min,
                Maximum = max,
                Increment = increment,
                DecimalPlaces = increment < 1 ? 2 : 1,
                Dock = DockStyle.Fill
            };

            parametersPanel.Controls.Add(labelControl);
            parametersPanel.Controls.Add(numeric);
        }

        private void InitializeUI()
        {
            // Fill particle systems list
            foreach (var particleSystem in sceneState.ParticleSystems)
            {
                particleSystemsBox.Items.Add(particleSystem.Name);
            }

            // Set initial values
            if (particleSystemsBox.Items.Count > 0)
            {
                particleSystemsBox.SelectedIndex = 0;
            }
        }

        private void particleSystemsBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (particleSystemsBox.SelectedItem != null)
            {
                selectedParticleSystem = sceneState.ParticleSystems.Find(p => p.Name.Equals(particleSystemsBox.SelectedItem.ToString()));
                if (selectedParticleSystem != null)
                {
                    UpdateUIFromParticleSystem();
                    UpdateControlsState();
                }
            }
        }

        private void UpdateUIFromParticleSystem()
        {
            if (selectedParticleSystem == null) return;

            minSizeNumeric.Value = (decimal)selectedParticleSystem.MinParticleSize;
            maxSizeNumeric.Value = (decimal)selectedParticleSystem.MaxParticleSize;
            spreadNumeric.Value = (decimal)selectedParticleSystem.ParticleSpread;
            speedNumeric.Value = (decimal)selectedParticleSystem.ParticleSpeed;
            lifeMinNumeric.Value = (decimal)selectedParticleSystem.ParticleLifeMin;
            lifeMaxNumeric.Value = (decimal)selectedParticleSystem.ParticleLifeMax;
            colorVariationNumeric.Value = (decimal)selectedParticleSystem.ColorVariation;
            
            if (effectTypeComboBox.Items.Count > 0)
            {
                effectTypeComboBox.SelectedItem = selectedParticleSystem.EffectType.ToString();
            }
            
            if (shapeComboBox.Items.Count > 0)
            {
                shapeComboBox.SelectedItem = selectedParticleSystem.Shape.ToString();
            }
            
            cyclicCheckBox.Checked = selectedParticleSystem.IsCyclic;
            UpdateControlsState();
        }

        private void UpdateControlsState()
        {
            bool isCyclic = selectedParticleSystem?.IsCyclic ?? false;
            
            // Отключаем элементы управления при цикличности
            emitButton.Enabled = !isCyclic;
            stopButton.Enabled = !isCyclic;
            effectTypeComboBox.Enabled = !isCyclic;
            shapeComboBox.Enabled = !isCyclic;
            
            // Оставляем доступными параметры настройки
            minSizeNumeric.Enabled = true;
            maxSizeNumeric.Enabled = true;
            spreadNumeric.Enabled = true;
            speedNumeric.Enabled = true;
            lifeMinNumeric.Enabled = true;
            lifeMaxNumeric.Enabled = true;
            colorVariationNumeric.Enabled = true;
            colorButton.Enabled = true;
        }

        private void UpdateParticleSystem()
        {
            if (selectedParticleSystem == null) return;

            selectedParticleSystem.MinParticleSize = (float)minSizeNumeric.Value;
            selectedParticleSystem.MaxParticleSize = (float)maxSizeNumeric.Value;
            selectedParticleSystem.ParticleSpread = (float)spreadNumeric.Value;
            selectedParticleSystem.ParticleSpeed = (float)speedNumeric.Value;
            selectedParticleSystem.ParticleLifeMin = (float)lifeMinNumeric.Value;
            selectedParticleSystem.ParticleLifeMax = (float)lifeMaxNumeric.Value;
            selectedParticleSystem.ColorVariation = (float)colorVariationNumeric.Value;
            
            if (effectTypeComboBox.SelectedItem != null)
            {
                selectedParticleSystem.EffectType = (ParticleEffectType)Enum.Parse(typeof(ParticleEffectType), effectTypeComboBox.SelectedItem.ToString());
            }
            
            if (shapeComboBox.SelectedItem != null)
            {
                selectedParticleSystem.Shape = (ParticleShape)Enum.Parse(typeof(ParticleShape), shapeComboBox.SelectedItem.ToString());
            }
        }

        private void emitButton_Click(object sender, EventArgs e)
        {
            if (selectedParticleSystem != null)
            {
                selectedParticleSystem.Start();
                UpdateControlsState();
            }
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            if (selectedParticleSystem != null)
            {
                selectedParticleSystem.Stop();
                UpdateControlsState();
            }
        }

        private void cyclicCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (selectedParticleSystem != null)
            {
                selectedParticleSystem.IsCyclic = cyclicCheckBox.Checked;
                UpdateControlsState();
            }
        }

        private void effectTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateParticleSystem();
        }

        private void shapeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateParticleSystem();
        }

        private void colorButton_Click(object sender, EventArgs e)
        {
            if (selectedParticleSystem == null) return;

            Vector3 currentColor;
            switch (selectedParticleSystem.EffectType)
            {
                case ParticleEffectType.Basic:
                    currentColor = selectedParticleSystem.BasicColor;
                    break;
                case ParticleEffectType.Fire:
                    currentColor = selectedParticleSystem.FireColor;
                    break;
                case ParticleEffectType.Sparkle:
                    currentColor = selectedParticleSystem.SparkleColor;
                    break;
                case ParticleEffectType.Fountain:
                    currentColor = selectedParticleSystem.FountainColor;
                    break;
                default:
                    return;
            }

            using (var colorDialog = new ColorDialog())
            {
                colorDialog.Color = System.Drawing.Color.FromArgb(
                    (int)(currentColor.X * 255),
                    (int)(currentColor.Y * 255),
                    (int)(currentColor.Z * 255)
                );

                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    Vector3 newColor = new Vector3(
                        colorDialog.Color.R / 255f,
                        colorDialog.Color.G / 255f,
                        colorDialog.Color.B / 255f
                    );

                    switch (selectedParticleSystem.EffectType)
                    {
                        case ParticleEffectType.Basic:
                            selectedParticleSystem.BasicColor = newColor;
                            break;
                        case ParticleEffectType.Fire:
                            selectedParticleSystem.FireColor = newColor;
                            break;
                        case ParticleEffectType.Sparkle:
                            selectedParticleSystem.SparkleColor = newColor;
                            break;
                        case ParticleEffectType.Fountain:
                            selectedParticleSystem.FountainColor = newColor;
                            break;
                    }
                }
            }
        }
    }
} 