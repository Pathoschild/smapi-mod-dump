#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float saturation;
float3 palette[64];
int palettesize;


sampler2D TextureSampler : register(s0)
{
    Texture = (Texture);
};


float4 MainPS(float4 position : SV_Position, float4 color : COLOR0, float2 TextureCoordinates : TEXCOORD0) : COLOR0
{
    float4 col = tex2D(TextureSampler, TextureCoordinates) * color;
	
	if(col.a == 0){
		return col;
	}
	
	if(saturation != 1){
	float l = 0.2125 * col.r + 0.7154 * col.g + 0.0721 * col.b;
	float s = 1 - saturation;
	
     col.r = max(0,min(1,col.r + s * (l - col.r)));
     col.g = max(0,min(1,col.g + s * (l - col.g)));
     col.b = max(0,min(1,col.b + s * (l - col.b)));
	}
	if(palettesize > 0){
		int closestcolor = 0;
		float mindistance = 1000000.0;
		for(int i = 0; i < 64; i++){
				float sampledistance = distance(col.rgb,palette[i].rgb);
				if(sampledistance < mindistance){
					closestcolor = i;
					mindistance = sampledistance;
				}
			}
	
		return float4(palette[closestcolor].r,palette[closestcolor].g,palette[closestcolor].b,1);
	}

    return col;
}

technique Visualize
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};