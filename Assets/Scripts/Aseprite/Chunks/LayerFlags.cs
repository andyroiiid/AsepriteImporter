using System;

namespace Aseprite.Chunks
{
    [Flags]
    public enum LayerFlags : ushort
    {
        Visible = 1,
        Editable = 2,
        LockMovement = 4,
        Background = 8,
        PreferLinkedCels = 16,
        LayerGroupShouldBeDisplayedCollapsed = 32,
        LayerIsAReferenceLayer = 64
    }
}