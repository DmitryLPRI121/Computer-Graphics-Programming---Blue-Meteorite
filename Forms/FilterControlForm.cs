using System;
using System.Windows.Forms;
using Computer_Graphics_Programming_Blue_Meteorite.Graphics;

namespace Computer_Graphics_Programming_Blue_Meteorite.Forms
{
    public partial class FilterControlForm : Form
    {
        private GrayscaleFilter grayscaleFilter;
        private SepiaFilter sepiaFilter;
        private BlurFilter blurFilter;
        private PixelizedFilter pixelizedFilter;
        private NightVisionFilter nightVisionFilter;
        private ComboBox filterComboBox;
        private TrackBar blurStrengthTrackBar;
        private Label blurStrengthLabel;
        private TrackBar pixelSizeTrackBar;
        private Label pixelSizeLabel;
        private TrackBar noiseStrengthTrackBar;
        private Label noiseStrengthLabel;

        public FilterControlForm(GrayscaleFilter grayscaleFilter, SepiaFilter sepiaFilter, BlurFilter blurFilter, PixelizedFilter pixelizedFilter, NightVisionFilter nightVisionFilter)
        {
            InitializeComponent();
            this.grayscaleFilter = grayscaleFilter;
            this.sepiaFilter = sepiaFilter;
            this.blurFilter = blurFilter;
            this.pixelizedFilter = pixelizedFilter;
            this.nightVisionFilter = nightVisionFilter;
            InitializeControls();
        }

        private void InitializeControls()
        {
            // Create filter selection label
            Label filterLabel = new Label
            {
                Text = "Filter:",
                Location = new System.Drawing.Point(20, 20),
                AutoSize = true
            };

            // Create filter selection combo box
            filterComboBox = new ComboBox
            {
                Location = new System.Drawing.Point(20, 50),
                Size = new System.Drawing.Size(200, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            // Add filter options
            filterComboBox.Items.AddRange(new string[] { "Standard", "Grayscale", "Sepia", "Blur", "Pixelized", "Night Vision" });
            
            // Set initial selection based on enabled filters
            if (nightVisionFilter.IsEnabled)
                filterComboBox.SelectedIndex = 5;
            else if (pixelizedFilter.IsEnabled)
                filterComboBox.SelectedIndex = 4;
            else if (blurFilter.IsEnabled)
                filterComboBox.SelectedIndex = 3;
            else if (sepiaFilter.IsEnabled)
                filterComboBox.SelectedIndex = 2;
            else if (grayscaleFilter.IsEnabled)
                filterComboBox.SelectedIndex = 1;
            else
                filterComboBox.SelectedIndex = 0;

            // Create blur strength controls
            blurStrengthLabel = new Label
            {
                Text = "Blur Strength:",
                Location = new System.Drawing.Point(20, 90),
                AutoSize = true,
                Visible = false
            };

            blurStrengthTrackBar = new TrackBar
            {
                Location = new System.Drawing.Point(20, 120),
                Size = new System.Drawing.Size(200, 45),
                Minimum = 1,
                Maximum = 10,
                Value = (int)(blurFilter.BlurStrength * 5),
                Visible = false
            };

            // Create pixel size controls
            pixelSizeLabel = new Label
            {
                Text = "Pixel Size:",
                Location = new System.Drawing.Point(20, 90),
                AutoSize = true,
                Visible = false
            };

            pixelSizeTrackBar = new TrackBar
            {
                Location = new System.Drawing.Point(20, 120),
                Size = new System.Drawing.Size(200, 45),
                Minimum = 1,
                Maximum = 50,
                Value = (int)pixelizedFilter.PixelSize,
                Visible = false
            };

            // Create noise strength controls for night vision
            noiseStrengthLabel = new Label
            {
                Text = "Noise Strength:",
                Location = new System.Drawing.Point(20, 90),
                AutoSize = true,
                Visible = false
            };

            noiseStrengthTrackBar = new TrackBar
            {
                Location = new System.Drawing.Point(20, 120),
                Size = new System.Drawing.Size(200, 45),
                Minimum = 1,
                Maximum = 10,
                Value = (int)(nightVisionFilter.NoiseStrength * 10),
                Visible = false
            };

            // Add event handlers
            filterComboBox.SelectedIndexChanged += (sender, e) =>
            {
                grayscaleFilter.IsEnabled = filterComboBox.SelectedIndex == 1;
                sepiaFilter.IsEnabled = filterComboBox.SelectedIndex == 2;
                blurFilter.IsEnabled = filterComboBox.SelectedIndex == 3;
                pixelizedFilter.IsEnabled = filterComboBox.SelectedIndex == 4;
                nightVisionFilter.IsEnabled = filterComboBox.SelectedIndex == 5;

                // Show/hide controls based on selected filter
                bool showBlurControls = filterComboBox.SelectedIndex == 3;
                bool showPixelControls = filterComboBox.SelectedIndex == 4;
                bool showNoiseControls = filterComboBox.SelectedIndex == 5;

                blurStrengthLabel.Visible = showBlurControls;
                blurStrengthTrackBar.Visible = showBlurControls;
                pixelSizeLabel.Visible = showPixelControls;
                pixelSizeTrackBar.Visible = showPixelControls;
                noiseStrengthLabel.Visible = showNoiseControls;
                noiseStrengthTrackBar.Visible = showNoiseControls;
            };

            blurStrengthTrackBar.ValueChanged += (sender, e) =>
            {
                blurFilter.BlurStrength = blurStrengthTrackBar.Value / 5.0f;
            };

            pixelSizeTrackBar.ValueChanged += (sender, e) =>
            {
                pixelizedFilter.PixelSize = pixelSizeTrackBar.Value;
            };

            noiseStrengthTrackBar.ValueChanged += (sender, e) =>
            {
                nightVisionFilter.NoiseStrength = noiseStrengthTrackBar.Value / 10.0f;
            };

            // Add controls to form
            this.Controls.AddRange(new Control[] { 
                filterLabel, 
                filterComboBox,
                blurStrengthLabel,
                blurStrengthTrackBar,
                pixelSizeLabel,
                pixelSizeTrackBar,
                noiseStrengthLabel,
                noiseStrengthTrackBar
            });

            // Update form size
            this.Size = new System.Drawing.Size(300, 200);
        }
    }
} 