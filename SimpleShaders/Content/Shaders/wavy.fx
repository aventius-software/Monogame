#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

sampler spriteTexture;

float2 SineWaveOriginal(float2 p)
{
    float pi = 3.14159;
    float A = 0.15;
    float w = 10.0 * pi;
    float t = 30.0 * pi / 180.0;
    float y = sin(w * p.x + t) * A;
    return (p.x, p.y + y);
}

float2 SineWavetest(float2 textureCoordinates)
{
    float pi = 3.14159;
    float A = 0.75;
    float waveLength = 78;//   10.0 * pi;
    float t = 1;// 30.0 * pi / 180.0;
    float offsetX = sin(waveLength * textureCoordinates.x + t) * A;
    
    return float2(textureCoordinates.x, textureCoordinates.y + offsetX);
}

float2 SineWavexx(float2 textureCoordinates)
{
    float pi = 3.14159;
    float A = 0.75;
    float waveLength = 78; //   10.0 * pi;
    float t = 1; // 30.0 * pi / 180.0;
    float offsetX = sin(waveLength * textureCoordinates.x + t) * A;
    
    return float2(textureCoordinates.x, textureCoordinates.y + offsetX);
}

float SineWavenew(float2 p)
{
    float tx = 0.3477, ty = 0.7812;
    // convert Vertex position <-1,+1> to texture coordinate <0,1> and some shrinking so the effect dont overlap screen
    p.x = (0.55 * p.x) + 0.5;
    p.y = (-0.55 * p.y) + 0.5;
    // wave distortion
    float x = sin(25.0 * p.y + 30.0 * p.x + 6.28 * tx) * 0.05;
    float y = sin(25.0 * p.y + 30.0 * p.x + 6.28 * ty) * 0.05;
    return float2(p.x + x, p.y + y);
}

uniform float2 u_resolution;
uniform float2 u_mouse;
uniform float u_time;

float2 SineWave(float2 textureCoordinates)
{
    float pi = 3.14159;
    float x = 0.15 * sin(textureCoordinates.x * pi); // * 0.15;
    float y = 0;//    sin(textureCoordinates.y * -32.55); // * 0.15;
    
    //p.x = (0.55 * p.x) + 0.5;
    //p.y = (-0.55 * p.y) + 0.5;
    
    return float2(textureCoordinates.x + x, textureCoordinates.y + y);
}

float4 WobblyShader(float2 textureCoordinates : TEXCOORD0) : COLOR0
{
    //position.x += a * sin(k * position.y + f * time);
    //float4 color = tex2D(s0, float2(1 - coords.x, coords.y));
    //return color;
    //float1 offsetX = sin(textureCoordinates.y / 16) * 2;
    //float4 colour = tex2D(spriteTexture, float2(offsetX + textureCoordinates.x, textureCoordinates.y));
    //return colour;
    
    float2 wavePosition = SineWave(textureCoordinates);
    float4 colour = tex2D(spriteTexture, wavePosition);
    
    return colour;
        
    
}

technique BasicColorDrawing
{
	pass P0
	{		
        PixelShader = compile PS_SHADERMODEL WobblyShader();
    }
};

/*
precision mediump float;
varying vec2 v_texCoord;
uniform sampler2D s_baseMap;

vec2 SineWave( vec2 p ){
    float pi = 3.14159;
    float A = 0.15;
    float w = 10.0 * pi;
    float t = 30.0*pi/180.0;
    float y = sin( w*p.x + t) * A; 
    return vec2(p.x, p.y+y);　
}
void main(){
    vec2 p = v_texCoord; 
    vec2 uv = SineWave( p ); 
    vec4 tcolor = texture2D(s_baseMap, uv); 
    gl_FragColor = tcolor; 
}
*/