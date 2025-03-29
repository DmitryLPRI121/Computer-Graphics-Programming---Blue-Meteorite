using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Computer_Graphics_Programming_Blue_Meteorite
{
    public class Shader
    {
        public int Handle;

        public Shader(string vertexPath, string fragmentPath)
        {
            if (!File.Exists(vertexPath))
            {
                throw new FileNotFoundException($"Vertex shader file not found: {vertexPath}");
            }
            if (!File.Exists(fragmentPath))
            {
                throw new FileNotFoundException($"Fragment shader file not found: {fragmentPath}");
            }

            string vertexShaderSource = File.ReadAllText(vertexPath);
            string fragmentShaderSource = File.ReadAllText(fragmentPath);

            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            int vertexLength = vertexShaderSource.Length; // Вычисляем длину шейдера
            GL.ShaderSource(vertexShader, 1, new string[] { vertexShaderSource }, new int[] { vertexLength }); // Передаем шейдерный код
            CompileShader(vertexShader);

            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            int fragmentLength = fragmentShaderSource.Length; // Вычисляем длину шейдера
            GL.ShaderSource(fragmentShader, 1, new string[] { fragmentShaderSource }, new int[] { fragmentLength }); // Передаем шейдерный код
            CompileShader(fragmentShader);

            Handle = GL.CreateProgram();
            GL.AttachShader(Handle, vertexShader);
            GL.AttachShader(Handle, fragmentShader);
            GL.LinkProgram(Handle);

            GL.DetachShader(Handle, vertexShader);
            GL.DetachShader(Handle, fragmentShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);
        }

        private void CompileShader(int shader)
        {
            GL.CompileShader(shader);
            GL.GetShader(shader, ShaderParameter.CompileStatus, out int code);
            if (code != (int)All.True)
            {
                string infoLog = GL.GetShaderInfoLog(shader);
                throw new Exception($"Ошибка компиляции шейдера: {infoLog}");
            }
        }

        public void Use()
        {
            GL.UseProgram(Handle);
        }

        public void SetMatrix4(string name, Matrix4 matrix)
        {
            int location = GL.GetUniformLocation(Handle, name);
            GL.UniformMatrix4(location, false, ref matrix);
        }

        internal void SetVector3(string name, Vector3 position)
        {
            int location = GL.GetUniformLocation(Handle, name);
            GL.Uniform3(location, position.X, position.Y, position.Z);
        }
        internal void SetInt(string name, int i)
        {
            int location = GL.GetUniformLocation(Handle, name);
            GL.Uniform1(location, i);
        }

        public void Dispose()
        {
            GL.DeleteProgram(Handle);
        }

        internal void SetFloat(string name, float f)
        {
            int location = GL.GetUniformLocation(Handle, name);
            GL.Uniform1(location, f);
        }
    }
}
