using OpenTK.Mathematics;

namespace Computer_Graphics_Programming_Blue_Meteorite
{
    public partial class ObjectTransformForm : Form
    {
        private SceneObjects sceneState;
        private SceneObject selectedObject;
        private bool isInitializing = false;
        private CheckedListBox objectsBox;
        private List<SceneObject> selectedObjects = new List<SceneObject>();

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
                Text = "Select Objects:",
                Location = new System.Drawing.Point(10, 10),
                AutoSize = true
            };

            objectsBox = new CheckedListBox
            {
                Location = new System.Drawing.Point(10, 30),
                Size = new System.Drawing.Size(200, 100),
                CheckOnClick = true
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
            objectScaleXCord = new NumericUpDown 
            { 
                Location = new System.Drawing.Point(30, 350), 
                Size = new System.Drawing.Size(60, 20), 
                DecimalPlaces = 2, 
                Minimum = -100m, 
                Maximum = 100m, 
                Increment = 0.1m, 
                Value = 1 
            };

            Label scaleYLabel = new Label { Text = "Y:", Location = new System.Drawing.Point(100, 350), AutoSize = true };
            objectScaleYCord = new NumericUpDown 
            { 
                Location = new System.Drawing.Point(120, 350), 
                Size = new System.Drawing.Size(60, 20), 
                DecimalPlaces = 2, 
                Minimum = -100m, 
                Maximum = 100m, 
                Increment = 0.1m, 
                Value = 1 
            };

            Label scaleZLabel = new Label { Text = "Z:", Location = new System.Drawing.Point(190, 350), AutoSize = true };
            objectScaleZCord = new NumericUpDown 
            { 
                Location = new System.Drawing.Point(210, 350), 
                Size = new System.Drawing.Size(60, 20), 
                DecimalPlaces = 2, 
                Minimum = -100m, 
                Maximum = 100m, 
                Increment = 0.1m, 
                Value = 1 
            };

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
            objectsBox.ItemCheck += objectsBox_ItemCheck;
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

        private void objectsBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            string objectName = objectsBox.Items[e.Index].ToString();
            SceneObject obj = sceneState.Objects.Find(o => o.Name.Equals(objectName));

            if (obj != null)
            {
                if (e.NewValue == CheckState.Checked)
                {
                    selectedObjects.Add(obj);
                    if (selectedObject == null)
                    {
                        selectedObject = obj;
                        UpdatePositionControls(obj);
                    }
                }
                else
                {
                    selectedObjects.Remove(obj);
                    if (selectedObject == obj)
                    {
                        selectedObject = selectedObjects.Count > 0 ? selectedObjects[0] : null;
                        if (selectedObject != null)
                        {
                            UpdatePositionControls(selectedObject);
                        }
                        else
                        {
                            ResetPositionControls();
                        }
                    }
                }
            }
        }

        private void UpdatePositionControls(SceneObject obj)
        {
            isInitializing = true;
            objectXCord.Value = (decimal)obj.Position.X;
            objectYCord.Value = (decimal)obj.Position.Y;
            objectZCord.Value = (decimal)obj.Position.Z;
            isInitializing = false;
        }

        private void ResetPositionControls()
        {
            isInitializing = true;
            objectXCord.Value = 0;
            objectYCord.Value = 0;
            objectZCord.Value = 0;
            isInitializing = false;
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
                foreach (var obj in selectedObjects)
                {
                    float x = (float)objectXCord.Value;
                    float y = (float)objectYCord.Value;
                    float z = (float)objectZCord.Value;

                    // Calculate the displacement vector
                    Vector3 displacement = new Vector3(x, y, z) - obj.Position;
                    
                    // For dynamic objects, we need to set the force application flag
                    if (obj.IsDynamic)
                    {
                        // Calculate a strong force to move the object to the target position
                        obj.AppliedForce = displacement * 100.0f;
                        obj.isForceApplied = true;
                    }
                    
                    // Always update the position directly in the scene state
                    obj.Position = new Vector3(x, y, z);
                }
            }
        }

        private void translateBtn_Click(object sender, EventArgs e)
        {
            lock (sceneState)
            {
                foreach (var obj in selectedObjects)
                {
                    float x = (float)objectTranslateXCord.Value;
                    float y = (float)objectTranslateYCord.Value;
                    float z = (float)objectTranslateZCord.Value;
                    
                    Vector3 translation = new Vector3(x, y, z);
                    
                    // For dynamic objects, set the force directly to ensure motion
                    if (obj.IsDynamic)
                    {
                        obj.AppliedForce = translation * 100.0f;
                        obj.isForceApplied = true;
                        
                        // Also apply the translation directly to the position for immediate feedback
                        obj.Position += translation;
                    }
                    else
                    {
                        // For static objects, simply translate
                        obj.ApplyTranslation(translation);
                    }
                }

                // Update the position display for the selected object
                if (selectedObject != null)
                {
                    UpdatePositionControls(selectedObject);
                }
            }
        }

        private void rotateBtn_Click(object sender, EventArgs e)
        {
            lock (sceneState)
            {
                foreach (var obj in selectedObjects)
                {
                    float x = (float)objectRotateXCord.Value;
                    float y = (float)objectRotateYCord.Value;
                    float z = (float)objectRotateZCord.Value;

                    obj.ApplyRotation(new Vector3(x, y, z));
                }
            }
        }

        private void scaleBtn_Click(object sender, EventArgs e)
        {
            lock (sceneState)
            {
                foreach (var obj in selectedObjects)
                {
                    float x = (float)objectScaleXCord.Value;
                    float y = (float)objectScaleYCord.Value;
                    float z = (float)objectScaleZCord.Value;

                    obj.ApplyScale(new Vector3(x, y, z));
                }
            }
        }

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