using OpenTK.Mathematics;

namespace Computer_Graphics_Programming_Blue_Meteorite
{
    public partial class ObjectTransformForm : Form
    {
        private SceneObjects sceneState;
        private SceneObject selectedObject;
        private bool isInitializing = false;

        public ObjectTransformForm(SceneObjects state)
        {
            sceneState = state;
            InitializeComponent();
            InitializeUI();
        }

        private void InitializeComponent()
        {
            this.Text = "Object Transform Control";
            this.Size = new System.Drawing.Size(400, 500);

            // Object selection
            Label objectLabel = new Label
            {
                Text = "Select Object:",
                Location = new System.Drawing.Point(10, 10),
                AutoSize = true
            };

            objectsBox = new ListBox
            {
                Location = new System.Drawing.Point(10, 30),
                Size = new System.Drawing.Size(200, 100)
            };

            // Position controls
            Label positionLabel = new Label
            {
                Text = "Position:",
                Location = new System.Drawing.Point(10, 140),
                AutoSize = true
            };

            Label posXLabel = new Label { Text = "X:", Location = new System.Drawing.Point(10, 170), AutoSize = true };
            objectXCord = new NumericUpDown { Location = new System.Drawing.Point(30, 170), Size = new System.Drawing.Size(60, 20), DecimalPlaces = 2, Minimum = -1000, Maximum = 1000, Increment = 0.1m };

            Label posYLabel = new Label { Text = "Y:", Location = new System.Drawing.Point(100, 170), AutoSize = true };
            objectYCord = new NumericUpDown { Location = new System.Drawing.Point(120, 170), Size = new System.Drawing.Size(60, 20), DecimalPlaces = 2, Minimum = -1000, Maximum = 1000, Increment = 0.1m };

            Label posZLabel = new Label { Text = "Z:", Location = new System.Drawing.Point(190, 170), AutoSize = true };
            objectZCord = new NumericUpDown { Location = new System.Drawing.Point(210, 170), Size = new System.Drawing.Size(60, 20), DecimalPlaces = 2, Minimum = -1000, Maximum = 1000, Increment = 0.1m };

            setPositionBtn = new Button
            {
                Text = "Set",
                Location = new System.Drawing.Point(280, 170),
                Size = new System.Drawing.Size(80, 20)
            };

            // Translation controls
            Label translateLabel = new Label
            {
                Text = "Translate:",
                Location = new System.Drawing.Point(10, 200),
                AutoSize = true
            };

            Label transXLabel = new Label { Text = "X:", Location = new System.Drawing.Point(10, 230), AutoSize = true };
            objectTranslateXCord = new NumericUpDown { Location = new System.Drawing.Point(30, 230), Size = new System.Drawing.Size(60, 20), DecimalPlaces = 2, Minimum = -1000, Maximum = 1000, Increment = 0.1m };

            Label transYLabel = new Label { Text = "Y:", Location = new System.Drawing.Point(100, 230), AutoSize = true };
            objectTranslateYCord = new NumericUpDown { Location = new System.Drawing.Point(120, 230), Size = new System.Drawing.Size(60, 20), DecimalPlaces = 2, Minimum = -1000, Maximum = 1000, Increment = 0.1m };

            Label transZLabel = new Label { Text = "Z:", Location = new System.Drawing.Point(190, 230), AutoSize = true };
            objectTranslateZCord = new NumericUpDown { Location = new System.Drawing.Point(210, 230), Size = new System.Drawing.Size(60, 20), DecimalPlaces = 2, Minimum = -1000, Maximum = 1000, Increment = 0.1m };

            translateBtn = new Button
            {
                Text = "Translate",
                Location = new System.Drawing.Point(280, 230),
                Size = new System.Drawing.Size(80, 20)
            };

            // Rotation controls
            Label rotateLabel = new Label
            {
                Text = "Rotate:",
                Location = new System.Drawing.Point(10, 260),
                AutoSize = true
            };

            Label rotXLabel = new Label { Text = "X:", Location = new System.Drawing.Point(10, 290), AutoSize = true };
            objectRotateXCord = new NumericUpDown { Location = new System.Drawing.Point(30, 290), Size = new System.Drawing.Size(60, 20), DecimalPlaces = 2, Minimum = -360, Maximum = 360, Increment = 1m };

            Label rotYLabel = new Label { Text = "Y:", Location = new System.Drawing.Point(100, 290), AutoSize = true };
            objectRotateYCord = new NumericUpDown { Location = new System.Drawing.Point(120, 290), Size = new System.Drawing.Size(60, 20), DecimalPlaces = 2, Minimum = -360, Maximum = 360, Increment = 1m };

            Label rotZLabel = new Label { Text = "Z:", Location = new System.Drawing.Point(190, 290), AutoSize = true };
            objectRotateZCord = new NumericUpDown { Location = new System.Drawing.Point(210, 290), Size = new System.Drawing.Size(60, 20), DecimalPlaces = 2, Minimum = -360, Maximum = 360, Increment = 1m };

            rotateBtn = new Button
            {
                Text = "Rotate",
                Location = new System.Drawing.Point(280, 290),
                Size = new System.Drawing.Size(80, 20)
            };

            // Scale controls
            Label scaleLabel = new Label
            {
                Text = "Scale:",
                Location = new System.Drawing.Point(10, 320),
                AutoSize = true
            };

            Label scaleXLabel = new Label { Text = "X:", Location = new System.Drawing.Point(10, 350), AutoSize = true };
            objectScaleXCord = new NumericUpDown { Location = new System.Drawing.Point(30, 350), Size = new System.Drawing.Size(60, 20), DecimalPlaces = 2, Minimum = 0.01m, Maximum = 100, Increment = 0.1m, Value = 1 };

            Label scaleYLabel = new Label { Text = "Y:", Location = new System.Drawing.Point(100, 350), AutoSize = true };
            objectScaleYCord = new NumericUpDown { Location = new System.Drawing.Point(120, 350), Size = new System.Drawing.Size(60, 20), DecimalPlaces = 2, Minimum = 0.01m, Maximum = 100, Increment = 0.1m, Value = 1 };

            Label scaleZLabel = new Label { Text = "Z:", Location = new System.Drawing.Point(190, 350), AutoSize = true };
            objectScaleZCord = new NumericUpDown { Location = new System.Drawing.Point(210, 350), Size = new System.Drawing.Size(60, 20), DecimalPlaces = 2, Minimum = 0.01m, Maximum = 100, Increment = 0.1m, Value = 1 };

            scaleBtn = new Button
            {
                Text = "Scale",
                Location = new System.Drawing.Point(280, 350),
                Size = new System.Drawing.Size(80, 20)
            };

            // Add controls to form
            this.Controls.AddRange(new Control[] {
                objectLabel, objectsBox,
                positionLabel, posXLabel, posYLabel, posZLabel, objectXCord, objectYCord, objectZCord, setPositionBtn,
                translateLabel, transXLabel, transYLabel, transZLabel, objectTranslateXCord, objectTranslateYCord, objectTranslateZCord, translateBtn,
                rotateLabel, rotXLabel, rotYLabel, rotZLabel, objectRotateXCord, objectRotateYCord, objectRotateZCord, rotateBtn,
                scaleLabel, scaleXLabel, scaleYLabel, scaleZLabel, objectScaleXCord, objectScaleYCord, objectScaleZCord, scaleBtn
            });

            // Add event handlers
            objectsBox.SelectedIndexChanged += objectsBox_SelectedIndexChanged;
            objectXCord.ValueChanged += objectsCord_Changed;
            objectYCord.ValueChanged += objectsCord_Changed;
            objectZCord.ValueChanged += objectsCord_Changed;
            setPositionBtn.Click += setPositionBtn_Click;
            translateBtn.Click += translateBtn_Click;
            rotateBtn.Click += rotateBtn_Click;
            scaleBtn.Click += scaleBtn_Click;
        }

        private void InitializeUI()
        {
            // Fill objects list
            foreach (var obj in sceneState.Objects)
            {
                objectsBox.Items.Add(obj.Name);
            }
        }

        private void objectsBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (objectsBox.SelectedItem != null)
            {
                selectedObject = sceneState.Objects.Find(o => o.Name.Equals(objectsBox.SelectedItem.ToString()));
                if (selectedObject != null)
                {
                    isInitializing = true;
                    objectXCord.Value = (decimal)selectedObject.Position.X;
                    objectYCord.Value = (decimal)selectedObject.Position.Y;
                    objectZCord.Value = (decimal)selectedObject.Position.Z;
                    isInitializing = false;
                }
            }
        }

        private void objectsCord_Changed(object sender, EventArgs e)
        {
            if (isInitializing) return;

            lock (sceneState)
            {
                if (selectedObject == null) return;

                var numeric = sender as NumericUpDown;
                if (numeric != null)
                {
                    float value = (float)numeric.Value;
                    switch (numeric.Name)
                    {
                        case "objectXCord":
                            selectedObject.Position = new Vector3(value, selectedObject.Position.Y, selectedObject.Position.Z);
                            break;
                        case "objectYCord":
                            selectedObject.Position = new Vector3(selectedObject.Position.X, value, selectedObject.Position.Z);
                            break;
                        case "objectZCord":
                            selectedObject.Position = new Vector3(selectedObject.Position.X, selectedObject.Position.Y, value);
                            break;
                    }
                }
            }
        }

        private void setPositionBtn_Click(object sender, EventArgs e)
        {
            lock (sceneState)
            {
                if (selectedObject != null)
                {
                    float x = (float)objectXCord.Value;
                    float y = (float)objectYCord.Value;
                    float z = (float)objectZCord.Value;

                    selectedObject.Position = new Vector3(x, y, z);
                }
            }
        }

        private void translateBtn_Click(object sender, EventArgs e)
        {
            lock (sceneState)
            {
                if (selectedObject != null)
                {
                    float x = (float)objectTranslateXCord.Value;
                    float y = (float)objectTranslateYCord.Value;
                    float z = (float)objectTranslateZCord.Value;

                    selectedObject.ApplyTranslation(new Vector3(x, y, z));

                    isInitializing = true;
                    objectXCord.Value = (decimal)selectedObject.Position.X;
                    objectYCord.Value = (decimal)selectedObject.Position.Y;
                    objectZCord.Value = (decimal)selectedObject.Position.Z;
                    isInitializing = false;
                }
            }
        }

        private void rotateBtn_Click(object sender, EventArgs e)
        {
            lock (sceneState)
            {
                if (selectedObject != null)
                {
                    float x = (float)objectRotateXCord.Value;
                    float y = (float)objectRotateYCord.Value;
                    float z = (float)objectRotateZCord.Value;

                    selectedObject.ApplyRotation(new Vector3(x, y, z));
                }
            }
        }

        private void scaleBtn_Click(object sender, EventArgs e)
        {
            lock (sceneState)
            {
                if (selectedObject != null)
                {
                    float x = (float)objectScaleXCord.Value;
                    float y = (float)objectScaleYCord.Value;
                    float z = (float)objectScaleZCord.Value;

                    selectedObject.ApplyScale(new Vector3(x, y, z));
                }
            }
        }

        private ListBox objectsBox;
        private NumericUpDown objectXCord;
        private NumericUpDown objectYCord;
        private NumericUpDown objectZCord;
        private NumericUpDown objectTranslateXCord;
        private NumericUpDown objectTranslateYCord;
        private NumericUpDown objectTranslateZCord;
        private NumericUpDown objectRotateXCord;
        private NumericUpDown objectRotateYCord;
        private NumericUpDown objectRotateZCord;
        private NumericUpDown objectScaleXCord;
        private NumericUpDown objectScaleYCord;
        private NumericUpDown objectScaleZCord;
        private Button setPositionBtn;
        private Button translateBtn;
        private Button rotateBtn;
        private Button scaleBtn;
    }
} 