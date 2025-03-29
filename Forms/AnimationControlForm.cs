using OpenTK.Mathematics;
using System.Windows.Forms;

namespace Computer_Graphics_Programming_Blue_Meteorite
{
    public partial class AnimationControlForm : Form
    {
        private SceneObjects sceneState;
        private SceneObject selectedObject;
        private AnimationSystem selectedAnimation;
        private Panel parameterPanel;
        private ComboBox animationTypeCombo;
        private Button addAnimationBtn;
        private Button removeAnimationBtn;
        private List<NumericUpDown> activeControls = new List<NumericUpDown>();
        private Label typeLabel;
        private ListBox objectsBox;
        private ListBox animBox;
        private Button animStartBtn;
        private Button animStopBtn;
        private TableLayoutPanel mainLayout;
        private TableLayoutPanel leftPanel;
        private TableLayoutPanel rightPanel;

        public AnimationControlForm(SceneObjects state)
        {
            sceneState = state;
            InitializeComponent();
            InitializeUI();
        }

        private void InitializeComponent()
        {
            this.Text = "Animation Control";
            this.Size = new System.Drawing.Size(800, 600);
            this.MinimumSize = new System.Drawing.Size(800, 600);

            // Main layout
            mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Padding = new Padding(10),
                ColumnStyles = {
                    new ColumnStyle(SizeType.Percent, 40F),
                    new ColumnStyle(SizeType.Percent, 60F)
                }
            };

            // Left panel (objects and animations list)
            leftPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 5,
                RowStyles = {
                    new RowStyle(SizeType.Absolute, 30F),
                    new RowStyle(SizeType.Absolute, 120F),
                    new RowStyle(SizeType.Absolute, 30F),
                    new RowStyle(SizeType.Absolute, 120F),
                    new RowStyle(SizeType.Absolute, 30F)
                }
            };

            // Right panel (animation controls and parameters)
            rightPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                RowStyles = {
                    new RowStyle(SizeType.Absolute, 30F),
                    new RowStyle(SizeType.Absolute, 40F),
                    new RowStyle(SizeType.Absolute, 40F),
                    new RowStyle(SizeType.Percent, 100F)
                }
            };

            // Create controls
            Label objectLabel = new Label
            {
                Text = "Select Object:",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            objectsBox = new ListBox
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle
            };

            Label animLabel = new Label
            {
                Text = "Animations:",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            animBox = new ListBox
            {
                Dock = DockStyle.Fill,
                ItemHeight = 20,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Animation control buttons
            var buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false
            };

            animStartBtn = new Button
            {
                Text = "Start",
                Size = new System.Drawing.Size(80, 30)
            };

            animStopBtn = new Button
            {
                Text = "Stop",
                Size = new System.Drawing.Size(80, 30),
                Enabled = false
            };

            removeAnimationBtn = new Button
            {
                Text = "Remove",
                Size = new System.Drawing.Size(80, 30),
                Enabled = false
            };

            buttonPanel.Controls.AddRange(new Control[] { animStartBtn, animStopBtn, removeAnimationBtn });

            // Animation type selection
            typeLabel = new Label
            {
                Text = "Animation Type:",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            animationTypeCombo = new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            // Add animation button
            addAnimationBtn = new Button
            {
                Text = "Add Animation",
                Dock = DockStyle.Fill,
                Enabled = false
            };

            // Parameter panel
            parameterPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                AutoScroll = true
            };

            // Add controls to panels
            leftPanel.Controls.Add(objectLabel, 0, 0);
            leftPanel.Controls.Add(objectsBox, 0, 1);
            leftPanel.Controls.Add(animLabel, 0, 2);
            leftPanel.Controls.Add(animBox, 0, 3);
            leftPanel.Controls.Add(buttonPanel, 0, 4);

            rightPanel.Controls.Add(typeLabel, 0, 0);
            rightPanel.Controls.Add(animationTypeCombo, 0, 1);
            rightPanel.Controls.Add(addAnimationBtn, 0, 2);
            rightPanel.Controls.Add(parameterPanel, 0, 3);

            // Add panels to main layout
            mainLayout.Controls.Add(leftPanel, 0, 0);
            mainLayout.Controls.Add(rightPanel, 1, 0);

            // Add main layout to form
            this.Controls.Add(mainLayout);

            // Add event handlers
            objectsBox.SelectedIndexChanged += objectsBox_SelectedIndexChanged;
            animBox.SelectedIndexChanged += animBox_SelectedIndexChanged;
            animStartBtn.Click += animStartBtn_Click;
            animStopBtn.Click += animStopBtn_Click;
            addAnimationBtn.Click += addAnimationBtn_Click;
            removeAnimationBtn.Click += removeAnimationBtn_Click;
            animationTypeCombo.SelectedIndexChanged += animationTypeCombo_SelectedIndexChanged;
        }

        private void InitializeUI()
        {
            // Fill objects list
            foreach (var obj in sceneState.Objects)
            {
                objectsBox.Items.Add(obj.Name);
            }

            // Fill animation types
            animationTypeCombo.Items.AddRange(new string[] {
                "Rotation",
                "Scale",
                "Translation"
            });
        }

        private void objectsBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (objectsBox.SelectedItem != null)
            {
                selectedObject = sceneState.Objects.Find(o => o.Name.Equals(objectsBox.SelectedItem.ToString()));
                if (selectedObject != null)
                {
                    UpdateAnimationsList();
                    addAnimationBtn.Enabled = true;
                }
            }
        }

        private void UpdateAnimationsList()
        {
            animBox.Items.Clear();
            foreach (var anim in sceneState.Animations.Where(a => a.TargetObject == selectedObject))
            {
                animBox.Items.Add(anim.Name);
            }
        }

        private void animBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            lock (sceneState)
            {
                if (animBox.SelectedIndex >= 0 && animBox.SelectedIndex < sceneState.Animations.Count)
                {
                    selectedAnimation = sceneState.GetAnimation(animBox.SelectedIndex);
                    animStartBtn.Enabled = true;
                    animStopBtn.Enabled = false;
                    removeAnimationBtn.Enabled = true;
                    UpdateParameterPanel();
                }
            }
        }

        private void animationTypeCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateParameterPanel();
        }

        private void UpdateParameterPanel()
        {
            parameterPanel.Controls.Clear();
            activeControls.Clear();

            if (selectedAnimation == null && animationTypeCombo.SelectedItem == null)
                return;

            string animType = selectedAnimation?.GetType().Name ?? animationTypeCombo.SelectedItem.ToString();
            int yOffset = 10;

            switch (animType)
            {
                case "RotationAnimation":
                    AddVector3Controls("Rotation Speed", yOffset, (anim) => ((RotationAnimation)anim).RotationSpeed, 
                        (anim, value) => ((RotationAnimation)anim).RotationSpeed = value);
                    break;
                case "ScaleAnimation":
                    AddVector3Controls("Scale Speed", yOffset, (anim) => ((ScaleAnimation)anim).ScaleSpeed,
                        (anim, value) => ((ScaleAnimation)anim).ScaleSpeed = value);
                    AddVector3Controls("Min Scale", yOffset + 100, (anim) => ((ScaleAnimation)anim).MinScale,
                        (anim, value) => ((ScaleAnimation)anim).MinScale = value);
                    AddVector3Controls("Max Scale", yOffset + 200, (anim) => ((ScaleAnimation)anim).MaxScale,
                        (anim, value) => ((ScaleAnimation)anim).MaxScale = value);
                    break;
                case "TranslationAnimation":
                    AddVector3Controls("Movement Speed", yOffset, (anim) => ((TranslationAnimation)anim).MovementSpeed,
                        (anim, value) => ((TranslationAnimation)anim).MovementSpeed = value);
                    AddVector3Controls("End Position", yOffset + 100, (anim) => ((TranslationAnimation)anim).EndPosition,
                        (anim, value) => ((TranslationAnimation)anim).EndPosition = value);
                    break;
            }
        }

        private void AddVector3Controls(string label, int yOffset, 
            Func<AnimationSystem, Vector3> getValue, 
            Action<AnimationSystem, Vector3> setValue)
        {
            Label lbl = new Label
            {
                Text = label,
                Location = new System.Drawing.Point(10, yOffset),
                AutoSize = true
            };

            NumericUpDown xNum = new NumericUpDown
            {
                Location = new System.Drawing.Point(10, yOffset + 20),
                Size = new System.Drawing.Size(80, 20),
                DecimalPlaces = 2,
                Increment = 0.1m,
                Minimum = -100m,
                Maximum = 100m
            };

            NumericUpDown yNum = new NumericUpDown
            {
                Location = new System.Drawing.Point(100, yOffset + 20),
                Size = new System.Drawing.Size(80, 20),
                DecimalPlaces = 2,
                Increment = 0.1m,
                Minimum = -100m,
                Maximum = 100m
            };

            NumericUpDown zNum = new NumericUpDown
            {
                Location = new System.Drawing.Point(190, yOffset + 20),
                Size = new System.Drawing.Size(80, 20),
                DecimalPlaces = 2,
                Increment = 0.1m,
                Minimum = -100m,
                Maximum = 100m
            };

            if (selectedAnimation != null)
            {
                Vector3 value = getValue(selectedAnimation);
                xNum.Value = (decimal)value.X;
                yNum.Value = (decimal)value.Y;
                zNum.Value = (decimal)value.Z;
            }

            // Add value changed handlers
            EventHandler valueChangedHandler = (s, e) =>
            {
                if (selectedAnimation != null)
                {
                    Vector3 newValue = new Vector3((float)xNum.Value, (float)yNum.Value, (float)zNum.Value);
                    setValue(selectedAnimation, newValue);
                }
            };

            xNum.ValueChanged += valueChangedHandler;
            yNum.ValueChanged += valueChangedHandler;
            zNum.ValueChanged += valueChangedHandler;

            activeControls.AddRange(new[] { xNum, yNum, zNum });
            parameterPanel.Controls.AddRange(new Control[] { lbl, xNum, yNum, zNum });
        }

        private void addAnimationBtn_Click(object sender, EventArgs e)
        {
            if (selectedObject == null || animationTypeCombo.SelectedItem == null)
                return;

            string animType = animationTypeCombo.SelectedItem.ToString();
            AnimationSystem newAnimation = null;

            switch (animType)
            {
                case "Rotation":
                    newAnimation = new RotationAnimation
                    {
                        Name = $"Rotation_{selectedObject.Name}",
                        TargetObject = selectedObject,
                        RotationSpeed = new Vector3(30, 0, 0)
                    };
                    break;
                case "Scale":
                    newAnimation = new ScaleAnimation
                    {
                        Name = $"Scale_{selectedObject.Name}",
                        TargetObject = selectedObject,
                        ScaleSpeed = new Vector3(1, 1, 1)
                    };
                    break;
                case "Translation":
                    newAnimation = new TranslationAnimation
                    {
                        Name = $"Translation_{selectedObject.Name}",
                        TargetObject = selectedObject,
                        MovementSpeed = new Vector3(1, 1, 1),
                        EndPosition = selectedObject.Position + new Vector3(5, 0, 0)
                    };
                    break;
            }

            if (newAnimation != null)
            {
                sceneState.Animations.Add(newAnimation);
                UpdateAnimationsList();
                selectedAnimation = newAnimation;
                UpdateParameterPanel();
            }
        }

        private void removeAnimationBtn_Click(object sender, EventArgs e)
        {
            if (selectedAnimation != null)
            {
                sceneState.Animations.Remove(selectedAnimation);
                UpdateAnimationsList();
                selectedAnimation = null;
                UpdateParameterPanel();
                removeAnimationBtn.Enabled = false;
            }
        }

        private void animStartBtn_Click(object sender, EventArgs e)
        {
            lock (sceneState)
            {
                if (selectedAnimation != null)
                {
                    selectedAnimation.Start();
                    animStartBtn.Enabled = false;
                    animStopBtn.Enabled = true;
                }
            }
        }

        private void animStopBtn_Click(object sender, EventArgs e)
        {
            if (selectedAnimation != null)
            {
                selectedAnimation.Stop();
                animStartBtn.Enabled = true;
                animStopBtn.Enabled = false;
            }
        }
    }
} 