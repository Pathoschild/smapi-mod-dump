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

// The color palette. This should be a one pixel tall strip of colors.
Texture2D Palette;

// The number of colors in the color palette.
int ColorCount;

// If this is set to true, convert the source color to monochrome first.
bool Monochrome;

// If this is set to true, dither the source color to improve output
// when working with limited palettes.
bool Dither;


// ===========
// Boilerplate
// ===========

sampler2D Sampler = sampler_state
{
	Texture = <Texture>;
};

sampler2D PaletteSampler = sampler_state
{
	Texture = <Palette>;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};


static const float BayerMatrix[16] = {
	0.0 / 16.0, 8.0 / 16.0, 2.0 / 16.0, 10.0 / 16.0,
	12.0 / 16.0, 4.0 / 16.0, 14.0 / 16.0, 6.0 / 16.0,
	3.0 / 16.0, 11.0 / 16.0, 1.0 / 16.0, 9.0 / 16.0,
	15.0 / 16.0, 7.0 / 16.0, 13.0 / 16.0, 5.0 / 16.0
};


// ===========
// The Shader
// ===========

float4 Main(VertexShaderOutput input) : COLOR
{
	// Get the original color.
	float4 original = tex2D(Sampler, input.TextureCoordinates);

	// Dithering!
	int x = (int)input.Position.x % 4;
	int y = (int)input.Position.y % 4;
	int index = y * 4 + x;

	// Calculate the value to add based on the BayerMatrix.
	// Here, we multiply by Dither which will be either 1 or 0
	// to control whether we do dithering at all.
	// I tried putting the dithering code in an if() block but
	// it caused compilation issues.
	float ditherValue = Dither * (BayerMatrix[index] - 0.5);

	// Add the dither value to our original color.
	original.rgb += ditherValue;

	// Convert to monochrome?
	if (Monochrome) {
		float value = dot(original.rgb, float3(0.2126, 0.7152, 0.0722));
		original = float4(value, value, value, original.a);
	}

	// Some working variables.
	float4 result = float4(0, 0, 0, 0);
	float dist = 10000000.0;

	// Loop through every color in the provided palette texture,
	// finding the closest match.
	for(int i = 0; i < ColorCount; i++) {
		float coord = i / (float) ColorCount;
		float4 c = tex2D(PaletteSampler, float2(coord, 0.25));
		float d = distance(original.rgb, c.rgb);

		// Is this color a closer match? Save it, then.
		if (d < dist) {
			dist = d;
			result = tex2D(PaletteSampler, float2(coord, 0.75));
		}
	}

	// ... and return the closest match.
	return result;
}

technique PalettizeTechnique {
	pass P0 {
		PixelShader = compile PS_SHADERMODEL Main();
	}
}
