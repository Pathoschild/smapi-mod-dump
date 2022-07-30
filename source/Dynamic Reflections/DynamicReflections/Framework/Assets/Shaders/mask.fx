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

Texture2D Mask;
sampler MaskSampler {
    Texture = ( Mask );
};

//https://community.monogame.net/t/how-to-mask-2d-tile-sprites/15813/2
//https://gamedev.stackexchange.com/questions/38118/best-way-to-mask-2d-sprites-in-xna

float4 MaskPS(float4 position : SV_Position, float4 color : COLOR0, float2 TextureCoordinates : TEXCOORD0): COLOR0 
{
    float4 tex = tex2D(TextureSampler, TextureCoordinates) * color;
    float4 mask = tex2D(MaskSampler, TextureCoordinates);

    return float4(tex.r, tex.g, tex.b, min(mask.a, tex.a));
}

technique Mask
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MaskPS();
    }
};