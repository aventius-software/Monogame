// Depending on if its opengl or not, we define a different pixel shader model
#if OPENGL    
    #define PS_SHADERMODEL ps_3_0
#else    
    #define PS_SHADERMODEL ps_4_0_level_9_1
#endif

// You'll need to set these parameters to whatever you prefer
float3 LightDirection;
float3 LightColour;
float3 AmbientColour;

// This is the texture passed by the sprite batch 'draw' method
sampler Texture;

// You'll need to supply the normal map texture as a parameter
sampler NormalMapTexture;

// We can name this function whatever, but we call it down below under the technique/pass section ;-)
float4 MainPixelShaderFunction(float2 textureCoordinates : TEXCOORD0) : COLOR0
{
    // Sample the 'original' texture pixel colour
    float4 pixelColour = tex2D(Texture, textureCoordinates);

    // Sample the normal map texture (note we ignore the alpha)
    float3 normalMapPixelColour = tex2D(NormalMapTexture, textureCoordinates).rgb;
    
    // Transform from [0,1] to [-1,1]
    normalMapPixelColour = normalize(normalMapPixelColour * 2.0 - 1.0);

    // Calculate the diffuse lighting
    float3 lightDirection = normalize(LightDirection);
    float diffuseIntensity = saturate(dot(normalMapPixelColour, lightDirection));
    float3 diffuseColour = diffuseIntensity * LightColour;

    // Combine the lighting with the texture colour
    float3 finalColour = pixelColour.rgb * (diffuseColour + AmbientColour);

    // Send back our new pixel colour
    return float4(finalColour, pixelColour.a);
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