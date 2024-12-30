// Depending on if its opengl or not, we define a different pixel shader model
#if OPENGL
    #define SV_POSITION POSITION
    #define VS_SHADERMODEL vs_3_0
    #define PS_SHADERMODEL ps_3_0
#else
    #define VS_SHADERMODEL vs_4_0_level_9_1
    #define PS_SHADERMODEL ps_4_0_level_9_1
#endif

// You'll need to set these parameters to whatever you prefer
float3 WorldPosition;
float3 LightPosition;
float3 LightColour;
float3 AmbientColour;
float LightRadius;

// This is the texture passed by the sprite batch 'draw' method
sampler Texture;

// You'll need to supply the normal map texture as a parameter
sampler NormalMapTexture;

// We'll use the output from the vertex shader as the input for the pixel shader, this
// is a workaround for certain shader versions that can't access position in a pixel
// shader...
struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Colour : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

// We can name this function whatever, but we call it down below under the technique/pass section ;-)
float4 MainPixelShaderFunction(VertexShaderOutput input) : COLOR
{ 
    //float3 wp = WorldPosition * 1;
    //float2 TopLeftScreen = float2(wp.x, wp.y);
    //float2 SizeScreen = float2(800, 600);
    //float2 screenCoord = TopLeftScreen + textureCoordinates * SizeScreen;
    //float3 screenPosition = float3(screenCoord.x, screenCoord.y, 0);
    
    //Vector2 TopLeftScreen = ImageTopLeft / ScreenResolution;
    //Vector2 SizeScreen = ImageSize / ScreenResolution;
    
    // Sample the texture color
    float4 pixelColour = tex2D(Texture, input.TextureCoordinates);

    // Sample the normal map for the same pixel coordinates
    float3 normalPixelColour = tex2D(NormalMapTexture, input.TextureCoordinates).rgb;
    
    // Transform from [0,1] to [-1,1]
    normalPixelColour = normalize((2 * normalPixelColour) - 1.0);

    // A very 'rough' way to calculate the world position for this pixel
    float3 pixelWorldPosition = WorldPosition * float3(input.TextureCoordinates.x, input.TextureCoordinates.y, 1);
    
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
    float3 finalColour = (diffuse + AmbientColour) * pixelColour.rgb;

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