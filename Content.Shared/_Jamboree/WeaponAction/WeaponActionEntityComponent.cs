// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.Damage;
using Robust.Shared.Audio;

namespace Content.Shared._Jamboree.WeaponAction;

[RegisterComponent]
public sealed partial class WeaponActionEntityComponent : Component
{
    [DataField]
    public DamageSpecifier Damage = new();

    [DataField]
    public SoundSpecifier HitSound = new SoundPathSpecifier("/Audio/_Goobstation/Wraith/Attack/Flesh_Stab_1.ogg");

    [DataField]
    public bool Knockdown = false;
}
