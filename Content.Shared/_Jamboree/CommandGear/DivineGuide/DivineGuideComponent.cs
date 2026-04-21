using Robust.Shared.Audio;
using Robust.Shared.Prototypes;

namespace Content.Shared._Jamboree.DivineGuide;

[RegisterComponent]
public sealed partial class DivineGuideComponent : Component
{
    [DataField]
    public LocId? LearnMessage { get; set; } = "divine-connection-success";

    [DataField]
    public LocId? FailedMessage { get; set; } = "divine-connection-failed";

    [DataField]
    public EntProtoId SpawnedProto = "Ash";

    [DataField]
    public SoundSpecifier? SoundOnUse = new SoundPathSpecifier("/Audio/Effects/fire.ogg", AudioParams.Default.WithVolume(10));
}
