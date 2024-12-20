#if OPENGL	
	#define PS_SHADERMODEL ps_3_0
#else	
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

// Set this parameter to control the level of transparency
float TransparencyLevel;

// This will be the texture passed to the shader by the sprite batch draw function
sampler Texture;

float4 Transparency(float2 textureCoordinates : TEXCOORD0) : COLOR0
{
	// Get the pixel colour of the texture at the specified coordinates
    float4 pixelColour = tex2D(Texture, textureCoordinates);
    
    // Basically multiply the pixel colour by the transparency amount (0 to 1) to affect transparency
    float4 newPixelColour = pixelColour * TransparencyLevel;
	
    return newPixelColour;
}

technique TransparencyTechnique
{
	pass P0
	{		
        PixelShader = compile PS_SHADERMODEL Transparency();
    }
};