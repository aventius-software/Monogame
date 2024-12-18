sampler spriteTexture;

float4 MyPixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(spriteTexture, coords);
    color.gb = color.r;
    return color;
}

technique Technique1
{
    pass Pass1
    {        
        PixelShader = compile ps_3_0 MyPixelShaderFunction();
    }
}