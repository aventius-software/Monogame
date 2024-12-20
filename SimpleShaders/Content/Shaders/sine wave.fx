#if OPENGL    
    #define PS_SHADERMODEL ps_3_0
#else    
    #define PS_SHADERMODEL ps_4_0_level_9_1
#endif

// This will be set by the sprite batch draw method texture
sampler Texture;

// High values will produce more waves
float Frequency;

// How 'far out' the wave will be, between 0 and 1, a value of 1 will 'wave' 
// outwards at the full width of the texture. So somewhere smaller is a nice
// place to start
float Amplitude;

// This doesn't have to be time, but needs to be small incrementing value
// that can give us a change in the sin value
float Time;

// Just alter the texture pixel colours a bit
float4 SineWave(float2 textureCoordinate : TEXCOORD0) : COLOR0
{
    // We get a pixel from the current x coordinate but with an offset
    // depending on the following variables. We use the 'time' to get
    // changes in order to 'animate' over a time period
    float newX = textureCoordinate.x + sin(textureCoordinate.y * Frequency + Time) * Amplitude;
    
    // Now we've got some new coordinates, note that we always use the same
    // y coordinate as we're only 'waving' horizontally in this shader ;-)
    float2 newTextureCoordinates = float2(newX, textureCoordinate.y);
    
    // Get the colour of the pixel at the new coordinates in the texture
    float4 pixelColour = tex2D(Texture, newTextureCoordinates);
        
    // Return our new pixel colour
    return pixelColour;
}

technique SineWaveTechnique
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL SineWave();
    }
};