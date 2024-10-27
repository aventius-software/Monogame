using Microsoft.Xna.Framework;
using Scellecs.Morpeh;

namespace OutrunStyleTest.Components;

internal struct CameraComponent : IComponent
{    
    public float DistanceToPlayer;// = 500;
    public float DistanceToProjectionPlane;// = 0;
    public Vector3 Position;
}
