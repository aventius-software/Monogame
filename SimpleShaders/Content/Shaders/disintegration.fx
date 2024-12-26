// Depending on if its opengl or not, we define a different pixel shader model
#if OPENGL
    #define PS_SHADERMODEL ps_3_0
#else    
    #define PS_SHADERMODEL ps_4_0_level_9_1
#endif

// This is the texture passed to the shader by the sprite batch 'draw' method
sampler Texture;

// Set these parameters as per normal...
float DisintegrationThreshold;

// We can name this function whatever, but we call it down below under the technique/pass section ;-)
float4 MainPixelShaderFunction(float2 textureCoordinates : TEXCOORD0) : COLOR0
{    
    // Play with the numbers to generate different 'noise' styles, the more 'random' the
    // numbers, the more random the noise will look like. Using 'cleaner' numbers will produce
    // more predictable 'shapes' and effects ;-)
    float noise = frac(sin(dot(textureCoordinates, float2(1.123, 2.123))) * 123456);
    
    // Values 0 to 1, with 1 being total disintegration ;-)
    if (noise < DisintegrationThreshold)
    {
        discard;        
    }
    
    return tex2D(Texture, textureCoordinates);
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