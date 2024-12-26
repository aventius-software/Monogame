// Depending on if its opengl or not, we define a different pixel shader model
#if OPENGL
    #define PS_SHADERMODEL ps_3_0
#else    
    #define PS_SHADERMODEL ps_4_0_level_9_1
#endif

// This is the texture passed to the shader by the sprite batch 'draw' method
sampler Texture;

// Set parameters as per normal
float Strength;
float Time;

// We can name this function whatever, but we call it down below under the technique/pass section ;-)
float4 MainPixelShaderFunction(float2 textureCoordinates : TEXCOORD0) : COLOR0
{    
    // Calculate the angle based on the texture coordinates and time
    float angle = atan2(textureCoordinates.y - 0.5, textureCoordinates.x - 0.5) + Time;

    // Calculate the spin effect
    float spin = sin(angle * Strength);

    // Sample the pixel at these coordinates
    float4 pixelColour = tex2D(Texture, textureCoordinates);

    // Apply the effect to each of the colour channels
    pixelColour.r *= 1.75 * spin;
    pixelColour.g *= 1.75 * spin;
    pixelColour.b *= 1.75 * spin;
    
    // Alter the alpha of non-transparent pixels over time
    if (pixelColour.a != 0.0) pixelColour.a = sin(Time * Strength / 4);

    return pixelColour;
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