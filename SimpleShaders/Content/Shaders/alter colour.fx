#if OPENGL
    #define SV_POSITION POSITION
    #define VS_SHADERMODEL vs_3_0
    #define PS_SHADERMODEL ps_3_0
#else
    #define VS_SHADERMODEL vs_4_0_level_9_1
    #define PS_SHADERMODEL ps_4_0_level_9_1
#endif

sampler spriteTexture;

float4 MyPixelShaderFunction(float2 textureCoordinate : TEXCOORD0) : COLOR0
{
    float4 colour = tex2D(spriteTexture, textureCoordinate);
    colour.gb = colour.r;
    
    return colour;
}

technique Technique1
{
    pass Pass1
    {        
        PixelShader = compile PS_SHADERMODEL MyPixelShaderFunction();
    }
}