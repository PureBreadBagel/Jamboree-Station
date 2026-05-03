// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.GameStates;

namespace Content.Shared._Impstation.MobStateClothingVisuals;

[RegisterComponent, NetworkedComponent]
public sealed partial class MobStateClothingVisualsComponent : Component
{
    [DataField]
    public string IncapacitatedPrefix = "incapacitated";

    public string? ClothingPrefix = null;
}

public sealed class ClothingMobStateChangedEvent : EntityEventArgs
{
    public ClothingMobStateChangedEvent()
    {

    }
}
