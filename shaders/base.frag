#version 330 core
out vec4 FragColor;
in vec2 TexCoord;
in vec3 FragPos;
in vec3 Normal;
in vec4 FragPosLightSpace;
in vec3 ObjectColor;

#define MAX_LIGHTS 10

uniform vec3 lightPos[MAX_LIGHTS];
uniform vec3 lightAmbient[MAX_LIGHTS];
uniform vec3 lightDiffuse[MAX_LIGHTS];
uniform vec3 lightSpecular[MAX_LIGHTS];
uniform float constantAttenuation[MAX_LIGHTS];
uniform float linearAttenuation[MAX_LIGHTS];
uniform float quadraticAttenuation[MAX_LIGHTS];
uniform int numLights;

uniform vec3 viewPos;
uniform sampler2D shadowMap;
uniform sampler2D texture_diffuse1;

// Global lighting uniforms
uniform vec3 globalAmbientColor;
uniform vec3 globalDiffuseColor;
uniform vec3 globalSpecularColor;
uniform float globalAmbientIntensity;
uniform float globalDiffuseIntensity;
uniform float globalSpecularIntensity;
uniform vec3 sunPosition;
uniform vec3 sunDirection;

float ShadowCalculation(vec4 fragPosLightSpace)
{
    vec3 projCoords = fragPosLightSpace.xyz / fragPosLightSpace.w;
    projCoords = projCoords * 0.5 + 0.5;

    if (projCoords.z > 1.0)
        return 0.0;

    float bias = max(0.05 * (1.0 - dot(normalize(Normal), normalize(lightPos[0] - FragPos))), 0.005);

    vec2 texelSize = 1.0 / textureSize(shadowMap, 0);

    float shadow = 0.0;

    for (int x = -1; x <= 1; ++x)
    {
        for (int y = -1; y <= 1; ++y)
        {
            float closestDepth = texture(shadowMap, projCoords.xy + vec2(x, y) * texelSize).r;
            shadow += (projCoords.z - bias > closestDepth) ? 1.0 : 0.0;
        }
    }

    shadow /= 9.0;

    return shadow;
}

void main()
{
    vec3 norm = normalize(Normal);
    vec3 viewDir = normalize(viewPos - FragPos);

    // Calculate local lighting from all light sources
    vec3 localLighting = vec3(0.0);
    for (int i = 0; i < numLights; i++)
    {
        vec3 lightDir = normalize(lightPos[i] - FragPos);
        float distanceToLight = length(lightPos[i] - FragPos);
        float lightAttenuation = 1.0 / (constantAttenuation[i] + linearAttenuation[i] * distanceToLight + quadraticAttenuation[i] * distanceToLight * distanceToLight);

        vec3 ambient = lightAmbient[i] * vec3(texture(texture_diffuse1, TexCoord)) * lightAttenuation;
        float diff = max(dot(norm, lightDir), 0.0);
        vec3 diffuse = lightDiffuse[i] * diff * vec3(texture(texture_diffuse1, TexCoord)) * lightAttenuation;
        vec3 reflectDir = reflect(-lightDir, norm);
        float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32);
        vec3 specular = lightSpecular[i] * spec * lightAttenuation;

        localLighting += ambient + diffuse + specular;
    }

    // Calculate global lighting using sun direction
    vec3 globalAmbient = globalAmbientColor * vec3(texture(texture_diffuse1, TexCoord)) * globalAmbientIntensity;
    float globalDiff = max(dot(norm, sunDirection), 0.0);
    vec3 globalDiffuse = globalDiffuseColor * globalDiff * vec3(texture(texture_diffuse1, TexCoord)) * globalDiffuseIntensity;
    vec3 globalReflectDir = reflect(-sunDirection, norm);
    float globalSpec = pow(max(dot(viewDir, globalReflectDir), 0.0), 32);
    vec3 globalSpecular = globalSpecularColor * globalSpec * globalSpecularIntensity;

    float shadow = ShadowCalculation(FragPosLightSpace);

    // Combine local and global lighting
    vec3 globalLighting = globalAmbient + globalDiffuse + globalSpecular;

    // Mix local and global lighting with some bias towards global lighting
    vec3 result = mix(localLighting, globalLighting, 0.7);

    // Combine results with object color
    vec3 finalResult = result * ObjectColor;

    FragColor = vec4(finalResult, 1.0);
}