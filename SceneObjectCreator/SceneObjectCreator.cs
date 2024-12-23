using Tao.OpenGl;

namespace Computer_Graphics_Programming___Blue_Meteorite
{
    public class Transform
    {
        public float[] Position { get; private set; } = { 0f, 0f, 0f };
        public float[] Rotation { get; private set; } = { 0f, 0f, 0f };
        public float Scale { get; private set; } = 1.0f;

        public Transform SetPosition(float x, float y, float z)
        {
            Position[0] = x;
            Position[1] = y;
            Position[2] = z;
            return this;
        }

        public Transform SetRotation(float x, float y, float z)
        {
            Rotation[0] = x;
            Rotation[1] = y;
            Rotation[2] = z;
            return this;
        }

        public Transform SetScale(float scale)
        {
            Scale = scale;
            return this;
        }

        public void ApplyTransformations()
        {
            Gl.glTranslatef(Position[0], Position[1], Position[2]);
            Gl.glRotatef(Rotation[0], 1.0f, 0.0f, 0.0f);
            Gl.glRotatef(Rotation[1], 0.0f, 1.0f, 0.0f);
            Gl.glRotatef(Rotation[2], 0.0f, 0.0f, 1.0f);
            Gl.glScalef(Scale, Scale, Scale);
        }
    }

    public class Object3D
    {
        private readonly Transform _transform = new Transform();

        public Object3D SetPosition(float x, float y, float z)
        {
            _transform.SetPosition(x, y, z);
            return this;
        }

        public Object3D SetRotation(float x, float y, float z)
        {
            _transform.SetRotation(x, y, z);
            return this;
        }

        public Object3D SetScale(float scale)
        {
            _transform.SetScale(scale);
            return this;
        }

        public void DrawPlane(float width, float height)
        {
            _transform.ApplyTransformations();

            Gl.glBegin(Gl.GL_QUADS);

            Gl.glVertex3f(-width / 2, 0, -height / 2);
            Gl.glVertex3f(width / 2, 0, -height / 2);
            Gl.glVertex3f(width / 2, 0, height / 2);
            Gl.glVertex3f(-width / 2, 0, height / 2);

            Gl.glEnd();
        }

        public void DrawCube(float size)
        {
            _transform.ApplyTransformations();

            float half = size / 2;

            Gl.glBegin(Gl.GL_QUADS);

            // Top face
            Gl.glVertex3f(-half, half, -half);
            Gl.glVertex3f(half, half, -half);
            Gl.glVertex3f(half, half, half);
            Gl.glVertex3f(-half, half, half);

            // Bottom face
            Gl.glVertex3f(-half, -half, -half);
            Gl.glVertex3f(half, -half, -half);
            Gl.glVertex3f(half, -half, half);
            Gl.glVertex3f(-half, -half, half);

            // Front face
            Gl.glVertex3f(-half, -half, half);
            Gl.glVertex3f(half, -half, half);
            Gl.glVertex3f(half, half, half);
            Gl.glVertex3f(-half, half, half);

            // Back face
            Gl.glVertex3f(-half, -half, -half);
            Gl.glVertex3f(half, -half, -half);
            Gl.glVertex3f(half, half, -half);
            Gl.glVertex3f(-half, half, -half);

            // Left face
            Gl.glVertex3f(-half, -half, -half);
            Gl.glVertex3f(-half, -half, half);
            Gl.glVertex3f(-half, half, half);
            Gl.glVertex3f(-half, half, -half);

            // Right face
            Gl.glVertex3f(half, -half, -half);
            Gl.glVertex3f(half, -half, half);
            Gl.glVertex3f(half, half, half);
            Gl.glVertex3f(half, half, -half);

            Gl.glEnd();
        }

        public void DrawSphere(float radius, int slices, int stacks)
        {
            _transform.ApplyTransformations();

            Glu.GLUquadric sphere = Glu.gluNewQuadric();
            Glu.gluQuadricDrawStyle(sphere, Glu.GLU_FILL);

            Glu.gluSphere(sphere, radius, slices, stacks);

            Glu.gluDeleteQuadric(sphere);
        }
    }

    public static class CreateObject
    {
        public static Object3D Cube(float size)
        {
            var obj = new Object3D();
            obj.DrawCube(size);
            return obj;
        }

        public static Object3D Plane(float width, float height)
        {
            var obj = new Object3D();
            obj.DrawPlane(width, height);
            return obj;
        }

        public static Object3D Sphere(float radius, int slices, int stacks)
        {
            var obj = new Object3D();
            obj.DrawSphere(radius, slices, stacks);
            return obj;
        }
    }
}