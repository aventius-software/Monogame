#if OPENGL	
	#define PS_SHADERMODEL ps_3_0
#else	
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float transparencyLevel;
sampler targetTexture;

float4 Transparency(float2 textureCoordinates : TEXCOORD0) : COLOR0
{
	// Sample the texture at the given texture coordinate
    float4 pixelColour = tex2D(targetTexture, textureCoordinates);
    
    // Transparency ;-) Maybe add a parameter so you could control the level
    pixelColour *= transparencyLevel;
	
    return pixelColour;
}

technique TransparencyTechnique
{
	pass P0
	{		
        PixelShader = compile PS_SHADERMODEL Transparency();
    }
};