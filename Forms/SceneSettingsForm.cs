using System;
using System.Drawing;
using System.Windows.Forms;

namespace Computer_Graphics_Programming_Blue_Meteorite
{
    public class SceneSettingsForm : Form
    {
        private SceneSettings scene;
        private SceneObjects sceneState;
        private TabControl tabControl;
        private TrackBar timeOfDaySlider;

        public SceneSettingsForm(SceneSettings scene, SceneObjects sceneState)
        {
            this.scene = scene;
            this.sceneState = sceneState;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Scene Settings";
            this.Size = new Size(400, 300);

            tabControl = new TabControl();
            tabControl.Dock = DockStyle.Fill;

            // Add Skybox tab
            TabPage skyboxTab = new TabPage("Skybox");
            skyboxTab.Padding = new Padding(10);

            // Time of day slider
            Label timeOfDayLabel = new Label();
            timeOfDayLabel.Text = "Time of Day:";
            timeOfDayLabel.Location = new Point(10, 20);
            timeOfDayLabel.AutoSize = true;
            skyboxTab.Controls.Add(timeOfDayLabel);

            timeOfDaySlider = new TrackBar();
            timeOfDaySlider.Location = new Point(10, 50);
            timeOfDaySlider.Width = 200;
            timeOfDaySlider.Minimum = 0;
            timeOfDaySlider.Maximum = 100;
            timeOfDaySlider.Value = (int)(sceneState.SkyboxTimeOfDay * 100);
            timeOfDaySlider.TickFrequency = 25;
            timeOfDaySlider.TickStyle = TickStyle.BottomRight;
            timeOfDaySlider.ValueChanged += (s, e) =>
            {
                lock (sceneState)
                {
                    sceneState.SkyboxTimeOfDay = timeOfDaySlider.Value / 100f;
                    if (scene != null)
                    {
                        scene.skybox.SetTimeOfDay(sceneState.SkyboxTimeOfDay);
                        scene.globalLight.SetTimeOfDay(sceneState.SkyboxTimeOfDay);
                    }
                }
            };
            skyboxTab.Controls.Add(timeOfDaySlider);

            // Time labels
            Label dawnLabel = new Label();
            dawnLabel.Text = "Dawn";
            dawnLabel.Location = new Point(10, 80);
            dawnLabel.AutoSize = true;
            skyboxTab.Controls.Add(dawnLabel);

            Label noonLabel = new Label();
            noonLabel.Text = "Noon";
            noonLabel.Location = new Point(90, 80);
            noonLabel.AutoSize = true;
            skyboxTab.Controls.Add(noonLabel);

            Label duskLabel = new Label();
            duskLabel.Text = "Dusk";
            duskLabel.Location = new Point(170, 80);
            duskLabel.AutoSize = true;
            skyboxTab.Controls.Add(duskLabel);

            // Auto-update checkbox
            CheckBox autoUpdateCheckbox = new CheckBox();
            autoUpdateCheckbox.Text = "Auto-update time";
            autoUpdateCheckbox.Location = new Point(10, 110);
            autoUpdateCheckbox.Checked = sceneState.SkyboxAutoUpdate;
            autoUpdateCheckbox.CheckedChanged += (s, e) =>
            {
                lock (sceneState)
                {
                    sceneState.SkyboxAutoUpdate = autoUpdateCheckbox.Checked;
                    if (scene != null)
                    {
                        scene.skybox.SetAutoUpdate(sceneState.SkyboxAutoUpdate);
                        scene.globalLight.SetAutoUpdate(sceneState.SkyboxAutoUpdate);
                    }
                    // Enable/disable time of day slider based on auto-update state
                    timeOfDaySlider.Enabled = !sceneState.SkyboxAutoUpdate;
                }
            };
            skyboxTab.Controls.Add(autoUpdateCheckbox);

            // Set initial state of the time of day slider
            timeOfDaySlider.Enabled = !sceneState.SkyboxAutoUpdate;

            tabControl.TabPages.Add(skyboxTab);
            this.Controls.Add(tabControl);
        }
    }
} 