// SPDX-FileCopyrightText: 2026 Goob Station Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Goobstation.Common.CCVar;
using Content.Server.Station.Components;
using Content.Shared.Maps;
using Robust.Server.GameObjects;
using Robust.Shared.Configuration;
using Robust.Shared.Map.Components;
using Robust.Shared.Timing;

namespace Content.Goobstation.Server.SpaceWhale;

public sealed class SpaceLeviathanDespawnSystem : EntitySystem
{
    [Dependency] private readonly IConfigurationManager _cfg = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;
    [Dependency] private readonly IGameTiming _timing = default!;

    private static readonly TimeSpan CheckInterval = TimeSpan.FromSeconds(10);
    private TimeSpan _nextCheck = TimeSpan.Zero;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<SpaceLeviathanComponent, ComponentStartup>(OnStartup);
    }

    private void OnStartup(Entity<SpaceLeviathanComponent> ent, ref ComponentStartup args)
    {
        _nextCheck = _timing.CurTime + CheckInterval;
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        if (_timing.CurTime < _nextCheck)
            return;

        _nextCheck = _timing.CurTime + CheckInterval;
        CheckLeviathanProximity();
    }

    private void CheckLeviathanProximity()
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

        var leviathanQuery = EntityQueryEnumerator<SpaceLeviathanComponent, TransformComponent>();
        while (leviathanQuery.MoveNext(out var leviathan, out _, out var leviathanXform))
        {
            foreach (var (_, grid, stationXform) in stations)
            {
                if (stationXform.MapUid != leviathanXform.MapUid)
                    continue;

                var leviathanPos = _transform.GetWorldPosition(leviathanXform);
                var stationPos = _transform.GetWorldPosition(stationXform);
                var distance = (leviathanPos - stationPos).Length();

                if (grid.LocalAABB.Size.Length() > 0)
                {
                    var gridRadius = grid.LocalAABB.Size.Length() / 2f;
                    distance = Math.Max(0, distance - gridRadius);
                }

                var despawnDistance = _cfg.GetCVar(GoobCVars.SpaceWhaleSpawnDistance) * 0.5f;

                if (distance <= despawnDistance)
                {
                    QueueDel(leviathan);
                    break;
                }
            }
        }
    }
}

[RegisterComponent, Access(typeof(SpaceLeviathanDespawnSystem))]
public sealed partial class SpaceLeviathanComponent : Component
{
}
