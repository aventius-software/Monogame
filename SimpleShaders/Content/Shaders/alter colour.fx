#if OPENGL    
    #define PS_SHADERMODEL ps_3_0
#else    
    #define PS_SHADERMODEL ps_4_0_level_9_1
#endif

// This will be set by the sprite batch draw method texture
sampler Texture;

// Just alter the texture pixel colours a bit
float4 AlterColour(float2 textureCoordinate : TEXCOORD0) : COLOR0
{
    // Get the colour of the pixel at the specified coordinates in the texture
    float4 colour = tex2D(Texture, textureCoordinate);
    
    // Just set the red of the r,g,b,a to zero (i.e. no red)
    colour.r = 0;
    
    // Return our 'amended' pixel colour
    return colour;
}

technique AlterColourTechnique
{
    pass P0
    {        
        PixelShader = compile PS_SHADERMODEL AlterColour();
    }
}