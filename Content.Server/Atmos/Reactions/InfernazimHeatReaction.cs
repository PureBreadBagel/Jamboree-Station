using Content.Server.Atmos.EntitySystems;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Reactions;
using Content.Server.Radiation.Systems;
using JetBrains.Annotations;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Map.Components;
using Robust.Server.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.GameObjects;

namespace Content.Server.Atmos.Reactions;

[UsedImplicitly]
[DataDefinition]
public sealed partial class InfernazimHeatReaction : IGasReactionEffect
{
    public ReactionResult React(
        GasMixture mixture,
        IGasMixtureHolder? holder,
        AtmosphereSystem atmosphereSystem,
        float reactionDelta)
    {
        if (reactionDelta <= 0f)
            return ReactionResult.NoReaction;

        var temperature = mixture.Temperature;
        const float targetTemperature = 7800f;

        if (temperature >= targetTemperature)
            return ReactionResult.NoReaction;

        var heatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true);
        if (heatCapacity <= Atmospherics.MinimumHeatCapacity)
            return ReactionResult.NoReaction;

        var deltaTemp = targetTemperature - temperature;
        var energyChange = deltaTemp * heatCapacity * 0.05f / reactionDelta;

        mixture.Temperature =
            (temperature * heatCapacity + energyChange) / heatCapacity;

        // Apply radioactive effects to entities at this tile
        if (holder is TileAtmosphere tile)
        {
            ApplyRadiationToEntitiesAtTile(tile);
        }

        return ReactionResult.Reacting;
    }

    private static void ApplyRadiationToEntitiesAtTile(TileAtmosphere tile)
    {
        try
        {
            var radiationSystem = IoCManager.Resolve<RadiationSystem>();
            var lookup = IoCManager.Resolve<EntityLookupSystem>();
            var entManager = IoCManager.Resolve<IEntityManager>();
            var mapSystem = IoCManager.Resolve<SharedMapSystem>();

            // Get the transform of the grid to find the map ID
            if (!entManager.TryGetComponent<TransformComponent>(tile.GridIndex, out var gridTransform))
                return;

            var mapId = gridTransform.MapID;

            // Get the MapGridComponent to properly convert tile to world coordinates
            if (!entManager.TryGetComponent<MapGridComponent>(tile.GridIndex, out var mapGridComp))
                return;

            // Convert tile coordinates to world coordinates
            var worldPos = mapSystem.GridTileToWorldPos(tile.GridIndex, mapGridComp, tile.GridIndices);
            var searchBox = new Box2(worldPos.X - 0.5f, worldPos.Y - 0.5f, worldPos.X + 0.5f, worldPos.Y + 0.5f);

            // Get all entities at this tile and irradiate them
            var entities = lookup.GetEntitiesIntersecting(mapId, searchBox);
            const float radsPerSecond = 5f; // Adjust this value to control radiation intensity
            const float radiationTime = 1f; // Duration of radiation in seconds

            foreach (var entity in entities)
            {
                radiationSystem.IrradiateEntity(entity, radsPerSecond, radiationTime);
            }
        }
        catch
        {
            // If systems aren't available, silently fail - reactions shouldn't crash the server
        }
    }
}
