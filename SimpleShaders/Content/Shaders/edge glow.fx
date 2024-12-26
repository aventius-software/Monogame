// Depending on if its opengl or not, we define a different pixel shader model
#if OPENGL
    #define PS_SHADERMODEL ps_3_0
#else    
    #define PS_SHADERMODEL ps_4_0_level_9_1
#endif

// This is the texture passed to the shader by the sprite batch 'draw' method
sampler Texture;

// Set these parameters as per normal...
float GlowIntensity;
float GlowThreshold;
float2 TextureDimensions;

// We can name this function whatever, but we call it down below under the technique/pass section ;-)
float4 MainPixelShaderFunction(float2 textureCoordinates : TEXCOORD0) : COLOR0
{
    // Define the size of a 'texel' (pixel in the texture)
    float2 texelSize = float2(1.0 / TextureDimensions.x, 1.0 / TextureDimensions.y);

    // Define matrices to use with the 'Sobel' algorithm as 
    // we'll try to detect edges by calculating the gradient 
    // of the image intensity. So, if its a flat gradient then
    // less likely its an edge, and vice-versa ;-)
    float3x3 sobelX = float3x3(
        -1, 0, 1,
        -2, 0, 2,
        -1, 0, 1
    );

    float3x3 sobelY = float3x3(
        -1, -2, -1,
         0, 0, 0,
         1, 2, 1
    );

    // Sample the surrounding pixels colours
    float3 sample[9];
    sample[0] = tex2D(Texture, textureCoordinates + texelSize * float2(-1, -1)).rgb;
    sample[1] = tex2D(Texture, textureCoordinates + texelSize * float2(0, -1)).rgb;
    sample[2] = tex2D(Texture, textureCoordinates + texelSize * float2(1, -1)).rgb;
    sample[3] = tex2D(Texture, textureCoordinates + texelSize * float2(-1, 0)).rgb;
    sample[4] = tex2D(Texture, textureCoordinates + texelSize * float2(0, 0)).rgb;
    sample[5] = tex2D(Texture, textureCoordinates + texelSize * float2(1, 0)).rgb;
    sample[6] = tex2D(Texture, textureCoordinates + texelSize * float2(-1, 1)).rgb;
    sample[7] = tex2D(Texture, textureCoordinates + texelSize * float2(0, 1)).rgb;
    sample[8] = tex2D(Texture, textureCoordinates + texelSize * float2(1, 1)).rgb;

    float3 sobelXResult = float3(0.0, 0.0, 0.0);
    float3 sobelYResult = float3(0.0, 0.0, 0.0);

    for (int i = 0; i < 3; i++)
    {
        for (int j = 0; j < 3; j++)
        {
            sobelXResult += sample[i * 3 + j] * sobelX[i][j];
            sobelYResult += sample[i * 3 + j] * sobelY[i][j];
        }
    }

    float edgeStrength = length(sobelXResult + sobelYResult);

    // Check if the pixel is an edge based on the edge strength
    if (edgeStrength > GlowThreshold)
    {
        // We 'think' it is, so we apply the glow
        float4 glow = float4(GlowIntensity, GlowIntensity, GlowIntensity, 1.0);
        return tex2D(Texture, textureCoordinates) + glow;
    }

    // Otherwise we 'think' its just a normal pixel
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