// Depending on if its opengl or not, we define a different pixel shader model
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

// We can name this function whatever, but we call it down below under the technique/pass section ;-)
float4 MainPixelShaderFunction(float2 textureCoordinates : TEXCOORD0) : COLOR0
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

// Can be called whatever you like
technique PixelShaderTechnique
{
    // Also can be called whatever you like
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPixelShaderFunction();
    }
};