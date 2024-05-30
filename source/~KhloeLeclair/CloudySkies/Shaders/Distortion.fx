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

// The strength of the distortion effect.
float Strength;

// The frequency of the distortion effect.
float Frequency;

// The total game time, in milliseconds.
float TotalTime;

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


// Math
float hash(float2 p) {
	return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453);
}

float noise(float2 p) {
	float2 i = floor(p);
	float2 f = frac(p);

	float a = hash(i);
	float b = hash(i + float2(1.0, 0.0));
	float c = hash(i + float2(0.0, 1.0));
	float d = hash(i + float2(1.0, 1.0));

	float2 u = f * f * (3.0 - 2.0 * f);
	return lerp(a, b, u.x) + (c - a) * u.y * (1.0 - u.x) + (d - b) * u.x * u.y;
}


// ===========
// The Shader
// ===========

float4 Main(VertexShaderOutput input) : COLOR
{
	// Create time-based noise
	float2 worldPos = (input.Position.xy + ViewportPosition) / ScreenSize;
	float2 noiseUV = (worldPos * Frequency) + TotalTime;
	float value = noise(noiseUV);
	float distortion = value * 2.0 - 1.0;

	//Debug drawing to view the noise pattern
	//return float4(value, value, value, 1.0);

	// Distort the UV coordinates
	float2 uv = input.TextureCoordinates;
	uv.y += distortion * Strength;

	// Sample the texture with the distorted UV coordinates
	return tex2D(Sampler, uv);
}

technique DistortionTechnique {
	pass P0 {
		PixelShader = compile PS_SHADERMODEL Main();
	}
}
