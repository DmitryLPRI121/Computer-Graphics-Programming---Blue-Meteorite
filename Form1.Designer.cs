// Form1.Designer.cs
using Tao.Platform.Windows;

namespace Computer_Graphics_Programming___Blue_Meteorite
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.sceneMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Exit = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpDialog = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.XCoordinateStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.YCoordinateStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.ZCoordinateStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.actionStrip = new System.Windows.Forms.ToolStrip();
            this.effectsStrip = new System.Windows.Forms.ToolStrip();
            this.sceneView = new Tao.Platform.Windows.SimpleOpenGlControl();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sceneMenuItem,
            this.settingsMenuItem,
            this.helpDialog});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // sceneMenuItem
            // 
            this.sceneMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Exit});
            this.sceneMenuItem.Name = "sceneMenuItem";
            this.sceneMenuItem.Size = new System.Drawing.Size(53, 20);
            this.sceneMenuItem.Text = "Сцена";
            // 
            // Exit
            // 
            this.Exit.Name = "Exit";
            this.Exit.Size = new System.Drawing.Size(109, 22);
            this.Exit.Text = "Выход";
            this.Exit.Click += new System.EventHandler(this.Exit_Click);
            // 
            // settingsMenuItem
            // 
            this.settingsMenuItem.Name = "settingsMenuItem";
            this.settingsMenuItem.Size = new System.Drawing.Size(79, 20);
            this.settingsMenuItem.Text = "Настройки";
            // 
            // helpDialog
            // 
            this.helpDialog.Name = "helpDialog";
            this.helpDialog.Size = new System.Drawing.Size(65, 20);
            this.helpDialog.Text = "Справка";
            this.helpDialog.Click += new System.EventHandler(this.helpDialog_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.XCoordinateStatus,
            this.YCoordinateStatus,
            this.ZCoordinateStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 428);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(800, 22);
            this.statusStrip1.TabIndex = 5;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // XCoordinateStatus
            // 
            this.XCoordinateStatus.Name = "XCoordinateStatus";
            this.XCoordinateStatus.Size = new System.Drawing.Size(17, 17);
            this.XCoordinateStatus.Text = "X:";
            // 
            // YCoordinateStatus
            // 
            this.YCoordinateStatus.Name = "YCoordinateStatus";
            this.YCoordinateStatus.Size = new System.Drawing.Size(17, 17);
            this.YCoordinateStatus.Text = "Y:";
            // 
            // ZCoordinateStatus
            // 
            this.ZCoordinateStatus.Name = "ZCoordinateStatus";
            this.ZCoordinateStatus.Size = new System.Drawing.Size(17, 17);
            this.ZCoordinateStatus.Text = "Z:";
            // 
            // actionStrip
            // 
            this.actionStrip.Dock = System.Windows.Forms.DockStyle.Left;
            this.actionStrip.Location = new System.Drawing.Point(0, 24);
            this.actionStrip.Name = "actionStrip";
            this.actionStrip.Size = new System.Drawing.Size(26, 404);
            this.actionStrip.TabIndex = 6;
            this.actionStrip.Text = "toolStrip1";
            // 
            // effectsStrip
            // 
            this.effectsStrip.Dock = System.Windows.Forms.DockStyle.Right;
            this.effectsStrip.Location = new System.Drawing.Point(774, 24);
            this.effectsStrip.Name = "effectsStrip";
            this.effectsStrip.Size = new System.Drawing.Size(26, 404);
            this.effectsStrip.TabIndex = 7;
            this.effectsStrip.Text = "toolStrip1";
            // 
            // sceneView
            // 
            this.sceneView.AccumBits = ((byte)(0));
            this.sceneView.AutoCheckErrors = false;
            this.sceneView.AutoFinish = false;
            this.sceneView.AutoMakeCurrent = true;
            this.sceneView.AutoSwapBuffers = true;
            this.sceneView.BackColor = System.Drawing.Color.Black;
            this.sceneView.ColorBits = ((byte)(32));
            this.sceneView.DepthBits = ((byte)(16));
            this.sceneView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sceneView.Location = new System.Drawing.Point(26, 24);
            this.sceneView.Name = "sceneView";
            this.sceneView.Size = new System.Drawing.Size(748, 404);
            this.sceneView.StencilBits = ((byte)(0));
            this.sceneView.TabIndex = 8;
            this.sceneView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.sceneView_MouseDown);
            this.sceneView.MouseMove += new System.Windows.Forms.MouseEventHandler(this.sceneView_MouseMove);
            this.sceneView.MouseUp += new System.Windows.Forms.MouseEventHandler(this.sceneView_MouseUp);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.sceneView);
            this.Controls.Add(this.effectsStrip);
            this.Controls.Add(this.actionStrip);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Сцена - Голубой метеорит";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResizeEnd += new System.EventHandler(this.Form1_Resize);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem sceneMenuItem;
        private System.Windows.Forms.ToolStripMenuItem Exit;
        private System.Windows.Forms.ToolStripMenuItem settingsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpDialog;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel XCoordinateStatus;
        private System.Windows.Forms.ToolStripStatusLabel YCoordinateStatus;
        private System.Windows.Forms.ToolStripStatusLabel ZCoordinateStatus;
        private System.Windows.Forms.ToolStrip actionStrip;
        private System.Windows.Forms.ToolStrip effectsStrip;
        private SimpleOpenGlControl sceneView;
    }
}

