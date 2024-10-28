using Microsoft.Xna.Framework;
using Scellecs.Morpeh;

namespace OutrunStyleTest.Components;

internal struct CameraComponent : IComponent
{    
    public float DistanceToPlayer;
    public float DistanceToProjectionPlane;
    public Vector3 Position;
}
