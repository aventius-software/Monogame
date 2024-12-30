// Depending on if its opengl or not, we define a different pixel shader model
#if OPENGL
    #define SV_POSITION POSITION
    #define VS_SHADERMODEL vs_3_0
    #define PS_SHADERMODEL ps_3_0
#else
    #define VS_SHADERMODEL vs_4_0_level_9_1
    #define PS_SHADERMODEL ps_4_0_level_9_1
#endif

// We need these so we can calculate the pixel position within
// the texture and also the 'world'. Otherwise we'd apply the
// same lighting to each 'sprite' (assuming this shader is being
// applied to several sprites), but we want sprites to be lit
// differently depending on their distance from a light source ;-)
float3 WorldPosition;
float2 TextureSize;

// Background ambient colour when no light source is present
float4 AmbientColour;

// Light source details
float3 LightPosition;
float4 LightColour;
float LightRadius;

// This is the texture passed by the sprite batch 'draw' method
sampler Texture;

// You'll need to supply the normal map texture as a parameter
sampler NormalMapTexture;

// We can name this function whatever, but we call it down below under the technique/pass section ;-)
float4 MainPixelShaderFunction(float2 textureCoordinates : TEXCOORD0) : COLOR0
{
    // Sample the texture color
    float4 pixelColour = tex2D(Texture, textureCoordinates);

    // Sample the normal map for the same pixel coordinates
    float3 normalPixelColour = tex2D(NormalMapTexture, textureCoordinates).rgb;
    
    // Transform from [0,1] to [-1,1]
    normalPixelColour = normalize((2 * normalPixelColour) - 1.0);

    // Calculate pixel position in texture and world
    float2 pixelTexturePosition = textureCoordinates * TextureSize;
    float3 pixelWorldPosition = WorldPosition + float3(pixelTexturePosition.x, pixelTexturePosition.y, 0);
    
    // Calculate the light direction and distance
    float3 lightDirection = LightPosition - pixelWorldPosition;
    float distance = length(lightDirection);
    lightDirection = normalize(lightDirection);

    // Calculate attenuation
    float attenuation = saturate(1.0 - (distance / LightRadius));

    // Calculate the diffuse lighting
    float diffuseIntensity = saturate(dot(normalPixelColour, lightDirection)) * attenuation;
    float3 diffuse = diffuseIntensity * LightColour;

    // Combine the lighting with the texture color
    float3 finalColour = (diffuse + AmbientColour.rgb) * pixelColour.rgb;

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