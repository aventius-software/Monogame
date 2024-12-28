// Depending on if its opengl or not, we define a different pixel shader model
#if OPENGL    
    #define PS_SHADERMODEL ps_3_0
#else    
    #define PS_SHADERMODEL ps_4_0_level_9_1
#endif

// Strength of the background ambient light if no light source is present, values 0.0 to 1.0
//float BackgroundAmbientLightStrength;

float3 LightDirection;
float3 LightColor = 1.0;
float3 AmbientColor = 0.35;

sampler ScreenTexture;
sampler NormalTexture;

// We can name this function whatever, but we call it down below under the technique/pass section ;-)
float4 MainPixelShaderFunction(float2 textureCoordinates : TEXCOORD0) : COLOR0
{    
    //Look up the texture value
    float4 tex = tex2D(ScreenTexture, textureCoordinates);

	//Look up the normalmap value
    float4 normal = 2 * tex2D(NormalTexture, textureCoordinates) - 1;

	// Compute lighting.
    float lightAmount = saturate(dot(normal.xyz, LightDirection));
    float4 color = tex;
    color.rgb *= AmbientColor + (lightAmount * LightColor);

    return color * tex;
}

// Can be called whatever you like
technique PixelShaderTechnique
{
    // Also can be called whatever you like
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPixelShaderFunction();
    }
}