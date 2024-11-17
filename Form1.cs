using System;
using System.Windows.Forms;

namespace Computer_Graphics_Programming___Blue_Meteorite
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            sceneView.InitializeContexts();
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
