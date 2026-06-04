// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Server._Impstation.StationEvents.Events;

namespace Content.Server._Impstation.StationEvents.Components;

[RegisterComponent, Access(typeof(ThavenMoodUpset))]
public sealed partial class ThavenMoodUpsetRuleComponent : Component;

