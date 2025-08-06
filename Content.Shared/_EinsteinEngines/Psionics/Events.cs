using Robust.Shared.Serialization;
using Content.Shared.Damage;
using Content.Shared.DoAfter;
using Content.Shared._EinsteinEngines.Abilities.Psionics;

namespace Content.Shared._EinsteinEngines.Psionics.Events;

[Serializable, NetSerializable]
public sealed partial class PsionicRegenerationDoAfterEvent : DoAfterEvent
{
    [DataField("startedAt", required: true)]
    public TimeSpan StartedAt;

    public PsionicRegenerationDoAfterEvent(TimeSpan startedAt)
    {
        StartedAt = startedAt;
    }

    public override DoAfterEvent Clone() => this;
}

[Serializable, NetSerializable]
public sealed partial class GlimmerWispDrainDoAfterEvent : SimpleDoAfterEvent { }

[Serializable, NetSerializable]
public sealed partial class HealingWordDoAfterEvent : DoAfterEvent
{
    [DataField(required: true)]
    public TimeSpan StartedAt;

    public HealingWordDoAfterEvent(TimeSpan startedAt)
    {
        StartedAt = startedAt;
    }

    public override DoAfterEvent Clone() => this;
}

[Serializable, NetSerializable]
public sealed partial class PsionicHealOtherDoAfterEvent : DoAfterEvent
{
    [DataField(required: true)]
    public TimeSpan StartedAt;

    [DataField]
    public DamageSpecifier? HealingAmount = default!;

    [DataField]
    public float? RotReduction;

    [DataField]
    public bool DoRevive;

    /// <summary>
    ///     Caster's Amplification that has been modified by the results of a MoodContest.
    /// </summary>
    // public float ModifiedAmplification = default!; # Jamboree - No mood system

    /// <summary>
    ///     Caster's Dampening that has been modified by the results of a MoodContest.
    /// </summary>
    // public float ModifiedDampening = default!; # Jamboree - No mood system

    public PsionicHealOtherDoAfterEvent(TimeSpan startedAt)
    {
        StartedAt = startedAt;
    }

    public override DoAfterEvent Clone() => this;
}

[Serializable, NetSerializable]
public sealed partial class AssayDoAfterEvent : DoAfterEvent
{
    [DataField(required: true)]
    public TimeSpan StartedAt;

    [DataField]
    public int FontSize = 12;

    [DataField]
    public string FontColor = "#8A00C2";

    private AssayDoAfterEvent()
    {
    }

    public AssayDoAfterEvent(TimeSpan startedAt, int fontSize, string fontColor)
    {
        StartedAt = startedAt;
        FontSize = fontSize;
        FontColor = fontColor;
    }

    public override DoAfterEvent Clone() => this;
}
