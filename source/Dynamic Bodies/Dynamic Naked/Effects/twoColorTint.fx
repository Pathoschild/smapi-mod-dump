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

float4 xColor1;
float4 xColor2;

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 UV : TEXCOORD0;
};

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float4 cIn = tex2D(InputSampler, input.UV) * input.Color;
	float alpha = cIn.a;
	float4 cOut = cIn;
	if (alpha == 0) return cOut;

	float4 Color1Light = xColor1 * 2.0;
	float4 Color1Dark = xColor1 * 0.5;

	float4 Color2Light = xColor2 * 2.0;
	float4 Color2Dark = xColor2 * 0.5;
	
	//Primary colour swap is based on grey
    if(cIn.r == cIn.g && cIn.g == cIn.b)
	{
		if (cIn.r > 0.5)
		{ 
			cOut = xColor1 + Color1Light * (cIn.r-0.5)/0.5;
		}
		else 
		{
			cOut = Color1Dark + xColor1 * cIn.r / 0.5;
		}
	}

	//Secondary swap colour is based on magenta
	if (cIn.r == cIn.b && cIn.g != cIn.b)
	{
		if (cIn.r > 0.5)
		{
			cOut = xColor2 + Color2Light * (cIn.r - 0.5) / 0.5;
		}
		else
		{
			cOut = Color2Dark + xColor2 * cIn.r / 0.5;
		}

	}

	//Is number getting too big? Lock it down...
	if (cOut.r > 1.0) cOut.r = 1.0;
	if (cOut.g > 1.0) cOut.g = 1.0;
	if (cOut.b > 1.0) cOut.b = 1.0;

	cOut.a = alpha;
	
	return cOut;
}

technique Technique1
{
	pass Pass1
	{
		PixelShader = compile PS_SHADERMODEL PixelShaderFunction();
	}
}