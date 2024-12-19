#if OPENGL	
	#define PS_SHADERMODEL ps_3_0
#else	
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

// Defines the amount of blending you want
float BlendingAmount;

// First texture is set by the draw sprite and whatever texture you use
sampler StartingTexture;

// We specify the next texture as a parameter
sampler BlendingTexture;

float4 BlendingTextures(float2 textureCoordinates : TEXCOORD0) : COLOR0
{
    // Get the pixel colours from the 2 textures
    float4 startingTexturePixelColour = tex2D(StartingTexture, textureCoordinates);
    float4 blendingTexturePixelColour = tex2D(BlendingTexture, textureCoordinates);
    
    // Blend pixels depending on the requested blending amount, could try other
    // interpolation functions like smoothstep ;-)
    return lerp(startingTexturePixelColour, blendingTexturePixelColour, BlendingAmount);
}

technique BlendingTexturesTechnique
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL BlendingTextures();
    }
};