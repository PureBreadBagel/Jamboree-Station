// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.Actions;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._Jamboree.Overlays;

[RegisterComponent, NetworkedComponent]
public sealed partial class TajaranNightVisionComponent : Component
{
    [DataField]
    public bool IsActive;

    [DataField]
    public float LightRadius = 8;

    [DataField]
    public EntProtoId? ToggleAction { get; set; } = "ToggleTajaranNightVision";

    [ViewVariables]
    public EntityUid? ToggleActionEntity;
}

public sealed partial class ToggleTajaranNightVisionEvent : InstantActionEvent;
