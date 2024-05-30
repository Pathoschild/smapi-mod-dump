#if OPENGL
    #define SV_POSITION POSITION
    #define VS_SHADERMODEL vs_3_0
    #define PS_SHADERMODEL ps_3_0
#else
    #define VS_SHADERMODEL vs_4_0_level_9_1
    #define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D tex;

sampler2D Sampler = sampler_state
{
    Texture = <tex>;
};

// From 0 to 1, the portion of the view that should be fully in focus
float Field = 0.6;
// From 0 upward, how severe (wide) the blur is, at maximum
float Intensity = 6.0;

// not for user consumption
float PitchX = 0.0;
float PitchY = 0.0;
float Center = 0.5;

struct VS_OUTPUT
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 UV : TEXCOORD0;
};

float weight(float x, float sigma)
{
    return 0.39894 * exp(-0.5 * x * x / (sigma * sigma)) / sigma;
}

float4 Gauss(float4 inputColor, float2 UV, float2 dir, float2 field)
{
    float4 color = tex2D(Sampler, UV) * inputColor;
    float dist = 0.0;
    float ramp = min(sqrt(Intensity), Intensity) / 20.0;
    if (UV.y > field.y) {
        dist = UV.y - field.y;
    } else if (UV.y < field.x) {
        dist = field.x - UV.y;
    }

    if (dist != 0.0) {
        float pct = min(dist/ramp, 1.0);
        float sigma = lerp(0, Intensity, pct * pct);
        int m = max(ceil(sigma), 1);
        color = float4(0.0, 0.0, 0.0, 0.0);
        float Z = 0.0;
        for (int i = -m; i <= m; ++i) {
            float w = weight(i, sigma);
            Z += w;
            color += tex2Dlod(Sampler, float4(UV + dir*i, 0.0, 1.0)) * w;
        }
        color /= Z;
    }
    return color;
}

float4 GaussH(VS_OUTPUT input) : COLOR
{
    return Gauss(input.Color, input.UV, float2(PitchX, 0.0),
            float2(min(Center, 0.5) - Field/2.0, max(Center, 0.5) + Field/2.0));
}

float4 GaussV(VS_OUTPUT input) : COLOR
{
    return Gauss(input.Color, input.UV, float2(0.0, PitchY),
            float2(min(Center, 0.5) - Field/2.0, max(Center, 0.5) + Field/2.0));
}

technique GaussH
{
    pass
    {
        PixelShader = compile PS_SHADERMODEL GaussH();
    }
};

technique GaussV
{
    pass
    {
        PixelShader = compile PS_SHADERMODEL GaussV();
    }
};
