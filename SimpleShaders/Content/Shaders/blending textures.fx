// Depending on if its opengl or not, we define a different pixel shader model
#if OPENGL	
	#define PS_SHADERMODEL ps_3_0
#else	
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

// Defines the amount of blending you want
float BlendingAmount;

// The first texture is set by the sprite batch 'draw' function and whatever texture you use
sampler StartingTexture;

// But we need to specify the next texture as a parameter for the shader (see 'GameMain')
sampler BlendingTexture;

// We can name this function whatever, but we call it down below under the technique/pass section ;-)
float4 MainPixelShaderFunction(float2 textureCoordinates : TEXCOORD0) : COLOR0
{
    // Get the pixel colours from the 2 textures
    float4 startingTexturePixelColour = tex2D(StartingTexture, textureCoordinates);
    float4 blendingTexturePixelColour = tex2D(BlendingTexture, textureCoordinates);
    
    // Blend pixels depending on the requested blending amount, could try other
    // interpolation functions like smoothstep to get different effects ;-)
    return lerp(startingTexturePixelColour, blendingTexturePixelColour, BlendingAmount);
}

// Can be called whatever you like
technique PixelShaderTechnique
{
    // Also can be called whatever you like
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPixelShaderFunction();
    }
};