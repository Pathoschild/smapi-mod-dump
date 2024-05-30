#if OPENGL
	#define PS_SHADERMODEL ps_3_0
#else
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

// ===========
// Parameters
// ===========

// The main texture that we're processing.
Texture2D Texture;

// Multiply how far away our pixels should be sampled.
float Distance;

// Whether we should use weights or not.
bool UseWeights;

// Weights for a 5x5 area of pixels.
float Weights[25];

// The dimensions of the screen, in pixels.
float2 ScreenSize;


// ===========
// Boilerplate
// ===========

sampler2D Sampler = sampler_state
{
	Texture = <Texture>;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};


// ===========
// The Shader
// ===========

float4 Main(VertexShaderOutput input) : COLOR
{
	int i = 0;
	float4 color = float4(0, 0, 0, 0);
	float count = 0.0;

	for(int y = -2; y <= 2; y++) {
		for(int x = -2; x <= 2; x++) {
			float2 uv = input.TextureCoordinates;
			uv.x += x * Distance / ScreenSize.x;
			uv.y += y * Distance / ScreenSize.y;

			float weight;
			if (UseWeights)
				weight = Weights[i];
			else
				weight = 1.0;

			if (weight != 0) {
				count += weight;
				color += weight * tex2D(Sampler, uv);
			}
			i++;
		}
	}

	if (count != 0.0)
		color /= count;

	color.a = 1.0;

	return color * input.Color;
}

technique BlurTechnique {
	pass P0 {
		PixelShader = compile PS_SHADERMODEL Main();
	}
}
