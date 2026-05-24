// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.Whitelist;

namespace Content.Shared._Jamboree.CommandGear.DefibrillatorGloves;

[RegisterComponent]
public sealed partial class DefibGlovesComponent : Component
{
    [DataField]
    public EntityWhitelist Whitelist = new();
}
