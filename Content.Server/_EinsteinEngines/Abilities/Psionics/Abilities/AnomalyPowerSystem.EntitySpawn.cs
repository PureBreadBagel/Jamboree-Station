// SPDX-FileCopyrightText: 2025 Baine Junk <wym0n@proton.me>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared._EinsteinEngines.Actions.Events;
using Content.Shared.Abilities.Psionics;
using Content.Shared.Anomaly.Effects.Components;
using Content.Shared.Random.Helpers;
using Robust.Shared.Map.Components;
using Robust.Shared.Random;

namespace Content.Server._EinsteinEngines.Abilities.Psionics;

public sealed partial class AnomalyPowerSystem
{
    private const string NoGrid = "entity-anomaly-no-grid";

    /// <summary>
    ///     This function handles emulating the effects of an "Entity Anomaly", using the caster as the "Anomaly",
    ///     while substituting their Psionic casting stats for "Severity and Stability".
    ///     Essentially, spawn entities on random tiles in a radius around the caster.
    /// </summary>
    private void DoEntityAnomalyEffects(EntityUid uid, PsionicComponent component, AnomalyPowerActionEvent args, bool overcharged = false)
    {
        if (args.EntitySpawnEntries is null)
            return;

        if (Transform(uid).GridUid is null)
        {
            _popup.PopupEntity(Loc.GetString(NoGrid), uid, uid);
            return;
        }

        if (overcharged)
            EntitySupercrit(uid, component, args);
        else EntityPulse(uid, component, args);
    }

    private void EntitySupercrit(EntityUid uid, PsionicComponent component, AnomalyPowerActionEvent args)
    {
        foreach (var entry in args.EntitySpawnEntries!)
        {
            if (!entry.Settings.SpawnOnSuperCritical)
                continue;

            SpawnEntities(uid, component, entry);
        }
    }

    private void EntityPulse(EntityUid uid, PsionicComponent component, AnomalyPowerActionEvent args)
    {
        if (args.EntitySpawnEntries is null)
            return;

        foreach (var entry in args.EntitySpawnEntries!)
        {
            if (!entry.Settings.SpawnOnPulse)
                continue;

            SpawnEntities(uid, component, entry);
        }
    }

    private void SpawnEntities(EntityUid uid, PsionicComponent component, EntitySpawnSettingsEntry entry)
    {
        if (!TryComp<MapGridComponent>(Transform(uid).GridUid, out var grid))
            return;

        var tiles = _anomalySystem.GetSpawningPoints(uid,
                        component.CurrentDampening,
                        component.CurrentAmplification,
                        entry.Settings,
                        (float) _glimmerSystem.GlimmerOutput / 1000,
                        component.CurrentAmplification,
                        component.CurrentAmplification);

        if (tiles is null)
            return;

        foreach (var tileref in tiles)
            Spawn(_random.Pick(entry.Spawns), _mapSystem.ToCenterCoordinates(tileref, grid));
    }
}
