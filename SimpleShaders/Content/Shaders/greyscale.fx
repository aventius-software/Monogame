#if OPENGL	
	#define PS_SHADERMODEL ps_3_0
#else	
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float GreyscaleLevel;
sampler Texture;

float4 Greyscale(float2 textureCoordinates : TEXCOORD0) : COLOR0
{
	// Get the colour of the pixel for the texture at the specified coordinates
    float4 pixelColour = tex2D(Texture, textureCoordinates);
	
	// Alter colour to make it greyscale
    pixelColour.rgb = (pixelColour.r + pixelColour.g + pixelColour.b) * GreyscaleLevel; // / 3.5f;
	
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

/*
Texture2D SpriteTexture;

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float4 col = tex2D(SpriteTextureSampler, input.TextureCoordinates) * input.Color;
    col.rgb = (col.r + col.g + col.b) / 3.0f;
    return col;
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};
*/