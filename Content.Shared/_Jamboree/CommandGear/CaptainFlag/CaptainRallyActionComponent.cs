using Content.Shared.Damage;
using Content.Shared.Whitelist;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;

namespace Content.Shared._Jamboree.CaptainFlag;

[RegisterComponent]
public sealed partial class CaptainRallyActionComponent : Component
{
    /// <summary>
    /// The amount that should be healed when action is activated, defined in YAML.
    /// </summary>
    [DataField]
    public DamageSpecifier Healing = new();

    /// <summary>
    /// The range that the action effects.
    /// </summary>
    [DataField]
    public int Range = 3;

    /// <summary>
    /// The effect that should be played on the user when the action is triggered.
    /// </summary>
    [DataField]
    public EntProtoId RallyEffect = "EffectCaptainRally";

    /// <summary>
    /// The effect that should be played on the person getting buffed when the action is triggered.
    /// </summary>
    [DataField]
    public EntProtoId RalliedEffect = "EffectSpark";

    /// <summary>
    /// The sound that should be played when the rally action is triggered.
    /// </summary>
    [DataField]
    public SoundSpecifier RallySoundPath = new SoundPathSpecifier("/Audio/_Jamboree/Effects/warhorn.ogg");
}
