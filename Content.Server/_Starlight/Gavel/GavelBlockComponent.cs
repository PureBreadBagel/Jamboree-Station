// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.Audio;

namespace Content.Server.Starlight.Gavel;

[RegisterComponent]
public sealed partial class GavelBlockComponent : Component
{
    [DataField]
    public SoundSpecifier HitSound;

    [DataField]
    public TimeSpan Cooldown = TimeSpan.FromSeconds(0.5);

    [DataField(readOnly: true)]
    public int Counter;

    [DataField(readOnly: true)]
    public int MaxCounter = 60;

    public TimeSpan? PrevSound;
}