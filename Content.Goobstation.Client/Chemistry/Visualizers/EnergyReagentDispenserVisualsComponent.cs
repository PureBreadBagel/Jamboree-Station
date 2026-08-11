// SPDX-FileCopyrightText: 2026 PureBreadBagel <purebreadbagel@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Goobstation.Client.Chemistry.Visualizers;

namespace Content.Goobstation.Client.Chemistry.Visualizers;

/// <summary>
/// Defines the sprite states used by an energy reagent dispenser when a beaker is inserted.
/// </summary>
[RegisterComponent]
[Access(typeof(EnergyReagentDispenserVisualizerSystem))]
public sealed partial class EnergyReagentDispenserVisualsComponent : Component
{
    /// <summary>
    /// The sprite state shown when no beaker is inserted.
    /// </summary>
    [DataField]
    public string BaseState = "icon";

    /// <summary>
    /// The sprite state shown when the dispenser has no power.
    /// </summary>
    [DataField]
    public string NoPowerState = "no-power";
}
