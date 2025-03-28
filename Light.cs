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
        }

        /// <summary>
        /// Устанавливает параметры света в основной шейдер.
        /// </summary>
        public void SetLightUniforms(Shader shader)
        {
            shader.SetVector3("lightPos", Position);
            shader.SetVector3("lightAmbient", Ambient);
            shader.SetVector3("lightDiffuse", Diffuse);
            shader.SetVector3("lightSpecular", Specular);

            // Установка коэффициентов аттенюации
            shader.SetFloat("constantAttenuation", ConstantAttenuation);
            shader.SetFloat("linearAttenuation", LinearAttenuation);
            shader.SetFloat("quadraticAttenuation", QuadraticAttenuation);

            // Передача параметров карты теней
            shadowMap.SetShaderUniforms(shader);
        }

        internal void ReCompute(Scene scene)
        {
            RecomputeLightSpaceMatrix();
            RenderShadows(scene);
        }

        /// <summary>
        /// Пересчитывает матрицу пространства света.
        /// </summary>
        public void RecomputeLightSpaceMatrix()
        {
            shadowMap.CalculateLightSpaceMatrix(Position, LookAt, nearPlane, farPlane);
        }

        /// <summary>
        /// Выполняет рендеринг теней.
        /// </summary>
        public void RenderShadows(Scene scene)
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

        /// <summary>
        /// Устанавливает параметры ближней и дальней плоскостей.
        /// </summary>
        public void SetDepthPlanes(float near, float far)
        {
            if (near <= 0 || far <= 0 || near >= far)
            {
                throw new ArgumentException("Некорректные значения ближней и дальней плоскостей.");
            }

            nearPlane = near;
            farPlane = far;
        }

        /// <summary>
        /// Устанавливает коэффициенты аттенюации.
        /// </summary>
        public void SetAttenuation(float constant, float linear, float quadratic)
        {
            ConstantAttenuation = constant;
            LinearAttenuation = linear;
            QuadraticAttenuation = quadratic;
        }
    }
}