// This is a modified version of the Colorizer shader from
// Nightshade by ichortower. Nightshade is available under the MIT license.

// MIT License
//
// Copyright (c) 2024 ichortower
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

#if OPENGL
    #define PS_SHADERMODEL ps_3_0
#else
    #define PS_SHADERMODEL ps_4_0_level_9_1
#endif

// ===========
// Parameters
// ===========

Texture2D Texture;

// All three of these range from -1.0 to +1.0.
float Saturation;
float Lightness;
float Contrast;

// This is an enum to select which graypoint to use.
int LumaType;

// The rgb values in each vector range from -1.0 to +1.0 and represent the
// color shift amount toward (+) or away from (-) the respective color.
float3 ShadowRgb;
float3 MidtoneRgb;
float3 HighlightRgb;


// ===========
// Boilerplate
// ===========

sampler2D Sampler = sampler_state
{
    Texture = <Texture>;
};

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

// ===========
// The Shader
// ===========

float4 Main(VertexShaderOutput input) : COLOR
{
    float4 color = tex2D(Sampler, input.TextureCoordinates) * input.Color;
	// We don't need to un-multiply, since we never have alpha
	// for Cloudy Skies shader layers.
	float3 unmul = color.rgb; // / color.a
    unmul = ApplySaturation(unmul);
    unmul = ApplyLightness(unmul);
    unmul = ApplyContrast(unmul);
    unmul = ApplyColorBalance(unmul);

	// We don't need to re-multiply. Same reason.
    color.rgb = unmul; // * color.a;
    return color;
}

technique Colorize
{
    pass
    {
        PixelShader = compile PS_SHADERMODEL Main();
    }
};
