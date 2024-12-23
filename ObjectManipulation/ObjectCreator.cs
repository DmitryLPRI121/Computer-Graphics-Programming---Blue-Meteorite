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

        public Transform ChangePosition(float x, float y, float z)
        {
            Position[0] += x;
            Position[1] += y;
            Position[2] += z;
            return this;
        }

        public Transform SetRotation(float x, float y, float z)
        {
            Rotation[0] = x;
            Rotation[1] = y;
            Rotation[2] = z;
            return this;
        }

        public Transform ChangeRotation(float x, float y, float z)
        {
            Rotation[0] += x;
            Rotation[1] += y;
            Rotation[2] += z;
            return this;
        }

        public Transform SetScale(float scale)
        {
            Scale = scale;
            return this;
        }

        public Transform ChangeScale(float scale)
        {
            Scale += scale;
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
        public Transform Transform { get; } = new Transform();

        public Object3D SetPosition(float x, float y, float z)
        {
            Transform.SetPosition(x, y, z);
            return this;
        }

        public Object3D ChangePosition(float x, float y, float z)
        {
            Transform.ChangePosition(x, y, z);
            return this;
        }

        public Object3D SetRotation(float x, float y, float z)
        {
            Transform.SetRotation(x, y, z);
            return this;
        }

        public Object3D ChangeRotation(float x, float y, float z)
        {
            Transform.ChangeRotation(x, y, z);
            return this;
        }

        public Object3D SetScale(float scale)
        {
            Transform.SetScale(scale);
            return this;
        }

        public Object3D ChangeScale(float scale)
        {
            Transform.ChangeScale(scale);
            return this;
        }

        public void RenderCube(float size)
        {
            Transform.ApplyTransformations();

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

        public void RenderSphere(float radius, int slices, int stacks)
        {
            Transform.ApplyTransformations();

            Glu.GLUquadric sphere = Glu.gluNewQuadric();
            Glu.gluQuadricDrawStyle(sphere, Glu.GLU_FILL);

            Glu.gluSphere(sphere, radius, slices, stacks);

            Glu.gluDeleteQuadric(sphere);
        }

        public void RenderCustom(float[][] vertices, int[][] faces)
        {
            Transform.ApplyTransformations();

            Gl.glBegin(Gl.GL_TRIANGLES);

            foreach (var face in faces)
            {
                foreach (var index in face)
                {
                    float[] vertex = vertices[index];
                    Gl.glVertex3f(vertex[0], vertex[1], vertex[2]);
                }
            }

            Gl.glEnd();
        }
    }

    public static class CreateObject
    {
        public static Object3D Cube(float size)
        {
            var obj = new Object3D();
            obj.RenderCube(size);
            return obj;
        }

        public static Object3D Sphere(float radius, int slices, int stacks)
        {
            var obj = new Object3D();
            obj.RenderSphere(radius, slices, stacks);
            return obj;
        }

        public static Object3D Custom(float[][] vertices, int[][] faces)
        {
            var obj = new Object3D();
            obj.RenderCustom(vertices, faces);
            return obj;
        }
    }
}