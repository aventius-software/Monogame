using Microsoft.Xna.Framework;
using Scellecs.Morpeh;

namespace OutrunStyleTest.Components;

internal struct CameraComponent : IComponent
{    
    public float DistanceToPlayer;          // Z-distance between camera and player
    public float DistanceToProjectionPlane; // Z-distance between camera and normalized projection plane
    public Vector3 Position;
}
