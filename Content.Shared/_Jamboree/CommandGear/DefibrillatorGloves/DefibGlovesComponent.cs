using Content.Shared.Whitelist;

namespace Content.Shared._Jamboree.CommandGear.DefibrillatorGloves;

[RegisterComponent]
public sealed partial class DefibGlovesComponent : Component
{
    [DataField]
    public EntityWhitelist Whitelist = new();
}
