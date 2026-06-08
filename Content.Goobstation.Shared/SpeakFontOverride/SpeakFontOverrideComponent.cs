// SPDX-FileCopyrightText: 2026 Goob Station Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.GameStates;

namespace Content.Goobstation.Shared.SpeakFontOverride;

[RegisterComponent, NetworkedComponent]
[AutoGenerateComponentState]
public sealed partial class SpeakFontOverrideComponent : Component
{
    [DataField, AutoNetworkedField]
    public bool Enabled = false;

    [DataField]
    public string? FontId;

    [DataField]
    public int? FontSize;

    [DataField]
    public Color? Color;

}
