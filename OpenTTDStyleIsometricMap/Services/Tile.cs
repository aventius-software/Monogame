using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace OpenTTDStyleIsometricMap.Services;

internal struct Tile
{
    public SlopeType SlopeType;    
    public bool IsHighlighted;
    public RectangleF Bounds;
    public Vector3 WorldPosition;
}