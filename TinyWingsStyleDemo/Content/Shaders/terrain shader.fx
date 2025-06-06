﻿#if OPENGL
    #define SV_POSITION POSITION
    #define VS_SHADERMODEL vs_3_0
    #define PS_SHADERMODEL ps_3_0
#else
    #define VS_SHADERMODEL vs_4_0_level_9_1
    #define PS_SHADERMODEL ps_4_0_level_9_1
#endif

// Input parameters
matrix World;
matrix View;
matrix Projection;

// Define the input structure for our vertex shader
struct VertexShaderInput
{
    float4 Position : POSITION0;
    float4 Colour : COLOR0;
    float2 TexureCoordinates : TEXCOORD0;
};

// Define the output structure for our vertex shader, note that
// we will pass the output from the vertex shader to the pixel
// shader in this structure!
struct VertexShaderOutput
{
    float4 Position : SV_Position;    
    float4 Colour : COLOR0;
    float2 PixelPosition : TEXCOORD0;
};

// Define the output for the pixel shader
struct PixelShaderOutput
{
    float4 Colour : COLOR0;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
    // We'll build up an output using this structure and pass it the pixel shader
    VertexShaderOutput output;
    
    // Generate transformation matrix
    float4x4 wvp = mul(World, mul(View, Projection));
    
    // Transform the position
    output.Position = mul(input.Position, wvp);
    
    // Just as normal...
    output.Colour = input.Colour;
    
    // For this particular shader we're not using a texture, so to get 
    // position to give to the pixel shader we 'fudge' and pass position 
    // via the 'TextureCoordinates' ;-)
    output.PixelPosition = input.Position.xy;
    
    return output;
}

PixelShaderOutput MainPS(VertexShaderOutput input)
{
    // Define the output for our pixel shader
    PixelShaderOutput output;
            
    // Extract the pixel position
    float2 pixelPosition = input.PixelPosition;

    // We'll use pixelPosition to create a simple terrain pattern    
    float spreadDistance = 450;
    float radius = 395;
    float2 position = fmod(pixelPosition, spreadDistance) / float2(radius, radius);

    // Calculate the distance from the center of the object
    float2 center = float2(0.5, 0.5);
    float distance = length(position - center);

    // Create a radial gradient pattern
    float gradient = smoothstep(0.0, 0.5, distance);
    output.Colour = lerp(float4(0.5, 1, 0, 1), float4(0, 0.5, 1, 1), gradient);
    
    return output;
}

technique DefaultTechnique
{
    pass
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};
