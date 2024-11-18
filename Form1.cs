using System;
using System.Windows.Forms;

namespace Computer_Graphics_Programming___Blue_Meteorite
{
    public partial class Form1 : Form
    {
        private SceneRenderer sceneRenderer;

        public Form1()
        {
            InitializeComponent();
            sceneRenderer = new SceneRenderer(sceneView);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            sceneView.InitializeContexts();
            sceneRenderer.Initialize();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            sceneRenderer.HandleKeyDown(e);
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            sceneRenderer.HandleKeyUp(e);
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            sceneRenderer.HandleMouseMove(e);
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            sceneRenderer.HandleMouseDown(e);
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            sceneRenderer.HandleMouseUp(e);
        }


        private void Form1_Resize(object sender, EventArgs e)
        {
            sceneRenderer.HandleResize(ClientSize.Width, ClientSize.Height);
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {

        }

        private void statusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
    }
}