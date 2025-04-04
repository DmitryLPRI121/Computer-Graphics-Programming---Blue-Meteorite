#version 330 core
layout(location = 0) in vec3 aPos;
layout(location = 1) in vec3 aNormal;
layout(location = 2) in vec2 aTexCoord;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
uniform mat4 lightSpaceMatrix;
uniform vec3 objectColor;
uniform float textureRepeat = 1.0;

out vec2 TexCoord;
out vec3 FragPos;
out vec3 Normal;
out vec4 FragPosLightSpace;
out vec3 ObjectColor;

void main()
{
    FragPos = vec3(model * vec4(aPos, 1.0));
    Normal = mat3(transpose(inverse(model))) * aNormal;
    gl_Position = projection * view * model * vec4(aPos, 1.0);
    TexCoord = aTexCoord * textureRepeat;
    ObjectColor = objectColor;

    FragPosLightSpace = lightSpaceMatrix * vec4(FragPos, 1.0);
}