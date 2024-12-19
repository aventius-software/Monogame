#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

// Defines the amount of blending you want
float blendingAmount;

// First texture is set by the draw sprite and whatever texture you use
sampler startingTexture;

// We specify the next texture as a parameter
sampler blendWithTexture;

float4 BlendingTextures(float2 textureCoordinates : TEXCOORD0) : COLOR0
{
    // Get the pixel colours from the 2 textures
    float4 colour1 = tex2D(startingTexture, textureCoordinates);
    float4 colour2 = tex2D(blendWithTexture, textureCoordinates);
    
    // Blend pixels depending on the requested blending amount
    return lerp(colour1, colour2, blendingAmount);
}

technique BlendingTexturesTechnique
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL BlendingTextures();
    }
};