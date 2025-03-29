using OpenTK.Mathematics;

namespace Computer_Graphics_Programming_Blue_Meteorite
{
    public class GlobalLight
    {
        private float timeOfDay = 0.5f; // Start at noon
        private bool autoUpdate = true;
        private float cycleSpeed = 60.0f; // Speed of time cycle (in seconds per full cycle)

        // Light properties that change with time of day
        private Vector3 ambientColor = new Vector3(0.2f, 0.2f, 0.3f);
        private Vector3 diffuseColor = new Vector3(1.0f, 1.0f, 0.9f);
        private Vector3 specularColor = new Vector3(1.0f, 1.0f, 0.8f);
        private float ambientIntensity = 0.2f;
        private float diffuseIntensity = 1.0f;
        private float specularIntensity = 0.5f;

        // Sun position and direction
        private Vector3 sunPosition;
        private Vector3 sunDirection;

        public void Update(float deltaTime)
        {
            if (autoUpdate)
            {
                timeOfDay = (timeOfDay + deltaTime / cycleSpeed) % 1.0f;
                UpdateLightProperties();
            }
        }

        private void UpdateLightProperties()
        {
            // Calculate sun position based on time of day
            // Using spherical coordinates for more realistic sun movement
            float sunAngle = timeOfDay * 2.0f * MathF.PI;
            
            // Calculate sun's position in 3D space
            // The sun moves in a circle around the scene, with some tilt to simulate seasons
            float tiltAngle = MathF.PI / 6.0f; // 30 degrees tilt
            float radius = 100.0f; // Distance of sun from scene center
            
            // Calculate sun position using spherical coordinates
            sunPosition = new Vector3(
                radius * MathF.Cos(sunAngle) * MathF.Cos(tiltAngle),
                radius * MathF.Sin(tiltAngle),
                radius * MathF.Sin(sunAngle) * MathF.Cos(tiltAngle)
            );

            // Calculate sun direction (from sun to scene center)
            sunDirection = Vector3.Normalize(-sunPosition);

            // Update light properties based on time of day
            if (timeOfDay < 0.2f) // Night to Dawn
            {
                float t = timeOfDay * 5.0f;
                ambientColor = Vector3.Lerp(new Vector3(0.1f, 0.1f, 0.2f), new Vector3(0.3f, 0.4f, 0.7f), t);
                diffuseColor = Vector3.Lerp(new Vector3(0.1f, 0.1f, 0.2f), new Vector3(0.8f, 0.9f, 1.0f), t);
                ambientIntensity = MathHelper.Lerp(0.1f, 0.3f, t);
                diffuseIntensity = MathHelper.Lerp(0.1f, 0.8f, t);
            }
            else if (timeOfDay < 0.4f) // Dawn to Day
            {
                float t = (timeOfDay - 0.2f) * 5.0f;
                ambientColor = Vector3.Lerp(new Vector3(0.3f, 0.4f, 0.7f), new Vector3(0.6f, 0.8f, 1.0f), t);
                diffuseColor = Vector3.Lerp(new Vector3(0.8f, 0.9f, 1.0f), new Vector3(1.0f, 1.0f, 1.0f), t);
                ambientIntensity = MathHelper.Lerp(0.3f, 0.5f, t);
                diffuseIntensity = MathHelper.Lerp(0.8f, 1.0f, t);
            }
            else if (timeOfDay < 0.6f) // Day to Dusk
            {
                float t = (timeOfDay - 0.4f) * 5.0f;
                ambientColor = Vector3.Lerp(new Vector3(0.6f, 0.8f, 1.0f), new Vector3(0.8f, 0.6f, 0.7f), t);
                diffuseColor = Vector3.Lerp(new Vector3(1.0f, 1.0f, 1.0f), new Vector3(0.9f, 0.7f, 0.5f), t);
                ambientIntensity = MathHelper.Lerp(0.5f, 0.4f, t);
                diffuseIntensity = MathHelper.Lerp(1.0f, 0.7f, t);
            }
            else // Dusk to Night
            {
                float t = (timeOfDay - 0.6f) * 2.5f;
                ambientColor = Vector3.Lerp(new Vector3(0.8f, 0.6f, 0.7f), new Vector3(0.1f, 0.1f, 0.2f), t);
                diffuseColor = Vector3.Lerp(new Vector3(0.9f, 0.7f, 0.5f), new Vector3(0.1f, 0.1f, 0.2f), t);
                ambientIntensity = MathHelper.Lerp(0.4f, 0.1f, t);
                diffuseIntensity = MathHelper.Lerp(0.7f, 0.1f, t);
            }

            // Update specular intensity based on sun position
            specularIntensity = MathHelper.Clamp(sunDirection.Y * 0.5f + 0.5f, 0.1f, 0.8f);
        }

        public void SetTimeOfDay(float time)
        {
            timeOfDay = time;
            autoUpdate = false;
            UpdateLightProperties();
        }

        public void SetAutoUpdate(bool auto)
        {
            autoUpdate = auto;
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