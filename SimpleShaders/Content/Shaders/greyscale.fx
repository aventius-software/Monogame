#if OPENGL	
	#define PS_SHADERMODEL ps_3_0
#else	
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

// Set the level of greyscale 0 to 1
float GreyscaleLevel;

// This is the texture passed to the shader by the sprite batch 'draw' method
sampler Texture;

float4 Greyscale(float2 textureCoordinates : TEXCOORD0) : COLOR0
{
	// Get the colour of the pixel for the texture at the specified coordinates
    float4 pixelColour = tex2D(Texture, textureCoordinates);
	
	// Alter colour to make it greyscale
    pixelColour.rgb = (pixelColour.r + pixelColour.g + pixelColour.b) * GreyscaleLevel;    
	
	// Return our altered pixel colour
    return pixelColour;
}

technique GreyscaleTechnique
{
	pass P0
	{
        PixelShader = compile PS_SHADERMODEL Greyscale();
    }
};