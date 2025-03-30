using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Computer_Graphics_Programming_Blue_Meteorite
{
    public class Shader
    {
        public int Handle;

        public Shader(string vertexPath, string fragmentPath)
        {
            // Проверяем наличие файлов шейдеров
            if (!File.Exists(vertexPath))
            {
                throw new FileNotFoundException($"Vertex shader file not found: {vertexPath}");
            }
            if (!File.Exists(fragmentPath))
            {
                throw new FileNotFoundException($"Fragment shader file not found: {fragmentPath}");
            }

            // Чтение кода шейдеров из файлов
            string vertexShaderSource;
            string fragmentShaderSource;
            try
            {
                vertexShaderSource = File.ReadAllText(vertexPath);
                fragmentShaderSource = File.ReadAllText(fragmentPath);
                
                // Выводим длину шейдерных кодов для отладки
                Console.WriteLine($"Vertex shader length: {vertexShaderSource.Length} bytes");
                Console.WriteLine($"Fragment shader length: {fragmentShaderSource.Length} bytes");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error reading shader files: {ex.Message}");
            }

            // Компиляция вертексного шейдера
            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexShaderSource);
            
            try
            {
                CompileShader(vertexShader, "Vertex shader");
            }
            catch (Exception ex)
            {
                GL.DeleteShader(vertexShader);
                throw new Exception($"Vertex shader compilation failed: {ex.Message}");
            }

            // Компиляция фрагментного шейдера
            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentShaderSource);
            
            try
            {
                CompileShader(fragmentShader, "Fragment shader");
            }
            catch (Exception ex)
            {
                GL.DeleteShader(vertexShader);
                GL.DeleteShader(fragmentShader);
                throw new Exception($"Fragment shader compilation failed: {ex.Message}");
            }

            // Создание и линковка шейдерной программы
            Handle = GL.CreateProgram();
            GL.AttachShader(Handle, vertexShader);
            GL.AttachShader(Handle, fragmentShader);
            
            try
            {
                LinkProgram();
            }
            catch (Exception ex)
            {
                GL.DetachShader(Handle, vertexShader);
                GL.DetachShader(Handle, fragmentShader);
                GL.DeleteShader(vertexShader);
                GL.DeleteShader(fragmentShader);
                GL.DeleteProgram(Handle);
                throw new Exception($"Shader program linking failed: {ex.Message}");
            }

            // Освобождаем ресурсы шейдеров, они больше не нужны
            GL.DetachShader(Handle, vertexShader);
            GL.DetachShader(Handle, fragmentShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);
            
            Console.WriteLine($"Successfully created shader program (Handle: {Handle})");
        }

        private void CompileShader(int shader, string type)
        {
            // Компилируем шейдер
            GL.CompileShader(shader);
            
            // Проверяем статус компиляции
            GL.GetShader(shader, ShaderParameter.CompileStatus, out int status);
            if (status != (int)All.True)
            {
                string infoLog = GL.GetShaderInfoLog(shader);
                throw new Exception($"Ошибка компиляции {type}: {infoLog}");
            }
        }
        
        private void LinkProgram()
        {
            // Линкуем программу
            GL.LinkProgram(Handle);
            
            // Проверяем статус линковки
            GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out int status);
            if (status != (int)All.True)
            {
                string infoLog = GL.GetProgramInfoLog(Handle);
                throw new Exception($"Ошибка линковки программы: {infoLog}");
            }
            
            // Проверяем статус валидации
            GL.ValidateProgram(Handle);
            GL.GetProgram(Handle, GetProgramParameterName.ValidateStatus, out status);
            if (status != (int)All.True)
            {
                string infoLog = GL.GetProgramInfoLog(Handle);
                Console.WriteLine($"Предупреждение валидации программы: {infoLog}");
            }
        }

        public void Use()
        {
            GL.UseProgram(Handle);
        }

        public void SetMatrix4(string name, Matrix4 matrix)
        {
            int location = GL.GetUniformLocation(Handle, name);
            if (location == -1)
            {
                Console.WriteLine($"Warning: Uniform '{name}' not found in shader program.");
                return;
            }
            GL.UniformMatrix4(location, false, ref matrix);
        }

        internal void SetVector3(string name, Vector3 vector)
        {
            int location = GL.GetUniformLocation(Handle, name);
            if (location == -1)
            {
                Console.WriteLine($"Warning: Uniform '{name}' not found in shader program.");
                return;
            }
            GL.Uniform3(location, vector.X, vector.Y, vector.Z);
        }
        
        internal void SetInt(string name, int value)
        {
            int location = GL.GetUniformLocation(Handle, name);
            if (location == -1)
            {
                Console.WriteLine($"Warning: Uniform '{name}' not found in shader program.");
                return;
            }
            GL.Uniform1(location, value);
        }

        internal void SetFloat(string name, float value)
        {
            int location = GL.GetUniformLocation(Handle, name);
            if (location == -1)
            {
                Console.WriteLine($"Warning: Uniform '{name}' not found in shader program.");
                return;
            }
            GL.Uniform1(location, value);
        }

        public void Dispose()
        {
            GL.DeleteProgram(Handle);
        }
    }
}
