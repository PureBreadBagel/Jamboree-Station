// SPDX-FileCopyrightText: 2025 Baine Junk <wym0n@proton.me>
// SPDX-FileCopyrightText: 2025 JamboreeBot <JamboreeBot@proton.me>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared._EinsteinEngines.Overlays;
using Content.Client.Overlays;
using Content.Shared._EinsteinEngines.Psionics.Components;
using Content.Shared.StatusIcon;
using Content.Shared.StatusIcon.Components;
using Robust.Shared.Prototypes;

/// <summary>
/// EVERYTHING HERE IS A MODIFIED VERSION OF CRIMINAL RECORDS
/// </summary>

namespace Content.Client._EinsteinEngines.Overlays;

public sealed class ShowPsionicsRecordIconsSystem : EquipmentHudSystem<ShowPsionicsRecordIconsComponent>
{
    [Dependency] private readonly IPrototypeManager _prototype = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<PsionicsRecordComponent, GetStatusIconsEvent>(OnGetStatusIconsEvent);
    }

    private void OnGetStatusIconsEvent(EntityUid uid, PsionicsRecordComponent component, ref GetStatusIconsEvent ev)
    {
        if (!IsActive)
            return;

        if (_prototype.TryIndex(component.StatusIcon, out var iconPrototype))
            ev.StatusIcons.Add(iconPrototype);
    }
}
