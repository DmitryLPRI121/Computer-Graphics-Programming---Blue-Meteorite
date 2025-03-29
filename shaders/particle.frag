#version 330 core
in vec3 Color;
in float Size;

uniform int particleShape;

out vec4 FragColor;

float circle(vec2 pos, float radius) {
    float dist = length(pos);
    return 1.0 - smoothstep(0.0, radius, dist);
}

float square(vec2 pos, float size) {
    vec2 q = abs(pos);
    float dist = max(q.x, q.y);
    return 1.0 - smoothstep(0.0, size, dist);
}

float star(vec2 pos, float size) {
    float angle = atan(pos.y, pos.x);
    float radius = length(pos);
    float star = abs(cos(angle * 5.0) * 0.5 + 0.5);
    float dist = abs(radius - star * size);
    return 1.0 - smoothstep(0.0, size * 0.2, dist);
}

float cross(vec2 pos, float size) {
    vec2 q = abs(pos);
    float dist = min(q.x, q.y);
    return 1.0 - smoothstep(0.0, size * 0.3, dist);
}

void main()
{
    vec2 center = vec2(0.5, 0.5);
    vec2 pos = gl_PointCoord - center;
    
    float alpha = 0.0;
    
    if (particleShape == 0) {
        alpha = circle(pos, 0.5);
    } else if (particleShape == 1) {
        alpha = square(pos, 0.5);
    } else if (particleShape == 2) {
        alpha = star(pos, 0.5);
    } else if (particleShape == 3) {
        alpha = cross(pos, 0.5);
    }
    
    if (alpha <= 0.0)
        discard;
    
    float glow = 1.0 - smoothstep(0.0, 0.2, length(pos));
    vec3 finalColor = Color * (1.0 + glow * 0.3);
    finalColor = pow(finalColor, vec3(0.8));
    
    FragColor = vec4(finalColor, alpha);
} 