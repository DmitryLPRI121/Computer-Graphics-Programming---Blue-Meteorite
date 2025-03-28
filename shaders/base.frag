#version 330 core
out vec4 FragColor;
in vec2 TexCoord;
in vec3 FragPos;
in vec3 Normal;
in vec4 FragPosLightSpace;

uniform vec3 lightPos;
uniform vec3 viewPos;
uniform vec3 lightAmbient;
uniform vec3 lightDiffuse;
uniform vec3 lightSpecular;
uniform sampler2D shadowMap;
uniform sampler2D texture_diffuse1;

uniform float constantAttenuation;
uniform float linearAttenuation;
uniform float quadraticAttenuation;

float ShadowCalculation(vec4 fragPosLightSpace)
{

    vec3 projCoords = fragPosLightSpace.xyz / fragPosLightSpace.w;
    projCoords = projCoords * 0.5 + 0.5;


    if (projCoords.z > 1.0)
        return 0.0;


    float bias = max(0.05 * (1.0 - dot(normalize(Normal), normalize(lightPos - FragPos))), 0.005);


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
    vec3 lightDir = normalize(lightPos - FragPos);

    float distanceToLight = length(lightPos - FragPos);

    float distanceToCamera = length(viewPos - FragPos);

    float lightAttenuation = 1.0 / (constantAttenuation + linearAttenuation * distanceToLight + quadraticAttenuation * distanceToLight * distanceToLight);
    float cameraAttenuation = 1.0 / (constantAttenuation + linearAttenuation * distanceToCamera + quadraticAttenuation * distanceToCamera * distanceToCamera);

    vec3 ambient = lightAmbient * vec3(texture(texture_diffuse1, TexCoord)) * lightAttenuation;

    float diff = max(dot(norm, lightDir), 0.0);
    vec3 diffuse = lightDiffuse * diff * vec3(texture(texture_diffuse1, TexCoord)) * lightAttenuation;

    vec3 viewDir = normalize(viewPos - FragPos);
    vec3 reflectDir = reflect(-lightDir, norm);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32);
    vec3 specular = lightSpecular * spec * lightAttenuation * cameraAttenuation;

    float shadow = ShadowCalculation(FragPosLightSpace);

    vec3 result = (1.0 - shadow * 10.0/distanceToLight) * (ambient + diffuse + specular);
    FragColor = vec4(result, 1.0);
}