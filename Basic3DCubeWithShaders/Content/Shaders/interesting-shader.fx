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
float Time;
sampler Texture;

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float4 Color : COLOR0;
    float3 Normal : NORMAL;
    float2 TextureCoordinates : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
    float3 Normal : NORMAL;
    float2 TextureCoordinates : TEXCOORD0;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{		
    VertexShaderOutput output = (VertexShaderOutput) 0;
    //output.Color = input.Color;
    output.TextureCoordinates = input.TextureCoordinates;
    
    /*	    
    float4x4 worldViewProjection = mul(mul(World, View), Projection);
    
    float waveHeight = sin(input.Position.x * 2.0 + Time) * 0.5;
    float3 newPos = input.Position;
    newPos.y += waveHeight;

    // Transform the vertex position by the WorldViewProjection matrix
    float4 transformedPos = mul(float4(newPos, 1.0), worldViewProjection);
    output.Position = transformedPos;

    // Pass through the normal
    output.Normal = input.Normal;
    */
    
    float4x4 worldViewProjection = mul(mul(World, View), Projection);
    output.Position = mul(input.Position, worldViewProjection);
    output.Color = input.Color;

    return output;		
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float2 textureCoordinates = input.TextureCoordinates;
    
    float Frequency = 2;
    float Amplitude = 0.5;
    float2 RippleCenter = float2(0.5, 0.5);
    
    // Find center for the ripple
    float2 toCenter = textureCoordinates - RippleCenter;
    float distance = length(toCenter);
    
    // Calculate the ripple
    float ripple = sin(distance * Frequency - Time) * Amplitude;
    
    // Adjust coordinates to find pixel
    textureCoordinates += normalize(toCenter) * ripple;
    
    // Return the pixel at the adjusted coordinates
    return tex2D(Texture, textureCoordinates);
    
	//return input.Color;
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};