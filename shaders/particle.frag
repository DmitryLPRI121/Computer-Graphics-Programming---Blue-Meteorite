#version 330 core
in vec3 Color;
in float Size;

out vec4 FragColor;

void main()
{
    vec2 center = vec2(0.5, 0.5);
    vec2 pos = gl_PointCoord - center;
    float dist = length(pos);
    
    if (dist > 0.5)
        discard;
        
    float alpha = 1.0 - smoothstep(0.0, 0.5, dist);
    
    float brightness = 1.0 - smoothstep(0.0, 0.3, dist);
    
    vec3 finalColor = Color * (1.0 + brightness * 0.5);
    
    FragColor = vec4(finalColor, alpha);
} 