// Depending on if its opengl or not, we define a different pixel shader model
#if OPENGL
	#define PS_SHADERMODEL ps_3_0
#else	
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

// Variable set to the colour tint we want
float4 Colour;

// This is the texture passed to the shader by the sprite draw command
sampler Texture;

// We can name this function whatever, but we call it down below under the technique/pass section ;-)
float4 MainPixelShaderFunction(float2 textureCoordinates : TEXCOORD0) : COLOR0
{
	// Get the colour of the pixel at the coordinates on the texture
    float4 originalPixelColour = tex2D(Texture, textureCoordinates);

	// Alter the pixel colour by the tint colour
    float4 tintedPixelColour = originalPixelColour * Colour;
	
	// Send back our new pixel colour for this position
    return tintedPixelColour;
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