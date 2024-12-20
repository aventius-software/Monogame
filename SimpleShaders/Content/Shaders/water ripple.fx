#if OPENGL
    #define PS_SHADERMODEL ps_3_0
#else    
    #define PS_SHADERMODEL ps_4_0_level_9_1
#endif

// This is the texture passed to the shader by the sprite batch 'draw' method
sampler Texture;

// Set these parameters as per normal...
float2 RippleCenter;
float Time;
float Amplitude;
float Frequency;

float4 WaterRipple(float2 textureCoordinates : TEXCOORD0) : COLOR0
{
    // Find center for the ripple
    float2 toCenter = textureCoordinates - RippleCenter;
    float distance = length(toCenter);
    
    // Calculate the ripple
    float ripple = sin(distance * Frequency - Time) * Amplitude;
    
    // Adjust coordinates to find pixel
    textureCoordinates += normalize(toCenter) * ripple;
    
    // Return the pixel at the adjusted coordinates
    return tex2D(Texture, textureCoordinates);
}

technique WaterRippleTechnique
{
    pass Pass0
    {
        PixelShader = compile PS_SHADERMODEL WaterRipple();
    }
};