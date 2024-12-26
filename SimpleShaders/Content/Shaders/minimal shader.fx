// Depending on if its opengl or not, we define a different pixel shader model
#if OPENGL
    #define PS_SHADERMODEL ps_3_0
#else    
    #define PS_SHADERMODEL ps_4_0_level_9_1
#endif

// We can name this function whatever, but we call it down below under the technique/pass section ;-)
float4 MainPixelShaderFunction(float2 textureCoordinates : TEXCOORD0) : COLOR0
{
    // Make up some new r,g,b,a values (between 0 and 1)
    float r = 1, g = 0, b = 0, a = 1;
    
    // return our new pixel colour for this pixel at the current coordinates
    return float4(r, g, b, a);
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