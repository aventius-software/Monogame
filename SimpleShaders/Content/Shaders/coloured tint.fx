#if OPENGL
	#define PS_SHADERMODEL ps_3_0
#else	
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

// Variable set to the colour tint we want
float4 Colour;

// This is the texture passed to the shader by the sprite draw command
sampler Texture;

float4 ColouredTint(float2 textureCoordinates : TEXCOORD0) : COLOR0
{
	// Get the colour of the pixel at the coordinates on the texture
    float4 pixelColour = tex2D(Texture, textureCoordinates);

	// Alter the pixel colour by the tint colour
    float4 tintedPixelColour = pixelColour * Colour;
	
	// Send back our new pixel colour at this position
    return tintedPixelColour;
}

technique ColouredTintTechnique
{
	pass P0
	{
        PixelShader = compile PS_SHADERMODEL ColouredTint();
    }
};