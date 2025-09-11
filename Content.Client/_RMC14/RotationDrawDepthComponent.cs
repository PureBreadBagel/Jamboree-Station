// SPDX-FileCopyrightText: 2025 JamboreeBot <JamboreeBot@proton.me>
// SPDX-FileCopyrightText: 2025 deltanedas <39013340+deltanedas@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Client._RMC14;

[RegisterComponent]
public sealed partial class RotationDrawDepthComponent : Component
{
    [DataField(customTypeSerializer: typeof(ConstantSerializer<DrawDepth>))]
    public int DefaultDrawDepth;

    [DataField(customTypeSerializer: typeof(ConstantSerializer<DrawDepth>))]
    public int SouthDrawDepth;
}
