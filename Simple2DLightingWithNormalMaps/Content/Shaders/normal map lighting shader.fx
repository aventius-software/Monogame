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

// We can name this function whatever, but we call it down below under the technique/pass section ;-)
float4 MainPixelShaderFunction(float2 textureCoordinates : TEXCOORD0) : COLOR0
{            
    // Sample the texture color
    float4 pixelColour = tex2D(Texture, textureCoordinates);

    // Sample the normal map for the same pixel coordinates
    float3 normalPixelColour = tex2D(NormalMapTexture, textureCoordinates).rgb;
    
    // Transform from [0,1] to [-1,1]
    normalPixelColour = normalize((2 * normalPixelColour) - 1.0);

    // A 'rough' way to calculate the world position for this pixel
    float3 pixelWorldPosition = WorldPosition * float3(textureCoordinates.x, textureCoordinates.y, 1);
    
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









/*
// These are our main parameters for our pixel shader
float3 LightPosition;
float3 LightColour;
float3 AmbientColour;
float LightRadius;

// Not used!
matrix WorldViewProjection;

// This is the texture passed by the sprite batch 'draw' method
sampler Texture;

// You'll need to supply the normal map texture as a parameter
sampler NormalMapTexture;

// Define the input structure for the vertex shader (we'll use 
// this to get a world position to use in our pixel shader)
struct VertexShaderInput
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
};

// Define the output for the vertex shader, which we'll use as 
// the input for our pixel shader later on...
struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
};

// Define the vertex shader, we're only using this to get world position
// for our pixel shader really, its not actually doing anything...
VertexShaderOutput MainVS(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput) 0;

    output.Position = mul(input.Position, WorldViewProjection);
    output.Color = input.Color;

    return output;
}

// Now define our pixel shader using the 'output' from the 
// vertex shader as its input ;-)
float4 MainPS(VertexShaderOutput input) : COLOR
{
    return input.Color;
}

// Can be called whatever you like
technique ShaderTechnique
{
    // Also can be called whatever you like
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};
*/




/*
// You'll need to set these parameters to whatever you prefer
//float3 WorldPosition;
float3 LightPosition;
float3 LightColour;
float3 AmbientColour;
float LightRadius;

// This is the texture passed by the sprite batch 'draw' method
sampler Texture;

// You'll need to supply the normal map texture as a parameter
sampler NormalMapTexture;

//struct VertexInput
//{
//    float4 Position : POSITION;
//    float2 TexCoord : TEXCOORD0;
//};

struct PixelInput
{
    float2 TexCoord : TEXCOORD0;
    float3 WorldPosition : TEXCOORD1;
};

//PixelInput MainVertexShaderFunction(VertexInput input)
//{
//    PixelInput output;
//    output.TexCoord = input.TexCoord;
//    output.WorldPosition = input.Position.xyz;
//    return output;
//}

// We can name this function whatever, but we call it down below under the technique/pass section ;-)
float4 MainPixelShaderFunction(PixelInput input) : COLOR
{        
    // Sample the texture color
    float4 color = tex2D(Texture, input.TexCoord);

    // Sample the normal map
    float3 normal = tex2D(NormalMapTexture, input.TexCoord).rgb;
    
    // Transform from [0,1] to [-1,1]
    normal = normalize(normal * 2.0 - 1.0);

    // Calculate the light direction and distance
    float3 lightDir = LightPosition - input.WorldPosition.xyz;
    float distance = length(lightDir);
    lightDir = normalize(lightDir);

    // Calculate attenuation
    float attenuation = saturate(1.0 - (distance / LightRadius) * (distance / LightRadius));

    // Calculate the diffuse lighting
    float diffuseIntensity = max(dot(normal, lightDir), 0.0) * attenuation;
    float3 diffuse = diffuseIntensity * LightColour;

    // Combine the lighting with the texture color
    float3 finalColor = (diffuse + AmbientColour) * color.rgb;

    return float4(finalColor, color.a);
}

// Can be called whatever you like
technique PixelShaderTechnique
{
    // Also can be called whatever you like
    pass P0
    {
        //VertexShader = compile VS_SHADERMODEL MainVertexShaderFunction();
        PixelShader = compile PS_SHADERMODEL MainPixelShaderFunction();
    }
};
*/














/*

    // Sample the 'original' texture pixel colour
    float4 pixelColour = tex2D(Texture, textureCoordinates);

    // Sample the normal map texture (note we ignore the alpha)
    float3 normalMapPixelColour = tex2D(NormalMapTexture, textureCoordinates).rgb;
    
    // Transform from [0,1] to [-1,1]
    normalMapPixelColour = normalize(normalMapPixelColour * 2.0 - 1.0);

    // Calculate the diffuse lighting
    float3 lightDirection = LightDirection; //normalize(LightDirection);
    float diffuseIntensity = saturate(dot(normalMapPixelColour, lightDirection));
    float3 diffuseColour = diffuseIntensity * LightColour;

    // Combine the lighting with the texture colour
    float3 finalColour = pixelColour.rgb * (diffuseColour + AmbientColour);

    // Send back our new pixel colour
    return float4(finalColour, pixelColour.a);
    
*/