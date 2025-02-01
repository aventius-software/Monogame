#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

matrix World;
matrix View;
matrix Projection;
sampler Texture;

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float4 Colour : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Colour : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	// Sort out projection etc...
    float4x4 worldViewProjection = mul(mul(World, View), Projection);
	
	// Set the position and colour
    output.Position = mul(input.Position, worldViewProjection);
	output.Colour = input.Colour;
    output.TextureCoordinates = input.TextureCoordinates;

	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{	
	// Return the pixel at the adjusted coordinates
    return tex2D(Texture, input.TextureCoordinates);
}

technique MyBasicShaderTechnique
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};