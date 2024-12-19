#if OPENGL
    #define PS_SHADERMODEL ps_3_0
#else    
    #define PS_SHADERMODEL ps_4_0_level_9_1
#endif

// Call the function whatever you like
float4 MyMinimalPixelShaderFunction(float2 textureCoordinates : TEXCOORD0) : COLOR0
{
    // Make up some new r,g,b,a values (between 0 and 1)
    float r = 1, g = 0, b = 0, a = 1;
    
    // return our new pixel colour
    return float4(r, g, b, a);
}

// Call the technique whatever you like
technique MyTechnique
{
    // Call the pass whatever too ;-)
    pass MyPass
    {
        // Name of the function here is the name of the main function that will return
        // your new pixel colour, in our case its this ;-)
        PixelShader = compile PS_SHADERMODEL MyMinimalPixelShaderFunction();
    }
}