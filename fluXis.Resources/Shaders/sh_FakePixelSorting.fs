layout(std140, set = 0, binding = 0) uniform m_FakePixelSortingParameters
{
    vec2 g_TexSize;
    float g_Time;
    float g_Strength;
};

layout(set = 1, binding = 0) uniform texture2D m_Texture;
layout(set = 1, binding = 1) uniform sampler m_Sampler;

layout(location = 0) out vec4 o_Colour;

// https://www.shadertoy.com/view/wljyRz <-- modified from here

#define MAX_OFFSET 80.

float rand(float co) { return fract(sin(co*(91.3458)) * 47453.5453); }

void main(void) {
    vec2 uv = (gl_FragCoord.xy / g_TexSize);
    vec2 texel = 1.0 / g_TexSize;
    
    vec4 img = texture(sampler2D(m_Texture, m_Sampler), uv);

    float step_y = texel.y * rand(uv.x) * MAX_OFFSET * g_Strength;

    step_y += rand(uv.x*uv.y*g_Time) * 0.025 * sin(g_Strength);
    step_y = mix(step_y, step_y * rand(uv.x*g_Time) * 0.5, sin(g_Strength));
    
    if (dot(img, vec4(0.299, 0.587, 0.114, 0.0)) > (1.2 * 0.50)) {
    	uv.y+=step_y;
    } else {
    	uv.y-=step_y;
    }
    
    o_Colour = texture(sampler2D(m_Texture, m_Sampler), uv);  
}