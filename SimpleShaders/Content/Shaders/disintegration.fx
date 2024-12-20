#if OPENGL
    #define PS_SHADERMODEL ps_3_0
#else    
    #define PS_SHADERMODEL ps_4_0_level_9_1
#endif

// This is the texture passed to the shader by the sprite batch 'draw' method
sampler Texture;

// Set these parameters as per normal...
float DisintegrationThreshold;

float4 Disintegrate(float2 textureCoordinates : TEXCOORD0) : COLOR0
{    
    // Play with the numbers to generate different 'noise' styles, the more 'random' the
    // numbers, the more random the noise will look like. Clean numbers will produce
    // more predictable 'shapes' and effect ;-)
    float noise = frac(sin(dot(textureCoordinates, float2(1.123, 2.123))) * 123456);
    
    // Values 0 to 1, with 1 being total disintegration ;-)
    if (noise < DisintegrationThreshold)
    {
        discard;        
    }
    
    return tex2D(Texture, textureCoordinates);
}

technique DisintegrateTechnique
{
    pass Pass0
    {
        PixelShader = compile PS_SHADERMODEL Disintegrate();
    }
};