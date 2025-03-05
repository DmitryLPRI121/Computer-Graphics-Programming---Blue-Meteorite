namespace Computer_Graphics_Programming_Blue_Meteorite
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
            menuStrip1 = new MenuStrip();
            сценаToolStripMenuItem = new ToolStripMenuItem();
            выходToolStripMenuItem = new ToolStripMenuItem();
            настройкиToolStripMenuItem = new ToolStripMenuItem();
            справкаToolStripMenuItem = new ToolStripMenuItem();
            statusStrip1 = new StatusStrip();
            cameraPositionX = new ToolStripStatusLabel();
            cameraPositionY = new ToolStripStatusLabel();
            cameraPositionZ = new ToolStripStatusLabel();
            glControl1 = new OpenTK.GLControl.GLControl();
            menuStrip1.SuspendLayout();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { сценаToolStripMenuItem, настройкиToolStripMenuItem, справкаToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(800, 24);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // сценаToolStripMenuItem
            // 
            сценаToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { выходToolStripMenuItem });
            сценаToolStripMenuItem.Name = "сценаToolStripMenuItem";
            сценаToolStripMenuItem.Size = new Size(53, 20);
            сценаToolStripMenuItem.Text = "Сцена";
            // 
            // выходToolStripMenuItem
            // 
            выходToolStripMenuItem.Name = "выходToolStripMenuItem";
            выходToolStripMenuItem.Size = new Size(109, 22);
            выходToolStripMenuItem.Text = "Выход";
            выходToolStripMenuItem.Click += выходToolStripMenuItem_Click;
            // 
            // настройкиToolStripMenuItem
            // 
            настройкиToolStripMenuItem.Name = "настройкиToolStripMenuItem";
            настройкиToolStripMenuItem.Size = new Size(79, 20);
            настройкиToolStripMenuItem.Text = "Настройки";
            // 
            // справкаToolStripMenuItem
            // 
            справкаToolStripMenuItem.Name = "справкаToolStripMenuItem";
            справкаToolStripMenuItem.Size = new Size(65, 20);
            справкаToolStripMenuItem.Text = "Справка";
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { cameraPositionX, cameraPositionY, cameraPositionZ });
            statusStrip1.Location = new Point(0, 428);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(800, 22);
            statusStrip1.TabIndex = 1;
            statusStrip1.Text = "statusStrip1";
            // 
            // cameraPositionX
            // 
            cameraPositionX.Name = "cameraPositionX";
            cameraPositionX.Size = new Size(16, 17);
            cameraPositionX.Text = "x:";
            // 
            // cameraPositionY
            // 
            cameraPositionY.Name = "cameraPositionY";
            cameraPositionY.Size = new Size(16, 17);
            cameraPositionY.Text = "y:";
            // 
            // cameraPositionZ
            // 
            cameraPositionZ.Name = "cameraPositionZ";
            cameraPositionZ.Size = new Size(15, 17);
            cameraPositionZ.Text = "z:";
            // 
            // glControl1
            // 
            glControl1.API = OpenTK.Windowing.Common.ContextAPI.OpenGL;
            glControl1.APIVersion = new Version(3, 3, 0, 0);
            glControl1.Dock = DockStyle.Fill;
            glControl1.Flags = OpenTK.Windowing.Common.ContextFlags.Default;
            glControl1.IsEventDriven = true;
            glControl1.Location = new Point(0, 24);
            glControl1.Name = "glControl1";
            glControl1.Profile = OpenTK.Windowing.Common.ContextProfile.Core;
            glControl1.SharedContext = null;
            glControl1.Size = new Size(800, 404);
            glControl1.TabIndex = 2;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(glControl1);
            Controls.Add(statusStrip1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "Form1";
            Text = "Сцена - Голубой метеорит";
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem сценаToolStripMenuItem;
        private ToolStripMenuItem выходToolStripMenuItem;
        private ToolStripMenuItem настройкиToolStripMenuItem;
        private ToolStripMenuItem справкаToolStripMenuItem;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel cameraPositionX;
        private ToolStripStatusLabel cameraPositionY;
        private ToolStripStatusLabel cameraPositionZ;
        private OpenTK.GLControl.GLControl glControl1;
    }
}
