// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.GameStates;

namespace Content.Shared._Impstation.EquipmentForceFacing;

/// <summary>
/// Only used for griffy suit's family guy death pose. Attempts to force the equippee to face north when their mobstate changes to critical or incapacitated
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class EquipmentForceFacingComponent : Component
{

}
