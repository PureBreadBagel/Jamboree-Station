// SPDX-FileCopyrightText: 2026 Jamboree Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Goobstation.Common.CCVar;
using Content.Goobstation.Shared._Jamboree.SpaceWhale;
using Content.Server.Station.Components;
using Content.Shared.Maps;
using Robust.Server.GameObjects;
using Robust.Shared.Configuration;
using Robust.Shared.Map.Components;
using Robust.Shared.Timing;

namespace Content.Goobstation.Server.SpaceWhale;

public sealed class SpaceWhaleDespawnSystem : EntitySystem
{
    [Dependency] private readonly IConfigurationManager _cfg = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;
    [Dependency] private readonly IGameTiming _timing = default!;

    private static readonly TimeSpan CheckInterval = TimeSpan.FromSeconds(10);
    private TimeSpan _nextCheck = TimeSpan.Zero;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<SpaceWhaleComponent, ComponentStartup>(OnStartup);
    }

    private void OnStartup(Entity<SpaceWhaleComponent> ent, ref ComponentStartup args)
    {
        _nextCheck = _timing.CurTime + CheckInterval;
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        if (_timing.CurTime < _nextCheck)
            return;

        _nextCheck = _timing.CurTime + CheckInterval;
        CheckWhaleProximity();
    }

    private void CheckWhaleProximity()
    {
        if (!_cfg.GetCVar(GoobCVars.SpaceWhaleSpawn))
            return;

        var stationQuery = EntityQueryEnumerator<BecomesStationComponent, MapGridComponent>();
        var stations = new List<(EntityUid Uid, MapGridComponent Grid, TransformComponent Xform)>();

        while (stationQuery.MoveNext(out var uid, out _, out var grid))
        {
            var xform = Transform(uid);
            stations.Add((uid, grid, xform));
        }

        if (stations.Count == 0)
            return;

        var whaleQuery = EntityQueryEnumerator<SpaceWhaleComponent, TransformComponent>();
        while (whaleQuery.MoveNext(out var whale, out var comp, out var whaleXform))
        {
            foreach (var (_, grid, stationXform) in stations)
            {
                if (stationXform.MapUid != whaleXform.MapUid)
                    continue;

                var whalePos = _transform.GetWorldPosition(whaleXform);
                var stationPos = _transform.GetWorldPosition(stationXform);
                var distance = (whalePos - stationPos).Length();

                if (grid.LocalAABB.Size.Length() > 0)
                {
                    var gridRadius = grid.LocalAABB.Size.Length() / 2f;
                    distance = Math.Max(0, distance - gridRadius);
                }

                var despawnDistance = comp.DespawnDistance > 0
                    ? comp.DespawnDistance
                    : _cfg.GetCVar(GoobCVars.SpaceWhaleSpawnDistance) * 0.5f;

                if (distance <= despawnDistance)
                {
                    QueueDel(whale);
                    break;
                }
            }
        }
    }
}
