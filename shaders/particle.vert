#version 330 core
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aColor;
layout (location = 2) in float aSize;

uniform mat4 projection;
uniform mat4 view;
uniform vec3 cameraPos;

out vec3 Color;
out float Size;

void main()
{
    Color = aColor;
    Size = aSize;
    
    gl_PointSize = Size;
    
    vec4 worldPos = vec4(aPos, 1.0);
    vec4 viewPos = view * worldPos;
    vec4 clipPos = projection * viewPos;
    
    gl_Position = clipPos;
} 