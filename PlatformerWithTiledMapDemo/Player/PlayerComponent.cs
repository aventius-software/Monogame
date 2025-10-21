using PlatformerWithTiledMapDemo.Shared;

namespace PlatformerWithTiledMapDemo.Player;

internal class PlayerComponent
{
    public FacingState Facing = FacingState.Right;
    public PlayerState State = PlayerState.Idle;
}
