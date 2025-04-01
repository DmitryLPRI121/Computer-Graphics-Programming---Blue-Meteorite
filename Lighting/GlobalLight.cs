using OpenTK.Mathematics;

namespace Computer_Graphics_Programming_Blue_Meteorite
{
    public class GlobalLight
    {
        private bool autoUpdate = true;
        private float cycleSpeed = 10.0f; // Синхронизируем скорость с Skybox
        private SceneObjects sceneState;

        // Свойства света, которые меняются в зависимости от времени суток
        private Vector3 ambientColor = new Vector3(0.2f, 0.2f, 0.3f);
        private Vector3 diffuseColor = new Vector3(1.0f, 1.0f, 0.9f);
        private Vector3 specularColor = new Vector3(1.0f, 1.0f, 0.8f);
        private float ambientIntensity = 0.2f;
        private float diffuseIntensity = 1.0f;
        private float specularIntensity = 0.5f;

        // Позиция и направление солнца
        private Vector3 sunPosition;
        private Vector3 sunDirection;

        public GlobalLight(SceneObjects sceneState = null)
        {
            this.sceneState = sceneState;
        }

        public void Update(float deltaTime)
        {
            if (autoUpdate && sceneState != null)
            {
                lock (sceneState)
                {
                    UpdateLightProperties(sceneState.SkyboxTimeOfDay);
                }
            }
        }

        private void UpdateLightProperties(float timeOfDay)
        {
            // Вычисляем позицию солнца на основе времени суток
            // Используем сферические координаты для более реалистичного движения солнца
            float sunAngle = timeOfDay * 2.0f * MathF.PI;
            
            // Вычисляем позицию солнца в 3D пространстве
            // Солнце движется по кругу вокруг сцены с некоторым наклоном для имитации сезонов
            float tiltAngle = MathF.PI / 6.0f; // Наклон 30 градусов
            float radius = 100.0f; // Расстояние солнца от центра сцены
            
            // Вычисляем позицию солнца с помощью сферических координат
            sunPosition = new Vector3(
                radius * MathF.Cos(sunAngle) * MathF.Cos(tiltAngle),
                radius * MathF.Sin(tiltAngle),
                radius * MathF.Sin(sunAngle) * MathF.Cos(tiltAngle)
            );

            // Вычисляем направление солнца (от солнца к центру сцены)
            sunDirection = Vector3.Normalize(-sunPosition);

            // Обновляем свойства света в зависимости от времени суток
            if (timeOfDay < 0.2f) // Ночь до рассвета
            {
                float t = timeOfDay * 5.0f;
                ambientColor = Vector3.Lerp(new Vector3(0.1f, 0.1f, 0.2f), new Vector3(0.3f, 0.4f, 0.7f), t);
                diffuseColor = Vector3.Lerp(new Vector3(0.1f, 0.1f, 0.2f), new Vector3(0.8f, 0.9f, 1.0f), t);
                ambientIntensity = MathHelper.Lerp(0.1f, 0.3f, t);
                diffuseIntensity = MathHelper.Lerp(0.1f, 0.8f, t);
            }
            else if (timeOfDay < 0.4f) // Рассвет до дня
            {
                float t = (timeOfDay - 0.2f) * 5.0f;
                ambientColor = Vector3.Lerp(new Vector3(0.3f, 0.4f, 0.7f), new Vector3(0.6f, 0.8f, 1.0f), t);
                diffuseColor = Vector3.Lerp(new Vector3(0.8f, 0.9f, 1.0f), new Vector3(1.0f, 1.0f, 1.0f), t);
                ambientIntensity = MathHelper.Lerp(0.3f, 0.5f, t);
                diffuseIntensity = MathHelper.Lerp(0.8f, 1.0f, t);
            }
            else if (timeOfDay < 0.6f) // День до сумерек
            {
                float t = (timeOfDay - 0.4f) * 5.0f;
                ambientColor = Vector3.Lerp(new Vector3(0.6f, 0.8f, 1.0f), new Vector3(0.8f, 0.6f, 0.7f), t);
                diffuseColor = Vector3.Lerp(new Vector3(1.0f, 1.0f, 1.0f), new Vector3(0.9f, 0.7f, 0.5f), t);
                ambientIntensity = MathHelper.Lerp(0.5f, 0.4f, t);
                diffuseIntensity = MathHelper.Lerp(1.0f, 0.7f, t);
            }
            else // Сумерки до ночи
            {
                float t = (timeOfDay - 0.6f) * 2.5f;
                ambientColor = Vector3.Lerp(new Vector3(0.8f, 0.6f, 0.7f), new Vector3(0.1f, 0.1f, 0.2f), t);
                diffuseColor = Vector3.Lerp(new Vector3(0.9f, 0.7f, 0.5f), new Vector3(0.1f, 0.1f, 0.2f), t);
                ambientIntensity = MathHelper.Lerp(0.4f, 0.1f, t);
                diffuseIntensity = MathHelper.Lerp(0.7f, 0.1f, t);
            }

            // Обновляем интенсивность отраженного света на основе позиции солнца
            specularIntensity = MathHelper.Clamp(sunDirection.Y * 0.5f + 0.5f, 0.1f, 0.8f);
        }

        public void SetTimeOfDay(float time)
        {
            if (sceneState != null)
            {
                lock (sceneState)
                {
                    sceneState.SkyboxTimeOfDay = time;
                    UpdateLightProperties(time);
                }
            }
        }

        public void SetAutoUpdate(bool auto)
        {
            autoUpdate = auto;
        }

        public void SetCycleSpeed(float speed)
        {
            cycleSpeed = speed;
        }

        public void SetLightUniforms(Shader shader)
        {
            shader.SetVector3("globalAmbientColor", ambientColor);
            shader.SetVector3("globalDiffuseColor", diffuseColor);
            shader.SetVector3("globalSpecularColor", specularColor);
            shader.SetFloat("globalAmbientIntensity", ambientIntensity);
            shader.SetFloat("globalDiffuseIntensity", diffuseIntensity);
            shader.SetFloat("globalSpecularIntensity", specularIntensity);
            shader.SetVector3("sunPosition", sunPosition);
            shader.SetVector3("sunDirection", sunDirection);
        }
    }
} 