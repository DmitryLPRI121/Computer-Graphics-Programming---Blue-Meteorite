using OpenTK.Mathematics;

namespace Computer_Graphics_Programming_Blue_Meteorite
{
    public partial class ParticleSystemControlForm : Form
    {
        private SceneState sceneState;
        private ParticleSystem selectedParticleSystem;

        public ParticleSystemControlForm(SceneState state)
        {
            sceneState = state;
            InitializeComponent();
            InitializeUI();
        }

        private void InitializeComponent()
        {
            this.Text = "Particle System Control";
            this.Size = new System.Drawing.Size(400, 300);

            // Create controls
            Label particleSystemLabel = new Label
            {
                Text = "Select Particle System:",
                Location = new System.Drawing.Point(10, 10),
                AutoSize = true
            };

            particleSystemsBox = new ComboBox
            {
                Location = new System.Drawing.Point(10, 30),
                Size = new System.Drawing.Size(200, 30),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            startParticleSystem = new Button
            {
                Text = "Start",
                Location = new System.Drawing.Point(220, 30),
                Size = new System.Drawing.Size(80, 30),
                Enabled = true
            };

            stopParticleSystem = new Button
            {
                Text = "Stop",
                Location = new System.Drawing.Point(220, 70),
                Size = new System.Drawing.Size(80, 30),
                Enabled = false
            };

            // Add controls to form
            this.Controls.AddRange(new Control[] { particleSystemLabel, particleSystemsBox, startParticleSystem, stopParticleSystem });

            // Add event handlers
            particleSystemsBox.SelectedIndexChanged += particleSystemsBox_SelectedIndexChanged;
            startParticleSystem.Click += startParticleSystem_Click;
            stopParticleSystem.Click += stopParticleSystem_Click;
        }

        private void InitializeUI()
        {
            // Fill particle systems list
            foreach (var particleSystem in sceneState.ParticleSystems)
            {
                particleSystemsBox.Items.Add(particleSystem.Name);
            }
        }

        private void particleSystemsBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (particleSystemsBox.SelectedItem != null)
            {
                selectedParticleSystem = sceneState.ParticleSystems.Find(p => p.Name.Equals(particleSystemsBox.SelectedItem.ToString()));
                if (selectedParticleSystem != null)
                {
                    startParticleSystem.Enabled = true;
                    stopParticleSystem.Enabled = false;
                }
            }
        }

        private void startParticleSystem_Click(object sender, EventArgs e)
        {
            if (selectedParticleSystem != null)
            {
                selectedParticleSystem.Start();
                startParticleSystem.Enabled = false;
                stopParticleSystem.Enabled = true;
            }
        }

        private void stopParticleSystem_Click(object sender, EventArgs e)
        {
            if (selectedParticleSystem != null)
            {
                selectedParticleSystem.Stop();
                startParticleSystem.Enabled = true;
                stopParticleSystem.Enabled = false;
            }
        }

        private ComboBox particleSystemsBox;
        private Button startParticleSystem;
        private Button stopParticleSystem;
    }
} 