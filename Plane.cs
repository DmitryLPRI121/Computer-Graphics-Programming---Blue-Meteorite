using Tao.OpenGl;

namespace Computer_Graphics_Programming___Blue_Meteorite
{
    public class Plane
    {
        public void Render()
        {
            Gl.glBegin(Gl.GL_QUADS);
            Gl.glColor3f(0.2f, 0.5f, 0.2f);
            Gl.glVertex3f(-100, 0, -100);
            Gl.glVertex3f(-100, 0, 100);
            Gl.glVertex3f(100, 0, 100);
            Gl.glVertex3f(100, 0, -100);
            Gl.glEnd();
        }
    }
}