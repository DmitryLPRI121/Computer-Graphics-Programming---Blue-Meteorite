using OpenTK.Mathematics;

namespace Computer_Graphics_Programming_Blue_Meteorite
{
    public partial class AnimationControlForm : Form
    {
        private SceneState sceneState;
        private SceneObject selectedObject;
        private IAnimation selectedAnimation;

        public AnimationControlForm(SceneState state)
        {
            sceneState = state;
            InitializeComponent();
            InitializeUI();
        }

        private void InitializeComponent()
        {
            this.Text = "Animation Control";
            this.Size = new System.Drawing.Size(400, 300);

            // Create controls
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

            Label animLabel = new Label
            {
                Text = "Animations:",
                Location = new System.Drawing.Point(10, 140),
                AutoSize = true
            };

            animBox = new ListBox
            {
                Location = new System.Drawing.Point(10, 160),
                Size = new System.Drawing.Size(200, 100)
            };

            animStartBtn = new Button
            {
                Text = "Start",
                Location = new System.Drawing.Point(220, 160),
                Size = new System.Drawing.Size(80, 30)
            };

            animStopBtn = new Button
            {
                Text = "Stop",
                Location = new System.Drawing.Point(220, 200),
                Size = new System.Drawing.Size(80, 30),
                Enabled = false
            };

            // Add controls to form
            this.Controls.AddRange(new Control[] { objectLabel, objectsBox, animLabel, animBox, animStartBtn, animStopBtn });

            // Add event handlers
            objectsBox.SelectedIndexChanged += objectsBox_SelectedIndexChanged;
            animBox.SelectedIndexChanged += animBox_SelectedIndexChanged;
            animStartBtn.Click += animStartBtn_Click;
            animStopBtn.Click += animStopBtn_Click;
        }

        private void InitializeUI()
        {
            // Fill objects list
            foreach (var obj in sceneState.Objects)
            {
                objectsBox.Items.Add(obj.Name);
            }

            // Fill animations list
            foreach (var anim in sceneState.Animations)
            {
                animBox.Items.Add(anim.Name);
            }
        }

        private void objectsBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (objectsBox.SelectedItem != null)
            {
                selectedObject = sceneState.Objects.Find(o => o.Name.Equals(objectsBox.SelectedItem.ToString()));
                if (selectedObject != null)
                {
                    // Update animations list for selected object
                    animBox.Items.Clear();
                    foreach (var anim in sceneState.Animations.Where(a => a.TargetObject == selectedObject))
                    {
                        animBox.Items.Add(anim.Name);
                    }
                }
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
                }
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

        private ListBox objectsBox;
        private ListBox animBox;
        private Button animStartBtn;
        private Button animStopBtn;
    }
} 