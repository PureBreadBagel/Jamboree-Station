using Content.Shared.Actions;
using Robust.Shared.Prototypes;

namespace Content.Shared._Jamboree.WeaponAction;

public sealed partial class EventWardenHalberdAction : InstantActionEvent
{
    [DataField]
    public EntProtoId Proto = "HalberdSlash";

    [DataField]
    public float Offset = 1.1f;

    [DataField]
    public float Speed = 0.1f;
}
