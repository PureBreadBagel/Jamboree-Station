// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._Starlight.Actions.Components;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class SpawnOnActionComponent : Component
{
    [DataField, AutoNetworkedField]
    public EntProtoId Action = "Spawn";

    [DataField, AutoNetworkedField]
    public EntityUid? ActionEntity;

    [DataField(required: true)]
    public EntProtoId EntityToSpawn;
}