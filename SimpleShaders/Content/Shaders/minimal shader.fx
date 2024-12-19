#if OPENGL
    #define PS_SHADERMODEL ps_3_0
#else    
    #define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float4 MyMinimalPixelShaderFunction(float2 textureCoordinates : TEXCOORD0) : COLOR0
{
    float r = 1, g = 0, b = 0, a = 1;
        
    return float4(r, g, b, a);
}

technique MyTechnique
{
    pass MyPass
    {        
        PixelShader = compile PS_SHADERMODEL MyMinimalPixelShaderFunction();
    }
}