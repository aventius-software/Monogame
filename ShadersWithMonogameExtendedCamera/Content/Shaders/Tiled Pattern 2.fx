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

// Hash function for pseudo-random number generation
float hash(float2 p)
{
    return frac(sin(dot(p, float2(12.9898, 78.233))) * 43758.5453);
}

// Tileable noise function using Perlin-like interpolation
float tiledNoise(float2 p)
{
    float2 i = floor(p);
    float2 f = frac(p);

	// Smoothstep interpolation for seamless blending
    float2 u = f * f * (3.0 - 2.0 * f);

	// Hash the four corners
    float n00 = hash(i + float2(0.0, 0.0));
    float n10 = hash(i + float2(1.0, 0.0));
    float n01 = hash(i + float2(0.0, 1.0));
    float n11 = hash(i + float2(1.0, 1.0));

	// Interpolate horizontally
    float nx0 = lerp(n00, n10, u.x);
    float nx1 = lerp(n01, n11, u.x);

	// Interpolate vertically
    return lerp(nx0, nx1, u.y);
}

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

// This is our main pixel shader. This is where the pattern is generated. The 
// world position is converted to UV coordinates in tile space
float4 MainPS(VertexShaderOutput input) : COLOR
{
	// Convert to 0–1 tile space
    float2 uv = input.WorldPosition / TileWidth;

	// Wrap explicitly
    uv = frac(uv);

	// Generate tileable noise at this position, scaled for finer detail
    float noise = tiledNoise(uv * 4.0);

	// Use the noise value to create a grayscale pattern
    return float4(noise, noise, noise, 1.0);
}

technique BasicColorDrawing
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};