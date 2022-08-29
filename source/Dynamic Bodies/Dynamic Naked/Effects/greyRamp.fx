#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

sampler2D InputSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

float4 xDarkColor;
float4 xColor;

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 UV : TEXCOORD0;
};

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float4 cIn = tex2D(InputSampler, input.UV) * input.Color;
	float4 cOut = cIn;

	float4 hairdarker = xDarkColor*0.75;
	float4 hairLight = xColor*0.75 + float4(0.25,0.25,0.25,1.0);

	const float threshold = 55.0 / 255.0;
	const float upper = 240.0 / 255.0;
	const float range = 185.0 / 255.0;
	float perc = 0.5;

	if (cIn.a > 0.387)// 99/255
	{
		
		float a = cIn.a;
        if(cIn.r > 0 && cIn.r == cIn.g && cIn.g == cIn.b)
		{
			//standard range colour
			if (cIn.r >= threshold && cIn.r < upper)
			{
				perc = (cIn.r - threshold) / range;
				cOut = xDarkColor * (1.0-perc) + xColor * perc;
				cOut.a = a;
			}

			//Darker
			if (cIn.r < threshold && cIn.r > 0)
			{
				perc = cIn.r / threshold;
				cOut = xDarkColor * perc;
				cOut.a = a;
			}

			//Lighter
			if (cIn.r >= upper)
			{
				perc = (cIn.r - upper) / (1.0 - upper);
				cOut = xColor * (1.0-perc) + hairLight * perc;
				cOut.a = a;
			}
		}
	}

	return cOut;
}

technique Technique1
{
	pass Pass1
	{
		PixelShader = compile PS_SHADERMODEL PixelShaderFunction();
	}
}