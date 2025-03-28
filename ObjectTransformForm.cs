using OpenTK.Mathematics;

namespace Computer_Graphics_Programming_Blue_Meteorite
{
    public partial class ObjectTransformForm : Form
    {
        private SceneState sceneState;
        private SceneObject selectedObject;

        public ObjectTransformForm(SceneState state)
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
            objectXCord = new TextBox { Location = new System.Drawing.Point(30, 170), Size = new System.Drawing.Size(60, 20) };

            Label posYLabel = new Label { Text = "Y:", Location = new System.Drawing.Point(100, 170), AutoSize = true };
            objectYCord = new TextBox { Location = new System.Drawing.Point(120, 170), Size = new System.Drawing.Size(60, 20) };

            Label posZLabel = new Label { Text = "Z:", Location = new System.Drawing.Point(190, 170), AutoSize = true };
            objectZCord = new TextBox { Location = new System.Drawing.Point(210, 170), Size = new System.Drawing.Size(60, 20) };

            // Translation controls
            Label translateLabel = new Label
            {
                Text = "Translate:",
                Location = new System.Drawing.Point(10, 200),
                AutoSize = true
            };

            Label transXLabel = new Label { Text = "X:", Location = new System.Drawing.Point(10, 230), AutoSize = true };
            objectTranslateXCord = new TextBox { Location = new System.Drawing.Point(30, 230), Size = new System.Drawing.Size(60, 20) };

            Label transYLabel = new Label { Text = "Y:", Location = new System.Drawing.Point(100, 230), AutoSize = true };
            objectTranslateYCord = new TextBox { Location = new System.Drawing.Point(120, 230), Size = new System.Drawing.Size(60, 20) };

            Label transZLabel = new Label { Text = "Z:", Location = new System.Drawing.Point(190, 230), AutoSize = true };
            objectTranslateZCord = new TextBox { Location = new System.Drawing.Point(210, 230), Size = new System.Drawing.Size(60, 20) };

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
            objectRotateXCord = new TextBox { Location = new System.Drawing.Point(30, 290), Size = new System.Drawing.Size(60, 20) };

            Label rotYLabel = new Label { Text = "Y:", Location = new System.Drawing.Point(100, 290), AutoSize = true };
            objectRotateYCord = new TextBox { Location = new System.Drawing.Point(120, 290), Size = new System.Drawing.Size(60, 20) };

            Label rotZLabel = new Label { Text = "Z:", Location = new System.Drawing.Point(190, 290), AutoSize = true };
            objectRotateZCord = new TextBox { Location = new System.Drawing.Point(210, 290), Size = new System.Drawing.Size(60, 20) };

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
            objectScaleXCord = new TextBox { Location = new System.Drawing.Point(30, 350), Size = new System.Drawing.Size(60, 20) };

            Label scaleYLabel = new Label { Text = "Y:", Location = new System.Drawing.Point(100, 350), AutoSize = true };
            objectScaleYCord = new TextBox { Location = new System.Drawing.Point(120, 350), Size = new System.Drawing.Size(60, 20) };

            Label scaleZLabel = new Label { Text = "Z:", Location = new System.Drawing.Point(190, 350), AutoSize = true };
            objectScaleZCord = new TextBox { Location = new System.Drawing.Point(210, 350), Size = new System.Drawing.Size(60, 20) };

            scaleBtn = new Button
            {
                Text = "Scale",
                Location = new System.Drawing.Point(280, 350),
                Size = new System.Drawing.Size(80, 20)
            };

            // Force controls
            Label forceLabel = new Label
            {
                Text = "Apply Force:",
                Location = new System.Drawing.Point(10, 380),
                AutoSize = true
            };

            Label forceXLabel = new Label { Text = "X:", Location = new System.Drawing.Point(10, 410), AutoSize = true };
            pushForceX = new TextBox { Location = new System.Drawing.Point(30, 410), Size = new System.Drawing.Size(60, 20) };

            Label forceYLabel = new Label { Text = "Y:", Location = new System.Drawing.Point(100, 410), AutoSize = true };
            pushForceY = new TextBox { Location = new System.Drawing.Point(120, 410), Size = new System.Drawing.Size(60, 20) };

            Label forceZLabel = new Label { Text = "Z:", Location = new System.Drawing.Point(190, 410), AutoSize = true };
            pushForceZ = new TextBox { Location = new System.Drawing.Point(210, 410), Size = new System.Drawing.Size(60, 20) };

            pushBtn = new Button
            {
                Text = "Push",
                Location = new System.Drawing.Point(280, 410),
                Size = new System.Drawing.Size(80, 20)
            };

            // Add controls to form
            this.Controls.AddRange(new Control[] {
                objectLabel, objectsBox,
                positionLabel, posXLabel, posYLabel, posZLabel, objectXCord, objectYCord, objectZCord,
                translateLabel, transXLabel, transYLabel, transZLabel, objectTranslateXCord, objectTranslateYCord, objectTranslateZCord, translateBtn,
                rotateLabel, rotXLabel, rotYLabel, rotZLabel, objectRotateXCord, objectRotateYCord, objectRotateZCord, rotateBtn,
                scaleLabel, scaleXLabel, scaleYLabel, scaleZLabel, objectScaleXCord, objectScaleYCord, objectScaleZCord, scaleBtn,
                forceLabel, forceXLabel, forceYLabel, forceZLabel, pushForceX, pushForceY, pushForceZ, pushBtn
            });

            // Add event handlers
            objectsBox.SelectedIndexChanged += objectsBox_SelectedIndexChanged;
            objectXCord.TextChanged += objectsCord_Changed;
            objectYCord.TextChanged += objectsCord_Changed;
            objectZCord.TextChanged += objectsCord_Changed;
            translateBtn.Click += translateBtn_Click;
            rotateBtn.Click += rotateBtn_Click;
            scaleBtn.Click += scaleBtn_Click;
            pushBtn.MouseDown += pushBtn_MouseDown;
            pushBtn.MouseUp += pushBtn_MouseUp;
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
                    objectXCord.Text = selectedObject.Position.X.ToString();
                    objectYCord.Text = selectedObject.Position.Y.ToString();
                    objectZCord.Text = selectedObject.Position.Z.ToString();
                }
            }
        }

        private void objectsCord_Changed(object sender, EventArgs e)
        {
            lock (sceneState)
            {
                if (selectedObject == null) return;

                var textBox = sender as TextBox;
                if (textBox != null && float.TryParse(textBox.Text.Replace('.', ','), out float value))
                {
                    switch (textBox.Name)
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

        private void translateBtn_Click(object sender, EventArgs e)
        {
            lock (sceneState)
            {
                try
                {
                    if (selectedObject != null)
                    {
                        float x = float.Parse(objectTranslateXCord.Text);
                        float y = float.Parse(objectTranslateYCord.Text);
                        float z = float.Parse(objectTranslateZCord.Text);

                        selectedObject.ApplyTranslation(new Vector3(x, y, z));

                        objectXCord.Text = selectedObject.Position.X.ToString();
                        objectYCord.Text = selectedObject.Position.Y.ToString();
                        objectZCord.Text = selectedObject.Position.Z.ToString();
                    }
                }
                catch (Exception _) { }
            }
        }

        private void rotateBtn_Click(object sender, EventArgs e)
        {
            lock (sceneState)
            {
                try
                {
                    if (selectedObject != null)
                    {
                        float x = float.Parse(objectRotateXCord.Text);
                        float y = float.Parse(objectRotateYCord.Text);
                        float z = float.Parse(objectRotateZCord.Text);

                        selectedObject.ApplyRotation(new Vector3(x, y, z));
                    }
                }
                catch (Exception _) { }
            }
        }

        private void scaleBtn_Click(object sender, EventArgs e)
        {
            lock (sceneState)
            {
                try
                {
                    if (selectedObject != null)
                    {
                        float x = float.Parse(objectScaleXCord.Text);
                        float y = float.Parse(objectScaleYCord.Text);
                        float z = float.Parse(objectScaleZCord.Text);

                        selectedObject.ApplyScale(new Vector3(x, y, z));
                    }
                }
                catch (Exception _) { }
            }
        }

        private void pushBtn_MouseDown(object sender, MouseEventArgs e)
        {
            lock (sceneState)
            {
                try
                {
                    if (selectedObject != null)
                    {
                        float x = float.Parse(pushForceX.Text);
                        float y = float.Parse(pushForceY.Text);
                        float z = float.Parse(pushForceZ.Text);

                        selectedObject.isForceApplied = true;
                        selectedObject.AppliedForce = new Vector3(x, y, z);
                    }
                }
                catch (Exception _) { }
            }
        }

        private void pushBtn_MouseUp(object sender, MouseEventArgs e)
        {
            lock (sceneState)
            {
                try
                {
                    if (selectedObject != null)
                    {
                        selectedObject.isForceApplied = false;
                    }
                }
                catch (Exception _) { }
            }
        }

        private ListBox objectsBox;
        private TextBox objectXCord;
        private TextBox objectYCord;
        private TextBox objectZCord;
        private TextBox objectTranslateXCord;
        private TextBox objectTranslateYCord;
        private TextBox objectTranslateZCord;
        private TextBox objectRotateXCord;
        private TextBox objectRotateYCord;
        private TextBox objectRotateZCord;
        private TextBox objectScaleXCord;
        private TextBox objectScaleYCord;
        private TextBox objectScaleZCord;
        private TextBox pushForceX;
        private TextBox pushForceY;
        private TextBox pushForceZ;
        private Button translateBtn;
        private Button rotateBtn;
        private Button scaleBtn;
        private Button pushBtn;
    }
} 