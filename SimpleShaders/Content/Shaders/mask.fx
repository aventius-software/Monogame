// Depending on if its opengl or not, we define a different pixel shader model
#if OPENGL
    #define PS_SHADERMODEL ps_3_0
#else    
    #define PS_SHADERMODEL ps_4_0_level_9_1
#endif

// This will be set by the sprite batch draw method texture
sampler Texture;

// We can name this function whatever, but we call it down below under the technique/pass section ;-)
float4 MainPixelShaderFunction(float2 textureCoordinates : TEXCOORD0) : COLOR0
{
    // Get the pixel colour at the specified coordinates in the texture
    float4 pixelColour = tex2D(Texture, textureCoordinates);

    // If existing colour, ignore and return current pixel
    if (!any(pixelColour))
        return pixelColour;

    // Return white
    return float4(1, 1, 1, 1);
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