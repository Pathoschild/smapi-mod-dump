#if OPENGL
    #define SV_POSITION POSITION
    #define VS_SHADERMODEL vs_3_0
    #define PS_SHADERMODEL ps_3_0
#else
    #define VS_SHADERMODEL vs_4_0_level_9_1
    #define PS_SHADERMODEL ps_4_0_level_9_1
#endif

sampler2D TextureSampler : register(s0)
{
    Texture = (Texture);
};

float Opacity = 1.0;

float4 OpacityPS(float4 position : SV_Position, float4 color : COLOR0, float2 TextureCoordinates : TEXCOORD0): COLOR0 
{
    float4 tex = tex2D(TextureSampler, TextureCoordinates) * color;

    return float4(tex.r, tex.g, tex.b, tex.a * Opacity);
}

technique OpacityFade
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL OpacityPS();
    }
};