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

//
// Parameters which will be passed in from the game
// It is the game's job to ensure these values are clamped correctly.
//

// All three of these range from -1.0 to +1.0.
float Saturation = 0.0;
float Lightness = 0.0;
float Contrast = 0.0;

// This is an enum to select which graypoint to use.
int LumaType = 0;

// The rgb values in each vector range from -1.0 to +1.0 and represent the
// color shift amount toward (+) or away from (-) the respective color.
float3 ShadowRgb = {0.0, 0.0, 0.0};
float3 MidtoneRgb = {0.0, 0.0, 0.0};
float3 HighlightRgb = {0.0, 0.0, 0.0};


static const float3 fluma[2] = {
    float3(0.2126, 0.7152, 0.0722),   // ITU BT.709
    float3(0.299, 0.587, 0.114),      // ITU BT.601
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float3 ApplySaturation(float3 rgb)
{
    if (Saturation != 0.0) {
        float graypoint = dot(rgb, fluma[LumaType]);
        float3 target;
        target.rgb = graypoint;
        return lerp(rgb, target, -Saturation);
    }
    return rgb;
}

float3 ApplyLightness(float3 rgb)
{
    if (Lightness != 0.0) {
        float3 target = {0.0, 0.0, 0.0};
        if (Lightness > 0.0) {
            target += 1.0;
        }
        return lerp(rgb, target, abs(Lightness));
    }
    return rgb;
}

float3 ApplyContrast(float3 rgb)
{
    if (Contrast != 0.0) {
        rgb = ((rgb - 0.5) * (Contrast + 1.0) * (Contrast + 1.0)) + 0.5;
    }
    return rgb;
}

/*
float ungamma(float v)
{
    if (v > 0.04045) {
        v = pow(abs((v+0.055)/1.055), 2.4);
    } else {
        v /= 12.92;
    }
    return v;
}

float regamma(float v)
{
    if (v > 0.0031308) {
        v = 1.055 * pow(abs(v), 1/2.4) - 0.055;
    } else {
        v *= 12.92;
    }
    return v;
}
*/

float3 ApplyColorBalance(float3 rgb)
{
    float intensity = (rgb.r + rgb.g + rgb.b) * 0.33333333;

    float shadow = 1.0 - intensity;
    shadow = shadow * shadow * shadow;

    float midtone = 1.0 - abs(-1.0 + 2*intensity);
    midtone = midtone * midtone * midtone;

    float highlight = intensity;
    highlight = highlight * highlight * highlight;

    float3 colorized = rgb + ShadowRgb * shadow +
            MidtoneRgb * midtone +
            HighlightRgb * highlight;
    // TODO potentially preserve luminance here
    return colorized;
}

float4 Main(VertexShaderOutput input) : COLOR
{
    float4 color = tex2D(Sampler, input.TextureCoordinates) * input.Color;
    float3 unmul = color.rgb / color.a;
    unmul = ApplySaturation(unmul);
    unmul = ApplyLightness(unmul);
    unmul = ApplyContrast(unmul);
    unmul = ApplyColorBalance(unmul);
    color.rgb = unmul * color.a;
    return color;
}

technique Colorizer
{
    pass
    {
        PixelShader = compile PS_SHADERMODEL Main();
    }
};
