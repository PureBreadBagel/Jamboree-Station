// SPDX-FileCopyrightText: 2025 Baine Junk <wym0n@proton.me>
// SPDX-FileCopyrightText: 2025 JamboreeBot <JamboreeBot@proton.me>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.GameStates;

namespace Content.Shared._DV.Medical.CrewMonitoring;

/// <summary>
/// Marker component that makes a crew monitoring console focus on
/// a station in the same map, instead of the grid the console is on.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class LongRangeCrewMonitorComponent : Component;
