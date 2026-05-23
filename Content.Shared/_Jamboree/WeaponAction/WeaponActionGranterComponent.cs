using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._Jamboree.WeaponAction;

[RegisterComponent, NetworkedComponent]
public sealed partial class WeaponActionGranterComponent : Component
{
    [DataField]
    public EntProtoId WeaponAction = new();

    [DataField]
    public EntityUid? WeaponActionEntity;

    [DataField]
    public SoundSpecifier UseSound = new SoundPathSpecifier("/Audio/_Goobstation/Weapons/MantisBlades/mantis_retract2.ogg");
}
