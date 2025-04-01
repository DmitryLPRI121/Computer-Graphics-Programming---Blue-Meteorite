using OpenTK.Mathematics;

namespace Computer_Graphics_Programming_Blue_Meteorite
{
    public class Light
    {
        // Шейдер для рендеринга теней
        private readonly Shader shadowShader;

        // Карта теней
        private readonly ShadowMap shadowMap;

        // Параметры света
        public Vector3 Position { get; set; }
        public Vector3 LookAt { get; set; } // Направление света
        public Vector3 Ambient { get; set; }
        public Vector3 Diffuse { get; set; }
        public Vector3 Specular { get; set; }

        // Коэффициенты аттенюации
        public float ConstantAttenuation { get; set; }
        public float LinearAttenuation { get; set; }
        public float QuadraticAttenuation { get; set; }

        // Параметры ближней и дальней плоскостей
        private float nearPlane = 1.0f;
        private float farPlane = 1000.0f;

        public Light()
        {
            // Инициализация шейдера и карты теней
            shadowShader = new Shader("shaders/shadow.vert", "shaders/shadow.frag");
            shadowMap = new ShadowMap(1024, 1024);

            // Установка максимальных значений по умолчанию
            Position = new Vector3(0, 5, 10);
            LookAt = new Vector3(0, 0, 0);
            Ambient = new Vector3(1.0f, 1.0f, 1.0f);  // Максимальное фоновое освещение
            Diffuse = new Vector3(2.0f, 2.0f, 2.0f);  // Максимальное рассеянное освещение
            Specular = new Vector3(1.0f, 1.0f, 1.0f); // Максимальное отраженное освещение

            // Минимальные коэффициенты затухания для максимального освещения
            ConstantAttenuation = 1.0f;
            LinearAttenuation = 0.09f;
            QuadraticAttenuation = 0.032f;
        }

        public void SetLightUniforms(Shader shader, int index)
        {
            shader.SetVector3($"lightPos[{index}]", Position);
            shader.SetVector3($"lightAmbient[{index}]", Ambient);
            shader.SetVector3($"lightDiffuse[{index}]", Diffuse);
            shader.SetVector3($"lightSpecular[{index}]", Specular);

            // Установка коэффициентов аттенюации
            shader.SetFloat($"constantAttenuation[{index}]", ConstantAttenuation);
            shader.SetFloat($"linearAttenuation[{index}]", LinearAttenuation);
            shader.SetFloat($"quadraticAttenuation[{index}]", QuadraticAttenuation);

            // Передача параметров карты теней
            if (index == 0) // Only set shadow map uniforms for the first light
            {
                shadowMap.SetShaderUniforms(shader);
            }
        }

        internal void ReCompute(SceneSettings scene)
        {
            RecomputeLightSpaceMatrix();
            RenderShadows(scene);
        }

        public void RecomputeLightSpaceMatrix()
        {
            try
            {
                shadowMap.CalculateLightSpaceMatrix(Position, LookAt, nearPlane, farPlane);
            }
            catch (Exception ex)
            {
                // Логирование или игнорирование ошибки, если расчет не удался
                Console.WriteLine($"Error in RecomputeLightSpaceMatrix: {ex.Message}");
            }
        }

        public void RenderShadows(SceneSettings scene)
        {
            try
            {
                shadowShader.Use();

                // Активируем карту теней для записи
                shadowMap.BindForWriting();

                // Устанавливаем uniform-переменные для шейдера теней
                shadowMap.SetShadowShaderUniforms(shadowShader);

                // Рендерим объекты сцены
                foreach (var obj in scene.sceneObjects)
                {
                    obj.Render(shadowShader);
                }

                // Отключаем запись в карту теней
                shadowMap.Unbind();
            }
            catch (Exception ex)
            {
                // Логирование или игнорирование ошибки при рендеринге теней
                Console.WriteLine($"Error in RenderShadows: {ex.Message}");
            }
        }

        public void SetDepthPlanes(float near, float far)
        {
            if (near <= 0 || far <= 0 || near >= far)
            {
                throw new ArgumentException("Некорректные значения ближней и дальней плоскостей.");
            }

            nearPlane = near;
            farPlane = far;
        }

        public void SetAttenuation(float constant, float linear, float quadratic)
        {
            ConstantAttenuation = constant;
            LinearAttenuation = linear;
            QuadraticAttenuation = quadratic;
        }
    }
}