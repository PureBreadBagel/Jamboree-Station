// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.Audio;
using Robust.Shared.Prototypes;

namespace Content.Shared._Jamboree.CommandGear.DivineGuide;

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
