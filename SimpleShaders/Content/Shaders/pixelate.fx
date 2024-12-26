// Depending on if its opengl or not, we define a different pixel shader model
#if OPENGL	
	#define PS_SHADERMODEL ps_3_0
#else	
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

// Size of the pixels
float PixelSize;

// Width and height of the texture
float2 TextureDimensions;

// This is the texture, gets passed to the shader by the sprite draw method
sampler Texture;

// We can name this function whatever, but we call it down below under the technique/pass section ;-)
float4 MainPixelShaderFunction(float2 textureCoordinates : TEXCOORD0) : COLOR0
{
    // Adjust the texture coordinates so we get the same pixel position depending
    // on the size of the pixelation requested (PixelSize) and the texture dimensions
    float2 newTextureCoordinates = floor(textureCoordinates * TextureDimensions / PixelSize) * PixelSize / TextureDimensions;
    
    // Get the pixel colour of the texture at the adjusted coordinates
    return tex2D(Texture, newTextureCoordinates);
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