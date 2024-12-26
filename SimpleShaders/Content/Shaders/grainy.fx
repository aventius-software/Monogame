// Depending on if its opengl or not, we define a different pixel shader model
#if OPENGL
    #define PS_SHADERMODEL ps_3_0
#else    
    #define PS_SHADERMODEL ps_4_0_level_9_1
#endif

// This will be set by the sprite batch draw method texture
sampler Texture;

// Set these parameters as per normal
float GrainIntensity;
float Time;

// We can name this function whatever, but we call it down below under the technique/pass section ;-)
float4 MainPixelShaderFunction(float2 textureCoordinates : TEXCOORD0) : COLOR0
{
    // Generate some random noise based on the pixel coordinates and time, fiddle around with these
    // 'random' numbers to see the different styles of 'noise' that get produced until you find what 
    // you want. Try to use 'messy' numbers for the most 'random' effect
    float noise = frac(sin(dot(textureCoordinates * float2(12.123, 58.123), float2(33718.1234, 53728.1234))) * 63453.1234 + Time);

    // Sample the texture
    float4 pixelColour = tex2D(Texture, textureCoordinates);

    // Apply the grain effect
    pixelColour.rgb += (noise - 0.5) * GrainIntensity;

    // Send back our 'new' pixel colour
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