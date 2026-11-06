layout(std140, set = 0, binding = 0) uniform m_AsciiArt2Parameters
{
    vec2 g_TexSize;
    float g_Strength;
    float g_CellSize;
};

layout(set = 1, binding = 0) uniform texture2D m_Texture;
layout(set = 1, binding = 1) uniform sampler m_Sampler;

layout(location = 0) out vec4 o_Colour;

// https://www.shadertoy.com/view/lssGDj <-- original idea


#define FLIP_Y 0
#define GLYPH_COUNT 95

// cada glifo mide 5x8 pixeles. se guarda en dos uint (5 bits por fila, 4 filas por uint)
//  x = filas 0-3 (arriba), y = filas 4-7 (abajo)
// ordenados de menos a mas pixeles encendidos (de oscuro a brillante)


const uvec2 GLYPHS[GLYPH_COUNT] = uvec2[GLYPH_COUNT](
    uvec2(0u, 0u),              //  
    uvec2(0u, 128u),            // .
    uvec2(128u, 4096u),         // :
    uvec2(266240u, 0u),         // `
    uvec2(135424u, 0u),         // '
    uvec2(0u, 4232u),           // ,
    uvec2(128u, 4232u),         // ;
    uvec2(31u, 0u),             // -
    uvec2(141856u, 0u),         // ^
    uvec2(0u, 31u),             // _
    uvec2(277u, 65536u),        // ~
    uvec2(135300u, 131200u),    // !
    uvec2(338240u, 0u),         // "
    uvec2(69896u, 266304u),     // (
    uvec2(266306u, 69888u),     // )
    uvec2(33860u, 279040u),     // /
    uvec2(69904u, 266304u),     // <
    uvec2(266305u, 69888u),     // >
    uvec2(540932u, 66592u),     // \  (backslash)
    uvec2(135300u, 135300u),    // |
    uvec2(4255u, 135168u),      // +
    uvec2(476194u, 131200u),    // ?
    uvec2(131460u, 135616u),    // i
    uvec2(729u, 541184u),       // r
    uvec2(561u, 567424u),       // v
    uvec2(554u, 141856u),       // x
    uvec2(102536u, 135264u),    // {
    uvec2(790658u, 135936u),    // }
    uvec2(143492u, 135616u),    // 1
    uvec2(992u, 1015808u),      // =
    uvec2(574788u, 135296u),    // Y
    uvec2(65730u, 68172u),      // j
    uvec2(397444u, 135616u),    // l
    uvec2(4782u, 692224u),      // *
    uvec2(1016900u, 270592u),   // 7
    uvec2(462980u, 135616u),    // I
    uvec2(231490u, 84352u),     // J
    uvec2(541200u, 541664u),    // L
    uvec2(1020036u, 135296u),   // T
    uvec2(467208u, 270784u),    // [
    uvec2(460866u, 68032u),     // ]
    uvec2(465u, 542144u),       // c
    uvec2(206108u, 270592u),    // f
    uvec2(271240u, 271552u),    // t
    uvec2(541268u, 807488u),    // k
    uvec2(729u, 575008u),       // n
    uvec2(465u, 574912u),       // o
    uvec2(561u, 576928u),       // u
    uvec2(561u, 709952u),       // w
    uvec2(812100u, 281696u),    // %
    uvec2(476688u, 542144u),    // C
    uvec2(575025u, 567424u),    // V
    uvec2(574788u, 345632u),    // X
    uvec2(496u, 460736u),       // s
    uvec2(994u, 140256u),       // z
    uvec2(476194u, 140256u),    // 2
    uvec2(1017986u, 50624u),    // 3
    uvec2(72018u, 1017920u),    // 4
    uvec2(1032734u, 541184u),   // F
    uvec2(576152u, 674336u),    // K
    uvec2(449u, 509408u),       // a
    uvec2(465u, 1032640u),      // e
    uvec2(541401u, 575008u),    // h
    uvec2(977u, 588304u),       // p
    uvec2(497u, 572449u),       // q
    uvec2(561u, 572462u),       // y
    uvec2(412296u, 706976u),    // &
    uvec2(205342u, 574912u),    // 6
    uvec2(476719u, 35200u),     // 9
    uvec2(1001022u, 541184u),   // P
    uvec2(508430u, 34752u),     // S
    uvec2(575025u, 574912u),    // U
    uvec2(1016900u, 279520u),   // Z
    uvec2(853u, 710304u),       // m
    uvec2(476721u, 574912u),    // O
    uvec2(541401u, 583360u),    // b
    uvec2(34227u, 576928u),     // d
    uvec2(497u, 572462u),       // g
    uvec2(147086u, 194688u),    // $
    uvec2(1033153u, 50624u),    // 5
    uvec2(476718u, 574912u),    // 8
    uvec2(575039u, 575008u),    // H
    uvec2(583347u, 575008u),    // N
    uvec2(476721u, 706976u),    // Q
    uvec2(476735u, 575008u),    // A
    uvec2(1001009u, 575424u),   // D
    uvec2(1032734u, 541664u),   // E
    uvec2(476695u, 574944u),    // G
    uvec2(585397u, 575008u),    // M
    uvec2(1001022u, 674336u),   // R
    uvec2(575029u, 716320u),    // W
    uvec2(476789u, 837056u),    // 0
    uvec2(338922u, 1026368u),   // #
    uvec2(1001022u, 575424u),   // B
    uvec2(476917u, 770528u)     // @
);

// gx: 0..4 (izquierda -> derecha), 
// gy: 0..7 (arriba -> abajo)
float glyphPixel(int idx, int gx, int gy)
{
    if (gx < 0 || gx > 4 || gy < 0 || gy > 7) return 0.0;
    uvec2 g = GLYPHS[idx];
    uint word = (gy < 4) ? g.x : g.y;
    int shift = (3 - (gy & 3)) * 5 + (4 - gx);
    return float((word >> uint(shift)) & 1u);
}

void main(void)
{
    vec2 pix = gl_FragCoord.xy;

    float cellW = max(6.0, g_CellSize * 32.0);
    vec2 cell = vec2(cellW, cellW * 1.5);

    vec2 cellIdx = floor(pix / cell);
    vec2 uv = (cellIdx + 0.5) * cell / g_TexSize.xy;
    vec3 col = texture(sampler2D(m_Texture, m_Sampler), uv).rgb;

    float gray = 0.3 * col.r + 0.59 * col.g + 0.11 * col.b;
    int idx = clamp(int(gray * float(GLYPH_COUNT)), 0, GLYPH_COUNT - 1);

    vec2 local = (pix - cellIdx * cell) / cell;
    int gx = int(floor(local.x * 6.0));
    int gy = int(floor(local.y * 9.0));

#if FLIP_Y
    gy = 7 - gy;
#endif

    col = col * glyphPixel(idx, gx, gy);

    vec4 original = texture(sampler2D(m_Texture, m_Sampler), gl_FragCoord.xy / g_TexSize);
    vec4 ascii = vec4(col, 1.0);
    o_Colour = mix(original, ascii, g_Strength);
}
