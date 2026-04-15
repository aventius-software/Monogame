#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

// Parameters which get passed to the shader from the C# code
matrix ViewProjection;
float TileWidth;

// Various fixed values which control the look of the pattern. These
// could be converted to parameters if you want to be able to change 
// them at runtime. However, for simplicity, I've just left them as 
// defines here in order to keep the C# code as simple as possible.
#define Blurryness 0.005f

// This is the struct which gets passed to the vertex shader as input
struct VertexShaderInput
{
	float4 Position : POSITION0;
	float4 Colour : COLOR0;
};

// This is the struct which is output from the vertex shader and which gets 
// passed as 'input' to the pixel shader.
struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Colour : COLOR0;
	float2 WorldPosition : TEXCOORD0;
};

// Our main vertex shader. This is pretty simple - we just transform the position
// using the view projection matrix and pass through the colour and world position 
// to the pixel shader. Although in this instance we're not using any colour. The 
// world position is needed in the pixel shader in order to calculate the UV 
// coordinates for the pattern so that they move in sync with the camera.
VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output;

	output.Position = mul(input.Position, ViewProjection);
	output.Colour = input.Colour;
    output.WorldPosition = input.Position.xy;

	return output;
}

// This is our main pixel shader. This is where the pattern is generated. The world position 
// is converted to UV coordinates in tile space and then we use a combination of noise and 
// sine waves to generate a pattern. Finally, we use smoothstep to create a blurred effect 
// around the edges of the pattern.
float4 MainPS(VertexShaderOutput input) : COLOR
{	
	// Convert to 0–1 tile space
    float2 uv = input.WorldPosition / TileWidth;

    // Wrap explicitly
    uv = frac(uv);

	uv.x += 0.2 * sin(uv.y * 4.);
    float numLines = 15. + uv.y * 0.4;
    float colNoise = noise(0.6 * uv.x * numLines);
    float colStripes = 0.5 + 0.5 * sin(uv.x * numLines * 0.75);
    float col = lerp(colNoise, colStripes, 0.5 + 0.5);
    float aA = 1. / (TileWidth * Blurryness);
    col = smoothstep(0.5 - aA, 0.5 + aA, col);
	        
    return float4(col, col, col, 1.0);
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};