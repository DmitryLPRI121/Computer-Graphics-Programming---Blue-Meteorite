using OpenTK.Mathematics;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Computer_Graphics_Programming_Blue_Meteorite
{
    public partial class Form1 : Form
    {
        private SceneState sceneState;
        private Scene scene;
        private AnimationControlForm animationForm;
        private ParticleSystemControlForm particleForm;
        private LightControlForm lightForm;
        private ObjectTransformForm transformForm;
        private SceneSettingsForm skyboxForm;

        public Form1(SceneState state, Scene scene)
        {
            this.sceneState = state;
            this.scene = scene;
            InitializeUI();
        }

        private void InitializeUI()
        {
            // Create buttons to open control forms
            Button animationBtn = new Button
            {
                Text = "Animation Control",
                Location = new System.Drawing.Point(10, 10),
                Size = new System.Drawing.Size(200, 30)
            };

            Button particleBtn = new Button
            {
                Text = "Particle System Control",
                Location = new System.Drawing.Point(10, 50),
                Size = new System.Drawing.Size(200, 30)
            };

            Button lightBtn = new Button
            {
                Text = "Light Control",
                Location = new System.Drawing.Point(10, 90),
                Size = new System.Drawing.Size(200, 30)
            };

            Button transformBtn = new Button
            {
                Text = "Object Transform Control",
                Location = new System.Drawing.Point(10, 130),
                Size = new System.Drawing.Size(200, 30)
            };

            Button skyboxBtn = new Button
            {
                Text = "Skybox Control",
                Location = new System.Drawing.Point(10, 170),
                Size = new System.Drawing.Size(200, 30)
            };

            // Add controls to form
            this.Controls.AddRange(new Control[] { animationBtn, particleBtn, lightBtn, transformBtn, skyboxBtn });

            // Add event handlers
            animationBtn.Click += (s, e) => OpenForm(ref animationForm, () => new AnimationControlForm(sceneState));
            particleBtn.Click += (s, e) => OpenForm(ref particleForm, () => new ParticleSystemControlForm(sceneState));
            lightBtn.Click += (s, e) => OpenForm(ref lightForm, () => new LightControlForm(sceneState));
            transformBtn.Click += (s, e) => OpenForm(ref transformForm, () => new ObjectTransformForm(sceneState));
            skyboxBtn.Click += (s, e) => OpenForm(ref skyboxForm, () => new SceneSettingsForm(scene, sceneState));

            // Initialize all control forms
            animationForm = new AnimationControlForm(sceneState);
            particleForm = new ParticleSystemControlForm(sceneState);
            lightForm = new LightControlForm(sceneState);
            transformForm = new ObjectTransformForm(sceneState);
            skyboxForm = new SceneSettingsForm(scene, sceneState);
        }

        private void OpenForm<T>(ref T form, Func<T> createForm) where T : Form
        {
            if (form == null || form.IsDisposed)
            {
                form = createForm();
            }
            form.Show();
        }
    }
}
