using Content.Shared.Actions;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._Jamboree.WeaponAction;

public sealed partial class EventWardenHalberdAction : InstantActionEvent
{
    [DataField]
    public EntProtoId Proto = "HalberdSlash";

    [DataField]
    public float Offset = 1.1f;

    [DataField]
    public float Speed = 0.1f;

    [DataField]
    public SoundSpecifier UseSound = new SoundPathSpecifier("/Audio/_Goobstation/Weapons/MantisBlades/mantis_retract2.ogg");
}
