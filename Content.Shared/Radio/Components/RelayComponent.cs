using Robust.Shared.GameObjects;

namespace Content.Shared.Radio.Components;

[RegisterComponent]
public sealed partial class RelayComponent : Component
{
    [DataField("isActive")]
    public bool IsActive = true;

    public bool BoostsLongRange => IsActive;
}
