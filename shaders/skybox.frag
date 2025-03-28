#version 330 core
out vec4 FragColor;

in vec3 TexCoords;

uniform float timeOfDay; // 0.0 to 1.0, where 0.0 is dawn, 0.25 is morning, 0.5 is noon, 0.75 is evening, 1.0 is night

void main()
{
    // Normalize the direction vector
    vec3 dir = normalize(TexCoords);
    
    // Calculate the angle between the direction and the up vector
    float angle = acos(dot(dir, vec3(0.0, 1.0, 0.0)));
    
    // Define colors for different times of day with more intermediate colors
    vec3 nightColor = vec3(0.1, 0.1, 0.3);     // Dark blue
    vec3 lateNightColor = vec3(0.15, 0.15, 0.35); // Slightly lighter blue
    vec3 dawnColor = vec3(0.3, 0.4, 0.7);      // Early dawn
    vec3 earlyMorningColor = vec3(0.4, 0.5, 0.8); // Dawn
    vec3 morningColor = vec3(0.5, 0.7, 1.0);    // Morning
    vec3 noonColor = vec3(0.6, 0.8, 1.0);       // Noon
    vec3 afternoonColor = vec3(0.7, 0.8, 1.0);  // Afternoon
    vec3 eveningColor = vec3(0.8, 0.6, 0.7);    // Evening
    vec3 duskColor = vec3(0.6, 0.4, 0.5);       // Dusk
    vec3 nightfallColor = vec3(0.3, 0.2, 0.4);  // Nightfall
    
    // Calculate sun position based on time of day
    float sunAngle = timeOfDay * 2.0 * 3.14159;
    vec3 sunDir = vec3(sin(sunAngle), cos(sunAngle), 0.0);
    
    // Calculate distance to sun
    float sunDist = acos(dot(dir, sunDir));
    
    // Create sun glow with smoother transition
    float sunGlow = smoothstep(0.15, 0.0, sunDist);
    
    // Mix sky colors based on time of day with smoother transitions
    vec3 skyColor;
    if (timeOfDay < 0.1) {
        float t = timeOfDay * 10.0;
        skyColor = mix(nightColor, lateNightColor, t);
    } else if (timeOfDay < 0.2) {
        float t = (timeOfDay - 0.1) * 10.0;
        skyColor = mix(lateNightColor, dawnColor, t);
    } else if (timeOfDay < 0.3) {
        float t = (timeOfDay - 0.2) * 10.0;
        skyColor = mix(dawnColor, earlyMorningColor, t);
    } else if (timeOfDay < 0.4) {
        float t = (timeOfDay - 0.3) * 10.0;
        skyColor = mix(earlyMorningColor, morningColor, t);
    } else if (timeOfDay < 0.5) {
        float t = (timeOfDay - 0.4) * 10.0;
        skyColor = mix(morningColor, noonColor, t);
    } else if (timeOfDay < 0.6) {
        float t = (timeOfDay - 0.5) * 10.0;
        skyColor = mix(noonColor, afternoonColor, t);
    } else if (timeOfDay < 0.7) {
        float t = (timeOfDay - 0.6) * 10.0;
        skyColor = mix(afternoonColor, eveningColor, t);
    } else if (timeOfDay < 0.8) {
        float t = (timeOfDay - 0.7) * 10.0;
        skyColor = mix(eveningColor, duskColor, t);
    } else if (timeOfDay < 0.9) {
        float t = (timeOfDay - 0.8) * 10.0;
        skyColor = mix(duskColor, nightfallColor, t);
    } else {
        float t = (timeOfDay - 0.9) * 10.0;
        skyColor = mix(nightfallColor, nightColor, t);
    }
    
    // Add sun glow with smoother transition
    vec3 sunColor = vec3(1.0, 0.9, 0.7);
    skyColor = mix(skyColor, sunColor, sunGlow * 0.5);
    
    // Add atmospheric scattering effect with smoother transition
    float horizonGlow = smoothstep(0.0, 0.5, angle);
    vec3 horizonColor = mix(vec3(0.8, 0.6, 0.4), vec3(0.4, 0.3, 0.2), timeOfDay);
    skyColor = mix(skyColor, horizonColor, horizonGlow * 0.3);
    
    // Adjust brightness based on time of day with smoother transition
    float brightness = mix(0.5, 1.0, sin(timeOfDay * 3.14159));
    skyColor *= brightness;
    
    FragColor = vec4(skyColor, 1.0);
} 