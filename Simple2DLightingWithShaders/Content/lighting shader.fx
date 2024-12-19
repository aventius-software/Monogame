#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

// Strength of the background ambient light if no light source is present, values 0.0 to 1.0
float backgroundAmbientLightStrength;

// The sprite batch draw texture will be the first texture passed to the shader, so
// we will assume this to be the background texture
sampler backgroundTexture;

// Next we need our texture containing all the light sources we've drawn to it...
sampler lightSourcesTexture;

float4 Simple2DLighting(float2 backgroundTextureCoordinates : TEXCOORD0) : COLOR0
{
    // Get the colour of the pixel from the background texture
    float4 backgroundPixelColour = tex2D(backgroundTexture, backgroundTextureCoordinates);

	// Next, get the colour of the pixel for the light sources texture. Since the background 
	// render target and the light sources render target are (and must be) the same size, we're 
	// getting the pixel at the same exact position on each texture. So we don't need to calculate
	// another set of coordinates. If they were different sizes we'd end up with some different 
	// weird offset effects, so always keep them both matching sizes ;-)
    float4 lightSourcesPixelColour = tex2D(lightSourcesTexture, backgroundTextureCoordinates);
	
	// Usually if any pixel where the light source pixel colour is black will be changed to
	// black, since we multiply the background pixel colour by the light source colour (i.e. anything
	// multiplied by zero is zero, of course). So sometimes you might want a little ambient light
	// for the rest of the background, if this value is set then anything outside the light sources
	// that would normally be black, now becomes the level of the ambient light strength
    if (lightSourcesPixelColour.r < backgroundAmbientLightStrength)
    {
        return backgroundPixelColour * backgroundAmbientLightStrength;
    }
	
	// If we're here, there is no ambient light value (i.e. its zero). So we just take the
	// background pixel colour and multiply by the light source pixel colour to get the final
	// colour for our background texture pixel ;-)
    return backgroundPixelColour * lightSourcesPixelColour;
}

technique LightingDrawing
{
	pass P0
	{		
        PixelShader = compile PS_SHADERMODEL Simple2DLighting();
    }
};