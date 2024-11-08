using Microsoft.Xna.Framework;
using Scellecs.Morpeh;

namespace OutrunStyleTest.Components;

internal struct CameraComponent : IComponent
{
    public float DistanceToPlayer;          // Z-distance between camera and player
    public float DistanceToProjectionPlane; // Z-distance between camera and normalized projection plane
    public float HeightAbovePlayer;         // The height above the player we want the camera to be    
    public Vector3 Position;                // Position of the camera
}
