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

// The horizontal pixel size.
float ScaleX;

// The vertical pixel size.
float ScaleY;


// The dimensions of the screen, in pixels.
float2 ScreenSize;

// The position of the top-left corner of the viewport, in pixels.
float2 ViewportPosition;


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
	float2 uv = input.TextureCoordinates;

	// Modify the uv to group it into Scale by Scale pixel
	// cubes that are offset with the viewport so we don't
	// get any weirdness when moving the camera.

	// TODO: Figure out why this breaks when
	// taking screenshots, or how to just improve this
	// logic in general.

	float x = floor(uv.x * ScreenSize.x) + ViewportPosition.x;
	float y = floor(uv.y * ScreenSize.y) + ViewportPosition.y;

	uv.x = uv.x - fmod(x, ScaleX) / ScreenSize.x;
	uv.y = uv.y - fmod(y, ScaleY) / ScreenSize.y;

	// And just return the sampled pixel.
	return tex2D(Sampler, uv);
}

technique PixelateTechnique {
	pass P0 {
		PixelShader = compile PS_SHADERMODEL Main();
	}
}
