float4 MyMinimalPixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    // R,G,B,alpha
    return float4(1, 0, 0, 1);
}

technique Technique1
{
    pass Pass1
    {
        // ps_2_0 is an OpenGL shader, if using DirectX you'd need to change this
        PixelShader = compile ps_2_0 MyMinimalPixelShaderFunction();
    }
}