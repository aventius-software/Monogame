// Depending on if its opengl or not, we define a different pixel shader model
#if OPENGL    
    #define PS_SHADERMODEL ps_3_0
#else    
    #define PS_SHADERMODEL ps_4_0_level_9_1
#endif

// How much 'split' you want between R,G,B
float Amount;

// This will be set by the sprite batch draw method texture
sampler Texture;

// We can name this function whatever, but we call it down below under the technique/pass section ;-)
float4 MainPixelShaderFunction(float2 textureCoordinates : TEXCOORD0) : COLOR0
{
    // Get the pixel colour at the current coordinates
    float4 pixelColour = tex2D(Texture, textureCoordinates);
    
    // If no colour then just return the pixel as it is...
    if (!any(pixelColour))
    {
        return pixelColour;
    }
    
    // Get coordinates...
    float2 uv = textureCoordinates.xy;
    float3 colour;
    
    // Alter (shift) colours
    colour.r = tex2D(Texture, float2(uv.x + Amount, uv.y)).r;
    colour.g = tex2D(Texture, uv).g;
    colour.b = tex2D(Texture, float2(uv.x - Amount, uv.y)).b;

    colour *= (1.0 - Amount * 0.5);

    // Return new pixel colour ;-)
    return float4(colour, 1.0);
}

// Can be called whatever you like
technique PixelShaderTechnique
{
    // Also can be called whatever you like
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPixelShaderFunction();
    }
}