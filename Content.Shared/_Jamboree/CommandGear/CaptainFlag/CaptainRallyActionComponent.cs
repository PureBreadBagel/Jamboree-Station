// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.Actions;
using Content.Shared.Damage;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;

namespace Content.Shared._Jamboree.CommandGear.CaptainFlag;

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

public sealed partial class CaptainRallyActionEvent : InstantActionEvent
{
}

[RegisterComponent]
public sealed partial class CaptainRallySpeedModifiedComponent : Component
{
    [DataField]
    public float WalkSpeedMultiplier = 1.2f;

    [DataField]
    public float SprintSpeedMultiplier = 1.2f;
}
