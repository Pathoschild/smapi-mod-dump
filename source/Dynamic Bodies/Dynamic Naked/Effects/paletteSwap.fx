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

float4 xSourcePalette[25];
float4 xTargetPalette[25];

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

	if (cIn.a > 0)
	{
		for (int i = 0; i < 25; i++)
		{
		
			if (xSourcePalette[i].r == cIn.r && xSourcePalette[i].g == cIn.g && xSourcePalette[i].b == cIn.b)
			{
				cOut = xTargetPalette[i];
				break;
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